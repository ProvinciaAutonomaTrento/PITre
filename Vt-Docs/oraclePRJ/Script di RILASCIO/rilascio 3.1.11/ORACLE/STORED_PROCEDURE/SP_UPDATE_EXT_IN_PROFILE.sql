create or replace
PROCEDURE SP_UPDATE_EXT_IN_PROFILE(anno number)

/*
    Aggiorna il campo EXT della profile inserendo l'estenzione del file relativa all'ultima 
    versione associata al documento/allegato.
    Se l'ultima versione non ha un file acquisito allora non effettua l'aggionamento,
    lasciando il valore di default null.
*/
IS
    p_ext    VARCHAR(2000);
    p_idProfile    VARCHAR(2000);
    errorMessage    VARCHAR(2000);
    
    CURSOR cursor_id_profile
    IS
        select system_id FROM profile where extract(year from creation_time)=anno;
    BEGIN
        BEGIN
        OPEN cursor_id_profile;
        LOOP
           FETCH cursor_id_profile
            INTO p_idProfile;
           EXIT WHEN cursor_id_profile%NOTFOUND;
           
           BEGIN
                p_ext := GETCHAIMG(p_idProfile);
                if(p_ext <> '0')
                THEN
                    BEGIN
                        UPDATE profile
                        SET ext = p_ext
                        WHERE system_id = p_idProfile;
                    END;
                END IF;                
           END;
     END LOOP;
     CLOSE cursor_id_profile;  
         EXCEPTION
               WHEN OTHERS
               THEN
                ROLLBACK;
                errorMessage := SQLERRM;
                DBMS_OUTPUT.PUT_LINE ('Errore in fase di aggiornamento' || errorMessage);
        END;

        COMMIT;
    END;