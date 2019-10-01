--------------------------------------------------------
--  DDL for Function GETDESCTIPOFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDESCTIPOFASC" (idAtto NUMBER)
RETURN VARCHAR2 IS risultato VARCHAR2(255 Byte);

BEGIN
begin

SELECT VAR_DESC_FASC
INTO risultato FROM DPA_tipo_fasc f WHERE SYSTEM_ID = idAtto;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END GetDescTipoFasc; 

/
