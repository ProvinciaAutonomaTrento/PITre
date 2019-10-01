using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adlib.Director.DirectorWSAWrapper;
using System.IO;
using Adlib.Director.DirectorWSAWrapper.JobManagementService;

namespace AdlibJobManager
{

    public class JobManagerFile
    {
        public string IntpuFileName;
        public string OutputFileName;
        public byte[] resultContent;
        public Guid jobID;
    }

    class JobManager
    {

        private bool useMtom;

        public bool UseMtom
        {
            get { return useMtom; }
            set { useMtom = value; }
        }

        private string _UncPathForInputs;

        public string UncPathForInputs
        {
            get { return _UncPathForInputs; }
            set { _UncPathForInputs = value; }
        }
        private string _FolderForOutputDocuments;

        public string FolderForOutputDocuments
        {
            get { return _FolderForOutputDocuments; }
            set { _FolderForOutputDocuments = value; }
        }
        private string _DirectorWSAUrl;

        public string DirectorWSAUrl
        {
            get { return _DirectorWSAUrl; }
            set { _DirectorWSAUrl = value; }
        }
        private string _ClientComponentID;

        public string ClientComponentID
        {
            get { return _ClientComponentID; }
            set { _ClientComponentID = value; }
        }
        private string _RepositoryID;

        public string RepositoryID
        {
            get { return _RepositoryID; }
            set { _RepositoryID = value; }
        }
        private string _UserDefinedID;

        public string UserDefinedID
        {
            get { return _UserDefinedID; }
            set { _UserDefinedID = value; }
        }
        private string _AdminScopeId;

        public string AdminScopeId
        {
            get { return _AdminScopeId; }
            set { _AdminScopeId = value; }
        }
        private bool _UseObjectLock;

        public bool UseObjectLock
        {
            get { return _UseObjectLock; }
            set { _UseObjectLock = value; }
        }
        private bool _EscapeMetadata;

        public bool EscapeMetadata
        {
            get { return _EscapeMetadata; }
            set { _EscapeMetadata = value; }
        }
        private string _workspace;

        public string Workspace
        {
            get { return _workspace; }
            set { _workspace = value; }
        }


        bool _submitDocsSeparately;

        public bool SubmitDocsSeparately
        {
            get { return _submitDocsSeparately; }
            set { _submitDocsSeparately = value; }
        }


        bool _submitReadyJob;

        public bool SubmitReadyJob
        {
            get { return _submitReadyJob; }
            set { _submitReadyJob = value; }
        }


        int _numberOfRepeats = 1;

        public int NumberOfRepeats
        {
            get { return _numberOfRepeats; }
            set { _numberOfRepeats = value; }
        }

        private DirectorWSAWrapper _DirectorWrapper = null;
        private string _AppWorkingPath = null;
        List<Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata> _MetadataList = new List<Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata>();

        public JobManager()
        {
            FileInfo fiAssembly = new FileInfo(System.Reflection.Assembly.GetCallingAssembly().Location);
            _AppWorkingPath = fiAssembly.DirectoryName;

        }

        public void Initialize(string ServiceUrl)
        {
            _DirectorWrapper = new DirectorWSAWrapper();
            //_DirectorWrapper.LogManager().AddLogger(this);
            if (!string.IsNullOrEmpty (ServiceUrl ))
                _DirectorWrapper.DirectorSettings.JobManagementServiceUrl = ServiceUrl;

            string tempFilePath = Path.Combine(_AppWorkingPath, "initsettings.xml");
            string metadataText = File.ReadAllText(tempFilePath);
            ProcessRulesResponse mdList = (ProcessRulesResponse)Serialize.DeserializeObject(metadataText, typeof(ProcessRulesResponse));
            foreach (Metadata met in mdList.MetadataOutputs)
            {
                if (met.Name.Equals("Adlib.Configuration.AdminScopeId"))
                    _AdminScopeId = met.Value;

                if (met.Name.Equals("Adlib.Configuration.MachineId"))
                    Adlib.Common.AdlibConfig.Instance().MachineId = new Guid (met.Value);

                    

            }

            
        }

        private void LoadSettings()
        {
            if (_DirectorWrapper != null && _DirectorWrapper.DirectorSettings != null)
            {
                _UncPathForInputs = _DirectorWrapper.DirectorSettings.CustomUncPathForInputDocs;
                _FolderForOutputDocuments = _DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted;
                _DirectorWSAUrl = _DirectorWrapper.DirectorSettings.JobManagementServiceUrl;
                _ClientComponentID = _DirectorWrapper.DirectorSettings.SubmitterComponentId.ToString();
                _RepositoryID = _DirectorWrapper.DirectorSettings.RepositoryId.ToString();
                _UserDefinedID = _DirectorWrapper.DirectorSettings.UserDefinedId;
                _AdminScopeId = _DirectorWrapper.DirectorSettings.AdminScopeId.ToString();
                _UseObjectLock = _DirectorWrapper.DirectorSettings.UseObjectLock;
                _EscapeMetadata = _DirectorWrapper.DirectorSettings.ExcapeMetadata;
                _workspace = _DirectorWrapper.DirectorSettings.WorkspaceId.ToString();
            }
        }


