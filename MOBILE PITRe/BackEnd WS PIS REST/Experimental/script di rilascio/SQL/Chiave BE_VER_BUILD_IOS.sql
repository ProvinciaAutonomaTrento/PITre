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

-- TODO: Set parameter values here.
SET @Codice = 'BE_VER_BUILD_IOS'
SET @Descrizione = 'Versione della build ios rilasciata in produzione'
SET @Valore = '8.1'
SET @Tipo_Chiave = 'F'
SET @Visibile = '1'
SET @Modificabile = '1'
SET @Globale = '1'
SET @myversione_CD = '5.2.4'
SET @Codice_Old_Webconfig = NULL
SET @Forza_Update = NULL
SET @RFU = NULL

EXECUTE @RC = [DOCSADM].[Utl_Insert_Chiave_Config] 
   @Codice
  ,@Descrizione
  ,@Valore
  ,@Tipo_Chiave
  ,@Visibile
  ,@Modificabile
  ,@Globale
  ,@myversione_CD
  ,@Codice_Old_Webconfig
  ,@Forza_Update
  ,@RFU
GO