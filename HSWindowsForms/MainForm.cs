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

        public MainForm()
        {
            _wc = new WebClient();

            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            
            LoadMyCollection();

            LoadDecks();
            
            LoadSummary();
        }

        private void LoadSummary()
        {
            radGridView1.DataSource = NetDecks.Valuations;
        }

        private void LoadDecks(bool force = false)
        {
            gridViewDecks.DataSource = force ? NetDecks.DownloadDecks() : NetDecks.Decks;
            if (gridViewDecks.SelectedRows.Count > 0)
            {
                gridViewDecks.SelectedRows[0].IsCurrent = false;
            }
        }

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

        private void radGridView1_GroupSummaryEvaluate(object sender, GroupSummaryEvaluationEventArgs e)
        {
            if (e.Parent != gridMyCollection.MasterTemplate && e.SummaryItem.AggregateExpression == null)
            {
                e.FormatString = $"{e.Value} / {e.Group.ItemCount} cards.";
            }

        }

        private readonly RadOffice2007ScreenTipElement _screenTip = new RadOffice2007ScreenTipElement();
        private void radGridView1_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            e.Delay = 1;
            GridDataCellElement cell = e.Item as GridDataCellElement;

            Card card = (Card)cell?.RowInfo.DataBoundItem;

            if (card?.Img == null) return;

            byte[] bytes = _wc.DownloadData(card.Img);
            MemoryStream ms = new MemoryStream(bytes);
            _screenTip.MainTextLabel.Image = Image.FromStream(ms);
            _screenTip.MainTextLabel.Text = "";
            _screenTip.CaptionVisible = false;
            _screenTip.FooterVisible = false;
            _screenTip.MainTextLabel.Margin = new Padding(-5, -35, -15, -20);

            _screenTip.AutoSize = true;
            cell.ScreenTip = _screenTip;
        }

        private void gridViewDecks_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
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
    }
}
