--------------------------------------------------------
--  DDL for Function GETDOCPRINCIPALE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDOCPRINCIPALE" (systemID INT)
RETURN varchar IS risultato varchar(2000);
BEGIN

SELECT DISTINCT A.docname into risultato
FROM profile A
WHERE 
 A.SYSTEM_ID =systemID;
RETURN risultato;
END getDocPrincipale; 

/
