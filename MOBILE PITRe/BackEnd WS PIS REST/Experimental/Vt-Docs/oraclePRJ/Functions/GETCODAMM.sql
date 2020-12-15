--------------------------------------------------------
--  DDL for Function GETCODAMM
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODAMM" (IDAMM NUMBER)
RETURN VARCHAR IS risultato VARCHAR(16);
BEGIN
BEGIN

SELECT VAR_CODICE_AMM INTO risultato FROM DPA_AMMINISTRA WHERE SYSTEM_ID = IDAMM;

RETURN risultato;
END;
END GetCODamm; 

/
