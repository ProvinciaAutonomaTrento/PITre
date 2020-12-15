--------------------------------------------------------
--  DDL for Function GETSEOGGATTIVO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSEOGGATTIVO" (systemid NUMBER, idtemplate NUMBER)
   RETURN CHAR 
IS  
   tmpvar   CHAR;
   cnt   INT;

BEGIN

         SELECT COUNT (*)
           INTO cnt
           FROM dpa_associazione_templates
          WHERE id_template = idtemplate
            AND doc_number IS NULL
            AND id_oggetto = systemid;


            IF (cnt > 0)
            THEN
               tmpvar := '1';
            ELSE
               tmpvar := '0';
            END IF;

      RETURN tmpvar;      
      EXCEPTION
         WHEN OTHERS
         THEN
            cnt := 0;
END getseoggattivo; 

/
