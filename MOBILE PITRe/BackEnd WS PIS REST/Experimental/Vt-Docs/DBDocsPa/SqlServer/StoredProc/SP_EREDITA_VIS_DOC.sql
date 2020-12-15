SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE  [@db_user].[SP_EREDITA_VIS_DOC]
@IDCorrGlobaleUO int,
@IDCorrGlobaleRuolo int,
@IDGruppo int,
@LivelloRuolo int,
@IDRegistro int,
@PariLivello int
AS

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