--------------------------------------------------------
--  DDL for Function GETMAXVER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETMAXVER" (docnum NUMBER)
RETURN NUMBER
IS
tmpvar   NUMBER;
BEGIN
SELECT /*+index (c) index (v1)*/
MAX (v1.version_id)
INTO tmpvar
FROM VERSIONS v1, components c
WHERE v1.docnumber = docnum
AND v1.version_id = c.version_id
AND c.file_size > 0;

IF (tmpvar IS NULL)
THEN
tmpvar := 0;
END IF;

RETURN tmpvar;
EXCEPTION
WHEN OTHERS
THEN
tmpvar := 0;
END getmaxver; 

/
