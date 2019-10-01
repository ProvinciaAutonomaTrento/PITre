

-- VELTRI - inserimento chiavi configurazione per nuova grafica


 EXECUTE DOCSADM.Utl_Insert_Chiave_Config 'FE_ENABLE_MITTENTI_MULTIPLI','Abilita la visualizzazione dei mittenti multipli nel FE' , '0','F','1','1','0','3.28' ,NULL, NULL, NULL   

 EXECUTE DOCSADM.Utl_Insert_Chiave_Config 'FE_INTERNAL_PROTOCOL','Abilita amministraizone al protocollo interno' ,'0','F','1','1','0','3.28',NULL, NULL, NULL                         

 EXECUTE DOCSADM.Utl_Insert_Chiave_Config'FE_KEY_WORDS','Abilita la parola chiave nella scheda del documento'  ,'1','F','1' ,'1','0','3.28' ,NULL, NULL, NULL

 EXECUTE DOCSADM.Utl_Insert_Chiave_Config'CONVERSIONE_PDF_SINCRONA_LC','Abilita la conversione sincrona pdf'  ,'0','F','1','1','0','3.28',NULL, NULL, NULL  
 
  EXECUTE DOCSADM.Utl_Insert_Chiave_Config'CONVERSIONE_PDF_LATO_SERVER','Abilita la conversione pdf lato server'   ,'0','F','1'  ,'1','0','3.28',NULL, NULL, NULL   

   EXECUTE DOCSADM.Utl_Insert_Chiave_Config'RUBRICA_RIGHE_PER_PAGINA','Definisce il numero di righe da visualizzare per pagina nella rubrica'  ,'8','F','1'  ,'1','0','3.28' ,NULL, NULL, NULL   

  EXECUTE DOCSADM.Utl_Insert_Chiave_Config'FE_SEDE_TRASM','Inserisce la sede dell utente nel dettaglio della trasmissione' ,'0','F','1'   ,'1','0','3.28'  ,NULL, NULL, NULL  
 
  EXECUTE DOCSADM.Utl_Insert_Chiave_Config'FE_DETTAGLI_FIRMA','Attiva/Disattiva Dettagli Firma Completi/Sintetici'  ,'1','B','1'  ,'1','0','3.28'  ,NULL, NULL, NULL 
                    
      
  
