--------------------------------------------------------
--  DDL for Function GENUMDOCPROTINFASCRF
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GENUMDOCPROTINFASCRF" (idfasc number,codrf varchar ) RETURN int IS
tmpVar int;
BEGIN
begin
/* Formatted on 2010/05/25 14:37 (Formatter Plus v4.8.8) */
SELECT COUNT (LINK)
  INTO tmpvar
  FROM project_components pc, PROFILE p
 WHERE p.system_id = pc.LINK
   AND p.num_proto IS NOT NULL
   AND p.cha_tipo_proto IN ('A', 'P', 'I')
   AND pc.project_id IN (SELECT system_id
                           FROM project
                          WHERE id_fascicolo = idfasc)
   and codrfappartenzaruolo(p.ID_RUOLO_PROT) =codrf;                      
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocProtInFascRF; 

/
