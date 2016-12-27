using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using HSCore.Model;
using HtmlAgilityPack;

namespace HSCore.Readers
{
    public class HearthstoneTopDecksBaseReader : BaseReader
    {
        private const string URL = "http://www.hearthstonetopdecks.com/";

        private string GetUrl()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(URL);

            return doc.DocumentNode.SelectNodes("//*[@id=\"page\"]/div[5]/div/div/div[2]/div/div/div[3]/a")[0].Attributes["href"].Value;
        }

        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(GetUrl());

            foreach (HtmlNode deckLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'deck-box-header')]/a[@href]"))
            {
                string deckUrl = deckLink.GetAttributeValue("href", string.Empty);
                if (deckUrl == "") continue;
                Deck deck = GetDeck(deckUrl);
                deck.Name = deckLink.InnerText.Replace("&#8217;", "'");
                deck.Source = SourceEnum.HearthstoneTopDecks;
                toReturn.Add(deck);
            }

            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            toReturn.Tier = 5;
            IEnumerable<HtmlNode> scripts = doc.DocumentNode.Descendants("script");

            foreach (var scriptNode in scripts)
            {
                string script = scriptNode.InnerHtml;
                if (script == "") continue;
                try
                {
                    string winRateString = script.Split(new string[] { "data: [" }, StringSplitOptions.None)[1].Split(new string[] { ", ]" }, StringSplitOptions.None)[0];
                    string[] winRateArray = winRateString.Split(',');

                    double total = winRateArray.Aggregate(0.0, (current, winRate) => current + int.Parse(winRate));

                    double deckWinPercent = total / (winRateArray.Length * 100);

                    if (deckWinPercent >= 0.55)
                    {
                        toReturn.Tier = 1;
                    }
                    else if (deckWinPercent >= 0.50)
                    {
                        toReturn.Tier = 2;
                    }
                    else if (deckWinPercent >= 0.45)
                    {
                        toReturn.Tier = 3;
                    }
                    else if (deckWinPercent >= 0.40)
                    {
                        toReturn.Tier = 4;
                    }
                    else
                    {
                        toReturn.Tier = 5;
                    }

                }
                catch(Exception)
                {
                    // ignored
                }
            }

            toReturn.Class = doc.DocumentNode.SelectNodes("//*[contains(@class,'deck-info')]/a")[0].InnerText;

            foreach (HtmlNode cardLink in doc.DocumentNode.SelectNodes("//*[contains(@class,'card-frame')]"))
            {
                string cardName = cardLink.SelectSingleNode("*[contains(@class,'card-name')]").InnerText.Replace("&#8217;", "'"); ;
                string cardCount = cardLink.SelectSingleNode("*[contains(@class,'card-count')]").InnerText;

                Card card = MyCollection.Get(cardName);
                toReturn.Cards.Add(card, int.Parse(cardCount));
            }

            return toReturn;
        }
    }
}
