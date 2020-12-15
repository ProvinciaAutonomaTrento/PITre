--------------------------------------------------------
--  DDL for Function ISNONPROT
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ISNONPROT" (idProfile number) RETURN NUMBER IS
tmpVar NUMBER;
/******************************************************************************
NAME:       isProtocollo
PURPOSE:

REVISIONS:
Ver        Date        Author           Description
---------  ----------  ---------------  ------------------------------------
1.0        17/02/2009          1. Created this function.

NOTES:

Automatically available Auto Replace Keywords:
Object Name:     isProtocollo
Sysdate:         17/02/2009
Date and Time:   17/02/2009, 9.40.21, and 17/02/2009 9.40.21
Username:         (set in TOAD Options, Procedure Editor)
Table Name:      PROFLE

******************************************************************************/
BEGIN
begin

tmpVar := 0;
SELECT count(A.system_id) into tmpVar From profile A where (A.system_id=idProfile and
(A.CHA_TIPO_PROTO IN ('G')  AND A.ID_DOCUMENTO_PRINCIPALE IS NULL) AND A.CHA_DA_PROTO = '0');

RETURN tmpVar;
EXCEPTION
WHEN OTHERS THEN
null;
end;
return tmpVar;
END isNonProt; 

/
