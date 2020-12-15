--------------------------------------------------------
--  DDL for Function GETDESCTIPODOC
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETDESCTIPODOC" (idtipologia NUMBER)
   RETURN VARCHAR2
IS
   risultato   VARCHAR2 (255 BYTE);
BEGIN
   BEGIN
      SELECT var_desc_atto
        INTO risultato
        FROM dpa_tipo_atto f
       WHERE system_id = idtipologia;
   EXCEPTION
      WHEN OTHERS
      THEN
         risultato := '';
   END;

   RETURN risultato;
END getdesctipodoc; 

/
