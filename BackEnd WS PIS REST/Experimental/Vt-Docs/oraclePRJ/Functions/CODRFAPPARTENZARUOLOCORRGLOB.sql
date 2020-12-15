--------------------------------------------------------
--  DDL for Function CODRFAPPARTENZARUOLOCORRGLOB
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CODRFAPPARTENZARUOLOCORRGLOB" (idRuolo INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (128);
BEGIN
   BEGIN
      SELECT var_codice
        INTO risultato
        FROM dpa_el_registri a
       WHERE cha_rf = '1'
         AND EXISTS (
                SELECT 'x'
                  FROM dpa_l_ruolo_reg t
                 WHERE t.id_registro = a.system_id
                   AND EXISTS (
                          SELECT 'x'
                            FROM dpa_corr_globali cg
                           WHERE cg.id_gruppo =idruolo and
                           cg.system_id=t.ID_RUOLO_IN_UO
                             ));
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenzaRuoloCOrrGLob; 

/
