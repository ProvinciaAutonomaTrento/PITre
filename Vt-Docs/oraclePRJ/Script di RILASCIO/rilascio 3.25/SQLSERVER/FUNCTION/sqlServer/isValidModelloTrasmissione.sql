

/****** Object:  UserDefinedFunction [DOCSADM].[IsValidModelloTrasmissione]    Script Date: 05/07/2013 10:24:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[IsValidModelloTrasmissione]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[IsValidModelloTrasmissione]
GO



/****** Object:  UserDefinedFunction [DOCSADM].[IsValidModelloTrasmissione]    Script Date: 05/07/2013 10:24:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



----------- FINE -

---- ALTER_IsValidModelloTrasmissione.MSSQL.SQL  marcatore per ricerca ----
CREATE FUNCTION [DOCSADM].[IsValidModelloTrasmissione](

-- Id del template da analizzare

@templateId int

)

RETURNS int

AS

BEGIN

DECLARE @retVal int = 0



-- Conteggio dei destinatari inibiti alla ricezione di trasmissioni

select @retVal = count('x')

from dpa_modelli_mitt_dest as md, dpa_corr_globali as cg

where md.ID_MODELLO = @templateId and md.CHA_TIPO_URP = 'R'

and md.cha_tipo_mitt_dest = 'D'

and md.id_corr_globali = cg.system_id

and (cg.cha_disabled_trasm = '1' OR cg.DTA_FINE is NOT NULL)



RETURN @retVal

END

GO


