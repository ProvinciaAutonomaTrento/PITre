--------------------------------------------------------
--  DDL for Function CODRFAPPARTENZA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CODRFAPPARTENZA" (idpeople INT)
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
                           WHERE cg.system_id = t.id_ruolo_in_uo
                             AND EXISTS (
                                    SELECT 'x'
                                      FROM peoplegroups pg
                                     WHERE cg.id_gruppo = pg.groups_system_id
                                       AND pg.people_system_id = idpeople)));
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenza; 

/
