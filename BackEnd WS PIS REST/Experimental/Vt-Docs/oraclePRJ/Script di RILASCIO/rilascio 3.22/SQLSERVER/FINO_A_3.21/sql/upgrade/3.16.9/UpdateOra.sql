-- file sql di update per il CD --
---- dpa_chiavi_configurazione.ORA.sql  marcatore per ricerca ----
-- BE_RIC_MITT_INTEROP_BY_MAIL_DESC
BEGIN
    DECLARE cnt int;
  BEGIN
    SELECT COUNT(*) INTO cnt FROM @db_user.DPA_CHIAVI_CONFIGURAZIONE 
    WHERE VAR_CODICE='BE_RIC_MITT_INTEROP_BY_MAIL_DESC';
    IF (cnt = 0) THEN       
  
insert into DPA_CHIAVI_CONFIGURAZIONE
   (system_id, ID_AMM,VAR_CODICE,VAR_DESCRIZIONE
   ,VAR_VALORE,CHA_TIPO_CHIAVE,CHA_VISIBILE
   ,CHA_MODIFICABILE,CHA_GLOBALE)
         values
  (SEQ_DPA_CHIAVI_CONFIG.nextval,0,
'BE_RIC_MITT_INTEROP_BY_MAIL_DESC'
  ,'ATTIVA LA RICERCA DEL MITTENTE PER INTEROPERABILITA PER DESCRIZIONE E MAIL ANZICHE SOLO MAIL. VALORI POSSIBILI 0 o 1'
  ,'1','B','1'
  ,'1','1');

  
    END IF;
    END;
END;
/

              
------------------
              
---- getValCampoProfDoc.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.getValCampoProfDoc(DocNumber INT, CustomObjectId INT)

RETURN VARCHAR IS result VARCHAR(255);

 

tipoOggetto varchar(255);tipoCont varchar(1);

 

BEGIN

 

/*

select b.descrizione

into tipoOggetto

from 

dpa_oggetti_custom a, dpa_tipo_oggetto b 

where 

a.system_id = CustomObjectId

and

a.id_tipo_oggetto = b.system_id;

*/

 

 

 

select b.descrizione,cha_tipo_Tar

into tipoOggetto,tipoCont

from 

dpa_oggetti_custom a, dpa_tipo_oggetto b 

where 

a.system_id = CustomObjectId

and

a.id_tipo_oggetto = b.system_id;

 

if (tipoOggetto = 'Corrispondente') then

      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 

      from dpa_CORR_globali cg where cg.SYSTEM_ID = (

      select valore_oggetto_db from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber));

     else

      

--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)

if(tipoOggetto = 'CasellaDiSelezione') then

    BEGIN

        declare item varchar(255);

        CURSOR curCasellaDiSelezione IS select  valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber) and valore_oggetto_db is not null; 

        BEGIN

            OPEN curCasellaDiSelezione;

            LOOP

            FETCH curCasellaDiSelezione INTO item;

            EXIT WHEN curCasellaDiSelezione%NOTFOUND;

                IF(result IS NOT NULL) THEN

                result := result||'; '||item ;

                ELSE

                result := result||item;

                END IF;        

            END LOOP;

            CLOSE curCasellaDiSelezione;

        END;    

    END;

    elsif(tipoOggetto = 'Contatore') then

    begin

        select getContatoreDoc(DocNumber,tipoCont)  into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 

 

    end;

else 

--Tutti gli altri

    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber); 

end if;

end if;

 

RETURN result;

 

exception

when no_data_found

then

result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 

RETURN result;

when others

then

result := SQLERRM; 

RETURN result;

 

END getValCampoProfDoc;

/
              
------------------
              
---- getValCampoProfDocOrder.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION             @db_user.getValCampoProfDocOrder(DocNumber INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);

BEGIN

select b.descrizione
into tipoOggetto
from 
dpa_oggetti_custom a, dpa_tipo_oggetto b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber));
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select  valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = to_char(DocNumber) and valore_oggetto_db is not null; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'; '||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_associazione_templates where id_oggetto = CustomObjectId and doc_number = DocNumber; 
end if;
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END getValCampoProfDocOrder;
/

              
------------------
              
---- GetValProfObjPrj.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION @db_user.GetValProfObjPrj
(PrjId INT, CustomObjectId INT)

RETURN VARCHAR IS result VARCHAR(255);

 

tipoOggetto varchar(255);tipoCont varchar(1);

 

BEGIN

 

select b.descrizione,cha_tipo_Tar

into tipoOggetto,tipoCont

from 

dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 

where 

a.system_id = CustomObjectId

and

a.id_tipo_oggetto = b.system_id;

 

if (tipoOggetto = 'Corrispondente') then

      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 

      from dpa_CORR_globali cg where cg.SYSTEM_ID = (

      select valore_oggetto_db from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId));

     else

      

