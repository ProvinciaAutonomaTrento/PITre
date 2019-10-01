CREATE OR REPLACE
PROCEDURE INITEVENTTRASM
IS
  rag        VARCHAR2(32 BYTE);
  desc_rag   VARCHAR2(32 BYTE);
  id_amm_rag NUMBER(10,0);
  --field for cursor POPULATES_FIELD_DESC_PRODUCER
  desc_prod VARCHAR2(500 BYTE);
  id_event  NUMBER(10,0);
  -- cursor to populate the field desc_producer in dpa_log
  CURSOR POPULATES_FIELD_DESC_PRODUCER
  IS
    SELECT l.system_id,
      g.GROUP_NAME
      || '('
      || u.FULL_NAME
      || ')'
    FROM DPA_LOG l
    JOIN PEOPLE u
    ON l.id_people_operatore = u.system_id
    LEFT JOIN groups g
    ON l.id_gruppo_operatore = g.system_id;
  CURSOR LIST_RAG_TRASM
  IS
    SELECT DISTINCT id_amm,
      UPPER(REPLACE(var_desc_ragione, ' ', '_')),
      var_desc_ragione
    FROM dpa_ragione_trasm
    WHERE id_amm IS NOT NULL;
BEGIN
  -- delete old event on trasmisssion document and folder
  --delete old event trasmission document or folder
  DELETE
  FROM DPA_LOG_ATTIVATI
  WHERE system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'DOC_TRASMESSO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'TRASM_DOC'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'FASC_TRASMESSO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'MOD_OGGETTO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'TASTO_VISTO_FASC'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'DOCUMENTOCONVERSIONEPDF'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'TASTO_VISTO_DOC'
    );
  --bonifica dpa_anagrafica_log
  UPDATE DPA_ANAGRAFICA_LOG
  SET VAR_METODO    = VAR_CODICE
  WHERE VAR_METODO IS NULL;
  --elimino il vecchio evento di conversione pdf
  DELETE DPA_ANAGRAFICA_LOG
  WHERE var_codice = 'DOCUMENTOCONVERSIONEPDF';
  --creo il nuovo evento di conversione pdf
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'DOCUMENTOCONVERSIONEPDF',
      'Conversione in pdf del documento',
      'DOCUMENTO',
      'DOCUMENTOCONVERSIONEPDF',
      'ALL',
      NULL,
      '1',
      '1',
      'S_PRODUCER_EVENT',
      'BLUE'
    );
  -- creo gli eventi di accettazione, rifiuto, visto trasmissione
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      SEQ.nextval,
      'ACCEPT_TRASM_FOLDER',
      'Effettuata accettazione trasmissione del fascicolo',
      'FASCICOLO',
      'ACCEPTTRASMFOLDER',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION ',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'ACCEPT_TRASM_DOCUMENT',
      'Effettuata accettazione trasmissione del documento',
      'DOCUMENTO',
      'ACCEPTTRASMDOCUMENT',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'REJECT_TRASM_FOLDER',
      'Effettuato rifiuto trasmissione del fascicolo',
      'FASCICOLO',
      'REJECTTRASMFOLDER',
      'ONE',
      NULL,
      '1',
      '0',
      'S_SENDER_USERS_ROLE_SENDER_TRANSMISSION',
      'RED'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'REJECT_TRASM_DOCUMENT',
      'Effettuato rifiuto trasmissione del documento',
      'DOCUMENTO',
      'REJECTTRASMDOCUMENT',
      'ONE',
      NULL,
      '1',
      '0',
      'S_SENDER_USERS_ROLE_SENDER_TRANSMISSION',
      'RED'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'CHECK_TRASM_FOLDER',
      'Visto il dettaglio trasmissione del fascicolo',
      'FASCICOLO',
      'CHECKTRASMFOLDER',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'CHECK_TRASM_DOCUMENT',
      'Visto il dettaglio trasmissione del documento',
      'DOCUMENTO',
      'CHECKTRASMDOCUMENT',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'MODIFIED_OBJECT_PROTO',
      'Modificato oggetto del Protocollo',
      'DOCUMENTO',
      'MODIFIEDOBJECTPROTO',
      'ONE',
      NULL,
      '1',
      '1',
      'ACL_DOCUMENT',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'MODIFIED_OBJECT_DOC',
      'Modificato oggetto del Documento',
      'DOCUMENTO',
      'MODIFIEDOBJECTDOC',
      'ONE',
      NULL,
      '0',
      '0',
      'ACL_DOCUMENT',
      'BLUE'
    );
  DELETE FROM DPA_ANAGRAFICA_LOG WHERE VAR_CODICE = 'DOC_CAMBIO_STATO';
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'DOC_CAMBIO_STATO',
      'Cambio stato del documento',
      'DOCUMENTO',
      'DOC_CAMBIO_STATO',
      'ONE',
      NULL,
      '1',
      '1',
      'ACL_DOCUMENT',
      'BLUE'
    );
    
    --evento mancata consegna ricevuta per interoperabilità semplificata
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY',
      'Ricevuta di mancata consegna per interoperabilità semplificata',
      'DOCUMENTO',
      'NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY',
      'ONE',
      NULL,
      '1',
      '1',
      'USERS_ROLE_PRODUCER',
      'BLUE'
    );
    
  OPEN LIST_RAG_TRASM;
  LOOP
    FETCH LIST_RAG_TRASM INTO id_amm_rag, rag, desc_rag;
    EXIT
  WHEN LIST_RAG_TRASM%NOTFOUND ;
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
      (
        seq.nextval,
        'TRASM_DOC_'
        || rag,
        'Effettuata trasmissione documento con ragione '
        || desc_rag,
        'DOCUMENTO',
        'TRASM_DOC_'
        || rag,
        NULL,
        id_amm_rag,
        '1',
        '0',
        'TRANSMISSION_RECIPIENTS',
        'BLUE'
      );
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
      (
        seq.nextval,
        'TRASM_FOLDER_'
        || rag,
        'Effettuata trasmissione fascicolo con ragione '
        || desc_rag,
        'FASCICOLO',
        'TRASM_FOLDER_'
        || rag,
        NULL,
        id_amm_rag,
        '1',
        '0',
        'TRANSMISSION_RECIPIENTS',
        'BLUE'
      );    
  END LOOP;
  CLOSE LIST_RAG_TRASM;
  OPEN POPULATES_FIELD_DESC_PRODUCER;
  LOOP
    FETCH POPULATES_FIELD_DESC_PRODUCER INTO id_event, desc_prod;
    EXIT
  WHEN POPULATES_FIELD_DESC_PRODUCER%NOTFOUND ;
    UPDATE DPA_LOG SET DESC_PRODUCER = desc_prod WHERE SYSTEM_ID = id_event;
  END LOOP;
  CLOSE POPULATES_FIELD_DESC_PRODUCER;
  COMMIT;
END;