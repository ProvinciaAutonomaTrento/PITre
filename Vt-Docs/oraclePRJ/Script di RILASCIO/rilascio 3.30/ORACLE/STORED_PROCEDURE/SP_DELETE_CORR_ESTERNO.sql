CREATE OR REPLACE PROCEDURE SP_DELETE_CORR_ESTERNO (IDCorrGlobale IN NUMBER, liste IN NUMBER, IdPeople IN NUMBER, IdGruppo IN NUMBER, ReturnValue OUT NUMBER)  IS

/*
-------------------------------------------------------------------------------------------------------
SP per la Cancellazione corrispondente

Valori di ritorno gestiti:

0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
1: DISABILITAZIONE EFFETTUATA - il corrispondente ? presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato
2: CORRISPONDENTE NON RIMOSSO - il corrispondente ? presente nella lista di distribuzione e non posso rimuoverlo
3: ERRORE: la DELETE sulla dpa_corr_globali NON ? andata a buon fine
4: ERRORE: la DELETE sulla dpa_dett_globali NON ? andata a buon fine
5: ERRORE: l' UPDATE sulla dpa_corr_globali NON ? andata a buon fine
6: ERRORE: la DELETE sulla dpa_liste_distr NON ? andata a buon fine
7: DISABILITAZIONE EFFETTUATA - il corrispondente non presente nella DPA_DOC_ARRIVO_PAR, ma utilizzato in campi profilati, quindi non viene cancellato, ma storicizzato
-------------------------------------------------------------------------------------------------------

*/

countDoc number; -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
cha_tipo_urp VARCHAR2(1);
var_inLista VARCHAR2(1); -- valore 'N' (il corr non ? presente in nessuna lista di sistribuzione), 'Y' altrimenti
countLista number;
new_var_cod_rubrica1 varchar2 (128);
v_curdoc_docnumber  NUMBER;
v_curdoc_idtemplate NUMBER;
v_curdoc_idoggetto  NUMBER;
v_curfasc_idproject  NUMBER;
v_curfasc_idtemplate NUMBER;
v_curfasc_idoggetto  NUMBER;

BEGIN
    DECLARE
countProfilato number;
countProfDoc number;
countProfFasc number;
IdRuolo number;

    CURSOR cursor_doc
   IS
      select at.doc_number, at.id_template, at.id_oggetto 
      from dpa_associazione_templates at, dpa_oggetti_custom oc, dpa_tipo_oggetto dto 
      where at.valore_oggetto_db = to_char(IDCorrGlobale)
      and at.ID_OGGETTO = oc.SYSTEM_ID
      and oc.ID_TIPO_OGGETTO = dto.SYSTEM_ID
      and upper(dto.DESCRIZIONE) = 'CORRISPONDENTE';

CURSOR cursor_fasc
   IS
      select atf.id_project, atf.id_template, atf.id_oggetto 
      from dpa_ass_templates_fasc atf, dpa_oggetti_custom_fasc ocf, dpa_tipo_oggetto_fasc tof
      where atf.valore_oggetto_db = to_char(IdCorrGlobale)
      and atf.ID_OGGETTO = ocf.SYSTEM_ID
      and ocf.ID_TIPO_OGGETTO = tof.SYSTEM_ID
      and upper(tof.DESCRIZIONE) = 'CORRISPONDENTE';

    BEGIN

    select cha_tipo_urp INTO cha_tipo_urp from dpa_corr_globali where system_id = IDCorrGlobale;
    
    select system_id INTO IdRuolo from dpa_corr_globali where id_gruppo = IdGruppo;

    var_inLista := 'N'; -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

    SELECT count(SYSTEM_ID) into countLista FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

    IF (countLista > 0) THEN -- se il corrispondente ? contenuto nelle liste di distribuzione
    BEGIN
        IF (liste = 1) THEN
        --- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente ? in una lista di distibuzione, in caso affermativo non posso rimuoverlo
        BEGIN
        -- CASO 1.1 - Il corrispondente ? predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).
            ReturnValue := 2;
            RETURN;
            END;
        ELSE
        -- CASO 2 - Le liste di distribuzione NON SONO abilitate

            BEGIN
            var_inLista := 'Y';
            END;
        END IF;
    END;
    END IF;


