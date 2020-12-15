

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


create procedure [@db_user].[Report_Annuale_FASC_VT]
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
DECLARE @MESE_VC VARCHAR (255)
DECLARE @GENNAIO varchar (255)
set @GENNAIO = '-'
DECLARE @FEBBRAIO varchar (255)
set @FEBBRAIO = '-'
DECLARE @MARZO varchar (255)
set @MARZO = '-'
DECLARE @APRILE varchar (255)
set @APRILE = '-'
DECLARE @MAGGIO varchar (255)
set @MAGGIO ='-'
DECLARE @GIUGNO varchar (255)
set @GIUGNO = '-'
DECLARE @LUGLIO varchar (255)
set @LUGLIO = '-'
DECLARE @AGOSTO varchar (255)
set @AGOSTO = '-'
DECLARE @SETTEMBRE varchar (255)
set @SETTEMBRE ='-'
DECLARE @OTTOBRE varchar (255)
set @OTTOBRE = '-'
DECLARE @NOVEMBRE varchar (255)
set @NOVEMBRE ='-'
DECLARE @DICEMBRE varchar (255)
set @DICEMBRE = '-'
DECLARE @VT_FASC_CREATI int
DECLARE @VT_FASC_CHIUSI int
SET @i = 1
-- variabili ausiliarie per il cursore che recupera la lista dei fascicoli
DECLARE @SYSTEM_ID_FASC INT
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

select system_id,description,var_codice
from project
where var_codice is not null
and
id_amm =@ID_AMM and cha_tipo_proj = 'T'
AND id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T')
and (id_registro = @id_registro OR id_registro is null)
order by VAR_COD_LIV1


/*Apertura del cursore*/
OPEN c_VociTit
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT

while(@@fetch_status=0)
BEGIN
/*2 QUERY- selezione dei fascicoli dellla relativa voce di titolario- (input @id_amm)*/
while @i <= @mese
begin
select @M_FASC_CREATI = M_FASC_CREATI ,@M_FASC_CHIUSI = M_FASC_CHIUSI, @VT_FASC_CREATI = VT_FASC_CREATI, @VT_FASC_CHIUSI = VT_FASC_CHIUSI
from
(select count(distinct(system_id)) as VT_FASC_CREATI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and (id_registro = @id_registro or id_registro is null) and year(dta_creazione) = @anno and cha_stato = 'A') as totPerCRVT,
(select count(distinct(system_id)) as VT_FASC_CHIUSI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and (id_registro = @id_registro or id_registro is null) and year(dta_chiusura) = @anno and cha_stato = 'C') as totPerChVT,
(select count(distinct(system_id)) as M_FASC_CREATI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and month(dta_creazione) = @i  and (id_registro = @id_registro or id_registro is null) and year(dta_creazione) = @anno and cha_stato = 'A') as m_fasc_creati,
(select count(distinct(system_id)) as M_FASC_CHIUSI from project where cha_tipo_proj = 'F' and id_amm = @ID_AMM and id_parent = @SYSTEM_ID_VT and month(dta_chiusura) = @i  and (id_registro = @id_registro or id_registro is null) and year(dta_chiusura) = @anno and cha_stato = 'C') as m_fasc_chiusi


if ((@VT_FASC_CREATI = 0) AND (@VT_FASC_CHIUSI = 0))
begin
set @MESE_VC = '-'
end
else
begin
set @MESE_VC = convert(varchar,@VT_FASC_CREATI)+'/'+convert(varchar,@VT_FASC_CHIUSI)
end
if(@i = 1)
begin
/*Gennaio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @gennaio = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @gennaio = '-'
end
end
if(@i = 2)
begin
/*Febbraio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @febbraio = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @febbraio = '-'
end

end
if(@i = 3)
begin
/*Marzo*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @marzo = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @marzo = '-'
end

end
if(@i = 4)
begin
/*Aprile*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @aprile = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @aprile = '-'
end
end
if(@i = 5)
begin
/*MAggio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @maggio = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @maggio = '-'
end
end
if(@i = 6)
begin
/*Giugno*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @giugno = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @giugno = '-'
end
end
if(@i = 7)
begin
/*Luglio*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @luglio = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @luglio = '-'
end
end
if(@i = 8)
begin
/*Agosto*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @agosto = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @agosto = '-'
end
end
if(@i = 9)
begin
/*Settembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @settembre = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @settembre = '-'
end
end
if(@i = 10)
begin
/*Ottobre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @ottobre = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @ottobre = '-'
end
end
if(@i = 11)
begin
/*Novembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @novembre = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @novembre = '-'
end
end
if(@i = 12)
begin
/*Dicembre*/
if(@M_FASC_CREATI>0 OR @M_FASC_CHIUSI >0)
begin
set @dicembre = convert(varchar,@M_FASC_CREATI)+'/'+convert(varchar,@M_FASC_CHIUSI)
end
else
begin
set @dicembre = '-'
end
end

set @i = @i +1

end

insert into #TEMP_REPORT_ANNUALE_FASC_VT
(FASC_CREATI,FASC_CHIUSI,VAR_COD,VAR_DESCR,GENNAIO,FEBBRAIO,MARZO,APRILE,MAGGIO,GIUGNO,LUGLIO,AGOSTO,SETTEMBRE,OTTOBRE,NOVEMBRE,DICEMBRE,VT_FAC_CREATI)
VALUES
(@FASC_CREATI,@FASC_CHIUSI,@VAR_CODICE_VT,@DESCRIPTION_VT,@gennaio,@febbraio,@marzo,@aprile,@maggio,@giugno,@luglio,@agosto,@settembre,@ottobre,@novembre,@dicembre,@MESE_VC)

/*Aggiorniamo il cursore delle voci di Titolario*/
SET @i = 1
FETCH next from c_VociTit into @SYSTEM_ID_VT,@DESCRIPTION_VT,@VAR_CODICE_VT
END

SELECT * FROM #TEMP_REPORT_ANNUALE_FASC_VT

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
