--------------------------------------------------------
--  DDL for Procedure IS_INSERTDATAINSIMPINTEROPLOG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_INSERTDATAINSIMPINTEROPLOG" (
  p_ProfileId Number,
  p_ErrorMessage Number,
  p_Text VarChar2
) As
Begin
  -- Inserimento voce di log
  Insert Into SimpInteropDbLog
  (
    ProfileId,
    ErrorMessage,
    Text
  )
  VALUES
  (
    p_ProfileId,
    p_ErrorMessage,
    p_Text
  );

  
End IS_InsertDataInSimpInteropLog;

/