-- Se la procedura va avanti, cio' significa che:
-- Le liste di distribuzione non sono abilitate (liste = 0), oppure sono abilitate (liste=1) ma il corrispondente che si tenta di rimuovere non ? contenuto in una lista

    SELECT count(ID_PROFILE) INTO countDoc  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST = IDCorrGlobale;

    IF (countDoc = 0) THEN
    -- CASO 3 -  il corrispondente non ? stato mai utilizzato come mitt/dest di protocolli
    BEGIN
        countProfilato := 0;    
    
        -- se il corrispondente non ? stato usato come mitt/dest ma ? stato usato nei campi profilati
        select count(system_id) into countProfDoc from dpa_associazione_templates 
        where valore_oggetto_db = to_char(IDCorrGlobale);
        
        select count(system_id) into countProfFasc from dpa_ass_templates_fasc
        where valore_oggetto_db = to_char(IdCorrGlobale);
        
        countProfilato := countProfDoc + countProfFasc;
        
        --dbms_output.put_line('countProfDoc ' || countProfDoc);
        --dbms_output.put_line('countProfFasc ' || countProfFasc);        
        
        IF (countProfilato > 0) THEN
        BEGIN
                -- storicizzo il corrispondente
            -- Ricavo il codice rubrica del corrispondente
            SELECT var_cod_rubrica
            INTO new_var_cod_rubrica1
            FROM dpa_corr_globali
            WHERE system_id = IDCorrGlobale;

            -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
            new_var_cod_rubrica1 := new_var_cod_rubrica1 || '_' || to_char(IDCorrGlobale);

            -- Storicizzo il corrispondente
            UPDATE DPA_CORR_GLOBALI
            SET DTA_FINE = SYSDATE(),
            var_cod_rubrica = new_var_cod_rubrica1,
            var_codice = new_var_cod_rubrica1,
            id_parent = null
            WHERE SYSTEM_ID = IDCorrGlobale;
            
            IF(countProfDoc > 0) THEN
            BEGIN
            
                OPEN cursor_doc;
                LOOP
                
                    FETCH cursor_doc
                        INTO v_curdoc_docnumber, v_curdoc_idtemplate, v_curdoc_idoggetto;

                    EXIT WHEN cursor_doc%NOTFOUND;

                    BEGIN
 
                        --dbms_output.put_line('v_curdoc_docnumber ' || v_curdoc_docnumber);
                        --dbms_output.put_line('v_curdoc_idtemplate ' || v_curdoc_idtemplate);        
                        --dbms_output.put_line('v_curdoc_idoggetto ' || v_curdoc_idoggetto);

                        INSERT INTO dpa_profil_sto
                            (systemid, id_template,
                            dta_modifica, id_profile, id_ogg_custom,
                            id_people, id_ruolo_in_uo, var_desc_modifica
                            )
                        VALUES (seq_dpa_profil_sto.NEXTVAL, v_curdoc_idtemplate,
                            SYSDATE, v_curdoc_docnumber, v_curdoc_idoggetto,
                            IdPeople, IdRuolo, 'Corrispondente storicizzato per eliminazione da rubrica'
                            );

                    END;

                END LOOP;

                CLOSE cursor_doc;

            
            END;
            END IF;
            
            IF(countProfFasc > 0) THEN
            BEGIN
            
                OPEN cursor_fasc;
                LOOP
                
                    FETCH cursor_fasc
                        INTO v_curfasc_idproject, v_curfasc_idtemplate, v_curfasc_idoggetto;

                    EXIT WHEN cursor_fasc%NOTFOUND;

                    BEGIN
            
                        --dbms_output.put_line('v_curfasc_idproject ' || v_curfasc_idproject);
                        --dbms_output.put_line('v_curfasc_idtemplate ' || v_curfasc_idtemplate);        
                        --dbms_output.put_line('v_curfasc_idoggetto ' || v_curfasc_idoggetto);

                        INSERT INTO dpa_profil_fasc_sto
                            (systemid, id_template,
                            dta_modifica, id_project, id_ogg_custom,
                            id_people, id_ruolo_in_uo, var_desc_modifica
                            )
                        VALUES (seq_dpa_profil_sto.NEXTVAL, v_curfasc_idtemplate,
                            SYSDATE, v_curfasc_idproject, v_curfasc_idoggetto,
                            IdPeople, IdRuolo, 'Corrispondente storicizzato per eliminazione da rubrica'
                            );

                    END;

                END LOOP;

                CLOSE cursor_fasc;
            
            END;
            END IF;
            
            ReturnValue:=7;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 7).
            
            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
                       
               RETURN;
            
                   END;
        ELSE
            BEGIN
                -- proseguo come prima
                -- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
                delete  FROM DPA_t_canale_corr where id_corr_globale=IDCorrGlobale;
                delete from dpa_dett_globali where id_corr_globali= IDCorrGlobale;
                
                DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = IDCorrGlobale;

                EXCEPTION
                WHEN OTHERS THEN
                ReturnValue:=3;-- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).

            END;
        END IF;
        
    END;

    IF (ReturnValue=3) THEN
        RETURN; --ESCO DALLA PROCEDURA
    ELSE
        BEGIN

        ReturnValue:=0;

        DELETE FROM DPA_T_CANALE_CORR WHERE  ID_CORR_GLOBALE = IDCorrGlobale;

        -- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poich? in fase di creazione di un ruolo
        -- non viene fatta la insert in tale tabella
        IF(cha_tipo_urp != 'R') THEN

        BEGIN
            -- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
            DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = IDCorrGlobale;

            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=4;-- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
        END;

            IF (ReturnValue=4) THEN
                RETURN; -- ESCO DALLA PROCEDURA
            ELSE
                ReturnValue:=0; -- CANCELLAZIONE ANDATA A BUON FINE
            END IF;
        END IF;

        IF (ReturnValue=0 AND liste = 0 AND var_inLista = 'Y')     THEN

        BEGIN
