--------------------------------------------------------
--  DDL for View VS12_N_TRASM_OLDER_THAN_30
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS12_N_TRASM_OLDER_THAN_30" ("TOT", "COD_STRUT") AS 
  select cod_strut,count(*) as tot from 
(SELECT codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) cod_strut,
                    dta_accettata - dta_invio acc_diff,
                    dta_rifiutata - dta_invio rif_diff
               FROM PROFILE p, v_trasmissione vt
              WHERE codrfappartenzaruolo (p.id_ruolo_creatore) != ' '
                AND getsedoctrasmconrag (p.system_id, 'TUTTE') = 1
                AND vt.id_profile = p.system_id
                AND codrfappartenzaruolo (vt.id_ruolo_in_uo_mitt) != ' '
                AND vt.cha_in_todolist = '0'
                AND (dta_accettata IS NOT NULL OR dta_rifiutata IS NOT NULL)
                AND vt.cha_tipo_ragione = 'W'  
                and ( NUMTODSINTERVAL(dta_accettata - dta_invio ,'DAY')>numtoDSinterval(30,'DAY')  or
                NUMTODSINTERVAL(dta_rifiutata - dta_invio,'DAY')>numtoDSinterval(30,'DAY'))   
                )
                 group by cod_strut  
                 order by cod_strut ;
