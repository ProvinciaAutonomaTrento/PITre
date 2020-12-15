--------------------------------------------------------
--  DDL for Function GETCODTIPORUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODTIPORUOLO" (idtipo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
if(idtipo is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_codice into risultato FROM DPA_tipo_ruolo WHERE SYSTEM_ID=idtipo;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END getcodtiporuolo; 

/
