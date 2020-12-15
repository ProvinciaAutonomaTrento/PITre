exec  Utl_Insert_Chiave_Config 'FE_FULLTEXT_SEARCH','La chiave abilita o meno la ricerca FullText'  -- Codice, Descrizione
  ,'true','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','1','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
 
 
 exec Utl_Insert_Chiave_Config 'BE_FULLTEXT_MAXROWS','La chiave indica la dimensione del risultato della ricerca FullText'  -- Codice, Descrizione
  ,'200','B','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','1','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL					--Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  
  
 exec Utl_Insert_Chiave_Config 'BE_FULLTEXT_CATALOG','La chiave riporta il nome del catalogo da interrogare per la ricerca FullText'  -- Codice, Descrizione
  ,'CATALOG_MIT','B','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','1','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  