begin
  Utl_Insert_Chiave_Config('BE_CONSERVAZIONE_INTERVALLO','Intervallo in giorni tra due verifiche di integrità su istanze in Conservazione nel Centro Servizi '  -- Codice, Descrizione
  ,'180','B','1'        --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','CS 1.5'   --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);  --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;
  
  
UPDATE dpa_chiavi_configurazione
SET
cha_conservazione='1'
WHERE
var_codice='BE_CONSERVAZIONE_INTERVALLO';
