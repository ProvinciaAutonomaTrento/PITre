--------------------------------------------------------
--  DDL for Function GETCODTIPOFUNZ
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODTIPOFUNZ" (idfunz NUMBER)
RETURN VARCHAR2 IS risultato VARCHAR2(16);

BEGIN
begin

SELECT VAR_COD_tipo INTO risultato FROM DPA_tipo_funzione WHERE SYSTEM_ID = idfunz;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END getcodtipofunz; 

/
