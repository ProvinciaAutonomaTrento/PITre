--------------------------------------------------------
--  DDL for Procedure DPA_INS_UT_RUOLO_IN_MOD_TRASM
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DPA_INS_UT_RUOLO_IN_MOD_TRASM" (
p_idpeople      IN      integer,
p_idcorrglob     IN    integer,
p_returnvalue     OUT   integer
)
IS

BEGIN
DECLARE
CURSOR modello (idpeople integer, idcorrglob integer)
IS
SELECT  distinct g.SYSTEM_ID as ID, g.ID_MODELLO as IdModello, g.ID_MODELLO_MITT_DEST as IdModelloMittDest
from dpa_modelli_dest_con_notifica g
where g.ID_MODELLO_MITT_DEST in (
SELECT  distinct a.SYSTEM_ID as ID
FROM dpa_modelli_mitt_dest a
WHERE a.CHA_TIPO_MITT_DEST = 'D'
AND a.ID_CORR_GLOBALI = idcorrglob
AND a.CHA_TIPO_URP = 'R')
and g.ID_PEOPLE not in (idpeople);

sysidmod        NUMBER;
utInRuolo         NUMBER;
utConNotifica   NUMBER;

BEGIN
p_returnvalue := 0;

FOR currentmod IN modello (p_idpeople, p_idcorrglob)
LOOP
BEGIN

select count(*) into utInRuolo from peoplegroups
where groups_system_id = (select id_gruppo from dpa_corr_globali where system_id = p_idcorrglob)
and dta_fine is null
and people_system_id <> p_idpeople ;

select count(distinct(id_people)) into utConNotifica from dpa_modelli_dest_con_notifica
where id_modello = currentmod.IdModello
and id_people in (select people_system_id from peoplegroups where groups_system_id = (select id_gruppo from dpa_corr_globali where system_id = p_idcorrglob) and dta_fine is null);

if(utInRuolo = utConNotifica)
then
begin 
    SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
    INTO sysidmod
    FROM DUAL;

    insert into dpa_modelli_dest_con_notifica
    (system_id, id_modello_mitt_dest, id_people, id_modello)
    VALUES
    (sysidmod, currentmod.IdModelloMittDest, p_idpeople, currentmod.IdModello);

    EXCEPTION
    WHEN OTHERS
    THEN
    p_returnvalue := -1;
END;
else
    p_returnvalue := 2;
end if;
end;
END LOOP;

p_returnvalue :=1;
END;
END dpa_ins_ut_ruolo_in_mod_trasm; 

/
