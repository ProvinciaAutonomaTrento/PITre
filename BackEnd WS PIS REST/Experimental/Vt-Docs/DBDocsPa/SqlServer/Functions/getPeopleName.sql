SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

create function @db_user.getPeopleName (@peopleId int)
returns varchar(256)
as
begin

declare @fullName varchar(256)

select @fullName = full_name
from people
where system_id = @peopleId;

return @fullName

end

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
