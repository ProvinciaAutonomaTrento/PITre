--------------------------------------------------------
--  DDL for Function GETREGDESCR
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETREGDESCR" (SYSREG INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
SELECT VAR_CODICE INTO risultato FROM DPA_EL_REGISTRI WHERE system_id=SYSREG;
RETURN risultato;
END Getregdescr; 

/
