--------------------------------------------------------
--  DDL for Function GETIDRUOLORIF
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIDRUOLORIF" (idruoloprot number,sysid number)
RETURN number IS risultato number;

BEGIN
if(idruoloprot is null)
then risultato:=0;
else
begin
risultato := 0;
select id_gruppo into risultato from dpa_corr_globali where id_uo in (
select d.ID_MITT_DEST from profile p,dpa_doc_arrivo_par d where d.ID_PROFILE=sysid and cha_tipo_proto='P' 
and p.ID_RUOLO_PROT =idruoloprot and 
  d.CHA_TIPO_MITT_DEST='M') and cha_riferimento ='1';
  exception when others then risultato:=0;
end;
end if;
RETURN risultato;

END getidruolorif; 

/
