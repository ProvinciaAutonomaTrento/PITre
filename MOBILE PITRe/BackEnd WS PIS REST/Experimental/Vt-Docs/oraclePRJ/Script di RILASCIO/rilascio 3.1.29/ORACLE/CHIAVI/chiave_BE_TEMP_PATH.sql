--INSERIRE IL PATH PER I FILE TEMPORANEI
BEGIN
  Utl_Insert_Chiave_Config('BE_TEMP_PATH','Path dei file temporanei'
  ,'C:\Sviluppo\Temp','B','0'
  ,'0','1','3.1.29'
  ,NULL, NULL, NULL);
END;
/
COMMIT;

