-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 07/03/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- HISTORICIZEROLE:
-- Store per la storicizzazione di un ruolo. Per ridurre al minimo le movimentazioni di dati, specialmente nella securiy, viene
-- adottata una tecnica di storicizzazione "verso l'alto" ovvero,quando si deve storicizzare un ruolo R con system id S e
-- codice rubrica C, viene inserita nella DPA_CORR_GLOBALI una tupla che si differenzia da quella di R solo per S che sar un nuovo
-- numero assegnato dalla sequence, var_codice e var_cod_rubrica che saranno uguali a quelli di R con l'aggiunta di _S, ed id_old
-- che sar impostato ad S (Attenzione! S  il system id di R). A questo punto, le eventuali modifiche ad attributi del ruolo
-- verranno salvate sul record di R e tutte le tuple di documenti (doc_arrivo_par), le informazioni sul creatore di documenti e
-- fascioli e le informazioni sulle trasmissioni che referenziano R, vengono aggiornate in modo che facciano riferimento al nuovo
-- record della DPA_CORR_GLOBALI appena inserito.
-- =============================================


CREATE PROCEDURE [DOCSADM].[HISTORICIZEROLE] 
(
	-- Id corr globali del ruolo da storicizzare
	@idCorrGlobRole			INT ,
	-- Eventuale nuovo codice da assegnare al ruolo
	@newRoleCode			VARCHAR(2000),
	-- Eventuale nuova descrizione da assegnare al ruolo
	@newRoleDescription		VARCHAR(2000),
	-- Identificativo dell'eventuale nuova UO in cui deve essere inserito il ruolo
	@newRoleUoId			VARCHAR(2000),
	-- Identificativo dell'eventuale nuovo tipo ruolo da assegnare al ruolo
	@newRoleTypeId			INT,
	-- Identificativo del record storicizzato
	@oldIdCorrGlobId		INT OUTPUT,
	-- Risultato dell'operazione
	@returnValue			INT OUTPUT
) 
AS
BEGIN
	-- Nome della colonna letta dalla tabella dei metadati
	DECLARE @colName VARCHAR(2000)
	-- Lista separata da , dei nomi delle colonne in cui eseguire la insert
	DECLARE @colNameList VARCHAR(4000)
	-- Lista separata da , dei valori da assegnare alle colonne
	DECLARE @colValuesList VARCHAR(4000)
	
	-- Selezione delle colonne della corr globali dalla tabella dei metadati
	DECLARE CURCOLUMNS CURSOR FOR
	SELECT NAME
	FROM SYS.COLUMNS
	WHERE OBJECT_ID = (SELECT OBJECT_ID FROM SYS.TABLES WHERE NAME = 'DPA_CORR_GLOBALI')
	
	OPEN CURCOLUMNS
	
	FETCH NEXT FROM CURCOLUMNS
	INTO @colName
	
	WHILE (@@FETCH_STATUS =0)
	BEGIN 
		-- Se la colonna  una colonna di quelle che deve eesere modificata, viene inserito il valore modificato altrimenti viene lasciata com'
		
		SET @colNameList = @colNameList+','+@colName
		
		IF (@colName = 'SYSTEM_ID')
			SET @colValuesList = @colValuesList+.........
		ELSE IF (@colName = 'VAR_CODICE')
			SET @colValuesList = @colValuesList+........
		ELSE IF (@colName = 'VAR_COD_RUBRICA')
			SET @colValuesList = @colValuesList+........
		ELSE IF (@colName = 'DTA_FINE')
			SET @colValuesList = @colValuesList+','+CAST(GETDATE() AS VARCHAR)
		ELSE
			SET @colValuesList = @colValuesList+','+@colName
		
		
		
		
		
		FETCH NEXT FROM CURCOLUMNS
		INTO @colName
	
	END
	
	CLOSE CURCOLUMNS
	DEALLOCATE CURCOLUMNS
	
	SET @colNameList = SUBSTRING(@colNameList,1,3)
	SET @colValuesList = SUBSTRING(@colValuesList,1,3)
	
	EXECUTE ( 'INSERT INTO dpa_corr_globali (' + @colNameList + ') 
							( SELECT ' + @colValuesList + ' FROM dpa_corr_globali WHERE system_id = ' + @idCorrGlobRole + ')')
							
	SELECT @oldIdCorrGlobId = MAX(SYSTEM_ID) FROM DOCSADM.DPA_CORR_GLOBALI WHERE ORIGINAL_ID = @idCorrGlobRole

	-- Aggiornamento dei dati relativi al nuovo ruolo e impostazione dell'id_old
	UPDATE DOCSADM.DPA_CORR_GLOBALI
	SET ID_OLD = @oldIdCorrGlobId,
		VAR_CODICE = @newRoleCode,
		VAR_DESC_CORR = @newRoleDescription,
		ID_UO = @newRoleUoId,
		ID_TIPO_RUOLO = @newRoleTypeId
	WHERE SYSTEM_ID = @idcorrglobrole
	
	-- Cancellazione dell'id gruppo per il gruppo storicizzato
	UPDATE DOCSADM.DPA_CORR_GLOBALI
	SET ID_GRUPPO = NULL
	WHERE  SYSTEM_ID = @oldIdCorrGlobId

	-- Aggiornamento degli id mittente e destinatario relativi al ruolo
	UPDATE DOCSADM.DPA_DOC_ARRIVO_PAR
	SET ID_MITT_DEST = @oldidcorrglobid
	WHERE ID_MITT_DEST = @idcorrglobrole

	-- Aggiornamento degli id dei ruoli creatori per documenti e fascicoli
	UPDATE DOCSADM.PROFILE
	SET ID_RUOLO_CREATORE = @oldidcorrglobid
	WHERE ID_RUOLO_CREATORE = @idcorrglobrole

	UPDATE PROJECT
	SET ID_RUOLO_CREATORE = @oldidcorrglobid
	WHERE ID_RUOLO_CREATORE = @idcorrglobrole

	-- Aggiornamento delle trasmissioni
	UPDATE DOCSADM.DPA_TRASMISSIONE
	SET ID_RUOLO_IN_UO = @oldidcorrglobid
	WHERE ID_RUOLO_IN_UO = @idcorrglobrole

	UPDATE DOCSADM.DPA_TRASM_SINGOLA
	SET ID_CORR_GLOBALE = @oldidcorrglobid
	WHERE ID_CORR_GLOBALE= @idcorrglobrole
	
	-- modifiche by Luciani

	--per le policy di conservazione
	UPDATE DOCSADM.POLICY_CONSERVAZIONE
	SET ID_RUOLO = @oldIdCorrGlobId 
	WHERE ID_RUOLO = @idCorrGlobRole


	-- Aggiornamento corrispondenti dei campi profilati
	UPDATE DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES 
	SET VALORE_OGGETTO_DB = @oldIdCorrGlobId
	WHERE EXISTS(SELECT 'x'	FROM DOCSADM.DPA_OGGETTI_CUSTOM OC WHERE OC.ID_TIPO_OGGETTO =
		(SELECT tObj.SYSTEM_ID FROM DPA_TIPO_OGGETTO TOBJ WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente')
			AND OC.SYSTEM_ID = DOCSADM.DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO)
	AND VALORE_OGGETTO_DB =  @IDCORRGLOBROLE 
	
	UPDATE DOCSADM.DPA_ASS_TEMPLATES_FASC 
	SET VALORE_OGGETTO_DB = @oldIdCorrGlobId
	WHERE EXISTS(SELECT 'x'	FROM DOCSADM.DPA_OGGETTI_CUSTOM_FASC OC	WHERE OC.ID_TIPO_OGGETTO =
		(SELECT tObj.SYSTEM_ID	FROM DOCSADM.DPA_TIPO_OGGETTO_FASC TOBJ	WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente')
			AND OC.SYSTEM_ID = DOCSADM.DPA_ASS_TEMPLATES_FASC.ID_OGGETTO)
	AND VALORE_OGGETTO_DB = @idCorrGlobRole 

	-- fine modifiche by Luciani

	SET @returnValue = 1 

END 


--colNameList := colNameList || ', ' || colName;

--CASE (colName)
--WHEN 'SYSTEM_ID' THEN
--colValuesList := colValuesList || ', ' || 'SEQ.NEXTVAL';
--WHEN 'VAR_CODICE' THEN
--colValuesList := colValuesList || ', ' || nq'#VAR_CODICE || '_' || SEQ.CURRVAL#';
--WHEN 'VAR_COD_RUBRICA' THEN
--colValuesList := colValuesList || ', ' || nq'#VAR_COD_RUBRICA || '_' || SEQ.CURRVAL#';
--WHEN 'DTA_FINE' THEN
--colValuesList := colValuesList || ', ' || 'SYSDATE';
--ELSE
--colValuesList := colValuesList || ', ' || colName;
--END CASE;
--END LOOP;
--CLOSE curColumns;

