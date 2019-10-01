-- MIBACT	-----
BEGIN

	Utl_Insert_Chiave_Microfunz('DO_RIC_ESTENDI_NODI_FIGLI_E_FASC'
	,'Estende la ricerca ai nodi figli e ad i fascicoli del nodo selezionato'
	,Null,'N',Null,'3.2.9',Null);
END;
/
COMMIT;


BEGIN
	Utl_Insert_Chiave_Microfunz('DO_DESCRIZIONI_FASC'
	,'Abilita il pulsante di selezione di una descrizione nella pagina del fascicolo'
	,Null,'N',Null,'3.2.9',Null);
	
	Utl_Insert_Chiave_Microfunz('DO_DESCRIZIONI_FASC_INSERT'
	,'Abilita la funzionalità di inserimento di una descrizione di fascicolo'
	,Null,'N',Null,'3.2.9',Null);
	
	Utl_Insert_Chiave_Microfunz('DO_DESCRIZIONI_FASC_INSERT_REG'
	,'Abilita la funzionalità di inserimento di una descrizione di fascicolo nel registro'
	,Null,'N',Null,'3.2.9',Null);
END;
/
COMMIT;