
ALTER PROCEDURE [DOCSADM].[SP_DELETE_CORR_ESTERNO] 
       @IDCorrGlobale INT, 
       @liste INT, 
       @IdPeople INT, 
       @IdGruppo INT, 
       @ReturnValue INT OUTPUT
       
AS
BEGIN

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

DECLARE @countDoc INT -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
DECLARE @cha_tipo_urp VARCHAR(1)
DECLARE @var_inLista VARCHAR(1) -- valore 'N' (il corr non ? presente in nessuna lista di sistribuzione), 'Y' altrimenti
DECLARE @countLista INT
DECLARE @new_var_cod_rubrica1 varchar(128)
DECLARE @v_curdoc_docnumber  INT
DECLARE @v_curdoc_idtemplate INT
DECLARE @v_curdoc_idoggetto  INT
DECLARE @v_curfasc_idproject  INT
DECLARE @v_curfasc_idtemplate INT
DECLARE @v_curfasc_idoggetto  INT


       DECLARE @countProfilato INT
       DECLARE @countProfDoc INT
       DECLARE @countProfFasc INT
       DECLARE @IdRuolo INT

    DECLARE cursor_doc CURSOR FOR 
             select at.doc_number, at.id_template, at.id_oggetto 
             from dpa_associazione_templates at, dpa_oggetti_custom oc, dpa_tipo_oggetto dto 
             where at.valore_oggetto_db = @IDCorrGlobale --to_char(IDCorrGlobale)
             and at.ID_OGGETTO = oc.SYSTEM_ID
             and oc.ID_TIPO_OGGETTO = dto.SYSTEM_ID
             and upper(dto.DESCRIZIONE) = 'CORRISPONDENTE';

       DECLARE cursor_fasc CURSOR FOR
             select atf.id_project, atf.id_template, atf.id_oggetto 
             from dpa_ass_templates_fasc atf, dpa_oggetti_custom_fasc ocf, dpa_tipo_oggetto_fasc tof
             where atf.valore_oggetto_db = @IDCorrGlobale --to_char(IdCorrGlobale)
             and atf.ID_OGGETTO = ocf.SYSTEM_ID
             and ocf.ID_TIPO_OGGETTO = tof.SYSTEM_ID
             and upper(tof.DESCRIZIONE) = 'CORRISPONDENTE';


    select @cha_tipo_urp = cha_tipo_urp from dpa_corr_globali where system_id = @IDCorrGlobale;
    
    select @IdRuolo = system_id from dpa_corr_globali where id_gruppo = @IdGruppo;

    SET @var_inLista = 'N'; -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

    SELECT @countLista = count(SYSTEM_ID) FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale;

    IF (@countLista > 0) -- se il corrispondente ? contenuto nelle liste di distribuzione
    BEGIN
        IF (@liste = 1)
        --- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente ? in una lista di distibuzione, in caso affermativo non posso rimuoverlo
        BEGIN
        -- CASO 1.1 - Il corrispondente ? predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).
            SET @ReturnValue = 2
            RETURN
        END
        ELSE
        -- CASO 2 - Le liste di distribuzione NON SONO abilitate
        BEGIN
                    SET @var_inLista = 'Y';
        END
    END


-- Se la procedura va avanti, cio' significa che:
-- Le liste di distribuzione non sono abilitate (liste = 0), oppure sono abilitate (liste=1) ma il corrispondente che si tenta di rimuovere non ? contenuto in una lista

    SELECT @countDoc = count(ID_PROFILE) FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST = @IDCorrGlobale;
