/*
Script per l'inserimento della chiave di DB 'BE_DISABLE_PROC_GEN_DSN_SEND'
*/
DECLARE @RC int
DECLARE @Codice varchar(100)
DECLARE @Descrizione varchar(2000)
DECLARE @Valore varchar(128)
DECLARE @Tipo_Chiave varchar(1)
DECLARE @Visibile varchar(1)
DECLARE @Modificabile varchar(1)
DECLARE @Globale varchar(1)
DECLARE @myversione_CD varchar(32)
DECLARE @Codice_Old_Webconfig varchar(128)
DECLARE @Forza_Update varchar(1)
DECLARE @RFU varchar(10)

-- TODO: impostare qui i valori dei parametri.

EXECUTE @RC = [DOCSADM].[Utl_Insert_Chiave_Config] 
   'BE_DISABLE_PROC_GEN_DSN_SEND'
  ,'Disabilita il processamento di mail DSN di un mittente. Valori: indirizzo mittente/false per disabilitare il filtro'
  ,'false'
  ,'B'
  ,'1'
  ,'1'
  ,'1'
  ,'3.30.1.5'
  ,NULL
  ,'1'
  ,NULL
GO
