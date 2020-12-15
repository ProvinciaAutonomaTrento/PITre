create or replace
FUNCTION           checkSecurityDocumento
(
idprofileParam INT,
idpeopleParam INT,
idgroupParam INT,
idRuoloPubblico INT,
tipoObjParam VARCHAR
)
RETURN INT IS retValue INT;

thingVar INT := NULL;

BEGIN
retValue := 0;

-- Verifica se il documento richiesto ? un allegato,
-- in tal caso verr? verificata la security del documento principale
SELECT ID_DOCUMENTO_PRINCIPALE INTO thingVar
FROM PROFILE
WHERE SYSTEM_ID = idprofileParam;

IF (thingVar IS NULL) THEN
thingVar := idprofileParam;
END IF;

retValue := checkSecurity(thingVar, idpeopleParam, idgroupParam, idRuoloPubblico, tipoObjParam);

RETURN retValue;
END checkSecurityDocumento; 
 