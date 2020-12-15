declare @ultimoID int

begin

Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ( 'AMM_LOGIN', 'Accesso Admin all''applicazione', 'UTENTE', 'AMM_LOGIN')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)


Insert into [@db_user].DPA_ANAGRAFICA_LOG
   ( VAR_CODICE, VAR_DESCRIZIONE, VAR_OGGETTO, VAR_METODO)
Values
   ('AMM_LOGOFF', 'Uscita Admin dall''applicazione', 'UTENTE', 'AMM_LOGOFF')

SELECT @ultimoID = SCOPE_IDENTITY() 
Insert into [@db_user].DPA_LOG_ATTIVATI(system_id_anagrafica, id_amm) values (@ultimoID, 0)

END
GO


if not exists (SELECT * FROM syscolumns WHERE name='CHA_DISABLED_TRASM' and id in (SELECT id FROM sysobjects WHERE name='DPA_CORR_GLOBALI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_CORR_GLOBALI] ADD CHA_DISABLED_TRASM VARCHAR(1)
END
GO

if not exists (SELECT * FROM syscolumns
	WHERE name='NON_RICERCABILE'
	and id in (SELECT id FROM sysobjects
	WHERE name='DPA_STATI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_STATI] ADD NON_RICERCABILE INTEGER
END
GO

if not exists (SELECT * FROM syscolumns WHERE name='NON_RICERCABILE'
	and id in (SELECT id FROM sysobjects WHERE name='DPA_STATI' and xtype='U'))
BEGIN
	ALTER TABLE [@db_user].[DPA_STATI] ADD NON_RICERCABILE INTEGER;
END
GO

if not exists (
		 select * from dbo.sysindexes
		 where name = N'INDX_VAR_SEGN'
		 and id = object_id(N'[@db_user].[PROFILE]')
		 )
begin
	CREATE INDEX [INDX_VAR_SEGN] ON [@db_user].[PROFILE]
	(VAR_SEGNATURA)

end
GO

if not exists (SELECT id FROM sysobjects
	WHERE name='DPA_POSIZ_TIMBRO' and xtype='U')
BEGIN
CREATE TABLE DPA_POSIZ_TIMBRO
( SYSTEM_ID  INTEGER               NOT NULL ,
  TIPO_POS   VARCHAR(10 ),
  POS_X      VARCHAR(50 ),
  POS_Y      VARCHAR(50 )	)
END
GO

if (not exists (select * from [@db_user].[DPA_ANAGRAFICA_LOG]
	where VAR_CODICE='DOCUMENTOSPEDISCI'))
BEGIN
		insert into [@db_user].dpa_anagrafica_log(var_codice, var_descrizione
		, var_oggetto, var_metodo)
		values ('DOCUMENTOSPEDISCI', 'Spedizione documento'
		, 'DOCUMENTO', 'DOCUMENTOSPEDISCI')
END
GO

-- Per correggere l'eliminazione delle email scaricate dal server di posta 
-- occorre correggere la chiave di configurazione
-- nel database da ELIMINA_MAIL_ELABORATE a BE_ELIMINA_MAIL_ELABORATE
if (exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	   where VAR_CODICE='ELIMINA_MAIL_ELABORATE'))
BEGIN	
if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_ELIMINA_MAIL_ELABORATE'))
	BEGIN
	update [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	set VAR_CODICE='BE_ELIMINA_MAIL_ELABORATE'
	 where VAR_CODICE='ELIMINA_MAIL_ELABORATE'
	END
END
GO

-- Per correggere l'errore sulla creazione di una nuova pratica e la classificazione rapida 
-- va configurata la chiave nel database FE_BLOCCA_CLASS con ID_AMM = 0 e valore = 0
if (exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	   where VAR_CODICE='FE_BLOCCA_CLASS' 
	   and ID_AMM <> 0 and VAR_valore <> 0))
BEGIN
	update [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	set  ID_AMM = 0, VAR_valore = 0
		where VAR_CODICE='FE_BLOCCA_CLASS' 
		and ID_AMM <> 0 
		and VAR_valore <> 0
END
GO


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_FASC_TUTTI_TIT'))
BEGIN	
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES (0
           ,'BE_FASC_TUTTI_TIT'
           ,'Se ad 1 permette la creazione di fascicoli su tutti i titolari anche quelli non attivi'
           ,'0'
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO 


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
	where VAR_CODICE='BE_NOTE_IN_SEGNATURA'))
BEGIN	
INSERT INTO [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
           ([ID_AMM]
           ,[VAR_CODICE]
           ,[VAR_DESCRIZIONE]
           ,[VAR_VALORE]
           ,[CHA_TIPO_CHIAVE]
           ,[CHA_VISIBILE]
           ,[CHA_MODIFICABILE]
           ,[CHA_GLOBALE]
           ,[VAR_CODICE_OLD_WEBCONFIG])
     VALUES (0
           ,'BE_NOTE_IN_SEGNATURA'
           ,'Aggiunge il campo note tutti nella segnatura xml per interoperabilit'
           ,'0'
           ,'B'
           ,'1'
           ,'1'
           ,'1'
           ,'')

END
GO

if (not exists (select * from [@db_user].[DPA_DISPOSITIVI_STAMPA] where Code='ZEBRA'))
BEGIN
	INSERT into @db_user.DPA_DISPOSITIVI_STAMPA (Code, Description) VALUES ('ZEBRA', 'ZEBRA');
END
GO

if (not exists (select * from [@db_user].[DPA_DISPOSITIVI_STAMPA] where Code='DYMO_LABEL_WRITER_400'))
BEGIN
	INSERT into @db_user.DPA_DISPOSITIVI_STAMPA (Code, Description) VALUES ('DYMO_LABEL_WRITER_400', 'DYMO LABEL WRITER 400');
END
GO

if (not exists (select * from [@db_user].DPA_POSIZ_TIMBRO
	where TIPO_POS='pos_upSx'))
BEGIN
Insert into [@db_user].DPA_POSIZ_TIMBRO   (SYSTEM_ID, TIPO_POS, POS_X, POS_Y)
	Values   (1, 'pos_upSx', '15', '30')
END
GO

if (not exists (select * from [@db_user].DPA_POSIZ_TIMBRO
	where TIPO_POS='pos_upDx'))
BEGIN
Insert into [@db_user].DPA_POSIZ_TIMBRO   (SYSTEM_ID, TIPO_POS, POS_X, POS_Y)
	Values   (2, 'pos_upDx', '400', '30')
END
 GO


if (not exists (select * from [@db_user].DPA_POSIZ_TIMBRO
	where TIPO_POS='pos_downSx'))
BEGIN
Insert into [@db_user].DPA_POSIZ_TIMBRO   (SYSTEM_ID, TIPO_POS, POS_X, POS_Y)
	Values   (3, 'pos_downSx', '15', '775')
END
 GO


if (not exists (select * from [@db_user].DPA_POSIZ_TIMBRO
	where TIPO_POS='pos_downDx'))
BEGIN
Insert into [@db_user].DPA_POSIZ_TIMBRO   (SYSTEM_ID, TIPO_POS, POS_X, POS_Y)
	Values   (4, 'pos_downDx', '400', '775')
END
 GO



IF  EXISTS (SELECT * FROM dbo.sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[getchaimg]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[getchaimg]
go

CREATE FUNCTION @db_user.getchaimg(@docnum INT)

RETURNS VARCHAR(4000)
AS
BEGIN

   DECLARE @tmpVar VARCHAR(30)
   DECLARE @error INT
   begin
      DECLARE @v_path VARCHAR(500)
      DECLARE @vMaxIdGenerica INT
      begin
         SELECT   @vMaxIdGenerica = MAX(v1.version_id)
         FROM @db_user.VERSIONS v1, @db_user.components c
         WHERE v1.docnumber = @docnum
         AND v1.version_id = c.version_id
         SELECT   @error = @@ERROR
         IF (@error <> 0)
            SET @vMaxIdGenerica = 0
      end
      begin
         select   @v_path = ext 
         from @db_user.components 
         where docnumber = @docnum 
         and version_id = @vMaxIdGenerica
         SELECT   @error = @@ERROR
         IF (@error <> 0)
            SET @tmpVar = '0'
      end
      if(@v_path <> '' OR @v_path is  not null) 
         SET @tmpVar = rtrim(ltrim(@v_path))
   else 
      SET @tmpVar = '0'

   end
   RETURN @tmpVar
END 
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects 
			WHERE id = OBJECT_ID(N'[@db_user].[IsValidModelloTrasmissione]')
			AND xtype in (N'FN', N'IF', N'TF')
			)
DROP  function [@db_user].[IsValidModelloTrasmissione] 
go

CREATE FUNCTION [@db_user].[IsValidModelloTrasmissione](
    -- Id del template da analizzare
    @templateId int
) 
RETURNS int 
-- 0 se il modello  valido, -1 se si  verificato un errore, un numero maggiore
-- di 0 se ci sono ruoli inibiti
/******************************************************************************

   AUTHOR:    Samuele Furnari
   NAME:       IsValidModelloTrasmissione
   PURPOSE:    
        Funzione per la verifica di validit di un modello di trasmissione.
        Per essere valido, il modello di cui viene passato l'id non deve
        contenere destinatari inibiti alla ricezione di trasmissioni

******************************************************************************/
AS
BEGIN
      DECLARE @retVal int = 0
      
      -- Conteggio dei destinatari inibiti alla ricezione di trasmissioni
      select @retVal = count('x')
      from dpa_modelli_mitt_dest as md, dpa_corr_globali as cg
      where md.ID_MODELLO = @templateId and md.CHA_TIPO_URP = 'R'
            and md.cha_tipo_mitt_dest = 'D' 
            and md.id_corr_globali = cg.system_id 
            and cg.cha_disabled_trasm = '1'
      
      RETURN @retVal
END
GO

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[I_SMISTAMENTO_SMISTADOC]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
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
@IDTrasmissione int,
@IDTrasmissioneUtenteMittente int,
@TrasmissioneConWorkflow bit,
@NoteGeneraliDocumento varchar(250),
@NoteIndividuali varchar(250),
@DataScadenza datetime,
@TipoDiritto nchar(1),
@Rights int,
@OriginalRights int,
@IDRagioneTrasm int,
@idpeopledelegato int
AS

DECLARE @resultValue INT
DECLARE @resultValueOut int
DECLARE @ReturnValue int
DECLARE @Identity int
DECLARE @IdentityTrasm int
DECLARE @isAccettata nvarchar(1)
DECLARE @isAccettataDelegato nvarchar(1) 
DECLARE @isVista nvarchar(1) 
DECLARE @isVistaDelegato nvarchar(1) 

BEGIN

      set @isAccettata = '0'
      set @isAccettataDelegato = '0'
      set @isVista = '0'
      set @isVistaDelegato = '0'
      
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
            @NoteGeneraliDocumento
      )

      IF (@@ROWCOUNT = 0)
            BEGIN
                  SET @ReturnValue=-2 -- errore inserimento nella dpa_trasmissione
            END
      ELSE
            BEGIN
                  -- Inserimento in tabella DPA_TRASM_SINGOLA
                  SET @Identity=scope_identity()
                  set @IdentityTrasm = @Identity
                  
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
                        @NoteIndividuali,
                        'S',
                        @DataScadenza,
                        NULL
                  )

                  IF (@@ROWCOUNT = 0)
                        BEGIN
                             SET @ReturnValue=-3  -- errore inserimento nella dpa_trasm_singola
                        END
                  ELSE
                        BEGIN
                             -- Inserimento in tabella DPA_TRASM_UTENTE
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
                                         SET @ReturnValue = - 4  -- errore inserimento nella dpa_trasm_utente
                                   END
                             ELSE
                                   BEGIN

                                         UPDATE DPA_TRASMISSIONE 
                                               SET DTA_INVIO = GETDATE() 
                                         WHERE SYSTEM_ID = @IdentityTrasm

                                         DECLARE @AccessRights int

                                         SET @AccessRights=
                                         (
                                               SELECT      MAX(ACCESSRIGHTS)
                                               FROM SECURITY
                                               WHERE       THING=@IDDocumento AND
                                               PERSONORGROUP=@IDPeopleDestinatario
                                         )

                                         IF (NOT @AccessRights IS NULL)
                                               BEGIN
                                                     IF (@AccessRights < @Rights)
                                                           UPDATE      SECURITY
                                                           SET   ACCESSRIGHTS=@Rights
                                                           WHERE       THING=@IDDocumento AND
                                                                 PERSONORGROUP=@IDPeopleDestinatario AND
                                                                 ACCESSRIGHTS=@AccessRights
                                               END
                                         ELSE        
                                               BEGIN
                                                     -- inserimento Rights
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
                                                           @TipoDiritto
                                                     )
                                               END
                                         
                                         IF (@TrasmissioneConWorkflow='1')
                                               BEGIN
                                                     -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
                                                     SET @isAccettata = 
                                                           (SELECT cha_accettata
                                                                 FROM dpa_trasm_utente 
                                                                 WHERE system_id = @idtrasmissioneutentemittente)
                                                       SET @isVista = 
                                                           (SELECT cha_vista
                                                                 FROM dpa_trasm_utente 
                                                                 WHERE system_id = @idtrasmissioneutentemittente)
                             
                                                     IF (@idPeopleDelegato > 0)
                                                           BEGIN
                                                                 -- Impostazione dei flag per la gestione del delegato
                                                                 SET @isVistaDelegato = '1'
                                                                     SET @isAccettataDelegato = '1'
                                                           END

                                                     IF (@isAccettata = '1')
                                                           BEGIN
                                                           -- caso in cui la trasmissione risulta gi accettata
                                                                 IF (@isVista = '0')
                                                                       BEGIN
                                                                                 -- l'oggetto trasmesso non risulta ancora visto,
                                                                                 -- pertanto vengono impostati i dati di visualizzazione
                                                                                 -- e viene rimossa la trasmissione dalla todolist
                                                                            UPDATE dpa_trasm_utente
                                                                            SET   dta_vista = (CASE WHEN dta_vista IS NULL THEN GETDATE() ELSE dta_vista END),
                                                                                  cha_vista = (case when dta_vista is null  then 1 else 0 end),
                                                                                               cha_vista_delegato = @isVistaDelegato,
                                                                                         cha_in_todolist = '0',
                                                                                               cha_valida = '0'
                                                                                      WHERE (   system_id = @idtrasmissioneutentemittente
                                                                                                OR system_id =
                                                                                                (SELECT tu.system_id
                                                                                                   FROM dpa_trasm_utente tu,
                                                                                                        dpa_trasmissione tx,
                                                                                                        dpa_trasm_singola ts
                                                                                                  WHERE tu.id_people = @idpeoplemittente
                                                                                                    AND tx.system_id = ts.id_trasmissione
                                                                                                    AND tx.system_id = @idtrasmissione
                                                                                                    AND ts.system_id = tu.id_trasm_singola
                                                                                                    AND ts.cha_tipo_dest = 'U')
                                                                                         )
                                                                       END
                                                                 ELSE
                                                                       BEGIN
                                                                            -- l'oggetto trasmesso visto,
                                                                                 -- pertanto la trasmissione viene solo rimossa dalla todolist

                                                                            UPDATE dpa_trasm_utente
                                                                                           SET cha_in_todolist = '0',
                                                                                               cha_valida = '0'
                                                                                       WHERE (   system_id = @idtrasmissioneutentemittente
                                                                                                OR system_id =
                                                                                                      (SELECT tu.system_id
                                                                                                         FROM dpa_trasm_utente tu,
                                                                                                              dpa_trasmissione tx,
                                                                                                              dpa_trasm_singola ts
                                                                                                        WHERE tu.id_people = @idpeoplemittente
                                                                                                          AND tx.system_id = ts.id_trasmissione
                                                                                                          AND tx.system_id = @idtrasmissione
                                                                                                          AND ts.system_id = tu.id_trasm_singola
                                                                                                          AND ts.cha_tipo_dest = 'U')
                                                                                               )
                             
                                                                       END
                                                           END
                                                     ELSE
                                                           BEGIN

                                                                     -- la trasmissione ancora non risulta accettata, pertanto:
                                                                     -- 1) viene accettata implicitamente, 
                                                                     -- 2) l'oggetto trasmesso impostato come visto,
                                                                     -- 3) la trasmissione rimossa la trasmissione da todolist
                                                                 UPDATE dpa_trasm_utente
                                                                         SET dta_vista = (CASE WHEN dta_vista IS NULL THEN GETDATE() ELSE dta_vista END),
                                                                       cha_vista = (case when dta_vista is null  then 1 else 0 end),
                                                                               cha_vista_delegato = @isVistaDelegato,
                                                                             dta_accettata = GETDATE(),
                                                                             cha_accettata = '1',
                                                                             cha_accettata_delegato = @isAccettataDelegato,
                                                                             var_note_acc = 'Documento accettato e smistato',                        
                                                                             cha_in_todolist = '0',
                                                                             cha_valida = '0'
                                                                       WHERE (   system_id = @idtrasmissioneutentemittente
                                                                              OR system_id =
                                                                                    (SELECT tu.system_id
                                                                                       FROM dpa_trasm_utente tu,
                                                                                            dpa_trasmissione tx,
                                                                                            dpa_trasm_singola ts
                                                                                      WHERE tu.id_people = @idpeoplemittente
                                                                                        AND tx.system_id = ts.id_trasmissione
                                                                                        AND tx.system_id = @idtrasmissione
                                                                                        AND ts.system_id = tu.id_trasm_singola
                                                                                        AND ts.cha_tipo_dest = 'U')
                                                                             ) 
                                                                             AND cha_valida = '1'
                                                           END

                                                     -- update security se diritti  trasmssione in accettazione =20
                                                       UPDATE security
                                                       SET     accessrights = @originalrights,
                                                               cha_tipo_diritto = 'T'
                                                       WHERE thing=@IDDocumento and   personorgroup IN (@idpeoplemittente, @idgruppomittente)
                                                               AND accessrights = 20

                                               END
                                         ELSE
                                               
                                               BEGIN
                                               
                                                     EXEC @db_user.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione, @idPeopleDelegato,  @resultValue out
                                                     
                                                     SET @resultValueOut= @resultValue
                                                     
                                                     IF(@resultValueOut=1)
                                                           BEGIN
                                                                 SET @ReturnValue = -4;
                                                                 RETURN
                                                           END
                                               END

                                         -- se la trasmissione era destinata a SINGOLO, 
                                         -- allora toglie la validit della trasmissione 
                                         -- a tutti gli altri utenti del ruolo (tranne a quella del mittente)
                                         IF ((SELECT top 1 A.CHA_TIPO_TRASM
                                               FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
                                               WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
                                               AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
                                               DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= @IDPeopleMittente AND
                                               TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
                                               and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente))
                                               ORDER BY CHA_TIPO_DEST
                                               )='S' AND @TrasmissioneConWorkflow='1')
                                                     -- se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente)
                                                     UPDATE      DPA_TRASM_UTENTE
                                                     SET   CHA_VALIDA = '0', cha_in_todolist = '0'
                                                     WHERE       ID_TRASM_SINGOLA IN
                                                           (SELECT A.SYSTEM_ID
                                                           FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
                                                           WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
                                                           AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
                                                           DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
                                                           TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
                                                           and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente)))
                                                           AND SYSTEM_ID NOT IN(@IDTrasmissioneUtenteMittente)

                                         SET @ReturnValue=0
                                   END
                             END
                        END
            END