print 'PRIMO BLOCCO '
    IF (@countDoc = 0)
    -- CASO 3 -  il corrispondente non ? stato mai utilizzato come mitt/dest di protocolli
    BEGIN
        SET @countProfilato = 0;    
    
        -- se il corrispondente non ? stato usato come mitt/dest ma ? stato usato nei campi profilati
        -- correzione SAB 10-02-2014
        select @countProfDoc = count(system_id) from dpa_associazione_templates 
        where valore_oggetto_db = convert(varchar,@IDCorrGlobale);
        
        select @countProfFasc = count(system_id) from dpa_ass_templates_fasc
        where valore_oggetto_db = convert(varchar,@IdCorrGlobale);
        
        SET @countProfilato = @countProfDoc + @countProfFasc;
       
        --dbms_output.put_line('countProfDoc ' || countProfDoc);
        --dbms_output.put_line('countProfFasc ' || countProfFasc);        
        
        IF (@countProfilato > 0)
        BEGIN
            -- storicizzo il corrispondente
            -- Ricavo il codice rubrica del corrispondente
            SELECT @new_var_cod_rubrica1 = var_cod_rubrica
            FROM dpa_corr_globali
            WHERE system_id = @IDCorrGlobale;

            -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
            SET @new_var_cod_rubrica1 = @new_var_cod_rubrica1 + '_' + CAST(@IDCorrGlobale as VARCHAR)

            -- Storicizzo il corrispondente
            UPDATE DPA_CORR_GLOBALI
            SET DTA_FINE = GETDATE(),
            var_cod_rubrica = @new_var_cod_rubrica1,
            var_codice = @new_var_cod_rubrica1,
            id_parent = null
            WHERE SYSTEM_ID = @IDCorrGlobale and CHA_TIPO_IE <> 'I';;
            
            
            IF(@countProfDoc > 0)
            BEGIN
            
                          OPEN cursor_doc
                          FETCH NEXT FROM cursor_doc
                          INTO @v_curdoc_docnumber, @v_curdoc_idtemplate, @v_curdoc_idoggetto
                          
                          WHILE @@FETCH_STATUS = 0
                          BEGIN 

                                 -- ATTENZIONE: questa INSERT prevede una colonna IDENTITY ma sul Deposito le IDENTITY sono
                                 --             stato disabilitate tutte
                                 --
                                 INSERT INTO dpa_profil_sto
                                       (
                                       id_template,
                                       dta_modifica, 
                                       id_profile, 
                                       id_ogg_custom,
                                       id_people, 
                                       id_ruolo_in_uo, 
                                       var_desc_modifica
                                       )
                                 VALUES 
                                       (
                                       @v_curdoc_idtemplate,
                                       GETDATE(), 
                                       @v_curdoc_docnumber, 
                                       @v_curdoc_idoggetto,
                                       @IdPeople, 
                                       @IdRuolo, 
                                       'Corrispondente storicizzato per eliminazione da rubrica'
                                        );
                                       
                                 FETCH NEXT FROM cursor_doc
                                 INTO @v_curdoc_docnumber, @v_curdoc_idtemplate, @v_curdoc_idoggetto

                END
                CLOSE cursor_doc
                          DEALLOCATE cursor_doc

            END
            
            IF(@countProfFasc > 0)
            BEGIN
            
                          OPEN cursor_fasc
                          FETCH NEXT FROM cursor_fasc
                          INTO @v_curfasc_idproject, @v_curfasc_idtemplate, @v_curfasc_idoggetto
                          
                          WHILE @@FETCH_STATUS = 0
                          BEGIN 
            

                                 -- ATTENZIONE: questa INSERT prevede una colonna IDENTITY ma sul Deposito le IDENTITY sono
                                 --             stato disabilitate tutte
                                 --
                    INSERT INTO dpa_profil_fasc_sto
                        (
                        id_template,
                        dta_modifica, 
                        id_project, 
                        id_ogg_custom,
                        id_people, 
                        id_ruolo_in_uo, 
                        var_desc_modifica
                        )
                    VALUES 
                                       (
                                       @v_curfasc_idtemplate,
                        GETDATE(), 
                        @v_curfasc_idproject, 
                        @v_curfasc_idoggetto,
                        @IdPeople, 
                        @IdRuolo, 
                        'Corrispondente storicizzato per eliminazione da rubrica'
                        );
                                       
                                 FETCH NEXT FROM cursor_fasc
                                 INTO @v_curfasc_idproject, @v_curfasc_idtemplate, @v_curfasc_idoggetto

                END
                CLOSE cursor_fasc
                          DEALLOCATE cursor_fasc

            END
            
            SET @ReturnValue = 7;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 7).
            
            IF (@@ERROR <> 0)
                    BEGIN
                          SET @ReturnValue = 5 -- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
                          RETURN
                    END
            
        END
        ELSE
            BEGIN
                -- proseguo come prima
                -- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
                delete FROM DPA_t_canale_corr where id_corr_globale = @IDCorrGlobale
                delete from dpa_dett_globali where id_corr_globali = @IDCorrGlobale
                
                DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = @IDCorrGlobale and CHA_TIPO_IE <> 'I';

                          IF (@@ERROR <> 0)
                          BEGIN
                                 SET @ReturnValue = 3 -- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).
                                 RETURN
                          END

            END
        
    IF (@ReturnValue = 3)
        RETURN --ESCO DALLA PROCEDURA
    ELSE
        BEGIN

                    SET @ReturnValue = 0

                    DELETE FROM DPA_T_CANALE_CORR WHERE  ID_CORR_GLOBALE = @IDCorrGlobale

                    -- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poich? in fase di creazione di un ruolo
                    -- non viene fatta la insert in tale tabella
                    IF(@cha_tipo_urp != 'R')

                    BEGIN
                          -- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
                          DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = @IDCorrGlobale

                          IF (@@ERROR <> 0)
                          BEGIN
                                 SET @ReturnValue = 4 -- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
                                 RETURN
                          END
                    END

                          IF (@ReturnValue=4)
                                 RETURN -- ESCO DALLA PROCEDURA
                          ELSE
                                 SET @ReturnValue = 0 -- CANCELLAZIONE ANDATA A BUON FINE

                    IF (@ReturnValue=0 AND @liste = 0 AND @var_inLista = 'Y')

                    BEGIN
       --se:
       -- 1) sono andate bene le DELETE precedenti
       -- 2) sono disabilitate le liste di distribuzione
       -- 3) il corrispondente ? nella DPA_LISTE_DISTR

       -- rimuovo il corrispondente dalla DPA_LISTE_DISTR
       -- rimuovo il corrispondente dalla DPA_LISTE_DISTR
                          DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = @IDCorrGlobale

                          IF (@@ERROR <> 0)
                          BEGIN
                                 SET @ReturnValue = 6 -- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).
                                 RETURN
                          END

                    END
             END
             
    END

    ELSE