--se:
-- 1) sono andate bene le DELETE precedenti
-- 2) sono disabilitate le liste di distribuzione
-- 3) il corrispondente ? nella DPA_LISTE_DISTR

-- rimuovo il corrispondente dalla DPA_LISTE_DISTR
-- rimuovo il corrispondente dalla DPA_LISTE_DISTR
            DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=6;-- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).

        END;

        END IF;

    END;
    END IF;
    ELSE

-- CASO 4 -  il corrispondente ?  stato utilizzato come mitt/dest di protocolli
-- 4.1) disabilitazione del corrispondente
-- Il nuovo codice rubrica da attribuire al corrispondente storicizzato
        declare new_var_cod_rubrica varchar2 (128);
        BEGIN

-- Ricavo il codice rubrica del corrispondente
        SELECT var_cod_rubrica
        INTO new_var_cod_rubrica
        FROM dpa_corr_globali
        WHERE system_id = IDCorrGlobale;

-- Costruisco il codice rubrica da attribuire la corrispondente
-- storicizzato
        new_var_cod_rubrica := new_var_cod_rubrica || '_' || to_char(IDCorrGlobale);

-- Storicizzo il corrispondente
        UPDATE DPA_CORR_GLOBALI
        SET DTA_FINE = SYSDATE(),
        var_cod_rubrica = new_var_cod_rubrica,
        var_codice = new_var_cod_rubrica,
        id_parent = null
        WHERE SYSTEM_ID = IDCorrGlobale;


        EXCEPTION
        WHEN OTHERS THEN
        ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).

        END;
        IF(ReturnValue=5) THEN
            RETURN;
        ELSE
            ReturnValue:=1;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
        END IF;

    END IF;
    END;
END;

/
