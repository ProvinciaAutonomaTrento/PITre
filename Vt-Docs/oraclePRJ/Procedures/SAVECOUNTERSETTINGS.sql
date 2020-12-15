--------------------------------------------------------
--  DDL for Procedure SAVECOUNTERSETTINGS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SAVECOUNTERSETTINGS" (
  -- Id del contatore
  countId   Number,
  -- Tipo di impostazioni specificato per un contatore (G o S)
  settingsType Varchar,
  -- Id del ruolo stampatore
  roleIdGroup  Number,
  -- Id dell'utente stampatore
  userIdPeople  Number,
  -- Id del ruolo responsabile
  roleRespIdGroup Number,
  -- Frequenza di stampa
  printFrequency Varchar,
  -- Data di partenza del servizio di stampa automatica
  dateAutomaticPrintStart Date,
  -- Data di stop del servizio di stampa automatica
  dateAutomaticPrintFinish  Date,
  -- Data prevista per la prossima stampa automatica
  dateNextAutomaticPrint  Date,
  -- Id del registro cui si riferiscono le impostazioni da salvare
  reg Number,
  -- Id dell'RF cui si riferscono le impostazioni da salvare
  rf Number,
  -- Sigla identificativa della tipologia in cui  definito il contatore (D, F)
  tipology Varchar,
  -- Stato del contatore di repertorio (O, C)
  state Varchar,
  -- Diritti da concedere al responsabile (R o RW)
  rights Varchar,
  -- Valore di ritorno
  returnValue Out Number
) AS BEGIN
  /******************************************************************************
  
    AUTHOR:   Samuele Furnari

    NAME:     SaveCounterSettings

    PURPOSE:  Store per il salvataggio delle modifiche apportate alle impostazioni
              di stampa per un determinato contatore di repertorio

  ******************************************************************************/
 
  -- Tipologia di impostazioni impostata per il contatore
  Declare actualSettingsType Char;
  
  Begin
    -- Se il tipo di impostazione scelta  G, vengono aggiornate le properiet per tutte le istanze
    -- del contatore counterId
    IF settingsType = 'G' THEN
      Begin
        Update dpa_registri_repertorio
          Set SettingsType = 'G',
          PrinterRoleRespId = roleidgroup,
          PrinterUserRespId = useridPeople,
          RoleRespId = rolerespidgroup,
          PrintFreq = printFrequency,
          DtaStart = dateAutomaticPrintStart,
          DtaFinish = dateAutomaticPrintFinish,
          DtaNextAutomaticPrint = dateNextAutomaticPrint,
          CounterState = state,
          Resprights = rights
        Where CounterId = countId And TipologyKind = tipology;  
      End;
    Else
      -- Altrimenti, se prima il tipo di impostazioni era G, vengono aggiornate tutte
      -- le istanze del contatore ad S ed in seguito vengono salvare le informazioni 
      -- per la specifica istanza specificata
      -- da registro / RF specificato
      Begin
        -- Valorizzazione corretta per l'id gruppo del ruolo responsabile
        Declare decodedRoleRespIdGroup Varchar (100);
        -- Valorizzazione corretta per l'id gruppo dello stampatore
        decodedRoleIdGroup Varchar (100);
        -- Valorizzazione corretta per l'id utente dello stampatore
        decodedUserIdPeople Varchar (100);
        
        Begin
        
          Select SettingsType Into actualSettingsType From dpa_registri_repertorio Where counterId = countId And RowNum = 1;
          
          If actualSettingsType != SettingsType And SettingsType = 'S' Then
            Update dpa_registri_repertorio
              Set SettingsType = 'S'/*,
              RoleRespId = null,
              PrinterRoleRespId = null,
              PrinterUserRespId = null,
              PrintFreq = 'N',
              DtaStart = null,
              DtaFinish = null,
              DtaNextAutomaticPrint = null,
              CounterState = 'O'*/
            Where CounterId = countId And TipologyKind = tipology; 
          End If;  
          
          If roleRespIdGroup is null Then
            decodedRoleRespIdGroup := 'null';
          Else
            decodedRoleRespIdGroup := roleRespIdGroup;
          End If;
          
          If roleIdGroup is null Then
            decodedRoleIdGroup := 'null';
          Else
            decodedRoleIdGroup := roleIdGroup;
          End If;
          
          If userIdPeople is null Then
            decodedUserIdPeople := 'null';
          Else
            decodedUserIdPeople := userIdPeople;
          End If;  
          
          Declare updateQuery varchar (2000) := 
              'Update dpa_registri_repertorio
              Set RoleRespId = ' || decodedRoleRespIdGroup || ',
              PrinterRoleRespId = ' || decodedRoleIdGroup || ',
              PrinterUserRespId = ' || decodedUserIdPeople || ',
              PrintFreq = ''' || printFrequency || ''',
              DtaStart = to_date(''' || to_char(dateAutomaticPrintStart, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              DtaFinish = to_date(''' || to_char(dateAutomaticPrintFinish, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              DtaNextAutomaticPrint = to_date(''' || to_char(dateNextAutomaticPrint, 'dd/MM/yyyy') || ''', ''dd/MM/yyyy''),
              CounterState = ''' || state || ''',
              Resprights = ''' || rights || '''
            Where CounterId = ' || countId || ' And TipologyKind = ''' || tipology || '''
              And ';
            
          Begin
            IF reg is not null And to_number(reg) > 0  THEN
              updateQuery := updateQuery || ' RegistryId = ' || reg || ' And ';
            Else
              updateQuery := updateQuery || ' RegistryId is null And';
            END IF ;
          
            IF rf is not null And to_number(rf) > 0  THEN
              updateQuery := updateQuery || ' RfId = ' || rf;
            Else
              updateQuery := updateQuery || ' RfId is null';
            END IF ;
            
            execute immediate updateQuery;
          End;  
        End;
      End;
    END IF ;
    
    -- Impostazione del valore di ritorno
    returnValue := 1;
  End;  
END SaveCounterSettings; 

/
