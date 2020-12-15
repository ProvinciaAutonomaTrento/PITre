--------------------------------------------------------
--  DDL for Function GETCHAFIRMATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCHAFIRMATO" (docnum number)
RETURN VARCHAR2
IS
isFirmato   VARCHAR2 (16);
BEGIN

DECLARE
vmaxidgenerica   NUMBER;

BEGIN
BEGIN
SELECT MAX (v1.version_id)
INTO vmaxidgenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
EXCEPTION
WHEN OTHERS
THEN
vmaxidgenerica := 0;
END;

BEGIN
SELECT cha_firmato
INTO isFirmato
FROM components
WHERE docnumber = docnum AND version_id = vmaxidgenerica;
EXCEPTION
WHEN OTHERS
THEN
isFirmato := '0';
END;

END;

RETURN isFirmato;
END getchafirmato;

/
