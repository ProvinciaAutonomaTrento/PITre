SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE @db_user.putFile
@versionId int,
@filePath varchar(500),
@fileSize int,
@printThumb varchar(64),
@iscartaceo smallint

AS

declare @retValue int
declare @docNumber int

if exists(select version_id from versions where version_id = @versionId)
begin
set @docNumber = (select docnumber from versions where version_id = @versionId)
update 	versions
set 	subversion = 'A',
cartaceo = @iscartaceo
where 	version_id = @versionId

set @retValue = @@rowcount
end
if (@retValue > 0 and exists(select version_id from components where version_id = @versionId))
begin
update 	components
set 	path = @filePath,
file_size = @fileSize,
var_impronta = @printThumb
where 	version_id = @versionId

set @retValue = @@rowcount
end

if (@retValue > 0 and exists(select docnumber from profile where docnumber = @docNumber))
begin
update 	profile
set 	cha_img = '1'
where	docnumber = @docNumber

set @retValue = @@rowcount
end
return @retValue

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO