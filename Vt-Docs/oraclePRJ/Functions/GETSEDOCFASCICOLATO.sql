--------------------------------------------------------
--  DDL for Function GETSEDOCFASCICOLATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSEDOCFASCICOLATO" (systemid NUMBER)
   RETURN CHAR
IS
   tmpvar   CHAR;
BEGIN
   DECLARE
      cnt   INT;
   BEGIN
      SELECT COUNT (LINK)
        INTO cnt
        FROM project_components
       WHERE LINK = systemid;
   EXCEPTION
      WHEN OTHERS
      THEN
         cnt := 0;

         IF (cnt > 0)
         THEN
            tmpvar := '1';
         ELSE
            tmpvar := '0';
         END IF;
   END;

   RETURN tmpvar;
END getsedocfascicolato; 

/
