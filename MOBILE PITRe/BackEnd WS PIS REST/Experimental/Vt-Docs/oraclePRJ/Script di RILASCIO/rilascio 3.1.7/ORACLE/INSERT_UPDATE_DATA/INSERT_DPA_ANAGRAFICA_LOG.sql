
/*Emanuela 04-06-2014: inserimenti in DPA_ANAGRAFICA_LOG per tracciare il visto sulle notifiche di rifiuto*/
Begin 
Utl_Insert_Chiave_Log(
    'CHECK_REJECT_DOCUMENT' , --Codice               
    'Vistata notifica di rifiuto del documento', --Descrizione          
    'DOCUMENTO', --oggetto              
    'CHECK_REJECT_DOCUMENT', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.1.7',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;


Begin 
Utl_Insert_Chiave_Log(
    'CHECK_REJECT_FOLDER' , --Codice               
    'Vistata notifica di rifiuto del fascicolo', --Descrizione          
    'FASCICOLO', --oggetto              
    'CHECK_REJECT_FOLDER', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.1.7',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;

/*Emanuela: attivazione dei 2 log precedenti per tutte le amministrazioni*/
INSERT INTO DPA_LOG_ATTIVATI
	(SELECT
		l.system_id,
		am.system_id,
		'CON'
FROM DPA_AMMINISTRA am, DPA_ANAGRAFICA_LOG l
	WHERE l.var_codice = 'CHECK_REJECT_DOCUMENT'
	);
		
		
INSERT INTO DPA_LOG_ATTIVATI
	(SELECT
		l.system_id,
		am.system_id,
		'CON'
FROM DPA_AMMINISTRA am, DPA_ANAGRAFICA_LOG l
	WHERE l.var_codice = 'CHECK_REJECT_FOLDER'
	);

