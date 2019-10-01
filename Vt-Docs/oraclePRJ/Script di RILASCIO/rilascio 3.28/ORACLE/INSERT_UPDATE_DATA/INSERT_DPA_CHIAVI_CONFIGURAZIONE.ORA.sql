

-- VELTRI - inserimento chiavi configurazione per nuova grafica

begin
  Utl_Insert_Chiave_Config('FE_ENABLE_MITTENTI_MULTIPLI','Abilita la visualizzazione dei mittenti multipli nel FE'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_INTERNAL_PROTOCOL','Abilita amministraizone al protocollo interno'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_KEY_WORDS','Abilita la parola chiave nella scheda del documento'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_SINCRONA_LC','Abilita la conversione sincrona pdf'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_LATO_SERVER','Abilita la conversione pdf lato server'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
   Utl_Insert_Chiave_Config('RUBRICA_RIGHE_PER_PAGINA','Definisce il numero di righe da visualizzare per pagina nella rubrica'  -- Codice, Descrizione
   ,'8','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
   ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
   ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
   end;
/

begin
  Utl_Insert_Chiave_Config('FE_SEDE_TRASM','Inserisce la sede dell utente nel dettaglio della trasmissione'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_DETTAGLI_FIRMA','Attiva/Disattiva Dettagli Firma Completi/Sintetici'  -- Codice, Descrizione
  ,'1','B','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

