--------------------------------------------------------
--  DDL for Function GETNOTEOLD
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETNOTEOLD" (SYS NUMBER)
   RETURN VARCHAR
IS
   tmpvar   VARCHAR (2000);
BEGIN
   BEGIN
      SELECT p13note
        INTO tmpvar
        FROM recupero_note_20081013
       WHERE psys = SYS;

      RETURN tmpvar;
   EXCEPTION
      WHEN OTHERS
      THEN
         tmpvar := '';
   END;

   RETURN tmpvar;
END getnoteold; 

/
