begin
  Utl_Insert_Chiave_Config('BLOCCO_MODIFICHE_DOC_IN_LF','Se ad 1, attiva il blocco delle modifiche del documento principale e suoi allegati nel caso in cui e attivo un processo di firma per il documento principale'
  ,'1','F','1'
  ,'1','0','3.2.13'
  ,NULL, NULL, NULL);
end; 

begin
  Utl_Insert_Chiave_Config('NOTIFICA_DEST_NO_INTEROP_OBB','Se ad 1, la notifica di presenza di destinatari non interoperanti è obbligatoria'
  ,'1','F','1'
  ,'1','0','3.2.13'
  ,NULL, NULL, NULL);
end; 