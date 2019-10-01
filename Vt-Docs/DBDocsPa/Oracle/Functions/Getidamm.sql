

CREATE OR REPLACE FUNCTION @db_user.Getidamm(idPeople NUMBER)
RETURN NUMBER IS risultato NUMBER;

BEGIN

SELECT ID_AMM INTO risultato FROM PEOPLE WHERE SYSTEM_ID = idPeople;

RETURN risultato;
END Getidamm;
/
