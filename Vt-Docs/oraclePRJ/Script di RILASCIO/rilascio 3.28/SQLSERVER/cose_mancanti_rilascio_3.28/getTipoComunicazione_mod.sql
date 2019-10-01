

/****** Object:  UserDefinedFunction [DOCSADM].[getTipoComunicazione]    Script Date: 05/06/2013 13:05:30 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[getTipoComunicazione]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[getTipoComunicazione]
GO



/****** Object:  UserDefinedFunction [DOCSADM].[getTipoComunicazione]    Script Date: 05/06/2013 13:05:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE function [DOCSADM].[getTipoComunicazione] (@p_systemId int)
returns varchar(150)
as
begin

declare @varTipoCom varchar(150)


            

return ''

end

GO


