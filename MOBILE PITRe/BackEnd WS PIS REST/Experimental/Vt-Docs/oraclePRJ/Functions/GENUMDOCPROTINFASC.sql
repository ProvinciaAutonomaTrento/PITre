--------------------------------------------------------
--  DDL for Function GENUMDOCPROTINFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GENUMDOCPROTINFASC" (idfasc number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(link) into tmpvar   from project_components pc,profile p where p.SYSTEM_ID=pc.link and p.num_proto is not null and p.CHA_TIPO_PROTO in ('A','P','I')  and  pc.project_id in (select system_id from project where id_fascicolo=idfasc);
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocProtInFasc; 

/
