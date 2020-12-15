-- TODOLIST unificata 
begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TODOLIST' and column_name='DTA_VISTA';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TODOLIST ADD (DTA_VISTA DATE DEFAULT TO_DATE(''01/01/1753'',''dd/mm/yyyy'') NOT NULL)';
    end if;
    end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='DA_VISUALIZZARE_RICERCA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD DA_VISUALIZZARE_RICERCA NUMBER';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='DA_VISUALIZZARE_RICERCA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD DA_VISUALIZZARE_RICERCA NUMBER';
		end if;
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_SOLO_MAIL_PEC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_SOLO_MAIL_PEC  VARCHAR2(1)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_MAIL_ELABORATE';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_MAIL_ELABORATE' and column_name='ID_REGISTRO';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_MAIL_ELABORATE ADD ID_REGISTRO  NUMBER(10)';
        end if;
    end if;
    end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_SERVER_IMAP';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_SERVER_IMAP VARCHAR2(128)';
		end if;
	end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='NUM_PORTA_IMAP';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD NUM_PORTA_IMAP NUMBER(10)';
		end if;
	end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_TIPO_CONNESSIONE';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_TIPO_CONNESSIONE VARCHAR2(10)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_INBOX_IMAP';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_INBOX_IMAP VARCHAR2(20)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_BOX_MAIL_ELABORATE';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_BOX_MAIL_ELABORATE VARCHAR2(50)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_MAIL_NON_ELABORATE';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD VAR_MAIL_NON_ELABORATE VARCHAR2(50)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_EL_REGISTRI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='CHA_IMAP_SSL';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD CHA_IMAP_SSL VARCHAR2(1)';
		end if;
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROFILE' and column_name='CHA_FIRMATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROFILE ADD (CHA_FIRMATO VARCHAR2(1) DEFAULT 0 NOT NULL)';
	end if;
	end;
end;
/

CREATE OR REPLACE FUNCTION getContatoreDoc (docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
 
BEGIN

valoreContatore := '';
annoContatore := '';
codiceRegRf := '';

select 
valore_oggetto_db, anno 
into valoreContatore, annoContatore
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
dpa_oggetti_custom.cha_tipo_tar = tipoContatore;

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
dpa_oggetti_custom.cha_tipo_tar = tipoContatore
and 
dpa_associazione_templates.id_aoo_rf = dpa_el_registri.system_id;
END;
END IF;

risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');

RETURN risultato;
END getContatoreDoc;
/

CREATE OR REPLACE FUNCTION getContatoreDocOrdinamento (docNumber INT, tipoContatore CHAR)
RETURN int IS risultato VARCHAR(255);

BEGIN

select 
valore_oggetto_db 
into risultato
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
dpa_oggetti_custom.cha_tipo_tar = tipoContatore;

RETURN to_number(risultato);

END getContatoreDocOrdinamento;
/

CREATE OR REPLACE FUNCTION getContatoreFasc (systemId INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
 
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
end;
end if;


risultato :=  codiceRegRf ||'-'|| annoContatore ||'-'|| valoreContatore;

RETURN risultato;
END getContatoreFasc;
/

CREATE OR REPLACE FUNCTION getContatoreFascContatore (systemId INT, tipoContatore CHAR)
RETURN int IS risultato VARCHAR(255);

BEGIN

select 
dpa_ass_templates_fasc.valore_oggetto_db
into risultato
from 
dpa_ass_templates_fasc, dpa_oggetti_custom_fasc, dpa_tipo_oggetto_fasc
where 
dpa_ass_templates_fasc.id_project = to_char(systemId)
and
dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id
and
dpa_oggetti_custom_fasc.id_tipo_oggetto = dpa_tipo_oggetto_fasc.system_id
and
dpa_tipo_oggetto_fasc.DESCRIZIONE = 'Contatore'
and
dpa_oggetti_custom_fasc.cha_tipo_tar = tipoContatore;
RETURN to_number(risultato);

END getContatoreFascContatore;
/

CREATE OR REPLACE FUNCTION vardescribe 
(sysid INT, typetable VARCHAR)
   RETURN VARCHAR2
IS
   risultato   VARCHAR2 (8000);
   tmpvar      VARCHAR2 (8000);
   tipo        CHAR;
   num_proto   NUMBER;
   doc_number  NUMBER;
BEGIN
   BEGIN
      tmpvar := NULL;
 
--MAIN
      IF (typetable = 'PEOPLENAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_people = sysid AND cha_tipo_urp = 'P' AND cha_tipo_ie = 'I';
      END IF;
 
      IF (typetable = 'GROUPNAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE system_id = sysid AND cha_tipo_urp = 'R';
      END IF;
 
      IF (typetable = 'DESC_RUOLO')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_gruppo = sysid AND cha_tipo_urp = 'R';
      END IF;
 
      IF (typetable = 'RAGIONETRASM')
      THEN
         SELECT var_desc_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'TIPO_RAGIONE')
      THEN
         SELECT cha_tipo_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'DATADOC')
      THEN
         BEGIN
            SELECT cha_tipo_proto, NVL (num_proto, 0)
              INTO tipo, num_proto
              FROM PROFILE
             WHERE system_id = sysid;
 
            IF (    tipo IS NOT NULL
                AND (tipo IN ('A', 'P', 'I') AND num_proto != 0)
               )
            THEN
               SELECT TO_CHAR (dta_proto, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            ELSE
               SELECT TO_CHAR (creation_date, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;
 
      IF (typetable = 'CHA_TIPO_PROTO')
      THEN
         SELECT cha_tipo_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'NUMPROTO')
      THEN
         SELECT num_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'CODFASC')
      THEN
         SELECT var_codice
           INTO risultato
           FROM project
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'DTA_CREAZ')
      THEN
         SELECT TO_CHAR (dta_creazione, 'yyyy')
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'NUM_FASC')
      THEN
         SELECT num_fascicolo
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'DESC_OGGETTO')
      THEN
         SELECT var_prof_oggetto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
 
      IF (typetable = 'DESC_FASC')
      THEN
         BEGIN
            SELECT description
              INTO risultato
              FROM project
             WHERE system_id = sysid;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '';
         END;
      END IF;
 
      IF (typetable = 'PROF_IDREG')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_registro
                    INTO risultato
                    FROM PROFILE
                   WHERE system_id = sysid;
 
                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;
 
      IF (typetable = 'ID_GRUPPO')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_gruppo
                    INTO risultato
                    FROM dpa_corr_globali
                   WHERE system_id = sysid;
 
                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;
 
      IF (typetable = 'SEGNATURA_DOCNUMBER')
      THEN
         BEGIN
            SELECT var_segnatura
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;
 
            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;
      
      IF (typetable = 'SEGNATURA_CODFASC')
      THEN
         BEGIN
            SELECT num_proto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;
 
            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;
 
      IF (typetable = 'OGGETTO_MITTENTE')
      THEN
         BEGIN
-- OGGETTO
            SELECT var_prof_oggetto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;
 
--MITTENTE
            BEGIN
               SELECT var_desc_corr
                 INTO tmpvar
                 FROM (SELECT var_desc_corr
                         FROM dpa_corr_globali a, dpa_doc_arrivo_par b
                        WHERE b.id_mitt_dest = a.system_id
                          AND b.cha_tipo_mitt_dest = 'M'
                          AND b.id_profile = sysid)
                WHERE ROWNUM = 1;
            EXCEPTION
               WHEN NO_DATA_FOUND
               THEN
                  tmpvar := '';
            END;
 
            IF (tmpvar IS NOT NULL)
            THEN
               risultato := risultato || '@@' || tmpvar;
            END IF;
         END;
      END IF;
 
      IF (typetable = 'PROFILE_CHA_IMG')
      THEN
         SELECT getchaimg (docnumber)
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'PROFILE_CHA_FIRMATO')
      THEN
         SELECT CHA_FIRMATO
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
      
      --IF(typetable='EXT_FILE_ACQUISITO')
      --THEN      
      --SELECT DOCNUMBER into doc_number from PROFILE WHERE SYSTEM_ID=sysid;
      
      --SELECT trim(UPPER(COMPONENTS.EXT))
       -- INTO risultato
       -- FROM COMPONENTS
       -- WHERE 
       -- COMPONENTS.VERSION_ID=(select max(versions.version_id)  from versions, components
       -- where versions.version_id=components.version_id AND versions.docnumber=doc_number);    
     -- END IF;
--ENDMAIN
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         NULL;
      WHEN OTHERS
      THEN
         RAISE;
   END;
 
   RETURN risultato;
END vardescribe;
/

CREATE OR REPLACE PROCEDURE @db_user.SPsetDataVista(
p_idPeople IN NUMBER,
p_idOggetto IN NUMBER,
p_idGruppo IN NUMBER,
p_tipoOggetto IN CHAR,
p_resultValue OUT number
) IS

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

p_cha_tipo_trasm CHAR(1) := NULL;
p_chaTipoDest NUMBER;


BEGIN
p_resultValue:=0;


DECLARE

CURSOR cursorTrasmSingolaDocumento IS

SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = p_idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = p_idPeople))
AND a.ID_PROFILE = p_idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID;

CURSOR cursorTrasmSingolaFascicolo IS
SELECT B.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione, b.cha_tipo_dest
FROM dpa_trasmissione a, dpa_trasm_singola b,  DPA_RAGIONE_TRASM c
WHERE a.dta_invio is not null and a.system_id = b.id_trasmissione and (b.id_corr_globale =
(select system_id from dpa_corr_globali where id_gruppo = p_idGruppo)
OR b.id_corr_globale =
(SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_people = p_idPeople))
AND a.ID_PROJECT = p_idOggetto and
b.ID_RAGIONE = c.SYSTEM_ID;

BEGIN


IF(p_tipoOggetto='D') THEN

FOR currentTrasmSingola IN cursorTrasmSingolaDocumento
LOOP
BEGIN

IF (currentTrasmSingola.cha_tipo_ragione = 'N' OR currentTrasmSingola.cha_tipo_ragione = 'I') then
-- SE è una trasmissione senza workFlow
begin

-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

update dpa_todolist 
set DTA_VISTA = SYSDATE 
where 
ID_TRASM_SINGOLA = currentTrasmSingola.SYSTEM_ID 
and ID_PEOPLE_DEST = p_idPeople --and ID_RUOLO_DEST in (p_idGruppo,0) 
and id_profile = p_idOggetto;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

begin

IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') then
-- se è una trasmissione  di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
begin
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
AND DPA_TRASM_UTENTE.ID_PEOPLE != p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

end if;
end;
ELSE
-- la ragione di trasmissione prevede workflow
BEGIN

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DPA_TRASM_UTENTE.DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

update dpa_todolist 
set DTA_VISTA = SYSDATE 
where 
ID_TRASM_SINGOLA = currentTrasmSingola.SYSTEM_ID and ID_PEOPLE_DEST = p_idPeople --and ID_RUOLO_DEST in (p_idGruppo,0) 
and id_profile = p_idOggetto;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
END;

END IF;

END;

END LOOP;
END IF;

IF(p_tipoOggetto='F') THEN

FOR currentTrasmSingola IN cursorTrasmSingolaFascicolo
LOOP
BEGIN

IF (currentTrasmSingola.cha_tipo_ragione = 'N' OR currentTrasmSingola.cha_tipo_ragione = 'I') then
-- SE è una trasmissione senza workFlow
begin

-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END),
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

update dpa_todolist 
set DTA_VISTA = SYSDATE 
where 
ID_TRASM_SINGOLA = currentTrasmSingola.SYSTEM_ID and ID_PEOPLE_DEST = p_idPeople --and ID_RUOLO_DEST in (p_idGruppo,0) 
and id_project = p_idOggetto;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;
begin
IF (currentTrasmSingola.cha_tipo_trasm = 'S' and currentTrasmSingola.cha_tipo_dest= 'R') then
-- se è una trasmissione di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
begin
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList

UPDATE DPA_TRASM_UTENTE SET
DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = '0'
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
AND DPA_TRASM_UTENTE.ID_PEOPLE != p_idPeople;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
end;

end if;
end;
ELSE
-- se la ragione di trasmissione prevede workflow
BEGIN

UPDATE DPA_TRASM_UTENTE
SET DPA_TRASM_UTENTE.CHA_VISTA = '1',
DPA_TRASM_UTENTE.CHA_IN_TODOLIST = (CASE WHEN DTA_ACCETTATA IS NULL THEN '1' ELSE '0' END),
DPA_TRASM_UTENTE.DTA_VISTA = (CASE WHEN DTA_VISTA IS NULL THEN  SYSDATE ELSE DTA_VISTA END)
WHERE
DPA_TRASM_UTENTE.DTA_VISTA IS NULL
AND id_trasm_singola = (currentTrasmSingola.SYSTEM_ID)
and DPA_TRASM_UTENTE.ID_PEOPLE = p_idPeople;

update dpa_todolist 
set DTA_VISTA = SYSDATE 
where 
ID_TRASM_SINGOLA = currentTrasmSingola.SYSTEM_ID and ID_PEOPLE_DEST = p_idPeople --and ID_RUOLO_DEST in (p_idGruppo,0) 
and id_project = p_idOggetto;

EXCEPTION
WHEN OTHERS THEN  p_resultValue:=1;
RETURN;
END;

END IF;

END;

END LOOP;

END IF;

END;
END SPsetDataVista;
/

CREATE OR REPLACE PROCEDURE @db_user.spsetdatavistasmistamento (
   p_idpeople         IN       NUMBER,
   p_idoggetto        IN       NUMBER,
   p_idgruppo         IN       NUMBER,
   p_tipooggetto      IN       CHAR,
   p_idtrasmissione   IN       NUMBER,
   p_iddelegato       IN       NUMBER,
   p_resultvalue      OUT      NUMBER
)
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
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.system_id = p_idtrasmissione
            AND a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ¿ una trasmissione senza workFlow
                  if (p_iddelegato = 0) then
                      BEGIN
                        -- nella trasmissione utente relativa all'utente che sta vedendo il documento setto la data di vista
                         UPDATE dpa_trasm_utente
                            SET dpa_trasm_utente.cha_vista = '1',
                                dpa_trasm_utente.dta_vista =
                                   (CASE
                                       WHEN dta_vista IS NULL
                                          THEN SYSDATE
                                       ELSE dta_vista
                                    END
                                   ),
                                dpa_trasm_utente.cha_in_todolist = '0'
                          WHERE dpa_trasm_utente.dta_vista IS NULL
                            AND id_trasm_singola =
                                                  (currenttrasmsingola.system_id
                                                  )
                            AND dpa_trasm_utente.id_people = p_idpeople;

                      EXCEPTION
                         WHEN OTHERS
                         THEN
                            p_resultvalue := 1;
                            RETURN;
                      END;
                  else
                      --in caso di delega
                      BEGIN
    -- nella trasmissione utente relativa all'utente che sta vedendo il documento
    -- setto la data di vista
                         UPDATE dpa_trasm_utente
                            SET dpa_trasm_utente.cha_vista = '1',
                                dpa_trasm_utente.cha_vista_delegato = '1',
                                dpa_trasm_utente.id_people_delegato = p_iddelegato,
                                dpa_trasm_utente.dta_vista =
                                   (CASE
                                       WHEN dta_vista IS NULL
                                          THEN SYSDATE
                                       ELSE dta_vista
                                    END
                                   ),
                                dpa_trasm_utente.cha_in_todolist = '0'
                          WHERE dpa_trasm_utente.dta_vista IS NULL
                            AND id_trasm_singola =
                                                  (currenttrasmsingola.system_id
                                                  )
                            AND dpa_trasm_utente.id_people = p_idpeople;
                      EXCEPTION
                         WHEN OTHERS
                         THEN
                            p_resultvalue := 1;
                            RETURN;
                      END;
                  end if;
                    
                  -- Impostazione data vista nella trasmissione in todolist
                  begin
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  
                      
                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                        if (p_iddelegato = 0) then
                            -- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
                            -- anche le trasmissioni singole relative agli altri utenti del ruolo
                            BEGIN
                                -- nelle trasmissioni utente relative agli altri utenti del ruolo
                                -- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                               UPDATE dpa_trasm_utente
                                  SET dpa_trasm_utente.cha_vista = '1',
                                      dpa_trasm_utente.cha_in_todolist = '0'
                                WHERE dpa_trasm_utente.dta_vista IS NULL
                                  AND id_trasm_singola =
                                                  (currenttrasmsingola.system_id
                                                  )
                                  AND dpa_trasm_utente.id_people != p_idpeople;
                            EXCEPTION
                               WHEN OTHERS
                               THEN
                                  p_resultvalue := 1;
                                  RETURN;
                            END;
                        else
                           BEGIN
                               UPDATE dpa_trasm_utente
                                  SET dpa_trasm_utente.cha_vista = '1',
                                      dpa_trasm_utente.cha_vista_delegato = '1',
                                      dpa_trasm_utente.id_people_delegato = p_iddelegato,
                                      dpa_trasm_utente.cha_in_todolist = '0'
                                WHERE dpa_trasm_utente.dta_vista IS NULL
                                  AND id_trasm_singola =
                                                  (currenttrasmsingola.system_id
                                                  )
                                  AND dpa_trasm_utente.id_people != p_idpeople;
                            EXCEPTION
                               WHEN OTHERS
                               THEN
                                  p_resultvalue := 1;
                                  RETURN; 
                            END;                       
                        end if;
                            
                     END IF;
                  END;
                      
               ELSE
               
                  BEGIN
                     -- la ragione di trasmissione prevede workflow
                     if (p_iddelegato = 0) then
                     
                         BEGIN
                            UPDATE dpa_trasm_utente
                               SET dpa_trasm_utente.cha_vista = '1',
                                   dpa_trasm_utente.dta_vista =
                                      (CASE
                                          WHEN dta_vista IS NULL
                                             THEN SYSDATE
                                          ELSE dta_vista
                                       END
                                      ),
                                   dta_accettata =
                                      (CASE
                                          WHEN dta_accettata IS NULL
                                             THEN SYSDATE
                                          ELSE dta_accettata
                                       END
                                      ),
                                   cha_accettata = '1',
                                   cha_valida = '0'--,
                                   --cha_in_todolist = '0'
                             WHERE dpa_trasm_utente.dta_vista IS NULL
                               AND id_trasm_singola =
                                                     currenttrasmsingola.system_id
                               AND dpa_trasm_utente.id_people = p_idpeople;

                            UPDATE dpa_todolist
                               SET dta_vista = SYSDATE
                             WHERE id_trasm_singola =
                                                     currenttrasmsingola.system_id
                               AND id_people_dest = p_idpeople;
                         EXCEPTION
                            WHEN OTHERS
                            THEN
                               p_resultvalue := 1;
                               RETURN;
                         END;
                     else
                         BEGIN
                            UPDATE dpa_trasm_utente
                               SET dpa_trasm_utente.cha_vista = '1',
                                dpa_trasm_utente.cha_vista_delegato = '1',
                                dpa_trasm_utente.id_people_delegato = p_iddelegato,
                                   dpa_trasm_utente.dta_vista =
                                      (CASE
                                          WHEN dta_vista IS NULL
                                             THEN SYSDATE
                                          ELSE dta_vista
                                       END
                                      ),
                                   dta_accettata =
                                      (CASE
                                          WHEN dta_accettata IS NULL
                                             THEN SYSDATE
                                          ELSE dta_accettata
                                       END
                                      ),
                                   cha_accettata = '1',
                                   cha_accettata_delegato = '1',
                                   cha_valida = '0'--,
                                   --cha_in_todolist = '0'
                             WHERE dpa_trasm_utente.dta_vista IS NULL
                               AND id_trasm_singola =
                                                     currenttrasmsingola.system_id
                               AND dpa_trasm_utente.id_people = p_idpeople;

                            UPDATE dpa_todolist
                               SET dta_vista = SYSDATE
                             WHERE id_trasm_singola =
                                                     currenttrasmsingola.system_id
                               AND id_people_dest = p_idpeople;
                         EXCEPTION
                            WHEN OTHERS
                            THEN
                               p_resultvalue := 1;
                               RETURN;
                         END;
                     end if;
                         
                     BEGIN
                         -- Rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
                         UPDATE     dpa_trasm_utente
                         SET        cha_in_todolist = '0'
                         WHERE      id_trasm_singola = currenttrasmsingola.system_id 
                                AND NOT  dpa_trasm_utente.dta_vista IS NULL
                                AND (cha_accettata = '1' OR cha_rifiutata = '1');
                                --AND dpa_trasm_utente.id_people = p_idpeople;

                         UPDATE dpa_todolist
                            SET dta_vista = SYSDATE
                          WHERE id_trasm_singola = currenttrasmsingola.system_id
                            AND id_people_dest = p_idpeople
                            AND id_profile = p_idoggetto;
                      EXCEPTION
                         WHEN OTHERS
                         THEN
                            p_resultvalue := 1;
                            RETURN;
                      END;
                         

                     BEGIN
-- se la trasm ¿ con WorkFlow ed ¿ di tipo UNO e il dest ¿ Ruolo allora levo la validit¿ della
-- trasmissione a tutti gli altri utenti del ruolo
                        IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                            AND currenttrasmsingola.cha_tipo_dest = 'R'
                           )
                        THEN
                           BEGIN
                              UPDATE dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_valida = '0',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE
-- DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                     id_trasm_singola =
                                                 currenttrasmsingola.system_id
                                 AND dpa_trasm_utente.id_people != p_idpeople;
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavistasmistamento;
/

CREATE OR REPLACE TRIGGER @db_user.TR_UPDATE_DPA_TODOLIST
AFTER UPDATE
OF CHA_IN_TODOLIST
ON @db_user.DPA_TRASM_UTENTE 
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN
if( :NEW.CHA_IN_TODOLIST='0')
THEN
DELETE /*+ ALL_ROWS */ DPA_TODOLIST WHERE ID_TRASM_UTENTE = :NEW.SYSTEM_ID;
END IF;
END;
/

CREATE OR REPLACE TRIGGER @db_user.TR_INSERT_DPA_TODOLIST
AFTER UPDATE
OF DTA_INVIO
ON @db_user.DPA_TRASMISSIONE 
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN

INSERT INTO @db_user.DPA_TODOLIST
SELECT :NEW.system_id,
dtu.id_trasm_singola,
dtu.system_id,
:NEW.dta_invio,
:NEW.id_people,
:NEW.id_ruolo_in_uo,
dtu.id_people,
dts.id_ragione,
:NEW.var_note_generali,
dts.var_note_sing,
dts.dta_scadenza,
:NEW.id_profile,
:NEW.id_project,
TO_NUMBER(Vardescribe(dts.id_corr_globale,'ID_GRUPPO')) AS id_ruolo_dest,
TO_NUMBER(Vardescribe(:NEW.id_profile,'PROF_IDREG')) AS id_registro,
DTS.CHA_TIPO_DEST,
CASE WHEN dtu.DTA_VISTA IS NULL THEN TO_DATE('01/01/1753','dd/mm/yyyy') ELSE dtu.DTA_VISTA END
FROM DPA_TRASM_SINGOLA dts,DPA_TRASM_UTENTE dtu
WHERE dtu.id_trasm_singola = dts.system_id AND dts.id_trasmissione = :NEW.system_id AND dtu.cha_in_todolist = 1;
END;
/

--modelli trasmissione
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_CORR_GLOBALI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='CHA_SEGRETARIO';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_CORR_GLOBALI ADD CHA_SEGRETARIO VARCHAR2(1 BYTE)';
		end if;
	end if;
	end;
end;
/

CREATE OR REPLACE PROCEDURE sp_get_ruolo_resp_uo_from_uo (
   p_id_uo        IN       NUMBER,
   p_tipo_ruolo            CHAR,
   p_id_corr               number,
   p_result       OUT      NUMBER
)
AS
BEGIN
   DECLARE
      v_isidparentnull        NUMBER;
      v_noruoloresponsabile   NUMBER;
      v_numeroresponsabili    NUMBER;
      v_idparent              NUMBER;
      v_id_uo                 NUMBER;
      v_id_uo_appo            NUMBER;
   BEGIN
      v_isidparentnull := 0;
      v_noruoloresponsabile := 0;
      v_numeroresponsabili := 0;
      v_idparent := 0;
      v_id_uo := p_id_uo;

      WHILE (v_isidparentnull = 0 AND v_noruoloresponsabile = 0)
      LOOP
         IF (p_tipo_ruolo = 'R')
         THEN
            SELECT COUNT (*)
              INTO v_numeroresponsabili
              FROM dpa_corr_globali
             WHERE id_uo = v_id_uo
               AND cha_tipo_urp = 'R'
               AND cha_responsabile = 1;

            IF (v_numeroresponsabili > 0)
            THEN
               SELECT system_id
                 INTO v_id_uo_appo
                 FROM dpa_corr_globali
                WHERE id_uo = v_id_uo
                  AND cha_tipo_urp = 'R'
                  AND cha_responsabile = 1;

		IF(v_id_uo_appo != p_id_corr)
		then
	            p_result:=v_id_uo_appo;
		    v_noruoloresponsabile := 1;
		else
		    SELECT id_parent
                    INTO v_idparent
                    FROM dpa_corr_globali
                    WHERE system_id = v_id_uo;
		     IF (v_idparent > 0)
                     THEN
                         v_id_uo := v_idparent;
                     ELSE
                         p_result := 0;
                         v_isidparentnull := 1;
                     END IF;
		END IF;    
            ELSE
               SELECT id_parent
                 INTO v_idparent
                 FROM dpa_corr_globali
                WHERE system_id = v_id_uo;

               IF (v_idparent > 0)
               THEN
                  v_id_uo := v_idparent;
               ELSE
                  p_result := 0;
                  v_isidparentnull := 1;
               END IF;
            END IF;
         ELSE
            SELECT COUNT (*)
              INTO v_numeroresponsabili
              FROM dpa_corr_globali
             WHERE id_uo = v_id_uo AND cha_tipo_urp = 'R'
                   AND cha_segretario = 1;

            IF (v_numeroresponsabili > 0)
            THEN
               SELECT system_id
                 INTO v_id_uo_appo
                 FROM dpa_corr_globali
                WHERE id_uo = v_id_uo
                  AND cha_tipo_urp = 'R'
                  AND cha_segretario = 1;

               IF(v_id_uo_appo != p_id_corr)
        then
                p_result:=v_id_uo_appo;
            v_noruoloresponsabile := 1;
        else
            SELECT id_parent
                    INTO v_idparent
                    FROM dpa_corr_globali
                    WHERE system_id = v_id_uo;
             IF (v_idparent > 0)
                     THEN
                         v_id_uo := v_idparent;
                     ELSE
                         p_result := 0;
                         v_isidparentnull := 1;
                     END IF;
        END IF;   
            ELSE
               SELECT id_parent
                 INTO v_idparent
                 FROM dpa_corr_globali
                WHERE system_id = v_id_uo;

               IF (v_idparent > 0)
               THEN
                  v_id_uo := v_idparent;
               ELSE
                  p_result := 0;
                  v_isidparentnull := 1;
               END IF;
            END IF;
         END IF;
      END LOOP;
   END;
END;
/

--ricerca estensione file acquisiti
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='COMPONENTS';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='COMPONENTS' and column_name='EXT';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE COMPONENTS ADD EXT CHAR(7) DEFAULT 0';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='PROFILE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='PROFILE' and column_name='CHA_FIRMATO';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE PROFILE ADD CHA_FIRMATO VARCHAR2(1 BYTE) DEFAULT 0';
		end if;
	end if;
	end;
end;
/



CREATE OR REPLACE FUNCTION getchaimg (docnum NUMBER)
   RETURN VARCHAR
IS
tmpVar varchar(7);
BEGIN
declare
v_path varchar(128);
vMaxIdGenerica number;
begin

begin
SELECT
MAX (v1.version_id)
INTO vMaxIdGenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docNum
AND v1.version_id = c.version_id;
EXCEPTION
WHEN OTHERS THEN
vMaxIdGenerica:=0;
end;

begin
select ext into v_path from components where docnumber=docNum and version_id=vMaxIdGenerica;

EXCEPTION
WHEN OTHERS THEN
tmpVar:='0';
end;

if(v_path <> '' OR v_path is  not null)
then tmpVar:= trim(v_path);
else tmpVar:='0';
end if;

end;
RETURN tmpVar;
END getChaImg;
/

CREATE OR REPLACE PROCEDURE putFile 
(
p_versionId int,
p_filePath nvarchar2,
p_fileSize int,
p_printThumb nvarchar2,
p_iscartaceo smallint,
p_estensione varchar,
p_isFirmato char 
)
IS retValue NUMBER;

docNum int := 0;

