
CREATE OR REPLACE FUNCTION GETCOUNTRICEVUTEINTEROP(IDDOCUMENT VARCHAR, TIPORICEVUTA VARCHAR) RETURN VARCHAR IS
COUNTRAPPORTO VARCHAR(30);
BEGIN
DECLARE
numeratore NUMBER;
denominatore NUMBER;
begin
SELECT count(1) into denominatore 
from dpa_stato_invio a, documenttypes b 
where a.id_documenttype= b.system_id 
and a.id_profile= iddocument 
and b.type_id in ('INTEROPERABILITA','SIMPLIFIEDINTEROPERABILITY');
IF(tiporicevuta='1') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and var_proto_dest is not null;
end if;
if (tiporicevuta='2') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and cha_annullato='1';
end if;
if(tiporicevuta='3') THEN
select count (1) into numeratore FROM dpa_stato_invio a where a.id_profile= iddocument and status_c_mask like '_____X%';
end if;
if(tiporicevuta='1' or tiporicevuta='2' or tiporicevuta='3') THEN
countrapporto := (numeratore||' su '|| denominatore);
else
countrapporto := '';
end if;

end;
  RETURN countrapporto;
END GETCOUNTRICEVUTEINTEROP;
/


