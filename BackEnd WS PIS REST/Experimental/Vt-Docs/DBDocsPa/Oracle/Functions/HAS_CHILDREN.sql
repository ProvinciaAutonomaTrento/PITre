CREATE OR REPLACE FUNCTION @db_user.HAS_CHILDREN (corrId number,tipoURP char)
RETURN number IS risultato number;
BEGIN
DECLARE

rtnUO1 number;
rtnRUO number;
rtnUO2 number;
BEGIN


select case when count(*) > 0 then 1 else 0 end into rtnUO1 from
dpa_corr_globali b where tipoURP='U' and
b.id_parent = corrId;
--oppure
select case when count(*) > 0 then 1 else 0 end into rtnUO2 from
dpa_corr_globali b where tipoURP='U' and
b.id_uo = corrId ;
--oppure
select case when count(*) > 0 then 1 else 0 end into rtnRUO from
dpa_corr_globali b where (tipoURP='R')  and
exists (select * from peoplegroups
where groups_system_id in (select id_gruppo from dpa_corr_globali where system_id=corrId ) and dta_fine is null)
;


risultato:=rtnUO1+rtnUO2+rtnRUO;
-- come se fosse un booleano
if(risultato>0)
then
risultato:=1;
else
risultato:=0;


end if;

return risultato;
end;
END has_children;
/

