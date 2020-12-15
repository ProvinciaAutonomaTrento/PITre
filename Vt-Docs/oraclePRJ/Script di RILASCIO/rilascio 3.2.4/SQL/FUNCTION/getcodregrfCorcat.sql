-- Creazione funzione "getcodregrfCorcat"
ALTER FUNCTION [DOCSADM].[getcodregrfCorcat] (
	@idruolo int
)
RETURNS varchar(256)
AS
BEGIN
	DECLARE	@risultato 			varchar(256);
	DECLARE	@item 				varchar(16);
	DECLARE cur CURSOR LOCAL FOR
		 SELECT l.var_codice
         FROM docsadm.dpa_l_ruolo_reg a inner join
		 	  docsadm.dpa_el_registri l on L.SYSTEM_ID = A.ID_REGISTRO
         WHERE a.id_ruolo_in_uo = @idruolo
         ORDER BY ISNULL (cha_rf, '0') asc;

	SET @risultato = '';
	open cur;
	fetch next from cur into @item;
	while(@@fetch_status=0)
	begin
		IF(LEN(@risultato) > 0)
		begin
			SET @risultato = @risultato + '; ' + @item
		end
		ELSE
		begin
			SET @risultato = @risultato + @item
		end
		fetch next from cur into @item
	end
	CLOSE cur;
	deallocate cur;
	RETURN @risultato;
END