BEGIN
   retValue := 0;
   
    select docnumber into docNum from versions where version_id = p_versionId;
        
    update     versions
    set     subversion = 'A',
          cartaceo = p_iscartaceo
    where     version_id = p_versionId;
   
    retValue := SQL%ROWCOUNT;
   
   
   if (retValue > 0) then
     update components
     set     path = p_filePath,
             file_size = p_fileSize,
             var_impronta = p_printThumb,
             ext =p_estensione
     where     version_id = p_versionId;
        
    retValue := SQL%ROWCOUNT;
   end if;
      
   if (retValue > 0) then
   
    if(p_isFirmato='1') THEN
       update     profile
        set     cha_img = '1', CHA_FIRMATO = '1'
        where    docnumber = docNum;
    ELSE
        update     profile
        set     cha_img = '1'
        where    docnumber = docNum;    
    END IF;
     
    retValue := SQL%ROWCOUNT;
   end if;
   
END putFile;
/

/

-- PROTOCOLLO TITOLARIO

--MODIFICHE PROJECT
--*****************
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV2';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_TITOLARIO';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV2 TO ET_TITOLARIO';
			end if;   
       end if;   
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV3';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO1';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV3 TO ET_LIVELLO1';
			end if;   
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV4';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO2';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV4 TO ET_LIVELLO2';
			end if;   
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV5';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO3';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV5 TO ET_LIVELLO3';
			end if;               
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV6';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO4';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV6 TO ET_LIVELLO4';
			end if;                           
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV7';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO5';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV7 TO ET_LIVELLO5';
			end if;                           
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='VAR_COD_LIV8';
        if (cnt != 0) then            
            select count(*) into cnt from cols where table_name='PROJECT' AND column_name='ET_LIVELLO6';
			if (cnt = 0) then            
				execute immediate 'ALTER TABLE PROJECT RENAME COLUMN VAR_COD_LIV8 TO ET_LIVELLO6';
			end if;                                      
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='CHA_BLOCCA_FIGLI';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD CHA_BLOCCA_FIGLI VARCHAR(2)';
        execute immediate 'UPDATE PROJECT SET CHA_BLOCCA_FIGLI = ''NO''';          
        end if; 
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='CHA_CONTA_PROT_TIT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD CHA_CONTA_PROT_TIT VARCHAR(2)';
        execute immediate 'UPDATE PROJECT SET CHA_CONTA_PROT_TIT = ''NO''';  
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='NUM_PROT_TIT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD NUM_PROT_TIT INT';
        end if;
 end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT' AND column_name='MAX_LIV_TIT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD MAX_LIV_TIT INT';
        execute immediate 'UPDATE PROJECT SET MAX_LIV_TIT = 6 WHERE ID_PARENT = 0 AND CHA_TIPO_PROJ = ''T'''; 
        end if;
    end;
end;
/

--MODIFICHE DPA_AMMINISTRA
--************************
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='VAR_FORMATO_PROT_TIT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD VAR_FORMATO_PROT_TIT VARCHAR(255)';
        end if;
    end;
end;
/

--CREAZIONE TABELLA DPA_PROTO_TIT
--*******************************
begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_PROTO_TIT';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_PROTO_TIT START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_PROTO_TIT';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_PROTO_TIT ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'ID_AMM INT, ' ||
            'ID_NODO_TIT INT, ' ||
            'ID_REGISTRO INT, ' ||
            'NUM_RIF INT, ' ||
            'CONSTRAINT PK_DPA_PROTO_TIT PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/ 

--POPOLAMENTO TABELLA DPA_PROTO_TIT
--*********************************
begin
    declare cnt int;
    begin
    select count(*) into cnt from dpa_proto_tit;
        if (cnt = 0) then
            declare
                cursor CURR_AMM is select system_id as idamm from dpa_amministra;
                idamm NUMBER; 
            begin
                open CURR_AMM;
                loop
                fetch CURR_AMM into idamm;
                exit when CURR_AMM%notfound;
                    declare
                        cursor CURR_PROJECT is select system_id from project where id_titolario = 0 and var_codice = 'T' and ID_AMM = idamm;
                        idtitolario NUMBER;
                    begin
                        open CURR_PROJECT;
                        loop
                        fetch CURR_PROJECT into idtitolario;
                        exit when CURR_PROJECT%notfound;
                            insert into dpa_proto_tit (SYSTEM_ID, ID_AMM, ID_NODO_TIT, ID_REGISTRO, NUM_RIF) values (SEQ_DPA_PROTO_TIT.nextval, idamm, idtitolario,null,1);
                            declare 
                                cursor CURR_REG is select system_id from dpa_el_registri where ID_AMM = idamm;
                                idregistro NUMBER;
                            begin
                            open CURR_REG;
                            loop
                            fetch CURR_REG into idregistro;
                            exit when CURR_REG%notfound;
                                insert into dpa_proto_tit (SYSTEM_ID, ID_AMM, ID_NODO_TIT, ID_REGISTRO, NUM_RIF) values (SEQ_DPA_PROTO_TIT.nextval, idamm, idtitolario,idregistro,1);    
                            end loop;
                        close CURR_REG;
                        end;
                        end loop;
                    close CURR_PROJECT;
                    end;                                             
                end loop;
                close CURR_AMM;
            end;
        end if;  
    end;
end;
/

--MODIFICHE PROFILE
--*****************
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='PROT_TIT';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD PROT_TIT VARCHAR(255)';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='NUM_IN_FASC';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD NUM_IN_FASC NUMBER';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='ID_FASC_PROT_TIT';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD ID_FASC_PROT_TIT NUMBER';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='NUM_PROT_TIT';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD NUM_PROT_TIT NUMBER';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='ID_TITOLARIO';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD ID_TITOLARIO NUMBER';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='DTA_PROTO_TIT';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROFILE ADD DTA_PROTO_TIT DATE';
        end if; 
    end;
end;
/

--MODIFICHE PROJECT_COMPONENTS
--****************************
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT_COMPONENTS' AND column_name='PROT_TIT';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROJECT_COMPONENTS ADD PROT_TIT VARCHAR(255)';
        end if; 
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROJECT_COMPONENTS' AND column_name='DTA_CLASS';
        if (cnt = 0) then            
            execute immediate 'ALTER TABLE PROJECT_COMPONENTS ADD DTA_CLASS DATE';
        end if; 
    end;
end;
/

--MODIFICHE STORED CREATE_NEW_NODO_TITOLARIO
--******************************************

CREATE OR REPLACE PROCEDURE CREATE_NEW_NODO_TITOLARIO(p_idAmm number, p_livelloNodo number,
p_description varchar2, p_codiceNodo varchar2, p_idRegistroNodo number, p_idParent number,
p_varCodLiv1 varchar2, p_mesiConservazione number, p_chaRW char, p_idTipoFascicolo number, p_bloccaFascicolo varchar2, p_sysIdTitolario number, p_noteNodo varchar, p_bloccaFigli varchar2, p_contatoreAttivo varchar2, p_numProtTit number, p_idTitolario OUT number) IS
BEGIN
DECLARE CURSOR currReg IS
select system_id
from DPA_EL_REGISTRI
WHERE ID_AMM = p_idAmm and cha_rf = '0';

secProj NUMBER;
secFasc NUMBER;
secRoot NUMBER;
varChiaveTit varchar2(64);
varChiaveFasc varchar2(64);
varChiaveRoot varchar2(64);
BEGIN
p_idTitolario:=0;

SELECT SEQ.NEXTVAL INTO secProj FROM DUAL;
p_idTitolario:= secProj;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveTit:= p_idamm ||'_'|| p_codiceNodo || '_' || p_idParent || '_0' ;
else
varChiaveTit:= p_codiceNodo || '_' || p_idParent || '_'  || p_idRegistroNodo;
end if;

BEGIN

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(
secProj,
p_description,
'Y',
'T',
p_codiceNodo,
p_idAmm,
p_idRegistroNodo,
p_livelloNodo,
NULL,
p_idParent,
p_varCodLiv1,
sysdate ,
NULL,
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveTit,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit
);


EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;

END;

BEGIN

SELECT SEQ.NEXTVAL INTO secFasc FROM DUAL;

if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_0' ;
else
varChiaveFasc:= p_codiceNodo || '_' || p_idTitolario || '_'  || p_idRegistroNodo;
end if;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(
secFasc,
p_description,
'Y',
'F',
p_codiceNodo,
P_idAmm,
p_idRegistroNodo,
NULL,
'G',
p_idTitolario,
NULL,
sysdate ,
'A',
NULL,
p_chaRW,
p_mesiConservazione,
varChiaveFasc,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit
);

EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;


BEGIN


if(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') then
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_0' ;
else
varChiaveRoot:= p_codiceNodo || '_' || secFasc || '_'  || p_idRegistroNodo;
end if;

SELECT SEQ.NEXTVAL INTO secRoot FROM DUAL;

INSERT INTO PROJECT
(
SYSTEM_ID,
DESCRIPTION,
ICONIZED,
CHA_TIPO_PROJ,
VAR_CODICE,
ID_AMM,
ID_REGISTRO,
NUM_LIVELLO,
CHA_TIPO_FASCICOLO,
ID_PARENT,
VAR_COD_LIV1,
DTA_APERTURA,
CHA_STATO,
ID_FASCICOLO,
CHA_RW,
NUM_MESI_CONSERVAZIONE,
VAR_CHIAVE_FASC,
ID_TIPO_FASC,
CHA_BLOCCA_FASC,
ID_TITOLARIO,
DTA_CREAZIONE,
VAR_NOTE,
CHA_BLOCCA_FIGLI,
CHA_CONTA_PROT_TIT,
NUM_PROT_TIT
)
VALUES
(
secRoot,
'Root Folder',
'Y',
'C',
NULL,
p_idAmm,
NULL,
NULL,
NULL,
secFasc,
NULL,
sysDate,
NULL,
secFasc,
p_chaRW,
p_mesiConservazione,
varChiaveRoot,
p_idTipoFascicolo,
p_bloccaFascicolo,
p_sysIdTitolario,
sysdate,
p_noteNodo,
p_bloccaFigli,
p_contatoreAttivo,
p_numProtTit
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
IF(p_idRegistroNodo IS NULL or p_idRegistroNodo = '') THEN
FOR currentReg IN currReg
LOOP
BEGIN
INSERT INTO DPA_REG_FASC
(
system_id,
id_Titolario,
num_rif,
id_registro
)
VALUES
(
seq.nextval,
p_idTitolario,
1,
currentReg.system_id
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END LOOP;

-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
NULL    -- SE IL NODO è COMUNE A TUTTI p_idRegistro = NULL
);

ELSE -- il nodo creato è  associato  a uno solo registro

BEGIN
insert into dpa_reg_fasc
(
system_id,
id_Titolario,
num_rif,
id_registro
)
values
(
seq.nextval,
p_idTitolario,
1,
p_idRegistroNodo    -- REGISTRO SU CUI  CREO IL NODO
);
EXCEPTION
WHEN OTHERS THEN  p_idTitolario:=0;
RETURN;
END;
END IF;
end;

END CREATE_NEW_NODO_TITOLARIO;
/

-- FINE PROTOCOLLO TITOLARIO

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_TRASM_SINGOLA';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_TRASM_SINGOLA' and column_name='CHA_SET_EREDITA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_TRASM_SINGOLA ADD CHA_SET_EREDITA CHAR(1) DEFAULT 1';
		end if;
	end if;
	end;
end;
/

CREATE OR REPLACE FUNCTION getVisibilita(p_IdAmm number, p_Idmodello number)

RETURN INT IS result INT;
ragioneCC number;
ragioneComp number;
ragioneConos number;
ragioneRef number;
ragioneTO number;

BEGIN

    begin
       result := 0;
       
       IF (p_IdAmm IS NOT NULL) THEN
       BEGIN
           SELECT DPA_AMMINISTRA.ID_RAGIONE_CC into ragioneCC FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm; 
           SELECT DPA_AMMINISTRA.ID_RAGIONE_COMPETENZA into ragioneComp FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm;
           SELECT DPA_AMMINISTRA.ID_RAGIONE_CONOSCENZA into ragioneConos FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm;
           SELECT DPA_AMMINISTRA.ID_RAGIONE_REFERENTE into ragioneRef FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm;
           SELECT DPA_AMMINISTRA.ID_RAGIONE_TO into ragioneTO FROM DPA_AMMINISTRA WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm;
           
           IF (ragioneCC != 0 AND ragioneCC IS NOT NULL) THEN
               SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_CC FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm);
           END IF;
           
           IF (ragioneComp != 0 AND ragioneComp IS NOT NULL AND result < 1) THEN
               SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_COMPETENZA FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm);
           END IF;
           
           IF (ragioneConos != 0 AND ragioneConos IS NOT NULL AND result < 1) THEN
               SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_CONOSCENZA FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm);
           END IF;
           
           IF (ragioneRef != 0 AND ragioneRef IS NOT NULL AND result < 1) THEN
               SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_REFERENTE FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm);
           END IF;
           
           IF (ragioneTO != 0 AND ragioneTO IS NOT NULL AND result < 1) THEN
               SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
               WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
               (SELECT DPA_AMMINISTRA.ID_RAGIONE_TO FROM DPA_AMMINISTRA
               WHERE DPA_AMMINISTRA.SYSTEM_ID = p_IdAmm);
           END IF;
       END;
       END IF;
       
       IF (p_Idmodello IS NOT NULL AND result < 1) THEN
           SELECT COUNT(DPA_RAGIONE_TRASM.SYSTEM_ID) into result FROM DPA_RAGIONE_TRASM
           WHERE DPA_RAGIONE_TRASM.CHA_EREDITA = '1' AND DPA_RAGIONE_TRASM.SYSTEM_ID IN
           (SELECT DISTINCT DPA_MODELLI_MITT_DEST.ID_RAGIONE FROM DPA_MODELLI_MITT_DEST
           WHERE DPA_MODELLI_MITT_DEST.ID_MODELLO = p_Idmodello);
       END IF;
       
       EXCEPTION
         WHEN NO_DATA_FOUND THEN result := 1;
         WHEN OTHERS THEN result := 1;
    end;
    
    IF(result > 0) THEN
        result := 1;
    ELSE
        result := 0;
    END IF;
    
    RETURN result;
    
END getVisibilita;
/

begin
declare   cnt int;
	begin
	select count(*) into cnt from user_tables where table_name='DPA_COUNT_TODOLIST';
		if (cnt = 1) then
			execute immediate	
				'DROP TABLE DPA_COUNT_TODOLIST CASCADE CONSTRAINTS';
		end if;
	end;
end;
/

begin
    declare   cnt int;
    begin
	select count(*) into cnt from user_tables where table_name='DPA_COUNT_TODOLIST';
		if (cnt = 0) then
		  execute immediate
			'CREATE TABLE DPA_COUNT_TODOLIST ' ||
			'( ' ||
			'  ID_PEOPLE              NUMBER                 NOT NULL, ' ||
			'  TS_STAMPA              DATE                   NOT NULL, ' ||
			'  TOT_DOC                NUMBER                 DEFAULT 0, ' ||
			'  TOT_DOC_NO_LETTI       NUMBER                 DEFAULT 0, ' ||
			'  TOT_DOC_NO_ACCETTATI   NUMBER                 DEFAULT 0, ' ||
			'  TOT_FASC               NUMBER                 DEFAULT 0, ' ||
			'  TOT_FASC_NO_LETTI      NUMBER                 DEFAULT 0, ' ||
			'  TOT_FASC_NO_ACCETTATI  NUMBER                 DEFAULT 0, ' ||
			'  TOT_DOC_PREDISPOSTI    NUMBER                 DEFAULT 0 ' ||
			') ';
		end if;
    end;    
end;    
/


CREATE OR REPLACE PROCEDURE @db_user.sp_dpa_count_todolist (
   id_people_p   IN   NUMBER,
   id_gruppo     IN   NUMBER,
   ts            IN   VARCHAR
)
IS
   trasmdoctot             NUMBER;
   trasmdocnonletti        NUMBER;
   trasmdocnonaccettati    NUMBER;
   trasmfasctot            NUMBER;
   trasmfascnonletti       NUMBER;
   trasmfascnonaccettati   NUMBER;
   docpredisposti          NUMBER;
   ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdoctot
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
--numero documenti non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonletti
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
--numero documenti non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonaccettati
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero fascicoli presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfasctot
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
--numero fascicoli non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonletti
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
--numero fascicoli non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonaccettati
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero documenti predisposti
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO docpredisposti
     FROM dpa_todolist
    WHERE id_profile > 0
      AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
   BEGIN                                                               -- MAIN
      ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');
-- SVUOTO LA TABELLA DEI DATI
      DELETE      dpa_count_todolist
            WHERE (id_people = id_people_p);

      INSERT INTO dpa_count_todolist
                  (id_people, ts_stampa, tot_doc, tot_doc_no_letti,
                   tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
                   tot_fasc_no_accettati, tot_doc_predisposti
                  )
           VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
                   trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
                   trasmfascnonaccettati, docpredisposti
                  );
   END;                                                                -- MAIN
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='ID_RF';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_CORR_GLOBALI ADD ID_RF NUMBER';
    end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION pop
RETURN INT IS retValue INT;
 
cnt INT := 0;
amm INT := 0;
codice VARCHAR2(128);
descrizione VARCHAR2(256);
aoo INT := 0;
systemId INT := 0;
recordCount INT := 0;

CURSOR cur IS
select distinct SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESC_REGISTRO, ID_AOO_COLLEGATA 
from DPA_EL_REGISTRI where CHA_RF = 1;

   
 BEGIN

  OPEN cur;
   LOOP
   FETCH cur INTO systemId, amm, codice, descrizione, aoo;
   EXIT WHEN cur%NOTFOUND;   
   
   begin
        cnt := 0;
        select count(*) into cnt from dpa_corr_globali where id_rf = systemId;
        if (cnt = 0)
        then
      insert into dpa_corr_globali(system_id,ID_AMM,ID_REGISTRO,VAR_COD_RUBRICA,VAR_DESC_CORR,DTA_INIZIO,CHA_TIPO_URP,ID_RF)
      values(seq.nextval,amm,aoo,codice,descrizione,sysdate,'F',systemId);
      end if;
    end;

   
        recordCount := cnt + recordCount; 

   
   END LOOP;
   
IF (recordCount > 0) THEN
  retValue := 1;
ELSE
  retValue := 0;
END IF;  

    RETURN retValue;
   
END pop;
/

DECLARE 
  RetVal INT;

BEGIN 
  RetVal := POP;
  COMMIT; 
END; 
/

CREATE OR REPLACE FUNCTION DEST_IN_LISTA (IdProfile INT)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);

CURSOR cur IS
SELECT L.VAR_DESC_CORR
      FROM DPA_LISTE_DISTR B, DPA_CORR_GLOBALI L, DPA_DOC_ARRIVO_PAR C
      WHERE
      C.ID_PROFILE = IdProfile AND
      C.CHA_TIPO_MITT_DEST = 'L' AND 
      B.ID_LISTA_DPA_CORR = C.ID_MITT_DEST
      AND L.SYSTEM_ID=B.ID_DPA_CORR
      AND L.DTA_FINE IS NULL;

BEGIN
risultato := NULL;
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

risultato := risultato||item || '(D) ';

END LOOP;

RETURN risultato;

END DEST_IN_LISTA;
/

CREATE OR REPLACE FUNCTION DEST_IN_RF (IdProfile INT)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);

CURSOR cur IS
SELECT B.VAR_DESC_CORR
      FROM DPA_DOC_ARRIVO_PAR L, DPA_L_RUOLO_REG C, DPA_CORR_GLOBALI B, DPA_CORR_GLOBALI D
      WHERE
      L.ID_PROFILE = IdProfile AND
      L.CHA_TIPO_MITT_DEST = 'F' AND
      L.ID_MITT_DEST = D.SYSTEM_ID AND
      D.ID_RF = C.ID_REGISTRO AND
      C.ID_RUOLO_IN_UO = B.SYSTEM_ID;

BEGIN
risultato := NULL;
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

risultato := risultato||item || '(D) ';

END LOOP;

RETURN risultato;

END DEST_IN_RF;
/

CREATE OR REPLACE FUNCTION CORRCAT (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);
tipo_mitt_dest VARCHAR(10);
LNG INT;

CURSOR cur IS
SELECT c.var_desc_corr, dap.cha_tipo_mitt_dest
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item,tipo_mitt_dest;
EXIT WHEN cur%NOTFOUND;

LNG:=LENGTh(risultato);

IF(risultato IS NOT NULL anD LNG>=(3900-128))
tHEN RETURN RISULTATO||'...';
ELSE
BEGIN

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'I') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MI)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'L') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_LISTA(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'F') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_RF(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;


END;
END IF;

END LOOP;

RETURN risultato;

END Corrcat;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='ID_GRUPPO_LISTE';
    if (cnt = 1) then        
            execute immediate 'ALTER TABLE DPA_CORR_GLOBALI MODIFY(ID_GRUPPO_LISTE NUMBER(10))';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_INS_CORR_TUTTI';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_INS_CORR_TUTTI'', ''Abilita inserimento di un nuovo utente nella rubrica'', ''N'')';
		end;
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_INS_CORR_RF';
    if (cnt = 0) then
		begin
			execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_INS_CORR_RF'', ''Abilita inserimento di un nuovo utente nella rubrica'', ''N'')';
		end;
	end if;
	end;
end;
/

begin   
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_INS_CORR_REG';
    if (cnt = 0) then
		begin
            execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_INS_CORR_REG'', ''Abilita inserimento di un nuovo utente nella rubrica'', ''N'')';
		end;
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' and column_name='VAR_TIPO_CONNESSIONE';
    if (cnt > 0) then        
            execute immediate ' update DPA_EL_REGISTRI SET VAR_TIPO_CONNESSIONE = ''POP'' where (VAR_TIPO_CONNESSIONE = '''') or (VAR_TIPO_CONNESSIONE is null)';
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_SALVA_RICERCHE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_SALVA_RICERCHE' and column_name='TIPO';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_SALVA_RICERCHE ADD TIPO CHAR';
		end if;
	end if;
	end;
end;
/

begin
    declare tt int;
  begin
    select count(distinct(tipo)) into tt from DPA_SALVA_RICERCHE;
    if (tt = 0) then        
            execute immediate 'UPDATE DPA_SALVA_RICERCHE SET TIPO = ''D''';
    end if;
    end;
end;
/

begin
	execute immediate 'update dpa_corr_globali set var_desc_corr = var_cognome || '' ''|| var_nome where cha_tipo_urp = ''P''';
end;
/

begin
	execute immediate 'UPDATE people SET full_name = var_cognome || '' '' || var_nome WHERE system_id IN (SELECT id_people FROM dpa_corr_globali WHERE cha_tipo_urp = ''P'')';
end;
/

-- Deleghe
CREATE OR REPLACE PROCEDURE createallegato (
   p_iddocumentoprincipale         INT,
   p_idpeople                      INT,
   p_comments                      NVARCHAR2,
   p_numeropagine                  INT,
   p_idpeopledelegato              INT,
   p_idprofile               OUT   INT,
   p_versionid               OUT   INT
)
IS
   returnvalue   NUMBER;
/******************************************************************************
   NAME:       createAllegato
   PURPOSE:    Creazione di un nuovo documento di tipo allegato
******************************************************************************/
   iddoctype     INT    := 0;
BEGIN
   returnvalue := 0;

   -- Reperimento tipologia atto del documento principale
   SELECT documenttype
     INTO iddoctype
     FROM PROFILE
    WHERE system_id = p_iddocumentoprincipale;

   -- Reperimento identity
   SELECT seq.NEXTVAL
     INTO p_idprofile
     FROM DUAL;

   INSERT INTO PROFILE
               (system_id, docnumber, typist, author, cha_tipo_proto,
                cha_da_proto, documenttype, creation_date, creation_time,
                id_documento_principale, id_people_delegato
               )
        VALUES (p_idprofile, p_idprofile, p_idpeople, p_idpeople, 'G',
                '0', iddoctype, SYSDATE, SYSDATE,
                p_iddocumentoprincipale, p_idpeopledelegato
               );

   returnvalue := SQL%ROWCOUNT;

   IF (returnvalue > 0)
   THEN
      -- Reperimento identity
      SELECT seq.NEXTVAL
        INTO p_versionid
        FROM DUAL;

      -- Inserimento record in tabella VERSIONS
      INSERT INTO VERSIONS
                  (version_id, docnumber, VERSION, subversion, version_label,
                   author, typist, comments, num_pag_allegati,
                   dta_creazione, cha_da_inviare, id_people_delegato
                  )
           VALUES (p_versionid, p_idprofile, 1, '!', '1',
                   p_idpeople, p_idpeople, p_comments, p_numeropagine,
                   SYSDATE, '1', p_idpeopledelegato
                  );

      -- Inserimento record in tabella COMPONENTS
      INSERT INTO components
                  (version_id, docnumber, file_size
                  )
           VALUES (p_versionid, p_idprofile, 0
                  );
   END IF;
END createallegato;
/


