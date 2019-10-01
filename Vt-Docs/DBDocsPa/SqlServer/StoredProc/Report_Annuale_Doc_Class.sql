

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

create procedure [@db_user].[Report_Annuale_Doc_Class]

@ID_AMM int,
@ID_REGISTRO int,
@ID_ANNO int,
@VAR_SEDE varchar (255) =''

AS

--DICHIARAZIONI VARIABILI
declare @TotDocClass float
declare @CodClass varchar (255)
declare @DescClass varchar (255)
declare @TotDocClassVT int
declare @PercDocClassVT float
declare @Contatore float

--SETTAGGIO INIZIALE VARIABILI
set @PercDocClassVT = 0
set @TotDocClass = 0
set @Contatore = 0


--SELECT PER LA CONTA DI TUTTI I DOCUMENTI CLASSIFICATI RELATIVAMENTE AD UNA AMMINISTRAZIONE
if(@var_sede <> '' and @var_sede is not null)
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = @db_user.dpa_l_ruolo_reg.id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @TotDocClass = ( SELECT COUNT(distinct(profile.system_id)) FROM profile,dpa_l_ruolo_reg
WHERE dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = @db_user.dpa_l_ruolo_reg.id_ruolo_in_uo
AND cha_fascicolato = '1'
AND YEAR(profile.creation_date) = @id_anno)
end
--TABELLA TEMPORANEA ALLOCAZIONE RISULTATI
CREATE TABLE [@db_user].[#TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI]
(
[TOT_DOC_CLASS ] int,
[COD_CLASS] [varchar] (255),
[DESC_CLASS] [varchar] (255),
[TOT_DOC_CLASS_VT] float,
[PERC_DOC_CLASS_VT] float,

) ON [PRIMARY]

-- variabili ausiliarie per il cursore che recupera le voci di titolario
DECLARE @SYSTEM_ID_VT INT
DECLARE @DESCRIPTION_VT VARCHAR (255)
DECLARE @VAR_CODICE_VT VARCHAR (255)

-- variabili ausiliarie per il cursore che re+cupera la lista dei fascicoli
DECLARE @SYSTEM_ID_FASC INT

-- variabili ausiliarie per il cursore che recupera la lista dei folder
DECLARE @SYSTEM_ID_FOLD INT

--1 QUERY- elenco voci di titolario  -- (input : @id_amm)
DECLARE c_VociTit CURSOR LOCAL FOR -- contiene tutte le voci di titolario (TIPO "T")
select system_id,description,var_codice from project where var_codice is not null
AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
AND id_amm =@ID_AMM and cha_tipo_proj = 'T'
and (id_registro = @id_registro OR id_registro is null)
order by VAR_COD_LIV1
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT
while(@@fetch_status=0)
BEGIN
--------2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)
DECLARE c_Fascicoli CURSOR LOCAL FOR -- contiene tutti i fascicoli (TIPO "F")
select system_id
from project
where cha_tipo_proj = 'F' and id_amm = @ID_AMM
and (id_registro = @id_registro or id_registro is null)
and id_parent = @SYSTEM_ID_VT
OPEN c_Fascicoli
FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
while(@@fetch_status=0)
BEGIN
-----------------3 QUERY--Selezione di tutti i folder del fascicolo preselezionato - (input @id_amm)
DECLARE c_Folder CURSOR LOCAL FOR --contiene tutti i folder (TIPO "C")
select system_id from project
where cha_tipo_proj = 'C' and id_amm = @ID_AMM
and id_parent =  @SYSTEM_ID_FASC
and (id_registro = @id_registro or id_registro is null)
OPEN c_Folder
FETCH next from c_Folder into @SYSTEM_ID_FOLD
while(@@fetch_status=0)
BEGIN --(3 ciclo - calcolo paraziale dei doc classificati in ogni folder)
if(@var_sede <> '' and @var_sede is not null)
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
AND ((@var_sede is null and profile.var_sede is null) OR (@var_sede is not null and profile.var_sede = @var_sede)))
end
else
begin
set @Contatore = @Contatore + (select count(distinct(profile.system_id)) from project_components , profile ,dpa_l_ruolo_reg
where  project_components.project_id = @SYSTEM_ID_FOLD
AND project_components.link = profile.docnumber
AND (YEAR(profile.creation_date) = @id_anno)
AND dpa_l_ruolo_reg.id_registro = @id_registro
AND profile.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo )
end
FETCH next from c_Folder into @SYSTEM_ID_FOLD
END
--(FINE 3 ciclo)
DEALLOCATE c_Folder

FETCH next from c_Fascicoli into @SYSTEM_ID_FASC
END
--(FINE 2 ciclo)
DEALLOCATE c_Fascicoli
-- al termine della conta di tutti i documenti classificati nei fascicoli, prima di cambiare voce di titolario
-- devo inserire le informazioni nella tabella temporanea e resettare il contatore.
---- calcolo delle percentuale dei documenti classificati nella voce di titolario rispetto al totale dei classificati
if ((@Contatore <> 0) and (@TotDocClass <>0))
begin
set @PercDocClassVT = ROUND(((@Contatore / @TotDocClass) * 100),2)
end
---- inserisco le informazioni nella tabella temporanea
if (@Contatore <> 0)
begin
INSERT INTO #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI (TOT_DOC_CLASS,COD_CLASS,DESC_CLASS,TOT_DOC_CLASS_VT,PERC_DOC_CLASS_VT)
VALUES (@TotDocClass,@VAR_CODICE_VT,@DESCRIPTION_VT,@Contatore,@PercDocClassVT)
end
---- reset del contatore parziale per la conta della prossima voce di titolario
set @Contatore = 0
---- reset del valore percentuale
set @PercDocClassVT = 0
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT
END
-- (FINE 1 ciclo)
DEALLOCATE c_VociTit

-- restituzione informazioni richieste
select * from #TEMP_REPORT_ANNUALE_DOC_CLASSIFICATI


GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO