/****** Object:  UserDefinedFunction [DOCSADM].[GetChaFirmato]    Script Date: 4/13/2017 2:06:08 PM ******/
ALTER function [DOCSADM].[GetChaTipoFirma] (@docNum int)
returns varchar(16)
as
begin
declare @risultato varchar(16)
declare @vmaxidGenarica int
begin
set @vmaxidGenarica =0
SELECT @vmaxidGenarica =max(v1.version_id) from versions v1, components c where v1.docnumber=@docnum and v1.version_id=c.version_id
end
begin
SELECT @risultato=CHA_TIPO_FIRMA FROM COMPONENTS  WHERE DOCNUMBER = @docNum and version_id=@vmaxidGenarica
end
return @risultato
end