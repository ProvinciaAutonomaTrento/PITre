Begin 
-- MEV Attivazione Cessione Diritti
-- by Iacozzilli Giordano.
--chiave di configurazione per la cessione dei diritti su un Doc.

--Utl_Insert_Chiave_Config(
--  Codice               VARCHAR2 ,
--  Descrizione          VARCHAR2 ,
--  Valore               VARCHAR2 ,
--  Tipo_Chiave          Varchar2 ,
--  Visibile             VARCHAR2 ,
--  Modificabile         VARCHAR2 ,
--  Globale              VARCHAR2 ,
--  Myversione_Cd          Varchar2 ,
--  Codice_Old_Webconfig Varchar2 ,
--  Forza_Update  VARCHAR2 , RFU VARCHAR2)
Utl_Insert_Chiave_Config('FE_CEDI_DIRITTI_IN_RUOLO',
	  'Chiave per abilitare o meno la Cessione dei diritti anche da parte di utenti non proprietari.',
	  '0',	  'F',	  '1',
	  '1',		0,	'3.23',
	  NULL,NULL,NULL);
-- FINE 
END;
/                                                                                                                   


BEGIN
-- by Veltri per import pregressi
   utl_insert_chiave_config ('FE_IMPORT_PREGRESSI',
                  'Chiave per abilitare import pregressi in amministrazione',
                  '0',                  'B',                  '1',
                  '1',                  '1',                  '3.23',
                  NULL,                  NULL,                  NULL);
END;
/


BEGIN
-- by Veltri per import pregressi
   utl_insert_chiave_config('BE_TIMER_PREGRESSI',
                  'Chiave per abilitare il timer delle priorita import Pregressi',
                  '60',                  'B',                  '1',
                  '1',                  '1',                  '3.23',
                  NULL,NULL,NULL);
END;
/

BEGIN
-- by Ferlito per MultiMail
Utl_Insert_Chiave_Config('FE_ATTIVA_GESTIONE_MULTIMAIL',
'Se questa chiave è a 0, non sarà possibile: 1. FrontEnd: inserire più di una casella mail per i corrispondenti esterni nuovi o in modifica.; 2. Amm.ne: non sarà possibile inserire più di una casella mail su Registri /RF.',
'0','F','1',
'1','0','3.23',
	  NULL,NULL,NULL);
END;
/



BEGIN
-- by Ferlito per Albo
Utl_Insert_Chiave_Config('FE_FILTRO_ALLEGATI_ESTERNI',
'Attiva/disattiva il filtro allegati sistemi esterni',
'0','F','1',
'1','0','3.23',
	  NULL,NULL,NULL);
END;
/

