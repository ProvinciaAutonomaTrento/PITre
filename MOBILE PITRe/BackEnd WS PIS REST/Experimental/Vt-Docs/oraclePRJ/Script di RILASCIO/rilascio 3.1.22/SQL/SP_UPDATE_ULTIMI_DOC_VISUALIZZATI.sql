IF OBJECT_ID ( 'SP_UPDATE_ULTIMI_DOC_VIS', 'P' ) IS NOT NULL   
    DROP PROCEDURE SP_UPDATE_ULTIMI_DOC_VIS;  
GO  
CREATE  
PROCEDURE             SP_UPDATE_ULTIMI_DOC_VIS (
@p_idpeople          INT,
@p_idgruppo          INT,
@p_idprofile         INT,
@p_idamm			INT)
AS
BEGIN
	DECLARE @numUltimiDoc      INT
	DECLARE @maxNumUltimiDoc   INT
	DECLARE @systemIdUltimaVis INT
	
	SET @systemIdUltimaVis = 0;
	SET @maxNumUltimiDoc = 0;

	BEGIN try
		SELECT @maxNumUltimiDoc = VAR_VALORE
		FROM DPA_CHIAVI_CONFIGURAZIONE
		WHERE var_codice =  'BE_NUM_ULTIMI_DOC_VISUALIZZATI' AND ID_AMM = @p_idamm
	END TRY
	BEGIN CATCH
			set @maxNumUltimiDoc =0
	END CATCH
	IF(@maxNumUltimiDoc != 0)
		--VERIFICO SE IL DOCUMENTO è GIA PRESENTE NELLA LISTA DEGLI ULTIMI DOCUMENTI VISUALIZZATI
		--SE è PRESENTE AGGIORNO LA DATA DI VISUALIZZAZIONE DEL DOCUMENTO
		BEGIN try
			SELECT @systemIdUltimaVis = SYSTEM_ID
			FROM DPA_ULTIMI_DOC_VISUALIZZATI
			WHERE ID_PEOPLE = @p_idpeople AND ID_GRUPPO = @p_idgruppo AND ID_PROFILE = @p_idprofile AND ID_AMM = @p_idamm
		END TRY
		BEGIN CATCH
			SET @systemIdUltimaVis =0
		END CATCH
		IF( @systemIdUltimaVis != 0 )
			BEGIN
				UPDATE DPA_ULTIMI_DOC_VISUALIZZATI
				SET DTA_VISUALIZZAZIONE = GETDATE()
				WHERE SYSTEM_ID = @systemIdUltimaVis
			END
		ELSE	--AGGIUNGO IL NUOVO RECORD E RIMUOVO NEL CASO IN CUI HO RAGGIUNTO IL NUMERO MASSIMO
			BEGIN
				INSERT INTO DPA_ULTIMI_DOC_VISUALIZZATI 
				(ID_PEOPLE, 
				ID_GRUPPO,
				ID_AMM,
				ID_PROFILE,
				DTA_VISUALIZZAZIONE)
				VALUES
				(@p_idpeople,
				@p_idgruppo,
				@p_idamm,
				@p_idprofile,
				GETDATE()
				)

				SELECT @numUltimiDoc = COUNT(SYSTEM_ID)
				FROM DPA_ULTIMI_DOC_VISUALIZZATI
				WHERE  ID_PEOPLE = @p_idpeople AND ID_GRUPPO = @p_idgruppo AND ID_AMM = @p_idamm;
					
				IF(@numUltimiDoc > @maxNumUltimiDoc)
					DELETE FROM DPA_ULTIMI_DOC_VISUALIZZATI
					WHERE ID_PEOPLE = @p_idpeople AND ID_GRUPPO = @p_idgruppo AND ID_AMM = @p_idamm 
						AND DTA_VISUALIZZAZIONE = ( SELECT MIN(DTA_VISUALIZZAZIONE)
										FROM DPA_ULTIMI_DOC_VISUALIZZATI
										WHERE ID_PEOPLE = @p_idpeople AND ID_GRUPPO = @p_idgruppo AND ID_AMM = @p_idamm
										)
			END
END