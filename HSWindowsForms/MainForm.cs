using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HSCore;
using HSCore.Model;
using HSWindowsForms.Helper;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class MainForm : RadForm
    {
        private readonly RadOffice2007ScreenTipElement _screenTip = new RadOffice2007ScreenTipElement();
        private readonly WebClient _wc;
        private readonly List<KnownColor> names = Enum.GetValues(typeof(KnownColor)).Cast<KnownColor>().ToList();
        private readonly Random randomGen = new Random();

        public MainForm()
        {
            NetDecks.ProgressChanged += SplashScreen.SetStatus;
            SplashScreen.ShowSplashScreen();
            _wc = new WebClient();

            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            LoadSummary();
            LoadDecks();
            LoadMyCollection();

            UnselectGrids();
            SplashScreen.CloseForm();
            NetDecks.ProgressChanged -= SplashScreen.SetStatus;
        }

        private void UnselectGrids()
        {
            if(gridCardValuation.SelectedRows.Count > 0)
                gridCardValuation.SelectedRows[0].IsCurrent = false;
            if(gridViewDecks.SelectedRows.Count > 0)
                gridViewDecks.SelectedRows[0].IsCurrent = false;
            if(gridMyCollection.SelectedRows.Count > 0)
                gridMyCollection.SelectedRows[0].IsCurrent = false;
        }

        private RadOffice2007ScreenTipElement GetScreenTip(Card card)
        {
            byte[] bytes = _wc.DownloadData(card.Img);
            MemoryStream ms = new MemoryStream(bytes);
            _screenTip.MainTextLabel.Image = Image.FromStream(ms);
            _screenTip.MainTextLabel.Text = "";
            _screenTip.CaptionVisible = false;
            _screenTip.FooterVisible = false;
            _screenTip.MainTextLabel.Margin = new Padding(-5, -35, -15, -20);

            _screenTip.AutoSize = true;

            return _screenTip;
        }

        private void btnWFeed_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(NetDecks.GetWeightedFeed());
        }

        #region Summary

        private void LoadSummary()
        {
            gridCardValuation.DataSource = NetDecks.Valuations.OrderByDescending(x => x.Value);
            gridCardValuation.MasterTemplate.ShowRowHeaderColumn = false;

            foreach(GridViewDataColumn column in gridCardValuation.Columns)
                if(column.DataType == typeof(double))
                    column.FormatString = @"{0:N2}";

            List<Pack> deckValue = (from SetEnum sType in Enum.GetValues(typeof(SetEnum)) select new Pack(sType)).Where(x => x.CanBuy).ToList();
            gridPack.DataSource = deckValue;
            gridPack.Height = 48 + deckValue.Count * 23;
            foreach(GridViewDataColumn column in gridPack.Columns)
                if(column.DataType == typeof(double))
                    column.FormatString = @"{0:N2}";

            StringBuilder sbLabelText = new StringBuilder();
            sbLabelText.AppendLine($@"{MyCollection.Cards.Sum(x => x.Own)}/ {MyCollection.Cards.Sum(x => x.MaxInDeck)}");
            sbLabelText.AppendLine();
            sbLabelText.AppendLine(
                                   $@"Common: {MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT && x.Rarity == "Common").Sum(x => x.Own)}/ {
                                           MyCollection.Cards.Where(
                                                                    x =>
                                                                        x.CardSetEnum ==
                                                                        SetEnum.KotFT &&
                                                                        x.Rarity == "Common").
                                                        Sum(x => x.MaxInDeck)
                                       }");
            sbLabelText.AppendLine(
                                   $@"Rare: {MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT && x.Rarity == "Rare").Sum(x => x.Own)}/ {
                                           MyCollection.Cards.Where(
                                                                    x =>
                                                                        x.CardSetEnum ==
                                                                        SetEnum.KotFT &&
                                                                        x.Rarity == "Rare").
                                                        Sum(x => x.MaxInDeck)
                                       }");
            sbLabelText.AppendLine(
                                   $@"Epic: {MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT && x.Rarity == "Epic").Sum(x => x.Own)}/ {
                                           MyCollection.Cards.Where(
                                                                    x =>
                                                                        x.CardSetEnum ==
                                                                        SetEnum.KotFT &&
                                                                        x.Rarity == "Epic").
                                                        Sum(x => x.MaxInDeck)
                                       }");
            sbLabelText.AppendLine(
                                   $@"Legendary: {MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT && x.Rarity == "Legendary").Sum(x => x.Own)}/ {
                                           MyCollection.Cards.Where(
                                                                    x =>
                                                                        x.CardSetEnum ==
                                                                        SetEnum.KotFT &&
                                                                        x.Rarity ==
                                                                        "Legendary").
                                                        Sum(x => x.MaxInDeck)
                                       }");
            sbLabelText.AppendLine(
                                   $@"All: {MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT).Sum(x => x.Own)}/ {
                                           MyCollection.Cards.Where(x => x.CardSetEnum == SetEnum.KotFT).
                                                        Sum(x => x.MaxInDeck)
                                       }");
            sbLabelText.AppendLine();
            sbLabelText.AppendLine($@"{MyCollection.Cards.Count(x => x.IsLegendary && x.Missing == 0)} legendaries");

            sbLabelText.AppendLine();

            radLabel1.Text = sbLabelText.ToString();
        }

        private void gridCardValuation_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            e.Delay = 1;
            GridDataCellElement cell = e.Item as GridDataCellElement;

            Valuation valuation = (Valuation) cell?.RowInfo.DataBoundItem;

            if(valuation?.Card.Img == null) return;

            cell.ScreenTip = GetScreenTip(valuation.Card);
        }

        private void gridCardValuation_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            Valuation valuation = (Valuation) e.CellElement.RowInfo.DataBoundItem;

            if(e.Column.Name == "color" || e.Column.Name == "Card")
            {
                e.CellElement.DrawFill = true;
                e.CellElement.GradientStyle = GradientStyles.Linear;
                e.CellElement.BackColor = Color.White;
                e.CellElement.GradientAngle = 45;
            }
            else
            {
                e.CellElement.DrawFill = false;
                e.CellElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
            }

            switch(e.Column.Name)
            {
                case "color":
                    switch(valuation.Card.Missing)
                    {
                        case 2:
                            e.CellElement.BackColor2 = Color.Red;
                            break;
                        case 1:
                            e.CellElement.BackColor2 = Color.Yellow;
                            break;
                        default:
                            e.CellElement.DrawFill = false;
                            e.CellElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                            break;
                    }
                    break;
                case "Card":
                    switch(valuation.Card.Rarity)
                    {
                        case "Legendary":
                            e.CellElement.BackColor2 = Color.Orange;
                            break;
                        case "Epic":
                            e.CellElement.BackColor2 = Color.Orchid;
                            break;
                        case "Rare":
                            e.CellElement.BackColor2 = Color.DodgerBlue;
                            break;
                        default:
                            e.CellElement.DrawFill = false;
                            e.CellElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                            break;
                    }
                    break;
            }
        }

        private void gridCardValuation_ViewCellFormatting(object sender, CellFormattingEventArgs e)
        {
            if(e.CellElement is GridFilterCellElement)
                if(e.CellElement.ColumnInfo.Name != "Card")
                    e.CellElement.Visibility = ElementVisibility.Collapsed;
        }

        private void gridCardValuation_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if(gridCardValuation.CurrentRow?.DataBoundItem == null) return;

            CardForm cardForm = new CardForm((gridCardValuation.CurrentRow.DataBoundItem as Valuation).Card);
            cardForm.Show();
        }

        #endregion

        #region Decks

        private void LoadDecks(bool force = false)
        {
            gridViewDecks.DataSource = force ? NetDecks.DownloadDecks() : NetDecks.Decks;

            if(!force) return;

            gridCardValuation.DataSource = NetDecks.Valuations;
            gridPack.DataSource = (from SetEnum sType in Enum.GetValues(typeof(SetEnum)) select new Pack(sType)).Where(x => x.CanBuy).ToList();
        }

        private void gridViewDecks_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if(gridViewDecks.CurrentRow?.DataBoundItem == null) return;

            DeckForm deckForm = new DeckForm(gridViewDecks.CurrentRow.DataBoundItem as Deck);
            deckForm.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            gridViewDecks.GroupDescriptors.Clear();
            gridViewDecks.FilterDescriptors.Clear();
            rbSource.ToggleState = ToggleState.Off;
            rbClass.ToggleState = ToggleState.Off;
            rbFree.ToggleState = ToggleState.Off;
            LoadDecks(true);
            UnselectGrids();
        }

        private void radRadioButton_CheckStateChanged(object sender, EventArgs e)
        {
            RadRadioButton radioButton = sender as RadRadioButton;

            RadRadioButton button = radioButton?.Parent.Controls.OfType<RadRadioButton>().FirstOrDefault(n => n.IsChecked);

            if(button == null) return;

            gridViewDecks.GroupDescriptors.Clear();
            gridViewDecks.FilterDescriptors.Clear();
            gridViewDecks.SortDescriptors.Clear();
            FilterDescriptor fDescriptor = new FilterDescriptor();
            GroupDescriptor gDescriptor = new GroupDescriptor();
            SortDescriptor sDescriptor = new SortDescriptor();
            switch(button.Name)
            {
                case "rbFree":
                    fDescriptor.Operator = FilterOperator.IsEqualTo;
                    fDescriptor.Value = 0;
                    fDescriptor.IsFilterEditor = true;
                    gridViewDecks.Columns["MyDust"].FilterDescriptor = fDescriptor;

                    sDescriptor.PropertyName = "Tier";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    sDescriptor = new SortDescriptor();
                    sDescriptor.PropertyName = "Dust";
                    sDescriptor.Direction = ListSortDirection.Descending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    break;
                case "rbClass":
                    gDescriptor.GroupNames.Add("Class", ListSortDirection.Ascending);
                    gridViewDecks.GroupDescriptors.Add(gDescriptor);

                    sDescriptor.PropertyName = "MyDust";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    sDescriptor = new SortDescriptor();
                    sDescriptor.PropertyName = "Tier";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    sDescriptor = new SortDescriptor();
                    sDescriptor.PropertyName = "Dust";
                    sDescriptor.Direction = ListSortDirection.Descending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);

                    fDescriptor.Operator = FilterOperator.IsEqualTo;
                    fDescriptor.Value = DeckType.Standard;
                    fDescriptor.IsFilterEditor = true;
                    gridViewDecks.Columns["DeckType"].FilterDescriptor = fDescriptor;
                    fDescriptor = new FilterDescriptor();
                    fDescriptor.Operator = FilterOperator.IsLessThanOrEqualTo;
                    fDescriptor.Value = 1;
                    fDescriptor.IsFilterEditor = true;
                    gridViewDecks.Columns["MissingCardNo"].FilterDescriptor = fDescriptor;
                    break;
                case "rbSource":
                    gDescriptor.GroupNames.Add("Source", ListSortDirection.Ascending);
                    gridViewDecks.GroupDescriptors.Add(gDescriptor);

                    sDescriptor.PropertyName = "Tier";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    sDescriptor = new SortDescriptor();
                    sDescriptor.PropertyName = "MyDust";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    break;
            }

            if(gridViewDecks.SelectedRows.Count > 0)
                gridViewDecks.SelectedRows[0].IsCurrent = false;

            gridViewDecks.MasterTemplate.ExpandAllGroups();
        }

        private void gridViewDecks_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            Deck currentDeck = (Deck) e.CellElement.RowInfo.DataBoundItem;
            // Reset the color of the cell.  
            e.CellElement.DrawFill = false;

            if(e.Column.Name != "Name") return;

            List<string> usedColours = new List<string>();
            foreach(GridViewRowInfo t in gridViewDecks.Rows)
            {
                Deck parDeck = (Deck) t.DataBoundItem;

                if(currentDeck.DuplicateIndicatior != parDeck.DuplicateIndicatior || currentDeck.Source == parDeck.Source) continue;

                e.CellElement.DrawFill = true;
                e.CellElement.GradientStyle = GradientStyles.Linear;
                e.CellElement.BackColor2 = Color.White;
                e.CellElement.GradientAngle = 225;

                Color parBackColor = t.Cells["Name"].Style.BackColor;
                if(parBackColor.ToArgb().Equals(Color.FromArgb(105, 105, 105).ToArgb()))
                {
                    KnownColor randomColorName = names.ElementAt(randomGen.Next(names.Count));
                    Color randomColor = Color.FromKnownColor(randomColorName);
                    double a = 1 - (0.299 * randomColor.R + 0.587 * randomColor.G + 0.114 * randomColor.B) / 255;
                    while(a < 0.3 || usedColours.Contains(randomColor.Name))
                    {
                        randomColorName = names.ElementAt(randomGen.Next(names.Count));
                        randomColor = Color.FromKnownColor(randomColorName);
                        a = 1 - (0.299 * randomColor.R + 0.587 * randomColor.G + 0.114 * randomColor.B) / 255;
                    }
                    parBackColor = randomColor;
                    usedColours.Add(randomColor.Name);
                }

                e.CellElement.BackColor = parBackColor;
                e.Row.Cells["Name"].Style.BackColor = parBackColor;
                t.Cells["Name"].Style.BackColor = parBackColor;
            }
        }

        #endregion

        #region My Collection

        private void LoadMyCollection()
        {
            gridMyCollection.Columns["Cost"].SortOrder = RadSortOrder.Ascending;
            gridMyCollection.Columns["Name"].SortOrder = RadSortOrder.Ascending;

            gridMyCollection.MasterTemplate.GroupComparer = new GroupComparer();

            gridMyCollection.DataSource = MyCollection.Cards;

            GridViewSummaryRowItem summaryRowItem = new GridViewSummaryRowItem();
            GridViewSummaryItem summaryItem = new GridViewSummaryItem();
            summaryItem.Name = "Own";
            summaryItem.AggregateExpression = "Sum(IIf(Own > 0, 1, 0))";
            summaryRowItem.Add(summaryItem);
            summaryItem = new GridViewSummaryItem();
            summaryItem.Name = "Missing";
            summaryItem.AggregateExpression = "Sum(IIf(Missing > 0, 1, 0))";
            summaryRowItem.Add(summaryItem);
            summaryItem = new GridViewSummaryItem();
            summaryItem.Name = "Dust";
            summaryItem.AggregateExpression = "Sum(Dust * Missing)";
            summaryRowItem.Add(summaryItem);

            gridMyCollection.SummaryRowsTop.Add(summaryRowItem);
            gridMyCollection.MasterTemplate.ShowTotals = true;
            gridMyCollection.MasterView.SummaryRows[0].PinPosition = PinnedRowPosition.Top;

            gridMyCollection.MasterTemplate.ExpandAllGroups();
        }

        private void gridMyCollection_GroupSummaryEvaluate(object sender, GroupSummaryEvaluationEventArgs e)
        {
            if(e.Parent != gridMyCollection.MasterTemplate && e.SummaryItem.AggregateExpression == null)
                e.FormatString = $"{e.Value} / {e.Group.ItemCount} cards.";
        }

        private void gridMyCollection_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            e.Delay = 1;
            GridDataCellElement cell = e.Item as GridDataCellElement;

            Card card = (Card) cell?.RowInfo.DataBoundItem;

            if(card?.Img == null) return;

            cell.ScreenTip = GetScreenTip(card);
        }

        #endregion
    }
}