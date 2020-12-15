--------------------------------------------------------
--  DDL for Procedure IS_INSERTDATAINRECEIVEDMSG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_INSERTDATAINRECEIVEDMSG" (
  p_MessageId VarChar2,
  p_ReceivedPrivate Number,
  p_Subject Varchar2,
  p_SenderDescription VarChar2,
  p_SenderUrl VarChar2,
  p_SenderAdministrationCode VarChar2, 
  p_AOOCode VarChar2, 
  p_RecordNumber Number, 
  p_RecordDate Date,
  p_ReceiverCode VarChar2
) As
Begin
  -- Inserimento informazioni sul messaggio ricevuto
  Insert Into SimpInteropReceivedMessage
  (
    MessageId,
    ReceivedPrivate,
    ReceivedDate,
    Subject,
    SenderDescription,
    SenderUrl,
    SenderAdministrationCode, 
    AOOCode, 
    RecordNumber, 
    RecordDate,
    ReceiverCode
  )
  VALUES
  (
    p_MessageId,
    p_ReceivedPrivate,
    SysDate,
    p_Subject,
    p_SenderDescription,
    p_SenderUrl,
    p_SenderAdministrationCode, 
    p_AOOCode, 
    p_RecordNumber, 
    p_RecordDate,
    p_ReceiverCode
  );

  
End IS_InsertDataInReceivedMsg;

/
