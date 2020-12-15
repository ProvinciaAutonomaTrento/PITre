--------------------------------------------------------
--  DDL for Procedure EXTENDVISIBILITYTOHIGHERROLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."EXTENDVISIBILITYTOHIGHERROLES" (
  -- Id dell'amministrazione 
  idAmm IN INTEGER,
  -- Id del gruppo da analizzare e di cui estendere la visibilit
  idGroup IN INTEGER,
  -- Indicatore dello scope della procedura di estensione di visibilit
  -- A -> Tutti, E -> Esclusione degli atipici
  extendScope IN VARCHAR,
  -- Flag utilizzato per indicare se bisogna copiare gli id di documenti e
  -- fascicoli nella tabella temporanea per l'allineamento asincrono con Documentum
  copyIdToTempTable IN INTEGER,
  returnValue OUT INTEGER
) AS 
BEGIN
   /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     ExtendVisibilityToHigherRoles

    PURPOSE:  Store per l'estensione della visibilit ai ruoli superiori. 

  ******************************************************************************/

  -- Selezione dei superiori dei ruolo
  DECLARE CURSOR higherRoles IS
    SELECT
      dpa_corr_globali.id_gruppo
      FROM dpa_corr_globali
      INNER JOIN dpa_tipo_ruolo
      ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
      WHERE dpa_corr_globali.id_uo IN
      (SELECT dpa_corr_globali.system_id
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.dta_fine IS NULL
      CONNECT BY PRIOR dpa_corr_globali.id_parent = dpa_corr_globali.system_id
      START WITH dpa_corr_globali.system_id =
      (SELECT dpa_corr_globali.id_uo
      FROM dpa_corr_globali
      WHERE dpa_corr_globali.id_gruppo = idGroup
      )
      )
      AND dpa_corr_globali.CHA_TIPO_URP = 'R'
      AND dpa_corr_globali.ID_AMM = idAmm
      AND dpa_corr_globali.DTA_FINE IS NULL
      AND dpa_tipo_ruolo.num_livello <
      (SELECT dpa_tipo_ruolo.num_livello
      FROM dpa_corr_globali
      INNER JOIN dpa_tipo_ruolo
      ON dpa_corr_globali.id_tipo_ruolo = dpa_tipo_ruolo.system_id
      WHERE dpa_corr_globali.id_gruppo  = idGroup
      );
      
  
  -- Id gruppo da analizzare
  idGroupToAnalyze  INTEGER;
  
  BEGIN OPEN higherRoles;
  LOOP FETCH higherRoles INTO idGroupToAnalyze;
  EXIT WHEN higherRoles%NOTFOUND;  
  
    -- Per ogni ruolo superiore, viene effettuata una operazione diversa 
    -- a seconda del tipo di estensione da eseguire
    CASE (extendScope)
        WHEN 'A' THEN
          CopySecurity(idGroup, idGroupToAnalyze);
        WHEN 'E' THEN
          CopySecurityWithoutAtipici(idGroup, idGroupToAnalyze);
    END CASE;
    
    -- Se richiesto, viene aggiornata la tabella delle sincronizzazione pending
    IF copyIdToTempTable = 1 THEN
      BEGIN
        INSERT INTO dpa_objects_sync_pending
        (
          id_doc_or_fasc,
          type,
          id_gruppo_to_sync
        )
        (
          SELECT p.system_id, 'D', idGroupToAnalyze
          FROM security s 
          Inner Join Profile P On P.System_Id = S.Thing 
          And (P.Cha_Privato Is Null Or P.Cha_Privato != 0)
          And (P.Cha_Personale is null Or P.Cha_Personale != 0)
          WHERE personorgroup = idGroupToAnalyze and 
          not exists( select 'x' 
            from dpa_objects_sync_pending sp 
            where sp.id_doc_or_fasc = s.thing AND
                  sp.id_gruppo_to_sync = idGroupToAnalyze
            ));
        
        INSERT INTO dpa_objects_sync_pending
        (
          id_doc_or_fasc,
          type,
          id_gruppo_to_sync
        )
        (
          SELECT p.system_id, 'F', idGroupToAnalyze
          FROM security s 
          INNER JOIN project p ON p.system_id = s.thing 
          Where Personorgroup = Idgrouptoanalyze And 
          (P.Cha_Privato Is Null Or P.Cha_Privato != 0) And
          not exists( select 'x' 
            from dpa_objects_sync_pending sp 
            where sp.id_doc_or_fasc = s.thing AND
                  sp.id_gruppo_to_sync = idGroupToAnalyze
          ));
      END;
    END IF;
  
  END LOOP;
  CLOSE higherRoles;

  returnValue := 0;
END;
END ExtendVisibilityToHigherRoles;

/
