

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

CREATE PROCEDURE [@db_user].[REPORT_ANNUALE_REG]
/*PARAMETRI DI INPUT**************************************************************************************************************/
@mese int,
@anno int,
@id_registro int,
@id_amm int,
@VAR_SEDE varchar (255) =''
AS

--verifica valore parametro @var_sede
if (@var_sede ='')
begin
set @var_sede = null
end

/*****DICHIARAZIONE DELLE VARIABILI**********************************************************************************************/
/******************************************************************/
/**********************Dati Riepilogativi dell'anno*********************/
/******************************************************************/
declare @totAnnDoc float
declare @totAnnProt float
declare @percAnnProt float
declare @totAnnProtA float
declare @percAnnProtA float
declare @totAnnProtP float
declare @percAnnProtP float
declare @totAnnProtI float
declare @percAnnProtI float
declare @totAnnDocGrigi float
declare @percAnnDocGrigi float
declare @totAnnDocClass float
declare @percAnnDocClass float
declare @totAnnDocProf float
declare @totAnnProtClass float
declare	@percAnnProtClass float
declare @totAnnProtAClass float
declare @percAnnProtAClass float
declare @totAnnProtPClass float
declare @percAnnProtPClass float
declare @totAnnProtIClass float
declare @percAnnProtIClass float
declare @totAnnProtAnnul float
declare @percAnnProtAnnul float
/******************************************************************/
/**************Dati Riepilogativi del Mese*****************************/
/******************************************************************/
/*Dati Generali*/
declare @totMonDoc float
declare @totMonProt float
declare @totMonProtA float
declare @totMonProtP float
declare @totMonProtI float
declare @totMonProtAnnul float
declare @totMonDocGrigi float
declare @totMonDocClass float
declare @totMonClassProtAnnul float
declare @totMonClassGrigi float
/*docs senza docs acq*/
declare @totMonDocProf float
declare @totMonProtClass float
declare @totMonProtAClass float
declare @totMonProtPClass float
declare @totMonProtIClass float

/*Percentuali*/
declare @percMonProt float
declare @percMonProtA float
declare @percMonProtP float
declare @percMonProtI float
declare @percMonProtAnnul float
declare @percMonDocGrigi float
declare @percMonDocClass float
declare @percMonProtClass float
declare @percMonProtAClass float
declare @percMonProtPClass float
declare @percMonProtIClass float
/*Dichiarazione delle variabili per i profili (Immagini) *************************************************************************************************/
/*Mensili*/
declare @totMonProf float
declare @totMonProfProt float
declare @totMonProfProtA float
declare @totMonProfProtP float
declare @totMonProfProtI float
declare @totMonProfGrigi float
declare @totMonProfProtAnnul float
/*Annuali*/
declare	@totAnnProf float
declare	@totAnnProfProt float
declare	@totAnnProfProtA float
declare	@totAnnProfProtP float
declare	@totAnnProfProtI float
declare	@totAnnProfGrigi float
declare @totAnnProfProtAnnull float
/*Percentuali*/
declare	@PercAnnProfProt float
declare	@PercAnnProfProtA float
declare	@PercAnnProfProtP float
declare	@PercAnnProfProtI float
declare @PercAnnProfProtAnnull float
declare	@PercAnnProfGrigi float

declare @TotAnnDocGrigiClass float
declare @percAnnDocGrigiClass float

declare @TotAnnProtAnnulClass float
declare @percAnnProtannulClass float

/*Impostiamo i valori di default*/
/*Mensili*/
set @totMonProf  = 0
set @totMonProfProt  = 0
set @totMonProfProtA  = 0
set @totMonProfProtP  = 0
set @totMonProfProtI  = 0
set @totMonProfGrigi  = 0
/*Annuali*/
set @totAnnProf  = 0
set @totAnnProfProt  = 0
set @totAnnProfProtA  = 0
set @totAnnProfProtP = 0
set @totAnnProfProtI = 0
set @totAnnProfGrigi = 0
set @totAnnProfProtAnnull = 0
/*Percentuali*/
set @PercAnnProfProt = 0
set @PercAnnProfProtA = 0
set @PercAnnProfProtP = 0
set @PercAnnProfProtI = 0
set @PercAnnProfGrigi = 0
set @PercAnnProfProtAnnull = 0
/**************************************************************************************************************************************************/


