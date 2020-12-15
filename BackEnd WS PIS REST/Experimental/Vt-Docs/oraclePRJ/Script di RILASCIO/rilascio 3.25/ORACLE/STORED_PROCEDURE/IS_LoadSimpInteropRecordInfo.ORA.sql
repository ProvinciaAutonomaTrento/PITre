begin 
Utl_Backup_Plsql_code ('PROCEDURE','IS_LoadSimpInteropRecordInfo'); 
end;
/

create or replace
Procedure          IS_LoadSimpInteropRecordInfo(
  p_DocumentId integer,
  p_SenderAdministrationCode Out VarChar2,
  p_SenderAOOCode Out VarChar2,
  p_SenderRecordNumber Out Number,
  p_SenderRecordDate Out Date,
  p_ReceiverAdministrationCode Out VarChar2,
  p_ReceiverAOOCode Out VarChar2,
  p_ReceiverRecordNumber Out Number,
  p_ReceiverRecordDate Out Date,
  p_SenderUrl Out VarChar2  ,
-- new parameter added lately  
  p_ReceiverCode Out VarChar2
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
