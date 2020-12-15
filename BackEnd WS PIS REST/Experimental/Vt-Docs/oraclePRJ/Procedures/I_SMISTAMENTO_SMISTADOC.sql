--------------------------------------------------------
--  DDL for Procedure I_SMISTAMENTO_SMISTADOC
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."I_SMISTAMENTO_SMISTADOC" (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idpeopledestinatario           IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-- -5: Errore in SPsetDataVistaSmistamento
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   identitytrasmsing   NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   resultvalue         NUMBER;   
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;
 
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasmsing
        FROM DUAL;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO INFOTN_COLL.dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO INFOTN_COLL.dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing, cha_tipo_trasm,
                   dta_scadenza, id_trasm_utente
                  )
           VALUES (identitytrasmsing, idragionetrasm, identitytrasm, 'U',
                   idcorrglobaledestinatario, noteindividuali, 'S',
                   datascadenza, NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;
 
   BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO INFOTN_COLL.dpa_trasm_utente
                  (system_id, id_trasm_singola, id_people,
                   dta_vista, dta_accettata, dta_rifiutata, dta_risposta,
                   cha_vista, cha_accettata, cha_rifiutata, var_note_acc,
                   var_note_rif, cha_valida, id_trasm_risp_sing
                  )
           VALUES (seq.NEXTVAL, identitytrasmsing, idpeopledestinatario,
                   NULL, NULL, NULL, NULL,
                   '0', '0', '0', NULL,
                   NULL, '1', NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;
 
   BEGIN
--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
      UPDATE dpa_trasmissione
         SET dta_invio = SYSDATE ()
       WHERE system_id = identitytrasm;
 
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento
                 AND personorgroup = idpeopledestinatario)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;
 
   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;
 
      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET  accessrights = rights,
                    cha_tipo_diritto = 'T'
             WHERE thing = iddocumento
               AND personorgroup = idpeopledestinatario
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights,
                      id_gruppo_trasm, cha_tipo_diritto
                     )
              VALUES (iddocumento, idpeopledestinatario, rights,
                      idgruppomittente, tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;
 
/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;            
        
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;
 
            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta gia accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',                        
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.thing=iddocumento and s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );
 
         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;
 
/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;
 
   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
 
   returnvalue := 0;
END; 

/
