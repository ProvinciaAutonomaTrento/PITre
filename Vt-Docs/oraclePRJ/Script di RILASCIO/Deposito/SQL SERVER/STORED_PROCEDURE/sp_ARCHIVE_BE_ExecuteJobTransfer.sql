USE PCM_DEPOSITO_1
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Giovanni Olivari
-- Create date: 30/05/2013
-- Description:	Esegue i job schedulati per l'entità Transfer
-- ==========================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_ExecuteJobTransfer
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



	-- Chiude e dealloca eventuali cursori rimasti aperti
	--
	execute sp_ARCHIVE_BE_CleanUpCursor 'executeTransfer_cursor'


	
	DECLARE @systemID INT
	DECLARE @transferID INT
	DECLARE @jobType INT
	DECLARE @returnValue INT
	DECLARE @NumberOfObjectsPerTransaction INT
	DECLARE @DatetimeLimit Datetime

	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType VARCHAR(50)
	DECLARE @logObjectID INT


	
	SET @logType = 'INFO'
	SET @log = 'Start execution job Transfer'

	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	
	
	DECLARE executeTransfer_cursor CURSOR
	FOR SELECT 
		[SYSTEM_ID]
		,[TRANSFER_ID]
		,[JOBTYPE_ID]
		--,[INSERTJOBTIMESTAMP]
		--,[STARTJOBTIMESTAMP]
		--,[ENDJOBTIMESTAMP]
		--,[EXECUTED]
	FROM [DOCSADM].[ARCHIVE_JOB_TRANSFER]
	WHERE [EXECUTED] = 0
	     
	OPEN executeTransfer_cursor
	FETCH executeTransfer_cursor INTO @systemID, @transferID, @jobType
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		
		SET @returnValue = 1
	 
		PRINT 'TransferID: ' + CAST(@transferID AS NVARCHAR(MAX))
		PRINT 'JobType: ' + CAST(@jobType AS NVARCHAR(MAX))
		
		-- Aggiorna la data di start
		--
		UPDATE [DOCSADM].[ARCHIVE_JOB_TRANSFER]
		SET [STARTJOBTIMESTAMP] = GETDATE()
		WHERE [SYSTEM_ID] = @systemID
		
		-- Esegue il comando schedulato in base al jobType
		--		
		-- 1 - RICERCA
		-- 2 - ANALISI
		-- 3 - ESECUZIONE

		IF (@jobType = 2)
			-- ANALISI
			BEGIN
				SET @logType = 'INFO'
				SET @log = 'Job ANALISI per il Transfer: ' + CAST(@transferID AS NVARCHAR(MAX))
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				EXECUTE sp_ARCHIVE_BE_StartAnalysisForTransfer @transferID				
			END
		ELSE
			BEGIN
			
				IF (@jobType = 3)
					-- ESECUZIONE
					BEGIN
						SET @logType = 'INFO'
						SET @log = 'Job ESECUZIONE per il Transfer: ' + CAST(@transferID AS NVARCHAR(MAX))
						
						EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

						SET @NumberOfObjectsPerTransaction = 100
						SET @DatetimeLimit = GETDATE()+1

						EXECUTE @returnValue = sp_ARCHIVE_BE_ExecuteTransferByID @transferID, @NumberOfObjectsPerTransaction, @DatetimeLimit

						SET @logType = 'INFO'
						SET @log = 'Transfer: ' + CAST(@transferID AS NVARCHAR(MAX)) + ' - RetValue: ' + CAST(@returnValue AS NVARCHAR(MAX))
						EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

					END
				ELSE
					-- Qualsiasi altro valore non è previsto per i job schedulati per il Transfer
					BEGIN
		
						set @logType = 'WARNING'
						set @log = 'JobType non previsto per la schedulazione Transfer - Valore: ' + CAST(@jobType AS NVARCHAR(MAX))
						
						EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
					
					END
					
			END
		
		-- Aggiorna la data di end e il flag di esecuzione se l'esecuzione è andata a buon fine
		--
		IF (@returnValue = 1)
			BEGIN
				SET @logType = 'INFO'
				SET @log = 'Aggiornamento end Transfer: ' + CAST(@transferID AS NVARCHAR(MAX)) + ' - RetValue: ' + CAST(@returnValue AS NVARCHAR(MAX))
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
				
			
				UPDATE [DOCSADM].[ARCHIVE_JOB_TRANSFER]
				SET [ENDJOBTIMESTAMP] = GETDATE()
				, [EXECUTED] = 1
				WHERE [SYSTEM_ID] = @systemID
			END
	   
	FETCH executeTransfer_cursor INTO @systemID, @transferID, @jobType
	END
	  
	CLOSE executeTransfer_cursor
	DEALLOCATE executeTransfer_cursor


	
	SET @logType = 'INFO'
	SET @log = 'End execution job Transfer'

	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

END
GO
