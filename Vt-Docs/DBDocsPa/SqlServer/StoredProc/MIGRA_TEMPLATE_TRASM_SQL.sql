SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

create  procedure [@db_user].[MIGRA_TEMPLATE_TRASM_SQL]
as
set nocount on
declare @id_modello_trasm int
declare @counter int
set @counter = 0

declare @id_trasm int
declare @id_amm int
declare @nome varchar(255)
declare @cha_tipo_oggetto varchar(1)
declare @id_reg int
declare @var_note_generali varchar(255)
declare @id_people int
declare @id_ruolo_in_uo int

BEGIN TRANSACTION ROOT
PRINT '-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER DOCUMENTI PROTOCOLLATI-- '
declare c_modelli_trasm cursor local for
select
dt.system_id,der.id_amm,dtt.var_template,dt.cha_tipo_oggetto,
p.id_registro,dt.var_note_generali,dt.id_people,dt.id_ruolo_in_uo
from
@db_user.dpa_templ_trasm as dtt,@db_user.dpa_trasmissione as dt,
@db_user.dpa_el_registri as der,@db_user.profile as p
where
dtt.id_trasmissione = dt.system_id
and der.system_id = p.id_registro and dt.id_profile = p.system_id
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT

open c_modelli_trasm
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,@id_ruolo_in_uo
while(@@fetch_status=0)
begin
set @counter = @counter + 1
print cast(@counter as varchar)+' - Migrazione Template per Documento: '+@nome
insert into [@db_user].[dpa_modelli_trasm]
([id_amm], [nome], [cha_tipo_oggetto], [id_registro],[var_note_generali],[id_people], [single])
values (@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,0)
set @id_modello_trasm = (select scope_identity())
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
values (@id_modello_trasm,'M',0,0,'','','R')
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
select @id_modello_trasm,'D', dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,dts.var_note_sing, dts.cha_tipo_dest
from @db_user.dpa_trasm_singola as dts where id_trasmissione = @id_trasm
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
UPDATE [@db_user].[dpa_modelli_mitt_dest] SET cha_tipo_urp = 'P' WHERE ID_MODELLO = @id_modello_trasm
AND CHA_TIPO_URP = 'U' AND CHA_TIPO_MITT_DEST='D'
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,@id_ruolo_in_uo
end
SET @id_modello_trasm = 0
SET @id_trasm = 0
SET @id_amm = 0
SET @nome = ''
SET @cha_tipo_oggetto = ''
SET @id_reg = 0
SET @var_note_generali = ''
SET @id_people = 0
SET @id_ruolo_in_uo = 0
deallocate c_modelli_trasm
PRINT '-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER FASCICOLI ASSOCIATI AL REGISTRO -- '
declare c_modelli_trasm cursor local for
select
dt.system_id,P.id_amm,dtt.var_template,dt.cha_tipo_oggetto,
p.id_registro,dt.var_note_generali,dt.id_people,dt.id_ruolo_in_uo
from
@db_user.dpa_templ_trasm as dtt,@db_user.dpa_trasmissione as dt, @db_user.PROJECT as p
where
dtt.id_trasmissione = dt.system_id  and
dt.id_project = p.system_id
AND P.CHA_TIPO_PROJ = 'F' AND P.CHA_TIPO_FASCICOLO = 'P'
AND P.ID_REGISTRO IS NOT NULL
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT

open c_modelli_trasm
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,@id_ruolo_in_uo
while(@@fetch_status=0)
begin
set @counter = @counter + 1
print cast(@counter as varchar)+' - Migrazione Template Per Fascicolo: '+@nome
insert into [@db_user].[dpa_modelli_trasm]
([id_amm], [nome], [cha_tipo_oggetto], [id_registro],[var_note_generali],[id_people], [single])
values (@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,0)
set @id_modello_trasm = (select scope_identity())
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
values (@id_modello_trasm,'M',0,0,'','','R')
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
select @id_modello_trasm,'D', dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,dts.var_note_sing, dts.cha_tipo_dest
from @db_user.dpa_trasm_singola as dts where id_trasmissione = @id_trasm
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
UPDATE [@db_user].[dpa_modelli_mitt_dest] SET cha_tipo_urp = 'P' WHERE ID_MODELLO = @id_modello_trasm
AND CHA_TIPO_URP = 'U' AND CHA_TIPO_MITT_DEST='D'
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@id_reg,
@var_note_generali,@id_people,@id_ruolo_in_uo
end
SET @id_modello_trasm = 0
SET @id_trasm = 0
SET @id_amm = 0
SET @nome = ''
SET @cha_tipo_oggetto = ''
SET @id_reg = 0
SET @var_note_generali = ''
SET @id_people = 0
SET @id_ruolo_in_uo = 0
deallocate c_modelli_trasm

PRINT '-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER DOCUMENTI GRIGI-- '
declare c_modelli_trasm cursor local for
select
dt.system_id,dtt.var_template,dt.cha_tipo_oggetto,dt.var_note_generali,
dt.id_people,dt.id_ruolo_in_uo
from dpa_trasmissione dt,profile p, dpa_templ_trasm dtt
where p.system_id = dt.id_profile and p.cha_tipo_proto = 'G' and dtt.id_trasmissione = dt.system_id
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT

open c_modelli_trasm
fetch next from c_modelli_trasm into @id_trasm,@nome,@cha_tipo_oggetto,@var_note_generali,@id_people,@id_ruolo_in_uo
while(@@fetch_status=0)
begin
set @counter = @counter + 1
print cast(@counter as varchar)+' - Migrazione Template Documento Grigio: '+@nome
select  top 1 @id_reg = der.system_id, @id_amm = der.id_amm
from dpa_l_ruolo_reg as dlrr,dpa_el_registri as der
where dlrr.id_Ruolo_in_uo = @id_ruolo_in_uo and dlrr.id_registro = der.system_id
insert into [@db_user].[dpa_modelli_trasm]
([id_amm], [nome], [cha_tipo_oggetto], [id_registro],[var_note_generali],[id_people], [single])
values (@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,0)
set @id_modello_trasm = (select scope_identity())
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
values (@id_modello_trasm,'M',0,0,'','','R')
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
select @id_modello_trasm,'D', dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,dts.var_note_sing, dts.cha_tipo_dest
from @db_user.dpa_trasm_singola as dts where id_trasmissione = @id_trasm
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
UPDATE [@db_user].[dpa_modelli_mitt_dest] SET cha_tipo_urp = 'P' WHERE ID_MODELLO = @id_modello_trasm
AND CHA_TIPO_URP = 'U' AND CHA_TIPO_MITT_DEST='D'
fetch next from c_modelli_trasm into @id_trasm,@nome,@cha_tipo_oggetto,@var_note_generali,@id_people,@id_ruolo_in_uo
end
SET @id_modello_trasm = 0
SET @id_trasm = 0
SET @id_amm = 0
SET @nome = ''
SET @cha_tipo_oggetto = ''
SET @id_reg = 0
SET @var_note_generali = ''
SET @id_people = 0
SET @id_ruolo_in_uo = 0
deallocate c_modelli_trasm

PRINT '-- START MIGRAZIONE TEMPLATE DI TRASMISSIONE PER FASCICOLI IN AMMINISTRAZIONE -- '
declare c_modelli_trasm cursor local for
select dt.system_id,P.id_amm,dtt.var_template,dt.cha_tipo_oggetto,dt.var_note_generali,dt.id_people,dt.id_ruolo_in_uo
from
@db_user.dpa_templ_trasm as dtt,@db_user.dpa_trasmissione as dt, @db_user.PROJECT as p
where dtt.id_trasmissione = dt.system_id  and dt.id_project = p.system_id
AND P.CHA_TIPO_PROJ = 'F' AND P.CHA_TIPO_FASCICOLO = 'P' AND P.ID_REGISTRO IS NULL
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT

open c_modelli_trasm
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@var_note_generali,@id_people,@id_ruolo_in_uo
while(@@fetch_status=0)
begin
set @counter = @counter + 1
print cast(@counter as varchar)+' - Migrazione Template Fascicolo: '+@nome
select top 1 @id_reg = der.system_id
from dpa_l_ruolo_reg as dlrr,dpa_el_registri as der
where dlrr.id_Ruolo_in_uo = @id_ruolo_in_uo and dlrr.id_registro = der.system_id
insert into [@db_user].[dpa_modelli_trasm]
([id_amm], [nome], [cha_tipo_oggetto], [id_registro],[var_note_generali],[id_people], [single])
values (@id_amm,@nome,@cha_tipo_oggetto,@id_reg,@var_note_generali,@id_people,0)
set @id_modello_trasm = (select scope_identity())
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
values (@id_modello_trasm,'M',0,0,'','','R')
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT
insert into [@db_user].[dpa_modelli_mitt_dest]
([id_modello],[cha_tipo_mitt_dest],[id_corr_globali],[id_ragione],[cha_tipo_trasm],[var_note_sing],[cha_tipo_urp])
select @id_modello_trasm,'D', dts.id_corr_globale, dts.id_ragione, dts.cha_tipo_trasm,dts.var_note_sing, dts.cha_tipo_dest
from @db_user.dpa_trasm_singola as dts where id_trasmissione = @id_trasm
if @@error <> 0 GOTO ROLLBACK_TRANSACTION_ROOT

UPDATE [@db_user].[dpa_modelli_mitt_dest] SET cha_tipo_urp = 'P' WHERE ID_MODELLO = @id_modello_trasm
AND CHA_TIPO_URP = 'U' AND CHA_TIPO_MITT_DEST='D'
fetch next from c_modelli_trasm into @id_trasm,@id_amm,@nome,@cha_tipo_oggetto,@var_note_generali,@id_people,@id_ruolo_in_uo
end

SET @id_modello_trasm = 0
SET @id_trasm = 0
SET @id_amm = 0
SET @nome = ''
SET @cha_tipo_oggetto = ''
SET @id_reg = 0
SET @var_note_generali = ''
SET @id_people = 0
SET @id_ruolo_in_uo = 0
deallocate c_modelli_trasm

set nocount off
COMMIT TRANSACTION ROOT
PRINT'-- COMMIT DELLA TRANSAZIONE ESEGUITO CORRETTAMENTE -- '
return 0

ROLLBACK_TRANSACTION_ROOT:
BEGIN
rollback transaction
PRINT '--ROLLBACK TRANSAZIONE ESEGUITO CORRETTAMENTE -- '
return -1
END


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO