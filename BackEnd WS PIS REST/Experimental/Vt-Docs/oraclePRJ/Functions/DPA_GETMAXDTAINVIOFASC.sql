--------------------------------------------------------
--  DDL for Function DPA_GETMAXDTAINVIOFASC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DPA_GETMAXDTAINVIOFASC" (id_fasc number) RETURN date IS
/******************************************************************************
NAME:       dpa_getMaxdtaInvioFAsc
PURPOSE:

REVISIONS:
Ver        Date        Author           Description
---------  ----------  ---------------  ------------------------------------
1.0        04/12/2007          1. Created this function.

NOTES:

Automatically available Auto Replace Keywords:
Object Name:     dpa_getMaxdtaInvioFAsc
Sysdate:         04/12/2007
Date and Time:   04/12/2007, 14.23.24, and 04/12/2007 14.23.24
Username:         (set in TOAD Options, Procedure Editor)
Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/
BEGIN
declare
tmpVar date;
begin

select max(dta_invio) into tmpVar from dpa_trasmissione where id_project=id_fasc;
RETURN tmpVar;
EXCEPTION
WHEN others THEN
RETURN tmpVar;
end;
END dpa_getMaxdtaInvioFAsc; 

/
