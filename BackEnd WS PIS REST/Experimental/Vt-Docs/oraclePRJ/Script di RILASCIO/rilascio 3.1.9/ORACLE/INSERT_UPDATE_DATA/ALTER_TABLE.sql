--EMANUELA: aggiunta colonna CHA_COPIA_VISIBILITA per mantenere l'informazione di diritti acquisiti per copia visibilita dopo rimozione e ripristino diritto sul ruolo
ALTER TABLE 
   SECURITY
ADD
   (CHA_COPIA_VISIBILITA  CHAR(1 CHAR) DEFAULT 0 );
   
   
ALTER TABLE 
   DELETED_SECURITY
ADD
   (CHA_COPIA_VISIBILITA  CHAR(1 CHAR) DEFAULT 0 );
   
   
ALTER TABLE 
   SEC_LOG
ADD
   (CHA_COPIA_VISIBILITA  CHAR(1 CHAR) DEFAULT 0 );
   
ALTER TABLE 
   DPA_COPY_LOG
ADD
   (CHA_COPIA_VISIBILITA  CHAR(1 CHAR) DEFAULT 0 );
   
   
UPDATE SECURITY SET CHA_COPIA_VISIBILITA = '1' WHERE VAR_NOTE_SEC = 'ACQUISITO PER COPIA VISIBILITA' ;

--EMANUELA: aggiunta colonna nella DPA_CHECK_MAILBOX per tenere traccia della data e ora in cui è stato effettuato lo scarico(aggiunta perchè nel report era presente la data
--di visiualizzazione del report e non la data di scarico)
ALTER TABLE 
   DPA_CHECK_MAILBOX
ADD
   (DTA_CONCLUDED DATE DEFAULT NULL);