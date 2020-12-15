
ALTER FUNCTION [DOCSADM].[getChaConsentiFasc](@id_parent INT,@cha_tipo_proj varchar(1), @cha_tipo_fascicolo varchar(1), @id_fascicolo int)
RETURNS varchar(1)
as
begin
declare @risultato varchar(1)

SET @risultato = '1'

set @risultato = (select cha_consenti_fasc from project where system_id = @id_parent)

return @risultato
end