using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Adlib.Director.SampleApplication
{
    public partial class SelectWorkspaceIdForm : Form
    {
        private List<Adlib.Director.DirectorWSAWrapper.RepositoryInfo> _PreloadedIdList = new List<Adlib.Director.DirectorWSAWrapper.RepositoryInfo>();
        private bool _OkPressed = false;
        private Adlib.Director.DirectorWSAWrapper.RepositoryInfo _SelectedItem = null;

        public SelectWorkspaceIdForm()
        {
            InitializeComponent();
            CreateHeadersForListView();

        }
        public void Init(List<Adlib.Director.DirectorWSAWrapper.RepositoryInfo> list)
        {
            _PreloadedIdList = list;
        }
        private void btnSelectIdOK_Click(object sender, EventArgs e)
        {
            FindSelectedItem();
            if (_SelectedItem == null)
            {
                MessageBox.Show("WorspaceVersionId must be specified.");
                _OkPressed = false;
            }
            else
            {
                _OkPressed = true;
                this.Close();
            } 

        }

        private void radioButtonPreLoadedList_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonTypeGuidValue_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FindSelectedItem()
        {
            ListView.SelectedListViewItemCollection items = listViewRepositoryInfo.SelectedItems;
            if (items != null && items.Count > 0)
            {

                Adlib.Director.DirectorWSAWrapper.RepositoryInfo repo = (Adlib.Director.DirectorWSAWrapper.RepositoryInfo)items[0].Tag;
                if (repo != null)
                {
                    _SelectedItem = repo;
                }
            }
        }
        public bool OkPressed()
        {

            return _OkPressed;
        }

        private void btnSelectIdCancel_Click(object sender, EventArgs e)
        {
            _OkPressed = false;
            this.Close();

        }
        private void CreateHeadersForListView()
        {
            ColumnHeader colHead;
            colHead = new ColumnHeader();
            colHead.Text = "Name";
            colHead.Width = (int)(listViewRepositoryInfo.Width * 0.2);
            listViewRepositoryInfo.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "Transformation WorkspaceId";
            colHead.Width = (int)(listViewRepositoryInfo.Width * 0.4);
            listViewRepositoryInfo.Columns.Add(colHead);

            colHead = new ColumnHeader();
            colHead.Text = "RepositoryId";
            colHead.Width = (int)(listViewRepositoryInfo.Width * 0.4);
            listViewRepositoryInfo.Columns.Add(colHead);
        }

        private void RefreshList()
        {
            Cursor.Current = Cursors.WaitCursor;
            listViewRepositoryInfo.BeginUpdate();
            bool updateStarted = true;
            listViewRepositoryInfo.Items.Clear();

            ListViewItem lvi;
            ListViewItem.ListViewSubItem lvsi;
            try
            {
                if (_PreloadedIdList != null && _PreloadedIdList.Count > 0)
                {
                    foreach (Adlib.Director.DirectorWSAWrapper.RepositoryInfo repo in _PreloadedIdList)
                    {
                        if (repo != null)
                        {
                            lvi = new ListViewItem();
                            lvi.Text = repo.Name;
                            lvi.Tag = repo;

                            lvsi = new ListViewItem.ListViewSubItem();
                            lvsi.Text = repo.TransformationWorkspaceId.ToString();
                            lvi.SubItems.Add(lvsi);

                            lvsi = new ListViewItem.ListViewSubItem();
                            lvsi.Text = repo.RepositoryId.ToString();
                            lvi.SubItems.Add(lvsi);


                            this.listViewRepositoryInfo.Items.Add(lvi);

                        }
                    }
                }

            }
            catch (Exception )
            {
            }
            finally
            {
                if (updateStarted)
                {
                    this.listViewRepositoryInfo.EndUpdate();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void SelectWorkspaceVersionIdForm_Shown(object sender, EventArgs e)
        {
            RefreshList();
        }

        public Adlib.Director.DirectorWSAWrapper.RepositoryInfo GetSelectedItem()
        {
            return _SelectedItem;
        }

        private void listViewRepositoryInfo_DoubleClick(object sender, EventArgs e)
        {

        }

        private void SelectWorkspaceIdForm_Load(object sender, EventArgs e)
        {

        }

    }
}
