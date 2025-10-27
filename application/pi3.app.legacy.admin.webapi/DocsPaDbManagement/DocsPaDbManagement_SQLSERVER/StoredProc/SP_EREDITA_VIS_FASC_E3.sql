SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO





ALTER     PROCEDURE SP_EREDITA_VIS_FASC_E3
	@IDCorrGlobaleUO int,	
	@IDCorrGlobaleRuolo int,
	@IDGruppo int,
	@LivelloRuolo int,
	@IDRegistro int,
	@PariLivello int
AS

/*
.......................................................................................
SP_EREDITA_VIS_FASC_E3        		ESTENSIONE 3
.......................................................................................
*/

DECLARE @IDUOInferiore int
DECLARE @id_uo_cursore int
DECLARE @retValue int

BEGIN			

	SET @retValue = 0

	DECLARE IDUOInferiori
	CURSOR LOCAL FOR
		select system_id 
		from dpa_corr_globali
		where cha_tipo_urp = 'U'
		and cha_tipo_ie = 'I'
		and dta_fine is null
		and id_old = 0
		and id_parent = @IDCorrGlobaleUO

		IF(@@ERROR <> 0) 
			RETURN(1)

	OPEN IDUOInferiori
	FETCH NEXT FROM IDUOInferiori
	INTO @id_uo_cursore 
	
	WHILE @@FETCH_STATUS = 0
		BEGIN			
			-- Prende tutti i fascicoli PROCEDIMENTALI dei ruoli inferiori/pari livello della UO data
			EXECUTE @retValue=SP_EREDITA_VIS_FASC_E1 @id_uo_cursore, @IDCorrGlobaleRuolo, @IDGruppo, @LivelloRuolo, @IDRegistro, @PariLivello
			
			IF(@retValue = 1)
				CONTINUE				
	
			FETCH NEXT FROM IDUOInferiori
			INTO @id_uo_cursore
		END
	
	CLOSE IDUOInferiori
	DEALLOCATE IDUOInferiori	

	RETURN @retValue	
END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

