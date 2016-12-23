using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows.Forms;
using HSCore;
using HSCore.Model;
using HSCore.Readers;
using HSWindowsForms.Helper;
using Newtonsoft.Json;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace HSWindowsForms
{
    public partial class MainForm : RadForm
    {
        IsolatedStorageFile isf = null;

        public MainForm()
        {
            isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
           IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain,
           typeof(System.Security.Policy.Url), typeof(System.Security.Policy.Url));
            IsolatedStorageFileStream isfs = new IsolatedStorageFileStream
                    ("decks.txt", FileMode.Create, isf);

            var temp1 = isf.GetFileNames("*");
            var temp2 = isf.GetDirectoryNames("*");

            var temp3 = isf.GetCreationTime("decks.txt");
            var temp4 = isf.GetLastWriteTime("decks.txt");
            var temp5 = isf.GetLastAccessTime("decks.txt");

            InitializeComponent();
            WindowState = FormWindowState.Maximized;

            List<Deck> decks = new List<Deck>();
            BaseReader reader = new HearthstoneTopDecksBaseReader();
            decks.AddRange(reader.GetDecks());
            reader = new TempoStormBaseReader();
            decks.AddRange(reader.GetDecks());
            reader = new ViciousSyndicateBaseReader();
            decks.AddRange(reader.GetDecks());

            string json = JsonConvert.SerializeObject(decks);

            gridViewDecks.DataSource = decks;

            LoadMyCollection();
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
    }
}
