if exists (select * from dbo.sysindexes where name = N'indx_COMP1' and id = object_id(N'[@db_user].[COMPONENTS]'))
drop index [@db_user].[COMPONENTS].[indx_COMP1]
GO
CREATE INDEX   [indx_COMP1] ON  [@db_user].[COMPONENTS] ([DOCNUMBER])  	
GO