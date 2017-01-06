using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using HSCore;
using HSCore.Extensions;
using HSCore.Model;
using HSCore.Readers;
using HSWindowsForms.Helper;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class MainForm : RadForm
    {
        private readonly WebClient _wc;
        private readonly RadOffice2007ScreenTipElement _screenTip = new RadOffice2007ScreenTipElement();

        public MainForm()
        {
            _wc = new WebClient();

            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            LoadSummary();
            LoadDecks();
            LoadMyCollection();

            UnselectGrids();
        }
        
        #region Summary
        private void LoadSummary()
        {
            gridCardValuation.DataSource = NetDecks.Valuations;
            gridCardValuation.MasterTemplate.ShowRowHeaderColumn = false;

            foreach (GridViewDataColumn column in gridCardValuation.Columns)
            {
                if (column.DataType == typeof(double))
                {
                    column.FormatString = @"{0:N2}";
                }
            }
        }

        private void gridCardValuation_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            e.Delay = 1;
            GridDataCellElement cell = e.Item as GridDataCellElement;

            Valuation valuation = (Valuation)cell?.RowInfo.DataBoundItem;

            if (valuation?.Card.Img == null) return;

            cell.ScreenTip = GetScreenTip(valuation.Card);
        }

        private void gridCardValuation_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            Valuation valuation = (Valuation)e.CellElement.RowInfo.DataBoundItem;

            if(e.Column.Name == "color" || e.Column.Name == "Card")
            {
                e.CellElement.DrawFill = true;
                e.CellElement.GradientStyle = GradientStyles.Linear;
                e.CellElement.BackColor = Color.White;
                e.CellElement.GradientAngle = 45;
            }
            else
            {
                e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
            }

            switch (e.Column.Name)
            {
                case "color":
                    switch (valuation.Card.Missing)
                    {
                        case 2:
                            e.CellElement.BackColor2 = Color.Red;
                            break;
                        case 1:
                            e.CellElement.BackColor2 = Color.Yellow;
                            break;
                        default:
                            e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                            break;
                    }
                    break;
                case "Card":
                    switch (valuation.Card.Rarity)
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
                            e.CellElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
                            e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
                            break;
                    }
                    break;
            }
        }
        #endregion

        #region Decks
        private void LoadDecks(bool force = false)
        {
            gridViewDecks.DataSource = force ? NetDecks.DownloadDecks() : NetDecks.Decks;
        }

        private void gridViewDecks_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if (gridViewDecks.CurrentRow == null) return;

            DeckForm deckForm = new DeckForm(gridViewDecks.CurrentRow.DataBoundItem as Deck);
            deckForm.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            gridViewDecks.GroupDescriptors.Clear();
            gridViewDecks.FilterDescriptors.Clear();
            rbCValue.ToggleState = ToggleState.Off;
            rbClass.ToggleState = ToggleState.Off;
            rbFree.ToggleState = ToggleState.Off;
            LoadDecks(true);
            UnselectGrids();
        }

        private void radRadioButton_CheckStateChanged(object sender, EventArgs e)
        {
            RadRadioButton radioButton = sender as RadRadioButton;

            RadRadioButton button = radioButton?.Parent.Controls.OfType<RadRadioButton>()
                                                 .FirstOrDefault(n => n.IsChecked);

            if (button == null) return;

            gridViewDecks.GroupDescriptors.Clear();
            gridViewDecks.FilterDescriptors.Clear();
            gridViewDecks.SortDescriptors.Clear();
            FilterDescriptor fDescriptor = new FilterDescriptor();
            GroupDescriptor gDescriptor = new GroupDescriptor();
            SortDescriptor sDescriptor = new SortDescriptor();
            switch (button.Name)
            {
                case "rbFree":
                    fDescriptor.Operator = FilterOperator.IsEqualTo;
                    fDescriptor.Value = 0;
                    fDescriptor.IsFilterEditor = true;
                    gridViewDecks.Columns["MyDust"].FilterDescriptor = fDescriptor;

                    sDescriptor.PropertyName = "Tier";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);
                    break;
                case "rbClass":
                    gDescriptor.GroupNames.Add("Class", ListSortDirection.Ascending);
                    gridViewDecks.GroupDescriptors.Add(gDescriptor);

                    sDescriptor.PropertyName = "MyDust";
                    sDescriptor.Direction = ListSortDirection.Ascending;
                    gridViewDecks.MasterTemplate.SortDescriptors.Add(sDescriptor);

                    fDescriptor.Operator = FilterOperator.IsEqualTo;
                    fDescriptor.Value = DeckType.Standard;
                    fDescriptor.IsFilterEditor = true;
                    gridViewDecks.Columns["DeckType"].FilterDescriptor = fDescriptor;
                    break;
                case "rbCValue":
                    break;
            }

            if (gridViewDecks.SelectedRows.Count > 0)
            {
                gridViewDecks.SelectedRows[0].IsCurrent = false;
            }

            gridViewDecks.MasterTemplate.ExpandAllGroups();
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
            if (e.Parent != gridMyCollection.MasterTemplate && e.SummaryItem.AggregateExpression == null)
            {
                e.FormatString = $"{e.Value} / {e.Group.ItemCount} cards.";
            }

        }

        private void gridMyCollection_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            e.Delay = 1;
            GridDataCellElement cell = e.Item as GridDataCellElement;

            Card card = (Card)cell?.RowInfo.DataBoundItem;

            if (card?.Img == null) return;

            cell.ScreenTip = GetScreenTip(card);
        }
        #endregion

        private void UnselectGrids()
        {
            if (gridCardValuation.SelectedRows.Count > 0)
            {
                gridCardValuation.SelectedRows[0].IsCurrent = false;
            }
            if (gridViewDecks.SelectedRows.Count > 0)
            {
                gridViewDecks.SelectedRows[0].IsCurrent = false;
            }
            if (gridMyCollection.SelectedRows.Count > 0)
            {
                gridMyCollection.SelectedRows[0].IsCurrent = false;
            }
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
    }
}
