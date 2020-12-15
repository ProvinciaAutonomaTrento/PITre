////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Adlib.Director.DirectorWSAWrapper;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using Adlib.Director.DirectorWSAWrapper.JobManagementService;


namespace Adlib.Director.SampleApplication
{
    public partial class MainForm : Form, DirectorWSAWrapper.Logging.ILogger
    {
        delegate void ScreenLogCallback(string message, string messageType);
        private string DefaultTimeFormat = "yyyy/MM/dd HH:mm:ss";
        private const string NO_FILE_SPECIFIED = "no file specified";
        private const string HelpFileURI = @"http://www.adlibsoftware.com/documents/Director/2.1/Adlib_Director_Sample App User_Guide.pdf";

        private int TabSettings = 5;
        private DirectorWSAWrapper.DirectorWSAWrapper _DirectorWrapper = null;
        private string _AppWorkingPath = null;
        //List<DirectorWSAWrapper.DirectorWSA.Metadata> _MetadataList = new List<Adlib.Director.DirectorWSAWrapper.DirectorWSA.Metadata>();
        List<DirectorWSAWrapper.JobManagementService.Metadata> _MetadataList = new List<Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata>();
        MetadataTemplate _MetadataTemplate = null;
        private ListViewColumnSorter lvwColumnSorter;
        private ListViewColumnSorter lvwMetadataColumnSorter;

        public MainForm()
        {
            InitializeComponent();
            CreateHeadersForListView();
            _DirectorWrapper = new DirectorWSAWrapper.DirectorWSAWrapper();
            _DirectorWrapper.LogManager().AddLogger(this);

            lvwColumnSorter = new ListViewColumnSorter();
            lvwMetadataColumnSorter = new ListViewColumnSorter();
            this.listControlJobs.ListViewItemSorter = lvwColumnSorter;
            this.listViewMetadataItems.ListViewItemSorter = lvwMetadataColumnSorter;

            SetDefaultValues();
            this.Text += " " + Utilities.AssemblyVersionFormatted;

            FileInfo fiAssembly = new FileInfo(System.Reflection.Assembly.GetCallingAssembly().Location);
            _AppWorkingPath = fiAssembly.DirectoryName;

            //load metadata template definitions
            StreamReader sr = null;
            try
            {
                string path = Path.Combine(_AppWorkingPath, "MetadataTemplate.xml");
                if (File.Exists(path))
                {
                    sr = File.OpenText(path);
                    string text = sr.ReadToEnd();
                    _MetadataTemplate = (MetadataTemplate)Serialize.DeserializeObject(text, typeof(MetadataTemplate));
                }
            }
            catch
            {

            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (_MetadataTemplate == null)
                {
                    _MetadataTemplate = new MetadataTemplate();
                }
                _MetadataTemplate.InitializeIfEmpty();
            }


        }

        private void SetDefaultValues()
        {
            btnSubmitJobsSubmit.Enabled = false;
            //radioButtonViewJobsShowAll.Checked = true;

            comboBoxTransferType.Items.Add("UNC location, (as per Settings)");
            comboBoxTransferType.Items.Add("Director HTTP File Repository (via MTOM)");
            comboBoxTransferType.SelectedIndex = 0;

            if (_DirectorWrapper != null && _DirectorWrapper.DirectorSettings != null)
            {
                textBoxUncPathForInputs.Text = _DirectorWrapper.DirectorSettings.CustomUncPathForInputDocs;
                textBoxFolderForOutputDocuments.Text = _DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted;
                textBoxDirectorWSAUrl.Text = _DirectorWrapper.DirectorSettings.JobManagementServiceUrl;
                textBoxClientComponentID.Text = _DirectorWrapper.DirectorSettings.SubmitterComponentId.ToString();
                textBoxRepositoryID.Text = _DirectorWrapper.DirectorSettings.RepositoryId.ToString();
                textBoxUserDefinedID.Text = _DirectorWrapper.DirectorSettings.UserDefinedId;
                textBoxAdminScopeId.Text = _DirectorWrapper.DirectorSettings.AdminScopeId.ToString();
                checkBoxUseObjectLock.Checked = _DirectorWrapper.DirectorSettings.UseObjectLock;
                checkBoxEscapeMetadata.Checked = _DirectorWrapper.DirectorSettings.ExcapeMetadata;
                workspaceTextBox.Text = _DirectorWrapper.DirectorSettings.WorkspaceId.ToString();
            }
            btnSubmitJobTicketSubmit.Enabled = false;


        }

