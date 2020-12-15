SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

 
create function @db_user.getDescCorr (@corrId int)
returns varchar(256)
as
begin

declare @varDescCorr varchar(256)

select @varDescCorr = var_desc_corr
from dpa_corr_globali
where system_id=@corrId;

return @varDescCorr

end


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO