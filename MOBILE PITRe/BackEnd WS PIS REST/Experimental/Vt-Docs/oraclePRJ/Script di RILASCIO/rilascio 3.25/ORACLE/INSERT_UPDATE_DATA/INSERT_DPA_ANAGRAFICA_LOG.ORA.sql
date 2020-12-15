Begin 
-- Lorusso per tasto visto
Utl_Insert_Chiave_Log(
    'TASTO_VISTO_DOC' , --Codice               
    'Selezione del tasto visto nel dettaglio trasmissione di un documento', --Descrizione          
    'DOCUMENTO', --oggetto              
    'TASTO_VISTO_DOC', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.25',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;    
/

Begin 
-- Lorusso per tasto visto
Utl_Insert_Chiave_Log(
    'TASTO_VISTO_FASC' , --Codice               
    'Selezione del tasto visto nel dettaglio trasmissione di un fascicolo', --Descrizione          
    'FASCICOLO', --oggetto              
    'TASTO_VISTO_FASC', --Metodo               
    Null,     --Forza_Aggiornamento  
    '3.25',   -- Myversione_Cd         
    Null      --RFU                  
    );
End;    
/
