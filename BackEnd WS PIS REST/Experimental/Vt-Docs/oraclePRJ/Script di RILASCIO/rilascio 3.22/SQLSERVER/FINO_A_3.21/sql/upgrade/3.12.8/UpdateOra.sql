/*
AUTORE:                      GABRIELE SERPI
Data creazione:                  09/06/2011
Scopo della modifica:         AGGIUNGERE LA COLONNA CHA_RICEVUTA_PEC  NEL DPA_EL_REGISTRI
                
*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_EL_REGISTRI';
      nomecolonna   VARCHAR2 (32)  := 'CHA_RICEVUTA_PEC';
      tipodato      VARCHAR2 (200) := 'VARCHAR2(2)';
      cnt           INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
            and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato;
         END IF;
      END IF;
   END;
END;
/

/*
AUTORE:                      P. De Luca
Data creazione:                  Luglio 2011
Scopo della modifica: Aggiungere la colonna "cha_infasato" per determinare
	se il record è stato già infasato o meno nella  DPA_CHIAVI_CONFIGURAZIONE

*/
BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'DPA_CHIAVI_CONFIG_TEMPLATE';
      nomecolonna   VARCHAR2 (32)  := 'CHA_INFASATO';
      tipodato      VARCHAR2 (200) := 'VARCHAR2(1)';
	  tipodefault   VARCHAR2 (200) := ' DEFAULT ''Y'' ';

	  cnt           INT;
      nomeutente        VARCHAR2 (200) := upper('@db_user');

   BEGIN
      SELECT COUNT (*)  INTO cnt FROM all_tables
       WHERE table_name = UPPER (nometabella) and owner=nomeutente;

      IF (cnt = 1)
      -- ok la tabella esiste
      THEN
         SELECT COUNT (*) INTO cnt FROM all_tab_columns
          WHERE table_name = UPPER (nometabella)
            AND column_name = UPPER (nomecolonna)
            and owner=nomeutente;

         IF (cnt = 0)
         THEN
            EXECUTE IMMEDIATE    'ALTER TABLE '|| nomeutente ||'.'|| nometabella
                              || ' ADD '|| nomecolonna
                              || ' '|| tipodato
							  ||tipodefault;
         END IF;
      END IF;
   END;
END;
/


CREATE OR REPLACE PROCEDURE @db_user.crea_keys_amministra
IS
   syscurramm   INT;
   syscurrkey   VARCHAR (32);
   cnt          INT;

   CURSOR curramm                    -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
   IS
      SELECT system_id
        FROM dpa_amministra;

   CURSOR currkey
     -- CURSORE CHE SCORRE LE CHIAVI della tabella DPA_CHIAVI_CONFIG_TEMPLATE
   IS
      SELECT var_codice
        FROM dpa_chiavi_config_template
        where cha_infasato = 'N';
BEGIN
   OPEN curramm;

   LOOP
      FETCH curramm
       INTO syscurramm;

      EXIT WHEN curramm%NOTFOUND;

      BEGIN
---cursore annidato x le chiavi di configurazione
         BEGIN
            OPEN currkey;

            LOOP
               FETCH currkey
                INTO syscurrkey;

               EXIT WHEN currkey%NOTFOUND;

               BEGIN
                  SELECT COUNT (*)
                    INTO cnt
                    FROM dpa_chiavi_configurazione
                   WHERE var_codice = syscurrkey AND id_amm = syscurramm;
               END;

               BEGIN
                  IF (cnt = 0)
                  THEN
                     INSERT INTO dpa_chiavi_configurazione
                                 (system_id, id_amm, var_codice,
                                  var_descrizione, var_valore,
                                  cha_tipo_chiave, cha_visibile,
                                  cha_modificabile, cha_globale)
                        (SELECT -- modifica di P. De Luca a seguito ticket ETT000000016485 di I. Giovacchini 
                        SEQ_DPA_CHIAVI_CONFIG.nextval,
                        -- (SELECT MAX (system_id) + 1 FROM dpa_chiavi_configurazione),
                                syscurramm AS id_amm, var_codice,
                                var_descrizione, var_valore, cha_tipo_chiave,
                                cha_visibile, cha_modificabile, '0'
                           FROM dpa_chiavi_config_template
                          WHERE var_codice = syscurrkey AND ROWNUM = 1);
                  END IF;
               END;
            END LOOP;

            CLOSE currkey;
         END;
