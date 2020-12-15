--------------------------------------------------------
--  DDL for Function GETSEMITTDEST
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETSEMITTDEST" (idcorr INT, idprofile NUMBER)
   RETURN INT
IS
   risultato   INT;
BEGIN
   BEGIN
      SELECT /*+index (DPA_DOC_ARRIVO_PAR)*/
             COUNT (system_id)
        INTO risultato
        FROM dpa_doc_arrivo_par
       WHERE id_mitt_dest = idcorr AND id_profile = idprofile;
   EXCEPTION
      WHEN OTHERS
      THEN
         risultato := 0;
   END;

   RETURN risultato;
END getsemittdest; 

/
