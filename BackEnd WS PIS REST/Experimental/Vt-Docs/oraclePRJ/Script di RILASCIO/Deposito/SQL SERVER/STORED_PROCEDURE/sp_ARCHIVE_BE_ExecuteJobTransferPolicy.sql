USE PCM_DEPOSITO_FINGER
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================================
-- Author:		Giovanni Olivari
-- Create date: 30/05/2013
-- Description:	Esegue i job schedulati per l'entità TransferPolicy
-- ================================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_ExecuteJobTransferPolicy
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



	-- Chiude e dealloca eventuali cursori rimasti aperti
	--
	execute sp_ARCHIVE_BE_CleanUpCursor 'executeTransferPolicy_cursor'


	
	DECLARE @systemID int
	DECLARE @transferPolicyID int
	DECLARE @jobType int
	
	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType VARCHAR(50)
	DECLARE @logObjectID int


	
	SET @logType = 'INFO'
	SET @log = 'Start execution job TransferPolicy'

	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	
	
	DECLARE executeTransferPolicy_cursor CURSOR
	FOR SELECT 
		[SYSTEM_ID]
		,[TRANSFERPOLICY_ID]
		,[JOBTYPE_ID]
		--,[INSERTJOBTIMESTAMP]
		--,[STARTJOBTIMESTAMP]
		--,[ENDJOBTIMESTAMP]
		--,[EXECUTED]
	FROM [DOCSADM].[ARCHIVE_JOB_TRANSFERPOLICY]
	WHERE [EXECUTED] = 0
	     
	OPEN executeTransferPolicy_cursor
	FETCH executeTransferPolicy_cursor INTO @systemID, @transferPolicyID, @jobType
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
	 
		PRINT 'TransferPolicyID: ' + CAST(@transferPolicyID AS NVARCHAR(MAX))
		PRINT 'JobType: ' + CAST(@jobType AS NVARCHAR(MAX))
		
		-- Aggiorna la data di start
		--
		UPDATE [DOCSADM].[ARCHIVE_JOB_TRANSFERPOLICY]
		SET [STARTJOBTIMESTAMP] = GETDATE()
		WHERE [SYSTEM_ID] = @systemID
		
		-- Esegue il comando schedulato in base al jobType
		--		
		-- 1 - RICERCA
		-- 2 - ANALISI
		-- 3 - ESECUZIONE

		IF (@jobType = 1)
			-- RICERCA
			BEGIN
				SET @logType = 'INFO'
				SET @log = 'Job RICERCA per il TransferPolicy: ' + CAST(@transferPolicyID AS NVARCHAR(MAX))
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				EXECUTE sp_ARCHIVE_BE_StartSearchForPolicy @transferPolicyID
			END
		ELSE
			BEGIN
			
				IF (@jobType = 2)
					-- ANALISI
					BEGIN
						SET @logType = 'INFO'
						SET @log = 'Job ANALISI per il TransferPolicy: ' + CAST(@transferPolicyID AS NVARCHAR(MAX))
						
						EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

						EXECUTE sp_ARCHIVE_BE_StartAnalysisForPolicy @transferPolicyID
					END
				ELSE
					-- Qualsiasi altro valore non è previsto per i job schedulati per il TransferPolicy
					BEGIN
		
						set @logType = 'WARNING'
						set @log = 'JobType non previsto per la schedulazione TransferPolicy - Valore: ' + CAST(@jobType AS NVARCHAR(MAX))
						
						EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
					
					END
					
			END
		
		-- Aggiorna la data di end e il flag di esecuzione
		--
		UPDATE [DOCSADM].[ARCHIVE_JOB_TRANSFERPOLICY]
		SET [ENDJOBTIMESTAMP] = GETDATE()
		, [EXECUTED] = 1
		WHERE [SYSTEM_ID] = @systemID
	   
	FETCH executeTransferPolicy_cursor INTO @systemID, @transferPolicyID, @jobType
	END
	  
	CLOSE executeTransferPolicy_cursor
	DEALLOCATE executeTransferPolicy_cursor


	
	SET @logType = 'INFO'
	SET @log = 'End execution job TransferPolicy'

	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

END
GO
