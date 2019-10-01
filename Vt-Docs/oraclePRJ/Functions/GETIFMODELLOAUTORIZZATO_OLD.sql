--------------------------------------------------------
--  DDL for Function GETIFMODELLOAUTORIZZATO_OLD
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIFMODELLOAUTORIZZATO_OLD" (
   id_ruolo          NUMBER,
   id_people         NUMBER,
   system_id         NUMBER,
   id_modellotrasm   NUMBER
)
   RETURN NUMBER
IS
   retval         NUMBER;
   accesssrigth   NUMBER;
   idragione      NUMBER;
   tipo_diritto   CHAR;

   CURSOR cur
   IS
      SELECT DISTINCT id_ragione
                 FROM dpa_modelli_mitt_dest
                WHERE id_modello = id_modellotrasm
                  AND cha_tipo_mitt_dest <> 'M';
BEGIN
   retval := 1;
   accesssrigth := getaccessrights (id_ruolo, id_people, system_id);

   IF (accesssrigth = 45)
   THEN
      BEGIN
         OPEN cur;

         LOOP
            FETCH cur
             INTO idragione;

            EXIT WHEN cur%NOTFOUND;

            SELECT cha_tipo_diritti
              INTO tipo_diritto
              FROM dpa_ragione_trasm
             WHERE system_id = idragione;

            IF (tipo_diritto <> 'R' AND tipo_diritto <> 'C')
            THEN
               BEGIN
                  retval := 0;
               END;
            END IF;

            EXIT WHEN retval = 0;
         END LOOP;

         CLOSE cur;
      END;
   END IF;

   RETURN retval;
END getifmodelloautorizzato_old; 

/
