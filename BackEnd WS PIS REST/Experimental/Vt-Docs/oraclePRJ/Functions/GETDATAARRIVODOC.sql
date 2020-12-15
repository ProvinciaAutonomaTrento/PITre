--------------------------------------------------------
--  DDL for Function GETDATAARRIVODOC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDATAARRIVODOC" (p_docnumber INT)
   RETURN DATE
IS
   risultato   DATE;
BEGIN
   SELECT dta_arrivo
     INTO risultato
     FROM (SELECT   /*+FIRST_ROWS(1) */
                    dta_arrivo
               FROM VERSIONS
              WHERE docnumber = p_docnumber
           ORDER BY version_id DESC)
    WHERE ROWNUM = 1;

   RETURN risultato;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getdataarrivodoc; 

/
