/*
Script per l'inserimento della chiave di DB 'BE_REMOTE_PDF_STAMPER_COORDS'
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

EXECUTE @RC = [@dbUser].[Utl_Insert_Chiave_Config] 
   'BE_REMOTE_PDF_STAMPER_COORDS'
  ,'Imposta le coordinate per il posizionamento della segnatura elettronica. Page;LeftX;LeftY;RightX;RightY'
  ,'1;350;800;700;850'
  ,'B'
  ,'1'
  ,'1'
  ,'1'
  ,'3.30.0'
  ,NULL
  ,'1'
  ,NULL
GO
