begin 
Utl_Backup_Plsql_code ('FUNCTION','getuo'); 
end;
/

create or replace
FUNCTION getuo(
      idruolo INT)
    RETURN VARCHAR
  Is
    risultato VARCHAR (256);
  BEGIN
    SELECT f.VAR_DESC_corr
    INTO risultato
    FROM dpa_corr_globali f
    WHERE system_id IN
      (SELECT id_uo FROM dpa_corr_globali WHERE system_id=idruolo
      );
    RETURN risultato;
  END getuo;
/
