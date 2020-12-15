SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


/*
-------------------------------------------------------------------------------------------------------
-- SP per la gestione della visibilit� dei ruoli superiori gerarchici
-- nello smistamento di un documento.
-- Valori di ritorno gestiti:
-- 0: Operazione andata a buon fine
-- 1: Altrimenti
-------------------------------------------------------------------------------------------------------
*/
CREATE   PROCEDURE  I_Smistamento_Setvisibsup 
@IDCorrGlobaleRuolo int,
@IDGruppoMittente int,
@LivelloRuoloMittente int,
@PariLivello int,
@DirittoDaEred int,
@IDDocumento int

AS

DECLARE @ExistIDGruppiSuperiori nvarchar(1)
DECLARE @recordCorrente int
DECLARE @idGruppoSuperiore int
DECLARE @ExistAccessRights nvarchar(1)
DECLARE @AccessRights int
DECLARE @accessRightsValue int
DECLARE @MyUo int
DECLARE @IDParent int
DECLARE @IDUnitaOrganizzativa int
DECLARE @LivelloRuolo int
DECLARE @IDRegistro int

SET @ExistIDGruppiSuperiori='Y'
SET @ExistAccessRights='Y'

-- variabile utilizzata per il reperimento del valore dal cursore
DECLARE @ID_GRUPPO int

SET @IDRegistro=
	(SELECT ID_REGISTRO
	 FROM PROFILE
	 WHERE SYSTEM_ID = @IDDocumento)

SET @MyUO=
 	 (SELECT id_uo
	 FROM DPA_CORR_GLOBALI
	 WHERE system_id = @IDCorrGlobaleRuolo)

SET @LivelloRuolo=
	(SELECT TR.NUM_LIVELLO
	FROM DPA_CORR_GLOBALI CG,DPA_TIPO_RUOLO TR
	WHERE CG.SYSTEM_ID = @IDCorrGlobaleRuolo  AND
	  CG.ID_TIPO_RUOLO = TR.SYSTEM_ID)

BEGIN
	
	/* visibilita' ai ruoli superiori della stessa UO ------------------------------------------ */
	 
	IF (@IDRegistro IS NULL)
		DECLARE IDGruppiSuperiori CURSOR FOR		
			SELECT 	CG.ID_GRUPPO
			FROM	DPA_CORR_GLOBALI CG,
				DPA_TIPO_RUOLO TR
			WHERE	ID_UO = @MyUO
				AND
				CHA_TIPO_URP = 'R'
				AND
				DTA_FINE IS NULL
				AND
				CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
				AND
				TR.NUM_LIVELLO < @LivelloRuolo
	ELSE
		DECLARE IDGruppiSuperiori CURSOR FOR		
			SELECT 	CG.ID_GRUPPO
			FROM	DPA_CORR_GLOBALI CG,
				DPA_TIPO_RUOLO TR,
				DPA_L_RUOLO_REG RR
			WHERE	CG.ID_UO = @MyUO
				AND
				CG.CHA_TIPO_URP = 'R'
				AND
				CG.DTA_FINE IS NULL
				AND
				CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
				AND
				CG.SYSTEM_ID = RR.ID_RUOLO_IN_UO
				AND
				RR.ID_REGISTRO = @IDRegistro
				AND
				TR.NUM_LIVELLO < @LivelloRuolo

	OPEN IDGruppiSuperiori
	
	FETCH NEXT FROM IDGruppiSuperiori
	INTO 	@ID_GRUPPO 
	
	WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Verifica se non vi sia gi� una trasmissione per il documento:
			-- - se presente, si distinguono 2 casi:
			--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato) 
			--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
			--	2) altrimenti non fa nulla
			-- - se non presente viene fatta in ogni caso la insert con
			--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
			SET @AccessRights=
				(
				SELECT 	MAX(ACCESSRIGHTS)
				FROM 	SECURITY
				WHERE 	THING = @IDDocumento
						AND
						PERSONORGROUP = @ID_GRUPPO
						AND
						CHA_TIPO_DIRITTO = 'A'
				)
	
			IF (NOT @AccessRights IS NULL)
				IF (@AccessRights < @DirittoDaEred)
					/* aggiornamento a DirittoDaEred */
					UPDATE 	SECURITY
					SET 	ACCESSRIGHTS = @DirittoDaEred
					WHERE 	THING = @IDDocumento
							AND
							PERSONORGROUP = @ID_GRUPPO
							AND
							CHA_TIPO_DIRITTO = 'A'
							AND 
							ACCESSRIGHTS = @AccessRights
			ELSE
				/* inserimento a DirittoDaEred */
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
					@ID_GRUPPO,
					@DirittoDaEred,
					@IDGruppoMittente,
					'A'
				)	
	
			-- Posizionamento sul record successivo del cursore temporaneo
			FETCH NEXT FROM IDGruppiSuperiori
			INTO 	@ID_GRUPPO 			

		END
	
	-- Chiusura e deallocazione cursore	
	CLOSE IDGruppiSuperiori
	DEALLOCATE IDGruppiSuperiori


 	/* visibilita' ai ruoli superiori delle UO superiori ------------------------------------*/
	WHILE (1=1)
	BEGIN		
		SET @IDUnitaOrganizzativa=
			(SELECT id_parent 
			FROM DPA_CORR_GLOBALI
			WHERE system_id = @MyUO)

		IF (@IDUnitaOrganizzativa=0)
			BREAK
		ELSE
			SET @MyUO=@IDUnitaOrganizzativa

		BEGIN
					
			IF (@PariLivello=0)
			   BEGIN
				IF (@IDRegistro IS NULL)
					DECLARE IDGruppiSuperiori CURSOR FOR
						SELECT 	CG.ID_GRUPPO
						FROM	DPA_CORR_GLOBALI CG,
							DPA_TIPO_RUOLO TR
						WHERE	ID_UO = @MyUO
							AND
							CHA_TIPO_URP = 'R'
							AND
							DTA_FINE IS NULL
							AND
							CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
							AND
							TR.NUM_LIVELLO < @LivelloRuolo
				ELSE
					DECLARE IDGruppiSuperiori CURSOR FOR
						SELECT 	CG.ID_GRUPPO
						FROM	DPA_CORR_GLOBALI CG,
							DPA_TIPO_RUOLO TR,
							DPA_L_RUOLO_REG RR
						WHERE	CG.ID_UO = @MyUO
							AND
							CG.CHA_TIPO_URP = 'R'
							AND
							CG.DTA_FINE IS NULL
							AND
							CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
							AND
							CG.SYSTEM_ID = RR.ID_RUOLO_IN_UO
							AND
							RR.ID_REGISTRO = @IDRegistro
							AND
							TR.NUM_LIVELLO < @LivelloRuolo
			  END
			ELSE
			  BEGIN
				IF (@IDRegistro IS NULL)
					DECLARE IDGruppiSuperiori CURSOR FOR
						SELECT 	CG.ID_GRUPPO
						FROM	DPA_CORR_GLOBALI CG,
							DPA_TIPO_RUOLO TR
						WHERE	ID_UO = @MyUO
							AND
							CHA_TIPO_URP = 'R'
							AND
							DTA_FINE IS NULL
							AND
							CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
							AND
							TR.NUM_LIVELLO <= @LivelloRuolo
				ELSE
					DECLARE IDGruppiSuperiori CURSOR FOR
						SELECT 	CG.ID_GRUPPO
						FROM	DPA_CORR_GLOBALI CG,
							DPA_TIPO_RUOLO TR,
							DPA_L_RUOLO_REG RR
						WHERE	CG.ID_UO = @MyUO
							AND
							CG.CHA_TIPO_URP = 'R'
							AND
							CG.DTA_FINE IS NULL
							AND
							CG.ID_TIPO_RUOLO = TR.SYSTEM_ID
							AND
							CG.SYSTEM_ID = RR.ID_RUOLO_IN_UO
							AND
							RR.ID_REGISTRO = @IDRegistro
							AND
							TR.NUM_LIVELLO <= @LivelloRuolo
			  END		
	
			OPEN IDGruppiSuperiori
						
			FETCH NEXT FROM IDGruppiSuperiori
			INTO 	@ID_GRUPPO 
			
			WHILE @@FETCH_STATUS = 0
				BEGIN
					-- Verifica se non vi sia gi� una trasmissione per il documento:
					-- - se presente, si distinguono 2 casi:
					--	1) se ACCESSRIGHT < DirittoDaEred (parametro passato) 
					--	   viene fatto un'aggiornamento impostandone il valore al parametro passato
					--	2) altrimenti non fa nulla
					-- - se non presente viene fatta in ogni caso la insert con
					--   valore di ACCESSRIGHT = DirittoDaEred (parametro passato)
					SET @AccessRights=
						(
						SELECT 	MAX(ACCESSRIGHTS)
						FROM 	SECURITY
						WHERE 	THING = @IDDocumento
								AND
								PERSONORGROUP = @ID_GRUPPO
								AND
								CHA_TIPO_DIRITTO = 'A'
						)
			
					IF (NOT @AccessRights IS NULL)
						IF (@AccessRights < @DirittoDaEred)
							/* aggiornamento a DirittoDaEred */
							UPDATE 	SECURITY
							SET 	ACCESSRIGHTS = @DirittoDaEred
							WHERE 	THING = @IDDocumento
									AND
									PERSONORGROUP = @ID_GRUPPO
									AND
									CHA_TIPO_DIRITTO = 'A'
									AND
									ACCESSRIGHTS = @AccessRights
					ELSE
						/* inserimento a DirittoDaEred */
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
							@ID_GRUPPO,
							@DirittoDaEred,
							@IDGruppoMittente,
							'A'
						)	
			
					-- Posizionamento sul record successivo del cursore temporaneo
					FETCH NEXT FROM IDGruppiSuperiori
					INTO 	@ID_GRUPPO 			
		
				END
			
			-- Chiusura e deallocazione cursore	
			CLOSE IDGruppiSuperiori
			DEALLOCATE IDGruppiSuperiori

		END	
	
	END

END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
