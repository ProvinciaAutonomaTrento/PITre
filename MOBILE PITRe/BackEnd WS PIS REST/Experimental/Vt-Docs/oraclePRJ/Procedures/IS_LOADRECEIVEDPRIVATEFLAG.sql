--------------------------------------------------------
--  DDL for Procedure IS_LOADRECEIVEDPRIVATEFLAG
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."IS_LOADRECEIVEDPRIVATEFLAG" 
(
  p_ProfileId Number,
  p_ReceivedPrivate Out Number
) As
Begin

  Select ReceivedPrivate Into p_ReceivedPrivate From SimpInteropReceivedMessage Where ProfileId = p_ProfileId;

End IS_LoadReceivedPrivateFlag;

/
