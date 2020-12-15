--------------------------------------------------------
--  DDL for Function DESCUO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DESCUO" (idUO INT)
RETURN VARCHAR IS risultato VARCHAR(256);

BEGIN
if(idUO is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_DESC_CORR into risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=idUO;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END DescUO; 

/
