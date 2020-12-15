Begin 
Utl_Insert_Chiave_Log(
    'SCAMBIA_DOC' , --Codice               
    'Scambia del documento', --Descrizione          
    'DOCUMENTO', --oggetto              
    'SCAMBIA_DOC', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.2.3',   -- Myversione_Cd         
    Null      --RFU                  
    );
	
	INSERT INTO DPA_LOG_ATTIVATI
    (SELECT
        l.system_id,
        am.system_id,
        'NN'
FROM DPA_AMMINISTRA am, DPA_ANAGRAFICA_LOG l
    WHERE l.var_codice = 'SCAMBIA_DOC'
    );
End;

/
COMMIT;


	