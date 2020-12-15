CREATE OR REPLACE PROCEDURE @db_user.dpa_setNumFasc is
begin

update dpa_reg_fasc set num_rif=1 where cha_automatico='1' and to_char(sysdate,'dd/mm')='01/01';
commit;
exception when others then null;
end dpa_setNumFasc;
/
