SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO


create function [@db_user].[GetSeRimuovibile](@idProfile INT,@idTipoAtto INT,@id_people_connesso INT,@id_people INT,@id_ruolo_creatore INT,@Cfg_Enable_canc_doc_trasm INT)
returns INT
as
begin
declare @risultato INT


SET @risultato = 0


SELECT @risultato = COUNT(NUM_PROTO) FROM PROFILE WHERE SYSTEM_ID = @idProfile

if(@risultato = 0)
begin

IF (@id_people = @id_people_connesso AND @Cfg_Enable_canc_doc_trasm=0)
BEGIN

select @risultato = count(trasm.system_id)
from dpa_trasmissione trasm, dpa_trasm_singola sing, dpa_trasm_utente ut, profile p
where trasm.system_id = sing.id_trasmissione and sing.system_id = ut.id_trasm_singola
and trasm.id_profile=p.SYSTEM_ID
and p.CHA_DA_PROTO <> 0 and p.NUM_PROTO is null
and trasm.id_profile = @idProfile

if(@risultato>0)
begin
SET @risultato = 1
return @risultato
end

END

IF @id_people <> @id_people_connesso
BEGIN

select @risultato = count(trasm.system_id)
from dpa_trasmissione trasm, dpa_trasm_singola sing, dpa_trasm_utente ut, dpa_ragione_trasm rag, profile p
where trasm.system_id = sing.id_trasmissione and sing.system_id = ut.id_trasm_singola and sing.id_ragione = rag.system_id
and trasm.id_profile=p.SYSTEM_ID
and p.CHA_DA_PROTO <> 0 and p.NUM_PROTO is null
and trasm.id_profile = @idProfile and rag.cha_tipo_ragione ='I' and upper (var_desc_ragione) = upper('INTEROPERABILITA')

if(@risultato=0)
begin
set @risultato = 1
return @risultato
end

SELECT @risultato = COUNT(*) FROM DPA_TIPO_F_RUOLO WHERE id_Ruolo_in_uo= @id_ruolo_creatore
AND ID_TIPO_FUNZ IN
(SELECT SYSTEM_ID FROM DPA_TIPO_FUNZIONE WHERE UPPER(VAR_COD_TIPO)='PRAU')
if (@risultato > 0)
begin
set @risultato = 1
return @risultato
end
END


select @risultato = count(profile.docnumber)
from profile, dpa_associazione_templates, dpa_oggetti_custom
where
profile.docnumber = dpa_associazione_templates.doc_number
and
dpa_associazione_templates.id_oggetto = dpa_oggetti_custom.system_id
and
profile.system_id = @idProfile
and
dpa_associazione_templates.id_template = @idTipoAtto
and
dpa_oggetti_custom.repertorio = 1
and
dpa_associazione_templates.valore_oggetto_db is not null
if (@risultato > 0)
begin
set @risultato = 1
return @risultato
end
end

else set @risultato = 1
return @risultato
end

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO