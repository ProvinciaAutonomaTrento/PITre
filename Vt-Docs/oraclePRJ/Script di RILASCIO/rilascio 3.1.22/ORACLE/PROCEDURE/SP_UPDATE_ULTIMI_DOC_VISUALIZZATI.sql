create or replace
PROCEDURE             SP_UPDATE_ULTIMI_DOC_VIS (
p_idpeople          NUMBER,
p_idgruppo          NUMBER,
p_idprofile         NUMBER,
p_idamm				NUMBER
)
IS
BEGIN
	DECLARE
	numUltimiDoc      NUMBER;
	maxNumUltimiDoc   NUMBER;
	systemIdUltimaVis NUMBER;
	
	BEGIN
		systemIdUltimaVis := 0;
		maxNumUltimiDoc := 0;

		BEGIN			
			SELECT VAR_VALORE
			INTO maxNumUltimiDoc
			FROM DPA_CHIAVI_CONFIGURAZIONE
			WHERE var_codice =  'BE_NUM_ULTIMI_DOC_VISUALIZZATI' AND ID_AMM = P_idamm;
			EXCEPTION
			   WHEN NO_DATA_FOUND THEN
			   maxNumUltimiDoc :=0;
		END;
		IF(maxNumUltimiDoc != 0)
		THEN
			--VERIFICO SE IL DOCUMENTO è GIA PRESENTE NELLA LISTA DEGLI ULTIMI DOCUMENTI VISUALIZZATI
			--SE è PRESENTE AGGIORNO LA DATA DI VISUALIZZAZIONE DEL DOCUMENTO
			BEGIN
				SELECT SYSTEM_ID
				INTO systemIdUltimaVis
				FROM DPA_ULTIMI_DOC_VISUALIZZATI
				WHERE ID_PEOPLE = p_idpeople AND ID_GRUPPO = p_idgruppo AND ID_PROFILE = p_idprofile AND ID_AMM = P_idamm;
				EXCEPTION
				   WHEN NO_DATA_FOUND THEN
				   systemIdUltimaVis :=0;
			END;
			IF( systemIdUltimaVis != 0 )
			THEN
				BEGIN
					UPDATE DPA_ULTIMI_DOC_VISUALIZZATI
					SET DTA_VISUALIZZAZIONE = SYSDATE
					WHERE SYSTEM_ID = systemIdUltimaVis;
				END;
			ELSE	--AGGIUNGO IL NUOVO RECORD E RIMUOVO NEL CASO IN CUI HO RAGGIUNTO IL NUMERO MASSIMO
				BEGIN
					INSERT INTO DPA_ULTIMI_DOC_VISUALIZZATI 
					(SYSTEM_ID,
					ID_PEOPLE, 
					ID_GRUPPO,
					ID_AMM,
					ID_PROFILE,
					DTA_VISUALIZZAZIONE)
					VALUES
					(SEQ.NEXTVAL,
					p_idpeople,
					p_idgruppo,
					P_idamm,
					p_idprofile,
					SYSDATE
					);	

					SELECT COUNT(SYSTEM_ID)
					INTO numUltimiDoc
					FROM DPA_ULTIMI_DOC_VISUALIZZATI
					WHERE  ID_PEOPLE = p_idpeople AND ID_GRUPPO = p_idgruppo AND ID_AMM = P_idamm;
					
					IF(numUltimiDoc > maxNumUltimiDoc)
					THEN
						DELETE FROM DPA_ULTIMI_DOC_VISUALIZZATI
						WHERE ID_PEOPLE = p_idpeople AND ID_GRUPPO = p_idgruppo AND ID_AMM = P_idamm 
							AND DTA_VISUALIZZAZIONE = ( SELECT MIN(DTA_VISUALIZZAZIONE)
											FROM DPA_ULTIMI_DOC_VISUALIZZATI
											WHERE ID_PEOPLE = P_IDPEOPLE AND ID_GRUPPO = P_IDGRUPPO AND ID_AMM = P_IDAMM
											);
					END IF;
				END;
			END IF;
		END IF;
	END;
	COMMIT;
END	SP_UPDATE_ULTIMI_DOC_VIS;