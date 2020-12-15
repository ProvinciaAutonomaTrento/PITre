CREATE OR REPLACE FUNCTION @db_user.tmp_GetCreatore(tipoUtenteRuolo char, tipoOggetto char, id int)
RETURN int IS retValue int;
accessrights int := null;
BEGIN
retValue := 0;

if (tipoOggetto = 'D') then
if (tipoUtenteRuolo = 'U') then
select p.author into retValue
from profile p
where system_id = id;
end if;
elsif (tipoUtenteRuolo = 'R') then
select cg.id_gruppo into retValue
from dpa_corr_globali cg
where system_id =
(
select p.id_ruolo_creatore
from profile p
where system_id = id
);
end if;

if (retValue is null) then
if (tipoUtenteRuolo = 'U') then
accessrights := 0;
elsif (tipoUtenteRuolo = 'R') then
accessrights := 255;
end if;

select s.personorgroup into retValue
from security s
where rownum = 1 and
s.thing = id and
s.accessrights = accessrights and
s.cha_tipo_diritto = 'P';
end if;

RETURN retValue;
END tmp_GetCreatore;
/
