begin
Utl_Insert_Chiave_Microfunz('GEST_PROCESSI_DI_FIRMA'
,'Abilita il sottomenu'' Processi di firma del menu'' gestione'
,Null,'N',Null,'3.1.17',Null);
End;


begin
Utl_Insert_Chiave_Microfunz('DO_DOC_FIRMA_ELETTRONICA'
,'Abilita il pulsante della firma elettronica di un file'
,Null,'N',Null,'3.1.17',Null);
End;


begin
Utl_Insert_Chiave_Microfunz('DO_DOC_AVANZAMENTO_ITER'
,'Abilita l''apposizione di firma elettronica con valenza di Avanzamento Iter'
,Null,'N',Null,'3.1.17',Null);
End;


begin
Utl_Insert_Chiave_Microfunz('DO_LIBRO_FIRMA'
,'Abilita il tab Libro firma nell''home page'
,Null,'N',Null,'3.1.17',Null);
End;


begin
Utl_Insert_Chiave_Microfunz('DO_START_SIGNATURE_PROCESS'
,'Abilita nel tab profilo/allegato il bottone per l''avvio di un processo di firma'
,Null,'N',Null,'3.1.17',Null);
End;

begin
Utl_Insert_Chiave_Microfunz('DO_STATE_SIGNATURE_PROCESS'
,'Abilita nel tab profilo/allegato il bottone per la visualizzazione dello stato del processo di firma'
,Null,'N',Null,'3.1.17',Null);
End;

begin
	  Utl_Insert_Chiave_Config('FE_LIBRO_FIRMA','Abilita la gestione del Libro Firma'
	  ,'1','F','0'
	  ,'0','1','3.1.18'
	  ,SYSDATE, NULL, NULL);
end; 

begin
	  Utl_Insert_Chiave_Config('FE_LIBRO_FIRMA_COLUMN_RECIPIENT','Abilita la visualizzazione della colonna destinatario nella griglia del tab Libro Firma'
	  ,'0','F','0'
	  ,'0','1','3.1.18'
	  ,SYSDATE, NULL, NULL);
end; 

begin
	  Utl_Insert_Chiave_Config('FE_LIBRO_FIRMA_COLUMN_TYPOLOGY','Abilita la visualizzazione della colonna tipologia nella griglia del tab Libro Firma'
	  ,'0','F','0'
	  ,'0','1','3.1.18'
	  ,SYSDATE, NULL, NULL);
end;

begin
	  Utl_Insert_Chiave_Config('FE_RIC_DESC_ALLEGATI','Abilita la ricerca degli allegati inserendo la descrizione del documento principale'
	  ,'1','F','0'
	  ,'0','1','3.1.18'
	  ,SYSDATE, NULL, NULL);
end;  