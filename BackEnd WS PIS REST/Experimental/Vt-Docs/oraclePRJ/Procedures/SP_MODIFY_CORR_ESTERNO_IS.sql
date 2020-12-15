--------------------------------------------------------
--  DDL for Procedure SP_MODIFY_CORR_ESTERNO_IS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_MODIFY_CORR_ESTERNO_IS" (
/*
versione della SP ad hoc per "Interoperabilit semplificata", by S. Furnari:
oltre a introdurre e gestire il nuovo parametro SimpInteropUrl , 
recepisce le modifiche introdotte in questa stessa versione 3.21 by C. Ferlito 
per gestire il nuovo campo var_desc_corr_old
*/
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
   var_iddoctype     IN       NUMBER,
   inrubricacomune   IN       CHAR,
   tipourp           IN       CHAR,
   localita             IN          VARCHAR2,
   luogoNascita      IN       VARCHAR2,
   dataNascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
-- aggiunto questo parametro e la gestione relativa rispetto alla vecchia versione
   SimpInteropUrl    IN       VARCHAR2,
   newid             OUT      NUMBER,
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cnt   integer;
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1):= '0';
      cha_tipourp             CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chaTipoIE                CHAR (1);
      numLivello               NUMBER          := 0;
      idParent                 NUMBER;
      idPesoOrg                NUMBER;
      idUO                     NUMBER;
      idGruppo                 NUMBER;
      idTipoRuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
      chapa                    CHAR (1);
      var_desc_old             VARCHAR2(256);
       url                    varchar2(4000);
   BEGIN

      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm,cha_pa, cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo,var_desc_corr_old, InteropUrl
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE, numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppo,var_desc_old         , url
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
          dbms_output.put_line('select effettuata') ;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
         dbms_output.put_line('primo blocco eccezione') ;
            outvalue := 0;
            RETURN;
      END reperimento_dati;


    if(tipourp is not null and cha_tipourp is not null and cha_tipourp!=tipourp) then
        cha_tipourp := tipourp;

    end if;


      <<dati_canale_utente>>
      if cha_tipourp = 'P'
      THEN
        BEGIN
         SELECT id_documenttype
           INTO v_id_doctype
           FROM dpa_t_canale_corr
          WHERE id_corr_globale = idcorrglobale;
        EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            dbms_output.put_line('2do blocco eccezione') ; 
            outvalue := 2;
        END dati_canale_utente;
      end if;

      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */

--VERIFICO se il corrisp ?? stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;

