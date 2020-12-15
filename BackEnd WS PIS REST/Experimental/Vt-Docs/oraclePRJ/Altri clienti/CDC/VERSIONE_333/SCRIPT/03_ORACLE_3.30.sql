
begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_NOTIFY';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_NOTIFY
			(
				SYSTEM_ID          NUMBER(10, 0) NOT NULL ,
				ID_EVENT           NUMBER(10, 0) NOT NULL ,
				DESC_PRODUCER      VARCHAR2(500 CHAR) NOT NULL ,
				ID_PEOPLE_RECEIVER NUMBER(10, 0) NOT NULL ,
				ID_GROUP_RECEIVER  NUMBER(10, 0) ,
				TYPE_NOTIFY        CHAR(1 CHAR) NOT NULL ,
				DTA_NOTIFY DATE NOT NULL ,
				FIELD_1               VARCHAR2(4000 CHAR) ,
				FIELD_2               VARCHAR2(4000 CHAR) ,
				FIELD_3               VARCHAR2(4000 CHAR) ,
				FIELD_4               VARCHAR2(4000 CHAR) ,
				MULTIPLICITY          CHAR(3 CHAR) DEFAULT ''ONE'',
				SPECIALIZED_FIELD     VARCHAR2(4000 CHAR),
				TYPE_EVENT            VARCHAR2(256 CHAR),
				DOMAINOBJECT          VARCHAR2(128 BYTE),
				ID_OBJECT             NUMBER(10,0),
				ID_SPECIALIZED_OBJECT NUMBER(10,0),
				DTA_EVENT DATE NOT NULL ,
				READ_NOTIFICATION CHAR(1 CHAR) DEFAULT ''0'',
				COLOR             VARCHAR2(200 CHAR) DEFAULT ''BLUE'',
				CONSTRAINT DPA_NOTIFY_PK PRIMARY KEY ( SYSTEM_ID ) ENABLE
			)';
		end if;
	end;
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_NOTIFY_HISTORY';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_NOTIFY_HISTORY
			  (
				SYSTEM_ID          NUMBER(10, 0) NOT NULL ,
				ID_NOTIFY          NUMBER(10, 0) NOT NULL ,
				ID_EVENT           NUMBER(10, 0) NOT NULL ,
				DESC_PRODUCER      VARCHAR2(500 CHAR) NOT NULL ,
				ID_PEOPLE_RECEIVER NUMBER(10, 0) NOT NULL ,
				ID_GROUP_RECEIVER  NUMBER(10, 0) ,
				TYPE_NOTIFY        CHAR(1 CHAR) NOT NULL ,
				DTA_NOTIFY DATE NOT NULL ,
				FIELD_1               VARCHAR2(4000 CHAR) ,
				FIELD_2               VARCHAR2(4000 CHAR) ,
				FIELD_3               VARCHAR2(4000 CHAR) ,
				FIELD_4               VARCHAR2(4000 CHAR) ,
				MULTIPLICITY          CHAR(3 CHAR) DEFAULT ''ONE'',
				SPECIALIZED_FIELD     VARCHAR2(4000 CHAR),
				TYPE_EVENT            VARCHAR2(256 CHAR),
				DOMAINOBJECT          VARCHAR2(128 BYTE),
				ID_OBJECT             NUMBER(10,0),
				ID_SPECIALIZED_OBJECT NUMBER(10,0),
				DTA_EVENT DATE NOT NULL ,
				READ_NOTIFICATION CHAR(1 CHAR) DEFAULT ''0'',
				CONSTRAINT DPA_NOTIFY_HISTORY_PK PRIMARY KEY ( SYSTEM_ID ) ENABLE
			  )';
		end if;
	end;
end;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='ANNO_ACC';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD (ANNO_ACC  VARCHAR2(20))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='ANNO_ACC';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD (ANNO_ACC  VARCHAR2(20))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY_HISTORY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY_HISTORY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_CHIAVI_CONFIGURAZIONE' and column_name='CHA_CONSERVAZIONE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE ADD (CHA_CONSERVAZIONE VARCHAR2(1 BYTE) DEFAULT 0 NOT NULL)';
end if;

end;
END;
/

 
 
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_EVENT_TYPE_ASSERTIONS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_EVENT_TYPE_ASSERTIONS
			  (
				SYSTEM_ID     NUMBER(10, 0) NOT NULL ,
				ID_TYPE_EVENT NUMBER(10, 0) NOT NULL ,
				DESC_TYPE_EVENT VARCHAR2(128 char),
				ID_AUR        NUMBER(10, 0) ,
				DESC_AUR      VARCHAR2 (500 CHAR),
				TYPE_AUR   VARCHAR2(50 char) ,
				TYPE_NOTIFY   CHAR(1 CHAR) NOT NULL ,
				IS_EXERCISE   CHAR (1CHAR) DEFAULT ''0'',
				ID_AMM          NUMBER(10,0) NOT NULL,
				CONSTRAINT DPA_EVENT_TYPE_ASSERTIONS_PK PRIMARY KEY ( SYSTEM_ID ) ENABLE
			  )';
		end if;
	end;
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_CONT_CUSTOM_DOC';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_CONT_CUSTOM_DOC
			(
				SYSTEM_ID    INTEGER                          NOT NULL,
				DATA_FINE    DATE,
				ID_OGG       INTEGER,
				DATA_INIZIO  DATE,
				SOSPESO		 VARCHAR2(2))
			';
		end if;
	end;
end;
/

BEGIN

 utl_add_index('3.30','@db_user@','DPA_CONT_CUSTOM_DOC',
    'DPA_CONT_CUSTOM_DOC_PK',null,
    'SYSTEM_ID',null,null,null,
    'NORMAL', null,null,null     );
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_CONT_CUSTOM_DOC' and column_name='VAR_MAIL_RIC_PENDENTE';

if cntcol > 0 then 
	execute immediate 
	'ALTER TABLE DPA_CONT_CUSTOM_DOC ADD (CONSTRAINT DPA_CONT_CUSTOM_DOC_PK PRIMARY KEY (SYSTEM_ID)';
end if;

end;
END;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_CONT_CUSTOM_FASC';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_CONT_CUSTOM_FASC
			(
			SYSTEM_ID    INTEGER                          NOT NULL,
			DATA_FINE    DATE,
			DATA_INIZIO  DATE,
			ID_OGG_FASC  INTEGER,
			SOSPESO		 VARCHAR2(2)
			)
			';
		end if;
	end;
end;
/

BEGIN

 utl_add_index('3.30','@db_user@','DPA_CONT_CUSTOM_FASC',
    'DPA_CONT_CUSTOM_FASC_PK',null,
    'SYSTEM_ID',null,null,null,
    'NORMAL', null,null,null     );
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_CONT_CUSTOM_FASC' and column_name='VAR_MAIL_RIC_PENDENTE';

if cntcol > 0 then 
	execute immediate 
	'ALTER TABLE DPA_CONT_CUSTOM_FASC ADD (CONSTRAINT DPA_CONT_CUSTOM_FASC_PK PRIMARY KEY (SYSTEM_ID)';
end if;

end;
END;
/


BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_MAIL_REGISTRI' and column_name='VAR_MAIL_RIC_PENDENTE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_MAIL_REGISTRI ADD (VAR_MAIL_RIC_PENDENTE  VARCHAR2(1))';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_EL_REGISTRI' and column_name='VAR_MAIL_RIC_PENDENTE';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_EL_REGISTRI ADD (VAR_MAIL_RIC_PENDENTE  VARCHAR2(1))';
end if;

end;
END;
/


/* Formatted on 05/07/2013 15:42:13 (QP5 v5.215.12089.38647) */
CREATE OR REPLACE FUNCTION getContatoreDoc2 (
   docNumber          INT,
   tipoContatore      CHAR,
   oggettoCustomId    INT)
   RETURN VARCHAR
IS
   risultato         VARCHAR (255);

   valoreContatore   VARCHAR (255);
   annoContatore     VARCHAR (255);
   codiceRegRf       VARCHAR (255);
   repertorio        NUMBER;
   DataInizio        DATE;
   DataFine          DATE;
   dInizio           VARCHAR (255);
   dFine             VARCHAR (255);
BEGIN
   dInizio := '';
   dFine := '';
   valoreContatore := '';
   annoContatore := '';
   codiceRegRf := '';


   BEGIN
      SELECT valore_oggetto_db, anno, repertorio
        INTO valoreContatore, annoContatore, repertorio
        FROM dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
       WHERE     dpa_associazione_templates.doc_number = TO_CHAR (docNumber)
             AND dpa_associazione_templates.id_oggetto =
                    dpa_oggetti_custom.system_id
             AND dpa_oggetti_custom.id_tipo_oggetto =
                    dpa_tipo_oggetto.system_id
             AND dpa_tipo_oggetto.descrizione = 'Contatore'
             AND dpa_oggetti_custom.system_id = oggettoCustomId;
   --dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
   --and
   --dpa_oggetti_custom.cha_tipo_tar = 'T';
   EXCEPTION
      WHEN OTHERS
      THEN
         NULL;
   END;

   IF (repertorio = 1)
   THEN
      BEGIN
         risultato := '#CONTATORE_DI_REPERTORIO#';
         RETURN risultato;
      END;
   END IF;

   IF (tipoContatore <> 'T')
   THEN
      BEGIN
         SELECT dpa_el_registri.var_codice
           INTO codiceRegRf
           FROM dpa_associazione_templates,
                dpa_oggetti_custom,
                dpa_tipo_oggetto,
                dpa_el_registri
          WHERE     dpa_associazione_templates.doc_number =
                       TO_CHAR (docNumber)
                AND dpa_associazione_templates.id_oggetto =
                       dpa_oggetti_custom.system_id
                AND dpa_oggetti_custom.id_tipo_oggetto =
                       dpa_tipo_oggetto.system_id
                AND dpa_tipo_oggetto.descrizione = 'Contatore'
                AND dpa_oggetti_custom.system_id = oggettoCustomId
                --dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
                --and
                --dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
                AND dpa_associazione_templates.id_aoo_rf =
                       dpa_el_registri.system_id;
      EXCEPTION
         WHEN OTHERS
         THEN
            NULL;
      END;
   END IF;

   --query pervedere se il contatore + custom ed estrarre intervallo
   --annocontatore
   BEGIN
      SELECT DPA_CONT_CUSTOM_DOC.DATA_INIZIO, DPA_CONT_CUSTOM_DOC.DATA_FINE
        INTO DataInizio, DataFine
        FROM DPA_ASSOCIAZIONE_TEMPLATES,
             DPA_OGGETTI_CUSTOM,
             DPA_CONT_CUSTOM_DOC,
             DPA_TIPO_OGGETTO
       WHERE     dpa_associazione_templates.doc_number = TO_CHAR (docNumber)
             AND dpa_associazione_templates.id_oggetto =
                    dpa_oggetti_custom.system_id
             AND dpa_oggetti_custom.system_id = dpa_cont_custom_doc.id_ogg
             AND dpa_oggetti_custom.id_tipo_oggetto =
                    dpa_tipo_oggetto.system_id
             AND dpa_tipo_oggetto.descrizione = 'Contatore';
   EXCEPTION
      WHEN OTHERS
      THEN
         NULL;
   END;

   dInizio := TO_CHAR (DataInizio, 'YYYY');
   dFine := TO_CHAR (DataFine, 'YYYY');

   IF (dInizio IS NOT NULL)
   THEN
      annoContatore := dInizio || '/' || dFine;
   END IF;


   IF (codiceRegRf IS NULL)
   THEN
      risultato := NVL (valoreContatore, '') || '-' || NVL (annoContatore, '');
   ELSE
      risultato :=
            NVL (codiceRegRf, '')
         || '-'
         || NVL (annoContatore, '')
         || '-'
         || NVL (valoreContatore, '');
   END IF;

   RETURN risultato;
END Getcontatoredoc2;
/


CREATE OR REPLACE FUNCTION getContatoreFasc (systemId INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
DataInizio DATE;
DataFine DATE;
dInizio VARCHAR(255);
dFine VARCHAR(255);

BEGIN

select
valore_oggetto_db, anno
into valoreContatore, annoContatore
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;

BEGIN
SELECT DPA_CONT_CUSTOM_FASC.DATA_INIZIO,DPA_CONT_CUSTOM_FASC.DATA_FINE INTO DataInizio, DataFine FROM
DPA_ASS_TEMPLATES_FASC,DPA_OGGETTI_CUSTOM_FASC,DPA_CONT_CUSTOM_FASC,DPA_TIPO_OGGETTO_FASC
WHERE
dpa_ass_templates_fasc.id_project = to_char(systemId)
AND
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
AND
dpa_oggetti_custom_fasc.system_id = dpa_cont_custom_fasc.id_ogg_fasc
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;


exception when others then null;

END;

dInizio :=  TO_CHAR(DataInizio,'YYYY');
dFine := TO_CHAR(DataFine,'YYYY');
IF( dInizio is not null )
THEN
annoContatore := dInizio||'/'||dFine;

END IF;

if (tipocontatore <> 'T') then
begin

select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore
and
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
END;
else
risultato :=   nvl(valoreContatore,'')||'-'|| nvl(annoContatore,'');
END IF;




RETURN risultato;
END getContatoreFasc;
/

CREATE OR REPLACE FUNCTION getContatoreDoc (docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;
DataInizio DATE;
DataFine DATE;
dInizio VARCHAR(255);
dFine VARCHAR(255);


BEGIN

dInizio := '';
dFine := '';
valoreContatore := '';
annoContatore := '';
codiceRegRf := '';


begin

select
valore_oggetto_db, anno, repertorio
into valoreContatore, annoContatore, repertorio
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
--and
--dpa_oggetti_custom.cha_tipo_tar = 'T';
exception when others then null;

end;

IF(repertorio = 1) THEN
BEGIN
risultato := '#CONTATORE_DI_REPERTORIO#';
RETURN risultato;
END;
END IF;

IF(tipoContatore<>'T') THEN
BEGIN
select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and

dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
--and
--dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
and
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
exception when others then null;

END;
END IF;

--query pervedere se il contatore + custom ed estrarre intervallo
--annocontatore 
BEGIN
 
 
 
SELECT DPA_CONT_CUSTOM_DOC.DATA_INIZIO,DPA_CONT_CUSTOM_DOC.DATA_FINE INTO DataInizio, DataFine FROM
DPA_ASSOCIAZIONE_TEMPLATES,DPA_OGGETTI_CUSTOM,DPA_CONT_CUSTOM_DOC,DPA_TIPO_OGGETTO
WHERE
dpa_associazione_templates.doc_number = to_char(docNumber)
AND
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
AND
dpa_oggetti_custom.system_id = dpa_cont_custom_doc.id_ogg
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore';


exception when others then null;
 

 
END;

dInizio :=  TO_CHAR(DataInizio,'YYYY');
dFine := TO_CHAR(DataFine,'YYYY');
IF( dInizio is not null )
THEN
annoContatore := dInizio||'/'||dFine;

END IF;



if(codiceRegRf is  null)
then
risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
else  
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
end if; 

RETURN risultato;
End Getcontatoredoc;
/

CREATE OR REPLACE PROCEDURE DocCustomControllaDataInizio IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_DOC%ROWTYPE;
datainizionuova DATE;
datafinenuova DATE;
CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_DOC;

BEGIN

OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today < custom.DATA_INIZIO ) THEN
                    DECLARE
                    idtemplate DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASSOCIAZIONE_TEMPLATES WHERE ID_OGGETTO = custom.ID_OGG;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_ATTO
                                    SET IN_ESERCIZIO = 'NO'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                                                   
                                        
                                    dbms_output.put_line(' la tipologia con contatore custom con id_ogg =  '||custom.ID_OGG||  ' ha data di inizio posteriore ad oggi ' );
                                        
                                
                                END LOOP;
                                
                            END;
                

                   

                END IF;
                
                

            END LOOP;

CLOSE c_custom;   

   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END DocCustomControllaDataInizio;
/

CREATE OR REPLACE PROCEDURE DocCustomControlModifyReset IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_DOC%ROWTYPE;
datainizionuova DATE;
datafinenuova DATE;

CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_DOC;
BEGIN
  OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today > custom.DATA_FINE + 1) THEN
                    DECLARE
                    idtemplate DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASSOCIAZIONE_TEMPLATES WHERE ID_OGGETTO = custom.ID_OGG;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_ATTO
                                    SET IN_ESERCIZIO = 'NO'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                        DECLARE
                                        
                                        BEGIN
                                        select (custom.DATA_INIZIO + NUMTOYMINTERVAL(1,'YEAR')) into datainizionuova  FROM DUAL;
                                        select (custom.DATA_FINE + NUMTOYMINTERVAL(1,'YEAR')) into datafinenuova  FROM DUAL;
                                        UPDATE DPA_CONT_CUSTOM_DOC
                                        SET DATA_INIZIO = datainizionuova, DATA_FINE=datafinenuova
                                        WHERE ID_OGG = custom.ID_OGG;
                                        commit work;
                                        UPDATE DPA_CONTATORI_DOC
                                        SET VALORE = 0
                                        WHERE ID_OGG = custom.ID_OGG;
                                        commit work;
                                       
                                        
                                        dbms_output.put_line(' la tipologia con idtemplate ' ||idtemplate|| ' ed id_oggetto : ' ||custom.ID_OGG||  '  è stata modificata :' );
                                        END;
                                
                                END LOOP;
                                
                            END;
                            
                UPDATE DPA_CONT_CUSTOM_DOC
                SET SOSPESO = 'NO'
                WHERE ID_OGG = custom.ID_OGG;
                commit work;

                END IF;
            

                
                
                

            END LOOP;