RETURN @ReturnValue

GO

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[I_SMISTAMENTO_SMISTADOC_U]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[I_SMISTAMENTO_SMISTADOC_U]
GO
CREATE PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC_U]
@IDPeopleMittente int,
@IDCorrGlobaleRuoloMittente int,
@IDGruppoMittente int,
@IDAmministrazioneMittente int,
@IDCorrGlobaleDestinatario int,
@IDDocumento int,
@IDTrasmissione int,
@IDTrasmissioneUtenteMittente int,
@TrasmissioneConWorkflow bit,
@NoteGeneraliDocumento varchar(250),
@NoteIndividuali varchar(250),
@DataScadenza datetime,
@TipoTrasmissione nchar(1),
@TipoDiritto nchar(1),
@Rights int,
@OriginalRights int,
@IDRagioneTrasm int,
@idpeopledelegato int

AS

DECLARE @ReturnValue int
DECLARE @resultValue int
DECLARE @resultValueOut int
DECLARE @Identity int
DECLARE @IDGroups int
DECLARE @AccessRights int
DECLARE @isAccettata nvarchar(1)
DECLARE @isAccettataDelegato nvarchar(1) 
DECLARE @isVista nvarchar(1) 
DECLARE @isVistaDelegato nvarchar(1) 

BEGIN
      set @isAccettata = '0'
      set @isAccettataDelegato = '0'
      set @isVista = '0'
      set @isVistaDelegato = '0'

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
      @NoteGeneraliDocumento
      )

      IF (@@ROWCOUNT = 0)
            BEGIN
                  SET @ReturnValue=-2
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
                  'R',
                  @IDCorrGlobaleDestinatario,
                  @NoteIndividuali,
                  @TipoTrasmissione,
                  @DataScadenza,
                  NULL
                  )

                  SET @Returnvalue = scope_identity()

                  IF (@@ROWCOUNT = 0)
                        BEGIN
                             SET @ReturnValue=-3
                        END
                  ELSE
                        BEGIN
                             SET @IDGroups =
                             (
                             SELECT A.ID_GRUPPO
                             FROM DPA_CORR_GLOBALI A
                             WHERE A.SYSTEM_ID = @IDCorrGlobaleDestinatario
                             )

                             SET @AccessRights=
                             (
                             SELECT      MAX(ACCESSRIGHTS)
                             FROM SECURITY
                             WHERE       THING = @IDDocumento
                             AND
                             PERSONORGROUP = @IDGroups
                             )

                             IF (NOT @AccessRights IS NULL)
                                   BEGIN
                                         IF (@AccessRights < @Rights)
                                               UPDATE      SECURITY
                                               SET   ACCESSRIGHTS=@Rights
                                               WHERE       THING = @IDDocumento
                                               AND
                                               PERSONORGROUP = @IDGroups
                                               AND
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
                                         @IDGroups,
                                         @Rights,
                                         @IDGruppoMittente,
                                         @TipoDiritto
                                         )
                                   END

                             IF (@TrasmissioneConWorkflow='1')
                                   BEGIN
                                         -- Verifica lo  stato di accettazione / visualizzazione della trasmissione utente
                                         SET @isAccettata = (SELECT cha_accettata 
                                                           FROM dpa_trasm_utente 
                                                           WHERE system_id = @IDTrasmissioneUtenteMittente)
                                         
                                           SET @isVista = (SELECT cha_vista
                                                       FROM dpa_trasm_utente 
                                                     where system_id = @IDTrasmissioneUtenteMittente) 
                                         
                                               if (@idPeopleDelegato > 0)
                                                   begin
                                                       -- Impostazione dei flag per la gestione del delegato
                                                       set @isVistaDelegato = '1'
                                                       set @isAccettataDelegato = '1'
                                                   end

                                         if (@isAccettata = '1')
                                               begin
                                                     -- caso in cui la trasmissione risulta gi accettata
                                                     if (@isVista = '0')
                                                           begin
                                                                 -- l'oggetto trasmesso non risulta ancora visto,
                                                                       -- pertanto vengono impostati i dati di visualizzazione
                                                                       -- e viene rimossa la trasmissione dalla todolist
                                                                      UPDATE       dpa_trasm_utente
                                                                         SET      dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
                                                                       cha_vista  =  (case when dta_vista is null  then 1 else 0 end),
                                                                       cha_vista_delegato = @isVistaDelegato,
                                                                                   cha_in_todolist = '0',
                                                                             cha_valida = '0'
                                                                       WHERE (   system_id = @IDTrasmissioneUtenteMittente
                                                                              OR system_id =
                                                                                    (SELECT tu.system_id
                                                                                       FROM dpa_trasm_utente tu,
                                                                                            dpa_trasmissione tx,
                                                                                            dpa_trasm_singola ts
                                                                                      WHERE tu.id_people = @idpeoplemittente
                                                                                        AND tx.system_id = ts.id_trasmissione
                                                                                        AND tx.system_id = @idtrasmissione
                                                                                        AND ts.system_id = tu.id_trasm_singola
                                                                                        AND ts.cha_tipo_dest = 'U')
                                                                             )
                                                           end
                                                     else
                                                           begin 
                                                                     -- l'oggetto trasmesso risulta visto,
                                                                     -- pertanto la trasmissione viene solo rimossa dalla todolist                     
                                                               UPDATE dpa_trasm_utente
                                                                         SET cha_in_todolist = '0',
                                                                             cha_valida = '0'
                                                                     WHERE (   system_id = @IDTrasmissioneUtenteMittente
                                                                              OR system_id =
                                                                                    (SELECT tu.system_id
                                                                                       FROM dpa_trasm_utente tu,
                                                                                            dpa_trasmissione tx,
                                                                                            dpa_trasm_singola ts
                                                                                      WHERE tu.id_people = @idpeoplemittente
                                                                                        AND tx.system_id = ts.id_trasmissione
                                                                                        AND tx.system_id = @idtrasmissione
                                                                                        AND ts.system_id = tu.id_trasm_singola
                                                                                        AND ts.cha_tipo_dest = 'U')
                                                                             )
                                                           end
                                               end
                                         else
                                               begin
                                                     
                                                     -- la trasmissione ancora non risulta accettata, pertanto:
                                                         -- 1) viene accettata implicitamente, 
                                                         -- 2) l'oggetto trasmesso impostato come visto,
                                                         -- 3) la trasmissione rimossa la trasmissione da todolist
                                                     UPDATE dpa_trasm_utente
                                                             SET      dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
                                                           cha_vista = (case when dta_vista is null  then 1 else 0 end),
                                                           cha_vista_delegato = @isVistaDelegato,
                                                                 dta_accettata = getdate(),
                                                                 cha_accettata = '1',
                                                                 cha_accettata_delegato = @isAccettataDelegato,
                                                                 var_note_acc = 'Documento accettato e smistato',
                                                                 cha_in_todolist = '0',
                                                                 cha_valida = '0'
                                                           WHERE (   system_id = @IDTrasmissioneUtenteMittente
                                                                   OR system_id =
                                                                        (SELECT tu.system_id
                                                                           FROM dpa_trasm_utente tu,
                                                                                dpa_trasmissione tx,
                                                                                dpa_trasm_singola ts
                                                                          WHERE tu.id_people = @idpeoplemittente
                                                                            AND tx.system_id = ts.id_trasmissione
                                                                            AND tx.system_id = @idtrasmissione
                                                                            AND ts.system_id = tu.id_trasm_singola
                                                                            AND ts.cha_tipo_dest = 'U')
                                                                 ) 
                                                                 AND cha_valida = '1'
                                               end
                                         
                                               --update security se diritti  trasmssione in accettazione =20
                                               UPDATE      security 
                                               SET     accessrights = @originalrights,
                                                       cha_tipo_diritto = 'T'
                                        WHERE thing=@IDDocumento and personorgroup IN (@idpeoplemittente, @idgruppomittente) 
                                               AND accessrights = 20
                                   END
                             ELSE
                                   BEGIN
                                         EXEC @db_user.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione, @idPeopleDelegato,  @resultValue out

                                         SET @resultValueOut = @resultValue

                                         IF(@resultValueOut=1)
                                               BEGIN
                                                     SET @ReturnValue = -4;
                                                     RETURN
                                               END
                                   END

                             /* se la trasmissione era destinata a SINGOLO, allora toglie la validit della trasmissione a tutti gli altri utenti del ruolo (tranne a quella del mittente) */
                             IF (  
                                   (SELECT     top 1 
                                               A.CHA_TIPO_TRASM
                                   FROM             DPA_TRASM_SINGOLA A, 
                                               DPA_TRASM_UTENTE B
                                   WHERE            A.SYSTEM_ID=B.ID_TRASM_SINGOLA
                                               AND B.SYSTEM_ID IN 
                                               (
                                                     SELECT      TU.SYSTEM_ID 
                                                     FROM DPA_TRASM_UTENTE TU,
                                                           DPA_TRASMISSIONE TX,
                                                           DPA_TRASM_SINGOLA TS 
                                                     WHERE       TU.ID_PEOPLE= @IDPeopleMittente AND
                                                           TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND 
                                                           TX.SYSTEM_ID=@IDTrasmissione AND 
                                                           TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA and 
                                                           TS.SYSTEM_ID = 
                                                           (
                                                                 SELECT ID_TRASM_SINGOLA 
                                                                 FROM DPA_TRASM_UTENTE 
                                                                 WHERE SYSTEM_ID = @IDTrasmissioneUtenteMittente
                                                           )
                                               )
                                   ORDER BY CHA_TIPO_DEST) = 'S' AND 
                                   @TrasmissioneConWorkflow='1')

                             BEGIN
                                   UPDATE            DPA_TRASM_UTENTE
                                   SET         CHA_VALIDA = '0', 
                                               cha_in_todolist = '0'
                                   WHERE             ID_TRASM_SINGOLA IN
                                               (SELECT A.SYSTEM_ID
                                               FROM DPA_TRASM_SINGOLA A, 
                                                     DPA_TRASM_UTENTE B
                                               WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
                                                     AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
                                                     DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
                                                     TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
                                                     and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente)))
                                                     AND SYSTEM_ID NOT IN( @IDTrasmissioneUtenteMittente)
                             END
                        END
                  END
            END

