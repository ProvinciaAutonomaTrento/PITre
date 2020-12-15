CREATE FUNCTION [DOCSADM].[checkDocumentoPubblico]
(
@thing INT,
@idRuoloPubblico INT
)
RETURNS INT
AS

BEGIN

DECLARE @retValue INT
declare @cnt int

set @retValue = 0 
set @cnt = 0

begin
SELECT @cnt=COUNT(*)  FROM security WHERE thing = @thing AND personorgroup = @idRuoloPubblico and ACCESSRIGHTS>0
end

if (@cnt > 0)
SET @retValue = 1
ELSE
SET @retValue = 0

RETURN @retValue
end