--------------------------------------------------------
--  DDL for Function GETTRANSREASONDESC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETTRANSREASONDESC" (idReason NUMBER)
RETURN VARCHAR2 IS result VARCHAR2(16);

BEGIN
begin

SELECT R.VAR_DESC_RAGIONE INTO result FROM DPA_RAGIONE_TRASM R WHERE R.SYSTEM_ID = idReason;

EXCEPTION
WHEN OTHERS THEN
result:='';
end;
RETURN result;
END GetTransReasonDesc; 

/
