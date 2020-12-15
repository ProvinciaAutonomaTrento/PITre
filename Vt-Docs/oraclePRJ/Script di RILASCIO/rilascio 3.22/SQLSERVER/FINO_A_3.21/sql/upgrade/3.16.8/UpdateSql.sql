-- BE_INDEX_SECURITY

if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_INDEX_SECURITY'))
BEGIN	
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES (0
           ,'BE_INDEX_SECURITY'
           ,'Se valorizzata ad 1, viene aggiunto un hint SQL che forza l’uso dell’indice sulla tabella SECURITY per migliorare le prestazioni delle select sulla tabella medesima'
           ,'1'
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO 

-- versione upgrade

Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.16.8')
GO


