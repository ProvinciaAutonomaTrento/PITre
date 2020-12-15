
CREATE OR REPLACE TRIGGER TR_INSERT_DPA_TODOLIST
AFTER UPDATE
OF DTA_INVIO
ON @db_user.DPA_TRASMISSIONE
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN

INSERT INTO @db_user.DPA_TODOLIST
SELECT :NEW.system_id,
dtu.id_trasm_singola,
dtu.system_id,
:NEW.dta_invio,
:NEW.id_people,
:NEW.id_ruolo_in_uo,
dtu.id_people,
dts.id_ragione,
:NEW.var_note_generali,
dts.var_note_sing,
dts.dta_scadenza,
:NEW.id_profile,
:NEW.id_project,
TO_NUMBER(Vardescribe(dts.id_corr_globale,'ID_GRUPPO')) AS id_ruolo_dest,
TO_NUMBER(Vardescribe(:NEW.id_profile,'PROF_IDREG')) AS id_registro,
DTS.CHA_TIPO_DEST
FROM DPA_TRASM_SINGOLA dts,DPA_TRASM_UTENTE dtu
WHERE dtu.id_trasm_singola = dts.system_id AND dts.id_trasmissione = :NEW.system_id AND dtu.cha_in_todolist = 1;
END;
/