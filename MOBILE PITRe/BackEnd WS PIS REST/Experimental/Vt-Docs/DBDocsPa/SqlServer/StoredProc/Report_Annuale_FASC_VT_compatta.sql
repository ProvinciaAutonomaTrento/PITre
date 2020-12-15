
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

create procedure [@db_user].[Report_Annuale_FASC_VT_compatta]
/***************DICHIARAZIONE PARAMETRI***********************/
@ID_AMM int,
@ID_REGISTRO int,
@ANNO int,
@MESE int

AS
/******************************************************************/
/***************DICHIARAZIONE VARIABILI***********************/
-- variabili ausiliarie per il cursore che recupera le voci di titolario
DECLARE @SYSTEM_ID_VT INT
DECLARE @DESCRIPTION_VT VARCHAR (255)
DECLARE @VAR_CODICE_VT VARCHAR (255)
DECLARE @M_FASC_CREATI int
DECLARE @FASC_CHIUSI int
DECLARE @FASC_CREATI int
DECLARE @M_FASC_CHIUSI int
DECLARE @i int
DECLARE @MESE_VC int
DECLARE @MESE_VC_C int

set @MESE_VC = 0
set @MESE_VC_C = 0

DECLARE @GENNAIO int
DECLARE @FEBBRAIO int
DECLARE @MARZO int
DECLARE @APRILE int
DECLARE @MAGGIO int
DECLARE @GIUGNO int
DECLARE @LUGLIO int
DECLARE @AGOSTO int
DECLARE @SETTEMBRE int
DECLARE @OTTOBRE int
DECLARE @NOVEMBRE int
DECLARE @DICEMBRE int

set @GENNAIO = 0
set @FEBBRAIO = 0
set @MARZO = 0
set @APRILE = 0
set @MAGGIO = 0
set @GIUGNO = 0
set @LUGLIO = 0
set @AGOSTO = 0
set @SETTEMBRE = 0
set @OTTOBRE = 0
set @NOVEMBRE = 0
set @DICEMBRE = 0

DECLARE @VT_FASC_CREATI int
DECLARE @VT_FASC_CHIUSI int
SET @i = 1
DECLARE @NUM_LIVELLO1 VARCHAR (255)
DECLARE @VAR_CODICE_LIVELLO1 VARCHAR (255)
DECLARE @DESCRIPTION__LIVELLO1 VARCHAR (255)
-- variabili ausiliarie per il cursore che recupera la lista dei fascicoli
DECLARE @SYSTEM_ID_FASC INT
-- dichiarazione delle variabili per il tot mensile di fasc. chiusi
DECLARE @GENNAIO_C int
DECLARE @FEBBRAIO_C int
DECLARE @MARZO_C int
DECLARE @APRILE_C int
DECLARE @MAGGIO_C int
DECLARE @GIUGNO_C int
DECLARE @LUGLIO_C int
DECLARE @AGOSTO_C int
DECLARE @SETTEMBRE_C int
DECLARE @OTTOBRE_C int
DECLARE @NOVEMBRE_C int
DECLARE @DICEMBRE_C int

set @GENNAIO_C = 0
set @FEBBRAIO_C = 0
set @MARZO_C = 0
set @APRILE_C = 0
set @MAGGIO_C = 0
set @GIUGNO_C = 0
set @LUGLIO_C = 0
set @AGOSTO_C = 0
set @SETTEMBRE_C = 0
set @OTTOBRE_C = 0
set @NOVEMBRE_C = 0
set @DICEMBRE_C = 0

--dichiarazione delle variabili ausiliare per il totale della singola voce di titolario
DECLARE @TOT_GENNAIO VARCHAR (255)
DECLARE @TOT_FEBBRAIO VARCHAR (255)
DECLARE @TOT_MARZO VARCHAR (255)
DECLARE @TOT_APRILE VARCHAR (255)
DECLARE @TOT_MAGGIO VARCHAR (255)
DECLARE @TOT_GIUGNO VARCHAR (255)
DECLARE @TOT_LUGLIO VARCHAR (255)
DECLARE @TOT_AGOSTO VARCHAR (255)
DECLARE @TOT_SETTEMBRE VARCHAR (255)
DECLARE @TOT_OTTOBRE VARCHAR (255)
DECLARE @TOT_NOVEMBRE VARCHAR (255)
DECLARE @TOT_DICEMBRE VARCHAR (255)
DECLARE @TOT_VT_T VARCHAR (255)
DECLARE @TOT_VT int
DECLARE @TOT_VT_C int
--inizializzazione delle  variabili ausiliare per il totale della singola voce di titolario
DECLARE @T_GENNAIO int
DECLARE @T_FEBBRAIO int
DECLARE @T_MARZO int
DECLARE @T_APRILE int
DECLARE @T_MAGGIO int
DECLARE @T_GIUGNO int
DECLARE @T_LUGLIO int
DECLARE @T_AGOSTO int
DECLARE @T_SETTEMBRE int
DECLARE @T_OTTOBRE int
DECLARE @T_NOVEMBRE int
DECLARE @T_DICEMBRE int

DECLARE @T_GENNAIO_C int
DECLARE @T_FEBBRAIO_C int
DECLARE @T_MARZO_C int
DECLARE @T_APRILE_C int
DECLARE @T_MAGGIO_C int
DECLARE @T_GIUGNO_C int
DECLARE @T_LUGLIO_C int
DECLARE @T_AGOSTO_C int
DECLARE @T_SETTEMBRE_C int
DECLARE @T_OTTOBRE_C int
DECLARE @T_NOVEMBRE_C int
DECLARE @T_DICEMBRE_C int

set @T_GENNAIO = 0
set @T_FEBBRAIO = 0
set @T_MARZO = 0
set @T_APRILE = 0
set @T_MAGGIO = 0
set @T_GIUGNO = 0
set @T_LUGLIO = 0
set @T_AGOSTO = 0
set @T_SETTEMBRE = 0
set @T_OTTOBRE = 0
set @T_NOVEMBRE = 0
set @T_DICEMBRE = 0

set @T_GENNAIO_C = 0
set @T_FEBBRAIO_C = 0
set @T_MARZO_C = 0
set @T_APRILE_C = 0
set @T_MAGGIO_C = 0
set @T_GIUGNO_C = 0
set @T_LUGLIO_C = 0
set @T_AGOSTO_C = 0
set @T_SETTEMBRE_C = 0
set @T_OTTOBRE_C = 0
set @T_NOVEMBRE_C = 0
set @T_DICEMBRE_C = 0

set @TOT_VT_T = '0/0'
set @TOT_VT = 0
set @TOT_VT_C = 0
/*******************************************************************/

