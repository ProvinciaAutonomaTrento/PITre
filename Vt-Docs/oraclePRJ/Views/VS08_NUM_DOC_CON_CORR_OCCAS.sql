--------------------------------------------------------
--  DDL for View VS08_NUM_DOC_CON_CORR_OCCAS
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS08_NUM_DOC_CON_CORR_OCCAS" ("COD_STRUT", "TIPO_DOC", "TOTALE_DOC", "PERCENTUALE_DOC") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' 
    and p.cha_tipo_proto not in ('G','R')
    and p.ID_DOCUMENTO_PRINCIPALE is null and getSeSoloMittDestOcc(p.system_id)=1
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) , tipo ;