CREATE OR REPLACE PROCEDURE createdocsp (
   p_idpeople                 NUMBER,
   p_doctype                  VARCHAR,
   p_idpeopledelegato         NUMBER,
   p_isFirmato				  VARCHAR,
   p_systemid           OUT   NUMBER
)
IS
BEGIN
   DECLARE
      docnum      NUMBER;
      verid       NUMBER;
      iddoctype   NUMBER;
   BEGIN
      p_systemid := 0;

      <<reperimento_documenttypes>>
      BEGIN
         SELECT system_id
           INTO iddoctype
           FROM documenttypes
          WHERE type_id = p_doctype;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            p_systemid := 0;
            RETURN;
      END reperimento_documenttypes;

      SELECT seq.NEXTVAL
        INTO docnum
        FROM DUAL;

      p_systemid := docnum;

      <<inserimento_in_profile>>
      BEGIN
         INSERT INTO PROFILE
                     (system_id, typist, author, documenttype,
                      creation_date, creation_time, docnumber,
                      id_people_delegato
                     )
              VALUES (docnum, p_idpeople, p_idpeople, iddoctype,
                      SYSDATE, SYSDATE, docnum,
                      p_idpeopledelegato
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_systemid := 0;
            RETURN;
      END inserimento_in_profile;

      <<inserimento_in_versions>>
      BEGIN
         SELECT seq.NEXTVAL
           INTO verid
           FROM DUAL;

         INSERT INTO VERSIONS
                     (version_id, docnumber, VERSION, subversion,
                      version_label, author, typist, dta_creazione,
                      id_people_delegato
                     )
              VALUES (verid, docnum, 1, '!',
                      '1', p_idpeople, p_idpeople, SYSDATE,
                      p_idpeopledelegato
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_systemid := 0;
            RETURN;
      END inserimento_in_versions;

      <<inserimento_in_components>>
      BEGIN
         INSERT INTO components
                     (version_id, docnumber, file_size, CHA_FIRMATO
                     )
              VALUES (verid, docnum, 0, p_isFirmato
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_systemid := 0;
            RETURN;
      END inserimento_in_components;

      <<inserimento_security>>
      BEGIN
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (docnum, p_idpeople, 0, NULL,
                      NULL
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_systemid := 0;
            RETURN;
      END inserimento_security;
   END;
END;
/

begin
    declare   cnt int;
    begin
	select count(*) into cnt from user_tables where table_name='DPA_DELEGHE';
		if (cnt = 0) then
		  execute immediate    
				'CREATE TABLE DPA_DELEGHE ' ||
				'( ' ||
				'SYSTEM_ID NUMBER NOT NULL, ' ||
				'ID_PEOPLE_DELEGANTE NUMBER, ' ||
				'ID_RUOLO_DELEGANTE NUMBER, ' ||
				'ID_PEOPLE_DELEGATO NUMBER, ' ||
				'ID_RUOLO_DELEGATO NUMBER, ' ||
				'DATA_DECORRENZA DATE, ' ||
				'DATA_SCADENZA DATE, ' ||
				'CHA_IN_ESERCIZIO CHAR(1), ' ||
				'COD_PEOPLE_DELEGANTE VARCHAR2(64 BYTE), ' ||
				'COD_RUOLO_DELEGANTE VARCHAR2(128 BYTE), ' ||
				'COD_PEOPLE_DELEGATO VARCHAR2(64 BYTE), ' ||
				'COD_RUOLO_DELEGATO VARCHAR2(128 BYTE), ' ||
				'ID_UO_DELEGATO NUMBER, ' ||
				'CONSTRAINT PK_DPA_DELEGHE PRIMARY KEY (SYSTEM_ID) ' ||  
				') ';
		end if;
    end;    
end;    
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_anagrafica_funzioni where cod_funzione='GEST_DELEGHE';
    if (cnt = 0) then
		begin
            INSERT INTO dpa_anagrafica_funzioni (cod_funzione,var_desc_funzione,disabled) values ('GEST_DELEGHE','Abilita il sottomenu'' Deleghe del menu'' Gestione','N');
		end;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_anagrafica_funzioni where cod_funzione='DIRITTO_DELEGA';
    if (cnt = 0) then        
		begin
            insert into dpa_anagrafica_funzioni (cod_funzione,var_desc_funzione,disabled) values ('DIRITTO_DELEGA','Abilita i pulsanti NUOVA, REVOCA e MODIFICA nella pagina di gestione delle deleghe','N');
		end;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_LOGIN' and column_name='USER_ID_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_LOGIN ADD USER_ID_DELEGATO VARCHAR(20)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_NOTE' and column_name='IDPEOPLEDELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_NOTE ADD IDPEOPLEDELEGATO INTEGER';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TODOLIST' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TODOLIST ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_DIAGRAMMI_STO' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_DIAGRAMMI_STO ADD ID_PEOPLE_DELEGATO INTEGER';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TRASM_UTENTE' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TRASM_UTENTE ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TRASMISSIONE' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TRASMISSIONE ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROFILE' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROFILE ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_LOGIN' and column_name='CHA_DELEGA';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_LOGIN ADD CHA_DELEGA CHAR(1)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='VERSIONS' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE VERSIONS ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

CREATE OR REPLACE TRIGGER TR_INSERT_DPA_TODOLIST
AFTER UPDATE
OF DTA_INVIO
ON @db_user.DPA_TRASMISSIONE 
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN
   INSERT INTO dpa_todolist
      SELECT :NEW.system_id, dtu.id_trasm_singola, dtu.system_id,
             :NEW.dta_invio, :NEW.id_people, :NEW.id_ruolo_in_uo,
             dtu.id_people, dts.id_ragione, :NEW.var_note_generali,
             dts.var_note_sing, dts.dta_scadenza, :NEW.id_profile,
             :NEW.id_project,
             TO_NUMBER (vardescribe (dts.id_corr_globale, 'ID_GRUPPO')
                       ) AS id_ruolo_dest,
             TO_NUMBER (vardescribe (:NEW.id_profile, 'PROF_IDREG')
                       ) AS id_registro,
             dts.cha_tipo_trasm,
             CASE
                WHEN dtu.dta_vista IS NULL
                   THEN TO_DATE ('01/01/1753', 'dd/mm/yyyy')
                ELSE dtu.dta_vista
             END,
             :NEW.ID_PEOPLE_DELEGATO
             FROM dpa_trasm_singola dts, dpa_trasm_utente dtu
       WHERE dtu.id_trasm_singola = dts.system_id
         AND dts.id_trasmissione = :NEW.system_id
         AND dtu.cha_in_todolist = 1;
END;
/
--fine deleghe

--AREA CONSERVAZIONE verifiche
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_ITEMS_CONSERVAZIONE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='VAR_OGGETTO';
  		if (cnt != 0) then
        	execute immediate 'alter table DPA_ITEMS_CONSERVAZIONE modify VAR_OGGETTO VARCHAR2(2000 BYTE)';
		end if;
	end if;
	end;
end;
/  

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_ITEMS_CONSERVAZIONE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='CHA_ESITO';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_ITEMS_CONSERVAZIONE ADD CHA_ESITO CHAR(1)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AREA_CONSERVAZIONE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_AREA_CONSERVAZIONE' and column_name='ID_PROFILE_TRASMISSIONE';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_AREA_CONSERVAZIONE ADD ID_PROFILE_TRASMISSIONE NUMBER';
		end if;
	end if;
	end;
end;
/

--conservazione html

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_ITEMS_CONSERVAZIONE';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_ITEMS_CONSERVAZIONE' and column_name='VAR_TIPO_ATTO';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_ITEMS_CONSERVAZIONE ADD VAR_TIPO_ATTO VARCHAR2(64 BYTE)';
		end if;
	end if;
	end;
end;
/ 

CREATE OR REPLACE PROCEDURE SP_INSERT_AREA_CONS
(
p_idAmm              NUMBER,
p_idPeople           NUMBER,
p_idProfile          NUMBER,
p_idProject          NUMBER,
p_codFasc            VARCHAR,
p_oggetto            VARCHAR,
p_tipoDoc            CHAR,
p_idGruppo           NUMBER,
p_idRegistro         NUMBER,
p_docNumber          NUMBER,
p_userId             VARCHAR,
p_tipoOggetto         char,
p_tipoAtto           VARCHAR,
p_result             OUT   NUMBER
)
IS

idRuoloInUo NUMBER:=0;
id_cons_1 NUMBER:=0;
id_cons_2 NUMBER:=0;
res number:=0;

begin

SELECT SEQ_CONSERVAZIONE.nextval into id_cons_1 from dual;

SELECT SEQ_CONSERVAZIONE.nextval into id_cons_2 from dual;

SELECT DPA_CORR_GLOBALI.SYSTEM_ID INTO idRuoloInUo FROM DPA_CORR_GLOBALI WHERE DPA_CORR_GLOBALI.ID_GRUPPO = p_idGruppo;

begin
SELECT DISTINCT DPA_AREA_CONSERVAZIONE.SYSTEM_ID  INTO res  FROM DPA_AREA_CONSERVAZIONE WHERE
DPA_AREA_CONSERVAZIONE.ID_PEOPLE=p_idPeople AND
DPA_AREA_CONSERVAZIONE.ID_RUOLO_IN_UO = idRuoloInUo AND
DPA_AREA_CONSERVAZIONE.CHA_STATO='N';
exception when others then res:=0;
end;
IF (res>0) THEN

INSERT INTO DPA_ITEMS_CONSERVAZIONE (
SYSTEM_ID,
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
id_cons_1,
res,
p_idProfile,
p_idProject,
p_tipoDoc,
p_oggetto,
p_idRegistro,
sysdate,
'N',
EMPTY_CLOB(),
p_codFasc,
p_docNumber,
p_tipoOggetto,
p_tipoAtto
);

p_result:=id_cons_1;

ELSE

INSERT INTO DPA_AREA_CONSERVAZIONE (
SYSTEM_ID,
ID_AMM,
ID_PEOPLE,
ID_RUOLO_IN_UO,
CHA_STATO,
DATA_APERTURA,
USER_ID,
ID_GRUPPO
)
VALUES(
id_cons_1,
p_idAmm,
p_idPeople,
idRuoloInUo,
'N',
sysdate,
p_userId,
p_idGruppo
);

INSERT INTO DPA_ITEMS_CONSERVAZIONE (
SYSTEM_ID,
ID_CONSERVAZIONE,
ID_PROFILE,
ID_PROJECT,
CHA_TIPO_DOC,
VAR_OGGETTO,
ID_REGISTRO,
DATA_INS,
CHA_STATO,
VAR_XML_METADATI,
COD_FASC,
DOCNUMBER,
CHA_TIPO_OGGETTO,
VAR_TIPO_ATTO
)
VALUES
(
id_cons_2,
id_cons_1,
p_idProfile,
p_idProject,
p_tipoDoc,
p_oggetto,
p_idRegistro,
sysdate,
'N',
EMPTY_CLOB(),
p_codFasc,
p_docNumber,
p_tipoOggetto,
p_tipoAtto
);

p_result:=id_cons_2;

END IF;

exception when others then p_result:=-1;

END;
/
--conservazione modofiche per le nuove verifiche dei supporti


begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_SUPPORTO';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_SUPPORTO' and column_name='PERC_VERIFICA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_SUPPORTO ADD PERC_VERIFICA NUMBER';
		end if;
	end if;
	end;
end;
/ 

begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_CONS_VERIFICA';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_CONS_VERIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;  
        
select count(*) into cnt from user_tables where table_name='DPA_CONS_VERIFICA';
		if (cnt = 0) then
			execute immediate	
				'CREATE TABLE DPA_CONS_VERIFICA ' ||
				'( ' ||
				'  SYSTEM_ID          NUMBER NOT NULL, ' ||
				'  ID_SUPPORTO        NUMBER, ' ||
				'  ID_ISTANZA         NUMBER, ' ||
				'  DATA_VER           DATE, ' ||
				'  NUM_VER            NUMBER, ' ||
				'  VAR_NOTE           VARCHAR2(256 BYTE), ' ||
				'  PERCENTUALE        NUMBER, ' ||
				'  ESITO              VARCHAR2(1 BYTE), ' ||	
				'  CONSTRAINT DPA_CONS_VERIFICA_PK PRIMARY KEY (SYSTEM_ID)' ||			
				')';
		end if;
		
	end;
end;
/

CREATE OR REPLACE PROCEDURE SP_INSERT_DPA_SUPPORTO 
(
p_copia             number,
p_collFisica        VARCHAR,
p_dataUltimaVer     DATE,
p_dataEliminazione  DATE,
p_esitoUltimaVer    NUMBER,
p_numeroVer         NUMBER,
p_dataProxVer       DATE,
p_dataAppoMarca     DATE,
p_dataScadMarca     DATE,
p_marca             VARCHAR,
p_idCons            NUMBER,
p_tipoSupp          NUMBER,
p_stato             CHAR,
p_note              VARCHAR,
p_query             CHAR,
p_idSupp            NUMBER,
p_percVerifica      NUMBER,
p_result        OUT NUMBER,
p_newId         OUT NUMBER
)

IS
numSuppProd NUMBER:=0;
numSuppTotali NUMBER:=0;
numSupporto   NUMBER:=0;

BEGIN

SELECT SEQ_SUPPORTO.nextval into numSupporto from dual;

IF(p_query='I') THEN

INSERT INTO DPA_SUPPORTO (
  SYSTEM_ID,               
  COPIA,                    
  DATA_PRODUZIONE,         
  VAR_COLLOCAZIONE_FISICA,  
  DATA_ULTIMA_VERIFICA,     
  DATA_ELIMINAZIONE,        
  ESITO_ULTIMA_VERIFICA,    
  VERIFICHE_EFFETTUATE,   
  DATA_PROX_VERIFICA,      
  DATA_APPO_MARCA,          
  DATA_SCADENZA_MARCA,      
  VAR_MARCA_TEMPORALE,     
  ID_CONSERVAZIONE,         
  ID_TIPO_SUPPORTO,        
  CHA_STATO,                
  VAR_NOTE,
  PERC_VERIFICA               
)
VALUES(
numSupporto,
p_copia,
SYSDATE,
p_collFisica,
p_dataUltimaVer,
p_dataEliminazione,
p_esitoUltimaVer,
p_numeroVer,
p_dataProxVer,
p_dataAppoMarca,
p_dataScadMarca,
p_marca,
p_idCons,
p_tipoSupp,
p_stato,
p_note,
p_percVerifica
);

p_newId:=numSupporto;

SELECT COUNT(*) INTO numSuppProd  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=p_idCons;

SELECT COUNT(*) INTO numSuppTotali  FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=p_idCons;

IF(numSuppProd=numSuppTotali) THEN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=p_idCons; 

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=p_idCons;

p_result:=1;
ELSE
p_result:=0;
END IF;


ELSE

UPDATE DPA_SUPPORTO SET DATA_PRODUZIONE=SYSDATE, VAR_COLLOCAZIONE_FISICA=p_collFisica,
DATA_PROX_VERIFICA=p_dataProxVer, CHA_STATO= p_stato, VAR_NOTE=p_note, DATA_ULTIMA_VERIFICA=SYSDATE, VERIFICHE_EFFETTUATE=p_numeroVer, ESITO_ULTIMA_VERIFICA=p_esitoUltimaVer, PERC_VERIFICA=p_percVerifica WHERE SYSTEM_ID=p_idSupp;

p_newId:=p_idSupp;

SELECT COUNT(*) INTO numSuppProd  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=
(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

SELECT COUNT(*) INTO numSuppTotali  FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

IF(numSuppProd=numSuppTotali) THEN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp); 

p_result:=1;
ELSE
p_result:=0;
END IF;


END IF;


END;
/

--modifiche conservazione per rigenera marca temporale
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_SUPPORTO';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_SUPPORTO' and column_name='NUM_MARCA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_SUPPORTO ADD NUM_MARCA NUMBER';
		end if;
	end if;
	end;
end;
/ 

--INIZIO : RIFERIMENTI MITTENTE
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CHA_RIFF_MITT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CHA_RIFF_MITT VARCHAR2(255)';
        end if; 
 end;
end;
/

begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_RISCONTRI_CLASSIFICA';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_RISCONTRI_CLASSIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_RISCONTRI_CLASSIFICA';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_RISCONTRI_CLASSIFICA ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'RIFF_MITT VARCHAR2(255), ' ||
            'ID_CORR_GLOB INT, ' ||
            'ID_TITOLARIO_DEST INT, ' ||
            'ID_REG_DEST INT, ' ||
            'CLASS_DEST VARCHAR2(255), ' ||
            'COD_FASC_DEST VARCHAR2(255), ' ||
            'PROT_TIT_DEST VARCHAR2(255), ' ||
            'DTA_RISCONTRO DATE, ' ||
            'CONSTRAINT PK_DPA_RISCONTRI_CLASSIFICA PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/
--FINE : RIFERIMENTI MITTENTE

--INVIO RICEVUTA RITORNO

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='DO_INVIO_RICEVUTE';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_INVIO_RICEVUTE','Abilita pulsante per invio manuale della ricevuta di ritorno','', 'N');
    end if;
    end;
end;
/

CREATE OR REPLACE PROCEDURE sp_ins_dpa_stato_invio 
AS
BEGIN 
	DECLARE
	profileId NUMBER;
	idMezzoSped number;


 CURSOR curr_dpa_stato_invio
   IS
      SELECT ID_PROFILE
        FROM dpa_stato_invio;
        
BEGIN

 select system_id INTO idMezzoSped from documenttypes where type_id='INTEROPERABILITA';

OPEN curr_dpa_stato_invio;

    LOOP

    FETCH curr_dpa_stato_invio
           INTO profileId;

          EXIT WHEN curr_dpa_stato_invio%NOTFOUND;
          BEGIN
          
            UPDATE dpa_stato_invio SET ID_DOCUMENTTYPE=idMezzoSped WHERE ID_PROFILE=profileId;
          
          END;

    END LOOP;
    
 CLOSE curr_dpa_stato_invio;
END;
END;
/


CREATE OR REPLACE PROCEDURE sp_ins_documentype_in_profile 
AS
BEGIN 
DECLARE 
profileId NUMBER;
idMezzoSped number;


 CURSOR curr_profile
   IS
      SELECT SYSTEM_ID
        FROM PROFILE WHERE cha_invio_conferma='1';
        
BEGIN

select system_id INTO idMezzoSped from documenttypes where type_id='INTEROPERABILITA';

OPEN curr_profile;

    LOOP

    FETCH curr_profile
           INTO profileId;

          EXIT WHEN curr_profile%NOTFOUND;
          BEGIN
          
            UPDATE profile SET DOCUMENTTYPE=idMezzoSped WHERE system_id=profileId;
          
          END;

    END LOOP;
    
 CLOSE curr_profile;
END;
END;
/


-- Replace funzione GetCountNote. Ora permette una ricerca
-- delle note anche in base alla visibilità.
CREATE OR REPLACE FUNCTION GetCountNote(tipoOggetto CHAR, idOggetto NUMBER, note NVARCHAR2, idUtente NUMBER, idGruppo NUMBER, tipoRic char)
RETURN NUMBER IS retValue NUMBER;

BEGIN

IF tipoRic = 'Q' THEN
    SELECT COUNT(SYSTEM_ID) INTO retValue
    FROM   DPA_NOTE N 
    WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
           N.IDOGGETTOASSOCIATO = idOggetto AND
           N.TESTO LIKE '%'||note||'%' AND
          (N.TIPOVISIBILITA = 'T' OR
          (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente) OR
          (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo));
          
    ELSIF tipoRic = 'T' THEN
        SELECT COUNT(SYSTEM_ID) INTO retValue
        FROM   DPA_NOTE N 
        WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
               N.IDOGGETTOASSOCIATO = idOggetto AND
               N.TESTO LIKE '%'||note||'%' AND
               N.TIPOVISIBILITA = 'T';
        
        ELSIF tipoRic = 'P' THEN
            SELECT COUNT(SYSTEM_ID) INTO retValue
            FROM   DPA_NOTE N 
            WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                   N.IDOGGETTOASSOCIATO = idOggetto AND
                   N.TESTO LIKE '%'||note||'%' AND
                   (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente);
                   
            ELSIF tipoRic = 'R' THEN
                SELECT COUNT(SYSTEM_ID) INTO retValue
                FROM   DPA_NOTE N 
                WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                       N.IDOGGETTOASSOCIATO = idOggetto AND
                       N.TESTO LIKE '%'||note||'%' AND
                       (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo);
                       
END IF;
      
RETURN retValue;
END GetCountNote;
/
-- Fine ridefinizione funzione GetCountNote


begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_DOC_ARRIVO_PAR';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_DOC_ARRIVO_PAR' and column_name='ID_DOCUMENTTYPES';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_DOC_ARRIVO_PAR ADD ID_DOCUMENTTYPES number(10)';
		end if;
	end if;
	end;
end;
/ 

CREATE OR REPLACE FUNCTION getnumProtoStampa (dNumber INT, anno INT, numeroProtoStart INT, numeroProtoEnd INT)
RETURN INT IS retValue INT;

cnt INT;

BEGIN
    cnt := 0;
    retValue := 0;

IF(dNumber is not null)
then
    if(anno is not null)
    then
			IF(numeroProtoStart IS NOT NULL)
            then
                    SELECT COUNT(A.SYSTEM_ID) into cnt 
                    From DPA_STAMPAREGISTRI A, PROFILE B
                    where A.NUM_PROTO_START <= numeroProtoStart
                    AND A.NUM_PROTO_END >= numeroProtoEnd
                    AND A.NUM_ANNO = anno
                    AND A.DOCNUMBER = B.SYSTEM_ID
					and A.docnumber = dNumber;
			ELSE
					SELECT COUNT(A.SYSTEM_ID) into cnt 
					From DPA_STAMPAREGISTRI A, PROFILE B
					where NUM_ANNO = anno
					AND A.DOCNUMBER = B.SYSTEM_ID
					and A.docnumber = dNumber;
            end if;
    else
			IF(numeroProtoStart IS NOT NULL)
            then
					SELECT COUNT(A.SYSTEM_ID) into cnt 
					From DPA_STAMPAREGISTRI A, PROFILE B 
					where NUM_PROTO_START <= numeroProtoStart
					AND NUM_PROTO_END >= numeroProtoEnd
					AND A.DOCNUMBER = B.SYSTEM_ID
					and A.docnumber = dNumber;
			ELSE
					SELECT COUNT(A.SYSTEM_ID) into cnt 
					From DPA_STAMPAREGISTRI A, PROFILE B
					WHERE A.DOCNUMBER = B.SYSTEM_ID
					and A.docnumber = dNumber;
            end if;
    end if;
end if;

   	IF (cnt > 0)
    then
        retValue := 1;
   	ELSE	
        retValue := 0;
    end if;
    
    RETURN retValue;

END getnumProtoStampa;
/


CREATE OR REPLACE FUNCTION getRegistroStampa (dNumber INT, idRegistro INT)
RETURN INT IS retValue INT;

cnt INT;

BEGIN
    cnt := 0;
    retValue := 0;

IF(dNumber is not null)
then
    if(idRegistro is not null)
    then
        SELECT COUNT(A.SYSTEM_ID) into cnt
	    From DPA_STAMPAREGISTRI A, PROFILE B
	    where A.DOCNUMBER = B.SYSTEM_ID
	    and A.docnumber=dNumber
	    and a.id_registro=idRegistro;   
    end if;
end if;

   	IF (cnt > 0)
    then
        retValue := 1;
   	ELSE	
        retValue := 0;
    end if;
    
    RETURN retValue;

END getRegistroStampa;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DOCUMENTTYPES where TYPE_ID='INTEROPERABILITA';
	if (cnt = 0) then		
	     INSERT INTO DOCUMENTTYPES (SYSTEM_ID, TYPE_ID, DESCRIPTION, DISABLED, STORAGE_TYPE, RETENTION_DAYS,
	MAX_VERSIONS, MAX_SUBVERSIONS, FULL_TEXT, TARGET_DOCSRVR, RET_2, RET_2_TYPE, KEEP_CRITERIA,
	VERSIONS_TO_KEEP, CHA_TIPO_CANALE )
  	SELECT
    SEQ.NEXTVAL,   'INTEROPERABILITA', 'INTEROPERABILITA', NULL, 'A', 0, 99, 26, 'N', 0, 0, 'A', 'L', 0, 'I'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DOCUMENTTYPES WHERE TYPE_ID='INTEROPERABILITA');

	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AMMINISTRA';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' and column_name='COL_SEGN';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD COL_SEGN char(1) DEFAULT (''2'')';
		end if;
	end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AMMINISTRA';
	if (cnt != 0) then		
		select count(*) into cnt from DPA_AMMINISTRA where COL_SEGN is null;
  	if (cnt != 0) then
        	execute immediate 'UPDATE DPA_AMMINISTRA SET COL_SEGN = ''2'' where COL_SEGN is null';
		end if;
	end if;
	end;
end;
/ 


CREATE OR REPLACE PROCEDURE dpa_ins_ut_ruolo_in_mod_trasm (
p_idpeople      IN      integer,
p_idcorrglob     IN    integer,
p_returnvalue     OUT   integer
)
IS

f_systemId NUMBER;

BEGIN
DECLARE
CURSOR modello (idpeople integer, idcorrglob integer)
IS
SELECT  distinct g.SYSTEM_ID as ID, g.ID_MODELLO as IdModello, g.ID_MODELLO_MITT_DEST as IdModelloMittDest
from dpa_modelli_dest_con_notifica g 
where g.ID_MODELLO_MITT_DEST in (
SELECT  distinct a.SYSTEM_ID as ID
FROM dpa_modelli_mitt_dest a
WHERE a.CHA_TIPO_MITT_DEST = 'D'
AND a.ID_CORR_GLOBALI = idcorrglob
AND a.CHA_TIPO_URP = 'R')
and g.ID_PEOPLE not in (idpeople);

sysidmod        NUMBER;

BEGIN
p_returnvalue := 0;

FOR currentmod IN modello (p_idpeople, p_idcorrglob)
LOOP
BEGIN
SELECT seq_dpa_modelli_mitt_dest.NEXTVAL
INTO sysidmod
FROM DUAL;

SELECT count(SYSTEM_ID) INTO f_systemId FROM dpa_modelli_dest_con_notifica
WHERE ID_MODELLO_MITT_DEST = currentMod.IdModelloMittDest
AND ID_PEOPLE = p_idpeople
AND ID_MODELLO = currentMod.IdModello;

if(f_systemId = 0)
then
begin
insert into dpa_modelli_dest_con_notifica
(system_id, id_modello_mitt_dest, id_people, id_modello)
VALUES
(sysidmod, currentmod.IdModelloMittDest, p_idpeople, currentmod.IdModello);
EXCEPTION
WHEN OTHERS
THEN
p_returnvalue := -1;
END;
END IF;

END;
END LOOP;

p_returnvalue :=1;
END;
END dpa_ins_ut_ruolo_in_mod_trasm;
/

CREATE OR REPLACE PROCEDURE dpa_del_ut_ruolo_in_mod_trasm (
p_idpeople      IN      integer,
p_idcorrglob     IN    integer,
p_returnvalue     OUT   integer
)
IS

BEGIN
p_returnvalue := 0;

begin
delete from dpa_modelli_dest_con_notifica a
where a.system_id in (
select distinct c.system_id from dpa_modelli_mitt_dest b, dpa_modelli_dest_con_notifica c
where c.ID_PEOPLE = p_idpeople
and c.ID_MODELLO = b.ID_MODELLO
and b.CHA_TIPO_MITT_DEST = 'D'
and b.CHA_TIPO_URP = 'R'
and b.ID_CORR_GLOBALI = p_idcorrglob
);

EXCEPTION
WHEN OTHERS
THEN
p_returnvalue := -2;
END;

p_returnvalue :=1;
END dpa_del_ut_ruolo_in_mod_trasm;
/

begin
    declare
           cnt int;
    begin	
    select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_CHIAVI_CONFIG';
		if (cnt = 0) then
      		 execute immediate 'CREATE SEQUENCE SEQ_DPA_CHIAVI_CONFIG START WITH 1 INCREMENT BY 1 MINVALUE 1';
		end if;	
		
		
	select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
		if (cnt = 0) then
			execute immediate	
				'CREATE TABLE DPA_CHIAVI_CONFIGURAZIONE ' ||
				'( ' ||
				'  SYSTEM_ID 		  					NUMBER NOT NULL , ' ||
				'  ID_AMM 			  					NUMBER, '           ||
				'  VAR_CODICE              				VARCHAR2(32)  NOT NULL , '     ||
                '  VAR_DESCRIZIONE              		VARCHAR2(256), '    ||
                '  VAR_VALORE              				VARCHAR2(128)  NOT NULL , '    ||
				'  CHA_TIPO_CHIAVE  					VARCHAR2(1), '      ||
				'  CHA_VISIBILE   					    VARCHAR2(1) DEFAULT 1 NOT NULL, '      ||
                '  CHA_MODIFICABILE  					VARCHAR2(1) DEFAULT 1 NOT NULL, '      ||	
                '  CHA_GLOBALE       					VARCHAR2(1)  DEFAULT 1 NOT NULL '      ||
				') TABLESPACE @ora_dattblspc_name';
		end if;
    end;
end;   
/       

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_LOG_PATH' and ID_AMM IS NULL;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, null, 'FE_LOG_PATH', 'path del log del FrontEnd', 'c:/docspa30/logs/frontend/',
    'F','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_LOG_PATH' and ID_AMM IS NULL);
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_LOG_LEVEL' and ID_AMM IS NULL;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, null, 'FE_LOG_LEVEL', 'flag attivazione log del FrontEnd (0=disattivato; 2= attivato)', '2',
    'F','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_LOG_LEVEL' and ID_AMM IS NULL);
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='BE_LOG_PATH' and ID_AMM IS NULL;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, null, 'BE_LOG_PATH', 'path del log del BackEnd', 'c:/docspa30/logs/backend/',
    'B','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='BE_LOG_PATH' and ID_AMM IS NULL);
	end if;
	end;
end;
/    

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='BE_LOG_LEVEL' and ID_AMM IS NULL;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, null, 'BE_LOG_LEVEL', 'flag attivazione log del BackEnd (0=disattivato; 2= attivato)', '2',
    'B','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='BE_LOG_LEVEL' and ID_AMM IS NULL);
	end if;
	end;
end;
/       
    
begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_LOG_PATH' and ID_AMM = 0;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, 0, 'FE_LOG_PATH', 'path del log del FrontEnd', 'c:/docspa30/logs/frontend/',
    'F','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_LOG_PATH' and ID_AMM = 0);
	end if;
	end;
end;
/
    
begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_LOG_LEVEL' and ID_AMM = 0;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, 0, 'FE_LOG_LEVEL', 'flag attivazione log del FrontEnd (0=disattivato; 2= attivato)', '2',
    'F','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='FE_LOG_LEVEL' and ID_AMM = 0);
	end if;
	end;
end;
/
         
begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='BE_LOG_PATH' and ID_AMM = 0;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, 0, 'BE_LOG_PATH', 'path del log del BackEnd', 'c:/docspa30/logs/backend/',
    'B','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='BE_LOG_PATH' and ID_AMM = 0);
	end if;
	end;
end;
/ 

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='BE_LOG_LEVEL' and ID_AMM = 0;
	if (cnt = 0) then		
	     INSERT INTO DPA_CHIAVI_CONFIGURAZIONE 
   (SYSTEM_ID, ID_AMM, VAR_CODICE, VAR_DESCRIZIONE, VAR_VALORE,
    CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, CHA_GLOBALE)
  	SELECT
    SEQ_DPA_CHIAVI_CONFIG.NEXTVAL, 0, 'BE_LOG_LEVEL', 'flag attivazione log del BackEnd (0=disattivato; 2= attivato)', '2',
    'B','1','1', '1'
  	FROM DUAL WHERE NOT EXISTS (SELECT * FROM DPA_CHIAVI_CONFIGURAZIONE WHERE VAR_CODICE='BE_LOG_LEVEL' and ID_AMM = 0 );
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_PROTO_TIT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_PROTO_TIT' and column_name='ID_FASC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_PROTO_TIT ADD ID_FASC NUMBER NULL';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_PROTO_TIT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_PROTO_TIT' and column_name='NUM_DOC_IN_FASC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_PROTO_TIT ADD NUM_DOC_IN_FASC NUMBER NULL';
        end if;
    end if;
    end;
end;
/
 
 begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='SPEDIZIONE_AUTO_DOC';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD SPEDIZIONE_AUTO_DOC CHAR(1) NULL';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='AVVISA_SPEDIZIONE_DOC';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD AVVISA_SPEDIZIONE_DOC CHAR(1) NULL';
        end if;
    end;
end;
/


CREATE OR REPLACE PROCEDURE CREATEALLEGATO 
(
p_idDocumentoPrincipale int,
p_idPeople int,
p_comments nvarchar2,
p_numeroPagine int,
p_idProfile out int,
p_versionId out int
)
IS returnvalue NUMBER;
idDocType INT := 0;