CLOSE c_custom;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END DocCustomControlModifyReset;
/


CREATE OR REPLACE PROCEDURE DocCustomMettiInEsercizio IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_DOC%ROWTYPE;
CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_DOC;                    

BEGIN
 OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today > custom.DATA_INIZIO AND today < custom.DATA_FINE+1 AND ((custom.SOSPESO='NO') OR (custom.SOSPESO IS NULL)) ) THEN
                    DECLARE
                    idtemplate DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASSOCIAZIONE_TEMPLATES WHERE ID_OGGETTO = custom.ID_OGG;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_ATTO
                                    SET IN_ESERCIZIO = 'SI'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                    dbms_output.put_line(' la tipologia con idtemplate ' ||idtemplate|| ' ed id_oggetto : ' ||custom.ID_OGG||  '  è in esercizio :' );
                                
                                
                                END LOOP;
                                
                            END;
                

                   

                END IF;
                
                

            END LOOP;

CLOSE c_custom;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END DocCustomMettiInEsercizio;
/



CREATE OR REPLACE PROCEDURE FascCustomControllaDataInizio IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_FASC%ROWTYPE;
datainizionuova DATE;
datafinenuova DATE;
CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_FASC;
BEGIN
   OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today < custom.DATA_INIZIO ) THEN
                    DECLARE
                    idtemplate DPA_ASS_TEMPLATES_FASC.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASS_TEMPLATES_FASC WHERE ID_OGGETTO = custom.ID_OGG_FASC;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_FASC
                                    SET IN_ESERCIZIO = 'NO'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                                                   
                                        
                                    dbms_output.put_line(' la tipologia con contatore custom con id_ogg =  '||custom.ID_OGG_FASC||  ' ha data di inizio posteriore ad oggi ' );
                                        
                                
                                END LOOP;
                                
                            END;
                

                   

                END IF;
                
                

            END LOOP;

CLOSE c_custom;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END FascCustomControllaDataInizio;
/




CREATE OR REPLACE PROCEDURE FascCustomControlModifyReset IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_FASC%ROWTYPE;
datainizionuova DATE;
datafinenuova DATE;
CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_FASC;
BEGIN
   OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today > custom.DATA_FINE +1 ) THEN
                    DECLARE
                    idtemplate DPA_ASS_TEMPLATES_FASC.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASS_TEMPLATES_FASC WHERE ID_OGGETTO = custom.ID_OGG_FASC;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_FASC
                                    SET IN_ESERCIZIO = 'NO'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                        DECLARE
                                        
                                        BEGIN
                                        select (custom.DATA_INIZIO + NUMTOYMINTERVAL(1,'YEAR')) into datainizionuova  FROM DUAL;
                                        select (custom.DATA_FINE + NUMTOYMINTERVAL(1,'YEAR')) into datafinenuova  FROM DUAL;
                                        UPDATE DPA_CONT_CUSTOM_FASC
                                        SET DATA_INIZIO = datainizionuova, DATA_FINE=datafinenuova
                                        WHERE ID_OGG_FASC = custom.ID_OGG_FASC;
                                        commit work;
                                        UPDATE DPA_CONTATORI_FASC
                                        SET VALORE = 0
                                        WHERE ID_OGG = custom.ID_OGG_FASC;
                                        commit work;
                                       
                                        
                                        dbms_output.put_line(' la tipologia con idtemplate ' ||idtemplate|| ' ed id_oggetto : ' ||custom.ID_OGG_FASC||  '  è stata modificata :' );
                                        END;
                                
                                END LOOP;
                                
                            END;
                

                UPDATE DPA_CONT_CUSTOM_FASC
                SET SOSPESO = 'NO'
                WHERE ID_OGG_FASC = custom.ID_OGG_FASC;
                commit work;

                END IF;
                
                

            END LOOP;

CLOSE c_custom;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END FascCustomControlModifyReset;
/


CREATE OR REPLACE PROCEDURE FascCustomMettiInEsercizio IS
today DATE:= SYSDATE;
custom DPA_CONT_CUSTOM_FASC%ROWTYPE;
CURSOR c_custom IS
SELECT * FROM DPA_CONT_CUSTOM_FASC;
BEGIN
   OPEN c_custom;
            LOOP

                FETCH c_custom into custom;
                EXIT WHEN c_custom%NOTFOUND;
                IF (today > custom.DATA_INIZIO AND today < custom.DATA_FINE+1 AND ((custom.SOSPESO='NO') OR (custom.SOSPESO IS NULL)) ) THEN
                    DECLARE
                    idtemplate DPA_ASS_TEMPLATES_FASC.ID_TEMPLATE%TYPE;
                    CURSOR c_template IS
                    SELECT DISTINCT ID_TEMPLATE FROM DPA_ASS_TEMPLATES_FASC WHERE ID_OGGETTO = custom.ID_OGG_FASC;   
                            BEGIN
                            
                                OPEN c_template;
                                
                                LOOP
                                
                                FETCH c_template into idtemplate;
                                EXIT WHEN c_template%NOTFOUND;
                                    UPDATE DPA_TIPO_FASC
                                    SET IN_ESERCIZIO = 'SI'
                                    WHERE SYSTEM_ID = idtemplate;
                                    commit work;
                                    dbms_output.put_line(' la tipologia con idtemplate ' ||idtemplate|| ' ed id_oggetto : ' ||custom.ID_OGG_FASC||  '  è in esercizio :' );
                                
                                
                                END LOOP;
                                
                            END;
                

                   

                END IF;
                
                

            END LOOP;

CLOSE c_custom;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END FascCustomMettiInEsercizio;
/


CREATE OR REPLACE FUNCTION GetDateInADL (sysID int, typeID char,idGruppo INT,idPeople INT)

RETURN date IS risultato date;


BEGIN --generale


begin --interno

IF (typeID = 'D') THEN
SELECT DISTINCT(DTA_INS) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROFILE = sysID
AND (ID_PEOPLE =idPeople or ID_PEOPLE=0) and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);

END IF;
IF (typeID = 'F') THEN
SELECT DISTINCT(DTA_INS) INTO risultato FROM DPA_AREA_LAVORO WHERE ID_PROJECT = sysID
AND (ID_PEOPLE =idPeople or ID_PEOPLE=0) and ID_RUOLO_IN_UO = (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_GRUPPO = idGruppo);

END IF;


EXCEPTION
WHEN NO_DATA_FOUND THEN risultato := '';
WHEN OTHERS THEN risultato := '';

end;

RETURN risultato;
END GetDateInADL;
/


---- script CAmillo per centro notifiche(??)
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY_HISTORY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY_HISTORY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/

BEGIN
declare cntcol int;

begin
select count(*) into cntcol from cols 
	where table_name='DPA_NOTIFY' and column_name='NOTES';

if cntcol = 0 then 
	execute immediate 
	'ALTER TABLE DPA_NOTIFY ADD (NOTES  VARCHAR2(2000 CHAR) DEFAULT NULL)';
end if;

end;
END;
/


/* Formatted on 08/07/2013 08:52:15 (QP5 v5.215.12089.38647) */
CREATE OR REPLACE FUNCTION getSender (systemID NUMBER)
   RETURN VARCHAR
IS
   tmpvar   VARCHAR (256);
BEGIN
   --MITTENTE
   BEGIN
      SELECT var_desc_corr
        INTO tmpvar
        FROM (SELECT var_desc_corr
                FROM dpa_corr_globali a, dpa_doc_arrivo_par b
               WHERE     b.id_mitt_dest = a.system_id
                     AND b.cha_tipo_mitt_dest = 'M'
                     AND b.id_profile = systemID)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         tmpvar := '';
   END;

   RETURN tmpvar;
END getSender;
/


CREATE OR REPLACE FUNCTION GETSEGNATURAREPERTORIO (docId INT, idAmm INT)
RETURN varchar IS segnatura clob;
/******************************************************************************
   NAME:       GETSEGNATURAREPERTORIO
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        07/06/2013   FerlitoCa       1. Calcola la segnatura di repertorio.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     GETSEGNATURAREPERTORIO
      Sysdate:         07/06/2013
      Date and Time:   07/06/2013, 14:26:37, and 07/06/2013 14:26:37
      Username:        FerlitoCa (set in TOAD Options, Procedure Editor)
      Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/

formato_cont VARCHAR2(255 Byte);
valore_ogg_db VARCHAR2 (100 Byte);
anno_rep NUMBER;
cod_db VARCHAR2(50 Byte);
dta_inserimento DATE;
idaoorf Number;
cod_reg Number;
cod_amm VARCHAR2 (16 Byte);       
CURSOR cur IS
SELECT oc.formato_contatore, 
       t.valore_oggetto_db, 
       t.anno, 
       t.codice_db,
       t.dta_ins,
       t.id_aoo_rf
      FROM dpa_associazione_templates t
      JOIN dpa_oggetti_custom oc
      ON t.doc_number  = docId
      AND t.id_oggetto =oc.system_id
      AND oc.repertorio='1';
      
BEGIN
segnatura := formato_cont;
OPEN cur;
LOOP
FETCH cur INTO formato_cont, valore_ogg_db, anno_rep, cod_db, dta_inserimento, idaoorf;
IF valore_ogg_db is not null
THEN 
 
formato_cont := replace(upper(formato_cont), 'ANNO', anno_rep || '');
formato_cont := replace(upper(formato_cont), 'CONTATORE', valore_ogg_db || '');
formato_cont := replace(upper(formato_cont), 'COD_UO', cod_db || '');
formato_cont := replace(upper(formato_cont),'GG/MM/AAAA HH:MM', TO_CHAR(dta_inserimento, 'DD/MM/YYYY HH24:MM') || '');
formato_cont := replace(upper(formato_cont),'GG/MM/AAAA', TO_CHAR(dta_inserimento, 'DD/MM/YYYY') || '');

IF idaoorf is not null and idaoorf != 0
THEN
select var_codice into cod_reg from dpa_el_registri where system_id = idaoorf;
END IF;

IF cod_reg is not null
THEN
formato_cont := replace(upper(formato_cont), 'RF', cod_reg || '');
formato_cont := replace(upper(formato_cont), 'AOO', cod_reg || '');
END IF;


IF idAmm is not null
THEN
select var_codice_amm into cod_amm from dpa_amministra where system_id = idAmm;
formato_cont := replace(upper(formato_cont), 'COD_AMM', cod_reg || '');
END IF;
segnatura:= formato_cont;
ELSE
segnatura:='';
END IF;
EXIT;
END LOOP;
RETURN segnatura;
END GETSEGNATURAREPERTORIO;
/

CREATE OR REPLACE PROCEDURE INITEVENTTRASM
IS
  rag        VARCHAR2(32 BYTE);
  desc_rag   VARCHAR2(32 BYTE);
  id_amm_rag NUMBER(10,0);
  --field for cursor POPULATES_FIELD_DESC_PRODUCER
  desc_prod VARCHAR2(500 BYTE);
  id_event  NUMBER(10,0);
  -- cursor to populate the field desc_producer in dpa_log
  CURSOR POPULATES_FIELD_DESC_PRODUCER
  IS
    SELECT l.system_id,
      g.GROUP_NAME
      || '('
      || u.FULL_NAME
      || ')'
    FROM DPA_LOG l
    JOIN PEOPLE u
    ON l.id_people_operatore = u.system_id
    LEFT JOIN groups g
    ON l.id_gruppo_operatore = g.system_id;
  CURSOR LIST_RAG_TRASM
  IS
    SELECT DISTINCT id_amm,
      UPPER(REPLACE(var_desc_ragione, ' ', '_')),
      var_desc_ragione
    FROM dpa_ragione_trasm
    WHERE id_amm IS NOT NULL;
BEGIN
  -- delete old event on trasmisssion document and folder
  --delete old event trasmission document or folder
  DELETE
  FROM DPA_LOG_ATTIVATI
  WHERE system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'DOC_TRASMESSO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'TRASM_DOC'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'FASC_TRASMESSO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID FROM dpa_anagrafica_log WHERE VAR_CODICE = 'MOD_OGGETTO'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'TASTO_VISTO_FASC'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'DOCUMENTOCONVERSIONEPDF'
    )
  OR system_id_anagrafica =
    (SELECT SYSTEM_ID
    FROM dpa_anagrafica_log
    WHERE VAR_CODICE = 'TASTO_VISTO_DOC'
    );
  --bonifica dpa_anagrafica_log
  UPDATE DPA_ANAGRAFICA_LOG
  SET VAR_METODO    = VAR_CODICE
  WHERE VAR_METODO IS NULL;
  --elimino il vecchio evento di conversione pdf
  DELETE DPA_ANAGRAFICA_LOG
  WHERE var_codice = 'DOCUMENTOCONVERSIONEPDF';
  --creo il nuovo evento di conversione pdf
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'DOCUMENTOCONVERSIONEPDF',
      'Conversione in pdf del documento',
      'DOCUMENTO',
      'DOCUMENTOCONVERSIONEPDF',
      'ALL',
      NULL,
      '1',
      '1',
      'S_PRODUCER_EVENT',
      'BLUE'
    );
  -- creo gli eventi di accettazione, rifiuto, visto trasmissione
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      SEQ.nextval,
      'ACCEPT_TRASM_FOLDER',
      'Effettuata accettazione trasmissione del fascicolo',
      'FASCICOLO',
      'ACCEPTTRASMFOLDER',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION ',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'ACCEPT_TRASM_DOCUMENT',
      'Effettuata accettazione trasmissione del documento',
      'DOCUMENTO',
      'ACCEPTTRASMDOCUMENT',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'REJECT_TRASM_FOLDER',
      'Effettuato rifiuto trasmissione del fascicolo',
      'FASCICOLO',
      'REJECTTRASMFOLDER',
      'ONE',
      NULL,
      '1',
      '0',
      'S_SENDER_USERS_ROLE_SENDER_TRANSMISSION',
      'RED'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'REJECT_TRASM_DOCUMENT',
      'Effettuato rifiuto trasmissione del documento',
      'DOCUMENTO',
      'REJECTTRASMDOCUMENT',
      'ONE',
      NULL,
      '1',
      '0',
      'S_SENDER_USERS_ROLE_SENDER_TRANSMISSION',
      'RED'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'CHECK_TRASM_FOLDER',
      'Visto il dettaglio trasmissione del fascicolo',
      'FASCICOLO',
      'CHECKTRASMFOLDER',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'CHECK_TRASM_DOCUMENT',
      'Visto il dettaglio trasmissione del documento',
      'DOCUMENTO',
      'CHECKTRASMDOCUMENT',
      'ALL',
      NULL,
      '1',
      '1',
      'SENDER_TRANSMISSION',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'MODIFIED_OBJECT_PROTO',
      'Modificato oggetto del Protocollo',
      'DOCUMENTO',
      'MODIFIEDOBJECTPROTO',
      'ONE',
      NULL,
      '1',
      '1',
      'ACL_DOCUMENT',
      'BLUE'
    );
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'MODIFIED_OBJECT_DOC',
      'Modificato oggetto del Documento',
      'DOCUMENTO',
      'MODIFIEDOBJECTDOC',
      'ONE',
      NULL,
      '0',
      '0',
      'ACL_DOCUMENT',
      'BLUE'
    );
  DELETE FROM DPA_ANAGRAFICA_LOG WHERE VAR_CODICE = 'DOC_CAMBIO_STATO';
  INSERT
  INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'DOC_CAMBIO_STATO',
      'Cambio stato del documento',
      'DOCUMENTO',
      'DOC_CAMBIO_STATO',
      'ONE',
      NULL,
      '1',
      '1',
      'ACL_DOCUMENT',
      'BLUE'
    );
    
    --evento mancata consegna ricevuta per interoperabilità semplificata
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
    (
      seq.nextval,
      'NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY',
      'Ricevuta di mancata consegna per interoperabilità semplificata',
      'DOCUMENTO',
      'NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY',
      'ONE',
      NULL,
      '1',
      '1',
      'USERS_ROLE_PRODUCER',
      'BLUE'
    );
    
  OPEN LIST_RAG_TRASM;
  LOOP
    FETCH LIST_RAG_TRASM INTO id_amm_rag, rag, desc_rag;
    EXIT
  WHEN LIST_RAG_TRASM%NOTFOUND ;
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
      (
        seq.nextval,
        'TRASM_DOC_'
        || rag,
        'Effettuata trasmissione documento con ragione '
        || desc_rag,
        'DOCUMENTO',
        'TRASM_DOC_'
        || rag,
        NULL,
        id_amm_rag,
        '1',
        '0',
        'TRANSMISSION_RECIPIENTS',
        'BLUE'
      );
    INSERT
    INTO DPA_ANAGRAFICA_LOG VALUES
      (
        seq.nextval,
        'TRASM_FOLDER_'
        || rag,
        'Effettuata trasmissione fascicolo con ragione '
        || desc_rag,
        'FASCICOLO',
        'TRASM_FOLDER_'
        || rag,
        NULL,
        id_amm_rag,
        '1',
        '0',
        'TRANSMISSION_RECIPIENTS',
        'BLUE'
      );    
  END LOOP;
  CLOSE LIST_RAG_TRASM;
  OPEN POPULATES_FIELD_DESC_PRODUCER;
  LOOP
    FETCH POPULATES_FIELD_DESC_PRODUCER INTO id_event, desc_prod;
    EXIT
  WHEN POPULATES_FIELD_DESC_PRODUCER%NOTFOUND ;
    UPDATE DPA_LOG SET DESC_PRODUCER = desc_prod WHERE SYSTEM_ID = id_event;
  END LOOP;
  CLOSE POPULATES_FIELD_DESC_PRODUCER;
  COMMIT;
END;
/

--------------------------------------------------------
--  File creato - giovedì-maggio-23-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure SPSETDATAVISTA
--------------------------------------------------------


  CREATE OR REPLACE PROCEDURE SPSETDATAVISTA (
    p_idpeople    IN NUMBER,
    p_idoggetto   IN NUMBER,
    p_idgruppo    IN NUMBER,
    p_tipooggetto IN CHAR,
    p_iddelegato  IN NUMBER,
    p_resultvalue OUT NUMBER )
IS
  /*
  ----------------------------------------------------------------------------------------
  dpa_trasm_singola.cha_tipo_trasm = 'S''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  dpa_trasm_singola.cha_tipo_trasm = 'T''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  */
  p_cha_tipo_trasm CHAR (1) := NULL;
  p_chatipodest    NUMBER;
