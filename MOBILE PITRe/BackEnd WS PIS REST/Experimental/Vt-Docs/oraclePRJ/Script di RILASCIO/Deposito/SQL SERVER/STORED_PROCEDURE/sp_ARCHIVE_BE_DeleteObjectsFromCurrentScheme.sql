USE PCM_DEPOSITO_1
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Giovanni Olivari
-- Create date: 27/05/2013
-- Description:	Elimina i record dal DB Corrente
-- =============================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_DeleteObjectsFromCurrentScheme
	@TransferID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType_Transfer int = 1 -- 'Transfer'
	DECLARE @logObjectID int = @TransferID
	DECLARE @errorCode int

	DECLARE @sql_string nvarchar(MAX)
	DECLARE @sql_string_filtro_temp_doc nvarchar(MAX)
	DECLARE @sql_string_filtro_trasferiti_doc nvarchar(MAX)
	DECLARE @sql_string_filtro_temp_fasc nvarchar(MAX)
	DECLARE @sql_string_filtro_trasferiti_fasc nvarchar(MAX)
	DECLARE @nomeSchemaCorrente varchar(200) 
	DECLARE @nomeUtenteCorrente varchar(200) 
	DECLARE @nomeUtenteDeposito varchar(200) 

	DECLARE @tipoStepConsolidamento_CONSOLIDATO_CONTENUTO_E_METADATI int = 2 -- Consolidato il documento nei suoi metadati fondamentali
	DECLARE @idAutoreConsolidamento int
	DECLARE @idRuoloConsolidamento int
	
	
	
	-- Lettura parametri di configurazione
	--
	SELECT @nomeSchemaCorrente=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_SCHEMA_CORRENTE'
	
	SELECT @nomeUtenteCorrente=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_UTENTE_CORRENTE'
	
	SELECT @nomeUtenteDeposito=[VALUE] FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'NOME_UTENTE_DEPOSITO'
	
	SELECT @idAutoreConsolidamento=CAST([VALUE] AS INT) FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'ID_AUTORE_CONSOLIDAMENTO'
	
	SELECT @idRuoloConsolidamento=CAST([VALUE] AS INT) FROM ARCHIVE_CONFIGURATION
	WHERE [KEY] = 'ID_RUOLO_CONSOLIDAMENTO'
	
	print 'Nome schema corrente: ' + @nomeSchemaCorrente
	print 'Nome utente corrente: ' + @nomeUtenteCorrente
	print 'Nome utente deposito: ' + @nomeUtenteDeposito
	print 'ID autore consolidamento: ' + CAST(@idAutoreConsolidamento AS VARCHAR(10))
	print 'ID ruolo consolidamento: ' + CAST(@idRuoloConsolidamento AS VARCHAR(10))
	
	SET @nomeSchemaCorrente = @nomeSchemaCorrente + '.' + @nomeUtenteCorrente
	print 'Schema/Utente corrente: ' + @nomeSchemaCorrente
	
	
	
	BEGIN TRANSACTION T1


	
	set @logType = 'INFO'
	set @log = 'Init procedura di eliminazione dei dati dal Corrente per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID



	-- Consolidamento dei documenti sul corrente:
	--		- portati in COPIA sul Deposito
	--		- per cui � stato esplicitato di mantenere una copia (MantieniCopia=1)
	--
	set @sql_string = CAST(N'
		UPDATE ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			SET CONSOLIDATION_STATE = ' AS NVARCHAR(MAX)) + CAST(@tipoStepConsolidamento_CONSOLIDATO_CONTENUTO_E_METADATI AS NVARCHAR(MAX)) + CAST(N'
			, CONSOLIDATION_AUTHOR = ' AS NVARCHAR(MAX)) + CAST(@idAutoreConsolidamento AS NVARCHAR(MAX)) + CAST(N'
			, CONSOLIDATION_ROLE = ' AS NVARCHAR(MAX)) + CAST(@idRuoloConsolidamento AS NVARCHAR(MAX)) + CAST(N'
			, CONSOLIDATION_DATE = GETDATE()
		WHERE ISNULL(CONSOLIDATION_STATE, 0) <> 2
		AND SYSTEM_ID IN
			(
			SELECT DISTINCT PROFILE_ID
			FROM ARCHIVE_TEMPPROFILE TP INNER JOIN ARCHIVE_TRANSFERPOLICY P ON TP.TRANSFERPOLICY_ID = P.SYSTEM_ID
			WHERE P.TRANSFER_ID = ' AS NVARCHAR(MAX)) + CAST(@TransferID AS NVARCHAR(MAX)) + CAST(N'
			AND P.ENABLED = 1
			AND 
				(
				TP.TIPOTRASFERIMENTO_VERSAMENTO = ''COPIA''
				OR
				TP.MANTIENICOPIA = 1
				)
			)' AS NVARCHAR(MAX)) + CAST(N'
		AND SYSTEM_ID IN
			(
			SELECT SYSTEM_ID FROM PROFILE
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante il consolidamento dei documenti in COPIA per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Consolidamento per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID



	-- Cancellazione dei documenti/fascicoli, e record correlati, TRASFERITI sul Deposito
	--
	-- ******
	-- Filtri
	-- ******

	-- Filtro documenti sulle tabelle temp del Deposito:
	--		- appartenenti a tutte le policy abilitate del trasferimento indicato
	--		- il tipo di trasferimento per l'intero versamento deve essere TRASFERIMENTO e non COPIA
	--		- il flag MantieniCopia deve essere 0
	--
	set @sql_string_filtro_temp_doc = CAST(N'
		SELECT DISTINCT PROFILE_ID
		FROM ARCHIVE_TEMPPROFILE TP INNER JOIN ARCHIVE_TRANSFERPOLICY P ON TP.TRANSFERPOLICY_ID = P.SYSTEM_ID
		WHERE P.TRANSFER_ID = ' AS NVARCHAR(MAX)) + CAST(@TransferID AS NVARCHAR(MAX)) + CAST(N'
		AND P.ENABLED = 1
		AND TP.TIPOTRASFERIMENTO_VERSAMENTO = ''TRASFERIMENTO''
		AND TP.MANTIENICOPIA = 0' AS NVARCHAR(MAX))
	
	-- Filtro documenti effettivamente trasferiti sul Deposito:
	--		- per poterli eliminare dal Corrente devono essere stati trasferiti, quindi presenti nella tabella PROFILE del Deposito
	--
	set @sql_string_filtro_trasferiti_doc = CAST(N'
		SELECT SYSTEM_ID FROM PROFILE' AS NVARCHAR(MAX))



	-- Filtro fascicoli sulle tabelle temp del Deposito:
	--		- appartenenti a tutte le policy abilitate del trasferimento indicato
	--		- il tipo di trasferimento per l'intero versamento deve essere TRASFERIMENTO e non COPIA
	--		- tenere conto della gerarchia dei fascicoli
	--
	
	-- Inserire tutti i record dei fascicoli nella tabella temporanea #deletedProject
	--
	IF OBJECT_ID('tempdb..#deletedProject') IS NOT NULL DROP TABLE #deletedProject
	CREATE TABLE #deletedProject
	(
		ID int
	)
	
	SET @sql_string = CAST(N'
		WITH GERARCHIA_FASCICOLO AS
		(
		SELECT P.SYSTEM_ID, P.ID_PARENT, P.ID_FASCICOLO, P.VAR_CODICE, P.CHA_TIPO_FASCICOLO, P.CHA_TIPO_PROJ, P.ID_REGISTRO, P.VAR_COD_LIV1
		FROM PROJECT P
		WHERE P.SYSTEM_ID IN 
			(
			SELECT DISTINCT PROJECT_ID
			FROM ARCHIVE_TEMPPROJECT TP INNER JOIN ARCHIVE_TRANSFERPOLICY P ON TP.TRANSFERPOLICY_ID = P.SYSTEM_ID
			WHERE P.TRANSFER_ID = ' AS NVARCHAR(MAX)) + CAST(@TransferID AS NVARCHAR(MAX)) + CAST(N'
			AND P.ENABLED = 1
			AND TP.DATRASFERIRE = 1
			AND TP.TIPOTRASFERIMENTO_VERSAMENTO = ''TRASFERIMENTO''
			)
		UNION ALL
		SELECT P.SYSTEM_ID, P.ID_PARENT, P.ID_FASCICOLO, P.VAR_CODICE, P.CHA_TIPO_FASCICOLO, P.CHA_TIPO_PROJ, P.ID_REGISTRO, P.VAR_COD_LIV1
		FROM PROJECT P INNER JOIN GERARCHIA_FASCICOLO GCT ON GCT.SYSTEM_ID = P.ID_PARENT
		)
		INSERT INTO #deletedProject (ID)
		SELECT DISTINCT SYSTEM_ID FROM GERARCHIA_FASCICOLO	
		' AS NVARCHAR(MAX))
		
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string
	
	set @sql_string_filtro_temp_fasc = CAST(N'
		SELECT ID FROM #deletedProject' AS NVARCHAR(MAX))
	
	-- Filtro fascicoli effettivamente trasferiti sul Deposito:
	--		- per poterli eliminare dal Corrente devono essere stati trasferiti, quindi presenti nella tabella PROJECT del Deposito
	--
	set @sql_string_filtro_trasferiti_fasc = CAST(N'
		SELECT SYSTEM_ID FROM PROJECT' AS NVARCHAR(MAX))



	-- *********
	-- Relazioni
	-- *********

	-- SECURITY (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella SECURITY (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella SECURITY (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID



	-- DELETED_SECURITY (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DELETED_SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DELETED_SECURITY (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DELETED_SECURITY (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- COMPONENTS
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.COMPONENTS
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)
		AND DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella COMPONENTS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella COMPONENTS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	

	
	-- VERSIONS
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.VERSIONS
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)
		AND DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella VERSIONS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella VERSIONS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_DOC_ARRIVO_PAR
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_DOC_ARRIVO_PAR
		WHERE ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DOC_ARRIVO_PAR per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DOC_ARRIVO_PAR per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_CORR_GLOBALI (solo se NON Interni e se legati all'unico documento da eliminare)
	--
	-- Creo una tabella temporanea in cui inserisco gli ID della tabella DPA_CORR_GLOBALI da eliminare
	-- e li utilizzo per eliminare anche i relativi record delle tabelle DPA_DETT_GLOBALI e DPA_MAIL_CORR_ESTERNI
	--
	IF OBJECT_ID('tempdb..#dpa_corr_globali_da_eliminare') IS NOT NULL DROP TABLE #dpa_corr_globali_da_eliminare
	CREATE TABLE #dpa_corr_globali_da_eliminare
	(
		ID int
	)

	SET @sql_string = CAST(N'
		INSERT INTO #dpa_corr_globali_da_eliminare (ID)
		SELECT AP.ID_MITT_DEST
		FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_DOC_ARRIVO_PAR AP 
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_CORR_GLOBALI CG ON AP.ID_MITT_DEST = CG.SYSTEM_ID
		WHERE CG.CHA_TIPO_IE <> ''I''
		AND AP.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND AP.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		GROUP BY AP.ID_MITT_DEST
		HAVING COUNT(*)=1' AS NVARCHAR(MAX))
		
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;
	
	-- DPA_DETT_GLOBALI
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_DETT_GLOBALI
		WHERE ID_CORR_GLOBALI IN
			(
			SELECT ID FROM #dpa_corr_globali_da_eliminare
			)' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DETT_GLOBALI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DETT_GLOBALI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_MAIL_CORR_ESTERNI
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_MAIL_CORR_ESTERNI
		WHERE ID_CORR IN
			(
			SELECT ID FROM #dpa_corr_globali_da_eliminare
			)' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_MAIL_CORR_ESTERNI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_MAIL_CORR_ESTERNI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
		
	-- DPA_CORR_GLOBALI
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_CORR_GLOBALI
		WHERE SYSTEM_ID IN
			(
			SELECT ID FROM #dpa_corr_globali_da_eliminare
			)' AS NVARCHAR(MAX))
				
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_CORR_GLOBALI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_CORR_GLOBALI per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- PROJECT_COMPONENTS
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROJECT_COMPONENTS
		WHERE LINK IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND LINK IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROJECT_COMPONENTS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROJECT_COMPONENTS per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_UTENTE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE TU FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_UTENTE TU 
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_SINGOLA TS ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_UTENTE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_UTENTE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_SINGOLA (Documenti) -  ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
	--
	set @sql_string = CAST(N'
		DELETE TS FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_SINGOLA TS
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_SINGOLA (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_SINGOLA (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASMISSIONE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE
		WHERE ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASMISSIONE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASMISSIONE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_NOTE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_NOTE
		WHERE TIPOOGGETTOASSOCIATO = ''D''
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_NOTE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_NOTE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TIMESTAMP_DOC (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TIMESTAMP_DOC
		WHERE DOC_NUMBER IN
		(
			SELECT DOCNUMBER FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.[PROFILE]
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
		)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_OGGETTARIO
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_OGGETTARIO
		WHERE SYSTEM_ID IN
			(
			SELECT ID_OGGETTO
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_OGGETTI_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_OGGETTI_STO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_CORR_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_CORR_STO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_CORR_STO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_CORR_STO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
		
	
	
	-- DPA_DATA_ARRIVO_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_DATA_ARRIVO_STO
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DATA_ARRIVO_STO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DATA_ARRIVO_STO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_STATO_INVIO
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_STATO_INVIO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_STATO_INVIO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_STATO_INVIO (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_PROF_PAROLE
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_PROF_PAROLE
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_PROF_PAROLE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_PROF_PAROLE (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_STAMPAREGISTRI
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_STAMPAREGISTRI
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER
			FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				AND ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)
			)
		' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_STAMPAREGISTRI (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_STAMPAREGISTRI (Documenti) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- SECURITY (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella SECURITY (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella SECURITY (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DELETED_SECURITY (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DELETED_SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DELETED_SECURITY (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DELETED_SECURITY (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_UTENTE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE TU FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_UTENTE TU 
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_SINGOLA TS ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_UTENTE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_UTENTE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_SINGOLA (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE TS FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASM_SINGOLA TS
		INNER JOIN ' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_SINGOLA (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_SINGOLA (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASMISSIONE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_TRASMISSIONE
		WHERE ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASMISSIONE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASMISSIONE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_NOTE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_NOTE
		WHERE TIPOOGGETTOASSOCIATO = ''F''
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_NOTE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_NOTE (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_ASS_TEMPLATES_FASC
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.DPA_ASS_TEMPLATES_FASC
		WHERE ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_ASS_TEMPLATES_FASC (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_ASS_TEMPLATES_FASC (Fascicoli) per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
		
	

	-- *******
	-- Oggetti
	-- *******
	
	-- PROFILE (documento principale e allegati)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROFILE
		WHERE 
			(
			SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			AND SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)	
		OR 
			(
			ID_DOCUMENTO_PRINCIPALE IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			AND ID_DOCUMENTO_PRINCIPALE IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_doc AS NVARCHAR(MAX)) + CAST(N'
				)
			)
			' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROFILE per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROFILE per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- PROJECT (fascicolo principale e suoi sotto-fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM
		' AS NVARCHAR(MAX)) + CAST(@nomeSchemaCorrente AS NVARCHAR(MAX)) + CAST(N'.PROJECT
		WHERE SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_temp_fasc AS NVARCHAR(MAX)) + CAST(N'
			)
		AND SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_trasferiti_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROJECT per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROJECT per il Transfer: ' + CAST(@TransferID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	

	COMMIT TRANSACTION T1
	
END
GO