begin 
Utl_Backup_Plsql_code ('PROCEDURE','SP_RIMUOVI_DOCUMENTI'); 
end;
/

create or replace
PROCEDURE SP_RIMUOVI_DOCUMENTI(
    idProfile IN NUMBER,
    ReturnValue OUT NUMBER)
IS
BEGIN
  DELETE
  FROM DPA_TRASM_UTENTE
  WHERE id_trasm_singola IN
    (SELECT system_id
    FROM DPA_TRASM_SINGOLA
    WHERE id_trasmissione IN
      (Select T.System_Id From Dpa_Trasmissione T Where T.Id_Profile = Idprofile
      )
    );
  DELETE
  FROM DPA_TRASM_SINGOLA
  Where Id_Trasmissione In
    (SELECT t.system_id FROM dpa_trasmissione t WHERE t.id_profile = idProfile
    );
  Delete From Dpa_Trasmissione Where Id_Profile = Idprofile;
  DELETE FROM project_components WHERE LINK = idProfile;
  DELETE FROM VERSIONS WHERE DOCNUMBER = idProfile;
  DELETE FROM COMPONENTS WHERE DOCNUMBER = idProfile;
  DELETE FROM DPA_AREA_LAVORO WHERE ID_PROFILE = idProfile;
  DELETE FROM DPA_PROF_PAROLE WHERE ID_PROFILE = idProfile;
  DELETE FROM PROFILE WHERE DOCNUMBER = idProfile;
  DELETE FROM SECURITY WHERE THING = idProfile;
  DELETE FROM dpa_todolist WHERE id_profile = idProfile;
  DELETE FROM DPA_DIAGRAMMI WHERE DOC_NUMBER = idProfile;
  DECLARE
    cnt INT;
  BEGIN
    SELECT COUNT(*)
    INTO cnt
    FROM DPA_ASSOCIAZIONE_TEMPLATES
    WHERE DOC_NUMBER = idProfile;
    IF (cnt         != 0) THEN
      DELETE FROM DPA_ASSOCIAZIONE_TEMPLATES WHERE DOC_NUMBER = idProfile;
    END IF;
  End;
  ReturnValue := 1;
END;
/
