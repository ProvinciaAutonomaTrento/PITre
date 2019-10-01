--------------------------------------------------------
--  DDL for Function CHECKVISIBILITAARCHIVIO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CHECKVISIBILITAARCHIVIO" 
(
tipoObjParam VARCHAR,
thingParam INT,
idgroupParam INT
)
RETURN INT IS retValue INT;

inArchivio VARCHAR(1) := '';
isArchivista INT := 0;

BEGIN
retValue := 0;

if (tipoObjParam = 'D')
then
SELECT CHA_IN_ARCHIVIO into inArchivio FROM PROFILE WHERE SYSTEM_ID=thingParam;
else
SELECT CHA_IN_ARCHIVIO into inArchivio FROM PROJECT WHERE SYSTEM_ID=thingParam;
end if;

isArchivista := 0;--checkGestioneArchivio(idgroupParam);

if ((inArchivio = '0' or inArchivio = '2') and isArchivista=0)
then retValue := 1;
end if ;

if ((inArchivio = '1' or inArchivio = '2') and isArchivista=1)
then retValue := 2;
end if;

if ( (inArchivio = '1' and isArchivista = '0') or (inArchivio = '0' and isArchivista = 1))
then retValue := 0;
end if;

RETURN retValue;

END checkVisibilitaArchivio; 

/
