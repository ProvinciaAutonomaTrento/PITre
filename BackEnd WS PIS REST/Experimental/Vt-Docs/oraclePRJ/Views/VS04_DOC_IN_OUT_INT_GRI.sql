--------------------------------------------------------
--  DDL for View VS04_DOC_IN_OUT_INT_GRI
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS04_DOC_IN_OUT_INT_GRI" ("COD_STRUT", "TIPO", "TOT", "PERCENT") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and p.id_documento_principale is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) ;
 

   COMMENT ON TABLE "ITCOLL_6GIU12"."VS04_DOC_IN_OUT_INT_GRI"  IS '-- Dettaglio documenti:
  --  04 - a partire dal totale documenti creati dal RF, numero e percentuale di documenti in ingresso, in uscita, interni, grigi(output = 10 numeri)';
