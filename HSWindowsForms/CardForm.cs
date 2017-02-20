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

namespace HSWindowsForms
{
    public partial class CardForm : Telerik.WinControls.UI.RadForm
    {
        public CardForm()
        {
            InitializeComponent();
        }

        public CardForm(Card _card)
        {
            InitializeComponent();
            Card card = _card;
            Text = _card.Name;

            Valuation valuation = NetDecks.Valuations.FirstOrDefault(x => x.Card == card);
            if (valuation == null) return;

            {
                radGridView1.DataSource = valuation.Decks.OrderBy(x => x.Key.Tier).ThenBy(x => x.Key.MyDust).Select(x=>x.Key).ToList();
                this.Size = new Size(this.Width, 70 + (valuation.Decks.Count * 25));
            }
        }
    }
}
