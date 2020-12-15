--------------------------------------------------------
--  DDL for View VS06_DOC_SENZA_ALLEGATI
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS06_DOC_SENZA_ALLEGATI" ("COD_STRUT", "TIPO", "TOT", "PERCENT") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and getchaimg(p.docnumber)='1' and p.ID_DOCUMENTO_PRINCIPALE is null and noAllegati(p.SYSTEM_ID)=0
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) ;
 

   COMMENT ON TABLE "ITCOLL_6GIU12"."VS06_DOC_SENZA_ALLEGATI"  IS '--Dettaglio allegati (dei documenti creati dal RF):
-->   06 *** - numero di documenti in ingresso, uscita e interni che non
--presentano
--> allegati (output = 3 numeri)';
