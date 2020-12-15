SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO







ALTER       PROCEDURE SP_EREDITA_VIS_FASC_E2
	@IDCorrGlobaleUO int,
	@IDCorrGlobaleRuolo int,	
	@IDGruppo int,
	@LivelloRuolo int,
	@IDRegistro int,
	@PariLivello int
AS

/*
.......................................................................................
SP_EREDITA_VIS_FASC_E2          		ESTENSIONE 2
.......................................................................................
Prende tutti i fascicoli PROCEDIMENTALI dei ruoli inferiori/pari livello della UO data
*/

DECLARE @retValue int
DECLARE @thing int
DECLARE @personorgroup int
DECLARE @acr int
DECLARE @dir varchar

DECLARE c2 CURSOR LOCAL FOR
	SELECT DISTINCT
		s.THING as thing,
		@IDGruppo as personorgroup,
		(CASE WHEN
			(s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
		THEN
			63
		ELSE
			s.accessrights
		END)  as acr,
		'A' as dir
	FROM SECURITY s,PROJECT p
	WHERE p.system_id = s.thing
	AND p.system_id IN (
		(SELECT a.system_id
			FROM PROJECT a
			WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND (a.id_registro = @IDRegistro or a.id_registro is null))
		) UNION (
		SELECT b.system_id
		FROM PROJECT b
		WHERE b.cha_tipo_proj = 'C'
		AND b.id_fascicolo IN (
				SELECT system_id
				FROM PROJECT
				WHERE cha_tipo_proj = 'F'
				AND cha_tipo_fascicolo = 'P'
				AND (id_registro = @IDRegistro or id_registro is null)))
	)
	AND s.personorgroup IN (
		SELECT
			c.SYSTEM_ID
		FROM  
			DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
		WHERE 
			a.SYSTEM_ID = b.ID_TIPO_RUOLO
			AND b.id_gruppo = c.system_id
			AND d.id_ruolo_in_uo = b.system_id
			AND b.CHA_TIPO_URP = 'R'
			AND b.CHA_TIPO_IE = 'I'
			AND b.DTA_FINE IS NULL
			AND (a.NUM_LIVELLO > @LivelloRuolo)
			AND b.ID_UO = @IDCorrGlobaleUO
			AND (d.id_registro = @IDRegistro or d.id_registro is null))
	
	IF(@@ERROR <> 0) 
		RETURN(1)	

DECLARE c3 CURSOR LOCAL FOR
	SELECT DISTINCT
		s.THING as thing,
		@IDGruppo as personorgroup,
		(CASE WHEN
			(s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A' OR s.cha_tipo_diritto='F'))
		THEN
			63
		ELSE
			s.accessrights
		END)  as acr,
		'A' as dir
	FROM SECURITY s,PROJECT p
	WHERE p.system_id = s.thing
	AND p.system_id IN (
		(SELECT a.system_id
			FROM PROJECT a
			WHERE ((a.cha_tipo_proj = 'F' AND a.cha_tipo_fascicolo = 'P') AND (a.id_registro = @IDRegistro or a.id_registro is null))
		) UNION (
		SELECT b.system_id
		FROM PROJECT b
		WHERE b.cha_tipo_proj = 'C'
		AND b.id_fascicolo IN (
				SELECT system_id
				FROM PROJECT
				WHERE cha_tipo_proj = 'F'
				AND cha_tipo_fascicolo = 'P'
				AND (id_registro = @IDRegistro or id_registro is null)))
	)
	AND s.personorgroup IN (
		SELECT
			c.SYSTEM_ID
		FROM  
			DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
		WHERE 
			a.SYSTEM_ID = b.ID_TIPO_RUOLO
			AND b.id_gruppo = c.system_id
			AND d.id_ruolo_in_uo = b.system_id
			AND b.CHA_TIPO_URP = 'R'
			AND b.CHA_TIPO_IE = 'I'
			AND b.DTA_FINE IS NULL
			AND (a.NUM_LIVELLO >= @LivelloRuolo)
			AND b.ID_UO = @IDCorrGlobaleUO
			AND (d.id_registro = @IDRegistro or d.id_registro is null))
	
	IF(@@ERROR <> 0) 
		RETURN(1)

BEGIN		
	SET @retValue = 0

	IF (@PariLivello=0)
		BEGIN
			OPEN c2
			FETCH NEXT FROM c2
			INTO @thing, @personorgroup,@acr,@dir 
			
			WHILE @@FETCH_STATUS = 0
				BEGIN		
					IF NOT EXISTS 
						(SELECT * 
							FROM SECURITY 
							WHERE THING = @thing AND
								PERSONORGROUP = @personorgroup AND
								ACCESSRIGHTS = @acr)	
						INSERT  INTO SECURITY
					 	 (THING, 
						  PERSONORGROUP, 
						  ACCESSRIGHTS, 
						  ID_GRUPPO_TRASM,
						  CHA_TIPO_DIRITTO)
			 			 VALUES
						  (@thing, 
						  @personorgroup, 
						  @acr, 
						  NULL, 
						  @dir)

					IF(@@ERROR <> 0)
						CONTINUE				
			
					FETCH NEXT FROM c2
					INTO @thing, @personorgroup,@acr,@dir 
				END
			
			CLOSE c2
			DEALLOCATE c2	
			
	 	END
	ELSE
		BEGIN
			OPEN c3
			FETCH NEXT FROM c3
			INTO @thing, @personorgroup,@acr,@dir 
			
			WHILE @@FETCH_STATUS = 0
				BEGIN	
					IF NOT EXISTS 
						(SELECT * 
							FROM SECURITY 
							WHERE THING = @thing AND
								PERSONORGROUP = @personorgroup AND
									ACCESSRIGHTS = @acr)			
						INSERT  INTO SECURITY
					 	 (THING, 
						  PERSONORGROUP, 
						  ACCESSRIGHTS, 
						  ID_GRUPPO_TRASM,
						  CHA_TIPO_DIRITTO)
			 			 VALUES
						  (@thing, 
						  @personorgroup, 
						  @acr, 
						  NULL, 
						  @dir)

					IF(@@ERROR <> 0)
						CONTINUE				
			
					FETCH NEXT FROM c3
					INTO @thing, @personorgroup,@acr,@dir 
				END
			
			CLOSE c3
			DEALLOCATE c3	
		END		
	
	

	RETURN @retValue
END






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

