--------------------------------------------------------
--  DDL for Function GETNUMLIVRUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETNUMLIVRUOLO" (idRuolo number)
RETURN number IS risultato number;

BEGIN
if(idRuolo is null)
then risultato:=0;
else
begin
risultato := 0;
SELECT num_livello into risultato FROM DPA_tipo_ruolo WHERE SYSTEM_ID=(select id_tipo_ruolo from dpa_corr_globali where system_id=idruolo);
exception  WHEN OTHers then risultato:=0;
end;
end if;
RETURN risultato;

END getnumlivRuolo; 

/