RETURN @ReturnValue

GO
if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[sp_eredita_vis_doc]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[sp_eredita_vis_doc]
GO
CREATE  procedure [@db_user].[sp_eredita_vis_doc]
   @IDCorrGlobaleUO      int,
   @IDCorrGlobaleRuolo   int,
   @IDGruppo             int,
   @LivelloRuolo         int,
   @IDRegistro           int,
   @PariLivello          int
AS

DECLARE @returnValue int
DECLARE @idUO int
DECLARE C_gerarchia CURSOR LOCAL FOR
with gerarchia as 
(select    cgbase.system_id, cgbase.ID_PARENT, 0 as level 
 FROM dpa_corr_globali as cgbase
	 WHERE cgbase.cha_tipo_ie = 'I'
	 AND cgbase.dta_fine IS NULL
	 AND cgbase.id_old = 0
	 and id_parent =  @IDCorrGlobaleUO
 UNION ALL  
 select    cgbase.system_id, cgbase.ID_PARENT, level +1
 FROM dpa_corr_globali cgbase INNER JOIN gerarchia g on cgbase.ID_PARENT = g.SYSTEM_ID  
	 WHERE cha_tipo_ie = 'I'
	 AND dta_fine IS NULL
	 AND id_old = 0
) 
select * from gerarchia








BEGIN
 SET   @returnvalue = 0

   IF (@PariLivello = 0)
     BEGIN
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT DISTINCT s.thing, @IDGruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL,                --  SABRI s.ID_GRUPPO_TRASM,
                                 'A'
                       FROM security s, PROFILE p
                      WHERE p.system_id = s.thing
                        AND p.cha_privato = '0'
                        AND (   p.id_registro = @IDRegistro
                             OR p.id_registro IS NULL
                            )
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello > @LivelloRuolo
                                  AND b.id_uo = @IDCorrGlobaleUO
                                  AND d.id_registro = @IDRegistro)
                        AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = @IDGruppo
                                  AND s1.thing = p.system_id);
      END
    IF (@PariLivello <> 0)
     BEGIN
         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT DISTINCT s.thing, @IDGruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL,                --  SABRI s.ID_GRUPPO_TRASM,
                                 'A'
                       FROM security s, PROFILE p
                      WHERE p.system_id = s.thing
                        AND p.cha_privato = '0'
                        AND (   p.id_registro = @IDRegistro
                             OR p.id_registro IS NULL
                            )
                        AND s.personorgroup IN (
                               SELECT DISTINCT c.system_id
                                          FROM dpa_tipo_ruolo a,
                                               dpa_corr_globali b,
                                               GROUPS c,
                                               dpa_l_ruolo_reg d
                                         WHERE a.system_id = b.id_tipo_ruolo
                                           AND b.id_gruppo = c.system_id
                                           AND d.id_ruolo_in_uo = b.system_id
                                           AND b.cha_tipo_urp = 'R'
                                           AND b.cha_tipo_ie = 'I'
                                           AND b.dta_fine IS NULL
                                           AND a.num_livello >= @LivelloRuolo
                                           AND b.id_uo = @IDCorrGlobaleUO
                                           AND d.id_registro = @IDRegistro)
                        AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = @IDGruppo
                                  AND s1.thing = p.system_id);

      END


/* UO INFERIORI */
   IF (@PariLivello = 0)
     BEGIN
    
    declare @idparent int
    declare @level int
	open C_gerarchia
	WHILE @@FETCH_STATUS = 0
	BEGIN
	FETCH NEXT FROM C_gerarchia into @idUO, @idparent, @level

         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                   DISTINCT s.thing, @IDGruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL,                --  SABRI s.ID_GRUPPO_TRASM,
                                 'A'
                       FROM security s, PROFILE p
                      WHERE p.system_id = s.thing
                        AND p.cha_privato = '0'
                        AND (   p.id_registro = @IDRegistro
                             OR p.id_registro IS NULL
                            )
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello > @LivelloRuolo
                                  AND d.id_registro = @IDRegistro
                                  AND b.id_uo  = @idUO   -- IN (
-- sostituita con ciclo su cursore
/*

                                         SELECT     system_id
                                               FROM dpa_corr_globali
                                              WHERE cha_tipo_ie = 'I'
                                                AND dta_fine IS NULL
                                                AND id_old = 0
                                         START WITH id_parent =
                                                               @idcorrglobaleuo
                                         CONNECT BY PRIOR system_id =
                                                                     id_parent)
*/
)
                        AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = @IDGruppo
                                  AND s1.thing = p.system_id);
      END
