USE PCM_DEPOSITO_FINGER
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================================================
-- Author:		Giovanni Olivari
-- Create date: 09/07/2013
-- Description:	Esegue lo scarto eliminando le informazioni dei documenti/fascicoli coinvolti
-- ==========================================================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_ExecuteDisposalByID 
	@DisposalID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType_Transfer int = 3 -- 'Disposal'
	DECLARE @logObjectID int = @DisposalID
	DECLARE @errorCode int

	DECLARE @sql_string nvarchar(MAX)
	DECLARE @sql_string_filtro_doc nvarchar(MAX)
	DECLARE @sql_string_filtro_fasc nvarchar(MAX)
	
	--DECLARE @sql_string_filtro_temp_doc nvarchar(MAX)
	--DECLARE @sql_string_filtro_trasferiti_doc nvarchar(MAX)
	--DECLARE @sql_string_filtro_temp_fasc nvarchar(MAX)
	--DECLARE @sql_string_filtro_trasferiti_fasc nvarchar(MAX)
	--DECLARE @nomeSchemaCorrente varchar(200) 
	--DECLARE @nomeUtenteCorrente varchar(200) 
	--DECLARE @nomeUtenteDeposito varchar(200) 

	--DECLARE @tipoStepConsolidamento_CONSOLIDATO_CONTENUTO_E_METADATI int = 2 -- Consolidato il documento nei suoi metadati fondamentali
	--DECLARE @idAutoreConsolidamento int
	--DECLARE @idRuoloConsolidamento int
	
/*
	
	
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
*/
	
	
	BEGIN TRANSACTION T1


	
	set @logType = 'INFO'
	set @log = 'Init procedura di eliminazione dei dati da scartare per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID



/*
	-- Consolidamento dei documenti portati in COPIA sul Deposito
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
			AND TP.TIPOTRASFERIMENTO_VERSAMENTO = ''COPIA''
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
*/


	-- Cancellazione dei documenti/fascicoli, e record correlati
	--
	-- ******
	-- Filtri
	-- ******

	-- Filtro documenti sulle tabelle temp del Deposito:
	--		- appartenenti al Disposal
	--		- DaScartare = 1 (True)
	--
	set @sql_string_filtro_doc = CAST(N'
		SELECT DISTINCT PROFILE_ID
		FROM ARCHIVE_TEMPPROFILEDISPOSAL TP
		WHERE TP.DISPOSAL_ID = ' AS NVARCHAR(MAX)) + CAST(@DisposalID AS NVARCHAR(MAX)) + CAST(N'
		AND TP.DASCARTARE = 1' AS NVARCHAR(MAX))

	-- Filtro fascicoli sulle tabelle temp del Deposito:
	--		- appartenenti al Disposal
	--		- DaScartare = 1 (True)
	--
	set @sql_string_filtro_fasc = CAST(N'
		SELECT DISTINCT PROJECT_ID
		FROM ARCHIVE_TEMPPROJECTDISPOSAL TP
		WHERE TP.DISPOSAL_ID = ' AS NVARCHAR(MAX)) + CAST(@DisposalID AS NVARCHAR(MAX)) + CAST(N'
		AND TP.DASCARTARE = 1' AS NVARCHAR(MAX))



	-- Soluzione temporanea: vengono messi gli oggetti (doc/fasc) nel cestino 
	-- per uno scarto "logico" in attesa dell'eliminazione dei file
	--

	-- Cestino PROFILE
	--
	set @sql_string = CAST(N'
		UPDATE PROFILE SET CHA_IN_CESTINO = 1
		WHERE SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante lo scarto (cestino) dei record dalla tabella PROFILE per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Scarto (cestino) dei record dalla tabella PROFILE per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

	-- Cestino PROJECT
	--
	set @sql_string = CAST(N'
		UPDATE PROJECT SET CHA_IN_CESTINO = 1
		WHERE SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante lo scarto (cestino) dei record dalla tabella PROJECT per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Scarto (cestino) dei record dalla tabella PROJECT per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID


	
	set @logType = 'INFO'
	set @log = 'Fine procedura di eliminazione dei dati da scartare per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID


	
	-- Cambio stato
	--
	DECLARE	@return_value INT
	DECLARE	@System_ID INT
	DECLARE @Disposal_ID INT
	DECLARE @DisposalStateType_ID INT
	DECLARE @disposalStateType_EFFETTUATO INT = 6
	
	EXEC	@return_value = sp_ARCHIVE_Insert_DisposalState
			@Disposal_ID = @DisposalID,
			@DisposalStateType_ID = @disposalStateType_EFFETTUATO,
			@System_ID = @System_ID OUTPUT



	COMMIT TRANSACTION T1





/*
	-- *******************************************************************************************
	-- Prima di iniziare l'eliminazione vanno salvate le info da mantenere per i doc/fasc scartati
	-- *******************************************************************************************



	-- *********
	-- Relazioni
	-- *********

	-- SECURITY (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella SECURITY (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella SECURITY (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID



	-- DELETED_SECURITY (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM DELETED_SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DELETED_SECURITY (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DELETED_SECURITY (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- COMPONENTS
	--
	set @sql_string = CAST(N'
		DELETE FROM COMPONENTS
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella COMPONENTS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella COMPONENTS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	

	
	-- VERSIONS
	--
	set @sql_string = CAST(N'
		DELETE FROM VERSIONS
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER FROM PROFILE WHERE SYSTEM_ID IN
				(
				' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella VERSIONS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella VERSIONS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_DOC_ARRIVO_PAR
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_DOC_ARRIVO_PAR
		WHERE ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DOC_ARRIVO_PAR per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DOC_ARRIVO_PAR per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
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
		FROM DPA_DOC_ARRIVO_PAR AP 
		INNER JOIN DPA_CORR_GLOBALI CG ON AP.ID_MITT_DEST = CG.SYSTEM_ID
		WHERE CG.CHA_TIPO_IE <> ''I''
		AND AP.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		GROUP BY AP.ID_MITT_DEST
		HAVING COUNT(*)=1' AS NVARCHAR(MAX))
		
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;
	
	-- DPA_DETT_GLOBALI
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_DETT_GLOBALI
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DETT_GLOBALI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DETT_GLOBALI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_MAIL_CORR_ESTERNI
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_MAIL_CORR_ESTERNI
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_MAIL_CORR_ESTERNI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_MAIL_CORR_ESTERNI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
		
	-- DPA_CORR_GLOBALI
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_CORR_GLOBALI
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_CORR_GLOBALI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_CORR_GLOBALI per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- PROJECT_COMPONENTS
	--
	set @sql_string = CAST(N'
		DELETE FROM PROJECT_COMPONENTS
		WHERE LINK IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROJECT_COMPONENTS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROJECT_COMPONENTS per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_UTENTE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE TU FROM
		DPA_TRASM_UTENTE TU 
		INNER JOIN DPA_TRASM_SINGOLA TS ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
		INNER JOIN DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_UTENTE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_UTENTE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_SINGOLA (Documenti) -  ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
	--
	set @sql_string = CAST(N'
		DELETE TS FROM
		DPA_TRASM_SINGOLA TS
		INNER JOIN DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_SINGOLA (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_SINGOLA (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASMISSIONE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_TRASMISSIONE
		WHERE ID_PROFILE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASMISSIONE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASMISSIONE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_NOTE (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_NOTE
		WHERE TIPOOGGETTOASSOCIATO = ''D''
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_NOTE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_NOTE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TIMESTAMP_DOC (Documenti)
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_TIMESTAMP_DOC
		WHERE DOC_NUMBER IN
		(
			SELECT DOCNUMBER FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_OGGETTARIO
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_OGGETTARIO
		WHERE SYSTEM_ID IN
			(
			SELECT ID_OGGETTO
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_OGGETTI_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_OGGETTI_STO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TIMESTAMP_DOC (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_CORR_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_CORR_STO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_CORR_STO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_CORR_STO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
		
	
	
	-- DPA_DATA_ARRIVO_STO
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_DATA_ARRIVO_STO
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_DATA_ARRIVO_STO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_DATA_ARRIVO_STO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_STATO_INVIO
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_STATO_INVIO
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_STATO_INVIO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_STATO_INVIO (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_PROF_PAROLE
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_PROF_PAROLE
		WHERE ID_PROFILE IN
			(
			SELECT SYSTEM_ID
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_PROF_PAROLE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_PROF_PAROLE (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_STAMPAREGISTRI
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_STAMPAREGISTRI
		WHERE DOCNUMBER IN
			(
			SELECT DOCNUMBER
			FROM PROFILE
			WHERE 
				(
				SYSTEM_ID IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
					)
				)	
			OR 
				(
				ID_DOCUMENTO_PRINCIPALE IN
					(
					' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_STAMPAREGISTRI (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_STAMPAREGISTRI (Documenti) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- SECURITY (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella SECURITY (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella SECURITY (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DELETED_SECURITY (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM DELETED_SECURITY
		WHERE THING IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DELETED_SECURITY (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DELETED_SECURITY (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_UTENTE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE TU FROM
		DPA_TRASM_UTENTE TU 
		INNER JOIN DPA_TRASM_SINGOLA TS ON TU.ID_TRASM_SINGOLA = TS.SYSTEM_ID
		INNER JOIN DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_UTENTE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_UTENTE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASM_SINGOLA (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE TS FROM
		DPA_TRASM_SINGOLA TS
		INNER JOIN DPA_TRASMISSIONE T ON TS.ID_TRASMISSIONE = T.SYSTEM_ID
		WHERE T.ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASM_SINGOLA (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASM_SINGOLA (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_TRASMISSIONE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_TRASMISSIONE
		WHERE ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_TRASMISSIONE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_TRASMISSIONE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_NOTE (Fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_NOTE
		WHERE TIPOOGGETTOASSOCIATO = ''F''
		AND IDOGGETTOASSOCIATO IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_NOTE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_NOTE (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- DPA_ASS_TEMPLATES_FASC
	--
	set @sql_string = CAST(N'
		DELETE FROM DPA_ASS_TEMPLATES_FASC
		WHERE ID_PROJECT IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella DPA_ASS_TEMPLATES_FASC (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella DPA_ASS_TEMPLATES_FASC (Fascicoli) per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
		
	

	-- *******
	-- Oggetti
	-- *******
	
	-- PROFILE (elimina tutti i doc e i suoi allegati)
	--
	set @sql_string = CAST(N'
		DELETE FROM PROFILE
		WHERE 
			SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
			)
		OR 
			ID_DOCUMENTO_PRINCIPALE IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_doc AS NVARCHAR(MAX)) + CAST(N'
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
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROFILE per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROFILE per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	
	
	-- PROJECT (da cancellare anche eventuali sotto-fascicoli)
	--
	set @sql_string = CAST(N'
		DELETE FROM PROJECT
		WHERE SYSTEM_ID IN
			(
			' AS NVARCHAR(MAX)) + CAST(@sql_string_filtro_fasc AS NVARCHAR(MAX)) + CAST(N'
			)' AS NVARCHAR(MAX))
			
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la cancellazione dei record dalla tabella PROJECT per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Cancellazione record dalla tabella PROJECT per il Disposal: ' + CAST(@DisposalID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_Transfer, @logObjectID
	
	

	COMMIT TRANSACTION T1
*/

END
GO
