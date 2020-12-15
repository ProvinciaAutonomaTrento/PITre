--------------------------------------------------------
--  DDL for Function CODRFAPPARTENZARUOLO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CODRFAPPARTENZARUOLO" (idRuolo INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (128);
BEGIN
   BEGIN

                select codice into risultato from (
                SELECT a.var_codice codice
                  FROM dpa_l_ruolo_reg t,dpa_corr_globali cg,dpa_el_registri a
                 WHERE t.id_registro = a.system_id and cha_rf = '1' and
                    cg.system_id =idRuolo and
                           cg.system_id=t.ID_RUOLO_IN_UO
                             order by t.system_id asc) where rownum=1;
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenzaRuolo; 

/
