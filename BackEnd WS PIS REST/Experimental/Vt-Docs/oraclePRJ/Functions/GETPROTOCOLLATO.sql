--------------------------------------------------------
--  DDL for Function GETPROTOCOLLATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETPROTOCOLLATO" (systemid number,tipoproto char,isProtoInternoEnabled char)
RETURN int IS risultato int;
BEGIN
begin
begin
if(tipoproto='T') then
if(isProtoInternoEnabled='1') then
select count(*) into risultato from profile where system_id=systemid and num_proto is not null and cha_tipo_proto in ('A','P','I');
else
select count(*) into risultato from profile where system_id=systemid and num_proto is not null and cha_tipo_proto in ('A','P');
end if;
else
select count(*) into risultato from profile where system_id=systemid and num_proto is not null and cha_tipo_proto = tipoproto;
end if;

exception when others then risultato:=0;
end;

RETURN risultato;
end;
END getprotocollato; 

/
