begin
  Utl_Insert_Chiave_Config('MAX_YEARS_SEARCHABLE','Se diversa da 0, indica il numero di anni entro cui deve essere compresa la data di creazione dei documenti'
  ,'2','B','1'
  ,'1','1','3.1.22'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;


begin
  Utl_Insert_Chiave_Config('MAX_INTERVAL_DATE_SEARCHABLE','Se diversa da 0, indica l''intervallo massimo entro cui deve essere compresa la data di creazione dei documenti'
  ,'2','B','1'
  ,'1','0','3.1.22'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;
