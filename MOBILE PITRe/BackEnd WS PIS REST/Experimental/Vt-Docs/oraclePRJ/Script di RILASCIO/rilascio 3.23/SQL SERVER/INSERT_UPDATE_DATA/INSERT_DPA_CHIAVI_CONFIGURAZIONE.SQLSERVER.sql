-- =============================================
-- Author:		FRANCESCO FONZO
-- Create date: 22/02/2013
-- Description:	CONVERSIONE DA ORACLE A SQL SERVER
-- INSERT_DPA_CHIAVI_CONFIGURAZIONE
-- =============================================

--chiave di configurazione per la cessione dei diritti su un Doc.

EXECUTE DOCSADM.Utl_Insert_Chiave_Config 'FE_CEDI_DIRITTI_IN_RUOLO','Chiave per abilitare o meno la Cessione dei diritti anche da parte di utenti non proprietari.','1','F','1','1',0,'3.23',NULL,NULL,NULL                                                                                                          


EXECUTE DOCSADM.utl_insert_chiave_config 'FE_IMPORT_PREGRESSI','Chiave per abilitare import pregressi in amministrazione','0','B','1','1','1','3.23',NULL,NULL,NULL


EXECUTE DOCSADM.utl_insert_chiave_config 'BE_TIMER_PREGRESSI','Chiave per abilitare il timer delle priorita import Pregressi','60','B','1','1','1','3.23',NULL,NULL,NULL


EXECUTE DOCSADM.Utl_Insert_Chiave_Config 'FE_ATTIVA_GESTIONE_MULTIMAIL','Se questa chiave  a 0, non sar possibile: 1. FrontEnd: inserire pi di una casella mail per i corrispondenti esterni nuovi o in modifica.; 2. Amm.ne: non sar possibile inserire pi di una casella mail su Registri /RF.','0','F','1','1','0','3.23',NULL,NULL,NULL