BEGIN
  p_resultvalue := 0;
  DECLARE
    CURSOR cursortrasmsingoladocumento
    IS
      SELECT b.system_id,
        b.cha_tipo_trasm,
        c.cha_tipo_ragione,
        b.cha_tipo_dest
      FROM dpa_trasmissione a,
        dpa_trasm_singola b,
        dpa_ragione_trasm c
      WHERE a.dta_invio      IS NOT NULL
      AND a.system_id         = b.id_trasmissione
      AND ( b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
        )
    OR b.id_corr_globale =
      (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
      ) )
      AND a.id_profile = p_idoggetto
      AND b.id_ragione = c.system_id;
      CURSOR cursortrasmsingolafascicolo
      IS
        SELECT b.system_id,
          b.cha_tipo_trasm,
          c.cha_tipo_ragione,
          b.cha_tipo_dest
        FROM dpa_trasmissione a,
          dpa_trasm_singola b,
          dpa_ragione_trasm c
        WHERE a.dta_invio      IS NOT NULL
        AND a.system_id         = b.id_trasmissione
        AND ( b.id_corr_globale =
          (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
          )
      OR b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
        ) )
        AND a.id_project = p_idoggetto
        AND b.id_ragione = c.system_id;
      BEGIN
        IF (p_tipooggetto          = 'D') THEN
          FOR currenttrasmsingola IN cursortrasmsingoladocumento
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I' ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista  IS NULL
                    AND id_trasm_singola               = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people     = p_idpeople;
                    -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                      dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista  IS NULL
                    AND id_trasm_singola               = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people     = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                  -- elimino la trasmissione singola all'utente che sta visualizzando il documento
                  --copio la notifica nello storico
                  INSERT
                  INTO DPA_NOTIFY_HISTORY
                    (
                      SYSTEM_ID,
                      ID_NOTIFY,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    )
                  SELECT seq.nextval,
                    SYSTEM_ID,
                    ID_EVENT,
                    DESC_PRODUCER,
                    ID_PEOPLE_RECEIVER,
                    ID_GROUP_RECEIVER,
                    TYPE_NOTIFY,
                    DTA_NOTIFY,
                    FIELD_1,
                    FIELD_2,
                    FIELD_3,
                    FIELD_4,
                    MULTIPLICITY,
                    SPECIALIZED_FIELD,
                    TYPE_EVENT,
                    DOMAINOBJECT,
                    ID_OBJECT,
                    ID_SPECIALIZED_OBJECT,
                    DTA_EVENT,
                    READ_NOTIFICATION
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER      = p_idpeople
                  AND (ID_GROUP_RECEIVER      = p_idgruppo
                  OR ID_GROUP_RECEIVER        = 0);
                  -- elimino la ntoifica
                  DELETE
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER      = p_idpeople
                  AND (ID_GROUP_RECEIVER      = p_idgruppo
                  OR ID_GROUP_RECEIVER        = 0);
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                    -- anche le trasmissioni singole relative agli altri utenti del ruolo
                    IF (p_iddelegato = 0) THEN
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista     = '1',
                          dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista  IS NULL
                        AND id_trasm_singola               = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people    != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola=dpa_trasm_utente.ID_TRASM_SINGOLA
                          AND a.id_people         = p_idpeople
                          ); --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista        = '1',
                          dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                          dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                          dpa_trasm_utente.cha_in_todolist    = '0'
                        WHERE dpa_trasm_utente.dta_vista     IS NULL
                        AND id_trasm_singola                  = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people       != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola=dpa_trasm_utente.ID_TRASM_SINGOLA
                          AND a.id_people         = p_idpeople
                          ); --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- la ragione di trasmissione prevede workflow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1');
                  --AND dpa_trasm_utente.id_people = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                  -- elimino la notifica all'utente che sta visualizzando il documento
                  --copio la notifica nello storico
                  INSERT
                  INTO DPA_NOTIFY_HISTORY
                    (
                      SYSTEM_ID,
                      ID_NOTIFY,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    )
                  SELECT seq.nextval,
                    SYSTEM_ID,
                    ID_EVENT,
                    DESC_PRODUCER,
                    ID_PEOPLE_RECEIVER,
                    ID_GROUP_RECEIVER,
                    TYPE_NOTIFY,
                    DTA_NOTIFY,
                    FIELD_1,
                    FIELD_2,
                    FIELD_3,
                    FIELD_4,
                    MULTIPLICITY,
                    SPECIALIZED_FIELD,
                    TYPE_EVENT,
                    DOMAINOBJECT,
                    ID_OBJECT,
                    ID_SPECIALIZED_OBJECT,
                    DTA_EVENT,
                    READ_NOTIFICATION
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER     IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                  -- elimino la notifica
                  DELETE
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER     IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
        IF (p_tipooggetto          = 'F') THEN
          FOR currenttrasmsingola IN cursortrasmsingolafascicolo
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I' ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista  IS NULL
                    AND id_trasm_singola               = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people     = p_idpeople;
                    -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --caso in cui si sta esercitando una delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                      dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista  IS NULL
                    AND id_trasm_singola               = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people     = p_idpeople;
                    -- elimino la notifica all'utente che ha visualizzato il documento
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                    --and ID_RUOLO_DEST in (p_idGruppo,0)
                  AND id_project = p_idoggetto;
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    IF (p_iddelegato                      = 0) THEN
                      -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                      -- anche le trasmissioni singole relative agli altri utenti del ruolo
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista     = '1',
                          dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista  IS NULL
                        AND id_trasm_singola               = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people    != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola=dpa_trasm_utente.ID_TRASM_SINGOLA
                          AND a.id_people         = p_idpeople
                          ); --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista        = '1',
                          dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                          dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                          dpa_trasm_utente.cha_in_todolist    = '0'
                        WHERE dpa_trasm_utente.dta_vista     IS NULL
                        AND id_trasm_singola                  = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people       != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola=dpa_trasm_utente.ID_TRASM_SINGOLA
                          AND a.id_people         = p_idpeople
                          ); --ovvero solo se io sono tra i notificati!!!
                        -- elimino la notifica a tutti gli utenti del ruolo
                        --copio la notifica nello storico
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- se la ragione di trasmissione prevede workflow
                IF (p_iddelegato =0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  -- caso in cui si sta esercitando una delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                      dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1')
                  AND dpa_trasm_utente.id_people      = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                    --and ID_RUOLO_DEST in (p_idGruppo,0)
                  AND id_project = p_idoggetto;
                  -- elimino la trasmissione singola all'utente che ha visualizzato il documento
                  --copio la notifica nello storico
                  INSERT
                  INTO DPA_NOTIFY_HISTORY
                    (
                      SYSTEM_ID,
                      ID_NOTIFY,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    )
                  SELECT seq.nextval,
                    SYSTEM_ID,
                    ID_EVENT,
                    DESC_PRODUCER,
                    ID_PEOPLE_RECEIVER,
                    ID_GROUP_RECEIVER,
                    TYPE_NOTIFY,
                    DTA_NOTIFY,
                    FIELD_1,
                    FIELD_2,
                    FIELD_3,
                    FIELD_4,
                    MULTIPLICITY,
                    SPECIALIZED_FIELD,
                    TYPE_EVENT,
                    DOMAINOBJECT,
                    ID_OBJECT,
                    ID_SPECIALIZED_OBJECT,
                    DTA_EVENT,
                    READ_NOTIFICATION
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER     IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                  -- elimino la ntoifica
                  DELETE
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER     IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
      END;
    END spsetdatavista;
/

--------------------------------------------------------
--  File creato - mercoledì-giugno-19-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure SPSETDATAVISTA_V2
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE SPSETDATAVISTA_V2 (
    p_idpeople    IN NUMBER,
    p_idoggetto   IN NUMBER,
    p_idgruppo    IN NUMBER,
    p_tipooggetto IN CHAR,
    p_iddelegato  IN NUMBER,
    p_resultvalue OUT NUMBER )
IS
  /*
  ----------------------------------------------------------------------------------------
  dpa_trasm_singola.cha_tipo_trasm = 'S''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  dpa_trasm_singola.cha_tipo_trasm = 'T''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  */
  p_cha_tipo_trasm CHAR (1) := NULL;
  p_chatipodest    NUMBER;
BEGIN
  p_resultvalue := 0;
  DECLARE
    CURSOR cursortrasmsingoladocumento
    IS
      SELECT b.system_id,
        b.cha_tipo_trasm,
        c.cha_tipo_ragione,
        b.cha_tipo_dest
      FROM dpa_trasmissione a,
        dpa_trasm_singola b,
        dpa_ragione_trasm c
      WHERE a.dta_invio      IS NOT NULL
      AND a.system_id         = b.id_trasmissione
      AND ( b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
        )
    OR b.id_corr_globale =
      (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
      ) )
      AND a.id_profile = p_idoggetto
      AND b.id_ragione = c.system_id;
      CURSOR cursortrasmsingolafascicolo
      IS
        SELECT b.system_id,
          b.cha_tipo_trasm,
          c.cha_tipo_ragione,
          b.cha_tipo_dest
        FROM dpa_trasmissione a,
          dpa_trasm_singola b,
          dpa_ragione_trasm c
        WHERE a.dta_invio      IS NOT NULL
        AND a.system_id         = b.id_trasmissione
        AND ( b.id_corr_globale =
          (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
          )
      OR b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
        ) )
        AND a.id_project = p_idoggetto
        AND b.id_ragione = c.system_id;
      BEGIN
        IF (p_tipooggetto          = 'D') THEN
          FOR currenttrasmsingola IN cursortrasmsingoladocumento
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I'
                -- modifica della 3.21 by S. Furnari
                OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )--,
                      --  dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )--,
                      -- dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                    -- anche le trasmissioni singole relative agli altri utenti del ruolo
                    IF (p_iddelegato = 0) THEN
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1'--,
                          --  dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people   != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista        = '1',
                          dpa_trasm_utente.cha_vista_delegato = '1',
                          dpa_trasm_utente.id_people_delegato = p_iddelegato--,
                          -- dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people   != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- la ragione di trasmissione prevede workflow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1');
                  --AND dpa_trasm_utente.id_people = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                  -- storicizzo ed elimino la notifica
                  --copio la notifica nello storico
                  INSERT
                  INTO DPA_NOTIFY_HISTORY
                    (
                      SYSTEM_ID,
                      ID_NOTIFY,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    )
                  SELECT seq.nextval,
                    SYSTEM_ID,
                    ID_EVENT,
                    DESC_PRODUCER,
                    ID_PEOPLE_RECEIVER,
                    ID_GROUP_RECEIVER,
                    TYPE_NOTIFY,
                    DTA_NOTIFY,
                    FIELD_1,
                    FIELD_2,
                    FIELD_3,
                    FIELD_4,
                    MULTIPLICITY,
                    SPECIALIZED_FIELD,
                    TYPE_EVENT,
                    DOMAINOBJECT,
                    ID_OBJECT,
                    ID_SPECIALIZED_OBJECT,
                    DTA_EVENT,
                    READ_NOTIFICATION
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER     IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                  -- elimino la ntoifica
                  DELETE
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER      IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
        IF (p_tipooggetto          = 'F') THEN
          FOR currenttrasmsingola IN cursortrasmsingolafascicolo
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I'
                -- modifica della 3.21 by S. Furnari
                OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )--,
                      --dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --caso in cui si sta esercitando una delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )--,
                      -- dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                    --and ID_RUOLO_DEST in (p_idGruppo,0)
                  AND id_project = p_idoggetto;
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    IF (p_iddelegato                      = 0) THEN
                      -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                      -- anche le trasmissioni singole relative agli altri utenti del ruolo
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1'--,
                          -- dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people   != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista        = '1',
                          dpa_trasm_utente.cha_vista_delegato = '1',
                          dpa_trasm_utente.id_people_delegato = p_iddelegato--,
                          -- dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people   != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- se la ragione di trasmissione prevede workflow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  -- caso in cui si sta esercitando una delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1')
                  AND dpa_trasm_utente.id_people      = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                    --and ID_RUOLO_DEST in (p_idGruppo,0)
                  AND id_project = p_idoggetto;
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1');
                  --AND dpa_trasm_utente.id_people = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                  -- storicizzo ed elimino la notifica
                  --copio la notifica nello storico
                  INSERT
                  INTO DPA_NOTIFY_HISTORY
                    (
                      SYSTEM_ID,
                      ID_NOTIFY,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION
                    )
                  SELECT seq.nextval,
                    SYSTEM_ID,
                    ID_EVENT,
                    DESC_PRODUCER,
                    ID_PEOPLE_RECEIVER,
                    ID_GROUP_RECEIVER,
                    TYPE_NOTIFY,
                    DTA_NOTIFY,
                    FIELD_1,
                    FIELD_2,
                    FIELD_3,
                    FIELD_4,
                    MULTIPLICITY,
                    SPECIALIZED_FIELD,
                    TYPE_EVENT,
                    DOMAINOBJECT,
                    ID_OBJECT,
                    ID_SPECIALIZED_OBJECT,
                    DTA_EVENT,
                    READ_NOTIFICATION
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER      IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                  -- elimino la ntoifica
                  DELETE
                  FROM DPA_NOTIFY
                  WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                  AND ID_PEOPLE_RECEIVER      IN
                    (SELECT ID_PEOPLE
                    FROM dpa_trasm_utente
                    WHERE id_trasm_singola              = currenttrasmsingola.system_id
                    AND NOT dpa_trasm_utente.dta_vista IS NULL
                    AND (cha_accettata                  = '1'
                    OR cha_rifiutata                    = '1')
                    )
                  AND (ID_GROUP_RECEIVER = p_idgruppo
                  OR ID_GROUP_RECEIVER   = 0);
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
      END;
    END spsetdatavista_v2;

