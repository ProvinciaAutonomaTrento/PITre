INSERT INTO DOCSADM.DPA_CHIAVI_CONFIGURAZIONE
           (ID_AMM
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
           ('0'
           ,'FE_ENABLE_DRAG_AND_DROP'
           ,'Abilita la funzionalita drag and drop in ADL'
           ,'1'
           ,'F'
           ,'0'
           ,'0'
           ,'1'
           ,null
           ,SYSDATETIME()
           ,'3.1.30'
           ,null)