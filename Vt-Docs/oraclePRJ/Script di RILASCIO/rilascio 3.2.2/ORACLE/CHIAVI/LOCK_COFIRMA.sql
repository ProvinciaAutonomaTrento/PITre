BEGIN
  UTL_INSERT_CHIAVE_CONFIG('FE_SET_TIPO_FIRMA','Tipo firma da eseguire in base al valore della chiave: 0 o default(Annidata), 1(Parallela), 2(Annidata non modificabile), 3(Parallela non modificabile)'
  ,'0','F','1'
  ,'1','1','3.2.2'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;