        private void UpdateSettings()
        {
            try
            {
                if (_DirectorWrapper != null)
                {
                    try
                    {
                        _DirectorWrapper.DirectorSettings.SubmitterComponentId = new Guid(_ClientComponentID);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert SubmitterComponentId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.SubmitterComponentId = Guid.Empty;
                    }
                    _DirectorWrapper.DirectorSettings.CustomUncPathForInputDocs = _UncPathForInputs;
                    _DirectorWrapper.DirectorSettings.JobManagementServiceUrl = _DirectorWSAUrl;
                    _DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted = _FolderForOutputDocuments;
                    try
                    {
                        _DirectorWrapper.DirectorSettings.RepositoryId = new Guid(_RepositoryID);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert RepositoryId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.RepositoryId = Guid.Empty;
                    }

                    _DirectorWrapper.DirectorSettings.UserDefinedId = _UserDefinedID;
                    _DirectorWrapper.DirectorSettings.UseObjectLock = _UseObjectLock;
                    _DirectorWrapper.DirectorSettings.ExcapeMetadata = _EscapeMetadata;

                    try
                    {
                        _DirectorWrapper.DirectorSettings.AdminScopeId = new Guid(_AdminScopeId);
                    }
                    catch (Exception e)
                    {
                        _DirectorWrapper.LogManager().ErrorException("Failed to convert AdminScopeId to Guid.", e);
                        _DirectorWrapper.DirectorSettings.AdminScopeId = Guid.Empty;
                    }
                    try
                    {
                        _DirectorWrapper.DirectorSettings.WorkspaceId = new Guid(_workspace);
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
            }
        }

        public void Unregister(string UserName, string Password, ref string errorMessage)
        {
            string userName = _DirectorWrapper.DirectorSettings.UserName;
            string pwd = _DirectorWrapper.DirectorSettings.UserPassword;
            if (string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.UserName) || string.IsNullOrEmpty(_DirectorWrapper.DirectorSettings.UserPassword))
            {
                userName = UserName;
                pwd = Password;
            }
            bool result = _DirectorWrapper.Unregister(userName, pwd);
            if (result)
            {
                
                errorMessage= "Component is unregistered.";
            }
            else
            {
                errorMessage = "Failed to unregister. See Log for details.";
            }

            LoadSettings();
        }

        public void Register(string UserName, string Password,ref string errorMessage)
        {

            UpdateSettings();
            bool initialized = false;
            bool registered = false;
            bool needToRegister = false;
            string response = "";

            if (Adlib.Common.AdlibConfig.Instance() == null)
            {
                errorMessage = "Failed to load settings. 'Adlib.config file is missing. This file is required for proper initialization.'";
                _DirectorWrapper.LogManager().Error(errorMessage);
                return;
            }
            if (Adlib.Common.AdlibConfig.Instance().MachineId != Guid.Empty)
            {
                try
                {

                    initialized = _DirectorWrapper.InitializeGenericConnector(true, ref response);
                    string tempFilePath = Path.Combine(_AppWorkingPath, "initsettings.xml");
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                    File.WriteAllText(tempFilePath, response);

                    //webBrowserSystemHealth.Navigate(tempFilePath);


                }
                catch { }
                if (initialized == false)//_DirectorWrapper.DirectorSettings.SubmitterComponentId == Guid.Empty)
                {
                    if (_DirectorWrapper.DirectorSettings.SubmitterComponentId == Guid.Empty)
                    {
                        _DirectorWrapper.LogManager().Error("Failed to initialize. No SubmitterComponentId was found.");
                        needToRegister = true;

                    }
                    else
                    {
                        _DirectorWrapper.LogManager().Error("Failed to initialize. Component is registered, but it is not assigned to any Environment.");
                    }

                }
                else
                {
                    if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList == null ||
                        _DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList.Count == 0)
                    {
                        _DirectorWrapper.LogManager().Error("Component is initialized, but no Sources are defined or assigned to an Instruction Set.");
                    }
                    else
                    {
                        if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList[0].TransformationWorkspaceId == Guid.Empty)
                        {
                            _DirectorWrapper.LogManager().Error("Component is initialized, but Source is not assigned to an Instruction Set.");
                        }
                    }
                }
            }
            else
            {
                _DirectorWrapper.LogManager().Error("Failed to Initialize. No MachineId was found.");
                needToRegister = true;
                
            }

