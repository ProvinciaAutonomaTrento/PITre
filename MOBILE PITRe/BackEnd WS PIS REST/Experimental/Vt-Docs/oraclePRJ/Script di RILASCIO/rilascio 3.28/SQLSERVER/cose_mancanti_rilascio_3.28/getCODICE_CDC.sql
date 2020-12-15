

/****** Object:  UserDefinedFunction [DOCSADM].[getCODICE_CDC]    Script Date: 05/06/2013 12:45:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[getCODICE_CDC]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[getCODICE_CDC]
GO


/****** Object:  UserDefinedFunction [DOCSADM].[getCODICE_CDC]    Script Date: 05/06/2013 12:45:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE function [DOCSADM].[getCODICE_CDC] (@p_systemId int)
returns varchar(50)
as
begin

declare @varCODICECDC varchar(50)

SELECT @varCODICECDC =CDC_CODICE
            FROM  DPA_CORR_GLOBALI  
            WHERE SYSTEM_ID = @p_systemId;
            

return @varCODICECDC

end

GO


