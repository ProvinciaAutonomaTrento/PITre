CREATE OR REPLACE FUNCTION @db_user.dpa_valore_oggetto_tipo_atto(id_atto number, doc_num number)
RETURN VARCHAR2 IS
BEGIN
declare
tmpVar varchar2(255) := ' ';
begin

if (id_atto = 84 )
then
begin
select a.valore_oggetto_db into tmpVar from dpa_associazione_templates a
where a.id_template=id_atto and a.doc_number=doc_num and a.id_oggetto=181;
RETURN tmpVar;
end;
else
return tmpVar ;
end if;

EXCEPTION
WHEN OTHERS THEN
return  tmpVar;

if tmpVar is null
then
tmpVar := ' ';
end if;


end;

END dpa_valore_oggetto_tipo_atto;
/