--Casella di selezione (Per la casella di selezione serve un caso particolare perch?? i valori sono multipli)

if(tipoOggetto = 'CasellaDiSelezione') then

    BEGIN

        declare item varchar(255);

        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId) and valore_oggetto_db is not null; 

        BEGIN

            OPEN curCasellaDiSelezione;

            LOOP

            FETCH curCasellaDiSelezione INTO item;

            EXIT WHEN curCasellaDiSelezione%NOTFOUND;

                IF(result IS NOT NULL) THEN

                result := result||'; '||item ;

                ELSE

                result := result||item;

                END IF;        

            END LOOP;

            CLOSE curCasellaDiSelezione;

        END;    

    END;

     elsif(tipoOggetto = 'Contatore') then

    begin

        select getContatoreFasc(PrjId,tipoCont)  into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 

 

    end;

else 

--Tutti gli altri

    select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId); 

end if;

end if;

 

RETURN result;

 

exception

when no_data_found

then

result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 

RETURN result;

when others

then

result := SQLERRM; 

RETURN result;

 

END GetValProfObjPrj;

/
              
------------------
              
---- GetValProfObjPrjOrder.ORA.sql  marcatore per ricerca ----
CREATE OR REPLACE FUNCTION             @db_user.GetValProfObjPrjOrder(PrjId INT, CustomObjectId INT)
RETURN VARCHAR IS result VARCHAR(255);

tipoOggetto varchar(255);

BEGIN

select b.descrizione
into tipoOggetto
from 
dpa_oggetti_custom_fasc a, dpa_tipo_oggetto_fasc b 
where 
a.system_id = CustomObjectId
and
a.id_tipo_oggetto = b.system_id;

if (tipoOggetto = 'Corrispondente') then
      select cg.var_cod_rubrica||' - '||cg.var_DESC_CORR into result 
      from dpa_CORR_globali cg where cg.SYSTEM_ID = (
      select valore_oggetto_db from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId);
     else
      
--Casella di selezione (Per la casella di selezione serve un caso particolare perch i valori sono multipli)
if(tipoOggetto = 'CasellaDiSelezione') then
    BEGIN
        declare item varchar(255);
        CURSOR curCasellaDiSelezione IS select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = to_char(PrjId) and valore_oggetto_db is not null; 
        BEGIN
            OPEN curCasellaDiSelezione;
            LOOP
            FETCH curCasellaDiSelezione INTO item;
            EXIT WHEN curCasellaDiSelezione%NOTFOUND;
                IF(result IS NOT NULL) THEN
                result := result||'; '||item ;
                ELSE
                result := result||item;
                END IF;        
            END LOOP;
            CLOSE curCasellaDiSelezione;
        END;    
    END;
else 
--Tutti gli altri
    select valore_oggetto_db into result from dpa_ass_templates_fasc where id_oggetto = CustomObjectId and id_project = PrjId; 
end if;
end if;

RETURN result;

exception
when no_data_found
then
result := null; --'id_oggetto = '||CustomObjectId|| 'doc_number = '||DocNumber; 
RETURN result;
when others
then
result := SQLERRM; 
RETURN result;

END GetValProfObjPrjOrder;
/


              
------------------
              
