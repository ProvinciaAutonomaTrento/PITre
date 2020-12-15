-- Creazione funzione "getutentiCorcat"
ALTER FUNCTION [DOCSADM].[getutentiCorcat] (
	@idRuolo 		int
)
RETURNS varchar(4000)
AS
BEGIN
	DECLARE	@risultato 		varchar(4000)
	DECLARE @item 			varchar(256)
	DECLARE cur CURSOR LOCAL FOR
		SELECT p.var_nome + ' ' + var_cognome + ' (' + p.user_id + ')' as name
		from docsadm.people p INNER JOIN
			docsadm.peoplegroups pg on pg.PEOPLE_SYSTEM_ID = p.system_id
		where pg.GROUPS_SYSTEM_ID = @idRuolo
		and pg.dta_fine is null
		and p.DISABLED = 'N'
	SET @risultato = ''
	open cur
	fetch next from cur into @item
	while @@fetch_status=0
	begin
		if (LEN(@risultato) > 0)
		begin
			SET @risultato =  @risultato + '; ' + @item
		end
		ELSE
		begin
			SET @risultato = @risultato + @item
		end
		fetch next from cur into @item
	end
	CLOSE cur
	deallocate cur
	RETURN @risultato
END