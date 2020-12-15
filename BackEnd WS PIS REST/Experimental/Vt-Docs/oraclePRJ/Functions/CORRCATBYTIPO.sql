--------------------------------------------------------
--  DDL for Function CORRCATBYTIPO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."CORRCATBYTIPO" (
   docid        INT,
   tipo_proto   VARCHAR,
   tipocorr     VARCHAR
)
   RETURN varchar IS risultato clob;
item clob;
tipo_mitt_dest VARCHAR(10);
LNG INT;
   CURSOR cur
   IS
      SELECT   c.var_desc_corr, dap.cha_tipo_mitt_dest
          FROM dpa_corr_globali c, dpa_doc_arrivo_par dap
         WHERE dap.id_profile = docid AND dap.id_mitt_dest = c.system_id
      ORDER BY dap.cha_tipo_mitt_dest DESC;
BEGIN
   risultato := '';

   OPEN cur;

   LOOP
      FETCH cur
       INTO item, tipo_mitt_dest;

      EXIT WHEN cur%NOTFOUND;
      lng := LENGTH (risultato);

      IF (risultato IS NOT NULL AND lng >= (3900 - 128))
      THEN
         RETURN risultato || '...';
      ELSE
         BEGIN
            IF (tipocorr = 'M')
            THEN
               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'MD')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (MM)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'A' AND tipo_mitt_dest = 'I')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (MI)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'M')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (M)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;
            END IF;

            IF (tipocorr = 'D')
            THEN
               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'D')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (D)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'C')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (CC)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'D')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (D)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'I' AND tipo_mitt_dest = 'C')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || item || ' (CC)';
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

               IF (tipo_proto = 'P' AND tipo_mitt_dest = 'L')
               THEN
                  IF (risultato IS NOT NULL)
                  THEN
                     risultato := risultato || '; ' || dest_in_lista (docid);
                  ELSE
                     risultato := risultato || item;
                  END IF;
               END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'F') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| item || '(D) ';


ELSE
risultato := risultato||item;
END IF;
END IF;
            END IF;
         END;
      END IF;
   END LOOP;

   RETURN risultato;
END corrcatbytipo; 

/
