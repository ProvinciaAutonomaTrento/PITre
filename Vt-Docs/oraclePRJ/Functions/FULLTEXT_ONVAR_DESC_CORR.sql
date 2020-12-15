--------------------------------------------------------
--  DDL for Function FULLTEXT_ONVAR_DESC_CORR
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."FULLTEXT_ONVAR_DESC_CORR" (stringa VARCHAR)
   RETURN fulltext_objrow PIPELINED
IS
   out_record   fulltext_obj := fulltext_obj (NULL, NULL, NULL);

   CURSOR c_sysid
   IS
      SELECT system_id, cg.var_desc_corr
        FROM dpa_corr_globali cg
       WHERE catsearch (cg.var_desc_corr, stringa, '') > 0
         ;
BEGIN
   FOR c1 IN c_sysid
   LOOP
      EXIT WHEN c_sysid%NOTFOUND;
      out_record.system_id := c1.system_id;
      out_record.var_desc_oggetto := c1.var_desc_corr;
      PIPE ROW (out_record);
   END LOOP;
EXCEPTION
   WHEN OTHERS
   THEN                                              
      RAISE; 
      out_record.system_id := 1;
      out_record.var_desc_oggetto := SUBSTR (SQLERRM, 1, 4000);
      PIPE ROW (out_record);
END; 

/
