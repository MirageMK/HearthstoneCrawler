using System;
using System.Collections.Generic;
using System.Web.Helpers;
using HSCore.Model;
using RestSharp;

namespace HSCore.Readers
{
    public class TempoStormReader : BaseReader
    {
        private const string URL = "http://www.tempostorm.com/";

        private const string END_POINT_SNAPSHOT =
            "api/snapshots/findOne?filter=%7B%22where%22%3A%7B%22slug%22%3A%22{date}%22%2C%22snapshotType%22%3A%22{type}%22%7D%2C%22include%22%3A%5B%7B%22relation%22%3A%22deckTiers%22%2C%22scope%22%3A%7B%22include%22%3A%5B%7B%22relation%22%3A%22deck%22%2C%22scope%22%3A%7B%22fields%22%3A%5B%22id%22%2C%22name%22%2C%22slug%22%2C%22playerClass%22%5D%2C%22include%22%3A%7B%22relation%22%3A%22slugs%22%2C%22scope%22%3A%7B%22fields%22%3A%5B%22linked%22%2C%22slug%22%5D%7D%7D%7D%7D%5D%7D%7D%5D%7D";
        private const string END_POINT_DECK =
            "api/decks/findOne?filter=%7B%22where%22%3A%7B%22slug%22%3A%22{slug}%22%7D%2C%22fields%22%3A%5B%22id%22%2C%22createdDate%22%2C%22name%22%2C%22description%22%2C%22playerClass%22%2C%22heroName%22%2C%22deckType%22%2C%22gameModeType%22%5D%2C%22include%22%3A%5B%7B%22relation%22%3A%22cards%22%2C%22scope%22%3A%7B%22include%22%3A%22card%22%2C%22scope%22%3A%7B%22fields%22%3A%5B%22id%22%2C%22name%22%5D%7D%7D%7D%5D%7D";


        private string GetUrl(string mode)
        {
            string toReturn = "";
            bool found = false;
            DateTime date = DateTime.Now.Date;
            while (!found)
            {
                string formatedDate = date.ToString("yyyy-MM-dd");

                RestClient client = new RestClient(URL);
                RestRequest request = new RestRequest(END_POINT_SNAPSHOT, Method.GET);
                request.AddUrlSegment("date", formatedDate);
                request.AddUrlSegment("type", mode.ToLower());

                IRestResponse response = client.Execute(request);
                string content = response.Content;

                if (content.IndexOf("Error", StringComparison.Ordinal) == -1 && content.IndexOf("\"snapshotType\":\"" + mode.ToLower() + "\"", StringComparison.Ordinal) != -1)
                {
                    found = true;
                    toReturn = response.ResponseUri.OriginalString;
                }
                else
                {
                    date = date.AddDays(-1);
                }
            }
            return toReturn;
        }
        public override List<Deck> GetDecks()
        {
            List<Deck> toReturn = new List<Deck>();
            foreach (DeckType dType in Enum.GetValues(typeof(DeckType)))
            {
                if (dType == DeckType.Undefined) continue;

                string deckTypeDescription = Enums.GetEnumDescription(dType);
                string url = GetUrl(deckTypeDescription);

                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest();
                IRestResponse response = client.Execute(request);
                dynamic snapshot = Json.Decode(response.Content);

                foreach (dynamic deckObj in snapshot.deckTiers)
                {
                    string deckurl = deckObj.deck.slugs[0].slug;

                    Deck deck = GetDeck(deckurl);
                    deck.DeckType = dType;
                    deck.Name = deckObj.name + " - T" + deckObj.tier;
                    deck.Class = deckObj.deck.playerClass;
                    deck.Tier = deckObj.tier;
                    deck.Source = SourceEnum.TempoStorm;
                    toReturn.Add(deck);
                }
            }
            return toReturn;
        }

        private Deck GetDeck(string url)
        {
            Deck toReturn = new Deck();
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest(END_POINT_DECK, Method.GET);
            request.AddUrlSegment("slug", url);

            IRestResponse response = client.Execute(request);
            dynamic deck = Json.Decode(response.Content);

            foreach (dynamic cardObj in deck.cards)
            {
                Card card = MyCollection.Get(cardObj.card.name);
                toReturn.Cards.Add(card, cardObj.cardQuantity);
            }

            return toReturn;
        }
    }
}
