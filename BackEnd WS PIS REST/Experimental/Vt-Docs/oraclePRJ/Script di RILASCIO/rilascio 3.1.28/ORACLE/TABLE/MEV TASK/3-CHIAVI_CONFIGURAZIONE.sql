begin
  Utl_Insert_Chiave_Config('ENABLE_TASK','Abilita l''utilizzo della funzionalità TASK'
  ,'1','B','0'
  ,'0','1','3.1.27'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;

begin
  Utl_Insert_Chiave_Config('ENABLE_COLOR_MULTIVALUE','Abilita la selezione del colore per i campi multivalore'
  ,'1','B','0'
  ,'0','1','3.1.28'
  ,NULL, NULL, NULL);
end; 
/
COMMIT;

--MICRO FUNZIONE
--TASK
  INSERT INTO DPA_TIPO_FUNZIONE
	(SELECT SEQ.NEXTVAL,
              'TASK',
              'TASK',
              '1',
              a.system_id
         FROM DPA_AMMINISTRA A);
         
  INSERT INTO DPA_FUNZIONI
    (SELECT seq.NEXTVAL,
        a.system_id,
        'DO_TASK',
        'DO_TASK',
        null,
        null,
        null,
        null,
        f.system_id
     FROM DPA_AMMINISTRA A, DPA_TIPO_FUNZIONE F
     WHERE F.VAR_COD_TIPO ='TASK' AND F.ID_AMM = A.SYSTEM_ID);