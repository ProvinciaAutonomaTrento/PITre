--------------------------------------------------------
--  DDL for Procedure BLOCCA_DA_CONV_PDF
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."BLOCCA_DA_CONV_PDF" IS


BEGIN
declare
ext  char;
idProfile number;
tmpVar number;
cursor docConv is select dc.DTA_CONVERSIONE,dc.ID_PROFILE from dpa_conv_pdf_server dc ;
dc docConv%rowtype;

begin

open docConv;



LOOP
fetch docConv into dc;
EXIT WHEN docConv%NOTFOUND;

begin
   tmpVar := 0;
   select id_profile into tmpvar from dpa_conv_pdf_server where id_profile=dc.id_Profile and  
dc.DTA_CONVERSIONE<sysdate-1/24;

   EXCEPTION
       WHEN OTHERS THEN tmpvar:=0;
end;

if(tmpvar<>0)
then
begin
select getChaImConvPDF(tmpvar) into ext from dual;

if(ext is not  null and upper(ext) ='1' )
then
delete from   dpa_conv_pdf_server where id_profile=dc.id_Profile;
delete from   dpa_checkin_checkout d where d.ID_DOCUMENT=dc.id_Profile;
commit;
end if;


   
   end; 
   end if;
      
   
end loop;

close docConv;

end;

    
END blocca_da_conv_pdf; 

/
