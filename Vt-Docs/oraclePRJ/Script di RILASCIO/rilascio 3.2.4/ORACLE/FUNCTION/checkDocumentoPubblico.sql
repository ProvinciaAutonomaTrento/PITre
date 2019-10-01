create or replace
FUNCTION          checkDocumentoPubblico
(
thingParam INT,
idRuoloPubblico INT
)
RETURN INT IS retValue INT;

cnt INT := 0;


BEGIN
retValue :=0;

SELECT COUNT(*) INTO cnt
FROM security
WHERE thing = thingParam AND personorgroup = idRuoloPubblico
and ACCESSRIGHTS>0;

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;

RETURN retValue;
END checkDocumentoPubblico; 
 