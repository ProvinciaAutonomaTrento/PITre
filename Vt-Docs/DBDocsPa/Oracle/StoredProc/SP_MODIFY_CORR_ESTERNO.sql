CREATE OR REPLACE PROCEDURE INFOTN_COLL.sp_modify_corr_esterno (
   idcorrglobale     IN       NUMBER,
   desc_corr         IN       VARCHAR2,
   nome              IN       VARCHAR2,
   cognome           IN       VARCHAR2,
   codice_aoo        IN       VARCHAR2,
   codice_amm        IN       VARCHAR2,
   email             IN       VARCHAR2,
   indirizzo         IN       VARCHAR2,
   cap               IN       VARCHAR2,
   provincia         IN       VARCHAR2,
   nazione           IN       VARCHAR2,
   citta             IN       VARCHAR2,
   cod_fiscale       IN       VARCHAR2,
   telefono          IN       VARCHAR2,
   telefono2         IN       VARCHAR2,
   note              IN       VARCHAR2,
   fax               IN       VARCHAR2,
   var_iddoctype              NUMBER,
   inrubricacomune   IN       CHAR,
   newid             OUT      NUMBER,
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1)       := '0';
      cha_tipo_urp             CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      cha_tipo_corr            CHAR (1);
   BEGIN

      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm
           INTO cod_rubrica, cha_tipo_urp, id_reg, idamm
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            outvalue := 0;
            RETURN;
      END reperimento_dati;

      <<dati_canale_utente>>
      BEGIN
         SELECT id_documenttype
           INTO v_id_doctype
           FROM dpa_t_canale_corr
          WHERE id_corr_globale = idcorrglobale;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            outvalue := 0;
      END dati_canale_utente;

      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipo_urp = 'U' OR cha_tipo_urp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */

--VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;

-- 1) non è stato mai utilizzato come corrisp in un protocollo
         IF /* 2 */ (myprofile = 0)
         THEN
            BEGIN
               UPDATE dpa_corr_globali
                  SET var_codice_aoo = codice_aoo,
                      var_codice_amm = codice_amm,
                      var_email = email,
                      var_desc_corr = desc_corr,
                      var_nome = nome,
                      var_cognome = cognome
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                  outvalue := 0;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipo_urp = 'U' OR cha_tipo_urp = 'P'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
                  UPDATE dpa_dett_globali
                     SET var_indirizzo = indirizzo,
                         var_cap = cap,
                         var_provincia = provincia,
                         var_nazione = nazione,
                         var_cod_fiscale = cod_fiscale,
                         var_telefono = telefono,
                         var_telefono2 = telefono2,
                         var_note = note,
                         var_citta = citta,
                         var_fax = fax
                   WHERE id_corr_globali = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     outvalue := 0;
                     RETURN;
               END update_dpa_dett_globali;
            END IF;                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
            IF /* 5 */ var_iddoctype != v_id_doctype
            THEN
-- SE ENTRO QUI VUOL DIRE CHE IL TIPO_CANALE DEL CORRISP
--ESTERNO è STATO CAMBIATO, QUINDI AGGIORNO LA DPA_T_CANALE_CORR
               BEGIN
                  UPDATE dpa_t_canale_corr
                     SET id_documenttype = var_iddoctype
                   WHERE id_corr_globale = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     outvalue := 0;
                     RETURN;
               END;
            END IF;                                                    /* 5 */
         ELSE
-- caso 2) Il corrisp è stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
            new_var_cod_rubrica :=
                                cod_rubrica || '_' || TO_CHAR (idcorrglobale);

            <<storicizzazione_corrisp>>
            BEGIN
               UPDATE dpa_corr_globali
                  SET dta_fine = SYSDATE (),
                      var_cod_rubrica = new_var_cod_rubrica,
                      var_codice = new_var_cod_rubrica
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                  outvalue := 0;
                  RETURN;
            END storicizzazione_corrisp;

            SELECT seq.NEXTVAL
              INTO newid
              FROM DUAL;

/* DOPO LA STORICIZZAZIONE DEL VECCHIO CORRISPONDENTE POSSO
INSERIRE IL NUOVO CORRISPONDENTE NELLA DPA_CORR_GLOBALI */
            <<inserimento_nuovo_corrisp>>
            BEGIN
               IF (inrubricacomune = '1')
               THEN
                  cha_tipo_corr := 'C';
               ELSE
                  cha_tipo_corr := 'S';
               END IF;

               INSERT INTO dpa_corr_globali
                           (system_id, num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm
                           )
                    VALUES (newid, '0', 'E', id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE (), '0', cod_rubrica,
                            cha_tipo_corr, cha_tipo_urp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  outvalue := 0;
                  RETURN;
            END inserimento_nuovo_corrisp;

/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
            IF /* 4 */ cha_tipo_urp = 'U' OR cha_tipo_urp = 'P'
            THEN
--PRENDO LA SYSTEM_ID APPENA INSERITA
               SELECT seq.NEXTVAL
                 INTO identitydettglobali
                 FROM DUAL;

               <<inserimento_dettaglio_corrisp>>
               BEGIN
                  INSERT INTO dpa_dett_globali
                              (system_id, id_corr_globali, var_indirizzo,
                               var_cap, var_provincia, var_nazione,
                               var_cod_fiscale, var_telefono, var_telefono2,
                               var_note, var_citta, var_fax
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     outvalue := 0;
                     RETURN;
               END inserimento_dettaglio_corrisp;
            END IF;                                                    /* 4 */

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
            <<inserimento_dpa_t_canale_corr>>
            BEGIN
               SELECT seq.NEXTVAL
                 INTO identitydpatcanalecorr
                 FROM DUAL;

               INSERT INTO dpa_t_canale_corr
                           (system_id, id_corr_globale, id_documenttype,
                            cha_preferito
                           )
                    VALUES (identitydpatcanalecorr, newid, var_iddoctype,
                            '1'
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  outvalue := 0;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;                                                       /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;
/