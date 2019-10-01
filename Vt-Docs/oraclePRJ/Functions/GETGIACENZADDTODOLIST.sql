--------------------------------------------------------
--  DDL for Function GETGIACENZADDTODOLIST
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETGIACENZADDTODOLIST" (
   docid        INT,
   chatiporag   CHAR
)
   RETURN VARCHAR
IS
   risultato   VARCHAR (2000);
BEGIN
   DECLARE
      maxdtaa   DATE;
      maxdtar   DATE;
      maxDtaV date;
   BEGIN
      IF (chatiporag = 'W')
      THEN
         BEGIN
            SELECT NVL (MAX (dta_accettata),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       ),
                   NVL (MAX (dta_rifiutata),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaa,
                   maxdtar
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;
             SELECT NVL (MAX (dta_invio),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaV
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;

            IF (maxdtaa > maxdtar)
            THEN
               SELECT NUMTODSINTERVAL (maxdtaa - maxdtaV, 'DAY')
                 INTO risultato
                 FROM dual;
            ELSE
               SELECT NUMTODSINTERVAL (maxdtar - maxdtaV, 'DAY')
                 INTO risultato
                 FROM dual;
                
            END IF;
         END;
      ELSE
         BEGIN
            SELECT NVL (MAX (dta_vista),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaa
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;
             SELECT NVL (MAX (dta_invio),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaV
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;

            SELECT NUMTODSINTERVAL (maxdtaa - maxDtaV, 'DAY')
              INTO risultato
            FROM dual;
         END;
      END IF;
   END;

   RETURN risultato;
END getgiacenzaddtodolist; 

/
