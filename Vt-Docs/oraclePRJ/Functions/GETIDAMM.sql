--------------------------------------------------------
--  DDL for Function GETIDAMM
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIDAMM" (idPeople NUMBER)
RETURN NUMBER IS risultato NUMBER;

BEGIN

SELECT ID_AMM INTO risultato FROM PEOPLE WHERE SYSTEM_ID = idPeople;

RETURN risultato;
END Getidamm; 

/
