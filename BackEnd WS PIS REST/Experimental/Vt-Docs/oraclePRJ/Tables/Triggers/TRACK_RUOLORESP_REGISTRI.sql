--------------------------------------------------------
--  DDL for Trigger TRACK_RUOLORESP_REGISTRI
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."TRACK_RUOLORESP_REGISTRI" 
After Update Of Id_Ruolo_Resp On Dpa_El_Registri 
Referencing Old As Old New As New

For Each Row
Declare 
PRAGMA AUTONOMOUS_TRANSACTION ;
ID INT; 
Begin
  Insert Into Utl_Track_Aooruoloresp ( Id_registro
  , Old_Aooruoloresp  
  , New_Aooruoloresp  
  , Cha_Changed_Notyet_Processed 
  , Dta_Changed ) 
Values ( :old.system_id
  ,:Old.Id_Ruolo_Resp 
  ,:New.Id_Ruolo_Resp 
  ,'y' -- will be changed to 'n' upon processing
  , Sysdate);
Commit; 
END;
/
ALTER TRIGGER "ITCOLL_6GIU12"."TRACK_RUOLORESP_REGISTRI" ENABLE;
