--------------------------------------------------------
--  DDL for Function GETESITOPUBBLICAZIONE
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETESITOPUBBLICAZIONE" (p_systemid INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
   esito       VARCHAR (256);
   errore      VARCHAR (256);
BEGIN
   SELECT esito_pubblicazione, errore_pubblicazione
     INTO esito, errore
     FROM pubblicazioni_documenti
    WHERE id_profile = p_systemid;

   IF (errore IS NOT NULL)
   THEN
      risultato := 'Il documento non e stato pubblicato ' || errore;
   ELSE
      IF (esito = '1' AND errore IS NULL)
      THEN
         risultato := 'Il documento e stato pubblicato';
      ELSE
         IF (esito = '0' AND errore IS NULL)
         THEN
            risultato := 'Il documento non e stato pubblicato';
         END IF;
      END IF;
   END IF;

   RETURN risultato;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      risultato := NULL;
      RETURN risultato;
END getesitopubblicazione; 

/
