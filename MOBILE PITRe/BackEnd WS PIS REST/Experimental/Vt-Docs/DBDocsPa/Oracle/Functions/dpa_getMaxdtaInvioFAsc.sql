CREATE OR REPLACE FUNCTION @db_user.dpa_getMaxdtaInvioFAsc(id_fasc number) RETURN date IS
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