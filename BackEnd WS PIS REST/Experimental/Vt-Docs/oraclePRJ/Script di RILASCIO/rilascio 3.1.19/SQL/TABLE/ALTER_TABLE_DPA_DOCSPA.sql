BEGIN
	declare @cntcol int;
 
	begin
		select @cntcol = count(*)
		from sys.tables t join sys.columns c on (c.object_id = t.object_id)
		where t.name = 'DPA_DOCSPA' and c.name='VAR_MESSAGGIO_LOGIN'
 
		if @cntcol = 0 
		begin
		   alter table DPA_DOCSPA add VAR_MESSAGGIO_LOGIN VARCHAR(2000)
		end
	end
END