--------------------------------------------------------
--  DDL for Procedure INSERTREGISTROREPERTORIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INSERTREGISTROREPERTORIO" (
  -- Id della tipologia
  tipologyId  Number,
  -- Id del contatore
  counterId Number,
  -- Tipo di contatore
  counterType char,
  -- Categoria della tipologia documentale cui appartirene il
  -- contatore da inserire
  tipologyKind char
) As
Begin
  /******************************************************************************

    AUTHOR:   Samuele Furnari

    NAME:     InsertRegistroRepertorio
    
    PURPOSE:  Store per l'inserimento di un registro di repertorio nell'anagrafica            

  ******************************************************************************/

  -- Cursore sui registri
  Declare Cursor cursorRegistries Is (
    Select system_id 
    From dpa_el_registri 
    Where cha_rf Is Null Or cha_rf = '0'
  );
  
  -- Cursore sugli RF
  Cursor cursorRf Is (
    Select system_id
    From INFOTN_COLL.dpa_el_registri
    Where cha_rf = '1'
  );  
  
  -- Id del registro / Rf
  registryRfId  Number;
  
  Begin
    Case (counterType)
      When 'T' Then
        -- Se il contatore  di tipologia, viene inserita una sola riga nell'anagrafica
        Insert Into INFOTN_COLL.dpa_registri_repertorio (
          TipologyId,
          CounterId,
          CounterState,
          SettingsType,
          RegistryId,
          RfId,
          RoleRespId,
          PrinterRoleRespId,
          PrinterUserRespId,
          PrintFreq,
          TipologyKind,
          DtaStart,
          DtaFinish,
          DtaNextAutomaticPrint,
          DtaLastPrint,
          LastPrintedNumber,
          Resprights
        )
        Values(
          tipologyId,
          counterId,
          'O',
          'G',
          null,
          null,
          null,
          null,
          null,
          'N',
          tipologyKind,
          null,
          null,
          null,
          null,
          0,
          'R');
      When 'A' Then
        -- Se  di AOO vengono inserite tante voci quanti sono i registri
        Begin
          Begin Open cursorRegistries;
          Loop Fetch cursorRegistries INTO registryRfId;
          EXIT WHEN cursorRegistries%NOTFOUND;
            Insert Into INFOTN_COLL.dpa_registri_repertorio (
              TipologyId,
              CounterId,
              CounterState,
              SettingsType,
              RegistryId,
              RfId,
              RoleRespId,
              PrinterRoleRespId,
              PrinterUserRespId,
              PrintFreq,
              TipologyKind,
              DtaStart,
              DtaFinish,
              DtaNextAutomaticPrint,
              DtaLastPrint,
              LastPrintedNumber,
              resprights
            )
            Values(
              tipologyId,
              counterId,
              'O',
              'G',
              registryRfId,
              null,
              null,
              null,
              null,
              'N',
              tipologyKind,
              null,
              null,
              null,
              null,
              0,
              'R'
            );
            End loop;  
            Close cursorRegistries;
          End;  
        End;  
      When 'R' Then
        -- Se  di RF vengono inserite tante voci quanti sono gli RF
        Begin
          BEGIN OPEN cursorRf;
            LOOP FETCH cursorRf INTO registryRfId;
            EXIT WHEN cursorRf%NOTFOUND;
            Insert Into INFOTN_COLL.dpa_registri_repertorio (
              TipologyId,
              CounterId,
              CounterState,
              SettingsType,
              RegistryId,
              RfId,
              RoleRespId,
              PrinterRoleRespId,
              PrinterUserRespId,
              PrintFreq,
              TipologyKind,
              DtaStart,
              DtaFinish,
              DtaNextAutomaticPrint,
              DtaLastPrint,
              LastPrintedNumber,
              resprights
            )
          Values (
              tipologyId,
              counterId,
              'O',
              'G',
              null,
              registryRfId,
              null,
              null,
              null,
              'N',
              tipologyKind,
              null,
              null,
              null,
              null,
              0,
              'R'
            );
          End loop;
          Close cursorRf;
        End;  
        End;  
    End Case;
  End;
  End Insertregistrorepertorio; 

/
