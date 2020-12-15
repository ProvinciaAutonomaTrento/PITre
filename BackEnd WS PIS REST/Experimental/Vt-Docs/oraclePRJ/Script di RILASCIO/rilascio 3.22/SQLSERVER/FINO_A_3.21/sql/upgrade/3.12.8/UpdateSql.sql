/*
AUTORE:					  SERPI GABRIELE 
Data creazione:				  09/05/2011
Scopo della modifica:		AGGIUNGERE LA COLONNA CHA_RICEVUTA_PEC  NEL DPA_EL_REGISTRI
				
*/
if not exists(select * from syscolumns where name='CHA_RICEVUTA_PEC' and id in
		(select id from sysobjects where name='DPA_EL_REGISTRI' and xtype='U'))
BEGIN
	declare @sqlstatement Nvarchar(100)
	declare @proprietario  varchar(100)

	declare @nometabella varchar(100)
	declare @nomecolonna varchar(100)
    
    SET @proprietario = '@db_user'

    SET @nometabella = 'DPA_EL_REGISTRI'
    SET @nomecolonna = 'CHA_RICEVUTA_PEC'

   SET @sqlstatement = N'alter table [' + @proprietario + '].[' + @nometabella + '] ADD ' + @nomecolonna +'  varchar(2) '
       execute sp_executesql @sqlstatement ;
	
END
GO

/*
AUTORE:                      P. De Luca
Data creazione:                  Luglio 2011
Scopo della modifica: Aggiungere la colonna "cha_infasato" per determinare 
	se il record è stato già infasato o meno nella  DPA_CHIAVI_CONFIGURAZIONE
                
*/
-- aggiungere colonna DPA_CHIAVI_CONFIG_TEMPLATE.cha_infasato VARCHAR(1) DEFAULT 'N'

if not exists (
			SELECT * FROM syscolumns
			WHERE name='CHA_INFASATO' and id in
			(SELECT id FROM sysobjects
			WHERE id = OBJECT_ID(N'[@db_user].[DPA_CHIAVI_CONFIG_TEMPLATE]') and xtype='U')
			)
BEGIN
       ALTER TABLE [@db_user].[DPA_CHIAVI_CONFIG_TEMPLATE] ADD cha_infasato VARCHAR(1) DEFAULT 'Y'
END
GO



