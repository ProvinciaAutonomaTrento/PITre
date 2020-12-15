IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[@db_user].[checkSecurity]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [@db_user].[checkSecurity] 
go
CREATE FUNCTION [@db_user].[checkSecurity] 
(
@thing INT,
@idpeople INT,
@idgroup INT,
@tipoObjParam VARCHAR
)
RETURNS INT
AS

BEGIN

DECLARE @retValue INT
declare @cnt int

set @retValue = [@db_user].checkVisibilitaArchivio(@tipoObjParam, @thing, @idgroup)
set @cnt = 0

IF (@retValue=1) 
	begin 
	   SELECT @cnt=COUNT(*)  FROM security WHERE thing = @thing AND (personorgroup IN (@idgroup, @idpeople))  and ACCESSRIGHTS>0
	end

if (@cnt > 0)
	SET @retValue = 1 
ELSE
	SET @retValue = 0

RETURN @retValue
end