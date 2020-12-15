CREATE OR REPLACE FUNCTION @db_user.GetDocFascicolato(idprofile number) RETURN NUMBER IS
tmpVar NUMBER;
BEGIN
tmpVar := 0;
begin
select count(link) into tmpvar from project_components where link=idprofile;
EXCEPTION
WHEN NO_DATA_FOUND THEN
RETURN tmpVar;
end;
if(tmpVar>0)
then tmpVar:=1;
end if;
return tmpVar;
END GetDocFascicolato;
/