--------------------------------------------------------
--  DDL for Function GETCODRUBRICACORR
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUBRICACORR" (sys number)
RETURN varchar IS tmpVar varchar(256);

BEGIN
BEGIN
tmpVar := '';
SELECT
UPPER(VAR_COD_RUBRICA) into tmpVar
FROM DPA_CORR_GLOBALI
WHERE SYSTEM_ID=sys;


EXCEPTION
WHEN OTHERS THEN
null;
END;
RETURN tmpVar;

END getcodRubricaCorr; 

/
