--------------------------------------------------------
--  DDL for Procedure IS_SETIDPROFFORSIMPINTEROPMSG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_SETIDPROFFORSIMPINTEROPMSG" (
  p_ProfileId Number,
  p_MessageId VarChar2
) As
Begin
  -- Aggiornamento del campo ProfileId dove compare il MessageId passato per parametro
  Update SimpInteropReceivedMessage
  Set ProfileId = p_ProfileId
  Where MessageId = p_MessageId;

  
End IS_SetIdProfForSimpInteropMsg;

/
