--------------------------------------------------------
--  DDL for Function GETMAXVER_MENO1
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETMAXVER_MENO1" (docnum NUMBER)
   RETURN NUMBER
IS
   tmpvar   NUMBER;
BEGIN
   DECLARE
      t1   NUMBER;
      v2   NUMBER;
   BEGIN
      SELECT /*+index (c) index (v1)*/
             (MAX (v1.version_label) - 2)
        INTO t1
        FROM VERSIONS v1, components c
       WHERE v1.docnumber = docnum
         AND v1.version_id = c.version_id
         AND c.file_size > 0;

      SELECT /*+index (v1)*/
             v1.version_id
        INTO v2
        FROM VERSIONS v1                                      --, components c
       WHERE v1.docnumber = docnum AND v1.version_label = t1;

      -- AND c.file_size > 0
      SELECT /*+index (c) */
             c.version_id
        INTO tmpvar
        FROM components c
       WHERE c.file_size > 0 AND c.docnumber = docnum AND c.version_id = v2;

      IF (tmpvar IS NULL)
      THEN
         tmpvar := 0;
      END IF;

      RETURN tmpvar;
   EXCEPTION
      WHEN OTHERS
      THEN
         tmpvar := 0;
   END;
END getmaxver_meno1; 

/
