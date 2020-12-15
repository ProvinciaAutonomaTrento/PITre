BEGIN
  UTL_INSERT_CHIAVE_CONFIG('BE_CONTENT_TYPE_MIME','Elenco dei content-type, separati da '';'', che la chilkat non riconosce come allegati.'
  ,'text/xml','B','1'
  ,'1','0','3.2.9'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;



BEGIN
  UTL_INSERT_CHIAVE_CONFIG('BE_MESSAGE_SEND_PEC','Testo da inserire per spedizione via mail.'
  ,'Messaggio di posta certificata' || chr(10) || 'Se avete ricevuto documenti firmati digitalmente con estensione .p7m e non riuscite a visualizzarne il contenuto utilizzate il link' || chr(10) || ' https://www.servizionline.provincia.tn.it/portale/verifica_firma_digitale/1095/verifica_firma_digitale/368220'
  ,'B','1'
  ,'1','0','3.2.9'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;


