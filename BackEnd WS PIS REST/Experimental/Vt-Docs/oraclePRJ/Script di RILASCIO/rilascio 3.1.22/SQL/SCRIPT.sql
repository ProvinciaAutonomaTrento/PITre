CREATE TABLE DPA_ULTIMI_DOC_VISUALIZZATI 
(
  SYSTEM_ID	INT
, ID_PEOPLE INT
, ID_GRUPPO INT
, ID_AMM	INT
, ID_PROFILE  INT
, DTA_VISUALIZZAZIONE DATE
);


exec Utl_Insert_Chiave_Config 'BE_NUM_ULTIMI_DOC_VISUALIZZATI','Chiave utilizzata per l''impostazione del numero degli ultimi documenti visualizzati da un utente'
  ,'5','B','1'
  ,'1','0','3.1.20'
  ,NULL, NULL, NULL