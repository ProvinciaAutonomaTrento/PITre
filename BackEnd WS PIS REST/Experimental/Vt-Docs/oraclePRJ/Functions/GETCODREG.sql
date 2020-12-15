--------------------------------------------------------
--  DDL for Function GETCODREG
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODREG" (idREG NUMBER)
RETURN VARCHAR2 IS risultato VARCHAR2(16);

BEGIN
begin

SELECT VAR_CODICE INTO risultato FROM DPA_EL_REGISTRI WHERE SYSTEM_ID = idREG;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END GetCodReg; 

/
