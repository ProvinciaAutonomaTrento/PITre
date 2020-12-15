--------------------------------------------------------
--  DDL for View VS02_UT_NELL_RF
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS02_UT_NELL_RF" ("COD_STRUT", "TOT") AS 
  SELECT a.var_codice cod_strut,count(*) tot
  FROM people p,
       peoplegroups pg,
       dpa_l_ruolo_reg t,
       dpa_el_registri a,
       dpa_corr_globali cg
 WHERE t.id_registro = a.system_id
   --AND a.system_id = 160201
   AND a.cha_rf = '1'
   AND cg.system_id = t.id_ruolo_in_uo
   AND cg.id_gruppo = pg.groups_system_id
   and pg.PEOPLE_SYSTEM_ID=p.system_id
 and pg.DTA_FINE is null
   and cg.dta_fine is null     
   group by a.var_codice
   order by a.var_codice ;
