use [PCM_DEPOSITO_FINGER]
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Giovanni Olivari
-- Create date: 11/04/2013
-- Description:	Init delle tabelle di base
-- =============================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_InitDatabase
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sql_string nvarchar(MAX)
	DECLARE @nomeSchemaCorrente varchar(200) 
	DECLARE @nomeUtenteCorrente varchar(200) 
	DECLARE @nomeUtenteDeposito varchar(200) 
	DECLARE @idAmministrazione varchar(10)

	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	
	
	
	-- Prima di eseguire l'aggiornamento delle info di configurazione base, 
	-- viene eseguita la SP che effetua una "puliza" delle autorizzazioni alla consultazione scadute
	--
	EXECUTE sp_ARCHIVE_BE_DeleteExpiredAuthorization

	
		
	-- Lettura parametri di configurazione
	--
	SELECT @nomeSchemaCorrente=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_SCHEMA_CORRENTE'
	
	SELECT @nomeUtenteCorrente=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_UTENTE_CORRENTE'
	
	SELECT @nomeUtenteDeposito=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_UTENTE_DEPOSITO'
	
	print 'Nome schema corrente: ' + @nomeSchemaCorrente
	print 'Nome utente corrente: ' + @nomeUtenteCorrente
	print 'Nome utente deposito: ' + @nomeUtenteDeposito
	
	SET @nomeSchemaCorrente = @nomeSchemaCorrente + '.' + @nomeUtenteCorrente
	print 'Schema/Utente corrente: ' + @nomeSchemaCorrente
	

	
	BEGIN TRANSACTION T1
	
	
	-- *********************************************************************************
	-- Aggiornamento dati che non dipendeno dall'amministrazione configurata
	-- *********************************************************************************
	
	
	
	-- ***************
	-- TIPO_COMPONENTI
	-- ***************

	SET @sql_string = CAST(N'MERGE TIPO_COMPONENTI AS TARGET
		USING ( 	
			SELECT 
				[CHA_TIPO_COMPONENTI] COLLATE Latin1_General_CI_AI
				,[DESCRIZIONE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[TIPO_COMPONENTI]
			)
		AS SOURCE ([CHA_TIPO_COMPONENTI]
				,[DESCRIZIONE])
		ON (TARGET.CHA_TIPO_COMPONENTI = SOURCE.CHA_TIPO_COMPONENTI)
		WHEN MATCHED THEN
			UPDATE SET
				[CHA_TIPO_COMPONENTI] = SOURCE.[CHA_TIPO_COMPONENTI]
				,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
		WHEN NOT MATCHED THEN
			INSERT (
				[CHA_TIPO_COMPONENTI]
				,[DESCRIZIONE]
				)
			VALUES (
				SOURCE.[CHA_TIPO_COMPONENTI]
				,SOURCE.[DESCRIZIONE]
			);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella TIPO_COMPONENTI'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella TIPO_COMPONENTI'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject			
			
	
	
	-- *********************
	-- DPA_LETTERE_DOCUMENTI
	-- *********************
	
	SET @sql_string = CAST(N'MERGE DPA_LETTERE_DOCUMENTI AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[CODICE]
				,[DESCRIZIONE]
				,[ETICHETTA]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_LETTERE_DOCUMENTI]
			)
		AS SOURCE ([SYSTEM_ID]
				,[CODICE]
				,[DESCRIZIONE]
				,[ETICHETTA])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[CODICE] = SOURCE.[CODICE]
				,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
				,[ETICHETTA] = SOURCE.[ETICHETTA]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[CODICE]
				,[DESCRIZIONE]
				,[ETICHETTA]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[CODICE]
				,SOURCE.[DESCRIZIONE]
				,SOURCE.[ETICHETTA]
			);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_LETTERE_DOCUMENTI'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_LETTERE_DOCUMENTI'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	-- *************
	-- DOCUMENTTYPES
	-- *************
	
	SET @sql_string = CAST(N'MERGE DOCUMENTTYPES AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[TYPE_ID]
				,[DESCRIPTION]
				,[DISABLED]
				,[STORAGE_TYPE]
				,[RETENTION_DAYS]
				,[MAX_VERSIONS]
				,[MAX_SUBVERSIONS]
				,[FULL_TEXT]
				,[TARGET_DOCSRVR]
				,[RET_2]
				,[RET_2_TYPE]
				,[KEEP_CRITERIA]
				,[VERSIONS_TO_KEEP]
				,[LAST_UPDATE]
				,[CHA_TIPO_CANALE]
				,[DELETED]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DOCUMENTTYPES]
			)
		AS SOURCE ([SYSTEM_ID]
				,[TYPE_ID]
				,[DESCRIPTION]
				,[DISABLED]
				,[STORAGE_TYPE]
				,[RETENTION_DAYS]
				,[MAX_VERSIONS]
				,[MAX_SUBVERSIONS]
				,[FULL_TEXT]
				,[TARGET_DOCSRVR]
				,[RET_2]
				,[RET_2_TYPE]
				,[KEEP_CRITERIA]
				,[VERSIONS_TO_KEEP]
				,[LAST_UPDATE]
				,[CHA_TIPO_CANALE]
				,[DELETED])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[TYPE_ID] = SOURCE.[TYPE_ID]
				,[DESCRIPTION] = SOURCE.[DESCRIPTION]
				,[DISABLED] = SOURCE.[DISABLED]
				,[STORAGE_TYPE] = SOURCE.[STORAGE_TYPE]
				,[RETENTION_DAYS] = SOURCE.[RETENTION_DAYS]
				,[MAX_VERSIONS] = SOURCE.[MAX_VERSIONS]
				,[MAX_SUBVERSIONS] = SOURCE.[MAX_SUBVERSIONS]
				,[FULL_TEXT] = SOURCE.[FULL_TEXT]
				,[TARGET_DOCSRVR] = SOURCE.[TARGET_DOCSRVR]
				,[RET_2] = SOURCE.[RET_2]
				,[RET_2_TYPE] = SOURCE.[RET_2_TYPE]
				,[KEEP_CRITERIA] = SOURCE.[KEEP_CRITERIA]
				,[VERSIONS_TO_KEEP] = SOURCE.[VERSIONS_TO_KEEP]
				,[LAST_UPDATE] = SOURCE.[LAST_UPDATE]
				,[CHA_TIPO_CANALE] = SOURCE.[CHA_TIPO_CANALE]
				,[DELETED] = SOURCE.[DELETED]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[TYPE_ID]
				,[DESCRIPTION]
				,[DISABLED]
				,[STORAGE_TYPE]
				,[RETENTION_DAYS]
				,[MAX_VERSIONS]
				,[MAX_SUBVERSIONS]
				,[FULL_TEXT]
				,[TARGET_DOCSRVR]
				,[RET_2]
				,[RET_2_TYPE]
				,[KEEP_CRITERIA]
				,[VERSIONS_TO_KEEP]
				,[LAST_UPDATE]
				,[CHA_TIPO_CANALE]
				,[DELETED]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[TYPE_ID]
				,SOURCE.[DESCRIPTION]
				,SOURCE.[DISABLED]
				,SOURCE.[STORAGE_TYPE]
				,SOURCE.[RETENTION_DAYS]
				,SOURCE.[MAX_VERSIONS]
				,SOURCE.[MAX_SUBVERSIONS]
				,SOURCE.[FULL_TEXT]
				,SOURCE.[TARGET_DOCSRVR]
				,SOURCE.[RET_2]
				,SOURCE.[RET_2_TYPE]
				,SOURCE.[KEEP_CRITERIA]
				,SOURCE.[VERSIONS_TO_KEEP]
				,SOURCE.[LAST_UPDATE]
				,SOURCE.[CHA_TIPO_CANALE]
				,SOURCE.[DELETED]
			);' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DOCUMENTTYPES'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DOCUMENTTYPES'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject			


	
	-- *********************
	-- DPA_FORMATI_DOCUMENTO
	-- *********************
	
	SET @sql_string = CAST(N'MERGE DPA_FORMATI_DOCUMENTO AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[ID_AMMINISTRAZIONE]
				,[FILE_TYPE_USED]
				,[DESCRIPTION]
				,[FILE_EXTENSION]
				,[MAX_FILE_SIZE]
				,[MAX_FILE_SIZE_ALERT_MODE]
				,[CONTAINS_FILE_MODEL]
				,[DOCUMENT_TYPE]
				,[FILE_TYPE_SIGNATURE]
				,[FILE_TYPE_PRESERVATION]
				,[FILE_TYPE_VALIDATION]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_FORMATI_DOCUMENTO]
			)
		AS SOURCE ([SYSTEM_ID]
				,[ID_AMMINISTRAZIONE]
				,[FILE_TYPE_USED]
				,[DESCRIPTION]
				,[FILE_EXTENSION]
				,[MAX_FILE_SIZE]
				,[MAX_FILE_SIZE_ALERT_MODE]
				,[CONTAINS_FILE_MODEL]
				,[DOCUMENT_TYPE]
				,[FILE_TYPE_SIGNATURE]
				,[FILE_TYPE_PRESERVATION]
				,[FILE_TYPE_VALIDATION])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[ID_AMMINISTRAZIONE] = SOURCE.[ID_AMMINISTRAZIONE]
				,[FILE_TYPE_USED] = SOURCE.[FILE_TYPE_USED]
				,[DESCRIPTION] = SOURCE.[DESCRIPTION]
				,[FILE_EXTENSION] = SOURCE.[FILE_EXTENSION]
				,[MAX_FILE_SIZE] = SOURCE.[MAX_FILE_SIZE]
				,[MAX_FILE_SIZE_ALERT_MODE] = SOURCE.[MAX_FILE_SIZE_ALERT_MODE]
				,[CONTAINS_FILE_MODEL] = SOURCE.[CONTAINS_FILE_MODEL]
				,[DOCUMENT_TYPE] = SOURCE.[DOCUMENT_TYPE]
				,[FILE_TYPE_SIGNATURE] = SOURCE.[FILE_TYPE_SIGNATURE]
				,[FILE_TYPE_PRESERVATION] = SOURCE.[FILE_TYPE_PRESERVATION]
				,[FILE_TYPE_VALIDATION] = SOURCE.[FILE_TYPE_VALIDATION]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[ID_AMMINISTRAZIONE]
				,[FILE_TYPE_USED]
				,[DESCRIPTION]
				,[FILE_EXTENSION]
				,[MAX_FILE_SIZE]
				,[MAX_FILE_SIZE_ALERT_MODE]
				,[CONTAINS_FILE_MODEL]
				,[DOCUMENT_TYPE]
				,[FILE_TYPE_SIGNATURE]
				,[FILE_TYPE_PRESERVATION]
				,[FILE_TYPE_VALIDATION]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[ID_AMMINISTRAZIONE]
				,SOURCE.[FILE_TYPE_USED]
				,SOURCE.[DESCRIPTION]
				,SOURCE.[FILE_EXTENSION]
				,SOURCE.[MAX_FILE_SIZE]
				,SOURCE.[MAX_FILE_SIZE_ALERT_MODE]
				,SOURCE.[CONTAINS_FILE_MODEL]
				,SOURCE.[DOCUMENT_TYPE]
				,SOURCE.[FILE_TYPE_SIGNATURE]
				,SOURCE.[FILE_TYPE_PRESERVATION]
				,SOURCE.[FILE_TYPE_VALIDATION]
			);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_FORMATI_DOCUMENTO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_FORMATI_DOCUMENTO'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	
	
	
	-- **************
	-- Anagrafica Log
	-- **************

	SET @sql_string = CAST(N'MERGE DPA_ANAGRAFICA_LOG AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[VAR_CODICE]
				,[VAR_DESCRIZIONE]
				,[VAR_OGGETTO]
				,[VAR_METODO]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ANAGRAFICA_LOG]
			)
		AS SOURCE ([SYSTEM_ID]
				,[VAR_CODICE]
				,[VAR_DESCRIZIONE]
				,[VAR_OGGETTO]
				,[VAR_METODO])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[VAR_CODICE] = SOURCE.[VAR_CODICE]
				,[VAR_DESCRIZIONE] = SOURCE.[VAR_DESCRIZIONE]
				,[VAR_OGGETTO] = SOURCE.[VAR_OGGETTO]
				,[VAR_METODO] = SOURCE.[VAR_METODO]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[VAR_CODICE]
				,[VAR_DESCRIZIONE]
				,[VAR_OGGETTO]
				,[VAR_METODO])
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[VAR_CODICE]
				,SOURCE.[VAR_DESCRIZIONE]
				,SOURCE.[VAR_OGGETTO]
				,SOURCE.[VAR_METODO]
			);' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	--EXECUTE sp_executesql @sql_string; Disabilitata perchè in conflitto con l'IDENTITY

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_ANAGRAFICA_LOG'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_ANAGRAFICA_LOG'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	
	
	
	-- ****************
	-- DPA_CARAT_TIMBRO
	-- ****************

	SET @sql_string = CAST(N'MERGE DPA_CARAT_TIMBRO AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[VAR_NOME]
				,[DIMENSIONE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_CARAT_TIMBRO]
			)
		AS SOURCE ([SYSTEM_ID]
				,[VAR_NOME]
				,[DIMENSIONE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[VAR_NOME] = SOURCE.[VAR_NOME]
				,[DIMENSIONE] = SOURCE.[DIMENSIONE]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[VAR_NOME]
				,[DIMENSIONE]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[VAR_NOME]
				,SOURCE.[DIMENSIONE]
			);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_CARAT_TIMBRO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_CARAT_TIMBRO'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- *****************
	-- DPA_COLORE_TIMBRO
	-- *****************

	SET @sql_string = CAST(N'MERGE DPA_COLORE_TIMBRO AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[VAR_NOME]
				,[DESCRIZIONE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_COLORE_TIMBRO]
			)
		AS SOURCE ([SYSTEM_ID]
				,[VAR_NOME]
				,[DESCRIZIONE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[VAR_NOME] = SOURCE.[VAR_NOME]
				,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[VAR_NOME]
				,[DESCRIZIONE]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[VAR_NOME]
				,SOURCE.[DESCRIZIONE]
			);' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_COLORE_TIMBRO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_COLORE_TIMBRO'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject			
			
	
	
	-- ****************
	-- DPA_POSIZ_TIMBRO
	-- ****************
	
	SET @sql_string = CAST(N'MERGE DPA_POSIZ_TIMBRO AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[TIPO_POS]
				,[POS_X]
				,[POS_Y]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_POSIZ_TIMBRO]
			)
		AS SOURCE ([SYSTEM_ID]
				,[TIPO_POS]
				,[POS_X]
				,[POS_Y])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[TIPO_POS] = SOURCE.[TIPO_POS]
				,[POS_X] = SOURCE.[POS_X]
				,[POS_Y] = SOURCE.[POS_Y]
		WHEN NOT MATCHED THEN
			INSERT (
				[SYSTEM_ID]
				,[TIPO_POS]
				,[POS_X]
				,[POS_Y]
				)
			VALUES (
				SOURCE.[SYSTEM_ID]
				,SOURCE.[TIPO_POS]
				,SOURCE.[POS_X]
				,SOURCE.[POS_Y]
			);' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_POSIZ_TIMBRO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_POSIZ_TIMBRO'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	
		
	
	-- **********************
	-- Tipo oggetto Documento
	-- **********************

	SET @sql_string = CAST(N'MERGE DPA_TIPO_OGGETTO AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_TIPO_OGGETTO]
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[TIPO] = SOURCE.[TIPO]
				  ,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[TIPO]
				  ,SOURCE.[DESCRIZIONE]);' AS NVARCHAR(MAX))
				  
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_TIPO_OGGETTO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_TIPO_OGGETTO'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- **********************
	-- Tipo oggetto Fascicolo
	-- **********************

	SET @sql_string = CAST(N'MERGE DPA_TIPO_OGGETTO_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_TIPO_OGGETTO_FASC]
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[TIPO] = SOURCE.[TIPO]
				  ,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[TIPO]
				  ,[DESCRIZIONE])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[TIPO]
				  ,SOURCE.[DESCRIZIONE]);' AS NVARCHAR(MAX))
				  
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_TIPO_OGGETTO_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_TIPO_OGGETTO_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- ******************
	-- DPA_OGGETTI_CUSTOM
	-- ******************
	
	SET @sql_string = CAST(N'MERGE DPA_OGGETTI_CUSTOM AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[NOME]
				,[DESCRIZIONE]
				,[ORIZZONTALE_VERTICALE]
				,[CAMPO_OBBLIGATORIO]
				,[MULTILINEA]
				,[NUMERO_DI_LINEE]
				,[NUMERO_DI_CARATTERI]
				,[CAMPO_DI_RICERCA]
				,[POSIZIONE]
				,[ID_TIPO_OGGETTO]
				,[RESET_ANNO]
				,[FORMATO_CONTATORE]
				,[ID_R_DEFAULT]
				,[RICERCA_CORR]
				,[CAMPO_COMUNE]
				,[CHA_TIPO_TAR]
				,[CONTA_DOPO]
				,[REPERTORIO]
				,[DA_VISUALIZZARE_RICERCA]
				,[FORMATO_ORA]
				,[TIPO_LINK]
				,[TIPO_OBJ_LINK]
				,[CONFIG_OBJ_EST]
				,[MODULO_SOTTOCONTATORE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_OGGETTI_CUSTOM]
			)
		AS SOURCE ([SYSTEM_ID]
				,[NOME]
				,[DESCRIZIONE]
				,[ORIZZONTALE_VERTICALE]
				,[CAMPO_OBBLIGATORIO]
				,[MULTILINEA]
				,[NUMERO_DI_LINEE]
				,[NUMERO_DI_CARATTERI]
				,[CAMPO_DI_RICERCA]
				,[POSIZIONE]
				,[ID_TIPO_OGGETTO]
				,[RESET_ANNO]
				,[FORMATO_CONTATORE]
				,[ID_R_DEFAULT]
				,[RICERCA_CORR]
				,[CAMPO_COMUNE]
				,[CHA_TIPO_TAR]
				,[CONTA_DOPO]
				,[REPERTORIO]
				,[DA_VISUALIZZARE_RICERCA]
				,[FORMATO_ORA]
				,[TIPO_LINK]
				,[TIPO_OBJ_LINK]
				,[CONFIG_OBJ_EST]
				,[MODULO_SOTTOCONTATORE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[NOME] = SOURCE.[NOME]
				,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
				,[ORIZZONTALE_VERTICALE] = SOURCE.[ORIZZONTALE_VERTICALE]
				,[CAMPO_OBBLIGATORIO] = SOURCE.[CAMPO_OBBLIGATORIO]
				,[MULTILINEA] = SOURCE.[MULTILINEA]
				,[NUMERO_DI_LINEE] = SOURCE.[NUMERO_DI_LINEE]
				,[NUMERO_DI_CARATTERI] = SOURCE.[NUMERO_DI_CARATTERI]
				,[CAMPO_DI_RICERCA] = SOURCE.[CAMPO_DI_RICERCA]
				,[POSIZIONE] = SOURCE.[POSIZIONE]
				,[ID_TIPO_OGGETTO] = SOURCE.[ID_TIPO_OGGETTO]
				,[RESET_ANNO] = SOURCE.[RESET_ANNO]
				,[FORMATO_CONTATORE] = SOURCE.[FORMATO_CONTATORE]
				,[ID_R_DEFAULT] = SOURCE.[ID_R_DEFAULT]
				,[RICERCA_CORR] = SOURCE.[RICERCA_CORR]
				,[CAMPO_COMUNE] = SOURCE.[CAMPO_COMUNE]
				,[CHA_TIPO_TAR] = SOURCE.[CHA_TIPO_TAR]
				,[CONTA_DOPO] = SOURCE.[CONTA_DOPO]
				,[REPERTORIO] = SOURCE.[REPERTORIO]
				,[DA_VISUALIZZARE_RICERCA] = SOURCE.[DA_VISUALIZZARE_RICERCA]
				,[FORMATO_ORA] = SOURCE.[FORMATO_ORA]
				,[TIPO_LINK] = SOURCE.[TIPO_LINK]
				,[TIPO_OBJ_LINK] = SOURCE.[TIPO_OBJ_LINK]
				,[CONFIG_OBJ_EST] = SOURCE.[CONFIG_OBJ_EST]
				,[MODULO_SOTTOCONTATORE] = SOURCE.[MODULO_SOTTOCONTATORE]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[NOME]
				,[DESCRIZIONE]
				,[ORIZZONTALE_VERTICALE]
				,[CAMPO_OBBLIGATORIO]
				,[MULTILINEA]
				,[NUMERO_DI_LINEE]
				,[NUMERO_DI_CARATTERI]
				,[CAMPO_DI_RICERCA]
				,[POSIZIONE]
				,[ID_TIPO_OGGETTO]
				,[RESET_ANNO]
				,[FORMATO_CONTATORE]
				,[ID_R_DEFAULT]
				,[RICERCA_CORR]
				,[CAMPO_COMUNE]
				,[CHA_TIPO_TAR]
				,[CONTA_DOPO]
				,[REPERTORIO]
				,[DA_VISUALIZZARE_RICERCA]
				,[FORMATO_ORA]
				,[TIPO_LINK]
				,[TIPO_OBJ_LINK]
				,[CONFIG_OBJ_EST]
				,[MODULO_SOTTOCONTATORE])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[NOME]
				,SOURCE.[DESCRIZIONE]
				,SOURCE.[ORIZZONTALE_VERTICALE]
				,SOURCE.[CAMPO_OBBLIGATORIO]
				,SOURCE.[MULTILINEA]
				,SOURCE.[NUMERO_DI_LINEE]
				,SOURCE.[NUMERO_DI_CARATTERI]
				,SOURCE.[CAMPO_DI_RICERCA]
				,SOURCE.[POSIZIONE]
				,SOURCE.[ID_TIPO_OGGETTO]
				,SOURCE.[RESET_ANNO]
				,SOURCE.[FORMATO_CONTATORE]
				,SOURCE.[ID_R_DEFAULT]
				,SOURCE.[RICERCA_CORR]
				,SOURCE.[CAMPO_COMUNE]
				,SOURCE.[CHA_TIPO_TAR]
				,SOURCE.[CONTA_DOPO]
				,SOURCE.[REPERTORIO]
				,SOURCE.[DA_VISUALIZZARE_RICERCA]
				,SOURCE.[FORMATO_ORA]
				,SOURCE.[TIPO_LINK]
				,SOURCE.[TIPO_OBJ_LINK]
				,SOURCE.[CONFIG_OBJ_EST]
				,SOURCE.[MODULO_SOTTOCONTATORE]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_OGGETTI_CUSTOM'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_OGGETTI_CUSTOM'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- ***********************
	-- DPA_ASSOCIAZIONE_VALORI
	-- ***********************
	
	SET @sql_string = CAST(N'MERGE DPA_ASSOCIAZIONE_VALORI AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[DESCRIZIONE_VALORE]
				,[VALORE]
				,[VALORE_DI_DEFAULT]
				,[ID_OGGETTO_CUSTOM]
				,[ABILITATO]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ASSOCIAZIONE_VALORI]
			)
		AS SOURCE ([SYSTEM_ID]
				,[DESCRIZIONE_VALORE]
				,[VALORE]
				,[VALORE_DI_DEFAULT]
				,[ID_OGGETTO_CUSTOM]
				,[ABILITATO])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[DESCRIZIONE_VALORE] = SOURCE.[DESCRIZIONE_VALORE]
				,[VALORE] = SOURCE.[VALORE]
				,[VALORE_DI_DEFAULT] = SOURCE.[VALORE_DI_DEFAULT]
				,[ID_OGGETTO_CUSTOM] = SOURCE.[ID_OGGETTO_CUSTOM]
				,[ABILITATO] = SOURCE.[ABILITATO]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[DESCRIZIONE_VALORE]
				,[VALORE]
				,[VALORE_DI_DEFAULT]
				,[ID_OGGETTO_CUSTOM]
				,[ABILITATO])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[DESCRIZIONE_VALORE]
				,SOURCE.[VALORE]
				,SOURCE.[VALORE_DI_DEFAULT]
				,SOURCE.[ID_OGGETTO_CUSTOM]
				,SOURCE.[ABILITATO]);' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_ASSOCIAZIONE_VALORI'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_ASSOCIAZIONE_VALORI'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject					
	
	
	
	-- ***********************
	-- DPA_OGGETTI_CUSTOM_FASC
	-- ***********************
	
	SET @sql_string = CAST(N'MERGE DPA_OGGETTI_CUSTOM_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[NOME]
				  ,[DESCRIZIONE]
				  ,[ORIZZONTALE_VERTICALE]
				  ,[CAMPO_OBBLIGATORIO]
				  ,[MULTILINEA]
				  ,[NUMERO_DI_LINEE]
				  ,[NUMERO_DI_CARATTERI]
				  ,[CAMPO_DI_RICERCA]
				  ,[POSIZIONE]
				  ,[ID_TIPO_OGGETTO]
				  ,[RESET_ANNO]
				  ,[FORMATO_CONTATORE]
				  ,[ID_R_DEFAULT]
				  ,[RICERCA_CORR]
				  ,[CAMPO_COMUNE]
				  ,[CHA_TIPO_TAR]
				  ,[CONTA_DOPO]
				  ,[REPERTORIO]
				  ,[DA_VISUALIZZARE_RICERCA]
				  ,[FORMATO_ORA]
				  ,[TIPO_LINK]
				  ,[TIPO_OBJ_LINK]
				  ,[CONFIG_OBJ_EST]
				  ,[MODULO_SOTTOCONTATORE]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_OGGETTI_CUSTOM_FASC]
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[NOME]
				  ,[DESCRIZIONE]
				  ,[ORIZZONTALE_VERTICALE]
				  ,[CAMPO_OBBLIGATORIO]
				  ,[MULTILINEA]
				  ,[NUMERO_DI_LINEE]
				  ,[NUMERO_DI_CARATTERI]
				  ,[CAMPO_DI_RICERCA]
				  ,[POSIZIONE]
				  ,[ID_TIPO_OGGETTO]
				  ,[RESET_ANNO]
				  ,[FORMATO_CONTATORE]
				  ,[ID_R_DEFAULT]
				  ,[RICERCA_CORR]
				  ,[CAMPO_COMUNE]
				  ,[CHA_TIPO_TAR]
				  ,[CONTA_DOPO]
				  ,[REPERTORIO]
				  ,[DA_VISUALIZZARE_RICERCA]
				  ,[FORMATO_ORA]
				  ,[TIPO_LINK]
				  ,[TIPO_OBJ_LINK]
				  ,[CONFIG_OBJ_EST]
				  ,[MODULO_SOTTOCONTATORE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[NOME] = SOURCE.[NOME]
				  ,[DESCRIZIONE] = SOURCE.[DESCRIZIONE]
				  ,[ORIZZONTALE_VERTICALE] = SOURCE.[ORIZZONTALE_VERTICALE]
				  ,[CAMPO_OBBLIGATORIO] = SOURCE.[CAMPO_OBBLIGATORIO]
				  ,[MULTILINEA] = SOURCE.[MULTILINEA]
				  ,[NUMERO_DI_LINEE] = SOURCE.[NUMERO_DI_LINEE]
				  ,[NUMERO_DI_CARATTERI] = SOURCE.[NUMERO_DI_CARATTERI]
				  ,[CAMPO_DI_RICERCA] = SOURCE.[CAMPO_DI_RICERCA]
				  ,[POSIZIONE] = SOURCE.[POSIZIONE]
				  ,[ID_TIPO_OGGETTO] = SOURCE.[ID_TIPO_OGGETTO]
				  ,[RESET_ANNO] = SOURCE.[RESET_ANNO]
				  ,[FORMATO_CONTATORE] = SOURCE.[FORMATO_CONTATORE]
				  ,[ID_R_DEFAULT] = SOURCE.[ID_R_DEFAULT]
				  ,[RICERCA_CORR] = SOURCE.[RICERCA_CORR]
				  ,[CAMPO_COMUNE] = SOURCE.[CAMPO_COMUNE]
				  ,[CHA_TIPO_TAR] = SOURCE.[CHA_TIPO_TAR]
				  ,[CONTA_DOPO] = SOURCE.[CONTA_DOPO]
				  ,[REPERTORIO] = SOURCE.[REPERTORIO]
				  ,[DA_VISUALIZZARE_RICERCA] = SOURCE.[DA_VISUALIZZARE_RICERCA]
				  ,[FORMATO_ORA] = SOURCE.[FORMATO_ORA]
				  ,[TIPO_LINK] = SOURCE.[TIPO_LINK]
				  ,[TIPO_OBJ_LINK] = SOURCE.[TIPO_OBJ_LINK]
				  ,[CONFIG_OBJ_EST] = SOURCE.[CONFIG_OBJ_EST]
				  ,[MODULO_SOTTOCONTATORE] = SOURCE.[MODULO_SOTTOCONTATORE]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[NOME]
				  ,[DESCRIZIONE]
				  ,[ORIZZONTALE_VERTICALE]
				  ,[CAMPO_OBBLIGATORIO]
				  ,[MULTILINEA]
				  ,[NUMERO_DI_LINEE]
				  ,[NUMERO_DI_CARATTERI]
				  ,[CAMPO_DI_RICERCA]
				  ,[POSIZIONE]
				  ,[ID_TIPO_OGGETTO]
				  ,[RESET_ANNO]
				  ,[FORMATO_CONTATORE]
				  ,[ID_R_DEFAULT]
				  ,[RICERCA_CORR]
				  ,[CAMPO_COMUNE]
				  ,[CHA_TIPO_TAR]
				  ,[CONTA_DOPO]
				  ,[REPERTORIO]
				  ,[DA_VISUALIZZARE_RICERCA]
				  ,[FORMATO_ORA]
				  ,[TIPO_LINK]
				  ,[TIPO_OBJ_LINK]
				  ,[CONFIG_OBJ_EST]
				  ,[MODULO_SOTTOCONTATORE])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[NOME]
				  ,SOURCE.[DESCRIZIONE]
				  ,SOURCE.[ORIZZONTALE_VERTICALE]
				  ,SOURCE.[CAMPO_OBBLIGATORIO]
				  ,SOURCE.[MULTILINEA]
				  ,SOURCE.[NUMERO_DI_LINEE]
				  ,SOURCE.[NUMERO_DI_CARATTERI]
				  ,SOURCE.[CAMPO_DI_RICERCA]
				  ,SOURCE.[POSIZIONE]
				  ,SOURCE.[ID_TIPO_OGGETTO]
				  ,SOURCE.[RESET_ANNO]
				  ,SOURCE.[FORMATO_CONTATORE]
				  ,SOURCE.[ID_R_DEFAULT]
				  ,SOURCE.[RICERCA_CORR]
				  ,SOURCE.[CAMPO_COMUNE]
				  ,SOURCE.[CHA_TIPO_TAR]
				  ,SOURCE.[CONTA_DOPO]
				  ,SOURCE.[REPERTORIO]
				  ,SOURCE.[DA_VISUALIZZARE_RICERCA]
				  ,SOURCE.[FORMATO_ORA]
				  ,SOURCE.[TIPO_LINK]
				  ,SOURCE.[TIPO_OBJ_LINK]
				  ,SOURCE.[CONFIG_OBJ_EST]
				  ,SOURCE.[MODULO_SOTTOCONTATORE]);' AS NVARCHAR(MAX))
				  	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_OGGETTI_CUSTOM_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_OGGETTI_CUSTOM_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	
	
	
	-- *******************
	-- DPA_ASS_VALORI_FASC
	-- *******************

	SET @sql_string = CAST(N'MERGE DPA_ASS_VALORI_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[DESCRIZIONE_VALORE]
				  ,[VALORE]
				  ,[VALORE_DI_DEFAULT]
				  ,[ID_OGGETTO_CUSTOM]
				  ,[ABILITATO]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ASS_VALORI_FASC]
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[DESCRIZIONE_VALORE]
				  ,[VALORE]
				  ,[VALORE_DI_DEFAULT]
				  ,[ID_OGGETTO_CUSTOM]
				  ,[ABILITATO])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[DESCRIZIONE_VALORE] = SOURCE.[DESCRIZIONE_VALORE]
				  ,[VALORE] = SOURCE.[VALORE]
				  ,[VALORE_DI_DEFAULT] = SOURCE.[VALORE_DI_DEFAULT]
				  ,[ID_OGGETTO_CUSTOM] = SOURCE.[ID_OGGETTO_CUSTOM]
				  ,[ABILITATO] = SOURCE.[ABILITATO]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[DESCRIZIONE_VALORE]
				  ,[VALORE]
				  ,[VALORE_DI_DEFAULT]
				  ,[ID_OGGETTO_CUSTOM]
				  ,[ABILITATO])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[DESCRIZIONE_VALORE]
				  ,SOURCE.[VALORE]
				  ,SOURCE.[VALORE_DI_DEFAULT]
				  ,SOURCE.[ID_OGGETTO_CUSTOM]
				  ,SOURCE.[ABILITATO]);' AS NVARCHAR(MAX))
		
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_ASS_VALORI_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_ASS_VALORI_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject		
	
	
	
	-- *********************
	-- Tipologia documentale
	-- *********************

	SET @sql_string = CAST(N'MERGE DPA_TIPO_ATTO AS TARGET
		USING (SELECT SYSTEM_ID
				,VAR_DESC_ATTO
				,ID_AMM
				,ABILITATO_SI_NO
				,IN_ESERCIZIO
				,PATH_MOD_1
				,EXT_MOD_1
				,PATH_MOD_2
				,EXT_MOD_2
				,GG_SCADENZA
				,GG_PRE_SCADENZA
				,CHA_PRIVATO
				,IPERDOCUMENTO
				,PATH_ALL_1
				,EXT_ALL_1
				,PATH_MOD_SU
				,COD_MOD_TRASM
				,COD_CLASS
				,PATH_MOD_EXC
				,NUM_MESI_CONSERVAZIONE
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
			+ CAST(N'.DPA_TIPO_ATTO WHERE ID_AMM IS NULL) 
		AS SOURCE (SYSTEM_ID
				,VAR_DESC_ATTO
				,ID_AMM
				,ABILITATO_SI_NO
				,IN_ESERCIZIO
				,PATH_MOD_1
				,EXT_MOD_1
				,PATH_MOD_2
				,EXT_MOD_2
				,GG_SCADENZA
				,GG_PRE_SCADENZA
				,CHA_PRIVATO
				,IPERDOCUMENTO
				,PATH_ALL_1
				,EXT_ALL_1
				,PATH_MOD_SU
				,COD_MOD_TRASM
				,COD_CLASS
				,PATH_MOD_EXC
				,NUM_MESI_CONSERVAZIONE)
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				VAR_DESC_ATTO = SOURCE.VAR_DESC_ATTO
				,ID_AMM = SOURCE.ID_AMM
				,ABILITATO_SI_NO = SOURCE.ABILITATO_SI_NO
				,IN_ESERCIZIO = SOURCE.IN_ESERCIZIO
				,PATH_MOD_1 = SOURCE.PATH_MOD_1
				,EXT_MOD_1 = SOURCE.EXT_MOD_1
				,PATH_MOD_2 = SOURCE.PATH_MOD_2
				,EXT_MOD_2 = SOURCE.EXT_MOD_2
				,GG_SCADENZA = SOURCE.GG_SCADENZA
				,GG_PRE_SCADENZA = SOURCE.GG_PRE_SCADENZA
				,CHA_PRIVATO = SOURCE.CHA_PRIVATO
				,IPERDOCUMENTO = SOURCE.IPERDOCUMENTO
				,PATH_ALL_1 = SOURCE.PATH_ALL_1
				,EXT_ALL_1 = SOURCE.EXT_ALL_1
				,PATH_MOD_SU = SOURCE.PATH_MOD_SU
				,COD_MOD_TRASM = SOURCE.COD_MOD_TRASM
				,COD_CLASS = SOURCE.COD_CLASS
				,PATH_MOD_EXC = SOURCE.PATH_MOD_EXC
				,NUM_MESI_CONSERVAZIONE = SOURCE.NUM_MESI_CONSERVAZIONE
		WHEN NOT MATCHED THEN
			INSERT (SYSTEM_ID
				,VAR_DESC_ATTO
				,ID_AMM
				,ABILITATO_SI_NO
				,IN_ESERCIZIO
				,PATH_MOD_1
				,EXT_MOD_1
				,PATH_MOD_2
				,EXT_MOD_2
				,GG_SCADENZA
				,GG_PRE_SCADENZA
				,CHA_PRIVATO
				,IPERDOCUMENTO
				,PATH_ALL_1
				,EXT_ALL_1
				,PATH_MOD_SU
				,COD_MOD_TRASM
				,COD_CLASS
				,PATH_MOD_EXC
				,NUM_MESI_CONSERVAZIONE)
			VALUES (SOURCE.SYSTEM_ID
				,SOURCE.VAR_DESC_ATTO
				,SOURCE.ID_AMM
				,SOURCE.ABILITATO_SI_NO
				,SOURCE.IN_ESERCIZIO
				,SOURCE.PATH_MOD_1
				,SOURCE.EXT_MOD_1
				,SOURCE.PATH_MOD_2
				,SOURCE.EXT_MOD_2
				,SOURCE.GG_SCADENZA
				,SOURCE.GG_PRE_SCADENZA
				,SOURCE.CHA_PRIVATO
				,SOURCE.IPERDOCUMENTO
				,SOURCE.PATH_ALL_1
				,SOURCE.EXT_ALL_1
				,SOURCE.PATH_MOD_SU
				,SOURCE.COD_MOD_TRASM
				,SOURCE.COD_CLASS
				,SOURCE.PATH_MOD_EXC
				,SOURCE.NUM_MESI_CONSERVAZIONE);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK

		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento delle tipologie documentali non appartenenti ad amministrazioni'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipologie documentali non appartenenti ad amministrazioni'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	-- *******************
	-- Tipologia fascicolo
	-- *******************

	SET @sql_string = CAST(N'MERGE DPA_TIPO_FASC AS TARGET
		USING (SELECT SYSTEM_ID
				,VAR_DESC_FASC
				,ID_AMM
				,ABILITATO_SI_NO
				,IN_ESERCIZIO
				,PATH_MOD_1
				,EXT_MOD_1
				,PATH_MOD_2
				,EXT_MOD_2
				,GG_SCADENZA
				,GG_PRE_SCADENZA
				,CHA_PRIVATO
				,IPERFASCICOLO
				,NUM_MESI_CONSERVAZIONE
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
			+ CAST(N'.DPA_TIPO_FASC WHERE ID_AMM IS NULL) 
		AS SOURCE (SYSTEM_ID
					,VAR_DESC_FASC
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERFASCICOLO
					,NUM_MESI_CONSERVAZIONE)
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				SYSTEM_ID = SOURCE.SYSTEM_ID
				,VAR_DESC_FASC = SOURCE.VAR_DESC_FASC
				,ID_AMM = SOURCE.ID_AMM
				,ABILITATO_SI_NO = SOURCE.ABILITATO_SI_NO
				,IN_ESERCIZIO = SOURCE.IN_ESERCIZIO
				,PATH_MOD_1 = SOURCE.PATH_MOD_1
				,EXT_MOD_1 = SOURCE.EXT_MOD_1
				,PATH_MOD_2 = SOURCE.PATH_MOD_2
				,EXT_MOD_2 = SOURCE.EXT_MOD_2
				,GG_SCADENZA = SOURCE.GG_SCADENZA
				,GG_PRE_SCADENZA = SOURCE.GG_PRE_SCADENZA
				,CHA_PRIVATO = SOURCE.CHA_PRIVATO
				,IPERFASCICOLO = SOURCE.IPERFASCICOLO
				,NUM_MESI_CONSERVAZIONE = SOURCE.NUM_MESI_CONSERVAZIONE
		WHEN NOT MATCHED THEN
			INSERT (SYSTEM_ID
					,VAR_DESC_FASC
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERFASCICOLO
					,NUM_MESI_CONSERVAZIONE)
			VALUES (SOURCE.SYSTEM_ID
					,SOURCE.VAR_DESC_FASC
					,SOURCE.ID_AMM
					,SOURCE.ABILITATO_SI_NO
					,SOURCE.IN_ESERCIZIO
					,SOURCE.PATH_MOD_1
					,SOURCE.EXT_MOD_1
					,SOURCE.PATH_MOD_2
					,SOURCE.EXT_MOD_2
					,SOURCE.GG_SCADENZA
					,SOURCE.GG_PRE_SCADENZA
					,SOURCE.CHA_PRIVATO
					,SOURCE.IPERFASCICOLO
					,SOURCE.NUM_MESI_CONSERVAZIONE);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento delle tipologie fascicoli non appartenenti ad amministrazioni'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipologie fascicoli non appartenenti ad amministrazioni'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	-- ***********************
	-- DPA_ANAGRAFICA_FUNZIONI
	-- ***********************
	
	SET @sql_string = CAST(N'MERGE DPA_ANAGRAFICA_FUNZIONI AS TARGET
		USING ( 	
		SELECT 
			[COD_FUNZIONE] COLLATE Latin1_General_CI_AI
			,[VAR_DESC_FUNZIONE]
			,[CHA_TIPO_FUNZ]
			,[DISABLED] COLLATE Latin1_General_CI_AI
		FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ANAGRAFICA_FUNZIONI]
		) 
		AS SOURCE (
			[COD_FUNZIONE]
			,[VAR_DESC_FUNZIONE]
			,[CHA_TIPO_FUNZ]
			,[DISABLED])
		ON (TARGET.COD_FUNZIONE = SOURCE.COD_FUNZIONE AND TARGET.DISABLED = SOURCE.DISABLED)
		WHEN MATCHED THEN
			UPDATE SET
				[COD_FUNZIONE] = SOURCE.[COD_FUNZIONE]
			  ,[VAR_DESC_FUNZIONE] = SOURCE.[VAR_DESC_FUNZIONE]
			  ,[CHA_TIPO_FUNZ] = SOURCE.[CHA_TIPO_FUNZ]
			  ,[DISABLED] = SOURCE.[DISABLED]
		WHEN NOT MATCHED THEN
			INSERT ([COD_FUNZIONE]
				,[VAR_DESC_FUNZIONE]
				,[CHA_TIPO_FUNZ]
				,[DISABLED])
			VALUES (SOURCE.[COD_FUNZIONE]
				  ,SOURCE.[VAR_DESC_FUNZIONE]
				  ,SOURCE.[CHA_TIPO_FUNZ]
				  ,SOURCE.[DISABLED]);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento dell''anagrafica delle funzioni'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento anagrafica funzioni'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	-- L'aggiornamento della tabella DPA_TIPO_FUNZIONE è stato commentato perchè riabilitata la proprietà IDENTITY
	-- del campo SYSTEM_ID

	-- *****************
	-- DPA_TIPO_FUNZIONE
	-- *****************
	
	--SET @sql_string = CAST(N'MERGE DPA_TIPO_FUNZIONE AS TARGET
	--	USING ( 	
	--	SELECT [SYSTEM_ID]
	--	  ,[VAR_COD_TIPO]
	--	  ,[VAR_DESC_TIPO_FUN]
	--	  ,[CHA_VIS]
	--	  ,[ID_AMM]
	--	FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_TIPO_FUNZIONE]
	--	) 
	--	AS SOURCE ([SYSTEM_ID]
	--	  ,[VAR_COD_TIPO]
	--	  ,[VAR_DESC_TIPO_FUN]
	--	  ,[CHA_VIS]
	--	  ,[ID_AMM])
	--	ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
	--	WHEN MATCHED THEN
	--		UPDATE SET
	--			[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
	--		  ,[VAR_COD_TIPO] = SOURCE.[VAR_COD_TIPO]
	--		  ,[VAR_DESC_TIPO_FUN] = SOURCE.[VAR_DESC_TIPO_FUN]
	--		  ,[CHA_VIS] = SOURCE.[CHA_VIS]
	--		  ,[ID_AMM] = SOURCE.[ID_AMM]
	--	WHEN NOT MATCHED THEN
	--		INSERT ([SYSTEM_ID]
	--		  ,[VAR_COD_TIPO]
	--		  ,[VAR_DESC_TIPO_FUN]
	--		  ,[CHA_VIS]
	--		  ,[ID_AMM])
	--		VALUES (SOURCE.[SYSTEM_ID]
	--		  ,SOURCE.[VAR_COD_TIPO]
	--		  ,SOURCE.[VAR_DESC_TIPO_FUN]
	--		  ,SOURCE.[CHA_VIS]
	--		  ,SOURCE.[ID_AMM]);' AS NVARCHAR(MAX))

	--PRINT @sql_string;

	--EXECUTE sp_executesql @sql_string;

	--IF @@ERROR <> 0
	--BEGIN
	--	-- Rollback the transaction
	--	ROLLBACK
		
	--	set @logType = 'ERROR'
	--	set @log = 'Errore durante l''aggiornamento dei tipi di funzione'
		
	--	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	--	-- Raise an error and return
	--	RAISERROR (@log, 16, 1)
	--	RETURN
	--END
	
	--set @logType = 'INFO'
	--set @log = 'Aggiornamento tipo funzione'
	
	--EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	-- L'aggiornamento della tabella DPA_TIPO_F_RUOLO è stato commentato perchè riabilitata la proprietà IDENTITY
	-- del campo SYSTEM_ID

	-- ****************
	-- DPA_TIPO_F_RUOLO
	-- ****************
	
	--SET @sql_string = CAST(N'MERGE DPA_TIPO_F_RUOLO AS TARGET
	--	USING ( 	
	--	SELECT [SYSTEM_ID]
	--		  ,[ID_TIPO_FUNZ]
	--		  ,[ID_RUOLO_IN_UO]
	--	  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_TIPO_F_RUOLO]
	--	) 
	--	AS SOURCE ([SYSTEM_ID]
	--		  ,[ID_TIPO_FUNZ]
	--		  ,[ID_RUOLO_IN_UO])
	--	ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
	--	WHEN MATCHED THEN
	--		UPDATE SET
	--			[SYSTEM_ID] = SOURCE.[SYSTEM_ID]
	--		  ,[ID_TIPO_FUNZ] = SOURCE.[ID_TIPO_FUNZ]
	--		  ,[ID_RUOLO_IN_UO] = SOURCE.[ID_RUOLO_IN_UO]
	--	WHEN NOT MATCHED THEN
	--		INSERT ([SYSTEM_ID]
	--		  ,[ID_TIPO_FUNZ]
	--		  ,[ID_RUOLO_IN_UO])
	--		VALUES (SOURCE.[SYSTEM_ID]
	--		  ,SOURCE.[ID_TIPO_FUNZ]
	--		  ,SOURCE.[ID_RUOLO_IN_UO]);' AS NVARCHAR(MAX))

	--PRINT @sql_string;

	--EXECUTE sp_executesql @sql_string;

	--IF @@ERROR <> 0
	--BEGIN
	--	-- Rollback the transaction
	--	ROLLBACK
		
	--	set @logType = 'ERROR'
	--	set @log = 'Errore durante l''aggiornamento della relazione ruolo-funzione'
		
	--	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	--	-- Raise an error and return
	--	RAISERROR (@log, 16, 1)
	--	RETURN
	--END
	
	--set @logType = 'INFO'
	--set @log = 'Aggiornamento ruolo-funzione'
	
	--EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	
		
	-- *****************
	-- DPA_RAGIONE_TRASM
	-- *****************
	
	SET @sql_string = CAST(N'MERGE DPA_RAGIONE_TRASM AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[VAR_DESC_RAGIONE]
				,[CHA_TIPO_RAGIONE]
				,[CHA_VIS]
				,[CHA_TIPO_DIRITTI]
				,[CHA_TIPO_DEST]
				,[CHA_RISPOSTA]
				,[VAR_NOTE]
				,[CHA_EREDITA]
				,[ID_AMM]
				,[CHA_TIPO_RISPOSTA]
				,[VAR_NOTIFICA_TRASM]
				,[VAR_TESTO_MSG_NOTIFICA_DOC]
				,[VAR_TESTO_MSG_NOTIFICA_FASC]
				,[CHA_CEDE_DIRITTI]
				,[CHA_RAG_SISTEMA]
				,[CHA_MANTIENI_LETT]
				,[CHA_MANTIENI_SCRITT]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_RAGIONE_TRASM]
			WHERE [ID_AMM] IS NULL
			)
		AS SOURCE ([SYSTEM_ID]
				,[VAR_DESC_RAGIONE]
				,[CHA_TIPO_RAGIONE]
				,[CHA_VIS]
				,[CHA_TIPO_DIRITTI]
				,[CHA_TIPO_DEST]
				,[CHA_RISPOSTA]
				,[VAR_NOTE]
				,[CHA_EREDITA]
				,[ID_AMM]
				,[CHA_TIPO_RISPOSTA]
				,[VAR_NOTIFICA_TRASM]
				,[VAR_TESTO_MSG_NOTIFICA_DOC]
				,[VAR_TESTO_MSG_NOTIFICA_FASC]
				,[CHA_CEDE_DIRITTI]
				,[CHA_RAG_SISTEMA]
				,[CHA_MANTIENI_LETT]
				,[CHA_MANTIENI_SCRITT])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				 [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[VAR_DESC_RAGIONE] = SOURCE.[VAR_DESC_RAGIONE]
				,[CHA_TIPO_RAGIONE] = SOURCE.[CHA_TIPO_RAGIONE]
				,[CHA_VIS] = SOURCE.[CHA_VIS]
				,[CHA_TIPO_DIRITTI] = SOURCE.[CHA_TIPO_DIRITTI]
				,[CHA_TIPO_DEST] = SOURCE.[CHA_TIPO_DEST]
				,[CHA_RISPOSTA] = SOURCE.[CHA_RISPOSTA]
				,[VAR_NOTE] = SOURCE.[VAR_NOTE]
				,[CHA_EREDITA] = SOURCE.[CHA_EREDITA]
				,[ID_AMM] = SOURCE.[ID_AMM]
				,[CHA_TIPO_RISPOSTA] = SOURCE.[CHA_TIPO_RISPOSTA]
				,[VAR_NOTIFICA_TRASM] = SOURCE.[VAR_NOTIFICA_TRASM]
				,[VAR_TESTO_MSG_NOTIFICA_DOC] = SOURCE.[VAR_TESTO_MSG_NOTIFICA_DOC]
				,[VAR_TESTO_MSG_NOTIFICA_FASC] = SOURCE.[VAR_TESTO_MSG_NOTIFICA_FASC]
				,[CHA_CEDE_DIRITTI] = SOURCE.[CHA_CEDE_DIRITTI]
				,[CHA_RAG_SISTEMA] = SOURCE.[CHA_RAG_SISTEMA]
				,[CHA_MANTIENI_LETT] = SOURCE.[CHA_MANTIENI_LETT]
				,[CHA_MANTIENI_SCRITT] = SOURCE.[CHA_MANTIENI_SCRITT]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[VAR_DESC_RAGIONE]
				,[CHA_TIPO_RAGIONE]
				,[CHA_VIS]
				,[CHA_TIPO_DIRITTI]
				,[CHA_TIPO_DEST]
				,[CHA_RISPOSTA]
				,[VAR_NOTE]
				,[CHA_EREDITA]
				,[ID_AMM]
				,[CHA_TIPO_RISPOSTA]
				,[VAR_NOTIFICA_TRASM]
				,[VAR_TESTO_MSG_NOTIFICA_DOC]
				,[VAR_TESTO_MSG_NOTIFICA_FASC]
				,[CHA_CEDE_DIRITTI]
				,[CHA_RAG_SISTEMA]
				,[CHA_MANTIENI_LETT]
				,[CHA_MANTIENI_SCRITT])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[VAR_DESC_RAGIONE]
				,SOURCE.[CHA_TIPO_RAGIONE]
				,SOURCE.[CHA_VIS]
				,SOURCE.[CHA_TIPO_DIRITTI]
				,SOURCE.[CHA_TIPO_DEST]
				,SOURCE.[CHA_RISPOSTA]
				,SOURCE.[VAR_NOTE]
				,SOURCE.[CHA_EREDITA]
				,SOURCE.[ID_AMM]
				,SOURCE.[CHA_TIPO_RISPOSTA]
				,SOURCE.[VAR_NOTIFICA_TRASM]
				,SOURCE.[VAR_TESTO_MSG_NOTIFICA_DOC]
				,SOURCE.[VAR_TESTO_MSG_NOTIFICA_FASC]
				,SOURCE.[CHA_CEDE_DIRITTI]
				,SOURCE.[CHA_RAG_SISTEMA]
				,SOURCE.[CHA_MANTIENI_LETT]
				,SOURCE.[CHA_MANTIENI_SCRITT]);' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_RAGIONE_TRASM'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_RAGIONE_TRASM'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				
	
	

	-- Chiude e dealloca eventuali cursori rimasti aperti
	--
	execute sp_ARCHIVE_BE_CleanUpCursor 'statement_cursor'
					
	
		
	-- *********************************************************************************
	-- Aggiornamento dati che dipendono dall'amministrazione configurata
	-- *********************************************************************************

	DECLARE statement_cursor CURSOR
	FOR SELECT CONVERT(varchar(10),SYSTEM_ID) FROM DPA_AMMINISTRA
	     
	OPEN statement_cursor
	FETCH statement_cursor INTO @idAmministrazione 
	WHILE (@@FETCH_STATUS = 0)
		BEGIN
	 
		print @idAmministrazione
		
		-- ********
		-- Registri
		-- ********
		--
		SET @sql_string = CAST(N'MERGE DPA_EL_REGISTRI AS TARGET
			USING (SELECT 
					[SYSTEM_ID]
				   ,[ID_AMM]
				   ,[VAR_CODICE]
				   ,[VAR_DESC_REGISTRO]
				   ,[VAR_EMAIL_REGISTRO]
				   ,[VAR_USER_MAIL]
				   ,[VAR_PWD_MAIL]
				   ,[DTA_OPEN]
				   ,[DTA_CLOSE]
				   ,[NUM_RIF]
				   ,[CHA_STATO]
				   ,[VAR_SERVER_SMTP]
				   ,[NUM_PORTA_SMTP]
				   ,[DTA_ULTIMO_PROTO]
				   ,[VAR_SERVER_POP]
				   ,[NUM_PORTA_POP]
				   ,[CHA_AUTOMATICO]
				   ,[VAR_USER_SMTP]
				   ,[VAR_PWD_SMTP]
				   ,[ID_PEOPLE_AOO]
				   ,[ID_RUOLO_AOO]
				   ,[CHA_SMTP_SSL]
				   ,[CHA_POP_SSL]
				   ,[CHA_SMTP_STA]
				   ,[CHA_AUTO_INTEROP]
				   ,[CHA_RF]
				   ,[ID_AOO_COLLEGATA]
				   ,[CHA_DISABILITATO]
				   ,[DIRITTO_RUOLO_AOO]
				   ,[ID_RUOLO_RESP]
				   ,[VAR_BOX_MAIL_ELABORATE]
				   ,[VAR_MAIL_NON_ELABORATE]
				   ,[CHA_IMAP_SSL]
				   ,[VAR_SERVER_IMAP]
				   ,[VAR_SOLO_MAIL_PEC]
				   ,[NUM_PORTA_IMAP]
				   ,[VAR_TIPO_CONNESSIONE]
				   ,[VAR_INBOX_IMAP]
				   ,[ID_PEOPLE_FACTORY]
				   ,[ID_GRUPPO_FACTORY]
				   ,[INVIO_RICEVUTA_MANUALE]
				   ,[CHA_RICEVUTA_PEC]
				   ,[FLAG_WSPIA]
				   ,[VAR_PREG]
				   ,[ANNO_PREG]
				   ,[VAR_CODICE_AOO_IPA]
				   ,[CODICE_UAC]
				   ,[CODICE_CLASSIFICAZIONE] 
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST('.DPA_EL_REGISTRI
				WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(') 
			AS SOURCE (
					[SYSTEM_ID]
				   ,[ID_AMM]
				   ,[VAR_CODICE]
				   ,[VAR_DESC_REGISTRO]
				   ,[VAR_EMAIL_REGISTRO]
				   ,[VAR_USER_MAIL]
				   ,[VAR_PWD_MAIL]
				   ,[DTA_OPEN]
				   ,[DTA_CLOSE]
				   ,[NUM_RIF]
				   ,[CHA_STATO]
				   ,[VAR_SERVER_SMTP]
				   ,[NUM_PORTA_SMTP]
				   ,[DTA_ULTIMO_PROTO]
				   ,[VAR_SERVER_POP]
				   ,[NUM_PORTA_POP]
				   ,[CHA_AUTOMATICO]
				   ,[VAR_USER_SMTP]
				   ,[VAR_PWD_SMTP]
				   ,[ID_PEOPLE_AOO]
				   ,[ID_RUOLO_AOO]
				   ,[CHA_SMTP_SSL]
				   ,[CHA_POP_SSL]
				   ,[CHA_SMTP_STA]
				   ,[CHA_AUTO_INTEROP]
				   ,[CHA_RF]
				   ,[ID_AOO_COLLEGATA]
				   ,[CHA_DISABILITATO]
				   ,[DIRITTO_RUOLO_AOO]
				   ,[ID_RUOLO_RESP]
				   ,[VAR_BOX_MAIL_ELABORATE]
				   ,[VAR_MAIL_NON_ELABORATE]
				   ,[CHA_IMAP_SSL]
				   ,[VAR_SERVER_IMAP]
				   ,[VAR_SOLO_MAIL_PEC]
				   ,[NUM_PORTA_IMAP]
				   ,[VAR_TIPO_CONNESSIONE]
				   ,[VAR_INBOX_IMAP]
				   ,[ID_PEOPLE_FACTORY]
				   ,[ID_GRUPPO_FACTORY]
				   ,[INVIO_RICEVUTA_MANUALE]
				   ,[CHA_RICEVUTA_PEC]
				   ,[FLAG_WSPIA]
				   ,[VAR_PREG]
				   ,[ANNO_PREG]
				   ,[VAR_CODICE_AOO_IPA]
				   ,[CODICE_UAC]
				   ,[CODICE_CLASSIFICAZIONE])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
				   ID_AMM = SOURCE.ID_AMM
				   ,VAR_CODICE = SOURCE.VAR_CODICE
				   ,VAR_DESC_REGISTRO = SOURCE.VAR_DESC_REGISTRO
				   ,VAR_EMAIL_REGISTRO = SOURCE.VAR_EMAIL_REGISTRO
				   ,VAR_USER_MAIL = SOURCE.VAR_USER_MAIL
				   ,VAR_PWD_MAIL = SOURCE.VAR_PWD_MAIL
				   ,DTA_OPEN = SOURCE.DTA_OPEN
				   ,DTA_CLOSE = SOURCE.DTA_CLOSE
				   ,NUM_RIF = SOURCE.NUM_RIF
				   ,CHA_STATO = SOURCE.CHA_STATO
				   ,VAR_SERVER_SMTP = SOURCE.VAR_SERVER_SMTP
				   ,NUM_PORTA_SMTP = SOURCE.NUM_PORTA_SMTP
				   ,DTA_ULTIMO_PROTO = SOURCE.DTA_ULTIMO_PROTO
				   ,VAR_SERVER_POP = SOURCE.VAR_SERVER_POP
				   ,NUM_PORTA_POP = SOURCE.NUM_PORTA_POP
				   ,CHA_AUTOMATICO = SOURCE.CHA_AUTOMATICO
				   ,VAR_USER_SMTP = SOURCE.VAR_USER_SMTP
				   ,VAR_PWD_SMTP = SOURCE.VAR_PWD_SMTP
				   ,ID_PEOPLE_AOO = SOURCE.ID_PEOPLE_AOO
				   ,ID_RUOLO_AOO = SOURCE.ID_RUOLO_AOO
				   ,CHA_SMTP_SSL = SOURCE.CHA_SMTP_SSL
				   ,CHA_POP_SSL = SOURCE.CHA_POP_SSL
				   ,CHA_SMTP_STA = SOURCE.CHA_SMTP_STA
				   ,CHA_AUTO_INTEROP = SOURCE.CHA_AUTO_INTEROP
				   ,CHA_RF = SOURCE.CHA_RF
				   ,ID_AOO_COLLEGATA = SOURCE.ID_AOO_COLLEGATA
				   ,CHA_DISABILITATO = SOURCE.CHA_DISABILITATO
				   ,DIRITTO_RUOLO_AOO = SOURCE.DIRITTO_RUOLO_AOO
				   ,ID_RUOLO_RESP = SOURCE.ID_RUOLO_RESP
				   ,VAR_BOX_MAIL_ELABORATE = SOURCE.VAR_BOX_MAIL_ELABORATE
				   ,VAR_MAIL_NON_ELABORATE = SOURCE.VAR_MAIL_NON_ELABORATE
				   ,CHA_IMAP_SSL = SOURCE.CHA_IMAP_SSL
				   ,VAR_SERVER_IMAP = SOURCE.VAR_SERVER_IMAP
				   ,VAR_SOLO_MAIL_PEC = SOURCE.VAR_SOLO_MAIL_PEC
				   ,NUM_PORTA_IMAP = SOURCE.NUM_PORTA_IMAP
				   ,VAR_TIPO_CONNESSIONE = SOURCE.VAR_TIPO_CONNESSIONE
				   ,VAR_INBOX_IMAP = SOURCE.VAR_INBOX_IMAP
				   ,ID_PEOPLE_FACTORY = SOURCE.ID_PEOPLE_FACTORY
				   ,ID_GRUPPO_FACTORY = SOURCE.ID_GRUPPO_FACTORY
				   ,INVIO_RICEVUTA_MANUALE = SOURCE.INVIO_RICEVUTA_MANUALE
				   ,CHA_RICEVUTA_PEC = SOURCE.CHA_RICEVUTA_PEC
				   ,FLAG_WSPIA = SOURCE.FLAG_WSPIA
				   ,VAR_PREG = SOURCE.VAR_PREG
				   ,ANNO_PREG = SOURCE.ANNO_PREG
				   ,VAR_CODICE_AOO_IPA = SOURCE.VAR_CODICE_AOO_IPA
				   ,CODICE_UAC = SOURCE.CODICE_UAC
				   ,CODICE_CLASSIFICAZIONE = SOURCE.CODICE_CLASSIFICAZIONE			
			WHEN NOT MATCHED THEN
				INSERT (
					SYSTEM_ID
				   ,ID_AMM
				   ,VAR_CODICE
				   ,VAR_DESC_REGISTRO
				   ,VAR_EMAIL_REGISTRO
				   ,VAR_USER_MAIL
				   ,VAR_PWD_MAIL
				   ,DTA_OPEN
				   ,DTA_CLOSE
				   ,NUM_RIF
				   ,CHA_STATO
				   ,VAR_SERVER_SMTP
				   ,NUM_PORTA_SMTP
				   ,DTA_ULTIMO_PROTO
				   ,VAR_SERVER_POP
				   ,NUM_PORTA_POP
				   ,CHA_AUTOMATICO
				   ,VAR_USER_SMTP
				   ,VAR_PWD_SMTP
				   ,ID_PEOPLE_AOO
				   ,ID_RUOLO_AOO
				   ,CHA_SMTP_SSL
				   ,CHA_POP_SSL
				   ,CHA_SMTP_STA
				   ,CHA_AUTO_INTEROP
				   ,CHA_RF
				   ,ID_AOO_COLLEGATA
				   ,CHA_DISABILITATO
				   ,DIRITTO_RUOLO_AOO
				   ,ID_RUOLO_RESP
				   ,VAR_BOX_MAIL_ELABORATE
				   ,VAR_MAIL_NON_ELABORATE
				   ,CHA_IMAP_SSL
				   ,VAR_SERVER_IMAP
				   ,VAR_SOLO_MAIL_PEC
				   ,NUM_PORTA_IMAP
				   ,VAR_TIPO_CONNESSIONE
				   ,VAR_INBOX_IMAP
				   ,ID_PEOPLE_FACTORY
				   ,ID_GRUPPO_FACTORY
				   ,INVIO_RICEVUTA_MANUALE
				   ,CHA_RICEVUTA_PEC
				   ,FLAG_WSPIA
				   ,VAR_PREG
				   ,ANNO_PREG
				   ,VAR_CODICE_AOO_IPA
				   ,CODICE_UAC
				   ,CODICE_CLASSIFICAZIONE)
				VALUES (
					SOURCE.SYSTEM_ID
				   ,SOURCE.ID_AMM
				   ,SOURCE.VAR_CODICE
				   ,SOURCE.VAR_DESC_REGISTRO
				   ,SOURCE.VAR_EMAIL_REGISTRO
				   ,SOURCE.VAR_USER_MAIL
				   ,SOURCE.VAR_PWD_MAIL
				   ,SOURCE.DTA_OPEN
				   ,SOURCE.DTA_CLOSE
				   ,SOURCE.NUM_RIF
				   ,SOURCE.CHA_STATO
				   ,SOURCE.VAR_SERVER_SMTP
				   ,SOURCE.NUM_PORTA_SMTP
				   ,SOURCE.DTA_ULTIMO_PROTO
				   ,SOURCE.VAR_SERVER_POP
				   ,SOURCE.NUM_PORTA_POP
				   ,SOURCE.CHA_AUTOMATICO
				   ,SOURCE.VAR_USER_SMTP
				   ,SOURCE.VAR_PWD_SMTP
				   ,SOURCE.ID_PEOPLE_AOO
				   ,SOURCE.ID_RUOLO_AOO
				   ,SOURCE.CHA_SMTP_SSL
				   ,SOURCE.CHA_POP_SSL
				   ,SOURCE.CHA_SMTP_STA
				   ,SOURCE.CHA_AUTO_INTEROP
				   ,SOURCE.CHA_RF
				   ,SOURCE.id_Aoo_Collegata
				   ,SOURCE.CHA_DISABILITATO
				   ,SOURCE.DIRITTO_RUOLO_AOO
				   ,SOURCE.ID_RUOLO_RESP
				   ,SOURCE.VAR_BOX_MAIL_ELABORATE
				   ,SOURCE.VAR_MAIL_NON_ELABORATE
				   ,SOURCE.CHA_IMAP_SSL
				   ,SOURCE.VAR_SERVER_IMAP
				   ,SOURCE.VAR_SOLO_MAIL_PEC
				   ,SOURCE.NUM_PORTA_IMAP
				   ,SOURCE.VAR_TIPO_CONNESSIONE
				   ,SOURCE.VAR_INBOX_IMAP
				   ,SOURCE.ID_PEOPLE_FACTORY
				   ,SOURCE.ID_GRUPPO_FACTORY
				   ,SOURCE.INVIO_RICEVUTA_MANUALE
				   ,SOURCE.CHA_RICEVUTA_PEC
				   ,SOURCE.FLAG_WSPIA
				   ,SOURCE.VAR_PREG
				   ,SOURCE.ANNO_PREG
				   ,SOURCE.VAR_CODICE_AOO_IPA
				   ,SOURCE.CODICE_UAC
				   ,SOURCE.CODICE_CLASSIFICAZIONE
					);' AS NVARCHAR(MAX))
				
		PRINT @sql_string;
	
		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento dei registri'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento dei registri'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
		
		
		
		-- *************
		-- Mail registri
		-- *************
		
		SET @sql_string = CAST(N'MERGE DPA_MAIL_REGISTRI AS TARGET
			USING ( 	
				SELECT 
					[SYSTEM_ID]
					,[ID_REGISTRO]
					,[VAR_PRINCIPALE]
					,[VAR_EMAIL_REGISTRO]
					,[VAR_USER_MAIL]
					,[VAR_PWD_MAIL]
					,[VAR_SERVER_SMTP]
					,[CHA_SMTP_SSL]
					,[CHA_POP_SSL]
					,[NUM_PORTA_SMTP]
					,[CHA_SMTP_STA]
					,[VAR_SERVER_POP]
					,[NUM_PORTA_POP]
					,[VAR_USER_SMTP]
					,[VAR_PWD_SMTP]
					,[VAR_INBOX_IMAP]
					,[VAR_SERVER_IMAP]
					,[NUM_PORTA_IMAP]
					,[VAR_TIPO_CONNESSIONE]
					,[VAR_BOX_MAIL_ELABORATE]
					,[VAR_MAIL_NON_ELABORATE]
					,[CHA_IMAP_SSL]
					,[VAR_SOLO_MAIL_PEC]
					,[CHA_RICEVUTA_PEC]
					,[VAR_NOTE]
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_MAIL_REGISTRI]
				WHERE [ID_REGISTRO] IN
					(
					SELECT SYSTEM_ID FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_EL_REGISTRI]
					WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			AS SOURCE ([SYSTEM_ID]
					,[ID_REGISTRO]
					,[VAR_PRINCIPALE]
					,[VAR_EMAIL_REGISTRO]
					,[VAR_USER_MAIL]
					,[VAR_PWD_MAIL]
					,[VAR_SERVER_SMTP]
					,[CHA_SMTP_SSL]
					,[CHA_POP_SSL]
					,[NUM_PORTA_SMTP]
					,[CHA_SMTP_STA]
					,[VAR_SERVER_POP]
					,[NUM_PORTA_POP]
					,[VAR_USER_SMTP]
					,[VAR_PWD_SMTP]
					,[VAR_INBOX_IMAP]
					,[VAR_SERVER_IMAP]
					,[NUM_PORTA_IMAP]
					,[VAR_TIPO_CONNESSIONE]
					,[VAR_BOX_MAIL_ELABORATE]
					,[VAR_MAIL_NON_ELABORATE]
					,[CHA_IMAP_SSL]
					,[VAR_SOLO_MAIL_PEC]
					,[CHA_RICEVUTA_PEC]
					,[VAR_NOTE])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
					,[ID_REGISTRO] = SOURCE.[ID_REGISTRO]
					,[VAR_PRINCIPALE] = SOURCE.[VAR_PRINCIPALE]
					,[VAR_EMAIL_REGISTRO] = SOURCE.[VAR_EMAIL_REGISTRO]
					,[VAR_USER_MAIL] = SOURCE.[VAR_USER_MAIL]
					,[VAR_PWD_MAIL] = SOURCE.[VAR_PWD_MAIL]
					,[VAR_SERVER_SMTP] = SOURCE.[VAR_SERVER_SMTP]
					,[CHA_SMTP_SSL] = SOURCE.[CHA_SMTP_SSL]
					,[CHA_POP_SSL] = SOURCE.[CHA_POP_SSL]
					,[NUM_PORTA_SMTP] = SOURCE.[NUM_PORTA_SMTP]
					,[CHA_SMTP_STA] = SOURCE.[CHA_SMTP_STA]
					,[VAR_SERVER_POP] = SOURCE.[VAR_SERVER_POP]
					,[NUM_PORTA_POP] = SOURCE.[NUM_PORTA_POP]
					,[VAR_USER_SMTP] = SOURCE.[VAR_USER_SMTP]
					,[VAR_PWD_SMTP] = SOURCE.[VAR_PWD_SMTP]
					,[VAR_INBOX_IMAP] = SOURCE.[VAR_INBOX_IMAP]
					,[VAR_SERVER_IMAP] = SOURCE.[VAR_SERVER_IMAP]
					,[NUM_PORTA_IMAP] = SOURCE.[NUM_PORTA_IMAP]
					,[VAR_TIPO_CONNESSIONE] = SOURCE.[VAR_TIPO_CONNESSIONE]
					,[VAR_BOX_MAIL_ELABORATE] = SOURCE.[VAR_BOX_MAIL_ELABORATE]
					,[VAR_MAIL_NON_ELABORATE] = SOURCE.[VAR_MAIL_NON_ELABORATE]
					,[CHA_IMAP_SSL] = SOURCE.[CHA_IMAP_SSL]
					,[VAR_SOLO_MAIL_PEC] = SOURCE.[VAR_SOLO_MAIL_PEC]
					,[CHA_RICEVUTA_PEC] = SOURCE.[CHA_RICEVUTA_PEC]
					,[VAR_NOTE] = SOURCE.[VAR_NOTE]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
					,[ID_REGISTRO]
					,[VAR_PRINCIPALE]
					,[VAR_EMAIL_REGISTRO]
					,[VAR_USER_MAIL]
					,[VAR_PWD_MAIL]
					,[VAR_SERVER_SMTP]
					,[CHA_SMTP_SSL]
					,[CHA_POP_SSL]
					,[NUM_PORTA_SMTP]
					,[CHA_SMTP_STA]
					,[VAR_SERVER_POP]
					,[NUM_PORTA_POP]
					,[VAR_USER_SMTP]
					,[VAR_PWD_SMTP]
					,[VAR_INBOX_IMAP]
					,[VAR_SERVER_IMAP]
					,[NUM_PORTA_IMAP]
					,[VAR_TIPO_CONNESSIONE]
					,[VAR_BOX_MAIL_ELABORATE]
					,[VAR_MAIL_NON_ELABORATE]
					,[CHA_IMAP_SSL]
					,[VAR_SOLO_MAIL_PEC]
					,[CHA_RICEVUTA_PEC]
					,[VAR_NOTE])
				VALUES (SOURCE.[SYSTEM_ID]
					,SOURCE.[ID_REGISTRO]
					,SOURCE.[VAR_PRINCIPALE]
					,SOURCE.[VAR_EMAIL_REGISTRO]
					,SOURCE.[VAR_USER_MAIL]
					,SOURCE.[VAR_PWD_MAIL]
					,SOURCE.[VAR_SERVER_SMTP]
					,SOURCE.[CHA_SMTP_SSL]
					,SOURCE.[CHA_POP_SSL]
					,SOURCE.[NUM_PORTA_SMTP]
					,SOURCE.[CHA_SMTP_STA]
					,SOURCE.[VAR_SERVER_POP]
					,SOURCE.[NUM_PORTA_POP]
					,SOURCE.[VAR_USER_SMTP]
					,SOURCE.[VAR_PWD_SMTP]
					,SOURCE.[VAR_INBOX_IMAP]
					,SOURCE.[VAR_SERVER_IMAP]
					,SOURCE.[NUM_PORTA_IMAP]
					,SOURCE.[VAR_TIPO_CONNESSIONE]
					,SOURCE.[VAR_BOX_MAIL_ELABORATE]
					,SOURCE.[VAR_MAIL_NON_ELABORATE]
					,SOURCE.[CHA_IMAP_SSL]
					,SOURCE.[VAR_SOLO_MAIL_PEC]
					,SOURCE.[CHA_RICEVUTA_PEC]
					,SOURCE.[VAR_NOTE]);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_MAIL_REGISTRI'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_MAIL_REGISTRI'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
		
		
		
		-- ******
		-- Parole
		-- ******

		SET @sql_string = CAST(N'MERGE DPA_PAROLE AS TARGET
			USING ( 	
				SELECT 
					[SYSTEM_ID]
					,[ID_AMM]
					,[VAR_DESC_PAROLA]
					,[ID_REGISTRO]
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_PAROLE]
				WHERE [ID_AMM] = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(N'
				)			
			AS SOURCE ([SYSTEM_ID]
					,[ID_AMM]
					,[VAR_DESC_PAROLA]
					,[ID_REGISTRO])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					 [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
					,[ID_AMM] = SOURCE.[ID_AMM]
					,[VAR_DESC_PAROLA] = SOURCE.[VAR_DESC_PAROLA]
					,[ID_REGISTRO] = SOURCE.[ID_REGISTRO]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
					,[ID_AMM]
					,[VAR_DESC_PAROLA]
					,[ID_REGISTRO])
				VALUES (SOURCE.[SYSTEM_ID]
					,SOURCE.[ID_AMM]
					,SOURCE.[VAR_DESC_PAROLA]
					,SOURCE.[ID_REGISTRO]);' AS NVARCHAR(MAX))
		
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_PAROLE'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_PAROLE'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject		
		
		
		
		-- *********************
		-- Tipologia documentale
		-- *********************

		SET @sql_string = CAST(N'MERGE DPA_TIPO_ATTO AS TARGET
			USING (SELECT SYSTEM_ID
					,VAR_DESC_ATTO
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERDOCUMENTO
					,PATH_ALL_1
					,EXT_ALL_1
					,PATH_MOD_SU
					,COD_MOD_TRASM
					,COD_CLASS
					,PATH_MOD_EXC
					,NUM_MESI_CONSERVAZIONE
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
				+ CAST(N'.DPA_TIPO_ATTO WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(') 
			AS SOURCE (SYSTEM_ID
					,VAR_DESC_ATTO
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERDOCUMENTO
					,PATH_ALL_1
					,EXT_ALL_1
					,PATH_MOD_SU
					,COD_MOD_TRASM
					,COD_CLASS
					,PATH_MOD_EXC
					,NUM_MESI_CONSERVAZIONE)
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					VAR_DESC_ATTO = SOURCE.VAR_DESC_ATTO
					,ID_AMM = SOURCE.ID_AMM
					,ABILITATO_SI_NO = SOURCE.ABILITATO_SI_NO
					,IN_ESERCIZIO = SOURCE.IN_ESERCIZIO
					,PATH_MOD_1 = SOURCE.PATH_MOD_1
					,EXT_MOD_1 = SOURCE.EXT_MOD_1
					,PATH_MOD_2 = SOURCE.PATH_MOD_2
					,EXT_MOD_2 = SOURCE.EXT_MOD_2
					,GG_SCADENZA = SOURCE.GG_SCADENZA
					,GG_PRE_SCADENZA = SOURCE.GG_PRE_SCADENZA
					,CHA_PRIVATO = SOURCE.CHA_PRIVATO
					,IPERDOCUMENTO = SOURCE.IPERDOCUMENTO
					,PATH_ALL_1 = SOURCE.PATH_ALL_1
					,EXT_ALL_1 = SOURCE.EXT_ALL_1
					,PATH_MOD_SU = SOURCE.PATH_MOD_SU
					,COD_MOD_TRASM = SOURCE.COD_MOD_TRASM
					,COD_CLASS = SOURCE.COD_CLASS
					,PATH_MOD_EXC = SOURCE.PATH_MOD_EXC
					,NUM_MESI_CONSERVAZIONE = SOURCE.NUM_MESI_CONSERVAZIONE
			WHEN NOT MATCHED THEN
				INSERT (SYSTEM_ID
					,VAR_DESC_ATTO
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERDOCUMENTO
					,PATH_ALL_1
					,EXT_ALL_1
					,PATH_MOD_SU
					,COD_MOD_TRASM
					,COD_CLASS
					,PATH_MOD_EXC
					,NUM_MESI_CONSERVAZIONE)
				VALUES (SOURCE.SYSTEM_ID
					,SOURCE.VAR_DESC_ATTO
					,SOURCE.ID_AMM
					,SOURCE.ABILITATO_SI_NO
					,SOURCE.IN_ESERCIZIO
					,SOURCE.PATH_MOD_1
					,SOURCE.EXT_MOD_1
					,SOURCE.PATH_MOD_2
					,SOURCE.EXT_MOD_2
					,SOURCE.GG_SCADENZA
					,SOURCE.GG_PRE_SCADENZA
					,SOURCE.CHA_PRIVATO
					,SOURCE.IPERDOCUMENTO
					,SOURCE.PATH_ALL_1
					,SOURCE.EXT_ALL_1
					,SOURCE.PATH_MOD_SU
					,SOURCE.COD_MOD_TRASM
					,SOURCE.COD_CLASS
					,SOURCE.PATH_MOD_EXC
					,SOURCE.NUM_MESI_CONSERVAZIONE);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento delle tipologie documentali appartenenti ad amministrazioni'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento delle tipologie documentali appartenenti ad amministrazioni'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- *******************
		-- Tipologia fascicolo
		-- *******************
		
		SET @sql_string = CAST(N'MERGE DPA_TIPO_FASC AS TARGET
			USING (SELECT SYSTEM_ID
					,VAR_DESC_FASC
					,ID_AMM
					,ABILITATO_SI_NO
					,IN_ESERCIZIO
					,PATH_MOD_1
					,EXT_MOD_1
					,PATH_MOD_2
					,EXT_MOD_2
					,GG_SCADENZA
					,GG_PRE_SCADENZA
					,CHA_PRIVATO
					,IPERFASCICOLO
					,NUM_MESI_CONSERVAZIONE
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
				+ CAST(N'.DPA_TIPO_FASC WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(') 
			AS SOURCE (SYSTEM_ID
						,VAR_DESC_FASC
						,ID_AMM
						,ABILITATO_SI_NO
						,IN_ESERCIZIO
						,PATH_MOD_1
						,EXT_MOD_1
						,PATH_MOD_2
						,EXT_MOD_2
						,GG_SCADENZA
						,GG_PRE_SCADENZA
						,CHA_PRIVATO
						,IPERFASCICOLO
						,NUM_MESI_CONSERVAZIONE)
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					SYSTEM_ID = SOURCE.SYSTEM_ID
					,VAR_DESC_FASC = SOURCE.VAR_DESC_FASC
					,ID_AMM = SOURCE.ID_AMM
					,ABILITATO_SI_NO = SOURCE.ABILITATO_SI_NO
					,IN_ESERCIZIO = SOURCE.IN_ESERCIZIO
					,PATH_MOD_1 = SOURCE.PATH_MOD_1
					,EXT_MOD_1 = SOURCE.EXT_MOD_1
					,PATH_MOD_2 = SOURCE.PATH_MOD_2
					,EXT_MOD_2 = SOURCE.EXT_MOD_2
					,GG_SCADENZA = SOURCE.GG_SCADENZA
					,GG_PRE_SCADENZA = SOURCE.GG_PRE_SCADENZA
					,CHA_PRIVATO = SOURCE.CHA_PRIVATO
					,IPERFASCICOLO = SOURCE.IPERFASCICOLO
					,NUM_MESI_CONSERVAZIONE = SOURCE.NUM_MESI_CONSERVAZIONE
			WHEN NOT MATCHED THEN
				INSERT (SYSTEM_ID
						,VAR_DESC_FASC
						,ID_AMM
						,ABILITATO_SI_NO
						,IN_ESERCIZIO
						,PATH_MOD_1
						,EXT_MOD_1
						,PATH_MOD_2
						,EXT_MOD_2
						,GG_SCADENZA
						,GG_PRE_SCADENZA
						,CHA_PRIVATO
						,IPERFASCICOLO
						,NUM_MESI_CONSERVAZIONE)
				VALUES (SOURCE.SYSTEM_ID
						,SOURCE.VAR_DESC_FASC
						,SOURCE.ID_AMM
						,SOURCE.ABILITATO_SI_NO
						,SOURCE.IN_ESERCIZIO
						,SOURCE.PATH_MOD_1
						,SOURCE.EXT_MOD_1
						,SOURCE.PATH_MOD_2
						,SOURCE.EXT_MOD_2
						,SOURCE.GG_SCADENZA
						,SOURCE.GG_PRE_SCADENZA
						,SOURCE.CHA_PRIVATO
						,SOURCE.IPERFASCICOLO
						,SOURCE.NUM_MESI_CONSERVAZIONE);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento delle tipologie faascicoli appartenenti ad amministrazioni'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento delle tipologie fascicoli appartenenti ad amministrazioni'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- ******
		-- People
		-- ******

		SET @sql_string = CAST(N'MERGE PEOPLE AS TARGET
			USING ( 	
			SELECT [SYSTEM_ID]
				  ,[USER_ID]
				  ,[FULL_NAME]
				  ,[DISABLED]
				  ,[USER_PASSWORD]
				  ,[USER_LOCATION]
				  ,[PHONE]
				  ,[EXTENSION]
				  ,[LAST_LOGIN_DATE]
				  ,[LAST_LOGIN_TIME]
				  ,[ALLOW_LOGIN]
				  ,[FAX]
				  ,[DID]
				  ,[TARGET_DOCSRVR]
				  ,[PRIMARY_GROUP]
				  ,[PRIMARY_LIB]
				  ,[PROFILE_DEFAULTS]
				  ,[CONNECT_BRIDGED]
				  ,[BUTTON_BAR]
				  ,[NETWORK_ID]
				  ,[ACL_DEFAULTS]
				  ,[SHOW_RESTORED]
				  ,[PASS_EXP_DATE]
				  ,[LOGINS_REMAINING]
				  ,[PSWORD_VALID_FOR]
				  ,[NO_EXP_DATE]
				  ,[DR_USER]
				  ,[SEARCH_FORM_ID]
				  ,[EMAIL_ADDRESS]
				  ,[LAST_UPDATE]
				  ,[CHA_AMMINISTRATORE]
				  ,[CHA_NOTIFICA]
				  ,[VAR_TELEFONO]
				  ,[ID_AMM]
				  ,[CHA_RESP_ASS]
				  ,[CHA_ASSEGNATARIO]
				  ,[VAR_COGNOME]
				  ,[VAR_NOME]
				  ,[CHA_NOTIFICA_CON_ALLEGATO]
				  ,[VAR_SEDE]
				  ,[ENCRYPTED_PASSWORD]
				  ,[PASSWORD_CREATION_DATE]
				  ,[PASSWORD_NEVER_EXPIRE]
				  ,[FROM_EMAIL_ADDRESS]
				  ,[LDAP_NEVER_SYNC]
				  ,[LDAP_ID_SYNC]
				  ,[ID_CLIENT_MODEL_PROCESSOR]
				  ,[FACTORY_USER]
				  ,[IS_ENABLED_SMART_CLIENT]
				  ,[SMART_CLIENT_PDF_CONV_ON_SCAN]
				  ,[LDAP_AUTHENTICATED]
				  ,[ACCETTAZIONE_DISSERV]
				  ,[ID_DISPOSITIVO_STAMPA]
				  ,[ABILITATO_CENTRO_SERVIZI]
				  ,[MATRICOLA]
				  ,[ABILITATO_CHIAVI_CONFIG]
				  ,[CHA_TIPO_COMPONENTI]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[PEOPLE]
			  WHERE [ID_AMM] = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(') 
			AS SOURCE ([SYSTEM_ID]
				  ,[USER_ID]
				  ,[FULL_NAME]
				  ,[DISABLED]
				  ,[USER_PASSWORD]
				  ,[USER_LOCATION]
				  ,[PHONE]
				  ,[EXTENSION]
				  ,[LAST_LOGIN_DATE]
				  ,[LAST_LOGIN_TIME]
				  ,[ALLOW_LOGIN]
				  ,[FAX]
				  ,[DID]
				  ,[TARGET_DOCSRVR]
				  ,[PRIMARY_GROUP]
				  ,[PRIMARY_LIB]
				  ,[PROFILE_DEFAULTS]
				  ,[CONNECT_BRIDGED]
				  ,[BUTTON_BAR]
				  ,[NETWORK_ID]
				  ,[ACL_DEFAULTS]
				  ,[SHOW_RESTORED]
				  ,[PASS_EXP_DATE]
				  ,[LOGINS_REMAINING]
				  ,[PSWORD_VALID_FOR]
				  ,[NO_EXP_DATE]
				  ,[DR_USER]
				  ,[SEARCH_FORM_ID]
				  ,[EMAIL_ADDRESS]
				  ,[LAST_UPDATE]
				  ,[CHA_AMMINISTRATORE]
				  ,[CHA_NOTIFICA]
				  ,[VAR_TELEFONO]
				  ,[ID_AMM]
				  ,[CHA_RESP_ASS]
				  ,[CHA_ASSEGNATARIO]
				  ,[VAR_COGNOME]
				  ,[VAR_NOME]
				  ,[CHA_NOTIFICA_CON_ALLEGATO]
				  ,[VAR_SEDE]
				  ,[ENCRYPTED_PASSWORD]
				  ,[PASSWORD_CREATION_DATE]
				  ,[PASSWORD_NEVER_EXPIRE]
				  ,[FROM_EMAIL_ADDRESS]
				  ,[LDAP_NEVER_SYNC]
				  ,[LDAP_ID_SYNC]
				  ,[ID_CLIENT_MODEL_PROCESSOR]
				  ,[FACTORY_USER]
				  ,[IS_ENABLED_SMART_CLIENT]
				  ,[SMART_CLIENT_PDF_CONV_ON_SCAN]
				  ,[LDAP_AUTHENTICATED]
				  ,[ACCETTAZIONE_DISSERV]
				  ,[ID_DISPOSITIVO_STAMPA]
				  ,[ABILITATO_CENTRO_SERVIZI]
				  ,[MATRICOLA]
				  ,[ABILITATO_CHIAVI_CONFIG]
				  ,[CHA_TIPO_COMPONENTI])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[USER_ID] = SOURCE.[USER_ID]
				  ,[FULL_NAME] = SOURCE.[FULL_NAME]
				  ,[DISABLED] = SOURCE.[DISABLED]
				  ,[USER_PASSWORD] = SOURCE.[USER_PASSWORD]
				  ,[USER_LOCATION] = SOURCE.[USER_LOCATION]
				  ,[PHONE] = SOURCE.[PHONE]
				  ,[EXTENSION] = SOURCE.[EXTENSION]
				  ,[LAST_LOGIN_DATE] = SOURCE.[LAST_LOGIN_DATE]
				  ,[LAST_LOGIN_TIME] = SOURCE.[LAST_LOGIN_TIME]
				  ,[ALLOW_LOGIN] = SOURCE.[ALLOW_LOGIN]
				  ,[FAX] = SOURCE.[FAX]
				  ,[DID] = SOURCE.[DID]
				  ,[TARGET_DOCSRVR] = SOURCE.[TARGET_DOCSRVR]
				  ,[PRIMARY_GROUP] = SOURCE.[PRIMARY_GROUP]
				  ,[PRIMARY_LIB] = SOURCE.[PRIMARY_LIB]
				  ,[PROFILE_DEFAULTS] = SOURCE.[PROFILE_DEFAULTS]
				  ,[CONNECT_BRIDGED] = SOURCE.[CONNECT_BRIDGED]
				  ,[BUTTON_BAR] = SOURCE.[BUTTON_BAR]
				  ,[NETWORK_ID] = SOURCE.[NETWORK_ID]
				  ,[ACL_DEFAULTS] = SOURCE.[ACL_DEFAULTS]
				  ,[SHOW_RESTORED] = SOURCE.[SHOW_RESTORED]
				  ,[PASS_EXP_DATE] = SOURCE.[PASS_EXP_DATE]
				  ,[LOGINS_REMAINING] = SOURCE.[LOGINS_REMAINING]
				  ,[PSWORD_VALID_FOR] = SOURCE.[PSWORD_VALID_FOR]
				  ,[NO_EXP_DATE] = SOURCE.[NO_EXP_DATE]
				  ,[DR_USER] = SOURCE.[DR_USER]
				  ,[SEARCH_FORM_ID] = SOURCE.[SEARCH_FORM_ID]
				  ,[EMAIL_ADDRESS] = SOURCE.[EMAIL_ADDRESS]
				  ,[LAST_UPDATE] = SOURCE.[LAST_UPDATE]
				  ,[CHA_AMMINISTRATORE] = SOURCE.[CHA_AMMINISTRATORE]
				  ,[CHA_NOTIFICA] = SOURCE.[CHA_NOTIFICA]
				  ,[VAR_TELEFONO] = SOURCE.[VAR_TELEFONO]
				  ,[ID_AMM] = SOURCE.[ID_AMM]
				  ,[CHA_RESP_ASS] = SOURCE.[CHA_RESP_ASS]
				  ,[CHA_ASSEGNATARIO] = SOURCE.[CHA_ASSEGNATARIO]
				  ,[VAR_COGNOME] = SOURCE.[VAR_COGNOME]
				  ,[VAR_NOME] = SOURCE.[VAR_NOME]
				  ,[CHA_NOTIFICA_CON_ALLEGATO] = SOURCE.[CHA_NOTIFICA_CON_ALLEGATO]
				  ,[VAR_SEDE] = SOURCE.[VAR_SEDE]
				  ,[ENCRYPTED_PASSWORD] = SOURCE.[ENCRYPTED_PASSWORD]
				  ,[PASSWORD_CREATION_DATE] = SOURCE.[PASSWORD_CREATION_DATE]
				  ,[PASSWORD_NEVER_EXPIRE] = SOURCE.[PASSWORD_NEVER_EXPIRE]
				  ,[FROM_EMAIL_ADDRESS] = SOURCE.[FROM_EMAIL_ADDRESS]
				  ,[LDAP_NEVER_SYNC] = SOURCE.[LDAP_NEVER_SYNC]
				  ,[LDAP_ID_SYNC] = SOURCE.[LDAP_ID_SYNC]
				  ,[ID_CLIENT_MODEL_PROCESSOR] = SOURCE.[ID_CLIENT_MODEL_PROCESSOR]
				  ,[FACTORY_USER] = SOURCE.[FACTORY_USER]
				  ,[IS_ENABLED_SMART_CLIENT] = SOURCE.[IS_ENABLED_SMART_CLIENT]
				  ,[SMART_CLIENT_PDF_CONV_ON_SCAN] = SOURCE.[SMART_CLIENT_PDF_CONV_ON_SCAN]
				  ,[LDAP_AUTHENTICATED] = SOURCE.[LDAP_AUTHENTICATED]
				  ,[ACCETTAZIONE_DISSERV] = SOURCE.[ACCETTAZIONE_DISSERV]
				  ,[ID_DISPOSITIVO_STAMPA] = SOURCE.[ID_DISPOSITIVO_STAMPA]
				  ,[ABILITATO_CENTRO_SERVIZI] = SOURCE.[ABILITATO_CENTRO_SERVIZI]
				  ,[MATRICOLA] = SOURCE.[MATRICOLA]
				  ,[ABILITATO_CHIAVI_CONFIG] = SOURCE.[ABILITATO_CHIAVI_CONFIG]
				  ,[CHA_TIPO_COMPONENTI] = SOURCE.[CHA_TIPO_COMPONENTI]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
				  ,[USER_ID]
				  ,[FULL_NAME]
				  ,[DISABLED]
				  ,[USER_PASSWORD]
				  ,[USER_LOCATION]
				  ,[PHONE]
				  ,[EXTENSION]
				  ,[LAST_LOGIN_DATE]
				  ,[LAST_LOGIN_TIME]
				  ,[ALLOW_LOGIN]
				  ,[FAX]
				  ,[DID]
				  ,[TARGET_DOCSRVR]
				  ,[PRIMARY_GROUP]
				  ,[PRIMARY_LIB]
				  ,[PROFILE_DEFAULTS]
				  ,[CONNECT_BRIDGED]
				  ,[BUTTON_BAR]
				  ,[NETWORK_ID]
				  ,[ACL_DEFAULTS]
				  ,[SHOW_RESTORED]
				  ,[PASS_EXP_DATE]
				  ,[LOGINS_REMAINING]
				  ,[PSWORD_VALID_FOR]
				  ,[NO_EXP_DATE]
				  ,[DR_USER]
				  ,[SEARCH_FORM_ID]
				  ,[EMAIL_ADDRESS]
				  ,[LAST_UPDATE]
				  ,[CHA_AMMINISTRATORE]
				  ,[CHA_NOTIFICA]
				  ,[VAR_TELEFONO]
				  ,[ID_AMM]
				  ,[CHA_RESP_ASS]
				  ,[CHA_ASSEGNATARIO]
				  ,[VAR_COGNOME]
				  ,[VAR_NOME]
				  ,[CHA_NOTIFICA_CON_ALLEGATO]
				  ,[VAR_SEDE]
				  ,[ENCRYPTED_PASSWORD]
				  ,[PASSWORD_CREATION_DATE]
				  ,[PASSWORD_NEVER_EXPIRE]
				  ,[FROM_EMAIL_ADDRESS]
				  ,[LDAP_NEVER_SYNC]
				  ,[LDAP_ID_SYNC]
				  ,[ID_CLIENT_MODEL_PROCESSOR]
				  ,[FACTORY_USER]
				  ,[IS_ENABLED_SMART_CLIENT]
				  ,[SMART_CLIENT_PDF_CONV_ON_SCAN]
				  ,[LDAP_AUTHENTICATED]
				  ,[ACCETTAZIONE_DISSERV]
				  ,[ID_DISPOSITIVO_STAMPA]
				  ,[ABILITATO_CENTRO_SERVIZI]
				  ,[MATRICOLA]
				  ,[ABILITATO_CHIAVI_CONFIG]
				  ,[CHA_TIPO_COMPONENTI])
				VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[USER_ID]
				  ,SOURCE.[FULL_NAME]
				  ,SOURCE.[DISABLED]
				  ,SOURCE.[USER_PASSWORD]
				  ,SOURCE.[USER_LOCATION]
				  ,SOURCE.[PHONE]
				  ,SOURCE.[EXTENSION]
				  ,SOURCE.[LAST_LOGIN_DATE]
				  ,SOURCE.[LAST_LOGIN_TIME]
				  ,SOURCE.[ALLOW_LOGIN]
				  ,SOURCE.[FAX]
				  ,SOURCE.[DID]
				  ,SOURCE.[TARGET_DOCSRVR]
				  ,SOURCE.[PRIMARY_GROUP]
				  ,SOURCE.[PRIMARY_LIB]
				  ,SOURCE.[PROFILE_DEFAULTS]
				  ,SOURCE.[CONNECT_BRIDGED]
				  ,SOURCE.[BUTTON_BAR]
				  ,SOURCE.[NETWORK_ID]
				  ,SOURCE.[ACL_DEFAULTS]
				  ,SOURCE.[SHOW_RESTORED]
				  ,SOURCE.[PASS_EXP_DATE]
				  ,SOURCE.[LOGINS_REMAINING]
				  ,SOURCE.[PSWORD_VALID_FOR]
				  ,SOURCE.[NO_EXP_DATE]
				  ,SOURCE.[DR_USER]
				  ,SOURCE.[SEARCH_FORM_ID]
				  ,SOURCE.[EMAIL_ADDRESS]
				  ,SOURCE.[LAST_UPDATE]
				  ,SOURCE.[CHA_AMMINISTRATORE]
				  ,SOURCE.[CHA_NOTIFICA]
				  ,SOURCE.[VAR_TELEFONO]
				  ,SOURCE.[ID_AMM]
				  ,SOURCE.[CHA_RESP_ASS]
				  ,SOURCE.[CHA_ASSEGNATARIO]
				  ,SOURCE.[VAR_COGNOME]
				  ,SOURCE.[VAR_NOME]
				  ,SOURCE.[CHA_NOTIFICA_CON_ALLEGATO]
				  ,SOURCE.[VAR_SEDE]
				  ,SOURCE.[ENCRYPTED_PASSWORD]
				  ,SOURCE.[PASSWORD_CREATION_DATE]
				  ,SOURCE.[PASSWORD_NEVER_EXPIRE]
				  ,SOURCE.[FROM_EMAIL_ADDRESS]
				  ,SOURCE.[LDAP_NEVER_SYNC]
				  ,SOURCE.[LDAP_ID_SYNC]
				  ,SOURCE.[ID_CLIENT_MODEL_PROCESSOR]
				  ,SOURCE.[FACTORY_USER]
				  ,SOURCE.[IS_ENABLED_SMART_CLIENT]
				  ,SOURCE.[SMART_CLIENT_PDF_CONV_ON_SCAN]
				  ,SOURCE.[LDAP_AUTHENTICATED]
				  ,SOURCE.[ACCETTAZIONE_DISSERV]
				  ,SOURCE.[ID_DISPOSITIVO_STAMPA]
				  ,SOURCE.[ABILITATO_CENTRO_SERVIZI]
				  ,SOURCE.[MATRICOLA]
				  ,SOURCE.[ABILITATO_CHIAVI_CONFIG]
				  ,SOURCE.[CHA_TIPO_COMPONENTI]);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella People'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento della tabella People'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- *******************************
		-- DPA_CORR_GLOBALI (Organigramma)
		-- *******************************
		
		SET @sql_string = CAST(N'MERGE DPA_CORR_GLOBALI AS TARGET
			USING (SELECT SYSTEM_ID
					,ID_REGISTRO
					,ID_AMM
					,VAR_COD_RUBRICA
					,VAR_DESC_CORR
					,ID_OLD
					,DTA_INIZIO
					,DTA_FINE
					,ID_PARENT
					,NUM_LIVELLO
					,VAR_CODICE
					,ID_GRUPPO
					,ID_TIPO_RUOLO
					,CHA_DEFAULT_TRASM
					,ID_UO
					,VAR_COGNOME
					,VAR_NOME
					,ID_PEOPLE
					,CHA_TIPO_CORR
					,CHA_TIPO_IE
					,CHA_TIPO_URP
					,CHA_PA
					,VAR_CODICE_AOO
					,VAR_CODICE_AMM
					,VAR_CODICE_ISTAT
					,ID_PESO
					,VAR_EMAIL
					,CHA_DETTAGLI
					,NUM_FIGLI
					,VAR_SMTP
					,NUM_PORTA_SMTP
					,VAR_FAX_USER_LOGIN
					,CHA_RIFERIMENTO
					,ID_PEOPLE_LISTE
					,ID_GRUPPO_LISTE
					,CHA_RESPONSABILE
					,ID_PESO_ORG
					,CHA_SEGRETARIO
					,ID_RF
					,COD_DESC_INTEROP
					,VAR_CHIAVE_AE
					,CHA_DISABLED_TRASM
					,VAR_ORIGINAL_CODE
					,ORIGINAL_ID
					,VAR_INSERT_BY_INTEROP
					,VAR_DESC_CORR_OLD
					,INTEROPRFID
					,CLASSIFICA_UO
					,INTEROPURL
					,INTEROPREGISTRYID
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
				+ CAST(N'.DPA_CORR_GLOBALI WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST('
				 AND CHA_TIPO_IE = ''I'') 
			AS SOURCE (SYSTEM_ID
					,ID_REGISTRO
					,ID_AMM
					,VAR_COD_RUBRICA
					,VAR_DESC_CORR
					,ID_OLD
					,DTA_INIZIO
					,DTA_FINE
					,ID_PARENT
					,NUM_LIVELLO
					,VAR_CODICE
					,ID_GRUPPO
					,ID_TIPO_RUOLO
					,CHA_DEFAULT_TRASM
					,ID_UO
					,VAR_COGNOME
					,VAR_NOME
					,ID_PEOPLE
					,CHA_TIPO_CORR
					,CHA_TIPO_IE
					,CHA_TIPO_URP
					,CHA_PA
					,VAR_CODICE_AOO
					,VAR_CODICE_AMM
					,VAR_CODICE_ISTAT
					,ID_PESO
					,VAR_EMAIL
					,CHA_DETTAGLI
					,NUM_FIGLI
					,VAR_SMTP
					,NUM_PORTA_SMTP
					,VAR_FAX_USER_LOGIN
					,CHA_RIFERIMENTO
					,ID_PEOPLE_LISTE
					,ID_GRUPPO_LISTE
					,CHA_RESPONSABILE
					,ID_PESO_ORG
					,CHA_SEGRETARIO
					,ID_RF
					,COD_DESC_INTEROP
					,VAR_CHIAVE_AE
					,CHA_DISABLED_TRASM
					,VAR_ORIGINAL_CODE
					,ORIGINAL_ID
					,VAR_INSERT_BY_INTEROP
					,VAR_DESC_CORR_OLD
					,INTEROPRFID
					,CLASSIFICA_UO
					,INTEROPURL
					,INTEROPREGISTRYID
					)
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					SYSTEM_ID = SOURCE.SYSTEM_ID
					,ID_REGISTRO = SOURCE.ID_REGISTRO
					,ID_AMM = SOURCE.ID_AMM
					,VAR_COD_RUBRICA = SOURCE.VAR_COD_RUBRICA
					,VAR_DESC_CORR = SOURCE.VAR_DESC_CORR
					,ID_OLD = SOURCE.ID_OLD
					,DTA_INIZIO = SOURCE.DTA_INIZIO
					,DTA_FINE = SOURCE.DTA_FINE
					,ID_PARENT = SOURCE.ID_PARENT
					,NUM_LIVELLO = SOURCE.NUM_LIVELLO
					,VAR_CODICE = SOURCE.VAR_CODICE
					,ID_GRUPPO = SOURCE.ID_GRUPPO
					,ID_TIPO_RUOLO = SOURCE.ID_TIPO_RUOLO
					,CHA_DEFAULT_TRASM = SOURCE.CHA_DEFAULT_TRASM
					,ID_UO = SOURCE.ID_UO
					,VAR_COGNOME = SOURCE.VAR_COGNOME
					,VAR_NOME = SOURCE.VAR_NOME
					,ID_PEOPLE = SOURCE.ID_PEOPLE
					,CHA_TIPO_CORR = SOURCE.CHA_TIPO_CORR
					,CHA_TIPO_IE = SOURCE.CHA_TIPO_IE
					,CHA_TIPO_URP = SOURCE.CHA_TIPO_URP
					,CHA_PA = SOURCE.CHA_PA
					,VAR_CODICE_AOO = SOURCE.VAR_CODICE_AOO
					,VAR_CODICE_AMM = SOURCE.VAR_CODICE_AMM
					,VAR_CODICE_ISTAT = SOURCE.VAR_CODICE_ISTAT
					,ID_PESO = SOURCE.ID_PESO
					,VAR_EMAIL = SOURCE.VAR_EMAIL
					,CHA_DETTAGLI = SOURCE.CHA_DETTAGLI
					,NUM_FIGLI = SOURCE.NUM_FIGLI
					,VAR_SMTP = SOURCE.VAR_SMTP
					,NUM_PORTA_SMTP = SOURCE.NUM_PORTA_SMTP
					,VAR_FAX_USER_LOGIN = SOURCE.VAR_FAX_USER_LOGIN
					,CHA_RIFERIMENTO = SOURCE.CHA_RIFERIMENTO
					,ID_PEOPLE_LISTE = SOURCE.ID_PEOPLE_LISTE
					,ID_GRUPPO_LISTE = SOURCE.ID_GRUPPO_LISTE
					,CHA_RESPONSABILE = SOURCE.CHA_RESPONSABILE
					,ID_PESO_ORG = SOURCE.ID_PESO_ORG
					,CHA_SEGRETARIO = SOURCE.CHA_SEGRETARIO
					,ID_RF = SOURCE.ID_RF
					,COD_DESC_INTEROP = SOURCE.COD_DESC_INTEROP
					,VAR_CHIAVE_AE = SOURCE.VAR_CHIAVE_AE
					,CHA_DISABLED_TRASM = SOURCE.CHA_DISABLED_TRASM
					,VAR_ORIGINAL_CODE = SOURCE.VAR_ORIGINAL_CODE
					,ORIGINAL_ID = SOURCE.ORIGINAL_ID
					,VAR_INSERT_BY_INTEROP = SOURCE.VAR_INSERT_BY_INTEROP
					,VAR_DESC_CORR_OLD = SOURCE.VAR_DESC_CORR_OLD
					,INTEROPRFID = SOURCE.INTEROPRFID
					,CLASSIFICA_UO = SOURCE.CLASSIFICA_UO
					,INTEROPURL = SOURCE.INTEROPURL
					,INTEROPREGISTRYID = SOURCE.INTEROPREGISTRYID
			WHEN NOT MATCHED THEN
				INSERT (SYSTEM_ID
						,ID_REGISTRO
						,ID_AMM
						,VAR_COD_RUBRICA
						,VAR_DESC_CORR
						,ID_OLD
						,DTA_INIZIO
						,DTA_FINE
						,ID_PARENT
						,NUM_LIVELLO
						,VAR_CODICE
						,ID_GRUPPO
						,ID_TIPO_RUOLO
						,CHA_DEFAULT_TRASM
						,ID_UO
						,VAR_COGNOME
						,VAR_NOME
						,ID_PEOPLE
						,CHA_TIPO_CORR
						,CHA_TIPO_IE
						,CHA_TIPO_URP
						,CHA_PA
						,VAR_CODICE_AOO
						,VAR_CODICE_AMM
						,VAR_CODICE_ISTAT
						,ID_PESO
						,VAR_EMAIL
						,CHA_DETTAGLI
						,NUM_FIGLI
						,VAR_SMTP
						,NUM_PORTA_SMTP
						,VAR_FAX_USER_LOGIN
						,CHA_RIFERIMENTO
						,ID_PEOPLE_LISTE
						,ID_GRUPPO_LISTE
						,CHA_RESPONSABILE
						,ID_PESO_ORG
						,CHA_SEGRETARIO
						,ID_RF
						,COD_DESC_INTEROP
						,VAR_CHIAVE_AE
						,CHA_DISABLED_TRASM
						,VAR_ORIGINAL_CODE
						,ORIGINAL_ID
						,VAR_INSERT_BY_INTEROP
						,VAR_DESC_CORR_OLD
						,INTEROPRFID
						,CLASSIFICA_UO
						,INTEROPURL
						,INTEROPREGISTRYID
						)
				VALUES (SOURCE.SYSTEM_ID
						,SOURCE.ID_REGISTRO
						,SOURCE.ID_AMM
						,SOURCE.VAR_COD_RUBRICA
						,SOURCE.VAR_DESC_CORR
						,SOURCE.ID_OLD
						,SOURCE.DTA_INIZIO
						,SOURCE.DTA_FINE
						,SOURCE.ID_PARENT
						,SOURCE.NUM_LIVELLO
						,SOURCE.VAR_CODICE
						,SOURCE.ID_GRUPPO
						,SOURCE.ID_TIPO_RUOLO
						,SOURCE.CHA_DEFAULT_TRASM
						,SOURCE.ID_UO
						,SOURCE.VAR_COGNOME
						,SOURCE.VAR_NOME
						,SOURCE.ID_PEOPLE
						,SOURCE.CHA_TIPO_CORR
						,SOURCE.CHA_TIPO_IE
						,SOURCE.CHA_TIPO_URP
						,SOURCE.CHA_PA
						,SOURCE.VAR_CODICE_AOO
						,SOURCE.VAR_CODICE_AMM
						,SOURCE.VAR_CODICE_ISTAT
						,SOURCE.ID_PESO
						,SOURCE.VAR_EMAIL
						,SOURCE.CHA_DETTAGLI
						,SOURCE.NUM_FIGLI
						,SOURCE.VAR_SMTP
						,SOURCE.NUM_PORTA_SMTP
						,SOURCE.VAR_FAX_USER_LOGIN
						,SOURCE.CHA_RIFERIMENTO
						,SOURCE.ID_PEOPLE_LISTE
						,SOURCE.ID_GRUPPO_LISTE
						,SOURCE.CHA_RESPONSABILE
						,SOURCE.ID_PESO_ORG
						,SOURCE.CHA_SEGRETARIO
						,SOURCE.ID_RF
						,SOURCE.COD_DESC_INTEROP
						,SOURCE.VAR_CHIAVE_AE
						,SOURCE.CHA_DISABLED_TRASM
						,SOURCE.VAR_ORIGINAL_CODE
						,SOURCE.ORIGINAL_ID
						,SOURCE.VAR_INSERT_BY_INTEROP
						,SOURCE.VAR_DESC_CORR_OLD
						,SOURCE.INTEROPRFID
						,SOURCE.CLASSIFICA_UO
						,SOURCE.INTEROPURL
						,SOURCE.INTEROPREGISTRYID
						);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento dell''organigramma'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento organigramma'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- Disabilita l'organigramma corrente aggiornando la data fine con la data/ora corrente.
		-- Nell'aggiornamento sono esclusi i nodi (UO/Ruli/Utenti) dell'organigramma creati per l'archivio
		--
		SET @sql_string = CAST(N'
			UPDATE DPA_CORR_GLOBALI 
			SET DTA_FINE = GETDATE() 
			WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(N' 
			AND DTA_FINE IS NULL
			AND CHA_TIPO_IE = ''I''
			AND SYSTEM_ID NOT IN
				(
				SELECT SYSTEM_ID FROM ARCHIVE_ORGANIZATIONALCHART
				)' AS NVARCHAR(MAX))
			
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della data fine per gli elementi dell''organigramma'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento data fine per gli elementi dell''organigramma'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- ****************
		-- DPA_DETT_GLOBALI
		-- ****************
		
		SET @sql_string = CAST(N'MERGE DPA_DETT_GLOBALI AS TARGET
			USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_CORR_GLOBALI]
				  ,[VAR_INDIRIZZO]
				  ,[VAR_CAP]
				  ,[VAR_PROVINCIA]
				  ,[VAR_NAZIONE]
				  ,[VAR_COD_FISCALE]
				  ,[VAR_TELEFONO]
				  ,[VAR_TELEFONO2]
				  ,[VAR_FAX]
				  ,[VAR_NOTE]
				  ,[VAR_COD_FISC]
				  ,[VAR_CITTA]
				  ,[VAR_LOCALITA]
				  ,[VAR_LUOGO_NASCITA]
				  ,[VAR_TITOLO]
				  ,[DTA_NASCITA]
				  ,[ID_QUALIFICA_CORR]
				  ,[CHA_SESSO]
				  ,[CHAR_PROVINCIA_NASCITA]
				  ,[VAR_COD_PI]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_DETT_GLOBALI]
			  WHERE [ID_CORR_GLOBALI] IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI)
			) 
			AS SOURCE ([SYSTEM_ID]
				  ,[ID_CORR_GLOBALI]
				  ,[VAR_INDIRIZZO]
				  ,[VAR_CAP]
				  ,[VAR_PROVINCIA]
				  ,[VAR_NAZIONE]
				  ,[VAR_COD_FISCALE]
				  ,[VAR_TELEFONO]
				  ,[VAR_TELEFONO2]
				  ,[VAR_FAX]
				  ,[VAR_NOTE]
				  ,[VAR_COD_FISC]
				  ,[VAR_CITTA]
				  ,[VAR_LOCALITA]
				  ,[VAR_LUOGO_NASCITA]
				  ,[VAR_TITOLO]
				  ,[DTA_NASCITA]
				  ,[ID_QUALIFICA_CORR]
				  ,[CHA_SESSO]
				  ,[CHAR_PROVINCIA_NASCITA]
				  ,[VAR_COD_PI])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[ID_CORR_GLOBALI] = SOURCE.[ID_CORR_GLOBALI]
				  ,[VAR_INDIRIZZO] = SOURCE.[VAR_INDIRIZZO]
				  ,[VAR_CAP] = SOURCE.[VAR_CAP]
				  ,[VAR_PROVINCIA] = SOURCE.[VAR_PROVINCIA]
				  ,[VAR_NAZIONE] = SOURCE.[VAR_NAZIONE]
				  ,[VAR_COD_FISCALE] = SOURCE.[VAR_COD_FISCALE]
				  ,[VAR_TELEFONO] = SOURCE.[VAR_TELEFONO]
				  ,[VAR_TELEFONO2] = SOURCE.[VAR_TELEFONO2]
				  ,[VAR_FAX] = SOURCE.[VAR_FAX]
				  ,[VAR_NOTE] = SOURCE.[VAR_NOTE]
				  ,[VAR_COD_FISC] = SOURCE.[VAR_COD_FISC]
				  ,[VAR_CITTA] = SOURCE.[VAR_CITTA]
				  ,[VAR_LOCALITA] = SOURCE.[VAR_LOCALITA]
				  ,[VAR_LUOGO_NASCITA] = SOURCE.[VAR_LUOGO_NASCITA]
				  ,[VAR_TITOLO] = SOURCE.[VAR_TITOLO]
				  ,[DTA_NASCITA] = SOURCE.[DTA_NASCITA]
				  ,[ID_QUALIFICA_CORR] = SOURCE.[ID_QUALIFICA_CORR]
				  ,[CHA_SESSO] = SOURCE.[CHA_SESSO]
				  ,[CHAR_PROVINCIA_NASCITA] = SOURCE.[CHAR_PROVINCIA_NASCITA]
				  ,[VAR_COD_PI] = SOURCE.[VAR_COD_PI]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
				  ,[ID_CORR_GLOBALI]
				  ,[VAR_INDIRIZZO]
				  ,[VAR_CAP]
				  ,[VAR_PROVINCIA]
				  ,[VAR_NAZIONE]
				  ,[VAR_COD_FISCALE]
				  ,[VAR_TELEFONO]
				  ,[VAR_TELEFONO2]
				  ,[VAR_FAX]
				  ,[VAR_NOTE]
				  ,[VAR_COD_FISC]
				  ,[VAR_CITTA]
				  ,[VAR_LOCALITA]
				  ,[VAR_LUOGO_NASCITA]
				  ,[VAR_TITOLO]
				  ,[DTA_NASCITA]
				  ,[ID_QUALIFICA_CORR]
				  ,[CHA_SESSO]
				  ,[CHAR_PROVINCIA_NASCITA]
				  ,[VAR_COD_PI])
				VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_CORR_GLOBALI]
				  ,SOURCE.[VAR_INDIRIZZO]
				  ,SOURCE.[VAR_CAP]
				  ,SOURCE.[VAR_PROVINCIA]
				  ,SOURCE.[VAR_NAZIONE]
				  ,SOURCE.[VAR_COD_FISCALE]
				  ,SOURCE.[VAR_TELEFONO]
				  ,SOURCE.[VAR_TELEFONO2]
				  ,SOURCE.[VAR_FAX]
				  ,SOURCE.[VAR_NOTE]
				  ,SOURCE.[VAR_COD_FISC]
				  ,SOURCE.[VAR_CITTA]
				  ,SOURCE.[VAR_LOCALITA]
				  ,SOURCE.[VAR_LUOGO_NASCITA]
				  ,SOURCE.[VAR_TITOLO]
				  ,SOURCE.[DTA_NASCITA]
				  ,SOURCE.[ID_QUALIFICA_CORR]
				  ,SOURCE.[CHA_SESSO]
				  ,SOURCE.[CHAR_PROVINCIA_NASCITA]
				  ,SOURCE.[VAR_COD_PI]);' AS NVARCHAR(MAX))
			
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_DETT_GLOBALI'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_DETT_GLOBALI'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- ******
		-- GROUPS
		-- ******

		-- GROUPS (select * from schemaCorrente.GROUPS g where g.SYSTEM_ID in (select distinct id_gruppo from DPA_CORR_GLOBALI))
		SET @sql_string = CAST(N'MERGE GROUPS AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[GROUP_ID]
				  ,[NETWORK_ID]
				  ,[GROUP_NAME]
				  ,[PROFILE_DEFAULTS]
				  ,[DISABLED]
				  ,[ALLOW_LOGIN]
				  ,[UNIV_ACCESS]
				  ,[DELETE_VERSIONS]
				  ,[EDIT_PREVIOUS_VER]
				  ,[MAX_VERSIONS]
				  ,[MAX_SUBVERSIONS]
				  ,[NEW_VERSIONS]
				  ,[SAVE_TO_REM_LIB]
				  ,[PRECONNECT_LIBS]
				  ,[NV_AUTHOR_EDIT]
				  ,[NV_ENTERED_BY]
				  ,[NV_BILLABLE]
				  ,[DISPLAY_VER_LIST]
				  ,[MV_DOCS_IF_CHNG]
				  ,[CHECKOUT]
				  ,[OTHER_CHECKIN]
				  ,[CHECKIN_REMINDER]
				  ,[RESET_STATUS]
				  ,[COPY_IN_USE]
				  ,[TEMPLATE_MANAGER]
				  ,[MASS_UPD_PROFILES]
				  ,[AUTO_LOGIN]
				  ,[VIEW_UNSECURED]
				  ,[PROMPT_PAGES]
				  ,[NONBILL_PAGES]
				  ,[DEFAULT_PAGES]
				  ,[GET_EDIT_INFO]
				  ,[VISIT_AUTHOR_EDIT]
				  ,[VISIT_ENTERED_BY]
				  ,[LCP_RUN]
				  ,[EDIT_VTS]
				  ,[EDIT_LIBPARMS]
				  ,[EDIT_WS_PARAMS]
				  ,[EDIT_USER_DEFAULTS]
				  ,[MANAGE_GROUPS]
				  ,[DI_RUN]
				  ,[MBLINST_RUN]
				  ,[DI_MANAGE]
				  ,[CR_RUN]
				  ,[SM_RUN]
				  ,[DD_RUN]
				  ,[DB_EDIT]
				  ,[DBI_RUN]
				  ,[INDEXER_RUN]
				  ,[INTERCHANGE_RUN]
				  ,[PROFSEC]
				  ,[ALLOW_DOC_DELETE]
				  ,[ALLOW_CONTENT_DEL]
				  ,[ALLOW_QUEUE_DEL]
				  ,[PROFILE_FORM]
				  ,[HITLIST_FORM]
				  ,[ACL_DEFAULTS]
				  ,[REMOVE_MON_LIST]
				  ,[DISPLAY_MON_LIST]
				  ,[WARN_SECURE]
				  ,[ONLY_READONLY]
				  ,[FORCE_CHECKIN]
				  ,[MBL_EDITCOPY]
				  ,[MBL_OVERWRITE]
				  ,[MIN_DISKFREE]
				  ,[AUTOCLEAN]
				  ,[DEF_SHAD_RETENTION]
				  ,[SHADOW_DOCS]
				  ,[SHADOW_PROFILES]
				  ,[SHADOW_SEC_DOCS]
				  ,[DEFAULT_FT_INDEX]
				  ,[DISABLE_NATIVE]
				  ,[MAKE_READ_ONLY]
				  ,[REMOVE_READ_ONLY]
				  ,[MAKE_VER_READONLY]
				  ,[MAKE_VER_WRITABLE]
				  ,[PUBLISH_VERSION]
				  ,[UNPUBLISH_VERSION]
				  ,[DATE_FORMAT]
				  ,[TIME_FORMAT]
				  ,[ITEM_MAX]
				  ,[PAGE_MAX]
				  ,[DEFAULT_VIEWER]
				  ,[MANAGE_PRF]
				  ,[MANAGE_CYD]
				  ,[DPACKAGE]
				  ,[ALLOW_APPINT]
				  ,[ALLOW_USRSETTINGS]
				  ,[ALLOW_DOC_CREATE]
				  ,[CREATE_FOLDER]
				  ,[ROOT_FOLDER]
				  ,[CREATE_RELATION]
				  ,[SHOW_RELATED]
				  ,[REMOVE_RELATION]
				  ,[ALLOW_NOTIF]
				  ,[ALLOW_PREVIEW]
				  ,[WARN_UPDATE_AVAIL]
				  ,[ENABLE_WORKSPACE]
				  ,[LAST_UPDATE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[GROUPS]
			WHERE SYSTEM_ID IN 
				(
				SELECT DISTINCT ID_GRUPPO 
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_CORR_GLOBALI 
				WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST('
				)
		) 
		AS SOURCE ([SYSTEM_ID]
				  ,[GROUP_ID]
				  ,[NETWORK_ID]
				  ,[GROUP_NAME]
				  ,[PROFILE_DEFAULTS]
				  ,[DISABLED]
				  ,[ALLOW_LOGIN]
				  ,[UNIV_ACCESS]
				  ,[DELETE_VERSIONS]
				  ,[EDIT_PREVIOUS_VER]
				  ,[MAX_VERSIONS]
				  ,[MAX_SUBVERSIONS]
				  ,[NEW_VERSIONS]
				  ,[SAVE_TO_REM_LIB]
				  ,[PRECONNECT_LIBS]
				  ,[NV_AUTHOR_EDIT]
				  ,[NV_ENTERED_BY]
				  ,[NV_BILLABLE]
				  ,[DISPLAY_VER_LIST]
				  ,[MV_DOCS_IF_CHNG]
				  ,[CHECKOUT]
				  ,[OTHER_CHECKIN]
				  ,[CHECKIN_REMINDER]
				  ,[RESET_STATUS]
				  ,[COPY_IN_USE]
				  ,[TEMPLATE_MANAGER]
				  ,[MASS_UPD_PROFILES]
				  ,[AUTO_LOGIN]
				  ,[VIEW_UNSECURED]
				  ,[PROMPT_PAGES]
				  ,[NONBILL_PAGES]
				  ,[DEFAULT_PAGES]
				  ,[GET_EDIT_INFO]
				  ,[VISIT_AUTHOR_EDIT]
				  ,[VISIT_ENTERED_BY]
				  ,[LCP_RUN]
				  ,[EDIT_VTS]
				  ,[EDIT_LIBPARMS]
				  ,[EDIT_WS_PARAMS]
				  ,[EDIT_USER_DEFAULTS]
				  ,[MANAGE_GROUPS]
				  ,[DI_RUN]
				  ,[MBLINST_RUN]
				  ,[DI_MANAGE]
				  ,[CR_RUN]
				  ,[SM_RUN]
				  ,[DD_RUN]
				  ,[DB_EDIT]
				  ,[DBI_RUN]
				  ,[INDEXER_RUN]
				  ,[INTERCHANGE_RUN]
				  ,[PROFSEC]
				  ,[ALLOW_DOC_DELETE]
				  ,[ALLOW_CONTENT_DEL]
				  ,[ALLOW_QUEUE_DEL]
				  ,[PROFILE_FORM]
				  ,[HITLIST_FORM]
				  ,[ACL_DEFAULTS]
				  ,[REMOVE_MON_LIST]
				  ,[DISPLAY_MON_LIST]
				  ,[WARN_SECURE]
				  ,[ONLY_READONLY]
				  ,[FORCE_CHECKIN]
				  ,[MBL_EDITCOPY]
				  ,[MBL_OVERWRITE]
				  ,[MIN_DISKFREE]
				  ,[AUTOCLEAN]
				  ,[DEF_SHAD_RETENTION]
				  ,[SHADOW_DOCS]
				  ,[SHADOW_PROFILES]
				  ,[SHADOW_SEC_DOCS]
				  ,[DEFAULT_FT_INDEX]
				  ,[DISABLE_NATIVE]
				  ,[MAKE_READ_ONLY]
				  ,[REMOVE_READ_ONLY]
				  ,[MAKE_VER_READONLY]
				  ,[MAKE_VER_WRITABLE]
				  ,[PUBLISH_VERSION]
				  ,[UNPUBLISH_VERSION]
				  ,[DATE_FORMAT]
				  ,[TIME_FORMAT]
				  ,[ITEM_MAX]
				  ,[PAGE_MAX]
				  ,[DEFAULT_VIEWER]
				  ,[MANAGE_PRF]
				  ,[MANAGE_CYD]
				  ,[DPACKAGE]
				  ,[ALLOW_APPINT]
				  ,[ALLOW_USRSETTINGS]
				  ,[ALLOW_DOC_CREATE]
				  ,[CREATE_FOLDER]
				  ,[ROOT_FOLDER]
				  ,[CREATE_RELATION]
				  ,[SHOW_RELATED]
				  ,[REMOVE_RELATION]
				  ,[ALLOW_NOTIF]
				  ,[ALLOW_PREVIEW]
				  ,[WARN_UPDATE_AVAIL]
				  ,[ENABLE_WORKSPACE]
				  ,[LAST_UPDATE])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[GROUP_ID] = SOURCE.[GROUP_ID]
				  ,[NETWORK_ID] = SOURCE.[NETWORK_ID]
				  ,[GROUP_NAME] = SOURCE.[GROUP_NAME]
				  ,[PROFILE_DEFAULTS] = SOURCE.[PROFILE_DEFAULTS]
				  ,[DISABLED] = SOURCE.[DISABLED]
				  ,[ALLOW_LOGIN] = SOURCE.[ALLOW_LOGIN]
				  ,[UNIV_ACCESS] = SOURCE.[UNIV_ACCESS]
				  ,[DELETE_VERSIONS] = SOURCE.[DELETE_VERSIONS]
				  ,[EDIT_PREVIOUS_VER] = SOURCE.[EDIT_PREVIOUS_VER]
				  ,[MAX_VERSIONS] = SOURCE.[MAX_VERSIONS]
				  ,[MAX_SUBVERSIONS] = SOURCE.[MAX_SUBVERSIONS]
				  ,[NEW_VERSIONS] = SOURCE.[NEW_VERSIONS]
				  ,[SAVE_TO_REM_LIB] = SOURCE.[SAVE_TO_REM_LIB]
				  ,[PRECONNECT_LIBS] = SOURCE.[PRECONNECT_LIBS]
				  ,[NV_AUTHOR_EDIT] = SOURCE.[NV_AUTHOR_EDIT]
				  ,[NV_ENTERED_BY] = SOURCE.[NV_ENTERED_BY]
				  ,[NV_BILLABLE] = SOURCE.[NV_BILLABLE]
				  ,[DISPLAY_VER_LIST] = SOURCE.[DISPLAY_VER_LIST]
				  ,[MV_DOCS_IF_CHNG] = SOURCE.[MV_DOCS_IF_CHNG]
				  ,[CHECKOUT] = SOURCE.[CHECKOUT]
				  ,[OTHER_CHECKIN] = SOURCE.[OTHER_CHECKIN]
				  ,[CHECKIN_REMINDER] = SOURCE.[CHECKIN_REMINDER]
				  ,[RESET_STATUS] = SOURCE.[RESET_STATUS]
				  ,[COPY_IN_USE] = SOURCE.[COPY_IN_USE]
				  ,[TEMPLATE_MANAGER] = SOURCE.[TEMPLATE_MANAGER]
				  ,[MASS_UPD_PROFILES] = SOURCE.[MASS_UPD_PROFILES]
				  ,[AUTO_LOGIN] = SOURCE.[AUTO_LOGIN]
				  ,[VIEW_UNSECURED] = SOURCE.[VIEW_UNSECURED]
				  ,[PROMPT_PAGES] = SOURCE.[PROMPT_PAGES]
				  ,[NONBILL_PAGES] = SOURCE.[NONBILL_PAGES]
				  ,[DEFAULT_PAGES] = SOURCE.[DEFAULT_PAGES]
				  ,[GET_EDIT_INFO] = SOURCE.[GET_EDIT_INFO]
				  ,[VISIT_AUTHOR_EDIT] = SOURCE.[VISIT_AUTHOR_EDIT]
				  ,[VISIT_ENTERED_BY] = SOURCE.[VISIT_ENTERED_BY]
				  ,[LCP_RUN] = SOURCE.[LCP_RUN]
				  ,[EDIT_VTS] = SOURCE.[EDIT_VTS]
				  ,[EDIT_LIBPARMS] = SOURCE.[EDIT_LIBPARMS]
				  ,[EDIT_WS_PARAMS] = SOURCE.[EDIT_WS_PARAMS]
				  ,[EDIT_USER_DEFAULTS] = SOURCE.[EDIT_USER_DEFAULTS]
				  ,[MANAGE_GROUPS] = SOURCE.[MANAGE_GROUPS]
				  ,[DI_RUN] = SOURCE.[DI_RUN]
				  ,[MBLINST_RUN] = SOURCE.[MBLINST_RUN]
				  ,[DI_MANAGE] = SOURCE.[DI_MANAGE]
				  ,[CR_RUN] = SOURCE.[CR_RUN]
				  ,[SM_RUN] = SOURCE.[SM_RUN]
				  ,[DD_RUN] = SOURCE.[DD_RUN]
				  ,[DB_EDIT] = SOURCE.[DB_EDIT]
				  ,[DBI_RUN] = SOURCE.[DBI_RUN]
				  ,[INDEXER_RUN] = SOURCE.[INDEXER_RUN]
				  ,[INTERCHANGE_RUN] = SOURCE.[INTERCHANGE_RUN]
				  ,[PROFSEC] = SOURCE.[PROFSEC]
				  ,[ALLOW_DOC_DELETE] = SOURCE.[ALLOW_DOC_DELETE]
				  ,[ALLOW_CONTENT_DEL] = SOURCE.[ALLOW_CONTENT_DEL]
				  ,[ALLOW_QUEUE_DEL] = SOURCE.[ALLOW_QUEUE_DEL]
				  ,[PROFILE_FORM] = SOURCE.[PROFILE_FORM]
				  ,[HITLIST_FORM] = SOURCE.[HITLIST_FORM]
				  ,[ACL_DEFAULTS] = SOURCE.[ACL_DEFAULTS]
				  ,[REMOVE_MON_LIST] = SOURCE.[REMOVE_MON_LIST]
				  ,[DISPLAY_MON_LIST] = SOURCE.[DISPLAY_MON_LIST]
				  ,[WARN_SECURE] = SOURCE.[WARN_SECURE]
				  ,[ONLY_READONLY] = SOURCE.[ONLY_READONLY]
				  ,[FORCE_CHECKIN] = SOURCE.[FORCE_CHECKIN]
				  ,[MBL_EDITCOPY] = SOURCE.[MBL_EDITCOPY]
				  ,[MBL_OVERWRITE] = SOURCE.[MBL_OVERWRITE]
				  ,[MIN_DISKFREE] = SOURCE.[MIN_DISKFREE]
				  ,[AUTOCLEAN] = SOURCE.[AUTOCLEAN]
				  ,[DEF_SHAD_RETENTION] = SOURCE.[DEF_SHAD_RETENTION]
				  ,[SHADOW_DOCS] = SOURCE.[SHADOW_DOCS]
				  ,[SHADOW_PROFILES] = SOURCE.[SHADOW_PROFILES]
				  ,[SHADOW_SEC_DOCS] = SOURCE.[SHADOW_SEC_DOCS]
				  ,[DEFAULT_FT_INDEX] = SOURCE.[DEFAULT_FT_INDEX]
				  ,[DISABLE_NATIVE] = SOURCE.[DISABLE_NATIVE]
				  ,[MAKE_READ_ONLY] = SOURCE.[MAKE_READ_ONLY]
				  ,[REMOVE_READ_ONLY] = SOURCE.[REMOVE_READ_ONLY]
				  ,[MAKE_VER_READONLY] = SOURCE.[MAKE_VER_READONLY]
				  ,[MAKE_VER_WRITABLE] = SOURCE.[MAKE_VER_WRITABLE]
				  ,[PUBLISH_VERSION] = SOURCE.[PUBLISH_VERSION]
				  ,[UNPUBLISH_VERSION] = SOURCE.[UNPUBLISH_VERSION]
				  ,[DATE_FORMAT] = SOURCE.[DATE_FORMAT]
				  ,[TIME_FORMAT] = SOURCE.[TIME_FORMAT]
				  ,[ITEM_MAX] = SOURCE.[ITEM_MAX]
				  ,[PAGE_MAX] = SOURCE.[PAGE_MAX]
				  ,[DEFAULT_VIEWER] = SOURCE.[DEFAULT_VIEWER]
				  ,[MANAGE_PRF] = SOURCE.[MANAGE_PRF]
				  ,[MANAGE_CYD] = SOURCE.[MANAGE_CYD]
				  ,[DPACKAGE] = SOURCE.[DPACKAGE]
				  ,[ALLOW_APPINT] = SOURCE.[ALLOW_APPINT]
				  ,[ALLOW_USRSETTINGS] = SOURCE.[ALLOW_USRSETTINGS]
				  ,[ALLOW_DOC_CREATE] = SOURCE.[ALLOW_DOC_CREATE]
				  ,[CREATE_FOLDER] = SOURCE.[CREATE_FOLDER]
				  ,[ROOT_FOLDER] = SOURCE.[ROOT_FOLDER]
				  ,[CREATE_RELATION] = SOURCE.[CREATE_RELATION]
				  ,[SHOW_RELATED] = SOURCE.[SHOW_RELATED]
				  ,[REMOVE_RELATION] = SOURCE.[REMOVE_RELATION]
				  ,[ALLOW_NOTIF] = SOURCE.[ALLOW_NOTIF]
				  ,[ALLOW_PREVIEW] = SOURCE.[ALLOW_PREVIEW]
				  ,[WARN_UPDATE_AVAIL] = SOURCE.[WARN_UPDATE_AVAIL]
				  ,[ENABLE_WORKSPACE] = SOURCE.[ENABLE_WORKSPACE]
				  ,[LAST_UPDATE] = SOURCE.[LAST_UPDATE]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[GROUP_ID]
				  ,[NETWORK_ID]
				  ,[GROUP_NAME]
				  ,[PROFILE_DEFAULTS]
				  ,[DISABLED]
				  ,[ALLOW_LOGIN]
				  ,[UNIV_ACCESS]
				  ,[DELETE_VERSIONS]
				  ,[EDIT_PREVIOUS_VER]
				  ,[MAX_VERSIONS]
				  ,[MAX_SUBVERSIONS]
				  ,[NEW_VERSIONS]
				  ,[SAVE_TO_REM_LIB]
				  ,[PRECONNECT_LIBS]
				  ,[NV_AUTHOR_EDIT]
				  ,[NV_ENTERED_BY]
				  ,[NV_BILLABLE]
				  ,[DISPLAY_VER_LIST]
				  ,[MV_DOCS_IF_CHNG]
				  ,[CHECKOUT]
				  ,[OTHER_CHECKIN]
				  ,[CHECKIN_REMINDER]
				  ,[RESET_STATUS]
				  ,[COPY_IN_USE]
				  ,[TEMPLATE_MANAGER]
				  ,[MASS_UPD_PROFILES]
				  ,[AUTO_LOGIN]
				  ,[VIEW_UNSECURED]
				  ,[PROMPT_PAGES]
				  ,[NONBILL_PAGES]
				  ,[DEFAULT_PAGES]
				  ,[GET_EDIT_INFO]
				  ,[VISIT_AUTHOR_EDIT]
				  ,[VISIT_ENTERED_BY]
				  ,[LCP_RUN]
				  ,[EDIT_VTS]
				  ,[EDIT_LIBPARMS]
				  ,[EDIT_WS_PARAMS]
				  ,[EDIT_USER_DEFAULTS]
				  ,[MANAGE_GROUPS]
				  ,[DI_RUN]
				  ,[MBLINST_RUN]
				  ,[DI_MANAGE]
				  ,[CR_RUN]
				  ,[SM_RUN]
				  ,[DD_RUN]
				  ,[DB_EDIT]
				  ,[DBI_RUN]
				  ,[INDEXER_RUN]
				  ,[INTERCHANGE_RUN]
				  ,[PROFSEC]
				  ,[ALLOW_DOC_DELETE]
				  ,[ALLOW_CONTENT_DEL]
				  ,[ALLOW_QUEUE_DEL]
				  ,[PROFILE_FORM]
				  ,[HITLIST_FORM]
				  ,[ACL_DEFAULTS]
				  ,[REMOVE_MON_LIST]
				  ,[DISPLAY_MON_LIST]
				  ,[WARN_SECURE]
				  ,[ONLY_READONLY]
				  ,[FORCE_CHECKIN]
				  ,[MBL_EDITCOPY]
				  ,[MBL_OVERWRITE]
				  ,[MIN_DISKFREE]
				  ,[AUTOCLEAN]
				  ,[DEF_SHAD_RETENTION]
				  ,[SHADOW_DOCS]
				  ,[SHADOW_PROFILES]
				  ,[SHADOW_SEC_DOCS]
				  ,[DEFAULT_FT_INDEX]
				  ,[DISABLE_NATIVE]
				  ,[MAKE_READ_ONLY]
				  ,[REMOVE_READ_ONLY]
				  ,[MAKE_VER_READONLY]
				  ,[MAKE_VER_WRITABLE]
				  ,[PUBLISH_VERSION]
				  ,[UNPUBLISH_VERSION]
				  ,[DATE_FORMAT]
				  ,[TIME_FORMAT]
				  ,[ITEM_MAX]
				  ,[PAGE_MAX]
				  ,[DEFAULT_VIEWER]
				  ,[MANAGE_PRF]
				  ,[MANAGE_CYD]
				  ,[DPACKAGE]
				  ,[ALLOW_APPINT]
				  ,[ALLOW_USRSETTINGS]
				  ,[ALLOW_DOC_CREATE]
				  ,[CREATE_FOLDER]
				  ,[ROOT_FOLDER]
				  ,[CREATE_RELATION]
				  ,[SHOW_RELATED]
				  ,[REMOVE_RELATION]
				  ,[ALLOW_NOTIF]
				  ,[ALLOW_PREVIEW]
				  ,[WARN_UPDATE_AVAIL]
				  ,[ENABLE_WORKSPACE]
				  ,[LAST_UPDATE])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[GROUP_ID]
				  ,SOURCE.[NETWORK_ID]
				  ,SOURCE.[GROUP_NAME]
				  ,SOURCE.[PROFILE_DEFAULTS]
				  ,SOURCE.[DISABLED]
				  ,SOURCE.[ALLOW_LOGIN]
				  ,SOURCE.[UNIV_ACCESS]
				  ,SOURCE.[DELETE_VERSIONS]
				  ,SOURCE.[EDIT_PREVIOUS_VER]
				  ,SOURCE.[MAX_VERSIONS]
				  ,SOURCE.[MAX_SUBVERSIONS]
				  ,SOURCE.[NEW_VERSIONS]
				  ,SOURCE.[SAVE_TO_REM_LIB]
				  ,SOURCE.[PRECONNECT_LIBS]
				  ,SOURCE.[NV_AUTHOR_EDIT]
				  ,SOURCE.[NV_ENTERED_BY]
				  ,SOURCE.[NV_BILLABLE]
				  ,SOURCE.[DISPLAY_VER_LIST]
				  ,SOURCE.[MV_DOCS_IF_CHNG]
				  ,SOURCE.[CHECKOUT]
				  ,SOURCE.[OTHER_CHECKIN]
				  ,SOURCE.[CHECKIN_REMINDER]
				  ,SOURCE.[RESET_STATUS]
				  ,SOURCE.[COPY_IN_USE]
				  ,SOURCE.[TEMPLATE_MANAGER]
				  ,SOURCE.[MASS_UPD_PROFILES]
				  ,SOURCE.[AUTO_LOGIN]
				  ,SOURCE.[VIEW_UNSECURED]
				  ,SOURCE.[PROMPT_PAGES]
				  ,SOURCE.[NONBILL_PAGES]
				  ,SOURCE.[DEFAULT_PAGES]
				  ,SOURCE.[GET_EDIT_INFO]
				  ,SOURCE.[VISIT_AUTHOR_EDIT]
				  ,SOURCE.[VISIT_ENTERED_BY]
				  ,SOURCE.[LCP_RUN]
				  ,SOURCE.[EDIT_VTS]
				  ,SOURCE.[EDIT_LIBPARMS]
				  ,SOURCE.[EDIT_WS_PARAMS]
				  ,SOURCE.[EDIT_USER_DEFAULTS]
				  ,SOURCE.[MANAGE_GROUPS]
				  ,SOURCE.[DI_RUN]
				  ,SOURCE.[MBLINST_RUN]
				  ,SOURCE.[DI_MANAGE]
				  ,SOURCE.[CR_RUN]
				  ,SOURCE.[SM_RUN]
				  ,SOURCE.[DD_RUN]
				  ,SOURCE.[DB_EDIT]
				  ,SOURCE.[DBI_RUN]
				  ,SOURCE.[INDEXER_RUN]
				  ,SOURCE.[INTERCHANGE_RUN]
				  ,SOURCE.[PROFSEC]
				  ,SOURCE.[ALLOW_DOC_DELETE]
				  ,SOURCE.[ALLOW_CONTENT_DEL]
				  ,SOURCE.[ALLOW_QUEUE_DEL]
				  ,SOURCE.[PROFILE_FORM]
				  ,SOURCE.[HITLIST_FORM]
				  ,SOURCE.[ACL_DEFAULTS]
				  ,SOURCE.[REMOVE_MON_LIST]
				  ,SOURCE.[DISPLAY_MON_LIST]
				  ,SOURCE.[WARN_SECURE]
				  ,SOURCE.[ONLY_READONLY]
				  ,SOURCE.[FORCE_CHECKIN]
				  ,SOURCE.[MBL_EDITCOPY]
				  ,SOURCE.[MBL_OVERWRITE]
				  ,SOURCE.[MIN_DISKFREE]
				  ,SOURCE.[AUTOCLEAN]
				  ,SOURCE.[DEF_SHAD_RETENTION]
				  ,SOURCE.[SHADOW_DOCS]
				  ,SOURCE.[SHADOW_PROFILES]
				  ,SOURCE.[SHADOW_SEC_DOCS]
				  ,SOURCE.[DEFAULT_FT_INDEX]
				  ,SOURCE.[DISABLE_NATIVE]
				  ,SOURCE.[MAKE_READ_ONLY]
				  ,SOURCE.[REMOVE_READ_ONLY]
				  ,SOURCE.[MAKE_VER_READONLY]
				  ,SOURCE.[MAKE_VER_WRITABLE]
				  ,SOURCE.[PUBLISH_VERSION]
				  ,SOURCE.[UNPUBLISH_VERSION]
				  ,SOURCE.[DATE_FORMAT]
				  ,SOURCE.[TIME_FORMAT]
				  ,SOURCE.[ITEM_MAX]
				  ,SOURCE.[PAGE_MAX]
				  ,SOURCE.[DEFAULT_VIEWER]
				  ,SOURCE.[MANAGE_PRF]
				  ,SOURCE.[MANAGE_CYD]
				  ,SOURCE.[DPACKAGE]
				  ,SOURCE.[ALLOW_APPINT]
				  ,SOURCE.[ALLOW_USRSETTINGS]
				  ,SOURCE.[ALLOW_DOC_CREATE]
				  ,SOURCE.[CREATE_FOLDER]
				  ,SOURCE.[ROOT_FOLDER]
				  ,SOURCE.[CREATE_RELATION]
				  ,SOURCE.[SHOW_RELATED]
				  ,SOURCE.[REMOVE_RELATION]
				  ,SOURCE.[ALLOW_NOTIF]
				  ,SOURCE.[ALLOW_PREVIEW]
				  ,SOURCE.[WARN_UPDATE_AVAIL]
				  ,SOURCE.[ENABLE_WORKSPACE]
				  ,SOURCE.[LAST_UPDATE]);' AS NVARCHAR(MAX))
			
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella GROUPS'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella GROUPS'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	

		-- ************
		-- PEOPLEGROUPS
		-- ************
	
		SET @sql_string = CAST(N'MERGE PEOPLEGROUPS AS TARGET
		USING ( 	
			SELECT DISTINCT [GROUPS_SYSTEM_ID]
			  ,[PEOPLE_SYSTEM_ID]
			  ,[LAST_UPDATE]
			  ,[CHA_UTENTE_RIF]
			  ,[CHA_PREFERITO]
			  ,[DTA_FINE]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[PEOPLEGROUPS]
			WHERE [PEOPLE_SYSTEM_ID] IN (SELECT SYSTEM_ID FROM PEOPLE)
		) 
		AS SOURCE ([GROUPS_SYSTEM_ID]
			  ,[PEOPLE_SYSTEM_ID]
			  ,[LAST_UPDATE]
			  ,[CHA_UTENTE_RIF]
			  ,[CHA_PREFERITO]
			  ,[DTA_FINE])
		ON (TARGET.PEOPLE_SYSTEM_ID = SOURCE.PEOPLE_SYSTEM_ID AND TARGET.GROUPS_SYSTEM_ID = SOURCE.GROUPS_SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
			   [GROUPS_SYSTEM_ID] = SOURCE.[GROUPS_SYSTEM_ID]
			  ,[PEOPLE_SYSTEM_ID] = SOURCE.[PEOPLE_SYSTEM_ID]
			  ,[LAST_UPDATE] = SOURCE.[LAST_UPDATE]
			  ,[CHA_UTENTE_RIF] = SOURCE.[CHA_UTENTE_RIF]
			  ,[CHA_PREFERITO] = SOURCE.[CHA_PREFERITO]
			  ,[DTA_FINE] = SOURCE.[DTA_FINE]
		WHEN NOT MATCHED THEN
			INSERT ([GROUPS_SYSTEM_ID]
			  ,[PEOPLE_SYSTEM_ID]
			  ,[LAST_UPDATE]
			  ,[CHA_UTENTE_RIF]
			  ,[CHA_PREFERITO]
			  ,[DTA_FINE])
			VALUES (SOURCE.[GROUPS_SYSTEM_ID]
			  ,SOURCE.[PEOPLE_SYSTEM_ID]
			  ,SOURCE.[LAST_UPDATE]
			  ,SOURCE.[CHA_UTENTE_RIF]
			  ,SOURCE.[CHA_PREFERITO]
			  ,SOURCE.[DTA_FINE]);' AS NVARCHAR(MAX))
			  
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella PEOPLEGROUPS'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella PEOPLEGROUPS'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- *****************
		-- DPA_T_CANALE_CORR
		-- *****************
		
		SET @sql_string = CAST(N'MERGE DPA_T_CANALE_CORR AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_CORR_GLOBALE]
				  ,[ID_DOCUMENTTYPE]
				  ,[CHA_PREFERITO]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_T_CANALE_CORR]
			WHERE [ID_CORR_GLOBALE] IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI)
		) 
		AS SOURCE ([SYSTEM_ID]
				  ,[ID_CORR_GLOBALE]
				  ,[ID_DOCUMENTTYPE]
				  ,[CHA_PREFERITO])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
			   [SYSTEM_ID] = SOURCE.SYSTEM_ID
			  ,[ID_CORR_GLOBALE] = SOURCE.[ID_CORR_GLOBALE]
			  ,[ID_DOCUMENTTYPE] = SOURCE.[ID_DOCUMENTTYPE]
			  ,[CHA_PREFERITO] = SOURCE.[CHA_PREFERITO]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[ID_CORR_GLOBALE]
				  ,[ID_DOCUMENTTYPE]
				  ,[CHA_PREFERITO])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_CORR_GLOBALE]
				  ,SOURCE.[ID_DOCUMENTTYPE]
				  ,SOURCE.[CHA_PREFERITO]);' AS NVARCHAR(MAX))
			  
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_T_CANALE_CORR'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_T_CANALE_CORR'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



		-- *********
		-- Titolario
		-- *********
		
		SET @sql_string = CAST(N'MERGE PROJECT AS TARGET
			USING (SELECT SYSTEM_ID
					,DESCRIPTION
					,ICONIZED
					,PROFILE_ID
					,CHA_TIPO_PROJ
					,VAR_CODICE
					,ID_AMM
					,ID_REGISTRO
					,NUM_LIVELLO
					,CHA_TIPO_FASCICOLO
					,ID_FASCICOLO
					,ID_PARENT
					,VAR_NOTE
					,DTA_APERTURA
					,DTA_CHIUSURA
					,CHA_STATO
					,VAR_COD_ULTIMO
					,VAR_COD_LIV1
					,ET_TITOLARIO
					,ET_LIVELLO1
					,ET_LIVELLO2
					,ET_LIVELLO3
					,ET_LIVELLO4
					,ET_LIVELLO5
					,ET_LIVELLO6
					,ID_TIPO_PROC
					,ID_LEGISLATURA
					,ETDOC_RANDOM_ID
					,DTA_CREAZIONE
					,NUM_FASCICOLO
					,ANNO_CREAZIONE
					,CHA_RW
					,ID_UO_REF
					,ID_UO_LF
					,DTA_UO_LF
					,NUM_MESI_CONSERVAZIONE
					,VAR_CHIAVE_FASC
					,CARTACEO
					,CHA_PRIVATO
					,ID_TIPO_FASC
					,CHA_BLOCCA_FASC
					,ID_TITOLARIO
					,DTA_ATTIVAZIONE
					,DTA_CESSAZIONE
					,DTA_SCADENZA
					,CHA_IN_ARCHIVIO
					,AUTHOR
					,ID_RUOLO_CREATORE
					,ID_UO_CREATORE
					,NUM_PROT_TIT
					,CHA_CONTA_PROT_TIT
					,CHA_BLOCCA_FIGLI
					,MAX_LIV_TIT
					,ID_PEOPLE_DELEGATO
					,CHA_CONTROLLATO
					,CHA_CONSENTI_CLASS
					,ID_RUOLO_CHIUSURA
					,ID_UO_CHIUSURA
					,ID_AUTHOR_CHIUSURA
					,CHA_COD_T_A
					,COD_EXT_APP
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
				+ CAST(N'.PROJECT P WHERE P.ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST('
				AND P.SYSTEM_ID NOT IN
				(
					SELECT FP.SYSTEM_ID FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
					+ CAST(N'.PROJECT FP WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(' AND CHA_TIPO_FASCICOLO=''P''
					UNION
					SELECT RFP.SYSTEM_ID FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
					+ CAST(N'.PROJECT RFP WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(' AND ID_PARENT IN 
						( SELECT SYSTEM_ID FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) 
					+ CAST(N'.PROJECT P WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(' AND CHA_TIPO_FASCICOLO=''P'')
				)
				) 
			AS SOURCE (SYSTEM_ID
					,DESCRIPTION
					,ICONIZED
					,PROFILE_ID
					,CHA_TIPO_PROJ
					,VAR_CODICE
					,ID_AMM
					,ID_REGISTRO
					,NUM_LIVELLO
					,CHA_TIPO_FASCICOLO
					,ID_FASCICOLO
					,ID_PARENT
					,VAR_NOTE
					,DTA_APERTURA
					,DTA_CHIUSURA
					,CHA_STATO
					,VAR_COD_ULTIMO
					,VAR_COD_LIV1
					,ET_TITOLARIO
					,ET_LIVELLO1
					,ET_LIVELLO2
					,ET_LIVELLO3
					,ET_LIVELLO4
					,ET_LIVELLO5
					,ET_LIVELLO6
					,ID_TIPO_PROC
					,ID_LEGISLATURA
					,ETDOC_RANDOM_ID
					,DTA_CREAZIONE
					,NUM_FASCICOLO
					,ANNO_CREAZIONE
					,CHA_RW
					,ID_UO_REF
					,ID_UO_LF
					,DTA_UO_LF
					,NUM_MESI_CONSERVAZIONE
					,VAR_CHIAVE_FASC
					,CARTACEO
					,CHA_PRIVATO
					,ID_TIPO_FASC
					,CHA_BLOCCA_FASC
					,ID_TITOLARIO
					,DTA_ATTIVAZIONE
					,DTA_CESSAZIONE
					,DTA_SCADENZA
					,CHA_IN_ARCHIVIO
					,AUTHOR
					,ID_RUOLO_CREATORE
					,ID_UO_CREATORE
					,NUM_PROT_TIT
					,CHA_CONTA_PROT_TIT
					,CHA_BLOCCA_FIGLI
					,MAX_LIV_TIT
					,ID_PEOPLE_DELEGATO
					,CHA_CONTROLLATO
					,CHA_CONSENTI_CLASS
					,ID_RUOLO_CHIUSURA
					,ID_UO_CHIUSURA
					,ID_AUTHOR_CHIUSURA
					,CHA_COD_T_A
					,COD_EXT_APP
					)
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					SYSTEM_ID = SOURCE.SYSTEM_ID
					,DESCRIPTION = SOURCE.DESCRIPTION
					,ICONIZED = SOURCE.ICONIZED
					,PROFILE_ID = SOURCE.PROFILE_ID
					,CHA_TIPO_PROJ = SOURCE.CHA_TIPO_PROJ
					,VAR_CODICE = SOURCE.VAR_CODICE
					,ID_AMM = SOURCE.ID_AMM
					,ID_REGISTRO = SOURCE.ID_REGISTRO
					,NUM_LIVELLO = SOURCE.NUM_LIVELLO
					,CHA_TIPO_FASCICOLO = SOURCE.CHA_TIPO_FASCICOLO
					,ID_FASCICOLO = SOURCE.ID_FASCICOLO
					,ID_PARENT = SOURCE.ID_PARENT
					,VAR_NOTE = SOURCE.VAR_NOTE
					,DTA_APERTURA = SOURCE.DTA_APERTURA
					,DTA_CHIUSURA = SOURCE.DTA_CHIUSURA
					,CHA_STATO = SOURCE.CHA_STATO
					,VAR_COD_ULTIMO = SOURCE.VAR_COD_ULTIMO
					,VAR_COD_LIV1 = SOURCE.VAR_COD_LIV1
					,ET_TITOLARIO = SOURCE.ET_TITOLARIO
					,ET_LIVELLO1 = SOURCE.ET_LIVELLO1
					,ET_LIVELLO2 = SOURCE.ET_LIVELLO2
					,ET_LIVELLO3 = SOURCE.ET_LIVELLO3
					,ET_LIVELLO4 = SOURCE.ET_LIVELLO4
					,ET_LIVELLO5 = SOURCE.ET_LIVELLO5
					,ET_LIVELLO6 = SOURCE.ET_LIVELLO6
					,ID_TIPO_PROC = SOURCE.ID_TIPO_PROC
					,ID_LEGISLATURA = SOURCE.ID_LEGISLATURA
					,ETDOC_RANDOM_ID = SOURCE.ETDOC_RANDOM_ID
					,DTA_CREAZIONE = SOURCE.DTA_CREAZIONE
					,NUM_FASCICOLO = SOURCE.NUM_FASCICOLO
					,ANNO_CREAZIONE = SOURCE.ANNO_CREAZIONE
					,CHA_RW = SOURCE.CHA_RW
					,ID_UO_REF = SOURCE.ID_UO_REF
					,ID_UO_LF = SOURCE.ID_UO_LF
					,DTA_UO_LF = SOURCE.DTA_UO_LF
					,NUM_MESI_CONSERVAZIONE = SOURCE.NUM_MESI_CONSERVAZIONE
					,VAR_CHIAVE_FASC = SOURCE.VAR_CHIAVE_FASC
					,CARTACEO = SOURCE.CARTACEO
					,CHA_PRIVATO = SOURCE.CHA_PRIVATO
					,ID_TIPO_FASC = SOURCE.ID_TIPO_FASC
					,CHA_BLOCCA_FASC = SOURCE.CHA_BLOCCA_FASC
					,ID_TITOLARIO = SOURCE.ID_TITOLARIO
					,DTA_ATTIVAZIONE = SOURCE.DTA_ATTIVAZIONE
					,DTA_CESSAZIONE = SOURCE.DTA_CESSAZIONE
					,DTA_SCADENZA = SOURCE.DTA_SCADENZA
					,CHA_IN_ARCHIVIO = SOURCE.CHA_IN_ARCHIVIO
					,AUTHOR = SOURCE.AUTHOR
					,ID_RUOLO_CREATORE = SOURCE.ID_RUOLO_CREATORE
					,ID_UO_CREATORE = SOURCE.ID_UO_CREATORE
					,NUM_PROT_TIT = SOURCE.NUM_PROT_TIT
					,CHA_CONTA_PROT_TIT = SOURCE.CHA_CONTA_PROT_TIT
					,CHA_BLOCCA_FIGLI = SOURCE.CHA_BLOCCA_FIGLI
					,MAX_LIV_TIT = SOURCE.MAX_LIV_TIT
					,ID_PEOPLE_DELEGATO = SOURCE.ID_PEOPLE_DELEGATO
					,CHA_CONTROLLATO = SOURCE.CHA_CONTROLLATO
					,CHA_CONSENTI_CLASS = SOURCE.CHA_CONSENTI_CLASS
					,ID_RUOLO_CHIUSURA = SOURCE.ID_RUOLO_CHIUSURA
					,ID_UO_CHIUSURA = SOURCE.ID_UO_CHIUSURA
					,ID_AUTHOR_CHIUSURA = SOURCE.ID_AUTHOR_CHIUSURA
					,CHA_COD_T_A = SOURCE.CHA_COD_T_A
					,COD_EXT_APP = SOURCE.COD_EXT_APP
			WHEN NOT MATCHED THEN
				INSERT (SYSTEM_ID
						,DESCRIPTION
						,ICONIZED
						,PROFILE_ID
						,CHA_TIPO_PROJ
						,VAR_CODICE
						,ID_AMM
						,ID_REGISTRO
						,NUM_LIVELLO
						,CHA_TIPO_FASCICOLO
						,ID_FASCICOLO
						,ID_PARENT
						,VAR_NOTE
						,DTA_APERTURA
						,DTA_CHIUSURA
						,CHA_STATO
						,VAR_COD_ULTIMO
						,VAR_COD_LIV1
						,ET_TITOLARIO
						,ET_LIVELLO1
						,ET_LIVELLO2
						,ET_LIVELLO3
						,ET_LIVELLO4
						,ET_LIVELLO5
						,ET_LIVELLO6
						,ID_TIPO_PROC
						,ID_LEGISLATURA
						,ETDOC_RANDOM_ID
						,DTA_CREAZIONE
						,NUM_FASCICOLO
						,ANNO_CREAZIONE
						,CHA_RW
						,ID_UO_REF
						,ID_UO_LF
						,DTA_UO_LF
						,NUM_MESI_CONSERVAZIONE
						,VAR_CHIAVE_FASC
						,CARTACEO
						,CHA_PRIVATO
						,ID_TIPO_FASC
						,CHA_BLOCCA_FASC
						,ID_TITOLARIO
						,DTA_ATTIVAZIONE
						,DTA_CESSAZIONE
						,DTA_SCADENZA
						,CHA_IN_ARCHIVIO
						,AUTHOR
						,ID_RUOLO_CREATORE
						,ID_UO_CREATORE
						,NUM_PROT_TIT
						,CHA_CONTA_PROT_TIT
						,CHA_BLOCCA_FIGLI
						,MAX_LIV_TIT
						,ID_PEOPLE_DELEGATO
						,CHA_CONTROLLATO
						,CHA_CONSENTI_CLASS
						,ID_RUOLO_CHIUSURA
						,ID_UO_CHIUSURA
						,ID_AUTHOR_CHIUSURA
						,CHA_COD_T_A
						,COD_EXT_APP
						)
				VALUES (SOURCE.SYSTEM_ID
						,SOURCE.DESCRIPTION
						,SOURCE.ICONIZED
						,SOURCE.PROFILE_ID
						,SOURCE.CHA_TIPO_PROJ
						,SOURCE.VAR_CODICE
						,SOURCE.ID_AMM
						,SOURCE.ID_REGISTRO
						,SOURCE.NUM_LIVELLO
						,SOURCE.CHA_TIPO_FASCICOLO
						,SOURCE.ID_FASCICOLO
						,SOURCE.ID_PARENT
						,SOURCE.VAR_NOTE
						,SOURCE.DTA_APERTURA
						,SOURCE.DTA_CHIUSURA
						,SOURCE.CHA_STATO
						,SOURCE.VAR_COD_ULTIMO
						,SOURCE.VAR_COD_LIV1
						,SOURCE.ET_TITOLARIO
						,SOURCE.ET_LIVELLO1
						,SOURCE.ET_LIVELLO2
						,SOURCE.ET_LIVELLO3
						,SOURCE.ET_LIVELLO4
						,SOURCE.ET_LIVELLO5
						,SOURCE.ET_LIVELLO6
						,SOURCE.ID_TIPO_PROC
						,SOURCE.ID_LEGISLATURA
						,SOURCE.ETDOC_RANDOM_ID
						,SOURCE.DTA_CREAZIONE
						,SOURCE.NUM_FASCICOLO
						,SOURCE.ANNO_CREAZIONE
						,SOURCE.CHA_RW
						,SOURCE.ID_UO_REF
						,SOURCE.ID_UO_LF
						,SOURCE.DTA_UO_LF
						,SOURCE.NUM_MESI_CONSERVAZIONE
						,SOURCE.VAR_CHIAVE_FASC
						,SOURCE.CARTACEO
						,SOURCE.CHA_PRIVATO
						,SOURCE.ID_TIPO_FASC
						,SOURCE.CHA_BLOCCA_FASC
						,SOURCE.ID_TITOLARIO
						,SOURCE.DTA_ATTIVAZIONE
						,SOURCE.DTA_CESSAZIONE
						,SOURCE.DTA_SCADENZA
						,SOURCE.CHA_IN_ARCHIVIO
						,SOURCE.AUTHOR
						,SOURCE.ID_RUOLO_CREATORE
						,SOURCE.ID_UO_CREATORE
						,SOURCE.NUM_PROT_TIT
						,SOURCE.CHA_CONTA_PROT_TIT
						,SOURCE.CHA_BLOCCA_FIGLI
						,SOURCE.MAX_LIV_TIT
						,SOURCE.ID_PEOPLE_DELEGATO
						,SOURCE.CHA_CONTROLLATO
						,SOURCE.CHA_CONSENTI_CLASS
						,SOURCE.ID_RUOLO_CHIUSURA
						,SOURCE.ID_UO_CHIUSURA
						,SOURCE.ID_AUTHOR_CHIUSURA
						,SOURCE.CHA_COD_T_A
						,SOURCE.COD_EXT_APP
						);' AS NVARCHAR(MAX))

		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento del titolario'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento titolario'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	
	
		-- *****************
		-- DPA_RAGIONE_TRASM
		-- *****************
		
		SET @sql_string = CAST(N'MERGE DPA_RAGIONE_TRASM AS TARGET
			USING ( 	
				SELECT [SYSTEM_ID]
					,[VAR_DESC_RAGIONE]
					,[CHA_TIPO_RAGIONE]
					,[CHA_VIS]
					,[CHA_TIPO_DIRITTI]
					,[CHA_TIPO_DEST]
					,[CHA_RISPOSTA]
					,[VAR_NOTE]
					,[CHA_EREDITA]
					,[ID_AMM]
					,[CHA_TIPO_RISPOSTA]
					,[VAR_NOTIFICA_TRASM]
					,[VAR_TESTO_MSG_NOTIFICA_DOC]
					,[VAR_TESTO_MSG_NOTIFICA_FASC]
					,[CHA_CEDE_DIRITTI]
					,[CHA_RAG_SISTEMA]
					,[CHA_MANTIENI_LETT]
					,[CHA_MANTIENI_SCRITT]
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_RAGIONE_TRASM]
				WHERE [ID_AMM] = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST(N'
				)
			AS SOURCE ([SYSTEM_ID]
					,[VAR_DESC_RAGIONE]
					,[CHA_TIPO_RAGIONE]
					,[CHA_VIS]
					,[CHA_TIPO_DIRITTI]
					,[CHA_TIPO_DEST]
					,[CHA_RISPOSTA]
					,[VAR_NOTE]
					,[CHA_EREDITA]
					,[ID_AMM]
					,[CHA_TIPO_RISPOSTA]
					,[VAR_NOTIFICA_TRASM]
					,[VAR_TESTO_MSG_NOTIFICA_DOC]
					,[VAR_TESTO_MSG_NOTIFICA_FASC]
					,[CHA_CEDE_DIRITTI]
					,[CHA_RAG_SISTEMA]
					,[CHA_MANTIENI_LETT]
					,[CHA_MANTIENI_SCRITT])
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					 [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
					,[VAR_DESC_RAGIONE] = SOURCE.[VAR_DESC_RAGIONE]
					,[CHA_TIPO_RAGIONE] = SOURCE.[CHA_TIPO_RAGIONE]
					,[CHA_VIS] = SOURCE.[CHA_VIS]
					,[CHA_TIPO_DIRITTI] = SOURCE.[CHA_TIPO_DIRITTI]
					,[CHA_TIPO_DEST] = SOURCE.[CHA_TIPO_DEST]
					,[CHA_RISPOSTA] = SOURCE.[CHA_RISPOSTA]
					,[VAR_NOTE] = SOURCE.[VAR_NOTE]
					,[CHA_EREDITA] = SOURCE.[CHA_EREDITA]
					,[ID_AMM] = SOURCE.[ID_AMM]
					,[CHA_TIPO_RISPOSTA] = SOURCE.[CHA_TIPO_RISPOSTA]
					,[VAR_NOTIFICA_TRASM] = SOURCE.[VAR_NOTIFICA_TRASM]
					,[VAR_TESTO_MSG_NOTIFICA_DOC] = SOURCE.[VAR_TESTO_MSG_NOTIFICA_DOC]
					,[VAR_TESTO_MSG_NOTIFICA_FASC] = SOURCE.[VAR_TESTO_MSG_NOTIFICA_FASC]
					,[CHA_CEDE_DIRITTI] = SOURCE.[CHA_CEDE_DIRITTI]
					,[CHA_RAG_SISTEMA] = SOURCE.[CHA_RAG_SISTEMA]
					,[CHA_MANTIENI_LETT] = SOURCE.[CHA_MANTIENI_LETT]
					,[CHA_MANTIENI_SCRITT] = SOURCE.[CHA_MANTIENI_SCRITT]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
					,[VAR_DESC_RAGIONE]
					,[CHA_TIPO_RAGIONE]
					,[CHA_VIS]
					,[CHA_TIPO_DIRITTI]
					,[CHA_TIPO_DEST]
					,[CHA_RISPOSTA]
					,[VAR_NOTE]
					,[CHA_EREDITA]
					,[ID_AMM]
					,[CHA_TIPO_RISPOSTA]
					,[VAR_NOTIFICA_TRASM]
					,[VAR_TESTO_MSG_NOTIFICA_DOC]
					,[VAR_TESTO_MSG_NOTIFICA_FASC]
					,[CHA_CEDE_DIRITTI]
					,[CHA_RAG_SISTEMA]
					,[CHA_MANTIENI_LETT]
					,[CHA_MANTIENI_SCRITT])
				VALUES (SOURCE.[SYSTEM_ID]
					,SOURCE.[VAR_DESC_RAGIONE]
					,SOURCE.[CHA_TIPO_RAGIONE]
					,SOURCE.[CHA_VIS]
					,SOURCE.[CHA_TIPO_DIRITTI]
					,SOURCE.[CHA_TIPO_DEST]
					,SOURCE.[CHA_RISPOSTA]
					,SOURCE.[VAR_NOTE]
					,SOURCE.[CHA_EREDITA]
					,SOURCE.[ID_AMM]
					,SOURCE.[CHA_TIPO_RISPOSTA]
					,SOURCE.[VAR_NOTIFICA_TRASM]
					,SOURCE.[VAR_TESTO_MSG_NOTIFICA_DOC]
					,SOURCE.[VAR_TESTO_MSG_NOTIFICA_FASC]
					,SOURCE.[CHA_CEDE_DIRITTI]
					,SOURCE.[CHA_RAG_SISTEMA]
					,SOURCE.[CHA_MANTIENI_LETT]
					,SOURCE.[CHA_MANTIENI_SCRITT]);' AS NVARCHAR(MAX))
					
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_RAGIONE_TRASM'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_RAGIONE_TRASM'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				
	
	
	
		-- **************
		-- DPA_TIPO_RUOLO
		-- **************
		--DECLARE @sql_string_tr nvarchar(MAX)
		
		SET @sql_string = CAST(N'MERGE DPA_TIPO_RUOLO AS TARGET
			USING ( 	
				SELECT 
					[SYSTEM_ID]
					,[ID_AMM]
					,[ID_PARENT]
					,[VAR_CODICE]
					,[NUM_LIVELLO]
					,[VAR_DESC_RUOLO]
				FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_TIPO_RUOLO]
				WHERE ID_AMM = ' AS NVARCHAR(MAX)) + CAST(@idAmministrazione AS NVARCHAR(MAX)) + CAST('
				)
			AS SOURCE (
					[SYSTEM_ID]
					,[ID_AMM]
					,[ID_PARENT]
					,[VAR_CODICE]
					,[NUM_LIVELLO]
					,[VAR_DESC_RUOLO]
					)
			ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
			WHEN MATCHED THEN
				UPDATE SET
					 [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
					,[ID_AMM] = SOURCE.[ID_AMM]
					,[ID_PARENT] = SOURCE.[ID_PARENT]
					,[VAR_CODICE] = SOURCE.[VAR_CODICE]
					,[NUM_LIVELLO] = SOURCE.[NUM_LIVELLO]
					,[VAR_DESC_RUOLO] = SOURCE.[VAR_DESC_RUOLO]
			WHEN NOT MATCHED THEN
				INSERT ([SYSTEM_ID]
					,[ID_AMM]
					,[ID_PARENT]
					,[VAR_CODICE]
					,[NUM_LIVELLO]
					,[VAR_DESC_RUOLO])
				VALUES (SOURCE.[SYSTEM_ID]
					,SOURCE.[ID_AMM]
					,SOURCE.[ID_PARENT]
					,SOURCE.[VAR_CODICE]
					,SOURCE.[NUM_LIVELLO]
					,SOURCE.[VAR_DESC_RUOLO]);' AS NVARCHAR(MAX))
					
		PRINT @sql_string;

		EXECUTE sp_executesql @sql_string;

		IF @@ERROR <> 0
		BEGIN
			-- Rollback the transaction
			ROLLBACK
			
			set @logType = 'ERROR'
			set @log = 'Errore durante l''aggiornamento della tabella DPA_TIPO_RUOLO'
			
			EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

			-- Raise an error and return
			RAISERROR (@log, 16, 1)
			RETURN
		END
		
		set @logType = 'INFO'
		set @log = 'Aggiornamento tabella DPA_TIPO_RUOLO'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				
	
	
	
	
	
		FETCH statement_cursor INTO @idAmministrazione     
	END
	  
	CLOSE statement_cursor
	DEALLOCATE statement_cursor
	

	
	-- ************************************************************
	-- Aggiornamento tabelle dipendenti dagli oggetti già importati
	-- ************************************************************
	
	-- *****************
	-- DPA_VIS_TIPO_FASC
	-- *****************
	
	SET @sql_string = CAST(N'MERGE DPA_VIS_TIPO_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_TIPO_FASC]
				  ,[ID_RUOLO]
				  ,[DIRITTI]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_VIS_TIPO_FASC]
			  WHERE [ID_TIPO_FASC] IN 
				  (
					SELECT SYSTEM_ID FROM DPA_TIPO_FASC
				  )
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[ID_TIPO_FASC]
				  ,[ID_RUOLO]
				  ,[DIRITTI])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[ID_TIPO_FASC] = SOURCE.[ID_TIPO_FASC]
				  ,[ID_RUOLO] = SOURCE.[ID_RUOLO]
				  ,[DIRITTI] = SOURCE.[DIRITTI]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[ID_TIPO_FASC]
				  ,[ID_RUOLO]
				  ,[DIRITTI])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_TIPO_FASC]
				  ,SOURCE.[ID_RUOLO]
				  ,SOURCE.[DIRITTI]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_VIS_TIPO_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_VIS_TIPO_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	


	-- ************************
	-- DPA_OGG_CUSTOM_COMP_FASC
	-- ************************
	
	SET @sql_string = CAST(N'MERGE DPA_OGG_CUSTOM_COMP_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_TEMPLATE]
				  ,[ID_OGG_CUSTOM]
				  ,[POSIZIONE]
				  ,[ENABLEDHISTORY]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_OGG_CUSTOM_COMP_FASC]
			  WHERE [ID_TEMPLATE] IN
				(
				SELECT SYSTEM_ID FROM DPA_TIPO_FASC
				)
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[ID_TEMPLATE]
				  ,[ID_OGG_CUSTOM]
				  ,[POSIZIONE]
				  ,[ENABLEDHISTORY])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
				  ,[ID_OGG_CUSTOM] = SOURCE.[ID_OGG_CUSTOM]
				  ,[POSIZIONE] = SOURCE.[POSIZIONE]
				  ,[ENABLEDHISTORY] = SOURCE.[ENABLEDHISTORY]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[ID_TEMPLATE]
				  ,[ID_OGG_CUSTOM]
				  ,[POSIZIONE]
				  ,[ENABLEDHISTORY])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_TEMPLATE]
				  ,SOURCE.[ID_OGG_CUSTOM]
				  ,SOURCE.[POSIZIONE]
				  ,SOURCE.[ENABLEDHISTORY]);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_OGG_CUSTOM_COMP_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_OGG_CUSTOM_COMP_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				  
	
	
	
	-- ***********************
	-- DPA_A_R_OGG_CUSTOM_FASC
	-- ***********************
	
	SET @sql_string = CAST(N'MERGE DPA_A_R_OGG_CUSTOM_FASC AS TARGET
	USING ( 	
		SELECT [SYSTEM_ID]
			  ,[ID_TEMPLATE]
			  ,[ID_OGGETTO_CUSTOM]
			  ,[ID_RUOLO]
			  ,[INS_MOD]
			  ,[VIS]
		  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_A_R_OGG_CUSTOM_FASC]
		  WHERE [ID_TEMPLATE] IN
			(
			SELECT SYSTEM_ID FROM DPA_TIPO_FASC
			)
		)
	AS SOURCE ([SYSTEM_ID]
			  ,[ID_TEMPLATE]
			  ,[ID_OGGETTO_CUSTOM]
			  ,[ID_RUOLO]
			  ,[INS_MOD]
			  ,[VIS])
	ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
	WHEN MATCHED THEN
		UPDATE SET
			   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
			  ,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
			  ,[ID_OGGETTO_CUSTOM] = SOURCE.[ID_OGGETTO_CUSTOM]
			  ,[ID_RUOLO] = SOURCE.[ID_RUOLO]
			  ,[INS_MOD] = SOURCE.[INS_MOD]
			  ,[VIS] = SOURCE.[VIS]
	WHEN NOT MATCHED THEN
		INSERT ([SYSTEM_ID]
			  ,[ID_TEMPLATE]
			  ,[ID_OGGETTO_CUSTOM]
			  ,[ID_RUOLO]
			  ,[INS_MOD]
			  ,[VIS])
		VALUES (SOURCE.[SYSTEM_ID]
			  ,SOURCE.[ID_TEMPLATE]
			  ,SOURCE.[ID_OGGETTO_CUSTOM]
			  ,SOURCE.[ID_RUOLO]
			  ,SOURCE.[INS_MOD]
			  ,SOURCE.[VIS]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	--EXECUTE sp_executesql @sql_string; Disabilitata perchè in conflitto con l'IDENTITY

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_A_R_OGG_CUSTOM_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_A_R_OGG_CUSTOM_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	-- TEST-------------------------------
	declare @numeroRec int
	declare @print varchar(1000)
	
	select @numeroRec = COUNT(*) from DPA_TIPO_FASC
	
	SET @print = CAST(N'Numero record per DPA_TIPO_FASC: ' AS NVARCHAR(MAX)) + CAST(@numeroRec AS NVARCHAR(MAX))
	print @print
	
	--------------------------------------
	
	
	
	-- ***********************
	-- DPA_ASS_TEMPLATES_FASC
	-- ***********************
	
	SET @sql_string = CAST(N'MERGE DPA_ASS_TEMPLATES_FASC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_OGGETTO]
				  ,[ID_TEMPLATE]
				  ,[ID_PROJECT]
				  ,[VALORE_OGGETTO_DB]
				  ,[ANNO]
				  ,[ID_AOO_RF]
				  ,[CODICE_DB]
				  ,[MANUAL_INSERT]
				  ,[VALORE_SC]
				  ,[DTA_INS]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ASS_TEMPLATES_FASC]
			  WHERE ID_PROJECT IS NULL OR ID_PROJECT = ''''
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[ID_OGGETTO]
				  ,[ID_TEMPLATE]
				  ,[ID_PROJECT]
				  ,[VALORE_OGGETTO_DB]
				  ,[ANNO]
				  ,[ID_AOO_RF]
				  ,[CODICE_DB]
				  ,[MANUAL_INSERT]
				  ,[VALORE_SC]
				  ,[DTA_INS])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[ID_OGGETTO] = SOURCE.[ID_OGGETTO]
				  ,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
				  ,[ID_PROJECT] = SOURCE.[ID_PROJECT]
				  ,[VALORE_OGGETTO_DB] = SOURCE.[VALORE_OGGETTO_DB]
				  ,[ANNO] = SOURCE.[ANNO]
				  ,[ID_AOO_RF] = SOURCE.[ID_AOO_RF]
				  ,[CODICE_DB] = SOURCE.[CODICE_DB]
				  ,[MANUAL_INSERT] = SOURCE.[MANUAL_INSERT]
				  ,[VALORE_SC] = SOURCE.[VALORE_SC]
				  ,[DTA_INS] = SOURCE.[DTA_INS]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[ID_OGGETTO]
				  ,[ID_TEMPLATE]
				  ,[ID_PROJECT]
				  ,[VALORE_OGGETTO_DB]
				  ,[ANNO]
				  ,[ID_AOO_RF]
				  ,[CODICE_DB]
				  ,[MANUAL_INSERT]
				  ,[VALORE_SC]
				  ,[DTA_INS])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_OGGETTO]
				  ,SOURCE.[ID_TEMPLATE]
				  ,SOURCE.[ID_PROJECT]
				  ,SOURCE.[VALORE_OGGETTO_DB]
				  ,SOURCE.[ANNO]
				  ,SOURCE.[ID_AOO_RF]
				  ,SOURCE.[CODICE_DB]
				  ,SOURCE.[MANUAL_INSERT]
				  ,SOURCE.[VALORE_SC]
				  ,SOURCE.[DTA_INS]);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_ASS_TEMPLATES_FASC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_ASS_TEMPLATES_FASC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				  	


	
	-- ****************
	-- DPA_VIS_TIPO_DOC
	-- ****************
	
	SET @sql_string = CAST(N'MERGE DPA_VIS_TIPO_DOC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				  ,[ID_TIPO_DOC]
				  ,[ID_RUOLO]
				  ,[DIRITTI]
			  FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_VIS_TIPO_DOC]
			  WHERE [ID_TIPO_DOC] IN 
				  (
					SELECT SYSTEM_ID FROM DPA_TIPO_ATTO
				  )
			)
		AS SOURCE ([SYSTEM_ID]
				  ,[ID_TIPO_DOC]
				  ,[ID_RUOLO]
				  ,[DIRITTI])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				  ,[ID_TIPO_DOC] = SOURCE.[ID_TIPO_DOC]
				  ,[ID_RUOLO] = SOURCE.[ID_RUOLO]
				  ,[DIRITTI] = SOURCE.[DIRITTI]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				  ,[ID_TIPO_DOC]
				  ,[ID_RUOLO]
				  ,[DIRITTI])
			VALUES (SOURCE.[SYSTEM_ID]
				  ,SOURCE.[ID_TIPO_DOC]
				  ,SOURCE.[ID_RUOLO]
				  ,SOURCE.[DIRITTI]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_VIS_TIPO_DOC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_VIS_TIPO_DOC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- **********************
	-- DPA_A_R_OGG_CUSTOM_DOC
	-- **********************
	
	SET @sql_string = CAST(N'MERGE DPA_A_R_OGG_CUSTOM_DOC AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGGETTO_CUSTOM]
				,[ID_RUOLO]
				,[INS_MOD]
				,[VIS]
				,[DEL_REP]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_A_R_OGG_CUSTOM_DOC]
			WHERE ID_TEMPLATE IN
				(
				SELECT SYSTEM_ID FROM DPA_TIPO_ATTO
				)
			)
		AS SOURCE ([SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGGETTO_CUSTOM]
				,[ID_RUOLO]
				,[INS_MOD]
				,[VIS]
				,[DEL_REP])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
				,[ID_OGGETTO_CUSTOM] = SOURCE.[ID_OGGETTO_CUSTOM]
				,[ID_RUOLO] = SOURCE.[ID_RUOLO]
				,[INS_MOD] = SOURCE.[INS_MOD]
				,[VIS] = SOURCE.[VIS]
				,[DEL_REP] = SOURCE.[DEL_REP]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGGETTO_CUSTOM]
				,[ID_RUOLO]
				,[INS_MOD]
				,[VIS]
				,[DEL_REP])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[ID_TEMPLATE]
				,SOURCE.[ID_OGGETTO_CUSTOM]
				,SOURCE.[ID_RUOLO]
				,SOURCE.[INS_MOD]
				,SOURCE.[VIS]
				,SOURCE.[DEL_REP]);' AS NVARCHAR(MAX))

	PRINT @sql_string;

	--EXECUTE sp_executesql @sql_string; Disabilitata perchè in conflitto con l'IDENTITY

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_A_R_OGG_CUSTOM_DOC'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_A_R_OGG_CUSTOM_DOC'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject				  	
	
	
	
	-- *******************
	-- DPA_OGG_CUSTOM_COMP
	-- *******************
	
	SET @sql_string = CAST(N'MERGE DPA_OGG_CUSTOM_COMP AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGG_CUSTOM]
				,[POSIZIONE]
				,[ENABLEDHISTORY]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_OGG_CUSTOM_COMP]
			WHERE ID_TEMPLATE IN
				(
				SELECT SYSTEM_ID FROM DPA_TIPO_ATTO
				)
			)
		AS SOURCE ([SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGG_CUSTOM]
				,[POSIZIONE]
				,[ENABLEDHISTORY])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				 [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
				,[ID_OGG_CUSTOM] = SOURCE.[ID_OGG_CUSTOM]
				,[POSIZIONE] = SOURCE.[POSIZIONE]
				,[ENABLEDHISTORY] = SOURCE.[ENABLEDHISTORY]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[ID_TEMPLATE]
				,[ID_OGG_CUSTOM]
				,[POSIZIONE]
				,[ENABLEDHISTORY])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[ID_TEMPLATE]
				,SOURCE.[ID_OGG_CUSTOM]
				,SOURCE.[POSIZIONE]
				,SOURCE.[ENABLEDHISTORY]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del titolario'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento titolario'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	
	
	
	
	-- *******************
	-- DPA_OGG_CUSTOM_COMP
	-- *******************

	SET @sql_string = CAST(N'MERGE DPA_ASSOCIAZIONE_TEMPLATES AS TARGET
		USING ( 	
			SELECT [SYSTEM_ID]
				,[ID_OGGETTO]
				,[ID_TEMPLATE]
				,[DOC_NUMBER]
				,[VALORE_OGGETTO_DB]
				,[ANNO]
				,[ID_AOO_RF]
				,[CODICE_DB]
				,[MANUAL_INSERT]
				,[VALORE_SC]
				,[DTA_INS]
				,[DTA_ANNULLAMENTO]
				,[DOC_NUMBER_RICERCA]
				,[VALORE_OGGETTO_DB_RICERCA]
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[DPA_ASSOCIAZIONE_TEMPLATES]
			WHERE (DOC_NUMBER = '''' OR DOC_NUMBER IS NULL)
			AND ID_TEMPLATE IN
				(
				SELECT SYSTEM_ID FROM DPA_TIPO_ATTO
				)
			)
		AS SOURCE ([SYSTEM_ID]
				,[ID_OGGETTO]
				,[ID_TEMPLATE]
				,[DOC_NUMBER]
				,[VALORE_OGGETTO_DB]
				,[ANNO]
				,[ID_AOO_RF]
				,[CODICE_DB]
				,[MANUAL_INSERT]
				,[VALORE_SC]
				,[DTA_INS]
				,[DTA_ANNULLAMENTO]
				,[DOC_NUMBER_RICERCA]
				,[VALORE_OGGETTO_DB_RICERCA])
		ON (TARGET.SYSTEM_ID = SOURCE.SYSTEM_ID)
		WHEN MATCHED THEN
			UPDATE SET
				   [SYSTEM_ID] = SOURCE.[SYSTEM_ID]
				,[ID_OGGETTO] = SOURCE.[ID_OGGETTO]
				,[ID_TEMPLATE] = SOURCE.[ID_TEMPLATE]
				,[DOC_NUMBER] = SOURCE.[DOC_NUMBER]
				,[VALORE_OGGETTO_DB] = SOURCE.[VALORE_OGGETTO_DB]
				,[ANNO] = SOURCE.[ANNO]
				,[ID_AOO_RF] = SOURCE.[ID_AOO_RF]
				,[CODICE_DB] = SOURCE.[CODICE_DB]
				,[MANUAL_INSERT] = SOURCE.[MANUAL_INSERT]
				,[VALORE_SC] = SOURCE.[VALORE_SC]
				,[DTA_INS] = SOURCE.[DTA_INS]
				,[DTA_ANNULLAMENTO] = SOURCE.[DTA_ANNULLAMENTO]
				,[DOC_NUMBER_RICERCA] = SOURCE.[DOC_NUMBER_RICERCA]
				,[VALORE_OGGETTO_DB_RICERCA] = SOURCE.[VALORE_OGGETTO_DB_RICERCA]
		WHEN NOT MATCHED THEN
			INSERT ([SYSTEM_ID]
				,[ID_OGGETTO]
				,[ID_TEMPLATE]
				,[DOC_NUMBER]
				,[VALORE_OGGETTO_DB]
				,[ANNO]
				,[ID_AOO_RF]
				,[CODICE_DB]
				,[MANUAL_INSERT]
				,[VALORE_SC]
				,[DTA_INS]
				,[DTA_ANNULLAMENTO]
				,[DOC_NUMBER_RICERCA]
				,[VALORE_OGGETTO_DB_RICERCA])
			VALUES (SOURCE.[SYSTEM_ID]
				,SOURCE.[ID_OGGETTO]
				,SOURCE.[ID_TEMPLATE]
				,SOURCE.[DOC_NUMBER]
				,SOURCE.[VALORE_OGGETTO_DB]
				,SOURCE.[ANNO]
				,SOURCE.[ID_AOO_RF]
				,SOURCE.[CODICE_DB]
				,SOURCE.[MANUAL_INSERT]
				,SOURCE.[VALORE_SC]
				,SOURCE.[DTA_INS]
				,SOURCE.[DTA_ANNULLAMENTO]
				,SOURCE.[DOC_NUMBER_RICERCA]
				,SOURCE.[VALORE_OGGETTO_DB_RICERCA]);' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento della tabella DPA_OGG_CUSTOM_COMP'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tabella DPA_OGG_CUSTOM_COMP'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject	

	
	
	-- *******************************************
	-- Aggiornamento ruolo archivista/consultatore
	-- *******************************************
	
	DECLARE @id_ruolo_arch INT = 0
	DECLARE @id_ruolo_cons INT = 0
	
	SELECT @id_ruolo_arch = VALUE FROM ARCHIVE_Configuration WHERE [KEY] = 'ID_CORR_GLOBALI_RUOLO_ARCHIVISTA'
	PRINT 'ID_CORR_GLOBALI_RUOLO_ARCHIVISTA: ' + CAST(@id_ruolo_arch AS VARCHAR(10))
	
	SELECT @id_ruolo_cons = VALUE FROM ARCHIVE_Configuration WHERE [KEY] = 'ID_CORR_GLOBALI_RUOLO_CONSULTATORE'
	PRINT 'ID_CORR_GLOBALI_RUOLO_CONSULTATORE: ' + CAST(@id_ruolo_cons AS VARCHAR(10))
	
	-- Archivista
	--
	IF (ISNULL(@id_ruolo_arch, 0) <> 0)
		BEGIN
		
			/*associare i registri al ruolo*/
			DELETE FROM dpa_l_ruolo_reg WHERE ID_RUOLO_IN_UO = @id_ruolo_arch
			
			insert into dpa_l_ruolo_reg (ID_REGISTRO, ID_RUOLO_IN_UO, DTA_INIZIO)
			select  system_id , @id_ruolo_arch, getDate() from dpa_el_registri


			/*associare le tipologie documento e i campi al ruolo */  
			DELETE FROM dpa_a_r_ogg_custom_doc WHERE ID_RUOLO = @id_ruolo_arch
		  
			insert into dpa_a_r_ogg_custom_doc ( id_template, id_oggetto_custom, id_ruolo, ins_mod, vis, del_rep)
			select id_template, id_oggetto, @id_ruolo_arch,0,1,null from 
			dpa_associazione_templates a, DPA_TIPO_ATTO b
			where a.ID_TEMPLATE = b.SYSTEM_ID and (a.doc_number is null or a.doc_number = '')


			/*associare le tipologie fascicoli e i campi al ruolo */
			DELETE FROM dpa_a_r_ogg_custom_fasc WHERE ID_RUOLO = @id_ruolo_arch
			
			insert into dpa_a_r_ogg_custom_fasc ( id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
			select id_template, id_oggetto, @id_ruolo_arch,0,1 from 
			DPA_ASS_TEMPLATES_FASC a, DPA_TIPO_ATTO b
			where a.ID_TEMPLATE = b.SYSTEM_ID and (a.Id_Project is null or a.Id_Project = '')
			
		END
		
	-- Consultatore
	--
	IF (ISNULL(@id_ruolo_cons, 0) <> 0)
		BEGIN
		
			/*associare i registri al ruolo*/
			DELETE FROM dpa_l_ruolo_reg WHERE ID_RUOLO_IN_UO = @id_ruolo_cons
			
			insert into dpa_l_ruolo_reg (ID_REGISTRO, ID_RUOLO_IN_UO, DTA_INIZIO)
			select  system_id , @id_ruolo_cons, getDate() from dpa_el_registri


			/*associare le tipologie documento e i campi al ruolo */  
			DELETE FROM dpa_a_r_ogg_custom_doc WHERE ID_RUOLO = @id_ruolo_cons
			
			insert into dpa_a_r_ogg_custom_doc ( id_template, id_oggetto_custom, id_ruolo, ins_mod, vis, del_rep)
			select id_template, id_oggetto, @id_ruolo_cons,0,1,null from 
			dpa_associazione_templates a, DPA_TIPO_ATTO b
			where a.ID_TEMPLATE = b.SYSTEM_ID and (a.doc_number is null or a.doc_number = '')


			/*associare le tipologie fascicoli e i campi al ruolo */  
			DELETE FROM dpa_a_r_ogg_custom_fasc WHERE ID_RUOLO = @id_ruolo_cons
			
			insert into dpa_a_r_ogg_custom_fasc ( id_template, id_oggetto_custom, id_ruolo, ins_mod, vis)
			select id_template, id_oggetto, @id_ruolo_cons,0,1 from 
			DPA_ASS_TEMPLATES_FASC a, DPA_TIPO_ATTO b
			where a.ID_TEMPLATE = b.SYSTEM_ID and (a.Id_Project is null or a.Id_Project = '')
		
		END
		
	
	
	-- ******************
	-- COMMIT TRANSACTION
	-- ******************
	
	IF @@ERROR <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento dei dati di base'
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento dati di base concluso regolarmente'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject



	COMMIT TRANSACTION T1
	
END
GO
