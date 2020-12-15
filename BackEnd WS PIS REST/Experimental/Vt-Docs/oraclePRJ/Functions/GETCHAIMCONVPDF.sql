--------------------------------------------------------
--  DDL for Function GETCHAIMCONVPDF
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCHAIMCONVPDF" (docNum number) RETURN char IS
tmpVar char;
BEGIN
declare
v_path varchar(128);
vMaxIdGenerica number;
begin

begin
SELECT /*+index (c) index (v1)*/
MAX (v1.version_id)
INTO vMaxIdGenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docNum
AND v1.version_id = c.version_id
AND c.file_size > 0;
EXCEPTION
WHEN OTHERS THEN
vMaxIdGenerica:=0;
end;

begin
select substr(path, length(path)-instr(reverse(path),'.')+2) into v_path from components where docnumber=docNum and version_id=vMaxIdGenerica;
EXCEPTION
WHEN OTHERS THEN
tmpVar:='0';
end;

if(upper(v_path) = 'PDF' )
then tmpVar:='1';
else tmpVar:='0';
end if;

end;
RETURN tmpVar;
END getChaImConvPDF; 

/
