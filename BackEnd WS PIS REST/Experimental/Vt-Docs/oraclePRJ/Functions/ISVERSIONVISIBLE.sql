--------------------------------------------------------
--  DDL for Function ISVERSIONVISIBLE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ISVERSIONVISIBLE" 
(
versionId NUMBER,
idPeople NUMBER,
idGroup NUMBER
)
RETURN INT IS retValue INT;

idProfile NUMBER(10) := 0;
maxVersionId NUMBER(10) := 0;
hideVersions NUMBER(10) := 0;
ownership NUMBER(10) := 0;

BEGIN
retValue := 0;

-- 1) Reperimento IdProfile e DocNumber del documento
select  p.system_id into idProfile
from    versions v 
        inner join profile p on v.docnumber = p.docnumber
where   v.version_id = versionId;

-- 2) verifica se la versione richiesta ? l'ultima
select max(v.version_id) into maxVersionId
from versions v
where v.docnumber = (select docnumber from profile where system_id = idProfile);

if (maxVersionId = versionId) then
    -- 2a.) Il versionId si riferisce all'ultima versione del documento, ? sempre visibile
    retValue := 1;
else

    -- 3) verifica se il documento ? stato trasmesso a me o al mio ruolo e 
        -- se tale trasmissione prevede le versioni precedenti nascoste 
    select count(*) into hideVersions
    from dpa_trasmissione t
          inner join dpa_trasm_singola ts on t.system_id = ts.id_trasmissione
        where t.id_profile = idProfile
        and ts.id_corr_globale in 
            (
                (select system_id from dpa_corr_globali where id_people = idPeople), 
                (select system_id from dpa_corr_globali where id_gruppo = idGroup)
            )
        and ts.hide_doc_versions = '1';

    if (hideVersions > 0) then
        -- 4) verifica in sercurity se sul doc non dispongo dei diritti di ownership 
        -- (trasmissione a me stesso) oppure abbia gi? acquisito i diritti di visibilit?
        -- (es. superiore gerarchico)
        select count(*) into ownership
        from security s 
        where thing = idProfile
                and personorgroup in (idPeople, idGroup)
                and (cha_tipo_diritto = 'P' or cha_tipo_diritto = 'A');
        
        
        if (ownership = 0) then
            -- Sul documento non si dispongono i diritti di ownership,
            -- pertanto la versione deve essere nascosta 
            retValue := 0;
        else
            -- Sul documento si dispongono gi? dei diritti di ownership,
            -- pertanto la versione non deve essere nascosta        
            retValue := 1;
        end if;
        
    else
        -- 3a) la tx non prevede di nascondere le versioni, quindi la versione ? sempre visibile
        retValue := 1;    
    end if;
end if;

RETURN retValue;
EXCEPTION
WHEN OTHERS
THEN
return -1;

END isVersionVisible; 

/
