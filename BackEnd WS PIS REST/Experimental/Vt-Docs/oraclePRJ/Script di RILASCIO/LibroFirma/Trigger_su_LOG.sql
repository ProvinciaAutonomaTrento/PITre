create or replace
TRIGGER TR_INS_EVENT_MONITOR
After Insert
ON DPA_LOG
REFERENCING NEW AS New
FOR EACH ROW
DECLARE
idazione number;
/******************************************************************************
   NAME:      
   PURPOSE:   
 
   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        10/03/2015      Abbatangeli       INSERT ON DPA_EVENT_MONITOR
 
   NOTES:
 
   Automatically available Auto Replace Keywords:
      Object Name:     TR_INS_EVENT_MONITOR
      Sysdate:         10/03/2015
      Date and Time:   10/03/2015, 11:59:00, and 10/03/2015 11:59:00
      Username:        Abbatangeli 
      Table Name:      DPA_LOG 
      Trigger Options:  (set in the "New PL/SQL Object" dialog)
******************************************************************************/
Begin
Begin
Select Id_Evento Into Idazione From Dpa_Anagrafica_Eventi Where Var_Cod_Azione = :New.Var_Cod_Azione;

 EXCEPTION
   WHEN NO_DATA_FOUND THEN
   IDAZIONE :=0;
  End;
If (Idazione !=0 )
  Then
     INSERT INTO DPA_EVENT_MONITOR (ID_DOCUMENTO , ID_EVENTO , ID_LOG, ID_GROUP, ID_PEOPLE_AZIONE, DATA_INSERIMENTO, ID_DELEGANTE, ID_PEOPLE) VALUES (:NEW.ID_OGGETTO,IDAZIONE,:NEW.SYSTEM_ID, :NEW.ID_GRUPPO_OPERATORE, :NEW.ID_PEOPLE_OPERATORE, :NEW.DTA_AZIONE, :NEW.ID_PEOPLE_DELEGANTE, NVL(:NEW.ID_PEOPLE_DELEGANTE,:NEW.ID_PEOPLE_OPERATORE));
   
End If;

   EXCEPTION
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       Raise;
End;