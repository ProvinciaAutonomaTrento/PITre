using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Adlib.Director.DirectorWSAWrapper.Jobs
{
    public class JobWatcher : JobHandlerBase
    {

        public override void Execute()
        {
            _LogManager.Info("About to execute JobWatcher. Total jobs to watch: " + (JobCacheMngr.Instance().GetJobCacheCount() + JobCacheMngr.Instance().GetJobWatchCacheCount()).ToString());
            DirectorWSAWrapper wrapper = new DirectorWSAWrapper();
            string response = "";
            bool init = wrapper.InitializeGenericConnector(ref response);
            if (!init)
            {
                wrapper.LogManager().Error("JobWatcher failed to initialize DirectorWSAWrapper: " + response);
                return;
            }
            List<Guid> guidList = new List<Guid>();
            List<JobManagementService.JobStatusResponse> jobStatusResponse = null;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            int count = 0;
            Thread.Sleep(500); //let everybody else to start
            while (NeedStop() == false)
            {
                count = 0;
                if (guidList != null && guidList.Count > 0)
                {
                    guidList.Clear();
                }
                if (NeedStop() == true)
                {

                    break;
                }
                try
                {
                    if (JobCacheMngr.Instance().GetJobWatchCacheCount() > 0)
                    {

                        //old heavy way
                        /*
                        watch.Reset();
                        watch.Start();
                        Guid[] arr = wrapper.GetJobsId(ViewJobs.Completed);
                        watch.Stop();
                        long getIdsDuration = watch.ElapsedMilliseconds;

                        if (arr != null && arr.Length > 0)
                        {
                            guidList = new List<Guid>(arr);
                        }
                        if (guidList != null)
                        {
                            count = guidList.Count();
                        }
                        if (count > 0)
                        {
                            _LogManager.Info(string.Format("JobWatcher found {0} completed jobs. Each will be quered for details and released after. GetJobsId duration: {1}ms", count, getIdsDuration));
                            
                            foreach (Guid jobId in guidList)
                            {
                                try
                                {
                                    JobRequest job = JobCacheMngr.Instance().GetJobByJobIdFromWatchCache(jobId);
                                    if (job != null)
                                    {
                                        JobCacheMngr.Instance().AddToJobReleaseCache(job);
                                        //JobCacheMngr.Instance().RemoveJobByJobIdFromWatchCache(jobId);
                                    }
                                }
                                catch (Exception je)
                                {
                                    _LogManager.Warn("Failed to move job for release: " + je.Message);
                                }
                            }
                        }
                         * */

                        //using new ligther call
                        List<Guid> ids = JobCacheMngr.Instance().GetJobWatchIdList();
                        watch.Reset();
                        watch.Start();
                        if (ids != null && ids.Count > 0)
                        {
                            jobStatusResponse = wrapper.GetJobsStatusInfo(ids);
                        }
                        watch.Stop();
                        long getIdsDuration = watch.ElapsedMilliseconds;
                        if (jobStatusResponse != null)
                        {
                            count = jobStatusResponse.Count();
                        }
                        if (count > 0)
                        {
                            int countCompleted = 0;
                            foreach (JobManagementService.JobStatusResponse rs in jobStatusResponse)
                            {
                                if (rs.JobStatus == JobManagementService.JobStatus.CompletedCancelled ||
                                    rs.JobStatus == JobManagementService.JobStatus.CompletedFailed ||
                                    rs.JobStatus == JobManagementService.JobStatus.CompletedResubmissionFailed ||
                                    rs.JobStatus == JobManagementService.JobStatus.CompletedSuccessful ||
                                    rs.JobStatus == JobManagementService.JobStatus.CompletedUncommitted)
                                {
                                    try
                                    {
                                        JobRequest job = JobCacheMngr.Instance().GetJobByJobIdFromWatchCache(rs.JobId);
                                        if (job != null)
                                        {
                                            JobCacheMngr.Instance().AddToJobReleaseCache(job);
                                            //JobCacheMngr.Instance().RemoveJobByJobIdFromWatchCache(jobId);
                                        }
                                    }
                                    catch (Exception je)
                                    {
                                        _LogManager.Warn("Failed to move job for release: " + je.Message);
                                    }
                                    countCompleted++;
                                }
                            }

                            if (countCompleted > 0)
                            {
                                _LogManager.Info(string.Format("JobWatcher found {0} completed jobs (out of {2} queued). Each will be queued for details and released after. GetJobsStatus duration: {1}ms ({2} total queued jobs)", countCompleted, getIdsDuration, count));
                            }
                        }
                    
                    }

                    if (NeedStop() == true)
                    {

                        break;
                    }
                    if (JobCacheMngr.Instance().GetJobWatchCacheCount() == 0 &&
                        JobCacheMngr.Instance().GetJobCacheCount() == 0)
                    {
                        _LogManager.Info("Exiting JobWatcher, because there are no more jobs to submit.");
                        break;
                    }
                    //sleep anyway before next query
                    if (JobCacheMngr.Instance().GetJobReleaseCacheCount() > 100)
                    {
                        Thread.Sleep(1000);
                    }
                    else if (JobCacheMngr.Instance().GetJobWatchCacheCount() == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Thread.Sleep(500);
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
