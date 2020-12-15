CREATE OR REPLACE FUNCTION INFOTN_COLL.getAccessRights(id_ruolo number,id_people number,systemid number)
return number IS risultato number;
idDocumentoPrincipale number := null;
thingVar number := null;

BEGIN

thingVar := systemid;

begin

begin

select id_documento_principale into idDocumentoPrincipale
from profile where system_id = thingVar;

EXCEPTION
WHEN no_data_found THEN
idDocumentoPrincipale:=null;
end;

if (not idDocumentoPrincipale is null) then
thingVar := idDocumentoPrincipale;
end if;

select max(accessrights) into risultato
from security
where thing=thingVar and
PERSONORGROUP in (id_ruolo,id_people);

EXCEPTION
WHEN OTHERS THEN
risultato:=-1;
end;

RETURN risultato;
END getAccessRights;
/