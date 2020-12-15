--------------------------------------------------------
--  DDL for Procedure LIBERA_ACL_DOC_TRASMESSI
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."LIBERA_ACL_DOC_TRASMESSI" IS
tmpVar NUMBER;
/******************************************************************************
   NAME:       libera_acl_doc_trasmessi
   PURPOSE:    

   REVISIONS:
   Ver        Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        14/07/2009          1. Created this procedure.

   NOTES:

   Automatically available Auto Replace Keywords:
      Object Name:     libera_acl_doc_trasmessi
      Sysdate:         14/07/2009
      Date and Time:   14/07/2009, 13.50.07, and 14/07/2009 13.50.07
      Username:         (set in TOAD Options, Procedure Editor)
      Table Name:       (set in the "New PL/SQL Object" dialog)

******************************************************************************/
begin
declare
cursor dt_r is select s.thing thing,v.dta_accettata,v.SYSTEM_ID_TU,v.SYSTEM_ID_TX,s.PERSONORGROUP,v.ID_RAGIONE from security s,v_trasmissione v where v.DTA_ACCETTATA is not null 
and v.ID_PROFILE=s.thing and s.ACCESSRIGHTS=20
and ( v.ID_PEOPLE_DEST = s.PERSONORGROUP OR v.ID_CORR_GLOBALE_DEST=(select system_id from dpa_corr_globali d where d.ID_GRUPPO=s.PERSONORGROUP));

cursor dt_p is
select s.thing thing,v.dta_accettata,v.SYSTEM_ID_TU,v.SYSTEM_ID_TX,s.PERSONORGROUP,v.ID_RAGIONE from security s,v_trasmissione v where v.DTA_ACCETTATA is not null 
and v.ID_PROFILE=s.thing and s.ACCESSRIGHTS=20
and ( v.ID_PEOPLE_DEST = s.PERSONORGROUP) ;
tipodiritti char;
diritti number;

BEGIN
   tmpVar := 0;
   

   
   FOR currentdt in dt_r
   loop
 begin
    select cha_tipo_diritti into tipodiritti  from dpa_ragione_trasm t where t.SYSTEM_ID= currentdt.id_ragione;
 if (tipodiritti is not null and tipodiritti ='W')
 then 
    diritti:=63;
    else if(tipodiritti is not null and tipodiritti ='R')
    then diritti:=45;
    end if;
 end if;
execute immediate 'update security set accessrights='||diritti||' where accessrights=20 and thing='||currentdt.thing||' and personorgroup='||currentdt.PERSONORGROUP;
   
   EXCEPTION
       WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
      null;
      end ;
     
      end loop;
      
      
       FOR currentdt in dt_p
   loop
 begin
    select cha_tipo_diritti into tipodiritti  from dpa_ragione_trasm t where t.SYSTEM_ID= currentdt.id_ragione;
 if (tipodiritti is not null and tipodiritti ='W')
 then 
    diritti:=63;
    else if(tipodiritti is not null and tipodiritti ='R')
    then diritti:=45;
    end if;
 end if;
execute immediate 'update security set accessrights='||diritti||' where accessrights=20 and thing='||currentdt.thing||' and personorgroup='||currentdt.PERSONORGROUP;
   
   EXCEPTION
       WHEN OTHERS THEN
       -- Consider logging the error and then re-raise
      null;
      end ;
     
      end loop;
      
      
      end;
END libera_acl_doc_trasmessi; 

/
