ALTER FUNCTION [DOCSADM].[checkSecurity]
(
@thing INT,
@idpeople INT,
@idgroup INT,
@idRuoloPubblico INT,
@tipoObjParam VARCHAR
)
RETURNS INT
AS

BEGIN

DECLARE @retValue INT
declare @cnt int

set @retValue = 1 -- DOCSADM.checkVisibilitaArchivio(@tipoObjParam, @thing, @idgroup)
set @cnt = 0

IF (@retValue=1)
begin
SELECT @cnt=COUNT(*)  FROM security WHERE thing = @thing AND (personorgroup IN (@idgroup, @idpeople, @idRuoloPubblico))  and ACCESSRIGHTS>0
end

if (@cnt > 0)
SET @retValue = 1
ELSE
SET @retValue = 0

RETURN @retValue
end