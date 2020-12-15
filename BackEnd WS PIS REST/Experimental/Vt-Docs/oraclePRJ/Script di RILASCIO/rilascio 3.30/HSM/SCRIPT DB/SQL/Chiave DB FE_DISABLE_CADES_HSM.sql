/*
Script per l'inserimento della chiave di DB 'FE_DISABLE_CADES_HSM'
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
   'FE_DISABLE_CADES_HSM'
  ,'Disabilita il radio button Cades nella maschera HSM'
  ,'1'
  ,'F'
  ,'1'
  ,'1'
  ,'0'
  ,'3.30.0'
  ,NULL
  ,'1'
  ,NULL
GO
