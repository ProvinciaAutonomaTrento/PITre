--------------------------------------------------------
--  DDL for View VS05_PERC_DOC_ACQUISITI
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS05_PERC_DOC_ACQUISITI" ("COD_STRUT", "TIPO", "TOT", "PERCENT") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' and p.ID_DOCUMENTO_PRINCIPALE is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) ;
 

   COMMENT ON TABLE "ITCOLL_6GIU12"."VS05_PERC_DOC_ACQUISITI"  IS '-- 05 ***- per i soli documenti in ingresso, in uscita e interni, numero e 
--> percentuale di documenti per i quali sia stato acquisito il documento 
--> principale (output = 6 numeri)
';
