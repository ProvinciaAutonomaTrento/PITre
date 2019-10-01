SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE function [@db_user].[corrcat] (@docId int, @dirProt varchar(1))
returns varchar(8000)
as
begin
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(1)

set @outcome=''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
from dpa_corr_globali c WITH (NOLOCK), dpa_doc_arrivo_par dap WITH (NOLOCK)
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr
while(@@fetch_status=0)
begin
if (@dirProt='P' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (M); '
end

if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (CC); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (M); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+@db_user.parsenull(@item)+' (CC); '
end

fetch next from cur into @item,@dirCorr

end

close cur

deallocate cur

if (len(@outcome)>0)

set @outcome = substring(@outcome,1,(len(@outcome)-1))

return @outcome

end


GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO