--------------------------------------------------------
--  DDL for Function GETUTENTICORCAT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETUTENTICORCAT" (idRuolo number)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT p.user_id from people p,peoplegroups pg where pg.PEOPLE_SYSTEM_ID = p.system_id 
and pg.GROUPS_SYSTEM_ID = idRuolo and pg.dta_fine is null and p.DISABLED = 'N';

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

exception  WHEN OTHers then risultato:='';
end;

RETURN risultato;
end; 

/
