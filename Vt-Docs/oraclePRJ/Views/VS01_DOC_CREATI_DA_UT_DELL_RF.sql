--------------------------------------------------------
--  DDL for View VS01_DOC_CREATI_DA_UT_DELL_RF
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS01_DOC_CREATI_DA_UT_DELL_RF" ("COD_STRUT", "TOT") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',
   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut, count(*) tot
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' '
group by  (codrfappartenzaRuolo(p.id_ruolo_creatore)) ;
