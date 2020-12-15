INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE
    )
  SELECT seq.nextval,
    'DOC_VERIFIED',
    'Apposta firma eletronica',
    'DOCUMENTO',
    'DOC_VERIFIED',
    a.system_id,
    '0',
    '0'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM
  FROM DPA_ANAGRAFICA_LOG L
  Where L.Var_Codice = 'DOC_VERIFIED';
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE
    )
  SELECT seq.nextval,
    'DOC_STEP_OVER',
    'Approvato passo successivo',
    'DOCUMENTO',
    'DOC_STEP_OVER',
    a.system_id,
    '0',
    '0'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM
  From Dpa_Anagrafica_Log L
  Where L.Var_Codice = 'DOC_STEP_OVER';
  

  INSERT INTO DPA_ANAGRAFICA_LOG (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO, ID_AMM, NOTIFICATION, CONFIGURABLE )
SELECT seq.nextval, 'DOC_SIGNATURE', 'Apposta firma digitale cades', 'DOCUMENTO', 'DOC_SIGNATURE', a.system_id, '0', '0'
FROM DPA_AMMINISTRA a ;
   
INSERT INTO DPA_LOG_ATTIVATI (SYSTEM_ID_ANAGRAFICA, ID_AMM )
SELECT L.SYSTEM_ID, L.ID_AMM
FROM DPA_ANAGRAFICA_LOG L
Where L.Var_Codice = 'DOC_SIGNATURE'; 


 INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE
    )
  SELECT seq.nextval,
    'DOC_SIGNATURE_P',
    'Apposta firma digitale pades',
    'DOCUMENTO',
    'DOC_SIGNATURE_P',
    a.system_id,
    '0',
    '0'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM
  From Dpa_Anagrafica_Log L
  Where L.Var_Codice = 'DOC_SIGNATURE_P'; 

  --MMM
  --INSERIMENTO NUOVA RAGIONE DI TRASMISSIONE IN LIBRO FIRMA
  INSERT
  INTO DPA_RAGIONE_TRASM
    (
      SYSTEM_ID,
      VAR_DESC_RAGIONE,
      CHA_TIPO_RAGIONE,
      CHA_VIS,
      CHA_TIPO_DIRITTI,
      CHA_TIPO_DEST,
      CHA_RISPOSTA,
      VAR_NOTE,
      CHA_EREDITA,
      CHA_TIPO_RISPOSTA,
      ID_AMM,
      VAR_NOTIFICA_TRASM,
      CHA_CEDE_DIRITTI,
      CHA_MANTIENI_LETT,
      CHA_MANTIENI_SCRITT,
      CHA_RAG_SISTEMA,
      CHA_PROC_RES
    )
    (SELECT SEQ.NEXTVAL,
        'FIRMA_DIGITALE_CADES',
        'W',
        '1',
        'W',
        'T',
        '0',
        'Ragione di trasmissione per firma digitale cades',
        '0',
        'C',
        a.system_id,
        NULL,
        'N',
        0,
        0,
        0,
        'DOC_SIGNATURE'
      FROM dpa_amministra a
    );
	
	  INSERT
  INTO DPA_RAGIONE_TRASM
    (
      SYSTEM_ID,
      VAR_DESC_RAGIONE,
      CHA_TIPO_RAGIONE,
      CHA_VIS,
      CHA_TIPO_DIRITTI,
      CHA_TIPO_DEST,
      CHA_RISPOSTA,
      VAR_NOTE,
      CHA_EREDITA,
      CHA_TIPO_RISPOSTA,
      ID_AMM,
      VAR_NOTIFICA_TRASM,
      CHA_CEDE_DIRITTI,
      CHA_MANTIENI_LETT,
      CHA_MANTIENI_SCRITT,
      CHA_RAG_SISTEMA,
      CHA_PROC_RES
    )
    (SELECT SEQ.NEXTVAL,
        'FIRMA_DIGITALE_PADES',
        'W',
        '1',
        'W',
        'T',
        '0',
        'Ragione di trasmissione per firma digitale pades',
        '0',
        'C',
        a.system_id,
        NULL,
        'N',
        0,
        0,
        0,
        'DOC_SIGNATURE_P'
      FROM dpa_amministra a
    );
	
  INSERT
  INTO DPA_RAGIONE_TRASM
    (
      SYSTEM_ID,
      VAR_DESC_RAGIONE,
      CHA_TIPO_RAGIONE,
      CHA_VIS,
      CHA_TIPO_DIRITTI,
      CHA_TIPO_DEST,
      CHA_RISPOSTA,
      VAR_NOTE,
      CHA_EREDITA,
      CHA_TIPO_RISPOSTA,
      ID_AMM,
      VAR_NOTIFICA_TRASM,
      CHA_CEDE_DIRITTI,
      CHA_MANTIENI_LETT,
      CHA_MANTIENI_SCRITT,
      CHA_RAG_SISTEMA,
      CHA_PROC_RES
    )
    (SELECT SEQ.NEXTVAL,
        'FIRMA_ELETTRONICA',
        'W',
        '1',
        'W',
        'T',
        '0',
        'Ragione di trasmissione per firma elettronica',
        '0',
        'C',
        a.system_id,
        NULL,
        'N',
        0,
        0,
        0,
        'DOC_VERIFIED'
      FROM dpa_amministra a
    );
  INSERT
  INTO DPA_RAGIONE_TRASM
    (
      SYSTEM_ID,
      VAR_DESC_RAGIONE,
      CHA_TIPO_RAGIONE,
      CHA_VIS,
      CHA_TIPO_DIRITTI,
      CHA_TIPO_DEST,
      CHA_RISPOSTA,
      VAR_NOTE,
      CHA_EREDITA,
      CHA_TIPO_RISPOSTA,
      ID_AMM,
      VAR_NOTIFICA_TRASM,
      CHA_CEDE_DIRITTI,
      CHA_MANTIENI_LETT,
      CHA_MANTIENI_SCRITT,
      CHA_RAG_SISTEMA,
      CHA_PROC_RES
    )
    (SELECT SEQ.NEXTVAL,
        'AVANZAMENTO_ITER',
        'W',
        '0',
        'W',
        'T',
        '0',
        'Ragione di trasmissione inserimento in libro firma per avanzamento iter',
        '0',
        'C',
        a.system_id,
        NULL,
        'N',
        0,
        0,
        1,
        'DOC_STEP_OVER'
      FROM DPA_AMMINISTRA A
    );
	
  INSERT
  INTO DPA_RAGIONE_TRASM
    (
      SYSTEM_ID,
      VAR_DESC_RAGIONE,
      CHA_TIPO_RAGIONE,
      CHA_VIS,
      CHA_TIPO_DIRITTI,
      CHA_TIPO_DEST,
      CHA_RISPOSTA,
      VAR_NOTE,
      CHA_EREDITA,
      CHA_TIPO_RISPOSTA,
      ID_AMM,
      VAR_NOTIFICA_TRASM,
      CHA_CEDE_DIRITTI,
      CHA_MANTIENI_LETT,
      CHA_MANTIENI_SCRITT,
      CHA_RAG_SISTEMA,
      CHA_PROC_RES
    )
    (SELECT SEQ.NEXTVAL,
        'PASSO_DI_PROCESSO',
        'N',
        '0',
        'W',
        'T',
        '0',
        'Ragione di trasmissione di sistema per l''evento di passo di processo',
        '0',
        'C',
        a.system_id,
        NULL,
        'N',
        0,
        0,
        1,
        'EVENT'
      FROM DPA_AMMINISTRA A
    );
	

	
  --INSERIMENTO IN ANAGRAFICA LOG DEGLI EVENTI DI INTERRUZIONE
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE',
    'Interruzione del processo di firma del documento da parte del proponente',
    'DOCUMENTO',
    'INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'RED'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'OBB'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE';
  
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE',
    'Interruzione del processo di firma dell''allegato da parte del proponente',
    'ALLEGATO',
    'INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'RED'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'OBB'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE';
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE',
    'Interruzione del processo di firma del documento da parte del titolare',
    'DOCUMENTO',
    'INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'RED'
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'OBB'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE';
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE',
    'Interruzione del processo di firma dell''allegato da parte del titolare',
    'ALLEGATO',
    'INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'RED'
  FROM DPA_AMMINISTRA a ;
  
  --ATTIVAZIONE DEI LOG
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'OBB'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE';
  
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'CONCLUSIONE_PROCESSO_LF_ALLEGATO',
    'Conclusione del processo di firma dell''allegato',
    'ALLEGATO',
    'CONCLUSIONE_PROCESSO_LF_ALLEGATO',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'BLUE'
  FROM DPA_AMMINISTRA a ;
  
    --INSERIMENTO ASSERZIONE
     INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              L.SYSTEM_ID,
              L.var_descrizione,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM DPA_ANAGRAFICA_LOG L JOIN DPA_AMMINISTRA A ON L.ID_AMM = A.SYSTEM_ID
		 WHERE L.VAR_CODICE = 'CONCLUSIONE_PROCESSO_LF_ALLEGATO');
  
  --ATTIVAZIONE DEI LOG
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'CON'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'CONCLUSIONE_PROCESSO_LF_ALLEGATO';
  
  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'CONCLUSIONE_PROCESSO_LF_DOCUMENTO',
    'Conclusione del processo di firma del documento',
    'DOCUMENTO',
    'CONCLUSIONE_PROCESSO_LF_DOCUMENTO',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'BLUE'
  FROM DPA_AMMINISTRA a ;
  

  
  --INSERIMENTO ASSERZIONE
     INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              L.SYSTEM_ID,
              L.var_descrizione,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM DPA_ANAGRAFICA_LOG L JOIN DPA_AMMINISTRA A ON L.ID_AMM = A.SYSTEM_ID
		 WHERE L.VAR_CODICE = 'CONCLUSIONE_PROCESSO_LF_DOCUMENTO');
  
  --ATTIVAZIONE DEI LOG
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'CON'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'CONCLUSIONE_PROCESSO_LF_DOCUMENTO';

  INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'TRONCAMENTO_PROCESSO',
    'Toncamento del processo di firma',
    'DOCUMENTO',
    'TRONCAMENTO_PROCESSO',
    'ONE',
    a.system_id,
    '1',
    '1',
    'USERS_ROLE_IN_PROCESS',
    'BLUE'
  FROM DPA_AMMINISTRA a ;
  

  
  --INSERIMENTO ASSERZIONE
     INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              L.SYSTEM_ID,
              L.var_descrizione,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM DPA_ANAGRAFICA_LOG L JOIN DPA_AMMINISTRA A ON L.ID_AMM = A.SYSTEM_ID
		 WHERE L.VAR_CODICE = 'TRONCAMENTO_PROCESSO');
  
  --ATTIVAZIONE DEI LOG
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'CON'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'TRONCAMENTO_PROCESSO';

    INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO',
    'Avviato processo di firma per il documento',
    'DOCUMENTO',
    'AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO',
    null,
    a.system_id,
    '0',
    '0',
    null,
    null
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'NN'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO';
  
    INSERT
  INTO DPA_ANAGRAFICA_LOG
    (
      SYSTEM_ID,
      VAR_CODICE,
      VAR_DESCRIZIONE,
      VAR_OGGETTO,
      VAR_METODO,
      MULTIPLICITY,
      ID_AMM,
      NOTIFICATION,
      CONFIGURABLE,
      NOTIFICATION_RECIPIENTS,
      COLOR
    )
  SELECT seq.nextval,
    'AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO',
    'Avviato processo di firma per l''allegato',
    'DOCUMENTO',
    'AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO',
    null,
    a.system_id,
    '0',
    '0',
    null,
    null
  FROM DPA_AMMINISTRA a ;
  
    
  INSERT
  INTO DPA_LOG_ATTIVATI
    (
      SYSTEM_ID_ANAGRAFICA,
      ID_AMM,
      NOTIFY
    )
  SELECT L.SYSTEM_ID,
    L.ID_AMM,
    'NN'
  FROM DPA_ANAGRAFICA_LOG L
  WHERE L.VAR_CODICE = 'AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO';