--------------------------------------------------------
--  DDL for Procedure INITREGREPERTORIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INITREGREPERTORIO" AS 
BEGIN
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     InitRegRepertorio

    PURPOSE:  Store per l'inizializzazione della tabella dell'anagrafica dei 
              registri di repertorio.

  ******************************************************************************/
  
  -- Preventiva pulizia dell'anagrafica dei registri di repertorio
  Delete From dpa_registri_repertorio;
  
  -- Id della tipologia, id del contatore, tipo di contatore definito e id del registro
  Declare tipologyId int;
          counterId int;
          counterType char;
          registryRfId int;
  
  Begin
      -- Cursore per scrorrere le informazioni sui contatori definiti per le tipologie
      -- documento
      Declare Cursor cursorCounters Is (
        Select 
        ta.system_id TipologyId,
        oc.system_id as CounterId,
        oc.cha_tipo_tar as CounterType
        From dpa_tipo_atto ta
        Inner Join dpa_ogg_custom_comp occ
        On ta.system_id = occ.id_template
        Inner Join dpa_oggetti_custom oc
        On occ.id_ogg_custom = oc.system_id
        Inner Join dpa_tipo_oggetto tobj
        On oc.id_tipo_oggetto = tobj.system_id
        Where lower(tobj.descrizione) = 'contatore'
        And oc.repertorio = 1
      );
      
      BEGIN OPEN cursorCounters;
      LOOP FETCH cursorCounters INTO tipologyId, counterId, counterType;
      EXIT WHEN cursorCounters%NOTFOUND;
        -- Inserimento delle informazioni sul registro nell'anagrafica
        InsertRegistroRepertorio(tipologyId, counterId, counterType, 'D');
    
      END LOOP;
      CLOSE cursorCounters;
    End;
    /*
    Begin
      -- Cursore per scorrere le informazioni sui contatori definiti per le tipologie
      -- fascicoli
      Declare Cursor cursorCountersFasc Is (
        Select 
        tf.system_id TipologyId,
        oc.system_id as CounterId,
        oc.cha_tipo_tar as CounterType
        From dpa_tipo_fasc tf
        Inner Join dpa_ogg_custom_comp_fasc occ
        On tf.system_id = occ.id_template
        Inner Join dpa_oggetti_custom_fasc oc
        On occ.id_ogg_custom = oc.system_id
        Inner Join dpa_tipo_oggetto_fasc tobj
        On oc.id_tipo_oggetto = tobj.system_id
        Where lower(tobj.descrizione) = 'contatore'
        And oc.repertorio = 1
      );
    
      BEGIN OPEN cursorCountersFasc;
      LOOP FETCH cursorCountersFasc INTO tipologyId, counterId, counterType;
      EXIT WHEN cursorCountersFasc%NOTFOUND;
        -- Inserimento delle informazioni sul registro nell'anagrafica
        InsertRegistroRepertorio(tipologyId, counterId, counterType, 'F');
    
      END LOOP;
      CLOSE cursorCountersFasc;
      End;  */
    End;  
  End;

/
