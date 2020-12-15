
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE @db_user.qs_spc_calc_actually_used(@DbName sysname, @obj_name sysname = null, @pages_actually_used decimal(19,2) output )
as
begin
create table #extentinfo (
file_id integer,
page_id integer,
pg_alloc integer,
ext_size integer,
obj_id integer,
index_id integer,
pfs_bytes binary(8))
if @obj_name is null
insert into #extentinfo exec ('dbcc extentinfo('''+ @DbName + ''')')
else
insert into #extentinfo exec ('dbcc extentinfo(''' + @DbName + ''',''' + @obj_name + ''')')
select @pages_actually_used = sum(dbo.qs_fn_interpret_pfs(pfs_bytes)) from #extentinfo
drop table #extentinfo
end
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO