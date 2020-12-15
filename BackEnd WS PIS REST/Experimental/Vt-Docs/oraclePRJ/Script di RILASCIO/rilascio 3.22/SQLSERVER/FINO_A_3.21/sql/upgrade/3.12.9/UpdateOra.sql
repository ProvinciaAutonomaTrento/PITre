/*
Palumbo Ch
 lunedì 30 maggio 2011 15:38
 funzionalità di smistamento: in test filtrava le ragioni
 e quindi nella funzionalità di smistamento avevamo meno trasmissioni rispetto alla TDL
*/
BEGIN
   UPDATE @db_user.dpa_ragione_trasm
      SET cha_cede_diritti = 'N'
    WHERE cha_cede_diritti IS NULL;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line ('errore sql: ' || SQLERRM);
      RAISE;
END;
/

/*
AUTORE:						Frezza S.
Data creazione:				01/07/2011
Scopo dell'inserimento:
'Abilita il pulsante della firma di un documento acquisito
o presente sul file-system tramite algoritmo SHA256'
*/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI f
	where f.COD_FUNZIONE ='DO_DOC_FIRMA_256';
    if (cnt = 0) then
       INSERT INTO @db_user.DPA_ANAGRAFICA_FUNZIONI
(
    COD_FUNZIONE,
    VAR_DESC_FUNZIONE,
    CHA_TIPO_FUNZ,
    DISABLED
)
VALUES
(
    'DO_DOC_FIRMA_256',
    'Abilita il pulsante della firma di un documento acquisito o presente sul file-system tramite algoritmo SHA256',
    NULL,
    'N'
)              ;

    end if;
    end;
end;
/


/*
AUTORE:						Veltri F.
Data creazione:				23/06/2011
Scopo dell'inserimento:
Il tasto rimuovi funziona soltanto con i documenti grigi
ricevuti per interoperabilità e ancora non protocollati.
L’icona rimuovi è visibile soltanto per i documenti interessati e non gli altri
*/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI f
	where f.COD_FUNZIONE ='DO_TODOLIST_RIMUOVI';
    if (cnt = 0) then
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI
	   VALUES('DO_TODOLIST_RIMUOVI'
	   , 'Abilita il pulsante di rimozione documento dalla todolist nel caso di documenti ricevuti per interoperabilità', '', 'N');
    end if;
    end;
end;
/



CREATE OR REPLACE function @db_user.gettestoultimanota
-- =============================================
-- Author:        P. De Luca
-- Create date:  15 giu 2011
-- Description:    ritorna il testo della nota più recente associata al documento o al fascicolo
-- =============================================
(
p_TIPOOGGETTOASSOCIATO varchar
, p_IDOGGETTOASSOCIATO int
, p_ID_RUOLO_IN_UO int
, p_IDUTENTECREATORE int
, p_IDRUOLOCREATORE int
)
RETURN varchar IS ultimanota varchar(2000);

BEGIN

IF (p_TIPOOGGETTOASSOCIATO  <> 'F' AND p_TIPOOGGETTOASSOCIATO  <> 'D') THEN
            ultimanota := '-1';
    RETURN ultimanota ;

END IF;


