--------------------------------------------------------
--  DDL for Procedure SET_MEZZO_INTEROP_FOR_RC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SET_MEZZO_INTEROP_FOR_RC" 
IS
/******************************************************************************
   NAME:       set_mezzo_interop_for_RC
   PURPOSE:  importando il corrispondente da RC il mezzo di spedizione deve
   essre MAIL se sono settati solo i campo email.
   oppure INTEROPERABILITA SE SONO SETTATI I CAMPI VAR_COD_AOO, COM_AMM E EMAIL
   OPPURE LETTERE SE NESSUNO DEI TRA CAMPI E' SETTATO.

   C'e STATO UN BUG PER CUI SE IL CORRISPONDENTE ERA GIa STATO SCARICATO E SI MODIFICANO
   QUESTI CAMPI IN RC, IL MEZZO O NON SI AGGIORNA OPPURE SI AGGIUNGE NELLA DOCUMENTTYPES
   ANZICHe FARE UPDATE..
   QUESTA SP SISTEMA LE COSE..



******************************************************************************/
BEGIN
   DECLARE
      interop   NUMBER;
      mail      NUMBER;
      lettera   NUMBER;

      CURSOR c
      IS
         SELECT   system_id, var_codice_amm var_cod_amm,
                  var_codice_aoo var_cod_aoo, var_email email
             FROM dpa_corr_globali d
            WHERE cha_tipo_corr IN ('C', 'S') AND cha_tipo_ie = 'E'
         --and d.system_id=21104311
         ORDER BY system_id;

      rc        c%ROWTYPE;
   BEGIN
      OPEN c;

      LOOP
         FETCH c
          INTO rc;

         EXIT WHEN c%NOTFOUND;

         SELECT system_id
           INTO interop
           FROM documenttypes d
          WHERE UPPER (d.description) IN ('INTEROPERABILITA');

         SELECT system_id
           INTO lettera
           FROM documenttypes d
          WHERE UPPER (d.description) IN ('LETTERA');

         SELECT system_id
           INTO mail
           FROM documenttypes d
          WHERE UPPER (d.description) IN ('MAIL');

         IF (rc.email IS NOT NULL)
         THEN
            BEGIN
               IF (rc.var_cod_aoo IS NOT NULL)
               THEN
                  BEGIN
                     IF (rc.var_cod_amm IS NOT NULL)
                     THEN
                        BEGIN
--caso interop
                           DELETE FROM dpa_t_canale_corr d
                                 WHERE d.id_corr_globale = rc.system_id;

                           INSERT INTO dpa_t_canale_corr
                                       (system_id, id_corr_globale,
                                        id_documenttype, cha_preferito
                                       )
                                VALUES (seq.NEXTVAL, rc.system_id,
                                        interop, '1'
                                       );

                           COMMIT;
                        END;
                     ELSE
                        --EMAIL NOT  NULL VAR_COD_AOO NOT  NULL MA VAR_COD_AMM NULL, QUINDI e MAIL
                        BEGIN
                           DELETE FROM dpa_t_canale_corr d
                                 WHERE d.id_corr_globale = rc.system_id;

                           INSERT INTO dpa_t_canale_corr
                                       (system_id, id_corr_globale,
                                        id_documenttype, cha_preferito
                                       )
                                VALUES (seq.NEXTVAL, rc.system_id,
                                        mail, '1'
                                       );

                           COMMIT;
                        END;
                     END IF;
                  END;
               ELSE
                  --EMAIL NOT NULL, MA VAR_COD_AOO NULL QUINDI e MAIL
                  BEGIN
                     DELETE FROM dpa_t_canale_corr d
                           WHERE d.id_corr_globale = rc.system_id;

                     INSERT INTO dpa_t_canale_corr
                                 (system_id, id_corr_globale,
                                  id_documenttype, cha_preferito
                                 )
                          VALUES (seq.NEXTVAL, rc.system_id,
                                  mail, '1'
                                 );

                     COMMIT;
                  END;
               END IF;
            END;
         ELSE                      --EMAIL NULL OR EMPTY SICURAMENTE e LETTERA
            BEGIN
               DELETE FROM dpa_t_canale_corr d
                     WHERE d.id_corr_globale = rc.system_id;

               INSERT INTO dpa_t_canale_corr
                           (system_id, id_corr_globale, id_documenttype,
                            cha_preferito
                           )
                    VALUES (seq.NEXTVAL, rc.system_id, lettera,
                            '1'
                           );

               COMMIT;
            END;
         END IF;
      END LOOP;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         RAISE;
      WHEN OTHERS
      THEN
         -- Consider logging the error and then re-raise
         RAISE;

         CLOSE c;
   END;
END; 

/
