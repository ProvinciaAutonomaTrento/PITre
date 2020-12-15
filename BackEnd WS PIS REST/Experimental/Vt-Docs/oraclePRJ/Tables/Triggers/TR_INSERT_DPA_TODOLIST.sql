--------------------------------------------------------
--  DDL for Trigger TR_INSERT_DPA_TODOLIST
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."TR_INSERT_DPA_TODOLIST" 
AFTER UPDATE
OF DTA_INVIO
ON DPA_TRASMISSIONE REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN
INSERT INTO dpa_todolist
SELECT :NEW.system_id, dtu.id_trasm_singola, dtu.system_id,
:NEW.dta_invio, :NEW.id_people, :NEW.id_ruolo_in_uo,
dtu.id_people, dts.id_ragione, :NEW.var_note_generali,
dts.var_note_sing, dts.dta_scadenza, :NEW.id_profile,
:NEW.id_project,
TO_NUMBER (vardescribe (dts.id_corr_globale, 'ID_GRUPPO')
) AS id_ruolo_dest,
TO_NUMBER (vardescribe (:NEW.id_profile, 'PROF_IDREG')
) AS id_registro,
dts.cha_tipo_trasm,
CASE
WHEN dtu.dta_vista IS NULL
THEN TO_DATE ('01/01/1753', 'dd/mm/yyyy')
ELSE dtu.dta_vista
END,
:NEW.ID_PEOPLE_DELEGATO
FROM dpa_trasm_singola dts, dpa_trasm_utente dtu
WHERE dtu.id_trasm_singola = dts.system_id
AND dts.id_trasmissione = :NEW.system_id
AND dtu.cha_in_todolist = 1;
END; 
/
ALTER TRIGGER "ITCOLL_6GIU12"."TR_INSERT_DPA_TODOLIST" ENABLE;
