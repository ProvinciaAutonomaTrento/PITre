using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Adlib.Director.DirectorWSAWrapper.Jobs
{
    public class JobFinalizer: JobHandlerBase
    {

        private bool _DeleteFolderOnRelease = false;

        public bool DeleteFolderOnRelease
        {
            get { return _DeleteFolderOnRelease; }
            set { _DeleteFolderOnRelease = value; }
        }
        public override void Execute()
        {
            _LogManager.Info("About to execute JobFinalizer. Total jobs to watch: " + (JobCacheMngr.Instance().GetJobCacheCount() + JobCacheMngr.Instance().GetJobWatchCacheCount() + JobCacheMngr.Instance().GetJobReleaseCacheCount()).ToString());
            DirectorWSAWrapper wrapper = new DirectorWSAWrapper();
            List<JobManagementService.Metadata> metadataList  = new List<JobManagementService.Metadata>();
            metadataList.Add(new JobManagementService.Metadata
            {
                Name = "Adlib.Job.Release.Light",
                Value = ""
            });
            string response = "";
            bool init = wrapper.InitializeGenericConnector(ref response);
            if (!init)
            {
                wrapper.LogManager().Error("JobFinalizer failed to initialize DirectorWSAWrapper: " + response);
                return;
            }
            //JobManagementService.JobDetailResponse jobDetails = null;
            List<JobManagementService.JobCompletionInfoResponse> jobCompletionInfoResponse = null;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            int count = 0;
            JobRequest job = null;
            Thread.Sleep(500); //let everybody else to start
            while (NeedStop() == false)
            {
                count = 0;

                if (NeedStop() == true)
                {

                    break;
                }
                try
                {
                    count = JobCacheMngr.Instance().GetJobReleaseCacheCount();
                    if (count > 0)
                    {
                        if (count > 0)
                        {
                            job = JobCacheMngr.Instance().GetNextJobFromReleaseCache();
                            while(job != null)
                            {
                                try
                                {
                                    //emulate quering job details
                                    watch.Reset();
                                    watch.Start();
                                    //heavy call
                                    //jobDetails = wrapper.GetJobDetails(job.JobId);
                                    //lighter call
                                    jobCompletionInfoResponse = wrapper.GetJobsCompletionInfo(new List<Guid>(){job.JobId});
                                    //print message here
                                    watch.Stop();
                                    long getDetailsDuration = watch.ElapsedMilliseconds;

                                    if (jobCompletionInfoResponse != null &&
                                        jobCompletionInfoResponse.Count == 1 && jobCompletionInfoResponse[0] != null)
                                    {
                                        _LogManager.Info(string.Format("Job Completion Info for job {0}: ResultCode: {1}, ResultMessage: {2}.", job.JobId,
                                            jobCompletionInfoResponse[0].ResultCode, jobCompletionInfoResponse[0].ResultMessage));
                                    }
                                    else
                                    {
                                        _LogManager.Error(string.Format("Failed to get Completion Info for: {0}.", job.JobId));
                                    }
                                    watch.Reset();
                                    watch.Start();
                                    if (_DeleteFolderOnRelease && jobCompletionInfoResponse != null &&
                                        jobCompletionInfoResponse.Count == 1 && jobCompletionInfoResponse[0] != null)
                                    {
                                        if (jobCompletionInfoResponse[0].JobDocumentsInfo != null && jobCompletionInfoResponse[0].JobDocumentsInfo.DocumentInputs != null &&
                                            jobCompletionInfoResponse[0].JobDocumentsInfo.DocumentInputs.Length > 0)
                                        {
                                            JobManagementService.DocumentInput di = jobCompletionInfoResponse[0].JobDocumentsInfo.DocumentInputs[0];
                                            if (di != null && !string.IsNullOrEmpty(di.Folder))
                                            {
                                                try
                                                {
                                                    Adlib.Common.IO.IOTools.DeleteFolderContents(di.Folder, true);
                                                    System.IO.Directory.Delete(di.Folder, true);
                                                }
                                                catch (Exception ioE2)
                                                {
                                                    _LogManager.Error(string.Format("Failed to delete folder for: {0}. Folder: {1}. Error: {2}", jobCompletionInfoResponse[0].JobId, di.Folder, ioE2.Message));
                                                }
                                            }
                                        }

                                    }
                                    watch.Stop();
                                    long deleteDuration = watch.ElapsedMilliseconds;
                                    watch.Start();
                                    JobManagementService.JobResponse[] rs = wrapper.ReleaseJobs(new List<Guid> { job.JobId }, metadataList);
                                    watch.Stop();
                                    long releaseDuration = watch.ElapsedMilliseconds;
                                    watch.Reset();
                                    _LogManager.Info(string.Format("JobFinalizer {0} released job: {1}. Result: {2}. GetJobsCompletionInfo duration: {3}ms, ReleaseJob duration: {4}ms. JobReleaseCacheCount: {5}. Delete folder duration: {6}", this._ThreadTag, job.JobId, rs[0].ResultCode, getDetailsDuration, releaseDuration, JobCacheMngr.Instance().GetJobReleaseCacheCount(), deleteDuration));
                                }
                                catch (Exception je)
                                {
                                    _LogManager.Warn("Failed to get job details or release it: " + je.Message);
                                }
                                job = JobCacheMngr.Instance().GetNextJobFromReleaseCache();
                            }
                        }
                    }

                    if (NeedStop() == true)
                    {

                        break;
                    }
                    if (JobCacheMngr.Instance().GetJobWatchCacheCount() == 0 &&
                        JobCacheMngr.Instance().GetJobCacheCount() == 0 &&
                        JobCacheMngr.Instance().GetJobReleaseCacheCount()==0)
                    {
                        _LogManager.Info("Exiting JobFinalizer, because there are no more jobs to release.");
                        break;
                    }
                    //sleep anyway before next query
                    if (JobCacheMngr.Instance().GetJobReleaseCacheCount() == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Sleep(5);
                    }

                }
                catch (Exception ee)
                {
                    _LogManager.Warn("Failed to release job: " + ee.Message);
                }
                finally
                {

                }
            }
        }

    }
}
