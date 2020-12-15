--------------------------------------------------------
--  DDL for Function GETCODRUBRICARUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUBRICARUOLO" (sys number)
RETURN varchar IS tmpVar varchar(256);

BEGIN
BEGIN
tmpVar := '';
SELECT
UPPER(VAR_COD_RUBRICA) into tmpVar
FROM DPA_CORR_GLOBALI
WHERE id_gruppo=sys;


EXCEPTION
WHEN OTHERS THEN
null;
END;
RETURN tmpVar;

END getcodRubricaRuolo; 

/
