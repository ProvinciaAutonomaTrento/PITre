   create or replace FUNCTION getseoggattivofasc (systemid NUMBER, idtemplate NUMBER)
   RETURN CHAR 
IS  
   tmpvar   CHAR;
   cnt   INT;

BEGIN

         SELECT COUNT (*)
           INTO cnt
           FROM dpa_ass_templates_fasc
          WHERE id_template = idtemplate
            AND id_project IS NULL
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
END getseoggattivofasc; 