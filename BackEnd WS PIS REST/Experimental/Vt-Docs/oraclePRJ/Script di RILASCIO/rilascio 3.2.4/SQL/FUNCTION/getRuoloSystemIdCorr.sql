ALTER FUNCTION [DOCSADM].[getRuoloSystemIdCorr]
(
	@idGruppo int
)
RETURNS varchar(256)
AS
BEGIN
	DECLARE @risultato varchar(256)

	SET @risultato = (select system_id from docsadm.dpa_corr_globali where id_Gruppo = @idGruppo)


	RETURN @risultato

END