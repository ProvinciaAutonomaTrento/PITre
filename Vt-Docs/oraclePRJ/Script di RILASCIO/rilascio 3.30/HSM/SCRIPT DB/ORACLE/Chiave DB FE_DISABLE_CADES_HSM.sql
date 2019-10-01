/*
Script per l'inserimento della chiave di DB 'FE_DISABLE_CADES_HSM'
*/

  DECLARE 
  CODICE VARCHAR2(200);
  DESCRIZIONE VARCHAR2(200);
  VALORE VARCHAR2(200);
  TIPO_CHIAVE VARCHAR2(200);
  VISIBILE VARCHAR2(200);
  MODIFICABILE VARCHAR2(200);
  GLOBALE VARCHAR2(200);
  MYVERSIONE_CD VARCHAR2(200);
  CODICE_OLD_WEBCONFIG VARCHAR2(200);
  FORZA_UPDATE VARCHAR2(200);
  RFU VARCHAR2(200);

BEGIN 
  CODICE := 'FE_DISABLE_CADES_HSM';
  DESCRIZIONE := 'Disabilita il radio button Cades nella maschera HSM';
  VALORE := '1';
  TIPO_CHIAVE := 'F';
  VISIBILE := '1';
  MODIFICABILE := '1';
  GLOBALE := '0';
  MYVERSIONE_CD := '3.30.0';
  CODICE_OLD_WEBCONFIG := NULL;
  FORZA_UPDATE := '1';
  RFU := NULL;

  SVILUPPO_PITRE.UTL_INSERT_CHIAVE_CONFIG ( CODICE, DESCRIZIONE, VALORE, TIPO_CHIAVE, VISIBILE, MODIFICABILE, GLOBALE, MYVERSIONE_CD, CODICE_OLD_WEBCONFIG, FORZA_UPDATE, RFU );
  COMMIT; 
END; 