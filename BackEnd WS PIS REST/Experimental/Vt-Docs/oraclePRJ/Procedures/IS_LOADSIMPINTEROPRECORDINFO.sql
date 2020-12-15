--------------------------------------------------------
--  DDL for Procedure IS_LOADSIMPINTEROPRECORDINFO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_LOADSIMPINTEROPRECORDINFO" (
  p_DocumentId Number,
  p_SenderAdministrationCode Out VarChar,
  p_SenderAOOCode Out VarChar,
  p_SenderRecordNumber Out Number,
  p_SenderRecordDate Out Date,
  p_ReceiverAdministrationCode Out VarChar,
  p_ReceiverAOOCode Out VarChar,
  p_ReceiverRecordNumber Out Number,
  p_ReceiverRecordDate Out Date,
  p_SenderUrl Out VarChar,
  p_ReceiverCode Out VarChar
) As
Begin
  -- Caricamento delle informazioni sul protocollo mittente
  Select SenderAdministrationCode, AOOCode, RecordNumber, RecordDate, SenderUrl, ReceiverCode
  Into p_SenderAdministrationCode, p_SenderAOOCode, p_SenderRecordNumber, p_SenderRecordDate, p_SenderUrl, p_ReceiverCode
  From SimpInteropReceivedMessage
  Where ProfileId = p_DocumentId;
  
  -- Caricamento informazioni sul protocollo creato nell'amministrazione destinataria
  Select (Select var_codice From dpa_el_registri Where System_Id = Id_Registro), 
          Num_Proto, 
          Dta_proto, 
          (Select var_codice_amm From dpa_amministra Where system_id = (Select id_amm From dpa_corr_globali Where system_id = Id_Uo_Prot))
  Into p_ReceiverAOOCode, p_ReceiverRecordNumber, p_ReceiverRecordDate, p_ReceiverAdministrationCode           
  From profile 
  Where System_id = p_DocumentId;

End IS_LoadSimpInteropRecordInfo;

/
