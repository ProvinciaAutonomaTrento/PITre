

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


CREATE function @db_user.DESCUO (@idUo  int)
returns varchar(256)
as
begin
declare @outcome varchar(256)
set @outcome=' '
if(@idUo is not null)
select @outcome= var_desc_corr  from dpa_corr_globali where system_id=@idUo
return @outcome
end

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
