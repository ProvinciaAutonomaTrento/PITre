
/****** Object:  UserDefinedFunction [DOCSADM].[corrcatricevuta]    Script Date: 05/06/2013 12:36:19 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[corrcatricevuta]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [DOCSADM].[corrcatricevuta]
GO


CREATE FUNCTION [DOCSADM].[corrcatricevuta] (@docId int, @dirProt varchar(1))
returns varchar(8000)
as
begin 
declare @item varchar(200)
declare @outcome varchar(8000)
declare @dirCorr varchar(2)
declare @proto_dest varchar(128)
declare @annull varchar(1)

set @outcome =''


declare cur CURSOR LOCAL

for select distinct c.var_desc_corr, dap.cha_tipo_mitt_dest
--Laura
, dsi.var_proto_dest, dsi.cha_annullato
from dpa_corr_globali c WITH (NOLOCK) , dpa_doc_arrivo_par dap WITH (NOLOCK) 
left outer join docsadm.dpa_stato_invio dsi on dap.SYSTEM_ID = dsi.ID_DOC_ARRIVO_PAR
where dap.id_profile=@docId
and dap.id_mitt_dest=c.system_id  
order by dap.cha_tipo_mitt_dest desc
open cur

fetch next from cur into @item,@dirCorr,@proto_dest,@annull
while(@@fetch_status=0)
begin

--Laura
if(@proto_dest is not null)
begin

	set @outcome=@outcome + '(*)'
end
	if (@dirProt='P' and @dirCorr='M')
	begin
		if(@item is not null)
		set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (M); '
	end

if (@dirProt='P' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (D); '
end

if (@dirProt='P' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (CC); '
end

if (@dirProt='A' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (M); '
end

if (@dirProt='A' and @dirCorr='MD')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (MM); '
end

if (@dirProt='A' and @dirCorr='I')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (MI); '
end

if (@dirProt='I' and @dirCorr='M')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (M); '
end

if (@dirProt='I' and @dirCorr='D')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (D); '
end

if (@dirProt='I' and @dirCorr='C')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].parsenull(@item)+' (CC); '
end

if (@dirProt='P' and @dirCorr='L')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].DEST_IN_LISTA(@docId)
end

if (@dirProt='P' and @dirCorr='F')
begin
if(@item is not null)
set @outcome=@outcome+[DOCSADM].DEST_IN_RF(@docId)
end

fetch next from cur into @item,@dirCorr,@proto_dest,@annull

end

close cur

deallocate cur

if (len(@outcome)>0)
begin
set @outcome = substring(@outcome,1,(len(@outcome)-1))
end 

if (len(@outcome)=0)
begin
set @outcome = null
end

return @outcome
end 

GO


