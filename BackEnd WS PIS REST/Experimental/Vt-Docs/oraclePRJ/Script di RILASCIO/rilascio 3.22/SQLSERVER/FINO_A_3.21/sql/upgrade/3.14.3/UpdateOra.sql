-- ALTER TABLE VERSIONS ADD CHA_SEGNATURA VARCHAR(1) NULL ;
-- Autore richiesta: C. Ferlito -- per MEV INPS 109 per aggiungere il campo  cha_segnatura che tiene traccia del documento associato alla  versione(se contiene segnatura permanente o meno)

CREATE OR REPLACE FUNCTION GetNumMittentiModelliTrasm (
   -- Id del template da analizzare
   templateid   NUMBER
)
    -- Numero dei mittenti del modello
   RETURN NUMBER
IS

   retval   NUMBER;
/******************************************************************************

    AUTHOR:    Samuele Furnari

   NAME:       GetNumMittentiModelliTrasm

   PURPOSE:

        Funzione per contare il numero di mittenti di un modello 
        di trasmissione

******************************************************************************/
BEGIN
    retval := 0;

    -- Conteggio dei mittenti del modello con system_id pari a quello passato
    -- per parametro
    SELECT count('x') INTO retval
    FROM dpa_modelli_trasm mt
        INNER JOIN dpa_modelli_mitt_dest md
            ON mt.SYSTEM_ID = md.ID_MODELLO
    WHERE mt.system_id = templateId
            AND md.CHA_TIPO_MITT_DEST = 'M';

   RETURN retval;
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN -1;
END GetNumMittentiModelliTrasm;
/



BEGIN
   DECLARE
      nometabella   VARCHAR2 (32)  := 'VERSIONS';
      nomecolonna   VARCHAR2 (32)  := 'CHA_SEGNATURA';
      tipodato      VARCHAR2 (200) := ' VARCHAR(1) ';
	  cnt           INT;

	nomeutente		VARCHAR2 (200) := upper('@db_user');

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


-- FE_CHECK_TITOLARIO_ATTIVO
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_CHECK_TITOLARIO_ATTIVO';
    IF (cnt = 0) THEN       
  
insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
   ,CHA_MODIFICABILE,CHA_GLOBALE)
         values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_CHECK_TITOLARIO_ATTIVO'
  ,'Chiave per l''attivazione di default del  titolario attivo nelle maschere di ricerca fascicoli'
  ,'0','F','1'
  ,'1','1');
  
    END IF;
    END;
END;
/

--FE_MULTI_STAMPA_ETICHETTA	
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_MULTI_STAMPA_ETICHETTA';
    IF (cnt = 0) THEN       
	insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
	   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
	   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
	   ,CHA_MODIFICABILE,CHA_GLOBALE)
			 values 
	  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'FE_MULTI_STAMPA_ETICHETTA'
	  ,'Chiave di attivazione della stampa multipla delle etichette nei protocolli in entrata/uscita'
	  ,'0','F','1'
	  ,'1','1');
  
    END IF;
    END;
END;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE
    WHERE VAR_CODICE='FE_PERMANENT_DISPLAYS_SEGNATURE';
    IF (cnt = 0) THEN       

-- il record serve per abilitare la segnatura permanente sui documenti acquisiti in formato pdf nei protocolli
INSERT INTO @db_user.DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM
   ,VAR_CODICE
   ,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE
   ,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
VALUES 
	(SEQ_DPA_CHIAVI_CONFIG.nextval,0,
           'FE_PERMANENT_DISPLAYS_SEGNATURE',
           'Abilita la segnatura permanente sui documenti acquisiti in formato pdf nei protocolli',
           '0',        'F',
           '1',		   '1',           '1' ) ;
    END IF;
    END;
END;
/



begin

Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
Values    (seq.nextval, sysdate, '3.14.3');

end;
/
