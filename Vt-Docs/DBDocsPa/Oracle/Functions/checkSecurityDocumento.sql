CREATE OR REPLACE FUNCTION @db_user.checkSecurityDocumento
(
idprofileParam INT,
idpeopleParam INT,
idgroupParam INT
)
RETURN INT IS retValue INT;

thingVar INT := NULL;

BEGIN
retValue := 0;

SELECT ID_DOCUMENTO_PRINCIPALE INTO thingVar
FROM PROFILE
WHERE SYSTEM_ID = idprofileParam;

IF (thingVar IS NULL) THEN
thingVar := idprofileParam;
END IF;

retValue := checkSecurity(thingVar, idpeopleParam, idgroupParam);

RETURN retValue;
END checkSecurityDocumento;
/