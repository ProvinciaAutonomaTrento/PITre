/*
***                        ATTENZIONE                       ***
*** Script convertito da Oracle a SQL Server ma non testato ***
*** Testare prima di utilizzare                             ***
*/
BEGIN
  Utl_Insert_Chiave_Config
  (
  'BE_CONS_VER_LEG_GG_NOTIFICHE', --CODICE
  'Intervallo in giorni dalla scadenza delle verifiche di leggibilità su documenti in Conservazione per inviare notifiche nel Centro Servizi ', --DESCRIZIONE
  '10',	--VALORE
  'B',	--TIPO_CHIAVE
  '1',	--VISIBILE
  '1',	--MODIFICABILE
  '0',	--GLOBALE
  'CS 1.5',	--myversione_CD
  NULL, NULL, NULL);  --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
END;
  
BEGIN
	UPDATE dpa_chiavi_configurazione
	SET
	cha_conservazione='1'
	WHERE
	var_codice='BE_CONS_VER_LEG_GG_NOTIFICHE';
END;