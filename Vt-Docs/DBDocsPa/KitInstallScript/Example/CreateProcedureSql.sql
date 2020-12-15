if exists (select * from dbo.sysobjects where id = object_id(N'[@db_user].[I_SMISTAMENTO_SMISTADOC]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [@db_user].[I_SMISTAMENTO_SMISTADOC]
GO
CREATE  PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC]
      @IDPeopleMittente int,
      @IDCorrGlobaleRuoloMittente int,
      @IDGruppoMittente int,
      @IDAmministrazioneMittente int,
      @IDPeopleDestinatario int,
      @IDCorrGlobaleDestinatario int,
      @IDDocumento int,
      @FlagCompetenza bit, -- Competenza=1, CC=0
      @IDTrasmissioneUtenteMittente int,
      @TrasmissioneConWorkflow bit, -- 1=WORKFLOW, 0=ALTRIMENTI
      @NoteGeneraliDocumento varchar(250) 
AS
DECLARE @IDRagioneTrasm int
DECLARE @Rights int
DECLARE @DescrizioneRagione varchar(32)
 IF (@FlagCompetenza=1)
      BEGIN
            SET @DescrizioneRagione='COMPETENZA'
            SET @Rights = 63
      END
ELSE
      BEGIN
            SET @DescrizioneRagione='CONOSCENZA'
            SET @Rights = 45
      END
SET @IDRagioneTrasm=(SELECT SYSTEM_ID 
                 FROM   DPA_RAGIONE_TRASM
                 WHERE ID_AMM=@IDAmministrazioneMittente AND
                        VAR_DESC_RAGIONE=@DescrizioneRagione)
DECLARE @ReturnValue int
IF (NOT @IDRagioneTrasm IS NULL)
      BEGIN
            DECLARE @Identity int
           INSERT INTO DPA_TRASMISSIONE
            (
                  ID_RUOLO_IN_UO,
                  ID_PEOPLE,
                  CHA_TIPO_OGGETTO,
                 ID_PROFILE,
                  ID_PROJECT,
                  DTA_INVIO,
                  VAR_NOTE_GENERALI            
            )
            VALUES
            (
                  @IDCorrGlobaleRuoloMittente,
                  @IDPeopleMittente,
                  'D',  
                  @IDDocumento,
                  NULL,
                  GETDATE(),
                  NULL
            )
        IF (@@ROWCOUNT = 0)
                  BEGIN
                        SET @ReturnValue=2
                  END
            ELSE
                  BEGIN                   
                        SET @Identity=scope_identity()
                        INSERT INTO DPA_TRASM_SINGOLA
                        (
                             ID_RAGIONE,
                             ID_TRASMISSIONE,
                             CHA_TIPO_DEST,
                             ID_CORR_GLOBALE,
                             VAR_NOTE_SING,
                             CHA_TIPO_TRASM,
                             DTA_SCADENZA,
                             ID_TRASM_UTENTE
                        )
                        VALUES
                        (
                             @IDRagioneTrasm,
                             @Identity,
                             'U',
                             @IDCorrGlobaleDestinatario,
                             NULL,
                             'S',
                             NULL,
                             NULL
                        )           
                        IF (@@ROWCOUNT = 0)
                              BEGIN
                                   SET @ReturnValue=3
                             END
                        ELSE        
                             BEGIN
                                   SET @Identity=scope_identity()
                              INSERT INTO DPA_TRASM_UTENTE
                                   (
                                         ID_TRASM_SINGOLA,
                                         ID_PEOPLE,
                                         DTA_VISTA,
                                         DTA_ACCETTATA,
                                         DTA_RIFIUTATA,
                                         DTA_RISPOSTA,
                                         CHA_VISTA,
                                         CHA_ACCETTATA,
                                         CHA_RIFIUTATA,
                                         VAR_NOTE_ACC,
                                         VAR_NOTE_RIF,
                                         CHA_VALIDA,
                                         ID_TRASM_RISP_SING
                                   )
                                   VALUES
                                   (
                                         @Identity,
                                         @IDPeopleDestinatario,
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
                                   )
                                    IF (@@ROWCOUNT = 0)
                                         BEGIN
                                               SET @ReturnValue=4
                                         END
                                   ELSE                                     
                                         BEGIN
                                         DECLARE @AccessRights int
                                         SET @AccessRights=
                                               (           
                                                     SELECT      MAX(ACCESSRIGHTS)
                                                     FROM SECURITY 
                                                     WHERE       THING=@IDDocumento AND
                                                           PERSONORGROUP=@IDPeopleDestinatario AND
                                                           CHA_TIPO_DIRITTO='T'
                                               )
                                         IF (NOT @AccessRights IS NULL)
                                               BEGIN
                                                     IF (@AccessRights < @Rights)
                                                           UPDATE      SECURITY
                                                           SET   ACCESSRIGHTS=@Rights
                                                           WHERE       THING=@IDDocumento AND
                                                                 PERSONORGROUP=@IDPeopleDestinatario AND
                                                                 CHA_TIPO_DIRITTO='T' AND
                                                                 ACCESSRIGHTS=@AccessRights
                                               END
                                         ELSE
                                               BEGIN
                                                     INSERT INTO SECURITY
                                                     (
                                                           THING,
                                                           PERSONORGROUP,
                                                           ACCESSRIGHTS,
                                                           ID_GRUPPO_TRASM,
                                                           CHA_TIPO_DIRITTO
                                                     )
                                                     VALUES
                                                     (
                                                          @IDDocumento,
                                                           @IDPeopleDestinatario,
                                                           @Rights,
                                                           @IDGruppoMittente,
                                                           'T'                                       
                                                     )
                                               END         
                
                                         IF (@TrasmissioneConWorkflow='1')
                                               UPDATE      DPA_TRASM_UTENTE
                                               SET   DTA_VISTA=GETDATE(),
                                                     CHA_VISTA='1',
                                                     DTA_ACCETTATA=GETDATE(),
                                                     CHA_ACCETTATA='1',
                                                     VAR_NOTE_ACC='Documento accettato e smistato'
                                               WHERE SYSTEM_ID=@IDTrasmissioneUtenteMittente
                                         ELSE                                                                        
                                               UPDATE      DPA_TRASM_UTENTE
                                               SET   DTA_VISTA = GETDATE(),
                                                     CHA_VISTA = '1'
                                               WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente;
                                         IF ((SELECT    CHA_TIPO_TRASM 
                                                FROM          DPA_TRASM_SINGOLA
                                                WHERE         SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente)
                                            )='S' AND @TrasmissioneConWorkflow='1')
                                                UPDATE      DPA_TRASM_UTENTE
                                               SET   CHA_VALIDA = '0'
                                               WHERE ID_TRASM_SINGOLA = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente)
                                               AND     SYSTEM_ID NOT IN (@IDTrasmissioneUtenteMittente)
                                         SET @ReturnValue=0
                                   END
                             END
                        END
      END
ELSE
      BEGIN
            SET @ReturnValue=1
      END
RETURN @ReturnValue
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
