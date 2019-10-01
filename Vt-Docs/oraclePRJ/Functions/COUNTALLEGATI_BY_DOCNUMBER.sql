--------------------------------------------------------
--  DDL for Function COUNTALLEGATI_BY_DOCNUMBER
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."COUNTALLEGATI_BY_DOCNUMBER" (Id INT)
RETURN number IS risultato number;

BEGIN
begin
SELECT COUNT(V.VERSION_ID) into risultato
FROM PROFILE P INNER JOIN VERSIONS V ON P.DOCNUMBER = V.DOCNUMBER
WHERE (P.DOCNUMBER = Id AND V.VERSION = 0) OR P.ID_DOCUMENTO_PRINCIPALE = Id;


EXCEPTION
WHEN OTHERS THEN
risultato:=-1;
end;
RETURN risultato;
END CountAllegati_BY_DocNumber; 

/
