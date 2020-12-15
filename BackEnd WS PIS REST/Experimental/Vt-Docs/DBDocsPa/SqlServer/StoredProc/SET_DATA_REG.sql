

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[SET_DATA_REG]

AS

declare @sysid int

BEGIN

DECLARE currReg CURSOR FOR
select  a.system_id
from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
and a.cha_automatico='1' and cha_stato='A'
begin
SET @sysid=0

OPEN currReg
FETCH NEXT FROM currReg
INTO @sysid
WHILE @@FETCH_STATUS = 0
BEGIN
insert into dpa_registro_sto(dta_open,dta_close,num_rif,id_registro,id_people,id_ruolo_in_uo)
SELECT a.dta_open,getdate(),b.num_rif,a.system_id,1,1
from dpa_el_registri a, dpa_reg_proto b where a.system_id=b.id_registro
and a.system_id = @sysid

update dpa_el_registri set dta_open =getdate(),cha_stato ='A',dta_close = null
WHERE SYSTEM_ID=@sysid

update dpa_reg_proto set num_rif=1 where  substring(convert(varchar,getdate(),103),1,5)='01/01'

FETCH NEXT FROM currReg  INTO  @sysid
END


CLOSE currReg
DEALLOCATE currReg
END
END


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO