
Begin 
	Utl_Backup_Plsql_Code    ( 'FUNCTION', 'INTEROPTABLEFUNCTION');
end;
/


CREATE OR REPLACE FUNCTION INTEROPTABLEFUNCTION (
   anno          NUMBER,
   id_registro   NUMBER,
   idAmm         NUMBER
)
   RETURN interoptablerow PIPELINED
IS
--inizializzazione
   out_rec_amm      interoptabletype
      := interoptabletype (NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL
                          );
   out_rec_anno     interoptabletype
      := interoptabletype (NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL,
                           NULL
                          );
-- variabili del cursore
   var_codice_amm   VARCHAR (255);
   var_codice_aoo   VARCHAR (255);
   doc_spediti      NUMBER;
   mese             NUMBER;

--Dichiarazione del cursore
   CURSOR c_data (a NUMBER, reg NUMBER)
   Is
      /* Select Distinct (Count (*)), To_Number (To_Char (Dta_Spedizione, 'MM')),
                      A.Var_Desc_Amm As Var_Codice_Amm, Var_Codice_Aoo
                 From Profile P,  Dpa_Stato_Invio Si , Dpa_Amministra A  
                 Where Si.Var_Codice_Amm = A.Var_Codice_Amm 
                 and cha_tipo_proto = 'P'
                  AND id_registro = reg
                  and nvl(p.CHA_IN_CESTINO,'0') = '0'
                  AND p.system_id = si.id_profile
                  And To_Number (To_Char (Dta_Spedizione, 'YYYY')) = A
                  AND si.var_codice_amm IS NOT NULL
                  And Upper (Var_Codice_Aoo) <> Upper (Getregdescr (Reg))
             GROUP BY ROLLUP ( A.Var_Desc_Amm, var_codice_aoo,
                              TO_NUMBER (TO_CHAR (dta_spedizione, 'MM')))
             ORDER BY  A.Var_Desc_Amm,
                      Var_Codice_Aoo,
                      To_Number (To_Char (Dta_Spedizione, 'MM'));*/

    Select /*+ PARALLEL (P,8)*/
Distinct  Count (*) As Conteggio, To_Number (To_Char (Dta_Spedizione, 'MM')) --As Mese_Spedizione --,D.Description
    , A.Var_Desc_Amm As Var_Codice_Amm, cg.VAR_DESC_CORR
           From Profile P
           , Dpa_Stato_Invio Si
           , Dpa_Amministra A
           , Documenttypes D
           , dpa_t_canale_corr tc
           , dpa_corr_globali cg
          WHERE cha_tipo_proto = 'P'
            AND tc.id_documenttype = d.system_id
            And Tc.Id_Corr_Globale = Si.Id_Corr_Globale
            And P.Id_Registro = reg
            And To_Number (To_Char (Dta_Spedizione, 'YYYY')) = A
            and d.type_id in ('INTEROPERABILITA','SIMPLIFIEDINTEROPERABILITY','MAIL')
            And Nvl (P.Cha_In_Cestino, '0') = '0'
            AND p.system_id = si.id_profile
            And Dta_Spedizione Is Not Null
            and cg.SYSTEM_ID = Si.ID_CORR_GLOBALE
            and A.SYSTEM_ID = idAmm
       GROUP BY ROLLUP ( A.Var_Desc_Amm, cg.VAR_DESC_CORR,
                              TO_NUMBER (TO_CHAR (dta_spedizione, 'MM')))
             ORDER BY  A.Var_Desc_Amm,
                      cg.VAR_DESC_CORR,
                      To_Number (To_Char (Dta_Spedizione, 'MM'))         ;             


BEGIN
   OPEN c_data (anno, id_registro);

--set iniziale variabili
   out_rec_amm.gennaio := 0;
   out_rec_amm.febbraio := 0;
   out_rec_amm.marzo := 0;
   out_rec_amm.aprile := 0;
   out_rec_amm.maggio := 0;
   out_rec_amm.giugno := 0;
   out_rec_amm.luglio := 0;
   out_rec_amm.agosto := 0;
   out_rec_amm.settembre := 0;
   out_rec_amm.ottobre := 0;
   out_rec_amm.novembre := 0;
   out_rec_amm.dicembre := 0;

   LOOP
      FETCH c_data
       INTO doc_spediti, mese, var_codice_amm, var_codice_aoo;

      EXIT WHEN c_data%NOTFOUND;

      IF (    (doc_spediti <> 0)
          AND (mese <> 0)
          AND (var_codice_amm IS NOT NULL)
          AND (var_codice_aoo IS NOT NULL)
         )
      THEN
         out_rec_amm.var_cod_amm := var_codice_amm;
         out_rec_amm.var_cod_aoo := var_codice_aoo;

         IF (mese = 1)
         THEN
            out_rec_amm.gennaio := doc_spediti;
         END IF;

         IF (mese = 2)
         THEN
            out_rec_amm.febbraio := doc_spediti;
         END IF;

         IF (mese = 3)
         THEN
            out_rec_amm.marzo := doc_spediti;
         END IF;

         IF (mese = 4)
         THEN
            out_rec_amm.aprile := doc_spediti;
         END IF;

         IF (mese = 5)
         THEN
            out_rec_amm.maggio := doc_spediti;
         END IF;

         IF (mese = 6)
         THEN
            out_rec_amm.giugno := doc_spediti;
         END IF;

         IF (mese = 7)
         THEN
            out_rec_amm.luglio := doc_spediti;
         END IF;

         IF (mese = 8)
         THEN
            out_rec_amm.agosto := doc_spediti;
         END IF;

         IF (mese = 9)
         THEN
            out_rec_amm.settembre := doc_spediti;
         END IF;

         IF (mese = 10)
         THEN
            out_rec_amm.ottobre := doc_spediti;
         END IF;

         IF (mese = 11)
         THEN
            out_rec_amm.novembre := doc_spediti;
         END IF;

         IF (mese = 12)
         THEN
            out_rec_amm.dicembre := doc_spediti;
         END IF;
      END IF;

-- TOTALE PARZIALE
      IF (    (doc_spediti <> 0)
          AND (mese IS NULL)
          AND (var_codice_amm IS NOT NULL)
          AND (var_codice_aoo IS NOT NULL)
         )
      THEN
         out_rec_amm.tot_m_sped := doc_spediti;
         out_rec_amm.tot_sped := 0;
         PIPE ROW (out_rec_amm);
         out_rec_amm.gennaio := 0;
         out_rec_amm.febbraio := 0;
         out_rec_amm.marzo := 0;
         out_rec_amm.aprile := 0;
         out_rec_amm.maggio := 0;
         out_rec_amm.giugno := 0;
         out_rec_amm.luglio := 0;
         out_rec_amm.agosto := 0;
         out_rec_amm.settembre := 0;
         out_rec_amm.ottobre := 0;
         out_rec_amm.novembre := 0;
         out_rec_amm.dicembre := 0;
      END IF;

-- TOTALE
      IF (    (doc_spediti <> 0)
          AND (mese IS NULL)
          AND (var_codice_amm IS NULL)
          AND (var_codice_aoo IS NULL)
         )
      THEN
-- INSERIMENTO DEL TOTALE FINALE
         out_rec_anno.var_cod_amm := '0';
         out_rec_anno.var_cod_aoo := '0';
         out_rec_anno.gennaio := '0';
         out_rec_anno.febbraio := '0';
         out_rec_anno.marzo := '0';
         out_rec_anno.aprile := '0';
         out_rec_anno.maggio := '0';
         out_rec_anno.giugno := '0';
         out_rec_anno.luglio := '0';
         out_rec_anno.agosto := '0';
         out_rec_anno.settembre := '0';
         out_rec_anno.ottobre := '0';
         out_rec_anno.novembre := '0';
         out_rec_anno.dicembre := '0';
         out_rec_anno.tot_m_sped := '0';
         out_rec_anno.tot_sped := doc_spediti;
         PIPE ROW (out_rec_anno);
      END IF;
   END LOOP;                                                --fine del cursore

   CLOSE c_data;

   RETURN;
EXCEPTION
   WHEN OTHERS
   Then
        Out_Rec_Anno.Var_Cod_Amm := Var_Codice_Amm;
         Out_Rec_Anno.Var_Cod_Aoo := Var_Codice_Aoo;
         out_rec_anno.gennaio := mese;
         out_rec_anno.febbraio := '0';
         out_rec_anno.marzo := '0';
         out_rec_anno.aprile := '0';
         out_rec_anno.maggio := '0';
         out_rec_anno.giugno := '0';
         out_rec_anno.luglio := '0';
         out_rec_anno.agosto := '0';
         out_rec_anno.settembre := '0';
         out_rec_anno.ottobre := '0';
         out_rec_anno.novembre := '0';
         out_rec_anno.dicembre := '0';
         Out_Rec_Anno.Tot_M_Sped := '0';
         out_rec_anno.tot_sped := substr(sqlerrm,1,20);
         Pipe Row (Out_Rec_Anno);
END interoptablefunction;
/



-- DE LUCA - modifica interoptablerow per gestione prospetto R5

begin
	execute immediate
		'drop TYPE        "INTEROPTABLEROW"';
end;
/

begin
	execute immediate
		'Drop Type          Interoptabletype';
end;
/		

begin
	execute immediate
		'Create Or Replace Type              Interoptabletype Is 
OBJECT (var_cod_amm varchar (256), Var_Cod_Aoo Varchar(256) -- prev 100
, Gennaio Varchar(20), Febbraio Varchar(20), Marzo Varchar(20), Aprile Varchar(20)
, maggio varchar(20), giugno varchar(20),
Luglio Varchar(20), Agosto Varchar(20), Settembre Varchar(20), Ottobre Varchar(20), Novembre Varchar(20)
, Dicembre Varchar(20), Tot_M_Sped Varchar(20),
Tot_Sped Varchar(20))';
end;
/

begin
	execute immediate
		'Create Or Replace Type "INTEROPTABLEROW" Is Table Of Interoptabletype';
end;		
/




-- VELTRI - inserimento chiavi configurazione per nuova grafica

begin
  Utl_Insert_Chiave_Config('FE_ENABLE_MITTENTI_MULTIPLI','Abilita la visualizzazione dei mittenti multipli nel FE'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_INTERNAL_PROTOCOL','Abilita amministraizone al protocollo interno'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_KEY_WORDS','Abilita la parola chiave nella scheda del documento'  -- Codice, Descrizione
  ,'1','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_SINCRONA_LC','Abilita la conversione sincrona pdf'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('CONVERSIONE_PDF_LATO_SERVER','Abilita la conversione pdf lato server'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
   Utl_Insert_Chiave_Config('RUBRICA_RIGHE_PER_PAGINA','Definisce il numero di righe da visualizzare per pagina nella rubrica'  -- Codice, Descrizione
   ,'8','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
   ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
   ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
   end;
/

begin
  Utl_Insert_Chiave_Config('FE_SEDE_TRASM','Inserisce la sede dell utente nel dettaglio della trasmissione'  -- Codice, Descrizione
  ,'0','F','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/

begin
  Utl_Insert_Chiave_Config('FE_DETTAGLI_FIRMA','Attiva/Disattiva Dettagli Firma Completi/Sintetici'  -- Codice, Descrizione
  ,'1','B','1'                            --Valore               ,Tipo_Chiave        ,Visibile
  ,'1','0','3.28'                  --Modificabile         ,Globale            ,myversione_CD
  ,NULL, NULL, NULL);    --Codice_Old_Webconfig ,Forza_Update_Valore,RFU
  end;  
/



-- no need to backup, in caso uncomment these lines

Begin Utl_Backup_Plsql_Code    ( 'PROCEDURE', 'putFile');
end;
/


CREATE OR REPLACE PROCEDURE putFile
(
p_versionId int,
p_filePath nvarchar2,
p_fileSize int,
p_printThumb nvarchar2,
p_iscartaceo smallint,
P_Estensione Varchar,
P_Isfirmato Char
, p_nomeoriginale Varchar default NULL -- added on 27 feb 2013 
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
 
 -- modifica by Paolo De Luca feb 2013 -- commentato ramo IF inutile
 --if (p_isFirmato='1') then  
 
update components
set  path = p_filePath,
file_size = p_fileSize,
var_impronta = p_printThumb,
Ext =P_Estensione,
Cha_Firmato = Nvl(P_Isfirmato,'0')  --'1'-- sufficiente usare sintassi nvl, inutile fare IF ELSE 
, Var_Nomeoriginale =nvl(P_Nomeoriginale,Var_Nomeoriginale )  -- added on 27 feb 2013 by PDL  
Where     Version_Id = P_Versionid;
-- modifica by Paolo De Luca feb 2013 -- commentato ramo ELSE inutile
/*else
update components
set     path = p_filePath,
file_size = p_fileSize,
var_impronta = p_printThumb,
ext =p_estensione,
Cha_Firmato = '0'
, VAR_NOMEORIGINALE =p_nomeoriginale  -- added on 27 feb 2013 
where     version_id = p_versionId; 
end if; */ -- fine ramo IF 

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



create or replace PROCEDURE INS_OCC_2
 (p_ID_REG in integer,
 p_IDAMM in integer,
 p_Prefix_cod_rub in VARCHAR,
 p_DESC_CORR in VARCHAR,
 p_CHA_DETTAGLI in VARCHAR,
 p_ID_Corr_Globali in VARCHAR,
 p_EMAIL in VARCHAR,
 p_RESULT OUT integer)
 as
 BEGIN
 declare sysid integer;
         id_reg NUMBER;
          idamm NUMBER;
          myprofile NUMBER:=0;
          Countprofilato Number := 0;
          Countprofdoc Number := 0;
          countProfFasc number     := 0;
          new_var_cod_rubrica1 VARCHAR2 (128);
          cod_rubrica VARCHAR2 (128);
 
 BEGIN
 -- verifica preesistenza dell'occ corrente
 -- per sviluppi futuri .....
 /*
 
 select system_id into p_RESULT
 from ( select system_id  from DPA_CORR_GLOBALI
 where UPPER (var_desc_corr) = UPPER (p_DESC_CORR) and  CHA_TIPO_CORR = 'O' AND ID_AMM=P_IDAMM)
 where rownum =1;
 
 
 EXCEPTION WHEN NO_DATA_FOUND THEN
 */
 --inserisco il nuovo occ_
 begin
 select seq.nextval into sysid from dual;
 
 --controlli preliminari
 --verifico se il corrisp  stato utilizzato come dest/mitt di protocolli
 IF(not(p_ID_Corr_Globali = 0))
   then
   BEGIN
     SELECT COUNT (id_profile)
       INTO myprofile
     FROM dpa_doc_arrivo_par
     Where Id_Mitt_Dest = p_ID_Corr_Globali;
 -- verifico se il corrispondente  stato usato o meno nei campi profilati
     Select Count(System_Id) 
       Into Countprofdoc 
     From Dpa_Associazione_Templates 
     where valore_oggetto_db = to_char(p_ID_Corr_Globali);
           
     Select Count(System_Id) 
       Into Countproffasc 
     from dpa_ass_templates_fasc
     where valore_oggetto_db = to_char(p_ID_Corr_Globali);
      
     Countprofilato := Countprofdoc + Countproffasc;
     END;
     END IF;     
 --FINE controlli preliminari
 
   IF(myprofile = 0 AND Countprofilato = 0)
   THEN
   BEGIN
   
     if(p_id_reg=0) then
       
       INSERT INTO
       DPA_CORR_GLOBALI (system_id,ID_REGISTRO,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
       VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
       VALUES (sysid,null,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
       returning system_id into p_RESULT;
       else
       INSERT INTO
       DPA_CORR_GLOBALI (system_id,id_registro,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
       VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
       VALUES (sysid,p_ID_REG,p_idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,0,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
       returning system_id into p_RESULT;
       end if;
 
   END;
   
   ELSE
     BEGIN
        Select Var_Cod_Rubrica, id_registro, id_amm                     
        INTO new_var_cod_rubrica1, id_reg, idamm
        FROM dpa_corr_globali
        WHERE system_id = p_ID_Corr_Globali;
        
        -- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
       new_var_cod_rubrica1 := new_var_cod_rubrica1 || '_' || TO_CHAR (p_ID_Corr_Globali);
     
         UPDATE DPA_CORR_GLOBALI
           SET DTA_FINE = SYSDATE(),
                         var_cod_rubrica = new_var_cod_rubrica1,
                         var_codice      = new_var_cod_rubrica1,
                         Id_Parent = Null
          WHERE SYSTEM_ID = p_ID_Corr_Globali;
         
         INSERT INTO
         DPA_CORR_GLOBALI (system_id,id_registro,ID_AMM,VAR_COD_RUBRICA,VAR_DESC_CORR,ID_OLD,DTA_INIZIO,ID_PARENT,
         VAR_CODICE,CHA_TIPO_CORR,CHA_DETTAGLI,VAR_EMAIL)
         VALUES (sysid,id_reg,idamm,p_Prefix_cod_rub||to_char(sysid),p_desc_corr,p_ID_Corr_Globali,sysdate,0,p_Prefix_cod_rub||to_char(sysid),'O',0,p_EMAIL)
         returning system_id into p_RESULT;
      END;
     END IF;
 
 END;
 END;
 
 end; 
/


begin
Utl_Backup_Plsql_code ('PROCEDURE','i_smistamento_smistadoc');
end;
/

CREATE OR REPLACE PROCEDURE i_smistamento_smistadoc(
    idpeoplemittente             IN NUMBER,
    idcorrglobaleruolomittente   IN NUMBER,
    idgruppomittente             IN NUMBER,
    idamministrazionemittente    IN NUMBER,
    idpeopledestinatario         IN NUMBER,
    idcorrglobaledestinatario    IN NUMBER,
    iddocumento                  IN NUMBER,
    idtrasmissione               IN NUMBER,
    idtrasmissioneutentemittente IN NUMBER,
    trasmissioneconworkflow      IN CHAR,
    notegeneralidocumento        IN VARCHAR2,
    noteindividuali              IN VARCHAR2,
    datascadenza                 IN DATE,
    tipodiritto                  IN CHAR,
    rights                       IN NUMBER,
    originalrights               IN NUMBER,
    idragionetrasm               IN NUMBER,
    idpeopledelegato             IN NUMBER,
    nonotify                     IN NUMBER,
    returnvalue OUT NUMBER )
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
  identitytrasm       NUMBER       := NULL;
  identitytrasmsing   NUMBER       := NULL;
  existaccessrights   CHAR (1)     := 'Y';
  accessrights        NUMBER       := NULL;
  accessrightsvalue   NUMBER       := NULL;
  tipotrasmsingola    CHAR (1)     := NULL;
  isaccettata         VARCHAR2 (1) := '0';
  isaccettatadelegato VARCHAR2 (1) := '0';
  isvista             VARCHAR2 (1) := '0';
  isvistadelegato     VARCHAR2 (1) := '0';
  resultvalue         NUMBER;
BEGIN
  BEGIN
    SELECT seq.NEXTVAL INTO identitytrasm FROM DUAL;
  END;
  BEGIN
    SELECT seq.NEXTVAL INTO identitytrasmsing FROM DUAL;
  END;
  BEGIN
    /*Inserimento in tabella DPA_TRASMISSIONE */
    IF (idpeopledelegato > 0) THEN
      INSERT
      INTO dpa_trasmissione
        (
          system_id,
          id_ruolo_in_uo,
          id_people,
          cha_tipo_oggetto,
          id_profile,
          id_project,
          dta_invio,
          var_note_generali,
          id_people_delegato
        )
        VALUES
        (
          identitytrasm,
          idcorrglobaleruolomittente,
          idpeoplemittente,
          'D',
          iddocumento,
          NULL,
          SYSDATE (),
          notegeneralidocumento,
          idpeopledelegato
        );
    ELSE
      INSERT
      INTO dpa_trasmissione
        (
          system_id,
          id_ruolo_in_uo,
          id_people,
          cha_tipo_oggetto,
          id_profile,
          id_project,
          dta_invio,
          var_note_generali
        )
        VALUES
        (
          identitytrasm,
          idcorrglobaleruolomittente,
          idpeoplemittente,
          'D',
          iddocumento,
          NULL,
          SYSDATE (),
          notegeneralidocumento
        );
    END IF;
  EXCEPTION
  WHEN OTHERS THEN
    returnvalue := -2;
    RETURN;
  END;
  BEGIN
    /* Inserimento in tabella DPA_TRASM_SINGOLA */
    INSERT
    INTO dpa_trasm_singola
      (
        system_id,
        id_ragione,
        id_trasmissione,
        cha_tipo_dest,
        id_corr_globale,
        var_note_sing,
        cha_tipo_trasm,
        dta_scadenza,
        id_trasm_utente
      )
      VALUES
      (
        identitytrasmsing,
        idragionetrasm,
        identitytrasm,
        'U',
        idcorrglobaledestinatario,
        noteindividuali,
        'S',
        datascadenza,
        NULL
      );
  EXCEPTION
  WHEN OTHERS THEN
    returnvalue := -3;
    RETURN;
  END;
  BEGIN
    /* Inserimento in tabella DPA_TRASM_UTENTE */
    INSERT
    INTO dpa_trasm_utente
      (
        system_id,
        id_trasm_singola,
        id_people,
        dta_vista,
        dta_accettata,
        dta_rifiutata,
        dta_risposta,
        cha_vista,
        cha_accettata,
        cha_rifiutata,
        var_note_acc,
        var_note_rif,
        cha_valida,
        id_trasm_risp_sing
      )
      VALUES
      (
        seq.NEXTVAL,
        identitytrasmsing,
        idpeopledestinatario,
        NULL,
        NULL,
        NULL,
        NULL,
        '0',
        '0',
        '0',
        NULL,
        NULL,
        '1',
        NULL
      );
  EXCEPTION
  WHEN OTHERS THEN
    returnvalue := -4;
    RETURN;
  END;
  BEGIN
    IF nonotify < 1 THEN
        BEGIN
            --per la gestione nuova dpa_todolist il trigger scatta solo se update dta_invio
            UPDATE dpa_trasmissione
            SET dta_invio   = SYSDATE ()
            WHERE system_id = identitytrasm;
            SELECT accessrights
            INTO accessrights
            FROM
            (SELECT accessrights
            FROM security
            WHERE thing       = iddocumento
            AND personorgroup = idpeopledestinatario
            )
            WHERE ROWNUM = 1;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            existaccessrights := 'N';
        END;
    END IF;
  END;
  IF existaccessrights   = 'Y' THEN
    accessrightsvalue   := accessrights;
    IF accessrightsvalue < rights THEN
      BEGIN
        /* aggiornamento a Rights */
        UPDATE security
        SET accessrights   = rights,
          cha_tipo_diritto = 'T'
        WHERE thing        = iddocumento
        AND personorgroup  = idpeopledestinatario
        AND accessrights   = accessrightsvalue;
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      END;
    END IF;
  ELSE
    BEGIN
      /* inserimento a Rights */
      INSERT
      INTO security
        (
          thing,
          personorgroup,
          accessrights,
          id_gruppo_trasm,
          cha_tipo_diritto
        )
        VALUES
        (
          iddocumento,
          idpeopledestinatario,
          rights,
          idgruppomittente,
          tipodiritto
        );
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      NULL;
    END;
  END IF;
  /* Aggiornamento trasmissione del mittente */
  IF (trasmissioneconworkflow = '1') THEN
    BEGIN
      -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
      SELECT cha_accettata
      INTO isaccettata
      FROM dpa_trasm_utente
      WHERE system_id = idtrasmissioneutentemittente;
      SELECT cha_vista
      INTO isvista
      FROM dpa_trasm_utente
      WHERE system_id      = idtrasmissioneutentemittente;
      IF (idpeopledelegato > 0) THEN
        BEGIN
          -- Impostazione dei flag per la gestione del delegato
          isvistadelegato     := '1';
          isaccettatadelegato := '1';
        END;
      END IF;
      IF (isaccettata = '1') THEN
        BEGIN
          -- caso in cui la trasmissione risulta gi? accettata
          IF (isvista = '0') THEN
            BEGIN
              -- l'oggetto trasmesso non risulta ancora visto,
              -- pertanto vengono impostati i dati di visualizzazione
              -- e viene rimossa la trasmissione dalla todolist
              UPDATE dpa_trasm_utente
              SET dta_vista = (
                CASE
                  WHEN dta_vista IS NULL
                  THEN SYSDATE
                  ELSE dta_vista
                END ),
                cha_vista = (
                CASE
                  WHEN dta_vista IS NULL
                  THEN 1
                  ELSE 0
                END ),
                cha_vista_delegato = isvistadelegato,
                cha_in_todolist    = '0',
                cha_valida         = '0'
              WHERE ( system_id    = idtrasmissioneutentemittente
              OR system_id         =
                (SELECT tu.system_id
                FROM dpa_trasm_utente tu,
                  dpa_trasmissione tx,
                  dpa_trasm_singola ts
                WHERE tu.id_people   = idpeoplemittente
                AND tx.system_id     = ts.id_trasmissione
                AND tx.system_id     = idtrasmissione
                AND ts.system_id     = tu.id_trasm_singola
                AND ts.cha_tipo_dest = 'U'
                ) );
            END;
          ELSE
            BEGIN
              -- l'oggetto trasmesso visto,
              -- pertanto la trasmissione viene solo rimossa dalla todolist
              UPDATE dpa_trasm_utente
              SET cha_in_todolist = '0',
                cha_valida        = '0'
              WHERE ( system_id   = idtrasmissioneutentemittente
              OR system_id        =
                (SELECT tu.system_id
                FROM dpa_trasm_utente tu,
                  dpa_trasmissione tx,
                  dpa_trasm_singola ts
                WHERE tu.id_people   = idpeoplemittente
                AND tx.system_id     = ts.id_trasmissione
                AND tx.system_id     = idtrasmissione
                AND ts.system_id     = tu.id_trasm_singola
                AND ts.cha_tipo_dest = 'U'
                ) );
            END;
          END IF;
        END;
      ELSE
        -- la trasmissione ancora non risulta accettata, pertanto:
        -- 1) viene accettata implicitamente,
        -- 2) l'oggetto trasmesso impostato come visto,
        -- 3) la trasmissione rimossa la trasmissione da todolist
        BEGIN
          UPDATE dpa_trasm_utente
          SET dta_vista = (
            CASE
              WHEN dta_vista IS NULL
              THEN SYSDATE
              ELSE dta_vista
            END ),
            cha_vista = (
            CASE
              WHEN dta_vista IS NULL
              THEN 1
              ELSE 0
            END),
            cha_vista_delegato     = isvistadelegato,
            dta_accettata          = SYSDATE (),
            cha_accettata          = '1',
            cha_accettata_delegato = isaccettatadelegato,
            var_note_acc           = 'Documento accettato e smistato',
            cha_in_todolist        = '0',
            cha_valida             = '0',
            id_people_delegato     =idpeopledelegato
          WHERE ( system_id        = idtrasmissioneutentemittente
          OR system_id             =
            (SELECT tu.system_id
            FROM dpa_trasm_utente tu,
              dpa_trasmissione tx,
              dpa_trasm_singola ts
            WHERE tu.id_people   = idpeoplemittente
            AND tx.system_id     = ts.id_trasmissione
            AND tx.system_id     = idtrasmissione
            AND ts.system_id     = tu.id_trasm_singola
            AND ts.cha_tipo_dest = 'U'
            ) )
          AND cha_valida = '1';
        END;
      END IF;
      --update security se diritti  trasmssione in accettazione =20
      BEGIN
        UPDATE security s
        SET s.accessrights   = originalrights,
          s.cha_tipo_diritto = 'T'
        WHERE s.thing        = iddocumento
        AND s.personorgroup IN (idpeoplemittente, idgruppomittente)
        AND s.accessrights   = 20;
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      END;
    END;
  ELSE
    BEGIN
      spsetdatavistasmistamento (idpeoplemittente, iddocumento, idgruppomittente, 'D', idtrasmissione, idpeopledelegato, resultvalue );
      IF (resultvalue = 1) THEN
        returnvalue  := -4;
        RETURN;
      END IF;
    END;
  END IF;
  /* verifica se la trasmissione singola  destinata a: "Tutti" (T) o a "Uno" (S) */
  BEGIN
    SELECT *
    INTO tipotrasmsingola
    FROM
      (SELECT a.cha_tipo_trasm
      FROM dpa_trasm_singola a,
        dpa_trasm_utente b
      WHERE a.system_id = b.id_trasm_singola
      AND b.system_id  IN
        (SELECT tu.system_id
        FROM dpa_trasm_utente tu,
          dpa_trasmissione tx,
          dpa_trasm_singola ts
        WHERE tu.id_people = idpeoplemittente
        AND tx.system_id   = ts.id_trasmissione
        AND tx.system_id   = idtrasmissione
        AND ts.system_id   = tu.id_trasm_singola
        AND ts.system_id   =
          (SELECT id_trasm_singola
          FROM dpa_trasm_utente
          WHERE system_id = idtrasmissioneutentemittente
          )
        )
      ORDER BY cha_tipo_dest
      )
    WHERE ROWNUM = 1;
  END;
  IF tipotrasmsingola = 'S' AND trasmissioneconworkflow = '1' THEN
    /* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
    BEGIN
      UPDATE dpa_trasm_utente
      SET cha_valida          = '0',
        cha_in_todolist       = '0'
      WHERE id_trasm_singola IN
        (SELECT a.system_id
        FROM dpa_trasm_singola a,
          dpa_trasm_utente b
        WHERE a.system_id = b.id_trasm_singola
        AND b.system_id  IN
          (SELECT tu.system_id
          FROM dpa_trasm_utente tu,
            dpa_trasmissione tx,
            dpa_trasm_singola ts
          WHERE tu.id_people = idpeoplemittente
          AND tx.system_id   = ts.id_trasmissione
          AND tx.system_id   = idtrasmissione
          AND ts.system_id   = tu.id_trasm_singola
          AND ts.system_id   =
            (SELECT id_trasm_singola
            FROM dpa_trasm_utente
            WHERE system_id = idtrasmissioneutentemittente
            )
          )
        )
      AND system_id NOT IN (idtrasmissioneutentemittente);
    END;
  END IF;
  returnvalue := 0;
END;
/


begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_JOBS';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_JOBS
  (
    ID NUMBER(10, 0) NOT NULL ,
    CONSTRAINT DPA_JOBS_PK PRIMARY KEY ( ID ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_CHECK_MAILBOX';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_CHECK_MAILBOX
  (
    ID           NUMBER(10, 0) NOT NULL ,
    IDJOB        NUMBER(10, 0) NOT NULL ,
    IDUSER       NUMBER(10, 0) NOT NULL ,
    IDROLE       NUMBER(10, 0) NOT NULL ,
    IDREG        NUMBER(10, 0) NOT NULL ,
    MAIL         VARCHAR2(200 CHAR) NOT NULL ,
    ELABORATE    NUMBER(5, 0) DEFAULT 0 ,
    TOTAL        NUMBER(5, 0) DEFAULT 0 ,
    CONCLUDED    VARCHAR2(1 CHAR) DEFAULT 0 NOT NULL ,
    MAILUSERID   VARCHAR2(1000 CHAR) ,
    ERRORMESSAGE VARCHAR2(4000 BYTE) ,
	MAILSERVER VARCHAR2(1000 CHAR),
    CONSTRAINT DPA_CHECK_MAIL_PK PRIMARY KEY ( ID ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	execute immediate
		'ALTER TABLE DPA_CHECK_MAILBOX ADD CONSTRAINT DPA_CHECK_MAILBOX_FK FOREIGN KEY ( IDJOB ) REFERENCES DPA_JOBS ( ID ) ON
DELETE CASCADE ENABLE';
end;
/

begin
	declare cnt int;
    begin
        
        select count(*) into cnt from user_tables where table_name='DPA_REPORT_MAILBOX';
        if (cnt = 0) then
          execute immediate    
			'CREATE TABLE DPA_REPORT_MAILBOX
  (
    ID               NUMBER(10, 0) NOT NULL ,
    ID_CHECK_MAILBOX NUMBER(10, 0) NOT NULL ,
    MAILID           VARCHAR2(200 CHAR) ,
    TYPE             VARCHAR2(100 CHAR) ,
    RECEIPT          VARCHAR2(200) ,
    DATE_MAIL DATE ,
    FROM_MAIL         VARCHAR2(200 CHAR) ,
    ERROR             VARCHAR2(4000 BYTE) ,
    COUNT_ATTACHMENTS NUMBER(5, 0) ,
    SUBJECT           VARCHAR2(4000 BYTE) ,
    CONSTRAINT DPA_REPORT_MAILBOX_PK PRIMARY KEY ( ID , ID_CHECK_MAILBOX ) ENABLE
  )';
		end if;
	end;
end;
/

begin
	execute immediate
		'ALTER TABLE DPA_REPORT_MAILBOX ADD CONSTRAINT DPA_REPORT_MAILBOX_FK FOREIGN KEY ( ID_CHECK_MAILBOX ) REFERENCES DPA_CHECK_MAILBOX ( ID ) ON
DELETE CASCADE ENABLE';
end;
/


-- lasciare una riga bianca vuota all'inizio e a fine file per evitare effetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin

-- Mauro Pace

utl_add_column ('3.28',
                   'DOCSPA30',
                   'DPA_AMMINISTRA',
                   'VAR_DETTAGLIO_FIRMA',
                   'VARCHAR2(256 BYTE)',
                   NULL,NULL,NULL,NULL);
                   
end;
/


begin

-- Andrea de marco - MEV Cessione Diritti - Luciani,Savino

 utl_add_column (
 '3.28',
   'DOCSPA30',
   'DPA_MODELLI_TRASM',
   'CHA_MANTIENI_SCRITTURA',
   'CHAR(1)',
   '0',
   NULL,
   NULL,
   NULL);
End;
/


begin

-- Andrea de marco - MEV Cessione Diritti - Luciani,Savino

 utl_add_column (
 '3.28',
   'DOCSPA30',
   'DPA_RAGIONE_TRASM',
   'CHA_MANTIENI_SCRITT',
   'CHAR(1)',
   '0',
   NULL,
   NULL,
   NULL);
End;
/

-- occorre verificare che il system_id del campo Corrispondente della dpa_oggetti_custom_fasc sia 7

begin
  execute immediate
          'UPDATE  DPA_OGGETTI_CUSTOM_fasc D SET RICERCA_CORR=''INTERNI/ESTERNI''  WHERE NVL(RICERCA_CORR,''0'') =''0'' AND ID_TIPO_OGGETTO=7';
end;
/

-- occorre verificare che il system_id del campo Corrispondente della dpa_oggetti_custom sia 1


begin
  execute immediate
          'UPDATE  DPA_OGGETTI_CUSTOM D SET RICERCA_CORR=''INTERNI/ESTERNI''  WHERE NVL(RICERCA_CORR,''0'') =''0'' AND ID_TIPO_OGGETTO=1';
end;
/


begin
Insert into DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)

select max(system_id) +1 , sysdate,  '3.28' from Dpa_Docspa;
end;
/



