
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin
-- MEV Fatturazione elettronica by Panici - Iazzetta
	declare cnt int;
        begin
        select count(*) into cnt from user_tables
				where table_name='DPA_DATI_FATTURAZIONE';

        if (cnt = 0) then
          execute immediate '
          CREATE TABLE DPA_DATI_FATTURAZIONE	(
          	SYSTEM_ID INT NOT NULL,
			CODICE_AMM VARCHAR2(128 BYTE) NOT NULL,
          	CODICE_AOO VARCHAR2(20 BYTE)  NOT NULL,
          	CODICE_UO  VARCHAR2(128 BYTE) NOT NULL,
          	CODICE_UAC VARCHAR2(128 BYTE),
          	CODICE_CLASSIFICAZIONE   VARCHAR2(20 BYTE),
          	VAR_UTENTE_PROPRIETARIO  VARCHAR2(128 BYTE),
          	VAR_TIPOLOGIA_DOCUMENTO  VARCHAR2(128 BYTE),
          	VAR_RAGIONE_TRASMISSIONE VARCHAR2(128 BYTE),
			ISTANZA_PITRE VARCHAR2(512 BYTE),
			MODELLO_TRASMISSIONE VARCHAR2(128 BYTE)
          	,  PRIMARY KEY (SYSTEM_ID)
                                             	)' ;
        end if;
        END;
END;
/

