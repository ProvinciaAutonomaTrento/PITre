--------------------------------------------------------
--  DDL for Function GETCODRUBRICAIDPAREMT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUBRICAIDPAREMT" (idUO INT)
RETURN VARCHAR IS risultato VARCHAR(256);

BEGIN
if(idUO is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_COD_RUBRICA into risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=idUO;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END GETCODRUBRICAIDPAREMT; 

/
