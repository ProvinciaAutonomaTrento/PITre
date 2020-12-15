USE PCM_DEPOSITO_1
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ====================================================
-- Author:		Giovanni Olivari
-- Create date: 22/05/2013
-- Description:	Esegue l'analisi per la Policy indicata
-- ====================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_StartAnalysisForPolicy
	@PolicyID int
AS
BEGIN
	DECLARE @COPIA VARCHAR(20)= 'COPIA'
	DECLARE @TRASFERIMENTO VARCHAR(20)= 'TRASFERIMENTO'
	
	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType_TransferPolicy int = 2 -- 'TransferPolicy'
	DECLARE @logObjectID int = @PolicyID
	DECLARE @errorCode int
	
	DECLARE @policyState int
	DECLARE @policyState_RICERCA_COMPLETATA int = 3 -- RICERCA COMPLETATA
	DECLARE @policyState_ANALISI_IN_CORSO int = 4 -- ANALISI IN CORSO
	DECLARE @policyState_ANALISI_COMPLETATA int = 5 -- ANALISI COMPLETATA


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



	-- Verifica che lo stato della policy sia compatibile con l'esecuzione dell'analisi
	--
	SELECT @policyState = TRANSFERPOLICYSTATE_ID
	FROM ARCHIVE_TRANSFERPOLICY
	WHERE SYSTEM_ID = @PolicyID

	IF (@policyState <> @policyState_RICERCA_COMPLETATA)
	BEGIN
		set @logType = 'ERROR'
		set @log = 'Policy in stato non compatibile con l''avvio dell''analisi - PolicyID: ' + CAST(@PolicyID AS NVARCHAR(MAX))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID
		print @log

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END



	BEGIN TRANSACTION T1



	-- Aggiorna lo stato della Policy a ANALISI IN CORSO
	--
	UPDATE ARCHIVE_TRANSFERPOLICY
	SET TRANSFERPOLICYSTATE_ID = @policyState_ANALISI_IN_CORSO
	WHERE SYSTEM_ID = @PolicyID 
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento stato ANALISI IN CORSO per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID



	-- *********
	-- DOCUMENTI
	-- *********
	
	

	-- Aggiorna tutti i documenti della policy a TRASFERIMENTO
	--
	UPDATE ARCHIVE_TempProfile 
	SET 
		TipoTrasferimento_Policy = @TRASFERIMENTO,
		CopiaPerCatenaDoc_Policy = 0,
		CopiaPerFascicolo_Policy = 0,
		CopiaPerConservazione_Policy = 0
	WHERE TransferPolicy_ID = @PolicyID
	
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento documenti per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento documenti per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID



    -- Fascicoli
    -- Un documento è in COPIA se:
    --		- è contenuto in un fascicolo procedimentale che non è presente nel DB di Deposito
    --		oppure
    --		- è contenuto in un fascicolo procedimentale che non fa parte della policy corrente (anche se presente nella policy ha il flag DaTrasferire = 0)
	--
	UPDATE ARCHIVE_TEMPPROFILE 
	SET 
		TIPOTRASFERIMENTO_POLICY = @COPIA,
		CopiaPerFascicolo_Policy = 1
	WHERE TRANSFERPOLICY_ID = @PolicyID
	AND PROFILE_ID IN
		(
		SELECT DISTINCT TDF.PROFILE_ID
		FROM ARCHIVE_TEMP_PROJECT_PROFILE TDF 
		LEFT OUTER JOIN PROJECT F ON TDF.PROJECT_ID = F.SYSTEM_ID -- Fascicoli già presenti nel DB del Deposito
		LEFT OUTER JOIN ARCHIVE_TEMPPROJECT TF ON TDF.PROJECT_ID = TF.PROJECT_ID -- Fascicoli della policy corrente
		WHERE TDF.TRANSFERPOLICY_ID = @POLICYID
		AND ISNULL(F.CHA_TIPO_FASCICOLO,'') != 'G'
		AND (F.SYSTEM_ID IS NULL OR (NOT TF.PROJECT_ID IS NULL AND TF.DATRASFERIRE = 0))
		)
		
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento documenti (Copia per fascicoli) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento documenti (Copia per fascicoli) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID
		

    
    -- Catene doc
    -- Un documento è in COPIA se:
    --		- il collegato non è già nel DB di Deposito 
    --		- il collegato non è nella policy corrente
	--
	UPDATE ARCHIVE_TEMPPROFILE 
	SET 
		TIPOTRASFERIMENTO_POLICY = @COPIA,
		CopiaPerCatenaDoc_Policy = 1
	WHERE TRANSFERPOLICY_ID = @PolicyID
	AND PROFILE_ID IN
		(
		SELECT distinct Profile_ID FROM
		(
		-- Documenti che fanno parte di catene per la policy corrente
		SELECT PROFILE_ID, LINKEDDOC_ID FROM ARCHIVE_TEMPCATENEDOC WHERE TRANSFERPOLICY_ID = @PolicyID
		UNION
		SELECT LINKEDDOC_ID, PROFILE_ID PROFILE_ID FROM ARCHIVE_TEMPCATENEDOC WHERE TRANSFERPOLICY_ID = @PolicyID
		) T
		WHERE LINKEDDOC_ID NOT IN (SELECT PROFILE_ID FROM ARCHIVE_TEMPPROFILE WHERE TRANSFERPOLICY_ID = @PolicyID)
		AND LINKEDDOC_ID NOT IN (SELECT SYSTEM_ID FROM PROFILE)
		)
	
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento documenti (Copia per catene doc) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento documenti (Copia per catene doc) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID


	    
    -- In conservazione
    --
	UPDATE ARCHIVE_TempProfile 
	SET 
		TipoTrasferimento_Policy = @COPIA,
		CopiaPerConservazione_Policy = 1
	WHERE TransferPolicy_ID = @PolicyID
	AND INCONSERVAZIONE = 1
	
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento documenti (Copia per conservazione) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento documenti (Copia per conservazione) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID
    
    
    
    -- Reset del flag MantieniCopia
    --
	--UPDATE ARCHIVE_TempProfile 
	--SET MantieniCopia = 0
	--WHERE TransferPolicy_ID = @PolicyID
	
	--set @errorCode = @@ERROR

	--IF @errorCode <> 0
	--BEGIN
	--	-- Rollback the transaction
	--	ROLLBACK
		
	--	set @logType = 'ERROR'
	--	set @log = 'Errore durante l''aggiornamento del flag MantieniCopia per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
	--	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

	--	-- Raise an error and return
	--	RAISERROR (@log, 16, 1)
	--	RETURN
	--END
	
	--set @logType = 'INFO'
	--set @log = 'Aggiornamento flag MantieniCopia per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	--EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID
    
    
    
    -- *********
    -- FASCICOLI
    -- *********
    
    
    
	-- Aggiorna tutti i fascicoli della policy a TRASFERIMENTO
	--
	UPDATE ARCHIVE_TempProject 
	SET 
		TipoTrasferimento_Policy = @TRASFERIMENTO
	WHERE TransferPolicy_ID = @PolicyID
	
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento (fascicoli) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento (fascicoli) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID


	    
    -- In conservazione
    --
	UPDATE ARCHIVE_TempProject
	SET 
		TipoTrasferimento_Policy = @COPIA
	WHERE TransferPolicy_ID = @PolicyID
	AND INCONSERVAZIONE = 1
	
	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''aggiornamento del tipo trasferimento fascicoli (Copia per conservazione) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX)) + ' - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento tipo trasferimento fascicoli (Copia per conservazione) per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID



	-- Aggiorna lo stato della Policy a ANALISI COMPLETATA
	--
	UPDATE ARCHIVE_TRANSFERPOLICY
	SET TRANSFERPOLICYSTATE_ID = @policyState_ANALISI_COMPLETATA
	WHERE SYSTEM_ID = @PolicyID 
	
	set @logType = 'INFO'
	set @log = 'Aggiornamento stato ANALISI COMPLETATA per la Policy: ' + CAST(@PolicyID AS NVARCHAR(MAX))
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType_TransferPolicy, @logObjectID
    
    
    
	COMMIT TRANSACTION T1
    
END
GO