DECLARE @MESE_VC VARCHAR (255)

set @percMonProt = 0
set @percMonProtA = 0
set @percMonProtP = 0
set @percMonProtI = 0
set @percMonProtAnnul = 0
set @percMonDocGrigi = 0
set @percMonDocClass = 0
set @percMonProtClass = 0
set @percMonProtAClass = 0
set @percMonProtPClass = 0
set @percMonProtIClass = 0
/******************************************************************/
set @totAnnDoc = 0
set @totAnnProt = 0
set @totAnnProtA = 0
set @totAnnProtP = 0
set @totAnnProtI = 0
set @totAnnDocGrigi = 0
set @totAnnDocClass = 0
set @totAnnDocProf = 0
set @totAnnProtClass = 0
set @totAnnProtAClass = 0
set @totAnnProtPClass = 0
set @totAnnProtIClass = 0
set @totAnnProtAnnul = 0
set @percAnnProt = 0
set @percAnnProtA = 0
set @percAnnProtP = 0
set @percAnnProtI = 0
set @percAnnDocGrigi = 0
set @percAnnDocClass = 0
set @percAnnProtClass = 0
set @percAnnProtAClass = 0
set @percAnnProtPClass = 0
set @percAnnProtIClass = 0
set @percAnnProtAnnul = 0

set @totMonProfProt  = 0
set @totMonProfProtA  = 0
set @totMonProfProtP  = 0
set @totMonProfProtI  = 0
set @totMonProfGrigi  = 0

set @TotAnnDocGrigiClass = 0
set @percAnnDocGrigiClass = 0
set @TotAnnProtAnnulClass = 0
set @percAnnProtannulClass = 0


/* Creo una tabella temporanea */
CREATE TABLE [@db_user].[#CC_REPORT_ANNUALE_BY_REG]
(
[THING] [varchar] (50),
[TOT_DOC] [varchar] (250),
[GRIGI] [varchar] (250),
[PERC_GRIGI] [varchar] (250),
[PROT] [varchar] (250),
[PERC_PROT] [varchar] (250),
[ANNULL] [varchar] (250),
[PERC_ANNULL] [varchar] (250),
[ARRIVO] [varchar] (250),
[PERC_ARRIVO] [varchar] (250),
[PARTENZA] [varchar] (250),
[PERC_PARTENZA] [varchar] (250),
[INTERNI] [varchar] (250),
[PERC_INTERNI] [varchar] (250),

) ON [PRIMARY]

/*cicliamo dall'inizio dell'anno fino al mese di interesse*/
declare @i int
set @i = 0
while @i < @mese
begin
/*Incrementiamo il contatore*/
set @i = @i +1
/*Query che recupera i dati del singolo mese*/
/*Totale dati del mese*/
/*Non filtriamo sul registro, questa query deve essere ripetuta per tutti i mesi di interesse per ogni registro*/
if(@var_sede <>'' and @var_sede is not null)
begin
select @totMonProtA = totMonProtA,
@totMonProtP = totMonProtP,
@totMonProtI = totMonProtI,
@totMonProtAnnul = totMonProtAnnul,
@totMonDocGrigi = totMonDocGrigi,
@totMonProfProtA  = totMonProfProtA,
@totMonProfProtP  = totMonProfProtP,
@totMonProfProtI  = totMonProfProtI,
@totMonProfProtAnnul = totMonProfProtAnnul,
@totMonProfGrigi  = totMonProfGrigi,
@totMonProtAClass = totMonProtAClass,
@totMonProtPClass = totMonProtPClass,
@totMonProtIClass = totMonProtIClass,
@totMonClassProtAnnul = totMonClassProtAnnul,
@totMonClassGrigi = totMonClassGrigi
from
(select count(system_id) as totMonProtA from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'A' AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is null) as totMonProtA,
(select count(system_id) as totMonProtP from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'P' AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is null) as totMonProtP,
(select count(system_id) as totMonProtI from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'I' AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is null) as totMonProtI,
(select count(system_id) as totMonProtAnnul from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and profile.var_sede = @var_sede and dta_annulla is not null) as totMonProtAnnul,
(select count(profile.system_id) as totMonDocGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and profile.cha_tipo_proto = 'G' and MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno and profile.var_sede = @var_sede) as totMonDocGrigi,
(select count(p.system_id) as totMonProfProtA from profile as p where cha_da_proto = '0' and cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'A' AND p.var_sede = @var_sede and dta_annulla is null) as totMonProfProtA,
(select count(p.system_id) as totMonProfProtP from profile as p where cha_da_proto = '0' and  cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'P' AND p.var_sede = @var_sede and dta_annulla is null) as totMonProfProtP,
(select count(p.system_id) as totMonProfProtI from profile as p where cha_da_proto = '0' and  cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'I' AND p.var_sede = @var_sede and dta_annulla is null) as totMonProfProtI,
(select count(p.system_id) as totMonProfProtAnnul from profile as p where cha_da_proto = '0' and cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND p.var_sede = @var_sede  and dta_annulla is not null) as totMonProfProtAnnul,
(select count(profile.system_id) as totMonProfGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and cha_img = '0' and MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno and cha_tipo_proto = 'G' AND profile.var_sede = @var_sede) as totMonProfGrigi,
(select count(system_id) As totMonProtAClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'A' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and profile.var_sede = @var_sede and dta_annulla is null) as totProtAClass,
(select count(system_id) As totMonProtPClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'P' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is null) as totProtPClass,
(select count(system_id) As totMonProtIClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'I' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is null) as totProtIClass,
(select count(system_id) As totMonClassProtAnnul from profile where cha_da_proto = '0' and id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND profile.var_sede = @var_sede and dta_annulla is not null) as totMonClassProtAnnul,
(select count(profile.system_id) As totMonClassGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and num_proto is null AND cha_tipo_proto = 'G' AND MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno AND profile.var_sede = @var_sede) as totMonClassGrigi
end
else
begin
select @totMonProtA = totMonProtA,
@totMonProtP = totMonProtP,
@totMonProtI = totMonProtI,
@totMonProtAnnul = totMonProtAnnul,
@totMonDocGrigi = totMonDocGrigi,
@totMonProfProtA  = totMonProfProtA,
@totMonProfProtP  = totMonProfProtP,
@totMonProfProtI  = totMonProfProtI,
@totMonProfProtAnnul = totMonProfProtAnnul,
@totMonProfGrigi  = totMonProfGrigi,
@totMonProtAClass = totMonProtAClass,
@totMonProtPClass = totMonProtPClass,
@totMonProtIClass = totMonProtIClass,
@totMonClassProtAnnul = totMonClassProtAnnul,
@totMonClassGrigi = totMonClassGrigi
from
(select count(system_id) as totMonProtA from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'A' AND NUM_ANNO_PROTO = @anno AND dta_annulla is null) as totMonProtA,
(select count(system_id) as totMonProtP from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'P' AND NUM_ANNO_PROTO = @anno AND dta_annulla is null) as totMonProtP,
(select count(system_id) as totMonProtI from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND cha_tipo_proto = 'I' AND NUM_ANNO_PROTO = @anno AND dta_annulla is null) as totMonProtI,
(select count(system_id) as totMonProtAnnul from profile where cha_da_proto = '0' and id_registro = @id_registro AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and dta_annulla is not null) as totMonProtAnnul,
(select count(profile.system_id) as totMonDocGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and profile.cha_tipo_proto = 'G' and MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno) as totMonDocGrigi,
(select count(p.system_id) as totMonProfProtA from profile as p where cha_da_proto = '0' and cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'A' AND dta_annulla is null) as totMonProfProtA,
(select count(p.system_id) as totMonProfProtP from profile as p where cha_da_proto = '0' and  cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'P' AND dta_annulla is null) as totMonProfProtP,
(select count(p.system_id) as totMonProfProtI from profile as p where cha_da_proto = '0' and  cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and cha_tipo_proto = 'I' AND dta_annulla is null) as totMonProfProtI,
(select count(p.system_id) as totMonProfProtAnnul from profile as p where cha_da_proto = '0' and cha_img = '0' and id_registro = @id_registro AND  MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND dta_annulla is not null) as totMonProfProtAnnul,
(select count(profile.system_id) as totMonProfGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and cha_img = '0' and MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno and cha_tipo_proto = 'G') as totMonProfGrigi,
(select count(system_id) As totMonProtAClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'A' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno and dta_annulla is null) as totProtAClass,
(select count(system_id) As totMonProtPClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'P' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND dta_annulla is null) as totProtPClass,
(select count(system_id) As totMonProtIClass from profile where cha_da_proto = '0' and  id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND cha_tipo_proto = 'I' AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND dta_annulla is null) as totProtIClass,
(select count(system_id) As totMonClassProtAnnul from profile where cha_da_proto = '0' and id_registro = @id_registro AND cha_fascicolato = '1' AND num_proto is not null AND MONTH(DTA_PROTO) = @i AND NUM_ANNO_PROTO = @anno AND dta_annulla is not null) as totMonClassProtAnnul,
(select count(profile.system_id) As totMonClassGrigi from profile,people where profile.author = people.system_id and people.id_amm = @id_amm and num_proto is null AND cha_tipo_proto = 'G' AND MONTH(CREATION_DATE) = @i AND YEAR(CREATION_DATE) = @anno) as totMonClassGrigi
end

/*Calcoliamo i valori annuali a partire dai dati del mese***************************************/
/*Documenti protocollati annullati e grigi*/
set @totMonProt = @totMonProtA + @totMonProtP + @totMonProtI + @totMonProtAnnul
set @totMonDoc = @totMonProt + @totMonDocGrigi
set @totAnnDoc = @totAnnDoc + @totMonDoc
set @totAnnProt = @totAnnProt + @totMonProt
set @totAnnProtA = @totAnnProtA + @totMonProtA
set @totAnnProtP = @totAnnProtP + @totMonProtP
set @totAnnProtI = @totAnnProtI + @totMonProtI
set @totAnnDocGrigi = @totAnnDocGrigi + @totMonDocGrigi
set @totAnnProtAnnul = @totAnnProtAnnul + @totMonProtAnnul

/*Documenti classificati protocollati annullati e grigi*/
set @totMonDocClass = @totMonClassGrigi + @totMonClassProtAnnul + @totMonProtAClass + @totMonProtPClass + @totMonProtIClass
set @totAnnDocClass = @totAnnDocClass + @totMonDocClass
set @totMonProtClass = @totMonClassProtAnnul + @totMonProtAClass + @totMonProtPClass + @totMonProtIClass
set @totAnnProtClass = @totAnnProtClass + @totMonProtClass
set @totAnnProtAClass = @totAnnProtAClass + @totMonProtAClass
set @totAnnProtPClass = @totAnnProtPClass + @totMonProtPClass
set @totAnnProtIClass = @totAnnProtIClass + @totMonProtIClass
set @totAnnProtAnnulClass = @totAnnProtAnnulClass + @totMonClassProtAnnul
set @TotAnnDocGrigiClass = @TotAnnDocGrigiClass + @totMonClassGrigi

/*Documenti senza immagine protocollati annullati e grigi*/
set @totMonProf = @totMonProfProtAnnul + @totMonProfGrigi + @totMonProfProtA + @totMonProfProtP + @totMonProfProtI
set @totAnnProf = @totAnnProf + @totMonProf
set @totMonProfProt =  @totMonProfProtAnnul + @totMonProfProtA + @totMonProfProtP + @totMonProfProtI
set @totAnnProfProt  = @totAnnProfProt + @totMonProfProt
set @totAnnProfProtA  = @totAnnProfProtA + @totMonProfProtA
set @totAnnProfProtP = @totAnnProfProtP + @totMonProfProtP
set @totAnnProfProtI = @totAnnProfProtI + @totMonProfProtI
set @totAnnProfGrigi = @totAnnProfGrigi + @totMonProfGrigi
set @totAnnProfProtAnnull = @totAnnProfProtAnnull + @totMonProfProtAnnul

/*****Percentuali************************************************************************/
/*Percentuale dei protoclli annullati classificati*/
if(@TotAnnProtAnnulClass <> 0 and @totAnnProtClass <> 0)
begin
set @percAnnProtannulClass = ROUND(((@TotAnnProtAnnulClass / @totAnnProtClass) * 100),2)
end
/*Percentuale annuale dei documenti grigi classificati*/
if(@totAnnDocClass <> 0 and @TotAnnDocGrigiClass <> 0)
begin
set @percAnnDocGrigiClass = ROUND(((@TotAnnDocGrigiClass / @totAnnDocClass) * 100),2)
end
/*Percentuale dei profili annullati*/
if((@TotAnnProfProt <> 0) AND (@totAnnProfProtAnnull <> 0))
begin
set @PercAnnProfProtAnnull = ROUND(((@totAnnProfProtAnnull / @TotAnnProfProt) * 100),2)
end
if(@totAnnProt <> 0)
begin

/*Percentuale di documenti protocollati*/
if(@totAnnProt <> 0 and @totAnnDoc <> 0)
set @percAnnProt = ROUND(((@totAnnProt / @totAnnDoc) * 100),2)
if(@totAnnProtA <> 0 and @totAnnProt <> 0)
begin
/*Percentuale di protocolli in arrivo*/
set @percAnnProtA = ROUND(((@totAnnProtA / @totAnnProt) * 100),2)
end

if(@totAnnProtP <> 0  and @totAnnProt <> 0)
begin
/*Percentuale di protocolli in partenza*/
set @percAnnProtP = ROUND(((@totAnnProtP / @totAnnProt) * 100),2)
end

if(@totAnnProtI <> 0  and @totAnnProt <> 0)
begin
/*Percentuale di protocolli interni*/
set @percAnnProtI = ROUND(((@totAnnProtI / @totAnnProt) * 100),2)
end

if(@totAnnProtAnnul <> 0  and @totAnnProt <> 0)
begin
/*Percentuale di protocolli annullati*/
set @percAnnProtAnnul = ROUND(((@totAnnProtAnnul / @totAnnProt) * 100),2)
end
end

if(@totAnnDoc <> 0)
begin
if(@totAnnDocGrigi <> 0 and @totAnnDoc <> 0)
begin
/*Percentuale di doc grigi*/
set @percAnnDocGrigi = ROUND(((@totAnnDocGrigi / @totAnnDoc ) * 100),2)
end
if(@totAnnDocClass <> 0 and @totAnnDoc <> 0)
begin
/*Percentuale di doc classificati*/
set @percAnnDocClass = ROUND(((@totAnnDocClass / @totAnnDoc) * 100),2)
end
end

if(@totAnnDocClass <> 0)
begin
if(@totAnnProtClass <> 0 and @totAnnDocClass <> 0)
begin
/*Percentuale di doc classificati e protocollati*/
set @percAnnProtClass = ROUND(((@totAnnProtClass / @totAnnDocClass)*100),2)
if(@totAnnProtAClass <> 0 and @totAnnDocClass <> 0)
begin
/*Percentuale di doc classificati e protocollati in arrivo*/
set @percAnnProtAClass = ROUND(((@totAnnProtAClass / @totAnnProtClass) * 100),2)
end
if(@totAnnProtPClass <> 0 and @totAnnDocClass <> 0)
begin
/*Percentuale di doc classificati e protocollati in partenza*/
set @percAnnProtPClass = ROUND(((@totAnnProtPClass / @totAnnProtClass) * 100),2)
end
if(@totAnnProtIClass <> 0 and @totAnnDocClass <> 0)
begin
/*Percentuale di doc classificati e protocollati interni*/
set @percAnnProtIClass = ROUND(((@totAnnProtIClass / @totAnnProtClass) * 100),2)
end
end
end

/*Calcoliamo le percentuali mensili**************************************************************************************/
if(@totMonDoc <> 0)
begin
if(@totMonProt <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di protocolli*/
set @percMonProt = ROUND(((@totMonProt / @totMonDoc) * 100),2)
if(@totMonProtA <> 0 and @totMonProt <> 0)
begin
/*Percentuale mensile di protocolli ARRIVO*/
set @percMonProtA = ROUND(((@totMonProtA / @totMonProt) * 100),2)
end
if(@totMonProtP <> 0 and @totMonProt <> 0)
begin
/*Percentuale mensile di protocolli PARTENZA*/
set @percMonProtP = ROUND(((@totMonProtP / @totMonProt) * 100),2)
end
if(@totMonProtI <> 0 and @totMonProt <> 0)
begin
/*Percentuale mensile di protocolli INTERNI*/
set @percMonProtI = ROUND(((@totMonProtI / @totMonProt) * 100),2)
end
if(@totMonProtAnnul <> 0 and @totMonProt <> 0)
begin
/*Percentuale mensile di protocolli Annullati*/
set @percMonProtAnnul = ROUND(((@totMonProtAnnul / @totMonProt) * 100),2)
end
end

if(@totMonDocGrigi <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di Doc Grigi*/
set @percMonDocGrigi = ROUND(((@totMonDocGrigi / @totMonDoc) * 100),2)
end
if(@totMonDocClass <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di Doc Class*/
set @percMonDocClass = ROUND(((@totMonDocClass / @totMonDoc) * 100),2)
end
if(@totMonProtClass <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di protocolli Class*/
set @percMonProtClass = ROUND(((@totMonProtClass / @totMonDoc) * 100),2)
end
if(@totMonProtAClass <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di protocolli Arrivo Class*/
set @percMonProtAClass = ROUND(((@totMonProtAClass / @totMonDoc) * 100),2)
end
if(@totMonProtPClass <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di protocolli Partenza Class*/
set @percMonProtPClass = ROUND(((@totMonProtPClass / @totMonDoc) * 100),2)
end
if(@totMonProtIClass <> 0 and @totMonDoc <> 0)
begin
/*Percentuale mensile di protocolli Interni Class*/
set @percMonProtIClass = ROUND(((@totMonProtIClass / @totMonDoc) * 100),2)
end

end
/*******************************************************************************************************************/
/*Calcoliamo le percentuali  dei profili ( Immagini)  */
if(@totAnnProf<>0)
begin
if(@totAnnProfGrigi<>0 and @totAnnProf <> 0)
begin
/*Percentuale  annuale di profili grigi*/
set @PercAnnProfGrigi = ROUND(((@totAnnProfGrigi / @totAnnProf) * 100),2)
end

if(@totAnnProfProt<>0 and @totAnnProf <> 0)
begin
/*Percentuale  annuale di profili protocollati*/
set @PercAnnProfProt = ROUND(((@totAnnProfProt / @totAnnProf) * 100),2)
end

if(@totAnnProfProtA<>0 and @totAnnProf <> 0)
begin
/*Percentuale  annuale di profili protocollati ARRIVO*/
set @PercAnnProfProtA = ROUND(((@totAnnProfProtA / @totAnnProfProt) * 100),2)
end

if(@totAnnProfProtP<>0 and @totAnnProf <> 0)
begin
/*Percentuale  annuale di profili protocollati PARTENZA*/
set @PercAnnProfProtP = ROUND(((@totAnnProfProtP / @totAnnProfProt) * 100),2)
end

if(@totAnnProfProtI<>0 and @totAnnProf <> 0)
begin
/*Percentuale  annuale di profili protocollati PARTENZA*/
set @PercAnnProfProtI = ROUND(((@totAnnProfProtI / @totAnnProfProt) * 100),2)
end
end
/*******************************************************************************************************************/

Select @MESE_VC =
CASE @i
WHEN 1 THEN 'Gennaio'
WHEN 2 THEN 'Febbraio'
WHEN 3 THEN 'Marzo'
WHEN 4 THEN 'Aprile'
WHEN 5 THEN 'Maggio'
WHEN 6 THEN 'Giugno'
WHEN 7 THEN 'Luglio'
WHEN 8 THEN 'Agosto'
WHEN 9 THEN 'Settembre'
WHEN 10 THEN 'Ottobre'
WHEN 11 THEN 'Novembre'
WHEN 12 THEN 'Dicembre'
end

/*inseriamo i dati mensili in una tabella*/
/*(ANNO,MESE,TOT_DOC,TOT_PROT,TOT_PROT_A,TOT_PROT_P,TOT_PROT_I,TOT_CLASS,TOT_PROF,TOT_CLASS_PROT,TOT_CLASS_PROT_A,TOT_CLASS_PROT_P,TOT_CLASS_PROT_I,TOT_DOC_GRIGI,TOT_PROT_ANNUL)*/
insert into #CC_REPORT_ANNUALE_BY_REG
(THING, TOT_DOC, GRIGI,PERC_GRIGI, PROT,PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
(@MESE_VC, @totMonDoc, @totMonDocGrigi, convert(varchar,@percMonDocGrigi), @totMonProt, convert(varchar,@percMonProt), @totMonProtAnnul, convert(varchar,@percMonProtAnnul), @totMonProtA, convert(varchar,@percMonProtA), @totMonProtP, convert(varchar,@percMonProtP), @totMonProtI, convert(varchar,@percMonProtI))


/*RESET DELLE VARIABILI*/
set @totMonDoc = 0
set @totMonProt = 0
set @totMonProtA = 0
set @totMonProtP = 0
set @totMonProtI = 0
set @totMonDocGrigi = 0
/*RESET DELLE PERCENTUALI MENSILI*/
set @percMonProt = 0
set @percMonProtA = 0
set @percMonProtP = 0
set @percMonProtI = 0
set @percMonProtAnnul = 0
set @percMonDocGrigi = 0
set @percMonDocClass = 0
set @percMonProtClass = 0
set @percMonProtAClass = 0
set @percMonProtPClass = 0
set @percMonProtIClass = 0
/**********************************/
end

/*Inseriamo nella tabella i valori reltivi all'anno*/
/*Aggiungiamo al totale dei documenti annuale il totale dei documenti grigi dell'anno */
set @totAnnDoc = @totAnnDoc
insert into #CC_REPORT_ANNUALE_BY_REG
(THING, TOT_DOC, GRIGI,PERC_GRIGI, PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
(@anno, @totAnnDoc, @totAnnDocGrigi, convert(varchar,@percAnnDocGrigi), @totAnnProt, convert(varchar,@percAnnProt), @totAnnProtAnnul, convert(varchar,@percAnnProtAnnul), @totAnnProtA, convert(varchar,@percAnnProtA), @totAnnProtP, convert(varchar,@percAnnProtP), @totAnnProtI, convert(varchar,@percAnnProtI))


/*Inseriamo nella tabella i valori reltivi alla classificazione*/
insert into #CC_REPORT_ANNUALE_BY_REG
(THING, TOT_DOC, GRIGI, PERC_GRIGI, PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
('Classificati', @totAnnDocClass, @TotAnnDocGrigiClass, @percAnnDocGrigiClass, @totAnnProtClass, convert(varchar,@percAnnProtClass), @TotAnnProtAnnulClass, @percAnnProtannulClass, @totAnnProtAClass, convert(varchar,@percAnnProtAClass), @totAnnProtPClass, convert(varchar,@percAnnProtPClass), @totAnnProtIClass, convert(varchar,@percAnnProtIClass))


/*Inseriamo nella tabella i valori reltivi alle Immagini - Doc. Fisici Acquisiti -*/
insert into #CC_REPORT_ANNUALE_BY_REG
(THING, TOT_DOC, GRIGI, PERC_GRIGI, PROT, PERC_PROT, ANNULL, PERC_ANNULL, ARRIVO, PERC_ARRIVO, PARTENZA, PERC_PARTENZA, INTERNI, PERC_INTERNI)
values
('Senza Img.', @totAnnProf, @totAnnProfGrigi, convert(varchar,@PercAnnProfGrigi), @totAnnProfProt, convert(varchar,@PercAnnProfProt), @totAnnProfProtAnnull, @PercAnnProfProtAnnull, @totAnnProfProtA, convert(varchar,@PercAnnProfProtA), @totAnnProfProtP, convert(varchar,@PercAnnProfProtP), @totMonProfProtI, convert(varchar,@PercAnnProfProtI))

/*RESET DELLE PERCENTUALI ANNUALI*/
set @percAnnProt = 0
set @percAnnProtA = 0
set @percAnnProtP = 0
set @percAnnProtI = 0
set @percAnnProtAnnul = 0
set @percAnnDocGrigi = 0
set @percAnnDocClass = 0
set @percAnnProtClass = 0
set @percAnnProtAClass = 0
set @percAnnProtPClass = 0
set @percAnnProtIClass = 0
set @PercAnnProfGrigi = 0
set @PercAnnProfProt = 0
set @PercAnnProfProtA = 0
set @PercAnnProfProtP = 0
set @PercAnnProfProtI = 0
/************************************/
/*Recuperiamo i valori dalla tabella temporanea*/
select * from #CC_REPORT_ANNUALE_BY_REG



GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO