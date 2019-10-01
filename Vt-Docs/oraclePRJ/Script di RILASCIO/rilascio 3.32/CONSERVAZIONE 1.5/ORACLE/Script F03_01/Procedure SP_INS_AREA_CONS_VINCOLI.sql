CREATE OR REPLACE PROCEDURE sp_ins_area_cons_vincoli (
   p_idamm                   NUMBER,
   p_idconservazione         NUMBER,
   p_idpeople                NUMBER,
   p_idprofile               NUMBER,
   p_idproject               NUMBER,
   p_codfasc                 VARCHAR,
   p_oggetto                 VARCHAR,
   p_tipodoc                 CHAR,
   p_idgruppo                NUMBER,
   p_idregistro              NUMBER,
   p_docnumber               NUMBER,
   p_userid                  VARCHAR,
   p_tipooggetto             CHAR,
   p_tipoatto                VARCHAR,
   p_idpolicy                NUMBER,
   --p_dimIstanzaViolato         BOOLEAN,
   --p_numDocIstanzaViolato     BOOLEAN,
   p_dimIstanzaViolato         NUMBER,
   p_numDocIstanzaViolato     NUMBER,
   --Aggiunti
   p_vincoloDim                 NUMBER,    -- valore massimo BYTE dimensione istanza comprensivo di tolleranza
   p_vincoloNumDoc             NUMBER,    -- numero di documenti massimi all'interno di un'istanza
   p_sizeItem                 NUMBER,
   --End Aggiunti
   p_result            OUT   NUMBER
)
IS
   idruoloinuo   NUMBER := 0;
   id_cons_1     NUMBER := 0;
   id_cons_2     NUMBER := 0;
   res           NUMBER := 0;
   dimIstCorr     NUMBER := 0; -- Dimensione istanza corrente o predefinita
   dimIst        NUMBER := 0;
   dimMaxByte     NUMBER := 0;
   numDocIstCorr NUMBER := 0; -- Numero documenti presenti nell'istanza corrente o predefinita
   numDocIst      NUMBER := 0;
   --dimIstanzaViolato         BOOLEAN := p_dimIstanzaViolato;
   --numDocIstanzaViolato     BOOLEAN := p_numDocIstanzaViolato;
   dimIstanzaViolato         BOOLEAN := FALSE;
   numDocIstanzaViolato     BOOLEAN := FALSE;
   --Aggiunta Modifica per istanza preferita spacchettata
   isIstanzaPrefered	CHAR := '0';
   
   
BEGIN
   BEGIN
        IF(p_dimIstanzaViolato = 1)
        THEN 
            dimIstanzaViolato := TRUE;
        ELSE
            dimIstanzaViolato := FALSE;
        END IF;
        
        IF(p_numDocIstanzaViolato = 1)
        THEN
            numDocIstanzaViolato := TRUE;
        ELSE
            numDocIstanzaViolato := FALSE;
        END IF;
   
      SELECT seq_conservazione.NEXTVAL
        INTO id_cons_1
        FROM DUAL;

      SELECT SEQ_DPA_ITEMS_CONSERVAZIONE.NEXTVAL
        INTO id_cons_2
        FROM DUAL;

      SELECT dpa_corr_globali.system_id
        INTO idruoloinuo
        FROM dpa_corr_globali
       WHERE dpa_corr_globali.id_gruppo = p_idgruppo;

      IF (p_idconservazione IS NULL)
      THEN
         BEGIN
            -- Reperimento istanza di conservazione Preferita
            SELECT dpa_area_conservazione.system_id
                       INTO res
                       FROM dpa_area_conservazione
                      WHERE dpa_area_conservazione.id_people = p_idpeople
                        AND dpa_area_conservazione.id_ruolo_in_uo =
                                                                   idruoloinuo
                        AND dpa_area_conservazione.cha_stato = 'N'
                        AND dpa_area_conservazione.is_preferred ='1'
                        order by dpa_area_conservazione.system_id desc;
                        -- Condizione per determinare le policy Manuali
                        -- AND id_policy is null
         EXCEPTION
            WHEN OTHERS
            THEN
               res := 0;
         END;
      ELSE
         BEGIN
            res := p_idconservazione;
         END;
      END IF;

      
      IF (res is null or res=0)
      THEN
         BEGIN
            -- Reperimento istanza di conservazione corrente - Non è presente la preferita
            SELECT sys INTO res 
            FROM (SELECT dpa_area_conservazione.system_id as sys
                  FROM dpa_area_conservazione
                  WHERE dpa_area_conservazione.id_people = p_idpeople
                  AND dpa_area_conservazione.id_ruolo_in_uo = idruoloinuo
                  AND dpa_area_conservazione.cha_stato = 'N' 
                  -- Condizione per determinare le policy Manuali
                  --AND id_policy is null
                  order by dpa_area_conservazione.system_id desc) 
            WHERE rownum=1;
         EXCEPTION
            WHEN OTHERS
            THEN
               res := 0;
         END;
      END IF;
      
    -- In res può esserci l'id dell'istanza di conservazione passata in input, 
    -- oppure l'id dell'stanza di conservazione preferita
    -- oppure l'id dell'istanza di conservazione per la coppia utente-ruolo in stato Nuovo = N
      IF (res > 0)
      THEN
        -- CORPO THEN
         BEGIN
            -- Ho recuperato un'istanza di conservazione Predefinita o Corrente:
            -- Verifico se tale istanza rispetta i vincoli:
            
            -- Calcolo DIMENSIONE CORRENTE istanza di conservazione
            SELECT SUM(size_item)
            INTO dimIst
            FROM dpa_items_conservazione 
            WHERE id_conservazione = res;
            
            IF(dimIst is null)
            THEN
                dimIst := 0;
            END IF;
            
            dimIstCorr:= dimIst + p_sizeItem;
            
            --dimMaxByte := p_vincoloDim * 1024 * 1024; -- Era in MB, è stata passata direttamente in Byte
            dimMaxByte := p_vincoloDim;
            
            -- Verifico la validità del vincolo
            IF(dimIstCorr > dimMaxByte)
            THEN 
                --p_dimIstanzaViolato := TRUE;
                dimIstanzaViolato := TRUE;
            ELSE
                dimIstanzaViolato := FALSE;
            END IF;
            
            
            -- Calcolo NUMERO DI DOCUMENTI corrente istanza di conservazione
            SELECT COUNT(system_id) 
            INTO numDocIst
            FROM dpa_items_conservazione 
            WHERE id_conservazione = res;
            
            numDocIstCorr:= numDocIst + 1;
            
            -- Verifico la validità del vincolo
            IF(numDocIstCorr > p_vincoloNumDoc)
            THEN 
                --p_numDocIstanzaViolato := TRUE;
                numDocIstanzaViolato := TRUE;
            ELSE
                numDocIstanzaViolato := FALSE;
            END IF;
            
			-- Verifico se istanza è preferred
			SELECT is_preferred 
            INTO isIstanzaPrefered
            FROM dpa_area_conservazione 
            WHERE system_id = res;
         
            -- VINCOLO SULLA DIMENSIONE ISTANZE VIOLATO
            --IF(p_dimIstanzaViolato)
            IF(dimIstanzaViolato)
            THEN
                BEGIN
                    -- VINCOLO NUMERO DOCUMENTI VIOLATO
                    --IF (p_numDocIstanzaViolato)
                    IF (numDocIstanzaViolato)
                    THEN
                        BEGIN
							-- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
                            
                            SELECT seq_conservazione.NEXTVAL
                            INTO id_cons_1
                            FROM DUAL;
                            
                            SELECT SEQ_DPA_ITEMS_CONSERVAZIONE.NEXTVAL
                            INTO id_cons_2
                            FROM DUAL;
                            
							-- Controllo se istanza è predefinita
                            IF(isIstanzaPrefered = '1')
							THEN 
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET IS_PREFERRED = NULL
								WHERE SYSTEM_ID = res;
								
								-- è un'istanza predefinita
								INSERT INTO dpa_area_conservazione
								 (system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato, is_preferred
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB(),'1'
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							ELSE
								-- non è predefinita
								INSERT INTO dpa_area_conservazione
								 (system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB()
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							END IF;
							
                        END;
                    ELSE
                    -- VINCOLO NUMERO DOCUMENTI NON VIOLATO e DIMENSIONE VIOLATO
                        BEGIN
                            -- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
                            
                            SELECT seq_conservazione.NEXTVAL
                            INTO id_cons_1
                            FROM DUAL;
                            
                            SELECT SEQ_DPA_ITEMS_CONSERVAZIONE.NEXTVAL
                            INTO id_cons_2
                            FROM DUAL;
                            
							-- Controllo se istanza è predefinita
                            IF(isIstanzaPrefered = '1')
							THEN 
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET IS_PREFERRED = NULL
								WHERE SYSTEM_ID = res;
								
								-- è un'istanza predefinita
								INSERT INTO dpa_area_conservazione
								 (system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato, is_preferred
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB(),'1'
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							ELSE
								-- non è predefinita
								INSERT INTO dpa_area_conservazione
								(system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB()
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							END IF;
							
                        END;
                    END IF;
                END;
            ELSE
            -- VINCOLO DIMENSIONE ISTANZE NON VIOLATO
                BEGIN
                    -- VINCOLO NUMERO DOCUMENTI VIOLATO
                    --IF (p_numDocIstanzaViolato)
                    IF (numDocIstanzaViolato)
                    THEN
                        BEGIN
                            -- INSERIMENTO IN UNA NUOVA ISTANZA DI CONSERVAZIONE
                            
                            SELECT seq_conservazione.NEXTVAL
                            INTO id_cons_1
                            FROM DUAL;
                            
                            SELECT SEQ_DPA_ITEMS_CONSERVAZIONE.NEXTVAL
                            INTO id_cons_2
                            FROM DUAL;
                            
							-- Controllo se istanza è predefinita
                            IF(isIstanzaPrefered = '1')
							THEN 
								--Aggiorno la vecchia a non preferred
								UPDATE DPA_AREA_CONSERVAZIONE
								SET IS_PREFERRED = NULL
								WHERE SYSTEM_ID = res;
								
								-- è un'istanza predefinita
								INSERT INTO dpa_area_conservazione
								 (system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato, is_preferred
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB(),'1'
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							ELSE
								INSERT INTO dpa_area_conservazione
								 (system_id, id_amm, id_people, id_ruolo_in_uo,
								  cha_stato, data_apertura, user_id, id_gruppo, id_policy,
								  var_file_chiusura, var_file_chiusura_firmato
								  
								 )
								VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
								  'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
								  EMPTY_CLOB(),EMPTY_CLOB()
								 );

								
								INSERT INTO dpa_items_conservazione
								 (system_id, id_conservazione, id_profile, id_project,
								  cha_tipo_doc, var_oggetto, id_registro, data_ins,
								  cha_stato, var_xml_metadati, cod_fasc, docnumber,
								  cha_tipo_oggetto, var_tipo_atto
								 )
								VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
								  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
								  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
								  p_tipooggetto, p_tipoatto
								 );

								p_result := id_cons_2;
								COMMIT;
							END IF;
							
                        END;
                    ELSE
                    -- VINCOLO NUMERO DOCUMENTI NON VIOLATO E DIMENSIONE ISTANZE NON VIOLATO
                        BEGIN
                            -- INSERIMENTO NELLA VECCHIA ISTANZA DI CONSERVAZIONE
                            INSERT INTO dpa_items_conservazione
                                 (system_id, id_conservazione, id_profile, id_project,
                                  cha_tipo_doc, var_oggetto, id_registro, data_ins,
                                  cha_stato, var_xml_metadati, cod_fasc, docnumber,
                                  cha_tipo_oggetto, var_tipo_atto 
                                 )
                            VALUES (id_cons_2, res, p_idprofile, p_idproject,
                                  p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                                  'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                                  p_tipooggetto, p_tipoatto
                                 );

                            p_result := id_cons_2;
                            COMMIT;
                        END;
                    END IF;
                END;
            END IF; -- END CONTROLLO dimIstanzaViolato
         END; -- END THEN (ID ISTANZA CONSERVAZIONE PASSATA IN INPUT O RECUPERATA)
      ELSE
        -- CORPO ELSE - ID ISTANZA NON RECUPERABILE - (non c'è una istanza manuale predefinita o corrente)
        -- Inserisco in una nuova istanza di conservazione, quindi controllo la validità dei vincoli.
        
        BEGIN
            --IF(NOT p_numDocIstanzaViolato AND NOT p_dimIstanzaViolato)
            IF(NOT numDocIstanzaViolato AND NOT dimIstanzaViolato)
            THEN
                 INSERT INTO dpa_area_conservazione
                             (system_id, id_amm, id_people, id_ruolo_in_uo,
                              cha_stato, data_apertura, user_id, id_gruppo, id_policy,
                              var_file_chiusura, var_file_chiusura_firmato
                              
                             )
                      VALUES (id_cons_1, p_idamm, p_idpeople, idruoloinuo,
                              'N', SYSDATE, p_userid, p_idgruppo, p_idpolicy,
                              EMPTY_CLOB(),EMPTY_CLOB()
                             );

                 INSERT INTO dpa_items_conservazione
                             (system_id, id_conservazione, id_profile, id_project,
                              cha_tipo_doc, var_oggetto, id_registro, data_ins,
                              cha_stato, var_xml_metadati, cod_fasc, docnumber,
                              cha_tipo_oggetto, var_tipo_atto
                             )
                      VALUES (id_cons_2, id_cons_1, p_idprofile, p_idproject,
                              p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                              'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                              p_tipooggetto, p_tipoatto
                             );

                 p_result := id_cons_2;
                 COMMIT;
            --ELSE
                --p_result := -1;
            END IF;
        END;
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         p_result := -1;
         ROLLBACK;
   END;
END;
/
