begin 
Utl_Backup_Plsql_code ('PROCEDURE','CopySecurityWithoutAtipici'); 
end;
/

create or replace
PROCEDURE          CopySecurityWithoutAtipici (
  -- Id gruppo del ruolo di cui copiare la visibilit
  sourceGroupId IN NUMBER,
  -- Id gruppo del ruolo di destinazione
  destinationGroupId IN NUMBER
)
AS 
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     CopySecurityWithoutAtipici

    PURPOSE:  Store per la copia di alcuni record della security esclusi 
              i record relativi a documenti e fascicoli atipici. I record 
              copiati avranno come tipo diritto 'A' 

  ******************************************************************************/

   -- Nome della colonna letta dalla tabella dei metadati
  DECLARE colName VARCHAR2 (2000);
  
  -- Lista separata da , dei nomi delle colonne in cui eseguire la insert
  colNameList VARCHAR2 (4000);
  
  -- Lista separata da , dei valori da assegnare alle colonne
  colValuesList VARCHAR2 (4000);
  
  -- Selezione delle colonne della security dalla tabella dei metadati
  CURSOR curColumns IS
    SELECT cname from col where tname = 'SECURITY' order by colno asc;
      
  BEGIN OPEN curColumns;
  LOOP FETCH curColumns INTO colName;
  EXIT WHEN curColumns%NOTFOUND;
  
    -- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'
    colNameList := colNameList || ', ' || colName;
    
    CASE (colName)
        WHEN 'THING' THEN
          colValuesList := colValuesList || ', DISTINCT(thing)';
        WHEN 'PERSONORGROUP' THEN
          colValuesList := colValuesList || ', ' || destinationGroupId;
        WHEN 'ACCESSRIGHTS' THEN
          colValuesList := colValuesList || ', DECODE(accessrights, 255, 63, accessrights, accessrights)';
        WHEN 'CHA_TIPO_DIRITTTO' THEN
          colValuesList := colValuesList || ', A';
        ELSE
          colValuesList := colValuesList || ', ' || colName;
    END CASE;
  END LOOP;
  CLOSE curColumns;
  
  colNameList := SUBSTR( colNameList, 3); 
  colValuesList := SUBSTR( colValuesList, 3); 
  
  EXECUTE IMMEDIATE 'INSERT INTO security (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM security s inner join profile p on s.thing = p.system_id where s.personorgroup = ' || sourcegroupid || ' and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' || destinationGroupId || '))';

  EXECUTE IMMEDIATE 'INSERT INTO security (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM security s inner join project p on s.thing = p.system_id where s.personorgroup = ' || sourcegroupid || ' and p.cha_cod_t_a = ''T'' or p.cha_cod_t_a is null AND NOT EXISTS (select ''x'' from security where thing = s.thing and personorgroup = ' || destinationGroupId || '))';
END;  
END CopySecurityWithoutAtipici; 
/
