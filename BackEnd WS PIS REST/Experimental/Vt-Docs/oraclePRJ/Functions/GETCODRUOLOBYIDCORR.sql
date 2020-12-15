--------------------------------------------------------
--  DDL for Function GETCODRUOLOBYIDCORR
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCODRUOLOBYIDCORR" (idcorr INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
   SELECT var_cod_rubrica
     INTO risultato
     FROM dpa_corr_globali
    WHERE system_id = idcorr;

   RETURN risultato;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getcodruolobyidcorr; 

/
