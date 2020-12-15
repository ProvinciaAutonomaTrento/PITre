CREATE PROCEDURE [DOCSADM].[putFile]
@versionId int,
@filePath varchar(500),
@fileSize int,
@printThumb varchar(64),
@iscartaceo smallint,
@estensione varchar(7),
@isFirmato char,
@tipoFirma varchar(2),
@nomeoriginale varchar(500), --= NULL --modifica Stefano
@idPeoplePutfile int
AS
 
declare @retValue int
declare @docNumber int
 
if exists(select version_id from versions where version_id = @versionId)
begin
set @docNumber = (select docnumber from versions where version_id = @versionId)
update      versions
set   subversion = 'A',
cartaceo = @iscartaceo
where       version_id = @versionId
 
set @retValue = @@rowcount
end
 
if (@retValue > 0 and exists(select version_id from components where version_id = @versionId))
begin
 
-- in quella oracle viene eliminato l'if su @isFirmato e viene utilizzato nvl
-- modifica Stefano
 
--if (@isFirmato='1')
begin
update      components
set   path = @filePath,
file_size = @fileSize,
var_impronta = @printThumb,
ext=@estensione,
cha_firmato = isnull(@isFirmato, '0'), --'1',   modifica Stefano                   
Var_Nomeoriginale = isnull(@nomeoriginale,Var_Nomeoriginale ),
ID_PEOPLE_PUTFILE = @idPeoplePutfile,
CHA_TIPO_FIRMA = isnull(@tipoFirma, 'N')
where       version_id = @versionId
--end
--else
--begin
--update    components
--set       path = @filePath,
--file_size = @fileSize,
--var_impronta = @printThumb,
--ext=@estensione,
--cha_firmato = '0'
--where     version_id = @versionId
--end
set @retValue = @@rowcount
end
 
if (@retValue > 0 and exists(select docnumber from profile where docnumber = @docNumber))
begin
 
if(@isFirmato='1')
begin
update      profile
set   cha_img = '1', cha_firmato='1'
where docnumber = @docNumber
end
else
begin
update      profile
set   cha_img = '1',cha_firmato='0'
where docnumber = @docNumber
end
set @retValue = @@rowcount
end
return @retValue
end
 
GO