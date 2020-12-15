--------------------------------------------------------
--  DDL for Function GETNUMMITTENTIMODELLITRASM
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETNUMMITTENTIMODELLITRASM" (
                                     -- Id del template da analizzare
                                     templateid NUMBER)
   -- Numero dei mittenti del modello
RETURN NUMBER
IS
   retval   NUMBER;
/******************************************************************************

    AUTHOR:    Samuele Furnari

   NAME:       GetNumMittentiModelliTrasm

   PURPOSE:

        Funzione per contare il numero di mittenti di un modello
        di trasmissione

******************************************************************************/
BEGIN
   retval := 0;

   -- Conteggio dei mittenti del modello con system_id pari a quello passato
   -- per parametro
   SELECT COUNT ('x')
     INTO retval
     FROM dpa_modelli_trasm mt INNER JOIN dpa_modelli_mitt_dest md
          ON mt.system_id = md.id_modello
    WHERE mt.system_id = templateid AND md.cha_tipo_mitt_dest = 'M';

   RETURN retval;
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN -1;
END getnummittentimodellitrasm; 

/
