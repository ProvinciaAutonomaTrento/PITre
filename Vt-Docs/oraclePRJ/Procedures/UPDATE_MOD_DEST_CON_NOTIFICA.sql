--------------------------------------------------------
--  DDL for Procedure UPDATE_MOD_DEST_CON_NOTIFICA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."UPDATE_MOD_DEST_CON_NOTIFICA" 
IS

contatore NUMBER;
BEGIN

DECLARE

CURSOR C IS select id_modello AS ID_MOD, id_modello_mitt_dest AS ID_MITT_DEST
from dpa_modelli_dest_con_notifica;

C1 C%ROWTYPE;
BEGIN
OPEN C;
LOOP
FETCH C INTO C1;
EXIT WHEN C%NOTFOUND;
begin
contatore := 0;

select count(*) into contatore from dpa_modelli_mitt_dest where id_modello = C1.ID_MOD and SYSTEM_ID = C1.ID_MITT_DEST;

if(contatore = 0)
then
begin
delete dpa_modelli_dest_con_notifica where id_modello = C1.ID_MOD and id_modello_mitt_dest = C1.ID_MITT_DEST;
end;
END IF;
END;

end loop;
close c;
COMMIT;
end;
END UPDATE_MOD_DEST_CON_NOTIFICA; 

/
