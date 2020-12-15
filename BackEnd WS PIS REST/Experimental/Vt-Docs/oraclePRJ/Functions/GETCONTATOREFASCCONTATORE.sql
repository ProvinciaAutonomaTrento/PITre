--------------------------------------------------------
--  DDL for Function GETCONTATOREFASCCONTATORE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETCONTATOREFASCCONTATORE" (systemid INT, tipocontatore CHAR)
   RETURN INT
IS
   risultato   VARCHAR (255);
BEGIN
   SELECT dpa_ass_templates_fasc.valore_oggetto_db
     INTO risultato
     FROM dpa_ass_templates_fasc,
          dpa_oggetti_custom_fasc,
          dpa_tipo_oggetto_fasc
    WHERE dpa_ass_templates_fasc.id_project = TO_CHAR (systemid)
      AND dpa_ass_templates_fasc.id_oggetto =
                                             dpa_oggetti_custom_fasc.system_id
      AND dpa_oggetti_custom_fasc.id_tipo_oggetto =
                                               dpa_tipo_oggetto_fasc.system_id
      AND dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
      AND dpa_oggetti_custom_fasc.da_visualizzare_ricerca = '1';

   RETURN TO_NUMBER (risultato);
END getcontatorefasccontatore; 

/
