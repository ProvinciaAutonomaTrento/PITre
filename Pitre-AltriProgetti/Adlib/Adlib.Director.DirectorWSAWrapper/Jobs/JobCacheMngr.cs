using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Adlib.Director.DirectorWSAWrapper.JobManagementService;
using System.Xml.Serialization;

namespace Adlib.Director.DirectorWSAWrapper.Jobs
{
    [Serializable]
    public class JobRequest
    {
        private string _inputFile;
        public bool SubmitAsOneCall = false;
        public Guid JobId = Guid.Empty;
        private List<Metadata> _MetadataList = null;
        private List<MetadataFile> _MetadataFileNameList = null;
        private string _NickName = null;
        private bool _Enable = true;
        private List<string> _inputFileList; 



        public JobRequest Clone()
        {
            JobRequest newJR = new JobRequest();
            newJR._Enable = this._Enable;
            newJR._MetadataFileNameList = new List<MetadataFile>(this._MetadataFileNameList);
            newJR._MetadataList = new List<Metadata>(this._MetadataList);
            newJR.SubmitAsOneCall = this.SubmitAsOneCall;
            newJR.NickName = this.NickName;
            newJR.InputFile = this.InputFile;
            return newJR;
        }
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        public string NickName
        {
            get { return _NickName; }
            set { _NickName = value; }
        }

        [XmlArray("MetadataList")]
        [XmlArrayItem("Metadata")]
        public List<Metadata> MetadataList
        {
            get 
            {
                if (_MetadataList == null)
                {
                    _MetadataList = new List<Metadata>();
                }
                return _MetadataList; 
            }
            set { _MetadataList = value; }
        }

        [XmlArray("MetadataFileNameList")]
        [XmlArrayItem("MetadataFileName")]
        public List<MetadataFile> MetadataFileNameList
        {
            get
            {
                if (_MetadataFileNameList == null)
                {
                    _MetadataFileNameList = new List<MetadataFile>();
                }
                return _MetadataFileNameList;
            }
            set { _MetadataFileNameList = value; }
        }

        [XmlArray("InputFileList")]
        [XmlArrayItem("InputFile")]
        public List<string> InputFileList
        {
            get
            {
                if (_inputFileList == null)
                {
                    _inputFileList = new List<string>();
                }

                return _inputFileList;
            }
            set { _inputFileList = value; }
        }

        public string InputFile
        {
            get { return _inputFile; }
            set
            {
                _inputFile = value;
               InputFileList.Add(_inputFile);
            }
        }
    }

    [Serializable]
    public class MetadataFile
    {
        public string FileName;
        public string MetadataName;
    }
    [Serializable]
    public class AutomatedTestRequest
    {
        protected List<JobRequest> _JobRequestList = null;
        public string BaseInputFilesPath;
        public string BaseSnippetsPath;

        [XmlArray("JobRequestList")]
        [XmlArrayItem("JobRequest")]
        public List<JobRequest> JobRequestList
        {
            get
            {
                if (_JobRequestList == null)
                {
                    _JobRequestList = new List<JobRequest>();
                }
                return _JobRequestList;
            }
            set { _JobRequestList = value; }
        }