--TABELLA TEMPORANEA ALLOCAZIONE RISULTATI
/*Tot. Colonne: 18*/
CREATE TABLE [@db_user].[#TEMP_REPORT_ANNUALE_FASC_VT]
(
[FASC_CREATI] [varchar] (255),
[FASC_CHIUSI] [varchar] (255),
[VAR_COD] [varchar] (255),
[VAR_DESCR] [varchar] (255),
[GENNAIO] [varchar] (255),
[FEBBRAIO] [varchar] (255),
[MARZO] [varchar] (255),
[APRILE] [varchar] (255),
[MAGGIO] [varchar] (255),
[GIUGNO] [varchar] (255),
[LUGLIO] [varchar] (255),
[AGOSTO] [varchar] (255),
[SETTEMBRE] [varchar] (255),
[OTTOBRE] [varchar] (255),
[NOVEMBRE] [varchar] (255),
[DICEMBRE] [varchar] (255),
[VT_FAC_CREATI] [varchar] (255)
) ON [PRIMARY]

/*******************************RECUPERO DEI DATI GENERALI************************************************************************************
FASC_CREATI:  Tutti i fascicoli con:
- anno di creazione pari all'anno di interesse,
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato

FASC_GEN_CREATI: Tutti i fascicoli generali:
- id_registro pari a null
- id_amm pari a quello selezionato

FASC_CHIUSI: Tutti i fascicoli chiusi:
- id_registro pari a qullo selezionato
- id_amm pari a quello selezionato
- anno della dati di chiusura pari a quello selezionato
*/
/*QUERY PER IL RECUPERO DEI DATI GENERALI*/
select @FASC_CREATI = FASC_CREATI, @FASC_CHIUSI = FASC_CHIUSI
from
(select count(distinct(system_id)) as FASC_CREATI from project where cha_tipo_proj = 'F' and year(dta_creazione) = @anno and  (id_registro = @ID_REGISTRO or id_registro is null) and id_amm = @ID_AMM and cha_stato = 'A') as A_Fasc_creati,
(select count(distinct(system_id)) as FASC_CHIUSI from project where cha_tipo_proj = 'F' and (id_registro = @ID_REGISTRO or id_registro = null) and id_amm = @ID_AMM and year(dta_chiusura)=@anno and cha_stato = 'C') as A_Fasc_chiusi

/*****************************************************************************************************************************************************/

/*1 QUERY- Recupera l'elenco delle voci di titolario  (input : @id_amm) */
/*-- contiene tutte le voci di titolario (TIPO "T")*/
DECLARE c_VociTit CURSOR LOCAL
FOR

select system_id,description,var_codice,num_livello
from project
where var_codice is not null
and id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
and id_amm =@ID_AMM and cha_tipo_proj = 'T'
and (id_registro = @id_registro OR id_registro is null)
order by var_cod_liv1

/*Apertura del cursore*/
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1

while(@@fetch_status=0)
BEGIN
if(@NUM_LIVELLO1 = 1)
begin
set @VAR_CODICE_LIVELLO1 = @VAR_CODICE_VT
set @DESCRIPTION__LIVELLO1 = @DESCRIPTION_VT
end
/*2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)*/
while @i <= @mese
begin
select @M_FASC_CREATI = M_FASC_CREATI ,@M_FASC_CHIUSI = M_FASC_CHIUSI, @VT_FASC_CREATI = VT_FASC_CREATI, @VT_FASC_CHIUSI = VT_FASC_CHIUSI
from
(select count(distinct(system_id)) as VT_FASC_CREATI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and (id_registro = @id_registro or id_registro is null) and year(dta_creazione) = @anno and cha_stato = 'A') as totPerCRVT,
(select count(distinct(system_id)) as VT_FASC_CHIUSI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and (id_registro = @id_registro or id_registro is null) and year(dta_chiusura) = @anno and cha_stato = 'C') as totPerChVT,
(select count(distinct(system_id)) as M_FASC_CREATI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and month(dta_creazione) = @i  and (id_registro = @id_registro or id_registro is null) and year(dta_creazione) = @anno and cha_stato = 'A') as m_fasc_creati,
(select count(distinct(system_id)) as M_FASC_CHIUSI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and month(dta_chiusura) = @i  and (id_registro = @id_registro or id_registro is null) and year(dta_chiusura) = @anno and cha_stato = 'C') as m_fasc_chiusi

set @MESE_VC = @VT_FASC_CREATI
set @MESE_VC_C = @VT_FASC_CHIUSI
if(@i = 1)
begin
/*Gennaio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @gennaio = @M_FASC_CREATI
set @gennaio_c = @M_FASC_CHIUSI
end
else
begin
set @gennaio = 0
set @gennaio_c = 0
end
end
if(@i = 2)
begin
/*Febbraio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @febbraio = @M_FASC_CREATI
set @febbraio_c = @M_FASC_CHIUSI
end
else
begin
set @febbraio = 0
set @febbraio_c = 0
end
end
if(@i = 3)
begin
/*Marzo*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @marzo = @M_FASC_CREATI
set @marzo_c =@M_FASC_CHIUSI
end
else
begin
set @marzo = 0
set @marzo_c = 0
end
end
if(@i = 4)
begin
/*Aprile*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @aprile = @M_FASC_CREATI
set @aprile_c = @M_FASC_CHIUSI
end
else
begin
set @aprile = 0
set @aprile_c = 0
end
end
if(@i = 5)
begin
/*MAggio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @maggio = @M_FASC_CREATI
set @maggio_c = @M_FASC_CHIUSI
end
else
begin
set @maggio = 0
set @maggio_c = 0
end
end
if(@i = 6)
begin
/*Giugno*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @giugno = @M_FASC_CREATI
set @giugno_c =@M_FASC_CHIUSI
end
else
begin
set @giugno = 0
set @giugno_c = 0
end
end
if(@i = 7)
begin
/*Luglio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @luglio = @M_FASC_CREATI
set @luglio_c = @M_FASC_CHIUSI
end
else
begin
set @luglio = 0
set @luglio_c = 0
end
end
if(@i = 8)
begin
/*Agosto*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @agosto = @M_FASC_CREATI
set @agosto_c = @M_FASC_CHIUSI
end
else
begin
set @agosto = 0
set @agosto_c = 0
end
end
if(@i = 9)
begin
/*Settembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @settembre = @M_FASC_CREATI
set @settembre_c = @M_FASC_CHIUSI
end
else
begin
set @settembre = 0
set @settembre_c = 0
end
end
if(@i = 10)
begin
/*Ottobre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @ottobre = @M_FASC_CREATI
set @ottobre_c = @M_FASC_CHIUSI
end
else
begin
set @ottobre = 0
set @ottobre_c = 0
end
end
if(@i = 11)
begin
/*Novembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @novembre = @M_FASC_CREATI
set @novembre_c = @M_FASC_CHIUSI
end
else
begin
set @novembre = 0
set @novembre_c = 0
end
end
if(@i = 12)
begin
/*Dicembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @dicembre = @M_FASC_CREATI
set @dicembre_c = @M_FASC_CHIUSI
end
else
begin
set @dicembre = 0
set @dicembre_c = 0
end
end

/*AGGIORNAMENTO DEL CONTATORE*/
set @i = @i +1

end

set @T_GENNAIO = @T_GENNAIO + @gennaio
set @T_FEBBRAIO = @T_FEBBRAIO + @FEBBRAIO
set @T_MARZO = @T_MARZO + @MARZO
set @T_APRILE = @T_APRILE + @APRILE
set @T_MAGGIO = @T_MAGGIO + @MAGGIO
set @T_GIUGNO = @T_GIUGNO + @GIUGNO
set @T_LUGLIO = @T_LUGLIO + @LUGLIO
set @T_AGOSTO = @T_AGOSTO + @AGOSTO
set @T_SETTEMBRE = @T_SETTEMBRE + @SETTEMBRE
set @T_OTTOBRE = @T_OTTOBRE + @OTTOBRE
set @T_NOVEMBRE = @T_NOVEMBRE + @NOVEMBRE
set @T_DICEMBRE = @T_DICEMBRE + @DICEMBRE

set @T_GENNAIO_C = @T_GENNAIO_C + @gennaio_C
set @T_FEBBRAIO_C = @T_FEBBRAIO_C + @FEBBRAIO_C
set @T_MARZO_C = @T_MARZO_C + @MARZO_C
set @T_APRILE_C = @T_APRILE_C + @APRILE_C
set @T_MAGGIO_C = @T_MAGGIO_C + @MAGGIO_C
set @T_GIUGNO_C = @T_GIUGNO_C + @GIUGNO_C
set @T_LUGLIO_C = @T_LUGLIO_C + @LUGLIO_C
set @T_AGOSTO_C = @T_AGOSTO_C + @AGOSTO_C
set @T_SETTEMBRE_C = @T_SETTEMBRE_C + @SETTEMBRE_C
set @T_OTTOBRE_C = @T_OTTOBRE_C + @OTTOBRE_C
set @T_NOVEMBRE_C = @T_NOVEMBRE_C + @NOVEMBRE_C
set @T_DICEMBRE_C = @T_DICEMBRE_C + @DICEMBRE_C

set @TOT_VT = @TOT_VT + @MESE_VC
set @TOT_VT_C = @T_GENNAIO_C + @T_FEBBRAIO_C + @T_MARZO_C + @T_APRILE_C + @T_MAGGIO_C + @T_GIUGNO_C + @T_LUGLIO_C + @T_AGOSTO_C + @T_SETTEMBRE_C + @T_OTTOBRE_C + @T_NOVEMBRE_C + @T_DICEMBRE_C
set @MESE_VC_C = 0
set @MESE_VC = 0

--Aggiornamento delle variabili annuali
IF ((@TOT_VT = 0) AND (@TOT_VT_C = 0)) set  @TOT_VT_T = '-'
ELSE set @TOT_VT_T = convert(varchar,@TOT_VT)+'/'+convert(varchar,@TOT_VT_C)

IF ((@t_gennaio = 0) AND (@t_gennaio_c = 0)) set  @TOT_GENNAIO = '-'
ELSE  set @TOT_GENNAIO = convert(varchar,@t_gennaio)+'/'+convert(varchar,@t_gennaio_c)

IF ((@t_febbraio = 0) AND (@t_febbraio_c = 0)) set  @TOT_FEBBRAIO = '-'
ELSE  set @TOT_FEBBRAIO = convert(varchar,@t_febbraio)+'/'+convert(varchar,@t_febbraio_c)

IF ((@t_marzo = 0) AND (@t_marzo_c = 0)) set  @TOT_MARZO = '-'
ELSE  set @TOT_MARZO = convert(varchar,@t_marzo)+'/'+convert(varchar,@t_marzo_c)

IF ((@t_aprile = 0) AND (@t_aprile = 0)) set  @TOT_APRILE = '-'
ELSE set @TOT_APRILE = convert(varchar,@t_aprile)+'/'+convert(varchar,@t_aprile_c)

IF ((@t_maggio = 0) AND (@t_maggio_c = 0)) set  @TOT_MAGGIO = '-'
ELSE set @TOT_MAGGIO = convert(varchar,@t_maggio)+'/'+convert(varchar,@t_maggio_c)

IF ((@t_giugno = 0) AND (@t_giugno_c = 0)) set  @TOT_GIUGNO = '-'
ELSE 	set @TOT_GIUGNO = convert(varchar,@t_giugno)+'/'+convert(varchar,@t_giugno_c)

IF ((@t_luglio = 0) AND (@t_luglio_c = 0)) set  @TOT_LUGLIO = '-'
ELSE 	set @TOT_LUGLIO = convert(varchar,@t_luglio)+'/'+convert(varchar,@t_luglio_c)

IF ((@t_agosto = 0) AND (@t_agosto_c = 0)) set  @TOT_AGOSTO = '-'
ELSE  set @TOT_AGOSTO = convert(varchar,@t_agosto)+'/'+convert(varchar,@t_agosto_c)

IF ((@t_settembre = 0) AND (@t_settembre_c = 0)) set  @TOT_SETTEMBRE = '-'
ELSE  set @TOT_SETTEMBRE = convert(varchar,@t_settembre)+'/'+convert(varchar,@t_settembre_c)

IF ((@t_ottobre = 0) AND (@t_ottobre_c = 0)) set  @TOT_OTTOBRE = '-'
ELSE  set @TOT_OTTOBRE = convert(varchar,@t_ottobre)+'/'+convert(varchar,@t_ottobre_c)

IF ((@t_novembre = 0) AND (@t_novembre_c = 0)) set  @TOT_NOVEMBRE = '-'
ELSE  set @TOT_NOVEMBRE = convert(varchar,@t_novembre)+'/'+convert(varchar,@t_novembre_c)

IF ((@t_dicembre = 0) AND (@t_dicembre_c = 0)) set  @TOT_DICEMBRE = '-'
ELSE set @TOT_DICEMBRE = convert(varchar,@t_dicembre)+'/'+convert(varchar,@t_dicembre_c)

/*Aggiorniamo il cursore delle voci di Titolario*/
SET @i = 1
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT,@NUM_LIVELLO1

if(@NUM_LIVELLO1=1)
begin
insert into #TEMP_REPORT_ANNUALE_FASC_VT
(FASC_CREATI,FASC_CHIUSI,VAR_COD,VAR_DESCR,GENNAIO,FEBBRAIO,MARZO,APRILE,MAGGIO,GIUGNO,LUGLIO,AGOSTO,SETTEMBRE,OTTOBRE,NOVEMBRE,DICEMBRE,VT_FAC_CREATI)
VALUES
(@FASC_CREATI,@FASC_CHIUSI,@VAR_CODICE_LIVELLO1,@DESCRIPTION__LIVELLO1,@tot_gennaio,@tot_febbraio,@tot_marzo,@tot_aprile,@tot_maggio,@tot_giugno,@tot_luglio,@tot_agosto,@tot_settembre,@tot_ottobre,@tot_novembre,@tot_dicembre,@TOT_VT_T)

--reset delle variabili
set @TOT_VT = 0
set @TOT_VT_C = 0

set @GENNAIO = 0
set @FEBBRAIO = 0
set @MARZO = 0
set @APRILE = 0
set @MAGGIO = 0
set @GIUGNO = 0
set @LUGLIO = 0
set @AGOSTO = 0
set @SETTEMBRE = 0
set @OTTOBRE = 0
set @NOVEMBRE = 0
set @DICEMBRE = 0

set @T_GENNAIO = 0
set @T_FEBBRAIO = 0

set @T_MARZO = 0
set @T_APRILE = 0
set @T_MAGGIO = 0
set @T_GIUGNO = 0
set @T_LUGLIO = 0
set @T_AGOSTO = 0
set @T_SETTEMBRE = 0
set @T_OTTOBRE = 0
set @T_NOVEMBRE = 0
set @T_DICEMBRE = 0

set @T_GENNAIO_C = 0
set @T_FEBBRAIO_C = 0
set @T_MARZO_C = 0
set @T_APRILE_C = 0
set @T_MAGGIO_C = 0
set @T_GIUGNO_C = 0
set @T_LUGLIO_C = 0
set @T_AGOSTO_C = 0
set @T_SETTEMBRE_C = 0
set @T_OTTOBRE_C = 0
set @T_NOVEMBRE_C = 0
set @T_DICEMBRE_C = 0
end
END

SELECT * FROM #TEMP_REPORT_ANNUALE_FASC_VT

GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO