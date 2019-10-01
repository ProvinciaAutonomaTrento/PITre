--------------------------------------------------------
--  DDL for Function GENUMDOCINFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GENUMDOCINFASC" (idfasc number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(link) into tmpvar   from project_components  where project_id in (select system_id from project where id_fascicolo=idfasc);
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocInFasc; 

/
