--------------------------------------------------------
--  DDL for View VS11A_GIACENZA_MEDIA_CON_WF
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS11A_GIACENZA_MEDIA_CON_WF" ("COD_STRUT", "MAX_ACC_DIFF", "MIN_ACC_DIFF", "MEDIA_ACC_DIFF", "MAX_RIF_DIFF", "MIN_RIF_DIFF", "MEDIA_RIF_DIFF") AS 
  SELECT   cod_strut, NUMTODSINTERVAL (MAX (acc_diff), 'DAY') max_acc_diff,
         NUMTODSINTERVAL (MIN (acc_diff), 'DAY') min_acc_diff,
         NUMTODSINTERVAL (AVG (acc_diff), 'DAY') media_acc_diff,
         NUMTODSINTERVAL (MAX (rif_diff), 'DAY') max_rif_diff,
         NUMTODSINTERVAL (MIN (rif_diff), 'DAY') min_rif_diff,
         NUMTODSINTERVAL (AVG (rif_diff), 'DAY') media_rif_diff
    FROM (SELECT codrfappartenzaruolo (p.id_ruolo_creatore) cod_strut,
                 
                     -- count(*) tot,LPAD(TO_CHAR(ROUND(RATIO_TO_REPORT(count(*)) OVER
                 -- (partition by grouping_id(codrfappartenzaRuolo(p.id_ruolo_creatore))) * 100 ,2), '999.99') || '%',20,' ') PERCENT  ,
                 -- dta_accettata,dta_rifiutata,dta_vista,dta_invio,vt.CHA_TIPO_ragione,
                 dta_accettata - dta_invio acc_diff,
                 dta_rifiutata - dta_invio rif_diff
            FROM PROFILE p, v_trasmissione vt
           WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
             AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
             AND vt.id_profile = p.system_id
             AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
             AND vt.cha_in_todolist = '0'
             AND (dta_accettata IS NOT NULL OR dta_rifiutata IS NOT NULL)
             AND vt.cha_tipo_ragione = 'W')
GROUP BY cod_strut ;
