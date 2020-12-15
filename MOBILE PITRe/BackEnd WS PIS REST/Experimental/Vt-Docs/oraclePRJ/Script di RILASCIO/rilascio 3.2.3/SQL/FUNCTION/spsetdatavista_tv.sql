ALTER PROCEDURE [DOCSADM].[spsetdatavista_tv] 
@idpeople     INT       ,
@idoggetto     INT       ,
@idgruppo      INT       ,
@tipooggetto   CHAR(2000)       ,
@iddelegato    INT       ,
@resultvalue   INT OUTPUT 
AS
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
BEGIN
   DECLARE @p_cha_tipo_trasm   CHAR(1) = NULL
   DECLARE @p_chatipodest      INT
   DECLARE @error INT
   SET @resultvalue = 0

   BEGIN
      DECLARE @cursortrasmsingoladocumento CURSOR
      DECLARE @cursortrasmsingolafascicolo CURSOR
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_system_id VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione VARCHAR(255)
      DECLARE @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest VARCHAR(255)
      IF (@tipooggetto = 'D')
      begin
         SET @cursortrasmsingoladocumento = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @idpeople))
         AND a.id_profile = @idoggetto
         AND b.id_ragione = c.system_id
         
		 OPEN @cursortrasmsingoladocumento
         FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
         @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               OR @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople

                     INSERT INTO DPA_NOTIFY_HISTORY (
                                       ID_NOTIFY,
                                       ID_EVENT,
                                       DESC_PRODUCER,
                                       ID_PEOPLE_RECEIVER,
                                       ID_GROUP_RECEIVER,
                                       TYPE_NOTIFY,
                                       DTA_NOTIFY,
                                       FIELD_1,
                                       FIELD_2,
                                       FIELD_3,
                                       FIELD_4,
                                       MULTIPLICITY,
                                       SPECIALIZED_FIELD,
                                       TYPE_EVENT,
                                       DOMAINOBJECT,
                                       ID_OBJECT,
                                       ID_SPECIALIZED_OBJECT,
                                       DTA_EVENT,
                                       READ_NOTIFICATION,
                                       NOTES)
                            SELECT SYSTEM_ID,
                                  ID_EVENT,
                                  DESC_PRODUCER,
                                  ID_PEOPLE_RECEIVER,
                                  ID_GROUP_RECEIVER,
                                  TYPE_NOTIFY,
                                  DTA_NOTIFY,
                                  FIELD_1,
                                  FIELD_2,
                                  FIELD_3,
                                  FIELD_4,
                                  MULTIPLICITY,
                                  SPECIALIZED_FIELD,
                                  TYPE_EVENT,
                                  DOMAINOBJECT,
                                  ID_OBJECT,
                                  ID_SPECIALIZED_OBJECT,
                                  DTA_EVENT,
                                  READ_NOTIFICATION,
                                  NOTES
                            FROM DPA_NOTIFY
                            WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                            AND ID_PEOPLE_RECEIVER = @idpeople
                            AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                        -- elimino la ntoifica
                        DELETE FROM DPA_NOTIFY
                        WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND ID_PEOPLE_RECEIVER = @idpeople
                        AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)


                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @iddelegato,cha_in_todolist = '0'  WHERE
                     id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
					 
					 INSERT INTO DPA_NOTIFY_HISTORY (
                                       ID_NOTIFY,
                                       ID_EVENT,
                                       DESC_PRODUCER,
                                       ID_PEOPLE_RECEIVER,
                                       ID_GROUP_RECEIVER,
                                       TYPE_NOTIFY,
                                       DTA_NOTIFY,
                                       FIELD_1,
                                       FIELD_2,
                                       FIELD_3,
                                       FIELD_4,
                                       MULTIPLICITY,
                                       SPECIALIZED_FIELD,
                                       TYPE_EVENT,
                                       DOMAINOBJECT,
                                       ID_OBJECT,
                                       ID_SPECIALIZED_OBJECT,
                                       DTA_EVENT,
                                       READ_NOTIFICATION,
                                       NOTES)
                            SELECT SYSTEM_ID,
                                  ID_EVENT,
                                  DESC_PRODUCER,
                                  ID_PEOPLE_RECEIVER,
                                  ID_GROUP_RECEIVER,
                                  TYPE_NOTIFY,
                                  DTA_NOTIFY,
                                  FIELD_1,
                                  FIELD_2,
                                  FIELD_3,
                                  FIELD_4,
                                  MULTIPLICITY,
                                  SPECIALIZED_FIELD,
                                  TYPE_EVENT,
                                  DOMAINOBJECT,
                                  ID_OBJECT,
                                  ID_SPECIALIZED_OBJECT,
                                  DTA_EVENT,
                                  READ_NOTIFICATION,
                                  NOTES
                            FROM DPA_NOTIFY
                            WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                            AND ID_PEOPLE_RECEIVER = @idpeople
                            AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                        -- elimino la ntoifica
                        DELETE FROM DPA_NOTIFY
                        WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND ID_PEOPLE_RECEIVER = @idpeople
                        AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

					 
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END


                  BEGIN
                     IF (@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest = 'R')
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        IF (@iddelegato = 0)
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           
							update dpa_trasm_utente  set cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE
							id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
							AND dpa_trasm_utente.id_people != @idpeople
							AND EXISTS(SELECT 'x'
							FROM dpa_trasm_utente a
							WHERE a.id_trasm_singola =
							dpa_trasm_utente.id_trasm_singola
							AND a.id_people = @idpeople)
						   
						    INSERT INTO DPA_NOTIFY_HISTORY (
                                             ID_NOTIFY,
                                             ID_EVENT,
                                             DESC_PRODUCER,
                                             ID_PEOPLE_RECEIVER,
                                             ID_GROUP_RECEIVER,
                                             TYPE_NOTIFY,
                                             DTA_NOTIFY,
                                             FIELD_1,
                                             FIELD_2,
                                             FIELD_3,
                                             FIELD_4,
                                             MULTIPLICITY,
                                             SPECIALIZED_FIELD,
                                             TYPE_EVENT,
                                             DOMAINOBJECT,
                                             ID_OBJECT,
                                             ID_SPECIALIZED_OBJECT,
                                             DTA_EVENT,
                                             READ_NOTIFICATION,
                                             NOTES)
                                SELECT  SYSTEM_ID,
                                        ID_EVENT,
                                        DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY,
                                        DTA_NOTIFY,
                                        FIELD_1,
                                        FIELD_2,
                                        FIELD_3,
                                        FIELD_4,
                                        MULTIPLICITY,
                                        SPECIALIZED_FIELD,
                                        TYPE_EVENT,
                                        DOMAINOBJECT,
                                        ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT,
                                        READ_NOTIFICATION,
                                        NOTES
                                FROM DPA_NOTIFY
                                WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                AND ID_PEOPLE_RECEIVER IN
                                    (SELECT id_people FROM dpa_trasm_utente
                                     WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                     AND id_people != @idpeople
                                     AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                                WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                                AND a.id_people = @idpeople))
                                     AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                            -- elimino la notifica
                            DELETE FROM DPA_NOTIFY
                            WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                            AND ID_PEOPLE_RECEIVER IN
                                (SELECT id_people FROM dpa_trasm_utente
                                 WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                 AND id_people != @idpeople
                                 AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                             WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                             AND a.id_people = @idpeople))
                                 AND ( ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

						   
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
							update dpa_trasm_utente  set cha_vista_delegato = '1',id_people_delegato =
							@iddelegato,cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE
							id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
							AND dpa_trasm_utente.id_people != @idpeople
							AND EXISTS(SELECT 'x'
							FROM dpa_trasm_utente a
							WHERE a.id_trasm_singola =
							dpa_trasm_utente.id_trasm_singola
							AND a.id_people = @idpeople)
						   
						    INSERT INTO DPA_NOTIFY_HISTORY (
                                             ID_NOTIFY,
                                             ID_EVENT,
                                             DESC_PRODUCER,
                                             ID_PEOPLE_RECEIVER,
                                             ID_GROUP_RECEIVER,
                                             TYPE_NOTIFY,
                                             DTA_NOTIFY,
                                             FIELD_1,
                                             FIELD_2,
                                             FIELD_3,
                                             FIELD_4,
                                             MULTIPLICITY,
                                             SPECIALIZED_FIELD,
                                             TYPE_EVENT,
                                             DOMAINOBJECT,
                                             ID_OBJECT,
                                             ID_SPECIALIZED_OBJECT,
                                             DTA_EVENT,
                                             READ_NOTIFICATION,
                                             NOTES)
                                SELECT  SYSTEM_ID,
                                        ID_EVENT,
                                        DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY,
                                        DTA_NOTIFY,
                                        FIELD_1,
                                        FIELD_2,
                                        FIELD_3,
                                        FIELD_4,
                                        MULTIPLICITY,
                                        SPECIALIZED_FIELD,
                                        TYPE_EVENT,
                                        DOMAINOBJECT,
                                        ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT,
                                        READ_NOTIFICATION,
                                        NOTES
                                    FROM DPA_NOTIFY
                                    WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                    AND ID_PEOPLE_RECEIVER IN
                                        (SELECT id_people FROM dpa_trasm_utente
                                         WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                         AND id_people != @idpeople
                                         AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                                    WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                                    AND a.id_people = @idpeople))
                                         AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)


                            -- elimino la notifica
                            DELETE FROM DPA_NOTIFY
                            WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                            AND ID_PEOPLE_RECEIVER IN
                                (SELECT id_people FROM dpa_trasm_utente
                                 WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                                 AND id_people != @idpeople
                                 AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                             WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                             AND a.id_people = @idpeople))
                                 AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

						   
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- la ragione di trasmissione prevede workflow
               begin
                  IF (@iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--in caso di delega
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',cha_vista_delegato = '1',id_people_delegato =
                     @iddelegato,
                     dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL
                     AND id_trasm_singola =(@CURSORTRASMSINGOLADOCUMENTO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata

                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND id_people_dest = @idpeople
                        AND id_profile = @idoggetto
                        SELECT   @error = @@ERROR
                     end
					 
					INSERT INTO DPA_NOTIFY_HISTORY ( ID_NOTIFY,
                                                     ID_EVENT,
                                                     DESC_PRODUCER,
                                                     ID_PEOPLE_RECEIVER,
                                                     ID_GROUP_RECEIVER,
                                                     TYPE_NOTIFY,
                                                     DTA_NOTIFY,
                                                     FIELD_1,
                                                     FIELD_2,
                                                     FIELD_3,
                                                     FIELD_4,
                                                     MULTIPLICITY,
                                                     SPECIALIZED_FIELD,
                                                     TYPE_EVENT,
                                                     DOMAINOBJECT,
                                                     ID_OBJECT,
                                                     ID_SPECIALIZED_OBJECT,
                                                     DTA_EVENT,
                                                     READ_NOTIFICATION)
                        SELECT SYSTEM_ID,
                               ID_EVENT,
                               DESC_PRODUCER,
                               ID_PEOPLE_RECEIVER,
                               ID_GROUP_RECEIVER,
                               TYPE_NOTIFY,
                               DTA_NOTIFY,
                               FIELD_1,
                               FIELD_2,
                               FIELD_3,
                               FIELD_4,
                               MULTIPLICITY,
                               SPECIALIZED_FIELD,
                               TYPE_EVENT,
                               DOMAINOBJECT,
                               ID_OBJECT,
                               ID_SPECIALIZED_OBJECT,
                               DTA_EVENT,
                               READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                        AND ID_PEOPLE_RECEIVER IN
                            (SELECT ID_PEOPLE FROM dpa_trasm_utente
                             WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                             AND dta_vista IS NOT NULL
                             AND ( cha_accettata = '1' OR cha_rifiutata = '1'))
                             AND ( ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                    -- elimino la ntoifica
                    DELETE FROM DPA_NOTIFY
                    WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLADOCUMENTO_system_id
                    AND ID_PEOPLE_RECEIVER IN
                        (SELECT ID_PEOPLE FROM dpa_trasm_utente
                         WHERE     id_trasm_singola = @CURSORTRASMSINGOLADOCUMENTO_system_id
                         AND dta_vista IS NOT NULL
                         AND (   cha_accettata = '1' OR cha_rifiutata = '1'))
                         AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                     -- imposto come letta la notifica per l'utente p_id_people
                    UPDATE dpa_notify
                    SET read_notification = '1'
                    WHERE     id_specialized_object = @CURSORTRASMSINGOLADOCUMENTO_system_id
                    AND id_people_receiver = @idpeople
					 
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingoladocumento INTO @CURSORTRASMSINGOLADOCUMENTO_system_id,@CURSORTRASMSINGOLADOCUMENTO_cha_tipo_trasm,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_ragione,
            @CURSORTRASMSINGOLADOCUMENTO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingoladocumento
      end

      IF (@tipooggetto = 'F')
      begin
         SET @cursortrasmsingolafascicolo = CURSOR  FOR SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
b.cha_tipo_dest
         FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
         WHERE a.dta_invio IS NOT NULL
         AND a.system_id = b.id_trasmissione
         AND (b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_gruppo = @idgruppo)
         OR b.id_corr_globale =(SELECT system_id
            FROM dpa_corr_globali
            WHERE id_people = @idpeople))
         AND a.id_project = @idoggetto
         AND b.id_ragione = c.system_id
         OPEN @cursortrasmsingolafascicolo
         FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
         @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         while @@FETCH_STATUS = 0
         begin
            BEGIN
               IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'N'
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'I'
-- modifica della 3.21 by S. Furnari
               OR @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione = 'S' -- Interoperabilit semplificata
)
-- SE ? una trasmissione senza workFlow
               begin
                  IF (@iddelegato = 0)
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
					 
					 INSERT INTO DPA_NOTIFY_HISTORY (
                                       ID_NOTIFY,
                                       ID_EVENT,
                                       DESC_PRODUCER,
                                       ID_PEOPLE_RECEIVER,
                                       ID_GROUP_RECEIVER,
                                       TYPE_NOTIFY,
                                       DTA_NOTIFY,
                                       FIELD_1,
                                       FIELD_2,
                                       FIELD_3,
                                       FIELD_4,
                                       MULTIPLICITY,
                                       SPECIALIZED_FIELD,
                                       TYPE_EVENT,
                                       DOMAINOBJECT,
                                       ID_OBJECT,
                                       ID_SPECIALIZED_OBJECT,
                                       DTA_EVENT,
                                       READ_NOTIFICATION,
                                       NOTES)
                            SELECT SYSTEM_ID,
                                  ID_EVENT,
                                  DESC_PRODUCER,
                                  ID_PEOPLE_RECEIVER,
                                  ID_GROUP_RECEIVER,
                                  TYPE_NOTIFY,
                                  DTA_NOTIFY,
                                  FIELD_1,
                                  FIELD_2,
                                  FIELD_3,
                                  FIELD_4,
                                  MULTIPLICITY,
                                  SPECIALIZED_FIELD,
                                  TYPE_EVENT,
                                  DOMAINOBJECT,
                                  ID_OBJECT,
                                  ID_SPECIALIZED_OBJECT,
                                  DTA_EVENT,
                                  READ_NOTIFICATION,
                                  NOTES
                            FROM DPA_NOTIFY
                            WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                            AND ID_PEOPLE_RECEIVER = @idpeople
                            AND ( ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                        -- elimino la notifica
                        DELETE FROM DPA_NOTIFY
                        WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND ID_PEOPLE_RECEIVER = @idpeople
                        AND ( ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
--caso in cui si sta esercitando una delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @iddelegato,cha_in_todolist = '0'  WHERE 
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
					 
					 INSERT INTO DPA_NOTIFY_HISTORY (
                                       ID_NOTIFY,
                                       ID_EVENT,
                                       DESC_PRODUCER,
                                       ID_PEOPLE_RECEIVER,
                                       ID_GROUP_RECEIVER,
                                       TYPE_NOTIFY,
                                       DTA_NOTIFY,
                                       FIELD_1,
                                       FIELD_2,
                                       FIELD_3,
                                       FIELD_4,
                                       MULTIPLICITY,
                                       SPECIALIZED_FIELD,
                                       TYPE_EVENT,
                                       DOMAINOBJECT,
                                       ID_OBJECT,
                                       ID_SPECIALIZED_OBJECT,
                                       DTA_EVENT,
                                       READ_NOTIFICATION,
                                       NOTES)
                            SELECT SYSTEM_ID,
                                  ID_EVENT,
                                  DESC_PRODUCER,
                                  ID_PEOPLE_RECEIVER,
                                  ID_GROUP_RECEIVER,
                                  TYPE_NOTIFY,
                                  DTA_NOTIFY,
                                  FIELD_1,
                                  FIELD_2,
                                  FIELD_3,
                                  FIELD_4,
                                  MULTIPLICITY,
                                  SPECIALIZED_FIELD,
                                  TYPE_EVENT,
                                  DOMAINOBJECT,
                                  ID_OBJECT,
                                  ID_SPECIALIZED_OBJECT,
                                  DTA_EVENT,
                                  READ_NOTIFICATION,
                                  NOTES
                            FROM DPA_NOTIFY
                            WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                            AND ID_PEOPLE_RECEIVER = @idpeople
                            AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                        -- elimino la notifica
                        DELETE FROM DPA_NOTIFY
                        WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND ID_PEOPLE_RECEIVER = @idpeople
                        AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)
					 
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
                     IF (@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm = 'S'
                     AND @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest = 'R')
                        IF (@iddelegato = 0)
-- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE 
                           id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @idpeople)
						   
						    INSERT INTO DPA_NOTIFY_HISTORY (
                                             ID_NOTIFY,
                                             ID_EVENT,
                                             DESC_PRODUCER,
                                             ID_PEOPLE_RECEIVER,
                                             ID_GROUP_RECEIVER,
                                             TYPE_NOTIFY,
                                             DTA_NOTIFY,
                                             FIELD_1,
                                             FIELD_2,
                                             FIELD_3,
                                             FIELD_4,
                                             MULTIPLICITY,
                                             SPECIALIZED_FIELD,
                                             TYPE_EVENT,
                                             DOMAINOBJECT,
                                             ID_OBJECT,
                                             ID_SPECIALIZED_OBJECT,
                                             DTA_EVENT,
                                             READ_NOTIFICATION,
                                             NOTES)
                                SELECT SYSTEM_ID,
                                        ID_EVENT,
                                        DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY,
                                        DTA_NOTIFY,
                                        FIELD_1,
                                        FIELD_2,
                                        FIELD_3,
                                        FIELD_4,
                                        MULTIPLICITY,
                                        SPECIALIZED_FIELD,
                                        TYPE_EVENT,
                                        DOMAINOBJECT,
                                        ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT,
                                        READ_NOTIFICATION,
                                        NOTES
                                FROM DPA_NOTIFY
                                WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                AND ID_PEOPLE_RECEIVER IN
                                    (SELECT id_people FROM dpa_trasm_utente
                                    WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                    AND id_people != @idpeople
                                    AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                                WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                                AND a.id_people = @idpeople))
                                    AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                            -- elimino la notifica
                            DELETE FROM DPA_NOTIFY
                            WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                            AND ID_PEOPLE_RECEIVER IN
                                (SELECT id_people FROM dpa_trasm_utente
                                 WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                 AND id_people != @idpeople
                                 AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                             WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                             AND a.id_people = @idpeople))
                                 AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

						   
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END
                     ELSE
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           update dpa_trasm_utente  set cha_vista_delegato = '1',id_people_delegato =
                           @iddelegato,cha_in_todolist = '0' FROM dpa_trasm_utente dpa_trasm_utente WHERE 
                           id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                           AND dpa_trasm_utente.id_people != @idpeople
                           AND EXISTS(SELECT 'x'
                           FROM dpa_trasm_utente a
                           WHERE a.id_trasm_singola =
                           dpa_trasm_utente.id_trasm_singola
                           AND a.id_people = @idpeople)
						   
						    INSERT INTO DPA_NOTIFY_HISTORY (
                                             ID_NOTIFY,
                                             ID_EVENT,
                                             DESC_PRODUCER,
                                             ID_PEOPLE_RECEIVER,
                                             ID_GROUP_RECEIVER,
                                             TYPE_NOTIFY,
                                             DTA_NOTIFY,
                                             FIELD_1,
                                             FIELD_2,
                                             FIELD_3,
                                             FIELD_4,
                                             MULTIPLICITY,
                                             SPECIALIZED_FIELD,
                                             TYPE_EVENT,
                                             DOMAINOBJECT,
                                             ID_OBJECT,
                                             ID_SPECIALIZED_OBJECT,
                                             DTA_EVENT,
                                             READ_NOTIFICATION,
                                             NOTES)
                                SELECT SYSTEM_ID,
                                        ID_EVENT,
                                        DESC_PRODUCER,
                                        ID_PEOPLE_RECEIVER,
                                        ID_GROUP_RECEIVER,
                                        TYPE_NOTIFY,
                                        DTA_NOTIFY,
                                        FIELD_1,
                                        FIELD_2,
                                        FIELD_3,
                                        FIELD_4,
                                        MULTIPLICITY,
                                        SPECIALIZED_FIELD,
                                        TYPE_EVENT,
                                        DOMAINOBJECT,
                                        ID_OBJECT,
                                        ID_SPECIALIZED_OBJECT,
                                        DTA_EVENT,
                                        READ_NOTIFICATION,
                                        NOTES
                                FROM DPA_NOTIFY
                                WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                AND ID_PEOPLE_RECEIVER IN
                                    (SELECT id_people FROM dpa_trasm_utente
                                     WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                     AND id_people != @idpeople
                                     AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                                 WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                                 AND a.id_people = @idpeople))
                                     AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                            -- elimino la notifica
                            DELETE FROM DPA_NOTIFY
                            WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                            AND ID_PEOPLE_RECEIVER IN
                                (SELECT id_people FROM dpa_trasm_utente
                                 WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                                 AND id_people != @idpeople
                                 AND EXISTS (SELECT 'x' FROM dpa_trasm_utente a
                                             WHERE     a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                                             AND a.id_people = @idpeople))
                                 AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

						   
                           SELECT   @error = @@ERROR
                           IF (@error <> 0)
                           begin
                              SET @resultvalue = 1
                              RETURN
                           end
