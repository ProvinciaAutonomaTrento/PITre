CREATE OR REPLACE FUNCTION @db_user.getCodTit(idParent number) RETURN varchar2 IS
tmpVar varchar2(32);
BEGIN
begin
select upper(var_codice) into tmpvar from project where system_id=idParent;
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:='';

end;
RETURN tmpVar;
END getCodTit;
/