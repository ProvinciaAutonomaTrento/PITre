--------------------------------------------------------
--  DDL for Function GETDTAINVIO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDTAINVIO" (idprofile number)
RETURN date IS risultato date;
BEGIN
select max(dta_invio) into risultato from dpa_trasmissione where id_profile=idprofile;
RETURN risultato;
END getDtaInvio; 

/
