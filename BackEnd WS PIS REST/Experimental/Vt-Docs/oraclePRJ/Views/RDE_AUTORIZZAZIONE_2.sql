--------------------------------------------------------
--  DDL for View RDE_AUTORIZZAZIONE_2
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."RDE_AUTORIZZAZIONE_2" ("IDREGISTROREMOTO", "IDUTENTEREMOTO", "CODREGISTROREMOTO") AS 
  SELECT DISTINCT dpa_el_registri.system_id AS idregistroremoto,
people.system_id AS idutenteremoto,
dpa_el_registri.var_codice
FROM dpa_tipo_f_ruolo INNER JOIN dpa_tipo_funzione
ON dpa_tipo_f_ruolo.id_tipo_funz =
dpa_tipo_funzione.system_id
INNER JOIN dpa_el_registri INNER JOIN dpa_l_ruolo_reg
ON dpa_el_registri.system_id = dpa_l_ruolo_reg.id_registro
INNER JOIN dpa_corr_globali
ON dpa_l_ruolo_reg.id_ruolo_in_uo =
dpa_corr_globali.system_id
INNER JOIN peoplegroups
ON dpa_corr_globali.id_gruppo =
peoplegroups.groups_system_id
INNER JOIN people
ON peoplegroups.people_system_id = people.system_id
ON dpa_tipo_f_ruolo.id_ruolo_in_uo =
dpa_l_ruolo_reg.id_ruolo_in_uo
WHERE (dpa_tipo_funzione.system_id IN (
SELECT id_tipo_funzione
FROM dpa_funzioni
WHERE cod_funzione =
'PROTO_EME')
)
AND (dpa_corr_globali.dta_fine IS NULL)
AND (dpa_corr_globali.cha_tipo_urp = 'R')
AND (   dpa_el_registri.cha_rf IS NULL
OR dpa_el_registri.cha_rf = '0'
)
AND (   dpa_el_registri.cha_disabilitato IS NULL
OR dpa_el_registri.cha_disabilitato = '0'
)
ORDER BY dpa_el_registri.system_id
;
