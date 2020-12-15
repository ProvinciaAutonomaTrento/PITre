
IF NOT EXISTS (
             SELECT * FROM dbo.sysobjects
             WHERE id = OBJECT_ID(N'[@db_user].[DPA_DISPOSITIVI_STAMPA]')
             AND OBJECTPROPERTY(id, N'IsUserTable') = 1)
BEGIN
CREATE TABLE [@db_user].[DPA_DISPOSITIVI_STAMPA](
Id INT IDENTITY(1,1)    NOT NULL PRIMARY KEY,
Code varchar(50)        not null,
Description varchar(50) not null,
)
END
GO

if (not exists (select * from [@db_user].[DPA_DISPOSITIVI_STAMPA] where Code='ZEBRA'))
BEGIN
	INSERT into @db_user.DPA_DISPOSITIVI_STAMPA (Code, Description) VALUES ('ZEBRA', 'ZEBRA');
END
GO

if (not exists (select * from [@db_user].[DPA_DISPOSITIVI_STAMPA] where Code='DYMO_LABEL_WRITER_400'))
BEGIN
	INSERT into @db_user.DPA_DISPOSITIVI_STAMPA (Code, Description) VALUES ('DYMO_LABEL_WRITER_400', 'DYMO LABEL WRITER 400');
END
GO

-- per sql server 2000 non si possono usare regole di on delete su FK
DECLARE @stringaversione NVARCHAR(128)
DECLARE @versione int
declare @sqlcmd nvarchar(4000)

SET @stringaversione = CAST(SERVERPROPERTY('productversion') AS NVARCHAR)
SET @versione = SUBSTRING(@stringaversione,1,CHARINDEX('.',@stringaversione)-1)
SET @versione = CAST(@versione AS INT)

if not exists(select * from syscolumns where name='ID_DISPOSITIVO_STAMPA' and id in
                       (select id from sysobjects where name='DPA_AMMINISTRA' and xtype='U')
                       )
BEGIN
ALTER TABLE [@db_user].DPA_AMMINISTRA ADD ID_DISPOSITIVO_STAMPA int NULL default 1
-- default a 1, dispositivo ZEBRA
execute sp_executesql N'update [@db_user].DPA_AMMINISTRA SET ID_DISPOSITIVO_STAMPA = 1 where ID_DISPOSITIVO_STAMPA IS NULL'

SET @sqlcmd = 'ALTER TABLE [@db_user].DPA_AMMINISTRA ADD FOREIGN KEY (ID_DISPOSITIVO_STAMPA)
    REFERENCES [@db_user].DPA_DISPOSITIVI_STAMPA(ID) '

if @versione > 8 -- versioni 2005 o superiori
   begin
   SET @sqlcmd = @sqlcmd +' on delete set NULL'
   END
print @sqlcmd
execute sp_executesql @sqlcmd
END
GO


-- per sql server 2000 non si possono usare regole di on delete su FK
DECLARE @stringaversione NVARCHAR(128)
DECLARE @versione int
declare @sqlcmd nvarchar(4000)

SET @stringaversione = CAST(SERVERPROPERTY('productversion') AS NVARCHAR)
SET @versione = SUBSTRING(@stringaversione,1,CHARINDEX('.',@stringaversione)-1)
SET @versione = CAST(@versione AS INT)

if not exists(select * from syscolumns where name='ID_DISPOSITIVO_STAMPA' and id in
(select id from sysobjects where name='PEOPLE' and xtype='U'))
BEGIN
ALTER TABLE [@db_user].PEOPLE ADD ID_DISPOSITIVO_STAMPA int NULL
execute sp_executesql N'update [@db_user].PEOPLE SET ID_DISPOSITIVO_STAMPA = 1 where ID_DISPOSITIVO_STAMPA IS NULL'

SET @sqlcmd = 'ALTER TABLE [@db_user].PEOPLE ADD FOREIGN KEY (ID_DISPOSITIVO_STAMPA)
      REFERENCES [@db_user].DPA_DISPOSITIVI_STAMPA(ID) ' --on delete set NULL

if @versione > 8 -- versioni 2005 o superiori
   begin
   SET @sqlcmd = @sqlcmd +' on delete set NULL'
   END
print @sqlcmd
execute sp_executesql @sqlcmd

END
GO


if (not exists (select * from [@db_user].[DPA_CHIAVI_CONFIGURAZIONE] where VAR_CODICE='USA_CONNECTBYPRIOR_OR_WITH'))
BEGIN
insert into [@db_user].[DPA_CHIAVI_CONFIGURAZIONE]
(
--SYSTEM_ID
ID_AMM
,VAR_CODICE
,VAR_DESCRIZIONE
,VAR_VALORE
,CHA_TIPO_CHIAVE
,CHA_VISIBILE
,CHA_MODIFICABILE
,CHA_GLOBALE)
values
(
--<SYSTEM_ID>
0
,'USA_CONNECTBYPRIOR_OR_WITH'
,'Chiave utilizzate per decidere se utilizzare le nuove query per la risalita gerarchica'
,'0'
,'B'
,'1'
,'1'
,'1');
END
GO

Insert into [@db_user].[DPA_DOCSPA]
   ( DTA_UPDATE, ID_VERSIONS_U)
 Values
   ( getdate(), '3.12.3')
GO
