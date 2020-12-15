--------------------------------------------------------
--  DDL for Procedure SP_INS_UO_CREATORE_PROJECT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_UO_CREATORE_PROJECT" 
AS
ruolo_creatore number;
projectId number;
uo number;


cursor curr_project is
select ID_RUOLO_CREATORE, system_id from project where cha_tipo_proj='F'
and CHA_TIPO_FASCICOLO = 'P';

BEGIN

open curr_project;
loop

fetch curr_project into ruolo_creatore, projectId;
exit when curr_project%notfound;
begin


select ID_UO into uo from dpa_corr_globali where id_gruppo=ruolo_creatore;

update project p set p.ID_UO_CREATORE=uo where system_id=projectId;

EXCEPTION
WHEN OTHERS THEN null;

end;
end loop;
END;

/
