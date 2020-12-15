--------------------------------------------------------
--  DDL for Procedure VIS_DOC_ANOMALA_CUSTOM
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."VIS_DOC_ANOMALA_CUSTOM" (p_id_amm NUMBER, p_querydoc VARCHAR)

IS

--DICHIARAZIONI

   s_idg_security       NUMBER;

   s_ar_security        NUMBER;

   s_td_security        VARCHAR (2);

   s_vn_security        VARCHAR (255);

   s_idg_r_sup          NUMBER;

   s_doc_number         NUMBER;

   s_id_fascicolo       NUMBER;

   s_cha_cod_t_a_fasc   VARCHAR (1024);

   n_id_gruppo          NUMBER;

   codice_atipicita     VARCHAR (255);

BEGIN

--CURSORE DOCUMENTI

   DECLARE

      TYPE empcurtyp IS REF CURSOR;

 

      documenti   empcurtyp;

   BEGIN

      OPEN documenti FOR p_querydoc;

 

      LOOP

         FETCH documenti

          INTO s_doc_number;

 

         EXIT WHEN documenti%NOTFOUND;

 

         --Cursore sulla security per lo specifico documento

         DECLARE

            CURSOR c_idg_security

            IS

               SELECT personorgroup, accessrights, cha_tipo_diritto,

                      var_note_sec

                 FROM security

                WHERE thing = s_doc_number AND accessrights > 20;

         BEGIN

            OPEN c_idg_security;

 

            LOOP

               FETCH c_idg_security

                INTO s_idg_security, s_ar_security, s_td_security,

                     s_vn_security;

 

               EXIT WHEN c_idg_security%NOTFOUND;

 

               --Gerachia ruolo proprietario documento

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

 

                        --DBMS_OUTPUT.PUT_LINE('DOCUMENTO : ' || s_doc_number || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);

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

                              WHERE thing = s_doc_number);

 

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

 

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);

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

                              WHERE thing = s_doc_number);

 

                     IF (    n_id_gruppo <> 0

                         AND NVL (INSTR (codice_atipicita, 'AGDT'), 0) = 0

                        )

                     THEN

                        codice_atipicita := codice_atipicita || 'AGDT-';

                     END IF;

                  END;

 

                  COMMIT;

               END IF;

 

               --Fascicolazione documento

               IF (UPPER (s_td_security) = 'F')

               THEN

                  DECLARE

                     CURSOR fascicoli

                     IS

                        SELECT system_id

                          FROM project

                         WHERE system_id IN (

                                  SELECT id_fascicolo

                                    FROM project

                                   WHERE system_id IN (

                                                    SELECT project_id

                                                      FROM project_components

                                                     WHERE LINK =

                                                                 s_doc_number))

                           AND cha_tipo_fascicolo = 'P';

                  BEGIN

                     OPEN fascicoli;

 

                     LOOP

                        FETCH fascicoli

                         INTO s_id_fascicolo;

 

                        EXIT WHEN fascicoli%NOTFOUND;

 

                        SELECT cha_cod_t_a

                          INTO s_cha_cod_t_a_fasc

                          FROM project

                         WHERE system_id = s_id_fascicolo;

 

                        IF (    s_cha_cod_t_a_fasc IS NOT NULL

                            AND UPPER (s_cha_cod_t_a_fasc) <> 'T'

                           )

                        THEN

                           IF (NVL (INSTR (codice_atipicita, 'AFCD'), 0) = 0

                              )

                           THEN

                              codice_atipicita := codice_atipicita || 'AFCD-';

                           END IF;

                        END IF;

                     END LOOP;

 

                     CLOSE fascicoli;

                  END;

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

 

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE DOCUMENTO : ' || s_doc_number || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);

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

                              WHERE thing = s_doc_number);

 

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

 

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

            UPDATE PROFILE

               SET cha_cod_t_a = codice_atipicita

             WHERE docnumber = s_doc_number;

 

            COMMIT;

            codice_atipicita := NULL;

         END IF;

 

         IF (SUBSTR (codice_atipicita, LENGTH (codice_atipicita)) = '-')

         THEN

            codice_atipicita :=

                   SUBSTR (codice_atipicita, 0, LENGTH (codice_atipicita) - 1);

 

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Documento ' || s_doc_number || ' - ' || codice_atipicita);

            UPDATE PROFILE

               SET cha_cod_t_a = codice_atipicita

             WHERE docnumber = s_doc_number;

 

            COMMIT;

            codice_atipicita := NULL;

         END IF;

      END LOOP;

 

      CLOSE documenti;

   END;

EXCEPTION

   WHEN OTHERS

   THEN

      DBMS_OUTPUT.put_line ('Errore nell''esecuzione della procedura');

END; 

/
