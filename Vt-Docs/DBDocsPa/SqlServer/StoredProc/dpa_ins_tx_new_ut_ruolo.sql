SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[dpa_ins_tx_new_ut_ruolo]
@idpeople int,
@idcorrglob int,
@returnvalue int out
AS

declare @id_trasmutente int
declare @tmpvar int
declare @sysidtx int
declare @idpeoplemitt int
declare @idruolomitt int
declare @idTrasm int
declare @ID int
declare @sysidtxut int
declare @dtainvio datetime
declare @idpeopletx int
declare @ruolo_in_uo int
declare @idragione int
declare @notegen varchar (250)
declare @note_sing varchar (250)
declare @scadenza datetime
declare @idprofile int
declare @idproject int
declare @idreg int
declare @cha_tipodest varchar(2)
declare @idtscorrglob int


BEGIN
set @returnvalue = 0

DECLARE trasm CURSOR LOCAL FOR
SELECT DISTINCT b.system_id AS ID,A.system_id as idTrasm,
a.id_people as idpeopletx,a.id_ruolo_in_uo as ruolo_in_uo,a.dta_invio as dtainvio
,a.var_note_generali as note_gen,b.id_ragione as idragione,a.id_project as idproj
,a.id_profile as idprof,b.var_note_sing as note_sing ,b.dta_scadenza as scadenza
,b.cha_tipo_dest as cha_tipodest, b.id_corr_globale as idtscorrglob
FROM
dpa_trasmissione a,dpa_trasm_singola b,dpa_trasm_utente c,dpa_ragione_trasm d
WHERE
a.system_id = b.id_trasmissione AND b.system_id = c.id_trasm_singola
AND a.dta_invio IS NOT NULL AND b.id_corr_globale = @idcorrglob
AND (a.cha_tipo_oggetto = 'D' OR a.cha_tipo_oggetto = 'F')
AND b.id_ragione = d.system_id
AND c.id_people NOT IN (@idpeople) and c.cha_in_todolist='1'

OPEN trasm
FETCH next from trasm into @ID,@idTrasm,@idpeopletx,@ruolo_in_uo,@dtainvio,@notegen,@idragione,@idproject,@idprofile,@note_sing,@scadenza,@cha_tipodest,@idtscorrglob
while(@@fetch_status=0)
BEGIN
begin

select @id_trasmutente = system_id
FROM @db_user.dpa_trasm_utente
WHERE id_trasm_singola = @ID
AND id_people = @idpeople;
if @@error <> 0 set @returnvalue = null
end
begin
DELETE FROM @db_user.dpa_trasm_utente WHERE
id_trasm_singola = @ID AND id_people = @idpeople;
if @@error <> 0 set @returnvalue = -1
end
begin

if(@id_trasmutente=null)
begin
DELETE FROM dpa_todolist WHERE id_trasm_utente = @id_trasmutente;
if @@error <> 0 set @returnvalue = -2
end
end
begin

INSERT INTO dpa_trasm_utente (id_people, id_trasm_singola, cha_vista,
cha_accettata, cha_rifiutata, cha_valida,cha_in_todolist)
VALUES (@idpeople, @ID, '0','0', '0', '1','1')
select @sysidtxut = @@identity
if @@error <> 0 set @returnvalue = -3
end
begin

set @idreg=null;
if(@idprofile is not null)
begin
set @idreg =  convert(int,@db_user.vardescribe (@idprofile, 'PROF_IDREG'))
end
INSERT INTO dpa_todolist(id_trasmissione, id_trasm_singola, id_trasm_utente,
dta_invio, id_people_mitt, id_ruolo_mitt,
id_people_dest, id_ragione_trasm, var_note_gen,
var_note_sing, dta_scadenza, id_profile, id_project,
id_ruolo_dest, id_registro, cha_tipo_trasm)
values
(@idTrasm, @ID,@sysidtxut,@dtainvio,@idpeopletx, @ruolo_in_uo,
@idpeople,@idragione, @notegen,
@note_sing, @scadenza, @idprofile, @idproject,
convert(int,@db_user.vardescribe (@idtscorrglob,'ID_GRUPPO')),@idreg,@cha_tipodest)
if @@error <> 0 set @returnvalue = -4
end

FETCH next from trasm into @ID,@idTrasm,@idpeopletx,@ruolo_in_uo,@dtainvio,@notegen,@idragione,@idproject,@idprofile,@note_sing,@scadenza,@cha_tipodest,@idtscorrglob

END
close trasm
deallocate trasm

set @returnvalue = 1


return @returnvalue
END
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO