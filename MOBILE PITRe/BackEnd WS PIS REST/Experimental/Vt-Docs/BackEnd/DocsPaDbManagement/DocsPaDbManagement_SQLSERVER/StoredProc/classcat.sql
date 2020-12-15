if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[classcat]') and xtype in (N'FN', N'IF', N'TF'))
drop function [DOCSADM].[classcat]
GO

CREATE function docsadm.classcat (@docId int)
returns varchar(8000)
as
begin
	declare @item varchar(200)
	declare @outcome varchar(8000)
	
	set @outcome=''
	
	declare cur CURSOR LOCAL
	for SELECT DISTINCT A.VAR_CODICE
	FROM PROJECT A WITH (NOLOCK)
    		WHERE A.CHA_TIPO_PROJ = 'F'
    		AND A.SYSTEM_ID IN
    		(SELECT A1.ID_FASCICOLO 
		 FROM PROJECT A1 WITH (NOLOCK), PROJECT_COMPONENTS B WITH (NOLOCK)
		 WHERE A1.SYSTEM_ID=B.PROJECT_ID AND B.LINK=@docId)
	open cur
	fetch next from cur into @item
	while(@@fetch_status=0)
	begin
		set @outcome=@outcome+docsadm.parsenull(@item)+', '
		fetch next from cur into @item
	end
	close cur
	deallocate cur
	if (len(@outcome)>0)
		set @outcome = substring(@outcome,1,(len(@outcome)-1))
	return @outcome
end