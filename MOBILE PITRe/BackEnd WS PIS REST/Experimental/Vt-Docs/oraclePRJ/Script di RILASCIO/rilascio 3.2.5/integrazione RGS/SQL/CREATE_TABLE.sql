CREATE TABLE DPA_TIPO_MESSAGGI_FLUSSO
(
  ID_MESSAGGIO           INT PRIMARY KEY NOT NULL,
  DESCRIZIONE     	 	 VARCHAR(2000)      NOT NULL ,
  CHA_MESSAGGIO_INIZIALE	CHAR(1),
  CHA_MESSAGGIO_FINALE		CHAR(1)
)

INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(101, 'Invio richiesta', '1', '0')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(102, 'Invio integrazione su richiesta', '0', '0')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(103, 'Invio integrazione spontanea', '0', '0')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(104, 'Invio ritiro provvedimento', '0', '0')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(201, 'Esito positivo','0', '1')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(202, 'Richiesta dati integrativi', '0', '0')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(203, 'Esito negativo', '0', '1')
INSERT INTO DPA_TIPO_MESSAGGI_FLUSSO VALUES(206, 'Accettazione richiesta di ritiro', '0', '1')


CREATE TABLE DPA_FLUSSO_PROCEDURALE
(
  SYSTEM_ID           	 INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
  ID_PROCESSO    	 	 VARCHAR(2000)      NOT NULL,
  ID_MESSAGGIO			 INT  NOT NULL,
  DTA_ARRIVO   		     DATETIME,
  ID_PROFILE			 INT NOT NULL,
  NOME_REGISTRO			 VARCHAR(2000),  --INFO RGS
  NUMERO_REGISTRO		 INT,			 --INFO RGS
  DTA_REGISTRO			 DATETIME		 --INFO RGS
)


CREATE TABLE DPA_CONTESTO_PROCEDURALE
(
	SYSTEM_ID           		INT PRIMARY KEY IDENTITY(1,1) NOT NULL,	
	TIPO_CONTESTO_PROCEDURALE	VARCHAR(100),
	NOME						VARCHAR(256),
	FAMIGLIA					INT,
	VERSIONE					INT
)


ALTER TABLE DPA_TIPO_ATTO 
ADD ID_CONTESTO_PROCEDURALE INT 


CREATE TABLE DPA_CORR_INTEROP
(
  ID_CORR           INT PRIMARY KEY NOT NULL,
  CHA_INTEROPERANTE_RGS    	 	CHAR(1)
)

CREATE TABLE DPA_FLUSSO_MESSAGGI
(
  SYSTEM_ID           		INT PRIMARY KEY IDENTITY(1,1) NOT NULL,	
  ID_MESSAGGIO              INT,
  ID_MESSAGGIO_SUCCESSIVO   INT
)

INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(101, 103)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(101, 104)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(102, 103)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(102, 104)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(103, 103)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(103, 104)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(202, 102)
INSERT INTO DPA_FLUSSO_MESSAGGI VALUES(202, 104)