BEGIN
returnvalue := 0;
SELECT DOCUMENTTYPE INTO idDocType
FROM PROFILE WHERE SYSTEM_ID = p_idDocumentoPrincipale;

SELECT SEQ.nextval INTO p_idProfile FROM dual;

INSERT INTO Profile
(
SYSTEM_ID,
DOCNUMBER,
TYPIST,
AUTHOR,
CHA_TIPO_PROTO,
CHA_DA_PROTO,
DOCUMENTTYPE,
CREATION_DATE,
CREATION_TIME,
ID_DOCUMENTO_PRINCIPALE
)
VALUES
(
p_idProfile,
p_idProfile,
p_idpeople,
p_idpeople,
'G',
'0',
idDocType,
SYSDATE,
SYSDATE,
p_idDocumentoPrincipale
);

returnvalue := SQL%ROWCOUNT;

IF (returnvalue > 0) THEN

SELECT SEQ.nextval INTO p_versionId FROM dual;

INSERT INTO VERSIONS
(
VERSION_ID,
DOCNUMBER,
VERSION,
SUBVERSION,
VERSION_LABEL,
AUTHOR,
TYPIST,
COMMENTS,
NUM_PAG_ALLEGATI,
DTA_CREAZIONE,
CHA_DA_INVIARE
)
VALUES
(
p_versionId,
p_idProfile,
1,
'!',
'1',
p_idPeople,
p_idPeople,
p_comments,
p_numeroPagine,
SYSDATE,
'1'
);

INSERT INTO COMPONENTS
(
VERSION_ID,
DOCNUMBER,
FILE_SIZE
)
VALUES
(
p_versionId,
p_idProfile,
0
);

END IF;

END createAllegato;
/

-- INSERIMENTO NUOVI OGGETTI DI LOGGING
begin
    declare cnt int;
    
    begin
        -- Inserimento voce per il log delle operazioni di creazione corrispondente
        select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='RUB_IMP_NEWCORR';
        if (cnt = 0) then        
            INSERT INTO DPA_ANAGRAFICA_LOG(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
            VALUES (seq.nextval, 'RUB_IMP_NEWCORR','Importa rubrica, creazione nuovo corrispondente', 'RUBRICA', 'IMPORTARUBRICACREA');
        end if;
        
        -- Inserimento voce per il log delle operazioni di modifica corrispondente
        select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='RUB_IMP_MODCORR';
        if (cnt = 0) then
            INSERT INTO DPA_ANAGRAFICA_LOG(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
            VALUES (seq.nextval, 'RUB_IMP_MODCORR','Importa rubrica, modifica corrispondente', 'RUBRICA', 'IMPORTARUBRICAMODIFICA');    
        end if;
    
        -- Inserimento voce per il log delle operazioni di cancellazione corrispondente
        select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='RUB_IMP_DELCORR';
        if (cnt = 0) then
            INSERT INTO DPA_ANAGRAFICA_LOG(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
            VALUES (seq.nextval, 'RUB_IMP_DELCORR','Importa rubrica, cancellazione corrispondente', 'RUBRICA', 'IMPORTARUBRICACANCELLA');
        end if;
    
        -- Inserimento voce per il log di eccezioni durante l'importazione della rubrica
        select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='RUB_IMP_EXCCORR';
        if (cnt = 0) then
            INSERT INTO DPA_ANAGRAFICA_LOG(SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
            VALUES (seq.nextval, 'RUB_IMP_EXCCORR','Importa rubrica, procedura di importazione', 'RUBRICA', 'IMPORTARUBRICAEXCEPTION');
        END IF;
    END;
end;
/

-- Inserimento voce per il logging del cambio pwd
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='USR_MOD_PASS';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_LOG (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
       VALUES (SEQ.NEXTVAL,'USR_MOD_PASS', 'Modifica della password da parte dell''amministratore o dell''utente', 'UTENTE', 'MODUSER');
    end if;
    end;
end;
/

-- Inserimento voce per il logging della modifica o cancellazione del corrispondente
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='RUB_MOD_CANC_CORR';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_LOG (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
       VALUES (SEQ.NEXTVAL,'RUB_MOD_CANC_CORR', 'Modifica o cancellazione di un corrispondente dalla rubrica', 'RUBRICA', 'CORRISPONDENTIDELETECORRISPONDENTEESTERNO');
    end if;
    end;
end;
/

-- INIZIO FRIEND APPLICATION
begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_FRIEND_APPLICATION';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_FRIEND_APPLICATION START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_FRIEND_APPLICATION';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_FRIEND_APPLICATION ' ||
            '( ' ||
			'SYSTEM_ID NUMBER, ' ||
			'COD_APPLICAZIONE VARCHAR(256), ' ||
			'COD_REGISTRO VARCHAR(16), ' ||
			'ID_REGISTRO NUMBER, ' ||
			'CONSTRAINT PK_DPA_FRIEND_APPLICATION PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/ 

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' AND column_name='ID_PEOPLE_FACTORY';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD ID_PEOPLE_FACTORY NUMBER';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' AND column_name='ID_GRUPPO_FACTORY';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD ID_GRUPPO_FACTORY NUMBER';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='FACTORY_USER';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD FACTORY_USER CHAR(1)';
        end if;
    end;
end;
/
-- FINE FRIEND APPLICATION

-- Nuova var describe
CREATE OR REPLACE FUNCTION vardescribe 
(sysid INT, typetable VARCHAR)
   RETURN VARCHAR2
IS
   risultato   VARCHAR2 (8000);
   tmpvar      VARCHAR2 (8000);
   tipo        CHAR;
   num_proto   NUMBER;
   doc_number  NUMBER;
BEGIN
   BEGIN
      tmpvar := NULL;

--MAIN
      IF (typetable = 'PEOPLENAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_people = sysid AND cha_tipo_urp = 'P' AND cha_tipo_ie = 'I';
      END IF;

      IF (typetable = 'GROUPNAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE system_id = sysid AND cha_tipo_urp = 'R';
      END IF;

      IF (typetable = 'DESC_RUOLO')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_gruppo = sysid AND cha_tipo_urp = 'R';
      END IF;

      IF (typetable = 'RAGIONETRASM')
      THEN
         SELECT var_desc_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'TIPO_RAGIONE')
      THEN
         SELECT cha_tipo_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DATADOC')
      THEN
         BEGIN
            SELECT cha_tipo_proto, NVL (num_proto, 0)
              INTO tipo, num_proto
              FROM PROFILE
             WHERE system_id = sysid;

            IF (    tipo IS NOT NULL
                AND (tipo IN ('A', 'P', 'I') AND num_proto != 0)
               )
            THEN
               SELECT TO_CHAR (dta_proto, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            ELSE
               SELECT TO_CHAR (creation_date, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;

      IF (typetable = 'CHA_TIPO_PROTO')
      THEN
         SELECT cha_tipo_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'NUMPROTO')
      THEN
         SELECT num_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'CODFASC')
      THEN
         SELECT var_codice
           INTO risultato
           FROM project
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DTA_CREAZ')
      THEN
         SELECT TO_CHAR (dta_creazione, 'yyyy')
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'NUM_FASC')
      THEN
         SELECT num_fascicolo
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DESC_OGGETTO')
      THEN
         SELECT var_prof_oggetto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DESC_FASC')
      THEN
         BEGIN
            SELECT description
              INTO risultato
              FROM project
             WHERE system_id = sysid;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '';
         END;
      END IF;

      IF (typetable = 'PROF_IDREG')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_registro
                    INTO risultato
                    FROM PROFILE
                   WHERE system_id = sysid;

                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;

      IF (typetable = 'ID_GRUPPO')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_gruppo
                    INTO risultato
                    FROM dpa_corr_globali
                   WHERE system_id = sysid;

                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;

      IF (typetable = 'SEGNATURA_DOCNUMBER')
      THEN
         BEGIN
            SELECT var_segnatura
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;
      
        IF (typetable = 'SEGNATURA_CODFASC')
      THEN
         BEGIN
            SELECT num_proto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;

      IF (typetable = 'OGGETTO_MITTENTE')
      THEN
         BEGIN
-- OGGETTO
            SELECT var_prof_oggetto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

--MITTENTE
            BEGIN
               SELECT var_desc_corr
                 INTO tmpvar
                 FROM (SELECT var_desc_corr
                         FROM dpa_corr_globali a, dpa_doc_arrivo_par b
                        WHERE b.id_mitt_dest = a.system_id
                          AND b.cha_tipo_mitt_dest = 'M'
                          AND b.id_profile = sysid)
                WHERE ROWNUM = 1;
            EXCEPTION
               WHEN NO_DATA_FOUND
               THEN
                  tmpvar := '';
            END;

            IF (tmpvar IS NOT NULL)
            THEN
               risultato := risultato || '@@' || tmpvar;
            END IF;
         END;
      END IF;

      IF (typetable = 'PROFILE_CHA_IMG')
      THEN
         SELECT getchaimg (docnumber)
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'PROFILE_CHA_FIRMATO')
      THEN
         SELECT CHA_FIRMATO
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
 
--ENDMAIN
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         NULL;
      WHEN OTHERS
      THEN
         RAISE;
   END;
   RETURN risultato;
end;
/
--fine vardescribe nuova

CREATE OR REPLACE PROCEDURE SP_INSERT_DPA_SUPPORTO
(
p_copia             number,
p_collFisica        VARCHAR,
p_dataUltimaVer     DATE,
p_dataEliminazione  DATE,
p_esitoUltimaVer    NUMBER,
p_numeroVer         NUMBER,
p_dataProxVer       DATE,
p_dataAppoMarca     DATE,
p_dataScadMarca     DATE,
p_marca             VARCHAR,
p_idCons            NUMBER,
p_tipoSupp          NUMBER,
p_stato             CHAR,
p_note              VARCHAR,
p_query             CHAR,
p_idSupp            NUMBER,
p_percVerifica      NUMBER,
p_progressivoMarca  NUMBER,
p_result        OUT NUMBER,
p_newId         OUT NUMBER
)

IS
numSuppProd NUMBER:=0;
numSuppTotali NUMBER:=0;
numSupporto   NUMBER:=0;

BEGIN

SELECT SEQ_SUPPORTO.nextval into numSupporto from dual;

IF(p_query='I') THEN

INSERT INTO DPA_SUPPORTO (
  SYSTEM_ID,               
  COPIA,                    
  DATA_PRODUZIONE,         
  VAR_COLLOCAZIONE_FISICA,  
  DATA_ULTIMA_VERIFICA,     
  DATA_ELIMINAZIONE,        
  ESITO_ULTIMA_VERIFICA,    
  VERIFICHE_EFFETTUATE,   
  DATA_PROX_VERIFICA,      
  DATA_APPO_MARCA,          
  DATA_SCADENZA_MARCA,      
  VAR_MARCA_TEMPORALE,     
  ID_CONSERVAZIONE,         
  ID_TIPO_SUPPORTO,        
  CHA_STATO,                
  VAR_NOTE,
  PERC_VERIFICA,
  NUM_MARCA               
)
VALUES(
numSupporto,
p_copia,
SYSDATE,
p_collFisica,
p_dataUltimaVer,
p_dataEliminazione,
p_esitoUltimaVer,
p_numeroVer,
p_dataProxVer,
p_dataAppoMarca,
p_dataScadMarca,
p_marca,
p_idCons,
p_tipoSupp,
p_stato,
p_note,
p_percVerifica,
p_progressivoMarca
);

p_newId:=numSupporto;

SELECT COUNT(*) INTO numSuppProd  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=p_idCons;

SELECT COUNT(*) INTO numSuppTotali  FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=p_idCons;

IF(numSuppProd=numSuppTotali) THEN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=p_idCons; 

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=p_idCons;

p_result:=1;
ELSE
p_result:=0;
END IF;


ELSE

UPDATE DPA_SUPPORTO SET DATA_PRODUZIONE=SYSDATE, VAR_COLLOCAZIONE_FISICA=p_collFisica,
DATA_PROX_VERIFICA=p_dataProxVer, CHA_STATO= p_stato, VAR_NOTE=p_note, DATA_ULTIMA_VERIFICA=SYSDATE, VERIFICHE_EFFETTUATE=p_numeroVer, ESITO_ULTIMA_VERIFICA=p_esitoUltimaVer, PERC_VERIFICA=p_percVerifica WHERE SYSTEM_ID=p_idSupp;

p_newId:=p_idSupp;

SELECT COUNT(*) INTO numSuppProd  FROM DPA_SUPPORTO WHERE (CHA_STATO='P' OR CHA_STATO='E' OR CHA_STATO='V') AND ID_CONSERVAZIONE=
(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

SELECT COUNT(*) INTO numSuppTotali  FROM DPA_SUPPORTO WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

IF(numSuppProd=numSuppTotali) THEN

UPDATE DPA_AREA_CONSERVAZIONE SET CHA_STATO='C' WHERE SYSTEM_ID=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp);

UPDATE DPA_ITEMS_CONSERVAZIONE SET CHA_STATO='C' WHERE ID_CONSERVAZIONE=(SELECT ID_CONSERVAZIONE FROM DPA_SUPPORTO WHERE SYSTEM_ID=p_idSupp); 

p_result:=1;
ELSE
p_result:=0;
END IF;

END IF;
END;
/

/* Formatted on 2009/12/11 14:43 (Formatter Plus v4.8.8) */
CREATE OR REPLACE PROCEDURE createallegato (
   p_iddocumentoprincipale         INT,
   p_idpeople                      INT,
   p_comments                      NVARCHAR2,
   p_numeropagine                  INT,
   p_idpeopledelegato              INT,
   p_idprofile               OUT   INT,
   p_versionid               OUT   INT
)
IS
   returnvalue   NUMBER;
/******************************************************************************

NAME:       createAllegato

PURPOSE:    Creazione di un nuovo documento di tipo allegato

******************************************************************************/
   iddoctype     INT    := 0;
BEGIN
   returnvalue := 0;

-- Reperimento tipologia atto del documento principale
   SELECT documenttype
     INTO iddoctype
     FROM PROFILE
    WHERE system_id = p_iddocumentoprincipale;

-- Reperimento identity
   SELECT seq.NEXTVAL
     INTO p_idprofile
     FROM DUAL;

   INSERT INTO PROFILE
               (system_id, docnumber, typist, author, cha_tipo_proto,
                cha_da_proto, documenttype, creation_date, creation_time,
                id_documento_principale, id_people_delegato,var_chiave_proto
               )
        VALUES (p_idprofile, p_idprofile, p_idpeople, p_idpeople, 'G',
                '0', iddoctype, SYSDATE, SYSDATE,
                p_iddocumentoprincipale, p_idpeopledelegato,to_char(p_idprofile)
               );

   returnvalue := SQL%ROWCOUNT;

   IF (returnvalue > 0)
   THEN
-- Reperimento identity
      SELECT seq.NEXTVAL
        INTO p_versionid
        FROM DUAL;

-- Inserimento record in tabella VERSIONS
      INSERT INTO VERSIONS
                  (version_id, docnumber, VERSION, subversion, version_label,
                   author, typist, comments, num_pag_allegati,
                   dta_creazione, cha_da_inviare, id_people_delegato
                  )
           VALUES (p_versionid, p_idprofile, 1, '!', '1',
                   p_idpeople, p_idpeople, p_comments, p_numeropagine,
                   SYSDATE, '1', p_idpeopledelegato
                  );

-- Inserimento record in tabella COMPONENTS
      INSERT INTO components
                  (version_id, docnumber, file_size
                  )
           VALUES (p_versionid, p_idprofile, 0
                  );
   END IF;
END createallegato;
/

CREATE OR REPLACE PROCEDURE spsetdatavista (
   p_idpeople      IN       NUMBER,
   p_idoggetto     IN       NUMBER,
   p_idgruppo      IN       NUMBER,
   p_tipooggetto   IN       CHAR,
   p_iddelegato    IN       NUMBER,
   p_resultvalue   OUT      NUMBER
)
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
 p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;

      CURSOR cursortrasmsingolafascicolo
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_project = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ¿ una trasmissione senza workFlow
                  if (p_iddelegato = 0)
                  then
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            dpa_trasm_utente.cha_in_todolist = '0'
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  else
                  --in caso di delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                            dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            dpa_trasm_utente.cha_in_todolist = '0'
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  end if;
                  begin
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        if (p_iddelegato = 0)
                        then
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           UPDATE dpa_trasm_utente
                              SET dpa_trasm_utente.cha_vista = '1',
                                  dpa_trasm_utente.cha_in_todolist = '0'
                            WHERE dpa_trasm_utente.dta_vista IS NULL
                              AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                              AND dpa_trasm_utente.id_people != p_idpeople;
                        EXCEPTION
                           WHEN OTHERS
                           THEN
                              p_resultvalue := 1;
                              RETURN;
                        END;
                        else
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           UPDATE dpa_trasm_utente
                              SET dpa_trasm_utente.cha_vista = '1',
                                  dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                                  dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                                  dpa_trasm_utente.cha_in_todolist = '0'
                            WHERE dpa_trasm_utente.dta_vista IS NULL
                              AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                              AND dpa_trasm_utente.id_people != p_idpeople;
                        EXCEPTION
                           WHEN OTHERS
                           THEN
                              p_resultvalue := 1;
                              RETURN;
                        END;
                        end if;
                     END IF;
                  END;
               ELSE
-- la ragione di trasmissione prevede workflow
                  if (p_iddelegato = 0)
                  then
                  BEGIN
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               )
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                    EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;                        
                  END;
                  ELSE
                  --in caso di delega
                  BEGIN
                    UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.cha_vista_delegato = '1',
                            dpa_trasm_utente.id_people_delegato = p_iddelegato,
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               )
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                    EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;                        
                  END;
                  end if;
                    
                  BEGIN
                     -- Rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
                     UPDATE     dpa_trasm_utente
                     SET        cha_in_todolist = '0'
                     WHERE      id_trasm_singola = currenttrasmsingola.system_id 
                            AND NOT  dpa_trasm_utente.dta_vista IS NULL
                            AND (cha_accettata = '1' OR cha_rifiutata = '1');
                            --AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople
                        AND id_profile = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;

      IF (p_tipooggetto = 'F')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingolafascicolo
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ¿ una trasmissione senza workFlow
                   
                  if (p_iddelegato = 0)
                  then 
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            dpa_trasm_utente.cha_in_todolist = '0'
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  else
                  --caso in cui si sta esercitando una delega
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                            dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            dpa_trasm_utente.cha_in_todolist = '0'
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  end if;
                  begin
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest =
                               p_idpeople
                                         --and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
                     if (p_iddelegato = 0)
                     then 
-- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           UPDATE dpa_trasm_utente
                              SET dpa_trasm_utente.cha_vista = '1',
                                  dpa_trasm_utente.cha_in_todolist = '0'
                            WHERE dpa_trasm_utente.dta_vista IS NULL
                              AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                              AND dpa_trasm_utente.id_people != p_idpeople;
                        EXCEPTION
                           WHEN OTHERS
                           THEN
                              p_resultvalue := 1;
                              RETURN;
                        END;
                        else
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           UPDATE dpa_trasm_utente
                              SET dpa_trasm_utente.cha_vista = '1',
                                  dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                                  dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                                  dpa_trasm_utente.cha_in_todolist = '0'
                            WHERE dpa_trasm_utente.dta_vista IS NULL
                              AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                              AND dpa_trasm_utente.id_people != p_idpeople;
                        EXCEPTION
                           WHEN OTHERS
                           THEN
                              p_resultvalue := 1;
                              RETURN;
                        END;
                        end if;
                     END IF;
                  END;
               ELSE
-- se la ragione di trasmissione prevede workflow
                  if (p_iddelegato =0)
                  then
                  BEGIN
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               )
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                    EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  else
                  -- caso in cui si sta esercitando una delega
                  BEGIN
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.CHA_VISTA_DELEGATO = '1',
                            dpa_trasm_utente.ID_PEOPLE_DELEGATO = p_iddelegato,
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               )
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;
                    EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;
                  end if;
                  begin
                     -- Rimozione trasmissione da todolist solo se è stata già accettata o rifiutata
                     UPDATE     dpa_trasm_utente
                     SET        cha_in_todolist = '0'
                     WHERE      id_trasm_singola = currenttrasmsingola.system_id 
                            AND NOT  dpa_trasm_utente.dta_vista IS NULL
                            AND (cha_accettata = '1' OR cha_rifiutata = '1')
                            AND dpa_trasm_utente.id_people = p_idpeople;
                            
                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest =
                               p_idpeople
                                         --and ID_RUOLO_DEST in (p_idGruppo,0)
                        AND id_project = p_idoggetto;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
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


begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_FRIEND_APPLICATION';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_FRIEND_APPLICATION START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_FRIEND_APPLICATION';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_FRIEND_APPLICATION ' ||
            '( ' ||
			'SYSTEM_ID NUMBER, ' ||
			'COD_APPLICAZIONE VARCHAR(256), ' ||
			'COD_REGISTRO VARCHAR(16), ' ||
			'ID_REGISTRO NUMBER, ' ||
			'CONSTRAINT PK_DPA_FRIEND_APPLICATION PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/ 

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' AND column_name='ID_PEOPLE_FACTORY';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD ID_PEOPLE_FACTORY NUMBER';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_EL_REGISTRI' AND column_name='ID_GRUPPO_FACTORY';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_EL_REGISTRI ADD ID_GRUPPO_FACTORY NUMBER';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='FACTORY_USER';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD FACTORY_USER CHAR(1)';
        end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TRASM_UTENTE' and column_name='CHA_VISTA_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TRASM_UTENTE ADD (CHA_VISTA_DELEGATO VARCHAR2(1) DEFAULT 0 NOT NULL)';
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TRASM_UTENTE' and column_name='CHA_ACCETTATA_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TRASM_UTENTE ADD (CHA_ACCETTATA_DELEGATO VARCHAR2(1) DEFAULT 0 NOT NULL)';
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TRASM_UTENTE' and column_name='CHA_RIFIUTATA_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TRASM_UTENTE ADD (CHA_RIFIUTATA_DELEGATO VARCHAR2(1) DEFAULT 0 NOT NULL)';
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_PEOPLE_DELEGATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROJECT ADD ID_PEOPLE_DELEGATO NUMBER(10)';
    end if;
    end;
end;
/

CREATE OR REPLACE PROCEDURE createprojectsp (
   p_idpeople                 NUMBER,
   p_description              VARCHAR,
   p_idpeopledelegato         NUMBER,
   p_projectid          OUT   NUMBER
)
IS
BEGIN
   DECLARE
      projid   NUMBER;
   BEGIN
      p_projectid := 0;

      SELECT seq.NEXTVAL
        INTO projid
        FROM DUAL;

      p_projectid := projid;

      <<inserimento_in_project>>
      BEGIN
         INSERT INTO project
                     (system_id, description, iconized, id_people_delegato
                     )
              VALUES (p_projectid, p_description, 'Y', p_idpeopledelegato
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_projectid := 0;
            RETURN;
      END inserimento_in_project;

      <<inserimento_security>>
      BEGIN
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (p_projectid, p_idpeople, 0, NULL,
                      NULL
                     );
      EXCEPTION
         WHEN OTHERS
         THEN
            p_projectid := 0;
            RETURN;
      END inserimento_security;
   END;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_MAIL_ELABORATE' and column_name='ID_PEOPLE';
    if (cnt != 0) then        
            execute immediate 'ALTER TABLE DPA_MAIL_ELABORATE DROP COLUMN ID_PEOPLE';
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_MAIL_ELABORATE' and column_name='VAR_NOTE';
    if (cnt != 0) then        
            execute immediate 'ALTER TABLE DPA_MAIL_ELABORATE DROP COLUMN VAR_NOTE';
	end if;
	end;
end;
/

CREATE OR REPLACE PROCEDURE @db_user.i_smistamento_smistadoc (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idpeopledestinatario           IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-- -5: Errore in SPsetDataVistaSmistamento
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   identitytrasmsing   NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   resultvalue         NUMBER;   
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing, cha_tipo_trasm,
                   dta_scadenza, id_trasm_utente
                  )
           VALUES (identitytrasmsing, idragionetrasm, identitytrasm, 'U',
                   idcorrglobaledestinatario, noteindividuali, 'S',
                   datascadenza, NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO dpa_trasm_utente
                  (system_id, id_trasm_singola, id_people,
                   dta_vista, dta_accettata, dta_rifiutata, dta_risposta,
                   cha_vista, cha_accettata, cha_rifiutata, var_note_acc,
                   var_note_rif, cha_valida, id_trasm_risp_sing
                  )
           VALUES (seq.NEXTVAL, identitytrasmsing, idpeopledestinatario,
                   NULL, NULL, NULL, NULL,
                   '0', '0', '0', NULL,
                   NULL, '1', NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;

   BEGIN
--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
      UPDATE dpa_trasmissione
         SET dta_invio = SYSDATE ()
       WHERE system_id = identitytrasm;

      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento
                 AND personorgroup = idpeopledestinatario)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idpeopledestinatario
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights,
                      id_gruppo_trasm, cha_tipo_diritto
                     )
              VALUES (iddocumento, idpeopledestinatario, rights,
                      idgruppomittente, tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;            
        
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;

            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta già accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',                        
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;

   returnvalue := 0;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   systrasmsing        NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   idutente            NUMBER;
   recordcorrente      NUMBER;
   idgroups            NUMBER   := NULL;
   idgruppo            NUMBER;
   resultvalue         NUMBER;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;    
      
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;
            
            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta già accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso risulta visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;
   

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.spsetdatavistasmistamento (
   p_idpeople         IN       NUMBER,
   p_idoggetto        IN       NUMBER,
   p_idgruppo         IN       NUMBER,
   p_tipooggetto      IN       CHAR,
   p_idtrasmissione   IN       NUMBER,
   p_resultvalue      OUT      NUMBER
)
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
   p_cha_tipo_trasm   CHAR (1) := NULL;
   p_chatipodest      NUMBER;
BEGIN
   p_resultvalue := 0;

   DECLARE
      CURSOR cursortrasmsingoladocumento
      IS
         SELECT b.system_id, b.cha_tipo_trasm, c.cha_tipo_ragione,
                b.cha_tipo_dest
           FROM dpa_trasmissione a, dpa_trasm_singola b, dpa_ragione_trasm c
          WHERE a.system_id = p_idtrasmissione
            AND a.dta_invio IS NOT NULL
            AND a.system_id = b.id_trasmissione
            AND (   b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_gruppo = p_idgruppo)
                 OR b.id_corr_globale = (SELECT system_id
                                           FROM dpa_corr_globali
                                          WHERE id_people = p_idpeople)
                )
            AND a.id_profile = p_idoggetto
            AND b.id_ragione = c.system_id;
   BEGIN
      IF (p_tipooggetto = 'D')
      THEN
         FOR currenttrasmsingola IN cursortrasmsingoladocumento
         LOOP
            BEGIN
               IF (   currenttrasmsingola.cha_tipo_ragione = 'N'
                   OR currenttrasmsingola.cha_tipo_ragione = 'I'
                  )
               THEN
-- SE ¿ una trasmissione senza workFlow
                  BEGIN
-- nella trasmissione utente relativa all'utente che sta vedendo il documento
-- setto la data di vista
                     UPDATE dpa_trasm_utente
                        SET dpa_trasm_utente.cha_vista = '1',
                            dpa_trasm_utente.dta_vista =
                               (CASE
                                   WHEN dta_vista IS NULL
                                      THEN SYSDATE
                                   ELSE dta_vista
                                END
                               ),
                            dpa_trasm_utente.cha_in_todolist = '0'
                      WHERE dpa_trasm_utente.dta_vista IS NULL
                        AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                        AND dpa_trasm_utente.id_people = p_idpeople;

                     UPDATE dpa_todolist
                        SET dta_vista = SYSDATE
                      WHERE id_trasm_singola = currenttrasmsingola.system_id
                        AND id_people_dest = p_idpeople;
                  EXCEPTION
                     WHEN OTHERS
                     THEN
                        p_resultvalue := 1;
                        RETURN;
                  END;

                  BEGIN
                     IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                         AND currenttrasmsingola.cha_tipo_dest = 'R'
                        )
                     THEN
-- se ¿ una trasmissione ¿ di tipo SINGOLA a un RUOLO allora devo aggiornare
-- anche le trasmissioni singole relative agli altri utenti del ruolo
                        BEGIN
-- nelle trasmissioni utente relative agli altri utenti del ruolo
-- non si setta la data di vista ma si eliminano semplicemente dalla ToDoList
                           UPDATE dpa_trasm_utente
                              SET dpa_trasm_utente.cha_vista = '1',
                                  dpa_trasm_utente.cha_in_todolist = '0'
                            WHERE dpa_trasm_utente.dta_vista IS NULL
                              AND id_trasm_singola =
                                              (currenttrasmsingola.system_id
                                              )
                              AND dpa_trasm_utente.id_people != p_idpeople;
                        EXCEPTION
                           WHEN OTHERS
                           THEN
                              p_resultvalue := 1;
                              RETURN;
                        END;
                     END IF;
                  END;
               ELSE
                  BEGIN
-- la ragione di trasmissione prevede workflow
                     BEGIN
                        UPDATE dpa_trasm_utente
                           SET dpa_trasm_utente.cha_vista = '1',
                               dpa_trasm_utente.dta_vista =
                                  (CASE
                                      WHEN dta_vista IS NULL
                                         THEN SYSDATE
                                      ELSE dta_vista
                                   END
                                  ),
                               dta_accettata =
                                  (CASE
                                      WHEN dta_accettata IS NULL
                                         THEN SYSDATE
                                      ELSE dta_accettata
                                   END
                                  ),
                               cha_accettata = '1',
                               cha_valida = '0',
                               cha_in_todolist = '0'
                         WHERE dpa_trasm_utente.dta_vista IS NULL
                           AND id_trasm_singola =
                                                 currenttrasmsingola.system_id
                           AND dpa_trasm_utente.id_people = p_idpeople;

                        UPDATE dpa_todolist
                           SET dta_vista = SYSDATE
                         WHERE id_trasm_singola =
                                                 currenttrasmsingola.system_id
                           AND id_people_dest = p_idpeople;
                     EXCEPTION
                        WHEN OTHERS
                        THEN
                           p_resultvalue := 1;
                           RETURN;
                     END;

                     BEGIN
-- se la trasm ¿ con WorkFlow ed ¿ di tipo UNO e il dest ¿ Ruolo allora levo la validit¿ della
-- trasmissione a tutti gli altri utenti del ruolo
                        IF (    currenttrasmsingola.cha_tipo_trasm = 'S'
                            AND currenttrasmsingola.cha_tipo_dest = 'R'
                           )
                        THEN
                           BEGIN
                              UPDATE dpa_trasm_utente
                                 SET dpa_trasm_utente.cha_valida = '0',
                                     dpa_trasm_utente.cha_in_todolist = '0'
                               WHERE
-- DPA_TRASM_UTENTE.DTA_VISTA IS NULL
                                     id_trasm_singola =
                                                 currenttrasmsingola.system_id
                                 AND dpa_trasm_utente.id_people != p_idpeople;
                           EXCEPTION
                              WHEN OTHERS
                              THEN
                                 p_resultvalue := 1;
                                 RETURN;
                           END;
                        END IF;
                     END;
                  END;
               END IF;
            END;
         END LOOP;
      END IF;
   END;
END spsetdatavistasmistamento;
/

--nuovi per cha_firmato
begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='components' and Column_name='CHA_FIRMATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE components ADD CHA_FIRMATO VARCHAR(1) DEFAULT 0';
    end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION getchafirmato (docnum number)
   RETURN VARCHAR2
IS
   isFirmato   VARCHAR2 (16);
BEGIN
    
     DECLARE
      vmaxidgenerica   NUMBER;
   
   BEGIN
      BEGIN
         SELECT MAX (v1.version_id)
           INTO vmaxidgenerica
           FROM VERSIONS v1, components c
          WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
      EXCEPTION
         WHEN OTHERS
         THEN
            vmaxidgenerica := 0;
      END;

      BEGIN
        SELECT cha_firmato
           INTO isFirmato
           FROM components
          WHERE docnumber = docnum AND version_id = vmaxidgenerica;
      EXCEPTION
         WHEN OTHERS
         THEN
            isFirmato := '0';
      END;

  END;

   RETURN isFirmato;
END getchafirmato;
/

CREATE OR REPLACE PROCEDURE putFile
(
p_versionId int,
p_filePath nvarchar2,
p_fileSize int,
p_printThumb nvarchar2,
p_iscartaceo smallint,
p_estensione varchar,
p_isFirmato char 
)
IS retValue NUMBER;
/******************************************************************************
   NAME:       putFile
   PURPOSE:    Impostazione dei dati per l'inserimento di un nuovo file

******************************************************************************/

docNum int := 0;

BEGIN
   retValue := 0;
   
   -- 1) Aggiornamento tabella VERSIONS
   --    vengono aggiornati i campi della tabella che identificano una versione acquisita
    
    -- Reperimento docnumber del documento
    select docnumber into docNum from versions where version_id = p_versionId;
        
    -- Aggiornamento stato acquisito della versione
    update     versions
    set     subversion = 'A',
          cartaceo = p_iscartaceo
    where     version_id = p_versionId;
   
    retValue := SQL%ROWCOUNT;
   
   -- 2) Aggiornamento tabella COMPONENTS
   
   if (retValue > 0) then
   -- Aggiornamento stato acquisito della versione
   if (p_isFirmato='1') then
        update components
         set  path = p_filePath,
             file_size = p_fileSize,
             var_impronta = p_printThumb,
             ext =p_estensione,
             cha_firmato = '1'
            where     version_id = p_versionId;
     else
        update components
        set     path = p_filePath,
             file_size = p_fileSize,
             var_impronta = p_printThumb,
             ext =p_estensione,
             cha_firmato = '0'
        where     version_id = p_versionId;
      end if;
        
    retValue := SQL%ROWCOUNT;
   end if;
   
   -- 3) Aggiornamento tabella PROFILE    
   if (retValue > 0) then
   if (p_isFirmato='1') then
        update     profile
        set     cha_img = '1', cha_firmato = '1'
        where    docnumber = docNum;
         else
         update     profile
        set     cha_img = '1', cha_firmato = '0'
        where    docnumber = docNum;
        end if;
    retValue := SQL%ROWCOUNT;
   end if;
   
