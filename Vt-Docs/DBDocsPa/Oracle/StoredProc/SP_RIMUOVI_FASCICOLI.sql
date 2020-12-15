CREATE OR REPLACE PROCEDURE @db_user.SP_RIMUOVI_FASCICOLI(idProject IN number, ReturnValue OUT NUMBER) IS
BEGIN

DELETE FROM DPA_TRASM_UTENTE WHERE id_trasm_singola in
(SELECT system_id FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in
(select    t.system_id  from dpa_trasmissione t where t.ID_PROJECT = idProject));

DELETE FROM DPA_TRASM_SINGOLA WHERE id_trasmissione in
(select    t.system_id  from dpa_trasmissione t where t.ID_PROJECT = idProject);


DELETE FROM DPA_TRASMISSIONE WHERE ID_PROJECT=idProject;

DELETE FROM project_components where project_id = idProject ;
DELETE FROM DPA_AREA_LAVORO WHERE ID_PROJECT = idProject;
DELETE FROM PROJECT WHERE SYSTEM_ID = idProject;
DELETE FROM SECURITY WHERE THING = idProject;
delete from dpa_todolist where id_project = idProject;


DECLARE cnt INT;
BEGIN
SELECT COUNT(*) INTO cnt FROM DPA_ASS_TEMPLATES_FASC
WHERE ID_PROJECT = idProject;
IF (cnt != 0) THEN
DELETE FROM DPA_ASS_TEMPLATES_FASC
WHERE ID_PROJECT = idProject;
END IF;
END;
ReturnValue:=1;
END;
/