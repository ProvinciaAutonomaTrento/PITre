--------------------------------------------------------
--  DDL for Procedure VIS_FASC_ANOMALA_ID_PROJECT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."VIS_FASC_ANOMALA_ID_PROJECT" (p_id_amm NUMBER, p_id_project in NUMBER, p_codice_atipicita out VARCHAR) IS

--DICHIARAZIONI
s_idg_security NUMBER;
s_ar_security NUMBER;
s_td_security VARCHAR(2);
s_vn_security VARCHAR(255);
s_idg_r_sup NUMBER;
n_id_gruppo NUMBER;

BEGIN

--Cursore sulla security per lo specifico fascicolo
DECLARE CURSOR c_idg_security IS 
SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec 
FROM security 
WHERE 
thing = p_id_project
AND
accessrights > 20;  
BEGIN OPEN c_idg_security;
LOOP FETCH c_idg_security INTO s_idg_security, s_ar_security, s_td_security, s_vn_security;
EXIT WHEN c_idg_security%NOTFOUND;

    --Gerachia ruolo proprietario del fascicolo
    IF(upper(s_td_security) = 'P') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('FASCICOLO : ' || p_id_project || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita,'AGRP'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGRP-';
            END IF;
        END;
        COMMIT; 
    END IF;

    
    --Gerarchia destinatario trasmissione
    IF(upper(s_td_security) = 'T') THEN
        DECLARE CURSOR ruoli_sup IS
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
                        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;                   
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || p_id_project || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);          
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGDT'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGDT-';
            END IF;
        END;
        COMMIT; 
    END IF;


    --Gerarchia ruolo destinatario di copia visibilit
    IF(upper(s_td_security) = 'A' AND upper(s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || p_id_project || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);   
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGCV'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGCV-';
            END IF;
        END;
        COMMIT;
    END IF;    


END LOOP;
CLOSE c_idg_security;
END; 

--Restituzione codice di atipicit
IF(p_codice_atipicita is null) THEN
    p_codice_atipicita := 'T';
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || p_id_project || ' - ' || p_codice_atipicita);
    update PROJECT set CHA_COD_T_A = p_codice_atipicita where SYSTEM_ID = p_id_project;
    COMMIT;
    RETURN;       
END IF;

IF(substr(p_codice_atipicita, length(p_codice_atipicita)) = '-') THEN
    p_codice_atipicita := substr(p_codice_atipicita, 0, length(p_codice_atipicita)-1);
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || p_id_project || ' - ' || p_codice_atipicita);
    update PROJECT set CHA_COD_T_A = p_codice_atipicita where SYSTEM_ID = p_id_project;
    COMMIT;
    RETURN;       
END IF;

EXCEPTION 
WHEN others THEN
DBMS_OUTPUT.PUT_LINE('Errore nell''esecuzione della procedura');

END; 

/
