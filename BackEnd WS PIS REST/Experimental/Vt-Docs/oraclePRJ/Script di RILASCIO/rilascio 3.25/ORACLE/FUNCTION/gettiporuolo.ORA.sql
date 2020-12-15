begin 
Utl_Backup_Plsql_code ('FUNCTION','gettiporuolo'); 
end;
/

create or replace
FUNCTION gettiporuolo(
      idgruppo INT)
    RETURN VARCHAR
  IS
    risultato VARCHAR(256);
  BEGIN
    SELECT f.var_desc_ruolo
    INTO risultato
    FROM dpa_corr_globali d,
      dpa_tipo_ruolo f
    WHERE id_gruppo = idgruppo
    AND f.system_id = d.id_tipo_ruolo;
    RETURN risultato;
  END gettiporuolo;
/
