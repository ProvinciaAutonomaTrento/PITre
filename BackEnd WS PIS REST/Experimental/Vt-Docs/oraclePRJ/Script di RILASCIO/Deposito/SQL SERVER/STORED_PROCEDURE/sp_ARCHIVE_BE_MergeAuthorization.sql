USE PCM_DEPOSITO_1
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =====================================================================================
-- Author:		Giovanni Olivari
-- Create date: 06/06/2013
-- Description:	Inserisce/Aggiorna (Merge) i dati dell'autorizzazione alla consultazione
--              di documenti e fascicoli per un determinato utente
-- =====================================================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_MergeAuthorization 
	@systemID INT OUTPUT
	,@peopleID INT
	,@startDate DATETIME
	,@endDate DATETIME
	,@note VARCHAR(2000)
	,@profileList VARCHAR(MAX)
	,@projectList VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @sql_string NVARCHAR(MAX)
	DECLARE @errorCode INT
	DECLARE @selectedSystemID INT = 0
	DECLARE @idCorrGlobaliRuoloConsultatore INT
	DECLARE @idGroupsRuoloConsultatore INT
	
	-- COSTANTI
	--
	DECLARE @tipoDiritto_ACQUISITO VARCHAR(1) = 'A'
	DECLARE @accessRights_SOLA_LETTURA int = 45
	
	
	-- Lettura parametri di configurazione
	--
	SELECT @idCorrGlobaliRuoloConsultatore=CAST([VALUE] AS INT) FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'ID_CORR_GLOBALI_RUOLO_CONSULTATORE'
	
	SELECT @idGroupsRuoloConsultatore=CAST([VALUE] AS INT) FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'ID_GROUPS_RUOLO_CONSULTATORE'
	
	print 'ID CORR GLOBALI RUOLO CONSULTATORE: ' + CAST(@idCorrGlobaliRuoloConsultatore AS VARCHAR(10))
	print 'ID GROUPS RUOLO CONSULTATORE: ' + CAST(@idGroupsRuoloConsultatore AS VARCHAR(10))
	
	SET @systemID = ISNULL(@systemID, 0)
	
	
	
	BEGIN TRANSACTION T1

	

	-- 1.a - Inserire/aggiornare l'autorizzazione
	--
	-- MERGE ARCHIVE_AUTHORIZATION AS TARGET
	--	USING ( 	
	--		SELECT 
	--			@systemID
	--			,@peopleID
	--			,@startDate
	--			,@endDate
	--			,@note
	--		)
	--	AS SOURCE (
	--		[SYSTEM_ID]
	--		,[PEOPLE_ID]
	--		,[STARTDATE]
	--		,[ENDDATE]
	--		,[NOTE])
	--	ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
	--	WHEN MATCHED THEN
	--		UPDATE SET
	--			[PEOPLE_ID] = SOURCE.PEOPLE_ID
	--			,[STARTDATE] = SOURCE.STARTDATE
	--			,[ENDDATE] = SOURCE.ENDDATE
	--			,[NOTE] = SOURCE.NOTE
	--	WHEN NOT MATCHED THEN
	--		INSERT (
	--			[PEOPLE_ID]
	--			,[STARTDATE]
	--			,[ENDDATE]
	--			,[NOTE]
	--			)
	--		VALUES (
	--			SOURCE.PEOPLE_ID
	--			,SOURCE.STARTDATE
	--			,SOURCE.ENDDATE
	--			,SOURCE.NOTE
	--		);
			
	-- Verifica se esiste già l'autorizzazione
	--
	-- Questa istruzione potrebbe essere riscritta utilizzando il comando MERGE definito sopra, 
	-- ma solo dopo aver alzato il livello di compatibilità ad un valore maggiore di SQL Server 2000
	--
	SELECT @selectedSystemID = ISNULL(SYSTEM_ID, 0)
	FROM ARCHIVE_AUTHORIZATION
	WHERE SYSTEM_ID = @systemID
	
	print '@systemID: ' + CAST(@systemID as VARCHAR(10))
	print '@selectedSystemID: ' + CAST(@selectedSystemID as VARCHAR(10))
	

	--set @logType = 'INFO'
	--set @log = '@systemID: ' + CAST(@systemID as VARCHAR(10))
	
	--EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	--set @log = '@selectedSystemID: ' + CAST(@selectedSystemID as VARCHAR(10))
	
	--EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	
	
	-- Inserisce/aggiorna l'autorizzazione in base al valore di @selectedSystemID
	-- 
	IF (ISNULL(@selectedSystemID, 0) = 0)
		BEGIN
			-- Inserisce la nuova autorizzazione e aggiorna il systemID da restituire come parametro di output
			--
			INSERT INTO ARCHIVE_AUTHORIZATION
			(
				[PEOPLE_ID]
				,[STARTDATE]
				,[ENDDATE]
				,[NOTE]
			)
			VALUES 
			(
				@peopleID
				,@startDate
				,@endDate
				,@note
			)	
			
			SET @systemID = @@IDENTITY
			print 'insert - @systemID: ' + CAST(@systemID as VARCHAR(10))
		END
	ELSE
		BEGIN
			-- Aggiorna l'autorizzaizone esistente
			--
			UPDATE ARCHIVE_AUTHORIZATION
			SET 	
				[PEOPLE_ID] = @peopleID
				,[STARTDATE] = @startDate
				,[ENDDATE] = @endDate
				,[NOTE] = @note
			WHERE SYSTEM_ID = @systemID
			
			print 'update - @systemID: ' + CAST(@systemID as VARCHAR(10))
		END
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''istruzione Merge per la tabella ARCHIVE_AUTHORIZATION' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END


	
	-- 1.b - Eliminare tutte i record dei doc/fasc eventualmente autorizzati in precedenza
	--
	DELETE FROM ARCHIVE_AUTHORIZEDOBJECT
	WHERE AUTHORIZATION_ID = @systemID
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la pulizia della tabella ARCHIVE_AUTHORIZEDOBJECT' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	
	
	-- 1.c - Inserire tutti i record dei doc/fasc nella tabella ARCHIVE_AuthorizedObject
	--
	IF OBJECT_ID('tempdb..#deletedObject') IS NOT NULL DROP TABLE #deletedObject
	CREATE TABLE #deletedObject
	(
		ID int
	)
	
	-- Fascicoli
	--
	IF (ISNULL(@projectList,'') <> '')
		BEGIN
			TRUNCATE TABLE #deletedObject
		
			SET @sql_string = CAST(N'
				INSERT INTO #deletedObject (ID)
				SELECT DISTINCT SYSTEM_ID FROM PROJECT
				WHERE SYSTEM_ID IN (' AS NVARCHAR(MAX)) + CAST(@projectList AS NVARCHAR(MAX)) + CAST(N')' AS NVARCHAR(MAX))
				
			PRINT @sql_string;

			EXECUTE sp_executesql @sql_string
			
			INSERT INTO [ARCHIVE_AUTHORIZEDOBJECT]([Authorization_ID],[Project_ID])
			SELECT @systemID, ID FROM #deletedObject
			
			set @errorCode = @@ERROR

			IF @errorCode <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''inserimento dei fascicoli nella tabella ARCHIVE_AUTHORIZEDOBJECT' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END	
			
			
			-- 26/06/2013
			-- Su richiesta del gruppo di Deposito: i documenti contenuti nei fascicoli non devono essere visibili nella
			-- autorizzazione ma solo nella SECURITY
			
			--TRUNCATE TABLE #deletedObject
			
			---- Documenti collegati ai fascicoli selezionati
			----
			--SET @sql_string = CAST(N'
			--	INSERT INTO #deletedObject (ID)
			--	SELECT DISTINCT SYSTEM_ID FROM PROFILE
			--	WHERE 
			--	SYSTEM_ID IN
			--		(
			--		SELECT DISTINCT LINK ID_DOCUMENTO
			--		FROM PROJECT_COMPONENTS PC
			--		WHERE PC.PROJECT_ID IN
			--			(
			--			SELECT P.SYSTEM_ID FROM PROJECT P WHERE P.ID_FASCICOLO IN 
			--				(
			--				SELECT PROJECT_ID FROM ARCHIVE_AUTHORIZEDOBJECT 
			--				WHERE AUTHORIZATION_ID = ' AS NVARCHAR(MAX)) + CAST(@systemID AS NVARCHAR(MAX)) + CAST(N'
			--				)
			--			)
			--		)' AS NVARCHAR(MAX))

			--PRINT @sql_string;

			--EXECUTE sp_executesql @sql_string
			
			--INSERT INTO [ARCHIVE_AUTHORIZEDOBJECT]([Authorization_ID],[Profile_ID])
			--SELECT @systemID, ID FROM #deletedObject
			
			--set @errorCode = @@ERROR

			--IF @errorCode <> 0
			--BEGIN
			--	-- Rollback the transaction
			--	ROLLBACK
				
			--	set @logType = 'ERROR'
			--	set @log = 'Errore durante l''inserimento dei documenti collegati ai fascicoli nella tabella ARCHIVE_AUTHORIZEDOBJECT' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
				
			--	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			--	-- Raise an error and return
			--	RAISERROR (@log, 16, 1)
			--	RETURN
			--END	
			
			
			
		END
	
	-- Documenti
	--
	IF (ISNULL(@profileList,'') <> '')
		BEGIN
			TRUNCATE TABLE #deletedObject
			
			SET @sql_string = CAST(N'
				INSERT INTO #deletedObject (ID)
				SELECT DISTINCT SYSTEM_ID FROM PROFILE
				WHERE 
				SYSTEM_ID IN (' AS NVARCHAR(MAX)) + CAST(@profileList AS NVARCHAR(MAX)) + CAST(N')' AS NVARCHAR(MAX)) 
				
			PRINT @sql_string;

			EXECUTE sp_executesql @sql_string;
			
			INSERT INTO [ARCHIVE_AuthorizedObject]([Authorization_ID],[Profile_ID])
			SELECT @systemID, ID FROM #deletedObject
			
			set @errorCode = @@ERROR

			IF @errorCode <> 0
			BEGIN
				-- Rollback the transaction
				ROLLBACK
				
				set @logType = 'ERROR'
				set @log = 'Errore durante l''inserimento dei documenti nella tabella ARCHIVE_AUTHORIZEDOBJECT' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
				
				EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

				-- Raise an error and return
				RAISERROR (@log, 16, 1)
				RETURN
			END
			
		END
	

	
	-- 2 - Inserire il relativo ID_CORR_GLOBALI dell'utente, a partire da ID_PEOPLE, nella tabella ARCHIVE_OrgChart
	--
	DELETE FROM ARCHIVE_ORGANIZATIONALCHART
	WHERE SYSTEM_ID IN
		(
		SELECT SYSTEM_ID 
		FROM DPA_CORR_GLOBALI
		WHERE ID_PEOPLE = @peopleID
		)
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento dell''utente nella tabella ARCHIVE_ORGANIZATIONALCHART' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END


	
	INSERT INTO ARCHIVE_ORGANIZATIONALCHART(SYSTEM_ID)
	SELECT SYSTEM_ID 
	FROM DPA_CORR_GLOBALI
	WHERE ID_PEOPLE = @peopleID
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento dell''utente nella tabella ARCHIVE_ORGANIZATIONALCHART' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	
	
	-- 3 - Impostare DPA_CORR_GLOBALI.DTA_FINE = NULL dove ID_PEOPLE = @peopleID
	--
	UPDATE DPA_CORR_GLOBALI
	SET DTA_FINE = NULL
	WHERE ID_PEOPLE = @peopleID
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento DTA_FINE = NULL per la tabella DPA_CORR_GLOBALI' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	
	
	--4 - Aggiungere il ruolo CONSULTATORE all'utente @peopleID, tabella PEOPLE_GROUPS (da capire come ricavare il ruolo)
	--  (
	--	GROUPS = systemID della Groups per il ruolo CONSULTATORE
	--	, PEOPLE = @peopleID
	--	)
	--
	DELETE FROM [PEOPLEGROUPS]
	WHERE [GROUPS_SYSTEM_ID] = @idGroupsRuoloConsultatore --fn_ARCHIVE_GetGroupsIDForRuoloConsultatore()
	AND [PEOPLE_SYSTEM_ID] = @peopleID
	
	INSERT INTO [PEOPLEGROUPS]
           ([GROUPS_SYSTEM_ID]
           ,[PEOPLE_SYSTEM_ID]
           ,[LAST_UPDATE]
           ,[CHA_UTENTE_RIF]
           ,[CHA_PREFERITO]
           ,[DTA_FINE])
     VALUES
           (@idGroupsRuoloConsultatore --fn_ARCHIVE_GetGroupsIDForRuoloConsultatore()
           ,@peopleID
           ,NULL
           ,NULL
           ,NULL
           ,NULL)
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento del ruolo consultatore nella tabella PEOPLEGROUPS' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END


		
	-- 5 - SECURITY: inserire una riga per ogni oggetto autorizzato
	--
	-- Nel caso in cui venga inserito un record che duplica la chiave (THING, PERSONORGROUP, ACCESSRIGHTS) verrà ignorato,
	-- in quanto il relativo indice unique ha l'impostazione [Ignore Duplicate Keys] = Yes
	--
	--	(
	--	THING = profileID/projectID
	--	, PERSONORGROUP = @peopleID
	--	, CHA_TIPO_DIRITTO = 'A'
	--	, ID_GRUPPO_TRASM = NULL
	--	, ACCESSRIGHTS = 45
	--	, VAR_NOTE_SEC = @authorizationID // in questo modo posso eliminare solo i record inseriti per l'autorizzazione, ed evitare di eliminare i record provenienti dal corrente
	--	)
	--
	DELETE FROM [SECURITY]
	WHERE [VAR_NOTE_SEC] = @systemID
		
	INSERT INTO [SECURITY]
           ([THING]
           ,[PERSONORGROUP]
           ,[ACCESSRIGHTS]
           ,[ID_GRUPPO_TRASM]
           ,[CHA_TIPO_DIRITTO]
           ,[HIDE_DOC_VERSIONS]
           ,[TS_INSERIMENTO]
           ,[VAR_NOTE_SEC])
	-- Documenti
	SELECT DISTINCT
		Profile_ID
		, @peopleID
		, @accessRights_SOLA_LETTURA
		, NULL
		, @tipoDiritto_ACQUISITO
		, NULL
		, NULL
		, @systemID
    FROM ARCHIVE_AuthorizedObject
    WHERE Authorization_ID = @systemID
    AND NOT Profile_ID IS NULL
    UNION
    -- Fascicoli
	SELECT DISTINCT
		ao.Project_ID
		, @peopleID
		, @accessRights_SOLA_LETTURA
		, NULL
		, @tipoDiritto_ACQUISITO
		, NULL
		, NULL
		, @systemID
    FROM ARCHIVE_AuthorizedObject ao
    WHERE ao.Authorization_ID = @systemID
    AND NOT ao.Project_ID IS NULL
    UNION
    -- Fascicoli (Root folder)
	SELECT DISTINCT
		p.SYSTEM_ID
		, @peopleID
		, @accessRights_SOLA_LETTURA
		, NULL
		, @tipoDiritto_ACQUISITO
		, NULL
		, NULL
		, @systemID
    FROM ARCHIVE_AuthorizedObject ao INNER JOIN PROJECT p ON ao.Project_ID = p.ID_FASCICOLO
    WHERE ao.Authorization_ID = @systemID
    AND NOT ao.Project_ID IS NULL
    UNION
    -- Documenti contenuti nei fascicoli
	SELECT DISTINCT 
		SYSTEM_ID 
		, @peopleID
		, @accessRights_SOLA_LETTURA
		, NULL
		, @tipoDiritto_ACQUISITO
		, NULL
		, NULL
		, @systemID
	FROM PROFILE
	WHERE SYSTEM_ID IN
		(
		SELECT DISTINCT LINK ID_DOCUMENTO
		FROM PROJECT_COMPONENTS PC
		WHERE PC.PROJECT_ID IN
			(
			SELECT P.SYSTEM_ID FROM PROJECT P WHERE P.ID_FASCICOLO IN 
				(
				SELECT PROJECT_ID FROM ARCHIVE_AUTHORIZEDOBJECT 
				WHERE AUTHORIZATION_ID = @systemID
				)
			)
		)
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento degli oggetti autorizzati nella tabella SECURITY' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END

	
	
	--6 - AreaDiLavoro (ADL): inserire una riga per ogni oggetto autorizzato
	--	(
	--	 SYSTEM_ID = Identity
	--	,ID_PEOPLE = @peopleID
	--	,ID_RUOLO_IN_UO = Id del ruolo consultatore (idCorrGlobali)
	--	,ID_PROFILE = oggetto da autorizzare
	--	,ID_PROJECT = oggetto da autorizzare
	--	,DTA_INS = getdate()
	--	,CHA_TIPO_DOC = PROFILE.CHA_TIPO_DOC
	--	,CHA_TIPO_FASC = PROJECT.CHA_TIPO_FASC
	--	,ID_REGISTRO = registro del doc/fasc
	--	)
	--
	DELETE FROM [DPA_AREA_LAVORO]
	WHERE [ID_PEOPLE] = @peopleID
	
	INSERT INTO [DPA_AREA_LAVORO]
           ([ID_PEOPLE]
           ,[ID_RUOLO_IN_UO]
           ,[ID_PROFILE]
           ,[ID_PROJECT]
           ,[DTA_INS]
           ,[CHA_TIPO_DOC]
           ,[CHA_TIPO_FASC]
           ,[ID_REGISTRO])
	SELECT DISTINCT
		@peopleID
		, @idCorrGlobaliRuoloConsultatore --FN_ARCHIVE_getCorrGlobaliIDForRuoloConsultatore()
		, A.PROFILE_ID
		, A.PROJECT_ID
		, GETDATE()
		, CASE WHEN (A.PROFILE_ID IS NULL) THEN NULL ELSE D.CHA_TIPO_PROTO END CHA_TIPO_DOC
		, CASE WHEN (A.PROJECT_ID IS NULL) THEN NULL ELSE F.CHA_TIPO_FASCICOLO END CHA_TIPO_FASC
		, CASE WHEN (A.PROFILE_ID IS NULL) THEN F.ID_REGISTRO ELSE D.ID_REGISTRO END ID_REGISTRO
	FROM ARCHIVE_AUTHORIZEDOBJECT A
	LEFT OUTER JOIN PROFILE D ON A.PROFILE_ID = D.SYSTEM_ID
	LEFT OUTER JOIN PROJECT F ON A.PROJECT_ID = F.SYSTEM_ID
	WHERE AUTHORIZATION_ID = @systemID
			
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento degli oggetti autorizzati nella tabella DPA_AREA_LAVORO' + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END



	COMMIT TRANSACTION T1



END
GO
