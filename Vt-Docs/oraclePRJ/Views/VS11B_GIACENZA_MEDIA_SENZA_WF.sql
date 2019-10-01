--------------------------------------------------------
--  DDL for View VS11B_GIACENZA_MEDIA_SENZA_WF
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS11B_GIACENZA_MEDIA_SENZA_WF" ("COD_STRUT", "MAX_VISTA_DIFF", "MIN_VISTA_DIFF", "MEDIA_VISTA_DIFF") AS 
  SELECT   cod_strut, NUMTODSINTERVAL (MAX (vista_diff), 'DAY') max_vista_diff,
         NUMTODSINTERVAL (MIN (vista_diff), 'DAY') min_vista_diff,
         NUMTODSINTERVAL (AVG (vista_diff), 'DAY') media_vista_diff
            FROM (SELECT codrfappartenzaruolo (p.id_ruolo_creatore) cod_strut,                       
                     -- count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER
                 -- (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore))) * 100 ,2), '999.99') || '%',20,' ') PERCENT  ,
                 -- dta_accettata,dta_rifiutata,dta_vista,dta_invio,vt.CHA_TIPO_ragione,
                                 dta_vista - dta_invio vista_diff
            FROM PROFILE p, v_trasmissione vt
           WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
             AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
             AND vt.id_profile = p.system_id
             AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
             AND vt.cha_in_todolist = '0'
             AND (dta_vista IS NOT NULL )
             AND vt.cha_tipo_ragione = 'N')
GROUP BY cod_strut ;
