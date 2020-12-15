--------------------------------------------------------
--  DDL for Procedure VIS_FASC_ANOMALA_INT_DATE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."VIS_FASC_ANOMALA_INT_DATE" (
   p_id_amm       NUMBER,
   p_start_date   VARCHAR,
   p_end_date     VARCHAR
)
IS
--DICHIARAZIONI
   s_idg_security     NUMBER;
   s_ar_security      NUMBER;
   s_td_security      VARCHAR (2);
   s_vn_security      VARCHAR (255);
   s_idg_r_sup        NUMBER;
   n_id_gruppo        NUMBER;
   s_id_fascicolo     NUMBER;
   codice_atipicita   VARCHAR (255);
BEGIN
--CURSORE FASCICOLI
   DECLARE
      CURSOR fascicoli
      IS
         SELECT system_id
           FROM project
          WHERE dta_creazione BETWEEN TO_DATE (p_start_date,
                                               'dd/mm/yyyy hh24.mi.ss'
                                              )
                                  AND TO_DATE (p_end_date,
                                               'dd/mm/yyyy hh24.mi.ss'
                                              )
            AND id_amm = p_id_amm
            AND cha_tipo_fascicolo = 'P';
   BEGIN
      OPEN fascicoli;

      LOOP
         FETCH fascicoli
          INTO s_id_fascicolo;

         EXIT WHEN fascicoli%NOTFOUND;

         --Cursore sulla security per lo specifico fascicolo
         DECLARE
            CURSOR c_idg_security
            IS
               SELECT personorgroup, accessrights, cha_tipo_diritto,
                      var_note_sec
                 FROM security
                WHERE thing = s_id_fascicolo AND accessrights > 20;
         BEGIN
            OPEN c_idg_security;

            LOOP
               FETCH c_idg_security
                INTO s_idg_security, s_ar_security, s_td_security,
                     s_vn_security;

               EXIT WHEN c_idg_security%NOTFOUND;

               --Gerachia ruolo proprietario del fascicolo
               IF (UPPER (s_td_security) = 'P')
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('FASCICOLO : ' || s_id_fascicolo || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGRP'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGRP-';
                     END IF;
                  END;

                  COMMIT;
               END IF;

               --Gerarchia destinatario trasmissione
               IF (UPPER (s_td_security) = 'T')
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGDT'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGDT-';
                     END IF;
                  END;

                  COMMIT;
               END IF;

               --Gerarchia ruolo destinatario di copia visibilita
               IF (    UPPER (s_td_security) = 'A'
                   AND UPPER (s_vn_security) =
                                              'ACQUISITO PER COPIA VISIBILITA'
                  )
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGCV'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGCV-';
                     END IF;
                  END;

                  COMMIT;
               END IF;
            END LOOP;

            CLOSE c_idg_security;
         END;

         --Restituzione codice di atipicita
         IF (codice_atipicita IS NULL)
         THEN
            codice_atipicita := 'T';

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
            UPDATE project
               SET cha_cod_t_a = codice_atipicita
             WHERE system_id = s_id_fascicolo;

            COMMIT;
            codice_atipicita := NULL;
         END IF;

         IF (SUBSTR (codice_atipicita, LENGTH (codice_atipicita)) = '-')
         THEN
            codice_atipicita :=
                   SUBSTR (codice_atipicita, 0, LENGTH (codice_atipicita) - 1);

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
            UPDATE project
               SET cha_cod_t_a = codice_atipicita
             WHERE system_id = s_id_fascicolo;

            COMMIT;
            codice_atipicita := NULL;
         END IF;
      END LOOP;

      CLOSE fascicoli;
   END;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line ('Errore nell''esecuzione della procedura');
END; 

/
