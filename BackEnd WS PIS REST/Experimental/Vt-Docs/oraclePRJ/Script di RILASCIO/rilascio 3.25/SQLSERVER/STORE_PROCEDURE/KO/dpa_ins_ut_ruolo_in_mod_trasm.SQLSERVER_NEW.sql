---- dpa_ins_ut_ruolo_in_mod_trasm.ORA.sql  marcatore per ricerca ----
begin 
Utl_Backup_Plsql_code ('PROCEDURE','dpa_ins_ut_ruolo_in_mod_trasm'); 
end;
/

create or replace
PROCEDURE dpa_ins_ut_ruolo_in_mod_trasm(
    p_idpeople   IN INTEGER,
    p_idcorrglob IN INTEGER,
    p_returnvalue OUT INTEGER )
IS
BEGIN
  DECLARE
    CURSOR modello (idpeople INTEGER, idcorrglob INTEGER)
    IS
      SELECT DISTINCT g.system_id AS ID,
        g.id_modello              AS idmodello,
        g.id_modello_mitt_dest    AS idmodellomittdest
      FROM dpa_modelli_dest_con_notifica g
      WHERE g.id_modello_mitt_dest IN
        ( SELECT DISTINCT a.system_id AS ID
        FROM dpa_modelli_mitt_dest a
        WHERE a.cha_tipo_mitt_dest = 'D'
        AND a.id_corr_globali      = idcorrglob
        AND a.cha_tipo_urp         = 'R'
        )
    AND g.id_people NOT IN (idpeople);
    sysidmod      NUMBER;
    utinruolo     NUMBER;
    utconnotifica NUMBER;
  BEGIN
    p_returnvalue := 0;
    FOR currentmod IN modello (p_idpeople, p_idcorrglob)
    LOOP
      BEGIN
        SELECT COUNT (*)
        INTO utinruolo
        FROM peoplegroups
        WHERE groups_system_id =
          (SELECT id_gruppo FROM dpa_corr_globali WHERE system_id = p_idcorrglob
          )
        AND dta_fine         IS NULL
        AND people_system_id <> p_idpeople;
        SELECT COUNT (DISTINCT (id_people))
        INTO utconnotifica
        FROM dpa_modelli_dest_con_notifica
        WHERE id_modello = currentmod.idmodello
        AND id_people   IN
          (SELECT people_system_id
          FROM peoplegroups
          WHERE groups_system_id =
            (SELECT id_gruppo FROM dpa_corr_globali WHERE system_id = p_idcorrglob
            )
          AND dta_fine IS NULL
          );
        IF (utinruolo = utconnotifica) THEN
          BEGIN
            SELECT seq_dpa_modelli_mitt_dest.NEXTVAL INTO sysidmod FROM DUAL;
            INSERT
            INTO dpa_modelli_dest_con_notifica
              (
                system_id,
                id_modello_mitt_dest,
                id_people,
                id_modello
              )
              VALUES
              (
                sysidmod,
                currentmod.idmodellomittdest,
                p_idpeople,
                currentmod.idmodello
              );
          EXCEPTION
          WHEN OTHERS THEN
            p_returnvalue := -1;
          END;
        ELSE
          p_returnvalue := 2;
        END IF;
      END;
    END LOOP;
    p_returnvalue := 1;
  END;
END dpa_ins_ut_ruolo_in_mod_trasm; 
/
              
----------- FINE -