-- CASO 4 -  il corrispondente ?  stato utilizzato come mitt/dest di protocolli
-- 4.1) disabilitazione del corrispondente
-- Il nuovo codice rubrica da attribuire al corrispondente storicizzato
        BEGIN
        print 'SECONDO BLOCCO '
        declare @new_var_cod_rubrica2 varchar(128);

-- Ricavo il codice rubrica del corrispondente
        SELECT @new_var_cod_rubrica2 = var_cod_rubrica
        --INTO new_var_cod_rubrica2
        FROM dpa_corr_globali
        WHERE system_id = @IDCorrGlobale;

-- Costruisco il codice rubrica da attribuire la corrispondente
-- storicizzato
        SET @new_var_cod_rubrica2 = @new_var_cod_rubrica2 + '_' + CAST(@IDCorrGlobale as varchar)

print 'SECONDO BLOCCO '
-- Storicizzo il corrispondente
        UPDATE DPA_CORR_GLOBALI
        SET DTA_FINE = GETDATE(),
        var_cod_rubrica = @new_var_cod_rubrica2,
        var_codice = @new_var_cod_rubrica2,
        id_parent = null
        WHERE SYSTEM_ID = @IDCorrGlobale and CHA_TIPO_IE <> 'I';


             IF (@@ERROR <> 0)
             BEGIN
                    SET @ReturnValue = 5 -- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
                    RETURN
             END

        IF(@ReturnValue = 5)
            RETURN
        ELSE
            SET @ReturnValue = 1   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
END

END