if exists ( select * from dbo.sysobjects
			where id = object_id(N'[@db_user].[CREA_KEYS_AMMINISTRA]') 
			and OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
drop procedure [@db_user].[CREA_KEYS_AMMINISTRA]
GO

CREATE PROCEDURE [@db_user].[CREA_KEYS_AMMINISTRA] AS
BEGIN

--TRY
BEGIN
TRANSACTION


DECLARE
@sysCurrAmm INT;
declare
@ErrorMessage varchar(100);
BEGIN

DECLARE

currAmm -- CURSORE CHE SCORRE LE AMMINISTRAZIONI
CURSOR
LOCAL FOR
SELECT
system_id
FROM
[DOCSADM].DPA_AMMINISTRA
OPEN

currAmm
FETCH
NEXT FROM currAmm
INTO
@sysCurrAmm
WHILE
@@FETCH_STATUS = 0
BEGIN

---cursore annidato x le chiavi di configurazione

DECLARE

@sysCurrKey varchar(32);
BEGIN

DECLARE
currKey -- CURSORE CHE SCORRE LE CHIAVI CON ID AMMINISTRAZIONE A NULL
CURSOR

LOCAL FOR
SELECT
var_codice
FROM
[DOCSADM].DPA_CHIAVI_CONFIG_TEMPLATE
where cha_infasato = 'N'

OPEN
currKey
FETCH
NEXT FROM currKey
INTO
@sysCurrKey
WHILE
@@FETCH_STATUS = 0
BEGIN


if not exists (select * from [DOCSADM].DPA_CHIAVI_CONFIGURAZIONE where VAR_CODICE=@sysCurrKey and ID_AMM =@sysCurrAmm)
begin
insert into [DOCSADM].DPA_CHIAVI_CONFIGURAZIONE
(ID_AMM,
VAR_CODICE ,
VAR_DESCRIZIONE ,
VAR_VALORE       ,
CHA_TIPO_CHIAVE  ,
CHA_VISIBILE     ,
CHA_MODIFICABILE ,
CHA_GLOBALE      )
(select top 1
@sysCurrAmm
as ID_AMM,
VAR_CODICE,
VAR_DESCRIZIONE,
VAR_VALORE     ,
CHA_TIPO_CHIAVE,
CHA_VISIBILE   ,
CHA_MODIFICABILE,
'0'
from [DOCSADM].DPA_CHIAVI_CONFIG_TEMPLATE where VAR_CODICE=@sysCurrKey)

end
--end

FETCH
NEXT FROM currKey
INTO
@sysCurrKey
END

CLOSE

currKey
DEALLOCATE

currKey
END


--- fine cursore annidato per chiavi di configurazione

FETCH
NEXT FROM currAmm
INTO

@sysCurrAmm
END

CLOSE

currAmm
DEALLOCATE

currAmm
END

update DPA_CHIAVI_CONFIG_TEMPLATE set cha_infasato = 'Y'; 
COMMIT TRAN
   
END

--TRY
BEGIN
--CATCH
IF
@@TRANCOUNT > 0

ROLLBACK
TRAN

END
--CATCH
SET  QUOTED_IDENTIFIER OFF
----fine nuova stored

GO


--dopo l'inserimento nella tabella dpa_chiavi_config_template, 
-- occorre eseguire la procedura CREA_KEYS_AMMINISTRA per infasare in DPA_CHIAVI_CONFIGURAZIONE
-- quindi, verificare i record in dpa_chiavi_config_template, prima di eseguire la procedura

-- FE_FASC_RAPIDA_REQUIRED	FALSE
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE tramite esecuzione della SP crea_keys_amministra
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('FE_FASC_RAPIDA_REQUIRED'
   , 'Obbligatorieta della classificazione o fascicolazione rapida'
   , 'false', 'F', '1', '1', 'N');

-- BE_SESSION_REPOSITORY_DISABLED	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('BE_SESSION_REPOSITORY_DISABLED'
   , 'Disabilita (TRUE) o Abilita (FALSE) acquisisci file immagine prima di salva/protocolla'
   , 'false', 'B', '1', '1', 'N');


-- BE_ELIMINA_MAIL_ELABORATE	0
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('BE_ELIMINA_MAIL_ELABORATE'
   , 'Eliminazione automatica delle mail pervenute su casella PEC processate da PITRE'
   , '0', 'B', '1', '1', 'N');

-- FE_TIPO_ATTO_REQUIRED	0
-- TIPO_ATTO_REQUIRED è stato sostituito da: getTipoDocObbl
--Insert into dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE   (SYSTEM_ID, VAR_CODICE, VAR_DESCRIZIONE , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
--Values (SEQ_DPA_CHIAVI_CONFIG_TEMPLATE.nextval, 'FE_TIPO_ATTO_REQUIRED', 'Obbligatorieta della tipologia documento', '0', 'F', '1', '1', 'N');


-- FE_PROTOIN_ACQ_DOC_OBBLIGATORIA 	false
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('FE_PROTOIN_ACQ_DOC_OBBLIGATORIA '
   , 'Acquisizione file obbligatoria sulla protocollazione semplificata'
   , 'false', 'F', '1', '1', 'N');

--FE_SMISTA_ABILITA_TRASM_RAPIDA	1
Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('FE_SMISTA_ABILITA_TRASM_RAPIDA'
   , 'Trasmissione rapida obligatoria  sulla protocollazione semplificata e sullo smistamento'
   , '1', 'F', '1', '1', 'N');


-- FE_ENABLE_PROT_RAPIDA_NO_UO	true
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('FE_ENABLE_PROT_RAPIDA_NO_UO'
   , 'Trasmissione obbligatoria sulla protocollazione semplificata'
   , 'true', 'F', '1', '1', 'N');

-- BE_ELIMINA_RICEVUTE_PEC	0
 Insert into @db_user.dpa_chiavi_config_template  -- poi si infasa in DPA_CHIAVI_CONFIGURAZIONE
   (VAR_CODICE
   , VAR_DESCRIZIONE
   , VAR_VALORE, CHA_TIPO_CHIAVE, CHA_VISIBILE, CHA_MODIFICABILE, cha_infasato)
 Values
   ('BE_ELIMINA_RICEVUTE_PEC'
   , 'Eliminazione automatica delle RICEVUTE PEC pervenute su casella PEC processate da PITRE '
   , '0', 'B', '1', '1', 'N');

GO

exec [@db_user].crea_keys_amministra; 
GO


