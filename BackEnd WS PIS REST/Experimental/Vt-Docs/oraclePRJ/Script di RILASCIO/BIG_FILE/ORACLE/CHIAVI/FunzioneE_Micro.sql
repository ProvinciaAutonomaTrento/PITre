--CREA LA MICRO
begin
Utl_Insert_Chiave_Microfunz('DO_ENABLE_BIGFILE'
,'Abilita il pulsante per la gestione dei file di grandi dimensioni'
,Null,'N',Null,'3.2.4',Null);
End;
-- Cheidere a luca se creare una nuova macro 
--CREA LA MACRO
  INSERT INTO DPA_TIPO_FUNZIONE
                (SELECT SEQ.NEXTVAL,
              'DO_ENABLE_BIGFILE',
              'Abilita il pulsante per la gestione dei file di grandi dimensioni',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A);
       
--INSERISCE LA MICRO NELLA MACRO
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
     WHERE F.VAR_COD_TIPO ='DO_ENABLE_BIGFILE' AND F.ID_AMM = A.SYSTEM_ID);