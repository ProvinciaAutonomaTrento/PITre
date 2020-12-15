-- file sql di update per il CD --
---- DPA_CHIAVI_CONFIGURAZIONE.ORA.sql  marcatore per ricerca ----

-- BE_INDEX_SECURITY
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_INDEX_SECURITY';
    IF (cnt = 0) THEN       
  
insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
   ,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_INDEX_SECURITY'
  ,'Se valorizzata ad 1, viene aggiunto un hint SQL che forza luso dellindice sulla tabella SECURITY per migliorare le prestazioni delle select sulla tabella medesima'
  ,'1','B','1'
  ,'1','1');

    END IF;
    END;
END;
/
------------------

---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
Values    (seq.nextval, sysdate, '3.16.8');
end;
/
