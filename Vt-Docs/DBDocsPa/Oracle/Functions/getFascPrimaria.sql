alter function @db_user.[getFascPrimaria](@iProfile INT, @idFascicolo int)
returns varchar(1)
as
begin

declare @fascPrimaria varchar(1)
SET @fascPrimaria = '0'
set @fascPrimaria = (SELECT B.CHA_FASC_PRIMARIA FROM PROJECT A, PROJECT_COMPONENTS B
          WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=@iProfile and ID_FASCICOLO=@idFascicolo)
				
if (@fascPrimaria is null)
set @fascPrimaria = '0'
return @fascPrimaria
/