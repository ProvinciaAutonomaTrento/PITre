SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[REPORT_ANNUALE_DOC_TRASM_INTEROP]
--- parametri di input
@anno int,
@id_registro int,
@mese int
AS

CREATE TABLE [@db_user].[#DOC_TRASM_AOO]
(

[VAR_COD_AMM] [varchar] (255),
[VAR_COD_AOO] [varchar] (255),
[GENNAIO] int,
[FEBBRAIO] int,
[MARZO] int,
[APRILE] int,
[MAGGIO] int,
[GIUGNO] int,
[LUGLIO] int,
[AGOSTO] int,
[SETTEMBRE] int,
[OTTOBRE] int,
[NOVEMBRE] int,
[DICEMBRE] int,
[TOT_M_SPED] int,
[TOT_SPED]int,

) ON [PRIMARY]

-- dichiarazioni variabili locali contatori
declare @contamese int
declare @totmon int
declare @totdoc int
---------------------
--dichiarazioni e setting variabili locali mensili
declare @GENNAIO INT
SET @GENNAIO = 0
declare @FEBBRAIO INT
SET @FEBBRAIO = 0
declare @MARZO INT
SET @MARZO = 0
declare @APRILE INT
SET @APRILE = 0
declare @MAGGIO INT
SET @MAGGIO = 0
declare @GIUGNO INT
SET @GIUGNO = 0
declare @LUGLIO INT
SET @LUGLIO = 0
declare @AGOSTO INT
SET @AGOSTO = 0
declare @SETTEMBRE INT
SET @SETTEMBRE = 0
declare @OTTOBRE INT
SET @OTTOBRE = 0
declare @NOVEMBRE INT
SET @NOVEMBRE = 0
declare @DICEMBRE INT
SET @DICEMBRE = 0
--set variabili locali------------
set @contamese = 1
set @totmon  = 0
set @totdoc = 0
------------------------------------
-- variabili del cursore
DECLARE @var_amm VARCHAR (255)
DECLARE @var_aoo VARCHAR (255)
DECLARE @doc_spediti int
-- cursore elenco amministrazioni, registri , e doc spediti per oguna di essa
DECLARE c_ElencoAmm CURSOR LOCAL FOR
-- recupero delle aministrazioni e registri a cui ho spedito e conto i doc spediti dalla singola AMM (input :@id_registro,@anno)
select distinct count(*) as doc_spediti, var_codice_amm,var_codice_aoo
from profile as p,dpa_stato_invio as si where cha_tipo_proto = 'P'
and id_registro = @id_registro and p.system_id = si.id_profile and year(dta_spedizione) = @anno
group by var_codice_amm, var_codice_aoo
--apro il cursore
OPEN c_ElencoAmm
--1 fetch del cursore
FETCH next from c_ElencoAmm into  @doc_spediti,@var_amm, @var_aoo

while(@@fetch_status=0)

BEGIN -----  (ciclo 1 ) per tutte le amministrazioni
---(ciclo2 )- x ogni mese)
while (@mese >= @contamese)
begin
if (@contamese = 1)
begin
select @GENNAIO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @GENNAIO
end
if (@contamese = 2)
begin
select @FEBBRAIO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @FEBBRAIO
end
if (@contamese = 3)
begin
select @MARZO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @MARZO
end
if (@contamese = 4)
begin
select @APRILE = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @APRILE
end
if (@contamese = 5)
begin
select @MAGGIO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @MAGGIO
end
if (@contamese = 6)
begin
select @GIUGNO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @GIUGNO
end
if (@contamese = 7)
begin
select @LUGLIO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @LUGLIO
end
if (@contamese = 8)
begin
select @AGOSTO = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @AGOSTO
end
if (@contamese = 9)
begin
select @SETTEMBRE = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @SETTEMBRE
end
if (@contamese = 10)
begin
select @OTTOBRE = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @OTTOBRE
end
if (@contamese =11)
begin
select @NOVEMBRE = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @NOVEMBRE
end
if (@contamese = 12)
begin
select @DICEMBRE = count(var_codice_amm) from profile as p,dpa_stato_invio as si
where cha_tipo_proto = 'P' and id_registro = @id_registro
and p.system_id = si.id_profile and month(dta_spedizione) = @contamese
and year(dta_spedizione) = @anno and var_codice_amm = @var_amm and var_codice_aoo = @var_aoo
--aggiorno conteggio
set @totmon = @totmon + @DICEMBRE
end
set @contamese = @contamese + 1
end ---(ciclo2 )
-- fine scansione mesi--> inserimento nella tabella temporanea
insert into #DOC_TRASM_AOO
(VAR_COD_AMM,VAR_COD_AOO,GENNAIO,FEBBRAIO,MARZO,APRILE,MAGGIO,GIUGNO,LUGLIO,AGOSTO,SETTEMBRE,OTTOBRE,NOVEMBRE,DICEMBRE,TOT_M_SPED,TOT_SPED)
VALUES
(@var_amm, @var_aoo, @GENNAIO, @FEBBRAIO, @MARZO, @APRILE, @MAGGIO, @GIUGNO, @LUGLIO, @AGOSTO, @SETTEMBRE, @OTTOBRE, @NOVEMBRE, @DICEMBRE, @totmon,0)
--reset variabili contatore mensili
set @totdoc = @totdoc + @totmon
set @contamese = 1
set @totmon = 0

FETCH next from c_ElencoAmm into @doc_spediti, @var_amm, @var_aoo

END ---- end (ciclo 1 )
/*Inseriamo il valore relativi all'anno*/
insert into #DOC_TRASM_AOO
(VAR_COD_AMM, VAR_COD_AOO, GENNAIO, FEBBRAIO, MARZO, APRILE, MAGGIO, GIUGNO, LUGLIO, AGOSTO, SETTEMBRE, OTTOBRE, NOVEMBRE, DICEMBRE, TOT_M_SPED, TOT_SPED)
VALUES
(0,0 ,0 ,0 ,0 ,0 ,0 ,0 ,0,0 ,0 ,0 ,0 ,0, 0 ,@totdoc)

DEALLOCATE c_ElencoAmm

select * from #DOC_TRASM_AOO



GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO