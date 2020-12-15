--------------------------------------------------------
--  DDL for Function GETFASCPRIMARIA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETFASCPRIMARIA" (iprofile NUMBER, idfascicolo NUMBER)
   RETURN VARCHAR2
IS
   fascprimaria   VARCHAR2 (1);
BEGIN
   SELECT NVL (b.cha_fasc_primaria, '0')
     INTO fascprimaria
     FROM project a, project_components b
    WHERE a.system_id = b.project_id
      AND b.LINK = iprofile
      AND id_fascicolo = idfascicolo
-- modifica by F. Veltri , deve tornare sempre un solo risultato anche in caso di sottofascicolo
      AND ROWNUM < 2;

   RETURN fascprimaria;
EXCEPTION
   WHEN OTHERS
   THEN
      fascprimaria := '0';
      RETURN fascprimaria;
END getfascprimaria; 

/
