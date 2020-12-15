CREATE OR REPLACE FUNCTION @db_user.getmaxver(docnum number) RETURN NUMBER IS

tmpvar number;

BEGIN

select /*+index (c) index (v1)*/ max(v1.version_id) into tmpvar

from versions v1,

components c

where       v1.docnumber=docnum and

v1.version_id = c.version_id and

c.file_size>0;



RETURN tmpVar;

EXCEPTION

WHEN others THEN

tmpVar:=0;

END getmaxver;
/