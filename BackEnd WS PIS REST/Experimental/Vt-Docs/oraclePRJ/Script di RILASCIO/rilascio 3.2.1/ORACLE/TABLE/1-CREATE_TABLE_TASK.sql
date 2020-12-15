DROP TABLE DPA_TASK;
CREATE TABLE DPA_TASK
(
  SYSTEM_ID           NUMBER                    NOT NULL,
  ID_RUOLO_MITT       NUMBER                    DEFAULT 0                     NOT NULL,
  ID_PEOPLE_MITT      NUMBER                    DEFAULT 0                     NOT NULL,
  ID_RUOLO_RECEIVER   NUMBER                    DEFAULT 0                     NOT NULL,
  ID_PEOPLE_RECEIVER  NUMBER                    DEFAULT 0                     NOT NULL,
  ID_PROFILE          NUMBER,
  ID_PROJECT          NUMBER,
  ID_PROFILE_REVIEW   NUMBER,
  ID_TRASMISSIONE     NUMBER,
  ID_TRASM_SINGOLA    NUMBER,
  ID_RAGIONE_TRASM    NUMBER,
  ID_TIPO_ATTO 	NUMBER,
  CHA_CONTRIBUTO    CHAR(1)
);


CREATE TABLE DPA_STATO_TASK
(
  SYSTEM_ID         NUMBER                    NOT NULL,
  ID_TASK      		NUMBER    DEFAULT 0       NOT NULL,
  DTA_APERTURA		DATE,
  DTA_SCADENZA		DATE,
  DTA_LAVORAZIONE   DATE,
  DTA_CHIUSURA		DATE,
  DTA_ANNULLAMENTO  DATE,
  CHA_STATO 		CHAR(1),
  NOTE_LAVORAZIONE  VARCHAR(2000),
  NOTE_RIAPERTURA 	VARCHAR(2000)
);

CREATE TABLE DPA_TIPO_RAGIONE
(
   ID_RAGIONE_TRASM			NUMBER,
   CHA_TIPO_TASK			CHAR(1),
   ID_TIPO_ATTO				NUMBER,
   CHA_CONTRIBUTO_OBBLIGATORIO 	CHAR(1) DEFAULT '0' 
);

CREATE SEQUENCE SEQ_DPA_TASK INCREMENT BY 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 CACHE 20;
CREATE SEQUENCE SEQ_DPA_STATO_TASK INCREMENT BY 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 CACHE 20;