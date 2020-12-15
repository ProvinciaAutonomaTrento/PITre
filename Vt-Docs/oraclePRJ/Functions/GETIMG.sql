--------------------------------------------------------
--  DDL for Function GETIMG
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIMG" (docnum NUMBER)
RETURN VARCHAR
IS
tmpVar varchar(7);
BEGIN
declare
v_path varchar(128);
vMaxIdGenerica number;
begin

begin
SELECT
MAX (v1.version_id)
INTO vMaxIdGenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docNum
AND v1.version_id = c.version_id;
EXCEPTION
WHEN OTHERS THEN
vMaxIdGenerica:=0;
end;

begin
select ext into v_path from components where docnumber=docNum and version_id=vMaxIdGenerica;

EXCEPTION
WHEN OTHERS THEN
tmpVar:='0';
end;

if(v_path <> '' OR v_path is  not null)
then tmpVar:= '1';
else tmpVar:='0';
end if;

end;
RETURN tmpVar;
END getimg;

/
