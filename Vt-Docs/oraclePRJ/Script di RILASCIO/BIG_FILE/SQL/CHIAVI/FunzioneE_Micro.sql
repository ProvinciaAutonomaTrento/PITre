INSERT  INTO dpa_anagrafica_funzioni
        ( COD_FUNZIONE,VAR_DESC_FUNZIONE
        ,CHA_TIPO_FUNZ,DISABLED )
        Values
        ( 'DO_ENABLE_BIGFILE','Abilita il pulsante per la gestione dei file di grandi dimensioni.'          
        ,null ,'N' )

--CREA LA MACRO
  INSERT INTO DPA_TIPO_FUNZIONE
                SELECT 'DO_ENABLE_BIGFILE',
              'Abilita il pulsante per la gestione dei file di grandi dimensioni.',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A
       
--INSERISCE LA MICRO NELLA MACRO
  INSERT INTO DPA_FUNZIONI
    SELECT a.system_id,
        'DO_ENABLE_BIGFILE',
        'DO_ENABLE_BIGFILE',
        null,
        null,
        null,
        null,
        f.system_id
     FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
     WHERE F.VAR_COD_TIPO ='DO_ENABLE_BIGFILE' AND F.ID_AMM = A.SYSTEM_ID