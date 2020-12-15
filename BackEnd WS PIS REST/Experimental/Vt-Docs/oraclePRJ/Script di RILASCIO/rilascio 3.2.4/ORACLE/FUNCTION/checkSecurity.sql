create or replace
FUNCTION          checkSecurity
(
thingParam INT,
idpeopleParam INT,
idgroupParam INT,
idRuoloPubblico INT,
tipoObjParam VARCHAR
)
RETURN INT IS retValue INT;

cnt INT := 0;


BEGIN
retValue :=1;
--select count(system_id) into retvalue from peo0ple where cha_superAdmin='1' and system_id=idpeopleParam;
--if (retValue=1) then
--retvalue:=retvalue+1;
--end if;
 -- ITEA_TEST.checkVisibilitaArchivio(tipoObjParam, thingParam, idgroupParam);
if (retValue=1) then
SELECT COUNT(*) INTO cnt
FROM security
WHERE thing = thingParam AND personorgroup IN (idgroupParam, idpeopleParam, idRuoloPubblico)
and ACCESSRIGHTS>0;

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;
end if;
RETURN retValue;
END checkSecurity; 
 