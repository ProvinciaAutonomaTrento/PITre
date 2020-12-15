--------------------------------------------------------
--  DDL for Function GETUO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETUO" (idruolo INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
   SELECT f.var_desc_corr
     INTO risultato
     FROM dpa_corr_globali f
    WHERE system_id IN (SELECT id_uo
                          FROM dpa_corr_globali
                         WHERE system_id = idruolo);

   RETURN risultato;
END getuo; 

/
