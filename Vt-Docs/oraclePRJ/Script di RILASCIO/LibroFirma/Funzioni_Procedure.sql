create or replace
FUNCTION getObjectMainDoc(systemID NUMBER)
    RETURN VARCHAR
  IS
    tmpvar VARCHAR (2000);
  BEGIN
    BEGIN
      SELECT var_prof_oggetto
      INTO tmpvar
      FROM PROFILE A
      WHERE A.SYSTEM_ID = systemID and  ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         tmpvar := '';
   END;

   RETURN tmpvar;
  END getObjectMainDoc;
  
  
  
create or replace
FUNCTION getCorrSystemId (peopleId INT)
RETURN varchar IS risultato varchar(256);
BEGIN

select system_id into risultato from dpa_corr_globali where id_people = peopleId;

RETURN RISULTATO;
END getCorrSystemId; 


CREATE OR REPLACE
FUNCTION getRuoloSystemIdCorr (idGruppo INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select system_id into risultato from dpa_corr_globali where id_Gruppo=idGruppo;
RETURN risultato;
END getRuoloSystemIdCorr;