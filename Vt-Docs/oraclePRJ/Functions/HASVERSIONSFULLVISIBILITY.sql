--------------------------------------------------------
--  DDL for Function HASVERSIONSFULLVISIBILITY
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."HASVERSIONSFULLVISIBILITY" 
(
idProfile NUMBER,
idPeople NUMBER,
idGroup NUMBER
)
RETURN INT IS retValue INT;

idDocument NUMBER(10) := 0;
idDocumentoPrincipale NUMBER(10) := 0;
hasSecurity INT := 0;

BEGIN

idDocument := idProfile;
retValue := 0;

-- 1a) Determina se il documento e un allegato
select p.id_documento_principale into idDocumentoPrincipale
from profile p
where p.system_id = idDocument;

if (not idDocumentoPrincipale is null and idDocumentoPrincipale > 0) then
    -- L'allegato non ha security, pertanto viene impostato l'id documento principale nell'id profile 
    idDocument := idDocumentoPrincipale;
end if;

-- 2) Verifica se l'utente dispone della visibilita sul documento
select /*+index (s)*/ count(*) into hasSecurity
from security s 
where s.thing = idDocument
        and s.personorgroup in (idPeople, idGroup);

if (hasSecurity > 0) then
            
    -- 4) verifica in sercurity se sul doc dispongo di diritti  
    -- superiori a quelli inviati per tramsmissione
                
    select count(*) into retValue
    from security s 
    where s.thing = idDocument
            and s.personorgroup in (idPeople, idGroup)
            and (s.hide_doc_versions is null or s.hide_doc_versions = '0');
else
    -- 2a) L'utente non dispone di alcun diritto di visibilita sul documento, versione non visibile a prescindere 
    retValue := 0;
end if;   

RETURN retValue;
EXCEPTION
WHEN OTHERS
THEN
return -1;

END hasVersionsFullVisibility; 

/
