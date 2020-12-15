--------------------------------------------------------
--  DDL for Procedure DPA_UPD_TODOLIST_DOC_CESTINO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."DPA_UPD_TODOLIST_DOC_CESTINO" (p_idProfile IN number
,p_returnvalue     OUT   integer) IS

BEGIN
declare

cursor    cTodolist  is    select
id_trasmissione,id_people_dest,id_trasm_utente
from dpa_todolist where id_profile=p_idProfile;

v_todolist cTodolist%rowtype;
begin
open cTodolist;
loop
fetch cTodolist into v_todolist;
exit when cTodolist%notfound;


/* prima versione
UPDATE dpa_trasm_utente
SET  CHA_IN_TODOLIST = '0',
CHA_VISTA  = (CASE WHEN DTA_VISTA IS NULL THEN 1 ELSE 0 END)
WHERE (SYSTEM_ID = v_todolist.id_trasm_utente
OR
SYSTEM_ID = (SELECT TU.SYSTEM_ID FROM
DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=v_todolist.id_people_dest AND
TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID in (v_todolist.id_trasmissione) AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
AND TS.CHA_TIPO_DEST= 'U')
)
AND CHA_VALIDA='1';

*/
UPDATE dpa_trasm_utente
SET  CHA_IN_TODOLIST = '0',
CHA_VISTA  = (CASE WHEN DTA_VISTA IS NULL THEN 1 ELSE 0 END)
WHERE SYSTEM_ID = v_todolist.id_trasm_utente;


end loop;
close cTodolist;
p_returnvalue:=1;


EXCEPTION
WHEN OTHERS THEN
p_returnvalue:=0;
end;


END dpa_upd_todolist_doc_cestino; 

/
