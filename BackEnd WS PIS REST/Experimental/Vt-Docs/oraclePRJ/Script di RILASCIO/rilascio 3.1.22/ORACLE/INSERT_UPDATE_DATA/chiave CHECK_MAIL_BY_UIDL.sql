BEGIN
  Utl_Insert_Chiave_Config('BE_CHECK_MAIL_BY_UIDL','Se diversa da 0, lo scarico della mail viene effettuato sfruttando l''UIDL e non l''indice'
  ,'1','B','1'
  ,'1','1','3.1.23'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;
