CREATE OR REPLACE PROCEDURE @db_user.putFile
(
p_versionId int,
p_filePath nvarchar2,
p_fileSize int,
p_printThumb nvarchar2,
p_iscartaceo smallint
)
IS retValue NUMBER;
docNum int := 0;

BEGIN
retValue := 0;

select docnumber into docNum from versions where version_id = p_versionId;
update 	versions
set 	subversion = 'A',
cartaceo = p_iscartaceo
where 	version_id = p_versionId;

retValue := SQL%ROWCOUNT;
if (retValue > 0) then

update components
set 	path = p_filePath,
file_size = p_fileSize,
var_impronta = p_printThumb
where 	version_id = p_versionId;

retValue := SQL%ROWCOUNT;
end if;

if (retValue > 0) then
update 	profile
set 	cha_img = '1'
where	docnumber = docNum;

retValue := SQL%ROWCOUNT;
end if;

END putFile;
/
