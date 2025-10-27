SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO







ALTER       PROCEDURE SP_EREDITA_VIS_DOC_E1
	@IDCorrGlobaleUO int,
	@IDCorrGlobaleRuolo int,	
	@IDGruppo int,
	@LivelloRuolo int,
	@IDRegistro int,
	@PariLivello int
AS

/*
.......................................................................................
SP_EREDITA_VIS_DOC_E1          		ESTENSIONE 1
.......................................................................................
*/

DECLARE @retValue int
DECLARE @thing int
DECLARE @personorgroup int
DECLARE @acr int
DECLARE @gtrasm int
DECLARE @dir varchar

DECLARE c1 CURSOR LOCAL FOR
	select 
		s.thing as thing,
		@IDGruppo as personorgroup,
		(case when
	   		(s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A')) 
		 then
	  		63
		 else
		 	s.accessrights
		 end)  as acr,
		s.id_gruppo_trasm as gtrasm,
		'A' as dir
	from security s, profile p
	where p.system_id = s.thing			
	and p.cha_privato = '0'
	and (p.id_registro = @IDRegistro or p.id_registro is null)
	and s.personorgroup in (
		SELECT	c.SYSTEM_ID
		FROM	DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
		WHERE 	a.SYSTEM_ID = b.ID_TIPO_RUOLO
		AND	b.id_gruppo = c.system_id
		AND 	d.id_ruolo_in_uo = b.system_id
		AND 	b.CHA_TIPO_URP = 'R' 
		AND 	b.CHA_TIPO_IE = 'I' 
		AND 	b.DTA_FINE IS NULL
		AND 	a.NUM_LIVELLO > @LivelloRuolo 
		AND	b.ID_UO = @IDCorrGlobaleUO 
		AND	d.id_registro = @IDRegistro
	)	

DECLARE c2 CURSOR LOCAL FOR
	select 
		s.thing as thing,
		@IDGruppo as personorgroup,
		(case when
	   		(s.accessrights=255 AND (s.cha_tipo_diritto='P' OR s.cha_tipo_diritto='A')) 
		 then
	  		63
		 else
		 	s.accessrights
		 end)  as acr,
		s.id_gruppo_trasm as gtrasm,
		'A' as dir
	from security s, profile p
	where p.system_id = s.thing			
	and p.cha_privato = '0'
	and (p.id_registro = @IDRegistro or p.id_registro is null)
	and s.personorgroup in (
		SELECT	c.SYSTEM_ID
		FROM	DPA_TIPO_RUOLO a, DPA_CORR_GLOBALI b, GROUPS c, DPA_L_RUOLO_REG d
		WHERE 	a.SYSTEM_ID = b.ID_TIPO_RUOLO
		AND	b.id_gruppo = c.system_id
		AND 	d.id_ruolo_in_uo = b.system_id
		AND 	b.CHA_TIPO_URP = 'R' 
		AND 	b.CHA_TIPO_IE = 'I' 
		AND 	b.DTA_FINE IS NULL
		AND 	a.NUM_LIVELLO >= @LivelloRuolo 
		AND	b.ID_UO = @IDCorrGlobaleUO 
		AND	d.id_registro = @IDRegistro
	)	

BEGIN		
	SET @retValue = 0

	IF (@PariLivello=0)
		BEGIN
			OPEN c1
			FETCH NEXT FROM c1
			INTO @thing, @personorgroup,@acr,@gtrasm,@dir 
			
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
						  @gtrasm, 
						  @dir)

					IF(@@ERROR <> 0)
						CONTINUE				
			
					FETCH NEXT FROM c1
					INTO @thing, @personorgroup,@acr,@gtrasm,@dir 
				END
			
			CLOSE c1
			DEALLOCATE c1				
	 	END
	ELSE
		BEGIN
			OPEN c2
			FETCH NEXT FROM c2
			INTO @thing, @personorgroup,@acr,@gtrasm,@dir 
			
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
						  @gtrasm, 
						  @dir)

					IF(@@ERROR <> 0)
						CONTINUE				
			
					FETCH NEXT FROM c2
					INTO @thing, @personorgroup,@acr,@gtrasm,@dir 
				END
			
			CLOSE c2
			DEALLOCATE c2	
		END	
	

	RETURN @retValue
END






GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

