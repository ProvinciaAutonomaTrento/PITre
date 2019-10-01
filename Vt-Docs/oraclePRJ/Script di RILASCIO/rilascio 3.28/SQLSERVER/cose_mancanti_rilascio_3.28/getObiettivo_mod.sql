

/****** Object:  UserDefinedFunction [DOCSADM].[getObiettivo]    Script Date: 05/06/2013 12:46:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[getObiettivo]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[getObiettivo]
GO



/****** Object:  UserDefinedFunction [DOCSADM].[getObiettivo]    Script Date: 05/06/2013 12:46:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE function [DOCSADM].[getObiettivo] (@p_systemId int)
returns varchar(200)
as
begin

declare @varCODobiettivo varchar(200)

            

return ''

end

GO


