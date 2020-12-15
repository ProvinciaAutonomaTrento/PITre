
begin
Utl_Backup_Plsql_code ('PROCEDURE','DPA_SETNUMFASC');
end;
/

create or replace
procedure                         DPA_SETNUMFASC is
/*
questa procedura RESETTA a 1 il progressivo dei procedimentali
, in più filtrando per il o i soli titolari non chiusi
*/
   num_rec     INT;
   messaggio   VARCHAR2 (100);
/* in alcuni casi la precedente versione era
update dpa_reg_fasc set num_rif=1 where cha_automatico='1' and to_char(sysdate,'dd/mm')='01/01';
*/
BEGIN
 UPDATE dpa_reg_fasc
         SET num_rif = 1
       WHERE TO_CHAR (SYSDATE, 'dd/mm') = '01/01'
       and NUM_RIF > 1
       AND id_titolario IN (
                          SELECT     pr.system_id
                                FROM project pr
                               WHERE pr.cha_tipo_proj = 'T'
                          CONNECT BY PRIOR pr.system_id = pr.id_parent
                          START WITH pr.id_titolario = 0
                                     AND pr.cha_stato != 'C');

   num_rec := SQL%ROWCOUNT;
   messaggio := SQLERRM;

   INSERT INTO temp_log_jobs
        VALUES ('aggiornati ' || num_rec || ' record in tabella dpa_reg_fasc ',
                SYSDATE);

   COMMIT;
EXCEPTION
   WHEN OTHERS
   THEN
      RAISE;
      messaggio := SQLERRM;
    ROLLBACK;
      INSERT INTO temp_log_jobs
           VALUES (   'Errore: '
                   || messaggio
                   || ' in aggiornamento tabella dpa_reg_fasc ',
                   SYSDATE);
    COMMIT;

END dpa_setnumfasc;
/

