--------------------------------------------------------
--  DDL for Function GETCODRUOLORIF
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUOLORIF" (system_id number)
RETURN VARCHAR2 IS risultato VARCHAR2(50);

BEGIN
begin


select id
INTO risultato from (
SELECT groups_system_id as id
 FROM peoplegroups gp WHERE gp.people_system_id = system_id and cha_preferito = '1'
) where rownum=1;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END getCodRuoloRif; 

/
