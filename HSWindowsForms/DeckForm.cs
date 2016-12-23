using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HSCore.Model;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class DeckForm : Telerik.WinControls.UI.RadForm
    {
        private Deck deck;

        public DeckForm(Deck _deck)
        {
            InitializeComponent();
            deck = _deck;
            Text = _deck.Name;
            radGridView1.DataSource = deck.Cards.OrderBy(x => x.Key.PlayerClass).ThenBy(x => x.Key.Cost);
            radGridView1.MasterTemplate.ShowRowHeaderColumn = false;

            this.Size = new Size(this.Width, 100 + (deck.Cards.Count * 23));
        }

        private void radGridView1_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            KeyValuePair<Card, int> card = (KeyValuePair<Card, int>) e.CellElement.RowInfo.DataBoundItem;
            switch(card.Value - card.Key.Own)
            {
                case 2:
                    e.CellElement.DrawFill = true;
                    e.CellElement.GradientStyle = GradientStyles.Solid;
                    e.CellElement.BackColor = Color.Red;
                    break;
                case 1:
                    e.CellElement.DrawFill = true;
                    e.CellElement.GradientStyle = GradientStyles.Solid;
                    e.CellElement.BackColor = Color.Yellow;
                    break;
                default:
                    e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                    e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                    e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                    break;
            }
        }

        private void radGridView1_CurrentRowChanging(object sender, CurrentRowChangingEventArgs e)
        {
            e.Cancel = true;
        }

        readonly RadOffice2007ScreenTipElement _screenTip = new RadOffice2007ScreenTipElement();
        private void radGridView1_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            GridDataCellElement cell = e.Item as GridDataCellElement;

            KeyValuePair<Card, int>? card = (KeyValuePair < Card, int>?) cell?.RowInfo.DataBoundItem;

            if (card?.Key.Img == null) return;
            

            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(card.Value.Key.Img);
            MemoryStream ms = new MemoryStream(bytes);
            _screenTip.MainTextLabel.Image = Image.FromStream(ms);
            _screenTip.CaptionLabel.Text = "";
            _screenTip.MainTextLabel.Text = "";
            _screenTip.CaptionLabel.Size = new Size(0, 0);

            _screenTip.EnableCustomSize = true;
            //Optionally set auto-size to false to specify exact size parameters
            _screenTip.AutoSize = false;
            _screenTip.Size = new Size(315, 460);

            cell.ScreenTip = _screenTip;
        }
    }
}
