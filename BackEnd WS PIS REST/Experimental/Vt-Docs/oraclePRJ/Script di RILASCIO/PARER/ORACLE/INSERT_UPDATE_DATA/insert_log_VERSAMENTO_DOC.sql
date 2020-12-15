Begin
Utl_Insert_Chiave_Log(
    'VERSAMENTO_DOC' , --Codice              
    'Versamento in conservazione di un documento', --Descrizione         
    'DOCUMENTO', --oggetto             
    'VERSAMENTO_DOC', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.x',   -- Myversione_Cd        
    Null      --RFU                 
    );
End;