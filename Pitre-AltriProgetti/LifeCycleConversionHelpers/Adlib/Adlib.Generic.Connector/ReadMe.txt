/////////////////////////////////////////////////////////////////////////////
//  Copyright (C) 1997-2011 Adlib Software
//  All rights reserved.
/////////////////////////////////////////////////////////////////////////////

Adlib Sample Application (Generic Connector)

CONTENTS
======================================================================================
I.   Steps to configure and run the Adlib Sample Application (Generic Connector)
II.  Submit document(s) for rendering
III. C# Source Code (Projects and Main Classes)

======================================================================================
I. Steps to configure and run the Adlib Sample Application (Generic Connector)
======================================================================================

The following steps must be performed prior to submitting document(s) for rendering.

1) Install the .NET Framework 3.5 or greater (http://www.microsoft.com/NET/)
2) Optional: Copy files to "C:\Program Files\Adlib\YourFolderName" if other Adlib components are already installed on the machine. If no Adlib components are present, Sample Application can be copied to any user folder.
3) If Sample Application is a single Adlib application on the machine:
	2.1) make sure correct Adib.Config file exists inside Sample Application working folder.
	2.2) remove <MachineId>...</MachineId> line, if actual machineId is unknown (this will trigger component/machine registration).
	2.3) Make sure 'ServiceUrl' is pointing to proper link. For Example:	<ServiceUrl>http://host1:80/Adlib</ServiceUrl>
4) Launch "Adlib.Generic.Connector.exe"
5) Go to the "Settings" tab and set/review following settings:
	- "Director Web Service URL" - This must reference the DirectorWSA(JobManagementService) URL (e.g.http://localhost/adlib/DirectorWSA/DirectorWSA.asmx) 
	- "Submitter Component ID" - This value will be auto-retrieved when Sample Application is initialized.
	- "Repository ID" - Optional. This ID is used to group all of the submitted jobs for a given source document repository. All clients that submit Jobs from the same Repository should use the same Repository ID. This allows multiple clients to submit and complete each other's Jobs, thereby enabling high availability.
	- "User defined ID" - Optional. This is an optional setting that allows the client to define its own ID for the Job. For example, this could be the Document ID used by the repository.
	- "UNC root folder for input documents" - This defines the shared folder used when "UNC Location" is selected for the "Transfer input Document to:" setting. Each job's documents are stored in a subfolder with the same name as the Job ID. The job output is stored in a subfolder called Output.
	- "Local root folder for output documents" - This defines the local folder where the Job output files will be copied when requested by the user.
	- "Transformation WorkspaceId" - id of a workspace with Transformation rules to be used. This would be loaded by Sample Application during initialization.
6) On "Settings" Tab press "Init settings" button. If message box prompts to register component - proceed.
7) If Sample Application is a single Adlib application on the machine:
	7.1) After registration - component should be assigned to correct Environment for Initialization - this can be done in Adlib UI, tab "Environments".
	7.2) On "Sources" tab (Adlib UI) - add newly registered component to "Generic Source".
8) On "Settings" Tab press "Init settings" button.

======================================================================================
II. Submit document(s) for rendering
======================================================================================

The following steps must be performed to submit document(s) for rendering.

1) Select Document(s) to submit for rendering (required):
	- Use the buttons on the right to add selected document(s) or all document(s) from a specific folder.

2) Define Document Transformation options via Metadata (optional):
	- Document Transformation (Rendering) options are defined by including metadata with the Job. Use the buttons on the right to load files that have predefined metadata and/or add individual metadata items. All metadata values can be edited (by double clicking the specific metadata row) to meet you specific requirements.

3) Modify job submission settings (optional):
	- Submit job N times - This defines the number of times the Sample Application will submit the Job. This is used to perform load and reliability tests of the system.
	- Transfer input documents to - defines how the input documents are transferred to the system. The "UNC location" copies the documents to the location defined by the "UNC root folder for input documents" value on the Settings tab.  The "Director HTTP File Repository (via MTOM)" uses the Message Transmission Optimization Mechanism to transfer the documents to the system.
	- Submit each document as separate job - This setting only takes effect if multiple documents are added in step 1.  This setting, if unselected, allows you to merge all documents into one rendition.  If selected, one rendition will be created for every document by sending one job per document.

