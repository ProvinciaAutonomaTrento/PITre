
CREATE OR REPLACE FUNCTION @db_user.getDtaInvio (idprofile number)
RETURN date IS risultato date;
BEGIN
select max(dta_invio) into risultato from dpa_trasmissione where id_profile=idprofile;
RETURN risultato;
END getDtaInvio;
/