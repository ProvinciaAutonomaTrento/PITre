--------------------------------------------------------
--  DDL for Procedure HISTORICIZEROLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."HISTORICIZEROLE" 
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
              che si differenzia da quella di R solo per S che sara un nuovo 
              numero assegnato dalla sequence, var_codice e var_cod_rubrica
              che saranno uguali a quelli di R con l'aggiunta di _S, ed id_old 
              che sara impostato ad S (Attenzione! S e il system id di R).
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
  
    -- Se la colonna e una colonna di quelle che deve eesere modificata, viene 
    -- inserito il valore modificato altrimenti viene lasciata com'e
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
  
  --per le policy di conservazione
  update policy_conservazione pc set pc.ID_RUOLO=oldIdCorrGlobId where id_ruolo=idCorrGlobRole;
  
  
  
  -- Aggiornamento corrispondenti dei campi profilati
  update DPA_ASSOCIAZIONE_TEMPLATES ASSTEMP
  SET assTemp.VALORE_OGGETTO_DB = OLDIDCORRGLOBID 
  WHERE EXISTS
    (SELECT 'x'
    FROM DPA_OGGETTI_CUSTOM OC
    WHERE oc.ID_TIPO_OGGETTO =
      (SELECT tObj.SYSTEM_ID
      FROM DPA_TIPO_OGGETTO TOBJ
      WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente'
      )
    AND OC.SYSTEM_ID = ASSTEMP.ID_OGGETTO
    )
  AND ASSTEMP.VALORE_OGGETTO_DB = '' || IDCORRGLOBROLE || '';

  update DPA_ASS_TEMPLATES_FASC ASSTEMP
  SET assTemp.VALORE_OGGETTO_DB = OLDIDCORRGLOBID 
  WHERE EXISTS
    (SELECT 'x'
    FROM DPA_OGGETTI_CUSTOM_FASC OC
    WHERE oc.ID_TIPO_OGGETTO =
      (SELECT tObj.SYSTEM_ID
      FROM DPA_TIPO_OGGETTO_FASC TOBJ
      WHERE LOWER(tObj.DESCRIZIONE) = 'corrispondente'
      )
    AND OC.SYSTEM_ID = ASSTEMP.ID_OGGETTO
    )
  AND ASSTEMP.VALORE_OGGETTO_DB = '' || IDCORRGLOBROLE || '';



  returnValue := 1;
  --EXCEPTION
  --  WHEN OTHERS THEN
  --    retunValue := - 1;
  
END;   
END HistoricizeRole; 

/
