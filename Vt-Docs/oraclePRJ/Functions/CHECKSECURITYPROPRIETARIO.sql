--------------------------------------------------------
--  DDL for Function CHECKSECURITYPROPRIETARIO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CHECKSECURITYPROPRIETARIO" 
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
FROM security s, project b
WHERE s.thing = thingParam
AND s.personorgroup IN (idgroupParam, idpeopleParam)
and s.cha_tipo_diritto='P'
and s.thing = b.system_id
AND b.CHA_TIPO_FASCICOLO = 'P';

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;

RETURN retValue;

END checkSecurityProprietario; 

/
