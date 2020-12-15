--------------------------------------------------------
--  DDL for View VS04_BIS_ANNULLATI
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS04_BIS_ANNULLATI" ("COD_STRUT", "ANNULLATO", "TOT", "PERCENT") AS 
  select  DECODE(GROUPING(codrfappartenzaRuolo(p.id_ruolo_creatore)), 1, 'TOTALE DOC',  codrfappartenzaRuolo(p.id_ruolo_creatore)) COD_Strut,
   DECODE(GROUPING(decode(dta_annulla,null,' ','SI')),1,'TOTALE DOC ',decode(dta_annulla,null,' ','SI')) ANNULLATO,
       count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER 
     (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore),decode(dta_annulla,null,' ','SI'))) * 100 ,2), '999.99') || '%',20,' ') PERCENT
    from  profile p where codrfappartenzaRuolo(p.id_ruolo_creatore)!=' ' and p.ID_DOCUMENTO_PRINCIPALE is null
group by  rollup (codrfappartenzaRuolo(p.id_ruolo_creatore),decode(dta_annulla,null,' ','SI'))
order by codrfappartenzaRuolo(p.id_ruolo_creatore) ;
 

   COMMENT ON TABLE "ITCOLL_6GIU12"."VS04_BIS_ANNULLATI"  IS '-- 04 bis - annullati';
