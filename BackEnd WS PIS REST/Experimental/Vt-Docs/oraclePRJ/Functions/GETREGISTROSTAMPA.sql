--------------------------------------------------------
--  DDL for Function GETREGISTROSTAMPA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETREGISTROSTAMPA" (dNumber INT, idRegistro INT)
RETURN INT IS retValue INT;

cnt INT;

BEGIN
cnt := 0;
retValue := 0;

IF(dNumber is not null)
then
if(idRegistro is not null)
then
SELECT COUNT(A.SYSTEM_ID) into cnt
From DPA_STAMPAREGISTRI A, PROFILE B
where A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber=dNumber
and a.id_registro=idRegistro;
end if;
end if;

IF (cnt > 0)
then
retValue := 1;
ELSE
retValue := 0;
end if;

RETURN retValue;

END getRegistroStampa;

/