END putFile;
/

CREATE OR REPLACE PROCEDURE sp_ins_ext_in_components
IS
   profile_docnumber   NUMBER;
   estensione          VARCHAR (30);
   id_version          NUMBER;
   id_last_version     NUMBER;
   estensioneappo      VARCHAR (30);
   indice number;

   CURSOR curr_components
   IS
      SELECT docnumber, version_id,
             SUBSTR (PATH, LENGTH (PATH) - INSTR (REVERSE (PATH), '.') + 2)
        FROM components;
BEGIN
   OPEN curr_components;

   LOOP
      FETCH curr_components
       INTO profile_docnumber, id_version, estensione;

      EXIT WHEN curr_components%NOTFOUND;

      BEGIN
         SELECT MAX (VERSIONS.version_id)
           INTO id_last_version
           FROM VERSIONS, components
          WHERE VERSIONS.docnumber = profile_docnumber
            AND VERSIONS.version_id = components.version_id;

                       
              
              
                IF (UPPER (TRIM (estensione)) = 'P7M')
         THEN
           SELECT           case 
 when  instr(f.path, '\')=0
          then f.path 
          else
          SUBSTR (f.PATH,
           LENGTH (f.PATH) - INSTR (REVERSE (f.PATH), '\') + 2,
           LENGTH (f.PATH)
          )                   
          end         
            INTO estensioneappo      
              FROM components f
             WHERE  f.docnumber = profile_docnumber and f.version_id = id_version;

            SELECT SUBSTR (estensioneappo,
                           INSTR (estensioneappo, '.') + 1,
                           LENGTH (estensioneappo)
                          )
              INTO estensione
              FROM DUAL;

            IF (UPPER (TRIM (estensione)) = 'P7M')
            THEN
               UPDATE components
                  SET ext = UPPER (estensione)
                WHERE  docnumber = profile_docnumber and version_id = id_version ;
            ELSE
               SELECT SUBSTR (estensione,
                                LENGTH (estensione)
                              - INSTR (REVERSE (estensione), '.')
                              + 2
                             )
                 INTO estensioneappo
                 FROM DUAL;

               WHILE (UPPER (TRIM (estensioneappo)) = 'P7M')
               LOOP
                  SELECT SUBSTR (estensione,
                                 0,
                                   LENGTH (estensione)
                                 - INSTR (REVERSE (estensione), '.')
                                )
                    INTO estensione
                    FROM DUAL;

                  SELECT SUBSTR (estensione,
                                   LENGTH (estensione)
                                 - INSTR (REVERSE (estensione), '.')
                                 + 2
                                )
                    INTO estensioneappo
                    FROM DUAL;
               END LOOP;
                
               select instr(estensione,'.') into indice from dual;
               if(indice>0) then
               
               select substr(estensione, instr(estensione,'.')+1, length(estensione)) into estensione from dual;
               end if;
               
               IF (LENGTH (estensione) > 7)
               THEN
                  estensione := SUBSTR (estensione, 0, 7);
               END IF;

               UPDATE components
                  SET ext = UPPER (estensione)
                WHERE version_id = id_version
                      AND docnumber = profile_docnumber and version_id = id_version ;
            END IF;

            IF (id_version = id_last_version)
            THEN
               UPDATE components
                  SET cha_firmato = '1'
                WHERE docnumber = profile_docnumber and version_id = id_version ;
            END IF;
         ELSE
            IF (id_version = id_last_version)
            THEN
               UPDATE components
                  SET cha_firmato = '0'
                WHERE docnumber = profile_docnumber and version_id = id_version ;
            END IF;
            
            select instr(estensione,'.') into indice from dual;
             if(indice>0) then
               
               select substr(estensione, instr(estensione,'.')+1, length(estensione)) into estensione from dual;
               end if;
            
            
            IF (LENGTH (estensione) > 7)
            THEN
               estensione := SUBSTR (estensione, 0, 7);
            END IF;

            UPDATE components
               SET ext = UPPER (estensione)
             WHERE docnumber = profile_docnumber and version_id = id_version ;
         END IF;
      --UPDATE components SET EXT=upper(estensione) where  docnumber=profile_docnumber and version_id = id_version AND;
      EXCEPTION
         WHEN OTHERS
         THEN
            NULL;
      END;
   END LOOP;

   CLOSE curr_components;
END sp_ins_ext_in_components;
/

BEGIN 
  @db_user.SP_INS_EXT_IN_COMPONENTS;
  COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.dpa3_get_hierarchy (
p_id_amm in varchar,
p_cod in varchar,
p_tipo_ie in varchar,
p_id_Ruolo in varchar,
p_codes out varchar)

as

begin
declare
v_c_type varchar(2);
v_p_cod varchar(64);
v_system_id int;
v_id_parent int;
v_id_uo int;
v_id_utente int;

begin

p_codes := '';
v_p_cod := p_cod;

select
id_parent, system_id, id_uo, id_people, cha_tipo_urp
into
v_id_parent, v_system_id, v_id_uo, v_id_utente, v_c_type
from
dpa_corr_globali
where
var_cod_rubrica = v_p_cod and
cha_tipo_ie = p_tipo_ie and
id_amm = p_id_amm and
dta_fine is null;


while (1 > 0) loop
if v_c_type = 'U' then
if (v_id_parent is null or v_id_parent = 0) then
exit;
end if;

select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_parent and id_amm = p_id_amm and dta_fine is null;
end if;

if v_c_type = 'R' then
if (v_id_uo is null or v_id_uo = 0) then
exit;
end if;

select var_cod_rubrica, system_id into v_p_cod, v_system_id from dpa_corr_globali where system_id = v_id_uo and id_amm = p_id_amm and dta_fine is null ;
end if;

if v_c_type = 'P' then
select var_cod_rubrica into v_p_cod from dpa_corr_globali where id_gruppo = p_id_Ruolo
and id_amm = p_id_amm and dta_fine is null;
end if;
if v_p_cod is null then
exit;
end if;

select
id_parent, system_id, id_uo, cha_tipo_urp
into
v_id_parent, v_system_id, v_id_uo, v_c_type
from
dpa_corr_globali
where
var_cod_rubrica = v_p_cod and
id_amm = p_id_amm and
dta_fine is null;

p_codes := v_p_cod || ':' || p_codes;
end loop;

p_codes := p_codes || p_cod;

end;
end;
/


CREATE OR REPLACE FUNCTION vardescribe 
(sysid INT, typetable VARCHAR)
   RETURN VARCHAR2
IS
   risultato   VARCHAR2 (8000);
   tmpvar      VARCHAR2 (8000);
   tipo        CHAR;
   num_proto   NUMBER;
   doc_number  NUMBER;
BEGIN
   BEGIN
      tmpvar := NULL;

--MAIN
      IF (typetable = 'PEOPLENAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_people = sysid AND cha_tipo_urp = 'P' AND cha_tipo_ie = 'I';
      END IF;

      IF (typetable = 'GROUPNAME')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE system_id = sysid AND cha_tipo_urp = 'R';
      END IF;

      IF (typetable = 'DESC_RUOLO')
      THEN
         SELECT var_desc_corr
           INTO risultato
           FROM dpa_corr_globali
          WHERE id_gruppo = sysid AND cha_tipo_urp = 'R';
      END IF;

      IF (typetable = 'RAGIONETRASM')
      THEN
         SELECT var_desc_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'TIPO_RAGIONE')
      THEN
         SELECT cha_tipo_ragione
           INTO risultato
           FROM dpa_ragione_trasm
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DATADOC')
      THEN
         BEGIN
            SELECT cha_tipo_proto, NVL (num_proto, 0)
              INTO tipo, num_proto
              FROM PROFILE
             WHERE system_id = sysid;

            IF (    tipo IS NOT NULL
                AND (tipo IN ('A', 'P', 'I') AND num_proto != 0)
               )
            THEN
               SELECT TO_CHAR (dta_proto, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            ELSE
               SELECT TO_CHAR (creation_date, 'dd/mm/yyyy')
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;

      IF (typetable = 'CHA_TIPO_PROTO')
      THEN
         SELECT cha_tipo_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'NUMPROTO')
      THEN
         SELECT num_proto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'CODFASC')
      THEN
         SELECT var_codice
           INTO risultato
           FROM project
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DTA_CREAZ')
      THEN
         SELECT TO_CHAR (dta_creazione, 'yyyy')
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'NUM_FASC')
      THEN
         SELECT num_fascicolo
            INTO risultato
            FROM project
            WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DESC_OGGETTO')
      THEN
         SELECT var_prof_oggetto
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;

      IF (typetable = 'DESC_FASC')
      THEN
         BEGIN
            SELECT description
              INTO risultato
              FROM project
             WHERE system_id = sysid;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '';
         END;
      END IF;

      IF (typetable = 'PROF_IDREG')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_registro
                    INTO risultato
                    FROM PROFILE
                   WHERE system_id = sysid;

                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;

      IF (typetable = 'ID_GRUPPO')
      THEN
         BEGIN
            IF sysid IS NOT NULL
            THEN
               BEGIN
                  SELECT id_gruppo
                    INTO risultato
                    FROM dpa_corr_globali
                   WHERE system_id = sysid;

                  IF (risultato IS NULL)
                  THEN
                     risultato := '0';
                  END IF;
               END;
            ELSE
               risultato := '0';
            END IF;
         EXCEPTION
            WHEN NO_DATA_FOUND
            THEN
               risultato := '0';
         END;
      END IF;

      IF (typetable = 'SEGNATURA_DOCNUMBER')
      THEN
         BEGIN
            SELECT var_segnatura
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;
      
        IF (typetable = 'SEGNATURA_CODFASC')
      THEN
         BEGIN
            SELECT num_proto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

            IF (risultato IS NULL)
            THEN
               SELECT docnumber
                 INTO risultato
                 FROM PROFILE
                WHERE system_id = sysid;
            END IF;
         END;
      END IF;

      IF (typetable = 'OGGETTO_MITTENTE')
      THEN
         BEGIN
-- OGGETTO
            SELECT var_prof_oggetto
              INTO risultato
              FROM PROFILE
             WHERE system_id = sysid;

--MITTENTE
            BEGIN
               SELECT var_desc_corr
                 INTO tmpvar
                 FROM (SELECT var_desc_corr
                         FROM dpa_corr_globali a, dpa_doc_arrivo_par b
                        WHERE b.id_mitt_dest = a.system_id
                          AND b.cha_tipo_mitt_dest = 'M'
                          AND b.id_profile = sysid)
                WHERE ROWNUM = 1;
            EXCEPTION
               WHEN NO_DATA_FOUND
               THEN
                  tmpvar := '';
            END;

            IF (tmpvar IS NOT NULL)
            THEN
               risultato := risultato || '@@' || tmpvar;
            END IF;
         END;
      END IF;

      IF (typetable = 'PROFILE_CHA_IMG')
      THEN
         SELECT getchaimg (docnumber)
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'PROFILE_CHA_FIRMATO')
      THEN
         SELECT CHA_FIRMATO
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
      
      IF (typetable = 'COMPONENTS_CHA_FIRMATO')
      THEN
         SELECT getchafirmato(docnumber)
           INTO risultato
           FROM PROFILE
          WHERE system_id = sysid;
      END IF;
 
--ENDMAIN
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         NULL;
      WHEN OTHERS
      THEN
         RAISE;
   END;
   RETURN risultato;
end;
/


--fine cha_firmato

begin
   INSERT INTO dpa_tipo_funzione
            (system_id, var_cod_tipo, var_desc_tipo_fun, cha_vis, id_amm)
   SELECT seq.NEXTVAL, 'PRAU_RF', 'PROTOCOLLAZIONE AUTOMATICA RF', cha_vis,
          id_amm
   FROM dpa_tipo_funzione
   WHERE UPPER (var_cod_tipo) = 'PRAU'
end;
/

CREATE OR REPLACE FUNCTION getDescRuolo (idGruppo INT)
RETURN varchar IS risultato varchar(256);
BEGIN
select var_desc_corr into risultato from dpa_corr_globali where id_Gruppo=idGruppo;
RETURN risultato;
END getDescRuolo;
/

 begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_APPS';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from @dbuser.APPS;
           execute immediate 'CREATE SEQUENCE SEQ_APPS START WITH ' || cnt ||' MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='COMPONENTS' and column_name='EXT';
    if (cnt = 1) then        
            execute immediate 'ALTER TABLE components MODIFY ext varchar2(256)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='dpa_formati_documento' and column_name='file_extension';
    if (cnt = 1) then        
            execute immediate 'ALTER TABLE dpa_formati_documento MODIFY file_extension varchar2(256)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_anagrafica_funzioni where cod_funzione='FILTRO_FASC_EXCEL';
    if (cnt = 0) then        
        begin
            insert into dpa_anagrafica_funzioni (cod_funzione,var_desc_funzione,disabled) values ('FILTRO_FASC_EXCEL','Abilita la ricerca fascicolo filtrata con un elenco di valori su file excel nella pagina di ricerca fascicoli','N');
        end;
    end if;
    end;
end;
/


CREATE OR REPLACE PROCEDURE @db_user.sp_dpa_count_todolist (
   id_people_p   IN   NUMBER,
   id_gruppo     IN   NUMBER,
   ts            IN   VARCHAR
)
IS
   trasmdoctot             NUMBER;
   trasmdocnonletti        NUMBER;
   trasmdocnonaccettati    NUMBER;
   trasmfasctot            NUMBER;
   trasmfascnonletti       NUMBER;
   trasmfascnonaccettati   NUMBER;
   docpredisposti          NUMBER;
   ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdoctot
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );

--numero documenti non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonletti
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero documenti non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonaccettati
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');

--numero fascicoli presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfasctot
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );

--numero fascicoli non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonletti
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero fascicoli non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonaccettati
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');

--numero documenti predisposti
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO docpredisposti
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');

   BEGIN                                                               -- MAIN
      ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');

-- SVUOTO LA TABELLA DEI DATI
      DELETE      dpa_count_todolist
            WHERE (id_people = id_people_p);

      INSERT INTO dpa_count_todolist
                  (id_people, ts_stampa, tot_doc, tot_doc_no_letti,
                   tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
                   tot_fasc_no_accettati, tot_doc_predisposti
                  )
           VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
                   trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
                   trasmfascnonaccettati, docpredisposti
                  );
   END;                                                                -- MAIN
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END;
/


CREATE OR REPLACE PROCEDURE @db_user.i_smistamento_smistadoc_u (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipotrasmissione               IN       CHAR,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   systrasmsing        NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   idutente            NUMBER;
   recordcorrente      NUMBER;
   idgroups            NUMBER   := NULL;
   idgruppo            NUMBER;
   resultvalue         NUMBER;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO systrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing,
                   cha_tipo_trasm, dta_scadenza, id_trasm_utente
                  )
           VALUES (systrasmsing, idragionetrasm, identitytrasm, 'R',
                   idcorrglobaledestinatario, noteindividuali,
                   tipotrasmissione, datascadenza, NULL
                  );

      returnvalue := systrasmsing;
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

-- Verifica se non vi sia gi una trasmissione per il documento:
-- - se presente, si distinguono 2 casi:
-- 1) se ACCESSRIGHT < Rights
--    viene fatto un'aggiornamento impostandone il valore a Rights
-- 2) altrimenti non fa nulla
-- - se non presente viene fatta in ogni caso la insert con
--   valore di ACCESSRIGHT = Rights
   BEGIN
      SELECT a.id_gruppo
        INTO idgroups
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobaledestinatario;
   END;

   idgruppo := idgroups;

   BEGIN
      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento AND personorgroup = idgruppo)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idgruppo
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto
                     )
              VALUES (iddocumento, idgruppo, rights, idgruppomittente,
                      tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;    
      
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;
            
            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta già accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso risulta visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.thing=iddocumento and s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;
   

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;
END;
/



CREATE OR REPLACE PROCEDURE @db_user.i_smistamento_smistadoc (
   idpeoplemittente               IN       NUMBER,
   idcorrglobaleruolomittente     IN       NUMBER,
   idgruppomittente               IN       NUMBER,
   idamministrazionemittente      IN       NUMBER,
   idpeopledestinatario           IN       NUMBER,
   idcorrglobaledestinatario      IN       NUMBER,
   iddocumento                    IN       NUMBER,
   idtrasmissione                 IN       NUMBER,
   idtrasmissioneutentemittente   IN       NUMBER,
   trasmissioneconworkflow        IN       CHAR,
   notegeneralidocumento          IN       VARCHAR2,
   noteindividuali                IN       VARCHAR2,
   datascadenza                   IN       DATE,
   tipodiritto                    IN       CHAR,
   rights                         IN       NUMBER,
   originalrights                 IN       NUMBER,
   idragionetrasm                 IN       NUMBER,
   idpeopledelegato               IN       NUMBER,
   returnvalue                    OUT      NUMBER
)
IS
/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione delle trasmissioni nello smistamento.
--
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- -2: Non  stato inserito il RECORD IN tabella DPA_TRASMISSIONI
-- -3: Non  stato inserito il RECORD IN tabella DPA_TRASM_SINGOLE
-- -4: Non  stato inserito il RECORD IN tabella DPA_TRASM_UTENTE
-- -5: Errore in SPsetDataVistaSmistamento
-------------------------------------------------------------------------------------------------------
*/
   identitytrasm       NUMBER   := NULL;
   identitytrasmsing   NUMBER   := NULL;
   existaccessrights   CHAR (1) := 'Y';
   accessrights        NUMBER   := NULL;
   accessrightsvalue   NUMBER   := NULL;
   tipotrasmsingola    CHAR (1) := NULL;
   isAccettata         VARCHAR2(1)  := '0';
   isAccettataDelegato VARCHAR2(1)  := '0';
   isVista             VARCHAR2(1)  := '0';    
   isVistaDelegato     VARCHAR2(1)  := '0';
   resultvalue         NUMBER;   
   
BEGIN
   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasm
        FROM DUAL;
   END;

   BEGIN
      SELECT seq.NEXTVAL
        INTO identitytrasmsing
        FROM DUAL;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASMISSIONE */
      INSERT INTO dpa_trasmissione
                  (system_id, id_ruolo_in_uo,
                   id_people, cha_tipo_oggetto, id_profile, id_project,
                   dta_invio, var_note_generali
                  )
           VALUES (identitytrasm, idcorrglobaleruolomittente,
                   idpeoplemittente, 'D', iddocumento, NULL,
                   SYSDATE (), notegeneralidocumento
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -2;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_SINGOLA */
      INSERT INTO dpa_trasm_singola
                  (system_id, id_ragione, id_trasmissione, cha_tipo_dest,
                   id_corr_globale, var_note_sing, cha_tipo_trasm,
                   dta_scadenza, id_trasm_utente
                  )
           VALUES (identitytrasmsing, idragionetrasm, identitytrasm, 'U',
                   idcorrglobaledestinatario, noteindividuali, 'S',
                   datascadenza, NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -3;
         RETURN;
   END;

   BEGIN
/* Inserimento in tabella DPA_TRASM_UTENTE */
      INSERT INTO dpa_trasm_utente
                  (system_id, id_trasm_singola, id_people,
                   dta_vista, dta_accettata, dta_rifiutata, dta_risposta,
                   cha_vista, cha_accettata, cha_rifiutata, var_note_acc,
                   var_note_rif, cha_valida, id_trasm_risp_sing
                  )
           VALUES (seq.NEXTVAL, identitytrasmsing, idpeopledestinatario,
                   NULL, NULL, NULL, NULL,
                   '0', '0', '0', NULL,
                   NULL, '1', NULL
                  );
   EXCEPTION
      WHEN OTHERS
      THEN
         returnvalue := -4;
         RETURN;
   END;

   BEGIN
--per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
      UPDATE dpa_trasmissione
         SET dta_invio = SYSDATE ()
       WHERE system_id = identitytrasm;

      SELECT accessrights
        INTO accessrights
        FROM (SELECT accessrights
                FROM security
               WHERE thing = iddocumento
                 AND personorgroup = idpeopledestinatario)
       WHERE ROWNUM = 1;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         existaccessrights := 'N';
   END;

   IF existaccessrights = 'Y'
   THEN
      accessrightsvalue := accessrights;

      IF accessrightsvalue < rights
      THEN
         BEGIN
/* aggiornamento a Rights */
            UPDATE security
               SET accessrights = rights
             WHERE thing = iddocumento
               AND personorgroup = idpeopledestinatario
               AND accessrights = accessrightsvalue;
         EXCEPTION
            WHEN DUP_VAL_ON_INDEX
            THEN
               NULL;
         END;
      END IF;
   ELSE
      BEGIN
/* inserimento a Rights */
         INSERT INTO security
                     (thing, personorgroup, accessrights,
                      id_gruppo_trasm, cha_tipo_diritto
                     )
              VALUES (iddocumento, idpeopledestinatario, rights,
                      idgruppomittente, tipodiritto
                     );
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            NULL;
      END;
   END IF;

/* Aggiornamento trasmissione del mittente */
   IF (trasmissioneconworkflow = '1') THEN
      BEGIN
            -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
            select  cha_accettata into isAccettata
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;
            
            select  cha_vista into isVista
            from    dpa_trasm_utente 
            where   system_id = idtrasmissioneutentemittente;            
        
            if (idPeopleDelegato > 0) then
                begin
                    -- Impostazione dei flag per la gestione del delegato
                    isVistaDelegato := '1';
                    isAccettataDelegato := '1';
                end;
            end if;

            if (isAccettata = '1') then
                begin
                    -- caso in cui la trasmissione risulta già accettata 
                    if (isVista = '0') then
                        begin
                            -- l'oggetto trasmesso non risulta ancora visto,
                            -- pertanto vengono impostati i dati di visualizzazione
                            -- e viene rimossa la trasmissione dalla todolist
                             UPDATE dpa_trasm_utente
                                SET dta_vista =
                                             (CASE
                                                 WHEN dta_vista IS NULL
                                                    THEN SYSDATE
                                                 ELSE dta_vista
                                              END
                                             ),
                                    cha_vista = (CASE
                                                    WHEN dta_vista IS NULL
                                                       THEN 1
                                                    ELSE 0
                                                 END),
                                    cha_vista_delegato = isVistaDelegato,
                                    cha_in_todolist = '0',
                                    cha_valida = '0'
                              WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                    
                     else
                        begin
                            -- l'oggetto trasmesso visto,
                            -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                            UPDATE dpa_trasm_utente
                                SET cha_in_todolist = '0',
                                    cha_valida = '0'
                            WHERE (   system_id = idtrasmissioneutentemittente
                                     OR system_id =
                                           (SELECT tu.system_id
                                              FROM dpa_trasm_utente tu,
                                                   dpa_trasmissione tx,
                                                   dpa_trasm_singola ts
                                             WHERE tu.id_people = idpeoplemittente
                                               AND tx.system_id = ts.id_trasmissione
                                               AND tx.system_id = idtrasmissione
                                               AND ts.system_id = tu.id_trasm_singola
                                               AND ts.cha_tipo_dest = 'U')
                                    );
                        end;                                   
                    end if;
                end;
            else
                -- la trasmissione ancora non risulta accettata, pertanto:
                -- 1) viene accettata implicitamente, 
                -- 2) l'oggetto trasmesso impostato come visto,
                -- 3) la trasmissione rimossa la trasmissione da todolist
                begin
                    UPDATE dpa_trasm_utente
                    SET dta_vista =
                                 (CASE
                                     WHEN dta_vista IS NULL
                                        THEN SYSDATE
                                     ELSE dta_vista
                                  END
                                 ),
                        cha_vista = (CASE
                                        WHEN dta_vista IS NULL
                                           THEN 1
                                        ELSE 0
                                     END),
                        cha_vista_delegato = isVistaDelegato,
                        dta_accettata = SYSDATE (),
                        cha_accettata = '1',
                        cha_accettata_delegato = isAccettataDelegato,
                        var_note_acc = 'Documento accettato e smistato',                        
                        cha_in_todolist = '0',
                        cha_valida = '0'
                  WHERE (   system_id = idtrasmissioneutentemittente
                         OR system_id =
                               (SELECT tu.system_id
                                  FROM dpa_trasm_utente tu,
                                       dpa_trasmissione tx,
                                       dpa_trasm_singola ts
                                 WHERE tu.id_people = idpeoplemittente
                                   AND tx.system_id = ts.id_trasmissione
                                   AND tx.system_id = idtrasmissione
                                   AND ts.system_id = tu.id_trasm_singola
                                   AND ts.cha_tipo_dest = 'U')
                        ) 
                        AND cha_valida = '1';
                end;
            end if;
 
            
            --update security se diritti  trasmssione in accettazione =20
            UPDATE security s
            SET     s.accessrights = originalrights,
                    s.cha_tipo_diritto = 'T'
            WHERE s.thing=iddocumento and s.personorgroup IN (idpeoplemittente, idgruppomittente)
                AND s.accessrights = 20;
      END;
   ELSE
      BEGIN
         spsetdatavistasmistamento (idpeoplemittente,
                                    iddocumento,
                                    idgruppomittente,
                                    'D',
                                    idtrasmissione,
                                    idpeopledelegato,
                                    resultvalue
                                   );

         IF (resultvalue = 1)
         THEN
            returnvalue := -4;
            RETURN;
         END IF;
      END;
   END IF;

/* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
   BEGIN
      SELECT *
        INTO tipotrasmsingola
        FROM (SELECT   a.cha_tipo_trasm
                  FROM dpa_trasm_singola a, dpa_trasm_utente b
                 WHERE a.system_id = b.id_trasm_singola
                   AND b.system_id IN (
                          SELECT tu.system_id
                            FROM dpa_trasm_utente tu,
                                 dpa_trasmissione tx,
                                 dpa_trasm_singola ts
                           WHERE tu.id_people = idpeoplemittente
                             AND tx.system_id = ts.id_trasmissione
                             AND tx.system_id = idtrasmissione
                             AND ts.system_id = tu.id_trasm_singola
                             AND ts.system_id =
                                    (SELECT id_trasm_singola
                                       FROM dpa_trasm_utente
                                      WHERE system_id =
                                                  idtrasmissioneutentemittente))
              ORDER BY cha_tipo_dest)
       WHERE ROWNUM = 1;
   END;

   IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1'
   THEN
/* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
      BEGIN
         UPDATE dpa_trasm_utente
            SET cha_valida = '0',
                cha_in_todolist = '0'
          WHERE id_trasm_singola IN (
                   SELECT a.system_id
                     FROM dpa_trasm_singola a, dpa_trasm_utente b
                    WHERE a.system_id = b.id_trasm_singola
                      AND b.system_id IN (
                             SELECT tu.system_id
                               FROM dpa_trasm_utente tu,
                                    dpa_trasmissione tx,
                                    dpa_trasm_singola ts
                              WHERE tu.id_people = idpeoplemittente
                                AND tx.system_id = ts.id_trasmissione
                                AND tx.system_id = idtrasmissione
                                AND ts.system_id = tu.id_trasm_singola
                                AND ts.system_id =
                                       (SELECT id_trasm_singola
                                          FROM dpa_trasm_utente
                                         WHERE system_id =
                                                  idtrasmissioneutentemittente)))
            AND system_id NOT IN (idtrasmissioneutentemittente);
      END;
   END IF;

   returnvalue := 0;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.sp_dpa_count_todolist (
   id_people_p   IN   NUMBER,
   id_gruppo     IN   NUMBER,
   ts            IN   VARCHAR
)
IS
   trasmdoctot             NUMBER;
   trasmdocnonletti        NUMBER;
   trasmdocnonaccettati    NUMBER;
   trasmfasctot            NUMBER;
   trasmfascnonletti       NUMBER;
   trasmfascnonaccettati   NUMBER;
   docpredisposti          NUMBER;
   ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdoctot
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );

--numero documenti non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonletti
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero documenti non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonaccettati
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                    AND dta_vista =  to_date('01/01/1753','dd/mm/yyyy HH24:mi:ss');

--numero fascicoli presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfasctot
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );

--numero fascicoli non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonletti
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');

--numero fascicoli non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonaccettati
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
     and exists (select dpa_ragione_trasm.system_id from dpa_ragione_trasm 
                    where id_ragione_trasm = dpa_ragione_trasm.system_id 
                    and dpa_ragione_trasm.CHA_TIPO_RAGIONE = 'W' ) 
                    AND dta_vista =  to_date('01/01/1753','dd/mm/yyyy HH24:mi:ss');

--numero documenti predisposti
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO docpredisposti
     FROM dpa_todolist
    WHERE (    id_profile > 0
           AND (   id_registro IN (
                      SELECT id_registro
                        FROM dpa_l_ruolo_reg g, dpa_el_registri r
                       WHERE r.system_id = g.id_registro
                         AND r.cha_rf = '0'
                         AND id_ruolo_in_uo IN (SELECT system_id
                                                  FROM dpa_corr_globali
                                                 WHERE id_gruppo = id_gruppo))
                OR id_registro = 0
               )
          )
      AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );

   BEGIN                                                               -- MAIN
      ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');

-- SVUOTO LA TABELLA DEI DATI
      DELETE      dpa_count_todolist
            WHERE (id_people = id_people_p);

      INSERT INTO dpa_count_todolist
                  (id_people, ts_stampa, tot_doc, tot_doc_no_letti,
                   tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
                   tot_fasc_no_accettati, tot_doc_predisposti
                  )
           VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
                   trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
                   trasmfascnonaccettati, docpredisposti
                  );
   END;                                                                -- MAIN
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END;
/

/*INIZIO VISIBILITA CAMPI PROFILAZIONE DINAMICA*/
--Creazione Tabella DPA_A_R_OGG_CUSTOM_DOC
begin
    declare cnt int;
    begin
    	select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_A_R_OGG_CUSTOM_DOC';
    	if (cnt = 0) then
    	   execute immediate 'CREATE SEQUENCE SEQ_DPA_A_R_OGG_CUSTOM_DOC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
    	end if;	
  
      select count(*) into cnt from user_tables where table_name='DPA_A_R_OGG_CUSTOM_DOC';
  	  if (cnt = 0) then
    	  execute immediate	
        'CREATE TABLE DPA_A_R_OGG_CUSTOM_DOC ' ||
      	'( ' ||
      	'SYSTEM_ID INT NOT NULL, ' ||
		'ID_TEMPLATE INT, ' ||
    	'ID_OGGETTO_CUSTOM INT, ' ||
        'ID_RUOLO INT, ' ||
		'INS_MOD INT, ' ||
		'VIS INT, ' ||
        'CONSTRAINT PK_DPA_A_R_OGG_CUSTOM_DOC PRIMARY KEY (SYSTEM_ID) ' ||	
      	')';
  	  end if;
  	  
  	  select count(*) into cnt from user_indexes where table_name='DPA_A_R_OGG_CUSTOM_DOC' and index_name='INDX_DPA_A_R_DOC1';
	  if (cnt = 0) then
		execute immediate 'CREATE INDEX INDX_DPA_A_R_DOC1 ON DPA_A_R_OGG_CUSTOM_DOC (ID_TEMPLATE, ID_OGGETTO_CUSTOM, ID_RUOLO) TABLESPACE @ora_idxtblspc_name LOGGING NOPARALLEL';
	  end if;
    end;	
end;	
/

--Creazione Tabella DPA_A_R_OGG_CUSTOM_FASC
begin
    declare cnt int;
    begin
    	select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_A_R_OGG_CUSTOM_FASC';
    	if (cnt = 0) then
    	   execute immediate 'CREATE SEQUENCE SEQ_DPA_A_R_OGG_CUSTOM_FASC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
    	end if;	
  
      select count(*) into cnt from user_tables where table_name='DPA_A_R_OGG_CUSTOM_FASC';
  	  if (cnt = 0) then
    	  execute immediate	
        'CREATE TABLE DPA_A_R_OGG_CUSTOM_FASC ' ||
      	'( ' ||
      	'SYSTEM_ID INT NOT NULL, ' ||
		'ID_TEMPLATE INT, ' ||
    	'ID_OGGETTO_CUSTOM INT, ' ||
        'ID_RUOLO INT, ' ||
		'INS_MOD INT, ' ||
		'VIS INT, ' ||
        'CONSTRAINT PK_DPA_A_R_OGG_CUSTOM_FASC PRIMARY KEY (SYSTEM_ID) ' ||	
      	')';
  	  end if;
  	  
  	  select count(*) into cnt from user_indexes where table_name='DPA_A_R_OGG_CUSTOM_FASC' and index_name='INDX_DPA_A_R_FASC1';
	  if (cnt = 0) then
		execute immediate 'CREATE INDEX INDX_DPA_A_R_FASC1 ON DPA_A_R_OGG_CUSTOM_FASC (ID_TEMPLATE, ID_OGGETTO_CUSTOM, ID_RUOLO) TABLESPACE @ora_idxtblspc_name LOGGING NOPARALLEL';
	  end if;
    end;	
end;	
/

--Popolazione DPA_A_R_OGG_CUSTOM_DOC - DPA_A_R_OGG_CUSTOM_FASC con la visibilità dei contatori con CONTA_DOPO attivo
BEGIN     
         DECLARE CURSOR c_idOggCustom IS SELECT id_Oggetto_Custom,id_Ruolo,inserimento FROM DPA_ASS_RUOLO_OGG_CUSTOM;
         par_id_Oggetto_Custom int;
         par_id_Ruolo int;
         par_inserimento int;
		 par_id_template int;
         cnt int;
         BEGIN  OPEN c_idOggCustom;
                LOOP FETCH c_idOggCustom into par_id_Oggetto_Custom, par_id_Ruolo, par_inserimento;
                EXIT WHEN c_idOggCustom%NOTFOUND;
                     --Verifico se l'oggetto custom è di una tipologia fascicolo
                     select count(*) INTO cnt from dpa_oggetti_custom where system_id = par_id_Oggetto_Custom and conta_dopo = 1;
                     IF (cnt = 1) THEN
                        select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                        if(cnt = 0) then
							select id_template INTO par_id_template from dpa_associazione_templates where id_oggetto = par_id_Oggetto_Custom and doc_number is null;
                            insert into /*+APPEND */ DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, par_inserimento, 1);
                        end if;                        
                     end if;
                     
                     --Verifico se l'oggetto custom è di una tipologia documento
                    select count(*) INTO cnt from dpa_oggetti_custom_fasc where system_id = par_id_Oggetto_Custom and conta_dopo = 1;
                    IF (cnt = 1) THEN
                        select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
                        if(cnt = 0) then
							select id_template INTO par_id_template from dpa_ass_templates_fasc where id_oggetto = par_id_Oggetto_Custom and id_project is null;
                            insert into /*+APPEND */ DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, par_inserimento, 1);
                        end if;
                    end if;
                END LOOP;
                CLOSE c_idOggCustom;
        END;          
END;
/

--Popolazione DPA_A_R_OGG_CUSTOM_DOC con la visibilità dei campi profilati escluso i contatori con CONTA_DOPO attivo
BEGIN
	--DECLARE CURSOR c_idTemplate IS SELECT system_id from dpa_tipo_atto;
	DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_atto where id_amm is not null;
	par_id_template int;
	par_id_oggetto_Custom int;
	par_id_ruolo int;
	par_id_amm int;
	cnt int;
	BEGIN  OPEN c_idTemplate;
		--LOOP FETCH c_idTemplate INTO par_id_template;
		LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
		EXIT WHEN c_idTemplate%NOTFOUND;
		
		DECLARE CURSOR c_oggCustom IS select dpa_associazione_templates.id_oggetto from dpa_associazione_templates, dpa_oggetti_custom where dpa_associazione_templates.id_template = par_id_template and dpa_associazione_templates.doc_number IS NULL and dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id and dpa_oggetti_custom.conta_dopo <> 1;
		BEGIN  OPEN c_oggCustom;
			LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
			EXIT WHEN c_oggCustom%NOTFOUND;
			
			--DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_doc where id_tipo_doc = par_id_template; --and id_ruolo <> 0;
			DECLARE CURSOR c_idRuoli IS select id_gruppo from dpa_corr_globali where id_amm = par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
			BEGIN OPEN c_idRuoli;
				LOOP FETCH c_idRuoli INTO par_id_ruolo;
				EXIT WHEN c_idRuoli%NOTFOUND;  
				
					select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_DOC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
					if(cnt = 0) then
						insert /*+APPEND */ into DPA_A_R_OGG_CUSTOM_DOC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_DOC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 1, 1);
					end if;
					
				END LOOP;
				CLOSE c_idRuoli;
			END;
			
			END LOOP;
			CLOSE c_oggCustom;
		END;		
		
		END LOOP;
		CLOSE c_idTemplate;
	END;
END;
/

--Popolazione DPA_A_R_OGG_CUSTOM_FASC con la visibilità dei campi profilati escluso i contatori con CONTA_DOPO attivo
BEGIN
	--DECLARE CURSOR c_idTemplate IS SELECT system_id from dpa_tipo_fasc;
	DECLARE CURSOR c_idTemplate IS SELECT system_id, id_amm from dpa_tipo_fasc where id_amm is not null;
	par_id_template int;
	par_id_oggetto_Custom int;
	par_id_ruolo int;
	par_id_amm int;
	cnt int;
	BEGIN  OPEN c_idTemplate;
		--LOOP FETCH c_idTemplate INTO par_id_template;
		LOOP FETCH c_idTemplate INTO par_id_template, par_id_amm;
		EXIT WHEN c_idTemplate%NOTFOUND;
		
		DECLARE CURSOR c_oggCustom IS select dpa_ass_templates_fasc.id_oggetto from dpa_ass_templates_fasc, dpa_oggetti_custom_fasc where dpa_ass_templates_fasc.id_template = par_id_template and dpa_ass_templates_fasc.id_project IS NULL and dpa_ass_templates_fasc.id_oggetto = dpa_oggetti_custom_fasc.system_id and dpa_oggetti_custom_fasc.conta_dopo <> 1;
		BEGIN  OPEN c_oggCustom;
			LOOP FETCH c_oggCustom INTO par_id_oggetto_Custom;
			EXIT WHEN c_oggCustom%NOTFOUND;
			
			--DECLARE CURSOR c_idRuoli IS select id_ruolo from dpa_vis_tipo_fasc where id_tipo_fasc = par_id_template; --and id_ruolo <> 0;
			DECLARE CURSOR c_idRuoli IS select id_gruppo from dpa_corr_globali where id_amm = par_id_amm and id_gruppo is not null and cha_tipo_urp = 'R';
			BEGIN OPEN c_idRuoli;
				LOOP FETCH c_idRuoli INTO par_id_ruolo;
				EXIT WHEN c_idRuoli%NOTFOUND;  
				
					select count(*) INTO cnt from DPA_A_R_OGG_CUSTOM_FASC where id_template = par_id_template and id_oggetto_custom = par_id_Oggetto_Custom and id_ruolo = par_id_Ruolo;
					if(cnt = 0) then
						insert /*+APPEND */ into DPA_A_R_OGG_CUSTOM_FASC (system_id, id_template, id_oggetto_custom, id_ruolo, ins_mod, vis) values(SEQ_DPA_A_R_OGG_CUSTOM_FASC.nextval, par_id_template, par_id_Oggetto_Custom, par_id_Ruolo, 1, 1);
					end if;
					
				END LOOP;
				CLOSE c_idRuoli;
			END;
			
			END LOOP;
			CLOSE c_oggCustom;
		END;		
		
		END LOOP;
		CLOSE c_idTemplate;
	END;
END;
/
/*FINE VISIBILITA CAMPI PROFILAZIONE DINAMICA*/

CREATE OR REPLACE FUNCTION iscorrispondenteinterno (
   idcorrglobali   INT,
   idregistro      INT
)
   RETURN NUMBER
IS
   RESULT   NUMBER;
BEGIN
   DECLARE
      tipourp                 VARCHAR (1);
      tipoie                  VARCHAR (1);
      idpeople                INT;
      numero_corrispondenti   INT;
   BEGIN
      tipoie := 'E';
      RESULT := 0;

      SELECT a.cha_tipo_ie
        INTO tipoie
        FROM dpa_corr_globali a
       WHERE a.system_id = idcorrglobali;

      IF (tipoie = 'I')
      THEN
         SELECT a.cha_tipo_urp
           INTO tipourp
           FROM dpa_corr_globali a
          WHERE a.system_id = idcorrglobali;

         IF (tipourp = 'U')
         THEN
            SELECT COUNT (*)
              INTO numero_corrispondenti
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND id_uo = idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = idregistro
               AND r.cha_rf = '0';
         END IF;

         IF (tipourp = 'R')
         THEN
            SELECT COUNT (*)
              INTO numero_corrispondenti
              FROM dpa_corr_globali a, dpa_l_ruolo_reg f, dpa_el_registri r
             WHERE cha_tipo_ie = 'I'
               AND cha_tipo_urp = 'R'
               AND a.system_id = idcorrglobali
               AND f.id_ruolo_in_uo = a.system_id
               AND f.id_registro = r.system_id
               AND r.system_id = idregistro
               AND r.cha_rf = '0';
         END IF;

         IF (tipourp = 'P')
         THEN
            BEGIN
               SELECT a.id_people
                 INTO idpeople
                 FROM dpa_corr_globali a
                WHERE a.system_id = idcorrglobali;

               SELECT   COUNT (a.system_id)
                   INTO numero_corrispondenti
                   FROM dpa_corr_globali a,
                        peoplegroups b,
                        dpa_l_ruolo_reg f,
                        dpa_el_registri r
                  WHERE a.id_gruppo = b.groups_system_id
                    AND b.dta_fine IS NULL
                    AND b.people_system_id = idpeople
                    AND f.id_ruolo_in_uo = a.system_id
                    AND f.id_registro = r.system_id
                    AND r.system_id = idregistro
                    AND r.cha_rf = '0'
               ORDER BY a.system_id DESC;
            END;
         END IF;
      END IF;

      IF (numero_corrispondenti > 0)
      THEN
         RESULT := 1;
      END IF;

      RETURN RESULT;
   END;
END iscorrispondenteinterno;
/

/* Inserimento funzione per l'esportazione dei documenti di un fascicolo */
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='EXP_DOC_MASSIVA';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('EXP_DOC_MASSIVA', 'Abilita il pulsante esporta documenti in fascicolo nella pagina di dettaglio del fascicolo', '', 'N');
    end if;
    end;
end;
/

/* Inserimento per cambio etichette protocollatura */

begin
    declare   cnt int;
    begin
    select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_LETTERE_DOCUMENTI';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_LETTERE_DOCUMENTI START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;   
	select count(*) into cnt from user_tables where table_name='DPA_LETTERE_DOCUMENTI';
		if (cnt = 0) then
		  execute immediate    
				'CREATE TABLE DPA_LETTERE_DOCUMENTI ' ||
				'( ' ||
				'SYSTEM_ID NUMBER PRIMARY KEY NOT NULL, ' ||
				'CODICE VARCHAR(100) NOT NULL, ' ||
				'DESCRIZIONE VARCHAR(255) NOT NULL, ' ||  
				'ETICHETTA VARCHAR(255) NOT NULL ' ||  
				') ';
		end if;
    end;    
end;    
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from DPA_LETTERE_DOCUMENTI;
	if (cnt = 0) then	
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'A','A','Ingresso');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'P','P','Uscita');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'I','I','Interno');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'G','NP','NP');
		insert into DPA_LETTERE_DOCUMENTI VALUES(SEQ_DPA_LETTERE_DOCUMENTI.nextval,'ALL','ALL','Allegato');	
	end if;
	end;
end;
/

begin
    declare   cnt int;
    begin
	select count(*) into cnt from user_tables where table_name='DPA_ASS_LETTERE_DOCUMENTI';
		if (cnt = 0) then
		  execute immediate    
				'CREATE TABLE DPA_ASS_LETTERE_DOCUMENTI ' ||
				'( ' ||
				'ID_AMM NUMBER NOT NULL, ' ||
				'ID_LETTERADOC NUMBER NOT NULL, ' ||
				'DESCRIZIONE VARCHAR(255) NOT NULL, ' ||  
				'ETICHETTA VARCHAR(255) NOT NULL ' || 
				') ';
		end if;
    end;    
end;    
/

/* Inserimento voci per definizione funzioni di importazione fascicoli / documenti / RDE */
begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_FASC';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_FASC','Abilita la voce di menu'' Import Fascicoli sotto Ricerca', '', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_DOCS';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_DOCS','Abilita la voce di menu'' Import Documenti sotto Documenti', '', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMP_RDE';
    if (cnt = 0) then        
       insert into @db_user.DPA_ANAGRAFICA_FUNZIONI VALUES('IMP_RDE','Abilita la voce di menu'' Import Emergenza sotto Gestione', '', 'N');
    end if;
    end;
end;
/

/* Fine dichiarazione voci per definizione funzioni */

--Elenco note
begin
    declare   cnt int;
    begin
    select count(*) into cnt from user_tables where table_name='DPA_ELENCO_NOTE';
        if (cnt = 0) then
          execute immediate    
                'CREATE TABLE DPA_ELENCO_NOTE ' ||
                '( ' ||
                'SYSTEM_ID NUMBER NOT NULL, ' ||
                'ID_REG_RF NUMBER, ' ||
                'VAR_DESC_NOTA VARCHAR2(256 BYTE), ' ||
				'COD_REG_RF VARCHAR2(128 BYTE), ' || 
                'CONSTRAINT PK_DPA_ELENCO_NOTE PRIMARY KEY (SYSTEM_ID) ' ||  
                ') ';
        end if;
    end;    
end;    
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='ELENCO_NOTE';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('ELENCO_NOTE','Abilita il sottomenu'' Elenco note dal menu'' Gestione','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='IMPORT_ELENCO_NOTE';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('IMPORT_ELENCO_NOTE','Abilita l''import di un elenco di note da foglio excel','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='RICERCA_NOTE_ELENCO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('RICERCA_NOTE_ELENCO','Abilita la ricerca delle note da un elenco','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE='INSERIMENTO_NOTERF';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('INSERIMENTO_NOTERF','Abilita l''inserimento delle note associate ad un dato rf','', 'N');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_NOTE' and column_name='IDRFASSOCIATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_NOTE ADD IDRFASSOCIATO INTEGER';
    end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION @db_user.GetCountNote(tipoOggetto CHAR, idOggetto NUMBER, note NVARCHAR2, idUtente NUMBER, idGruppo NUMBER, tipoRic char, idRegistro NUMBER)
RETURN NUMBER IS retValue NUMBER;

BEGIN

IF tipoRic = 'Q' THEN
    SELECT COUNT(SYSTEM_ID) INTO retValue
    FROM   DPA_NOTE N 
    WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
           N.IDOGGETTOASSOCIATO = idOggetto AND
           N.TESTO LIKE '%'||note||'%' AND
          (N.TIPOVISIBILITA = 'T' OR
          (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente) OR
          (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo));
          
    ELSIF tipoRic = 'T' THEN
        SELECT COUNT(SYSTEM_ID) INTO retValue
        FROM   DPA_NOTE N 
        WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
               N.IDOGGETTOASSOCIATO = idOggetto AND
               N.TESTO LIKE '%'||note||'%' AND
               N.TIPOVISIBILITA = 'T';
        
        ELSIF tipoRic = 'P' THEN
            SELECT COUNT(SYSTEM_ID) INTO retValue
            FROM   DPA_NOTE N 
            WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                   N.IDOGGETTOASSOCIATO = idOggetto AND
                   N.TESTO LIKE '%'||note||'%' AND
                   (N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente);
                   
            ELSIF tipoRic = 'R' THEN
                SELECT COUNT(SYSTEM_ID) INTO retValue
                FROM   DPA_NOTE N 
                WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                       N.IDOGGETTOASSOCIATO = idOggetto AND
                       N.TESTO LIKE '%'||note||'%' AND
                       (N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo);
                       
                ELSIF tipoRic = 'F' THEN
                    SELECT COUNT(SYSTEM_ID) INTO retValue
                    FROM   DPA_NOTE N 
                    WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                       N.IDOGGETTOASSOCIATO = idOggetto AND
                       N.TESTO LIKE '%'||note||'%' AND
                       (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = idRegistro);

                       
END IF;
      
RETURN retValue;
END GetCountNote;
/

CREATE OR REPLACE FUNCTION  @db_user.GetCountNote(tipoOggetto CHAR, idOggetto NUMBER, note NVARCHAR2, idUtente NUMBER, idGruppo NUMBER, tipoRic char, idRegistro NUMBER)
RETURN NUMBER IS retValue NUMBER;

BEGIN



IF tipoRic = 'Q' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
N.TESTO LIKE '%'||note||'%' AND
(N.TIPOVISIBILITA = 'T' OR
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente) OR
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo)OR 
(n.tipovisibilita = 'F' AND n.idrfassociato in 
(select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=idGruppo ))
)

);

ELSIF tipoRic = 'T' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
N.TESTO LIKE '%'||note||'%' AND
N.TIPOVISIBILITA = 'T';

ELSIF tipoRic = 'P' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
N.TESTO LIKE '%'||note||'%' AND
(N.TIPOVISIBILITA = 'P' AND N.IDUTENTECREATORE = idUtente);

ELSIF tipoRic = 'R' THEN
SELECT COUNT(SYSTEM_ID) INTO retValue
FROM   DPA_NOTE N
WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
N.IDOGGETTOASSOCIATO = idOggetto AND
N.TESTO LIKE '%'||note||'%' AND
(N.TIPOVISIBILITA = 'R' AND N.IDRUOLOCREATORE = idGruppo);

 ELSIF tipoRic = 'F' and idRegistro<>0 THEN
                    SELECT COUNT(SYSTEM_ID) INTO retValue
                    FROM   DPA_NOTE N 
                    WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                       N.IDOGGETTOASSOCIATO = idOggetto AND
                       N.TESTO LIKE '%'||note||'%' AND
                       (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO = idRegistro);
                       
                       ELSIF tipoRic = 'F' and idRegistro=0 THEN
                    SELECT COUNT(SYSTEM_ID) INTO retValue
                    FROM   DPA_NOTE N 
                    WHERE  N.TIPOOGGETTOASSOCIATO = tipoOggetto AND
                       N.IDOGGETTOASSOCIATO = idOggetto AND
                       N.TESTO LIKE '%'||note||'%' AND
                       (N.TIPOVISIBILITA = 'F' and N.IDRFASSOCIATO in 
                        (select id_registro from dpa_l_ruolo_Reg r,dpa_el_registri lr where lr.CHA_RF='1' and r.ID_REGISTRO=lr.SYSTEM_ID
                            and r.ID_RUOLO_IN_UO in (select system_id from dpa_corr_globali where id_gruppo=idGruppo )));



END IF;

RETURN retValue;
END GetCountNote;
/

--fine elenco note

--INIZIO IPERDOCUMENTO
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' AND column_name='IPERFASCICOLO';
        if (cnt != 0) then            
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO RENAME COLUMN IPERFASCICOLO TO IPERDOCUMENTO';
        end if;   
 end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='COD_MOD_TRASM';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD COD_MOD_TRASM VARCHAR2(128)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='COD_CLASS';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD COD_CLASS VARCHAR2(128)';
	end if;
	end;
end;
/

--FINE IPERDOCUMENTO

CREATE OR REPLACE PROCEDURE @db_user.sp_dpa_count_todolist_no_reg (
   id_people_p   IN   NUMBER,
   id_gruppo     IN   NUMBER,
   ts            IN   VARCHAR
)
IS
   trasmdoctot             NUMBER;
   trasmdocnonletti        NUMBER;
   trasmdocnonaccettati    NUMBER;
   trasmfasctot            NUMBER;
   trasmfascnonletti       NUMBER;
   trasmfascnonaccettati   NUMBER;
   docpredisposti          NUMBER;
   ts_stampa_p             DATE;
BEGIN
--numero documenti presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdoctot
     FROM dpa_todolist
    WHERE id_profile > 0 
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
        
--numero documenti non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonletti
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
      
