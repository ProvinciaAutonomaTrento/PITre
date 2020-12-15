create or replace FUNCTION EsisteDestinatarioMaiTrasmesso (p_idProfile INT )
   RETURN INT IS retValue   INT;
BEGIN
	DECLARE 
	idDestinatario INT;
	tipoURP VARCHAR(1);
	countTrasm   INT;
	CURSOR cursor_destinatari
		IS
			SELECT G.SYSTEM_ID, G.CHA_TIPO_URP
			FROM DPA_DOC_ARRIVO_PAR D JOIN DPA_CORR_GLOBALI G ON D.ID_MITT_DEST = G.SYSTEM_ID
			WHERE D.ID_PROFILE = p_idProfile AND D.CHA_TIPO_MITT_DEST IN ('D', 'C', 'F')
			AND G.CHA_TIPO_IE = 'I';

	BEGIN
		retValue := 0;
		OPEN cursor_destinatari;
		LOOP
		FETCH cursor_destinatari INTO idDestinatario, tipoURP;
		EXIT WHEN cursor_destinatari%NOTFOUND; 
		
			IF(tipoURP = 'U')
			THEN
				BEGIN
					SELECT SYSTEM_ID INTO idDestinatario
					FROM DPA_CORR_GLOBALI
					WHERE ID_UO = idDestinatario AND DTA_FINE IS NULL AND CHA_TIPO_URP = 'R' AND CHA_TIPO_IE = 'I'
					AND	CHA_RIFERIMENTO = '1' AND ROWNUM = 1;			
					EXCEPTION
					  WHEN OTHERS  THEN
							idDestinatario := 0;
				END;
			END IF;
			IF(idDestinatario > 0)
			THEN
				BEGIN
				
					BEGIN
						SELECT COUNT(S.SYSTEM_ID) INTO countTrasm
						FROM DPA_TRASMISSIONE T JOIN DPA_TRASM_SINGOLA S ON T.SYSTEM_ID = S.ID_TRASMISSIONE
						WHERE T.ID_PROFILE = p_idProfile AND S.ID_CORR_GLOBALE = idDestinatario AND T.DTA_INVIO IS NOT NULL;
						EXCEPTION
						  WHEN OTHERS  THEN
								countTrasm := 0;
					END;
					
					IF(countTrasm = 0)
					THEN
						BEGIN
							retValue := 1;
							EXIT;
						END;
					END IF;
				END;
			ELSE
				BEGIN
					retValue := 1;
					EXIT;
				END;
			END IF;
		END LOOP;
		CLOSE cursor_destinatari;
	END;
	
	RETURN retValue;

END; 