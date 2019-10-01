--EMANUELA: inserimento log per quando aggiungo/rimuovo un documento in sottofascicolo e quando creo/modifico/elimino un sottofascicolo

Begin 
Utl_Insert_Chiave_Log(
    'MODIFY_FOLDER_FASC' , --Codice               
    'Modificata la descrizione del folder di un fascicolo', --Descrizione          
    'FASCICOLO', --oggetto              
    'MODIFY_FOLDER_FASC', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.1.8',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;



Begin 
Utl_Insert_Chiave_Log(
    'DELETE_FOLDER_FASC' , --Codice               
    'Rimosso folder dal fascicolo', --Descrizione          
    'FASCICOLO', --oggetto              
    'DELETE_FOLDER_FASC', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.1.8',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;



INSERT INTO DPA_LOG_ATTIVATI
    (SELECT
        l.system_id,
        am.system_id,
        'CON'
FROM DPA_AMMINISTRA am, DPA_ANAGRAFICA_LOG l
    WHERE l.var_codice = 'MODIFY_FOLDER_FASC'
    );
	
	
	
INSERT INTO DPA_LOG_ATTIVATI
    (SELECT
        l.system_id,
        am.system_id,
        'CON'
FROM DPA_AMMINISTRA am, DPA_ANAGRAFICA_LOG l
    WHERE l.var_codice = 'DELETE_FOLDER_FASC'
    );