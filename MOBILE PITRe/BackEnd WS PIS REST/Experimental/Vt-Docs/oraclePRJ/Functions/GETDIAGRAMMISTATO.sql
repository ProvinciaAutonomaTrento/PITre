--------------------------------------------------------
--  DDL for Function GETDIAGRAMMISTATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDIAGRAMMISTATO" (p_docorid INT, p_tipo VARCHAR)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
   IF (p_tipo = 'D')
   THEN
      SELECT var_descrizione
        INTO risultato
        FROM dpa_stati f, dpa_diagrammi b
       WHERE b.doc_number = p_docorid AND f.system_id = b.id_stato;

      RETURN risultato;
   END IF;

   IF (p_tipo = 'F')
   THEN
      SELECT var_descrizione
        INTO risultato
        FROM dpa_stati f, dpa_diagrammi b
       WHERE b.id_project = p_docorid AND f.system_id = b.id_stato;

      RETURN risultato;
   END IF;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getdiagrammistato; 

/
