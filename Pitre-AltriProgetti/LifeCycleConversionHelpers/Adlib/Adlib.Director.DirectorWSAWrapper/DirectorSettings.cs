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
using System.Xml.Serialization;


namespace Adlib.Director.DirectorWSAWrapper
{
    public class RepositoryInfo
    {
        private string _Name;
        private Guid _TransformationWorkspaceId = Guid.Empty;
        private Guid _RepositoryId = Guid.Empty;

        public Guid RepositoryId
        {
            get { return _RepositoryId; }
            set { _RepositoryId = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public Guid TransformationWorkspaceId
        {
            get { return _TransformationWorkspaceId; }
            set { _TransformationWorkspaceId = value; }
        }

        public RepositoryInfo()
        {

        }
        public RepositoryInfo(string name, Guid id)
        {
            _Name = name;
            _TransformationWorkspaceId = id;
        }
        public RepositoryInfo(Adlib.Common.Schema.Types.Connectors.RepositoryType repo)
        {
            if (repo != null)
            {
                _Name = repo.Name;
                _TransformationWorkspaceId = repo.TransformationWorkspaceId;
                _RepositoryId = repo.RepositoryId;
            }
        }
    }
   [Serializable()]
   public class DirectorSettings
   {
       private const string OutputPathSubFolder = "DirectorSampleAppJobsOutput";
       private const string JobManagementServiceSiteRelativePath = "/JobManagementService/JobManagementService.svc";
       private string AdlibRootSiteUrl = "http://" + System.Net.Dns.GetHostName() + "/Adlib";
       
       private string _jobManagementServiceUrl = null;

       private Guid _SubmitterComponentId = Guid.Empty;
       private Guid _RepositoryId = Guid.Empty;
       private string _UserDefinedId;
       private string _OutputDocumentsPathAfterCompleted = null;
       private string _CustomUncPathForInputDocs = @"\\" + System.Net.Dns.GetHostName() + @"\AdlibJobFiles";
       private bool _FirstStart = true;
       private Guid _AdminScopeId = Guid.Empty;
       private bool _UseObjectLock = false;
       private Guid _WorkspaceId = Guid.Empty;
       private bool _ExcapeMetadata = true;
       private bool _MoveInputFileToUncFolder = false;
       private bool _UseDirectorFileManagementCall = false;
       private string _UserName;
       private string _UserPassword;
       private JobManagementServiceEndpointBehavior _jobManagementServiceEndpointBehavior;

       [XmlIgnore]
       public JobManagementServiceEndpointBehavior JobManagementServiceEndpointBehavior
       {
           get
           {
               if (this._jobManagementServiceEndpointBehavior == null)
               {
                   this._jobManagementServiceEndpointBehavior = new JobManagementServiceEndpointBehavior();
               }

               return this._jobManagementServiceEndpointBehavior;
           }
       }

       [XmlIgnore]
       public string UserName
       {
           get { return _UserName; }
           set { _UserName = value; }
       }

       [XmlIgnore]
       public string UserPassword
       {
           get { return _UserPassword; }
           set { _UserPassword = value; }
       }
       [XmlIgnore]
       public bool UseDirectorFileManagementCall
       {
           get { return _UseDirectorFileManagementCall; }
           set { _UseDirectorFileManagementCall = value; }
       }

       [XmlIgnore]
       public bool MoveInputFileToUncFolder
       {
           get { return _MoveInputFileToUncFolder; }
           set { _MoveInputFileToUncFolder = value; }
       }

       public bool ExcapeMetadata
       {
           get { return _ExcapeMetadata; }
           set { _ExcapeMetadata = value; }
       }

       private List<RepositoryInfo> _JobSettingsWorkspaceIdList = new List<RepositoryInfo>();

       //this setting will be loaded from DB during component initialization.
       [XmlIgnore]
       public List<RepositoryInfo> JobSettingsWorkspaceIdList
       {
           get 
           {
               if (_JobSettingsWorkspaceIdList == null)
               {
                   _JobSettingsWorkspaceIdList = new List<RepositoryInfo>();
               }
               return _JobSettingsWorkspaceIdList; 
           }
           set { _JobSettingsWorkspaceIdList = value; }
       }

       public DirectorSettings() : this(null) { }

       public DirectorSettings(DirectorSettings copySettings)
       {
           this._jobManagementServiceEndpointBehavior = new JobManagementServiceEndpointBehavior();
           OutputDocumentsPathAfterCompleted = null;
           Update(copySettings);
       }

       ~DirectorSettings()
       {
           this._jobManagementServiceEndpointBehavior = null;
       }
       
       public void Update(DirectorSettings copySettings)
       {
           if (copySettings != null)
           {
               JobManagementServiceUrl = copySettings.JobManagementServiceUrl;
               //ApplicationId = copySettings.ApplicationId;
               //ApplicationVariationState = copySettings.ApplicationVariationState;
               SubmitterComponentId = copySettings.SubmitterComponentId;
               RepositoryId = copySettings.RepositoryId;
               UserDefinedId = copySettings.UserDefinedId;
               OutputDocumentsPathAfterCompleted = copySettings.OutputDocumentsPathAfterCompleted;
               CustomUncPathForInputDocs = copySettings.CustomUncPathForInputDocs;
               FirstStart = copySettings.FirstStart;
               AdminScopeId = copySettings.AdminScopeId;
               UseObjectLock = copySettings.UseObjectLock;
               WorkspaceId = copySettings.WorkspaceId;
               ExcapeMetadata = copySettings.ExcapeMetadata;
           }
       }

       //this parameter will be generated from services base, loaded from Adlib.Config file + actual web-service name.
       [XmlIgnore]
       public string JobManagementServiceUrl
       {
           get
           {
               if (string.IsNullOrEmpty(_jobManagementServiceUrl))
               {
                   try
                   {
                       if (Adlib.Common.AdlibConfig.Instance() != null &&
						   !string.IsNullOrEmpty(Adlib.Common.AdlibConfig.Instance().JobManagementServiceUrl))
                       {
                           _jobManagementServiceUrl = Adlib.Common.AdlibConfig.Instance().JobManagementServiceUrl + JobManagementServiceSiteRelativePath;
                       }
                   }
                   catch (Exception)
                   {
                       
                   }
                   finally
                   {
                       if (string.IsNullOrEmpty(_jobManagementServiceUrl))
                       {
                           _jobManagementServiceUrl = AdlibRootSiteUrl +  JobManagementServiceSiteRelativePath;
                       }

                   }
               }
               return _jobManagementServiceUrl;
           }
           set
           {
               if (!string.IsNullOrEmpty(value))
               {
                   _jobManagementServiceUrl = value;
               }
           }
       }

       //this setting will be loaded from DB during component initialization.
       [XmlIgnore]
       public Guid WorkspaceId
       {
           get { return _WorkspaceId; }
           set { _WorkspaceId = value; }
       }

       //this setting will be loaded from DB during component initialization.
       [XmlIgnore]
       public Guid AdminScopeId
       {
           get
           {
               return _AdminScopeId;
           }
           set
           {
               _AdminScopeId = value;
               /*
                              if (!string.IsNullOrEmpty(value))
                              {
                                  _AdminScopeId = value;
                              }
                              else
                              {
                                  _AdminScopeId = Guid.Empty.ToString();
                              }*/
           }
       }

       //this setting will be loaded from DB during component initialization.
       //this element identifies current component.
       [XmlIgnore]
       public Guid SubmitterComponentId
       {
           get
           {
               return _SubmitterComponentId;
           }
           set
           {
               _SubmitterComponentId = value;
           }
       }
       //this setting will be loaded from DB during component initialization.
       [XmlIgnore]
       public Guid RepositoryId
       {
           get
           {
               return _RepositoryId;
           }
           set
           {
               _RepositoryId = value;
           }
       }
       public string UserDefinedId
       {
           get
           {
               return _UserDefinedId;
           }
           set
           {
               _UserDefinedId = value;
           }
       }
       public string OutputDocumentsPathAfterCompleted
       {
           get
           {
               return _OutputDocumentsPathAfterCompleted;
           }
           set
           {
               if (!string.IsNullOrEmpty(value))
               {
                   _OutputDocumentsPathAfterCompleted = value;
               }
               else
               {
                   string xLocation = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                   if (string.IsNullOrEmpty(xLocation))
                   {
                       _OutputDocumentsPathAfterCompleted = Path.Combine(xLocation, OutputPathSubFolder);
                   }
               }
           }

       }
       public string CustomUncPathForInputDocs
       {
           get
           {
               return _CustomUncPathForInputDocs;
           }
           set
           {
               _CustomUncPathForInputDocs = value;
           }
       }


       public bool FirstStart
       {
           get
           {
               return _FirstStart;
           }
           set
           {
               _FirstStart = value;
           }
       }
       public bool UseObjectLock
       {
           get
           {
               return _UseObjectLock;
           }
           set
           {
               _UseObjectLock = value;
           }
       }              
   }
   public class DirectorSettingsMngr
   {
        public DirectorSettings DeserializeDirectorSettings(string xmlSettings)
        {
            return (DirectorSettings) Serialize.DeserializeObject(xmlSettings, typeof(DirectorSettings));
        }
        public string DeserializeDirectorSettings(DirectorSettings directorSettings)
        {
            return Serialize.SerializeObject(directorSettings, typeof(DirectorSettings));
        }
   }
    
   
}