        public void PrepareFilesPath()
        {
            if (!string.IsNullOrEmpty(BaseInputFilesPath) || !string.IsNullOrEmpty(BaseSnippetsPath))
            {
                if (JobRequestList != null && JobRequestList.Count > 0)
                {
                    foreach (JobRequest jr in JobRequestList)
                    {
                        if (jr != null)
                        {
                            if (jr.MetadataFileNameList != null && jr.MetadataFileNameList.Count > 0 && !string.IsNullOrEmpty(BaseSnippetsPath))
                            {
                                foreach (MetadataFile mf in jr.MetadataFileNameList)
                                {
                                    if (mf != null && !string.IsNullOrEmpty(mf.FileName))
                                    {
                                        mf.FileName = System.IO.Path.Combine(BaseSnippetsPath, mf.FileName);
                                    }
                                }
                            }
                            //here is new inplementation with input list
                            if (jr.InputFileList != null && jr.InputFileList.Count > 0 && !string.IsNullOrEmpty(BaseSnippetsPath))
                            {
                                for (int i = 0; i < jr.InputFileList.Count; i++ )
                                {
                                    if (!string.IsNullOrEmpty(BaseInputFilesPath) && !string.IsNullOrEmpty(jr.InputFileList[i]))
                                    {
                                        jr.InputFileList[i] = System.IO.Path.Combine(BaseInputFilesPath, jr.InputFileList[i]);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public class JobCacheMngr
    {

        private static JobCacheMngr _Instance = null;
        private static object _SyncInstanceObject = new object();
        private List<JobSubmitter> _SubmiterList = new List<JobSubmitter>();
        private List<JobFinalizer> _FinalizerList = new List<JobFinalizer>();
        private JobWatcher _JobWatcher = new JobWatcher();
        private List<JobRequest> _JobCache = new List<JobRequest>();
        private System.Collections.Generic.Dictionary<Guid, JobRequest> _JobWatchCache = new Dictionary<Guid,JobRequest>();
        private List<JobRequest> _JobReleaseCache = new List<JobRequest>();


        private JobCacheMngr()
        {

        }

        public static JobCacheMngr Instance()
        {
            lock (_SyncInstanceObject)
            {
                if (_Instance == null)
                {
                    _Instance = new JobCacheMngr();
                }
            }
            return _Instance;
        }

        public void StartWatcherAndSubmitters(int numSubmitters, int numReleasers, Logging.LogMngr logMngr, bool deleteFoldersOnRelease, int maxNumFilesInSubfolder, int sleepBetweenJobs)
        {
            if (numSubmitters <= 0)
            {
                numSubmitters = 1;
            }
            if (numReleasers < 0)
            {
                numReleasers = 0;
            }
            for (int i = 0; i < numSubmitters; i++)
            {
                JobSubmitter js = new JobSubmitter();
                js.Initialize(logMngr);
                js.MaxNumberFilesInSubfolder = maxNumFilesInSubfolder;
                js.ThreadTag = "SubmitterThread_" + i;
                js.SleepBetweenJobs = sleepBetweenJobs;
                _SubmiterList.Add(js);
                ThreadPool.QueueUserWorkItem(js.Execute);
            }

            
            _JobWatcher.Initialize(logMngr);
            if (numReleasers > 0)
            {
                ThreadPool.QueueUserWorkItem(_JobWatcher.Execute);

                for (int i = 0; i < numReleasers; i++)
                {
                    JobFinalizer jf = new JobFinalizer();
                    jf.Initialize(logMngr);
                    jf.DeleteFolderOnRelease = deleteFoldersOnRelease;
                    jf.ThreadTag = "FinalizerThread_" + i;
                    _FinalizerList.Add(jf);
                    ThreadPool.QueueUserWorkItem(jf.Execute);
                }
            }

        }

        public void StopWatch()
        {
            _JobWatcher.NeedStop();
            foreach (JobSubmitter js in _SubmiterList)
            {
                js.NeedStop();
            }
            foreach (JobFinalizer jf in _FinalizerList)
            {
                jf.NeedStop();
            }

        }
        public void AddToJobCache(JobRequest request)
        {
            if (request != null)
            {
                lock (_SyncInstanceObject)
                {
                    _JobCache.Add(request);
                }
            }
        }
        public JobRequest GetNextJobFromCache()
        {
            JobRequest job = null;
            lock (_SyncInstanceObject)
            {
                if (_JobCache.Count > 0)
                {
                    job = _JobCache[0];
                    _JobCache.RemoveAt(0);
                }
            }
            return job;
        }
        public int GetJobCacheCount()
        {
            int count = 0;
            lock (_SyncInstanceObject)
            {
                count = _JobCache.Count();
            }
            return count;
        }
        public void AddToJobReleaseCache(JobRequest request)
        {
            if (request != null)
            {
                lock (_SyncInstanceObject)
                {
                    _JobReleaseCache.Add(request);
                }
            }
        }
        public JobRequest GetNextJobFromReleaseCache()
        {
            JobRequest job = null;
            lock (_SyncInstanceObject)
            {
                if (_JobReleaseCache.Count > 0)
                {
                    job = _JobReleaseCache[0];
                    _JobReleaseCache.RemoveAt(0);
                }
            }
            return job;
        }
        public int GetJobReleaseCacheCount()
        {
            int count = 0;
            lock (_SyncInstanceObject)
            {
                count = _JobReleaseCache.Count();
            }
            return count;
        }

        public void AddToJobWatchCache(JobRequest request)
        {
            if (request != null)
            {
                lock (_SyncInstanceObject)
                {
                    _JobWatchCache.Add(request.JobId, request);
                }
            }
        }
        public JobRequest GetJobByJobIdFromWatchCache(Guid jobId)
        {
            JobRequest job = null;
            lock (_SyncInstanceObject)
            {
                if (_JobWatchCache.Count > 0)
                {
                    if (_JobWatchCache.ContainsKey(jobId))
                    {
                        job = _JobWatchCache[jobId];
                        if (job != null)
                        {
                            _JobWatchCache.Remove(jobId);
                        }
                    }
                }
            }
            return job;
        }

        public void RemoveJobByJobIdFromWatchCache(Guid jobId)
        {
            JobRequest job = null;
            lock (_SyncInstanceObject)
            {
                if (_JobWatchCache.Count > 0)
                {
                    job = _JobWatchCache[jobId];
                    if (job != null)
                    {
                        _JobWatchCache.Remove(jobId);
                    }
                }
            }
        }
        
        public int GetJobWatchCacheCount()
        {
            int count = 0;
            lock (_SyncInstanceObject)
            {
                count = _JobWatchCache.Count();
            }
            return count;
        }
        public List<Guid> GetJobWatchIdList()
        {
            List<Guid> list = new List<Guid>();
            lock (_SyncInstanceObject)
            {
                if (_JobWatchCache.Count()>0)
                    foreach (Guid id in _JobWatchCache.Keys)
                    {
                        list.Add(id);
                    }
            }
            return list;
        }
    }
}
