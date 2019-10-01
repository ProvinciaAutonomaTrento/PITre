begin 
Utl_Backup_Plsql_code ('PROCEDURE','IS_InsertDataInReceivedMsg'); 
end;
/

create or replace
PROCEDURE IS_InsertDataInReceivedMsg(
    p_MessageId                VARCHAR2,
    p_ReceivedPrivate          NUMBER,
    p_Subject                  VARCHAR2,
    p_SenderDescription        VARCHAR2,
    p_SenderUrl                VARCHAR2,
    p_SenderAdministrationCode VARCHAR2,
    p_AOOCode                  VARCHAR2,
    p_RecordNumber             NUMBER,
    p_RecordDate               DATE,
    p_ReceiverCode             VARCHAR2 )
AS
BEGIN
  -- Inserimento informazioni sul messaggio ricevuto
  INSERT
  INTO SimpInteropReceivedMessage
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
END IS_InsertDataInReceivedMsg;
/
