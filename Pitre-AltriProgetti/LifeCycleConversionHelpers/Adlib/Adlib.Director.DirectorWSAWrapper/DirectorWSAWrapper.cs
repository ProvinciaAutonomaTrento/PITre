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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using Adlib.Director.DirectorWSAWrapper.Logging;
using System.Threading;
using System.Text.RegularExpressions;
using Adlib.Runtime.Heartbeat;
using System.Xml;
using System.Reflection;

using Adlib.Director.DirectorWSAWrapper.JobManagementService;
using System.ServiceModel;

namespace Adlib.Director.DirectorWSAWrapper
{
    public enum ViewJobs
    {
        ShowAll,
        Completed,
        InProgress
    }
    public class JobDownloadRecord
    {
        public Guid JobId;
        public bool UseFileRepository = false;
        private JobDownloadRecord() { }
        public JobDownloadRecord(Guid jobId, bool useFileRepository)
        {
            JobId = jobId;
            UseFileRepository = useFileRepository;
        }
    }
    public class DirectorWSAWrapper
    {
        private const string _LogApplicationName = "Adlib.Generic.Connector";
        private const string _DirectorSettingsFileName = "AdlibGenericConnectorSettings.xml";


        private string _DirectorSettingsFullPath;
        private DirectorSettings _DirectorSettings = new DirectorSettings();
        private Logging.LogMngr _LogMngr;
        private ILogger _ElsLogger;
        private List<JobDownloadRecord> _SubmittedJobs = new List<JobDownloadRecord>();
        private bool _DirectConnectorIsInitializedOnce = false;
        private static Regex _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE = new Regex(MD_NAME_JOB_TICKET_TEMPLATE, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE_V21 = new Regex(MD_NAME_JOB_TICKET_TEMPLATE_V21, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _RegEx_MD_ENGINE_JOB_SETTINGS_TEMPLATE_NAME = new Regex(Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_ENGINE_JOB_SETTINGS_TEMPLATE_NAME, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private HeartbeatThread _HeartbeatThreadHandler = null;
        private string _SettingsString = "";
        JobManagementService.JobManagementServiceClient _jms = null;
        string _prevServiceAddress = "";

        //public consts
        public const string MD_NAME_JOB_TICKET_TEMPLATE_V21 = "Adlib.Director.*XmlJobTicketTemplate*"; //old snippets
        public const string MD_NAME_JOB_TICKET_TEMPLATE = "Adlib.SystemManager.*XmlJobTicketTemplate*"; //old snippets
        public const string MD_ENGINE_JOB_SETTINGS_TEMPLATE_NAME = "Adlib.Schemas.Engines.*.Job.*";//new snippets

        public const string MD_NAME_CUSTOM_JOB_TICKET_TEMPLATE = "Adlib.SystemManager.CustomXmlJobTicketTemplate";
        public const string MD_INSTALLED_COMPONENT_FRIENDLY_NAME = "Adlib.Configuration.InstalledComponent.FriendlyName";

        public const string DIRECTOR_WSA_EXECUTE_COMMAND_TEST = "GetSystemHealth";
        public const string DIRECTOR_WSA_EXECUTE_COMMAND_COMPONENT_STATUS = "GetComponentsStatus";

        public const string MD_NAME_ADMIN_SCOPE_ID = "Adlib.Configuration.AdminScopeId";

        public string GenericConnectorType = "GenericConnector";

        public DirectorWSAWrapper()
        {
            string xSettingsLocation = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            FileInfo fiAssemblyLocation = new FileInfo(xSettingsLocation);
            FileInfo fiSettings = new FileInfo(Path.Combine(fiAssemblyLocation.DirectoryName, _DirectorSettingsFileName));
            _DirectorSettingsFullPath = fiSettings.FullName;
            if (!fiSettings.Exists)
            {
                DirectorSettings = new DirectorSettings();
            }
            else
            {
                try
                {
                    DirectorSettings = (DirectorSettings)Serialize.DeserializeObject(File.ReadAllText(fiSettings.FullName), typeof(DirectorSettings));
                }
                catch
                {
                    DirectorSettings = new DirectorSettings();
                }
            }
            try
            {
                _ElsLogger = new Adlib.Director.DirectorWSAWrapper.Logging.ElsLogger();
                Adlib.Director.DirectorWSAWrapper.Logging.ElsLogger.ApplicationName = _LogApplicationName;
                _LogMngr = new Adlib.Director.DirectorWSAWrapper.Logging.LogMngr();
                _LogMngr.AddLogger(_ElsLogger);

                if (Adlib.Common.AdlibConfig.Instance() == null)
                {
                    _LogMngr.Error("'Adlib.config file is missing. This file is required for proper initialization.'");
                }
            }
            catch
            {
            }


        }

        /// <summary>
        /// Returns a logging instance to log messages to file and screen
        /// </summary>
        /// <param name=""></param>
        public Logging.LogMngr LogManager()
        {
            return _LogMngr;
        }

        /// <summary>
        /// Property to set/get Director-related settings 
        /// </summary>
        /// <param name=""></param>
        public DirectorSettings DirectorSettings
        {
            get
            {
                if (_DirectorSettings == null)
                {
                    _DirectorSettings = new DirectorSettings();
                }
                return _DirectorSettings;
            }
            set
            {
                if (value == null)
                {
                    _DirectorSettings = new DirectorSettings();
                }
                else
                {
                    if (_DirectorSettings == null)
                    {
                        _DirectorSettings = new DirectorSettings(value);
                    }
                    else
                    {
                        _DirectorSettings.Update(value);
                    }
                }
            }
        }

        /// <summary>
        /// Saves Director-related settings to the disk
        /// </summary>
        /// <param name=""></param>
        public bool SaveSettings()
        {
            bool saveResult = false;
            FileInfo fiSettings = null;
            if (!string.IsNullOrEmpty(_DirectorSettingsFullPath))
            {
                string xSettingsLocation = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                FileInfo fiAssemblyLocation = new FileInfo(xSettingsLocation);
                fiSettings = new FileInfo(Path.Combine(fiAssemblyLocation.DirectoryName, _DirectorSettingsFileName));
                _DirectorSettingsFullPath = fiSettings.FullName;
            }
            try
            {
                if (fiSettings != null)
                {
                    if (!Directory.Exists(fiSettings.DirectoryName))
                    {
                        Directory.CreateDirectory(fiSettings.DirectoryName);
                    }
                    if (File.Exists(_DirectorSettingsFullPath))
                    {
                        File.Delete(_DirectorSettingsFullPath);
                    }
                    string settingsOutput = Serialize.SerializeObject(DirectorSettings, typeof(DirectorSettings));

                    System.IO.FileStream fs = new System.IO.FileStream(_DirectorSettingsFullPath, FileMode.OpenOrCreate | System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter streamWriter = new StreamWriter(fs);
                    streamWriter.Write(settingsOutput);
                    streamWriter.Flush();
                    streamWriter.Close();
                    fs.Close();
                }

            }
            catch (IOException ioException)
            {
                LogManager().Warn("Failed to save settings. Error: " + ioException.Message);
            }
            return saveResult;
        }


        /// <summary>
        /// Calls DirectorWSA.ExecuteCommand method to query Director System health. If system is not healthy, jobs will not be 
        /// processed successfully.
        /// </summary>
        /// <param name=""></param>
        public string RefreshSystemHealth()
        {
            //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
            JobManagementServiceClient jms = GetJobManagementService();
            ExecuteCommandResponse response = jms.ExecuteCommand(DIRECTOR_WSA_EXECUTE_COMMAND_TEST, null);
            string xResult = Serialize.SerializeObject(response, typeof(ExecuteCommandResponse));
            return xResult;
            //return response.ResultMessage;
        }

        //Get status of all components installed in the system
        public string GetComponentStatus()
        {
            //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
            JobManagementServiceClient jms = GetJobManagementService();
            ExecuteCommandResponse response = jms.ExecuteCommand(DIRECTOR_WSA_EXECUTE_COMMAND_COMPONENT_STATUS, null);
            //string xResult = Serialize.SerializeObject(response, typeof(Adlib.Director.DirectorWSAWrapper.DirectorWSA.ExecuteCommandResponse));
            string xResult = null;
            if (response.ResultCode == 0)
            {
                xResult = response.ResultMessage;
            }
            else
            {
                xResult = Serialize.SerializeObject(response, typeof(ExecuteCommandResponse));
            }
            return xResult;
            //return response.ResultMessage;
        }


        /// <summary>
        /// Returns Job completion info for the specified JobId.
        /// This call be used to get details on output documents when job is completed.
        /// </summary>
        /// <param name=""></param>
        public List<JobCompletionInfoResponse> GetJobsCompletionInfo(List<Guid> jobIds)
        {
            JobManagementServiceClient jms = GetJobManagementService();
            JobCompletionInfoResponse[] responseList = jms.GetJobsCompletionInfo(jobIds.ToArray(), null);

            List<JobCompletionInfoResponse> responses = null;
            if (responseList != null)
            {
                responses = new List<JobCompletionInfoResponse>(responseList);
            }
            return responses;
        }

        /// <summary>
        /// Returns Job status info for the specified JobId.
        /// This call be used to get details on output documents when job is completed.
        /// </summary>
        /// <param name=""></param>
        public List<JobStatusResponse> GetJobsStatusInfo(List<Guid> jobIds)
        {
            JobManagementServiceClient jms = GetJobManagementService();
            JobStatusResponse[] responseList = jms.GetJobsStatus(jobIds.ToArray(), null);

            List<JobStatusResponse> responses = null;
            if (responseList != null)
            {
                responses = new List<JobStatusResponse>(responseList);
            }
            return responses;
        }

        /// <summary>
        /// Returns Job details for the specified JobId.
        /// This call be used to get details on output documents when job is completed.
        /// </summary>
        /// <param name=""></param>
        public JobDetailResponse GetJobDetails(Guid jobId)
        {
            Guid[] guidList = new Guid[1];
            guidList[0] = jobId;
            JobDetailResponse[] responseList = GetJobDetails(guidList);
            if (responseList != null && responseList.Length > 0)
            {
                return responseList[0];
            }
            else return null;
        }

        /// <summary>
        /// Returns list of Job details for the specified jobIds
        /// </summary>
        /// <param name=""></param>
        public JobDetailResponse[] GetJobDetails(Guid[] jobIds)
        {
            JobManagementServiceClient jms = GetJobManagementService();
            JobDetailResponse[] responseList = GetJobDetails(jms, jobIds);
            return responseList;
        }

        /// <summary>
        /// Returns list of all the job ids (without details) with specific status(es): Completed/InProgress/All
        /// </summary>
        /// <param name=""></param>
        public Guid[] GetJobsId(ViewJobs viewJobsFlag)
        {
            Guid[] jobIds = null;
            try
            {

                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                JobManagementServiceClient jms = GetJobManagementService();
                JobKey jobKey = new JobKey();
                jobKey.SubmitterComponentId = DirectorSettings.SubmitterComponentId;
                jobKey.RepositoryId = DirectorSettings.RepositoryId;
                jobKey.UserDefinedId = DirectorSettings.UserDefinedId;
                List<JobStatus> statusList = new List<JobStatus>();
                if (viewJobsFlag == ViewJobs.ShowAll || viewJobsFlag == ViewJobs.Completed)
                {
                    statusList.Add(JobStatus.CompletedCancelled);
                    statusList.Add(JobStatus.CompletedFailed);
                    statusList.Add(JobStatus.CompletedResubmissionFailed);
                    statusList.Add(JobStatus.CompletedSuccessful);
                    statusList.Add(JobStatus.CompletedUncommitted);
                    statusList.Add(JobStatus.CompletedJarRejected);
                }
                if (viewJobsFlag == ViewJobs.InProgress || viewJobsFlag == ViewJobs.ShowAll)
                {
                    statusList.Add(JobStatus.Committed);
                    statusList.Add(JobStatus.Initialized);
                    statusList.Add(JobStatus.Processing);
                    statusList.Add(JobStatus.Ready);
                    statusList.Add(JobStatus.UnInitialized);
                }

                jobIds = jms.GetJobsId(_DirectorSettings.AdminScopeId, jobKey, statusList.ToArray(), null);
                LogManager().Info("JobManagementService.GetJobsId call completed succesfully.");
            }
            catch (Exception e)
            {
                LogManager().ErrorException("DirectorWSA.GetJobsId call failed.", e);
            }
            return jobIds;
        }


        
        /// <summary>
        /// Returns list of all the job (with details) with specific status(es): Completed/InProgress/All
        /// </summary>
        /// <param name=""></param>
        public JobDetailResponse[] GetJobsDetails(ViewJobs viewJobsFlag)
        {
            JobDetailResponse[] jobDetails = null;
            try
            {

                Guid[] guidList = GetJobsId(viewJobsFlag);
                JobManagementServiceClient jms = GetJobManagementService();
                if (guidList != null && guidList.Length > 0)
                {
                    jobDetails = GetJobDetails(jms, guidList);
                    LogManager().Info("JobManagementService.GetJobDetails call completed succesfully.");
                }
            }
            catch (Exception e)
            {
                LogManager().ErrorException("DirectorWSA.GetJobDetails call failed.", e);
            }
            return jobDetails;
        }

        /// <summary>
        /// This is a call for submitting all documents from specified folder (reccursive) to Director for rendering.
        /// All input documents will be merged to single output document.
        /// </summary>
        /// <param name=""></param>
        public Guid SubmitDocuments(string inputFolder,
                                   List<Metadata> inputMetadata,
                                   bool transferDocsToRepository)
        {
            Guid jobId = Guid.Empty;
            DirectoryInfo dInfo = new DirectoryInfo(inputFolder);
            if (dInfo.Exists == false)
            {
                LogManager().Error("Input document doesn't exist. Failed to submit a document.");
                return jobId;
            }
            else
            {
                List<string> fileList = new List<string>();
                Utilities.GetFilesList(ref fileList, inputFolder, null, true);
                if (fileList.Count == 0)
                {
                    LogManager().Error("No document(s) were found. Failed to submit the document(s).");
                }
                else
                {
                    jobId = SubmitDocuments(fileList, inputMetadata, transferDocsToRepository);
                }
            }

            return jobId;

        }

        //This call escapes a string so it can be included to XML elements values safely.
        //<  	-> 	&lt;
        //> 	-> 	&gt;
        //" 	-> 	&quot;
        //' 	-> 	&apos;
        //& 	-> 	&amp;

        private string EscapeXmlString(string xmlString)
        {
            string resString = System.Security.SecurityElement.Escape(xmlString); // - not good to use only this call, because it will escape already escaped symbols
            resString = resString.Replace("&amp;amp;", "&amp;");
            resString = resString.Replace("&amp;lt;", "&lt;");
            resString = resString.Replace("&amp;gt;", "&gt;");
            resString = resString.Replace("&amp;apos;", "&apos;");
            resString = resString.Replace("&amp;quot;", "&quot;");
            return resString;

        }

        public Guid SubmitDocuments(List<string> inputDocuments,
                                   List<Metadata> origInputMetadata,
                                   bool transferDocsToRepository)
        {
            return SubmitDocuments(inputDocuments, origInputMetadata, transferDocsToRepository, null);
        }

        /// <summary>
        /// This is a core call for submitting list of input documents to the Director for rendering.
        /// All input documents will be merged to single output document.
        /// In order to have one output per each document this call should be called separately per each input file.
        /// Flag transferDocsToRepository forces input documents to be transferred to FileRepository using MTOM mechanism.
        /// </summary>
        /// <param name=""></param>
        public Guid SubmitDocuments(List<string> inputDocuments,
                                   List<Metadata> origInputMetadata,
                                   bool transferDocsToRepository,
                                    string inputsSubpath)
        {
            List<Metadata> inputMetadata = new List<Metadata>();
            Metadata[] metadataList = null;
            ProcessRulesRequest rulesRequest = new ProcessRulesRequest();
            JobManagementService.JobManagementServiceClient jms = null;


            //need to create a copy, because this arra can be used by other job submittions
            if (origInputMetadata != null && origInputMetadata.Count > 0)
            {
                inputMetadata = new List<Metadata>(origInputMetadata.ToArray());
            }
            else
            {
                inputMetadata = new List<Metadata>();
            }

            Guid jobId = Guid.Empty;
            string response = "";
            // Component needs to initialize once, when it starts. Here we will initialize if it wasn't done before.
            bool initialized = InitializeGenericConnector(ref response);
            //mandatory field
            if (_DirectorSettings.SubmitterComponentId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). SubmitterComponentId can't be empty.");
                return jobId;
            }
            //mandatory field
            if (_DirectorSettings.AdminScopeId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). AdminScopeId can't be empty.");
                return jobId;
            }
            //mandatory field
            if (_DirectorSettings.WorkspaceId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). Transformation WorkspaceId can't be empty. Sources should be properly configured for this component.");
                return jobId;
            }

            if (initialized == false)
            {
                LogManager().Warn("Failed to initialize settings from MRE. Values from Settings tab will be used.");
            }

            //need at least one input document.
            if (inputDocuments == null || inputDocuments.Count == 0)
            {
                LogManager().Error("No document(s) were found. Failed to submit the document(s).");
                return jobId;
            }

            //Here we escape metadata, except for actual job tickets
            if (inputMetadata != null && inputMetadata.Count > 0)
            {

                Match xMatch1 = null;
                Match xMatch2 = null;
                Match xMatch3 = null;
                foreach (Metadata md in inputMetadata)
                {
                    //escape Metadata here, because it goes to Job Ticket, which is an XML file
                    if (md != null && md.Value != null && md.Value.Length > 0 && _DirectorSettings.ExcapeMetadata == true)
                    {
                        //we should not escape actual job ticket snippets
                        xMatch1 = _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE.Match(md.Name);
                        xMatch2 = _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE_V21.Match(md.Name);
                        xMatch3 = _RegEx_MD_ENGINE_JOB_SETTINGS_TEMPLATE_NAME.Match(md.Name);
                        if (xMatch1.Success == false && xMatch2.Success == false && xMatch3.Success == false)
                        {

                            md.Value = EscapeXmlString(md.Value);
                        }
                        else
                        {
                            md.Type = JobManagementService.MetadataType.XML;
                        }

                    }
                }
            }


            // Need to add Metadata with "Adlib.Configuration.InstalledComponentId" name, which uniquely identifies component.
            // This metadata could be used in JAR/Transformation rulesets
            Metadata componentIdMd = new Metadata();
            componentIdMd.Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_INSTALLED_COMPONENT_ID;
            componentIdMd.Value = DirectorSettings.SubmitterComponentId.ToString().ToUpper();
            componentIdMd.Type = MetadataType.Guid;
            inputMetadata.Add(componentIdMd);

            //Output extension will be defined by transformation rules selected.
            //SystemManager will set proper output file type & correct output file extension.
            string outputExtension = "pdf";
            Guid objectLockId = Guid.Empty;
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {
                bool defineFilesResult = false;
                bool uploadResult = false;

                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                jms = GetJobManagementService();
                //jms = GetNotCachedJobManagementService();

                // if settings force to use ObjectLock for the input file - we have to call AquireObjectLock call
                // If call fail - submittion will fail too.
                // As a sample we are going to use machineName and first input file name as a locking object
                if (DirectorSettings.UseObjectLock == true)
                {
                    string objectLockSource = Dns.GetHostName() + "<" + inputDocuments[0] + ">";
                    List<Metadata> objectLockMetadata = new List<Metadata>();
                    //populate your menatada here for AquireObjectLock
                    //For example ComponentHeartbeatId should be specified 
                    Metadata md = new Metadata();
                    md.Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_COMPONENT_HEARTBEAT_ID;
                    //Replace empty one with real one
                    md.Value = Guid.Empty.ToString();
                    objectLockMetadata.Add(md);
                    //Adlib.Director.DirectorWSAWrapper.DirectorWSA.ObjectLockResponse objLockResponse = directorWsa.AcquireObjectLock(objectLockSource, _DirectorSettings.SubmitterComponentId, _DirectorSettings.SubmitterComponentId, objectLockMetadata.ToArray());
                    JobManagementService.ObjectLockResponse objLockResponse = jms.AcquireObjectLock(objectLockSource, _DirectorSettings.SubmitterComponentId, _DirectorSettings.SubmitterComponentId, objectLockMetadata.ToArray());
                    if (objLockResponse != null && objLockResponse.ResultCode == 0)
                    {
                        objectLockId = objLockResponse.ObjectLockId;
                    }
                    else
                    {
                        LogManager().Error("Failed to submit job, because call AquireObjectLock was failed for: " + objectLockSource);
                        return jobId;
                    }
                }



                JobKey jobKey = new JobKey();
                jobKey.SubmitterComponentId = DirectorSettings.SubmitterComponentId;
                jobKey.RepositoryId = DirectorSettings.RepositoryId;
                jobKey.UserDefinedId = DirectorSettings.UserDefinedId;

                List<RulesetDefinition> rulesDefs = new List<RulesetDefinition>();
                rulesDefs = new List<RulesetDefinition>();

                RulesetDefinition rulesDef = new RulesetDefinition();
                rulesDef.ConfigurableEntityName = "JAR";
                rulesDef.WorkspaceId = _DirectorSettings.WorkspaceId;
                rulesDefs.Add(rulesDef);
                rulesRequest.RulesDefinitions = rulesDefs.ToArray();
                //Step 1 - Begin Job Transaction

                List<Metadata> systemMetadataList = null;
                Metadata[] systemMetadataArray = null;
                // if we previously aquired object lock - we need to update it with actual job id.
                // objectLockId will be added to system metadata list, oposite to job related metadata list.
                if (DirectorSettings.UseObjectLock == true && objectLockId != Guid.Empty)
                {
                    if (inputMetadata == null)
                    {
                        inputMetadata = new List<Metadata>();
                    }
                    systemMetadataList = new List<Metadata>();
                    Metadata objectLockMetadata = new Metadata();
                    objectLockMetadata.Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_OBJECTLOCK_OBJECTLOCK_ID;
                    objectLockMetadata.Value = objectLockId.ToString();
                    systemMetadataList.Add(objectLockMetadata);
                    systemMetadataArray = systemMetadataList.ToArray();
                }

                if (inputMetadata != null && inputMetadata.Count > 0)
                {
                    metadataList = inputMetadata.ToArray();
                }
                //IMPORTANT!
                //Job related metadata (which will be used for running JAR & Transformation rules) should be part of ProcessRUlesRequest.
                rulesRequest.MetadataInputs = metadataList;


                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.BeginJobTransactionResponse beginTransResponse = directorWsa.BeginJobTransaction(_DirectorSettings.AdminScopeId, jobKey, rulesRequest, systemMetadataArray);
                watch.Start();
                BeginJobTransactionResponse beginTransResponse = jms.BeginJobTransaction(_DirectorSettings.AdminScopeId, jobKey, rulesRequest, systemMetadataArray);
                watch.Stop();


                if (beginTransResponse.ResultCode != 0)
                {
                    LogManager().Error("JobManagementService.BeginJobTransaction failed. Message: " + beginTransResponse.ResultMessage);
                    if (DirectorSettings.UseObjectLock == true && objectLockId != Guid.Empty)
                    {
                        //Adlib.Director.DirectorWSAWrapper.DirectorWSA.ObjectLockResponse releaseObjLockResponse = directorWsa.ReleaseObjectLock(objectLockId, null);
                        ObjectLockResponse releaseObjLockResponse = jms.ReleaseObjectLock(objectLockId, null);
                        if (releaseObjLockResponse != null && releaseObjLockResponse.ResultCode == 0)
                        {
                            LogManager().Info("Completed ReleaseObjectLock for ObjectLockId: " + objectLockId);
                        }
                        else
                        {
                            LogManager().Error("Failed to ReleaseObjectLock for ObjectLockId: " + objectLockId);
                            return jobId;
                        }

                    }
                }
                //continue, if we managed to BeginJobTransaction
                else
                {
                    jobId = beginTransResponse.JobId;
                    LogManager().Info(string.Format("JobManagementService.BeginJobTransaction completed successfully. JobId: {0}, BeginJobTransactionAccepted is {1}. Duration: {2} sec", beginTransResponse.JobId, beginTransResponse.BeginJobTransactionAccepted, ((double)watch.ElapsedMilliseconds) / 1000));

                    //Step 2  - transfer or define files
                    if (transferDocsToRepository)
                    {
                        //upload file to FIleRepository using MTOM
                        uploadResult = UploadJobFiles(beginTransResponse.JobId, inputDocuments, 2 /*2Mb chunck. If this value is 0 - all file will be uploaded in one chunk*/);
                        if (!uploadResult)
                        {
                            LogManager().Error("Failed to upload files to File Repository. Failed to submit a new job. JobId: " + beginTransResponse.JobId);
                        }
                        else
                        {
                            //Here we define output filename & extention.
                            //It is important to get job details before defining output, so inputs don't get wiped out.
                            JobDetailResponse[] jobDetailResponse = jms.GetJobsDetail(new Guid[1] { beginTransResponse.JobId }, null);
                            if (jobDetailResponse != null && jobDetailResponse.Length > 0)
                            {
                                FileInfo fi = new FileInfo(inputDocuments[0]);
                                //Here we define output file name. For example we will use first file in a list name.
                                //Output file extensions will be difined later, based on Transformation rules results.
                                //System Manager will appropriate output file type to job ticket and will correct output file extention.
                                jobDetailResponse[0].JobDocumentsInfo.DocumentOutputs[0].FileName = fi.Name + outputExtension;
                                JobFilesResponse fiResp = jms.DefineJobFiles(beginTransResponse.JobId, jobDetailResponse[0].JobDocumentsInfo, null);
                                defineFilesResult = fiResp.SendJobFilesSuccessful;
                            }
                        }
                    }
                    else
                    {
                        //we can't use local path to DirectorWSA, because Express server(s) will not be able access those files - Director/Express can be on different machine
                        // so, we are going to copy local file to unc path, specified in settings (with JobId subfolder)
                        // DateTime startCopyDT = DateTime.Now;
                        watch.Reset();

                        string subpath = beginTransResponse.JobId.ToString();
                        if (!string.IsNullOrEmpty(inputsSubpath))
                        {
                            subpath = inputsSubpath + "\\" + subpath;
                        }
                        watch.Start();
                        List<string> uncInputPathList = CopyFilesToUNCPath(inputDocuments, subpath);
                        watch.Stop();
                        double copyDuration = ((double)watch.ElapsedMilliseconds) / 1000;
                        //after files were copied to UNC location - we need to update job details to have proper input document path specified.
                        watch.Reset();
                        watch.Start();
                        defineFilesResult = DefineJobFiles(uncInputPathList, beginTransResponse.JobId, outputExtension, jms);
                        watch.Stop();
                        LogManager().Info(string.Format("JobManagementService.DefineJobFiles completed result: {0}. JobId is {1}. Duration: {2}. Copy file(s) duration: {3} sec", defineFilesResult, beginTransResponse.JobId, ((double)watch.ElapsedMilliseconds) / 1000, copyDuration));
                    }

                    //Step 3 - Commit Job
                    if (defineFilesResult)
                    {
                        watch.Reset();
                        watch.Start();
                        JobResponse commitJobResponse = jms.CommitJobTransaction(beginTransResponse.JobId, metadataList);
                        watch.Stop();
                        if (commitJobResponse.ResultCode != 0)
                        {
                            LogManager().Error(string.Format("JobManagementService.CommitJobTransaction failed. JobId: {0}, Message: {1}", beginTransResponse.JobId, commitJobResponse.ResultMessage));
                            if (DirectorSettings.UseObjectLock == true && objectLockId != Guid.Empty)
                            {
                                ObjectLockResponse releaseObjLockResponse = jms.ReleaseObjectLock(objectLockId, null);
                                if (releaseObjLockResponse != null && releaseObjLockResponse.ResultCode == 0)
                                {
                                    LogManager().Info("Completed ReleaseObjectLock for ObjectLockId: " + objectLockId);
                                }
                                else
                                {
                                    LogManager().Error("Failed to ReleaseObjectLock for ObjectLockId: " + objectLockId);
                                    return jobId;
                                }

                            }
                            jobId = Guid.Empty;
                        }
                        else
                        {
                            LogManager().Info(string.Format("JobManagementService.CommitJobTransaction completed successfully. JobId is {0}. Duration: {1} sec", commitJobResponse.JobId, ((double)watch.ElapsedMilliseconds) / 1000));
                        }
                    }
                    else
                    {
                        jobId = Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager().ErrorException("Submit Documents failed.", ex);
            }

            if (metadataList != null && metadataList.Length > 0)
            {
                metadataList = null;
            }
            if (inputMetadata != null && inputMetadata.Count > 0)
            {
                inputMetadata.Clear();
            }

            return jobId;

        }

        public Guid SubmitJobSimple(List<string> inputDocuments, List<Metadata> origInputMetadata, Guid desiredJobId)
        {
            return SubmitJobSimple(inputDocuments, origInputMetadata, null, desiredJobId);
        }

        /// <summary>
        /// This is a core call for submitting list of input documents to the Director for rendering in one web-service call.
        /// All input documents will be merged to single output document.
        /// In order to have one output per each document this call should be called separately per each input file.
        /// </summary>
        public Guid SubmitJobSimple(List<string> inputDocuments, List<Metadata> origInputMetadata, string inputsSubpath, Guid desiredJobId)
        {
            List<Metadata> inputMetadata = new List<Metadata>();
            Metadata[] metadataList = null;
            ProcessRulesRequest rulesRequest = new ProcessRulesRequest();
            JobManagementService.JobManagementServiceClient jms = null;


            //need to create a copy, because this arra can be used by other job submittions
            if (origInputMetadata != null && origInputMetadata.Count > 0)
            {
                inputMetadata = new List<Metadata>(origInputMetadata.ToArray());
            }
            else
            {
                inputMetadata = new List<Metadata>();
            }

            Guid jobId = Guid.NewGuid();
            if (desiredJobId != Guid.Empty)
            {
                jobId = desiredJobId;
                inputMetadata.Add(new Metadata(){Name = "Adlib.Job.JobId", Value = jobId.ToString()});
            }
            string response = "";
            // Component needs to initialize once, when it starts. Here we will initialize if it wasn't done before.
            bool initialized = InitializeGenericConnector(ref response);
            //mandatory field
            if (_DirectorSettings.SubmitterComponentId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). SubmitterComponentId can't be empty.");
                return jobId;
            }
            //mandatory field
            if (_DirectorSettings.AdminScopeId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). AdminScopeId can't be empty.");
                return jobId;
            }
            //mandatory field
            if (_DirectorSettings.WorkspaceId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the document(s). Transformation WorkspaceId can't be empty. Sources should be properly configured for this component.");
                return jobId;
            }

            if (initialized == false)
            {
                LogManager().Warn("Failed to initialize settings from MRE. Values from Settings tab will be used.");
            }

            //need at least one input document.
            if (inputDocuments == null || inputDocuments.Count == 0)
            {
                LogManager().Error("No document(s) were found. Failed to submit the document(s).");
                return jobId;
            }

            //Here we escape metadata, except for actual job tickets
            if (inputMetadata != null && inputMetadata.Count > 0)
            {

                Match xMatch1 = null;
                Match xMatch2 = null;
                Match xMatch3 = null;
                foreach (Metadata md in inputMetadata)
                {
                    //escape Metadata here, because it goes to Job Ticket, which is an XML file
                    if (md != null && md.Value != null && md.Value.Length > 0 && _DirectorSettings.ExcapeMetadata == true)
                    {
                        //we should not escape actual job ticket snippets
                        xMatch1 = _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE.Match(md.Name);
                        xMatch2 = _RegEx_MD_MD_NAME_JOB_TICKET_TEMPLATE_V21.Match(md.Name);
                        xMatch3 = _RegEx_MD_ENGINE_JOB_SETTINGS_TEMPLATE_NAME.Match(md.Name);
                        if (xMatch1.Success == false && xMatch2.Success == false && xMatch3.Success == false)
                        {

                            md.Value = EscapeXmlString(md.Value);
                        }
                        else
                        {
                            md.Type = JobManagementService.MetadataType.XML;
                        }

                    }
                }
            }


            // Need to add Metadata with "Adlib.Configuration.InstalledComponentId" name, which uniquely identifies component.
            // This metadata could be used in JAR/Transformation rulesets
            Metadata componentIdMd = new Metadata();
            componentIdMd.Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_INSTALLED_COMPONENT_ID;
            componentIdMd.Value = DirectorSettings.SubmitterComponentId.ToString().ToUpper();
            componentIdMd.Type = MetadataType.Guid;
            inputMetadata.Add(componentIdMd);

            //Output extension will be defined by transformation rules selected.
            //SystemManager will set proper output file type & correct output file extension.
            string outputExtension = "pdf";
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            try
            {

                jms = GetJobManagementService();
                //jms = GetNotCachedJobManagementService();


                JobKey jobKey = new JobKey();
                jobKey.SubmitterComponentId = DirectorSettings.SubmitterComponentId;
                jobKey.RepositoryId = DirectorSettings.RepositoryId;
                jobKey.UserDefinedId = DirectorSettings.UserDefinedId;

                List<RulesetDefinition> rulesDefs = new List<RulesetDefinition>();
                rulesDefs = new List<RulesetDefinition>();

                RulesetDefinition rulesDef = new RulesetDefinition();
                rulesDef.ConfigurableEntityName = "JAR";
                rulesDef.WorkspaceId = _DirectorSettings.WorkspaceId;
                rulesDefs.Add(rulesDef);
                rulesRequest.RulesDefinitions = rulesDefs.ToArray();
                //Step 1 - Begin Job Transaction

                Metadata[] systemMetadataArray = null;

                if (inputMetadata != null && inputMetadata.Count > 0)
                {
                    metadataList = inputMetadata.ToArray();
                }
                //IMPORTANT!
                //Job related metadata (which will be used for running JAR & Transformation rules) should be part of ProcessRUlesRequest.
                rulesRequest.MetadataInputs = metadataList;

                //we can't use local path to DirectorWSA, because Express server(s) will not be able access those files - Director/Express can be on different machine
                // so, we are going to copy local file to unc path, specified in settings (with JobId subfolder)
                // DateTime startCopyDT = DateTime.Now;

                string subpath = jobId.ToString();
                if (!string.IsNullOrEmpty(inputsSubpath))
                {
                    subpath = inputsSubpath + "\\" + subpath;
                }
                List<string> uncInputPathList = CopyFilesToUNCPath(inputDocuments, subpath);
                JobDocumentsInfo jdInfo = BuildJobDocumentsInfo(uncInputPathList, jobId, outputExtension);

                watch.Start();
                SubmitReadyJobResponse beginTransResponse = jms.SubmitReadyJob(_DirectorSettings.AdminScopeId, jobKey, rulesRequest, jdInfo, systemMetadataArray);
                watch.Stop();


                if (beginTransResponse.ResultCode != 0)
                {
                    LogManager().Error("JobManagementService.SubmitReadyJob failed. Message: " + beginTransResponse.ResultMessage);
                }
                //continue, if we managed to BeginJobTransaction
                else
                {
                    jobId = beginTransResponse.JobId;
                    LogManager().Info(string.Format("JobManagementService.SubmitReadyJob completed successfully. JobId: {0}, BeginJobTransactionAccepted is {1}. Duration: {2} sec", beginTransResponse.JobId, beginTransResponse.BeginJobTransactionAccepted, ((double)watch.ElapsedMilliseconds) / 1000));

                }
            }
            catch (Exception ex)
            {
                LogManager().ErrorException("Submit Documents failed.", ex);
            }
            finally
            {


            }

            
            if (metadataList != null && metadataList.Length > 0)
            {
                metadataList = null;
            }
            if (inputMetadata != null && inputMetadata.Count > 0)
            {
                inputMetadata.Clear();
            }

            return jobId;

        }
        
        
        
        
        /// <summary>
        /// This call allows to pass already created Job Ticket (as text) to the Director for rendition.
        /// Other new style job tickets, which can be defined in Transformation rules will be ignored - only legacy job tickets will be used
        /// </summary>
        /// <param name=""></param>
        public bool SubmitJobTicket(string jobTicket)
        {
            bool result = false;
            string response = "";
            bool initialized = InitializeGenericConnector(ref response);
            if (_DirectorSettings.SubmitterComponentId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the Job Ticket. SubmitterComponentId can't be empty.");
                return result;
            }
            if (_DirectorSettings.AdminScopeId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the Job Ticket. AdminScopeId can't be empty.");
                return result;
            }
            if (_DirectorSettings.WorkspaceId == Guid.Empty)
            {

                LogManager().Error("Failed to submit the Job Ticket. Transformation WorkspaceId can't be empty.");
                return result;
            }

            Guid jobId = Guid.Empty;
            try
            {
                if (string.IsNullOrEmpty(jobTicket))
                {
                    //todo - log message
                }
                else
                {
                    List<Metadata> metadataList = new List<Metadata>();
                    //metadata name for legacy job ticket should match the following template - 'Adlib.Director.*XmlJobTicketTemplate*' or 'Adlib.SystemManager.*XmlJobTicketTemplate*'
                    Metadata metadata = new Metadata();
                    metadata.Name = MD_NAME_CUSTOM_JOB_TICKET_TEMPLATE;
                    metadata.Value = jobTicket;
                    metadataList.Add(metadata);
                    //This metadata triggers support only for legacy job tickets.
                    Metadata metadataFlag = new Metadata();
                    metadataFlag.Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_JOBSETTINGS_EXCLUSIVE_JOB_TICKET_TEMPLATE;
                    metadataFlag.Value = "true";
                    metadataList.Add(metadataFlag);
                    //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                    JobManagementServiceClient jms = GetJobManagementService();

                    JobKey jobKey = new JobKey();
                    jobKey.SubmitterComponentId = DirectorSettings.SubmitterComponentId;
                    jobKey.RepositoryId = DirectorSettings.RepositoryId;
                    jobKey.UserDefinedId = DirectorSettings.UserDefinedId;

                    ProcessRulesRequest rulesRequest = new ProcessRulesRequest();
                    List<RulesetDefinition> rulesDefs = new List<RulesetDefinition>();
                    rulesDefs = new List<RulesetDefinition>();

                    RulesetDefinition rulesDef = new RulesetDefinition();
                    rulesDef.ConfigurableEntityName = "JAR";
                    rulesDef.WorkspaceId = _DirectorSettings.WorkspaceId;
                    rulesDefs.Add(rulesDef);
                    rulesRequest.RulesDefinitions = rulesDefs.ToArray();
                    rulesRequest.MetadataInputs = metadataList.ToArray();

                    //Step 1 - Begin Job Transaction
                    BeginJobTransactionResponse beginTransResponse = jms.BeginJobTransaction(_DirectorSettings.AdminScopeId, jobKey, rulesRequest, null);
                    if (beginTransResponse.ResultCode != 0)
                    {
                        LogManager().Error("DirectorWSA.BeginJobTransaction failed. Message: " + beginTransResponse.ResultMessage);
                    }
                    else
                    {
                        jobId = beginTransResponse.JobId;
                        LogManager().Info(string.Format("DirectorWSA.BeginJobTransaction completed successfully. JobId: {0}, BeginJobTransactionAccepted is {1}", beginTransResponse.JobId, beginTransResponse.BeginJobTransactionAccepted));
                        //Step 2  - commit job. No file transfer or definition is required, because legacy job ticket should have inputs/outputs be already defined.
                        JobResponse commitJobResponse = jms.CommitJobTransaction(beginTransResponse.JobId, null);
                        if (commitJobResponse.ResultCode != 0)
                        {
                            LogManager().Error(string.Format("JobManagementService.CommitJobTransaction failed. JobId: {0}, Message: {1}", beginTransResponse.JobId, commitJobResponse.ResultMessage));
                        }
                        else
                        {
                            result = true;
                            LogManager().Info("JobManagementService.CommitJobTransaction completed successfully. JobId is " + commitJobResponse.JobId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager().ErrorException("Submit Job Ticket failed.", ex);
            }

            
          
            return result;

        }

        /// <summary>
        /// Cancells all specified jobs, if they can be cancelled (this depends on current JobStatus).
        /// </summary>
        /// <param name=""></param>
        public JobResponse[] CancelJobs(List<Guid> jobIds)
        {
            JobResponse[] jobResponses = null;

            try
            {
                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                JobManagementServiceClient jms = GetJobManagementService();
                jobResponses = jms.CancelJobs(jobIds.ToArray(), null);
                LogManager().Info("DirectorWSA.CancelJobs call completed successfully.");
            }
            catch (Exception e)
            {
                LogManager().ErrorException("DirectorWSA.CancelJobs call failed.", e);
            }

            return jobResponses;
        }
        public JobResponse[] ReleaseJobs(List<Guid> jobIds)
        {
            return ReleaseJobs(jobIds, null);
        }
        /// <summary>
        /// Releases all specified jobs, if they can be released (job should be in one of the Completed* state to be released).
        /// </summary>
        /// <param name=""></param>
        public JobResponse[] ReleaseJobs(List<Guid> jobIds, List<Metadata> metadataList)
        {
            JobResponse[] jobResponses = null;
            Metadata[][] list = new Metadata[1][];
            if (metadataList!= null && metadataList.Count > 0)
            {
                list[0] = metadataList.ToArray();
            }
            try
            {
                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                JobManagementServiceClient jms = GetJobManagementService();
                jobResponses = jms.ReleaseCompletedJobs(jobIds.ToArray(), list);
                LogManager().Info("DirectorWSA.ReleaseCompletedJobs call completed successfully.");
                if (jobResponses != null && jobResponses.Length > 0)
                {
                    foreach (JobResponse response in jobResponses)
                    {
                        if (response != null && response.ResultCode == 0)
                        {
                            RemoveJobDownloadRecordByJobId(response.JobId);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogManager().ErrorException("JobManagementService.ReleaseCompletedJobs call failed.", e);
            }
            return jobResponses;
        }

        /// <summary>
        /// Downloads output document(s) for specified JobId.
        /// File will be either copied from UNC path or downloaded through DirectorWSA.GetJobFiles call (MTOM)
        /// </summary>
        /// <param name=""></param>
        public bool DownloadOutputDocuments(Guid jobId)
        {
            JobDownloadRecord jobRecord = GetJobDownloadRecordByJobId(jobId);
            return DownloadOutputDocuments(jobId, jobRecord.UseFileRepository, 2);
        }

        /// <summary>
        /// Downloads output document(s) for specified JobId.
        /// File will be either copied from UNC path or downloaded through DirectorWSA.GetJobFiles call (MTOM)
        /// </summary>
        /// <param name=""></param>
        public bool DownloadOutputDocuments(Guid jobId, bool useFileRepository, double chunkSizeMb)
        {
            bool result = false;
            string outputPath = Path.Combine(DirectorSettings.OutputDocumentsPathAfterCompleted, jobId.ToString("N"));
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
            JobManagementServiceClient jms = GetJobManagementService();
            JobDetailResponse[] jobResponse = GetJobDetails(jms, new Guid[] { jobId });
            if (jobResponse != null && jobResponse.Length > 0)
            {
                if (jobResponse[0].JobDocumentsInfo != null &&
                    jobResponse[0].JobDocumentsInfo.DocumentOutputs != null &&
                    jobResponse[0].JobDocumentsInfo.DocumentOutputs.Length > 0)
                {
                    if (jobResponse[0].JobStatus != JobStatus.CompletedSuccessful)
                    {
                        LogManager().Warn(string.Format("Can't download outputs for this job: {0}, incorrect status: {1}:", jobResponse[0].JobId, jobResponse[0].JobStatus));
                        return false;
                    }
                    //Transfer files from FileRepository using DirectorWSA
                    if (useFileRepository)
                    {
                        try
                        {

                            bool xContinueFlag;
                            string rootFolder = jobResponse[0].JobDocumentsInfo.DocumentOutputs[0].Destinations[0].Folder;
                            List<string> actualFolderList = new List<string>();

                            foreach (DocumentOutput xDocOutput in jobResponse[0].JobDocumentsInfo.DocumentOutputs)
                            {
                                string subfolder = null;
                                string outputFilePath = outputPath;
                                JobFile[] xJobFileArray = new JobFile[1];
                                byte[][] xFileByteArray = new byte[1][];
                                long[] xFileLengthArray = new long[1];

                                xJobFileArray[0] = new JobFile();
                                xJobFileArray[0].FileName = xDocOutput.FileName;
                                xJobFileArray[0].Offset = 0;
                                xJobFileArray[0].BytesRead = Convert.ToInt32(chunkSizeMb * 1024 * 1024);

                                //need to find if file is in subfolder
                                if (xDocOutput.Destinations != null &&
                                    xDocOutput.Destinations.Length > 0 &&
                                    !string.IsNullOrEmpty(xDocOutput.Destinations[0].Folder))
                                {
                                    if (string.Compare(rootFolder, xDocOutput.Destinations[0].Folder, true) != 0)
                                    {
                                        int index = xDocOutput.Destinations[0].Folder.IndexOf(rootFolder);
                                        if (index >= 0 && rootFolder.Length + 1 < xDocOutput.Destinations[0].Folder.Length)
                                        {
                                            subfolder = xDocOutput.Destinations[0].Folder.Substring(rootFolder.Length, xDocOutput.Destinations[0].Folder.Length - rootFolder.Length);
                                            xJobFileArray[0].RelativeFolder = subfolder;
                                            //LogManager().Info("GetJobFiles subfolder is found: " + xJobFileArray[0].RelativeFolder);
                                            outputFilePath = System.IO.Path.Combine(outputPath, subfolder.Replace('/', '\\'));
                                            if (Directory.Exists(outputFilePath) == false)
                                            {
                                                Directory.CreateDirectory(outputFilePath);
                                            }
                                            //LogManager().Info("GetJobFiles path with subfolder: " + outputFilePath);
                                        }
                                    }
                                }

                                xFileLengthArray[0] = xDocOutput.FileLength;

                                //remove the file in case it already exists
                                foreach (JobFile xFile in xJobFileArray)
                                {
                                    if (File.Exists(Path.Combine(outputFilePath, xFile.FileName)))
                                        File.Delete(Path.Combine(outputFilePath, xFile.FileName));
                                }
                                actualFolderList.Add(outputFilePath);
                                do
                                {
                                    xContinueFlag = false;
                                    xFileByteArray = jms.GetJobFiles(jobId, xJobFileArray, null);

                                    if (xFileByteArray != null && xFileByteArray.Length > 0)
                                    {
                                        for (int x = 0; x < xJobFileArray.Length; x++)
                                        {
                                            LogManager().Info(string.Format("Downloading file {0} to {1} folder. JobId: {2}", xJobFileArray[x].FileName, outputFilePath, jobId.ToString()));

                                            using (FileStream fs = new FileStream(Path.Combine(outputFilePath, xJobFileArray[x].FileName), FileMode.Append, FileAccess.Write))
                                            {
                                                //append next set of bytes to file
                                                fs.Write(xFileByteArray[x], 0, xFileByteArray[x].Length);
                                                //increment number of bytes read
                                                xJobFileArray[x].Offset += xJobFileArray[x].BytesRead;
                                            }


                                            if (xJobFileArray[x].Offset < xFileLengthArray[x])
                                                xContinueFlag = true;

                                        }
                                    }
                                    else
                                    {
                                        LogManager().Error("Failed to download job outputs from File Repository. No Data was returned. JobId: " + jobId);
                                        break;
                                    }

                                } while (xContinueFlag);
                                result = true;

                            }
                            //files are complete, compare hashes
                            StringBuilder xResultString = new StringBuilder("<GetFilesResults>");

                            int i = 0;
                            foreach (DocumentOutput xDocOutput in jobResponse[0].JobDocumentsInfo.DocumentOutputs)
                            {
                                string xLocalLength;
                                string xServerHash = xDocOutput.FileHash;
                                string xServerLength = xDocOutput.FileLength.ToString();

                                byte[] xHash;
                                MD5CryptoServiceProvider xMd5Hasher = new MD5CryptoServiceProvider();

                                using (FileStream fs = new FileStream(Path.Combine(actualFolderList[i], xDocOutput.FileName), FileMode.Open, FileAccess.Read))
                                {
                                    xHash = xMd5Hasher.ComputeHash(fs);
                                    xLocalLength = fs.Length.ToString();
                                }
                                i++;
                                string xLocalHash = BitConverter.ToString(xHash).Replace("-", " ");

                                xResultString.Append(@"<File Name=""" + xDocOutput.FileName +
                                    @""" ServerMD5=""" + xServerHash +
                                    @""" ServerFileLength=""" + xServerLength +
                                    @""" LocalMD5=""" + xLocalHash +
                                    @""" LocalFileLength=""" + xLocalLength + @""" />");
                                if (string.Compare(xServerHash, xLocalHash) != 0)
                                {
                                    result = false;
                                }
                            }

                            xResultString.Append(@"</GetFilesResults>");
                            //todo - log result
                        }
                        catch (Exception err)
                        {
                            LogManager().ErrorException("Failed to download job outputs from File Repository. JobId: " + jobId, err);
                        }
                        if (result == true)
                        {
                            LogManager().Info(string.Format("Completed download of output document(s) for JobId: {0}, OutputPath: {1}", jobId, outputPath));
                        }
                        else
                        {
                            LogManager().Info(string.Format("Failed to download output document(s) for JobId: {0}, OutputPath: {1}. See log for details.", jobId, outputPath));
                        }
                    }
                    //Copy files from network location
                    else
                    {
                        result = true;
                        foreach (DocumentOutput xDocOutput in jobResponse[0].JobDocumentsInfo.DocumentOutputs)
                        {
                            try
                            {
                                string targetPath = Path.Combine(outputPath, xDocOutput.FileName);
                                if (File.Exists(targetPath))
                                {
                                    File.Delete(targetPath);
                                }
                                if (xDocOutput.Destinations[0].Type == OutputDestinationType.Folder)
                                {
                                    File.Copy(Path.Combine(xDocOutput.Destinations[0].Folder, xDocOutput.FileName), targetPath, true);
                                }
                                else //TODO - download from URI location
                                {
                                    Utilities.DownloadToLocalPath(Path.Combine(xDocOutput.Destinations[0].Folder, xDocOutput.FileName), targetPath);
                                }
                            }
                            catch (IOException ioException)
                            {
                                LogManager().ErrorException("Failed to copy job outputs. JobId: " + jobId, ioException);
                                result = false;
                            }
                            catch (Exception ex)
                            {
                                LogManager().ErrorException("Failed to copy job outputs. JobId: " + jobId, ex);
                                result = false;
                            }
                        }
                        if (result == true)
                        {
                            LogManager().Info(string.Format("Completed copying output document(s) for JobId: {0}, OutputPath: {1}", jobId, outputPath));
                        }
                        else
                        {
                            LogManager().Info(string.Format("Failed to copy output document(s) for JobId: {0}, OutputPath: {1}. See log for details.", jobId, outputPath));
                        }

                    }
                }
            }
            else
            {

            }
            return false;
        }

        /// <summary>
        /// Upload specified input files to Director File Repository using DirectorWSA.PutJobFiles call (MTOM)
        /// </summary>
        /// <param name=""></param>
        public bool UploadJobFiles(Guid jobId, List<string> inputDocPathList, double chunkSizeMb)
        {
            return UploadJobFiles(jobId, inputDocPathList, GetJobManagementService(), chunkSizeMb);
        }

        /// <summary>
        /// Defines input/output document(s) in DB, but doesn't copy them.
        /// Path to documents should not be local path, unless all Director and Express components are installed on the same machine.
        /// </summary>
        /// <param name=""></param>
        public bool DefineJobFiles(List<string> uncInputPathList, Guid jobId, string outputExtention)
        {
            return DefineJobFiles(uncInputPathList, jobId, outputExtention, GetJobManagementService());
        }

        public JobManagementService.JobManagementServiceClient GetJobManagementService()
        {

            if (_jms == null || string.Compare(_prevServiceAddress, this.DirectorSettings.JobManagementServiceUrl, true) != 0)
            {
                //LogManager().Info(string.Format("Initializing JMS. Endpoint: {0}", this.DirectorSettings.JobManagementServiceUrl));

                
                if (_jms != null)
                {
                    try
                    {
                        _jms.Close();
                        _jms = null;
                    }
                    catch (Exception) { }
                }
                
                BasicHttpBinding binding = new BasicHttpBinding();

                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = long.MaxValue;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MessageEncoding = WSMessageEncoding.Mtom;
                binding.SendTimeout = TimeSpan.FromMinutes(1);
                binding.OpenTimeout = TimeSpan.FromMinutes(1);
                binding.CloseTimeout = TimeSpan.FromMinutes(1);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
                binding.AllowCookies = false;
                binding.BypassProxyOnLocal = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.TextEncoding = System.Text.Encoding.UTF8;
                binding.TransferMode = TransferMode.Buffered;
                binding.UseDefaultWebProxy = true;

                binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                binding.ReaderQuotas.MaxDepth = int.MaxValue;
                binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

                EndpointAddress endPointAddress = new EndpointAddress(this.DirectorSettings.JobManagementServiceUrl);
                _prevServiceAddress = this.DirectorSettings.JobManagementServiceUrl;

                _jms = new JobManagementService.JobManagementServiceClient(binding, endPointAddress);

                _jms.Endpoint.Behaviors.Add(this.DirectorSettings.JobManagementServiceEndpointBehavior);
            }

            return _jms;
        }

        public JobManagementService.JobManagementServiceClient GetNotCachedJobManagementService()
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = long.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.SendTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;

            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            EndpointAddress endPointAddress = new EndpointAddress(this.DirectorSettings.JobManagementServiceUrl);
            _prevServiceAddress = this.DirectorSettings.JobManagementServiceUrl;

            JobManagementService.JobManagementServiceClient jms = new JobManagementService.JobManagementServiceClient(binding, endPointAddress);

            jms.Endpoint.Behaviors.Add(this.DirectorSettings.JobManagementServiceEndpointBehavior);

            return jms;
        }

        //private Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA GetDirectorWSA()
        //{
        //    Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = new Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA();
        //    if (string.Compare(directorWsa.Url, DirectorSettings.DirectorWSAUrl) != 0)
        //    {
        //        directorWsa.Url = DirectorSettings.DirectorWSAUrl;
        //    }
        //    directorWsa.Credentials = System.Net.CredentialCache.DefaultCredentials;
        //    directorWsa.Timeout = 600000;
        //    return directorWsa;
        //}

        private JobDetailResponse[] GetJobDetails(JobManagementServiceClient jms, Guid[] jobIds)
        {
            JobDetailResponse[] response = jms.GetJobsDetail(jobIds, null);
            return response;
        }

        private bool UploadJobFiles(Guid jobId, List<string> inputDocPathList, JobManagementServiceClient jms, double chunkSizeMb)
        {
            int xNumDocs = inputDocPathList.Count;
            bool resultFlag = true;
            JobFilesResponse jobFileResponse = null;
            //for return status
            StringBuilder xResultString = new StringBuilder();
            xResultString.Append("<Results>");

            try
            {
                //set up job file collection list, and job file buffer collection list
                List<JobFile> xJobFileList = new List<JobFile>(xNumDocs);
                List<FileInfo> fiInputDocList = new List<FileInfo>();
                foreach (string inputDocPath in inputDocPathList)
                {
                    FileInfo xInputFileInfo = new FileInfo(inputDocPath);
                    if (xInputFileInfo.Exists)
                    {
                        fiInputDocList.Add(xInputFileInfo);
                    }
                }
                foreach (FileInfo xInputFileInfo in fiInputDocList)
                {
                    long xFileSize = xInputFileInfo.Length;

                    JobFile xJobFile = new JobFile();
                    xJobFile.FileName = xInputFileInfo.Name;
                    xJobFile.Offset = 0;
                    xJobFile.BytesRead = 0;

                    byte[] xJobFileBuffer;

                    //Entire file or chunks?
                    if (chunkSizeMb == 0 || chunkSizeMb * 1024 * 1024 > xFileSize)
                    {
                        //entire file
                        xJobFileBuffer = new byte[xInputFileInfo.Length];
                    }
                    else
                    {
                        //chunk
                        xJobFileBuffer = new byte[Convert.ToInt64(Convert.ToDouble(chunkSizeMb) * 1024 * 1024)];
                    }
                    //xJobFileBuffers.Add(xJobFileBuffer);

                    bool continueFlag = false;
                    jobFileResponse = null;
                    using (FileStream fs = File.OpenRead(xInputFileInfo.FullName))
                    {
                        do
                        {
                            xJobFile.Offset += xJobFile.BytesRead;

                            if (xJobFile.Offset < xFileSize)
                            {
                                //read next set of bytes into buffer and get bytes read
                                fs.Position = xJobFile.Offset;
                                xJobFile.BytesRead = fs.Read(xJobFileBuffer, 0, xJobFileBuffer.Length);

                                if (xJobFile.Offset + xJobFile.BytesRead < xFileSize)
                                {
                                    continueFlag = true;
                                }
                                else
                                {
                                    continueFlag = false;
                                }
                            }
                            jobFileResponse = jms.PutJobFiles(jobId, new JobFile[] { xJobFile }, new byte[][] { xJobFileBuffer }, null);
                            if (!jobFileResponse.SendJobFilesSuccessful)
                            {
                                LogManager().Error(string.Format("Call JobManagementService.PutJobFiles failed. JobId: {0}, Message: {1}", jobId, jobFileResponse.ResultMessage));
                                break;
                            }
                        }
                        while (continueFlag);
                        fs.Close();

                        resultFlag &= jobFileResponse.SendJobFilesSuccessful;
                    }
                    xJobFileList.Add(xJobFile);

                }
                if (jobFileResponse != null && resultFlag == true)
                {
                    JobDetailResponse[] xDetailResponse = jms.GetJobsDetail(new Guid[1] { jobId }, null);
                    if (xDetailResponse != null && xDetailResponse.Length > 0)
                    {
                        string xOutputLocation = xDetailResponse[0].JobDocumentsInfo.DocumentOutputs[0].Destinations[0].Folder;

                        xResultString.Append("<JobFilesResponse SendJobFilesSuccess=\"" + jobFileResponse.SendJobFilesSuccessful.ToString()
                            + "OutputURI=\"" + xOutputLocation
                            + "\"/>");


                        //do hash checks to see if files where uploaded correctly
                        for (int i = 0; i < xJobFileList.Count; i++)
                        {
                            //checksum test
                            byte[] xHashBytes;
                            MD5CryptoServiceProvider xMD5Hasher = new MD5CryptoServiceProvider();
                            using (FileStream fs = new FileStream(fiInputDocList[i].FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                                xHashBytes = xMD5Hasher.ComputeHash(fs);

                            string xLocalHash = BitConverter.ToString(xHashBytes);

                            //the server hash is updated on the first GetJobsDetail call, but only if the hash value is blank
                            string xServerHash = xDetailResponse[0].JobDocumentsInfo.DocumentInputs[i].FileHash;

                            xResultString.Append(@"<File Name=""" + xJobFileList[i].FileName +
                                                    @""" LocalHash=""" + xLocalHash +
                                                    @""" ServerHash=""" + xServerHash +
                                                    @""" HashType=""MD5"" />");

                        }
                        xResultString.Append("</Results>");
                        //todo - log results
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager().ErrorException("Failed to upload job inputs to File Repository. JobId: " + jobId, ex);
            }

            return resultFlag;
        }

        private JobDocumentsInfo BuildJobDocumentsInfo(List<string> uncInputPathList, Guid jobId, string outputExtention)
        {
            if (!string.IsNullOrEmpty(outputExtention))
            {
                if (!outputExtention.StartsWith("."))
                {
                    outputExtention = "." + outputExtention;
                }
            }
            JobDocumentsInfo jdInfo = new Adlib.Director.DirectorWSAWrapper.JobManagementService.JobDocumentsInfo();
            List<DocumentInput> inputList = new List<DocumentInput>();
            DocumentOutput docOutput = new Adlib.Director.DirectorWSAWrapper.JobManagementService.DocumentOutput();
            for (int i = 0; i < uncInputPathList.Count; i++)
            {
                string filePath = uncInputPathList[i];
                FileInfo inputFileInfo = new FileInfo(filePath);
                if (inputFileInfo.Exists)
                {
                    //we need to define one output, for this here as example we are going to use first input file name
                    if (i == 0)
                    {
                        docOutput.FileName = inputFileInfo.Name + outputExtention;
                        docOutput.Destinations = new Adlib.Director.DirectorWSAWrapper.JobManagementService.Destination[1];
                        docOutput.Destinations[0] = new Adlib.Director.DirectorWSAWrapper.JobManagementService.Destination();
                        docOutput.Destinations[0].Type = Adlib.Director.DirectorWSAWrapper.JobManagementService.OutputDestinationType.Folder;
                        docOutput.Destinations[0].Folder = Path.Combine(inputFileInfo.DirectoryName, "Output");
                    }

                    DocumentInput docIn = new DocumentInput();
                    docIn.FileName = inputFileInfo.Name;
                    docIn.Folder = inputFileInfo.DirectoryName;
                    docIn.FileLength = inputFileInfo.Length;
                    inputList.Add(docIn);

                }
            }
            jdInfo.DocumentOutputs = new DocumentOutput[] { docOutput };
            jdInfo.DocumentInputs = inputList.ToArray();

            return jdInfo;
        }

        private bool DefineJobFiles(List<string> uncInputPathList, Guid jobId, string outputExtention, JobManagementServiceClient jms)
        {
            bool resultFlag = false;
            JobDetailResponse[] xDetailResponse = jms.GetJobsDetail(new Guid[1] { jobId }, null);
            try
            {
                if (xDetailResponse != null && xDetailResponse.Length > 0)
                {
                    JobDocumentsInfo jdInfo = BuildJobDocumentsInfo(uncInputPathList, jobId, outputExtention);
                    JobFilesResponse jfResp = jms.DefineJobFiles(jobId, jdInfo, null);
                    if (jfResp.SendJobFilesSuccessful == false)
                    {
                        resultFlag = false;
                        LogManager().Error(string.Format("Failed to call DirectorWsa.DefineJobFiles. JobId: {0}, Message: {1}", jobId, jfResp.ResultMessage));
                        //todo log message here
                    }
                    else
                    {
                        resultFlag = true;
                    }
                    jdInfo = null;
                    jfResp = null;
                }
            }
            catch (Exception ex)
            {
                LogManager().ErrorException("Failed to define job inputs. JobId: " + jobId, ex);
            }

            return resultFlag;
        }
        private List<string> CopyFilesToUNCPath(List<string> pathList, string subPath)
        {
            List<string> uncPathList = new List<string>();

            if (Directory.Exists(DirectorSettings.CustomUncPathForInputDocs) == false)
            {
                throw new Exception(string.Format("Unc path for Input Documents doesn't exist. Can't continue. Path: {0}", DirectorSettings.CustomUncPathForInputDocs));
            }
            string targetPath = Path.Combine(DirectorSettings.CustomUncPathForInputDocs, subPath);
            if (Directory.Exists(targetPath) == false)
            {
                Directory.CreateDirectory(targetPath);
            }
            string targetFilePath = null;
            foreach (string filePath in pathList)
            {
                try
                {
                    FileInfo fi = new FileInfo(filePath);
                    targetFilePath = Path.Combine(targetPath, fi.Name);

                    if (_DirectorSettings.UseDirectorFileManagementCall)
                    {
                        //following code snippet shows how to use DirectorWSA.ExecuteCOmmand->COpyFIle functionality.
                        //Important - source and destination path should be valid for DirectorWSA, meaning
                        // if all components are on the same machine - path can be local, 
                        // otherway it should be UNC.

                        JobManagementServiceClient jms = GetJobManagementService();
                        List<Metadata> metadataInputs = new List<Metadata>();
                        metadataInputs.Add(new JobManagementService.Metadata());
                        metadataInputs[0].Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_EXECUTE_COMMAND_FILE_MANAGEMENT_COMMAND;
                        metadataInputs[0].Value = "CopyFile";
                        if (_DirectorSettings.MoveInputFileToUncFolder)
                        {
                            metadataInputs[0].Value = "MoveFile";
                        }

                        metadataInputs.Add(new JobManagementService.Metadata());
                        metadataInputs[1].Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_EXECUTE_COMMAND_FILE_MANAGEMENT_SOURCE;
                        metadataInputs[1].Value = filePath;
                        metadataInputs.Add(new JobManagementService.Metadata());
                        metadataInputs[2].Name = Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_EXECUTE_COMMAND_FILE_MANAGEMENT_TARGET;
                        metadataInputs[2].Value = targetFilePath;
                        ExecuteCommandResponse response = jms.ExecuteCommand(Adlib.Director.Definitions.ExecuteCommandType.FileManagement.ToString(), metadataInputs.ToArray());

                    }
                    else
                    {
                        if (_DirectorSettings.MoveInputFileToUncFolder)
                        {
                            fi.MoveTo(targetFilePath);
                        }
                        else
                        {
                            fi.CopyTo(targetFilePath, true);
                        }
                    }
                    uncPathList.Add(targetFilePath);
                }
                catch (IOException ioException)
                {
                    if (_DirectorSettings.MoveInputFileToUncFolder)
                    {
                        LogManager().ErrorException(string.Format("Failed to move input file to UNC location. Target path: {0}", targetFilePath), ioException);
                    }
                    else
                    {
                        LogManager().ErrorException(string.Format("Failed to copy input file to UNC location. Target path: {0}", targetFilePath), ioException);
                    }
                    throw;
                }

            }
            return uncPathList;
        }
        private List<string> ReplaceLocalPathWithUNCPath(List<string> pathList)
        {
            List<string> uncPathList = new List<string>();
            string localHostName = Dns.GetHostName();
            int index = 0;
            string driveLetter = null;
            string uncPath = null;
            foreach (string filePath in pathList)
            {
                FileInfo fi = new FileInfo(filePath);
                if (filePath.Length > 2 && filePath.Substring(0, 2) == @"\\")
                {
                    uncPathList.Add(filePath);
                }
                else
                {
                    index = filePath.IndexOf(@":\");
                    if (filePath.Length > 3 && index > 0)
                    {
                        driveLetter = filePath.Substring(0, index);
                        uncPath = @"\\" + localHostName + @"\" + driveLetter + @"$\" + filePath.Substring(index + 2, filePath.Length - index - 2);
                        uncPathList.Add(uncPath);
                    }
                }

            }
            return uncPathList;
        }


        private JobDownloadRecord GetJobDownloadRecordByJobId(Guid jobId)
        {
            JobDownloadRecord resultRecord = new JobDownloadRecord(jobId, false);
            foreach (JobDownloadRecord record in _SubmittedJobs)
            {
                if (record != null && record.JobId == jobId)
                {
                    resultRecord = record;
                    break;
                }
            }
            return resultRecord;
        }
        private bool RemoveJobDownloadRecordByJobId(Guid jobId)
        {
            bool result = false;
            foreach (JobDownloadRecord record in _SubmittedJobs)
            {
                if (record != null && record.JobId == jobId)
                {
                    _SubmittedJobs.Remove(record);
                    result = true;
                    break;
                }
            }
            return result;
        }


        public bool InitializeGenericConnector(ref string response)
        {
            return InitializeGenericConnector(false, ref response);
        }

        //This function call ProcessInitRules to initialize component.
        // It is required for every connector to initialize prior to submitting jobs, because 
        // 
        public bool InitializeGenericConnector(bool force, ref string responseStr)
        {
            if (_DirectConnectorIsInitializedOnce == true && force == false)
            {
                return true;
            }
            bool result = false;

            try
            {
                //Adlib.Director.DirectorWSAWrapper.DirectorWSA.DirectorWSA directorWsa = GetDirectorWSA();
                JobManagementServiceClient jms = GetJobManagementService();
                InitRulesRequest request = new InitRulesRequest();
                List<Metadata> metadataInputs = new List<Metadata>();
                Metadata componentName = new Metadata();
                metadataInputs.Add(new Metadata());
                metadataInputs[0].Name = Director.Definitions.Consts.CommonMetadataNames.MD_CONFIG_ENTITY_NAME;
                metadataInputs[0].Value = Adlib.Director.Definitions.Enums.ConfigurableEntityNameType.GenericConnector.ToString();
                metadataInputs.Add(new Metadata());
                Guid machineId = Adlib.Common.AdlibConfig.Instance().MachineId;
                metadataInputs[1].Name = Director.Definitions.Consts.CommonMetadataNames.MD_MACHINE_ID;
                metadataInputs[1].Value = machineId.ToString();

                //Uri xPathUri = new Uri(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
                //string xLocation = Path.GetDirectoryName(xPathUri.LocalPath);
                string xLocation = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

                LogManager().Info("GenericConnector InstallPath is " + xLocation);
#if (!DEBUG)

                 if (!string.IsNullOrEmpty(xLocation))
                 {
                     metadataInputs.Add(new Adlib.Director.DirectorWSAWrapper.JobManagementService.Metadata());
                     metadataInputs[2].Name = Director.Definitions.Consts.CommonMetadataNames.MD_INSTALLED_COMPONENT_INSTALL_PATH;
                     metadataInputs[2].Value = xLocation;
                 }
#else
                /*
                 if (!string.IsNullOrEmpty(xLocation))
                 {
                     metadataInputs.Add(new Adlib.Director.DirectorWSAWrapper.DirectorWSA.Metadata());
                     metadataInputs[2].Name = Director.Definitions.Consts.CommonMetadataNames.MD_INSTALLED_COMPONENT_INSTALL_PATH;
                     metadataInputs[2].Value = xLocation;
                 }*/
                //debug&unit tests
#endif
                request.MetadataInputs = metadataInputs.ToArray();
                request.ConfigurableEntityName = Adlib.Director.Definitions.Enums.ConfigurableEntityNameType.GenericConnector.ToString(); ;

                ProcessRulesResponse response = jms.ProcessInitRules(request, null);

                responseStr = Adlib.Common.SerializationUtilities.SerializeObject(response, typeof(ProcessRulesResponse));

                if (_DirectorSettings.JobSettingsWorkspaceIdList != null)
                {
                    _DirectorSettings.JobSettingsWorkspaceIdList.Clear();
                }
                _DirectorSettings.WorkspaceId = Guid.Empty;
                _DirectorSettings.AdminScopeId = Guid.Empty;
                _DirectorSettings.SubmitterComponentId = Guid.Empty;
                _DirectorSettings.RepositoryId = Guid.Empty;
                if (response != null)
                {
                    if (response.ResultCode == 0)
                    {
                        _DirectConnectorIsInitializedOnce = true;
                    }

                    if (response.MetadataOutputs != null && response.MetadataOutputs.Length > 0)
                    {
                        foreach (Metadata md in response.MetadataOutputs)
                        {
                            if (md != null && !string.IsNullOrEmpty(md.Name) && !string.IsNullOrEmpty(md.Value))
                            {
                                if (string.Compare(md.Name, Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_DIRECT_CONNECTOR_SETTINGS) == 0)
                                {
                                    try
                                    {

                                        Adlib.Common.Schema.Types.Connectors.GenericConnectorSettings directSettings = null;
                                        directSettings = (Adlib.Common.Schema.Types.Connectors.GenericConnectorSettings)Adlib.Common.SerializationUtilities.DeserializeObject(md.Value, typeof(Adlib.Common.Schema.Types.Connectors.GenericConnectorSettings), Adlib.Common.Const.AdlibNamespace);
                                        _SettingsString = md.Value;
                                        if (directSettings != null && directSettings.RepositoryList != null)
                                        {
                                            foreach (Adlib.Common.Schema.Types.Connectors.RepositoryType repo in directSettings.RepositoryList)
                                            {
                                                if (repo != null)
                                                {
                                                    _DirectorSettings.JobSettingsWorkspaceIdList.Add(new RepositoryInfo(repo));
                                                }
                                            }
                                        }

                                        if (_DirectorSettings.JobSettingsWorkspaceIdList != null &&
                                            _DirectorSettings.JobSettingsWorkspaceIdList.Count > 0 &&
                                            _DirectorSettings.JobSettingsWorkspaceIdList[0] != null)
                                        {
                                            _DirectorSettings.WorkspaceId = _DirectorSettings.JobSettingsWorkspaceIdList[0].TransformationWorkspaceId;
                                            _DirectorSettings.RepositoryId = _DirectorSettings.JobSettingsWorkspaceIdList[0].RepositoryId;
                                            _DirectConnectorIsInitializedOnce = true;
                                            if (response.ResultCode == 0)
                                            {
                                                result = true;
                                            }
                                            LogManager().Info("Transformation WorkspaceId was loaded from MRE: " + _DirectorSettings.WorkspaceId);
                                        }
                                    }
                                    catch { }
                                }
                                if (string.Compare(md.Name, Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_ADMIN_SCOPE_ID) == 0)
                                {
                                    try
                                    {
                                        _DirectorSettings.AdminScopeId = new Guid(md.Value);
                                        LogManager().Info("AdminScopeId was loaded from MRE: " + _DirectorSettings.AdminScopeId);
                                    }
                                    catch { }
                                }
                                if (string.Compare(md.Name, Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_INSTALLED_COMPONENT_ID) == 0)
                                {
                                    try
                                    {
                                        _DirectorSettings.SubmitterComponentId = new Guid(md.Value);
                                        LogManager().Info("InstalledComponentId was loaded from MRE: " + _DirectorSettings.SubmitterComponentId);
                                        if (response.ResultCode == 0)
                                        {
                                            result = true;
                                        }
                                    }
                                    catch { }
                                }
                                //Generic COnnector should not log directly to DB, it should use
                                // services call to log message. This will be implemented later.
                                /*
                                if (string.Compare(md.Name, Adlib.Director.Definitions.Consts.CommonMetadataNames.MD_ELS_LOG_SETTINGS) == 0)
                                {
                                    List<ILogger> loggers = LogManager().GetLoggers();
                                    if (loggers != null && loggers.Count > 0 && md.Value !=null && md.Value.Length > 0)
                                    {
                                        foreach (ILogger logger in loggers)
                                        {
                                            if (logger!= null && logger is ElsLogger)
                                            {
                                                ElsLogger.InitLogger(md.Value);
                                            }
                                        }
                                    }
                                }
                                */
                            }
                        }

                    }
                }

                //no need to heartbeat now.
                //StartHeartbeating();
            }
            catch (Exception e)
            {
                LogManager().ErrorException("Failed to initialize GenericConnector(SampleApp).", e);
                result = false;
            }
            if (result == false)
            {
                LogManager().Warn("Couldn't locate WorkspaceId for GenericConnector(SampleApp). WorkspaceId can be set on Settings page.");
                //_WorkspaceId = Guid.Empty;
            }
            return result;
        }

        public void StopHeartbeating()
        {
            if (_HeartbeatThreadHandler != null)
            {
                _HeartbeatThreadHandler.StopHeartbeating();
                _HeartbeatThreadHandler = null;
            }
        }

        private void StartHeartbeating()
        {
            try
            {
                if (_HeartbeatThreadHandler == null)
                {
                    if (_DirectorSettings != null && _DirectorSettings.SubmitterComponentId != Guid.Empty)
                    {

                        _HeartbeatThreadHandler = new HeartbeatThread();

                        ComponentHeartbeat heartbeat = new ComponentHeartbeat(_DirectorSettings.SubmitterComponentId);
                        heartbeat.ComponentInitData = _SettingsString;
                        heartbeat.Status = Adlib.Director.Definitions.Enums.ComponentHeartbeatStatusType.Initialized;
                        _HeartbeatThreadHandler.Initialize(heartbeat, 0);
                        _HeartbeatThreadHandler.StartHearbeating();

                        LogManager().Info("Heartbeating thread started.");
                    }
                    else
                    {
                        LogManager().Error("Failed to start heartbeating service, because settings are missing.");
                    }
                }

            }
            catch (Exception ee)
            {
                LogManager().ErrorException("Failed to start Heartbeating thread.", ee);

            }
        }
        public bool Unregister(string userName, string password)
        {
            bool result = false;
            try
            {
                JobManagementServiceClient jms = GetJobManagementService();
                UnregisterComponentRequest request = new UnregisterComponentRequest()
                {
                    UserDetails = new UserDetails()
                    {
                        Username = userName,
                        Password = password
                    },
                    ComponentId = _DirectorSettings.SubmitterComponentId,
                    AdministrativeScopeId = _DirectorSettings.AdminScopeId
                };


                UnregisterComponentResponse response = jms.UnregisterComponent(request);
                if (response.ResultCode != 0)
                {
                    _LogMngr.Error("Failed to Unregister component: " + response.ResultMessage);
                    result = false;
                }
                else
                {
                    _LogMngr.Info("Unregistration completed.");
                    _DirectorSettings.UserName = "";
                    _DirectorSettings.UserPassword = "";
                    _DirectorSettings.AdminScopeId = Guid.Empty;
                    _DirectorSettings.JobSettingsWorkspaceIdList = new List<RepositoryInfo>();
                    _DirectorSettings.RepositoryId = Guid.Empty;
                    _DirectorSettings.SubmitterComponentId = Guid.Empty;
                    _DirectorSettings.WorkspaceId = Guid.Empty;
                    result = true;
                }
            }
            catch (Exception e)
            {
                _LogMngr.Error("Failed to unregister component. Error: " + e.Message);
            }


            return result;
        }
        public bool Register(string userName, string password)
        {
            return Register(userName, password, null);
        }
        public bool Register(string userName, string password, string friendlyName)
        {
            bool result = false;
            Guid machineId = Guid.Empty;
            bool newMachineIdWasCreated = false;
            Guid adminScopeId = Guid.Empty;


            if (Adlib.Common.AdlibConfig.Instance() != null)
            {
                machineId = Adlib.Common.AdlibConfig.Instance().MachineId;
            }
            if (machineId == Guid.Empty)
            {
                machineId = Guid.NewGuid();
                newMachineIdWasCreated = true;
            }


            try
            {
                JobManagementServiceClient jms = GetJobManagementService();
                RegisterComponentRequest request = new RegisterComponentRequest()
                {
                    UserDetails = new UserDetails()
                    {
                        Username = userName,
                        Password = password
                    },
                    Machine = new MachineInfo()
                    {
                        Id = machineId,
                        Name = System.Net.Dns.GetHostName(),
                        NetworkName = System.Environment.UserDomainName,
                        Ip = GetLocalIpAddress()
                    },
                    Component = new ComponentInfo()
                    {
                        InstallPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
                        ComponentType = GenericConnectorType,
                        Version = this.AssemblyVersion
                    }
                };

                if (!string.IsNullOrEmpty(friendlyName))
                {
                    request.MetadataInputs = new Metadata[1];
                    request.MetadataInputs[0] = new Metadata()
                    {
                        Name = MD_INSTALLED_COMPONENT_FRIENDLY_NAME,
                        Value = friendlyName
                    };
                }

                RegisterComponentResponse response = jms.RegisterComponent(request);
                if (response.ResultCode != 0)
                {
                    _LogMngr.Error("Failed to Register component: " + response.ResultMessage);
                    return false;
                }
                else
                {
                    _LogMngr.Info(string.Format("Registration completed. MachineId: {0}, ComponentId: {1}", machineId, response.ComponentId));
                    _DirectorSettings.SubmitterComponentId = response.ComponentId;
                    _DirectorSettings.AdminScopeId = response.AdministrativeScopeId;
                    result = true;
                    _DirectorSettings.UserName = userName;
                    _DirectorSettings.UserPassword = password;
                    //need to save machineId to config file, if it is newly generated
                    if (newMachineIdWasCreated)
                    {
                        UpdateAdlibConfigMachineId(machineId);
                    }
                }
            }
            catch (Exception e)
            {
                _LogMngr.Error("Failed to register component. Error: " + e.Message);
            }
            return result;
        }

        public static string GetLocalIpAddress()
        {
            try
            {
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in localIPs)
                {
                    // is localhost
                    if (hostIP.AddressFamily.ToString() == "InterNetwork")
                        return hostIP.ToString();
                }
            }
            catch { }
            return null;
        }

        private string AssemblyVersion
        {
            get
            {
                return "V" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()
                    + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString()
                    + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

            }
        }

        private string GetAssemblyVersion(Assembly assembly)
        {
            string result = string.Empty;
            if (assembly != null)
            {
                result = "V" + assembly.GetName().Version.Major.ToString()
                    + assembly.GetName().Version.Minor.ToString()
                    + assembly.GetName().Version.Build.ToString();
            }
            return result;
        }

        private void UpdateAdlibConfigMachineId(Guid machineId)
        {
            bool fileUpdated = false;
            string fileName = null;
            if (Adlib.Common.AdlibConfig.Instance() != null)
            {
                string path = Adlib.Common.AdlibConfig.Instance().AdlibConfigurationFileLocation;
                if (!string.IsNullOrEmpty(path))
                {
                    fileName = Path.Combine(path, "Adlib.config");
                }
            }
            if (!string.IsNullOrEmpty(fileName) ||
                File.Exists(fileName) == true)
            {
                try
                {
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(fileName);
                    XmlNode node = xDoc.DocumentElement;
                    if (node != null && node.ChildNodes != null)
                    {
                        bool nodefound = false;
                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            if (childNode != null)
                            {
                                if (childNode is XmlElement && childNode.Name == "MachineId")
                                {
                                    childNode.InnerText = machineId.ToString();
                                    fileUpdated = true;
                                    break;
                                }
                            }
                        }
                        if (!nodefound)
                        {
                            XmlNode newNode = xDoc.CreateNode(XmlNodeType.Element, "MachineId", null);
                            newNode.InnerText = machineId.ToString(); ;

                            node.AppendChild(newNode);
                        }
                    }
                    xDoc.Save(fileName);
                    //need force to reload config file
                    Adlib.Common.AdlibConfig.Instance(true);
                }
                catch (Exception ee)
                {
                    _LogMngr.Error(string.Format("Failed to save new machineId ({0}) to Adlib.config file: {1}. Error: {2}.", machineId, fileName, ee.Message));
                }
            }
            if (fileUpdated == false)
            {
                _LogMngr.Error(string.Format("Failed to save new machineId ({0}) to Adlib.config file: {1}.", machineId, fileName));
            }
            else
            {
                _LogMngr.Error(string.Format("Adlib.config was updated with new machineId ({0}). Adlib.config file: {1}.", machineId, fileName));
            }

        }

        //we cache some jobs for download feature, but we are going to cache no more then 100 jobs, so memory doesn't grow
        private void AddJobToCache(JobDownloadRecord jobRecord)
        {
            if (_SubmittedJobs != null)
            {
                if (_SubmittedJobs.Count > 100)
                {
                    //clean up array so it doesn't grow huge
                    _SubmittedJobs.RemoveRange(1, 10);
                }
                _SubmittedJobs.Add(jobRecord);
            }
        }
    }
}