--ovvero solo se io sono tra i notificati!!!
                        END


                  END
               end
            ELSE
-- se la ragione di trasmissione prevede workflow
               begin
                  IF (@iddelegato = 0)
                  BEGIN
                     update dpa_trasm_utente set cha_vista = '1',dta_vista =(CASE
                     WHEN dta_vista IS NULL
                     THEN GetDate()
                  ELSE dta_vista
                     END)  WHERE dpa_trasm_utente.dta_vista IS NULL and
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               ELSE
-- caso in cui si sta esercitando una delega
                  BEGIN
 
                     update dpa_trasm_utente set cha_vista_delegato = '1',id_people_delegato =
                     @iddelegato  WHERE
                     id_trasm_singola =(@CURSORTRASMSINGOLAFASCICOLO_system_id)
                     AND dpa_trasm_utente.id_people = @idpeople
                     SELECT   @error = @@ERROR
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END

                  BEGIN
-- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                     update dpa_trasm_utente set cha_in_todolist = '0'  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                     AND NOT dpa_trasm_utente.dta_vista IS NULL
                     AND (cha_accettata = '1' OR cha_rifiutata = '1')
                     AND dpa_trasm_utente.id_people = @idpeople
                     SELECT   @error = @@ERROR
                     IF (@error = 0)
                     begin
                        update dpa_todolist set dta_vista = GetDate()  WHERE id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND id_people_dest = @idpeople
                        AND id_project = @idoggetto
                        SELECT   @error = @@ERROR
                     end
					 
					INSERT INTO DPA_NOTIFY_HISTORY (ID_NOTIFY,
                                                     ID_EVENT,
                                                     DESC_PRODUCER,
                                                     ID_PEOPLE_RECEIVER,
                                                     ID_GROUP_RECEIVER,
                                                     TYPE_NOTIFY,
                                                     DTA_NOTIFY,
                                                     FIELD_1,
                                                     FIELD_2,
                                                     FIELD_3,
                                                     FIELD_4,
                                                     MULTIPLICITY,
                                                     SPECIALIZED_FIELD,
                                                     TYPE_EVENT,
                                                     DOMAINOBJECT,
                                                     ID_OBJECT,
                                                     ID_SPECIALIZED_OBJECT,
                                                     DTA_EVENT,
                                                     READ_NOTIFICATION)
                        SELECT SYSTEM_ID,
                               ID_EVENT,
                               DESC_PRODUCER,
                               ID_PEOPLE_RECEIVER,
                               ID_GROUP_RECEIVER,
                               TYPE_NOTIFY,
                               DTA_NOTIFY,
                               FIELD_1,
                               FIELD_2,
                               FIELD_3,
                               FIELD_4,
                               MULTIPLICITY,
                               SPECIALIZED_FIELD,
                               TYPE_EVENT,
                               DOMAINOBJECT,
                               ID_OBJECT,
                               ID_SPECIALIZED_OBJECT,
                               DTA_EVENT,
                               READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                        AND ID_PEOPLE_RECEIVER IN
                            (SELECT id_people FROM dpa_trasm_utente
                             WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                             AND dta_vista IS NOT NULL
                             AND (   cha_accettata = '1' OR cha_rifiutata = '1')
                             AND id_people = @idpeople)
                             AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                    -- elimino la ntoifica
                    DELETE FROM DPA_NOTIFY
                    WHERE     ID_SPECIALIZED_OBJECT = @CURSORTRASMSINGOLAFASCICOLO_system_id
                    AND ID_PEOPLE_RECEIVER IN
                        (SELECT id_people FROM dpa_trasm_utente
                         WHERE     id_trasm_singola = @CURSORTRASMSINGOLAFASCICOLO_system_id
                         AND dta_vista IS NOT NULL
                         AND (   cha_accettata = '1' OR cha_rifiutata = '1')
                         AND id_people = @idpeople)
                         AND (   ID_GROUP_RECEIVER = @idgruppo OR ID_GROUP_RECEIVER = 0)

                     -- imposto come letta la notifica per l'utente p_id_people
                    UPDATE dpa_notify
                    SET read_notification = '1'
                    WHERE     id_specialized_object = @CURSORTRASMSINGOLAFASCICOLO_system_id
                    AND id_people_receiver = @idpeople

					 
                     IF (@error <> 0)
                     begin
                        SET @resultvalue = 1
                        RETURN
                     end
                  END
               end

            END
            FETCH NEXT FROM @cursortrasmsingolafascicolo INTO @CURSORTRASMSINGOLAFASCICOLO_system_id,@CURSORTRASMSINGOLAFASCICOLO_cha_tipo_trasm,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_ragione,
            @CURSORTRASMSINGOLAFASCICOLO_cha_tipo_dest
         end
         CLOSE @cursortrasmsingolafascicolo
      end

   END
END 


GO