-- 1) non ?? stato mai utilizzato come corrisp in un protocollo
         IF /* 2 */ (myprofile = 0)
         THEN
            BEGIN
               UPDATE dpa_corr_globali
                  SET var_codice_aoo = codice_aoo,
                      var_codice_amm = codice_amm,
                      var_email = email,
                      var_desc_corr = desc_corr,
                      var_nome = nome,
                      var_cognome = cognome,
                      cha_pa=chapa,
                      cha_tipo_urp=cha_tipourp,
                      InteropUrl = SimpInteropUrl
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
               dbms_output.put_line('3o blocco eccezione') ;
                  outvalue := 3;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'     OR cha_tipourp = 'F'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
                      
               
               
                     SELECT count(*) into cnt
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale ; 
                      
                      IF (cnt = 0)
                      THEN 
                       dbms_output.put_line('sono nella INSERT,id_corr_globali =  '||idcorrglobale) ; 
                      INSERT INTO dpa_dett_globali (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                          VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);
                           END IF;
                           
                        IF (cnt = 1)
                      THEN    
                        dbms_output.put_line('sono nella UPDATE') ; 
                     UPDATE dpa_dett_globali SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                          WHERE (id_corr_globali = idcorrglobale) ; 
                                              END IF;
                                               
                      /*
                      
                      MERGE INTO dpa_dett_globali
                        USING (
                          SELECT system_id as id_interno
                          FROM dpa_dett_globali
                          WHERE id_corr_globali = idcorrglobale) select_interna
                        ON (system_id = select_interna.id_interno)
                        WHEN MATCHED THEN
                          UPDATE SET
                             var_indirizzo = indirizzo,
                             var_cap = cap,
                             var_provincia = provincia,
                             var_nazione = nazione,
                             var_cod_fiscale = cod_fiscale,
                             var_telefono = telefono,
                             var_telefono2 = telefono2,
                             var_note = note,
                             var_citta = citta,
                             var_fax = fax,
                             var_localita = localita,
                          var_luogo_nascita = luogoNascita,
                         dta_nascita = dataNascita,
                         var_titolo = titolo
                         WHERE (id_corr_globali = idcorrglobale) 
                        WHEN NOT MATCHED THEN
                       INSERT (
                          system_id,
                          id_corr_globali, 
                          var_indirizzo ,
                                 var_cap ,
                                 var_provincia ,
                                 var_nazione ,
                                 var_cod_fiscale ,
                                 var_telefono ,
                                 var_telefono2 ,
                                 var_note ,
                                 var_citta ,
                                 var_fax ,
                                 var_localita,
                                  var_luogo_nascita,
                                 dta_nascita,
                                 var_titolo)
                         VALUES (
                          seq.nextval,
                          idcorrglobale,
                          indirizzo,
                                 cap,
                                 provincia,
                                 nazione,
                                 cod_fiscale,
                                 telefono,
                                 telefono2,
                                 note,
                                 citta,
                                 fax,
                                 localita,
                                 luogoNascita,
                                 dataNascita,
                                 titolo);

                          
                        */
                         commit; 
                                  dbms_output.put_line('sono nella merge') ; 
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('4o blocco eccezione'||SQLERRM) ; 
                     outvalue := 4;
                     RETURN;
               END update_dpa_dett_globali;
            END IF;                                                    /* 3 */

--METTI QUI UPDATE SU DPA_T_CANALE_CORR
--AGGIORNO LA DPA_T_CANALE_CORR
               BEGIN
                  UPDATE dpa_t_canale_corr
                     SET id_documenttype = var_iddoctype
                   WHERE id_corr_globale = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('5o blocco eccezione') ; 
                     outvalue := 5;
                     RETURN;
               END;
         ELSE
-- caso 2) Il corrisp ?? stato utilizzato come corrisp in un protocollo
-- NUOVO CODICE RUBRICA
            new_var_cod_rubrica :=
                                cod_rubrica || '_' || TO_CHAR (idcorrglobale);

            <<storicizzazione_corrisp>>
            BEGIN
               UPDATE dpa_corr_globali
                  SET dta_fine = SYSDATE,
                     var_cod_rubrica = new_var_cod_rubrica,
                      var_codice = new_var_cod_rubrica,
                      id_parent = NULL
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                 dbms_output.put_line('6o blocco eccezione') ; 
                  outvalue := 6;
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
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo,var_desc_corr_old     , InteropUrl
                           )
                    VALUES (newid, numLivello, chaTipoIE, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idParent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm, chapa, idPesoOrg,
                            idGruppo, idTipoRuolo, idUO, var_desc_old , SimpInteropUrl
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  dbms_output.put_line('7o blocco eccezione') ; 
                  outvalue := 7;
                  RETURN;
            END inserimento_nuovo_corrisp;

 
/* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
E UNITA' ORGANIZZATIVE */
            IF /* 4 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
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
                               var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax, localita, luogoNascita, dataNascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('8o blocco eccezione') ; 
                     outvalue := 8;
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
                  dbms_output.put_line('9o blocco eccezione') ; 
                  outvalue := 9;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;   
         
         
--se fa parte di una lista, allora la devo aggiornare.
if newid IS NOT NULL 
THEN 
    update dpa_liste_distr d set d.ID_DPA_CORR=newid where d.ID_DPA_CORR=idcorrglobale;
end if;

         
         
                                                             /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;

/
