create or replace
TRIGGER TR_INSERT_EVENT_TRASM
AFTER INSERT
ON DPA_RAGIONE_TRASM
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN
   INSERT INTO DPA_ANAGRAFICA_LOG
        VALUES (
                  seq.NEXTVAL,
                  'TRASM_DOC_' || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                     'Effettuata trasmissione documento con ragione '
                  || :new.VAR_DESC_RAGIONE,
                  'DOCUMENTO',
                  'TRASM_DOC_' || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                  NULL,
                  :new.ID_AMM,
                  '1',
                  '0',
                  'TRANSMISSION_RECIPIENTS',
                  'BLUE',
                 '0',                                         --follow_config
                  '0'                                                 --follow
                     );

   INSERT INTO DPA_ANAGRAFICA_LOG
        VALUES (
                  seq.NEXTVAL,
                     'TRASM_FOLDER_'
                  || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                     'Effettuata trasmissione fascicolo con ragione '
                  || :new.VAR_DESC_RAGIONE,
                  'FASCICOLO',
                     'TRASM_FOLDER_'
                  || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                  NULL,
                  :new.ID_AMM,
                  '1',
                  '0',
                  'TRANSMISSION_RECIPIENTS',
                  'BLUE',
                  '0',                                         --follow_config
                   '0'                                                 --follow
                     );

   INSERT INTO dpa_log_attivati
      (SELECT l.system_id, :new.ID_AMM, 'OBB'
         FROM dpa_anagrafica_log l
        WHERE (    (   l.var_codice =
                             'TRASM_FOLDER_'
                          || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_')
                    OR var_codice =
                             'TRASM_DOC_'
                          || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'))
               AND l.id_amm = :new.ID_AMM));
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
END;