/

--------------------------------------------------------
--  File creato - mercoledì-giugno-19-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure SPSETDATAVISTA_TV
--------------------------------------------------------
set define off;

CREATE OR REPLACE PROCEDURE SPSETDATAVISTA_TV (
    p_idpeople    IN NUMBER,
    p_idoggetto   IN NUMBER,
    p_idgruppo    IN NUMBER,
    p_tipooggetto IN CHAR,
    p_iddelegato  IN NUMBER,
    p_resultvalue OUT NUMBER )
IS
  /*
  ----------------------------------------------------------------------------------------
  RICHIAMATA SOLO DAL TASTO VISTO, agisce solo sulle trasmissioni NO WKFL. TOGLIENDOLE DALLA TDL
  NON SETTA DATA VISTA PERCH LO FA la SP_SET_DATAVISTA_V2
  dpa_trasm_singola.cha_tipo_trasm = 'S''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione= 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo sparisce a tutti nella ToDoList
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  dpa_trasm_singola.cha_tipo_trasm = 'T''
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'N' OR  DPA_RAGIONE_TRASM.cha_tipo_ragione = 'I''
  (ovvero senza WorkFlow): se accetta un utene in un ruolo deve sparire solo all'utente che accetta, non a tutti
  -  se DPA_RAGIONE_TRASM.cha_tipo_ragione = 'W' : non deve sparire a nessuno, si setta solo la dta_vista
  relativa all'utente corrente
  */
  p_cha_tipo_trasm CHAR (1) := NULL;
  p_chatipodest    NUMBER;
