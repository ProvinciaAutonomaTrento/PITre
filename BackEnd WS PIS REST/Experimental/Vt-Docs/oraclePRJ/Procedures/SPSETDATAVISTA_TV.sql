--------------------------------------------------------
--  DDL for Procedure SPSETDATAVISTA_TV
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SPSETDATAVISTA_TV" (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
IS
/*
----------------------------------------------------------------------------------------

RICHIAMATA SOLO DAL TASTO VISTO, agisce solo sulle trasmissioni NO WKFL. TOGLIENDOLE DALLA TDL 
NON SETTA DATA VISTA PERCH LO FA la SP_SET_DATAVISTA_V2


dpa_trasm_singola.cha_tipo_trasm = 'S''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

dpa_trasm_singola.cha_tipo_trasm = 'T''
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
(ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
-  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
relativa all'utente corrente

*/
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                              -- dpa_trasm_utente.dta_vista =
                               --   (CASE
                               --       WHEN dta_vista IS NULL
                               --          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE -- dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                             -- --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                 /* BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (p_iddelegato = 0)
                        THEN
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--in caso di delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  )
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1');

--AND dpa_trasm_utente.id_people = p_idpeople;
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
                   OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                  )
               THEN
-- SE ? una trasmissione senza workFlow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET --dpa_trasm_utente.cha_vista = '1',
                               --dpa_trasm_utente.dta_vista =
                              --    (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                              --     END
                              --    ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
--caso in cui si sta esercitando una delega
                     BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                        UPDATE dpa_trasm_utente
                           SET  --dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                               --dpa_trasm_utente.dta_vista =
                               --   (CASE
                              --        WHEN dta_vista IS NULL
                              --           THEN SYSDATE
                              --        ELSE dta_vista
                             --      END
                             --     ),
                               dpa_trasm_utente.cha_in_todolist = '0'
                         WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                /*  BEGIN
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END; */

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        IF (p_iddelegato = 0)
                        THEN
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET --dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                  id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        ELSE
                           BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                              UPDATE dpa_trasm_utente dpa_trasm_utente
                                 SET -- dpa_trasm_utente.cha_vista = '1',
                                     dpa_trasm_utente.cha_vista_delegato = '1',
                                     dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato,
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                                                                id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                                 AND dpa_trasm_utente.id_people != p_idpeople
                                 AND EXISTS (
                                        SELECT 'x'
                                          FROM dpa_trasm_utente a
                                         WHERE a.id_trasm_singola =
                                                  dpa_trasm_utente.id_trasm_singola
                                           AND a.id_people = p_idpeople);
                                  --ovvero solo se io sono tra i notificati!!!
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  IF (p_iddelegato = 0)
                  THEN
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                              dpa_trasm_utente.dta_vista =
                                  (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 )
                         WHERE dpa_trasm_utente.dta_vista IS NULL and
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  ELSE
-- caso in cui si sta esercitando una delega
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET -- dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.cha_vista_delegato = '1',
                               dpa_trasm_utente.id_people_delegato =
                                                                  p_iddelegato--,
                              -- dpa_trasm_utente.dta_vista =
                              --    (CASE
                               --       WHEN dta_vista IS NULL
                               ---          THEN SYSDATE
                               --       ELSE dta_vista
                               --    END
                               --   )
                         WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                            id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                           AND dpa_trasm_utente.id_people = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;
                  END IF;

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     UPDATE dpa_trasm_utente
                        SET cha_in_todolist = '0'
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND NOT dpa_trasm_utente.dta_vista IS NULL
                        AND (cha_accettata = '1' OR cha_rifiutata = '1')
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
--and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavista_tv;

/