            if (needToRegister)
            {

                registered = _DirectorWrapper.Register(UserName, Password);
                if (registered)
                {
                    initialized = _DirectorWrapper.InitializeGenericConnector(true, ref response);
                    if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList == null ||
                        _DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList.Count == 0)
                    {
                        _DirectorWrapper.LogManager().Error("Component is registered and initialized, but no Sources are defined or assigned to an Instruction Set.");
                    }
                    else
                    {
                        if (_DirectorWrapper.DirectorSettings.JobSettingsWorkspaceIdList[0].TransformationWorkspaceId == Guid.Empty)
                        {
                            _DirectorWrapper.LogManager().Error("Component is registered and initialized, but Source is not assigned to an Instruction Set.");
                        }
                        else
                        {
                            errorMessage ="Component is registered.";
                        }
                    }
                }
                else
                {
                    errorMessage = "Failed to register. See Log for details";
                }

            }
            LoadSettings();
        }


        public List<JobManagerFile> SubmitJobs(List<string> fileList, ref string errorMessage)
        {
            List<JobManagerFile> JobIDS = new List<JobManagerFile>();
            int numInputDocs = fileList.Count;
            if (numInputDocs < 1)
            {
                errorMessage ="Can't submit the job: At least one input document must be specified.";
                return null;
            }
            if (useMtom == false)
            {
                if (string.IsNullOrEmpty(UncPathForInputs))
                {
                    errorMessage=string.Format("Can't submit the job: The UNC location to which the input document will be copied must be set in the  field on the Settings tab.");
                    return null;
                }
                else if (!UncPathForInputs.StartsWith("\\"))
                {
                    errorMessage =string.Format("Warning: The UNC location to which the input document will be copied is local path. Engine(s) should have access to this folder in order to process jobs.");
                }

                if (!Directory.Exists(UncPathForInputs))
                {
                    errorMessage =string.Format(" Can't submit the job:  The UNC location to which the input document will be copied doesn't exist. See the field on the Settings tab.");
                    return null;
                }
            }
            try
            {
                List<string> inputDocList = new List<string>();
                foreach (string docName in fileList)
                {
                    inputDocList.Add(docName);
                }
                bool result = SubmitDocuments(inputDocList,
                                               _submitDocsSeparately,
                                               _numberOfRepeats,
                                               useMtom, _submitReadyJob, ref JobIDS);
            }
            catch (Exception ex)
            {
                _DirectorWrapper.LogManager().ErrorException("Failed to submit a new job.", ex);
            }

            return JobIDS;
        }

        private bool SubmitDocuments(List<string> docList, bool submitSeparately, int numOfRepeats, bool transferToRepository, bool useOneCall, ref List<JobManagerFile> JobIDS )
        {
            List<string> listToSubmit = new List<string>();
            bool result = true;
            bool singleJobResult = false;
            //List<DirectorWSAWrapper.DirectorWSA.Metadata> metadataListToSubmit = new List<Adlib.Director.DirectorWSAWrapper.DirectorWSA.Metadata>();
            List<Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata> metadataListToSubmit = new List<Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata>();
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
                            JobIDS.Add(new JobManagerFile { jobID = jobId, IntpuFileName = Path.GetFileName (docPath) });
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
                    JobIDS.Add(new JobManagerFile { jobID = jobId, IntpuFileName = Path.GetFileName (docList.FirstOrDefault())});
                }
                
                TimeSpan ts = DateTime.Now - dtStart;
                _DirectorWrapper.LogManager().Info(string.Format("Job submission took {0} sec", ts.TotalMilliseconds / 1000));
                //System.Threading.Thread.Sleep(100);
            }
            TimeSpan ts1 = DateTime.Now - dtAllStart;
            _DirectorWrapper.LogManager().Info(string.Format("All batch of Job submission took {0} sec. Job count: {1}", ts1.TotalMilliseconds / 1000, jobCount));

            return result;
        }


        public JobDetailResponse[] RefreshList()
        {
            ViewJobs viewJobsFlag = ViewJobs.ShowAll;
            return _DirectorWrapper.GetJobsDetails(viewJobsFlag);
        }

        public void WaitAllReady(List<JobManagerFile> fileList)
        {
            List<Guid> jobIds = (from a in fileList select a.jobID).ToList();
            while (true)
            {
                bool exit= true;
                List<JobStatusResponse> responseLst = _DirectorWrapper.GetJobsStatusInfo(jobIds);
                foreach (JobStatusResponse response in responseLst)
                {
                    if (response.JobStatus != JobStatus.CompletedSuccessful)
                        exit = false;
                }
                if (exit)
                    break;

                System.Threading.Thread.Sleep(100);
            }
        }


        public void DownloadFiles(List <JobManagerFile> fileList)
        {
            foreach (JobManagerFile vo in fileList)
            {
                _DirectorWrapper.DownloadOutputDocuments(vo.jobID);
                string path = Path.Combine (_DirectorWrapper.DirectorSettings.OutputDocumentsPathAfterCompleted ,vo.jobID.ToString().Replace ("-",""));
                string[] files = Directory.GetFiles (path);
                if (files.Length == 1)
                {
                    vo.resultContent = File.ReadAllBytes(files.FirstOrDefault());
                    vo.OutputFileName = Path.GetFileName ( files.FirstOrDefault());
                }
                foreach (string file in files)
                    File.Delete (file);

                Directory.Delete(path,true);
            }
        }

        public void CancelJobs(List<JobManagerFile> fileList)
        {
            List<Guid> jobIds = (from a in fileList select a.jobID).ToList();
            _DirectorWrapper.CancelJobs(jobIds);
        }

        public void ReleaseJobs(List<JobManagerFile> fileList)
        {
            List<Guid> jobIds = (from a in fileList select a.jobID).ToList();
             _DirectorWrapper.ReleaseJobs(jobIds);
        }

    }
}
