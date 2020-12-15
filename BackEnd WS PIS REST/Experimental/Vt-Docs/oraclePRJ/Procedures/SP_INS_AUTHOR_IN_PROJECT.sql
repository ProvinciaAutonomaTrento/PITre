--------------------------------------------------------
--  DDL for Procedure SP_INS_AUTHOR_IN_PROJECT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INS_AUTHOR_IN_PROJECT" 
AS
id_project number;
autore number;

cursor curr_project is
select system_id from project
where   cha_tipo_proj='F'
and CHA_TIPO_FASCICOLO = 'P';

BEGIN

open curr_project;
loop

fetch curr_project into id_project;
exit when curr_project%notfound;

begin

select person INTO autore  from (

SELECT s.personorgroup person FROM security s, project b WHERE s.thing=id_project and

s.thing = b.system_id

and s.cha_tipo_diritto='P' and s.accessrights=0

AND b.CHA_TIPO_FASCICOLO = 'P')

where rownum=1;

update project p set p.author=autore where system_id=id_project;



EXCEPTION
WHEN OTHERS THEN null;

end;

end loop;
END;

/
