
--DA ABILITARE PER VERSIONI DI ORACLE PRECEDENTI ALLA 12C
BEGIN
  Utl_Insert_Chiave_Config('BE_INDICE_QUERY_RICERCHE','Chiave per abilitare l''utilizzo dell''indice nella query di ricerca'
  ,'1','B','0'
  ,'0','1','3.1.29'
  ,NULL, NULL, NULL);
END;
/
COMMIT;