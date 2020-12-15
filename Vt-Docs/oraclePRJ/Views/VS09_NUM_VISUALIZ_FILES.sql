--------------------------------------------------------
--  DDL for View VS09_NUM_VISUALIZ_FILES
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS09_NUM_VISUALIZ_FILES" ("COD_STRUT", "TOT_VISUALIZ", "TIPO_DOC") AS 
  select DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',   codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,COUNT(G.SYSTEM_ID) TOT_VISUALIZ,
   DECODE(GROUPING(CHA_TIPO_PROTO),1,'TOTALE DOC ',    decode(cha_tipo_proto,'A','ARRIVO','P','PARTENZA','I','INTERNO','G','NP','R','STAMPA REGISTRO'
   ) )TIPO
    from  DPA_LOG G,profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and  VAR_COD_AZIONE='GET_FILE' and g.id_oggetto=p.system_id 
   and (ISALLEGATO(P.SYSTEM_ID)='1' OR ISPROTOCOLLO(P.SYSTEM_ID)='1')
group by ROLLUP  (codrfappartenzaRuolo(p.id_ruolo_creatore),cha_tipo_proto)
order by codrfappartenzaRuolo(p.id_ruolo_creatore) ;
