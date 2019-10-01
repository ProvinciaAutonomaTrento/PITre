SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

ALTER   PROCEDURE [@db_user].[I_SMISTAMENTO_SMISTADOC_U]
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
@IDRagioneTrasm int
AS

DECLARE @ReturnValue int
DECLARE @resultValue int
DECLARE @resultValueOut int
DECLARE @Identity int
DECLARE @IDGroups int
DECLARE @AccessRights int

BEGIN
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
						SELECT 	MAX(ACCESSRIGHTS)
						FROM 	SECURITY
						WHERE 	THING = @IDDocumento
						AND
						PERSONORGROUP = @IDGroups
						)

					IF (NOT @AccessRights IS NULL)
						BEGIN
							IF (@AccessRights < @Rights)
								UPDATE 	SECURITY
								SET 	ACCESSRIGHTS=@Rights
								WHERE 	THING = @IDDocumento
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
						UPDATE 	DPA_TRASM_UTENTE
						SET
						dta_vista = (case when dta_vista is null then GETDATE() else dta_vista end),
						cha_vista  =  (case when dta_vista is null  then 1 else 0 end),
						DTA_ACCETTATA=GETDATE(),
						CHA_ACCETTATA='1',
						VAR_NOTE_ACC='Documento accettato e smistato',
						CHA_IN_TODOLIST = '0'
						WHERE (SYSTEM_ID = @IDTrasmissioneUtenteMittente
						OR SYSTEM_ID = (SELECT TU.SYSTEM_ID FROM
						DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE=@IDPeopleMittente AND
						TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
						AND TS.CHA_TIPO_DEST= 'U')
						)
						AND CHA_VALIDA='1'
					ELSE
						BEGIN

							EXEC DOCSADM.SPsetDataVistaSmistamento @IDPeopleMittente, @IDDocumento, @IDGruppoMittente, 'D', @idTrasmissione,  @resultValue out

							SET @resultValueOut = @resultValue

							IF(@resultValueOut=1)
								BEGIN
									SET @ReturnValue = -4;
									RETURN
								END
						END

					IF ((SELECT top 1 A.CHA_TIPO_TRASM
						FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
						WHERE A.SYSTEM_ID=B.ID_TRASM_SINGOLA
						AND B.SYSTEM_ID IN (SELECT TU.SYSTEM_ID FROM
						DPA_TRASM_UTENTE TU,DPA_TRASMISSIONE TX,DPA_TRASM_SINGOLA TS WHERE TU.ID_PEOPLE= @IDPeopleMittente AND
						TX.SYSTEM_ID=TS.ID_TRASMISSIONE AND TX.SYSTEM_ID=@IDTrasmissione AND TS.SYSTEM_ID=TU.ID_TRASM_SINGOLA
						and TS.SYSTEM_ID = (SELECT ID_TRASM_SINGOLA FROM DPA_TRASM_UTENTE WHERE SYSTEM_ID =@IDTrasmissioneUtenteMittente))
						ORDER BY CHA_TIPO_DEST
						)='S' AND @TrasmissioneConWorkflow='1')

						BEGIN
							UPDATE 	DPA_TRASM_UTENTE
							SET 	CHA_VALIDA = '0', cha_in_todolist = '0'
							WHERE ID_TRASM_SINGOLA IN
							(SELECT A.SYSTEM_ID
							FROM DPA_TRASM_SINGOLA A, DPA_TRASM_UTENTE B
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
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

