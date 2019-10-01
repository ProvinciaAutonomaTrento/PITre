--------------------------------------------------------
--  DDL for Function GENUMDOCINFASC2
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GENUMDOCINFASC2" (idParent number,amm number,idreg number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(link) into tmpvar   from project_components  
where project_id in (

   SELECT system_id
        FROM project
       WHERE cha_tipo_proj = 'C'
         AND id_amm = amm
         AND id_fascicolo in 
         (
         SELECT system_id
        FROM project
       WHERE cha_tipo_proj = 'F'
         AND id_amm = amm
         AND (id_registro = idreg OR id_registro IS NULL)
         AND id_parent = idParent
         )
         AND (id_registro = idreg OR id_registro IS NULL));

EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocInFasc2; 

/