BEGIN
  p_resultvalue := 0;
  DECLARE
    CURSOR cursortrasmsingoladocumento
    IS
      SELECT b.system_id,
        b.cha_tipo_trasm,
        c.cha_tipo_ragione,
        b.cha_tipo_dest
      FROM dpa_trasmissione a,
        dpa_trasm_singola b,
        dpa_ragione_trasm c
      WHERE a.dta_invio      IS NOT NULL
      AND a.system_id         = b.id_trasmissione
      AND ( b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
        )
    OR b.id_corr_globale =
      (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
      ) )
      AND a.id_profile = p_idoggetto
      AND b.id_ragione = c.system_id;
      CURSOR cursortrasmsingolafascicolo
      IS
        SELECT b.system_id,
          b.cha_tipo_trasm,
          c.cha_tipo_ragione,
          b.cha_tipo_dest
        FROM dpa_trasmissione a,
          dpa_trasm_singola b,
          dpa_ragione_trasm c
        WHERE a.dta_invio      IS NOT NULL
        AND a.system_id         = b.id_trasmissione
        AND ( b.id_corr_globale =
          (SELECT system_id FROM dpa_corr_globali WHERE id_gruppo = p_idgruppo
          )
      OR b.id_corr_globale =
        (SELECT system_id FROM dpa_corr_globali WHERE id_people = p_idpeople
        ) )
        AND a.id_project = p_idoggetto
        AND b.id_ragione = c.system_id;
      BEGIN
        IF (p_tipooggetto          = 'D') THEN
          FOR currenttrasmsingola IN cursortrasmsingoladocumento
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I'
                -- modifica della 3.21 by S. Furnari
                OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- elimino dalla TDL(solo momentaneamente, poi andrà eliminato con il NC) la trasmissione a utente
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE -- dpa_trasm_utente.dta_vista IS NULL AND
                      id_trasm_singola             = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people = p_idpeople;
                    -- elimino la trasmissione singola all'utente che ha cliccato su visto
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION,
                        NOTES
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION,
                      NOTES
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento setto l'id delegato
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato   = p_iddelegato,
                      dpa_trasm_utente.cha_in_todolist      = '0'
                    WHERE id_trasm_singola                  =(currenttrasmsingola.system_id)
                    AND dpa_trasm_utente.id_people          = p_idpeople;
                    -- elimino la trasmissione singola all'utente che ha cliccato su visto
                    --copio la notifica nello storico
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION,
                        NOTES
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION,
                      NOTES
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la ntoifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                    -- anche le trasmissioni singole relative agli altri utenti del ruolo
                    IF (p_iddelegato = 0) THEN
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET --dpa_trasm_utente.cha_vista = '1',
                          dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                        --elimino le notifiche degli altri utenti nel ruolo
                        --storicizzo le notifiche
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION,
                            NOTES
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION,
                          NOTES
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET -- dpa_trasm_utente.cha_vista = '1',
                          dpa_trasm_utente.cha_vista_delegato = '1',
                          dpa_trasm_utente.id_people_delegato = p_iddelegato,
                          dpa_trasm_utente.cha_in_todolist    = '0'
                        WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                        --elimino le notifiche degli altri utenti nel ruolo
                        --storicizzo le notifiche
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION,
                            NOTES
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION,
                          NOTES
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- la ragione di trasmissione prevede workflow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --in caso di delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista        = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      dpa_trasm_utente.dta_vista          = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1');
                  --AND dpa_trasm_utente.id_people = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                  AND id_profile         = p_idoggetto;
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
        IF (p_tipooggetto          = 'F') THEN
          FOR currenttrasmsingola IN cursortrasmsingolafascicolo
          LOOP
            BEGIN
              IF ( currenttrasmsingola.cha_tipo_ragione = 'N' OR currenttrasmsingola.cha_tipo_ragione = 'I'
                -- modifica della 3.21 by S. Furnari
                OR currenttrasmsingola.cha_tipo_ragione = 'S' -- Interoperabilit semplificata
                ) THEN
                -- SE ? una trasmissione senza workFlow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET --dpa_trasm_utente.cha_vista = '1',
                      --dpa_trasm_utente.dta_vista =
                      --    (CASE
                      --        WHEN dta_vista IS NULL
                      --           THEN SYSDATE
                      --        ELSE dta_vista
                      --     END
                      --    ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                      id_trasm_singola             = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people = p_idpeople;
                    --elimino le notifiche degli altri utenti nel ruolo
                    --storicizzo le notifiche
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION,
                        NOTES
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION,
                      NOTES
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la notifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  --caso in cui si sta esercitando una delega
                  BEGIN
                    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
                    -- setto la data di vista
                    UPDATE dpa_trasm_utente
                    SET --dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                      --dpa_trasm_utente.dta_vista =
                      --   (CASE
                      --        WHEN dta_vista IS NULL
                      --           THEN SYSDATE
                      --        ELSE dta_vista
                      --      END
                      --     ),
                      dpa_trasm_utente.cha_in_todolist = '0'
                    WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                      id_trasm_singola             = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people = p_idpeople;
                    --elimino le notifiche degli altri utenti nel ruolo
                    --storicizzo le notifiche
                    INSERT
                    INTO DPA_NOTIFY_HISTORY
                      (
                        SYSTEM_ID,
                        ID_NOTIFY,
                        ID_EVENT,
                        DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER,
                        TYPE_NOTIFY,
                        DTA_NOTIFY,
                        FIELD_1,
                        FIELD_2,
                        FIELD_3,
                        FIELD_4,
                        MULTIPLICITY,
                        SPECIALIZED_FIELD,
                        TYPE_EVENT,
                        DOMAINOBJECT,
                        ID_OBJECT,
                        ID_SPECIALIZED_OBJECT,
                        DTA_EVENT,
                        READ_NOTIFICATION,
                        NOTES
                      )
                    SELECT seq.nextval,
                      SYSTEM_ID,
                      ID_EVENT,
                      DESC_PRODUCER,
                      ID_PEOPLE_RECEIVER,
                      ID_GROUP_RECEIVER,
                      TYPE_NOTIFY,
                      DTA_NOTIFY,
                      FIELD_1,
                      FIELD_2,
                      FIELD_3,
                      FIELD_4,
                      MULTIPLICITY,
                      SPECIALIZED_FIELD,
                      TYPE_EVENT,
                      DOMAINOBJECT,
                      ID_OBJECT,
                      ID_SPECIALIZED_OBJECT,
                      DTA_EVENT,
                      READ_NOTIFICATION,
                      NOTES
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                    -- elimino la notifica
                    DELETE
                    FROM DPA_NOTIFY
                    WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id
                    AND ID_PEOPLE_RECEIVER      = p_idpeople
                    AND (ID_GROUP_RECEIVER      = p_idgruppo
                    OR ID_GROUP_RECEIVER        = 0);
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                /*  BEGIN
                UPDATE dpa_todolist
                SET dta_vista = SYSDATE
                WHERE id_trasm_singola = currenttrasmsingola.system_id
                AND id_people_dest = p_idpeople
                --and ID_RUOLO_DEST in (p_idGruppo,0)
                AND id_project = p_idoggetto;
                EXCEPTION
                WHEN OTHERS
                THEN
                p_resultvalue := 1;
                RETURN;
                END; */
                BEGIN
                  IF ( currenttrasmsingola.cha_tipo_trasm = 'S' AND currenttrasmsingola.cha_tipo_dest = 'R' ) THEN
                    IF (p_iddelegato                      = 0) THEN
                      -- se ? una trasmissione ? di tipo SINGOLA a un RUOLO allora devo aggiornare
                      -- anche le trasmissioni singole relative agli altri utenti del ruolo
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET --dpa_trasm_utente.cha_vista = '1',
                          dpa_trasm_utente.cha_in_todolist = '0'
                        WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                        --elimino le notifiche degli altri utenti nel ruolo
                        --storicizzo le notifiche
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION,
                            NOTES
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION,
                          NOTES
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    ELSE
                      BEGIN
                        -- nelle trasmissioni utente relative agli altri utenti del ruolo
                        -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                        UPDATE dpa_trasm_utente dpa_trasm_utente
                        SET -- dpa_trasm_utente.cha_vista = '1',
                          dpa_trasm_utente.cha_vista_delegato = '1',
                          dpa_trasm_utente.id_people_delegato = p_iddelegato,
                          dpa_trasm_utente.cha_in_todolist    = '0'
                        WHERE --dpa_trasm_utente.dta_vista IS NULL AND
                          id_trasm_singola              = (currenttrasmsingola.system_id )
                        AND dpa_trasm_utente.id_people != p_idpeople
                        AND EXISTS
                          (SELECT 'x'
                          FROM dpa_trasm_utente a
                          WHERE a.id_trasm_singola = dpa_trasm_utente.id_trasm_singola
                          AND a.id_people          = p_idpeople
                          );
                        --ovvero solo se io sono tra i notificati!!!
                        --elimino le notifiche degli altri utenti nel ruolo
                        --storicizzo le notifiche
                        INSERT
                        INTO DPA_NOTIFY_HISTORY
                          (
                            SYSTEM_ID,
                            ID_NOTIFY,
                            ID_EVENT,
                            DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER,
                            TYPE_NOTIFY,
                            DTA_NOTIFY,
                            FIELD_1,
                            FIELD_2,
                            FIELD_3,
                            FIELD_4,
                            MULTIPLICITY,
                            SPECIALIZED_FIELD,
                            TYPE_EVENT,
                            DOMAINOBJECT,
                            ID_OBJECT,
                            ID_SPECIALIZED_OBJECT,
                            DTA_EVENT,
                            READ_NOTIFICATION,
                            NOTES
                          )
                        SELECT seq.nextval,
                          SYSTEM_ID,
                          ID_EVENT,
                          DESC_PRODUCER,
                          ID_PEOPLE_RECEIVER,
                          ID_GROUP_RECEIVER,
                          TYPE_NOTIFY,
                          DTA_NOTIFY,
                          FIELD_1,
                          FIELD_2,
                          FIELD_3,
                          FIELD_4,
                          MULTIPLICITY,
                          SPECIALIZED_FIELD,
                          TYPE_EVENT,
                          DOMAINOBJECT,
                          ID_OBJECT,
                          ID_SPECIALIZED_OBJECT,
                          DTA_EVENT,
                          READ_NOTIFICATION,
                          NOTES
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                        -- elimino la notifica
                        DELETE
                        FROM DPA_NOTIFY
                        WHERE ID_SPECIALIZED_OBJECT = currenttrasmsingola.system_id;
                      EXCEPTION
                      WHEN OTHERS THEN
                        p_resultvalue := 1;
                        RETURN;
                      END;
                    END IF;
                  END IF;
                END;
              ELSE
                -- se la ragione di trasmissione prevede workflow
                IF (p_iddelegato = 0) THEN
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.dta_vista   = (
                      CASE
                        WHEN dta_vista IS NULL
                        THEN SYSDATE
                        ELSE dta_vista
                      END )
                    WHERE dpa_trasm_utente.dta_vista IS NULL
                    AND id_trasm_singola              = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people    = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                ELSE
                  -- caso in cui si sta esercitando una delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                    SET -- dpa_trasm_utente.cha_vista = '1',
                      dpa_trasm_utente.cha_vista_delegato = '1',
                      dpa_trasm_utente.id_people_delegato = p_iddelegato--,
                      -- dpa_trasm_utente.dta_vista =
                      --    (CASE
                      --       WHEN dta_vista IS NULL
                      ---          THEN SYSDATE
                      --       ELSE dta_vista
                      --    END
                      --   )
                    WHERE-- dpa_trasm_utente.dta_vista IS NULL AND
                      id_trasm_singola             = (currenttrasmsingola.system_id )
                    AND dpa_trasm_utente.id_people = p_idpeople;
                  EXCEPTION
                  WHEN OTHERS THEN
                    p_resultvalue := 1;
                    RETURN;
                  END;
                END IF;
                BEGIN
                  -- Rimozione trasmissione da todolist solo se ? stata gi? accettata o rifiutata
                  UPDATE dpa_trasm_utente
                  SET cha_in_todolist                 = '0'
                  WHERE id_trasm_singola              = currenttrasmsingola.system_id
                  AND NOT dpa_trasm_utente.dta_vista IS NULL
                  AND (cha_accettata                  = '1'
                  OR cha_rifiutata                    = '1')
                  AND dpa_trasm_utente.id_people      = p_idpeople;
                  UPDATE dpa_todolist
                  SET dta_vista          = SYSDATE
                  WHERE id_trasm_singola = currenttrasmsingola.system_id
                  AND id_people_dest     = p_idpeople
                    --and ID_RUOLO_DEST in (p_idGruppo,0)
                  AND id_project = p_idoggetto;
                EXCEPTION
                WHEN OTHERS THEN
                  p_resultvalue := 1;
                  RETURN;
                END;
              END IF;
            END;
          END LOOP;
        END IF;
      END;
    END spsetdatavista_tv;

/


--------------------------------------------------------
--  File creato - martedì-maggio-14-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package Body UTILITA
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY UTILITA as 

Procedure             Utl_Insert_Log_W_Autcommit (Nomeutente Varchar2,
 data_eseguito DATE,
 comando_eseguito VARCHAR2,
 CATEGORIA_COMANDO VARCHAR2,
 esito VARCHAR2)

Is
cnt int;
MYUSERNAME VARCHAR2(200);
UTL_LOG_VARCHAR_DATA_LENGTH INT; 
PRAGMA AUTONOMOUS_TRANSACTION ;

BEGIN
select USERNAME INTO MYUSERNAME from user_users;

SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_UTL_SYSTEM_LOG';
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE SEQ_UTL_SYSTEM_LOG '||
                        'START WITH 1                       '||
                        ' MAXVALUE 99999999999     MINVALUE 1  '||
                        ' NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='UTL_SYSTEM_LOG';
IF (cnt = 0) THEN
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.UTL_SYSTEM_LOG');
end IF;

select min(data_length) into UTL_LOG_VARCHAR_DATA_LENGTH 
    from cols 
    where table_name = 'UTL_SYSTEM_LOG'
    and column_name in ('COMANDO_RICHIESTO', 'CATEGORIA_COMANDO', 'ESITO_OPERAZIONE') ;  

IF UTL_LOG_VARCHAR_DATA_LENGTH < 2000 THEN 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY COMANDO_RICHIESTO VARCHAR2(2000) '; 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY CATEGORIA_COMANDO VARCHAR2(2000) ';
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY ESITO_OPERAZIONE  VARCHAR2(2000) ';
end if; 

 INSERT INTO UTL_SYSTEM_LOG
 ( ID , DATA_OPERAZIONE
  , COMANDO_RICHIESTO
  , CATEGORIA_COMANDO
  , ESITO_OPERAZIONE   )
 VALUES ( SEQ_UTL_SYSTEM_LOG.nextval, nvl(data_eseguito, SYSTIMESTAMP)
 , substr(comando_eseguito  ,1,2000)
 , substr(CATEGORIA_COMANDO ,1,2000)
 , substr(esito             ,1,2000)  ) ;

 commit; -- si può fare perché c'è PRAGMA AUTONOMOUS_TRANSACTION prima in DECLARE
EXCEPTION
 WHEN OTHERS THEN
 DBMS_OUTPUT.put_line ('errore generico in insert log' || SQLERRM);
 RAISE; --manda errore a sp chiamante
End Utl_Insert_Log_W_Autcommit ;


PROCEDURE UTL_STORICIZZA_LOG (interval_of_days int) IS
Pragma Autonomous_Transaction ;
idx_esistente   EXCEPTION;
Pragma Exception_Init (Idx_Esistente  , -955);
-- ORA-01408 esiste già un indice per questa lista di colonne
Idx_Esistente_su_colonne   Exception;
Pragma Exception_Init (Idx_Esistente_su_colonne  , -01408);

Myusername Varchar2(200);
istruz_sql Varchar2(200);
Is_Compression_Enabled Varchar2(200);
Stringa_Msg   Varchar2(2000);
Min_Dta_Azione_Log         Date; 
Max_Dta_Azione_log_Storico Date; 
dummy varchar2(32); 
Btimestamp Timestamp; secs_elapsed number(8,1);
BEGIN
Select Username Into Myusername From User_Users;
--Select Systimestamp Into Btimestamp From Dual ;

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella STORICO ha min dta_azione:'
    ||To_Char(Min_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||To_Char(Max_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')     
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_S From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_S From Dpa_Log_Storico)   ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella DPA_LOG ha min dta_azione:'
    ||to_char(Min_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_L From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_L From Dpa_Log) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

select compression into is_compression_enabled
    from user_tables 
    where table_name = 'DPA_LOG_STORICO' ;

if is_compression_enabled = 'DISABLED' THEN 
-- il comando abilita solo la compressione per i nuovi record, non la effettua su record esistenti
-- se si volesse comprimere i record esistenti, eseguire alter table DPA_LOG_STORICO move compress; 
    Istruz_Sql := 'alter table '||MYUSERNAME||'.DPA_LOG_STORICO compress storage (buffer_pool recycle)'  ;
    Execute Immediate  Istruz_Sql ;
    utl_insert_log_w_autcommit (      MYUSERNAME        ,Sysdate    
      ,Istruz_Sql --comando_eseguito VARCHAR2,
      ,'procedura UTL_STORICIZZA_LOG, package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
      ,'abilitata compressione per tabella '||MYUSERNAME||'.DPA_LOG_STORICO' 
      ) ; 
End If; 
  
-- create indexes INDX_DTA_AZIONE on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE on DPA_LOG (DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (
            Myusername        --nomeutente VARCHAR2,
            ,Sysdate          --data_eseguito DATE,
            ,Istruz_Sql       --comando_eseguito VARCHAR2,
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
            ,'OK'  --esito VARCHAR2
            ) ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      When others then 
            utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
            RAISE;
   End;
-- now with DPA_LOG_STORICO   
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE_STORICO on DPA_LOG_STORICO(DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (Myusername,Sysdate,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'       ,'OK') ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      
      When others then 
           utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
             Raise;
   End;
-- end of create indexes on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist

select min(dta_azione) into min_dta_azione_log
    From Dpa_Log;  
Select Max(Dta_Azione) Into Max_Dta_Azione_log_storico
    From Dpa_Log_storico;  

If Min_Dta_Azione_Log < Nvl(Max_Dta_Azione_Log_Storico, To_Date('01-01-1970','dd-mm-yyyy') ) Then
  -- N8OT OK !
  Utl_Insert_Log_W_Autcommit (Myusername  ,Sysdate    , 'procedura interrotta '        
    ,'invocata procedura UTL_STORICIZZA_LOG, in package UTILITA'
    ,'Bonificare DPA_LOG_STORICO! min(dta_azione) non è maggiore o uguale a Max(Dta_Azione_storico) '    ) ; 

Else  -- ok, Min_Dta_Azione_Log è maggiore o uguale a nvl(Max_Dta_Azione_log_storico, to_date('01-01-1970','dd-mm-yyyy') )

-- since dpa_log_storico has STORAGE COMPRESSION enabled, append hint will now store rows in compressed form
  INSERT /*+ append */ INTO dpa_log_storico (
      SYSTEM_ID,         USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                    ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE
    From Dpa_Log
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0';  
  
  Delete From Dpa_Log 
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0'; 
  
  Commit;
End If; 
-- END IF Min_Dta_Azione_Log < Max_Dta_Azione_log_storico 


Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')     into stringa_msg
From (Select Min(Dta_Azione) As Min_Dta_Azione From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione From Dpa_Log_Storico)
   , (Select 'STORICO' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_Lend From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_LEnd From Dpa_Log)
   , (Select 'DPA_LOG' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,Stringa_Msg     ) ; 


EXCEPTION
    When Others  Then
    utl_insert_log_w_autcommit (
    MYUSERNAME  --nomeutente VARCHAR2,
    ,Sysdate    --data_eseguito DATE,
    ,'invocata procedura UTL_STORICIZZA_LOG; in package UTILITA'         --comando_eseguito VARCHAR2,
    ,'categoria STORICIZZA LOG'         --CATEGORIA_COMANDO VARCHAR2,
    ,'eseguito rollback. Rilevato errore: '||SQLERRM           --esito VARCHAR2
    ) ; 
    ROLLBACK;
End UTL_STORICIZZA_LOG;


procedure UTL_OPTIMIZE_CONTEXT_INDEX (log_level int 
-- 0 = disabled, 1= only error messages
--, 2 = gives total elapsed time, 3 gives elapsed per single index
, Optimize_Mode Varchar2 -- can be 'FAST' or 'FULL'  or 'FULL_PARALLEL8' or 'REBUILD_INDEX'
 -- in 'REBUILD_INDEX' mode, index is not available, but elapsed time can be much less than with FULL mode, even with 'FULL_PARALLEL8' mode
) as

Cursor C_Text Is
   Select Index_Name, Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Like '%replace%' and Parameters is not null)
   Union Select Index_Name, 'replace '||Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Not Like '%replace%' And Parameters Is Not Null)
   union Select Index_Name, NULL As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And Parameters Is Null;
    
Myusername Varchar2(200); Myindex Varchar2(200); myparam Varchar2(200);
Btimestamp Timestamp; Stringa_Msg Varchar2(2000);
--minutes_elapsed number(8,1);   seconds_elapsed  number(8,1);  
privilege_granted  varchar2(20);
begin
select USERNAME INTO MYUSERNAME from user_users;

    BEGIN
        select privilege into privilege_granted from  user_tab_privs where table_name= 'CTX_DDL' ;
        IF (nvl(privilege_granted,'null') <> 'EXECUTE' ) THEN
            RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
        end IF;
    exception when NO_DATA_FOUND then
                    RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
              when others then RAISE;
    END;

select systimestamp into Btimestamp from dual ;

For Text In C_Text
loop
    Myindex := Text.Index_Name ;
    myparam := Text.myidxparam ;
    
--OPTIMIZE_mode can be 'FULL_PARALLEL8' or 'REBUILD_INDEX' or 'FAST' or 'FULL'  
    IF OPTIMIZE_mode = 'FULL_PARALLEL8' THEN 
    -- maxtime specify maximum optimization time, in minutes, for FULL optimize.
      -- for PAT, with optimize degree 8 elapsed 36 min approx 
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode, Maxtime => 59, Parallel_Degree => 8 ) ;
     elsif  (OPTIMIZE_mode = 'REBUILD_INDEX' AND myparam IS NULL) THEN 
        Execute Immediate  'alter index '||Myindex||' rebuild ' ; 
      Elsif  (Optimize_Mode = 'REBUILD_INDEX' And Myparam Is Not Null) Then 
        Execute Immediate  'Alter Index '||Myindex||' Rebuild Parameters('' '|| Myparam||''')' ;  
    Else  -- 'FAST' or other mode
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode) ;
    END IF; 
    
 
 Select  'Context index '||Myindex||' '||Optimize_Mode||' optimized. Elapsed '
 ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
 ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds since start ' 
      into stringa_msg from dual ;

 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
    if log_level > 2 then    -- scrive nella tabella utl_system_log
        Utl_Insert_Log_W_Autcommit (Myusername           ,Sysdate  
        ,stringa_msg        ,'OPTIMIZE_CONTEXT_INDEX'               ,'ok'   ) ; 
    end if;

End Loop;
      
      
Select Optimize_Mode||' optimized all '||Myusername||
      ' context indexes via UTL_OPTIMIZE_CONTEXT_INDEX in '
      ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
      ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds'       
      into stringa_msg from dual ;

 If Log_Level > 1 Then -- scrive nella tabella utl_system_log
 Utl_Insert_Log_W_Autcommit (Myusername   ,Sysdate     , stringa_msg
    ,'OPTIMIZE_CONTEXT_INDEX'       ,'ok'   ) ; 
 end if;

exception when others then
 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
 if log_level > 0 then -- scrive nella tabella utl_system_log
 utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,sysdate  --DATE,
    ,'KO WHILE '||OPTIMIZE_mode||' OPTIMIZING '||MYINDEX||' VIA THE sp UTL_OPTIMIZE_CONTEXT_INDEX ' -- VARCHAR2,
    ,'OPTIMIZE_CONTEXT_INDEX'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)
              ) ; --           VARCHAR2)
 end if ;

End;

Procedure Utl_Setsecurityruoloreg Is 
Cursor C_Ruolo Is 
  Select Id_Registro, New_Aooruoloresp    
  From Utl_Track_Aooruoloresp 
    Where Cha_Changed_Notyet_Processed = 'y'; 
Myusername Varchar2(32);
Grantotale Int := 0; 
myflag boolean := true; 

Begin
Select Username Into  Myusername   From User_Users; 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Starting SP ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ;     
        
For Ruolo In C_Ruolo Loop 
exit when C_Ruolo%NOTFOUND;

If Myflag Then
    myflag := false; 
    Lock Table Utl_Track_Aooruoloresp In Exclusive Mode; 
     utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Started SP; locked Table Utl_Track_Aooruoloresp In Exclusive Mode ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ; 
end if;

 Insert /*+ append (Security) */ Into Security (  Thing     
                        , Personorgroup
                        , Accessrights
                        , Id_Gruppo_Trasm
                        , Cha_Tipo_Diritto )  
    Select P.System_Id        As Thing               
      ,Ruolo.New_Aooruoloresp --,Cg.Id_Gruppo           
      As Personorgroup
      ,Elr.Diritto_Ruolo_Aoo  As Accessrights
      ,Null                   As Id_Gruppo_Trasm 
      ,'A'                    As Cha_Tipo_Diritto
    From Profile P,  Dpa_El_Registri Elr --, Dpa_Corr_Globali Cg
    Where P.Id_Registro=Elr.System_Id --Elr.Id_Ruolo_Resp= Cg.System_Id And 
    And Elr.Id_Ruolo_Resp Is Not Null 
    and Elr.Cha_Rf= '0' 
    and p.num_proto is not null
    and not exists (select 'x' from SECURITY s1 
      Where S1.Thing=P.System_Id 
      And S1.Personorgroup = Ruolo.New_Aooruoloresp -- Cg.Id_Gruppo --idGruppo 
      And S1.Accessrights >= Elr.Diritto_Ruolo_Aoo --Diritto 
      );
grantotale := grantotale + SQL%ROWCOUNT; 
End Loop;      

if grantotale > 0 then
     update Utl_Track_Aooruoloresp 
      Set Cha_Changed_Notyet_Processed = 'n'        ;
     commit;   
        
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'inseriti '||grantotale|| ' record in SECURITY a seguito della modifica del Ruolo responsabile AOO' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 
else 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'trovati zero record da inserire in SECURITY ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 

end if; 


Exception
  When No_Data_Found Then Null; 
  When Others Then 
  rollback; 
   utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'ko and rollback while running SP Utl_Setsecurityruoloreg ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)         ) ; 
    Raise; 
end Utl_Setsecurityruoloreg;

End Utilita ;

/


---- fine script Camillo

CREATE OR REPLACE PROCEDURE SP_DELETE_CORR_ESTERNO (IDCorrGlobale IN NUMBER, liste IN NUMBER, IdPeople IN NUMBER, IdGruppo IN NUMBER, ReturnValue OUT NUMBER)  IS

/*
-------------------------------------------------------------------------------------------------------
SP per la Cancellazione corrispondente

Valori di ritorno gestiti:

0: CANCELLAZIONE EFFETTUATA - operazione andata a buon fine
1: DISABILITAZIONE EFFETTUATA - il corrispondente ? presente nella DPA_DOC_ARRIVO_PAR, quindi non viene cancellato
2: CORRISPONDENTE NON RIMOSSO - il corrispondente ? presente nella lista di distribuzione e non posso rimuoverlo
3: ERRORE: la DELETE sulla dpa_corr_globali NON ? andata a buon fine
4: ERRORE: la DELETE sulla dpa_dett_globali NON ? andata a buon fine
5: ERRORE: l' UPDATE sulla dpa_corr_globali NON ? andata a buon fine
6: ERRORE: la DELETE sulla dpa_liste_distr NON ? andata a buon fine
7: DISABILITAZIONE EFFETTUATA - il corrispondente non presente nella DPA_DOC_ARRIVO_PAR, ma utilizzato in campi profilati, quindi non viene cancellato, ma storicizzato
-------------------------------------------------------------------------------------------------------

*/

countDoc number; -- variabile usata per contenere il numero di documenti che hanno IL CORRISPONDENTE come
cha_tipo_urp VARCHAR2(1);
var_inLista VARCHAR2(1); -- valore 'N' (il corr non ? presente in nessuna lista di sistribuzione), 'Y' altrimenti
countLista number;
new_var_cod_rubrica1 varchar2 (128);
v_curdoc_docnumber  NUMBER;
v_curdoc_idtemplate NUMBER;
v_curdoc_idoggetto  NUMBER;
v_curfasc_idproject  NUMBER;
v_curfasc_idtemplate NUMBER;
v_curfasc_idoggetto  NUMBER;

BEGIN
    DECLARE
countProfilato number;
countProfDoc number;
countProfFasc number;
IdRuolo number;

    CURSOR cursor_doc
   IS
      select at.doc_number, at.id_template, at.id_oggetto 
      from dpa_associazione_templates at, dpa_oggetti_custom oc, dpa_tipo_oggetto dto 
      where at.valore_oggetto_db = to_char(IDCorrGlobale)
      and at.ID_OGGETTO = oc.SYSTEM_ID
      and oc.ID_TIPO_OGGETTO = dto.SYSTEM_ID
      and upper(dto.DESCRIZIONE) = 'CORRISPONDENTE';

CURSOR cursor_fasc
   IS
      select atf.id_project, atf.id_template, atf.id_oggetto 
      from dpa_ass_templates_fasc atf, dpa_oggetti_custom_fasc ocf, dpa_tipo_oggetto_fasc tof
      where atf.valore_oggetto_db = to_char(IdCorrGlobale)
      and atf.ID_OGGETTO = ocf.SYSTEM_ID
      and ocf.ID_TIPO_OGGETTO = tof.SYSTEM_ID
      and upper(tof.DESCRIZIONE) = 'CORRISPONDENTE';

    BEGIN

    select cha_tipo_urp INTO cha_tipo_urp from dpa_corr_globali where system_id = IDCorrGlobale;
    
    select system_id INTO IdRuolo from dpa_corr_globali where id_gruppo = IdGruppo;

    var_inLista := 'N'; -- di default si assume che il corr nn sia nella DPA_LISTE_DISTR

    SELECT count(SYSTEM_ID) into countLista FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

    IF (countLista > 0) THEN -- se il corrispondente ? contenuto nelle liste di distribuzione
    BEGIN
        IF (liste = 1) THEN
        --- CASO 1 - Le liste di distribuzione SONO abilitate: verifico se il corrispondente ? in una lista di distibuzione, in caso affermativo non posso rimuoverlo
        BEGIN
        -- CASO 1.1 - Il corrispondente ? predente in almeno una lista, quindi esco senza poterlo rimuoverere (VALORE RITORNATO = 2).
            ReturnValue := 2;
            RETURN;
            END;
        ELSE
        -- CASO 2 - Le liste di distribuzione NON SONO abilitate

            BEGIN
            var_inLista := 'Y';
            END;
        END IF;
    END;
    END IF;


-- Se la procedura va avanti, cio' significa che:
-- Le liste di distribuzione non sono abilitate (liste = 0), oppure sono abilitate (liste=1) ma il corrispondente che si tenta di rimuovere non ? contenuto in una lista

    SELECT count(ID_PROFILE) INTO countDoc  FROM DPA_DOC_ARRIVO_PAR WHERE ID_MITT_DEST = IDCorrGlobale;

    IF (countDoc = 0) THEN
    -- CASO 3 -  il corrispondente non ? stato mai utilizzato come mitt/dest di protocolli
    BEGIN
        countProfilato := 0;    
    
        -- se il corrispondente non ? stato usato come mitt/dest ma ? stato usato nei campi profilati
        select count(system_id) into countProfDoc from dpa_associazione_templates 
        where valore_oggetto_db = to_char(IDCorrGlobale);
        
        select count(system_id) into countProfFasc from dpa_ass_templates_fasc
        where valore_oggetto_db = to_char(IdCorrGlobale);
        
        countProfilato := countProfDoc + countProfFasc;
        
        --dbms_output.put_line('countProfDoc ' || countProfDoc);
        --dbms_output.put_line('countProfFasc ' || countProfFasc);        
        
        IF (countProfilato > 0) THEN
        BEGIN
                -- storicizzo il corrispondente
            -- Ricavo il codice rubrica del corrispondente
            SELECT var_cod_rubrica
            INTO new_var_cod_rubrica1
            FROM dpa_corr_globali
            WHERE system_id = IDCorrGlobale;

            -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
            new_var_cod_rubrica1 := new_var_cod_rubrica1 || '_' || to_char(IDCorrGlobale);

            -- Storicizzo il corrispondente
            UPDATE DPA_CORR_GLOBALI
            SET DTA_FINE = SYSDATE(),
            var_cod_rubrica = new_var_cod_rubrica1,
            var_codice = new_var_cod_rubrica1,
            id_parent = null
            WHERE SYSTEM_ID = IDCorrGlobale;
            
            IF(countProfDoc > 0) THEN
            BEGIN
            
                OPEN cursor_doc;
                LOOP
                
                    FETCH cursor_doc
                        INTO v_curdoc_docnumber, v_curdoc_idtemplate, v_curdoc_idoggetto;

                    EXIT WHEN cursor_doc%NOTFOUND;

                    BEGIN
 
                        --dbms_output.put_line('v_curdoc_docnumber ' || v_curdoc_docnumber);
                        --dbms_output.put_line('v_curdoc_idtemplate ' || v_curdoc_idtemplate);        
                        --dbms_output.put_line('v_curdoc_idoggetto ' || v_curdoc_idoggetto);

                        INSERT INTO dpa_profil_sto
                            (systemid, id_template,
                            dta_modifica, id_profile, id_ogg_custom,
                            id_people, id_ruolo_in_uo, var_desc_modifica
                            )
                        VALUES (seq_dpa_profil_sto.NEXTVAL, v_curdoc_idtemplate,
                            SYSDATE, v_curdoc_docnumber, v_curdoc_idoggetto,
                            IdPeople, IdRuolo, 'Corrispondente storicizzato per eliminazione da rubrica'
                            );

                    END;

                END LOOP;

                CLOSE cursor_doc;

            
            END;
            END IF;
            
            IF(countProfFasc > 0) THEN
            BEGIN
            
                OPEN cursor_fasc;
                LOOP
                
                    FETCH cursor_fasc
                        INTO v_curfasc_idproject, v_curfasc_idtemplate, v_curfasc_idoggetto;

                    EXIT WHEN cursor_fasc%NOTFOUND;

                    BEGIN
            
                        --dbms_output.put_line('v_curfasc_idproject ' || v_curfasc_idproject);
                        --dbms_output.put_line('v_curfasc_idtemplate ' || v_curfasc_idtemplate);        
                        --dbms_output.put_line('v_curfasc_idoggetto ' || v_curfasc_idoggetto);

                        INSERT INTO dpa_profil_fasc_sto
                            (systemid, id_template,
                            dta_modifica, id_project, id_ogg_custom,
                            id_people, id_ruolo_in_uo, var_desc_modifica
                            )
                        VALUES (seq_dpa_profil_sto.NEXTVAL, v_curfasc_idtemplate,
                            SYSDATE, v_curfasc_idproject, v_curfasc_idoggetto,
                            IdPeople, IdRuolo, 'Corrispondente storicizzato per eliminazione da rubrica'
                            );

                    END;

                END LOOP;

                CLOSE cursor_fasc;
            
            END;
            END IF;
            
            ReturnValue:=7;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 7).
            
            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
                       
               RETURN;
            
                   END;
        ELSE
            BEGIN
                -- proseguo come prima
                -- CAS0 3.1 - lo rimuovo dalla DPA_CORR_GLOBALI
                delete  FROM DPA_t_canale_corr where id_corr_globale=IDCorrGlobale;
                delete from dpa_dett_globali where id_corr_globali= IDCorrGlobale;
                
                DELETE FROM DPA_CORR_GLOBALI WHERE  SYSTEM_ID = IDCorrGlobale;

                EXCEPTION
                WHEN OTHERS THEN
                ReturnValue:=3;-- CAS0 3.1.1 - la rimozione da DPA_CORR_GLOBALI NON va a buon fine  (VALORE RITORNATO = 3).

            END;
        END IF;
        
    END;

    IF (ReturnValue=3) THEN
        RETURN; --ESCO DALLA PROCEDURA
    ELSE
        BEGIN

        ReturnValue:=0;

        DELETE FROM DPA_T_CANALE_CORR WHERE  ID_CORR_GLOBALE = IDCorrGlobale;

        -- per i RUOLI non deve essere cancellata la DPA_DETT_GLOBALI poich? in fase di creazione di un ruolo
        -- non viene fatta la insert in tale tabella
        IF(cha_tipo_urp != 'R') THEN

        BEGIN
            -- CAS0 3.1.2 - la rimozione da DPA_CORR_GLOBALI va a buon fine
            DELETE FROM DPA_DETT_GLOBALI WHERE  ID_CORR_GLOBALI = IDCorrGlobale;

            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=4;-- CAS0 3.1.2.1 - la rimozione da DPA_DETT_GLOBALI NON va a buon fine  (VALORE RITORNATO = 4).
        END;

            IF (ReturnValue=4) THEN
                RETURN; -- ESCO DALLA PROCEDURA
            ELSE
                ReturnValue:=0; -- CANCELLAZIONE ANDATA A BUON FINE
            END IF;
        END IF;

        IF (ReturnValue=0 AND liste = 0 AND var_inLista = 'Y')     THEN

        BEGIN
--se:
-- 1) sono andate bene le DELETE precedenti
-- 2) sono disabilitate le liste di distribuzione
-- 3) il corrispondente ? nella DPA_LISTE_DISTR

-- rimuovo il corrispondente dalla DPA_LISTE_DISTR
-- rimuovo il corrispondente dalla DPA_LISTE_DISTR
            DELETE FROM DPA_LISTE_DISTR WHERE ID_DPA_CORR = IDCorrGlobale;

            EXCEPTION
            WHEN OTHERS THEN
            ReturnValue:=6;-- la rimozione da DPA_LISTE_DISTR NON va a buon fine  (VALORE RITORNATO = 6).

        END;

        END IF;

    END;
    END IF;
    ELSE

-- CASO 4 -  il corrispondente ?  stato utilizzato come mitt/dest di protocolli
-- 4.1) disabilitazione del corrispondente
-- Il nuovo codice rubrica da attribuire al corrispondente storicizzato
        declare new_var_cod_rubrica varchar2 (128);
        BEGIN

-- Ricavo il codice rubrica del corrispondente
        SELECT var_cod_rubrica
        INTO new_var_cod_rubrica
        FROM dpa_corr_globali
        WHERE system_id = IDCorrGlobale;

-- Costruisco il codice rubrica da attribuire la corrispondente
-- storicizzato
        new_var_cod_rubrica := new_var_cod_rubrica || '_' || to_char(IDCorrGlobale);

-- Storicizzo il corrispondente
        UPDATE DPA_CORR_GLOBALI
        SET DTA_FINE = SYSDATE(),
        var_cod_rubrica = new_var_cod_rubrica,
        var_codice = new_var_cod_rubrica,
        id_parent = null
        WHERE SYSTEM_ID = IDCorrGlobale;


        EXCEPTION
        WHEN OTHERS THEN
        ReturnValue:=5;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).

        END;
        IF(ReturnValue=5) THEN
            RETURN;
        ELSE
            ReturnValue:=1;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 1).
        END IF;

    END IF;
    END;
END;

/


--------------------------------------------------------
--  File creato - martedì-maggio-14-2013   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Package Body UTILITA
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY UTILITA as 

Procedure             Utl_Insert_Log_W_Autcommit (Nomeutente Varchar2,
 data_eseguito DATE,
 comando_eseguito VARCHAR2,
 CATEGORIA_COMANDO VARCHAR2,
 esito VARCHAR2)

Is
cnt int;
MYUSERNAME VARCHAR2(200);
UTL_LOG_VARCHAR_DATA_LENGTH INT; 
PRAGMA AUTONOMOUS_TRANSACTION ;

BEGIN
select USERNAME INTO MYUSERNAME from user_users;

SELECT COUNT(*) INTO cnt FROM user_sequences where sequence_name='SEQ_UTL_SYSTEM_LOG';
If (Cnt = 0) Then
        Execute Immediate 'CREATE SEQUENCE SEQ_UTL_SYSTEM_LOG '||
                        'START WITH 1                       '||
                        ' MAXVALUE 99999999999     MINVALUE 1  '||
                        ' NOCYCLE   NOCACHE   NOORDER';
end IF;

SELECT COUNT(*) INTO cnt FROM user_tables where table_name='UTL_SYSTEM_LOG';
IF (cnt = 0) THEN
    RAISE_APPLICATION_ERROR(-20001,'Missing table '||MYUSERNAME||'.UTL_SYSTEM_LOG');
end IF;

select min(data_length) into UTL_LOG_VARCHAR_DATA_LENGTH 
    from cols 
    where table_name = 'UTL_SYSTEM_LOG'
    and column_name in ('COMANDO_RICHIESTO', 'CATEGORIA_COMANDO', 'ESITO_OPERAZIONE') ;  

IF UTL_LOG_VARCHAR_DATA_LENGTH < 2000 THEN 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY COMANDO_RICHIESTO VARCHAR2(2000) '; 
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY CATEGORIA_COMANDO VARCHAR2(2000) ';
    EXECUTE IMMEDIATE 'ALTER TABLE UTL_SYSTEM_LOG MODIFY ESITO_OPERAZIONE  VARCHAR2(2000) ';
end if; 

 INSERT INTO UTL_SYSTEM_LOG
 ( ID , DATA_OPERAZIONE
  , COMANDO_RICHIESTO
  , CATEGORIA_COMANDO
  , ESITO_OPERAZIONE   )
 VALUES ( SEQ_UTL_SYSTEM_LOG.nextval, nvl(data_eseguito, SYSTIMESTAMP)
 , substr(comando_eseguito  ,1,2000)
 , substr(CATEGORIA_COMANDO ,1,2000)
 , substr(esito             ,1,2000)  ) ;

 commit; -- si può fare perché c'è PRAGMA AUTONOMOUS_TRANSACTION prima in DECLARE
EXCEPTION
 WHEN OTHERS THEN
 DBMS_OUTPUT.put_line ('errore generico in insert log' || SQLERRM);
 RAISE; --manda errore a sp chiamante
End Utl_Insert_Log_W_Autcommit ;


PROCEDURE UTL_STORICIZZA_LOG (interval_of_days int) IS
Pragma Autonomous_Transaction ;
idx_esistente   EXCEPTION;
Pragma Exception_Init (Idx_Esistente  , -955);
-- ORA-01408 esiste già un indice per questa lista di colonne
Idx_Esistente_su_colonne   Exception;
Pragma Exception_Init (Idx_Esistente_su_colonne  , -01408);

Myusername Varchar2(200);
istruz_sql Varchar2(200);
Is_Compression_Enabled Varchar2(200);
Stringa_Msg   Varchar2(2000);
Min_Dta_Azione_Log         Date; 
Max_Dta_Azione_log_Storico Date; 
dummy varchar2(32); 
Btimestamp Timestamp; secs_elapsed number(8,1);
BEGIN
Select Username Into Myusername From User_Users;
--Select Systimestamp Into Btimestamp From Dual ;

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella STORICO ha min dta_azione:'
    ||To_Char(Min_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||To_Char(Max_Dta_Azione_S,'dd/mm/yyyy hh24:mi:ss')     
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_S From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_S From Dpa_Log_Storico)   ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'Prima di aver iniziato esecuzione della procedura, alle '||Systimestamp||', la tabella DPA_LOG ha min dta_azione:'
    ||to_char(Min_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_L,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_L From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_L From Dpa_Log) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

select compression into is_compression_enabled
    from user_tables 
    where table_name = 'DPA_LOG_STORICO' ;

if is_compression_enabled = 'DISABLED' THEN 
-- il comando abilita solo la compressione per i nuovi record, non la effettua su record esistenti
-- se si volesse comprimere i record esistenti, eseguire alter table DPA_LOG_STORICO move compress; 
    Istruz_Sql := 'alter table '||MYUSERNAME||'.DPA_LOG_STORICO compress storage (buffer_pool recycle)'  ;
    Execute Immediate  Istruz_Sql ;
    utl_insert_log_w_autcommit (      MYUSERNAME        ,Sysdate    
      ,Istruz_Sql --comando_eseguito VARCHAR2,
      ,'procedura UTL_STORICIZZA_LOG, package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
      ,'abilitata compressione per tabella '||MYUSERNAME||'.DPA_LOG_STORICO' 
      ) ; 
End If; 
  
-- create indexes INDX_DTA_AZIONE on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE on DPA_LOG (DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (
            Myusername        --nomeutente VARCHAR2,
            ,Sysdate          --data_eseguito DATE,
            ,Istruz_Sql       --comando_eseguito VARCHAR2,
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'         --CATEGORIA_COMANDO VARCHAR2,
            ,'OK'  --esito VARCHAR2
            ) ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      When others then 
            utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
            RAISE;
   End;
-- now with DPA_LOG_STORICO   
   Begin
      Istruz_Sql := 'create index INDX_DTA_AZIONE_STORICO on DPA_LOG_STORICO(DTA_AZIONE) compress storage (buffer_pool recycle)';
      EXECUTE IMMEDIATE  istruz_sql ;
      utl_insert_log_w_autcommit (Myusername,Sysdate,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'       ,'OK') ;
   EXCEPTION
      WHEN idx_esistente       THEN
         Dbms_Output.Put_Line ('indice già esistente');
      When Idx_Esistente_Su_Colonne Then
         Dbms_Output.Put_Line ('indice già esistente');
      
      When others then 
           utl_insert_log_w_autcommit ( Myusername,Sysdate ,Istruz_Sql       
            ,'procedura UTL_STORICIZZA_LOG, in package UTILITA'            ,'KO!'              ) ;
             Raise;
   End;
-- end of create indexes on DPA_LOG(DTA_AZIONE) and DPA_LOG_STORICO(DTA_AZIONE) if do not exist

select min(dta_azione) into min_dta_azione_log
    From Dpa_Log;  
Select Max(Dta_Azione) Into Max_Dta_Azione_log_storico
    From Dpa_Log_storico;  

If Min_Dta_Azione_Log < Nvl(Max_Dta_Azione_Log_Storico, To_Date('01-01-1970','dd-mm-yyyy') ) Then
  -- N8OT OK !
  Utl_Insert_Log_W_Autcommit (Myusername  ,Sysdate    , 'procedura interrotta '        
    ,'invocata procedura UTL_STORICIZZA_LOG, in package UTILITA'
    ,'Bonificare DPA_LOG_STORICO! min(dta_azione) non è maggiore o uguale a Max(Dta_Azione_storico) '    ) ; 

Else  -- ok, Min_Dta_Azione_Log è maggiore o uguale a nvl(Max_Dta_Azione_log_storico, to_date('01-01-1970','dd-mm-yyyy') )

-- since dpa_log_storico has STORAGE COMPRESSION enabled, append hint will now store rows in compressed form
  INSERT /*+ append */ INTO dpa_log_storico (
      SYSTEM_ID,         USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                    ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE)
  SELECT SYSTEM_ID,    USERID_OPERATORE,    ID_PEOPLE_OPERATORE,
      ID_GRUPPO_OPERATORE,                   ID_AMM,
      DTA_AZIONE,    VAR_OGGETTO,    ID_OGGETTO,    VAR_DESC_OGGETTO,
      VAR_COD_AZIONE,    CHA_ESITO,    VAR_DESC_AZIONE
    From Dpa_Log
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0';  
  
  Delete From Dpa_Log 
        Where (Dta_Azione between Min_Dta_Azione_Log and (Min_Dta_Azione_Log + interval_of_days)) and check_notify = '0'; 
  
  Commit;
End If; 
-- END IF Min_Dta_Azione_Log < Max_Dta_Azione_log_storico 


Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione,'dd/mm/yyyy hh24:mi:ss')     into stringa_msg
From (Select Min(Dta_Azione) As Min_Dta_Azione From Dpa_Log_Storico)
   , (Select Max(Dta_Azione) As Max_Dta_Azione From Dpa_Log_Storico)
   , (Select 'STORICO' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,stringa_msg     ) ; 

Select 'A fine esecuzione della procedura, alle '||Systimestamp||',                 la tabella '||Tabelladi||' ha min dta_azione:'
    ||to_char(Min_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss')||' e Max Dta_Azione: ' ||to_char(Max_Dta_Azione_LEnd,'dd/mm/yyyy hh24:mi:ss') 
    Into Stringa_Msg
From (Select Min(Dta_Azione) As Min_Dta_Azione_Lend From Dpa_Log)
   , (Select Max(Dta_Azione) As Max_Dta_Azione_LEnd From Dpa_Log)
   , (Select 'DPA_LOG' As Tabelladi From Dual) ; 
utl_insert_log_w_autcommit (MYUSERNAME      ,sysdate       ,'estrazione intervallo date da DPA_LOG e DPA_LOG_STORICO' 
    ,'procedura UTL_STORICIZZA_LOG, package UTILITA'             ,Stringa_Msg     ) ; 


EXCEPTION
    When Others  Then
    utl_insert_log_w_autcommit (
    MYUSERNAME  --nomeutente VARCHAR2,
    ,Sysdate    --data_eseguito DATE,
    ,'invocata procedura UTL_STORICIZZA_LOG; in package UTILITA'         --comando_eseguito VARCHAR2,
    ,'categoria STORICIZZA LOG'         --CATEGORIA_COMANDO VARCHAR2,
    ,'eseguito rollback. Rilevato errore: '||SQLERRM           --esito VARCHAR2
    ) ; 
    ROLLBACK;
End UTL_STORICIZZA_LOG;


procedure UTL_OPTIMIZE_CONTEXT_INDEX (log_level int 
-- 0 = disabled, 1= only error messages
--, 2 = gives total elapsed time, 3 gives elapsed per single index
, Optimize_Mode Varchar2 -- can be 'FAST' or 'FULL'  or 'FULL_PARALLEL8' or 'REBUILD_INDEX'
 -- in 'REBUILD_INDEX' mode, index is not available, but elapsed time can be much less than with FULL mode, even with 'FULL_PARALLEL8' mode
) as

Cursor C_Text Is
   Select Index_Name, Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Like '%replace%' and Parameters is not null)
   Union Select Index_Name, 'replace '||Parameters As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And (Lower(Parameters) Not Like '%replace%' And Parameters Is Not Null)
   union Select Index_Name, NULL As Myidxparam From User_Indexes
    Where Index_Type='DOMAIN' And Ityp_Name = 'CONTEXT' And Parameters Is Null;
    
Myusername Varchar2(200); Myindex Varchar2(200); myparam Varchar2(200);
Btimestamp Timestamp; Stringa_Msg Varchar2(2000);
--minutes_elapsed number(8,1);   seconds_elapsed  number(8,1);  
privilege_granted  varchar2(20);
begin
select USERNAME INTO MYUSERNAME from user_users;

    BEGIN
        select privilege into privilege_granted from  user_tab_privs where table_name= 'CTX_DDL' ;
        IF (nvl(privilege_granted,'null') <> 'EXECUTE' ) THEN
            RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
        end IF;
    exception when NO_DATA_FOUND then
                    RAISE_APPLICATION_ERROR(-20002,'Missing EXECUTE privilege on CTX_DDL; EXECUTE privilege must be given by DBA to '||MYUSERNAME);
              when others then RAISE;
    END;

select systimestamp into Btimestamp from dual ;

For Text In C_Text
loop
    Myindex := Text.Index_Name ;
    myparam := Text.myidxparam ;
    
--OPTIMIZE_mode can be 'FULL_PARALLEL8' or 'REBUILD_INDEX' or 'FAST' or 'FULL'  
    IF OPTIMIZE_mode = 'FULL_PARALLEL8' THEN 
    -- maxtime specify maximum optimization time, in minutes, for FULL optimize.
      -- for PAT, with optimize degree 8 elapsed 36 min approx 
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode, Maxtime => 59, Parallel_Degree => 8 ) ;
     elsif  (OPTIMIZE_mode = 'REBUILD_INDEX' AND myparam IS NULL) THEN 
        Execute Immediate  'alter index '||Myindex||' rebuild ' ; 
      Elsif  (Optimize_Mode = 'REBUILD_INDEX' And Myparam Is Not Null) Then 
        Execute Immediate  'Alter Index '||Myindex||' Rebuild Parameters('' '|| Myparam||''')' ;  
    Else  -- 'FAST' or other mode
      Ctx_Ddl.Optimize_Index(Myindex,Optimize_Mode) ;
    END IF; 
    
 
 Select  'Context index '||Myindex||' '||Optimize_Mode||' optimized. Elapsed '
 ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
 ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds since start ' 
      into stringa_msg from dual ;

 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
    if log_level > 2 then    -- scrive nella tabella utl_system_log
        Utl_Insert_Log_W_Autcommit (Myusername           ,Sysdate  
        ,stringa_msg        ,'OPTIMIZE_CONTEXT_INDEX'               ,'ok'   ) ; 
    end if;

End Loop;
      
      
Select Optimize_Mode||' optimized all '||Myusername||
      ' context indexes via UTL_OPTIMIZE_CONTEXT_INDEX in '
      ||Extract(Minute From Systimestamp - Btimestamp) ||' minutes and '
      ||round(Extract(Second From Systimestamp - Btimestamp))|| ' seconds'       
      into stringa_msg from dual ;

 If Log_Level > 1 Then -- scrive nella tabella utl_system_log
 Utl_Insert_Log_W_Autcommit (Myusername   ,Sysdate     , stringa_msg
    ,'OPTIMIZE_CONTEXT_INDEX'       ,'ok'   ) ; 
 end if;

exception when others then
 -- 0 = disabled, 1= only error messages, 2 = gives total elapsed time, 3 gives elapsed per single index
 if log_level > 0 then -- scrive nella tabella utl_system_log
 utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,sysdate  --DATE,
    ,'KO WHILE '||OPTIMIZE_mode||' OPTIMIZING '||MYINDEX||' VIA THE sp UTL_OPTIMIZE_CONTEXT_INDEX ' -- VARCHAR2,
    ,'OPTIMIZE_CONTEXT_INDEX'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)
              ) ; --           VARCHAR2)
 end if ;