close      C_gerarchia
deallocate C_gerarchia

END

 IF (@PariLivello <> 0)
    BEGIN


 open     C_gerarchia
 WHILE @@FETCH_STATUS = 0
BEGIN
FETCH NEXT FROM C_gerarchia into @idUO, @idparent, @level




         INSERT INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                   DISTINCT s.thing, @IDGruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL,                --  SABRI s.ID_GRUPPO_TRASM,
                                 'A'
                       FROM security s, PROFILE p
                      WHERE p.system_id = s.thing
                        AND p.cha_privato = '0'
                        AND (   p.id_registro = @IDRegistro
                             OR p.id_registro IS NULL
                            )
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello >= @LivelloRuolo
                                  AND d.id_registro = @IDRegistro
                                  AND b.id_uo = @idUO   -- IN (
-- sostituita con ciclo su cursore
/*

                                         SELECT     system_id
                                               FROM dpa_corr_globali
                                              WHERE cha_tipo_ie = 'I'
                                                AND dta_fine IS NULL
                                                AND id_old = 0
                                         START WITH id_parent =
                                                               @idcorrglobaleuo
                                         CONNECT BY PRIOR system_id =
                                                                     id_parent)
*/
)
                        AND NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = @IDGruppo
                                  AND s1.thing = p.system_id)
      END
END
END
RETURN @returnValue
GO

if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[SP_MODIFY_CORR_ESTERNO]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[SP_MODIFY_CORR_ESTERNO]
GO
CREATE  PROCEDURE [@db_user].[SP_MODIFY_CORR_ESTERNO]
@IDCorrGlobale INT,
@desc_corr  VARCHAR(128),
@nome  VARCHAR(50),
@cognome  VARCHAR(50),
@codice_aoo VARCHAR(16),
@codice_amm VARCHAR(32),
@email  VARCHAR(128),
@indirizzo VARCHAR(128),
@cap VARCHAR(5),
@provincia VARCHAR(2),
@nazione VARCHAR(32),
@citta  VARCHAR(64),
@cod_fiscale VARCHAR(16),
@telefono VARCHAR(16),
@telefono2 VARCHAR(16),
@note VARCHAR(250),
@fax VARCHAR(16),
@var_idDocType INT,
@inrubricacomune VARCHAR(1),
@tipourp VARCHAR(1),
@localita VARCHAR(128),
@luogoNascita VARCHAR(128),
@dataNascita VARCHAR(64),
@titolo VARCHAR(64),
--@provincianascita VARCHAR(10),
--@sesso CHAR(1),
--@inanagraficaesterna VARCHAR(1),
--@nomeAnagraficaEsterna VARCHAR(1),
@newid INTEGER OUTPUT
AS

/*
a livello macro, La SP effettua le seguenti operazioni:

A)	update del corrispondente vecchio (DPA_CORR_GLOBALI):
- settando var_cod_rubrica = var_cod_rubrica_system_id;
- settando dta_fine = GETDATE();
	A.1) Solo per UTENTI e UO :	
			update del dettaglio del nuovo corrispondente (DPA_DETT_GLOBALI e DPA_T_CANALE_CORR)

B)	insert del nuovo corrispondente (DPA_CORR_GLOBALI):
- codice rubrica = codice rubrica del corrispondente che ? stato storicizzato al punto 1)
- id_old = system_id del corrispondente storicizzato al punto 1)

	B.1) Solo per UTENTI e UO :	
			insert del dettaglio del nuovo corrispondente (DPA_DETT_GLOBALI e DPA_T_CANALE_CORR)

NOTA BENE: Se il corrispondente non  stato mai utilizzato come corrisp in un protocollo, 
	non devo storicizzare, aggiorno solamente i dati 


*/

DECLARE @ReturnValue INT
DECLARE @cod_rubrica VARCHAR(128)
DECLARE @id_reg INT
DECLARE @idAmm INT
DECLARE @new_var_cod_rubrica VARCHAR(128)
DECLARE @cha_dettaglio VARCHAR(1)
DECLARE @cha_tipo_urp VARCHAR(1)
DECLARE @v_id_docType INT
DECLARE @cha_pa VARCHAR(1)
DECLARE @cha_TipoIE VARCHAR(1)
DECLARE @num_livello INT
DECLARE @id_parent INT
DECLARE @id_peso_org INT
DECLARE @id_uo INT
DECLARE @id_tipo_ruolo INT
DECLARE @id_gruppo INT
DECLARE @chiave_esterna VARCHAR(12)