--- fine cursore annidato per chiavi di configurazione
      END;
   END LOOP;

   CLOSE curramm;

   --COMMIT;
   update DPA_CHIAVI_CONFIG_TEMPLATE set cha_infasato = 'Y'; 
   
END;
/


BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='BE_SALVA_EMAIL_IN_LOCALE';
    IF (cnt = 0) THEN

-- il record serve per ....  <---- inserire commento
        insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_SALVA_EMAIL_IN_LOCALE','Abilita il salvatggio delle mail in locale ( 1 attivo 0 no)', '1','B','1','1','1');
    END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_VISUAL_DOC_SMISTAMENTO';
    IF (cnt = 0) THEN

  insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_VISUAL_DOC_SMISTAMENTO','Determina il default per la checkbox su Visualizza Documento su smistamento (1=checked, 0=not checked)', '1','F','1','1','1');
END IF;
    END;
END;
/

--dopo l'inserimento nella tabella dpa_chiavi_config_template,
-- occorre eseguire la procedura CREA_KEYS_AMMINISTRA per infasare in DPA_CHIAVI_CONFIGURAZIONE
-- quindi, verificare i record in dpa_chiavi_config_template, prima di eseguire la procedura
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='FE_FASC_RAPIDA_REQUIRED';
    IF (cnt = 0) THEN   
-- FE_FASC_RAPIDA_REQUIRED	FALSE
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_FASC_RAPIDA_REQUIRED'
   , 'Obbligatorieta della classificazione o fascicolazione rapida'
   , 'false', 'F', '1', '1', 'N');
END IF;

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_SESSION_REPOSITORY_DISABLED';
    IF (cnt = 0) THEN  
-- BE_SESSION_REPOSITORY_DISABLED	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_SESSION_REPOSITORY_DISABLED'
   , 'Disabilita (TRUE) o Abilita (FALSE) acquisisci file immagine prima di salva/protocolla'
   , 'false', 'B', '1', '1', 'N');
END IF;

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_ELIMINA_MAIL_ELABORATE';
    IF (cnt = 0) THEN  
-- BE_ELIMINA_MAIL_ELABORATE	0
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_ELIMINA_MAIL_ELABORATE'
   , 'Eliminazione automatica delle mail pervenute su casella PEC processate da PITRE'
   , '0', 'B', '1', '1', 'N');
END IF;

-- FE_TIPO_ATTO_REQUIRED	0
-- TIPO_ATTO_REQUIRED è stato sostituito da: getTipoDocObbl
--Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
--Values (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_TIPO_ATTO_REQUIRED', 'Obbligatorieta della tipologia documento', '0', 'F', '1', '1', 'N');

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='FE_PROTOIN_ACQ_DOC_OBBLIGATORIA';
    IF (cnt = 0) THEN  
-- FE_PROTOIN_ACQ_DOC_OBBLIGATORIA 	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_PROTOIN_ACQ_DOC_OBBLIGATORIA'
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1', 'N');
END IF;

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='FE_SMISTA_ABILITA_TRASM_RAPIDA';
    IF (cnt = 0) THEN  
--FE_SMISTA_ABILITA_TRASM_RAPIDA	1
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_SMISTA_ABILITA_TRASM_RAPIDA'
   , 'Trasmissione rapida obligatoria  sulla protocollazione semplificata e sullo smistamento'
   , '1', 'F', '1', '1', 'N');
END IF;

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='FE_ENABLE_PROT_RAPIDA_NO_UO';
    IF (cnt = 0) THEN  
-- FE_ENABLE_PROT_RAPIDA_NO_UO	true
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_ENABLE_PROT_RAPIDA_NO_UO'
   , 'Trasmissione obbligatoria sulla protocollazione semplificata'
   , 'true', 'F', '1', '1', 'N');
END IF;

 SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_ELIMINA_RICEVUTE_PEC';
    IF (cnt = 0) THEN  
-- BE_ELIMINA_RICEVUTE_PEC	0
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'BE_ELIMINA_RICEVUTE_PEC'
   , 'Eliminazione automatica delle RICEVUTE PEC pervenute su casella PEC processate da PITRE '
   , '0', 'B', '1', '1', 'N');
END IF;
end;
end;
/

begin
crea_keys_amministra;
end;
/





begin
Insert into @db_user.DPA_DOCSPA    (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values          (seq.nextval, sysdate, '3.12.8');
end;
/
