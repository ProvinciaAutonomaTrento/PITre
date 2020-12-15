CREATE OR REPLACE PROCEDURE @db_user.I_Smistamento_Smistadoc_R_2(
        IDPeopleMittente IN NUMBER,
        IDCorrGlobaleRuoloMittente IN NUMBER,
        IDGruppoMittente IN NUMBER,
        IDAmministrazioneMittente IN NUMBER,
        IDCorrGlobaleDestinatario IN NUMBER,
        IDDocumento IN NUMBER,
        TipoDiritto IN CHAR,
        Rights IN NUMBER,
        IDRagioneTrasm IN NUMBER,
        ReturnValue OUT NUMBER)    IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni a ruolo nella protocollazione semplificata
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-------------------------------------------------------------------------------------------------------
*/

IdentityTrasm NUMBER := NULL;
IdentityTrasmSing NUMBER := NULL;

ExistAccessRights CHAR(1) := 'Y';
AccessRights NUMBER:= NULL;
accessRightsValue NUMBER := NULL;

IDUtente NUMBER;
recordCorrente NUMBER;

IDGroups NUMBER := NULL;
IDGruppo NUMBER;

/* Prende gli utenti del ruolo */
CURSOR cursor_IDUtenti (ID_CORR_GLOB_RUOLO NUMBER) IS
        SELECT     P.SYSTEM_ID
        FROM     GROUPS G,
        PEOPLEGROUPS PG,
        PEOPLE P,
        DPA_CORR_GLOBALI CG
        WHERE     PG.GROUPS_SYSTEM_ID=G.SYSTEM_ID AND
        PG.PEOPLE_SYSTEM_ID=P.SYSTEM_ID AND
        G.SYSTEM_ID = (SELECT A.ID_GRUPPO FROM DPA_CORR_GLOBALI A WHERE A.SYSTEM_ID = ID_CORR_GLOB_RUOLO) AND
        P.DISABLED NOT IN ('Y') AND
        P.SYSTEM_ID=CG.ID_PEOPLE
        AND CG.CHA_TIPO_URP != 'L'
        AND CG.DTA_FINE IS NULL
        AND PG.DTA_FINE IS NULL;

BEGIN

    BEGIN
        SELECT seq.NEXTVAL INTO IdentityTrasm FROM dual;
    END;

    BEGIN
        SELECT seq.NEXTVAL INTO IdentityTrasmSing FROM dual;
    END;

    BEGIN
        /* Inserimento in tabella DPA_TRASMISSIONE */
        INSERT INTO DPA_TRASMISSIONE
        (
        SYSTEM_ID,
        ID_RUOLO_IN_UO,
        ID_PEOPLE,
        CHA_TIPO_OGGETTO,
        ID_PROFILE,
        ID_PROJECT,
        DTA_INVIO,
        VAR_NOTE_GENERALI
        )
        VALUES
        (
        IdentityTrasm,
        IDCorrGlobaleRuoloMittente,
        IDPeopleMittente,
        'D',
        IDDocumento,
        NULL,
        SYSDATE(),
        NULL
        );
    END;

    BEGIN
        INSERT INTO DPA_TRASM_SINGOLA
        (
        SYSTEM_ID,
        ID_RAGIONE,
        ID_TRASMISSIONE,
        CHA_TIPO_DEST,
        ID_CORR_GLOBALE,
        VAR_NOTE_SING,
        CHA_TIPO_TRASM,
        DTA_SCADENZA,
        ID_TRASM_UTENTE
        )
        VALUES
        (
        IdentityTrasmSing,
        IDRagioneTrasm,
        IdentityTrasm,
        'R',
        IDCorrGlobaleDestinatario,
        NULL,
        'S',
        NULL,
        NULL
        );
    END;

    BEGIN
    
        OPEN cursor_IDUtenti(IDCorrGlobaleDestinatario);
        
        LOOP
        
            FETCH cursor_IDUtenti INTO recordCorrente;
            EXIT WHEN cursor_IDUtenti%NOTFOUND;
            IDUtente := recordCorrente;

            BEGIN
                -- Inserimento in tabella DPA_TRASM_UTENTE
                INSERT INTO DPA_TRASM_UTENTE
                (
                SYSTEM_ID,
                ID_TRASM_SINGOLA,
                ID_PEOPLE,
                DTA_VISTA,
                DTA_ACCETTATA,
                DTA_RIFIUTATA,
                DTA_RISPOSTA,
                CHA_VISTA,
                CHA_ACCETTATA,
                CHA_RIFIUTATA,
                VAR_NOTE_ACC,
                VAR_NOTE_RIF,
                CHA_VALIDA,
                ID_TRASM_RISP_SING
                )
                VALUES
                (
                seq.NEXTVAL,
                IdentityTrasmSing,
                IDUtente,
                NULL,
                NULL,
                NULL,
                NULL,
                '0',
                '0',
                '0',
                NULL,
                NULL,
                '1',
                NULL
                );
            END;
            
        END LOOP;
        
        CLOSE cursor_IDUtenti;

    END;

    -- Verifica se non vi sia gia una trasmissione per il documento:
    -- - se presente, si distinguono 2 casi:
    --    1) se ACCESSRIGHT < Rights
    --       viene fatto un'aggiornamento impostandone il valore a Rights
    --    2) altrimenti non fa nulla
    -- - se non presente viene fatta in ogni caso la insert con
    --   valore di ACCESSRIGHT = Rights
    BEGIN

        --per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio.!!!
        update dpa_trasmissione set dta_invio=sysdate() where system_id=IdentityTrasm;

        SELECT A.ID_GRUPPO INTO IDGroups
        FROM DPA_CORR_GLOBALI A
        WHERE A.SYSTEM_ID = IDCorrGlobaleDestinatario;
        
    END;

    IDGruppo := IDGroups;

    BEGIN
        SELECT ACCESSRIGHTS INTO AccessRights FROM (
        SELECT   ACCESSRIGHTS
        FROM     SECURITY
        WHERE    THING = IDDocumento
        AND      PERSONORGROUP = IDGruppo
        ) WHERE ROWNUM = 1;
        
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
            ExistAccessRights := 'N';
        
    END;

    IF ExistAccessRights = 'Y' THEN

        accessRightsValue := AccessRights;

        IF accessRightsValue < Rights THEN
        
            BEGIN
                /* aggiornamento a Rights */
                UPDATE     SECURITY
                SET     ACCESSRIGHTS = Rights
                WHERE     THING = IDDocumento
                AND
                PERSONORGROUP = IDGruppo
                AND ACCESSRIGHTS = accessRightsValue;
                
                EXCEPTION
                    WHEN DUP_VAL_ON_INDEX THEN
                    NULL;
            END;
        
        END IF;
        
    ELSE
    
        BEGIN
            /* inserimento a Rights */
            INSERT INTO SECURITY
            (
            THING,
            PERSONORGROUP,
            ACCESSRIGHTS,
            ID_GRUPPO_TRASM,
            CHA_TIPO_DIRITTO
            )
            VALUES
            (
            IDDocumento,
            IDGruppo,
            Rights,
            IDGruppoMittente,
            TipoDiritto
            );
                
            EXCEPTION
                WHEN DUP_VAL_ON_INDEX THEN
                NULL;
        END;
        
    END IF;

    ReturnValue := 0;

END;
/
