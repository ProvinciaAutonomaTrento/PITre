BEGIN

	Utl_Insert_Chiave_Microfunz('DO_CREATE_MODEL_PROCESS'
	,'Abilita la creazione di modelli di processo di firma'
	,Null,'N',Null,'3.2.8',Null);

	Utl_Insert_Chiave_Microfunz('DO_DISABLE_FIRMA_ELETTRONICA'
	,'Nasconde il pulsante per la firma elettronica dal dettaglio del documento'
	,Null,'N',Null,'3.2.8',Null);

END;
/
COMMIT;