End;

Procedure Utl_Setsecurityruoloreg Is 
Cursor C_Ruolo Is 
  Select Id_Registro, New_Aooruoloresp    
  From Utl_Track_Aooruoloresp 
    Where Cha_Changed_Notyet_Processed = 'y'; 
Myusername Varchar2(32);
Grantotale Int := 0; 
myflag boolean := true; 

Begin
Select Username Into  Myusername   From User_Users; 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Starting SP ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ;     
        
For Ruolo In C_Ruolo Loop 
exit when C_Ruolo%NOTFOUND;

If Myflag Then
    myflag := false; 
    Lock Table Utl_Track_Aooruoloresp In Exclusive Mode; 
     utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
        ,Sysdate  --DATE,
        ,'Started SP; locked Table Utl_Track_Aooruoloresp In Exclusive Mode ....' -- VARCHAR2,
        ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
        ,'ok'         ) ; 
end if;

 Insert /*+ append (Security) */ Into Security (  Thing     
                        , Personorgroup
                        , Accessrights
                        , Id_Gruppo_Trasm
                        , Cha_Tipo_Diritto )  
    Select P.System_Id        As Thing               
      ,Ruolo.New_Aooruoloresp --,Cg.Id_Gruppo           
      As Personorgroup
      ,Elr.Diritto_Ruolo_Aoo  As Accessrights
      ,Null                   As Id_Gruppo_Trasm 
      ,'A'                    As Cha_Tipo_Diritto
    From Profile P,  Dpa_El_Registri Elr --, Dpa_Corr_Globali Cg
    Where P.Id_Registro=Elr.System_Id --Elr.Id_Ruolo_Resp= Cg.System_Id And 
    And Elr.Id_Ruolo_Resp Is Not Null 
    and Elr.Cha_Rf= '0' 
    and p.num_proto is not null
    and not exists (select 'x' from SECURITY s1 
      Where S1.Thing=P.System_Id 
      And S1.Personorgroup = Ruolo.New_Aooruoloresp -- Cg.Id_Gruppo --idGruppo 
      And S1.Accessrights >= Elr.Diritto_Ruolo_Aoo --Diritto 
      );
