CREATE TABLE DPA_VERSAMENTO
(
SYSTEM_ID	NUMBER NOT NULL,
ID_PROFILE	NUMBER,
ID_PEOPLE	NUMBER,
ID_RUOLO	NUMBER,
ID_AMM		NUMBER,
CHA_STATO	VARCHAR2(2),
DTA_INVIO	DATE,
VAR_FILE_RISPOSTA	CLOB,
CHA_WARNING	VARCHAR2(1),
VAR_FILE_METADATI	CLOB
);