using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HSCore;
using HSCore.Model;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class CardForm : Telerik.WinControls.UI.RadForm
    {
        private Card card;

        public CardForm(Card _card)
        {
            InitializeComponent();
            card = _card;
            Text = _card.Name;

            Valuation valuation = NetDecks.Valuations.FirstOrDefault(x => x.Card == card);
            if(valuation == null) return;
            {
                List<Deck> unique = valuation.Decks.OrderBy(x => x.Key.Tier).ThenBy(x => x.Key.MyDust).Select(x => x.Key).GroupBy(x => x.DuplicateIndicatior, (k, g) => g.First()).ToList();

                radGridView1.MasterTemplate.ShowRowHeaderColumn = false;
                radGridView1.DataSource = unique;
                radGridView1.Size = new Size(this.Width, 30 + (unique.Count * 23));
                this.Size = new Size(this.Width, 100 + (unique.Count * 23));
                label1.Text = $"{unique.Count} unique of {valuation.Decks.Count}";
            }
        }

        private void radGridView1_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            Deck deck = (Deck) e.CellElement.RowInfo.DataBoundItem;

            if(e.Column.Name == "color")
            {
                e.CellElement.DrawFill = true;
                e.CellElement.GradientStyle = GradientStyles.Linear;
                e.CellElement.BackColor = Color.White;
                e.CellElement.GradientAngle = 45;
            }

            switch(e.Column.Name)
            {
                case "color":
                    foreach(KeyValuePair<Card, int> keyValuePair in deck.Cards)
                    {
                        if(keyValuePair.Key != card) continue;
                        if(card.Own >= keyValuePair.Value)
                        {
                            e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                            break;
                        }
                        if(deck.MyDust == card.Dust)
                        {
                            e.CellElement.BackColor2 = Color.Green;
                            break;
                        }
                        if(keyValuePair.Value - card.Own == 1)
                        {
                            e.CellElement.BackColor2 = Color.Yellow;
                            break;
                        }
                        if(keyValuePair.Value - card.Own == 2)
                        {
                            e.CellElement.BackColor2 = Color.Red;
                            break;
                        }
                    }
                    break;
            }
        }

        private void radGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(radGridView1.CurrentRow?.DataBoundItem == null) return;

            DeckForm deckForm = new DeckForm(radGridView1.CurrentRow.DataBoundItem as Deck);
            deckForm.Show();
        }
    }
}