---- HistoricizeRole.ORA.sql  marcatore per ricerca ----
create or replace
PROCEDURE @db_user.HistoricizeRole 
(
  -- Id corr globali del ruolo da storicizzare
  idCorrGlobRole IN INTEGER  ,
  -- Eventuale nuovo codice da assegnare al ruolo
  newRoleCode   IN VARCHAR2,
  -- Eventuale nuova descrizione da assegnare al ruolo
  newRoleDescription  IN VARCHAR2, 
  -- Identificativo dell'eventuale nuova UO in cui deve essere inserito il ruolo
  newRoleUoId   IN VARCHAR2,
  -- Identificativo dell'eventuale nuovo tipo ruolo da assegnare al ruolo
  newRoleTypeId in number,
  -- Identificativo del record storicizzato
  oldIdCorrGlobId OUT INTEGER,
  -- Risultato dell'operazione
  returnValue OUT INTEGER
) AS 
BEGIN
    /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     HistoricizeRole 

    PURPOSE:  Store per la storicizzazione di un ruolo. Per ridurre al minimo 
              le movimentazioni di dati, specialmente nella securiy, viene
              adottata una tecnica di storicizzazione "verso l'alto" ovvero,
              quando si deve storicizzare un ruolo R con system id S e 
              codice rubrica C, viene inserita nella DPA_CORR_GLOBALI una tupla 
              che si differenzia da quella di R solo per S che sar un nuovo 
              numero assegnato dalla sequence, var_codice e var_cod_rubrica
              che saranno uguali a quelli di R con l'aggiunta di _S, ed id_old 
              che sar impostato ad S (Attenzione! S  il system id di R).
              A questo punto, le eventuali modifiche ad attributi del ruolo
              verranno salvate sul record di R e tutte le tuple di documenti
              (doc_arrivo_par), le informazioni sul creatore di documenti e
              fascioli e le informazioni sulle trasmissioni che referenziano R, 
              vengono aggiornate in modo che facciano riferimento al nuovo 
              record della DPA_CORR_GLOBALI appena inserito.
              
              Fare riferimento al corpo della store per maggiori dettagli.

  ******************************************************************************/
  
  -- Nome della colonna letta dalla tabella dei metadati
  DECLARE colName VARCHAR2 (2000);
  
  -- Lista separata da , dei nomi delle colonne in cui eseguire la insert
  colNameList VARCHAR2 (4000);
  
  -- Lista separata da , dei valori da assegnare alle colonne
  colValuesList VARCHAR2 (4000);
  
  -- Selezione delle colonne della corr globali dalla tabella dei metadati
  CURSOR curColumns IS
    SELECT cname from col where tname = 'DPA_CORR_GLOBALI' order by colno asc;
      
  BEGIN OPEN curColumns;
  LOOP FETCH curColumns INTO colName;
  EXIT WHEN curColumns%NOTFOUND;
  
    -- Se la colonna  una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'
    colNameList := colNameList || ', ' || colName;
    
    CASE (colName)
        WHEN 'SYSTEM_ID' THEN
          colValuesList := colValuesList || ', ' || 'SEQ.NEXTVAL';
        WHEN 'VAR_CODICE' THEN
          colValuesList := colValuesList || ', ' || nq'#VAR_CODICE || '_' || SEQ.CURRVAL#';
        WHEN 'VAR_COD_RUBRICA' THEN
          colValuesList := colValuesList || ', ' || nq'#VAR_COD_RUBRICA || '_' || SEQ.CURRVAL#';
        WHEN 'DTA_FINE' THEN
          colValuesList := colValuesList || ', ' || 'SYSDATE';
        ELSE
          colValuesList := colValuesList || ', ' || colName;
    END CASE;
  END LOOP;
  CLOSE curColumns;
  
  colNameList := SUBSTR( colNameList, 3); 
  colValuesList := SUBSTR( colValuesList, 3); 
  
  EXECUTE IMMEDIATE 'INSERT INTO dpa_corr_globali (' || colNameList || ') ( SELECT ' || colValuesList || ' FROM dpa_corr_globali WHERE system_id = ' || idCorrGlobRole || ')';
  
  SELECT MAX(system_id) INTO oldIdCorrGlobId FROM dpa_corr_globali WHERE original_id = idCorrGlobRole;
  
  
  -- Aggiornamento dei dati relativi al nuovo ruolo e impostazione dell'id_old
  update dpa_corr_globali 
  set id_old = oldidcorrglobid,
      var_codice = newRoleCode,
      var_desc_corr = newRoleDescription,
      id_uo = newRoleUoId,
      id_tipo_ruolo = newRoleTypeId
  where system_id = idcorrglobrole;
  
  -- Cancellazione dell'id gruppo per il gruppo storicizzato
  update dpa_corr_globali 
  set id_gruppo = null 
  where  system_id = oldidcorrglobid;
  
  -- Aggiornamento degli id mittente e destinatario relativi al ruolo
  update dpa_doc_arrivo_par
  set id_mitt_dest = oldidcorrglobid
  where id_mitt_dest = idcorrglobrole;
  
  -- Aggiornamento degli id dei ruoli creatori per documenti e fascicoli
  update profile 
  set id_ruolo_creatore = oldidcorrglobid 
  where id_ruolo_creatore = idcorrglobrole;
  
  update project 
  set id_ruolo_creatore = oldidcorrglobid 
  where id_ruolo_creatore = idcorrglobrole;
  
  -- Aggiornamento delle trasmissioni
  update dpa_trasmissione
  set id_ruolo_in_uo = oldidcorrglobid
  where id_ruolo_in_uo = idcorrglobrole;
  
  update dpa_trasm_singola
  set id_corr_globale = oldidcorrglobid
  where id_corr_globale= idcorrglobrole;


  returnValue := 1;
  --EXCEPTION
  --  WHEN OTHERS THEN
  --    retunValue := - 1;
  
END;   
END HistoricizeRole;
/              
------------------
              
---- insert_DPA_DOCSPA.ORA.sql  marcatore per ricerca ----
begin
Insert into @db_user.DPA_DOCSPA (SYSTEM_ID, DTA_UPDATE, ID_VERSIONS_U)
 Values    (seq.nextval, sysdate, '3.16.9');
end;
/