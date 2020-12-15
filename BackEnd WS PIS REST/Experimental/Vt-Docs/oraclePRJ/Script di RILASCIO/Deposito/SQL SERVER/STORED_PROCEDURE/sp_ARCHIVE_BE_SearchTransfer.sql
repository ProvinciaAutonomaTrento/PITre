USE PCM_DEPOSITO_FINGER
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ==========================================================
-- Author:		Giovanni Olivari
-- Create date: 31/05/2013
-- Description:	Ricerca dei versamenti con filtro sugli stati
-- ==========================================================
ALTER PROCEDURE DOCSADM.sp_ARCHIVE_BE_SearchTransfer 
	@filtro_InDefinizione VARCHAR(1000),
	@filtro_AnalisiCompletata VARCHAR(1000),
	@filtro_Proposto VARCHAR(1000),
	@filtro_Approvato VARCHAR(1000),
	@filtro_InEsecuzione VARCHAR(1000),
	@filtro_Effettuato VARCHAR(1000),
	@filtro_InErrore VARCHAR(1000),
	@tipoStatoEsecuzione INT -- 1 -> ESEGUITO; 2 -> ERRORE; 3 -> TUTTI
AS
BEGIN

	-- Log test
	--
	DECLARE @log VARCHAR(2000)
	DECLARE @logType VARCHAR(10)
	DECLARE @logObject VARCHAR(50) = OBJECT_NAME(@@PROCID)

	set @logType = 'INFO'
	
	/*
	Log utlizzato per Debug
	
	set @log = '@filtro_InDefinizione: ' + CAST(ISNULL(@filtro_InDefinizione, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_AnalisiCompletata: ' + CAST(ISNULL(@filtro_AnalisiCompletata, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_Proposto: ' + CAST(ISNULL(@filtro_Proposto, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_Approvato: ' + CAST(ISNULL(@filtro_Approvato, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_InEsecuzione: ' + CAST(ISNULL(@filtro_InEsecuzione, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_Effettuato: ' + CAST(ISNULL(@filtro_Effettuato, 'NULL') AS NVARCHAR(MAX))
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject

	set @log = '@filtro_InErrore: ' + CAST(ISNULL(@filtro_InErrore, 'NULL') AS NVARCHAR(MAX))	
	EXECUTE sp_ARCHIVE_BE_InsertLog @log, @logType, @logObject
	*/
	


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @sql_string nvarchar(MAX)
	DECLARE @sql_string_select nvarchar(MAX)
	DECLARE @sql_string_from nvarchar(MAX)
	DECLARE @sql_string_where nvarchar(MAX)
	DECLARE @sql_string_filtroStati nvarchar(MAX) = ''
	DECLARE @sql_string_filtroStatiEsecuzione nvarchar(MAX) = ''

	SET @sql_string_select = CAST(N'
		SELECT 
			A.SYSTEM_ID ID_VERSAMENTO,
			A.ID_AMMINISTRAZIONE,
			A.DESCRIPTION DESCRIZIONE,
			TST.NAME STATO,
			TS.DATETIME DATA,
			NUM_DOC.NUM_DOCUMENTI_TRASFERITI NUMERO_DOC_EFFETTIVI,
			NUM_DOC.NUM_DOCUMENTI_COPIATI NUMERO_DOC_COPIE 
		' AS NVARCHAR(MAX))

	SET @sql_string_from = CAST(N' 
		FROM 
			ARCHIVE_TRANSFER A,
			ARCHIVE_TRANSFERSTATE TS,
			ARCHIVE_TRANSFERSTATETYPE TST,
			(
			SELECT 
				T.SYSTEM_ID TRANSFER_ID
				, (SUM(ISNULL(T_NUM_DOCUMENTI_TRASFERITI.NUM_DOCUMENTI, 0)) + SUM(ISNULL(T_NUM_DOCUMENTI_COPIATI.NUM_DOCUMENTI, 0))) TOTALE_DOCUMENTI
				, SUM(ISNULL(T_NUM_DOCUMENTI_TRASFERITI.NUM_DOCUMENTI, 0)) NUM_DOCUMENTI_TRASFERITI
				, SUM(ISNULL(T_NUM_DOCUMENTI_COPIATI.NUM_DOCUMENTI, 0)) NUM_DOCUMENTI_COPIATI
			FROM 
			ARCHIVE_TRANSFER T
			LEFT OUTER JOIN ARCHIVE_TRANSFERPOLICY TP ON T.SYSTEM_ID = TP.TRANSFER_ID
			LEFT OUTER JOIN 
					(
					SELECT P.TRANSFERPOLICY_ID, COUNT(*) NUM_DOCUMENTI
					FROM ARCHIVE_TEMPPROFILE P
					WHERE P.TIPOTRASFERIMENTO_VERSAMENTO = ''TRASFERIMENTO''
					GROUP BY P.TRANSFERPOLICY_ID
					) T_NUM_DOCUMENTI_TRASFERITI ON T_NUM_DOCUMENTI_TRASFERITI.TRANSFERPOLICY_ID = TP.SYSTEM_ID
					LEFT OUTER JOIN 
					(
					SELECT P.TRANSFERPOLICY_ID, COUNT(*) NUM_DOCUMENTI
					FROM ARCHIVE_TEMPPROFILE P
					WHERE P.TIPOTRASFERIMENTO_VERSAMENTO = ''COPIA''
					GROUP BY P.TRANSFERPOLICY_ID
					) T_NUM_DOCUMENTI_COPIATI ON T_NUM_DOCUMENTI_COPIATI.TRANSFERPOLICY_ID = TP.SYSTEM_ID
			GROUP BY T.SYSTEM_ID
			) NUM_DOC,
			(
			SELECT T.TRANSFER_ID, T.TRANSFERSTATETYPE_ID, T.DATETIME
			FROM
				(
				SELECT TS.TRANSFER_ID, TS.TRANSFERSTATETYPE_ID, TS.DATETIME
				, RN = ROW_NUMBER() OVER (PARTITION BY TS.TRANSFER_ID ORDER BY TS.SYSTEM_ID DESC)
				FROM ARCHIVE_TRANSFERSTATE TS
				) T
			WHERE T.RN = 1
			) STATO_CORRENTE 			
		' AS NVARCHAR(MAX))
	
	SET @sql_string_where = CAST(N' 
		WHERE 
			A.SYSTEM_ID = TS.TRANSFER_ID 
			AND A.SYSTEM_ID = NUM_DOC.TRANSFER_ID
			AND A.SYSTEM_ID = STATO_CORRENTE.TRANSFER_ID
			AND STATO_CORRENTE.TRANSFERSTATETYPE_ID = TST.SYSTEM_ID
		' AS NVARCHAR(MAX))
	
	
	
	-- Costruisce la condizione per selezionare i versamenti effettuati completamente, compreso lo spostamento dei file se previsto
	-- Un versamento è Effettuato se:
	--		- È nello stato 6 (EFFETTUATO) e non ha spostamenti di file pendenti
	--			oppure
	--		- È nello stato 8 (EFFETTUATO COMPRESI FILE)
	--
	DECLARE @sql_string_condizione_VersamentoEffettuato nvarchar(MAX)
	
	SET @sql_string_condizione_VersamentoEffettuato = '((STATO_CORRENTE.TRANSFERSTATETYPE_ID = 6 AND NOT EXISTS (SELECT ''X'' FROM ARCHIVE_TEMPTRANSFERFILE TTF WHERE TTF.TRANSFER_ID = STATO_CORRENTE.TRANSFER_ID)) OR STATO_CORRENTE.TRANSFERSTATETYPE_ID = 8) '
	
	SELECT @sql_string_filtroStatiEsecuzione = CASE @tipoStatoEsecuzione
		WHEN 1 THEN ' AND ' + @sql_string_condizione_VersamentoEffettuato
		WHEN 2 THEN ' AND STATO_CORRENTE.TRANSFERSTATETYPE_ID = 7 '
		WHEN 3 THEN ' AND (' + @sql_string_condizione_VersamentoEffettuato + ' OR STATO_CORRENTE.TRANSFERSTATETYPE_ID = 7) '
		ELSE ''
	END
	
	IF (ISNULL(@sql_string_filtroStatiEsecuzione, '') = '')
		BEGIN
			DECLARE @message VARCHAR(200) = 'Valore non previsto per il parametro @tipoStatoEsecuzione - Valori accettati: (1,2,3) - Fornito: ' + + CAST(@tipoStatoEsecuzione AS NVARCHAR(MAX))
			RAISERROR (@message, 16, 1)
			RETURN			
		END
	
	-- 1 - IN DEFINIZIONE
	--
	IF (ISNULL(@filtro_InDefinizione, '') <> '')
		BEGIN
			
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_InDefinizione AS NVARCHAR(MAX)) + CAST(' T1 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T1.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',1' AS NVARCHAR(MAX))
			
		END
	
	-- 2 - ANALISI COMPLETATA
	--
	IF (ISNULL(@filtro_AnalisiCompletata, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_AnalisiCompletata AS NVARCHAR(MAX)) + CAST(' T2 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T2.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',2' AS NVARCHAR(MAX))
			
		END
	
	-- 3 - PROPOSTO
	--
	IF (ISNULL(@filtro_Proposto, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_Proposto AS NVARCHAR(MAX)) + CAST(' T3 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T3.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',3' AS NVARCHAR(MAX))
			
		END
	
	-- 4 - APPROVATO
	--
	IF (ISNULL(@filtro_Approvato, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_Approvato AS NVARCHAR(MAX)) + CAST(' T4 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T4.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',4' AS NVARCHAR(MAX))
			
		END
	
	-- 5 - IN ESECUZIONE
	--
	IF (ISNULL(@filtro_InEsecuzione, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_InEsecuzione AS NVARCHAR(MAX)) + CAST(' T5 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T5.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',5' AS NVARCHAR(MAX))
			
		END
	
	-- 6 - EFFETTUATO
	--
	IF (ISNULL(@filtro_Effettuato, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_Effettuato AS NVARCHAR(MAX)) + CAST(' T6 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T6.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',6' AS NVARCHAR(MAX))
			
		END
	
	-- 7 - IN ERRORE
	--
	IF (ISNULL(@filtro_InErrore, '') <> '')
		BEGIN
		
			SET @sql_string_from = @sql_string_from + CAST(',' AS NVARCHAR(MAX)) + CAST(@filtro_InErrore AS NVARCHAR(MAX)) + CAST(' T7 ' AS NVARCHAR(MAX))
		
			SET @sql_string_where = @sql_string_where + CAST(N' AND A.SYSTEM_ID = T7.TRANSFER_ID ' AS NVARCHAR(MAX))
			
			--SET @sql_string_filtroStati = @sql_string_filtroStati + CAST(N',7' AS NVARCHAR(MAX))
			
		END



	SET @sql_string_filtroStati = CAST(N'6,7' AS NVARCHAR(MAX))

	IF (ISNULL(@sql_string_filtroStati, '') <> '')
		BEGIN
			
			SET @sql_string_where = @sql_string_where + CAST(N' AND TS.TRANSFERSTATETYPE_ID IN (' AS NVARCHAR(MAX)) + CAST(@sql_string_filtroStati AS NVARCHAR(MAX)) + CAST(')' AS NVARCHAR(MAX))
			
		END

	SET @sql_string = 
		CAST(@sql_string_select AS NVARCHAR(MAX)) 
		+ CAST(@sql_string_from AS NVARCHAR(MAX)) 
		+ CAST(@sql_string_where AS NVARCHAR(MAX))
		+ CAST(@sql_string_filtroStatiEsecuzione AS NVARCHAR(MAX));
	
	PRINT @sql_string;
	
	EXECUTE sp_executesql @sql_string;

END
GO
