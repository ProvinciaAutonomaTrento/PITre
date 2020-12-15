SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


create procedure [@db_user].[dpa_setNumFasc] AS

begin

update @db_user.dpa_reg_fasc set num_rif=1 where cha_automatico='1' and convert(varchar,getdate(),3)='01/01';--to_char(getdate,'dd/mm')='01/01'
commit transaction
exception --when others then null;
end

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
