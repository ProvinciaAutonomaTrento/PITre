USE [NOME_DATABASE]
GO

/****** Object:  StoredProcedure [DOCSADM].[SaveCounterSettings]    Script Date: 05/05/2016 15:30:10 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[SaveCounterSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE [DOCSADM].[SaveCounterSettings]
GO

USE [invalsiTest]
GO

/****** Object:  StoredProcedure [DOCSADM].[SaveCounterSettings]    Script Date: 05/05/2016 15:30:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [DOCSADM].[SaveCounterSettings]
-- Id del contatore
@countId   INT,
-- Tipo di impostazioni specificato per un contatore (G o S)
@settingsType VARCHAR(4000),
-- Id del ruolo stampatore
@roleIdGroup  INT,
-- Id dell'utente stampatore
@userIdPeople  INT,
-- Id del ruolo responsabile
@roleRespIdGroup INT,
-- Frequenza di stampa
@printFrequency VARCHAR(4000),
-- Data di partenza del servizio di stampa automatica
@dateAutomaticPrintStart DATETIME,
-- Data di stop del servizio di stampa automatica
@dateAutomaticPrintFinish  DATETIME,
-- Data prevista per la prossima stampa automatica
@dateNextAutomaticPrint  DATETIME,
-- Id del registro cui si riferiscono le impostazioni da salvare
@reg INT,
-- Id dell'RF cui si riferscono le impostazioni da salvare
@rf INT,
-- Sigla identificativa della tipologia in cui  definito il contatore (D, F)
@tipology VARCHAR(4000),
-- Stato del contatore di repertorio (O, C)
@state VARCHAR(4000),
-- Diritti da concedere al responsabile (R o RW)
@rights VARCHAR(4000)
--,
-- Valore di ritorno
--@returnValue INT OUTPUT  
AS
BEGIN
/******************************************************************************

AUTHOR:   Samuele Furnari

NAME:     SaveCounterSettings

PURPOSE:  Store per il salvataggio delle modifiche apportate alle impostazioni
di stampa per un determinato contatore di repertorio

******************************************************************************/
-- NEW
IF (CONVERT( VARCHAR(30), @dateAutomaticPrintStart, 103) = '01/01/0001')
SET @dateAutomaticPrintStart = NULL

IF (CONVERT( VARCHAR(30), @dateAutomaticPrintFinish, 103) = '01/01/0001')
SET @dateAutomaticPrintFinish = NULL

IF (CONVERT( VARCHAR(30),@dateNextAutomaticPrint, 103) = '01/01/0001')
SET @dateNextAutomaticPrint = NULL

-- Tipologia di impostazioni impostata per il contatore


-- Se il tipo di impostazione scelta  G, vengono aggiornate le properiet per tutte le istanze
-- del contatore counterId
DECLARE @actualSettingsType CHAR
DECLARE @outvalue INT

IF @settingsType = 'G'
Begin
update DOCSADM.dpa_registri_repertorio set @settingsType = 'G',PrinterRoleRespId = @roleIdGroup,PrinterUserRespId = @userIdPeople,
RoleRespId = @roleRespIdGroup,PrintFreq = @printFrequency,
DtaStart = @dateAutomaticPrintStart,
DtaFinish = @dateAutomaticPrintFinish,
DtaNextAutomaticPrint = @dateNextAutomaticPrint,
CounterState = @state,
Resprights = @rights  Where CounterId = @countId And TipologyKind = @tipology
End
Else
-- Altrimenti, se prima il tipo di impostazioni era G, vengono aggiornate tutte
-- le istanze del contatore ad S ed in seguito vengono salvare le informazioni
-- per la specifica istanza specificata
-- da registro / RF specificato
Begin
-- Valorizzazione corretta per l'id gruppo del ruolo responsabile
DECLARE @decodedRoleRespIdGroup VARCHAR(100)
-- Valorizzazione corretta per l'id gruppo dello stampatore
DECLARE @decodedRoleIdGroup VARCHAR(100)
-- Valorizzazione corretta per l'id utente dello stampatore
DECLARE @decodedUserIdPeople VARCHAR(100)
Select  TOP 1 @actualSettingsType = @settingsType From DOCSADM.dpa_registri_repertorio
Where counterId = @countId
If @actualSettingsType != @settingsType And @settingsType = 'S'
/*,
RoleRespId = null,
PrinterRoleRespId = null,
PrinterUserRespId = null,
PrintFreq = 'N',
DtaStart = null,
DtaFinish = null,
DtaNextAutomaticPrint = null,
CounterState = 'O'*/
update DOCSADM.dpa_registri_repertorio set @settingsType = 'S'
Where CounterId = @countId And TipologyKind = @tipology

If @roleRespIdGroup is null
SET @decodedRoleRespIdGroup = 'null'
Else
SET @decodedRoleRespIdGroup = @roleRespIdGroup

If @roleIdGroup is null
SET @decodedRoleIdGroup = 'null'
Else
SET @decodedRoleIdGroup = @roleIdGroup

If @userIdPeople is null
SET @decodedUserIdPeople = 'null'
Else
SET @decodedUserIdPeople = @userIdPeople

Begin
DECLARE @updateQuery VARCHAR(2000) 
set @updateQuery = 'Update DOCSADM.dpa_registri_repertorio
Set RoleRespId = ' + @decodedRoleRespIdGroup + ',
PrinterRoleRespId = ' + @decodedRoleIdGroup + ',
PrinterUserRespId = ' + @decodedUserIdPeople + ',
PrintFreq ''' + @printFrequency + ''',
DtaStart = ' + @dateAutomaticPrintStart + ',
DtaFinish = ' + @dateAutomaticPrintFinish + ',
DtaNextAutomaticPrint = ' + @dateNextAutomaticPrint + ',
CounterState = ''' + @state + ''',
Resprights = ''' + @rights + ''',
Where CounterId = ' + @countId + '
And TipologyKind = ''' + @tipology + '''
And '
IF @reg is not null And CAST(@reg as INT) > 0
SET @updateQuery = @updateQuery + ' RegistryId = ' + @reg + ' And '
Else
SET @updateQuery = @updateQuery + 'RegistryId is null And'

IF @rf is not null And CAST(@rf as INT) > 0
SET @updateQuery = @updateQuery + ' RfId = ' + @rf
Else
SET @updateQuery = @updateQuery + ' RfId is null'

execute(@updateQuery)
End

End


-- Impostazione del valore di ritorno
SET @outvalue = 1
RETURN @outvalue

END




GO

