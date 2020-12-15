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
           ,'FE_LINEARIZZE_PDF'
           ,'Abilita la visualizzazione linearizzata dei file pdf'
           ,'1'
           ,'F'
           ,'1'
           ,'1'
           ,'1'
           ,null
           ,SYSDATETIME()
           ,'3.1.28'
           ,null)