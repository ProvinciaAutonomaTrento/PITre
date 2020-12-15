--------------------------------------------------------
--  DDL for Procedure DPA_DEL_UT_RUOLO_IN_MOD_TRASM
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DPA_DEL_UT_RUOLO_IN_MOD_TRASM" (
p_idpeople      IN      integer,
p_idcorrglob     IN    integer,
p_returnvalue     OUT   integer
)
IS

BEGIN
p_returnvalue := 0;

begin
delete from dpa_modelli_dest_con_notifica a
where a.system_id in (
select distinct c.system_id from dpa_modelli_mitt_dest b, dpa_modelli_dest_con_notifica c
where c.ID_PEOPLE = p_idpeople
and c.ID_MODELLO = b.ID_MODELLO
and b.CHA_TIPO_MITT_DEST = 'D'
and b.CHA_TIPO_URP = 'R'
and b.ID_CORR_GLOBALI = p_idcorrglob
);

EXCEPTION
WHEN OTHERS
THEN
p_returnvalue := -2;
END;

p_returnvalue :=1;
END dpa_del_ut_ruolo_in_mod_trasm;

/