4) Submit job with document(s) and metadata:
	- Use the "Submit job" button to send all documents and metadata to the Director WSA.

======================================================================================
III. C# Source Code (Projects and Main Classes)
======================================================================================

There are 2 main projects in the Microsoft .NET C# Solution (using Visual Studio 2008):
   
1) Adlib.Generic.Connector - Windows Form project, which utilizes functionality from Adlib.Director.DirectorWSAWrapper.

2) Adlib.Director.DirectorWSAWrapper - this Class Library project is a functional wrapper for Director WSA(Job Management Service).

Main classes:
- DirectorWSAWrapper - main class, which provides following functions:

	* Logging.LogMngr LogManager() - Returns a logging instance to log messages to file and screen
	
	* DirectorSettings DirectorSettings - Property to get/set Director settings. Settings file (DirectorSampleAppSettings.xml) is not required on first start, it will be created with default values. Values can be modified on Settings tab.
	
	* bool SaveSettings() - Saves Director-related settings to the disk 
	
	* string RefreshSystemHealth() - Calls DirectorWSA.ExecuteCommand method to query Director System health. If system is not healthy, jobs will not be processed successfully.
	
	* JobDetailResponse GetJobDetails(Guid jobId) - Returns Job details for the specified JobId, including job status, IDs, error details, etc..
	
	* JobDetailResponse[] GetJobDetails(Guid[] jobIds) - Same as above, except the input and output objects are arrays.
	
	* JobDetailResponse[] GetJobsDetails(ViewJobs viewJobsFlag) - Returns an array of all Jobs and details which job status are either Completed, InProgress or All.
	
	* bool SubmitDocuments(string inputFolder, List<Metadata> inputMetadata, bool transferDocsToRepository, string outputExtension) - Submits all documents (recursively) from specified folder to Director for rendering. All input documents will be merged into a single rendition document.

	* public bool SubmitDocuments(List<string> inputDocuments, List< Metadata> inputMetadata, bool transferDocsToRepository, string outputExtension) - Submits list of input documents to the Director for rendering. All input documents will be merged into a single rendition document. 
	
	* public bool SubmitJobTicket(string jobTicket) - Submits a Job Ticket (as text) to Director for processing.
	
	* JobResponse[] CancelJobs(List<Guid> jobIds) - Cancels all Jobs specified in the list for Jobs with JobStatus that is either initialized, Committed or InProgress.
	
	* JobResponse[] ReleaseJobs(List<Guid> jobIds) - Releases all Jobs specified in the list for Jobs with JobStatus that is Completed*. Job documents that have been added using PubJobFiles will be deleted (including output files).
	
	* bool DownloadOutputDocuments(Guid jobId) - Downloads output document(s) for specified JobId. The file will be either copied from UNC path or downloaded through DirectorWSA.GetJobFiles call (HTTP MTOM), depending on how the documents were submitted.
	
	* bool DownloadOutputDocuments(Guid jobId, bool useFileRepository, double chunkSizeMb) - Downloads output document(s) for specified JobId. The file will be either copied from UNC path or downloaded through DirectorWSA.GetJobFiles call (HTTP MTOM), depending on how the documents were submitted.

	* bool UploadJobFiles(Guid jobId, List<string> inputDocPathList, double chunkSizeMb) - Uploads the specified input documents to the Director file repository using DirectorWSA.PutJobFiles call (HTTP MTOM)
	
	* bool DefineJobFiles(List<string> uncInputPathList, Guid jobId, string outputExtention) - Defines the full paths for the input and output document(s) for a Job. Path to documents should not be a local path, unless all Adlib components are installed on the same machine.