IF p_TIPOOGGETTOASSOCIATO = 'F' THEN
SELECT TESTO into ultimanota
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN PROJECT PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID
      WHERE
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in
      (select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ;
END IF;

IF p_TIPOOGGETTOASSOCIATO = 'D' THEN  --join con la profile invece della project
SELECT TESTO into ultimanota
      FROM
      (
      SELECT /*+ FIRST_ROWS(1) */
      -- HINT PRECEDENTE serve per ottimizzare tempi risposta, da oracle 10 in poi
      -- N.SYSTEM_ID,
      nvl(N.TESTO,'null') as testo
     FROM    DPA_NOTE N
      LEFT JOIN People P ON N.IDUTENTECREATORE = P.SYSTEM_ID
      LEFT JOIN Groups G ON N.IDRUOLOCREATORE = G.SYSTEM_ID
      LEFT JOIN profile PR  ON N.IDOGGETTOASSOCIATO =  PR.SYSTEM_ID
      WHERE
      N.TIPOOGGETTOASSOCIATO = p_TIPOOGGETTOASSOCIATO AND
      N.IDOGGETTOASSOCIATO = p_IDOGGETTOASSOCIATO AND
      (N.TIPOVISIBILITA = 'T' OR
      (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in
      (select id_registro from dpa_l_ruolo_reg rr where  rr.ID_RUOLO_IN_UO = p_ID_RUOLO_IN_UO)) OR
      (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = p_IDUTENTECREATORE) OR
      (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = p_IDRUOLOCREATORE))
      ORDER BY N.DATACREAZIONE DESC
      )
      WHERE   ROWNUM = 1 ;

END IF;
return ultimanota;


EXCEPTION
when no_data_found  then
ultimanota := '';
 return ultimanota;

when others  then
ultimanota := '-1';
 return ultimanota;

 END;
/


CREATE OR REPLACE PROCEDURE @db_user.sp_modify_corr_esterno (
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
   localita          IN       VARCHAR2,
   luogonascita      IN       VARCHAR2,
   datanascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
   newid             OUT      NUMBER,
   returnvalue       OUT      NUMBER
)
IS
BEGIN
   DECLARE
      cnt                      INTEGER;
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      cha_dettaglio            CHAR (1)       := '0';
      cha_tipourp              CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chatipoie                CHAR (1);
      numlivello               NUMBER         := 0;
      idparent                 NUMBER;
      idpesoorg                NUMBER;
      iduo                     NUMBER;
      idgruppo                 NUMBER;
      idtiporuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
      chapa                    CHAR (1);
   BEGIN

      <<reperimento_dati>>
      BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm, cha_pa,
                cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo,
                id_tipo_ruolo, id_gruppo
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa,
                chatipoie, numlivello, idparent, idpesoorg, iduo,
                idtiporuolo, idgruppo
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;

         DBMS_OUTPUT.put_line ('select effettuata');
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            DBMS_OUTPUT.put_line ('primo blocco eccezione');
            outvalue := 0;
            RETURN;
      END reperimento_dati;

      IF (    tipourp IS NOT NULL
          AND cha_tipourp IS NOT NULL
          AND cha_tipourp != tipourp
         )
      THEN
         cha_tipourp := tipourp;
      END IF;

      <<dati_canale_utente>>
      IF cha_tipourp = 'P'
      THEN
         BEGIN
            SELECT id_documenttype
              INTO v_id_doctype
              FROM dpa_t_canale_corr
             WHERE id_corr_globale = idcorrglobale;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               DBMS_OUTPUT.put_line ('2do blocco eccezione');
               outvalue := 2;
         END dati_canale_utente;
      END IF;

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
                      cha_pa = chapa,
                      cha_tipo_urp = cha_tipourp
                WHERE system_id = idcorrglobale;
            EXCEPTION
               WHEN OTHERS
               THEN
                  DBMS_OUTPUT.put_line ('3o blocco eccezione');
                  outvalue := 3;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI ?? ANDTATA A BUON FINE
PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI
*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
                  SELECT COUNT (*)
                    INTO cnt
                    FROM dpa_dett_globali
                   WHERE id_corr_globali = idcorrglobale;

                  IF (cnt = 0)
                  THEN
                     DBMS_OUTPUT.put_line
                                  (   'sono nella INSERT,id_corr_globali =  '
                                   || idcorrglobale
                                  );

                     INSERT INTO dpa_dett_globali
                                 (system_id, id_corr_globali, var_indirizzo,
                                  var_cap, var_provincia, var_nazione,
                                  var_cod_fiscale, var_telefono,
                                  var_telefono2, var_note, var_citta,
                                  var_fax, var_localita, var_luogo_nascita,
                                  dta_nascita, var_titolo
                                 )
                          VALUES (seq.NEXTVAL, idcorrglobale, indirizzo,
                                  cap, provincia, nazione,
                                  cod_fiscale, telefono,
                                  telefono2, note, citta,
                                  fax, localita, luogonascita,
                                  datanascita, titolo
                                 );
                  END IF;

                  IF (cnt = 1)
                  THEN
                     DBMS_OUTPUT.put_line ('sono nella UPDATE');

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
                            var_fax = fax,
                            var_localita = localita,
                            var_luogo_nascita = luogonascita,
                            dta_nascita = datanascita,
                            var_titolo = titolo
                      WHERE (id_corr_globali = idcorrglobale);
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
                  COMMIT;
                  DBMS_OUTPUT.put_line ('sono nella merge');
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     DBMS_OUTPUT.put_line ('4o blocco eccezione' || SQLERRM);
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
                  DBMS_OUTPUT.put_line ('5o blocco eccezione');
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
                  DBMS_OUTPUT.put_line ('6o blocco eccezione');
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
                            cha_tipo_corr, cha_tipo_urp, var_codice_aoo,
                            var_cod_rubrica, cha_dettagli, var_email,
                            var_codice_amm, cha_pa, id_peso_org, id_gruppo,
                            id_tipo_ruolo, id_uo
                           )
                    VALUES (newid, numlivello, chatipoie, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idparent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp, codice_aoo,
                            cod_rubrica, cha_dettaglio, email,
                            codice_amm, chapa, idpesoorg, idgruppo,
                            idtiporuolo, iduo
                           );
            EXCEPTION
               WHEN OTHERS
               THEN
                  DBMS_OUTPUT.put_line ('7o blocco eccezione');
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
                               var_note, var_citta, var_fax, var_localita,
                               var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, telefono, telefono2,
                               note, citta, fax, localita,
                               luogonascita, datanascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     DBMS_OUTPUT.put_line ('8o blocco eccezione');
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
                  DBMS_OUTPUT.put_line ('9o blocco eccezione');
                  outvalue := 9;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;

--se fa parte di una lista, allora la devo aggiornare.
         IF newid IS NOT NULL
         THEN
            UPDATE dpa_liste_distr d
               SET d.id_dpa_corr = newid
             WHERE d.id_dpa_corr = idcorrglobale;
         END IF;
      /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;
/


BEGIN
   DECLARE
    indiceesiste EXCEPTION;
      PRAGMA EXCEPTION_INIT(indiceesiste, -1408);
	
      cnt       INT;
	  
	  nomeutente	VARCHAR2 (200) := upper('@db_user');
	  istruzioneSQL VARCHAR2(2000) ; 
    
   BEGIN
     istruzioneSQL  := 'CREATE INDEX '||nomeutente||'.INDX_MODELLI_MITT_DEST_1 
	 ON DPA_MODELLI_MITT_DEST (ID_MODELLO) ' ; 
    
	 SELECT COUNT (*) INTO cnt
        FROM all_indexes
       WHERE index_name = upper('INDX_MODELLI_MITT_DEST_1')
       and table_name=upper('DPA_MODELLI_MITT_DEST')
	   and owner=nomeutente;

      IF (cnt = 0)
      THEN
            EXECUTE IMMEDIATE istruzioneSQL;
      END IF;

    EXCEPTION 
WHEN indiceesiste
THEN DBMS_OUTPUT.PUT_LINE('esiste già un indice per questa lista di colonne'); 

   END;
END;
/


CREATE OR REPLACE FUNCTION @db_user.getifmodelloautorizzato (
   id_ruolo          NUMBER,
   id_people         NUMBER,
   system_id         NUMBER,
   id_modellotrasm   NUMBER,
   accesssrigth      NUMBER
)
   RETURN NUMBER
IS
   retval         NUMBER;
--accesssRigth number;
   idragione      NUMBER;
   tipo_diritto   CHAR;
   
   hide_versions INT := NULL;
   isDocument INT := NULL;
   consolidationState INT := 0;
   idObject NUMBER(10) := NULL;

   CURSOR cur
   IS
      SELECT DISTINCT id_ragione
                 FROM dpa_modelli_mitt_dest
                WHERE id_modello = id_modellotrasm
                  AND cha_tipo_mitt_dest <> 'M';
BEGIN
   retval := 1;
   
   -- Verifica se il modello trasmissione includa almeno una trasmissione singola
   -- con modalità nascondi versioni precedenti
   select count(*) into hide_versions 
   from dpa_modelli_mitt_dest 
   where id_modello = id_modellotrasm and hide_doc_versions = '1'; 
   
   if (not hide_versions is null and hide_versions > 0) then
       idObject := system_id;
       
       -- verifica se l'id fornito si riferisce ad un documento o fascicolo
       select count(*) into isDocument
       from profile p
       where p.system_id = idObject;
           
       if (isDocument > 0) then
        -- L'istanza su cui si sta applicando il modello è un documento,
        -- verifica se sia consolidato
            
            select p.consolidation_state into consolidationState
            from profile p
            where p.system_id = idObject;
                
            if (consolidationState is null or consolidationState = 0) then
                -- Il modello prevede di nascondere le versioni di un documento precedenti a quella corrente
                -- al destinatario della trasmissione, ma in tal caso il documento non è stato ancora consolidato,
                -- pertanto il modello non può essere utilizzato
                retval := 0;
            end if;
       end if;

   end if;

    if (retval = 1) then
    --accesssRigth:= getaccessrights(id_ruolo, id_people, system_id);
       IF (accesssrigth = 45)
       THEN
          BEGIN
             OPEN cur;

             LOOP
                FETCH cur
                 INTO idragione;

                EXIT WHEN cur%NOTFOUND;

                SELECT cha_tipo_diritti
                  INTO tipo_diritto
                  FROM dpa_ragione_trasm
                 WHERE system_id = idragione;

                IF (tipo_diritto <> 'R' AND tipo_diritto <> 'C')
                THEN
                   BEGIN
                      retval := 0;
                   END;
                END IF;

                EXIT WHEN retval = 0;
             END LOOP;

             CLOSE cur;
          END;
       END IF;
    end if;       

   RETURN retval;
END getifmodelloautorizzato;
/

CREATE OR REPLACE FUNCTION getcontatoredocordinamento (docnumber INT, tipocontatore CHAR)
   RETURN INT
IS
   risultato   VARCHAR (255);
BEGIN
   SELECT valore_oggetto_db
     INTO risultato
     FROM dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
    WHERE dpa_associazione_templates.doc_number = TO_CHAR (docnumber)
      AND dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
      AND dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
      AND dpa_tipo_oggetto.descrizione = 'Contatore'
      AND dpa_oggetti_custom.da_visualizzare_ricerca = '1';

   RETURN TO_NUMBER (risultato);
END getcontatoredocordinamento;
/




begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
Values
   (seq.nextval, sysdate, '3.12.9');
end;
/