--numero documenti non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmdocnonaccettati
     FROM dpa_todolist
    WHERE id_profile > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero fascicoli presenti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfasctot
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          );
--numero fascicoli non letti in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonletti
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND dta_vista = TO_DATE ('01/01/1753', 'dd/mm/yyyy');
--numero fascicoli non ancora accettati in todolist
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO trasmfascnonaccettati
     FROM dpa_todolist
    WHERE id_project > 0
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
--numero documenti predisposti
   SELECT COUNT (DISTINCT (id_trasmissione))
     INTO docpredisposti
     FROM dpa_todolist
    WHERE id_profile > 0
         AND id_profile IN (SELECT system_id
                           FROM PROFILE
                          WHERE cha_da_proto = '1')
      AND (   (id_people_dest = id_people_p AND id_ruolo_dest = id_gruppo)
           OR (id_people_dest = id_people_p AND id_ruolo_dest = 0)
          )
      AND id_trasm_utente IN (SELECT system_id
                                FROM dpa_trasm_utente
                               WHERE cha_accettata = '0');
   BEGIN                                                               -- MAIN
      ts_stampa_p := TO_DATE (ts, 'dd/mm/yyyy hh24:mi:ss');
-- SVUOTO LA TABELLA DEI DATI
      DELETE      dpa_count_todolist
            WHERE (id_people = id_people_p);

      INSERT INTO dpa_count_todolist
                  (id_people, ts_stampa, tot_doc, tot_doc_no_letti,
                   tot_doc_no_accettati, tot_fasc, tot_fasc_no_letti,
                   tot_fasc_no_accettati, tot_doc_predisposti
                  )
           VALUES (id_people_p, ts_stampa_p, trasmdoctot, trasmdocnonletti,
                   trasmdocnonaccettati, trasmfasctot, trasmfascnonletti,
                   trasmfascnonaccettati, docpredisposti
                  );
   END;                                                                -- MAIN
EXCEPTION
   WHEN OTHERS
   THEN
      RETURN;
END;
/

--Inizio FASCICOLI CONTROLLATI
begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROJECT' and column_name='CHA_CONTROLLATO';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROJECT ADD CHA_CONTROLLATO VARCHAR(2)';
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='FASC_CONTROLLATO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_LOG (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
       VALUES (SEQ.NEXTVAL,'FASC_CONTROLLATO', 'Modificata proprietà controllato del fascicolo', 'FASCICOLO', 'FASCCONTROLLATO');
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from dpa_anagrafica_funzioni where cod_funzione='FASC_CONTROLLATO';
    if (cnt = 0) then
		begin
            INSERT INTO dpa_anagrafica_funzioni (cod_funzione,var_desc_funzione,disabled) values ('FASC_CONTROLLATO','Permette la creazione/modifica di un fascicolo controllato', 'N');
		end;
    end if;
    end;
end;
/
-- FINE FASCICOLI CONTROLLATI

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='GROUPS';
	if (cnt != 0) then		
        	execute immediate 'ALTER TABLE GROUPS MODIFY(GROUP_NAME VARCHAR2(256 BYTE))';
		end if;
	end;
end;
/ 

--INIZIO MITTENTI MULTIPLI
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_DOC_ARRIVO_PAR';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_DOC_ARRIVO_PAR' and column_name='CHA_TIPO_MITT_DEST';
  	if (cnt != 0) then
        	execute immediate 'ALTER TABLE DPA_DOC_ARRIVO_PAR MODIFY CHA_TIPO_MITT_DEST VARCHAR(2)';
		end if;
	end if;
	end;
end;
/

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_CORR_STO';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_CORR_STO' and column_name='CHA_TIPO_MITT_DES';
  	if (cnt != 0) then
        	execute immediate 'ALTER TABLE DPA_CORR_STO MODIFY CHA_TIPO_MITT_DES VARCHAR(2)';
		end if;
	end if;
	end;
end;
/
--FINE MITTENTI MULTIPLI

begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_AMMINISTRA';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' and column_name='TIPO_DOC_OBBL';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD (TIPO_DOC_OBBL  CHAR(1 BYTE) DEFAULT 0)';
		end if;
	end if;
	end;
end;
/ 

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_MODELLI_TRASM' and column_name='NO_NOTIFY';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_MODELLI_TRASM ADD NO_NOTIFY VARCHAR(1)';
    end if;
    end;
end;
/

-- Creazione tabella DPA_DATA_ARRIVO_STO
begin
declare cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_DATA_ARRIVO_STO';
        if (cnt = 0) then
            execute immediate
                'CREATE TABLE DPA_DATA_ARRIVO_STO' ||
                '('||
                      'SYSTEM_ID     INTEGER NOT NULL primary key ,' ||
                      'DOCNUMBER     INTEGER NOT NULL references PROFILE (SYSTEM_ID),' ||
                      'DTA_ARRIVO    DATE,' ||
                      'ID_GROUP      INTEGER NOT NULL,' ||
                      'ID_PEOPLE     INTEGER NOT NULL references PEOPLE (SYSTEM_ID),' ||
                      'DTA_MODIFICA  DATE' ||
                ') TABLESPACE @ora_dattblspc_name';
                
            end if;
            end;
            end;
            /

-- Sequence per la tabella DPA_DATA_ARRIVO_STO
begin
	declare
		   cnt int;
	begin	
		select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_DATA_ARRIVO_STO';
		if (cnt = 0) then
		   select nvl((max(last_number)+1),1) into cnt from user_sequences;
		   execute immediate 'CREATE SEQUENCE SEQ_DPA_DATA_ARRIVO_STO START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE NOCACHE NOORDER';
		end if;	
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_PROT_DATA_ORA_MODIFICA';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_PROT_DATA_ORA_MODIFICA'', ''Abilito la modifica di data arrivo e ora pervenuto nella protocollazione'', ''N'')';
		end;
	end if;
	end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='DO_PROT_DATA_ORA_STORIA';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI(COD_FUNZIONE, VAR_DESC_FUNZIONE, DISABLED) Values (''DO_PROT_DATA_ORA_STORIA'', ''Abilita il pulsante per lo storico della data arrivo e ora pervenuto nella creazione del protocollo'', ''N'')';
		end;
	end if;
	end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_MODELLI_TRASM' and column_name='NO_NOTIFY';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_MODELLI_TRASM ADD NO_NOTIFY VARCHAR(1)';
    end if;
    end;
end;
/



begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='DPA_CORR_GLOBALI' and column_name='COD_DESC_INTEROP';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE DPA_CORR_GLOBALI ADD COD_DESC_INTEROP VARCHAR(516)';
    end if;
    end;
end;
/

--TIMESTAMP DOCUMENTI
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE = 'DO_TIMESTAMP';
    if (cnt = 0) then        
            execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES(''DO_TIMESTAMP'',''Abilita la generazione di un timestamp per i documenti'',null,''N'')';
    end if;
    end;
end;
/

begin
    declare   cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_TIMESTAMP_DOC';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_TIMESTAMP_DOC START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_TIMESTAMP_DOC';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_TIMESTAMP_DOC ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'DOC_NUMBER INT, ' ||
			'VERSION_ID INT, ' ||
			'ID_PEOPLE INT, ' ||
			'DTA_CREAZIONE DATE, ' ||
			'DTA_SCADENZA DATE, ' ||
			'NUM_SERIE VARCHAR2(64), ' ||
			'S_N_CERTIFICATO VARCHAR2(64), ' ||
			'ALG_HASH VARCHAR2(64), ' ||
			'SOGGETTO VARCHAR2(64), ' ||
			'PAESE VARCHAR2(64), ' ||
			'TSR_FILE CLOB, ' ||
			'CONSTRAINT PK_DPA_TIMESTAMP_DOC PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/ 	
--FINE TIMESTAMP DOCUMENTI


--NOTIFICHE MAIL PEC
begin
declare cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_TIPO_NOTIFICA';
        if (cnt = 0) then
            execute immediate
				'CREATE TABLE DPA_TIPO_NOTIFICA' ||
				'(' ||
				'SYSTEM_ID            NUMBER                   NOT NULL,' ||
				'VAR_CODICE_NOTIFICA  VARCHAR2(50 BYTE)        NOT NULL,' ||
				'VAR_DESCRIZIONE      VARCHAR2(255 BYTE)' ||
				')TABLESPACE @ora_dattblspc_name'
            end if;
            end;
            end;
            /

			
			begin
declare cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_NOTIFICA';
        if (cnt = 0) then
            execute immediate
				'CREATE TABLE DPA_NOTIFICA' ||
				'(' ||
				'SYSTEM_ID               NUMBER,' ||
				'ID_TIPO_NOTIFICA        NUMBER,' ||
				'DOCNUMBER               NUMBER,' ||
				'VAR_MITTENTE            VARCHAR2(255 BYTE),' ||
				'VAR_TIPO_DESTINATARIO   VARCHAR2(100 BYTE),' ||
				'VAR_DESTINATARIO        VARCHAR2(255 BYTE),' ||
				'VAR_RISPOSTE            VARCHAR2(255 BYTE),' ||
				'VAR_OGGETTO             VARCHAR2(516 BYTE),' ||
				'VAR_GESTIONE_EMITTENTE  VARCHAR2(255 BYTE),' ||
				'VAR_ZONA                VARCHAR2(10 BYTE),' ||
				'VAR_GIORNO_ORA          DATE,' ||
				'VAR_IDENTIFICATIVO      VARCHAR2(516 BYTE),' ||
				'VAR_MSGID               VARCHAR2(516 BYTE),' ||
				'VAR_TIPO_RICEVUTA       VARCHAR2(516 BYTE),' ||
				'VAR_CONSEGNA            VARCHAR2(516 BYTE),' ||
				'VAR_RICEZIONE           VARCHAR2(516 BYTE),' ||
				'VAR_ERRORE_ESTESO       CLOB,' ||
				' VAR_ERRORE_RICEVUTA     VARCHAR2(50 BYTE)' ||
				')TABLESPACE @ora_dattblspc_name'
            end if;
            end;
            end;
            /

			begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_NOTIFICA';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_NOTIFICA;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_NOTIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_TIPO_NOTIFICA';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_TIPO_NOTIFICA;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_TIPO_NOTIFICA START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/


begin
    declare cnt int;
  begin
    select count(*) into cnt from cols where table_name='PROFILE' and column_name='CHA_DOCUMENTO_DA_PEC';
    if (cnt = 0) then        
            execute immediate 'ALTER TABLE PROFILE ADD CHA_DOCUMENTO_DA_PEC VARCHAR(1)';
    end if;
    end;
end;
/

-- LOCALITA'
begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_LOCALITA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_LOCALITA VARCHAR2(128)';
		end if;
	end if;
	end;
end;
/

-- modifica store procedure

 

CREATE OR REPLACE PROCEDURE PAT_PRE.sp_modify_corr_esterno (
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
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
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
               outvalue := 0;
         END dati_canale_utente;
      END IF;

      IF /* 0 */ outvalue = 1
      THEN
         IF /* 1 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
         THEN
            cha_dettaglio := '1';
         END IF;                                                       /* 1 */

--VERIFICO se il corrisp è stato utilizzato come dest/mitt di protocolli
         SELECT COUNT (id_profile)
           INTO myprofile
           FROM dpa_doc_arrivo_par
          WHERE id_mitt_dest = idcorrglobale;

-- 1) non è stato mai utilizzato come corrisp in un protocollo
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
                  outvalue := 0;
                  RETURN;
            END;

/* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDTATA A BUON FINE

PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI

*/
            IF /* 3 */ cha_tipourp = 'U' OR cha_tipourp = 'P'
            THEN

               <<update_dpa_dett_globali>>
               BEGIN
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
                   WHERE id_corr_globali = idcorrglobale;
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     outvalue := 0;
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
                  outvalue := 0;
                  RETURN;
            END;
         ELSE
-- caso 2) Il corrisp è stato utilizzato come corrisp in un protocollo

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
                  outvalue := 0;
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
                  outvalue := 0;
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
                     outvalue := 0;
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
                  outvalue := 0;
                  RETURN;
            END inserimento_dpa_t_canale_corr;
         END IF;                                                       /* 2 */
      END IF /* 0 */;

      returnvalue := outvalue;
   END;
END;
/
-- FINE LOCALITA'

--NUOVA FUNZIONE getIfModelloAutorizzato
CREATE OR REPLACE FUNCTION @db_user.getIfModelloAutorizzato
(
id_ruolo  number,
id_people  number,
system_id  number,
id_modelloTrasm number,
accesssRigth number
)
RETURN NUMBER IS retVal NUMBER;
--accesssRigth number;
idRagione  number;
tipo_diritto  char;
cursor cur is
select distinct ID_RAGIONE FROM dpa_modelli_mitt_dest WHERE id_modello=id_modelloTrasm and CHA_TIPO_MITT_DEST <> 'M';
BEGIN
retVal:=1;
--accesssRigth:= getaccessrights(id_ruolo, id_people, system_id);
if (accesssRigth = 45)
then
begin
OPEN cur;
LOOP
FETCH cur INTO idRagione;
EXIT WHEN cur%NOTFOUND;
select CHA_TIPO_DIRITTI into tipo_diritto from DPA_RAGIONE_TRASM where system_id=idRagione;
if(tipo_diritto <> 'R' and tipo_diritto <> 'C')
then
begin
retVal:=0;
end;
end if;
exit when  retVal=0;
end loop;
close cur;
end;
end if;
RETURN retVal;
END getIfModelloAutorizzato;
/
--FINE NUOVA FUNZIONE getIfModelloAutorizzato

--INIZIO NEW CORRCAT
CREATE OR REPLACE FUNCTION CORRCAT (docId INT, tipo_proto VARCHAR)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);
tipo_mitt_dest VARCHAR(10);
LNG INT;

CURSOR cur IS
SELECT c.var_desc_corr, dap.cha_tipo_mitt_dest
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item,tipo_mitt_dest;
EXIT WHEN cur%NOTFOUND;

LNG:=LENGTh(risultato);

IF(risultato IS NOT NULL anD LNG>=(3900-128))
tHEN RETURN RISULTATO||'...';
ELSE
BEGIN

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'MD') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MM)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'A' AND tipo_mitt_dest = 'I') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (MI)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'M') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (M)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'D') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (D)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'I' AND tipo_mitt_dest = 'C') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ||' (CC)';
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'L') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_LISTA(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;

IF(tipo_proto = 'P' AND tipo_mitt_dest = 'F') THEN
IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '|| DEST_IN_RF(docId);
ELSE
risultato := risultato||item;
END IF;
END IF;


END;
END IF;

END LOOP;

RETURN risultato;

END Corrcat;
/
--FINE NEW CORRCAT

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='IS_ENABLED_SMART_CLIENT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD IS_ENABLED_SMART_CLIENT CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='IS_ENABLED_SMART_CLIENT';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD IS_ENABLED_SMART_CLIENT CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_AMMINISTRA' AND column_name='SMART_CLIENT_PDF_CONV_ON_SCAN';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_AMMINISTRA ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='SMART_CLIENT_PDF_CONV_ON_SCAN';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD SMART_CLIENT_PDF_CONV_ON_SCAN CHAR(1)';
        end if;
    end;
end;
/

CREATE OR REPLACE FUNCTION classcat (docId INT)
RETURN varchar IS risultato varchar(4000);

item varchar(4000);

CURSOR cur IS
SELECT DISTINCT A.VAR_CODICE
FROM PROJECT A
WHERE A.CHA_TIPO_PROJ = 'F'
AND A.SYSTEM_ID IN
(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=docId);

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

RETURN risultato;

END classcat;





--inizio chiavi di configurazione
begin
    declare
           cnt int;
    begin    
    select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_VOCI_MENU_ADMIN';
        if (cnt = 0) then
               execute immediate 'CREATE SEQUENCE SEQ_DPA_VOCI_MENU_ADMIN START WITH 1 INCREMENT BY 1 MINVALUE 1';
        end if;   
        
     end;
 end;

begin
    declare
           cnt int;
    begin    
    select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_CHIAVI_CONFIG_TEMPLATE';
        if (cnt = 0) then
               execute immediate 'CREATE SEQUENCE SEQ_DPA_CHIAVI_CONFIG_TEMPLATE START WITH 1 INCREMENT BY 1 MINVALUE 1';
        end if;   
        
     end;
 end;

--Dimitri de filippo 22/10/2010
--creazione tabella template per le chiavi di configurazione non globali
begin
    declare   cnt INT;
    begin
select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIG_TEMPLATE';
if (cnt = 0) then
  execute immediate
    'CREATE TABLE DPA_CHIAVI_CONFIG_TEMPLATE ' ||
    '( ' ||
      'SYSTEM_ID            NUMBER                   NOT NULL, ' ||
      'VAR_CODICE           varchar(32)              NOT NULL, ' ||
      'VAR_DESCRIZIONE      varchar(512),                      ' ||
      'VAR_VALORE           varchar(32)              NOT NULL, ' ||
      'CHA_TIPO_CHIAVE      char(1),                           ' ||
      'CHA_VISIBILE         char(1)                  DEFAULT 1, ' ||
      'CHA_MODIFICABILE     char(1)                  DEFAULT 1 ' ||
    ')';
     
    execute immediate
     'CREATE UNIQUE INDEX DPA_CHIAVI_CONFIG_TEMPLATE_PK ON DPA_CHIAVI_CONFIG_TEMPLATE     ' ||
    '(SYSTEM_ID)                                               ' ||
    'LOGGING                                                   ';
    
    execute immediate
    'ALTER TABLE DPA_CHIAVI_CONFIG_TEMPLATE ADD ( ' ||
      'CONSTRAINT DPA_CHIAVI_CONFIG_TEMPLATE_PK                        ' ||
     'PRIMARY KEY                                              ' ||
     '(SYSTEM_ID)                                              ' ||
        'USING INDEX  ) ';

    
    end if;
    end;    
end;    




--aggiunge la colonna “VAR_CODICE_OLD_WEBCONFIG” alla tabella
--“DPA_CHIAVI_CONFIGURAZIONE” per tenere traccia del vecchio codice della chiave
-- nella vecchia gestione da Web.config
if not exists (SELECT * FROM syscolumns WHERE name='VAR_CODICE_OLD_WEBCONFIG' and id in 
	(SELECT id FROM sysobjects WHERE name='DPA_CHIAVI_CONFIGURAZIONE' and xtype='U'))
BEGIN
	ALTER TABLE [DPA_CHIAVI_CONFIGURAZIONE]
	ADD  VAR_CODICE_OLD_WEBCONFIG  varchar(64);
END;

--aumenta la lunghezza della colonna VAR_DESCRIZIONE da varchar(256) a 
--varchar(512)
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CHIAVI_CONFIGURAZIONE' and column_name='VAR_DESCRIZIONE';
      if (cnt != 0) then
            execute immediate 'ALTER TABLE DPA_CHIAVI_CONFIGURAZIONE alter column VAR_DESCRIZIONE varchar(512)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_MAX_LENGTH_NOTE';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE ( SYSTEM_ID,   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE ,  CHA_GLOBALE ,  VAR_CODICE_OLD_WEBCONFIG ) VALUES ((select max(SYSTEM_ID)+1 from DPA_CHIAVI_CONFIGURAZIONE),  ''FE_MAX_LENGTH_NOTE'', ''Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Note'', ''2000'', ''F'', ''1'', ''1'', ''0'', ''FE_MAX_LENGTH_NOTE'')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_MAX_LENGTH_OGGETTO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE ( SYSTEM_ID,   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE ,  CHA_GLOBALE ,  VAR_CODICE_OLD_WEBCONFIG ) VALUES ((select max(SYSTEM_ID)+1 from DPA_CHIAVI_CONFIGURAZIONE),  ''FE_MAX_LENGTH_OGGETTO'', ''Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto '', ''2000'', ''F'', ''1'', ''1'', ''0'', ''FE_MAX_LENGTH_OGGETTO'')';
        end if;
    end if;
    end;
end;
/

 
 

 
 
begin
    declare   cnt int;
 begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_MAX_LENGTH_NOTE';
   if (cnt = 0) then
         INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE ( SYSTEM_ID,   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) 
         SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL,  'FE_MAX_LENGTH_NOTE', 'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Note ', '2000', 'F', '1', '1'  FROM DUAL ;
   end if;
  end;    
end;    
/
begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_MAX_LENGTH_OGGETTO';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE ( SYSTEM_ID,   VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE )
 SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL,  'FE_MAX_LENGTH_OGGETTO', 'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Oggetto ', '2000', 'F', '1', '1'FROM DUAL;
   end if;
    end;    
end;    
/
begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_MAX_LENGTH_DESC_FASC';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE (  SYSTEM_ID ,  VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) 
SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL, 'FE_MAX_LENGTH_DESC_FASC', 'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Fascicolo ', '2000', 'F', '1', '1' FROM DUAL;
  end if;
    end;    
end;    
/

begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_MAX_LENGTH_DESC_TRASM';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE (SYSTEM_ID, VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) 
SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL, 'FE_MAX_LENGTH_DESC_TRASM', 'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Trasmissione ', '2000', 'F', '1', '1' FROM DUAL;
  end if;
    end;    
end;    

/
begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_MAX_LENGTH_DESC_ALLEGATO';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE (SYSTEM_ID, VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  ) 
SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL,'FE_MAX_LENGTH_DESC_ALLEGATO', 'Chiave utilizzata per l''impostazione del numero massimo di caratteri digitabili nei campi Descrizione Allegato ', '200', 'F', '1', '1' FROM DUAL;
  end if;
    end;    
end;    

/
begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_RICEVUTA_PROTOCOLLO_PDF';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE (SYSTEM_ID, VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE   ) 
SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL,'FE_RICEVUTA_PROTOCOLLO_PDF', 'Chiave per l''impostazione del formato di stampa della ricevuta di un protocollo. Valori: 1= PDF; 0=stampa tramite activex ', '0', 'F', '1', '1' FROM DUAL;
  end if;
    end;    
end;    

/
begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_VOCI_MENU_ADMIN where VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE';
if (cnt = 0) then
INSERT INTO @db_user.DPA_VOCI_MENU_ADMIN (SYSTEM_ID,VAR_CODICE,VAR_DESCRIZIONE,VAR_VISIBILITA_MENU)
SELECT SEQ_DPA_VOCI_MENU_ADMIN.NEXTVAL,'FE_ABILITA_GEST_DOCS_ST_FINALE','GESTIONE DOCS STATO FINALE',''FROM DUAL;
  end if;
    end;    
end;    
/

begin
    declare   cnt int;
    begin
select count(*) into cnt from DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE='FE_ABILITA_GEST_DOCS_ST_FINALE';
if (cnt = 0) then
INSERT into @db_user.DPA_CHIAVI_CONFIG_TEMPLATE (SYSTEM_ID, VAR_CODICE ,  VAR_DESCRIZIONE ,  VAR_VALORE ,  CHA_TIPO_CHIAVE ,  CHA_VISIBILE ,  CHA_MODIFICABILE  )
SELECT SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.NEXTVAL,'FE_ABILITA_GEST_DOCS_ST_FINALE', 'Chiave per consentire lo sblocco dei documenti in stato finale. Valori: 1= CONSENTI; 0=NON CONSENTIRE ', '1','F', '0', '0' FROM DUAL;
  end if;
    end;    
end;    

/

--Dimitri de Filippo
--Stored che scorre la tabella DPA_CHIAVI_CONFIG_TEMPLATE e 
--inserisce le corrispondenti chiavi non globali nella tabella DPA_CHIAVI_CONFIGURAZIONE per ciascuna amministrazione

CREATE OR REPLACE procedure @db_user.CREA_KEYS_AMMINISTRA IS
sysCurrAmm INT;
sysCurrKey varchar(32);
cnt INT;

          CURSOR currAmm -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
            is
            SELECT system_id
            FROM @db_user.DPA_AMMINISTRA;
  
               CURSOR currKey -- CURSORE CHE SCORRE LE CHIAVI della tabella DPA_CHIAVI_CONFIG_TEMPLATE

                        is
                        SELECT var_codice
                        FROM @db_user.DPA_CHIAVI_CONFIG_TEMPLATE;
           
                begin
                     open currAmm;
                        LOOP
                         fetch  currAmm into sysCurrAmm;
                         exit when currAmm%NOTFOUND;
                           BEGIN

                          ---cursore annidato x le chiavi di configurazione
                        begin
 
                         open currKey;
                            
                                LOOP
                                 fetch currKey into sysCurrKey;
                                 exit when currKey%NOTFOUND;
                               BEGIN
                                   
                                    SELECT COUNT(*) INTO cnt from @db_user.DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE=sysCurrKey and ID_AMM =sysCurrAmm;
                                end ;                     
                                    BEGIN
             
                                        IF (cnt = 0) THEN
                                            insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE
                                            (SYSTEM_ID,
                                            ID_AMM, 
                                            VAR_CODICE, 
                                            VAR_DESCRIZIONE, 
                                            VAR_VALORE, 
                                            CHA_TIPO_CHIAVE, 
                                            CHA_VISIBILE, 
                                            CHA_MODIFICABILE, 
                                            CHA_GLOBALE 
                                            )
                                            (select 
                                            (select max(SYSTEM_ID)+1 from @db_user.DPA_CHIAVI_CONFIGURAZIONE),
                                            sysCurrAmm as ID_AMM,
                                             VAR_CODICE, 
                                            VAR_DESCRIZIONE, 
                                            VAR_VALORE, 
                                            CHA_TIPO_CHIAVE, 
                                            CHA_VISIBILE, 
                                            CHA_MODIFICABILE, 
                                            '0' 
                                            from @db_user.DPA_CHIAVI_CONFIG_TEMPLATE 
                                            where VAR_CODICE=sysCurrKey
                                            and rownum=1);
                                        
                                        END IF;


                                    END;
                               end loop;
                         close  currKey;
                         end;
                     
                        


                           --- fine cursore annidato per chiavi di configurazione

                          END;
                        end loop;
                    CLOSE currAmm;
                     commit;
                 end;
/

/
----------------------------------------fine gestione chiavi configurazione

BEGIN 
  CREA_KEYS_AMMINISTRA;
  COMMIT;
END;
/

CREATE OR REPLACE PROCEDURE @db_user.setsecurityRuoloReg
(idCorrGlobali IN NUMBER, idProfile IN NUMBER,diritto IN NUMBER,Idreg in NUMBER ReturnValue OUT NUMBER) IS

idGruppo dpa_corr_globali.id_gruppo%TYPE;

BEGIN

SELECT ID_GRUPPO INTO idGruppo FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID =
idCorrGlobali;

IF (idGruppo IS NOT NULL) THEN
BEGIN
SELECT MAX(accessrights) INTO ReturnValue from security  where thing =
idProfile and personorgroup = idGruppo;
END;
--
IF (ReturnValue < diritto ) THEN
BEGIN
update security set accessrights = diritto where thing = idProfile and
personorgroup = idGruppo;
END;
END IF;

IF (ReturnValue IS NULL) THEN
BEGIN
insert into security values(idProfile,idGruppo,diritto,null,'A');

END;
END IF;

END IF;



insert into security ( THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO )  
select SYSTEM_ID,idGruppo,diritto,null,'A' from PROFILE p where ID_REGISTRO=Idreg
and num_proto is not null
and not exists (select 'x' from SECURITY s1 where s1.THING=p.system_id and
s1.PERSONORGROUP=idGruppo and s1.ACCESSRIGHTS=diritto )

ReturnValue := diritto;
END setsecurityRuoloReg;
/

-- INOLTRA MASSIVO
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='MASSIVE_INOLTRA';
    IF (cnt = 0) THEN        
       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('MASSIVE_INOLTRA' , 'Abilita l''utente a compiere inoltra massivi', NULL, 'N');
    END IF;
    END;
END;
/

-- FINE AZIONI MASSIVE
EXCEPTION
WHEN NO_DATA_FOUND THEN ReturnValue:=0;
WHEN OTHERS THEN ReturnValue := -1;




begin
	declare cnt int;
  begin
	select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
	if (cnt != 0) then		
		select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='FORMATO_ORA';
  	if (cnt = 0) then
        	execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD FORMATO_ORA VARCHAR(10)';
		end if;
	end if;
	end;
end;
/ 
--FINE MODIFICA CAMPO DATA PROFILAZIONE DINAMICA

begin
    declare   cnt int;
    begin
select count(*) into cnt from user_tables where table_name='DPA_MODELLI_DELEGA';
if (cnt = 0) then
  execute immediate
    'CREATE TABLE DPA_MODELLI_DELEGA ' ||
    '( ' ||
      'SYSTEM_ID            NUMBER                   NOT NULL, ' ||
      'ID_PEOPLE_DELEGANTE  NUMBER                   NOT NULL, ' ||
      'ID_RUOLO_DELEGANTE   NUMBER,                            ' ||
      'ID_PEOPLE_DELEGATO   NUMBER                   NOT NULL, ' ||
      'ID_RUOLO_DELEGATO    NUMBER                   NOT NULL, ' ||
      'INTERVALLO           INTEGER,                           ' ||
      'DTA_INIZIO           DATE,                              ' ||
      'DTA_FINE             DATE,                              ' ||
      'NOME                 VARCHAR2(100 BYTE)       NOT NULL  ' ||
    ')                                                         ';
     
    execute immediate
     'CREATE UNIQUE INDEX DPA_MODELLI_DELEGA_PK ON DPA_MODELLI_DELEGA     ' ||
    '(SYSTEM_ID)                                               ' ||
    'LOGGING                                                   ';
    
    execute immediate
    'ALTER TABLE DPA_MODELLI_DELEGA ADD (                      ' ||
      'CONSTRAINT DPA_MODELLI_DELEGA_PK                        ' ||
     'PRIMARY KEY                                              ' ||
     '(SYSTEM_ID)                                              ' ||
        'USING INDEX  )                                         ';

    
    end if;
    end;    
end;    
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='TIPO_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD TIPO_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='TIPO_OBJ_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD TIPO_OBJ_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='TIPO_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD TIPO_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='TIPO_OBJ_LINK';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD TIPO_OBJ_LINK varchar2(50 byte)';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO where upper(descrizione)='LINK';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL,''Link'',''Link'')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_OGGETTO_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where upper(descrizione)='LINK';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID,TIPO,DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL,''Link'',''Link'')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_MODELLI_TRASM ';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_MODELLI_TRASM ' and column_name='CHA_MANTIENI_LETTURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_MODELLI_TRASM ADD CHA_MANTIENI_LETTURA char(1) NULL';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_RAGIONE_TRASM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_RAGIONE_TRASM' and column_name='CHA_MANTIENI_LETT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_RAGIONE_TRASM ADD CHA_MANTIENI_LETT char(1) NULL';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG where VAR_CODICE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_ANAGRAFICA_LOG SET VAR_CODICE = ''PUT_FILE'' WHERE VAR_CODICE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_LOG where VAR_COD_AZIONE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_LOG SET VAR_COD_AZIONE = ''PUT_FILE'' WHERE VAR_COD_AZIONE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare cnt int;
    begin
    select count(*) into cnt from DPA_LOG_STORICO where VAR_COD_AZIONE='GET_FIRMA';
        if (cnt > 0) then
            execute immediate 'UPDATE DPA_LOG_STORICO SET VAR_COD_AZIONE = ''PUT_FILE'' WHERE VAR_COD_AZIONE = ''GET_FIRMA''';            
        end if;
    end;    
