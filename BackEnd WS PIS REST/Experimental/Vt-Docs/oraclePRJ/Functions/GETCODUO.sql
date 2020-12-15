--------------------------------------------------------
--  DDL for Function GETCODUO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODUO" (idUO INT)
RETURN VARCHAR IS risultato VARCHAR(256);

BEGIN
if(idUO is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_CODICE into risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=idUO;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END GETcodUO;

/