grantotale := grantotale + SQL%ROWCOUNT; 
End Loop;      

if grantotale > 0 then
     update Utl_Track_Aooruoloresp 
      Set Cha_Changed_Notyet_Processed = 'n'        ;
     commit;   
        
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'inseriti '||grantotale|| ' record in SECURITY a seguito della modifica del Ruolo responsabile AOO' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 
else 
 utl_insert_log_w_autcommit (    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'trovati zero record da inserire in SECURITY ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,'ok'         ) ; 

end if; 


Exception
  When No_Data_Found Then Null; 
  When Others Then 
  rollback; 
   utl_insert_log_w_autcommit (
    MYUSERNAME   --VARCHAR2,
    ,Sysdate  --DATE,
    ,'ko and rollback while running SP Utl_Setsecurityruoloreg ' -- VARCHAR2,
    ,'Utl_Setsecurityruoloreg'       --VARCHAR2,
    ,SUBSTR('ko! '||sqlerrm,200)         ) ; 
    Raise; 
end Utl_Setsecurityruoloreg;

End Utilita ;

/


begin
  Utl_Insert_Chiave_Config('FE_ENABLE_MITTENTI_MULTIPLI','Abilita la visualizzazione dei mittenti multipli nel FE'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_INTERNAL_PROTOCOL','Abilita amministraizone al protocollo interno'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_KEY_WORDS','Abilita la parola chiave nella scheda del documento'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_SINCRONA_LC','Abilita la conversione sincrona pdf'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_LATO_SERVER','Abilita la conversione pdf lato server'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
   Utl_Insert_Chiave_Config('RUBRICA_RIGHE_PER_PAGINA','Definisce il numero di righe da visualizzare per pagina nella rubrica'  -- Codice, Descrizione
   ,'8','F','1'                            --Valore 
   ,'1','0','4.00.0'                  --Modificabile         ,Globale 
   ,NULL, NULL, NULL);    --Codice_Old_Webconfig 
   end;
/

begin
  Utl_Insert_Chiave_Config('FE_RENDER_PDF','Permette la creazione di un file in pdf da posizione segnatura'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/


begin
  Utl_Insert_Chiave_Config('FE_REQ_CONV_PDF','Obbliga la conversione in pdf alla firma'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_IDENTITY_CARD','La chiave abilita o meno la gestione della carta di identità'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','4.00.0'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;
/


CREATE OR REPLACE PROCEDURE AggiornaContatori IS

ID_OGG_CUS DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO%TYPE;
id_temp DPA_ASSOCIAZIONE_TEMPLATES.SYSTEM_ID%TYPE;
DataInizio DATE;
DataFine DATE;
cursor c_system_id is
SELECT DPA_TIPO_ATTO.SYSTEM_ID FROM DPA_TIPO_ATTO WHERE
UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='COMUNICATO'
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO)='CIRCOLARE DOCENTI' 
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO)='CIRCOLARE ATA' 
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO)='AVVISO AGLI STUDENTI'
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO)='COMUNICAZIONE SCUOLA-FAMIGLIA';
BEGIN
DataInizio:= to_date('01/09/2013', 'dd/mm/yyyy');
DataFine:= to_date('31/08/2014', 'dd/mm/yyyy');

   OPEN c_system_id;
    LOOP
    
        FETCH c_system_id into id_temp;
            EXIT WHEN c_system_id%NOTFOUND;
                       
            SELECT DISTINCT ID_OGGETTO INTO ID_OGG_CUS FROM DPA_ASSOCIAZIONE_TEMPLATES,DPA_OGGETTI_CUSTOM
            WHERE DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO=DPA_OGGETTI_CUSTOM.SYSTEM_ID
            AND DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE = id_temp AND DPA_OGGETTI_CUSTOM.ID_TIPO_OGGETTO=59 ;
            dbms_output.put_line(ID_OGG_CUS);
            UPDATE DPA_OGGETTI_CUSTOM SET DPA_OGGETTI_CUSTOM.FORMATO_CONTATORE='CONTATORE - a.s. ANNO'
            WHERE DPA_OGGETTI_CUSTOM.SYSTEM_ID=ID_OGG_CUS;
            dbms_output.put_line('modificato');
            INSERT INTO DPA_CONT_CUSTOM_DOC (DATA_INIZIO,DATA_FINE,SOSPESO,ID_OGG,SYSTEM_ID)
            VALUES (DataInizio,DataFine,'NO',ID_OGG_CUS,seq.nextval);
            COMMIT WORK;
  
    END LOOP;
   
   CLOSE c_system_id;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       RAISE;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END AggiornaContatori;
/

CREATE OR REPLACE PROCEDURE AggiornaContatoriFasc IS

ID_OGG_CUS DPA_ASS_TEMPLATES_FASC.ID_OGGETTO%TYPE;
id_temp DPA_ASS_TEMPLATES_FASC.SYSTEM_ID%TYPE;
DataInizio DATE;
DataFine DATE;
cursor c_system_id is
SELECT DPA_TIPO_FASC.SYSTEM_ID FROM DPA_TIPO_FASC WHERE
lower(DPA_TIPO_FASC.VAR_DESC_FASC) ='comunicato'
OR lower(DPA_TIPO_FASC.VAR_DESC_FASC)='circolare docenti' 
OR lower(DPA_TIPO_FASC.VAR_DESC_FASC)='circolare ata' 
OR lower(DPA_TIPO_FASC.VAR_DESC_FASC)='avviso agli studenti'
OR lower(DPA_TIPO_FASC.VAR_DESC_FASC)='comunicazione scuola-famiglia' ;
BEGIN
/******************************************************************************
  AUTHOR:   Carlo Francesco Fuccia
  NAME:     AGGIORNACONTATORIFASC
  PURPOSE:  Store la conversione delle vecchie tipologie del contatore(classico)
  in custom. Viene impostata sia la data di conteggio che il formato del 
  contatore. Tale procedura è relativa alla tipologia "fascicolo"
  ******************************************************************************/
DataInizio:= to_date('01/09/2013', 'dd/mm/yyyy');
DataFine:= to_date('31/08/2014', 'dd/mm/yyyy');

   OPEN c_system_id;
    LOOP
    
        FETCH c_system_id into id_temp;
            EXIT WHEN c_system_id%NOTFOUND;
                       
            SELECT DISTINCT ID_OGGETTO INTO ID_OGG_CUS FROM DPA_ASS_TEMPLATES_FASC,DPA_OGGETTI_CUSTOM_FASC
            WHERE DPA_ASS_TEMPLATES_FASC.ID_OGGETTO=DPA_OGGETTI_CUSTOM_FASC.SYSTEM_ID
            AND DPA_ASS_TEMPLATES_FASC.ID_TEMPLATE = id_temp AND DPA_OGGETTI_CUSTOM_FASC.ID_TIPO_OGGETTO=59 ;
            dbms_output.put_line(ID_OGG_CUS);
            UPDATE DPA_OGGETTI_CUSTOM_FASC SET DPA_OGGETTI_CUSTOM_FASC.FORMATO_CONTATORE='CONTATORE - a.s. ANNO'
            WHERE DPA_OGGETTI_CUSTOM_FASC.SYSTEM_ID=ID_OGG_CUS;
            dbms_output.put_line('modificato');
            INSERT INTO DPA_CONT_CUSTOM_FASC (DATA_INIZIO,DATA_FINE,SOSPESO,ID_OGG_FASC,SYSTEM_ID)
            VALUES (DataInizio,DataFine,'NO',ID_OGG_CUS,seq.nextval);
            COMMIT WORK;
    END LOOP;
   
   CLOSE c_system_id;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       RAISE;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END AggiornaContatoriFasc;
