--------------------------------------------------------
--  DDL for Function GETFLAGNOTIFICA
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETFLAGNOTIFICA" (idmodello number,idpeople number)

RETURN varchar IS risultato varchar(2000);

BEGIN

risultato:=0; 

begin

 
if(idmodello <> 0) then
select count(*) into risultato from dpa_modelli_dest_con_notifica dcn where dcn.id_modello_mitt_dest = idmodello and dcn.id_people = idpeople;
end if;


 

exception when others then risultato:=0; 

 

 

 

end;

 

if(risultato>0)

then risultato:=1;

end if;

 

RETURN risultato;

END getFlagNotifica; 

/
