CREATE OR REPLACE PROCEDURE @db_user.dpa_upd_todolist_doc_cestino(p_idProfile IN number
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