end;   
/

begin
    declare
         cnt int;
begin
select count(*) into cnt from user_tables where table_name='DPA_DISSERVIZI';
		if (cnt = 0) then
			execute immediate
                'CREATE TABLE DPA_DISSERVIZI ' ||
                '( ' ||
                '  SYSTEM_ID             NUMBER, ' ||
                '  STATO                 VARCHAR2(32 BYTE), ' ||
                '  TESTO_NOTIFICA        VARCHAR2(1024 BYTE), ' ||
                '  TESTO_EMAIL_NOTIFICA  VARCHAR2(1024 BYTE), ' ||
                '  TESTO_PAG_CORTESIA    VARCHAR2(1024 BYTE), ' ||
                '  TESTO_EMAIL_RIPRESA   VARCHAR2(4 BYTE), ' ||
                '  NOTIFICATO      NUMBER(10) ' ||
                ') TABLESPACE @ora_dattblspc_name';
            end if;
            end;
            end;
            /

			begin
    declare
         cnt int;
    begin    
    
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_DISSERVIZI';
        if (cnt = 0) then
         begin
           select max(system_id)+1 into cnt from DPA_DISSERVIZI;
           execute immediate 'CREATE SEQUENCE SEQ_DPA_DISSERVIZI START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
         end;
        end if;    
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PEOPLE' AND column_name='ACCETTAZIONE_DISSERV';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PEOPLE ADD ACCETTAZIONE_DISSERV VARCHAR(1)';
        end if;
    end;
end;
/    
    
	-- campo luogo nascita

begin

    declare cnt int;

  begin

    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';

    if (cnt != 0) then        

        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_LUOGO_NASCITA';

      if (cnt = 0) then

            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_LUOGO_NASCITA VARCHAR2(64)';

        end if;

    end if;

    end;

end;

/

 

-- campo data di nascita

begin

    declare cnt int;

  begin

    select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='DTA_NASCITA';

    if (cnt = 0) then        

            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD DTA_NASCITA VARCHAR2(64)';

    end if;

    end;

end;

/

 

-- campo titolo

begin

    declare cnt int;

  begin

    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';

    if (cnt != 0) then        

        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='VAR_TITOLO';

      if (cnt = 0) then

            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD VAR_TITOLO VARCHAR2(64)';

        end if;

    end if;

    end;

end;

/

-- TABELLA DELLE QUALIFICHE

 

begin

    declare   cnt int;

      begin 

  

        select count(*) into cnt from user_tables where table_name='DPA_QUALIFICA_CORRISPONDENTE';

        if (cnt = 0) then

          execute immediate    

            'CREATE TABLE DPA_QUALIFICA_CORRISPONDENTE ' ||

            '( ' ||

            'SYSTEM_ID INT PRIMARY KEY NOT NULL, ' ||

            'VAR_TITOLO VARCHAR2(64), ' ||

            'DTA_FINE_VALIDITA DATE) ';    

        end if;

    end;    

end;   

/

-- chiave esterna da fare solo dopo aver eseguito la creazione della tabella DPA_QUALIFICA_CORRISPONDENTE

 

 

begin

    declare cnt int;

  begin

    select count(*) into cnt from user_tables where table_name='DPA_DETT_GLOBALI';

    if (cnt != 0) then        

        select count(*) into cnt from cols where table_name='DPA_DETT_GLOBALI' and column_name='ID_QUALIFICA_CORR';

      if (cnt = 0) then

            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD ID_QUALIFICA_CORR INT';

            execute immediate 'ALTER TABLE DPA_DETT_GLOBALI ADD FOREIGN KEY (ID_QUALIFICA_CORR) REFERENCES DPA_QUALIFICA_CORRISPONDENTE(SYSTEM_ID)';

        end if;

    end if;

    end;

end;

/



-- popolamento tabella titoli corrispondenti



BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Arch.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 1, 'Arch.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Avv.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE  VALUES ( 2, 'Avv.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Dott.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 3, 'Dott.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Dott.ssa';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 4, 'Dott.ssa', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Dr.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 5, 'Dr.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Geom.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 6, 'Geom.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Ing.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 7, 'Ing.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Mo.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 8, 'Mo.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Mons.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 9, 'Mons.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'on.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 10, 'on.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Prof.';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 11, 'Prof.', NULL );

         END IF;

      END IF;

   END;

END;

/


BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Prof.ssa';

 

         IF (cnt = 0)

         THEN

            INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 12, 'Prof.ssa', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Rag.';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 13, 'Rag.', NULL );

         END IF;

      END IF;

   END;

END;

/

BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Rev.';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 14, 'Rev.', NULL );

         END IF;

      END IF;

   END;

END;

/


BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Sig.';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 15, 'Sig.', NULL );

         END IF;

      END IF;

   END;

END;

/


BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Sig.na.';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 16, 'Sig.na.', NULL );

         END IF;

      END IF;

   END;

END;

/


BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Sig.ra';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 17, 'Sig.ra', NULL );

         END IF;

      END IF;

   END;

END;

/


BEGIN

   DECLARE

      cnt   INT;

   BEGIN

      SELECT COUNT (*) INTO cnt FROM cols c

       WHERE c.table_name = 'DPA_QUALIFICA_CORRISPONDENTE'

         AND c.column_name = 'VAR_TITOLO';

 

      IF (cnt = 1)

      THEN

         SELECT COUNT (*) INTO cnt FROM dpa_qualifica_corrispondente qc

          WHERE qc.var_titolo = 'Sig.ra/sig.na';

 

         IF (cnt = 0)

         THEN

           INSERT INTO DPA_QUALIFICA_CORRISPONDENTE VALUES ( 18, 'Sig.ra/sig.na', NULL );

         END IF;

      END IF;

   END;

END;

/

-- fine popolamento tabella titoli

    
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_TIMER_DISSERVIZIO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '((select max(SYSTEM_ID)+1 from DPA_CHIAVI_CONFIGURAZIONE)' ||
           ',0' ||
           ', ''FE_TIMER_DISSERVIZIO'' '||
           ', ''Configurazione Timer DIsservizio, valore corrispondente a intervallo del Timer in millisecondi'''||
           ',''60000''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''0''' || 
           ')';
        end if;
    end if;
    end;
end;
/

-- REPORT FASCICOLI
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='EXP_FASC_COUNT';
    IF (cnt = 0) THEN        
       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 
        VALUES ('EXP_FASC_COUNT' , 'Abilita l''utente ai report dei fascicoli', NULL, 'N');
    END IF;
    END;
END;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_RUOLO_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_RUOLO_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_UO_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_UO_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROJECT';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROJECT' and column_name='ID_AUTHOR_CHIUSURA';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROJECT ADD ID_AUTHOR_CHIUSURA NUMBER';
        end if;
    end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_FORMATI_DOCUMENTO' AND column_name='FILE_TYPE_SIGNATURE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_SIGNATURE INTEGER NULL';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_FORMATI_DOCUMENTO' AND column_name='FILE_TYPE_PRESERVATION';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_FORMATI_DOCUMENTO ADD FILE_TYPE_PRESERVATION INTEGER NULL';
        end if;
    end;
end;
/

//Corte dei conti
begin
    declare cnt int;  
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO where DESCRIZIONE = 'ContatoreSottocontatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL, ''ContatoreSottocontatore'', ''ContatoreSottocontatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where DESCRIZIONE = 'ContatoreSottocontatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL, ''ContatoreSottocontatore'', ''ContatoreSottocontatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM_FASC' and column_name='MODULO_SOTTOCONTATORE';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM_FASC ADD MODULO_SOTTOCONTATORE NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_OGGETTI_CUSTOM';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_OGGETTI_CUSTOM' and column_name='MODULO_SOTTOCONTATORE';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_OGGETTI_CUSTOM ADD MODULO_SOTTOCONTATORE NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CONTATORI_DOC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CONTATORI_DOC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_CONTATORI_DOC ADD VALORE_SC INTEGER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CONTATORI_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_CONTATORI_FASC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_CONTATORI_FASC ADD VALORE_SC INTEGER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD VALORE_SC NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='VALORE_SC';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD VALORE_SC NUMBER';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='DTA_INS';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD DTA_INS DATE';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='DTA_INS';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD DTA_INS DATE';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;  
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO where DESCRIZIONE = 'Separatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO.NEXTVAL, ''Separatore'', ''Separatore'')';
      end if;
  end;
end;
/

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_TIPO_OGGETTO_FASC where DESCRIZIONE = 'Separatore';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_TIPO_OGGETTO_FASC (SYSTEM_ID, TIPO, DESCRIZIONE) VALUES (SEQ_DPA_TIPO_OGGETTO_FASC.NEXTVAL, ''Separatore'', ''Separatore'')';
      end if;
  end;
end;
/
//Fine corte dei conti

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='CODICE_DB';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD CODICE_DB VARCHAR(50)';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASSOCIAZIONE_TEMPLATES';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASSOCIAZIONE_TEMPLATES' and column_name='MANUAL_INSERT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASSOCIAZIONE_TEMPLATES ADD MANUAL_INSERT INT';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='CODICE_DB';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD CODICE_DB VARCHAR(50)';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_ASS_TEMPLATES_FASC';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_ASS_TEMPLATES_FASC' and column_name='MANUAL_INSERT';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_ASS_TEMPLATES_FASC ADD MANUAL_INSERT INT';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_ATTO';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='PATH_MOD_SU';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD PATH_MOD_SU VARCHAR(255)';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE = 'GEST_SU';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED) VALUES (''GEST_SU'', ''Abilita il sottomenu Stampa unione del menu Gestione'', null,''N'')';
      end if;
  end;
end;

begin
    declare cnt int;
  begin
      select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI where COD_FUNZIONE = 'MASSIVE_REMOVE_VERSIONS';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE,CHA_TIPO_FUNZ, DISABLED) VALUES (''MASSIVE_REMOVE_VERSIONS'', ''Abilita la rimozione massiva delle versioni dei doc grigi'', null,''N'')';
      end if;
  end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_DETTAGLIO_TRASM_TODOLIST';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ.NEXTVAL' ||
           ',0' ||
           ', ''FE_DETTAGLIO_TRASM_TODOLIST'' '||
           ', ''Utilizzata per visualizzare l intero dettaglio della trasmissione dalla todolist'''||
           ',''1''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''1''' || 
           ')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_EXPORT_DA_MODELLO';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ.NEXTVAL' ||
           ',0' ||
           ', ''FE_EXPORT_DA_MODELLO'' '||
           ', ''Abilitazione export ricerca da modello (0=disabilitato, 1= abilitato)'''||
           ',''1''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''0''' || 
           ')';
        end if;
    end if;
    end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_TIPO_ATTO';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='DPA_TIPO_ATTO' and column_name='Path_Mod_Exc';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TIPO_ATTO ADD Path_Mod_Exc VARCHAR(255)';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='DPA_CHIAVI_CONFIGURAZIONE';
    if (cnt != 0) then        
        select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE='FE_ESTRAZIONE_LOG';
      if (cnt = 0) then
        execute immediate 'INSERT INTO DPA_CHIAVI_CONFIGURAZIONE'||
            '(SYSTEM_ID'||
            ',ID_AMM'||
           ',VAR_CODICE'||
           ',VAR_DESCRIZIONE'||
           ',VAR_VALORE' ||
           ',CHA_TIPO_CHIAVE' ||
           ',CHA_VISIBILE' ||
           ',CHA_MODIFICABILE' ||
           ',CHA_GLOBALE' ||
           ')' ||
     'VALUES' ||
           '(SEQ.NEXTVAL' ||
           ',0' ||
           ', ''FE_ESTRAZIONE_LOG'' '||
           ', ''Utilizzata per abilitare estrazione dei log da amministrazione'''||
           ',''1''' ||
           ',''F''' ||
           ',''1''' ||
           ',''1''' ||
           ',''1''' || 
           ')';
        end if;
    end if;
    end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_CHIAVI_CONFIGURAZIONE l where l.var_codice ='BE_CONSOLIDAMENTO';
    
    if (cnt = 0) then       
    insert into DPA_CHIAVI_CONFIGURAZIONE
    (
    system_id,
    id_amm,
    var_codice,
    var_descrizione,
    var_valore,
    cha_tipo_chiave,
    cha_visibile,
    cha_modificabile,
    cha_globale,
    var_codice_old_webconfig
    )
    values 
    (
    seq.nextval,
    0,
    'BE_CONSOLIDAMENTO',
    'Abilitazione della funzione di consolidamento dello stato di un documento',
    '1',
    'B',
    '1',
    '1',
    '1',
    null
    );
    end if;
  end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='CONSOLIDADOCUMENTO';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'CONSOLIDADOCUMENTO',
	'Azione di consolidamento dello stato di un documento',
	'DOCUMENTO',
	'CONSOLIDADOCUMENTO'
	);
    end if;
  end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_CONSOLIDAMENTO';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_CONSOLIDAMENTO', 'Abilita il consolidamento di un documento', '', 'N');
    end if;
    end;
end;
/
begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_CONSOLIDAMENTO_METADATI';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_CONSOLIDAMENTO_METADATI', 'Abilita il consolidamento dei metadati di un documento', '', 'N');
    end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_STATE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_STATE CHAR(1)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_AUTHOR';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_AUTHOR NUMBER(10)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_ROLE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_ROLE NUMBER(10)';
        end if;
    end;
end;
/
begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='PROFILE' AND column_name='CONSOLIDATION_DATE';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CONSOLIDATION_DATE DATE';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_STATI' AND column_name='STATO_CONSOLIDAMENTO';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_STATI ADD STATO_CONSOLIDAMENTO CHAR(1) NULL';
        end if;
    end;
end;
/

begin
    declare cnt int;
    begin
        select count(*) into cnt from user_tables where table_name='DPA_CONSOLIDATED_DOCS';
        if (cnt = 0) then
            execute immediate 
            'CREATE TABLE DPA_CONSOLIDATED_DOCS
                (
                  ID                       NUMBER(10),
                  DOCNAME                  VARCHAR2(240 BYTE),
                  CREATION_DATE            DATE,
                  DOCUMENTTYPE             NUMBER(10),
                  AUTHOR                   NUMBER(10),
                  AUTHOR_NAME              VARCHAR2(4000 BYTE),
                  ID_RUOLO_CREATORE        NUMBER,
                  RUOLO_CREATORE           VARCHAR2(4000 BYTE),
                  NUM_PROTO                NUMBER(10),
                  NUM_ANNO_PROTO           NUMBER(10),
                  DTA_PROTO                DATE,
                  ID_PEOPLE_PROT           NUMBER,
                  PEOPLE_PROT              VARCHAR2(4000 BYTE),
                  ID_RUOLO_PROT            NUMBER,
                  RUOLO_PROT               VARCHAR2(4000 BYTE),
                  ID_REGISTRO              NUMBER(10),
                  REGISTRO                 VARCHAR2(4000 BYTE),
                  CHA_TIPO_PROTO           VARCHAR2(1 BYTE),
                  VAR_PROTO_IN             VARCHAR2(128 BYTE),
                  DTA_PROTO_IN             DATE,
                  DTA_ANNULLA              DATE,
                  ID_OGGETTO               NUMBER(10),
                  VAR_PROF_OGGETTO         VARCHAR2(2000 BYTE),
                  MITT_DEST                VARCHAR2(4000 BYTE),
                  ID_DOCUMENTO_PRINCIPALE  INTEGER
                )';            
        end if;
    end;
end;
/

BEGIN
   DECLARE cnt   INT;
   BEGIN
	 SELECT COUNT (*) INTO cnt FROM user_tables WHERE table_name = 'DPA_CHIAVI_CONFIGURAZIONE';
		IF (cnt != 0) THEN
			SELECT COUNT (*) INTO cnt FROM dpa_chiavi_configurazione WHERE var_codice = 'FE_TAB_TRASM_ALL';
		 IF (cnt = 0) THEN
			INSERT INTO DPA_CHIAVI_CONFIGURAZIONE
				(SYSTEM_ID,
				ID_AMM,
				VAR_CODICE,
				VAR_DESCRIZIONE,
				VAR_VALORE,
				CHA_TIPO_CHIAVE,
				CHA_VISIBILE,CHA_MODIFICABILE,
				CHA_GLOBALE
				)
				VALUES
				(SEQ.NEXTVAL,
				0, 
				'FE_TAB_TRASM_ALL', 
				'Utilizzata per vedere dal tab classifica tutti i fascicoli',
				'1',
				'F',
				'1',
				'1',
				'1'
				);
         END IF;
      END IF;
   END;
END;

/

BEGIN

   DECLARE cnt   INT;

   BEGIN

        SELECT COUNT (*) INTO cnt FROM user_tables WHERE table_name = 'DPA_CHIAVI_CONFIGURAZIONE';

             IF (cnt != 0) THEN

                    SELECT COUNT (*) INTO cnt FROM dpa_chiavi_configurazione WHERE var_codice = 'BE_URL_WA';

              IF (cnt = 0) THEN

                    INSERT INTO DPA_CHIAVI_CONFIGURAZIONE

                           (SYSTEM_ID,

                           ID_AMM,

                           VAR_CODICE,

                           VAR_DESCRIZIONE,

                           VAR_VALORE,

                           CHA_TIPO_CHIAVE,

                           CHA_VISIBILE,CHA_MODIFICABILE,

                           CHA_GLOBALE

                           )

                           VALUES

                           (SEQ.NEXTVAL,

                           0, 

                           'BE_URL_WA', 

                           'Utilizzata per identificare il path del Front end per esportare il link tramite web service',

                           '1',

                           'B',

                           '1',

                           '1',

                           '1'

                           );

         END IF;

      END IF;

   END;

END;

 

/

begin
    declare cnt int;
    begin
        select count(*) into cnt from cols where table_name='DPA_TRASM_SINGOLA' AND column_name='HIDE_DOC_VERSIONS';
        if (cnt = 0) then
            execute immediate 'ALTER TABLE DPA_TRASM_SINGOLA ADD HIDE_DOC_VERSIONS CHAR(1)';
        end if;
    end;
end;
/
CREATE OR REPLACE FUNCTION isVersionVisible
(
versionId NUMBER,
idPeople NUMBER,
idGroup NUMBER
)
RETURN INT IS retValue INT;

idProfile NUMBER(10) := 0;
idDocumentoPrincipale NUMBER(10) := 0;
maxVersionId NUMBER(10) := 0;
hideVersions NUMBER(10) := 0;
ownership NUMBER(10) := 0;

BEGIN
retValue := 0;

-- 1) Reperimento IdProfile e DocNumber del documento
select  p.system_id into idProfile
from    versions v 
        inner join profile p on v.docnumber = p.docnumber
where   v.version_id = versionId;

-- 2) verifica se la versione richiesta è l'ultima
select max(v.version_id) into maxVersionId
from versions v
where v.docnumber = (select docnumber from profile where system_id = idProfile);

-- 2a) Verifica se l'idprofile si riferisce ad un allegato: 
--      in tal caso deve essere reperito l'id del documento principale
select p.id_documento_principale into idDocumentoPrincipale
from profile p 
where p.system_id = idProfile;

if (not idDocumentoPrincipale is null and idDocumentoPrincipale > 0) then
    -- La versione è di un allegato, 
    -- impostazione dell'id del documento principale come idprofile
    idProfile := idDocumentoPrincipale;    
end if;

if (maxVersionId = versionId) then
    -- 2a.) Il versionId si riferisce all'ultima versione del documento, è sempre visibile
    retValue := 1;
else

    -- 3) verifica se il documento è stato trasmesso a me o al mio ruolo e 
        -- se tale trasmissione prevede le versioni precedenti nascoste 
        -- NB: in presenza di più trasmissioni dello stesso documento,
        -- VINCE quella che nasconde le versioni, ovvero la più restrittiva
    select count(*) into hideVersions
    from dpa_trasmissione t
          inner join dpa_trasm_singola ts on t.system_id = ts.id_trasmissione
        where t.id_profile = idProfile
        and ts.id_corr_globale in 
            (
                (select system_id from dpa_corr_globali where id_people = idPeople), 
                (select system_id from dpa_corr_globali where id_gruppo = idGroup)
            )
        and ts.hide_doc_versions = '1';

    if (hideVersions > 0) then
        -- 4) verifica in sercurity se sul doc non dispongo dei diritti di ownership 
        -- (trasmissione a me stesso) oppure abbia già acquisito i diritti di visibilità
        -- (es. superiore gerarchico)
        select count(*) into ownership
        from security s 
        where thing = idProfile
                and personorgroup in (idPeople, idGroup)
                and (cha_tipo_diritto = 'P' or cha_tipo_diritto = 'A');
        
        
        if (ownership = 0) then
            -- Sul documento non si dispongono i diritti di ownership,
            -- pertanto la versione deve essere nascosta 
            retValue := 0;
        else
            -- Sul documento si dispongono già dei diritti di ownership,
            -- pertanto la versione non deve essere nascosta        
            retValue := 1;
        end if;
        
    else
        -- 3a) la tx non prevede di nascondere le versioni, quindi la versione è sempre visibile
        retValue := 1;    
    end if;
end if;

RETURN retValue;
EXCEPTION
WHEN OTHERS
THEN
return -1;

END isVersionVisible;
/



begin
    declare cnt int;
  begin
    select count(*) into cnt from @db_user.DPA_ANAGRAFICA_LOG l where l.var_codice ='MODIFICADOCSTATOFINALE';
    if (cnt = 0) then       
                insert into @db_user.dpa_anagrafica_log
                (
                system_id,
                var_codice,
                var_descrizione,
                var_oggetto,
                var_metodo
                )
                values 
                (
                seq.nextval,
                'MODIFICADOCSTATOFINALE',
                'Azione di modifica dei diritti di lettura / scrittura sul documento in stato finale',
                'DOCUMENTO',
                'MODIFICADOCSTATOFINALE'
                );
    end if;
  end;
end;
begin
    declare cnt int;
  begin
    select count(*) into cnt from user_tables where table_name='PROFILE';
    if (cnt != 0) then        
        select count(*) into cnt from cols where table_name='PROFILE' and column_name='CHA_UNLOCKED_FINAL_STATE';
      if (cnt = 0) then
            execute immediate 'ALTER TABLE PROFILE ADD CHA_UNLOCKED_FINAL_STATE varchar(1) NULL';
        end if;
    end if;
    end;
end;

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_FUNZIONI f where f.COD_FUNZIONE ='DO_AMM_CONSERVAZIONE_WA';
    if (cnt = 0) then        
       insert into DPA_ANAGRAFICA_FUNZIONI VALUES('DO_AMM_CONSERVAZIONE_WA', 'Permette l''amministrazione della WA di Conservazione', '', 'N');
    end if;
    end;
end;


begin
    declare cnt int;
    begin
        select count(*) into cnt from user_sequences where sequence_name='SEQ_DPA_GRIDS';
        if (cnt = 0) then
           execute immediate 'CREATE SEQUENCE SEQ_DPA_GRIDS START WITH 1 MAXVALUE 999999999999999999999999999 MINVALUE 1 NOCYCLE CACHE 20';
        end if;    
  
        select count(*) into cnt from user_tables where table_name='DPA_GRIDS';
        if (cnt = 0) then
          execute immediate    
            'CREATE TABLE DPA_GRIDS ' ||
            '( ' ||
            'SYSTEM_ID INT NOT NULL, ' ||
            'USER_ID INT, ' ||
            'ROLE_ID INT, ' ||
            'ADMINISTRATION_ID INT, ' ||
            'SEARCH_ID INT, ' ||
            'SERIALIZED_GRID CLOB, ' ||
            'TYPE_GRID VARCHAR2(30 BYTE), ' ||
            'CONSTRAINT PK_DPA_GRIDS PRIMARY KEY (SYSTEM_ID) ' ||    
            ')';
        end if;
    end;    
end;    
/  

begin
    declare cnt int;
  begin
    select count(*) into cnt from  DPA_ANAGRAFICA_FUNZIONI where Cod_funzione='GRID_PERSONALIZATION';
    if (cnt = 0) then
		begin
           execute immediate 'INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) VALUES (''GRID_PERSONALIZATION'', ''Abilita ruolo alla personalizzazione delle griglie di ricerca.'', NULL, ''N'')';
		end;
	end if;
	end;
end;
/

BEGIN

    DECLARE cnt int;

  BEGIN

    SELECT COUNT(*) INTO cnt FROM DPA_ANAGRAFICA_FUNZIONI WHERE COD_FUNZIONE='STAMPA_REG_NO_SEC';

    IF (cnt = 0) THEN        

       INSERT INTO DPA_ANAGRAFICA_FUNZIONI (COD_FUNZIONE, VAR_DESC_FUNZIONE, CHA_TIPO_FUNZ, DISABLED) 

        VALUES ('STAMPA_REG_NO_SEC' , 'Abilita utente a stampare registri senza controllo sulla sicurezza', NULL, 'N');

    END IF;

    END;

END;

/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOTIMESTAMP';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOTIMESTAMP',
	'Marca Temporale del documento',
	'DOCUMENTO',
	'DOCUMENTOTIMESTAMP'
	);
    end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOCONVERSIONEPDF';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOCONVERSIONEPDF',
	'Conversione in pdf del documento',
	'DOCUMENTO',
	'DOCUMENTOCONVERSIONEPDF'
	);
    end if;
  end;
end;
/

begin
    declare cnt int;
  begin
    select count(*) into cnt from DPA_ANAGRAFICA_LOG l where l.var_codice ='DOCUMENTOCONSERVAZIONE';
    if (cnt = 0) then       
	insert into dpa_anagrafica_log
	(
	system_id,
	var_codice,
	var_descrizione,
	var_oggetto,
	var_metodo
	)
	values 
	(
	seq.nextval,
	'DOCUMENTOCONSERVAZIONE',
	'Invio in conservazione del documento',
	'DOCUMENTO',
	'DOCUMENTOCONSERVAZIONE'
	);
    end if;
  end;
end;
/

begin
Insert into @db_user.DPA_DOCSPA
   (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values
   (seq.nextval, sysdate, '3.12.0');
end;
/

BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_SALVA_EMAIL_IN_LOCALE';
    IF (cnt = 0) THEN       
    
-- il record serve per ....  <---- inserire commento     
        insert into @db_user.DPA_CHIAVI_CONFIGURAZIONE 
   (system_id, 

ID_AMM,VAR_CODICE,VAR_DESCRIZIONE,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE,CHA_MODIFICABILE,CHA_GLOBALE)
         values 
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,'BE_SALVA_EMAIL_IN_LOCALE','Abilita il salvatggio delle mail in locale ( 1 attivo 

0 no)', '1','B','1','1','1');
    END IF;
    END;
END;
/
