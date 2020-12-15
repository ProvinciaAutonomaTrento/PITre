IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Transfer]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Transfer]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Transfer] (
[System_ID] int identity  NOT NULL  
, [ID_Amministrazione] int  NOT NULL  
, [Description] varchar(200)  NOT NULL  
, [Note] varchar(2000)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Transfer] ADD CONSTRAINT [ARCHIVE_Transfer_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempProject]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempProject]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempProject] (
[TransferPolicy_ID] int  NOT NULL  
, [Project_ID] int  NOT NULL  
, [ProjectCode] varchar(2000)  NULL  
, [ProjectType] varchar(10)  NULL  
, [Registro] varchar(200)  NULL  
, [UO] varchar(200)  NULL  
, [Tipologia] varchar(200)  NULL  
, [Titolario] varchar(200)  NULL  
, [ClasseTitolario] varchar(200)  NULL  
, [DataChiusura] datetime  NULL  
, [DaTrasferire] int  NULL  
, [InConservazione] int  NULL  
, [TipoTrasferimento_Policy] varchar(20)  NULL  
, [TipoTrasferimento_Versamento] varchar(20)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProject] ADD CONSTRAINT [ARCHIVE_TempProject_PK] PRIMARY KEY CLUSTERED (
[TransferPolicy_ID]
, [Project_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempTransferFile]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempTransferFile]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempTransferFile] (
[System_ID] int identity  NOT NULL  
, [Transfer_ID] int  NOT NULL  
, [DocNumber] int  NOT NULL  
, [Version_ID] int  NOT NULL  
, [OriginalPath] varchar(1000)  NOT NULL  
, [OriginalHash] varchar(100)  NOT NULL  
, [Processed] int  NOT NULL  
, [ProcessResult] int  NULL  
, [ProcessError] varchar(1000)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempTransferFile] ADD CONSTRAINT [ARCHIVE_TempTransferFile_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferStateType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferStateType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferStateType] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferStateType] ADD CONSTRAINT [ARCHIVE_TransferStateType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Temp_Project_Profile]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Temp_Project_Profile]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Temp_Project_Profile] (
[Project_ID] int  NOT NULL  
, [Profile_ID] int  NOT NULL  
, [TransferPolicy_ID] int  NOT NULL  
, [PolicyAssociation] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Temp_Project_Profile] ADD CONSTRAINT [ARCHIVE_Temp_Project_Profile_PK] PRIMARY KEY CLUSTERED (
[Profile_ID]
, [Project_ID]
, [TransferPolicy_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Log]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Log]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Log] (
[System_ID] int identity  NOT NULL  
, [Timestamp] datetime  NOT NULL  
, [Action] varchar(2000)  NOT NULL  
, [ActionType] varchar(10)  NOT NULL  
, [ActionObject] varchar(50)  NOT NULL  
, [ObjectType] int  NULL  
, [ObjectID] int  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Log] ADD CONSTRAINT [ARCHIVE_Log_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferState]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferState]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferState] (
[System_ID] int identity  NOT NULL  
, [Transfer_ID] int  NOT NULL  
, [TransferStateType_ID] int  NOT NULL  
, [DateTime] datetime  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferState] ADD CONSTRAINT [ARCHIVE_TransferState_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_ObjectType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_ObjectType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_ObjectType] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_ObjectType] ADD CONSTRAINT [ARCHIVE_ObjectType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferPolicy_ProfileType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferPolicy_ProfileType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferPolicy_ProfileType] (
[TransferPolicy_ID] int  NOT NULL  
, [ProfileType_ID] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy_ProfileType] ADD CONSTRAINT [ARCHIVE_TransferPolicy_ProfileType_PK] PRIMARY KEY CLUSTERED (
[ProfileType_ID]
, [TransferPolicy_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_ProfileType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_ProfileType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_ProfileType] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_ProfileType] ADD CONSTRAINT [ARCHIVE_ProfileType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempCateneDoc]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempCateneDoc]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempCateneDoc] (
[TransferPolicy_ID] int  NOT NULL  
, [Profile_ID] int  NOT NULL  
, [LinkedDoc_ID] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempCateneDoc] ADD CONSTRAINT [ARCHIVE_TempCateneDoc_PK] PRIMARY KEY CLUSTERED (
[LinkedDoc_ID]
, [Profile_ID]
, [TransferPolicy_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferPolicy]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferPolicy]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID] int identity  NOT NULL  
, [Description] varchar(200)  NOT NULL  
, [Enabled] int  NOT NULL  
, [Transfer_ID] int  NOT NULL  
, [TransferPolicyType_ID] int  NOT NULL  
, [TransferPolicyState_ID] int  NOT NULL  
, [Registro_ID] int  NULL  
, [UO_ID] int  NULL  
, [IncludiSottoalberoUO] int  NULL  
, [Tipologia_ID] int  NULL  
, [Titolario_ID] int  NULL  
, [ClasseTitolario] varchar(100)  NULL  
, [IncludiSottoalberoClasseTit] int  NULL  
, [AnnoCreazioneDa] int  NULL  
, [AnnoCreazioneA] int  NULL  
, [AnnoProtocollazioneDa] int  NULL  
, [AnnoProtocollazioneA] int  NULL  
, [AnnoChiusuraDa] int  NULL  
, [AnnoChiusuraA] int  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy] ADD CONSTRAINT [ARCHIVE_TransferPolicy_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempProfile]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempProfile]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempProfile] (
[TransferPolicy_ID] int  NOT NULL  
, [Profile_ID] int  NOT NULL  
, [OggettoDocumento] varchar(2000)  NULL  
, [TipoDocumento] varchar(1)  NULL  
, [Timestamp] datetime  NOT NULL  
, [Registro] varchar(200)  NULL  
, [UO] varchar(200)  NULL  
, [Tipologia] varchar(200)  NULL  
, [DataCreazione] datetime  NULL  
, [DataUltimoAccesso] datetime  NULL  
, [NumeroUtentiAccedutiUltimoAnno] int  NULL  
, [NumeroAccessiUltimoAnno] int  NULL  
, [TipoTrasferimento_Policy] varchar(20)  NULL  
, [TipoTrasferimento_Versamento] varchar(20)  NULL  
, [CopiaPerFascicolo_Policy] int  NULL  
, [CopiaPerCatenaDoc_Policy] int  NULL  
, [CopiaPerConservazione_Policy] int  NULL  
, [CopiaPerFascicolo_Versamento] int  NULL  
, [CopiaPerCatenaDoc_Versamento] int  NULL  
, [CopiaPerConservazione_Versamento] int  NULL  
, [InConservazione] int  NULL  
, [MantieniCopia] int  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProfile] ADD CONSTRAINT [ARCHIVE_TempProfile_PK] PRIMARY KEY CLUSTERED (
[TransferPolicy_ID]
, [Profile_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Disposal]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Disposal]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Disposal] (
[System_ID] int identity  NOT NULL  
, [Description] varchar(200)  NOT NULL  
, [ID_Amministrazione] int  NOT NULL  
, [Note] varchar(2000)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Disposal] ADD CONSTRAINT [ARCHIVE_Disposal_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferPolicyType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferPolicyType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferPolicyType] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicyType] ADD CONSTRAINT [ARCHIVE_TransferPolicyType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_JOB_Transfer]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_JOB_Transfer]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_JOB_Transfer] (
[System_ID] int identity  NOT NULL  
, [Transfer_ID] int  NOT NULL  
, [JobType_ID] int  NOT NULL  
, [InsertJobTimestamp] datetime  NOT NULL  
, [StartJobTimestamp] datetime  NULL  
, [EndJobTimestamp] datetime  NULL  
, [Executed] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Transfer] ADD CONSTRAINT [ARCHIVE_JOB_Transfer_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_OrganizationalChart]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_OrganizationalChart]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_OrganizationalChart] (
[System_ID] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_OrganizationalChart] ADD CONSTRAINT [ARCHIVE_OrganizationalChart_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_JOB_TransferPolicy]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_JOB_TransferPolicy]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_JOB_TransferPolicy] (
[System_ID] int identity  NOT NULL  
, [TransferPolicy_ID] int  NOT NULL  
, [JobType_ID] int  NOT NULL  
, [InsertJobTimestamp] datetime  NOT NULL  
, [StartJobTimestamp] datetime  NULL  
, [EndJobTimestamp] datetime  NULL  
, [Executed] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_TransferPolicy] ADD CONSTRAINT [ARCHIVE_JOB_TransferPolicy_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_DisposalState]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_DisposalState]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_DisposalState] (
[System_ID] int identity  NOT NULL  
, [Disposal_ID] int  NOT NULL  
, [DisposalStateType_ID] int  NOT NULL  
, [DateTime] datetime  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_DisposalState] ADD CONSTRAINT [ARCHIVE_DisposalState_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_JOB_Disposal]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_JOB_Disposal]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_JOB_Disposal] (
[System_ID] int identity  NOT NULL  
, [Disposal_ID] int  NOT NULL  
, [JobType_ID] int  NOT NULL  
, [InsertJobTimestamp] datetime  NOT NULL  
, [StartJobTimestamp] datetime  NULL  
, [EndJobTimestamp] datetime  NULL  
, [Executed] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Disposal] ADD CONSTRAINT [ARCHIVE_JOB_Disposal_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_DisposalStateType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_DisposalStateType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_DisposalStateType] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_DisposalStateType] ADD CONSTRAINT [ARCHIVE_DisposalStateType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_JobType]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_JobType]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_JobType] (
[System_ID] int  NOT NULL  
, [Nome] varchar(20)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_JobType] ADD CONSTRAINT [ARCHIVE_JobType_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Authorization]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Authorization]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Authorization] (
[System_ID] int identity  NOT NULL  
, [People_ID] int  NOT NULL  
, [StartDate] datetime  NOT NULL  
, [EndDate] datetime  NOT NULL  
, [Note] varchar(2000)  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Authorization] ADD CONSTRAINT [ARCHIVE_Authorization_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_Configuration]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_Configuration]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_Configuration] (
[Key] varchar(200)  NOT NULL  
, [Value] varchar(200)  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Configuration] ADD CONSTRAINT [ARCHIVE_Configuration_PK] PRIMARY KEY CLUSTERED (
[Key]
, [Value]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TransferPolicyState]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TransferPolicyState]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TransferPolicyState] (
[System_ID] int  NOT NULL  
, [Name] varchar(50)  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicyState] ADD CONSTRAINT [ARCHIVE_TransferPolicyState_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempProfileDisposal]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempProfileDisposal]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempProfileDisposal] (
[Disposal_ID] int  NOT NULL  
, [Profile_ID] int  NOT NULL  
, [DaScartare] int  NOT NULL  
)
GO

GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_AuthorizedObject]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_AuthorizedObject]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_AuthorizedObject] (
[System_ID] int identity  NOT NULL  
, [Authorization_ID] int  NOT NULL  
, [Project_ID] int  NULL  
, [Profile_ID] int  NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_AuthorizedObject] ADD CONSTRAINT [ARCHIVE_AuthorizedObject_PK] PRIMARY KEY CLUSTERED (
[System_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempTransferProfile]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempTransferProfile]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempTransferProfile] (
[Transfer_ID] int  NOT NULL  
, [Profile_ID] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempTransferProfile] ADD CONSTRAINT [ARCHIVE_TempTransferProfile_PK] PRIMARY KEY CLUSTERED (
[Transfer_ID]
, [Profile_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempTransferProject]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempTransferProject]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempTransferProject] (
[Transfer_ID] int  NOT NULL  
, [Project_ID] int  NOT NULL  
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempTransferProject] ADD CONSTRAINT [ARCHIVE_TempTransferProject_PK] PRIMARY KEY CLUSTERED (
[Transfer_ID]
, [Project_ID]
)
GO
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[ARCHIVE_TempProjectDisposal]') AND type in (N'U'))
    DROP TABLE [DOCSADM].[ARCHIVE_TempProjectDisposal]
GO
CREATE TABLE [DOCSADM].[ARCHIVE_TempProjectDisposal] (
[Disposal_ID] int  NOT NULL  
, [Project_ID] int  NOT NULL  
, [DaScartare] int  NOT NULL  
)
GO

