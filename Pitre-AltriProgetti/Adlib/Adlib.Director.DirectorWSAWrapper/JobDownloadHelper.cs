using System;
using System.Collections.Generic;
using System.Text;

namespace Adlib.Director.DirectorWSAWrapper
{

    public class JobDownloadHelper
    {
        private static List<JobDownloadRecord> _jobListForDownload = new List<JobDownloadRecord>();
        private static object _lockObject = new object();
        private static  DirectorWSAWrapper _DirectorWrapper = null;
        private static bool _stopWork = false;

        public static void Initialize(DirectorWSAWrapper directorWrapper)
        {
            _DirectorWrapper = directorWrapper;
        }
        public static List<JobDownloadRecord> GetJobIdsForDownload()
        {
            List<JobDownloadRecord> jobIds = new List<JobDownloadRecord>();
            lock (_lockObject)
            {
                jobIds.AddRange(_jobListForDownload.ToArray());
            }
            return jobIds;
        }
        public static void AddJobIdForDownload(Guid jobId, bool useFileRepository)
        {
            lock (_lockObject)
            {
                _jobListForDownload.Add(new JobDownloadRecord(jobId, useFileRepository));
            }
        }
        public static void RemoveJobIdFromDownload(Guid jobId)
        {
            lock (_lockObject)
            {
                foreach (JobDownloadRecord jobRecord in _jobListForDownload)
                {
                    if (jobRecord != null && jobRecord.JobId == jobId)
                    {
                        _jobListForDownload.Remove(jobRecord);
                        break;
                    }
                }
            }
        }
        public static void RemoveAllJobIds()
        {
            lock (_lockObject)
            {
                _jobListForDownload.Clear();
            }
        }
        public static void StopWork()
        {
            _stopWork = true;
        }

        public static void DoWork()
        {
            bool result = false;
            List<JobDownloadRecord> jobList = null;
            while(true)
            {
                if (_stopWork)
                    break;
                jobList = GetJobIdsForDownload();
                if (jobList != null && jobList.Count > 0)
                {
                    foreach (JobDownloadRecord jobRecord in jobList)
                    {
                        if (_DirectorWrapper != null)
                        {
                            try
                            {
                                Adlib.Director.DirectorWSAWrapper.DirectorWSA.JobDetailResponse response = _DirectorWrapper.GetJobDetails(jobRecord.JobId);
                                if (response != null)
                                {
                                    if (response.JobStatus == Adlib.Director.DirectorWSAWrapper.DirectorWSA.JobStatus.CompletedSuccessful)
                                    {
                                        result = _DirectorWrapper.DownloadOutputDocuments(jobRecord.JobId, jobRecord.UseFileRepository, 2 /*2Mb chunks*/);
                                        if (result == false)
                                        {
                                            _DirectorWrapper.LogManager().Info(string.Format("Failed to download outputs for job with JobId: {0}. See log for details.", jobRecord.JobId));
                                        }
                                        else
                                        {
                                            _DirectorWrapper.LogManager().Info(string.Format("Outputs were successfully downloaded for job with JobId: {0}.", jobRecord.JobId));
                                        }
                                        RemoveJobIdFromDownload(jobRecord.JobId);
                                    }
                                    else
                                        if (response.JobStatus == Adlib.Director.DirectorWSAWrapper.DirectorWSA.JobStatus.CompletedCancelled ||
                                            response.JobStatus == Adlib.Director.DirectorWSAWrapper.DirectorWSA.JobStatus.CompletedFailed ||
                                            response.JobStatus == Adlib.Director.DirectorWSAWrapper.DirectorWSA.JobStatus.CompletedResubmissionFailed)
                                        {
                                            _DirectorWrapper.LogManager().Info(string.Format("Removing job with JobId: {0} from download list because it's status is {1}", jobRecord.JobId, response.JobStatus.ToString()));
                                            RemoveJobIdFromDownload(jobRecord.JobId);
                                        }

                                }
                                else
                                {
                                    _DirectorWrapper.LogManager().Error(string.Format("Can't find Job with JobId: {0} to download outputs. Deleting job from queue.", jobRecord.JobId));
                                    RemoveJobIdFromDownload(jobRecord.JobId);

                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        if (_stopWork)
                            break;
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                if (_stopWork)
                    break;
                System.Threading.Thread.Sleep(5000);
            }

        }

    }
}
