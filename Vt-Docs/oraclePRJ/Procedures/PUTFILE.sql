--------------------------------------------------------
--  DDL for Procedure PUTFILE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."PUTFILE" 
(
p_versionId int,
p_filePath nvarchar2,
p_fileSize int,
p_printThumb nvarchar2,
p_iscartaceo smallint,
p_estensione varchar,
p_isFirmato char
)
IS retValue NUMBER;
/******************************************************************************
NAME:       putFile
PURPOSE:    Impostazione dei dati per l'inserimento di un nuovo file

******************************************************************************/

docNum int := 0;

BEGIN
retValue := 0;

-- 1) Aggiornamento tabella VERSIONS
--    vengono aggiornati i campi della tabella che identificano una versione acquisita

-- Reperimento docnumber del documento
select docnumber into docNum from versions where version_id = p_versionId;

-- Aggiornamento stato acquisito della versione
update     versions
set     subversion = 'A',
cartaceo = p_iscartaceo
where     version_id = p_versionId;

retValue := SQL%ROWCOUNT;

-- 2) Aggiornamento tabella COMPONENTS

if (retValue > 0) then
-- Aggiornamento stato acquisito della versione
if (p_isFirmato='1') then
update components
set  path = p_filePath,
file_size = p_fileSize,
var_impronta = p_printThumb,
ext =p_estensione,
cha_firmato = '1'
where     version_id = p_versionId;
else
update components
set     path = p_filePath,
file_size = p_fileSize,
var_impronta = p_printThumb,
ext =p_estensione,
cha_firmato = '0'
where     version_id = p_versionId;
end if;

retValue := SQL%ROWCOUNT;
end if;

-- 3) Aggiornamento tabella PROFILE
if (retValue > 0) then
if (p_isFirmato='1') then
update     profile
set     cha_img = '1', cha_firmato = '1'
where    docnumber = docNum;
else
update     profile
set     cha_img = '1', cha_firmato = '0'
where    docnumber = docNum;
end if;
retValue := SQL%ROWCOUNT;
end if;

END putFile;

/
