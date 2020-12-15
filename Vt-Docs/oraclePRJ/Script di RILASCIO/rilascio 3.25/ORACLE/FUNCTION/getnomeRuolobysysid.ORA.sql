begin 
Utl_Backup_Plsql_code ('FUNCTION','getnomeRuolobysysid'); 
end;
/

create or replace
FUNCTION getnomeRuolobysysid(
      my_system_id INT)
    RETURN VARCHAR
  IS
    risultato VARCHAR (256);
  BEGIN
    SELECT group_name INTO risultato FROM groups g WHERE system_id=my_system_id;
    Return Risultato;
  END getnomeRuolobysysid ;
/
