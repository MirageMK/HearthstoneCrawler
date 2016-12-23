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
using HSCore.Model;
using HSCore.Readers;
using HSWindowsForms.Helper;
using Newtonsoft.Json;
using Telerik.WinControls;
using Telerik.WinControls.Data;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class MainForm : RadForm
    {
        private readonly IsolatedStorageFile _isf;
        private const string FILENAME = "HSDecks.txt";

        public MainForm()
        {
            _isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
           IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
           typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));

            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            LoadMyCollection();

            LoadDecks();
        }

        private void LoadDecks()
        {
            DateTimeOffset lastChange = _isf.GetLastWriteTime(FILENAME);

            List<Deck> decks = _isf.LoadObject<List<Deck>>(FILENAME);
            if (decks == null || !lastChange.Date.Equals(DateTime.Now.Date))
            {
                decks = new List<Deck>();
                BaseReader reader = new HearthstoneTopDecksBaseReader();
                decks.AddRange(reader.GetDecks());
                reader = new TempoStormBaseReader();
                decks.AddRange(reader.GetDecks());
                reader = new ViciousSyndicateBaseReader();
                decks.AddRange(reader.GetDecks());

                _isf.SaveObject(decks, FILENAME);
            }

            gridViewDecks.DataSource = decks;
            if (gridViewDecks.SelectedRows.Count > 0)
            {
                gridViewDecks.SelectedRows[0].IsCurrent = false;
            }
        }

        private void LoadMyCollection()
        {
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
        }

        private void radGridView1_GroupSummaryEvaluate(object sender, GroupSummaryEvaluationEventArgs e)
        {
            if (e.Parent != gridMyCollection.MasterTemplate && e.SummaryItem.AggregateExpression == null)
            {
                e.FormatString = $"{e.Value} / {e.Group.ItemCount} cards.";
            }

        }


        readonly RadOffice2007ScreenTipElement _screenTip = new RadOffice2007ScreenTipElement();
        private void radGridView1_ScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            GridDataCellElement cell = e.Item as GridDataCellElement;

            if (cell?.RowInfo.Cells["Img"]?.Value == null) return;

            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData(cell.RowInfo.Cells["Img"].Value.ToString());
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

        private void gridViewDecks_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            DeckForm deckForm = new DeckForm(gridViewDecks.CurrentRow.DataBoundItem as Deck);
            deckForm.Show();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.gridViewDecks.GroupDescriptors.Clear();
            this.gridViewDecks.FilterDescriptors.Clear();
            LoadDecks();
        }

        private void radRadioButton_CheckStateChanged(object sender, EventArgs e)
        {
            RadRadioButton radioButton = sender as RadRadioButton;

            RadRadioButton button = radioButton?.Parent.Controls.OfType<RadRadioButton>()
                                                 .FirstOrDefault(n => n.IsChecked);

            if (button == null) return;

            this.gridViewDecks.GroupDescriptors.Clear();
            this.gridViewDecks.FilterDescriptors.Clear();
            FilterDescriptor fDescriptor = new FilterDescriptor();
            GroupDescriptor gDescriptor = new GroupDescriptor();
            switch (button.Name)
            {
                case "rbFree":
                    fDescriptor.Operator = FilterOperator.IsEqualTo;
                    fDescriptor.Value = 0;
                    fDescriptor.IsFilterEditor = true;
                    this.gridViewDecks.Columns["MyDust"].FilterDescriptor = fDescriptor;
                    break;
                case "rbClass":
                    gDescriptor.GroupNames.Add("Class", ListSortDirection.Ascending);
                    this.gridViewDecks.GroupDescriptors.Add(gDescriptor);
                    break;
                case "rbCValue":
                    break;
            }
        }
    }
}