        private void LoadSettings()
        {
            if (_DirectorWrapper != null && _DirectorWrapper.DirectorSettings != null)
            {
                textBoxUncPathForInputs.Text = _DirectorWrapper.DirectorSettings.CustomUncPathForInputDocs;
                textBoxFolderForOutputDocuments.Text = _DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted;
                textBoxDirectorWSAUrl.Text = _DirectorWrapper.DirectorSettings.JobManagementServiceUrl;
                textBoxClientComponentID.Text = _DirectorWrapper.DirectorSettings.SubmitterComponentId.ToString();
                textBoxRepositoryID.Text = _DirectorWrapper.DirectorSettings.RepositoryId.ToString();
                textBoxUserDefinedID.Text = _DirectorWrapper.DirectorSettings.UserDefinedId;
                textBoxAdminScopeId.Text = _DirectorWrapper.DirectorSettings.AdminScopeId.ToString();
                checkBoxUseObjectLock.Checked = _DirectorWrapper.DirectorSettings.UseObjectLock;
                checkBoxEscapeMetadata.Checked = _DirectorWrapper.DirectorSettings.ExcapeMetadata;
                workspaceTextBox.Text = _DirectorWrapper.DirectorSettings.WorkspaceId.ToString();
                if (_DirectorWrapper.DirectorSettings.SubmitterComponentId == Guid.Empty)
                {
                    buttonUnregister.Enabled = false;
                }
                else
                {
                    buttonUnregister.Enabled = true;
                }

            }
        }
        private void CreateHeadersForListView()
        {
            ColumnHeader colHead;

            colHead = new ColumnHeader();
            colHead.Text = "Time";
            colHead.Width = (int)(listControlEvents.Width * 0.15); //10% of list 
            this.listControlEvents.Columns.Add(colHead);

            colHead = new ColumnHeader();
            colHead.Text = "Type";
            colHead.Width = (int)(listControlEvents.Width * 0.1); //5%
            this.listControlEvents.Columns.Add(colHead);

            colHead = new ColumnHeader();
            colHead.Text = "Message";
            colHead.Width = (int)(listControlEvents.Width * 0.75); //85%
            this.listControlEvents.Columns.Add(colHead);
            listControlEvents.View = View.Details;

            colHead = new ColumnHeader();
            colHead.Text = "JobID";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "Status";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "ExpressServer";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "TimeStartedUTC";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "TimeCompletedUTC";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "Director duration in sec";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "Express duration in sec";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "OutputFolder";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "OutputFile";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "FileSize";
            listControlJobs.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "JobQueue";
            listControlJobs.Columns.Add(colHead);
            listControlJobs.View = View.Details;


            colHead = new ColumnHeader();
            colHead.Text = "Name";
            colHead.Width = (int)(listViewMetadataItems.Width * 0.20);
            listViewMetadataItems.Columns.Add(colHead);
            colHead = new ColumnHeader();
            colHead.Text = "Value";
            colHead.Width = (int)(listViewMetadataItems.Width * 0.80);
            listViewMetadataItems.Columns.Add(colHead);
            /*
            colHead = new ColumnHeader();
            colHead.Text = "Data";
            colHead.Width = (int)(listViewMetadataItems.Width * 0.47);
            listViewMetadataItems.Columns.Add(colHead);
            listViewMetadataItems.View = View.Details;
            */

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_DirectorWrapper.DirectorSettings != null &&
                !string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.JobManagementServiceUrl) &&
                _DirectorWrapper.DirectorSettings.FirstStart == false)
            {
                try
                {
                    //this DirectorWSA call is called here, because we want to initialize connection to DirectorWSA, load caches and so on.
                    //_DirectorWrapper.RefreshSystemHealth();
                }
                catch { }
            }
            if (_DirectorWrapper.DirectorSettings != null &&
                !string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.JobManagementServiceUrl))
            {
                try
                {
                    //this DirectorWSA call is called here, because we want to initialize connection to DirectorWSA, load caches and so on.
                    //_DirectorWrapper.RefreshSystemHealth();
                    string result = "";
                    _DirectorWrapper.InitializeGenericConnector(ref result);
                }
                catch { }

            }
            /* //Welcome screen code
            WelcomeScreenForm welcomeForm = new WelcomeScreenForm();
            welcomeForm.ShowDialog(this);
            int userSelection = welcomeForm.GetUserSelection();
            if (userSelection  > 0)
            {
                this.tabControl.SelectTab(userSelection);
            }
             */
            if (_DirectorWrapper.DirectorSettings != null)
            {
                if (_DirectorWrapper.DirectorSettings.FirstStart)
                {
                    this.tabControl.SelectTab(TabSettings);
                }
            }
            _DirectorWrapper.DirectorSettings.FirstStart = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateSettings();
            _DirectorWrapper.StopHeartbeating();
            //_DirectorWrapper.SaveSettings();
        }


        private void tabSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnRefreshJobsList_Click(object sender, EventArgs e)
        {
            RefreshJobList();
        }

        private void RefreshJobList()
        {
            bool updateStarted = false;
            ViewJobs viewJobsFlag = ViewJobs.ShowAll;
            if (radioButtonViewJobsCompleted.Checked)
            {
                viewJobsFlag = ViewJobs.Completed;
            }
            else if (radioButtonViewJobsInProgress.Checked)
            {
                viewJobsFlag = ViewJobs.InProgress;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                this.listControlJobs.BeginUpdate();
                updateStarted = true;
                this.listControlJobs.Items.Clear();

                JobDetailResponse[] jobList = _DirectorWrapper.GetJobsDetails(viewJobsFlag);
                ListViewItem lvi;
                ListViewItem.ListViewSubItem lvsi;
                string outputPath = null;
                string outputFile = null;
                long fileSize = 0;

                if (jobList != null && jobList.Length > 0)
                {
                    foreach (JobDetailResponse jobDetail in jobList)
                    {
                        outputPath = null;
                        outputFile = null;
                        fileSize = 0;

                        lvi = new ListViewItem();
                        lvi.Text = jobDetail.JobId.ToString();
                        lvi.Tag = jobDetail.JobId;

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = jobDetail.JobStatus.ToString();
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = "";
                        if (jobDetail.TransformationEngineComponentId != Guid.Empty)
                        {
                            lvsi.Text = jobDetail.TransformationEngineComponentId.ToString() + ":" + jobDetail.TransformationEngineMachineName;
                        }
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        if (jobDetail.JobManagementServiceStartedOnIso8601Utc > DateTime.MinValue.ToUniversalTime())
                        {
                            lvsi.Text = jobDetail.JobManagementServiceStartedOnIso8601Utc.ToLocalTime().ToString(DefaultTimeFormat);
                            lvsi.Tag = jobDetail.JobManagementServiceStartedOnIso8601Utc.ToLocalTime();

                        }
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        if (jobDetail.JobManagementServiceCompletedOnIso8601Utc > DateTime.MinValue.ToUniversalTime())
                        {
                            lvsi.Text = jobDetail.JobManagementServiceCompletedOnIso8601Utc.ToLocalTime().ToString(DefaultTimeFormat);
                            lvsi.Tag = jobDetail.JobManagementServiceCompletedOnIso8601Utc.ToLocalTime();
                        }
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = "";
                        if (jobDetail.JobManagementServiceStartedOnIso8601Utc > DateTime.MinValue.ToUniversalTime() &&
                            jobDetail.JobManagementServiceCompletedOnIso8601Utc > DateTime.MinValue.ToUniversalTime())
                        {
                            TimeSpan ts = jobDetail.JobManagementServiceCompletedOnIso8601Utc - jobDetail.JobManagementServiceStartedOnIso8601Utc;
                            if (ts.TotalSeconds > 0)
                            {
                                lvsi.Text = ts.TotalSeconds.ToString("F2");
                                lvsi.Tag = ts.TotalSeconds;
                            }
                        }
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        if (jobDetail.TransformationEngineConversionTimeSec > 0)
                        {
                            lvsi.Text = jobDetail.TransformationEngineConversionTimeSec.ToString("F2");
                            lvsi.Tag = jobDetail.TransformationEngineConversionTimeSec;
                        }
                        lvi.SubItems.Add(lvsi);

                        //getting output file details here:
                        if (jobDetail.JobDocumentsInfo != null &&
                            jobDetail.JobDocumentsInfo.DocumentOutputs != null &&
                            jobDetail.JobDocumentsInfo.DocumentOutputs.Length > 0)
                        {
                            if (jobDetail.JobDocumentsInfo.DocumentOutputs[0].Destinations != null &&
                                jobDetail.JobDocumentsInfo.DocumentOutputs[0].Destinations.Length > 0)
                            {
                                outputPath = jobDetail.JobDocumentsInfo.DocumentOutputs[0].Destinations[0].Folder;
                            }
                            outputFile = jobDetail.JobDocumentsInfo.DocumentOutputs[0].FileName;
                            fileSize = jobDetail.JobDocumentsInfo.DocumentOutputs[0].FileLength;
                        }

                        //

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = outputPath;
                        lvi.SubItems.Add(lvsi);
                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = outputFile;
                        lvi.SubItems.Add(lvsi);
                        lvsi = new ListViewItem.ListViewSubItem();
                        if (fileSize > 0)
                        {
                            lvsi.Text = fileSize.ToString();
                            lvsi.Tag = fileSize;
                        }
                        lvi.SubItems.Add(lvsi);
                        lvsi = new ListViewItem.ListViewSubItem();
                        if (jobDetail.JobQueuePosition > 0)
                        {
                            lvsi.Text = jobDetail.JobQueuePosition.ToString();
                            lvsi.Tag = jobDetail.JobQueuePosition;
                        }
                        lvi.SubItems.Add(lvsi);


                        this.listControlJobs.Items.Add(lvi);
                    }
                }

            }
            catch (Exception e)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to retrieve the list of jobs.", e);
            }
            finally
            {
                if (updateStarted)
                {
                    this.listControlJobs.EndUpdate();
                }
                Cursor.Current = Cursors.Default;
            }
        }
        public void Info(string message)
        {
            ScreenLog(message, "Info");
        }
        public void Debug(string message)
        {
            //throw new NotImplementedException();
        }
        public void Error(string message)
        {
            ScreenLog(message, "Error");
        }
        public void Warn(string message)
        {
            ScreenLog(message, "Warning");
        }
        public void ErrorException(Exception exception)
        {
            ScreenLog("Exception: " + exception.Message, "ErrorException");
        }
        public void ErrorException(string message, Exception exception)
        {
            ScreenLog(message + " Exception: " + exception.Message, "ErrorException");
        }
        private void ScreenLog(string message, string messageType)
        {
            try
            {
                if (this.listControlEvents.InvokeRequired)
                {
                    ScreenLogCallback d = new ScreenLogCallback(ScreenLog);
                    this.Invoke(d, new object[] { message });
                }
                else
                {
                    try
                    {
                        ListViewItem lvi;
                        ListViewItem.ListViewSubItem lvsi;
                        this.listControlEvents.BeginUpdate();
                        lvi = new ListViewItem();
                        lvi.Text = DateTime.Now.ToString(DefaultTimeFormat);
                        //lvi.ImageIndex = 0;
                        //lvi.Tag = di.FullName;

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = messageType;
                        lvi.SubItems.Add(lvsi);

                        lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Text = message;
                        lvi.SubItems.Add(lvsi);

                        this.listControlEvents.Items.Add(lvi);
                        this.listControlEvents.EndUpdate();
                    }
                    catch (System.Exception err)
                    {
                        MessageBox.Show("Error: " + err.Message);
                    }

                    this.listControlEvents.View = View.Details;
                }
            }
            catch
            {
                //ScreenLog(e.Message, Color.Red);
            }
        }

        private void btnCancelJobs_Click(object sender, EventArgs e)
        {
            List<Guid> jobIds = new List<Guid>();
            ListView.SelectedListViewItemCollection items = listControlJobs.SelectedItems;
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    jobIds.Add((Guid)item.Tag);
                }
            }
            if (jobIds.Count > 0)
            {
                _DirectorWrapper.CancelJobs(jobIds);
                RefreshJobList();
            }
        }

        private void btnReleaseJobs_Click(object sender, EventArgs e)
        {
            List<Guid> jobIds = new List<Guid>();
            ListView.SelectedListViewItemCollection items = listControlJobs.SelectedItems;
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    jobIds.Add((Guid)item.Tag);
                }
            }
            if (jobIds.Count > 0)
            {
                //Only completed jobs will be released!
                _DirectorWrapper.ReleaseJobs(jobIds);
                RefreshJobList();
            }
        }

        private void buttonBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = folderBrowserDialog.RootFolder.ToString();
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxFolderForOutputDocuments.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnOpenUNCPath_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = folderBrowserDialog.RootFolder.ToString();
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxUncPathForInputs.Text = folderBrowserDialog.SelectedPath;
            }
        }


        private void btnSettingsUpdate_Click(object sender, EventArgs e)
        {
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (_DirectorWrapper != null)
                {

                    try
                    {
                        _DirectorWrapper.DirectorSettings.SubmitterComponentId = new Guid(textBoxClientComponentID.Text);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert SubmitterComponentId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.SubmitterComponentId = Guid.Empty;
                    }
                    _DirectorWrapper.DirectorSettings.CustomUncPathForInputDocs = textBoxUncPathForInputs.Text;
                    _DirectorWrapper.DirectorSettings.JobManagementServiceUrl = textBoxDirectorWSAUrl.Text;
                    _DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted = textBoxFolderForOutputDocuments.Text;
                    try
                    {
                        _DirectorWrapper.DirectorSettings.RepositoryId = new Guid(textBoxRepositoryID.Text);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert RepositoryId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.RepositoryId = Guid.Empty;
                    }

                    _DirectorWrapper.DirectorSettings.UserDefinedId = textBoxUserDefinedID.Text;
                    _DirectorWrapper.DirectorSettings.UseObjectLock = checkBoxUseObjectLock.Checked;
                    _DirectorWrapper.DirectorSettings.ExcapeMetadata = checkBoxEscapeMetadata.Checked;

                    try
                    {
                        _DirectorWrapper.DirectorSettings.AdminScopeId = new Guid(textBoxAdminScopeId.Text);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert AdminScopeId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.AdminScopeId = Guid.Empty;
                    }
                    try
                    {
                        _DirectorWrapper.DirectorSettings.WorkspaceId = new Guid(workspaceTextBox.Text);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert WorkspaceVersionId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.WorkspaceId = Guid.Empty;
                    }
                    _DirectorWrapper.SaveSettings();
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to save settings to file.", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void richTextBoxJobTicketTemplate_TextChanged(object sender, EventArgs e)
        {
            if (richTextBoxJobTicketTemplate.Text != null &&
                richTextBoxJobTicketTemplate.Text.Length > 0)
            {
                btnSubmitJobTicketSubmit.Enabled = true;
            }
            else
            {
                btnSubmitJobTicketSubmit.Enabled = false;
            }
        }

        private void btnSubmitJobTicketLoad_Click(object sender, EventArgs e)
        {
            StreamReader sr = null;
            try
            {
                openFileDialog.DefaultExt = "xml";
                openFileDialog.Filter = "Xml files (*.xml)|*.xml";
                openFileDialog.Multiselect = false;
                openFileDialog.InitialDirectory = Path.Combine(_AppWorkingPath, @"SampleDocuments\XML Job Tickets");
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;
                    if (File.Exists(fileName))
                    {
                        sr = File.OpenText(fileName);
                        string text = sr.ReadToEnd();
                        richTextBoxJobTicketTemplate.Text = text;
                    }
                    else
                    {
                        _DirectorWrapper.LogManager().Error("File not found: " + fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to load XML Job Ticket file.", ex);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
            }
        }

        private void btnSubmitJobTicketSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (richTextBoxJobTicketTemplate.Text != null &&
                    richTextBoxJobTicketTemplate.Text.Length > 0)
                {
                    string errorMsg = null;
                    if (Utilities.ValidateXml(richTextBoxJobTicketTemplate.Text, ref errorMsg) == false)
                    {
                        MessageBox.Show("Invalid XML: " + errorMsg);
                        return;
                    }
                    bool result = _DirectorWrapper.SubmitJobTicket(richTextBoxJobTicketTemplate.Text);
                    if (result)
                    {
                        MessageBox.Show("Job Ticket was submitted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to submit Job Ticket. See Log tab or log file for details.");
                    }

                }
                else
                {
                    MessageBox.Show("Enter Job Ticket data first.");
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }


        private void tabSettings_Leave(object sender, EventArgs e)
        {
            UpdateSettings();
        }


        private void btnSubmitJobAddDocuments_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.InitialDirectory = Path.Combine(_AppWorkingPath, @"SampleDocuments\Docs");
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string[] fileNames = openFileDialog.FileNames;
                    if (fileNames != null && fileNames.Length > 0)
                    {
                        listBoxInputDocuments.BeginUpdate();
                        foreach (string fileName in fileNames)
                        {
                            if (File.Exists(fileName))
                            {
                                listBoxInputDocuments.Items.Add(fileName);
                            }
                        }
                        listBoxInputDocuments.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to select job document inputs.", ex);
            }
            if (listBoxInputDocuments.Items.Count > 0)
            {
                btnSubmitJobsSubmit.Enabled = true;
            }
            else
            {
                btnSubmitJobsSubmit.Enabled = false;
            }
        }

        private void btnSubmitJobClearAllDocuments_Click(object sender, EventArgs e)
        {
            listBoxInputDocuments.BeginUpdate();
            listBoxInputDocuments.Items.Clear();
            listBoxInputDocuments.EndUpdate();
            btnSubmitJobsSubmit.Enabled = false;
        }

        private void btnSubmitJobAddFolder_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog.SelectedPath = Path.Combine(_AppWorkingPath, @"SampleDocuments\Docs");
                if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string path = folderBrowserDialog.SelectedPath;
                    List<string> fileList = new List<string>();
                    Utilities.GetFilesList(ref fileList, path, null, true);
                    if (fileList.Count > 0)
                    {
                        listBoxInputDocuments.BeginUpdate();
                        foreach (string fileName in fileList)
                        {
                            if (File.Exists(fileName))
                            {
                                listBoxInputDocuments.Items.Add(fileName);
                            }
                        }
                        listBoxInputDocuments.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to select job document inputs.", ex);
            }
            if (listBoxInputDocuments.Items.Count > 0)
            {
                btnSubmitJobsSubmit.Enabled = true;
            }
            else
            {
                btnSubmitJobsSubmit.Enabled = false;
            }


        }

        private void btnSubmitJobsSubmit_Click(object sender, EventArgs e)
        {
            bool useMtom = false;
            if (comboBoxTransferType.SelectedIndex == 1)
                useMtom = true;
            int numInputDocs = listBoxInputDocuments.Items.Count;
            if (numInputDocs < 1)
            {
                MessageBox.Show("Can't submit the job: At least one input document must be specified.");
                return;
            }
            if (useMtom == false)
            {
                if (string.IsNullOrEmpty(textBoxUncPathForInputs.Text))
                {
                    MessageBox.Show(string.Format("Can't submit the job: The UNC location to which the input document will be copied must be set in the '{0}' field on the Settings tab.", labelUncPathForInputs.Text));
                    return;
                }
                else if (!textBoxUncPathForInputs.Text.StartsWith("\\"))
                {
                    DialogResult dr = MessageBox.Show(string.Format("Warning: The UNC location to which the input document will be copied is local path. Engine(s) should have access to this folder in order to process jobs. Do you want to continue?", labelUncPathForInputs.Text, MessageBoxButtons.OKCancel));
                    if (dr != DialogResult.OK)
                    {
                        return;
                    }
                }

                if (!Directory.Exists(textBoxUncPathForInputs.Text))
                {
                    MessageBox.Show(string.Format(" Can't submit the job:  The UNC location to which the input document will be copied doesn't exist. See the '{0}' field on the Settings tab.", labelUncPathForInputs.Text));
                    return;
                }
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                List<string> inputDocList = new List<string>();
                foreach (string docName in listBoxInputDocuments.Items)
                {
                    inputDocList.Add(docName);
                }
                bool result = SubmitDocuments(inputDocList,
                                               checkBoxSubmitDocsSeparately.Checked,
                                               (int)numericUpDownNumberOfRepeats.Value,
                                               useMtom, checkBoxSubmitReadyJob.Checked);

                if (result)
                {
                    MessageBox.Show("The document(s) were submitted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to submit one or more documents. See Log tab or log file for details.");
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to submit a new job.", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }
        private bool SubmitDocuments(List<string> docList, bool submitSeparately, int numOfRepeats, bool transferToRepository, bool useOneCall)
        {
            List<string> listToSubmit = new List<string>();
            bool result = true;
            bool singleJobResult = false;
            //List<DirectorWSAWrapper.DirectorWSA.Metadata> metadataListToSubmit = new List<Adlib.Director.DirectorWSAWrapper.DirectorWSA.Metadata>();
            List<DirectorWSAWrapper.JobManagementService.Metadata> metadataListToSubmit = new List<DirectorWSAWrapper.JobManagementService.Metadata>();
            //DirectorWSAWrapper.JobManagementService.Metadata

            if (numOfRepeats < 1)
            {
                numOfRepeats = 1;
            }
            DateTime dtAllStart = DateTime.Now;
            int jobCount = 0;
            for (int i = 0; i < numOfRepeats; i++)
            {
                metadataListToSubmit.Clear();
                if (_MetadataList != null && _MetadataList.Count > 0)
                {
                    //need to make a copy, because this list can be modified before submitting to DirectorWSA.
                    metadataListToSubmit = _MetadataList.GetRange(0, _MetadataList.Count);
                }

                DateTime dtStart = DateTime.Now;
                if (submitSeparately)
                {

                    foreach (string docPath in docList)
                    {
                        listToSubmit.Clear();
                        if (File.Exists(docPath))
                        {
                            listToSubmit.Add(docPath);
                            singleJobResult = false;
                            Guid jobId = Guid.Empty;

                            if (!useOneCall)
                            {
                                jobId = _DirectorWrapper.SubmitDocuments(listToSubmit, metadataListToSubmit, transferToRepository);
                            }
                            else
                            {
                                jobId = _DirectorWrapper.SubmitJobSimple(listToSubmit, metadataListToSubmit, Guid.NewGuid());
                            }
                            jobCount++;
                            if (jobId != Guid.Empty)
                            {
                                singleJobResult = true;
                            }
                            result &= singleJobResult;
                            //System.Threading.Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    singleJobResult = false;
                    Guid jobId = Guid.Empty;

                    if (!useOneCall)
                    {
                        jobId = _DirectorWrapper.SubmitDocuments(docList, metadataListToSubmit, transferToRepository);
                    }
                    else
                    {
                        jobId = _DirectorWrapper.SubmitJobSimple(docList, metadataListToSubmit, Guid.NewGuid());
                    }
                    jobCount++;
                    if (jobId != Guid.Empty)
                    {
                        singleJobResult = true;
                    }
                    result &= singleJobResult;
                }

                TimeSpan ts = DateTime.Now - dtStart;
                _DirectorWrapper.LogManager().Info(string.Format("Job submission took {0} sec", ts.TotalMilliseconds / 1000));
                //System.Threading.Thread.Sleep(100);
            }
            TimeSpan ts1 = DateTime.Now - dtAllStart;
            _DirectorWrapper.LogManager().Info(string.Format("All batch of Job submission took {0} sec. Job count: {1}", ts1.TotalMilliseconds / 1000, jobCount));

            return result;
        }

        private void btnDownloadOtuputs_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxFolderForOutputDocuments.Text))
            {
                MessageBox.Show(string.Format("The root folder for output documents must be set first. See '{0}' on the Settings tab.", labelFolderForOutputs.Text));
                return;
            }
            if (!Directory.Exists(textBoxFolderForOutputDocuments.Text))
            {
                MessageBox.Show(string.Format("The root folder for output documents is not accessible. Please validate the value '{0}' on the Settings tab.", labelFolderForOutputs.Text));
                return;
            }
            List<Guid> jobIds = new List<Guid>();
            ListView.SelectedListViewItemCollection items = listControlJobs.SelectedItems;
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    jobIds.Add((Guid)item.Tag);
                }
            }
            if (jobIds.Count > 0)
            {
                foreach (Guid jobId in jobIds)
                {
                    _DirectorWrapper.DownloadOutputDocuments(jobId);
                }
            }
        }

		// Probably should move this elsewhere, but it's only used here.
		class TraceLogMessageSink : JobManagementServiceClientMessageInspector.IMessageSink
		{
			public string logPath;

			public TraceLogMessageSink(string filename, bool preserveHistory)
			{
				logPath = filename;
				if (!preserveHistory)
				{
					ClearHistory(preserveHistory);
				}
			}

			private void ClearHistory(bool preserveHistory)
			{
				if (!String.IsNullOrEmpty(logPath))
				{
					if (File.Exists(logPath) && !preserveHistory)
					{
						File.Delete(logPath);
					}
				}
			}

			public bool WriteMessage(string message)
			{
				if (!String.IsNullOrEmpty(logPath))
				{
					using (StreamWriter file = new StreamWriter(logPath, true, Encoding.Unicode))
					{
						file.WriteLine();
						file.WriteLine();	// These two writelines are from the original implementation - just trying to make this consistent.
						file.WriteLine(message);
					}
				}
				return true;
			}
		}

		private static string StartTrace = "Start Trace";
		private static string StopTrace = "Stop Trace";
        private void btnSoapTraceStartStop_Click(object sender, EventArgs e)
        {
            btnSoapTraceStartStop.Enabled = false;

			string logPath = GetTraceLogFileName();

            try
            {
				if (btnSoapTraceStartStop.Text == StartTrace)
                {
					_DirectorWrapper.DirectorSettings.JobManagementServiceEndpointBehavior.ClientMessageInspector.MessageSink = new TraceLogMessageSink(logPath, preserveHistory: false);

					btnSoapTraceStartStop.Text = StopTrace;
                }
                else
                {
					_DirectorWrapper.DirectorSettings.JobManagementServiceEndpointBehavior.ClientMessageInspector.MessageSink = null;

					btnSoapTraceStartStop.Text = StartTrace;
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException(string.Format("Failed to save the trace output to the file '{0}'.", logPath), ex);
				_DirectorWrapper.DirectorSettings.JobManagementServiceEndpointBehavior.ClientMessageInspector.MessageSink = null;
			}
            
            btnSoapTraceStartStop.Enabled = true;
        }

		private string GetTraceLogFileName()
		{
			return Path.Combine(_AppWorkingPath, "SampleApplicationSoapMessageTrace.log");
		}

        private void btnSoapTraceClear_Click(object sender, EventArgs e)
        {
			string logPath = GetTraceLogFileName();

			if (File.Exists(logPath))
            {
				File.Delete(logPath);
            }
        }

        private void btnSoapTraceSaveAsFile_Click(object sender, EventArgs e)
        {
            btnSoapTraceSaveAsFile.Enabled = false;

            string fileName = NO_FILE_SPECIFIED;
            try
            {
                saveFileDialog.DefaultExt = "log";
                saveFileDialog.Filter = "Log files (*.log)|*.log";
                openFileDialog.InitialDirectory = _AppWorkingPath;
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
					string _SoapTracerFilePath = GetTraceLogFileName();
                    if (!string.IsNullOrEmpty(_SoapTracerFilePath) && File.Exists(_SoapTracerFilePath))
                    {
                        File.Copy(_SoapTracerFilePath, fileName, overwrite:true);
                    }
                    else
                    {
                        _DirectorWrapper.LogManager().Error(string.Format("Failed to save the SOAP trace output to the file '{0}'.", fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException(string.Format("Failed to save the SOAP trace output to the file '{0}'.", fileName), ex);
            }

            btnSoapTraceSaveAsFile.Enabled = true;
        }

        private void tabPageSoapTrace_Enter(object sender, EventArgs e)
        {
            // Do nothing
        }

        private void btnSubmitJobAddJobTicketTemplate_Click(object sender, EventArgs e)
        {

        }

        private void btnSubmitJobLoadMetadata_Click(object sender, EventArgs e)
        {
            StreamReader sr = null;
            string fileName = "";
            try
            {
                openFileDialog.DefaultExt = "xml";
                openFileDialog.Filter = "Xml files (*.xml)|*.xml";
                openFileDialog.InitialDirectory = Path.Combine(_AppWorkingPath, "Metadata");
                openFileDialog.Multiselect = false;
                openFileDialog.FileName = "";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    if (File.Exists(fileName))
                    {
                        sr = File.OpenText(fileName);
                        string metadataText = sr.ReadToEnd();

                        UserMetadataList mdList = (UserMetadataList)Serialize.DeserializeObject(metadataText, typeof(UserMetadataList));
                        if (mdList != null && mdList.MetadataList != null && mdList.MetadataList.Count > 0)
                        {
                            foreach (DirectorWSAWrapper.JobManagementService.Metadata xMetadata in mdList.MetadataList)
                            {
                                AddOrUpdateMetadataItem(xMetadata);
                            }
                            /*
                            XmlDocument xMetaDataXML = new XmlDocument();
                            xMetaDataXML.LoadXml(metadataText);

                            XmlNodeList xMetaDataNodes = xMetaDataXML.DocumentElement.GetElementsByTagName("metadata");
                            DirectorWSAWrapper.DirectorWSA.Metadata xMetadata = null;
                            for (int i = 0; i < xMetaDataNodes.Count; i++)
                            {
                                XmlNode xCurrentMetaDataNode = xMetaDataNodes[i];
                                xMetadata = new DirectorWSAWrapper.DirectorWSA.Metadata();
                                xMetadata.Name = xCurrentMetaDataNode.Attributes["name"].Value;
                                xMetadata.Value = xCurrentMetaDataNode.Attributes["value"].Value;
                                if (xCurrentMetaDataNode.Attributes["Data"] != null)
                                {
                                    xMetadata.Data = xCurrentMetaDataNode.Attributes["Data"].Value;
                                }
                                AddOrUpdateMetadataItem(xMetadata);
                            }
                            */
                            RefreshMetadataList();
                        }
                    }
                    else
                    {
                        _DirectorWrapper.LogManager().Error(string.Format("File not found: {0}", fileName));
                    }
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException(string.Format("Failed to load metadata from the file '{0}'", fileName), ex);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }

        private void RefreshMetadataList()
        {
            Cursor.Current = Cursors.WaitCursor;
            listViewMetadataItems.BeginUpdate();
            bool updateStarted = true;
            listViewMetadataItems.Items.Clear();

            ListViewItem lvi;
            ListViewItem.ListViewSubItem lvsi;
            try
            {
                if (_MetadataList != null && _MetadataList.Count > 0)
                {
                    foreach (DirectorWSAWrapper.JobManagementService.Metadata metadata in _MetadataList)
                    {
                        if (metadata != null)
                        {
                            lvi = new ListViewItem();
                            lvi.Text = metadata.Name;
                            lvi.Tag = metadata;

                            lvsi = new ListViewItem.ListViewSubItem();
                            lvsi.Text = metadata.Value;
                            lvi.SubItems.Add(lvsi);

                            /*
                            lvsi = new ListViewItem.ListViewSubItem();
                            lvsi.Text = metadata.Value;
                            lvi.SubItems.Add(lvsi);
                            */
                            this.listViewMetadataItems.Items.Add(lvi);

                        }
                    }
                }

            }
            catch (Exception e)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to refresh the list of metadata.", e);
            }
            finally
            {
                if (updateStarted)
                {
                    this.listViewMetadataItems.EndUpdate();
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnDeleteMetadataItems_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listViewMetadataItems.SelectedItems;
            if (items != null && items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    _MetadataList.Remove((DirectorWSAWrapper.JobManagementService.Metadata)item.Tag);
                }
                RefreshMetadataList();
            }
        }
        private void AddOrUpdateMetadataItem(DirectorWSAWrapper.JobManagementService.Metadata newMetadata)
        {
            AddOrUpdateMetadataItem(newMetadata, null);
        }
        private void AddOrUpdateMetadataItem(DirectorWSAWrapper.JobManagementService.Metadata newMetadata, DirectorWSAWrapper.JobManagementService.Metadata oldMetadataToUpdate)
        {
            bool found = false;
            if (newMetadata != null)
            {
                foreach (DirectorWSAWrapper.JobManagementService.Metadata existingMetadata in _MetadataList)
                {
                    if (existingMetadata != null && !string.IsNullOrEmpty(newMetadata.Name) &&
                        !string.IsNullOrEmpty(existingMetadata.Name) &&
                        string.Compare(newMetadata.Name, existingMetadata.Name, true) == 0)
                    {
                        found = true;
                        existingMetadata.Value = newMetadata.Value;
                        //existingMetadata.Data = newMetadata.Data;
                        break;
                    }
                    if (oldMetadataToUpdate != null)
                    {
                        if (existingMetadata != null && !string.IsNullOrEmpty(oldMetadataToUpdate.Name) &&
                            !string.IsNullOrEmpty(existingMetadata.Name) &&
                            string.Compare(oldMetadataToUpdate.Name, existingMetadata.Name, true) == 0)
                        {
                            found = true;
                            existingMetadata.Name = newMetadata.Name;
                            existingMetadata.Value = newMetadata.Value;
                            //existingMetadata.Data = newMetadata.Data;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    _MetadataList.Add(newMetadata);
                }
            }
        }

        private void listViewMetadataItems_DoubleClick(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = listViewMetadataItems.SelectedItems;
            object selectedTag = null;
            if (items != null && items.Count > 0 && items[0].Tag != null)
            {
                DirectorWSAWrapper.JobManagementService.Metadata oldMetadata = (DirectorWSAWrapper.JobManagementService.Metadata)items[0].Tag;
                selectedTag = (DirectorWSAWrapper.JobManagementService.Metadata)items[0].Tag;
                /*
                EditMetadata editMetadataForm = new EditMetadata();
                editMetadataForm.Init(ref metadata);
                editMetadataForm.ShowDialog(this);
                if (editMetadataForm.OkPressed())
                {
                    RefreshMetadataList();
                }*/
                AddNewMetadataForm editMetadataForm = new AddNewMetadataForm();
                editMetadataForm.Init(_MetadataTemplate);
                if (oldMetadata != null)
                {
                    editMetadataForm.SetDataForEdit(oldMetadata.Name, oldMetadata.Value, oldMetadata.Value);

                }
                editMetadataForm.ShowDialog(this);
                if (editMetadataForm.OkPressed())
                {
                    DirectorWSAWrapper.JobManagementService.Metadata newMetadata = editMetadataForm.GetAddedMetadata();
                    if (newMetadata != null && newMetadata.Name != null)
                    {
                        AddOrUpdateMetadataItem(newMetadata, oldMetadata);
                        RefreshMetadataList();
                    }
                }
                foreach (ListViewItem item in listViewMetadataItems.Items)
                {
                    if (selectedTag != null && selectedTag == item.Tag)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

        }

        private void btnSubmitJobAddMetadata_Click(object sender, EventArgs e)
        {
            DirectorWSAWrapper.JobManagementService.Metadata metadata = null;
            AddNewMetadataForm addMetadataForm = new AddNewMetadataForm();
            addMetadataForm.Init(_MetadataTemplate);
            addMetadataForm.ShowDialog(this);
            if (addMetadataForm.OkPressed())
            {
                metadata = addMetadataForm.GetAddedMetadata();
                if (metadata != null && metadata.Name != null)
                {
                    AddOrUpdateMetadataItem(metadata);
                    RefreshMetadataList();
                }
            }
        }

        private void btnSaveMetadata_Click(object sender, EventArgs e)
        {
            if (_MetadataList == null || _MetadataList.Count == 0)
            {
                MessageBox.Show(string.Format("There are no metadata items defined.  Before attempting to save, please use the '{0}' button to add metadata items to the list, or the '{1}' button to load metadata from a previously saved file.", btnSubmitJobAddMetadata.Text, btnSubmitJobLoadMetadata.Text));
                return;
            }
            string fileName = NO_FILE_SPECIFIED;
            try
            {
                saveFileDialog.DefaultExt = "xml";
                saveFileDialog.Filter = "Xml files (*.xml)|*.xml";
                saveFileDialog.InitialDirectory = Path.Combine(_AppWorkingPath, "Metadata");
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    fileName = saveFileDialog.FileName;
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    UserMetadataList customList = new UserMetadataList();
                    customList.MetadataList = _MetadataList;
                    string output = Serialize.SerializeObject(customList, typeof(UserMetadataList));

                    System.IO.FileStream fs = new System.IO.FileStream(fileName, FileMode.OpenOrCreate | System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter streamWriter = new StreamWriter(fs);
                    streamWriter.Write(output);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException(string.Format("Failed to save the SOAP trace output to the file '{0}'.", fileName), ex);
            }

        }

        private void btnRefreshSystemHealth_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSettings();
                Cursor.Current = Cursors.WaitCursor;
                string xResult = _DirectorWrapper.RefreshSystemHealth();
                string tempFilePath = Path.Combine(_AppWorkingPath, "systemhealth.xml");
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
                File.WriteAllText(tempFilePath, xResult);

                webBrowserSystemHealth.Navigate(tempFilePath);
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to retrieve the System Health status.", ex);
                MessageBox.Show("An exception occured that prevented the system health status data from being retrieved.  Please see the Log tab for more information.");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void groupBoxSubmitDocsStep2_Enter(object sender, EventArgs e)
        {

        }

        private void listControlJobs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listControlJobs.Sort();

        }

        private void listViewMetadataItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwMetadataColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwMetadataColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwMetadataColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwMetadataColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwMetadataColumnSorter.SortColumn = e.Column;
                lvwMetadataColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listViewMetadataItems.Sort();

        }

        private void radioButtonViewJobsShowAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonViewJobsShowAll.Checked == true)
            {
                RefreshJobList();
            }
        }

        private void radioButtonViewJobsCompleted_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonViewJobsCompleted.Checked == true)
            {
                RefreshJobList();
            }
        }

        private void radioButtonViewJobsInProgress_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonViewJobsInProgress.Checked == true)
            {
                RefreshJobList();
            }

        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            string logFilePath = _DirectorWrapper.LogManager().GetLogFilePath();
            if (logFilePath != null && logFilePath.Length > 0 && File.Exists(logFilePath))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "notepad.exe";
                psi.Arguments = logFilePath;
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show(string.Format("The log file '{0}' does not exist or is not accessible", logFilePath));
            }
        }

        private void btnSeeWSDL_Click(object sender, EventArgs e)
        {
            UpdateSettings();
            if (textBoxDirectorWSAUrl.Text == null ||
                textBoxDirectorWSAUrl.Text.Length == 0)
            {
                MessageBox.Show(string.Format("Please enter a valid URL in the '{0}' textbox.", labelDirectorWSAUrl.Text));
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "explorer.exe";
                psi.Arguments = textBoxDirectorWSAUrl.Text + "?wsdl";
                Process.Start(psi);

            }
        }

        private void btnLoadSettings_Click(object sender, EventArgs e)
        {
            UpdateSettings();
            bool initialized = false;
            bool registered = false;
            bool needToRegister = false;
            string response = "";

            if (Adlib.Common.AdlibConfig.Instance() == null)
            {
                string error = "Failed to load settings. 'Adlib.config file is missing. This file is required for proper initialization.'";
                _DirectorWrapper.LogManager().Error(error);
                MessageBox.Show(error);


                return;
            }
            if (Adlib.Common.AdlibConfig.Instance().MachineId != Guid.Empty)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    initialized = _DirectorWrapper.InitializeGenericConnector(true, ref response);
                    string tempFilePath = Path.Combine(_AppWorkingPath, "initsettings.xml");
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                    File.WriteAllText(tempFilePath, response);

                    webBrowserSystemHealth.Navigate(tempFilePath);


                }
                catch { }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
                if (initialized == false)//_DirectorWrapper.DirectorSettings.SubmitterComponentId == Guid.Empty)
                {
                    if (_DirectorWrapper.DirectorSettings.SubmitterComponentId == Guid.Empty)
                    {
                        _DirectorWrapper.LogManager().Error("Failed to initialize. No SubmitterComponentId was found.");
                        DialogResult dr = MessageBox.Show("Failed to initialize. No SubmitterComponentId was found. Would you like to register?", "Initialization failure", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.OK)
                        {
                            needToRegister = true;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        _DirectorWrapper.LogManager().Error("Failed to initialize. Component is registered, but it is not assigned to any Environment.");
                        DialogResult dr = MessageBox.Show("Failed to initialize. Component is registered, but it is not assigned to any Environment.", "Initialization failure");
                    }

                }
                else
                {
                    if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList == null ||
                        _DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList.Count == 0)
                    {
                        _DirectorWrapper.LogManager().Error("Component is initialized, but no Sources are defined or assigned to an Instruction Set.");
                        MessageBox.Show("Component is initialized, but no Sources are defined or assigned to an Instruction Set.", "Initialization warning");
                    }
                    else
                    {
                        if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList[0].TransformationWorkspaceId == Guid.Empty)
                        {
                            _DirectorWrapper.LogManager().Error("Component is initialized, but Source is not assigned to an Instruction Set.");
                            MessageBox.Show("Component is initialized, but Source is not assigned to an Instruction Set.", "Initialization warning");
                        }
                    }
                }
            }
            else
            {
                _DirectorWrapper.LogManager().Error("Failed to Initialize. No MachineId was found.");
                DialogResult dr = MessageBox.Show("Failed to initialize. No MachineId was found. Would you like to register?", "Initialization failure", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    needToRegister = true;
                }
                else
                {
                }
            }

            if (needToRegister)
            {
                UserDetailsForm userForm = new UserDetailsForm();
                userForm.ShowDialog();
                if (userForm.IsOkPressed())
                {
                    registered = _DirectorWrapper.Register(userForm.UserName(), userForm.Password(), userForm.FriendlyName);
                    if (registered)
                    {
                        initialized = _DirectorWrapper.InitializeGenericConnector(true, ref response);
                        if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList == null ||
                            _DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList.Count == 0)
                        {
                            _DirectorWrapper.LogManager().Error("Component is registered and initialized, but no Sources are defined or assigned to an Instruction Set.");
                            MessageBox.Show("Component is initialized, but no Sources are defined or assigned to an Instruction Set.", "Initialization warning");
                        }
                        else
                        {
                            if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList[0].TransformationWorkspaceId == Guid.Empty)
                            {
                                _DirectorWrapper.LogManager().Error("Component is registered and initialized, but Source is not assigned to an Instruction Set.");
                                MessageBox.Show("Component is initialized, but Source is not assigned to an Instruction Set.", "Initialization warning");
                            }
                            else
                            {
                                MessageBox.Show("Component is registered.", "Registration");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to register. See Log for details.", "Registration failure");
                    }
                }
                else
                {
                    MessageBox.Show("Can't register component without proper credentials.", "Registration failure");
                }
            }
            LoadSettings();

        }
        private void btnClearLogList_Click(object sender, EventArgs e)
        {
            listControlEvents.Items.Clear();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            /*
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            psi.Arguments = HelpFileURI;
            Process.Start(psi);*/

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "notepad.exe";
            psi.Arguments = System.IO.Path.Combine(_AppWorkingPath, "readme.txt");
            Process.Start(psi);

        }


        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 5)
            {
                LoadSettings();
            }
            else
            {
                UpdateSettings();
            }
        }

        private void btnSelectWorkspaceVersionId_Click(object sender, EventArgs e)
        {
            SelectWorkspaceIdForm selectWorkspaceVersionIdForm = new SelectWorkspaceIdForm();
            selectWorkspaceVersionIdForm.Init(_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList);
            selectWorkspaceVersionIdForm.ShowDialog(this);
            if (selectWorkspaceVersionIdForm.OkPressed())
            {
                Adlib.Director.DirectorWSAWrapper.RepositoryInfo repo = selectWorkspaceVersionIdForm.GetSelectedItem();
                if (repo != null)
                {
                    Guid workspaceVersionId = repo.TransformationWorkspaceId;
                    Guid repositoryId = repo.RepositoryId;
                    if (_DirectorWrapper != null &&
                        _DirectorWrapper.DirectorSettings != null)
                    {
                        _DirectorWrapper.DirectorSettings.WorkspaceId = workspaceVersionId;
                        _DirectorWrapper.DirectorSettings.RepositoryId = repositoryId;
                    }
                    workspaceTextBox.Text = workspaceVersionId.ToString();
                    textBoxRepositoryID.Text = repositoryId.ToString();
                }

            }
        }

        private void btnComponentStatus_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateSettings();
                Cursor.Current = Cursors.WaitCursor;
                string xResult = _DirectorWrapper.GetComponentStatus();
                string tempFilePath = Path.Combine(_AppWorkingPath, "componentstatus.xml");
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
                File.WriteAllText(tempFilePath, xResult);

                webBrowserSystemHealth.Navigate(tempFilePath);
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to retrieve the Component status.", ex);
                MessageBox.Show("An exception occured that prevented the component status data from being retrieved.  Please see the Log tab for more information.");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void buttonUnregister_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Would you like to unregister?", "Unregistration", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                string userName = _DirectorWrapper.DirectorSettings.UserName;
                string pwd = _DirectorWrapper.DirectorSettings.UserPassword;
                if (string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.UserName) || string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.UserPassword))
                {
                    UserDetailsForm userForm = new UserDetailsForm();
                    userForm.ShowFriendlyName = false;
                    userForm.ShowDialog();
                    if (userForm.IsOkPressed())
                    {
                        userName = userForm.UserName();
                        pwd = userForm.Password();
                    }
                }
                bool result = _DirectorWrapper.Unregister(userName, pwd);
                if (result)
                {
                    MessageBox.Show("Component is unregistered.", "Unregistration");
                }
                else
                {
                    MessageBox.Show("Failed to unregister. See Log for details.", "Unregistration failure");
                }

                LoadSettings();
            }

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnViewTraffic_Click(object sender, EventArgs e)
        {
            btnViewTraffic.Enabled = false;

            string trafficLogPath = Path.Combine(_AppWorkingPath, "SampleApplicationSoapMessageTrace.log");

            if (File.Exists(trafficLogPath))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "notepad.exe";
                psi.Arguments = trafficLogPath;
                Process.Start(psi);
            }
            else
            {
                MessageBox.Show("There is no traffic to view at the moment. Click \"Start Trace\" to begin monitoring traffic between the service and client.");
            }

            btnViewTraffic.Enabled = true;
        }

        private void comboBoxTransferType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTransferType.SelectedIndex == 1)
            {
                checkBoxSubmitReadyJob.Enabled = false;
                checkBoxSubmitReadyJob.Checked = false;
            }
            else
            {
                checkBoxSubmitReadyJob.Enabled = true;
            }
        }



    }
    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int _ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder _OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer _ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            _ColumnToSort = 0;
            _OrderOfSort = SortOrder.None;
            _ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            object itemX = listviewX.SubItems[_ColumnToSort].Text;
            object itemY = listviewY.SubItems[_ColumnToSort].Text;
            if (listviewX.SubItems[_ColumnToSort].Tag != null ||
                listviewY.SubItems[_ColumnToSort].Tag != null)
            {
                itemX = listviewX.SubItems[_ColumnToSort].Tag;
                itemY = listviewY.SubItems[_ColumnToSort].Tag;
            }
            compareResult = _ObjectCompare.Compare(itemX, itemY);
            // Calculate correct return value based on object comparison
            if (_OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (_OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                _ColumnToSort = value;
            }
            get
            {
                return _ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                _OrderOfSort = value;
            }
            get
            {
                return _OrderOfSort;
            }
        }

    }


}
