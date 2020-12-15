--------------------------------------------------------
--  DDL for Procedure SP_INSERT_DOC_ARCHIVIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INSERT_DOC_ARCHIVIO" (p_idProfile NUMBER, p_serie NUMBER, p_result OUT NUMBER) IS
inFasc number:=0;
inSerie number := 0;
docNumber number := 0;
BEGIN

--select docnumber into docNumber from profile where system_id = p_idProfile;

SELECT count(*) INTO inFasc FROM PROJECT A
WHERE A.CHA_TIPO_PROJ = 'F' AND
A.SYSTEM_ID IN
(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B
WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=p_idProfile);

select count(*) into inSerie
from profile, dpa_associazione_templates, dpa_oggetti_custom
where
profile.docnumber = dpa_associazione_templates.doc_number
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
profile.docnumber in (select docnumber from profile where system_id = p_idProfile)
and
dpa_oggetti_custom.repertorio = 1
and
dpa_associazione_templates.valore_oggetto_db is not null;

if ((inFasc=1 and p_serie=1) or (inFasc>1 and p_serie=0) or (inSerie>0 and p_serie=0)) then
begin
UPDATE PROFILE SET  CHA_IN_ARCHIVIO='2' WHERE SYSTEM_ID = p_idProfile;
p_result:=2;
end;
else
begin
UPDATE PROFILE SET  CHA_IN_ARCHIVIO='1' WHERE SYSTEM_ID = p_idProfile;
DELETE FROM DPA_AREA_LAVORO WHERE ID_PROFILE = p_idProfile;
update dpa_trasm_utente set cha_in_todolist = '0' where id_trasm_singola in
(select system_id from dpa_trasm_singola where id_trasmissione in
(select system_id from dpa_trasmissione where id_profile = p_idProfile));
p_result:=1;
end;
end if;
exception when others then p_result:=-1;
END SP_INSERT_DOC_ARCHIVIO; 

/
