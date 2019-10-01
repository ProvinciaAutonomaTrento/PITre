BEGIN
	
	-- Aggiunta policy
	Utl_Insert_Chiave_Log (
	'AMM_POLICY_PARER_NEW' , --Codice              
    'Creazione nuova policy PARER', --Descrizione         
    'CONSERVAZIONE_PARER', --oggetto             
    'AMMPOLICYPARERNEW', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.22',   -- Myversione_Cd        
    Null      --RFU  
	);
	
	-- Modifica policy
	Utl_Insert_Chiave_Log (
	'AMM_POLICY_PARER_MOD' , --Codice              
    'Modifica policy PARER', --Descrizione         
    'CONSERVAZIONE_PARER', --oggetto             
    'AMMPOLICYPARERMOD', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.22',   -- Myversione_Cd        
    Null      --RFU  
	);
	
	-- Eliminazione policy
	Utl_Insert_Chiave_Log (
	'AMM_POLICY_PARER_DEL' , --Codice              
    'Eliminazione policy PARER', --Descrizione         
    'CONSERVAZIONE_PARER', --oggetto             
    'AMMPOLICYPARERDEL', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.22',   -- Myversione_Cd        
    Null      --RFU  
	);
	
	-- Attivazione policy
	Utl_Insert_Chiave_Log (
	'AMM_POLICY_PARER_ON' , --Codice              
    'Attivazione policy PARER', --Descrizione         
    'CONSERVAZIONE_PARER', --oggetto             
    'AMMPOLICYPARERON', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.22',   -- Myversione_Cd        
    Null      --RFU  
	);
	
	-- Disattivazione policy
	Utl_Insert_Chiave_Log (
	'AMM_POLICY_PARER_OFF' , --Codice              
    'Disattivazione policy PARER', --Descrizione         
    'CONSERVAZIONE_PARER', --oggetto             
    'AMMPOLICYPAREROFF', --Metodo              
    Null,     --Forza_Aggiornamento 
    '3.1.22',   -- Myversione_Cd        
    Null      --RFU  
	);

END;