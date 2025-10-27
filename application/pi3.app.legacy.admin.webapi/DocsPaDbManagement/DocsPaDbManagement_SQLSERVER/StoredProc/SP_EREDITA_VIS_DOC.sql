SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO



ALTER   PROCEDURE SP_EREDITA_VIS_DOC
	@IDCorrGlobaleUO int,
	@IDCorrGlobaleRuolo int,	
	@IDGruppo int,
	@LivelloRuolo int,
	@IDRegistro int,
	@PariLivello int
AS

/*
.........................................................................................
SP_EREDITA_VIS_DOC          		MAIN
.........................................................................................
EREDITARIETA' DELLA VISIBILITA' DEI DOCUMENTI DA PARTE DI UN NUOVO RUOLO IN ORGANIGRAMMA
.........................................................................................
SP per il processo di inserimento della visibilità da ereditare sui documenti.

VALORI DI INPUT:

@IDCorrGlobaleUO 	= [system_id - tabella dpa_corr_globali] UO nella quale è stato 
		   	inserito il nuovo ruolo;
@IDCorrGlobaleRuolo	= [system_id - tabella dpa_corr_globali] Nuovo ruolo inserito

@IDGruppo		= [system_id - tabella groups] Nuovo ruolo inserito

@LivelloRuolo		= [num_livello - tabella dpa_tipo_uolo] Livello del nuovo ruolo

@IDRegistro		= [system_id - tabella dpa_el_registri] Registro associato al 
			nuovo ruolo 

@PariLivello		= chiave sul web.config della WS : EST_VIS_SUP_PARI_LIV
*/

DECLARE @retValue int

BEGIN	 		
	SET @retValue = 0

	-- prende ruoli inferiori nella stessa UO
	EXECUTE @retValue=SP_EREDITA_VIS_DOC_E1 @IDCorrGlobaleUO, @IDCorrGlobaleRuolo, @IDGruppo, @LivelloRuolo, @IDRegistro, @PariLivello
		
		IF(@retValue = 1)
			RETURN 1

	-- cicla tra le UO inferiori
	EXECUTE @retValue=SP_EREDITA_VIS_DOC_E2 @IDCorrGlobaleUO, @IDCorrGlobaleRuolo, @IDGruppo, @LivelloRuolo, @IDRegistro, @PariLivello

		IF(@retValue = 1)
			RETURN 1

	RETURN @retValue
END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

