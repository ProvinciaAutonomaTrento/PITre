
CREATE OR REPLACE FUNCTION getfunctCorcat(idruolo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT getcodtipofunz(id_tipo_funz) FROM dpa_tipo_f_ruolo WHERE id_ruolo_in_uo=idruolo;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

exception  WHEN OTHers then risultato:='';
end;

RETURN risultato;
end; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE                CercaStringa IS

ID_OGG_CUS DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO%TYPE;
id_temp DPA_ASSOCIAZIONE_TEMPLATES.SYSTEM_ID%TYPE;
stringa1 DPA_OGGETTI_CUSTOM.FORMATO_CONTATORE%TYPE;
stringa2 VARCHAR (255);
stringa3 VARCHAR(255);

cursor c_system_id is
SELECT DPA_TIPO_ATTO.SYSTEM_ID FROM DPA_TIPO_ATTO WHERE
DPA_TIPO_ATTO.VAR_DESC_ATTO ='Comunicato' OR DPA_TIPO_ATTO.VAR_DESC_ATTO='Circolare docenti';
BEGIN
stringa2 := '2013';
stringa3:='';
   OPEN c_system_id;
    LOOP
    
        FETCH c_system_id into id_temp;
            EXIT WHEN c_system_id%NOTFOUND;
                       
            SELECT DISTINCT ID_OGGETTO,FORMATO_CONTATORE INTO ID_OGG_CUS,stringa1 FROM DPA_ASSOCIAZIONE_TEMPLATES,DPA_OGGETTI_CUSTOM
            WHERE DPA_ASSOCIAZIONE_TEMPLATES.ID_OGGETTO=DPA_OGGETTI_CUSTOM.SYSTEM_ID
            AND DPA_ASSOCIAZIONE_TEMPLATES.ID_TEMPLATE = id_temp AND DPA_OGGETTI_CUSTOM.ID_TIPO_OGGETTO=59 ;
            dbms_output.put_line(ID_OGG_CUS);
            --UPDATE DPA_OGGETTI_CUSTOM SET DPA_OGGETTI_CUSTOM.FORMATO_CONTATORE='CONTATORE - a.s. ANNO'
            --WHERE DPA_OGGETTI_CUSTOM.SYSTEM_ID=ID_OGG_CUS;
            dbms_output.put_line(stringa1);
            if (instr( stringa1,'a.s.' )!=0)
            THEN
            dbms_output.put_line('trovato');
            stringa3 :=CONCAT('- a.s. ',stringa2);
            dbms_output.put_line(stringa3);
            else
            dbms_output.put_line('non trovato');
            END IF;
    
    
   
    END LOOP;
   
   CLOSE c_system_id;
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       RAISE;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END CercaStringa; 

/

SHOW ERRORS;

CREATE TABLE EMAILS
(
  IDELEMENTORUBRICA  NUMBER,
  EMAIL              VARCHAR2(200 BYTE),
  PREFERITA          INTEGER,
  NOTE               VARCHAR2(4000 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE HERMES_HUMMINGBIRD  -- NO
(
  ID_FK_HERMES    NUMBER                        NOT NULL,
  ID_HERMES       NUMBER,
  ID_HUMMINGBIRD  NUMBER,
  FILE_NAME       VARCHAR2(200 BYTE),
  DAT_CARI        DATE                          DEFAULT SYSDATE
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;
/*

CREATE TABLE MV_PROSPETTI_DOCCLASSCOMP_ALL
(
  ID_AMM             NUMBER,
  ID_REGISTR         NUMBER,
  ID_ANNO            NUMBER,
  SEDE               VARCHAR2(256 BYTE),
  TITOLARIO          NUMBER,
  TOT_DOC_CLASS      NUMBER,
  COD_CLASS          VARCHAR2(256 BYTE),
  DESC_CLASS         VARCHAR2(3000 BYTE),
  TOT_DOC_CLASS_VT   NUMBER,
  PERC_DOC_CLASS_VT  NUMBER,
  NUM_LIVELLO        VARCHAR2(256 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE TABLE MV_PROSPETTI_DOCCLASS_ALL
(
  ID_AMM             NUMBER,
  ID_REGISTR         NUMBER,
  ID_ANNO            NUMBER,
  SEDE               VARCHAR2(256 BYTE),
  TITOLARIO          NUMBER,
  TOT_DOC_CLASS      NUMBER,
  COD_CLASS          VARCHAR2(255 BYTE),
  DESC_CLASS         VARCHAR2(3000 BYTE),
  TOT_DOC_CLASS_VT   NUMBER,
  PERC_DOC_CLASS_VT  NUMBER,
  NUM_LIVELLO        NUMBER,
  VAR_COD_LIV1       VARCHAR2(32 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE MV_PROSPETTI_DOCGRIGI
(
  ID_AMM            NUMBER(10),
  ID_REGISTRO       NUMBER(10),
  CREATION_MONTH    DATE,
  VAR_SEDE          VARCHAR2(64 BYTE),
  FLAG_IMMAGINE     VARCHAR2(4000 BYTE),
  DISTINCT_COUNT    NUMBER,
  UNDISTINCT_COUNT  NUMBER
)
NOLOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE LANGUAGE
(
  SYSTEM_ID     INTEGER,
  CODE          VARCHAR2(200 BYTE),
  DESCRIPTION   VARCHAR2(200 BYTE),
  CULTURE_INFO  VARCHAR2(200 BYTE),
  ALIGN         VARCHAR2(100 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;





CREATE OR REPLACE TRIGGER track_RuoloResp_Registri --NO
After Update Of Id_Ruolo_Resp On Dpa_El_Registri 
Referencing Old As Old New As New

For Each Row
Declare 
PRAGMA AUTONOMOUS_TRANSACTION ;
ID INT; 
Begin
  Insert Into Utl_Track_Aooruoloresp ( Id_registro
  , Old_Aooruoloresp  
  , New_Aooruoloresp  
  , Cha_Changed_Notyet_Processed 
  , Dta_Changed ) 
Values ( :old.system_id
  ,:Old.Id_Ruolo_Resp 
  ,:New.Id_Ruolo_Resp 
  ,'y' -- will be changed to 'n' upon processing
  , Sysdate);
Commit; 
END;
/
SHOW ERRORS;

CREATE OR REPLACE TRIGGER TR_INSERT_EVENT_TRASM  --NO
AFTER INSERT
ON DPA_RAGIONE_TRASM
REFERENCING NEW AS NEW OLD AS OLD
FOR EACH ROW
BEGIN
   INSERT INTO DPA_ANAGRAFICA_LOG
        VALUES (
                  seq.NEXTVAL,
                  'TRASM_DOC_' || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                     'Effettuata trasmissione documento con ragione '
                  || :new.VAR_DESC_RAGIONE,
                  'DOCUMENTO',
                  'TRASM_DOC_' || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                  NULL,
                  :new.ID_AMM,
                  '1',
                  '0',
                  'TRANSMISSION_RECIPIENTS',
                  'BLUE'--,
                 -- '0',                                         --follow_config
                --  '0'                                                 --follow
                     );

   INSERT INTO DPA_ANAGRAFICA_LOG
        VALUES (
                  seq.NEXTVAL,
                     'TRASM_FOLDER_'
                  || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                     'Effettuata trasmissione fascicolo con ragione '
                  || :new.VAR_DESC_RAGIONE,
                  'FASCICOLO',
                     'TRASM_FOLDER_'
                  || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'),
                  NULL,
                  :new.ID_AMM,
                  '1',
                  '0',
                  'TRANSMISSION_RECIPIENTS',
                  'BLUE'--,
                --  '0',                                         --follow_config
                --  '0'                                                 --follow
                     );

   INSERT INTO dpa_log_attivati
      (SELECT l.system_id, :new.ID_AMM, 'OBB'
         FROM dpa_anagrafica_log l
        WHERE (    (   l.var_codice =
                             'TRASM_FOLDER_'
                          || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_')
                    OR var_codice =
                             'TRASM_DOC_'
                          || REPLACE (:new.VAR_DESC_RAGIONE, ' ', '_'))
               AND l.id_amm = :new.ID_AMM));
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
END;
/
SHOW ERRORS;

CREATE OR REPLACE TRIGGER TR_DELETE_EVENT_TRASM  --NO
   AFTER DELETE
   ON DPA_RAGIONE_TRASM    --****************************************************************************
   NAME:       TR_DELETE_EVENT_TRASM

   PURPOSE:      This keeps aligned trigger events of type transmission with the reasons of
                transmission in dpa_ragione_trasm

   REVISIONS:
   Ver        Date        Author
   ---------  ----------  ---------------
   1.0        05/09/2013   Camillo Ferlito

-- *****************************************************************************

   REFERENCING NEW AS New OLD AS Old
   FOR EACH ROW
BEGIN
   UPDATE dpa_log
      SET check_notify = '0'
    WHERE (    (   var_cod_azione =
                         'TRASM_DOC_'
                      || REPLACE (:old.var_desc_ragione, ' ', '_')
                OR var_cod_azione =
                         'TRASM_FOLDER_'
                      || REPLACE (:old.var_desc_ragione, ' ', '_'))
           AND id_amm = :old.id_amm);

   DELETE FROM dpa_log_attivati
         WHERE system_id_anagrafica IN
                  (SELECT system_id
                     FROM dpa_anagrafica_log
                    WHERE     (   var_codice =
                                        'TRASM_DOC_'
                                     || REPLACE (:old.var_desc_ragione,
                                                 ' ',
                                                 '_')
                               OR var_codice =
                                        'TRASM_FOLDER_'
                                     || REPLACE (:old.var_desc_ragione,
                                                 ' ',
                                                 '_'))
                          AND id_amm = :old.id_amm);

   DELETE DPA_ANAGRAFICA_LOG
    WHERE     var_codice =
                 'TRASM_DOC_' || REPLACE (:old.var_desc_ragione, ' ', '_')
          AND id_amm = :old.id_amm;

   DELETE DPA_ANAGRAFICA_LOG
    WHERE     var_codice =
                 'TRASM_FOLDER_' || REPLACE (:old.var_desc_ragione, ' ', '_')
          AND id_amm = :old.id_amm;
EXCEPTION
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
END;
/
SHOW ERRORS;

CREATE TABLE PGU_LOG --NO
(
  SYSTEM_ID         NUMBER                      NOT NULL,
  USERID_OPERATORE  VARCHAR2(32 BYTE),
  ID_AMM            NUMBER,
  DTA_AZIONE        DATE,
  VAR_OGGETTO       VARCHAR2(256 BYTE),
  VAR_DESC_OGGETTO  VARCHAR2(2000 BYTE),
  VAR_COD_AZIONE    VARCHAR2(256 BYTE),
  CHA_ESITO         CHAR(1 BYTE),
  VAR_DESC_AZIONE   VARCHAR2(2000 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_TODOLIST_MIGRATION  --NO
(
  ID_TRASMISSIONE   NUMBER(10)                  NOT NULL,
  ID_TRASM_SINGOLA  NUMBER(10)                  NOT NULL,
  ID_TRASM_UTENTE   NUMBER(10)                  NOT NULL,
  ID_PEOPLE_DEST    NUMBER(10),
  ID_RUOLO_DEST     NUMBER(10)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE OR REPLACE FORCE VIEW DPA_V_LOGS
(UTENTE, RUOLO, RF, TOT_AZIONE, TIPO_AZIONE)
AS 
select UTENTE,RUOLO,RF,count(*)TOT_AZIONE,TIPO_AZIONE from (
select 
l.USERID_OPERATORE USER_ID,
getpeoplename(l.ID_PEOPLE_OPERATORE) UTENTE,
l.ID_GRUPPO_OPERATORE+1 ID_RUOLO,
getdescruolo(l.ID_GRUPPO_OPERATORE) RUOLO,
CODRFAPPARTENZARUOLO(l.ID_GRUPPO_OPERATORE+1) RF,
l.VAR_COD_AZIONE CODICE_TIPO_AZIONE,
l.VAR_DESC_AZIONE TIPO_AZIONE,
l.DTA_AZIONE DATA_AZIONE
 from dpa_log l where
l.DTA_AZIONE between 
to_date('01/03/2010 00:00:00','dd/mm/yyyy HH24:mi:ss') 
and to_date('01/05/2010 23:59:59','dd/mm/yyyy HH24:mi:ss')
and l.VAR_COD_AZIONE not in('LOGIN','LOGOFF')
--
union
select 
l.USERID_OPERATORE USER_ID,
getpeoplename(l.ID_PEOPLE_OPERATORE) UTENTE,
l.ID_GRUPPO_OPERATORE+1 ID_RUOLO,
getdescruolo(l.ID_GRUPPO_OPERATORE) RUOLO,
CODRFAPPARTENZARUOLO(l.ID_GRUPPO_OPERATORE+1) RF,
l.VAR_COD_AZIONE CODICE_TIPO_AZIONE,
l.VAR_DESC_AZIONE TIPO_AZIONE,
l.DTA_AZIONE DATA_AZIONE
 from dpa_log_storico l where
l.DTA_AZIONE between 
to_date('01/03/2010 00:00:00','dd/mm/yyyy HH24:mi:ss') 
and to_date('01/05/2010 23:59:59','dd/mm/yyyy HH24:mi:ss')
and l.VAR_COD_AZIONE not in('LOGIN','LOGOFF')
--and  CODRFAPPARTENZARUOLO(l.ID_GRUPPO_OPERATORE+1)='RFS112'
)
group by UTENTE,RUOLO,RF,TIPO_AZIONE
order by UTENTE,RUOLO,RF,TIPO_AZIONE;

CREATE OR REPLACE FORCE VIEW VS01_DOC_CREATI_DA_UT_DELL_RF
(COD_STRUT, TOT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',
   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut, count(*) tot
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' '
group by  (codrfappartenzaRuolo(p.id_ruolo_creatore));

CREATE OR REPLACE FORCE VIEW VS02_UT_NELL_RF
(COD_STRUT, TOT)
AS 
SELECT a.var_codice cod_strut,count(*) tot
  FROM people p,
       peoplegroups pg,
       dpa_l_ruolo_reg t,
       dpa_el_registri a,
       dpa_corr_globali cg
 WHERE t.id_registro = a.system_id
   --AND a.system_id = 160201
   AND a.cha_rf = '1'
   AND cg.system_id = t.id_ruolo_in_uo
   AND cg.id_gruppo = pg.groups_system_id
   and pg.PEOPLE_SYSTEM_ID=p.system_id
 and pg.DTA_FINE is null
   and cg.dta_fine is null     
   group by a.var_codice
   order by a.var_codice;

CREATE OR REPLACE FORCE VIEW VS04_BIS_ANNULLATI
(COD_STRUT, ANNULLATO, TOT, PERCENT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(decode(dta_annulla,null,' ','SI')),1,'TOTALE DOC ',decode(dta_annulla,null,' ','SI')) ANNULLATO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),decode(dta_annulla,null,' ','SI'))) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and p.ID_DOCUMENTO_PRINCIPALE is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),decode(dta_annulla,null,' ','SI'))
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS04_DOC_IN_OUT_INT_GRI
(COD_STRUT, TIPO, TOT, PERCENT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and p.id_documento_principale is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS05_PERC_DOC_ACQUISITI
(COD_STRUT, TIPO, TOT, PERCENT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' and p.ID_DOCUMENTO_PRINCIPALE is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS13_NUM_MEDIO_DOC_IN_FASC
(COD_STRUT, MEDIA_NUM_DOC_PER_FASC)
AS 
select   cod_strut, avg(num_doc_per_fasc) media_num_doc_per_fasc

from(

SELECT  getcodtit(p1.id_parent), DECODE (GROUPING (codrfappartenza (s.personorgroup)),
                 1, 'TOTALE DOC',
                 codrfappartenza (s.personorgroup)
                ) cod_strut,
         COUNT (LINK) num_doc_per_fasc
    FROM project p1, project_components pg,security s
   WHERE EXISTS (
            SELECT 'x'
              FROM project p, security s
             WHERE p.cha_tipo_fascicolo = 'P'
               AND s.thing = p.system_id
               AND codrfappartenza (s.personorgroup) != ' '
               AND s.accessrights = 0
               AND p1.id_fascicolo = p.system_id)
     AND p1.cha_tipo_proj = 'C'
     AND pg.project_id = p1.system_id
     and s.thing=p1.system_id and s.accessrights=0
GROUP BY  (codrfappartenza (s.personorgroup),getcodtit(p1.id_parent))
)
group by cod_strut
order by cod_strut;

CREATE OR REPLACE FORCE VIEW VS06_DOC_SENZA_ALLEGATI
(COD_STRUT, TIPO, TOT, PERCENT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' and p.ID_DOCUMENTO_PRINCIPALE is null and noAllegati(p.SYSTEM_ID)=0
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS07_NUM_DOC_CON_ALLEGATI
(COD_STRUT, TIPO_DOC, TOTALE_DOC, PERCENTUALE_DOC)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' and p.ID_DOCUMENTO_PRINCIPALE is null and noAllegati(p.SYSTEM_ID)=1
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS08_NUM_DOC_CON_CORR_OCCAS
(COD_STRUT, TIPO_DOC, TOTALE_DOC, PERCENTUALE_DOC)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' 
    and p.cha_tipo_proto not in ('G','R')
    and p.ID_DOCUMENTO_PRINCIPALE is null and getSeSoloMittDestOcc(p.system_id)=1
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) , tipo;

CREATE OR REPLACE FORCE VIEW VS09_NUM_VISUALIZ_FILES
(COD_STRUT, TOT_VISUALIZ, TIPO_DOC)
AS 
select DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,COUNT(G.SYSTEM_ID) TOT_VISUALIZ,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO
    from  DPA_LOG G,profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and  VAR_COD_AZIONE='GET_FILE' and g.id_oggetto=p.system_id 
   and (ISALLEGATO(P.SYSTEM_ID)='1' OR ISPROTOCOLLO(P.SYSTEM_ID)='1')
group by ROLLUP  (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW V_VISIBILITA_PROT
(SEGNATURA_ID, THING, PERSONORGROUP, RUOLO, COD__RUBR_RUOLO, 
 USER_ID, PEOPLE, DIRITTO, TIPO_DIRITTO, ID_GRUPPO_TRASM, 
 NUM_PROTO, ID_REGISTRO, NUM_ANNO_PROTO)
AS 
SELECT p.docname segnatura_id, s.thing thing, s.personorgroup personorgroup,
       getpeoplename (s.personorgroup) ruolo,getcodRubricaRuolo(s.personorgroup) cod__rubr_ruolo,GETPEOPLEuserid(s.personorgroup) USER_ID  ,
       getdescruolo (s.personorgroup) people, s.accessrights diritto,
       s.cha_tipo_diritto tipo_diritto, s.id_gruppo_trasm,
       p.num_proto num_proto, p.id_registro id_registro,
       p.num_anno_proto num_anno_proto
  FROM security s, PROFILE p
 WHERE s.thing = p.system_id AND p.num_proto > 0;

CREATE OR REPLACE TRIGGER UTL_AFTER_ALTER_ON_SCHEMA After Alter On Schema
Declare
   PRAGMA AUTONOMOUS_TRANSACTION;
Msg Varchar2(20000) := 'message from trigger UTL_AFTER_ALTER_ON_SCHEMA. 
      The time is: ';
v_Subject Varchar2(2000) ; 
Nomeutente       Varchar2(32);
Data_Eseguito    Date;
Comando_Eseguito Varchar2(2000);
Versione_Cd      Varchar2(200);
esito            VARCHAR2(200);
Myobj_Status     Varchar2(200);
V_Current_Docspa_Version Varchar2(20);

Sql_Text  Ora_Name_List_T;
Stmt      Varchar2(2000);
n   int;
Flag_Changed_First_Time Int := 1; -- I assume I have to raise an alert every time, see below
Cnt Int :=0;
linedebug int := 0;

Cursor C_Plsql (Nome_Plsql Varchar2) Is 
Select Name as nome, Text As Testo , to_clob(Text) As Testo_clob 
From User_Source Where Upper(Name)=Upper(Nome_Plsql) ; 
--V_Clob Clob := 'CREATE OR REPLACE ';
L_Clob Clob := ' ';
Begin
Msg := Msg||To_Char(Sysdate, 'Day DD MON, YYYY HH24:MI:SS')||'
  ';
linedebug :=31;
n := ora_sql_txt(sql_text);
FOR i IN 1..n LOOP
 stmt := stmt || sql_text(i);
End Loop;

Comando_Eseguito := nvl('text of triggering statement: ' || Stmt ,'n.d.');
Select Username Into Nomeutente  From User_Users;
linedebug :=39;

If Ora_Dict_Obj_Type = 'TRIGGER' Then 
  select status into myobj_status
       From User_TRIGGERs Where TRIGGER_Name = Ora_Dict_Obj_Name;
  else 
  Select Status Into Myobj_Status
       From User_Objects Where Object_Name = Ora_Dict_Obj_Name and Object_Type=Ora_Dict_Obj_Type;
End If;
linedebug :=48;
If Ora_Dict_Obj_Type In ('PROCEDURE', 'FUNCTION', 'TRIGGER', 'PACKAGE', 'PACKAGE BODY' ) Then 

-- can I lower the flag for a given procedure frequently changing?
    Select Count(*) Into Cnt From Utl_Track_Plsql_Change_ Where Var_Dict_Obj_Name = Ora_Dict_Obj_Name;
    If Cnt = 0 Then 
linedebug :=54;
    Begin  -- try to get current value of Docspa Version 
        Select id_versions_u  into V_Current_Docspa_Version From (
          Select id_versions_u 
          From Dpa_Docspa
          Order By Dta_Update Desc Nulls Last)
        Where Rownum <2;
    Exception When Others Then V_Current_Docspa_Version := 'n.d.'; 
    end;
    
    Insert Into Utl_Track_Plsql_Change_ 
        (Var_Dict_Obj_Type, Var_Dict_Obj_Name, Dta_Of_Change
        , Var_Current_Docspa_Version, CLOB_PREVIOUS_VERSION_OF_CODE )          
      Values 
        (Ora_Dict_Obj_Type, Ora_Dict_Obj_Name, Sysdate      
        , V_Current_Docspa_Version, Empty_Clob() ) 
             Returning Clob_Previous_Version_Of_Code Into L_Clob         ;
       linedebug :=71;
       For Plsql In C_Plsql (Ora_Dict_Obj_Name) Loop
           Dbms_Lob.Writeappend (L_Clob, Length( Plsql.Testo_Clob),  Plsql.Testo_Clob );
          
          -- alternative way
          -- Dbms_Lob.Append (V_Clob,  (Plsql.Testo) || ' ');
       End Loop;
       
       
       
linedebug :=79;
      Flag_Changed_First_Time := 1 ; -- enforce the flag, plsql code is changing for the first time, I mean since last release
    Else 
      Utl_Write_To_Appcatalog (Ora_Dict_Obj_Type, Ora_Dict_Obj_Name) ; 
      
      Flag_Changed_First_Time := 0 ; -- can lower the flag, code has already changed and was already tracked in utl_ table
    End If;
    Commit;
end if; 



If  Flag_Changed_First_Time = 1 And Ora_Dict_Obj_Name Not Like 'UTL%'  Then 
linedebug :=89;
Utl_Insert_Log (Nomeutente,Sysdate
, substr(Comando_Eseguito,1,200)        , 'n.d.' --Versione_Cd Varchar2
, 'ok' ); --esito VARCHAR2

 V_Subject     := Ora_Dict_Obj_Type||' '||Ora_Dict_Obj_Name||' altered !';

linedebug :=96;
Utl_Mail.Send (
    Sender      => 'sviluppo_pitre@vtdocs11g.com',
    Recipients  => 'leonardo.lorusso@nttdata.com',
    Subject     => V_Subject,
    Message     => Msg||'
       from client_ip :'||NVL(ora_client_ip_address, 'N/A')||' -- 
       '
       ||' sysevent is      : '||Nvl(Ora_Sysevent,'N/A')
       ||' 
       -- 
       
       '||Comando_Eseguito);
End If;

Exception When Others Then 
  Utl_Insert_Log (Nomeutente,sysDate
      , Comando_Eseguito        , 'n.d.' --Versione_Cd Varchar2
      , 'linedebug '||linedebug||'- ko per '||SQLERRM ); --esito VARCHAR2

    raise; 
end;
/
SHOW ERRORS;


ALTER TABLE PGU_ENTI --NO 
 ADD (URLWSPITRE  VARCHAR2(2000 BYTE));

ALTER TABLE PGU_UTENTI  --NO
 ADD (EMAIL  VARCHAR2(255 BYTE));

ALTER TABLE PGU_ENTI_UTENTI
 ADD (CHA_ABILITATO_CONFIGURAZIONE  VARCHAR2(1 CHAR) DEFAULT 0);



CREATE OR REPLACE TRIGGER Utl_Before_Create_On_Schema Before Create On Schema
DISABLE
begin
RAISE_APPLICATION_ERROR (
         num => -20000,
         Msg => 'Cannot create object due to '||Ora_Dict_Obj_Name||' trigger; ask your DBAs');
End;
/
SHOW ERRORS;



CREATE OR REPLACE FORCE VIEW VS10_NUM_DOC_NON_TRASMES
(COD_STRUT, TIPO_DOC, NUMERO_TOT, PERCETUALE_TOT)
AS 
select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getsedoctrasmconrag(p.system_id,'TUTTE')=0
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore);

CREATE OR REPLACE FORCE VIEW VS11A_GIACENZA_MEDIA_CON_WF
(COD_STRUT, MAX_ACC_DIFF, MIN_ACC_DIFF, MEDIA_ACC_DIFF, MAX_RIF_DIFF, 
 MIN_RIF_DIFF, MEDIA_RIF_DIFF)
AS 
SELECT   cod_strut, NUMTODSINTERVAL (MAX (acc_diff), 'DAY') max_acc_diff,
         NUMTODSINTERVAL (MIN (acc_diff), 'DAY') min_acc_diff,
         NUMTODSINTERVAL (AVG (acc_diff), 'DAY') media_acc_diff,
         NUMTODSINTERVAL (MAX (rif_diff), 'DAY') max_rif_diff,
         NUMTODSINTERVAL (MIN (rif_diff), 'DAY') min_rif_diff,
         NUMTODSINTERVAL (AVG (rif_diff), 'DAY') media_rif_diff
    FROM (SELECT codrfappartenzaruolo (p.id_ruolo_creatore) cod_strut,
                 
                     -- count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER
                 -- (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore))) * 100 ,2), '999.99') || '%',20,' ') PERCENT  ,
                 -- dta_accettata,dta_rifiutata,dta_vista,dta_invio,vt.CHA_TIPO_ragione,
                 dta_accettata - dta_invio acc_diff,
                 dta_rifiutata - dta_invio rif_diff
            FROM PROFILE p, v_trasmissione vt
           WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
             AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
             AND vt.id_profile = p.system_id
             AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
             AND vt.cha_in_todolist = '0'
             AND (dta_accettata IS NOT NULL OR dta_rifiutata IS NOT NULL)
             AND vt.cha_tipo_ragione = 'W')
GROUP BY cod_strut;

CREATE OR REPLACE FORCE VIEW VS11B_GIACENZA_MEDIA_SENZA_WF
(COD_STRUT, MAX_VISTA_DIFF, MIN_VISTA_DIFF, MEDIA_VISTA_DIFF)
AS 
SELECT   cod_strut, NUMTODSINTERVAL (MAX (vista_diff), 'DAY') max_vista_diff,
         NUMTODSINTERVAL (MIN (vista_diff), 'DAY') min_vista_diff,
         NUMTODSINTERVAL (AVG (vista_diff), 'DAY') media_vista_diff
            FROM (SELECT codrfappartenzaruolo (p.id_ruolo_creatore) cod_strut,                       
                     -- count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER
                 -- (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore))) * 100 ,2), '999.99') || '%',20,' ') PERCENT  ,
                 -- dta_accettata,dta_rifiutata,dta_vista,dta_invio,vt.CHA_TIPO_ragione,
                                 dta_vista - dta_invio vista_diff
            FROM PROFILE p, v_trasmissione vt
           WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
             AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
             AND vt.id_profile = p.system_id
             AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
             AND vt.cha_in_todolist = '0'
             AND (dta_vista IS NOT NULL )
             AND vt.cha_tipo_ragione = 'N')
GROUP BY cod_strut;

CREATE OR REPLACE FORCE VIEW VS12_N_TRASM_OLDER_THAN_30
(TOT, COD_STRUT)
AS 
select cod_strut,count(*) as tot from 
(SELECT codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) cod_strut,
                    dta_accettata - dta_invio acc_diff,
                    dta_rifiutata - dta_invio rif_diff
               FROM PROFILE p, v_trasmissione vt
              WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
                AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
                AND vt.id_profile = p.system_id
                AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
                AND vt.cha_in_todolist = '0'
                AND (dta_accettata IS NOT NULL OR dta_rifiutata IS NOT NULL)
                AND vt.cha_tipo_ragione = 'W'  
                and ( NUMTODSINTERVAL(dta_accettata - dta_invio ,'DAY')>numtoDSinterval(30,'DAY')  or
                NUMTODSINTERVAL(dta_rifiutata - dta_invio,'DAY')>numtoDSinterval(30,'DAY'))   
                )
                 group by cod_strut  
                 order by cod_strut;



CREATE OR REPLACE PROCEDURE                CREATE_ASSERTIONS_EVENTS
IS
   idTypeEvent     NUMBER (10, 0);
   descTypeEvent   VARCHAR2 (128 CHAR);
BEGIN
   -- Creo un asserzione per l'evento di conversione pdf a livello di amministrazione
   -- dove specifico che le notifiche associate a questo evento devono giungere a tutti i
   -- potenziali ruoli destinatari presenti in amministrazione come informative
   SELECT system_id, var_descrizione
     INTO idTypeEvent, descTypeEvent
     FROM dpa_anagrafica_log
    WHERE var_codice = 'DOCUMENTOCONVERSIONEPDF';

   INSERT INTO dpa_event_type_assertions
      (SELECT seq.NEXTVAL,
              idTypeEvent,
              descTypeEvent,
              a.system_id,
              a.var_codice_amm,
              'AMM',
              'I',
              '1',
              a.system_id
         FROM dpa_amministra a);
END;

/

SHOW ERRORS;


*/

/*
CREATE OR REPLACE TRIGGER UTL_AFTER_CREATE_ON_SCHEMA After CREATE On Schema
Declare
Msg Varchar2(20000) := 'message from trigger UTL_AFTER_CREATE_ON_SCHEMA. The time is: ';
v_Subject Varchar2(2000) ; 
Nomeutente      Varchar2(32);
Data_Eseguito   Date;
Comando_Eseguito Varchar2(2000);
Versione_Cd     Varchar2(200);
Esito           Varchar2(200);
V_Current_Docspa_Version Varchar2(20);
Flag_Changed_First_Time Int := 1; -- I assume I have to raise an alert every time, see below
cnt int :=0;
Begin

If Ora_Dict_Obj_Type In ('PROCEDURE', 'FUNCTION', 'TRIGGER', 'PACKAGE' , 'PACKAGE BODY') Then 
-- isolate this in autonomous call!

-- can I lower the flag for a given procedure frequently changing?
    Select Count(*) Into Cnt From Utl_Track_Plsql_Change_ Where VAR_Dict_Obj_Name = Ora_Dict_Obj_Name;
    If Cnt = 0 Then 
    
    Begin  -- try to get current value of Docspa Version 
        Select id_versions_u  into V_Current_Docspa_Version From (
          Select id_versions_u 
          From Dpa_Docspa
          Order By Dta_Update Desc Nulls Last)
        Where Rownum <2;
    Exception When Others Then V_Current_Docspa_Version := 'n.d.'; 
    end;
    
     Insert Into Utl_Track_Plsql_Change_ 
        (Var_Dict_Obj_Type, Var_Dict_Obj_Name, Dta_Of_Change
        , Var_Current_Docspa_Version )          
      Values 
        (Ora_Dict_Obj_Type, Ora_Dict_Obj_Name, Sysdate      
        , V_Current_Docspa_Version ) ;
        
        
      --commit;
      Flag_Changed_First_Time := 1 ; -- enforce the flag, plsql code is changing for the first time, I mean since last release
    Else 
      Flag_Changed_First_Time := 0 ; -- can lower the flag, code has already changed and was already tracked in utl_ table
    end If;
end if; 

if  Flag_Changed_First_Time = 1 and Ora_Dict_Obj_Name not like 'UTL%'  then 

      Msg := Msg||To_Char(Sysdate, 'Day DD MON, YYYY HH24:MI:SS');
      Comando_Eseguito := 'Created '||Ora_Dict_Obj_Type||' '||Ora_Dict_Obj_Name||' ON Database:'||ora_database_name ;
      select username into Nomeutente  from user_users;
      
      Utl_Insert_Log (Nomeutente,sysDate
      , Comando_Eseguito        , 'n.d.' --Versione_Cd Varchar2
      , 'ok' ); --esito VARCHAR2
      
       v_Subject     := Ora_Dict_Obj_Type||' '||Ora_Dict_Obj_Name||' created !';
      
        Utl_Mail.Send (
            Sender      => 'sviluppo_pitre@vtdocs11g.com',
            Recipients  => 'leonardo.lorusso@nttdata.com',
            Subject     => v_Subject,
            Message     => Msg||'
          
          '||Comando_Eseguito);
end if; 
end;
/
SHOW ERRORS;
*/

/*

*/

CREATE SEQUENCE SEQ_HERMES_HUMMINGBIRD
  START WITH 35
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  NOCACHE
  NOORDER;
CREATE TABLE DPA_DATI_FATTURAZIONE  --OK
(
  SYSTEM_ID                 INTEGER             NOT NULL,
  CODICE_AMM                VARCHAR2(128 BYTE)  NOT NULL,
  CODICE_AOO                VARCHAR2(20 BYTE)   NOT NULL,
  CODICE_UO                 VARCHAR2(128 BYTE)  NOT NULL,
  CODICE_UAC                VARCHAR2(128 BYTE),
  CODICE_CLASSIFICAZIONE    VARCHAR2(20 BYTE),
  VAR_UTENTE_PROPRIETARIO   VARCHAR2(128 BYTE),
  VAR_TIPOLOGIA_DOCUMENTO   VARCHAR2(128 BYTE),
  VAR_RAGIONE_TRASMISSIONE  VARCHAR2(128 BYTE),
  ISTANZA_PITRE             VARCHAR2(512 BYTE),
  MODELLO_TRASMISSIONE      VARCHAR2(128 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


ALTER TABLE DPA_SUPPORTO
 ADD (PERC_VERIFICA_LEGG  NUMBER);

ALTER TABLE DPA_SUPPORTO
 ADD (DATA_ULTIMA_VERIFICA_LEGG  DATE);

ALTER TABLE DPA_SUPPORTO
 ADD (ESITO_ULTIMA_VERIFICA_LEGG  NUMBER);

ALTER TABLE DPA_SUPPORTO
 ADD (VERIFICHE_LEGG_EFFETTUATE  NUMBER);

ALTER TABLE DPA_SUPPORTO
 ADD (DATA_PROX_VERIFICA_LEGG  DATE);

ALTER TABLE DPA_TIPO_ATTO
 ADD (PATH_XSD_ASSOCIATO  VARCHAR2(255 BYTE));

ALTER TABLE DPA_TIPO_ATTO
 ADD (IS_TYPE_INSTANCE  CHAR(1 CHAR)                DEFAULT 0);

CREATE TABLE ELEMENTIRUBRICA  --NO  rubrica comune 
(
  ID                  INTEGER                   NOT NULL,
  CODICE              NVARCHAR2(50)             NOT NULL,
  DESCRIZIONE         NVARCHAR2(255)            NOT NULL,
  DATACREAZIONE       DATE                      NOT NULL,
  DATAULTIMAMODIFICA  DATE                      NOT NULL,
  IDUTENTECREATORE    INTEGER                   NOT NULL,
  INDIRIZZO           NVARCHAR2(255),
  TELEFONO            NVARCHAR2(20),
  FAX                 NVARCHAR2(20),
  CITTA               NVARCHAR2(50),
  CAP                 NVARCHAR2(5),
  PROVINCIA           NVARCHAR2(2),
  NAZIONE             NVARCHAR2(50),
  AOO                 NVARCHAR2(255),
  TIPOCORRISPONDENTE  VARCHAR2(5 BYTE),
  IDAMMINISTRAZIONE   NUMBER,
  CHA_PUBBLICATO      NVARCHAR2(1),
  VAR_COD_PI          VARCHAR2(11 CHAR),
  VAR_COD_FISC        VARCHAR2(16 CHAR),
  AMMINISTRAZIONE     VARCHAR2(64 BYTE),
  URL                 VARCHAR2(128 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;


CREATE TABLE DPA_AREA_ESIBIZIONE 
(
  SYSTEM_ID                  NUMBER             NOT NULL,
  ID_AMM                     NUMBER,
  ID_PEOPLE                  NUMBER,
  ID_RUOLO_IN_UO             NUMBER,
  CHA_STATO                  CHAR(1 BYTE),
  VAR_NOTE                   VARCHAR2(500 BYTE),
  VAR_DESCRIZIONE            VARCHAR2(250 BYTE),
  DATA_CREAZIONE             DATE,
  DATA_CERTIFICAZIONE        DATE,
  DATA_CHIUSURA              DATE,
  DATA_RIFIUTO               DATE,
  VAR_NOTE_RIFIUTO           VARCHAR2(256 BYTE),
  ID_PEOPLE_CERTIFICATORE    NUMBER,
  VAR_MARCA_TEMPORALE        VARCHAR2(3000 BYTE),
  VAR_FIRMA_RESPONSABILE     VARCHAR2(1024 BYTE),
  CHA_CERTIFICAZIONE         CHAR(1 BYTE),
  VAR_FILE_CHIUSURA          CLOB,
  VAR_FILE_CHIUSURA_FIRMATO  CLOB,
  ID_PROFILE_CERTIFICAZIONE  NUMBER
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_ITEMS_ESIBIZIONE
(
  SYSTEM_ID         NUMBER                      NOT NULL,
  ID_ESIBIZIONE     NUMBER,
  ID_PROFILE        NUMBER,
  ID_PROJECT        NUMBER,
  CHA_TIPO_DOC      CHAR(1 BYTE),
  VAR_OGGETTO       VARCHAR2(2000 BYTE),
  ID_REGISTRO       NUMBER,
  DATA_INS          DATE,
  CHA_STATO         CHAR(1 BYTE),
  VAR_XML_METADATI  CLOB,
  SIZE_ITEM         NUMBER,
  COD_FASC          VARCHAR2(64 BYTE),
  DOCNUMBER         NUMBER,
  CHA_TIPO_OGGETTO  CHAR(1 CHAR),
  VAR_TIPO_FILE     VARCHAR2(32 BYTE),
  NUMERO_ALLEGATI   NUMBER,
  CHA_ESITO         CHAR(1 BYTE),
  VAR_TIPO_ATTO     VARCHAR2(64 BYTE),
  ID_CONSERVAZIONE  NUMBER
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_EXTERNAL_SYSTEMS
(
  SYSTEM_ID                NUMBER               NOT NULL,
  VAR_CODE_APPLICATION     VARCHAR2(32 BYTE)    NOT NULL,
  VAR_DESCRIZIONE          VARCHAR2(512 BYTE),
  VAR_USER_ID              VARCHAR2(255 BYTE)   NOT NULL,
  VAR_PIS_METHODS_ALLOWED  VARCHAR2(3000 BYTE),
  ID_SYSTEM_ROLE           NUMBER               NOT NULL,
  ID_AMM                   NUMBER,
  VAR_DESC_ESTESA          VARCHAR2(1000 BYTE),
  VAR_TKN_TIME             VARCHAR2(20 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_ESITO_VERIFICA_CONS
(
  SYSTEM_ID  NUMBER                             NOT NULL,
  VALORE     VARCHAR2(200 BYTE)                 NOT NULL
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

ALTER TABLE DPA_OGG_CUSTOM_COMP
 ADD (OPZIONI_XML_ASSOC  VARCHAR2(2000 BYTE));

CREATE OR REPLACE FUNCTION             geNumDocProtInFascRF(idfasc number,codrf varchar ) RETURN int IS
tmpVar int;
BEGIN
begin
/* Formatted on 2010/05/25 14:37 (Formatter Plus v4.8.8) */
SELECT COUNT (LINK)
  INTO tmpvar
  FROM project_components pc, PROFILE p
 WHERE p.system_id = pc.LINK
   AND p.num_proto IS NOT NULL
   AND p.cha_tipo_proto IN ('A', 'P', 'I')
   AND pc.project_id IN (SELECT system_id
                           FROM project
                          WHERE id_fascicolo = idfasc)
   and codrfappartenzaruolo(p.ID_RUOLO_PROT) =codrf;                      
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocProtInFascRF; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          GetCODamm(IDAMM NUMBER)
RETURN VARCHAR IS risultato VARCHAR(16);
BEGIN
BEGIN

SELECT VAR_CODICE_AMM INTO risultato FROM DPA_AMMINISTRA WHERE SYSTEM_ID = IDAMM;

RETURN risultato;
END;
END GetCODamm; 

/

SHOW ERRORS;

CREATE OR REPLACE Function Getcodicerfbyprofileid (System_Id_Profile Int)
Return Varchar Is Codicerf varchar2(512);
Begin
Select -- G.Group_Id As Codiceruolo, Cg.Var_Desc_Corr DescRuolo, 
Er.Var_Codice  into Codicerf
From Dpa_El_Registri Er, Dpa_L_Ruolo_Reg Err, Dpa_Corr_Globali Cg, groups g, profile p
Where Cha_Rf='1' And Err.Id_Registro = Er.System_Id 
And Cg.System_Id = Err.Id_Ruolo_In_Uo
And Cg.Id_Gruppo = G.System_Id
And P.Id_Ruolo_Creatore = Cg.System_Id
And Var_Segnatura Is Not Null
And Instr(Var_Segnatura,Er.Var_Codice) > 0
And P.System_Id = System_Id_Profile ;
Return Codicerf;
exception when others then Codicerf := '';return Codicerf; 
end; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          getcodregCorcat(idruolo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT getcodreg(id_registro) FROM dpa_l_ruolo_reg WHERE id_ruolo_in_uo=idruolo;

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

exception  WHEN OTHers then risultato:='';
end;

RETURN risultato;
end; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getidruoloRif (idruoloprot number,sysid number)
RETURN number IS risultato number;

BEGIN
if(idruoloprot is null)
then risultato:=0;
else
begin
risultato := 0;
select id_gruppo into risultato from dpa_corr_globali where id_uo in (
select d.ID_MITT_DEST from profile p,dpa_doc_arrivo_par d where d.ID_PROFILE=sysid and cha_tipo_proto='P' 
and p.ID_RUOLO_PROT =idruoloprot and 
  d.CHA_TIPO_MITT_DEST='M') and cha_riferimento ='1';
  exception when others then risultato:=0;
end;
end if;
RETURN risultato;

END getidruolorif; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION                getInEsibizione (IDPROFILE number, Idproject number, typeID char, idPeople number, idGruppo number, idConservazione number)

RETURN INT IS risultato INT;
res_appo INT;
idRuoloInUo NUMBER;
BEGIN
    
    begin

        SELECT DPA_CORR_GLOBALI.SYSTEM_ID INTO idRuoloInUo 
        FROM DPA_CORR_GLOBALI 
        WHERE DPA_CORR_GLOBALI.ID_GRUPPO = idGruppo;

        -- Documenti
        IF (typeID = 'D' AND Idproject is null ) 
        THEN
            SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
            FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
            WHERE
            DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
            AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
            AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N' 
            AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
            AND    DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo  
            AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT IS  NULL
            --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
            ;
        ELSE
            IF (typeID = 'D' AND Idproject is NOT null ) 
            THEN
                SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
                FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
                WHERE DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
                AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
                AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N' 
                AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
                AND DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo  
                AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT =Idproject
                --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
                ;
            END IF;
        END IF;
    
        -- Fascicoli
        IF (typeID = 'F') 
        THEN
            SELECT COUNT(DPA_ITEMS_ESIBIZIONE.SYSTEM_ID) INTO risultato 
            FROM DPA_AREA_ESIBIZIONE, DPA_ITEMS_ESIBIZIONE 
            WHERE DPA_ITEMS_ESIBIZIONE.ID_ESIBIZIONE=DPA_AREA_ESIBIZIONE.SYSTEM_ID 
            AND DPA_ITEMS_ESIBIZIONE.ID_PROJECT = Idproject
            AND DPA_ITEMS_ESIBIZIONE.ID_PROFILE = IDPROFILE
            AND DPA_AREA_ESIBIZIONE.CHA_STATO ='N'
            AND DPA_AREA_ESIBIZIONE.ID_PEOPLE=idPeople 
            AND DPA_AREA_ESIBIZIONE.ID_RUOLO_IN_UO = idRuoloInUo
            --AND DPA_ITEMS_ESIBIZIONE.ID_CONSERVAZIONE = idConservazione
            ;
        END IF;

        IF (risultato > 0) THEN
        risultato := 1;
        ELSE
        risultato:=0;
        END IF;

        EXCEPTION
        WHEN NO_DATA_FOUND 
            THEN risultato := 0;
        WHEN OTHERS 
            THEN risultato := 0;
    end;
    
    RETURN risultato;

END getInEsibizione; 

/

SHOW ERRORS;

CREATE OR REPLACE Function          Corrcat_Det (Docid Int, Tipo_Proto Varchar)  
RETURN  varchar DETERMINISTIC IS risultato clob;

item clob;
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
risultato := risultato||'; '|| item || '(D) ';
ELSE
risultato := risultato||item;
END IF;
END IF;


END;
END IF;

END LOOP;

RETURN risultato;

END CORRCAT_DET;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION                CheckSimpleSearch
(
systemId INT,
text VARCHAR,
numRecord int
)
RETURN INT IS retValue INT;

cnt INT := 0;


BEGIN
retValue :=1;

if (retValue=1) then
if(numRecord =0)
then
SELECT COUNT(*) into cnt
from profile a LEFT JOIN dpa_doc_arrivo_par dp ON dp.id_profile = systemId LEFT JOIN dpa_corr_globali sim ON sim.system_id = dp.id_mitt_dest and systemId = dp.ID_PROFILE
where a.system_id = systemId and  (contains (a.var_prof_oggetto, text) > 0 OR (dp.id_profile = systemId AND sim.system_id = dp.id_mitt_dest and dp.ID_PROFILE = systemId AND contains (sim.var_desc_corr, 'test') > 0));
else
SELECT COUNT(*) into cnt
from profile a LEFT JOIN dpa_doc_arrivo_par dp ON dp.id_profile = systemId LEFT JOIN dpa_corr_globali sim ON sim.system_id = dp.id_mitt_dest and systemId = dp.ID_PROFILE
where a.system_id = systemId and  (contains (a.var_prof_oggetto, text) > 0 OR (dp.id_profile = systemId AND sim.system_id = dp.id_mitt_dest and dp.ID_PROFILE = systemId AND contains (sim.var_desc_corr, 'test') > 0) OR a.num_proto = numRecord);
end if;
/*SELECT COUNT(*) INTO cnt
FROM security
WHERE thing = thingParam AND personorgroup IN (idgroupParam, idpeopleParam)
and ACCESSRIGHTS>0;
*/

IF (cnt > 0) THEN
retValue := 1;
ELSE
retValue := 0;
END IF;
end if;
RETURN retValue;
END CheckSimpleSearch; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getpeoplebyRole(idCorrGlobale number)
RETURN number IS tmpVar number;

BEGIN
 tmpVar := 0;

begin
   SELECT people_system_id
     INTO tmpVar
     FROM (SELECT people_system_id
             FROM peoplegroups pg
            WHERE pg.groups_system_id = idCorrGlobale - 1)
    WHERE ROWNUM = 1;

   
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
      NULL;
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
      RAISE;
    end;
   RETURN tmpVar; 
    
END getpeoplebyRole;

/

SHOW ERRORS;


CREATE TABLE DPA_TRANS_STATE_DOC_IN_PRJ
(
  SYSTEM_ID         NUMBER(10),
  ID_STEP           NUMBER(10)                  NOT NULL,
  ID_DOC_TYPE       NUMBER(10)                  NOT NULL,
  ID_STATE_DIAGRAM  NUMBER(10)                  NOT NULL,
  ID_STATE          NUMBER(10)                  NOT NULL
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

ALTER TABLE PUBLISHER_INSTANCES
 ADD (PUBLISHERSERVICEURL  NVARCHAR2(1000));

CREATE OR REPLACE PROCEDURE SP_EST_CAMPI_DOC_PREGRESSI
( id_oggetto_in IN NUMBER
, id_template_in IN NUMBER
, anno_in IN NUMBER
, id_aoo_rf_in IN NUMBER
, returnvalue OUT NUMBER
) 
/******************************************************************************

  AUTHOR:  Lembo Alessandro

  NAME:     SP_EST_CAMPI_DOC_PREGRESSI

  PURPOSE:  Store procedure per propagare l'aggiunta di campi in una tipologia ai documenti pregressi.

******************************************************************************/
IS
--prelevo i doc pregressi
CURSOR DOC_PREGRESSI
IS
SELECT DISTINCT(DOC_NUMBER) 
FROM DPA_ASSOCIAZIONE_TEMPLATES 
WHERE ID_TEMPLATE= id_template_in 
AND DOC_NUMBER IS NOT NULL;

id_doc NUMBER(15);

BEGIN
OPEN DOC_PREGRESSI;
LOOP FETCH DOC_PREGRESSI INTO id_doc;
EXIT WHEN DOC_PREGRESSI%NOTFOUND;
      INSERT INTO DPA_ASSOCIAZIONE_TEMPLATES
      (
      SYSTEM_ID, 
      ID_OGGETTO,
      ID_TEMPLATE,
      Doc_Number,
      Valore_Oggetto_Db,
      Anno,
      ID_AOO_RF,
      CODICE_DB,
      MANUAL_INSERT,
      VALORE_SC,
      DTA_INS,
      ANNO_ACC
      )
      VALUES
      (
      SEQ_DPA_ASSOCIAZIONE_TEMPLATES.nextval, 
      id_oggetto_in,
      id_template_in,
      id_doc,
      '',
      anno_in,
      id_aoo_rf_in,
      '',
      0,
      NULL,
      SYSDATE,
      ''
      );
      END LOOP;
      CLOSE DOC_PREGRESSI;
      returnvalue:=0;
EXCEPTION WHEN OTHERS THEN returnvalue:=-1; RETURN;

END SP_EST_CAMPI_DOC_PREGRESSI;

/

SHOW ERRORS;

CREATE SEQUENCE SEQ_DPA_EXTERNAL_SYSTEMS
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_DPA_ITEMS_ESIBIZIONE
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_DPA_LOG  -- occhio all'inizio (prendere il valore più alto della seq ed aumentare di almeno 100.000
  START WITH 34181537
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_INST_ACC
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_DPA_TRANS_STATE_DOC_IN_PRJ
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;


CREATE SEQUENCE SEQ_DATI_FATTURAZIONE
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE OR REPLACE PROCEDURE PrimaSP IS
CURSOR custom_cur IS
SELECT SYSTEM_ID,DATA_FINE,ID_OGG,DATA_INIZIO FROM DPA_CONT_CUSTOM_DOC;
custom_rec custom_cur%rowtype;

BEGIN

LOOP
FETCH custom_cur into custom_rec;
EXIT WHEN custom_cur%NOTFOUND;
dbms_output.put_line( custom_rec.ID_OGG  );

END LOOP;
 
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END PrimaSP; 

/

SHOW ERRORS;



CREATE OR REPLACE PROCEDURE                sp_CheckConsConvAndUpdate (
   p_id_istanza                           NUMBER,
   p_doc_number                           NUMBER,
   p_newVersionId                         NUMBER,
   returnvalue                       OUT  NUMBER
  
)
IS
    estenzioneNuovaVersione    VARCHAR(5);
    numDocReportDaConv         NUMBER;
    numDocReportConvertiti     NUMBER;
    numDocReportInErrore       NUMBER;
    
    
BEGIN
DBMS_OUTPUT.put_line ('inizio stored');
       
      
   BEGIN
        DBMS_OUTPUT.put_line ('Controllo conversione documento in PDF');
        returnvalue := 1;
        
        select c.EXT 
        into estenzioneNuovaVersione
        from components c
        where c.docnumber = p_doc_number 
        AND c.version_id = p_newVersionId;
        
       DBMS_OUTPUT.put_line ('Aggiornamento stato documento del report EXT ' || estenzioneNuovaVersione);
        
        IF (estenzioneNuovaVersione = 'pdf')
         THEN
         
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
            
            
            UPDATE dpa_items_conservazione
            SET    VAR_TIPO_FILE = '.pdf'
            WHERE  ID_CONSERVAZIONE = p_id_istanza
            AND    ID_PROFILE = p_doc_number;
                        
            commit;
            --resultConv := '1';
            
            DBMS_OUTPUT.put_line ('Aggiornamento reportConservazione esito OK! Estensione documento: '|| estenzioneNuovaVersione);
          
          ELSE
          
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1',
                   ERRORE = '1',
                   TIPOERRORE = '2'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
    
            --resultConv := '1';
            commit;
            DBMS_OUTPUT.put_line ('Aggiornamento reportConservazione esito KO! Estensione documento:'|| estenzioneNuovaVersione);
        END IF;
      
  EXCEPTION
     WHEN OTHERS
     THEN 
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1',
                   ERRORE = '1',
                   TIPOERRORE = '2'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
        DBMS_OUTPUT.put_line ('ERRORE Controllo conversione documento in PDF');
        commit;
        --resultConv := 0;
       
  END;
   
   
   BEGIN
      --result := 0;

      SELECT    count(*) 
        INTO    numDocReportDaConv
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = 1;

      SELECT    count(*) 
        INTO    numDocReportConvertiti
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = '1'
        AND     rc.convertito = '1'
        AND     (rc.errore = 0 OR rc.errore is null);
              
      SELECT    count(*) 
        INTO    numDocReportInErrore
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = '1'
        AND     rc.convertito = '1'
        AND     rc.errore = '1'
        AND     rc.tipoerrore = '2';
      
      DBMS_OUTPUT.put_line ('Controllo documenti istanza '|| p_id_istanza ||' ' || numDocReportDaConv ||' ' || numDocReportConvertiti);
      
      IF (numDocReportDaConv > 0 AND numDocReportDaConv = numDocReportConvertiti)
      THEN
          IF numDocReportInErrore > 0 
          THEN
            
            UPDATE dpa_area_conservazione
            SET cha_stato = 'Z' --ERRORE CONVERSIONE
            WHERE system_id = p_id_istanza;
            
            DBMS_OUTPUT.put_line ('stato in errore');
            commit;
          ELSE
          
            UPDATE dpa_area_conservazione
            SET cha_stato = 'I',
                data_invio = SYSDATE
            WHERE system_id = p_id_istanza;
            
            DBMS_OUTPUT.put_line ('stato Inviata');
            commit;
          END IF;
         
      
         
         
      END IF;

      EXCEPTION
         WHEN OTHERS
         THEN 
           UPDATE dpa_area_conservazione
            SET cha_stato = 'Z' --ERRORE CONVERSIONE
            WHERE system_id = p_id_istanza;
            commit;
            DBMS_OUTPUT.put_line ('eccezione: stato in errore');
      END;

    DBMS_OUTPUT.put_line ('fine stored');
END sp_CheckConsConvAndUpdate; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE SelectDateCustom IS

datainizio DPA_CONT_CUSTOM_DOC.DATA_INIZIO%TYPE;
datafine DPA_CONT_CUSTOM_DOC.DATA_FINE%TYPE;
CURSOR date_cur IS
SELECT DATA_INIZIO,DATA_FINE FROM DPA_CONT_CUSTOM_DOC;
BEGIN
OPEN date_cur;
LOOP

FETCH date_cur into datainizio,datafine;
EXIT WHEN date_cur%NOTFOUND;

dbms_output.put_line(datainizio || ' ' || datafine);




END LOOP;

CLOSE date_cur;
  
   EXCEPTION
     WHEN NO_DATA_FOUND THEN
       NULL;
     WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
       RAISE;
END SelectDateCustom; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE                SP_FOLLOW_DOMAINOBJECT (
   -- Id corr globali della UO cui apparteneva il ruolo eliminato
   idProject     IN     NUMBER,
   idProfile     IN     NUMBER,
   operation     IN     NUMBER,
   idPeople      IN     NUMBER,
   idGroup       IN     NUMBER,
   idAmm         IN     NUMBER,
   application   IN     VARCHAR2,
   returnvalue      OUT NUMBER)
/******************************************************************************

  AUTHOR:  Camillo Ferlito

  NAME:     SP_FOLLOW_DOMAINOBJECT

  PURPOSE:  store procedures for the management of the concept of follow document / folder.

******************************************************************************/
IS
   --lista delle triple (utente, ruolo, app) che seguono il fascicolo con id = idProject
   CURSOR LIST_FOLLOW_FOLDER
   IS
      SELECT ID_ROLE_FOLLOW,
             ID_PEOPLE_FOLLOW,
             ID_AMM,
             APP
        FROM DPA_FOLLOW_OBJECT
       WHERE ID_OBJECT = idProject AND DOMAINOBJECT = 'FASCICOLO';

   --lista dei documenti nel fascicolo con id = idProject sui quali l'utenza( idpeople, idgroup) ha visibilità
   CURSOR LIST_DOC_IN_FOLDER
   IS
      SELECT DISTINCT s.thing
        FROM project_components pc
             JOIN project p
                ON pc.project_id = p.system_id AND p.id_fascicolo = idProject
             JOIN security s
                ON pc.link = s.thing
       WHERE s.personorgroup = idGroup AND s.accessrights >= 45;

   -- variabili di appoggio utilizzati per scorrere i cursori
   id_role     NUMBER (10) := 0;
   id_people   NUMBER (10) := 0;
   id_amm      NUMBER (10) := 0;
   appl        VARCHAR2 (500 CHAR);
   id_doc      NUMBER (10);
   counter     INT (10) := 0;
BEGIN
   IF operation = 0                                            -- AddDocFolder
   THEN
      OPEN LIST_FOLLOW_FOLDER;

      LOOP
         FETCH LIST_FOLLOW_FOLDER
         INTO id_role, id_people, id_amm, appl;

         EXIT WHEN LIST_FOLLOW_FOLDER%NOTFOUND;

         -- se viene già seguito il documento allora si elimina e crea nuovamente il record
         DELETE FROM DPA_FOLLOW_OBJECT
               WHERE     ID_OBJECT = idProfile
                     AND ID_ROLE_FOLLOW = id_role
                     AND ID_PEOPLE_FOLLOW = id_people
                     AND APP = appl;

         INSERT INTO DPA_FOLLOW_OBJECT
              VALUES (idProfile,
                      'DOCUMENTO',
                      id_role,
                      id_people,
                      id_amm,
                      appl);
      END LOOP;


      CLOSE LIST_FOLLOW_FOLDER;

      returnvalue := 0;
   ELSIF operation = 1                                      -- RemoveDocFolder
   THEN
      OPEN LIST_FOLLOW_FOLDER;

      LOOP
         FETCH LIST_FOLLOW_FOLDER
         INTO id_role, id_people, id_amm, appl;

         EXIT WHEN LIST_FOLLOW_FOLDER%NOTFOUND;

         -- se  count > 0 vuol dire che il documento è contenuto anche in qualche altro fascicolo
         -- seguito dalla terna (id_group, id_people, appl), dunque il documento continua ad essere seguito da (id_group, id_people, appl)

         SELECT COUNT (*)
           INTO counter
           FROM dpa_follow_object
          WHERE     ID_OBJECT IN
                       ( -- recupero gli altri fascicoli dove è contenuto il doc da eliminare
                        SELECT DISTINCT p.id_fascicolo
                          FROM project_components pc
                               JOIN project p
                                  ON     pc.project_id =
                                            p.system_id
                                     AND p.id_fascicolo !=
                                            idProject
                               JOIN security s
                                  ON pc.link = s.thing
                         WHERE     pc.link = idProfile
                               AND s.personorgroup = id_role
                               AND s.accessrights >= 45)
                AND ID_ROLE_FOLLOW = id_role
                AND ID_PEOPLE_FOLLOW = id_people
                AND APP = appl;

         IF counter = 0
         THEN
            DELETE DPA_FOLLOW_OBJECT
             WHERE     ID_OBJECT = idProfile
                   AND ID_ROLE_FOLLOW = id_role
                   AND ID_PEOPLE_FOLLOW = id_people
                   AND APP = appl;
         END IF;
      END LOOP;

      CLOSE LIST_FOLLOW_FOLDER;

      returnvalue := 0;
   ELSIF operation = 2                                             --AddFolder
   THEN
      SELECT COUNT (*)
        INTO counter
        FROM security s JOIN project p ON s.thing = p.system_id
       WHERE     s.thing = idProject
             AND s.personorgroup = idGroup
             AND s.accessrights >= 45;

      IF counter > 0 -- se si tratta di un fascicolo sul quale il ruolo ha visibilità andiamo avanti nell'esecuzione
      THEN
         SELECT COUNT (*)
           INTO counter
           FROM dpa_follow_object
          WHERE     ID_OBJECT = idProject
                AND ID_ROLE_FOLLOW = idGroup
                AND ID_PEOPLE_FOLLOW = idPeople
                AND APP = application;

         -- se il fascicolo non è già seguito da (idGroup, idPeople, application)
         --allora aggiungo il record in dpa_follow_object
         IF counter = 0
         THEN
            INSERT INTO dpa_follow_object
                 VALUES (idProject,
                         'FASCICOLO',
                         idGroup,
                         idPeople,
                         idAmm,
                         application);
         END IF;

         OPEN LIST_DOC_IN_FOLDER;

         -- cerco l'elenco dei documenti contenuti nel fascicolo e visibili al ruolo idGroup
         LOOP
            FETCH LIST_DOC_IN_FOLDER INTO id_doc;

            EXIT WHEN LIST_DOC_IN_FOLDER%NOTFOUND;

            --elimino ed inserisco il record in quanto l'utenza(idPeople, idGroup) potrebbe già seguire il documentoù
            DELETE DPA_FOLLOW_OBJECT
             WHERE     ID_OBJECT = id_doc
                   AND ID_ROLE_FOLLOW = idGroup
                   AND ID_PEOPLE_FOLLOW = idPeople
                   AND APP = application;

            INSERT INTO dpa_follow_object
                 VALUES (id_doc,
                         'DOCUMENTO',
                         idGroup,
                         idPeople,
                         idAmm,
                         application);
         END LOOP;

         CLOSE LIST_DOC_IN_FOLDER;
      END IF;

      returnvalue := 0;
   ELSIF operation = 3                                          --RemoveFolder
   THEN      -- controllo che idProject si riferisca realmente ad un fascicolo
      SELECT COUNT (*)
        INTO counter
        FROM project p
       WHERE system_id = idProject;

      IF counter > 0
      THEN
         OPEN LIST_DOC_IN_FOLDER;

         LOOP
            FETCH LIST_DOC_IN_FOLDER INTO id_doc;

            EXIT WHEN LIST_DOC_IN_FOLDER%NOTFOUND;

            SELECT COUNT (*) -- numero dei fascicoli contenenti il documento id_doc  e seguiti da
              -- (idGroup, idPeople, application)
              INTO counter
              FROM dpa_follow_object
             WHERE     ID_OBJECT IN
                          ( -- recupero gli altri fascicoli dove è contenuto il doc
                           SELECT DISTINCT p.id_fascicolo
                             FROM project_components pc
                                  JOIN project p
                                     ON     pc.project_id =
                                               p.system_id
                                        AND p.id_fascicolo !=
                                               idProject
                                  JOIN security s
                                     ON pc.link = s.thing
                            WHERE     pc.link = id_doc
                                  AND s.personorgroup = idGroup
                                  AND s.accessrights >= 45)
                   AND ID_ROLE_FOLLOW = idGroup
                   AND ID_PEOPLE_FOLLOW = idPeople
                   AND APP = application;

            IF counter = 0 -- se il documento è contenuto in altri fascicoli seguiti dalla terna
            -- quest'ultima deve continuare a seguirlo
            THEN
               DELETE FROM dpa_follow_object
                     WHERE     id_object = id_doc
                           AND id_role_follow = idGroup
                           AND id_people_follow = idpeople
                           AND app = application;
            END IF;
         END LOOP;

         CLOSE LIST_DOC_IN_FOLDER;

         --elimino il segui sul fascicolo
         DELETE DPA_FOLLOW_OBJECT
          WHERE     ID_OBJECT = idProject
                AND ID_ROLE_FOLLOW = idGroup
                AND ID_PEOPLE_FOLLOW = idPeople
                AND APP = application;
      END IF;

      returnvalue := 0;
   ELSIF operation = 4                                                --AddDoc
   THEN
      SELECT COUNT (*) --questo controllo permette di prevenire errori di sviluppo del tipo
        -- passagio di un idProject al posto di un idProfile, prevenire il follow su un documento non visibile alll'utente
        INTO counter
        FROM security s JOIN profile p ON s.thing = p.system_id
       WHERE     s.thing = idProfile
             AND s.personorgroup = idGroup
             AND s.accessrights >= 45;

      IF counter > 0
      THEN
         DELETE DPA_FOLLOW_OBJECT
          WHERE     ID_OBJECT = idProfile
                AND ID_ROLE_FOLLOW = idGroup
                AND ID_PEOPLE_FOLLOW = idPeople
                AND APP = application;

         INSERT INTO dpa_follow_object
              VALUES (idProfile,
                      'DOCUMENTO',
                      idGroup,
                      idPeople,
                      idAmm,
                      application);
      END IF;

      returnvalue := 0;
   ELSIF operation = 5                                             --RemoveDoc
   THEN
      SELECT COUNT (*)
        INTO counter
        FROM security s JOIN profile p ON s.thing = p.system_id
       WHERE     s.thing = idProfile
             AND s.personorgroup = idGroup
             AND s.accessrights >= 45;

      IF counter > 0
      THEN
         DELETE DPA_FOLLOW_OBJECT
          WHERE     ID_OBJECT = idProfile
                AND ID_ROLE_FOLLOW = idGroup
                AND ID_PEOPLE_FOLLOW = idPeople
                AND APP = application;
      END IF;

      returnvalue := 0;
   END IF;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line (
         'Eccezione durante l''esecuzione della store procedure SPFollowDomainObject');
      returnvalue := -1;
      RETURN;
END;

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE sp_getcontatorealertcons
(
p_userid NUMBER, 
p_groupid NUMBER,
p_ammid NUMBER, 
p_operaz VARCHAR ,
p_risultato OUT NUMBER
)

-- 0: le condizioni per l'invio dell'alert non sono soddisfatte
-- 1: le condizioni per l'invio dell'alert sono soddisfatte
-- -1: errore nella funzione

IS
isPresente number;
contatore number;
intervallo number;
soglia number;
dta_fine_mon date;
sysid number;
BEGIN
-- 1.
-- esiste già un contatore per l'utente
-- per l'alert selezionato?
SELECT COUNT(*)
INTO isPresente
FROM dpa_alert_conservazione
WHERE
id_amm = p_ammid AND
id_people = p_userid AND
id_gruppo = p_groupid AND
var_codice = p_operaz;
-- 2a.
-- il contatore non c'è > creo il record
DBMS_OUTPUT.PUT_LINE('isPresente = ' || isPresente); -- REMOVE
IF(isPresente = 0) THEN
  SELECT seq_dpa_alert_conservazione.nextval
  INTO sysid
  FROM dual;
  
	INSERT INTO dpa_alert_conservazione
	(system_id, id_amm, id_people, id_gruppo, var_codice, num_operazioni, dta_inizio_monitoraggio)
	VALUES
	( sysid, p_ammid, p_userid, p_groupid, p_operaz, 1, SYSDATE);
	p_risultato := 0;
ELSE
	-- 2b.
	-- il contatore esiste
	-- controllo se il periodo di monitoraggio è terminato
	-- cioè se data_inizio_mon + intervallo è successiva alla
	-- data attuale
	IF(p_operaz = 'LEGGIBILITA_SINGOLO') THEN
		SELECT num_legg_sing_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;
	ELSIF(p_operaz = 'DOWNLOAD') THEN
		SELECT num_download_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;	
	ELSIF(p_operaz = 'SFOGLIA') THEN
		SELECT num_sfoglia_periodo_mon
		INTO intervallo
		FROM dpa_config_alert_cons
		WHERE id_amm=p_ammid;
	END IF;
	DBMS_OUTPUT.PUT_LINE('INTERVALLO = ' || intervallo); -- REMOVE
  
	SELECT dta_inizio_monitoraggio
	INTO dta_fine_mon
	FROM dpa_alert_conservazione
	WHERE id_amm = p_ammid AND
	id_people = p_userid AND
	id_gruppo = p_groupid AND
	var_codice = p_operaz;
	dta_fine_mon := dta_fine_mon + intervallo;
  DBMS_OUTPUT.PUT_LINE('data fine mon = ' || dta_fine_mon); -- REMOVE
	-- 3a.
	-- il periodo di monitoraggio è terminato
	-- inizializzo il contatore a 1 e alla data attuale
	IF (dta_fine_mon < SYSDATE) THEN
		UPDATE dpa_alert_conservazione
		SET
		num_operazioni=1,
		dta_inizio_monitoraggio=SYSDATE
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
		p_risultato := 0;
	ELSE
	-- 3b.
	-- il periodo di monitoraggio è in corso
	-- incremento il contatore e verifico se ha raggiunto la soglia
	-- per l'invio dell'alert
		IF(p_operaz = 'LEGGIBILITA_SINGOLO') THEN
			SELECT num_legg_sing_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;
		ELSIF(p_operaz = 'DOWNLOAD') THEN
			SELECT num_download_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;
		ELSIF(p_operaz = 'SFOGLIA') THEN
			SELECT num_sfoglia_max_oper
			INTO soglia
			FROM dpa_config_alert_cons
			WHERE id_amm=p_ammid;		
		END IF;
		
		UPDATE dpa_alert_conservazione
		SET
		num_operazioni = num_operazioni+1
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
	
		SELECT num_operazioni
		INTO contatore
		FROM dpa_alert_conservazione
		WHERE
		id_people = p_userid AND
		id_gruppo = p_groupid AND 
		id_amm = p_ammid AND
		var_codice = p_operaz;
		-- 4a.
		-- il contatore è inferiore alla soglia
		-- non invio alert
		IF(contatore<=soglia) THEN
			p_risultato := 0;
		ELSE
		-- 4b.
		-- è stata raggiunta la soglia
		-- azzero il contatore e aggiorno la data
			UPDATE dpa_alert_conservazione
			SET
			num_operazioni = 1,
			dta_inizio_monitoraggio = SYSDATE
			WHERE
			id_people = p_userid AND
			id_gruppo = p_groupid AND 
			id_amm = p_ammid AND
			var_codice = p_operaz;
			
			p_risultato := 1;
		END IF;
	END IF;
END IF;

EXCEPTION
WHEN OTHERS THEN
	p_risultato := -1;

END sp_getcontatorealertcons;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          noAllegati (docId INT)
RETURN int IS risultato int;
BEGIN
select count(system_id) into risultato from profile p where p.ID_DOCUMENTO_PRINCIPALE =docId;
if(risultato>0)
then risultato:=1;
end if;
RETURN risultato;
END noAllegati; 

/

SHOW ERRORS;



CREATE OR REPLACE FUNCTION getcodtipofunz(idfunz NUMBER)
RETURN VARCHAR2 IS risultato VARCHAR2(16);

BEGIN
begin

SELECT VAR_COD_tipo INTO risultato FROM DPA_tipo_funzione WHERE SYSTEM_ID = idfunz;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END getcodtipofunz; 

/

SHOW ERRORS;
 -----------   FINO A QUI 1----------
 
 
ALTER TABLE PEOPLE
 ADD (CHA_SUPERADMIN  CHAR(1 BYTE)                  DEFAULT '0');

ALTER TABLE PEOPLE
 ADD (ABILITATO_ESIBIZIONE  CHAR(1 BYTE)            DEFAULT '0');

ALTER TABLE PEOPLE
 ADD (CHA_SYSTEM_USER  VARCHAR2(1 BYTE)             DEFAULT '0');

ALTER TABLE PEOPLE
 ADD (VAR_MEMENTO  VARCHAR2(50 BYTE));



ALTER TABLE DPA_AMMINISTRA
 ADD (TIPO_COMPONENTI  CHAR(1 BYTE));

ALTER TABLE DPA_CORR_GLOBALI
 ADD (CHA_SYSTEM_ROLE  VARCHAR2(1 BYTE)             DEFAULT '0');


--update dpa_corr_globali set CHA_SYSTEM_ROLE = '0';


CREATE SEQUENCE SEQ_AMMINISRAZIONI
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE TABLE DPA_PRODUCT_INTEGRATION_SVCS
(
  SYSTEM_ID    NUMBER                           NOT NULL,
  METHOD_NAME  VARCHAR2(255 BYTE),
  DESCRIPTION  VARCHAR2(511 BYTE),
  FILE_SVC     VARCHAR2(127 BYTE),
  CHECKABLE    VARCHAR2(1 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE INST_ACC
(
  SYSTEM_ID                   NUMBER(10)        NOT NULL,
  DESCRIPTION                 VARCHAR2(2000 CHAR) NOT NULL,
  DTA_CREAZIONE               DATE              NOT NULL,
  DTA_CHIUSURA                DATE,
  ID_PEOPLE_PROPRIETARIO      NUMBER(10)        NOT NULL,
  ID_GRUPPO_PROPRIETARIO      NUMBER(10)        NOT NULL,
  ID_RICHIEDENTE              NUMBER(10),
  DTA_RICHIESTA               DATE,
  ID_DOCUMENTO_RICHIESTO      NUMBER(10),
  NOTE                        VARCHAR2(2000 CHAR),
  CHA_STATO_DOWNLOAD_INOLTRO  CHAR(1 CHAR)      DEFAULT '0',
  ID_PROFILE_DOWNLOAD         NUMBER(10)        DEFAULT 0
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE INST_ACC_DOC
(
  SYSTEM_ID       NUMBER(10)                    NOT NULL,
  ID_INST_ACC     NUMBER(10)                    NOT NULL,
  DOCNUMBER       NUMBER(10),
  ID_PROJECT      NUMBER(10),
  ID_PARENT       NUMBER(10),
  TIPO_RICHIESTA  VARCHAR2(256 CHAR),
  ENABLE          CHAR(2 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE INST_ACC_ATT
(
  SYSTEM_ID        NUMBER(10),
  ID_INST_ACC_DOC  NUMBER(10)                   NOT NULL,
  ID_ATTACH        NUMBER(10)                   NOT NULL,
  ENABLE           CHAR(2 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_CONFIG_ALERT_CONS
(
  SYSTEM_ID                       NUMBER        NOT NULL,
  ID_AMM                          NUMBER,
  VAR_SERVER_SMTP                 CHAR(64 BYTE),
  NUM_PORTA_SMTP                  NUMBER,
  CHA_SMTP_SSL                    CHAR(1 BYTE),
  VAR_USER_MAIL                   CHAR(128 BYTE),
  VAR_PWD_MAIL                    CHAR(128 BYTE),
  VAR_MAIL_NOTIFICA               CHAR(500 BYTE),
  CHA_ALERT_LEGGIBILITA_SCADENZA  CHAR(1 BYTE),
  NUM_LEGG_SCADENZA_TERMINE       NUMBER,
  NUM_LEGG_SCADENZA_TOLLERANZA    NUMBER,
  CHA_ALERT_LEGGIBILITA_MAX_DOC   CHAR(1 BYTE),
  NUM_LEGGIBILITA_MAX_DOC_PERC    NUMBER,
  CHA_ALERT_LEGGIBILITA_SING      CHAR(1 BYTE),
  NUM_LEGG_SING_MAX_OPER          NUMBER,
  NUM_LEGG_SING_PERIODO_MON       NUMBER,
  CHA_ALERT_DOWNLOAD              CHAR(1 BYTE),
  NUM_DOWNLOAD_MAX_OPER           NUMBER,
  NUM_DOWNLOAD_PERIODO_MON        NUMBER,
  CHA_ALERT_SFOGLIA               CHAR(1 BYTE),
  NUM_SFOGLIA_MAX_OPER            NUMBER,
  NUM_SFOGLIA_PERIODO_MON         NUMBER,
  VAR_MAIL_DESTINATARIO           VARCHAR2(128 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_ALERT_CONSERVAZIONE
(
  SYSTEM_ID                NUMBER               NOT NULL,
  ID_AMM                   NUMBER,
  ID_PEOPLE                NUMBER,
  ID_GRUPPO                NUMBER,
  VAR_CODICE               VARCHAR2(250 BYTE),
  NUM_OPERAZIONI           NUMBER,
  DTA_INIZIO_MONITORAGGIO  DATE
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_VERIFICA_FORMATI_CONS
(
  SYSTEM_ID         NUMBER                      NOT NULL,
  ID_ISTANZA        NUMBER                      NOT NULL,
  ID_ITEM           NUMBER,
  DOCNUMBER         NUMBER,
  ID_PROJECT        NUMBER,
  ID_DOCPRINCIPALE  NUMBER,
  VERSION_ID        NUMBER,
  TIPO_FILE         CHAR(1 BYTE),
  CONSOLIDATO       NUMBER(1),
  CONVERTIBILE      NUMBER(1),
  MODIFICA          NUMBER(1),
  UT_PROP           NUMBER,
  RUOLO_PROP        NUMBER,
  VALIDO            NUMBER(1),
  AMMESSO           NUMBER(1),
  FIRMATA           NUMBER(1),
  MARCATA           NUMBER(1),
  ERRORE            NUMBER(1),
  TIPOERRORE        NUMBER(1),
  DACONVERTIRE      NUMBER(1),
  CONVERTITO        NUMBER(1),
  ESITO             VARCHAR2(400 BYTE),
  ESTENSIONE        VARCHAR2(32 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_STATO_VERIFICA_CONS
(
  SYSTEM_ID  NUMBER                             NOT NULL,
  VALORE     VARCHAR2(200 BYTE)                 NOT NULL
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE OR REPLACE FUNCTION getcodtiporuolo(idtipo INT)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
if(idtipo is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_codice into risultato FROM DPA_tipo_ruolo WHERE SYSTEM_ID=idtipo;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END getcodtiporuolo; 

/

SHOW ERRORS;

ALTER TABLE DPA_COPY_LOG
 ADD (CHA_COPIA_VISIBILITA  CHAR(1 CHAR)            DEFAULT 0);

CREATE OR REPLACE function  getlong( p_tname in varchar2,
                                         p_cname in varchar2,
                                         p_rowid in rowid ) return varchar2
    as
        l_cursor    integer default dbms_sql.open_cursor;
        l_n         number;
        l_long_val  varchar2(4000);
        l_long_len  number;
        l_buflen    number := 4000;
       l_curpos    number := 0;
   begin
       dbms_sql.parse( l_cursor,
                      'select ' || p_cname || ' from ' || p_tname ||
                                                        ' where rowid = :x',
                       dbms_sql.native );
       dbms_sql.bind_variable( l_cursor, ':x', p_rowid );
   
       dbms_sql.define_column_long(l_cursor, 1);
       l_n := dbms_sql.execute(l_cursor);
   
       if (dbms_sql.fetch_rows(l_cursor)>0)
       then
          dbms_sql.column_value_long(l_cursor, 1, l_buflen, l_curpos ,
                                     l_long_val, l_long_len );
      end if;
      dbms_sql.close_cursor(l_cursor);
      return l_long_val;
   End Getlong;

/

SHOW ERRORS;

CREATE TABLE DPA_CORR_GLO_X_INT_TIPI_RUO
(
  SYSTEM_ID           NUMBER(10),
  ID_REGISTRO         NUMBER(10),
  ID_AMM              NUMBER(10),
  VAR_COD_RUBRICA     VARCHAR2(128 BYTE),
  VAR_DESC_CORR       VARCHAR2(256 BYTE),
  ID_OLD              NUMBER(10),
  DTA_INIZIO          DATE,
  DTA_FINE            DATE,
  ID_PARENT           NUMBER(10),
  NUM_LIVELLO         NUMBER(10),
  VAR_CODICE          VARCHAR2(128 BYTE),
  ID_GRUPPO           NUMBER(10),
  ID_TIPO_RUOLO       NUMBER(10),
  CHA_DEFAULT_TRASM   VARCHAR2(1 BYTE),
  ID_UO               NUMBER(10),
  VAR_COGNOME         VARCHAR2(50 BYTE),
  VAR_NOME            VARCHAR2(50 BYTE),
  ID_PEOPLE           NUMBER(10),
  CHA_TIPO_CORR       VARCHAR2(1 BYTE),
  CHA_TIPO_IE         VARCHAR2(1 BYTE),
  CHA_TIPO_URP        VARCHAR2(1 BYTE),
  CHA_PA              VARCHAR2(1 BYTE),
  VAR_CODICE_AOO      VARCHAR2(16 BYTE),
  VAR_CODICE_AMM      VARCHAR2(32 BYTE),
  VAR_CODICE_ISTAT    VARCHAR2(32 BYTE),
  ID_PESO             NUMBER(10),
  VAR_EMAIL           VARCHAR2(128 BYTE),
  CHA_DETTAGLI        VARCHAR2(1 BYTE),
  NUM_FIGLI           NUMBER(10),
  VAR_SMTP            VARCHAR2(128 BYTE),
  NUM_PORTA_SMTP      NUMBER(10),
  VAR_FAX_USER_LOGIN  VARCHAR2(8 BYTE),
  CHA_RIFERIMENTO     VARCHAR2(1 BYTE),
  ID_PEOPLE_LISTE     INTEGER,
  ID_GRUPPO_LISTE     NUMBER(10),
  CHA_RESPONSABILE    VARCHAR2(1 BYTE),
  ID_PESO_ORG         NUMBER(10),
  AOO_SOSPESO         VARCHAR2(1 BYTE),
  CHA_SEGRETARIO      VARCHAR2(1 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE OR REPLACE FUNCTION                getCodTit2(idParent number) RETURN varchar2 IS
tmpVar varchar2(32);
BEGIN
begin
select upper(var_codice) into tmpvar from project where system_id=idParent;
if(tmpvar is null)
then
select getcodtit(id_parent) into tmpvar from project where cha_tipo_proj='F' connect by prior id_parent = system_id start with system_id=idParent and rownum = 1;
end if;
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:='';

end;
RETURN tmpVar;
END getCodTit2; 

/

SHOW ERRORS;

CREATE OR REPLACE Function CreateJobs
   RETURN number
IS
   idNumber number;

BEGIN
      INSERT INTO DPA_JOBS
      VALUES(seq.nextval);
      idNumber := seq.currval;
RETURN idNumber;
END;

/

SHOW ERRORS;


CREATE OR REPLACE FUNCTION             codrfappartenzaRuolo (idRuolo INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (128);
BEGIN
   BEGIN

                select codice into risultato from (
                SELECT a.var_codice codice
                  FROM dpa_l_ruolo_reg t,dpa_corr_globali cg,dpa_el_registri a
                 WHERE t.id_registro = a.system_id and cha_rf = '1' and
                    cg.system_id =idRuolo and
                           cg.system_id=t.ID_RUOLO_IN_UO
                             order by t.system_id asc) where rownum=1;
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenzaRuolo; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE SP_EST_CAMPI_FASC_PREGRESSI 
( id_oggetto_in IN NUMBER
, id_template_in IN NUMBER
, anno_in IN NUMBER
, id_aoo_rf_in IN NUMBER
, returnvalue OUT NUMBER
) 
/******************************************************************************

  AUTHOR:  Lembo Alessandro

  NAME:     SP_EST_CAMPI_FASC_PREGRESSI

  PURPOSE:  Store procedure per propagare l'aggiunta di campi in una tipologia ai fascicoli pregressi.

******************************************************************************/
IS
--prelevo i fasc pregressi
CURSOR FASC_PREGRESSI
IS
SELECT DISTINCT(id_project) 
FROM DPA_ASS_TEMPLATES_FASC 
WHERE ID_TEMPLATE= id_template_in 
AND id_project IS NOT NULL;

id_fasc NUMBER(15);

BEGIN
OPEN FASC_PREGRESSI;
LOOP FETCH FASC_PREGRESSI INTO id_fasc;
EXIT WHEN FASC_PREGRESSI%NOTFOUND;
      INSERT INTO DPA_ASS_TEMPLATES_FASC
      (
      SYSTEM_ID, 
      ID_OGGETTO,
      ID_TEMPLATE,
      id_project,
      Valore_Oggetto_Db,
      Anno,
      ID_AOO_RF,
      CODICE_DB,
      MANUAL_INSERT,
      VALORE_SC,
      DTA_INS,
      ANNO_ACC
      )
      VALUES
      (
      SEQ_DPA_ASS_TEMPLATES_FASC.nextval, 
      id_oggetto_in,
      id_template_in,
      id_fasc,
      '',
      anno_in,
      id_aoo_rf_in,
      '',
      0,
      NULL,
      SYSDATE,
      ''
      );
      END LOOP;
      CLOSE FASC_PREGRESSI;
      returnvalue:=0;
EXCEPTION WHEN OTHERS THEN returnvalue:=-1; RETURN;

END SP_EST_CAMPI_FASC_PREGRESSI;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getutentiCorcat(idRuolo number)
RETURN VARCHAR IS risultato VARCHAR(256);
BEGIN
declare item varchar(4000);

CURSOR cur IS
SELECT p.user_id from people p,peoplegroups pg where pg.PEOPLE_SYSTEM_ID = p.system_id 
and pg.GROUPS_SYSTEM_ID = idRuolo and pg.dta_fine is null and p.DISABLED = 'N';

BEGIN
risultato := '';
OPEN cur;
LOOP
FETCH cur INTO item;
EXIT WHEN cur%NOTFOUND;

IF(risultato IS NOT NULL) THEN
risultato := risultato||'; '||item ;
ELSE
risultato := risultato||item;
END IF;

END LOOP;

exception  WHEN OTHers then risultato:='';
end;

RETURN risultato;
end; 

/

SHOW ERRORS;

ALTER TABLE POLICY_CONSERVAZIONE
 ADD (STATO_CONVERSIONE  VARCHAR2(10 BYTE));

CREATE OR REPLACE PROCEDURE SP_MODIFY_UO_FATTURAZIONE (
	oldCodiceUO	IN	VARCHAR2,
	newCodiceUO	IN	VARCHAR2,
	idAmm		IN	NUMBER,
	codiceAoo	IN	VARCHAR2,
  returnValue 	OUT	NUMBER
)

Is
	codiceAmmIpa 	VARCHAR2(128);
	codiceAooIpa	VARCHAR2(128);
	codiceUac 	VARCHAR2(128);
	codiceClassificazione 	VARCHAR2(128);
  isFatturazione VARCHAR2(128) := NULL;
		
 
begin 

	-- selezione il codice ipa amministrazione tramite l'id
	SELECT var_codice_amm_ipa
	INTO codiceAmmIpa
	FROM dpa_amministra 
	WHERE system_id = idAmm;

	--selezione del codice ipa aoo tramite il codice AOO
	SELECT var_codice_aoo_ipa
	INTO codiceAooIpa
	FROM dpa_el_registri
	WHERE var_codice = codiceAoo AND CHA_RF=0;
  
  
  SELECT var_desc_atto
  INTO isFatturazione
  FROM DPA_TIPO_ATTO
  WHERE ID_AMM= idAmm AND var_desc_atto = 'Fattura elettronica';

	-- caso di inserimento della UO in PITRE: mi accorgo che si tratta di una nuova UO perchè oldCodiceUO è null
	if (oldCodiceUO IS NULL)
  THEN

    BEGIN
        SELECT codice_uac, codice_classificazione
        INTO codiceUac, codiceClassificazione
        FROM dpa_el_registri
        WHERE Id_Amm= idAmm AND var_codice = codiceAoo;

      end;

    if(isFatturazione is not NULL)
    then
    BEGIN
        INSERT INTO DPA_DATI_FATTURAZIONE 
        (system_id, codice_amm, codice_aoo, codice_uo, codice_uac, codice_classificazione, var_utente_proprietario, var_tipologia_documento, var_ragione_trasmissione)
        VALUES(SEQ_DATI_FATTURAZIONE.Nextval, codiceAmmIpa, codiceAooIpa, newCodiceUO, codiceUac, codiceClassificazione, 'PROVA_TIBCO', 'Fattura elettronica', 'Ricevimento fattura');
        returnValue:=SQL%ROWCOUNT;
         EXCEPTION
         WHEN OTHERS  THEN
        dbms_output.put_line('5o blocco eccezione') ; 
        returnvalue := 5;
        RETURN;
    END;
   
    END IF;


      -- se outValue è 0, allora la UO non è presente nella tabella TIBCO; in tal caso si avvia il job
      -- che si occupa di ritentare l'aggiornamento nella tabella
	
	ELSE
	-- caso di modifica del codice UO in PITRE
	BEGIN
		UPDATE DPA_DATI_FATTURAZIONE
		SET codice_uo = newCodiceUO
		WHERE UPPER(codice_amm) = UPPER(codiceAmmIpa) AND UPPER(codice_aoo) = UPPER(codiceAooIpa) AND UPPER(codice_uo) = UPPER(oldCodiceUO);
		returnValue:=SQL%ROWCOUNT;
     EXCEPTION
    WHEN OTHERS  THEN
      dbms_output.put_line('6o blocco eccezione') ; 
      returnvalue := 6;
      RETURN;
	END;
	END IF;

end;

/

SHOW ERRORS;

CREATE OR REPLACE procedure utl_write_to_appcatalog (tipo_obj Varchar2, Nome_Plsql Varchar2) is 
Cursor C_Plsql (Nome_Plsql Varchar2) Is 
Select Type, Name As Nome, Text As Testo_Originale 
-- testo in minuscolo, più spazi trasformati in uno solo
, To_Clob( Lower( --Replace (,Chr(59),    Chr(59)||Chr(10)  ) 
        Regexp_Replace(Text, '[ ]+', Chr(32) )   )
          ) As Testo_Clob 
, To_Clob(Lower(Text)) As Testo_Clob_cn_spazi           
From User_Source 
Where Upper(Name)=Upper(Nome_Plsql) ; 

Istruzione Varchar2(32000); Num_Rows Int; Cnt Int;  --My_New_Clob Clob; 
L_Clob      Clob := 'CREATE OR REPLACE '; 
L_Clob_safe Clob := 'CREATE OR REPLACE '; 
titolo Varchar2(24);
Begin

For Plsql In C_Plsql (Nome_Plsql )  Loop
           Dbms_Lob.Writeappend (L_Clob,      Length( Plsql.Testo_Clob)         ,  Plsql.Testo_Clob );-- alternative way  -- Dbms_Lob.Append (V_Clob,  (Plsql.Testo) || ' ');
           Dbms_Lob.Writeappend (L_Clob_safe, Length( Plsql.Testo_Clob_cn_spazi),  Plsql.Testo_Clob_cn_spazi);
       End Loop;
       -- if not exists:
  Select Count(*) Into Cnt From Utl_Track_Plsql_Change_  
        Where Var_Dict_Obj_Type= Upper(Tipo_Obj) and Var_Dict_Obj_Name= Upper(Nome_Plsql);
  If Cnt = 0 Then        
  titolo := Utl_Get_Titolo_Pitre;
       Insert Into Utl_Track_Plsql_Change_ (Var_Dict_Obj_Type,Var_Dict_Obj_Name,Dta_Of_Change       
            ,Var_Current_Docspa_Version,Clob_Previous_Version_Of_Code,Clob_Current_Version_Of_Code, Clob_Code_safe_to_restore)
        Values (Upper(Tipo_Obj), Upper(Nome_Plsql), Sysdate
            , Titolo, Null, L_Clob   ,L_Clob_safe     );
      Dbms_Output.Put_Line('Codice inserito per '||Nome_Plsql||' in Utl_Track_Plsql_Change_.Clob_Current_Version_Of_Code; '
      ||'inseriti : '||To_Char(Sql%Rowcount )||' record.'); 
  else 
       Istruzione :=        
       'Update Utl_Track_Plsql_Change_ 
       Set Clob_Current_Version_Of_Code = :1
       , Clob_Code_safe_to_restore = :2
       Where Var_Dict_Obj_Name = '''||Nome_Plsql ||''' and Var_Dict_Obj_Type= Upper('''||tipo_obj||''') ';
       
       Execute Immediate Istruzione Using L_Clob,L_Clob_safe ;              
       Dbms_Output.Put_Line('Codice aggiornato per '||Nome_Plsql||' in Utl_Track_Plsql_Change_.Clob_Current_Version_Of_Code; '
       ||'aggiornati : '||To_Char(Sql%Rowcount )||' record.'); 
  End If; 
  Commit;
exception        When Others Then
    Rollback;
    Raise;
  end;

/

SHOW ERRORS;


CREATE OR REPLACE PROCEDURE                ATTIVA_LOG_CONSERVAZIONE (
  returnValue     OUT    NUMBER
)
IS
BEGIN
  DECLARE
      -- Cursore per scorrere tutte le amministrazioni su cui attivare i log_amm di conservazione
      CURSOR idAmministrazioni
      IS
         SELECT SYSTEM_ID
         FROM dpa_amministra;
         
      -- Count del system_id_anagrafica già attivi un dpa_log_attivati
      SysIdAnag NUMBER;
    
    BEGIN
    
    -- Per ogni amministrazione, scorro tutti i log_amm con oggetto conservazione
      FOR idAmm IN idAmministrazioni
      LOOP
         
         BEGIN
         
            DECLARE
                -- Cursore per prelevare tutti i systemid dei log_amm con oggetto conservazione
                CURSOR systemIdLogs
                IS
                    SELECT SYSTEM_ID
                    FROM DPA_ANAGRAFICA_LOG
                    WHERE UPPER(VAR_OGGETTO) = UPPER('conservazione')
                    --AND VAR_CODICE like 'AMM_%'
                    AND (ID_AMM = idAmm.SYSTEM_ID OR ID_AMM IS NULL);
            
            BEGIN
            
            returnValue := 0;
            
            -- Per ogni id dei Log_Amm con oggetto conservazione in anagraficaLog, inserisco i log attivati
            FOR sysId IN systemIdLogs
            LOOP
                BEGIN
                    -- Così non Funziona
                    --if not exists(select system_id_anagrafica from DPA_LOG_ATTIVATI where system_id_anagrafica = systemId)
                    --INSERT INTO DPA_LOG_ATTIVATI
                    --(system_id_anagrafica, id_amm, notify)
                    --VALUES
                    --(systemId, idAmm, 'NN');
                    
                    BEGIN
                        SELECT count(system_id_anagrafica)
                        INTO   SysIdAnag
                        FROM   DPA_LOG_ATTIVATI
                        WHERE  system_id_anagrafica = sysId.SYSTEM_ID
                        AND ID_AMM = idAmm.SYSTEM_ID;

                        IF (SysIdAnag = 0)
                        THEN
                        INSERT INTO DPA_LOG_ATTIVATI
                        (system_id_anagrafica, id_amm, notify)
                        VALUES
                        (sysId.SYSTEM_ID, idAmm.SYSTEM_ID, 'NN');
                        COMMIT;
                        
                        END IF;
                        --exception
                        --when DUP_VAL_ON_INDEX
                        --then ROLLBACK;
                    END;
                    
                EXCEPTION
                    WHEN OTHERS
                    THEN
                    returnValue := 1;
                    RETURN;
                END;
            END LOOP;
            
            END;
         
         END;
         
      END LOOP;
    
    END;
END ATTIVA_LOG_CONSERVAZIONE; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE       sp_insert_area_esib (
   p_idamm                   NUMBER,
   p_idesibizione             NUMBER,
   p_idpeople                NUMBER,
   p_idprofile               NUMBER,
   p_idproject               NUMBER,
   p_codfasc                 VARCHAR,
   p_oggetto                 VARCHAR,
   p_tipodoc                 CHAR,
   p_idgruppo                NUMBER,
   p_idregistro              NUMBER,
   p_docnumber               NUMBER,
   p_userid                  VARCHAR,
   p_tipooggetto             CHAR,
   p_tipoatto                VARCHAR,
   p_idconservazione         NUMBER,
   p_result            OUT   NUMBER
)
IS
   idruoloinuo              NUMBER := 0;
   id_area_esibizione       NUMBER := 0;
   id_items_esibizione      NUMBER := 0;
   res                      NUMBER := 0;
BEGIN
   BEGIN
    --Recupero sequence di DPA_AREA_ESIBIZIONE
      SELECT seq_dpa_area_esibizione.NEXTVAL
        INTO id_area_esibizione
        FROM DUAL;
        
    --Recupero sequence di DPA_ITEMS_ESIBIZIONE
      SELECT seq_dpa_items_esibizione.NEXTVAL
        INTO id_items_esibizione
        FROM DUAL;
    
    --Recupero idRuoloInUo dalla DPA_CORR_GLOBALI
      SELECT dpa_corr_globali.system_id
        INTO idruoloinuo
        FROM dpa_corr_globali
       WHERE dpa_corr_globali.id_gruppo = p_idgruppo;

      IF (p_idesibizione IS NULL)
      THEN
         BEGIN
            -- Reperimento di una istanza di esibizione corrente in stato Nuovo per la coppia id_people e id_ruolo_in_uo
            SELECT sys INTO res 
            FROM (
                    SELECT dpa_area_esibizione.system_id as sys
                    FROM dpa_area_esibizione
                    WHERE dpa_area_esibizione.id_people = p_idpeople
                    AND dpa_area_esibizione.id_ruolo_in_uo = idruoloinuo
                    AND dpa_area_esibizione.cha_stato = 'N' 
                    order by dpa_area_esibizione.system_id desc
                ) 
            WHERE rownum=1;
            
         EXCEPTION
            WHEN OTHERS
            THEN
               res := 0;
         END;
      ELSE
         BEGIN
            res := p_idesibizione;
         END;
      END IF;

      IF (res > 0)
      THEN
         INSERT INTO dpa_items_esibizione
                     (system_id, id_esibizione, id_profile, id_project,
                      cha_tipo_doc, var_oggetto, id_registro, data_ins,
                      cha_stato, var_xml_metadati, cod_fasc, docnumber,
                      cha_tipo_oggetto, var_tipo_atto, id_conservazione 
                     )
              VALUES (id_items_esibizione, res, p_idprofile, p_idproject,
                      p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                      'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                      p_tipooggetto, p_tipoatto, p_idconservazione
                     );

         p_result := id_items_esibizione;
      ELSE
         INSERT INTO dpa_area_esibizione
                     (system_id, id_amm, id_people, id_ruolo_in_uo,
                      cha_stato, data_creazione
                     )
              VALUES (id_area_esibizione, p_idamm, p_idpeople, idruoloinuo,
                      'N', SYSDATE
                     );

         INSERT INTO dpa_items_esibizione
                     (system_id, id_esibizione, id_profile, id_project,
                      cha_tipo_doc, var_oggetto, id_registro, data_ins,
                      cha_stato, var_xml_metadati, cod_fasc, docnumber,
                      cha_tipo_oggetto, var_tipo_atto, id_conservazione
                     )
              VALUES (id_items_esibizione, id_area_esibizione, p_idprofile, p_idproject,
                      p_tipodoc, p_oggetto, p_idregistro, SYSDATE,
                      'N', EMPTY_CLOB (), p_codfasc, p_docnumber,
                      p_tipooggetto, p_tipoatto, p_idconservazione
                     );

         p_result := id_items_esibizione;
      END IF;
   EXCEPTION
      WHEN OTHERS
      THEN
         p_result := -1;
   END;
END; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          GETCODRUBRICAIDPAREMT (idUO INT)
RETURN VARCHAR IS risultato VARCHAR(256);

BEGIN
if(idUO is null)
then risultato:=' ';
else
begin
risultato := ' ';
SELECT VAR_COD_RUBRICA into risultato FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID=idUO;
exception  WHEN OTHers then risultato:='';
end;
end if;
RETURN risultato;

END GETCODRUBRICAIDPAREMT; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          getcodRubricaRuolo(sys number)
RETURN varchar IS tmpVar varchar(256);

BEGIN
BEGIN
tmpVar := '';
SELECT
UPPER(VAR_COD_RUBRICA) into tmpVar
FROM DPA_CORR_GLOBALI
WHERE id_gruppo=sys;


EXCEPTION
WHEN OTHERS THEN
null;
END;
RETURN tmpVar;

END getcodRubricaRuolo; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getCodRuoloRif(system_id number)
RETURN VARCHAR2 IS risultato VARCHAR2(50);

BEGIN
begin


select id
INTO risultato from (
SELECT groups_system_id as id
 FROM peoplegroups gp WHERE gp.people_system_id = system_id and cha_preferito = '1'
) where rownum=1;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END getCodRuoloRif; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getinscarto (
   idprofile   NUMBER,
   idproject   NUMBER,
   idpeople    NUMBER,
   idgruppo    NUMBER
)
   RETURN INT
IS
   risultato     INT;
   res_appo      INT;
   idruoloinuo   NUMBER;
BEGIN
   BEGIN
      SELECT dpa_corr_globali.system_id
        INTO idruoloinuo
        FROM dpa_corr_globali
       WHERE dpa_corr_globali.id_gruppo = idgruppo;

      SELECT COUNT (dpa_items_scarto.system_id)
        INTO risultato
        FROM dpa_area_scarto, dpa_items_scarto
       WHERE dpa_items_scarto.id_scarto = dpa_area_scarto.system_id
         AND id_project = idproject
         AND dpa_items_scarto.cha_stato = 'N'
         AND dpa_area_scarto.id_people = idpeople
         AND dpa_area_scarto.id_ruolo_in_uo = idruoloinuo;

      IF (risultato > 0)
      THEN
         risultato := 1;
      ELSE
         risultato := 0;
      END IF;
   EXCEPTION
      WHEN NO_DATA_FOUND
      THEN
         risultato := 0;
      WHEN OTHERS
      THEN
         risultato := 0;
   END;

   RETURN risultato;
END getinscarto; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          getnumlivRuolo (idRuolo number)
RETURN number IS risultato number;

BEGIN
if(idRuolo is null)
then risultato:=0;
else
begin
risultato := 0;
SELECT num_livello into risultato FROM DPA_tipo_ruolo WHERE SYSTEM_ID=(select id_tipo_ruolo from dpa_corr_globali where system_id=idruolo);
exception  WHEN OTHers then risultato:=0;
end;
end if;
RETURN risultato;

END getnumlivRuolo; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          getPeopleNamebyUserid (userid varchar)
RETURN varchar IS risultato varchar(256);
BEGIN

select full_name into risultato from people where upper(user_id) = upper(userid);

RETURN risultato;
END getPeopleNamebyUserid; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getsemittdest (idcorr INT, idprofile NUMBER)
   RETURN INT
IS
   risultato   INT;
BEGIN
   SELECT /*+index (DPA_DOC_ARRIVO_PAR)*/
             COUNT (system_id)
        INTO risultato
        FROM dpa_doc_arrivo_par
       Where Id_Mitt_Dest = Idcorr And Id_Profile = Idprofile;
   RETURN risultato;
   EXCEPTION
      WHEN OTHERS
      THEN
         Risultato := 0;
         RETURN risultato;
      
END getsemittdest;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getSeSoloMittDestOcc (docId INT)
RETURN int IS risultato int;
BEGIN
SELECT count(dap.SYSTEM_ID) into risultato
FROM DPA_CORR_GLOBALI c , DPA_DOC_ARRIVO_PAR dap
WHERE dap.id_profile=docId
AND dap.id_mitt_dest=c.system_id and c.cha_tipo_ie in ('I','E')
;
if(risultato>0)
then risultato :=0;
else risultato:=1;
end if;
RETURN risultato;
END getSeSoloMittDestOcc; 

/

SHOW ERRORS;

CREATE OR REPLACE Procedure Blocca_Da_Conv_Pdf Is

BEGIN
declare
ext  char;
idProfile number;
tmpVar number;
cursor docConv is select dc.DTA_CONVERSIONE,dc.ID_PROFILE from dpa_conv_pdf_server dc ;
dc docConv%rowtype;

begin

open docConv;



LOOP
fetch docConv into dc;
EXIT WHEN docConv%NOTFOUND;

begin
   tmpVar := 0;
   select id_profile into tmpvar from dpa_conv_pdf_server where id_profile=dc.id_Profile and  
dc.DTA_CONVERSIONE<sysdate-1/24;

   EXCEPTION
       WHEN OTHERS THEN tmpvar:=0;
end;

if(tmpvar<>0)
then
begin
select getChaImConvPDF(tmpvar) into ext from dual;

if(ext is not  null and upper(ext) ='1' )
then
delete from   dpa_conv_pdf_server where id_profile=dc.id_Profile;
delete from   dpa_checkin_checkout d where d.ID_DOCUMENT=dc.id_Profile;
commit;
end if;


   
   end; 
   end if;
      
   
end loop;

close docConv;

end;

    
END blocca_da_conv_pdf;

/

SHOW ERRORS;


 ------------------------ FIN QUI  2 ---------------
 
ALTER TABLE DPA_AREA_CONSERVAZIONE
 ADD (ESITO_VERIFICA  NUMBER);

ALTER TABLE DPA_AREA_CONSERVAZIONE
 ADD (ID_ISTANZA_RIGENERATA  NUMBER);

ALTER TABLE SECURITY
 ADD (CHA_COPIA_VISIBILITA  CHAR(1 CHAR)        ); --    DEFAULT 0);
 
 
 update security set CHA_COPIA_VISIBILITA = '0';

CREATE OR REPLACE FUNCTION                getInConsNoSecPerEsib(IDPROFILE number,Idproject number, typeID char)
RETURN VARCHAR IS result VARCHAR(3000);

BEGIN

--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)
if(typeID = 'D') then
    BEGIN
        declare item varchar(3000);
        CURSOR curCasellaDiSelezione IS select ID_CONSERVAZIONE into result from DPA_ITEMS_CONSERVAZIONE,DPA_AREA_CONSERVAZIONE where DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE=DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROFILE = IDPROFILE AND DPA_AREA_CONSERVAZIONE.CHA_STATO in ('C', 'V') AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N'; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'-'||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
    elsif(typeID = 'F') then
     BEGIN
        declare item varchar(3000);
        CURSOR curCasellaDiSelezione IS select ID_CONSERVAZIONE into result from DPA_AREA_CONSERVAZIONE, DPA_ITEMS_CONSERVAZIONE WHERE DPA_ITEMS_CONSERVAZIONE.ID_CONSERVAZIONE=DPA_AREA_CONSERVAZIONE.SYSTEM_ID AND ID_PROJECT = Idproject AND DPA_AREA_CONSERVAZIONE.CHA_STATO in ('C','V') AND DPA_ITEMS_CONSERVAZIONE.CHA_STATO != 'N' group by ID_CONSERVAZIONE;
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'-'||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
end if;

RETURN result;

exception
when no_data_found
then
result := null; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END getInConsNoSecPerEsib; 

/

SHOW ERRORS;


CREATE OR REPLACE FUNCTION             getFolderGEn (projectId INT)
RETURN number IS risultato number;
BEGIN
select * into risultato from (SELECT A.system_id FROM PROJECT A WHERE A.id_fascicolo=projectId and a.ID_FASCICOLO=a.ID_PARENT and a.CHA_TIPO_PROJ='C'  )
where rownum=1 ;
RETURN risultato;
END getFolderGEn; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE             sp_dpa_setlargetext (
   sysid                 NUMBER,
   var_filtriric         VARCHAR2,
   returnvalue     OUT   NUMBER
)
IS
   tmpvar   NUMBER;
   tmpClob  CLOB;
BEGIN
   tmpvar := 0;
   
   select var_filtri_ric into tmpClob
   from dpa_salva_ricerche where system_id = sysid for update;
    
    DBMS_LOB.WRITE(tmpClob, length(var_filtriric),1,var_filtriric);
    
   --UPDATE dpa_salva_ricerche f
     -- SET f.var_filtri_ric = var_filtriric
 --   WHERE system_id = sysid;

   returnvalue := 0;
EXCEPTION
   WHEN NO_DATA_FOUND
   THEN
     RAISE;
   WHEN OTHERS
   THEN
      -- Consider logging the error and then re-raise
    
     RAISE;
      
COMMIT;
      
END sp_dpa_setlargetext; 

/

SHOW ERRORS;

/*
CREATE OR REPLACE TRIGGER UTL_TRACK_DML_ON_LOG_INSTALL before update or delete On DPA_LOG_INSTALL
DISABLE
Declare
Nomeutente      Varchar2(32);
Data_Eseguito   Date;
Comando_Eseguito Varchar2(2000);
Versione_Cd     Varchar2(200);
Esito           Varchar2(200);
trg_status      Varchar2(200);
mytrg_name      Varchar2(200);
begin
 comando_Eseguito := 'someone has tampered table DPA_LOG_INSTALL! ';
 select username into Nomeutente  from user_users;
      
      Utl_Insert_Log (Nomeutente       ,sysDate
      , Comando_Eseguito       , 'n.d.' --Versione_Cd Varchar2
      , 'ok'); --esito VARCHAR2
end;
/
SHOW ERRORS;

*/

CREATE TABLE DPA_ITEMS_SCARTO
(
  SYSTEM_ID     INTEGER                         NOT NULL,
  ID_SCARTO     INTEGER,
  ID_PROFILE    INTEGER,
  ID_PROJECT    INTEGER,
  CHA_TIPO_DOC  VARCHAR2(10 BYTE),
  VAR_OGGETTO   VARCHAR2(1000 BYTE),
  ID_REGISTRO   INTEGER,
  DATA_INS      DATE,
  CHA_STATO     VARCHAR2(10 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE SEQUENCE SEQ_DPA_AREA_ESIBIZIONE
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_DPA_VERIFICA_FORMATI_CONS
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_ER
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  NOCACHE
  NOORDER;



CREATE SEQUENCE SEQ_INST_ACC_ATT
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_INST_ACC_DOC
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_LANGUAGE
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_NOTIFICATIONINSTANCES
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE SEQUENCE SEQ_PUBLISHER
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  NOCACHE
  NOORDER;

CREATE SEQUENCE SEQ_SUBSCRIBER
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 0
  NOCYCLE
  NOCACHE
  NOORDER;

CREATE SEQUENCE SEQ_UT
  START WITH 1
  MAXVALUE 999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  NOCACHE
  NOORDER;

CREATE TABLE DPA_STATI_SERVIZI
(
  ID_SERVIZIO  NUMBER(10)                       NOT NULL,
  ID_STATO     NUMBER(10)                       NOT NULL
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_FATTURA
(
  IDAMMINISTRAZIONE     VARCHAR2(100 BYTE),
  TEMPLATEXML           VARCHAR2(4000 BYTE),
  FORMATO_TRASMISSIONE  VARCHAR2(255 BYTE),
  ESIGIBILITA_IVA       VARCHAR2(50 BYTE),
  CONDIZIONI_PAGAMENTO  VARCHAR2(255 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_SERVIZI_ESTERNI
(
  SYSTEM_ID    NUMBER(10)                       NOT NULL,
  DESCRIZIONE  VARCHAR2(255 BYTE),
  SERVIZIO     VARCHAR2(500 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_PARAMETRI_SERVIZIO
(
  ID_SERVIZIO   NUMBER(10)                      NOT NULL,
  ID_PARAMETRO  NUMBER(10)                      NOT NULL
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_PARAMETRO_SERVIZI
(
  SYSTEM_ID    NUMBER(10)                       NOT NULL,
  DESCRIZIONE  VARCHAR2(255 BYTE)               NOT NULL,
  TIPO_VALORE  VARCHAR2(255 BYTE),
  POSIZIONE    NUMBER                           DEFAULT 1
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_PROFILE_FATTURA
(
  DOCNUMBER  VARCHAR2(100 BYTE)                 NOT NULL,
  IDSDI      VARCHAR2(100 BYTE)                 NOT NULL,
  DIAGRAMMA  VARCHAR2(100 BYTE)                 NOT NULL
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE OUTPUT
(
  RESULT  CLOB
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE MV_PROSPETTI_DOCGRIGICLASS
(
  ID_AMM              NUMBER(10),
  ID_REGISTRO         NUMBER(10),
  ID_TITOLARIO        NUMBER,
  GRIGIO_MONTH        DATE,
  VAR_SEDE            VARCHAR2(64 BYTE),
  NVL_CHA_IN_CESTINO  VARCHAR2(1 BYTE),
  DISTINCT_COUNT      NUMBER,
  UNDISTINCT_COUNT    NUMBER
)
NOLOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE MV_PROSPETTI_DOCUMENTI
(
  ID_REGISTRO_0FORNULL     NUMBER,
  CREATION_YEAR            DATE,
  PROTO_MONTH              DATE,
  NUM_ANNO_PROTO           NUMBER(10),
  ID_UO_PROT               NUMBER,
  CHA_DA_PROTO             VARCHAR2(1 BYTE),
  CHA_IN_CESTINO_0FORNULL  VARCHAR2(1 BYTE),
  VAR_SEDE                 VARCHAR2(64 BYTE),
  CHA_TIPO_PROTO           VARCHAR2(1 BYTE),
  FLAG_IMMAGINE            VARCHAR2(4000 BYTE),
  FLAG_ANNULLATO           NUMBER,
  FLAG_NUM_PROTO           NUMBER,
  UNDISTINCT_COUNT         NUMBER
)
NOLOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE MV_PROSPETTI_DOCUMENTICLASS
(
  ID_AMM              NUMBER(10),
  ID_TITOLARIO        NUMBER,
  ID_REGISTRO         NUMBER(10),
  PROTO_MONTH         DATE,
  CREATION_MONTH      DATE,
  VAR_SEDE            VARCHAR2(64 BYTE),
  NVL_CHA_IN_CESTINO  VARCHAR2(1 BYTE),
  CHA_TIPO_PROTO      VARCHAR2(1 BYTE),
  FLAG_ANNULLATO      NUMBER,
  FLAG_NUM_PROTO      NUMBER,
  FLAG_DTA_PROTO      NUMBER,
  DISTINCT_COUNT      NUMBER,
  NOT_DISTINCT_COUNT  NUMBER
)
NOLOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE MV_PROSPETTI_DOCUMENTICLASSUO
(
  ID_REGISTRO         NUMBER(10),
  ID_UO_PROT          NUMBER,
  CHA_TIPO_PROTO      VARCHAR2(1 BYTE),
  NUM_ANNO_PROTO      NUMBER(10),
  NVL_CHA_IN_CESTINO  VARCHAR2(1 BYTE),
  FLAG_ANNULLATO      NUMBER,
  UNDISTINCT_COUNT    NUMBER,
  DISTINCT_COUNT      NUMBER
)
NOLOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE OR REPLACE FUNCTION             codrfappartenza (idpeople INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (128);
BEGIN
   BEGIN
      SELECT var_codice
        INTO risultato
        FROM dpa_el_registri a
       WHERE cha_rf = '1'
         AND EXISTS (
                SELECT 'x'
                  FROM dpa_l_ruolo_reg t
                 WHERE t.id_registro = a.system_id
                   AND EXISTS (
                          SELECT 'x'
                            FROM dpa_corr_globali cg
                           WHERE cg.system_id = t.id_ruolo_in_uo
                             AND EXISTS (
                                    SELECT 'x'
                                      FROM peoplegroups pg
                                     WHERE cg.id_gruppo = pg.groups_system_id
                                       AND pg.people_system_id = idpeople)));
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenza; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE          libera_acl_doc_trasmessi IS
tmpVar NUMBER;
/******************************************************************************
   NAME:       libera_acl_doc_trasmessi
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        14/07/2009          1. Created this procedure.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     libera_acl_doc_trasmessi
      Sysdate:         14/07/2009
      Date and Time:   14/07/2009, 13.50.07, and 14/07/2009 13.50.07
      Username:         (set in TOAD Options, Procedure Editor)
      Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/
begin
declare
cursor dt_r is select s.thing thing,v.dta_accettata,v.SYSTEM_ID_TU,v.SYSTEM_ID_TX,s.PERSONORGROUP,v.ID_RAGIONE from security s,v_trasmissione v where v.DTA_ACCETTATA is not null 
and v.ID_PROFILE=s.thing and s.ACCESSRIGHTS=20
and ( v.ID_PEOPLE_DEST = s.PERSONORGROUP OR v.ID_CORR_GLOBALE_DEST=(select system_id from dpa_corr_globali d where d.ID_GRUPPO=s.PERSONORGROUP));

cursor dt_p is
select s.thing thing,v.dta_accettata,v.SYSTEM_ID_TU,v.SYSTEM_ID_TX,s.PERSONORGROUP,v.ID_RAGIONE from security s,v_trasmissione v where v.DTA_ACCETTATA is not null 
and v.ID_PROFILE=s.thing and s.ACCESSRIGHTS=20
and ( v.ID_PEOPLE_DEST = s.PERSONORGROUP) ;
tipodiritti char;
diritti number;

BEGIN
   tmpVar := 0;
   

   
   FOR currentdt in dt_r
   loop
 begin
    select cha_tipo_diritti into tipodiritti  from dpa_ragione_trasm t where t.SYSTEM_ID= currentdt.id_ragione;
 if (tipodiritti is not null and tipodiritti ='W')
 then 
    diritti:=63;
    else if(tipodiritti is not null and tipodiritti ='R')
    then diritti:=45;
    end if;
 end if;
execute immediate 'update security set accessrights='||diritti||' where accessrights=20 and thing='||currentdt.thing||' and personorgroup='||currentdt.PERSONORGROUP;
   
   EXCEPTION
       WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
      null;
      end ;
     
      end loop;
      
      
       FOR currentdt in dt_p
   loop
 begin
    select cha_tipo_diritti into tipodiritti  from dpa_ragione_trasm t where t.SYSTEM_ID= currentdt.id_ragione;
 if (tipodiritti is not null and tipodiritti ='W')
 then 
    diritti:=63;
    else if(tipodiritti is not null and tipodiritti ='R')
    then diritti:=45;
    end if;
 end if;
execute immediate 'update security set accessrights='||diritti||' where accessrights=20 and thing='||currentdt.thing||' and personorgroup='||currentdt.PERSONORGROUP;
   
   EXCEPTION
       WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
      null;
      end ;
     
      end loop;
      
      
      end;
END libera_acl_doc_trasmessi; 

/

SHOW ERRORS;


CREATE OR REPLACE FUNCTION          getgiacenzaddtodolist (
   docid        INT,
   chatiporag   CHAR
)
   RETURN VARCHAR
IS
   risultato   VARCHAR (2000);
BEGIN
   DECLARE
      maxdtaa   DATE;
      maxdtar   DATE;
      maxDtaV date;
   BEGIN
      IF (chatiporag = 'W')
      THEN
         BEGIN
            SELECT NVL (MAX (dta_accettata),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       ),
                   NVL (MAX (dta_rifiutata),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaa,
                   maxdtar
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;
             SELECT NVL (MAX (dta_invio),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaV
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;

            IF (maxdtaa > maxdtar)
            THEN
               SELECT NUMTODSINTERVAL (maxdtaa - maxdtaV, 'DAY')
                 INTO risultato
                 FROM dual;
            ELSE
               SELECT NUMTODSINTERVAL (maxdtar - maxdtaV, 'DAY')
                 INTO risultato
                 FROM dual;
                
            END IF;
         END;
      ELSE
         BEGIN
            SELECT NVL (MAX (dta_vista),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaa
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;
             SELECT NVL (MAX (dta_invio),
                        TO_DATE ('01/01/1753 00:00:00',
                                 'dd/mm/yyyy HH24:mi:ss'
                                )
                       )
              INTO maxdtaV
              FROM v_trasmissione vt
             WHERE vt.id_profile = docid;

            SELECT NUMTODSINTERVAL (maxdtaa - maxDtaV, 'DAY')
              INTO risultato
            FROM dual;
         END;
      END IF;
   END;

   RETURN risultato;
END getgiacenzaddtodolist; 

/

SHOW ERRORS;



CREATE OR REPLACE FORCE VIEW V_TRASMISSIONE
(SYSTEM_ID_TX, ID_RUOLO_IN_UO_MITT, ID_PEOPLE_MITT, CHA_TIPO_OGGETTO, ID_PROFILE, 
 ID_PROJECT, DTA_INVIO, VAR_NOTE_GENERALI, CHA_CESSIONE, CHA_SALVATA_CON_CESSIONE, 
 SYSTEM_ID_TS, ID_RAGIONE, ID_TRASMISSIONE, CHA_TIPO_DEST, ID_CORR_GLOBALE_DEST, 
 VAR_NOTE_SING, CHA_TIPO_TRASM, DTA_SCADENZA, ID_TRASM_UTENTE, SYSTEM_ID_TU, 
 ID_TRASM_SINGOLA, ID_PEOPLE_DEST, DTA_VISTA, DTA_ACCETTATA, DTA_RIFIUTATA, 
 DTA_RISPOSTA, CHA_VISTA, CHA_ACCETTATA, CHA_RIFIUTATA, VAR_NOTE_ACC, 
 VAR_NOTE_RIF, CHA_VALIDA, ID_TRASM_RISP_SING, CHA_IN_TODOLIST, DTA_RIMOZIONE_TODOLIST, 
 SYSTEM_ID_TR, VAR_DESC_RAGIONE, CHA_TIPO_RAGIONE, CHA_VIS, CHA_TIPO_DIRITTI, 
 CHA_TIPO_DEST_TR, CHA_RISPOSTA, VAR_NOTE, CHA_EREDITA, ID_AMM, 
 CHA_TIPO_RISPOSTA, VAR_NOTIFICA_TRASM, VAR_TESTO_MSG_NOTIFICA_DOC, VAR_TESTO_MSG_NOTIFICA_FASC, CHA_CEDE_DIRITTI, 
 CHA_RAG_SISTEMA)
AS 
SELECT tx."SYSTEM_ID", tx."ID_RUOLO_IN_UO", tx."ID_PEOPLE",
          tx."CHA_TIPO_OGGETTO", tx."ID_PROFILE", tx."ID_PROJECT",
          tx."DTA_INVIO", tx."VAR_NOTE_GENERALI", tx."CHA_CESSIONE",
          tx."CHA_SALVATA_CON_CESSIONE", ts."SYSTEM_ID", ts."ID_RAGIONE",
          ts."ID_TRASMISSIONE", ts."CHA_TIPO_DEST", ts."ID_CORR_GLOBALE",
          ts."VAR_NOTE_SING", ts."CHA_TIPO_TRASM", ts."DTA_SCADENZA",
          ts."ID_TRASM_UTENTE", tu."SYSTEM_ID", tu."ID_TRASM_SINGOLA",
          tu."ID_PEOPLE", tu."DTA_VISTA", tu."DTA_ACCETTATA",
          tu."DTA_RIFIUTATA", tu."DTA_RISPOSTA", tu."CHA_VISTA",
          tu."CHA_ACCETTATA", tu."CHA_RIFIUTATA", tu."VAR_NOTE_ACC",
          tu."VAR_NOTE_RIF", tu."CHA_VALIDA", tu."ID_TRASM_RISP_SING",
          tu."CHA_IN_TODOLIST", tu."DTA_RIMOZIONE_TODOLIST", tr."SYSTEM_ID",
          tr."VAR_DESC_RAGIONE", tr."CHA_TIPO_RAGIONE", tr."CHA_VIS",
          tr."CHA_TIPO_DIRITTI", tr."CHA_TIPO_DEST", tr."CHA_RISPOSTA",
          tr."VAR_NOTE", tr."CHA_EREDITA", tr."ID_AMM",
          tr."CHA_TIPO_RISPOSTA", tr."VAR_NOTIFICA_TRASM",
          tr."VAR_TESTO_MSG_NOTIFICA_DOC", tr."VAR_TESTO_MSG_NOTIFICA_FASC",
          tr."CHA_CEDE_DIRITTI", tr."CHA_RAG_SISTEMA"
     FROM dpa_trasmissione tx,
          dpa_trasm_singola ts,
          dpa_trasm_utente tu,
          dpa_ragione_trasm tr
    WHERE tx.system_id = ts.id_trasmissione
      AND ts.system_id = tu.id_trasm_singola
      AND ts.id_ragione = tr.system_id;


CREATE OR REPLACE FUNCTION dateDiff( p_dt1 IN varchar2,
                                     p_dt2 IN varchar2 )
 
RETURN NUMBER IS
 
GG_LAV NUMBER;
 
BEGIN
 
DECLARE
 
dt1 DATE; -- corrisponde alla data di apwertura
dt2 DATE; -- corrisponde alla data di chiusura
 
  
BEGIN
 
IF (p_dt1 = '' OR p_dt1 IS NULL)
THEN
return 0;
end if;
 
dt1 := TO_DATE(p_dt1,'DD/MM/YYYY');
 
IF (p_dt2 = '' OR p_dt2 IS NULL)
    THEN
        dt2 := TRUNC(SYSDATE);
 
    ELSE
    dt2 := TO_DATE(p_dt2,'DD/MM/YYYY');
 
end if;
 
SELECT dt2 - dt1 into GG_LAV FROM DUAL;
 
return GG_LAV;
 
END;
END; 

/

SHOW ERRORS;

ALTER TABLE DPA_OGG_CUSTOM_COMP
 ADD (CAMPO_XML_ASSOC  VARCHAR2(255 BYTE));

CREATE TABLE SEC_LOG
(
  THING                 NUMBER(10),
  PERSONORGROUP         NUMBER(10),
  ACCESSRIGHTS          NUMBER(10),
  ID_GRUPPO_TRASM       NUMBER(10),
  CHA_TIPO_DIRITTO      VARCHAR2(1 BYTE),
  HIDE_DOC_VERSIONS     CHAR(1 BYTE),
  TS_INSERIMENTO        TIMESTAMP(6),
  VAR_NOTE_SEC          VARCHAR2(255 BYTE),
  CHA_COPIA_VISIBILITA  CHAR(1 CHAR)            DEFAULT 0
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE UTENTI
(
  ID                  INTEGER                   NOT NULL,
  NOME                NVARCHAR2(50)             NOT NULL,
  PASSWORD            NVARCHAR2(50)             NOT NULL,
  DATACREAZIONE       DATE                      NOT NULL,
  DATAULTIMAMODIFICA  DATE                      NOT NULL,
  AMMINISTRATORE      CHAR(1 BYTE)              NOT NULL
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE UTENTI_LOGGATI
(
  TOT_UTENTI  NUMBER,
  DATATIME    DATE
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE DPA_EVENT_AUTOMATIC_STATE   ---??? serve 
(
  SYSTEM_ID   NUMBER                            NOT NULL,
  VAR_CODICE  VARCHAR2(200 BYTE),
  VAR_DESC    VARCHAR2(400 BYTE)
)
LOGGING 
COMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

CREATE TABLE AMMINISTRAZIONI
(
  ID               NUMBER                       NOT NULL,
  AMMINISTRAZIONE  VARCHAR2(255 BYTE)           NOT NULL,
  URL              VARCHAR2(4000 BYTE)
)
LOGGING 
NOCOMPRESS 
NOCACHE
NOPARALLEL
MONITORING;

ALTER TABLE DELETED_SECURITY
 ADD (CHA_COPIA_VISIBILITA  CHAR(1 CHAR)            DEFAULT 0);



CREATE OR REPLACE FUNCTION                getCodeProject(idParent number) RETURN varchar2 IS
tmpVar varchar2(32);
BEGIN
begin
select upper(var_codice) into tmpvar from project where system_id=idParent;
if(tmpvar is null)
then
select upper(var_codice) into tmpvar from project where cha_tipo_proj='F' connect by prior id_parent = system_id start with system_id=idParent and rownum = 1;
end if;
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:='';

end;
RETURN tmpVar;
END getCodeProject; 

/

SHOW ERRORS;

CREATE OR REPLACE procedure             VIS_FASC_ANOMALA_ID_PROJECT (p_id_amm NUMBER, p_id_project in NUMBER, p_codice_atipicita out VARCHAR) IS

--DICHIARAZIONI
s_idg_security NUMBER;
s_ar_security NUMBER;
s_td_security VARCHAR(2);
s_vn_security VARCHAR(255);
s_idg_r_sup NUMBER;
n_id_gruppo NUMBER;

BEGIN

--Cursore sulla security per lo specifico fascicolo
DECLARE CURSOR c_idg_security IS 
SELECT personorgroup, accessrights, cha_tipo_diritto, var_note_sec 
FROM security 
WHERE 
thing = p_id_project
AND
accessrights > 20;  
BEGIN OPEN c_idg_security;
LOOP FETCH c_idg_security INTO s_idg_security, s_ar_security, s_td_security, s_vn_security;
EXIT WHEN c_idg_security%NOTFOUND;

    --Gerachia ruolo proprietario del fascicolo
    IF(upper(s_td_security) = 'P') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('FASCICOLO : ' || p_id_project || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita,'AGRP'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGRP-';
            END IF;
        END;
        COMMIT; 
    END IF;

    
    --Gerarchia destinatario trasmissione
    IF(upper(s_td_security) = 'T') THEN
        DECLARE CURSOR ruoli_sup IS
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
                        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;                   
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || p_id_project || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);          
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGDT'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGDT-';
            END IF;
        END;
        COMMIT; 
    END IF;


    --Gerarchia ruolo destinatario di copia visibilit
    IF(upper(s_td_security) = 'A' AND upper(s_vn_security) = 'ACQUISITO PER COPIA VISIBILITA') THEN
        DECLARE CURSOR ruoli_sup IS 
        SELECT dpa_corr_globali.id_gruppo 
        FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id 
        WHERE
            dpa_corr_globali.id_uo in (
                SELECT dpa_corr_globali.system_id
                FROM dpa_corr_globali
                WHERE 
                dpa_corr_globali.dta_fine IS NULL
                CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id START WITH dpa_corr_globali.system_id = (SELECT dpa_corr_globali.id_uo FROM dpa_corr_globali WHERE dpa_corr_globali.id_gruppo = s_idg_security)
                )
        AND
        dpa_corr_globali.CHA_TIPO_URP = 'R'
        AND
        dpa_corr_globali.ID_AMM = p_id_amm
        AND
        dpa_corr_globali.DTA_FINE IS NULL
        AND 
        dpa_tipo_ruolo.num_livello < (SELECT dpa_tipo_ruolo.num_livello FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id WHERE dpa_corr_globali.id_gruppo = s_idg_security);
        
        BEGIN OPEN ruoli_sup;
        LOOP FETCH ruoli_sup INTO s_idg_r_sup;
        EXIT WHEN ruoli_sup%NOTFOUND;
            --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || p_id_project || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
            INSERT INTO DPA_VIS_ANOMALA (ID_GRUPPO) VALUES(s_idg_r_sup);   
        END LOOP;
        CLOSE ruoli_sup;
        END;
        --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
        --Se si ottiene un insieme vuoto, vuol dire che la visibilit non ha anomalie
        BEGIN
            n_id_gruppo := 0;
            SELECT COUNT(*) INTO n_id_gruppo FROM
            (
            SELECT ID_GRUPPO FROM DPA_VIS_ANOMALA
            MINUS
            SELECT PERSONORGROUP FROM SECURITY WHERE THING = p_id_project
            );
            IF(n_id_gruppo <> 0 AND nvl(instr(p_codice_atipicita, 'AGCV'), 0) = 0) THEN
                p_codice_atipicita := p_codice_atipicita || 'AGCV-';
            END IF;
        END;
        COMMIT;
    END IF;    


END LOOP;
CLOSE c_idg_security;
END; 

--Restituzione codice di atipicit
IF(p_codice_atipicita is null) THEN
    p_codice_atipicita := 'T';
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || p_id_project || ' - ' || p_codice_atipicita);
    update PROJECT set CHA_COD_T_A = p_codice_atipicita where SYSTEM_ID = p_id_project;
    COMMIT;
    RETURN;       
END IF;

IF(substr(p_codice_atipicita, length(p_codice_atipicita)) = '-') THEN
    p_codice_atipicita := substr(p_codice_atipicita, 0, length(p_codice_atipicita)-1);
    --DBMS_OUTPUT.PUT_LINE('Codici Atipicit Fascicolo ' || p_id_project || ' - ' || p_codice_atipicita);
    update PROJECT set CHA_COD_T_A = p_codice_atipicita where SYSTEM_ID = p_id_project;
    COMMIT;
    RETURN;       
END IF;

EXCEPTION 
WHEN others THEN
DBMS_OUTPUT.PUT_LINE('Errore nell''esecuzione della procedura');

END; 

/

SHOW ERRORS;

CREATE OR REPLACE PROCEDURE vis_fasc_anomala_int_date (
   p_id_amm       NUMBER,
   p_start_date   VARCHAR,
   p_end_date     VARCHAR
)
IS
--DICHIARAZIONI
   s_idg_security     NUMBER;
   s_ar_security      NUMBER;
   s_td_security      VARCHAR (2);
   s_vn_security      VARCHAR (255);
   s_idg_r_sup        NUMBER;
   n_id_gruppo        NUMBER;
   s_id_fascicolo     NUMBER;
   codice_atipicita   VARCHAR (255);
BEGIN
--CURSORE FASCICOLI
   DECLARE
      CURSOR fascicoli
      IS
         SELECT system_id
           FROM project
          WHERE dta_creazione BETWEEN TO_DATE (p_start_date,
                                               'dd/mm/yyyy hh24.mi.ss'
                                              )
                                  AND TO_DATE (p_end_date,
                                               'dd/mm/yyyy hh24.mi.ss'
                                              )
            AND id_amm = p_id_amm
            AND cha_tipo_fascicolo = 'P';
   BEGIN
      OPEN fascicoli;

      LOOP
         FETCH fascicoli
          INTO s_id_fascicolo;

         EXIT WHEN fascicoli%NOTFOUND;

         --Cursore sulla security per lo specifico fascicolo
         DECLARE
            CURSOR c_idg_security
            IS
               SELECT personorgroup, accessrights, cha_tipo_diritto,
                      var_note_sec
                 FROM security
                WHERE thing = s_id_fascicolo AND accessrights > 20;
         BEGIN
            OPEN c_idg_security;

            LOOP
               FETCH c_idg_security
                INTO s_idg_security, s_ar_security, s_td_security,
                     s_vn_security;

               EXIT WHEN c_idg_security%NOTFOUND;

               --Gerachia ruolo proprietario del fascicolo
               IF (UPPER (s_td_security) = 'P')
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('FASCICOLO : ' || s_id_fascicolo || ' Ruolo gerarchicamente superiore a ruolo proprietario: ' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGRP'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGRP-';
                     END IF;
                  END;

                  COMMIT;
               END IF;

               --Gerarchia destinatario trasmissione
               IF (UPPER (s_td_security) = 'T')
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGDT'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGDT-';
                     END IF;
                  END;

                  COMMIT;
               END IF;

               --Gerarchia ruolo destinatario di copia visibilita
               IF (    UPPER (s_td_security) = 'A'
                   AND UPPER (s_vn_security) =
                                              'ACQUISITO PER COPIA VISIBILITA'
                  )
               THEN
                  DECLARE
                     CURSOR ruoli_sup
                     IS
                        SELECT dpa_corr_globali.id_gruppo
                          FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                               ON dpa_corr_globali.id_tipo_ruolo =
                                                     dpa_tipo_ruolo.system_id
                         WHERE dpa_corr_globali.id_uo IN (
                                  SELECT     dpa_corr_globali.system_id
                                        FROM dpa_corr_globali
                                       WHERE dpa_corr_globali.dta_fine IS NULL
                                  CONNECT BY PRIOR dpa_corr_globali.id_parent =
                                                   dpa_corr_globali.system_id
                                  START WITH dpa_corr_globali.system_id =
                                                (SELECT dpa_corr_globali.id_uo
                                                   FROM dpa_corr_globali
                                                  WHERE dpa_corr_globali.id_gruppo =
                                                               s_idg_security))
                           AND dpa_corr_globali.cha_tipo_urp = 'R'
                           AND dpa_corr_globali.id_amm = p_id_amm
                           AND dpa_corr_globali.dta_fine IS NULL
                           AND dpa_tipo_ruolo.num_livello <
                                  (SELECT dpa_tipo_ruolo.num_livello
                                     FROM dpa_corr_globali INNER JOIN dpa_tipo_ruolo
                                          ON dpa_corr_globali.id_tipo_ruolo =
                                                      dpa_tipo_ruolo.system_id
                                    WHERE dpa_corr_globali.id_gruppo =
                                                                s_idg_security);
                  BEGIN
                     OPEN ruoli_sup;

                     LOOP
                        FETCH ruoli_sup
                         INTO s_idg_r_sup;

                        EXIT WHEN ruoli_sup%NOTFOUND;

                        --DBMS_OUTPUT.PUT_LINE('TRASMISSIONE FASCICOLO : ' || s_id_fascicolo || ' Ruolo Gerarchicamente superiore a ruolo destinatario della trasmissione :' || s_idg_r_sup);
                        INSERT INTO dpa_vis_anomala
                                    (id_gruppo
                                    )
                             VALUES (s_idg_r_sup
                                    );
                     END LOOP;

                     CLOSE ruoli_sup;
                  END;

                  --Si effettua una sottrazione fra l'insieme degli id_gruppo ricostruiti e quelli presenti in security
                  --Se si ottiene un insieme vuoto, vuol dire che la visibilita non ha anomalie
                  BEGIN
                     n_id_gruppo := 0;

                     SELECT COUNT (*)
                       INTO n_id_gruppo
                       FROM (SELECT id_gruppo
                               FROM dpa_vis_anomala
                             MINUS
                             SELECT personorgroup
                               FROM security
                              WHERE thing = s_id_fascicolo);

                     IF (    n_id_gruppo <> 0
                         AND NVL (INSTR (codice_atipicita, 'AGCV'), 0) = 0
                        )
                     THEN
                        codice_atipicita := codice_atipicita || 'AGCV-';
                     END IF;
                  END;

                  COMMIT;
               END IF;
            END LOOP;

            CLOSE c_idg_security;
         END;

         --Restituzione codice di atipicita
         IF (codice_atipicita IS NULL)
         THEN
            codice_atipicita := 'T';

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
            UPDATE project
               SET cha_cod_t_a = codice_atipicita
             WHERE system_id = s_id_fascicolo;

            COMMIT;
            codice_atipicita := NULL;
         END IF;

         IF (SUBSTR (codice_atipicita, LENGTH (codice_atipicita)) = '-')
         THEN
            codice_atipicita :=
                   SUBSTR (codice_atipicita, 0, LENGTH (codice_atipicita) - 1);

            --DBMS_OUTPUT.PUT_LINE('Codici Atipicita Fascicolo ' || s_id_fascicolo || ' - ' || codice_atipicita);
            UPDATE project
               SET cha_cod_t_a = codice_atipicita
             WHERE system_id = s_id_fascicolo;

            COMMIT;
            codice_atipicita := NULL;
         END IF;
      END LOOP;

      CLOSE fascicoli;
   END;
EXCEPTION
   WHEN OTHERS
   THEN
      DBMS_OUTPUT.put_line ('Errore nell''esecuzione della procedura');
END; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getmaxver_meno1 (docnum NUMBER)
   RETURN NUMBER
IS
   tmpvar   NUMBER;
BEGIN
   DECLARE
      t1   NUMBER;
      v2   NUMBER;
   BEGIN
      SELECT /*+index (c) index (v1)*/
             (MAX (v1.version_label) - 2)
        INTO t1
        FROM VERSIONS v1, components c
       WHERE v1.docnumber = docnum
         AND v1.version_id = c.version_id
         AND c.file_size > 0;

      SELECT /*+index (v1)*/
             v1.version_id
        INTO v2
        FROM VERSIONS v1                                      --, components c
       WHERE v1.docnumber = docnum AND v1.version_label = t1;

      -- AND c.file_size > 0
      SELECT /*+index (c) */
             c.version_id
        INTO tmpvar
        FROM components c
       WHERE c.file_size > 0 AND c.docnumber = docnum AND c.version_id = v2;

      IF (tmpvar IS NULL)
      THEN
         tmpvar := 0;
      END IF;

      RETURN tmpvar;
   EXCEPTION
      WHEN OTHERS
      THEN
         tmpvar := 0;
   END;
END getmaxver_meno1; 

/

SHOW ERRORS;



ALTER TABLE DPA_DETT_GLOBALI
 ADD (VAR_COD_IPA  VARCHAR2(50 BYTE));

CREATE OR REPLACE FUNCTION          getDocPrincipale (systemID INT)
RETURN varchar IS risultato varchar(2000);
BEGIN

SELECT DISTINCT A.docname into risultato
FROM profile A
WHERE 
 A.SYSTEM_ID =systemID;
RETURN risultato;
END getDocPrincipale; 

/

SHOW ERRORS;

CREATE SEQUENCE SEQ_AMMINISTRAZIONI
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

CREATE OR REPLACE FUNCTION getsedocfascicolato (systemid NUMBER)
   RETURN CHAR
IS
   Tmpvar   Char;      
   cnt   INT;
Begin
SELECT COUNT (LINK)
     INTO cnt
FROM project_components
WHERE LINK = systemid;

IF (cnt > 0)  THEN
   tmpvar := '1';
ELSE
   tmpvar := '0';
END IF;
Return Tmpvar;

Exception      When Others      Then         Tmpvar := '0';Return Tmpvar;
END getsedocfascicolato;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          GetDescTipoFasc(idAtto NUMBER)
RETURN VARCHAR2 IS risultato VARCHAR2(255 Byte);

BEGIN
begin

SELECT VAR_DESC_FASC
INTO risultato FROM DPA_tipo_fasc f WHERE SYSTEM_ID = idAtto;

EXCEPTION
WHEN OTHERS THEN
risultato:='';
end;
RETURN risultato;
END GetDescTipoFasc; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION           getChaImConvPDF(docNum number) RETURN char IS
tmpVar char;
BEGIN
declare
v_path varchar(128);
vMaxIdGenerica number;
begin

begin
SELECT /*+index (c) index (v1)*/
MAX (v1.version_id)
INTO vMaxIdGenerica
FROM VERSIONS v1, components c
WHERE v1.docnumber = docNum
AND v1.version_id = c.version_id
AND c.file_size > 0;
EXCEPTION
WHEN OTHERS THEN
vMaxIdGenerica:=0;
end;

begin
select substr(path, length(path)-instr(reverse(path),'.')+2) into v_path from components where docnumber=docNum and version_id=vMaxIdGenerica;
EXCEPTION
WHEN OTHERS THEN
tmpVar:='0';
end;

if(upper(v_path) = 'PDF' )
then tmpVar:='1';
else tmpVar:='0';
end if;

end;
RETURN tmpVar;
END getChaImConvPDF; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          geNumDocInFasc(idfasc number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(link) into tmpvar   from project_components  where project_id in (select system_id from project where id_fascicolo=idfasc);
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocInFasc; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          geNumDocInFasc2(idParent number,amm number,idreg number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(distinct(link)) into tmpvar   from project_components  
where project_id in (

   SELECT system_id
        FROM project
       WHERE cha_tipo_proj = 'C'
         AND id_amm = amm
         AND id_fascicolo in 
         (
         SELECT system_id
        FROM project
       WHERE cha_tipo_proj = 'F'
         AND id_amm = amm
         AND (id_registro = idreg OR id_registro IS NULL)
         AND id_parent = idParent
         )
         AND (id_registro = idreg OR id_registro IS NULL));

EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocInFasc2; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION             geNumDocProtInFasc(idfasc number) RETURN int IS
tmpVar int;
BEGIN
begin
select count(link) into tmpvar   from project_components pc,profile p where p.SYSTEM_ID=pc.link and p.num_proto is not null and p.CHA_TIPO_PROTO in ('A','P','I')  and  pc.project_id in (select system_id from project where id_fascicolo=idfasc);
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:=0;

end;
RETURN tmpVar;
END geNumDocProtInFasc; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION             getChaCartaceo (docnum number)
RETURN VARCHAR2
IS
Iscartaceo   Varchar2 (16);
vmaxidgenerica   NUMBER;

BEGIN
BEGIN
  SELECT MAX (v1.version_id)
    INTO vmaxidgenerica
  FROM VERSIONS v1, components c
  Where V1.Docnumber = Docnum And V1.Version_Id = C.Version_Id;
  
  EXCEPTION   WHEN OTHERS   THEN  Vmaxidgenerica := 0;
END;

BEGIN
  Select Cartaceo
    INTO isCartaceo
  FROM VERSIONS
  WHERE docnumber = docnum AND version_id = vmaxidgenerica;
  
  EXCEPTION  WHEN OTHERS  THEN  Iscartaceo := '0';
END;

RETURN isCartaceo;
END getChaCartaceo;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION          getCodFASC(idFASC number) RETURN varchar2 IS
tmpVar varchar2(32);
BEGIN
begin
select upper(var_codice) into tmpvar from project where system_id=idFASC;
EXCEPTION
WHEN NO_DATA_FOUND THEN
NULL;
WHEN OTHERS THEN
tmpVar:='';

end;
RETURN tmpVar;
END getCodFASC; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getdesctipodoc (idtipologia NUMBER)
   RETURN VARCHAR2
IS
   risultato   VARCHAR2 (255 BYTE);
BEGIN
   BEGIN
      SELECT var_desc_atto
        INTO risultato
        FROM dpa_tipo_atto f
       WHERE system_id = idtipologia;
   EXCEPTION
      WHEN OTHERS
      THEN
         risultato := '';
   END;

   RETURN risultato;
END getdesctipodoc; 

/

SHOW ERRORS;

/*
CREATE OR REPLACE TRIGGER UTL_AFTER_DROP_ON_SCHEMA After Drop On Schema
DISABLE
Declare
Nomeutente      Varchar2(32);
Data_Eseguito   Date;
Comando_Eseguito Varchar2(2000);
Versione_Cd     Varchar2(200);
esito           VARCHAR2(200);
Begin

Comando_Eseguito := 'Dropped '||Ora_Dict_Obj_Type||' '||Ora_Dict_Obj_Name||' ON Database:'||ora_database_name ;
select username into Nomeutente  from user_users;

Utl_Insert_Log (Nomeutente
,sysDate
, Comando_Eseguito
, 'n.d.' --Versione_Cd Varchar2
, 'ok'--esito VARCHAR2
);
end;
/
SHOW ERRORS;

*/


CREATE SEQUENCE SEQ_DPA_ALERT_CONSERVAZIONE
  START WITH 1
  MAXVALUE 9999999999999999999999999999
  MINVALUE 1
  NOCYCLE
  CACHE 20
  NOORDER;

ALTER TABLE POLICY_CONSERVAZIONE
 ADD (MESI_AVVISO_LEGGIBILITA  VARCHAR2(100 BYTE));

CREATE OR REPLACE FUNCTION             codrfappartenzaRuoloCOrrGLob (idRuolo INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (128);
BEGIN
   BEGIN
      SELECT var_codice
        INTO risultato
        FROM dpa_el_registri a
       WHERE cha_rf = '1'
         AND EXISTS (
                SELECT 'x'
                  FROM dpa_l_ruolo_reg t
                 WHERE t.id_registro = a.system_id
                   AND EXISTS (
                          SELECT 'x'
                            FROM dpa_corr_globali cg
                           WHERE cg.id_gruppo =idruolo and
                           cg.system_id=t.ID_RUOLO_IN_UO
                             ));
   EXCEPTION
      WHEN others
      THEN
         risultato := ' ';
   END;

   RETURN risultato;
END codrfappartenzaRuoloCOrrGLob; 

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION getContatoreDocIdObj(
      docNumber     INT,
      tipoContatore CHAR,
      IdObj         INT)
    RETURN VARCHAR
  IS
    risultato       VARCHAR(255);
    valoreContatore VARCHAR(255);
    annoContatore   VARCHAR(255);
    codiceRegRf     VARCHAR(255);
  BEGIN
    valoreContatore := '';
    annoContatore   := '';
    codiceRegRf     := '';
    SELECT valore_oggetto_db,
      anno
    INTO valoreContatore,
      annoContatore
    FROM dpa_associazione_templates,
      dpa_oggetti_custom,
      dpa_tipo_oggetto
    WHERE dpa_associazione_templates.doc_number = TO_CHAR(docNumber)
    AND dpa_associazione_templates.id_oggetto   = dpa_oggetti_custom.system_id
    AND dpa_oggetti_custom.id_tipo_oggetto      = dpa_tipo_oggetto.system_id
    AND dpa_tipo_oggetto.descrizione            = 'Contatore'
    AND dpa_oggetti_custom.system_id            =IdObj
    AND dpa_oggetti_custom.cha_tipo_tar         = tipoContatore;
    IF(tipoContatore                           <>'T') THEN
      BEGIN
        SELECT dpa_el_registri.var_codice
        INTO codiceRegRf
        FROM dpa_associazione_templates,
          dpa_oggetti_custom,
          dpa_tipo_oggetto,
          dpa_el_registri
        WHERE dpa_associazione_templates.doc_number = TO_CHAR(docNumber)
        AND dpa_associazione_templates.id_oggetto   = dpa_oggetti_custom.system_id
        AND dpa_oggetti_custom.id_tipo_oggetto      = dpa_tipo_oggetto.system_id
        AND dpa_tipo_oggetto.descrizione            = 'Contatore'
        AND dpa_oggetti_custom.system_id            =IdObj
        AND dpa_oggetti_custom.cha_tipo_tar         = tipoContatore
        AND dpa_associazione_templates.id_aoo_rf    = dpa_el_registri.system_id;
      END;
    END IF;
    IF(codiceRegRf IS NOT NULL) THEN
      risultato    := NVL(codiceRegRf,'') ||'-'|| NVL(annoContatore,'') ||'-'|| NVL(valoreContatore,'');
    ELSE
      risultato := NVL(annoContatore,'') ||'-'|| NVL(valoreContatore,'');
    END IF;
    RETURN risultato;
  END getContatoreDocIdObj;

/

SHOW ERRORS;

CREATE OR REPLACE FUNCTION                getContatoreDoc_cf (docNumber INT, tipoContatore CHAR)
RETURN VARCHAR IS risultato VARCHAR(255);

valoreContatore VARCHAR(255);
annoContatore VARCHAR(255);
codiceRegRf VARCHAR(255);
repertorio NUMBER;


DataInizio DATE;
DataFine DATE;
dInizio VARCHAR(255);
dFine VARCHAR(255);
annocorrente VARCHAR2(255);

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



SELECT DATA_INIZIO,DATA_FINE INTO DataInizio, DataFine FROM
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

IF(to_char(DataInizio,'YYYY') != 0 AND to_char(DataFine, 'YYYY') !=0) THEN
begin
dInizio := to_char(DataInizio,'YYYY');
dFine := to_char(DataFine, 'YYYY');
annoCorrente := dInizio||'/'||dFine;
end;
END IF;


END;

--in caso di date di intervallo nulla la query procede come nella versione precedente




if(codiceRegRf is  null)
then
risultato :=    nvl(valoreContatore,'')||'-'||nvl(annoContatore,'') ;
else  
risultato :=  nvl(codiceRegRf,'') ||'-'|| nvl(annoContatore,'') ||'-'|| nvl(valoreContatore,'');
end if; 

RETURN risultato;
End Getcontatoredoc_cf; 

/

SHOW ERRORS;

ALTER TABLE PUBLISHER_EVENTS
 ADD (LOADATTATCHSIFDOCTYPE  CHAR(1 BYTE));

 /
 SHOW ERRORS;
 
 
 CREATE OR REPLACE PROCEDURE scambiachaimg (
   p_idprincipale         NUMBER,
   p_idallegato           NUMBER,
   returnvalue      OUT   NUMBER
)
IS
BEGIN
   DECLARE
      cha_principale   NUMBER;
      cha_allegato     NUMBER;
   BEGIN
      cha_principale := 0;
      cha_allegato := 0;
      returnvalue := 0;

      <<reperimento_cha_principale>>
      BEGIN
         SELECT cha_img
           INTO cha_principale
           FROM PROFILE
          WHERE docnumber = p_idprincipale;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            returnvalue := 1;
            RETURN;
      END reperimento_cha_principale;

      dbms_output.PUT_LINE('Cha_principale: '||cha_principale );

      <<reperimento_cha_allegato>>
      BEGIN
         SELECT cha_img
           INTO cha_allegato
           FROM PROFILE
          WHERE docnumber = p_idallegato;
      EXCEPTION
         WHEN NO_DATA_FOUND
         THEN
            returnvalue := 2;
            RETURN;
      END reperimento_cha_allegato;

      dbms_output.PUT_LINE('Cha_allegato: ' || cha_allegato);

      IF (cha_principale <> cha_allegato)
      THEN
         BEGIN

            <<update_profile_principale>>
            BEGIN
               UPDATE PROFILE
                  SET cha_img = cha_allegato
                WHERE system_id = p_idprincipale;
            EXCEPTION
               WHEN OTHERS
               THEN
                  returnvalue := 3;
                  RETURN;
            END update_profile_principale;
            dbms_output.PUT_LINE('Aggiornata profile principale');

            <<update_profile_allegato>>
            BEGIN
               UPDATE PROFILE
                  SET cha_img = cha_principale
                WHERE system_id = p_idallegato;
            EXCEPTION
               WHEN OTHERS
               THEN
                  returnvalue := 4;
                  RETURN;
            END update_profile_allegato;
            dbms_output.PUT_LINE('Aggiornata profile allegato');
         END;
      END IF;
   END;
END;
/

SHOW ERRORS;

 
 