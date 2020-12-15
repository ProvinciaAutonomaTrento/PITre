--CREA LA MICRO
begin
Utl_Insert_Chiave_Microfunz('PREVIEW_FULL_DOWNLOAD'
,'Abilita il bottone per lo scarico del file completo nel caso di antemprima'
,Null,'N',Null,'3.1.23',Null);
End;
-- Cheidere a luca se creare una nuova macro 
--CREA LA MACRO
  INSERT INTO DPA_TIPO_FUNZIONE
                (SELECT SEQ.NEXTVAL,
              'FULL_DOWNLOAD',
              'ABILITA DOWNLOAD COMPLETO IN ANTEPRIMA',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A);
       
--INSERISCE LA MICRO NELLA MACRO
  INSERT INTO DPA_FUNZIONI
    (SELECT seq.NEXTVAL,
        a.system_id,
        'PREVIEW_FULL_DOWNLOAD',
        'PREVIEW_FULL_DOWNLOAD',
        null,
        null,
        null,
        null,
        f.system_id
     FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
     WHERE F.VAR_COD_TIPO ='FULL_DOWNLOAD' AND F.ID_AMM = A.SYSTEM_ID);