create or replace FUNCTION GetIdDestTrasmDocInUo (
	p_id_uo VARCHAR,
	p_docnumber VARCHAR
)
RETURN clob IS p_id_destinatari clob;
BEGIN
	DECLARE
	v_id_corr_globali int;
	v_id_gruppo int;
	v_temp_id_dest clob;
	BEGIN
		DECLARE CURSOR cur_ruoli
		IS
			SELECT DISTINCT    CG.SYSTEM_ID, CG.ID_GRUPPO
			FROM DPA_CORR_GLOBALI CG
			WHERE  CG.ID_UO = p_id_uo AND
			CG.CHA_TIPO_URP='R' AND
			CG.DTA_FINE IS NULL;

			BEGIN
				p_id_destinatari := '';
				OPEN cur_ruoli;
				LOOP
				FETCH cur_ruoli INTO v_id_corr_globali, v_id_gruppo;
				EXIT WHEN cur_ruoli%NOTFOUND;
					v_temp_id_dest := '';
					SELECT rtrim(xmlagg(XMLELEMENT(e, S.ID_CORR_GLOBALE || '_' || U.ID_PEOPLE, ';').EXTRACT('//text()')).GetClobVal(), ',') --LISTAGG(S.ID_CORR_GLOBALE || '_' || U.ID_PEOPLE, ';') WITHIN GROUP (ORDER BY S.ID_CORR_GLOBALE) 
                    INTO v_temp_id_dest
                    FROM DPA_TRASMISSIONE T JOIN DPA_TRASM_SINGOLA S ON T.SYSTEM_ID = S.ID_TRASMISSIONE
                    JOIN DPA_TRASM_UTENTE U ON S.SYSTEM_ID = U.ID_TRASM_SINGOLA
                    WHERE T.ID_PROFILE = p_docnumber AND 
                    (S.ID_CORR_GLOBALE = v_id_corr_globali 
                    OR EXISTS (SELECT 'X' FROM PEOPLEGROUPS 
                    WHERE GROUPS_SYSTEM_ID = v_id_gruppo AND DTA_FINE IS NULL AND PEOPLE_SYSTEM_ID IN 
                    (SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = S.ID_CORR_GLOBALE) ));

					IF( v_temp_id_dest IS NOT NULL)
					THEN
						p_id_destinatari := v_temp_id_dest || ';' || p_id_destinatari;
					END IF;

				END LOOP;
				CLOSE cur_ruoli;
			END;
		END;
		RETURN p_id_destinatari;
END;