BEGIN

if(@tipourp is not null and @tipourp != '' and @cha_tipo_urp is not null and @cha_tipo_urp!=@tipourp)
begin
set @cha_tipo_urp = @tipourp
end

SELECT
@cod_rubrica = VAR_COD_RUBRICA,
@cha_tipo_urp = CHA_TIPO_URP,
@id_reg = ID_REGISTRO,
@idAmm = ID_AMM,
@cha_pa = CHA_PA, 
@cha_TipoIE = CHA_TIPO_IE,
@num_livello = NUM_LIVELLO, 
@id_parent = ID_PARENT, 
@id_peso_org = ID_PESO_ORG, 
@id_uo = ID_UO, 
@id_tipo_ruolo = ID_TIPO_RUOLO, 
@id_gruppo = ID_GRUPPO,
@chiave_esterna = VAR_CHIAVE_AE
FROM DPA_CORR_GLOBALI
WHERE system_id = @IDCorrGlobale

IF @@ROWCOUNT > 0  --1 update del corrispondente vecchio
BEGIN

SELECT @v_id_docType = ID_DOCUMENTTYPE
FROM DPA_T_CANALE_CORR
WHERE ID_CORR_GLOBALE = @IDCorrGlobale

IF @@ROWCOUNT > 0  --sono sempre nel ramo 1, per update del corrispondente vecchio
BEGIN
-- calcolo il nuovo codice rubrica
--SET @new_var_cod_rubrica = @var_cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale)
SET @new_var_cod_rubrica = @cod_rubrica + '_'+ CONVERT(varchar(32), @IDCorrGlobale )
SET @cha_dettaglio = '0' -- default

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
SET @cha_dettaglio = '1'
BEGIN

--VERIFICO se il corrisp ? stato utilizzato come dest/mitt di protocolli
SELECT ID_PROFILE
FROM DPA_DOC_ARRIVO_PAR
WHERE ID_MITT_DEST =  @IDCorrGlobale

-- 1) non ? stato mai utilizzato come corrisp in un protocollo
IF (@@ROWCOUNT = 0)
BEGIN
--non devo storicizzare, aggiorno solamente i dati
UPDATE 	DPA_CORR_GLOBALI
SET 	VAR_CODICE_AOO= @codice_aoo,
		VAR_CODICE_AMM = @codice_amm,
		VAR_EMAIL = @email,
		VAR_DESC_CORR = @desc_corr,
		VAR_NOME = @nome,
		VAR_COGNOME= @cognome,
		CHA_PA=@cha_pa,
		CHA_TIPO_URP=@cha_tipo_urp
WHERE 	SYSTEM_ID = @IDCorrGlobale

IF(@@ROWCOUNT > 0) -- SE UPDATE ? andata a buon fine
BEGIN
		begin
--per utenti e Uo aggiorno il dettaglio
		IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' )
		BEGIN

		--se fa parte di una lista, allora la devo aggiornare
			update dpa_liste_distr set ID_DPA_CORR=@newid where ID_DPA_CORR=@idcorrglobale


			UPDATE DPA_DETT_GLOBALI
			SET 	VAR_INDIRIZZO = @indirizzo,
					VAR_CAP = @cap,
					VAR_PROVINCIA = @provincia,
					VAR_NAZIONE = @nazione,
					VAR_COD_FISCALE = @cod_fiscale,
					VAR_TELEFONO = @telefono,
					VAR_TELEFONO2 = @telefono2,
					VAR_NOTE= @note,
					VAR_CITTA= @citta,
					VAR_FAX = @fax,
					VAR_LOCALITA = @localita,
					VAR_LUOGO_NASCITA = @luogoNascita,
					DTA_NASCITA = @dataNascita,
					VAR_TITOLO = @titolo--,
					--CHA_SESSO = @sesso,
					--CHAR_PROVINCIA_NASCITA = @provincianascita
			WHERE ID_CORR_GLOBALI = @IDCorrGlobale

				IF(@@ROWCOUNT > 0)
				SET @ReturnValue = 1
				ELSE
				SET @ReturnValue = 0

		END
		ELSE
			SET @ReturnValue = 1 -- CASO RUOLI
end

IF (@ReturnValue = 1)

BEGIN

	UPDATE DPA_T_CANALE_CORR
		SET  ID_DOCUMENTTYPE =  @var_idDocType
		WHERE ID_CORR_GLOBALE = @IDCorrGlobale

	IF(@@ROWCOUNT > 0)
		SET @ReturnValue = 1
	ELSE
		SET @ReturnValue = 0
	END

END

ELSE
	SET @ReturnValue = 0
	
END
ELSE

-- caso 2) Il corrisp ? stato utilizzato come destinatario
BEGIN
--  INIZIO STORICIZZAZIONE DEL CORRISPONDENTE

		UPDATE	DPA_CORR_GLOBALI
		SET 	DTA_FINE = GETDATE(),  
				VAR_COD_RUBRICA =@new_var_cod_rubrica,  
				VAR_CODICE =@new_var_cod_rubrica,
				ID_PARENT = NULL
		WHERE 	SYSTEM_ID = @IDCorrGlobale

-- se la storicizzazione ? andata a buon fine,
--posso inserire il nuovo corrispondente
IF @@ROWCOUNT > 0

BEGIN
DECLARE @cha_tipo_corr VARCHAR(1)
IF (@inrubricacomune = '1')
	SET @cha_tipo_corr = 'C'
ELSE
	--IF (@inanagraficaesterna='1')
	--	SET @cha_tipo_corr= @nomeAnagraficaEsterna
	--ELSE	
		SET @cha_tipo_corr = 'S'

		INSERT INTO DPA_CORR_GLOBALI (
						NUM_LIVELLO, 						CHA_TIPO_IE,
						ID_REGISTRO, 						ID_AMM,
						VAR_DESC_CORR, 						VAR_NOME,
						VAR_COGNOME, 						ID_OLD,
						DTA_INIZIO, 						ID_PARENT,
						VAR_CODICE, 						CHA_TIPO_CORR,
						CHA_TIPO_URP, 						VAR_CODICE_AOO,
						VAR_COD_RUBRICA, 						CHA_DETTAGLI,
						VAR_EMAIL, 						VAR_CODICE_AMM,
						CHA_PA, 						ID_PESO_ORG,
						ID_GRUPPO, 						ID_TIPO_RUOLO,
						ID_UO, 						VAR_CHIAVE_AE)
		VALUES (@num_livello, 						@cha_TipoIE,
						@id_reg, 						@idAmm,
						@desc_corr, 						@nome,
						@cognome, 						@IDCorrGlobale,
						GETDATE(), 						@id_parent,
						@cod_rubrica, 						@cha_tipo_corr,
						@cha_tipo_urp, 						@codice_aoo,
						@cod_rubrica, 						@cha_dettaglio,
						@email, 						@codice_amm,
						@cha_pa, 						@id_peso_org,
						@id_gruppo, 						@id_tipo_ruolo,
						@id_uo, 						@chiave_esterna)

