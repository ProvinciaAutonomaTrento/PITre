USE PCM_DEPOSITO_FINGER
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =======================================================================================
-- Author:		Giovanni Olivari
-- Create date: 05/07/2013
-- Description:	Effettua la ricerca dei documenti/fascicoli i cui termini di conservazione
--              sono scaduti, e quindi possono essere scartati
-- =======================================================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_StartSearchForDisposal
	@DisposalID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)
	DECLARE @logObjectType VARCHAR(50) = 3 -- 'Disposal'
	DECLARE @logObjectID int = @DisposalID
	DECLARE @errorCode int
	
	DECLARE @sql_string nvarchar(MAX)
	DECLARE @annoCorrente INT
	DECLARE @dataLimite VARCHAR(10)



/*
	SPECIFICHE:

	- Gestione della sovrapposizione di più termini: vince il periodo più lungo
	- Selezione della data in cui inizia il calcolo per il termine della conservazione
		- Per un fascicolo procedimentale è la data di chiusura; eredita i termini di conservazione da
			- Classe di titolario sotto cui è stato creato
			- Tipologia fascicolo a cui appartiene
		- Per un documento è la data di creazione; eredita i termini di conservazione da
			- Tipologia documento a cui appartiene
			- Classe titolario del fascicolo in cui è contenuto, sia procedimentale che generale
	- Lo scarto avviene sempre per blocchi di anni, quindi il calcolo si fa sempre dal 01/01 al 31/12 dell'anno precedente all'avvio della ricerca
	- Di fatto i registro di repertorio e i registri di protocollo non si scartano, 
	  quindi dobbiamo mantenere tutte le informazioni che gestiamo per ciascun documento all’interno di queste due tipologie di registro.
	- Vengono scartati soltanto documenti e fascicoli trasferiti definitivamente nel depostito (non in copia)

	- Logica sulla sovrapposizione dei termini di conservazione:
		- Se il campo NUM_MESI_CONSERVAZIONE è valorizzato con 0 oppure è NULL, viene ignorato (se tutti i riferimenti sono così allora non entra mai nelle liste di scarto)
		- Se è valorizzato con N>0, allora viene preso in considerazione, confrontandolo con gli altri termini, e vale sempre la regola che vince il periodo più lungo
		- Di solito (almeno nell'installazione di PCM) viene utilizzato il valore 10000 per definire il termine di conservazione illimitato

*/	



	SET @annoCorrente = YEAR(GETDATE())
	SET @dataLimite = '01-01-' + CAST(@annoCorrente AS VARCHAR(4))



	BEGIN TRANSACTION T1


	
	-- Pulizia delle tabelle temp
	--
	DELETE FROM ARCHIVE_TEMPPROFILEDISPOSAL WHERE DISPOSAL_ID = @DisposalID

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la pulizia della tabella ARCHIVE_TEMPPROFILEDISPOSAL - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
				
	set @logType = 'INFO'
	set @log = 'Pulizia tabella ARCHIVE_TEMPPROFILEDISPOSAL'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID



	DELETE FROM ARCHIVE_TEMPPROJECTDISPOSAL WHERE DISPOSAL_ID = @DisposalID

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante la pulizia della tabella ARCHIVE_TEMPPROJECTDISPOSAL - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
				
	set @logType = 'INFO'
	set @log = 'Pulizia tabella ARCHIVE_TEMPPROJECTDISPOSAL'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID



	-- PROFILE
	-- Vengono selezionati i documenti che:
	--		- non sono già stati scartati (non sono nel cestino)
	--		- non sono nella stampa registro
	--		- non sono nella stampa repertorio
	--
	SET @sql_string = CAST(N'
		INSERT INTO ARCHIVE_TEMPPROFILEDISPOSAL (DISPOSAL_ID, PROFILE_ID, DASCARTARE)
		SELECT DISTINCT ' AS NVARCHAR(MAX)) + CAST(@DisposalID AS NVARCHAR(MAX)) + CAST(N', PROFILE_ID, 1
		FROM
			(
			SELECT 
				PROFILE_ID
				, DOCNUMBER
				, DATA_SCADENZA_TIPOLOGIA_DOC
				, DATA_SCADENZA_FASCICOLO
				, DATA_SCADENZA_TIPOLOGIA_FASC
				, 
				(
					SELECT MAX(D) 
					FROM 
						(
						VALUES 
							(DATA_SCADENZA_TIPOLOGIA_DOC), 
							(DATA_SCADENZA_FASCICOLO),
							(DATA_SCADENZA_TIPOLOGIA_FASC)
						) AS VALUE(D)
				) AS SCADENZA_MAX
			FROM
				(
				SELECT 
					P.SYSTEM_ID PROFILE_ID
					, P.DOCNUMBER
					, DATEADD(MONTH, CASE WHEN TA.NUM_MESI_CONSERVAZIONE = 0 THEN NULL ELSE TA.NUM_MESI_CONSERVAZIONE END, P.CREATION_DATE) DATA_SCADENZA_TIPOLOGIA_DOC
					, DATEADD(MONTH, CASE WHEN F.NUM_MESI_CONSERVAZIONE = 0 THEN NULL ELSE F.NUM_MESI_CONSERVAZIONE END, P.CREATION_DATE) DATA_SCADENZA_FASCICOLO
					, DATEADD(MONTH, CASE WHEN TF.NUM_MESI_CONSERVAZIONE = 0 THEN NULL ELSE TF.NUM_MESI_CONSERVAZIONE END, P.CREATION_DATE) DATA_SCADENZA_TIPOLOGIA_FASC
				FROM PROFILE P
				LEFT OUTER JOIN DPA_TIPO_ATTO TA ON P.ID_TIPO_ATTO = TA.SYSTEM_ID
				LEFT OUTER JOIN PROJECT_COMPONENTS PC ON PC.LINK = P.SYSTEM_ID
				LEFT OUTER JOIN PROJECT RF ON RF.SYSTEM_ID = PC.PROJECT_ID
				LEFT OUTER JOIN PROJECT F ON F.SYSTEM_ID = RF.ID_PARENT
				LEFT OUTER JOIN DPA_TIPO_FASC TF ON TF.SYSTEM_ID = F.ID_TIPO_FASC
				WHERE ISNULL(P.CHA_IN_CESTINO, 0) = 0
				AND NOT EXISTS
					(
					SELECT ''X'' FROM DPA_STAMPAREGISTRI SR1 WHERE SR1.DOCNUMBER = P.DOCNUMBER
					)
				AND NOT EXISTS
					(
					SELECT ''X'' FROM DPA_STAMPA_REPERTORI SR2 WHERE SR2.DOCNUMBER = P.DOCNUMBER
					)
				AND EXISTS
					(
					SELECT ''X'' FROM ARCHIVE_TEMPPROFILE TP WHERE TP.PROFILE_ID = P.SYSTEM_ID
					AND TP.TIPOTRASFERIMENTO_VERSAMENTO = ''TRASFERIMENTO''
					)					
				) DATE_SCADENZA
			) T
		WHERE T.SCADENZA_MAX < CONVERT(DATETIME,''' AS NVARCHAR(MAX)) + CAST(@dataLimite AS NVARCHAR(MAX)) + CAST(N''', 103)
		' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento nella tabella ARCHIVE_TEMPPROFILEDISPOSAL - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
				
	set @logType = 'INFO'
	set @log = 'Inserimento dati in ARCHIVE_TEMPPROFILEDISPOSAL'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID



	-- PROJECT
	--
	SET @sql_string = CAST(N'
		INSERT INTO ARCHIVE_TEMPPROJECTDISPOSAL (DISPOSAL_ID, PROJECT_ID, DASCARTARE)
		SELECT DISTINCT ' AS NVARCHAR(MAX)) + CAST(@DisposalID AS NVARCHAR(MAX)) + CAST(N', PROJECT_ID, 1
		FROM
			(
			SELECT 
				PROJECT_ID
				, DATA_SCADENZA_TIPOLOGIA_FASC
				, DATA_SCADENZA_FASCICOLO
				, 
				(
					SELECT MAX(D) 
					FROM 
						(
						VALUES 
							(DATA_SCADENZA_TIPOLOGIA_FASC), 
							(DATA_SCADENZA_FASCICOLO)
						) AS VALUE(D)
				) AS SCADENZA_MAX
			FROM
				(
				SELECT 
					P.SYSTEM_ID PROJECT_ID
					, DATEADD(MONTH, CASE WHEN TF.NUM_MESI_CONSERVAZIONE = 0 THEN NULL ELSE TF.NUM_MESI_CONSERVAZIONE END, P.DTA_CHIUSURA) DATA_SCADENZA_TIPOLOGIA_FASC
					, DATEADD(MONTH, CASE WHEN CT.NUM_MESI_CONSERVAZIONE = 0 THEN NULL ELSE CT.NUM_MESI_CONSERVAZIONE END, P.DTA_CHIUSURA) DATA_SCADENZA_FASCICOLO
				FROM PROJECT P
				LEFT OUTER JOIN DPA_TIPO_FASC TF ON P.ID_TIPO_FASC = TF.SYSTEM_ID
				LEFT OUTER JOIN PROJECT CT ON CT.SYSTEM_ID = P.ID_PARENT
				WHERE P.CHA_TIPO_FASCICOLO = ''P''
				AND ISNULL(P.CHA_IN_CESTINO, 0) = 0
				AND EXISTS
					(
					SELECT ''X'' FROM ARCHIVE_TEMPPROJECT TP WHERE TP.PROJECT_ID = P.SYSTEM_ID
					AND TP.TIPOTRASFERIMENTO_VERSAMENTO = ''TRASFERIMENTO''
					)					
				) DATE_SCADENZA
			) T
		WHERE T.SCADENZA_MAX < CONVERT(DATETIME,''' AS NVARCHAR(MAX)) + CAST(@dataLimite AS NVARCHAR(MAX)) + CAST(N''', 103)
		' AS NVARCHAR(MAX))
	
	PRINT @sql_string;

	EXECUTE sp_executesql @sql_string;

	set @errorCode = @@ERROR

	IF @errorCode <> 0
	BEGIN
		-- Rollback the transaction
		ROLLBACK
		
		set @logType = 'ERROR'
		set @log = 'Errore durante l''inserimento nella tabella ARCHIVE_TEMPPROJECTDISPOSAL - Codice errore: ' + CAST(@errorCode AS NVARCHAR(8))
		
		EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID

		-- Raise an error and return
		RAISERROR (@log, 16, 1)
		RETURN
	END
				
	set @logType = 'INFO'
	set @log = 'Inserimento dati in ARCHIVE_TEMPPROJECTDISPOSAL'
	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject, @logObjectType, @logObjectID
	
	
	
	-- Cambio stato
	--
	DECLARE	@return_value INT
	DECLARE	@System_ID INT
	DECLARE @Disposal_ID INT
	DECLARE @DisposalStateType_ID INT
	DECLARE @disposalStateType_RICERCA_COMPLETATA INT = 2
	
	EXEC	@return_value = sp_ARCHIVE_Insert_DisposalState
			@Disposal_ID = @DisposalID,
			@DisposalStateType_ID = @disposalStateType_RICERCA_COMPLETATA,
			@System_ID = @System_ID OUTPUT
	
	COMMIT TRANSACTION T1

END
GO
