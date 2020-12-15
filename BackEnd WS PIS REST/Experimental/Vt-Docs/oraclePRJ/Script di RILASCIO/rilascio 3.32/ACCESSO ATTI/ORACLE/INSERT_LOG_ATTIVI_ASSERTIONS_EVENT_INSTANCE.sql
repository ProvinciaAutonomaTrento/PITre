create or replace
PROCEDURE INSERT_LOG_ATTIVI_ASSERTIONS_EVENT_INSTANCE
IS
   idTypeEvent     NUMBER (10, 0);
   descTypeEvent   VARCHAR2 (128 CHAR);
BEGIN
   -- LOG CREATED_FILE_ZIP_INSTANCE_ACCESS
   SELECT system_id, var_descrizione
     INTO idTypeEvent, descTypeEvent
     FROM dpa_anagrafica_log
    WHERE var_codice = 'CREATED_FILE_ZIP_INSTANCE_ACCESS';

   INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              idTypeEvent,
              descTypeEvent,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM dpa_amministra a);
	
	INSERT INTO DPA_LOG_ATTIVATI
		(
			SYSTEM_ID_ANAGRAFICA,
			ID_AMM,
			NOTIFY
		)
		SELECT idTypeEvent, am.system_id, 'CON'
		FROM DPA_AMMINISTRA am;
	
	-- LOG FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS
   SELECT system_id, var_descrizione
     INTO idTypeEvent, descTypeEvent
     FROM dpa_anagrafica_log
    WHERE var_codice = 'FAILED_CREATING_FILE_ZIP_INSTANCE_ACCESS';

   INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              idTypeEvent,
              descTypeEvent,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM dpa_amministra a);
		 
	INSERT INTO DPA_LOG_ATTIVATI
		(
			SYSTEM_ID_ANAGRAFICA,
			ID_AMM,
			NOTIFY
		)
		SELECT idTypeEvent, am.system_id, 'CON'
		FROM DPA_AMMINISTRA am;
END;