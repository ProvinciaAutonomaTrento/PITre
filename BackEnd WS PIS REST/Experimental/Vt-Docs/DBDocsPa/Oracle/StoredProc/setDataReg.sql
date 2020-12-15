CREATE OR REPLACE PROCEDURE @db_user.setDataReg IS
BEGIN

DECLARE
CURSOR C IS select a.dta_open, b.num_rif, a.system_id AS SYSID
from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
and a.cha_automatico='1' and cha_stato='A';
C1 C%ROWTYPE;
BEGIN
OPEN C;
LOOP
FETCH C INTO C1;
EXIT WHEN C%NOTFOUND;
begin
--INSERT INTO DPA_REGISTRI_STO
insert into dpa_registro_sto(system_id,dta_open,dta_close,num_rif,id_registro,id_people,id_ruolo_in_uo)
SELECT (seq.nextval),a.dta_open,SYSDATE,b.num_rif,a.system_id,1,1
from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
and a.system_id = C1.SYSID;

update dpa_el_registri set dta_open =sysdate,cha_stato ='A',dta_close = null
WHERE SYSTEM_ID=C1.SYSID;

update dpa_reg_proto set num_rif=1 where to_char(sysdate,'dd/mm')='01/01';


EXCEPTION
WHEN OTHERS THEN
NULL;
END;

end loop;
close c;
COMMIT;
end;
END setDataReg;
/