/

CREATE OR REPLACE FUNCTION getContatoreDoc (docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
annoAccademico VARCHAR(255);
formatoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;

BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';

begin

    select
    valore_oggetto_db, anno,anno_acc,formato_contatore, repertorio
    into valoreContatore, annoContatore,annoAccademico,formatoContatore,repertorio
    from
    dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
    where
    dpa_associazione_templates.doc_number = to_char(docNumber)
    and
    dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
    and
    dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
    and
    dpa_tipo_oggetto.descrizione = 'Contatore'
    and
    dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
    --and
    --dpa_oggetti_custom.cha_tipo_tar = 'T';
    exception when others then null;

    end;

    IF(repertorio = 1) THEN
    BEGIN
    risultato := '#CONTATORE_DI_REPERTORIO#';
    RETURN risultato;
    END;
    END IF;

    IF(tipoContatore<>'T') THEN
    BEGIN
    select
    dpa_el_registri.var_codice
    into codiceRegRf
    from
    dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
    where
    dpa_associazione_templates.doc_number = to_char(docNumber)
    and
    dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
    and
    dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
    and

    dpa_tipo_oggetto.descrizione = 'Contatore'
    and
    dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
    --and
    --dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
    and
    dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
    exception when others then null;

    END;
    END IF;

      
    IF( annoAccademico is not null )
        THEN
        annoContatore := annoAccademico;
        
        IF (instr(formatoContatore,'a.s.' )!=0)
        THEN
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||' - a.s. '||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||' - a.s. '|| nvl(valoreContatore,'');
            end if; 
        ELSE
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
            end if;
                
        END IF;
   
    END IF;

    IF(annoAccademico is null)
        THEN
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
        end if; 
     
    END IF;
 
    RETURN risultato;
    End Getcontatoredoc;
/

CREATE OR REPLACE FUNCTION getContatoreFasc (systemId INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
annoAccademico VARCHAR(255);
formatoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);


BEGIN

select
valore_oggetto_db, anno,anno_acc,formato_contatore
into valoreContatore, annoContatore,annoAccademico,formatoContatore
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;



if (tipocontatore <> 'T') then
begin

select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc, dpa_el_registri
where
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.descrizione = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore
and
dpa_ass_templates_fasc.id_aoo_rf = dpa_el_registri.system_id;
IF( annoAccademico is not null )
  THEN
  annoContatore := annoAccademico;
        IF (instr(formatoContatore,'a.s.' )!=0)
        THEN
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||' - a.s. '|| nvl(valoreContatore,'');
        ELSE
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
        END IF;
           
END IF;

IF(annoAccademico is null)
    THEN
        risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
END IF;

END;
else

IF( annoAccademico is not null )
   THEN
   annoContatore := annoAccademico;
        IF (instr(formatoContatore,'a.s.' )!=0)
        THEN
            risultato :=    nvl(valoreContatore,'')||' - a.s. '||nvl(annoContatore,'') ;
        ELSE
        risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
        END IF;
END IF;
 
IF(annoAccademico is null)
    THEN
       risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
END IF;


END IF;

RETURN risultato;
END getContatoreFasc;
/

CREATE OR REPLACE FUNCTION getContatoreDoc2 (docNumber INT, tipoContatore CHAR, oggettoCustomId INT)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
annoAccademico VARCHAR(255);
formatoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;



BEGIN


valoreContatore := '';
annoContatore := '';
codiceRegRf := '';


begin

select
valore_oggetto_db, anno,anno_acc,formato_contatore, repertorio
into valoreContatore, annoContatore,annoAccademico,formatoContatore, repertorio
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.system_id= oggettoCustomId;
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1';
--and
--dpa_oggetti_custom.cha_tipo_tar = 'T';
exception when others then null;

end;

IF(repertorio = 1) THEN
BEGIN
risultato := '#CONTATORE_DI_REPERTORIO#';
RETURN risultato;
END;
END IF;

IF(tipoContatore<>'T') THEN
BEGIN
select
dpa_el_registri.var_codice
into codiceRegRf
from
dpa_associazione_templates, dpa_oggetti_custom, dpa_tipo_oggetto, dpa_el_registri
where
dpa_associazione_templates.doc_number = to_char(docNumber)
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
dpa_oggetti_custom.id_tipo_oggetto = dpa_tipo_oggetto.system_id
and
dpa_tipo_oggetto.descrizione = 'Contatore'
and
dpa_oggetti_custom.system_id= oggettoCustomId
--dpa_oggetti_custom.DA_VISUALIZZARE_RICERCA='1'
--and
--dpa_oggetti_custom.cha_tipo_tar = tipoContatore;
and
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
exception when others then null;

END;
END IF;



 IF( annoAccademico is not null )
    
        THEN
        annoContatore := annoAccademico;
        
        IF (instr(formatoContatore,'a.s.' )!=0)
        THEN
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||' - a.s. '||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||' - a.s. '|| nvl(valoreContatore,'');
            end if; 
        ELSE
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
            end if;
        
        
        END IF;
            

    END IF;


IF(annoAccademico is null)
        THEN
            if(codiceRegRf is  null)
            then
            risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
            else  
            risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
        end if; 
     
END IF;

RETURN risultato;
End Getcontatoredoc2;
/

CREATE OR REPLACE PROCEDURE AggiornaAnnoAccademico IS

ID_OGG_CUS DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO%TYPE;
id_temp DPA_TIPO_ATTO.SYSTEM_ID%TYPE;
id_ass DPA_ASSOCIAZIONE_TEMPLATES.SYSTEM_ID%TYPE;
anno_accademico VARCHAR2(225);

cursor c_system_id is
SELECT DPA_TIPO_ATTO.SYSTEM_ID FROM DPA_TIPO_ATTO WHERE
UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='COMUNICATO'
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='CIRCOLARE DOCENTI' 
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='CIRCOLARE ATA' 
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='AVVISO AGLI STUDENTI'
OR UPPER(DPA_TIPO_ATTO.VAR_DESC_ATTO) ='COMUNICAZIONE SCUOLA-FAMIGLIA';
BEGIN
/******************************************************************************
  AUTHOR:   Carlo Francesco Fuccia
  NAME:     ProvaAggiornaAnnoAcc
  PURPOSE:  Store per inserire il valore "anno accademico" nei documenti aventi
  un campo di tipo contatore. Serve per unificare la visualizzazione dei vecchi
  contatori annuali con quella dei contatori custom. Tale store agisce solo su
  determinate tipologie di documenti.
  ******************************************************************************/
anno_accademico:='2012/2013';

   OPEN c_system_id;
    LOOP
    
        FETCH c_system_id into id_temp;
            EXIT WHEN c_system_id%NOTFOUND;
                       
            SELECT DISTINCT ID_OGGETTO INTO ID_OGG_CUS FROM DPA_ASSOCIAZIONE_TEMPLATES,DPA_OGGETTI_CUSTOM
            WHERE DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO=DPA_OGGETTI_CUSTOM.SYSTEM_ID
            AND DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE = id_temp AND DPA_OGGETTI_CUSTOM.ID_TIPO_OGGETTO=59 ;
            dbms_output.put_line(ID_OGG_CUS);
            dbms_output.put_line('modificato');
            DECLARE
            cursor c_ass_system_id is
            SELECT DPA_ASSOCIAZIONE_TEMPLATES.SYSTEM_ID FROM DPA_ASSOCIAZIONE_TEMPLATES
            WHERE DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE=id_temp AND DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO=ID_OGG_CUS
            AND DPA_ASSOCIAZIONE_TEMPLATES.DOC_NUMBER IS NOT NULL ;
            
            BEGIN
            
            OPEN c_ass_system_id;
            
                LOOP
                
                FETCH c_ass_system_id into id_ass;
                EXIT WHEN c_ass_system_id%NOTFOUND;
                
                UPDATE DPA_ASSOCIAZIONE_TEMPLATES SET ANNO_ACC =anno_accademico
                WHERE DPA_ASSOCIAZIONE_TEMPLATES.SYSTEM_ID=id_ass;
                commit work;
                dbms_output.put_line(id_ass);
                dbms_output.put_line(anno_accademico);
                                
                
                END LOOP;
                
                CLOSE c_ass_system_id;
                
                EXCEPTION
                 WHEN NO_DATA_FOUND THEN
                   RAISE;
                 WHEN OTHERS THEN
                   -- Consider logging the error and then re-raise
                   RAISE;
                            
            END;
    END LOOP;
   
   CLOSE c_system_id;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       RAISE;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END AggiornaAnnoAccademico;
/

BEGIN

 utl_add_index('3.30','@db_user@','DPA_LOG',
    'INDX_LOG3',null,
    'VAR_COD_AZIONE, CHECK_NOTIFY, ID_AMM',null,null,null,
    'NORMAL', null,null,null     );
END;
/

CREATE OR REPLACE PROCEDURE insert_micro_da_ni
IS
   tmpVar   NUMBER;
/******************************************************************************
   NAME:       insert_micro_da_ni
   PURPOSE:

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        02/08/2013   LucianiLu       1. Created this procedure.

   NOTES:


      inserisce le nuove micro funzioni della Nuova Interfaccia
      in una nuova Funzione (una per ciascuna amm.ne presente sul db)
       NUOVE_MICRO_DA_NI e poi associata tale funzione
      a tutti i ruoli di tutte le amm.ni

******************************************************************************/
BEGIN
   DECLARE
      CURSOR amm
      IS
         SELECT system_id
           FROM dpa_amministra
          ;

      amm_rec      amm%ROWTYPE;

      tmpVar       NUMBER := 0;
      idtipofunz   NUMBER := 0;
   BEGIN
      OPEN amm;

      LOOP
         BEGIN
            FETCH amm INTO amm_rec;

            EXIT WHEN amm%NOTFOUND;

            SELECT seq.NEXTVAL INTO idtipofunz FROM DUAL;


update dpa_anagrafica_funzioni d set disabled='N' where D.COD_FUNZIONE= 'DO_TAB_ARCHIVIO';


update dpa_anagrafica_funzioni d set disabled='N' where D.COD_FUNZIONE= 'CERCA_DOC_ADV';


            INSERT INTO dpa_tipo_funzione (system_id,
                                           var_cod_tipo,
                                           var_desc_tipo_fun,
                                           cha_vis,
                                           id_amm)
                 VALUES (idtipofunz,
                         'NUOVE_MICRO_DA_NI',
                         'MICRO Funzioni della Nuova Interfaccia',
                         '1',
                         amm_rec.system_id);


            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (seq.NEXTVAL,
                         amm_rec.system_id,
                         'CERCA_DOC_ADV',
                         'Ricerca documenti avanzata selezionata di default',
                         NULL,
                         NULL,
                         NULL,
                         NULL,
                         idtipofunz);

         

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (seq.NEXTVAL,
                         amm_rec.system_id,
                         'DO_ADL_ROLE',
                         'Abilita adl ruolo',
                         NULL,
                         NULL,
                         NULL,
                         NULL,
                         idtipofunz);

            

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (seq.NEXTVAL,
                         amm_rec.system_id,
                         'DO_TAB_ARCHIVIO',
                         'Abilita il tab archivio nelle ricerche fascicoli',
                         NULL,
                         NULL,
                         NULL,
                         NULL,
                         idtipofunz);

            

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (seq.NEXTVAL,
                         amm_rec.system_id,
                         'FASC_STRUCTURE',
                         'Abilita il tab struttura nel fascicolo',
                         NULL,
                         NULL,
                         NULL,
                         NULL,
                         idtipofunz);

        

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'GEST_REP_SPED',
                           'Abilita il sottomenu'' Report Spedizione del menu'' Gestione',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);

        
            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'FASC_INS_DOC',
                           'Abilita la possibilità di inserire documenti nei sottofascicoli (necessaria per il Drag&Drop)',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);

         

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'DO_DEL_DOC_FASC',
                           'Abilita la possibilità di rimuovere documenti dai sottofascicoli (necessaria per il Drag&Drop)',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);

            --UserManager.IsAuthorizedFunctions("FASC_INS_DOC")
          

            --   && UserManager.IsAuthorizedFunctions("DO_DEL_DOC_FASC")


            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'FASC_NEW_FOLDER',
                           'Abilita la possibilità di creare sottofascicoli (necessaria per il Drag&Drop)',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);


          
            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'FASC_MOD_FOLDER',
                           'Abilita la possibilità di modificare sottofascicoli (necessaria per il Drag&Drop)',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);

           

            INSERT INTO dpa_funzioni (SYSTEM_ID,
                                      ID_AMM,
                                      COD_FUNZIONE,
                                      VAR_DESC_FUNZIONE,
                                      ID_PARENT,
                                      CHA_TIPO_FUNZ,
                                      ID_PESO,
                                      CHA_FLAG_PARENT,
                                      ID_TIPO_FUNZIONE)
                 VALUES (
                           seq.NEXTVAL,
                           amm_rec.system_id,
                           'FASC_DEL_FOLDER',
                           'Abilita la possibilità di rimuovere sottofascicoli (necessaria per il Drag&Drop)',
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           idtipofunz);

            --    && UserManager.IsAuthorizedFunctions("FASC_NEW_FOLDER")
            --   && UserManager.IsAuthorizedFunctions("FASC_MOD_FOLDER")
            --  && UserManager.IsAuthorizedFunctions("FASC_DEL_FOLDER")



            COMMIT;

           INSERT INTO dpa_tipo_f_ruolo (SYSTEM_ID, ID_TIPO_FUNZ, ID_RUOLO_IN_UO)
   SELECT seq.NEXTVAL, idtipofunz, system_id
     FROM dpa_corr_globali a
    WHERE     id_amm = amm_rec.system_id
          AND cha_tipo_ie = 'I'
          AND cha_tipo_urp = 'R'
          AND dta_fine IS NULL
          AND id_gruppo IN (SELECT groups_system_id
                              FROM peoplegroups
                             WHERE dta_fine IS NULL)
          AND NOT EXISTS
                     (SELECT 'x'
                        FROM dpa_tipo_f_ruolo fr
                       WHERE     FR.ID_RUOLO_IN_UO = A.SYSTEM_ID
                             AND FR.ID_TIPO_FUNZ = idtipofunz);

            COMMIT;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               RAISE;
            WHEN OTHERS
            THEN
               -- Consider logging the error and then re-raise
               RAISE;
         END;
      END LOOP;

      CLOSE amm;
   END;
END insert_micro_da_ni;
/

/*** 
BEGIN
declare cntcol int;

begin
select count(*) into cntcol from dpa_tipo_funzione  
	where var_cod_tipo='NUOVE_MICRO_DA_NI';

if cntcol = 0 then 
	execute immediate 
	'INSERT_MICRO_DA_NI;';
end if;

end;
END;
/
****/ 


CREATE OR REPLACE PROCEDURE ENABLE_ALL_LOGS
IS
  id_log        NUMBER(10,0);
  notific       CHAR(1 char);
  conf          CHAR(1 char);
  notify        VARCHAR2(3 char);
  idAmm         NUMBER(10,0);
  -- cursor to all logs

  CURSOR LIST_LOGS
  IS
    SELECT a.SYSTEM_ID, a.NOTIFICATION, a.CONFIGURABLE, a.ID_AMM
    FROM DPA_ANAGRAFICA_LOG a;
BEGIN
  -- delete all logs active
  
  DELETE
  FROM DPA_LOG_ATTIVATI;
  
     
  OPEN LIST_LOGS;
  LOOP
    FETCH LIST_LOGS INTO   id_log, notific, conf, idAmm;
    EXIT
        WHEN LIST_LOGS%NOTFOUND ;
        
    IF notific = '0'
        THEN
            notify := 'NN';
    ELSIF notific = '1' and conf = '0'
        THEN
            notify:= 'OBB';
    ELSE
        notify:= 'CON';
    END IF;
    
    
    IF idAmm is not null
        THEN
            INSERT
            INTO DPA_LOG_ATTIVATI VALUES
            (
                id_log,
                idAmm,
                notify
            );
    ELSE
            INSERT INTO DPA_LOG_ATTIVATI
            (
                SYSTEM_ID_ANAGRAFICA,
                ID_AMM,
                NOTIFY
            )
            SELECT id_log, am.system_id, notify
            FROM DPA_AMMINISTRA am;
    END IF;
    
  END LOOP;
  CLOSE LIST_LOGS;
  
  COMMIT;
END;
/

begin
Insert into DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
select max(system_id) +1 , sysdate,  '3.30' from Dpa_Docspa;
end;
/

			   
			   