--prendo la systemId appena inserita
SET @newid = @@identity

IF @@ROWCOUNT > 0 -- se l'inserimento del nuovo corrisp ? andato a buon fine

IF(@cha_tipo_urp='U' OR @cha_tipo_urp='P' ) -- CASO UTENTE/UO: inserisco il dettaglio
BEGIN

			--se fa parte di una lista, allora la devo aggiornare
			update dpa_liste_distr set ID_DPA_CORR=@newid where ID_DPA_CORR=@idcorrglobale

					INSERT INTO DPA_DETT_GLOBALI
					(ID_CORR_GLOBALI,					VAR_INDIRIZZO,
					VAR_CAP, 					VAR_PROVINCIA,
					VAR_NAZIONE, 					VAR_COD_FISCALE,
					VAR_TELEFONO, 					VAR_TELEFONO2,
					VAR_NOTE, 					VAR_CITTA,
					VAR_FAX, 					VAR_LOCALITA,
					VAR_LUOGO_NASCITA, 					DTA_NASCITA,
					VAR_TITOLO--, 					CHA_SESSO,
					--CHAR_PROVINCIA_NASCITA
					)
					VALUES
					(@newid, 					@indirizzo,
					@cap, 					@provincia,
					@nazione, 					@cod_fiscale,
					@telefono, 					@telefono2,
					@note, 					@citta,
					@fax, 					@localita,
					@luogoNascita, 					@dataNascita,
					@titolo--, 					@sesso,
					--@provincianascita	
					)

IF @@ROWCOUNT > 0
		-- se la insert su dpa_dett_globali ? andata a buon fine
		SET @ReturnValue = 1 -- valore ritornato 1
ELSE
		-- se la insert su dpa_dett_globali non ? andata a buon fine
		SET @ReturnValue = 0 -- valore ritornato 0
END

ELSE  -- CASO RUOLO: non inserisco il dettaglio
-- vuol dire che il corrispondente ? un RUOLO (quindi non deve essere fatta la insert sulla dpa_dett_globali)
--valore ritornato 1 perch? significa che l'operazione di inserimento del nuovo ruolo ? andato a buon fine
SET @ReturnValue = 1

IF (@ReturnValue = 1)

BEGIN

				INSERT INTO DPA_T_CANALE_CORR
				(ID_CORR_GLOBALE,				ID_DOCUMENTTYPE,				CHA_PREFERITO)
				VALUES
				(@newid,				@var_idDocType,				'1')

IF @@ROWCOUNT > 0
		-- se la insert su DPA_T_CANALE_CORR ? andata a buon fine
		SET @ReturnValue = 1 -- valore ritornato 1
ELSE
		-- se la insert su DPA_T_CANALE_CORR non ? andata a buon fine
		SET @ReturnValue = 0 -- valore ritornato 0
END
ELSE

SET @ReturnValue = 0 -- inserimento non andato a buon fine: ritorno 0 ed esco

-- FINE STORICIZZAZIONE

END

END

END

END
ELSE
SET @ReturnValue = 0


END

ELSE
			SET @ReturnValue = 0 -- la storicizzazione del corrispondente ? andata male:  ritorno 0 ed esco -- END 1
END

RETURN @ReturnValue

GO


if exists ( select * from dbo.sysobjects 
			where id = object_id(N'[@db_user].[SP_RIMUOVI_DOCUMENTI]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[SP_RIMUOVI_DOCUMENTI]
GO

CREATE PROCEDURE @db_user.SP_RIMUOVI_DOCUMENTI 
@idProfile INT , 
@ReturnValue INT OUTPUT  AS
BEGIN

   DELETE FROM @db_user.DPA_TRASM_UTENTE 
   WHERE id_trasm_singola in
	   (SELECT system_id FROM @db_user.DPA_TRASM_SINGOLA 
	   WHERE id_trasmissione in
			(select t.system_id  
				from @db_user.dpa_trasmissione t 
					where t.id_profile = @idProfile
			)
		)

   DELETE FROM @db_user.DPA_TRASM_SINGOLA 
   WHERE id_trasmissione in
		(select t.system_id from @db_user.dpa_trasmissione t 
		where t.id_profile = @idProfile
		)


   DELETE FROM @db_user.DPA_TRASMISSIONE WHERE id_profile = @idProfile

   DELETE FROM @db_user.project_components where LINK = @idProfile 
   
   DELETE FROM @db_user.VERSIONS WHERE DOCNUMBER = @idProfile
   
   DELETE FROM @db_user.COMPONENTS WHERE DOCNUMBER = @idProfile
   
   DELETE FROM @db_user.DPA_AREA_LAVORO WHERE ID_PROFILE = @idProfile
   
   DELETE FROM @db_user.DPA_PROF_PAROLE WHERE ID_PROFILE = @idProfile
   
   DELETE FROM @db_user.PROFILE WHERE DOCNUMBER = @idProfile
   
   DELETE FROM @db_user.SECURITY WHERE THING = @idProfile
   
   DELETE from @db_user.dpa_todolist where id_profile = @idProfile
   
   DELETE FROM @db_user.DPA_DIAGRAMMI WHERE DOC_NUMBER = @idProfile

   BEGIN
      DECLARE @cnt INT
      SELECT   @cnt = COUNT(*) FROM @db_user.DPA_ASSOCIAZIONE_TEMPLATES
      WHERE DOC_NUMBER = @idProfile
      IF (@cnt != 0)
         DELETE FROM @db_user.DPA_ASSOCIAZIONE_TEMPLATES
         WHERE DOC_NUMBER = @idProfile

   END
   SET @ReturnValue = 1
END

GO

Insert into [@db_user].[DPA_DOCSPA]
   (DTA_UPDATE, ID_VERSIONS_U)
 Values      (getdate(), '3.15.2')
GO

