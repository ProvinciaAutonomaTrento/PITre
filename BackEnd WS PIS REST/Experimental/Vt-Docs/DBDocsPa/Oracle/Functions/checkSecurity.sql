CREATE OR REPLACE FUNCTION @db_user.checkSecurity
(
thingParam INT,
idpeopleParam INT,
idgroupParam INT
)
RETURN INT IS retValue INT;

cnt INT := 0;

BEGIN
retValue := 0;

SELECT COUNT(*) INTO cnt
FROM security
WHERE thing = thingParam AND personorgroup IN (idgroupParam, idpeopleParam);

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;

RETURN retValue;

END checkSecurity;
/