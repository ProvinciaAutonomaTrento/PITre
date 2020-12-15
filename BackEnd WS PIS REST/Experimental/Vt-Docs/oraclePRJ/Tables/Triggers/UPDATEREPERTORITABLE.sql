--------------------------------------------------------
--  DDL for Trigger UPDATEREPERTORITABLE
--------------------------------------------------------

  CREATE OR REPLACE TRIGGER "ITCOLL_6GIU12"."UPDATEREPERTORITABLE" After
  Insert On Dpa_El_Registri For Each Row Begin
    /******************************************************************************
      AUTHOR:    Samuele Furnari
      NAME:      UpdateRepertoriTable
      PURPOSE:   Questo trigger in ascolto sulla dpa_el_registri scatta ogni volta
                 che viene aggiunto un registro o un RF e serve per aggiungere ad
                 ogni contatore di repertorio di tipo AOO / RF, un riferimento
                 al nuovo registro / RF
    ******************************************************************************/
    -- Cursore per scorrere tutti i repertori di RF
    Declare Cursor RepertoriCursor (Mycha_Tipo_Tar Varchar) Is
    (Select Oc.System_Id
    From Dpa_Oggetti_Custom Oc
    Where Repertorio     = '1'
    And Cha_Tipo_Tar     = Mycha_Tipo_Tar -- 'R' o 'A'
    And Id_Tipo_Oggetto In
      (Select System_Id
      From Dpa_Tipo_Oggetto
      Where Lower(Descrizione) = 'contatore'
      )
    );
  -- Id del repertorio
  RepId Number;
  -- Tipo di impostazioni scelte per lo specifico repertorio
  SettType Varchar (1);
  -- Id del registro e dell'RF
  Registry Number;
  Rf       Number;
  -- Sigla identificativa del tipo di repertorio da modificare (A o R)
  RepType Varchar (1);
  Begin
    -- Inizializzazione del cursore per scorrere i repertori a seconda
    -- che si stia inserendo un RF o un Registro
    If :New.Cha_Rf = '1' Then
      RepType     := 'R';
    Else
      RepType := 'A';
    End If ;
    Begin
      Open RepertoriCursor(RepType);
      Loop
        Fetch RepertoriCursor Into RepId;
        Exit
      When RepertoriCursor%NotFound;
        -- Se si sta inserendo un registro viene inizializzato
        -- il parametro registry altrimenti viene valorizzato il
        -- parametro rf
        If :New.Cha_Rf = '0' Then
          Begin
            Registry := :New.System_Id;
            Rf       := Null;
          End;
        Else
          Begin
            Registry := Null;
            Rf       := :New.System_Id;
          End;
        End If ;
        -- Selezione delle impostazioni relative al contatore in esame
        -- (viene prelevata la prima istanza in quanto una qualsiasi istanza
        -- e sufficiente per determinare come procedere
        Begin
          Select SettingsType
          Into SettType
          From Dpa_Registri_Repertorio
          Where CounterId = RepId
          And Rownum      = 1;
        Exception
        When Others Then
          SettType := '';
        End;
        Begin
          If SettType = 'G' Then
            Begin
              -- Se il tipo di impostazione e G, viene inserita nell'anagrafica una riga uguale
              -- alla prima impostazione del repertorio con la data di ultima stampa impostata a null
              -- e con l'ultimo numero stampato impostato a 0
              Insert
              Into Dpa_Registri_Repertorio
                (
                  Tipologyid,
                  Counterid,
                  Counterstate,
                  Settingstype,
                  Registryid,
                  Rfid,
                  Rolerespid,
                  Printerrolerespid,
                  Printeruserrespid,
                  Printfreq,
                  Tipologykind,
                  Dtastart,
                  Dtafinish,
                  Dtanextautomaticprint,
                  Dtalastprint,
                  Lastprintednumber,
                  Resprights
                )
                (Select Tipologyid,
                    CounterId,
                    CounterState,
                    SettingsType,
                    Registry,
                    Rf,
                    RolerespId,
                    PrinterRoleRespId,
                    PrinterUserRespId,
                    PrintFreq,
                    TipologyKind,
                    DtaStart,
                    DtaFinish,
                    DtaNextAutomaticPrint,
                    Null,
                    0,
                    RespRights
                  From Dpa_Registri_Repertorio
                  Where CounterId = RepId
                  And Rownum      = 1
                );
            Exception
            When Others Then
              SettType := '';
            End;
          Else
            Begin
              If SettType = 'S' Then
                Begin
                  -- Altrimenti le impostazioni sono singole per ogni repertorio.
                  -- In questo caso, viene inserita una riga uguale alla prima istanta
                  -- di configurazione relativa al repertorio ad esclusione di:
                  --    - stato che viene impostato ad Aperto
                  --    - rfid / registry id che vengono impostati a seconda del registro inserito
                  --    - ultimo numero stampato che viene impostato a 0
                  --    - date che vengono impostate tutte a null
                  --    - respondabili che vengono impostati a null
                  --    - frequenza di stampa automatica che viene impostata ad N
                  Insert
                  Into Dpa_Registri_Repertorio
                    (
                      Tipologyid,
                      Counterid,
                      Counterstate,
                      Settingstype,
                      Registryid,
                      Rfid,
                      Rolerespid,
                      Printerrolerespid,
                      Printeruserrespid,
                      Printfreq,
                      Tipologykind,
                      Dtastart,
                      Dtafinish,
                      Dtanextautomaticprint,
                      Dtalastprint,
                      Lastprintednumber,
                      Resprights
                    )
                    (Select Tipologyid,
                        Counterid,
                        Counterstate,
                        Settingstype,
                        Registry,
                        Rf,
                        Null,
                        Null,
                        Null,
                        'N',
                        Tipologykind,
                        Null,
                        Null,
                        Null,
                        Null,
                        0,
                        Resprights
                      From Dpa_Registri_Repertorio
                      Where Counterid = Repid
                      And Rownum      = 1
                    );
                Exception
                When Others Then
                  Setttype := '';
                End;
              End If;
            End;
          End If;
        End;
      End Loop;
    End;
    Close Repertoricursor;
  End;
End;
/
ALTER TRIGGER "ITCOLL_6GIU12"."UPDATEREPERTORITABLE" ENABLE;
