--------------------------------------------------------
--  DDL for Function GETNUMPROTOSTAMPA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETNUMPROTOSTAMPA" (dNumber INT, anno INT, numeroProtoStart INT, numeroProtoEnd INT)
RETURN INT IS retValue INT;

cnt INT;

BEGIN
cnt := 0;
retValue := 0;

IF(dNumber is not null)
then
if(anno is not null)
then
IF(numeroProtoStart IS NOT NULL)
then
SELECT COUNT(A.SYSTEM_ID) into cnt
From DPA_STAMPAREGISTRI A, PROFILE B
where A.NUM_PROTO_START <= numeroProtoStart
AND A.NUM_PROTO_END >= numeroProtoEnd
AND A.NUM_ANNO = anno
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber = dNumber;
ELSE
SELECT COUNT(A.SYSTEM_ID) into cnt
From DPA_STAMPAREGISTRI A, PROFILE B
where NUM_ANNO = anno
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber = dNumber;
end if;
else
IF(numeroProtoStart IS NOT NULL)
then
SELECT COUNT(A.SYSTEM_ID) into cnt
From DPA_STAMPAREGISTRI A, PROFILE B
where NUM_PROTO_START <= numeroProtoStart
AND NUM_PROTO_END >= numeroProtoEnd
AND A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber = dNumber;
ELSE
SELECT COUNT(A.SYSTEM_ID) into cnt
From DPA_STAMPAREGISTRI A, PROFILE B
WHERE A.DOCNUMBER = B.SYSTEM_ID
and A.docnumber = dNumber;
end if;
end if;
end if;

IF (cnt > 0)
then
retValue := 1;
ELSE
retValue := 0;
end if;

RETURN retValue;

END getnumProtoStampa;

/
