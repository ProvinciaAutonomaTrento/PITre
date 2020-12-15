INSERT INTO DPA_CHIAVI_CONFIGURAZIONE
           (SYSTEM_ID
           ,ID_AMM
           ,VAR_CODICE
           ,VAR_DESCRIZIONE
           ,VAR_VALORE
           ,CHA_TIPO_CHIAVE
           ,CHA_VISIBILE
           ,CHA_MODIFICABILE
           ,CHA_GLOBALE
           ,VAR_CODICE_OLD_WEBCONFIG
           ,DTA_INSERIMENTO
           ,VERSIONE_CD
           ,CHA_CONSERVAZIONE)
     VALUES
           (SEQ_DPA_CHIAVI_CONFIG.NEXTVAL
           ,'0'
           ,'FE_LINEARIZZE_PDF'
           ,'Abilita la visualizzazione linearizzata dei file pdf'
           ,'1'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,null
           ,SYSDATE
           ,'3.1.28'
           ,'0')