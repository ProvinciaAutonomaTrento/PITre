using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Adlib.Director.DirectorWSAWrapper.Jobs
{
    public class JobSubmitter : JobHandlerBase
    {
        private string DefaultTimeFormat = "yyyy_MM_dd_HH_mm_ss_fff";
        private int _MaxNumberFilesInSubfolder = 0;
        private int _SleepBetweenJobs = 0;

        public int SleepBetweenJobs
        {
            get { return _SleepBetweenJobs; }
            set 
            { 
                _SleepBetweenJobs = value;
                if (_SleepBetweenJobs < 0)
                {
                    _SleepBetweenJobs = 0;
                }
            }
        }

        public int MaxNumberFilesInSubfolder
        {
            get { return _MaxNumberFilesInSubfolder; }
            set 
            { 
                _MaxNumberFilesInSubfolder = value;
            }
        }

        public override void Execute()
        {
            _LogManager.Info("About to execute JobSubmitter. Total jobs to submit: " + JobCacheMngr.Instance().GetJobCacheCount());
            JobRequest job = null;
            Guid jobId = Guid.Empty;
            DirectorWSAWrapper wrapper = new DirectorWSAWrapper();
            string response = "";
            bool init = wrapper.InitializeGenericConnector(ref response);
            if (!init)
            {
                wrapper.LogManager().Error("JobSubmitter failed to initialize DirectorWSAWrapper: " + response);
                return;
            }
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            string fullPath = wrapper.DirectorSettings.CustomUncPathForInputDocs;
            int currentJob = 0;
            string subfolder = _ThreadTag;
            if (_MaxNumberFilesInSubfolder > 0)
            {
                subfolder = _ThreadTag + "\\" + DateTime.Now.ToString(DefaultTimeFormat);
                fullPath = System.IO.Path.Combine(wrapper.DirectorSettings.CustomUncPathForInputDocs, subfolder);
                System.IO.Directory.CreateDirectory(fullPath);
            }
            Thread.Sleep(500); //let everybody else to start
            while (NeedStop() == false)
            {
                job = null;
                jobId = Guid.Empty;
                if (NeedStop() == true)
                {
                    break;
                }
                if (_SleepBetweenJobs > 0)
                {
                    System.Threading.Thread.Sleep(_SleepBetweenJobs);
                }
                try
                {
                    job = JobCacheMngr.Instance().GetNextJobFromCache();
                    if (job == null)
                    {
                        _LogManager.Info("Exiting JobSubmitter, because there are no more jobs to submit.");
                        break;
                    }

                    if (_MaxNumberFilesInSubfolder > 0 && currentJob >= _MaxNumberFilesInSubfolder)
                    {
                        subfolder = _ThreadTag + "\\" + DateTime.Now.ToString(DefaultTimeFormat);
                        fullPath = System.IO.Path.Combine(wrapper.DirectorSettings.CustomUncPathForInputDocs, subfolder);
                        System.IO.Directory.CreateDirectory(fullPath);
                        currentJob = 0;
                    }

                    watch.Reset();
                    watch.Start();
                    string tempSubfolder = subfolder;
                    if (!string.IsNullOrEmpty(job.NickName))
                    {
                        if (!string.IsNullOrEmpty(tempSubfolder))
                        {
                            tempSubfolder += "\\";
                        }
                        tempSubfolder += job.NickName;
                    }
                    if (job.SubmitAsOneCall)
                    {
                        jobId = wrapper.SubmitJobSimple(job.InputFileList, job.MetadataList, tempSubfolder, Guid.NewGuid());
                    }
                    else
                    {
                        jobId = wrapper.SubmitDocuments(job.InputFileList, job.MetadataList, false, tempSubfolder);
                    }
                    currentJob++;
                    watch.Stop();
                    if (jobId == Guid.Empty)
                    {
                        _LogManager.Error("JobSubmitter failed to submit job. See log for details.");
                    }
                    else
                    {
                        job.JobId = jobId;
                        JobCacheMngr.Instance().AddToJobWatchCache(job);
                        _LogManager.Info(string.Format("JobSubmitter added new job: {0}. Duration: {1}ms",jobId, watch.ElapsedMilliseconds));
                    }

                }
                catch (Exception ee)
                {
                    _LogManager.Warn("Failed to submit job: " + ee.Message);
                }
                finally
                {

                }
            }
        }
    }
}