GO
GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProject] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_TempProject_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_Temp_Project_Profile] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_Temp_Project_Profile_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_Log] WITH CHECK ADD CONSTRAINT [ARCHIVE_ObjectType_ARCHIVE_Log_FK1] FOREIGN KEY (
[ObjectType]
)
REFERENCES [DOCSADM].[ARCHIVE_ObjectType] (
[System_ID]
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferState] WITH CHECK ADD CONSTRAINT [ARCHIVE_Transfer_ARCHIVE_TransferState_FK1] FOREIGN KEY (
[Transfer_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Transfer] (
[System_ID]
)
ON DELETE CASCADE
ALTER TABLE [DOCSADM].[ARCHIVE_TransferState] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferStateType_ARCHIVE_TransferState_FK1] FOREIGN KEY (
[TransferStateType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferStateType] (
[System_ID]
)
GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy_ProfileType] WITH CHECK ADD CONSTRAINT [ARCHIVE_ProfileType_ARCHIVE_TransferPolicy_ProfileType_FK1] FOREIGN KEY (
[ProfileType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_ProfileType] (
[System_ID]
)
ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy_ProfileType] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_TransferPolicy_ProfileType_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempCateneDoc] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_TempCateneDoc_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy] WITH CHECK ADD CONSTRAINT [ARCHIVE_Transfer_ARCHIVE_TransferPolicy_FK1] FOREIGN KEY (
[Transfer_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Transfer] (
[System_ID]
)
ON DELETE CASCADE
ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicyType_ARCHIVE_TransferPolicy_FK1] FOREIGN KEY (
[TransferPolicyType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicyType] (
[System_ID]
)
ALTER TABLE [DOCSADM].[ARCHIVE_TransferPolicy] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicyState_ARCHIVE_TransferPolicy_FK1] FOREIGN KEY (
[TransferPolicyState_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicyState] (
[System_ID]
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProfile] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_TempProfile_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Transfer] WITH CHECK ADD CONSTRAINT [ARCHIVE_Transfer_ARCHIVE_JOB_Transfer_FK1] FOREIGN KEY (
[Transfer_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Transfer] (
[System_ID]
)
ON DELETE CASCADE
ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Transfer] WITH CHECK ADD CONSTRAINT [ARCHIVE_JobType_ARCHIVE_JOB_Transfer_FK1] FOREIGN KEY (
[JobType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_JobType] (
[System_ID]
)
GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_TransferPolicy] WITH CHECK ADD CONSTRAINT [ARCHIVE_JobType_ARCHIVE_JOB_TransferPolicy_FK1] FOREIGN KEY (
[JobType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_JobType] (
[System_ID]
)
ALTER TABLE [DOCSADM].[ARCHIVE_JOB_TransferPolicy] WITH CHECK ADD CONSTRAINT [ARCHIVE_TransferPolicy_ARCHIVE_JOB_TransferPolicy_FK1] FOREIGN KEY (
[TransferPolicy_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_TransferPolicy] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_DisposalState] WITH CHECK ADD CONSTRAINT [ARCHIVE_Disposal_ARCHIVE_DisposalState_FK1] FOREIGN KEY (
[Disposal_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Disposal] (
[System_ID]
)
ON DELETE CASCADE
ALTER TABLE [DOCSADM].[ARCHIVE_DisposalState] WITH CHECK ADD CONSTRAINT [ARCHIVE_DisposalStateType_ARCHIVE_DisposalState_FK1] FOREIGN KEY (
[DisposalStateType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_DisposalStateType] (
[System_ID]
)
GO

ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Disposal] WITH CHECK ADD CONSTRAINT [ARCHIVE_JobType_ARCHIVE_JOB_Disposal_FK1] FOREIGN KEY (
[JobType_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_JobType] (
[System_ID]
)
ALTER TABLE [DOCSADM].[ARCHIVE_JOB_Disposal] WITH CHECK ADD CONSTRAINT [ARCHIVE_Disposal_ARCHIVE_JOB_Disposal_FK1] FOREIGN KEY (
[Disposal_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Disposal] (
[System_ID]
)
ON DELETE CASCADE
GO

GO

GO

GO

GO

GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProfileDisposal] WITH CHECK ADD CONSTRAINT [ARCHIVE_Disposal_ARCHIVE_TempProfileDisposal_FK1] FOREIGN KEY (
[Disposal_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Disposal] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_AuthorizedObject] WITH CHECK ADD CONSTRAINT [ARCHIVE_Authorization_ARCHIVE_AuthorizedObject_FK1] FOREIGN KEY (
[Authorization_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Authorization] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempTransferProfile] WITH CHECK ADD CONSTRAINT [ARCHIVE_Transfer_ARCHIVE_TempTransferProfile_FK1] FOREIGN KEY (
[Transfer_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Transfer] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempTransferProject] WITH CHECK ADD CONSTRAINT [ARCHIVE_Transfer_ARCHIVE_TempTransferProject_FK1] FOREIGN KEY (
[Transfer_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Transfer] (
[System_ID]
)
ON DELETE CASCADE
GO

ALTER TABLE [DOCSADM].[ARCHIVE_TempProjectDisposal] WITH CHECK ADD CONSTRAINT [ARCHIVE_Disposal_ARCHIVE_TempProjectDisposal_FK1] FOREIGN KEY (
[Disposal_ID]
)
REFERENCES [DOCSADM].[ARCHIVE_Disposal] (
[System_ID]
)
ON DELETE CASCADE
GO

