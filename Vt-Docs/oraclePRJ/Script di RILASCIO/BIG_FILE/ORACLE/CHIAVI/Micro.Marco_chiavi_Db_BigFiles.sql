
BEGIN
   UTL_INSERT_CHIAVE_CONFIG(  'FE_DO_SEND_BIG_FILE',
    'Dimensione massima in byte consentita per la spedizione via mail o pec'
  ,'31457280','F','1'
  ,'1','0','3.2.7'
  ,NULL, NULL, NULL);


  UTL_INSERT_CHIAVE_CONFIG(   'FE_DO_BIG_FILE_MIN',
    'Se non è null e maggiore di 0 definisce la dimensione in byte da cui un file è definito di tipo "big file"',
    '31457280','F','1'
  ,'1','0','3.2.7'
  ,NULL, NULL, NULL);
  
    UTL_INSERT_CHIAVE_CONFIG(  'FE_DO_BIG_FILE_MAX',
    'Deve essere maggiore di FE_DO_BIG_FILE_MIN e impone il limite di dimensione  in byte accettata da big file',
    '104857600','F','1'
  ,'1','0','3.2.7'
  ,NULL, NULL, NULL);
  
    UTL_INSERT_CHIAVE_CONFIG(   'FE_PREVIEW_MB_LIMIT',
    'Se null o 0 spegne le anteprime ed il suo valore (maggiore di 0) definisce il valore minimo in mb dal quale i pdf vengono visualizzati in anteprime',
    '3','F','1'
  ,'1','0','3.2.7'
  ,NULL, NULL, NULL);
  
     UTL_INSERT_CHIAVE_CONFIG(  'FE_PREVIEW_PDF_UP_DOWN',
    'ATTIVA DISATTIVA PULSANTI DI NAVIGAZIONE TRA LE ANTEPRIME PDF',
    '1','F','1'
  ,'1','0','3.2.7'
  ,NULL, NULL, NULL);
  

       UTL_INSERT_CHIAVE_CONFIG(  'BE_PREVIEW_PG',
    'NUMERO PAGINE DI UNA ANTEPRIMA BIG FILE PDF',
    '5','F','1'
  ,'1','1','3.2.7'
  ,NULL, NULL, NULL);


  
  end;
  /

BEGIN
Utl_Insert_Chiave_Microfunz('DOWNLOAD_BIG_FILE'
,' aBILITA il download di big file tramite i pulsanti "salva in locale" e "versione per stampa"'
,Null,'N',Null,'3.2.7',Null);


  INSERT INTO DPA_TIPO_FUNZIONE
              (SELECT SEQ.NEXTVAL,
              'DOWNLOAD_BIG_FILE',
              'DOWNLOAD_BIG_FILE',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A);
       
                 
  INSERT INTO DPA_FUNZIONI
    (SELECT seq.NEXTVAL,
        a.system_id,
        'DOWNLOAD_BIG_FILE',
        'DOWNLOAD_BIG_FILE',
        null,
        null,
        null,
        null,
        f.system_id
     FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
     WHERE F.VAR_COD_TIPO ='DOWNLOAD_BIG_FILE' AND F.ID_AMM = A.SYSTEM_ID);

End;
/
begin
Utl_Insert_Chiave_Microfunz('DO_ENABLE_BIGFILE','Abilita utente per utilizzo della app client per upload big file'
,Null,'N',Null,'3.2.7',Null);


  INSERT INTO DPA_TIPO_FUNZIONE
              (SELECT SEQ.NEXTVAL,
              'UPLOAD_BIG_FILE',
              'UPLOAD_BIG_FILE',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A);
       
                 
  INSERT INTO DPA_FUNZIONI
    (SELECT seq.NEXTVAL,
        a.system_id,
        'DO_ENABLE_BIGFILE',
        'DO_ENABLE_BIGFILE',
        null,
        null,
        null,
        null,
        f.system_id
     FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
     WHERE F.VAR_COD_TIPO ='UPLOAD_BIG_FILE' AND F.ID_AMM = A.SYSTEM_ID);

End;
/


