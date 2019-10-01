--------------------------------------------------------
--  DDL for View RDE_UTENTE_2
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."RDE_UTENTE_2" ("IDUTENTE", "IDUTENTEREMOTO", "IDAMMINISTRAZIONEREMOTO", "COGNOME", "NOME", "CODAMMINISTRAZIONEREMOTO") AS 
  SELECT DISTINCT a.user_id AS idutente, a.system_id AS idutenteremoto,
b.id_amm AS idamministrazioneremoto,
b.var_cognome AS cognome, b.var_nome AS nome,
d.var_codice_amm AS codamministrazioneremoto
FROM dpa_corr_globali b,
peoplegroups c,
people a,
dpa_amministra d
WHERE a.system_id = b.id_people
AND a.id_amm = d.system_id
AND (b.cha_tipo_urp = 'P')
AND (b.cha_tipo_ie = 'I')
AND (b.dta_fine IS NULL)
AND (    a.system_id = c.people_system_id
AND c.groups_system_id IN (
SELECT n.id_gruppo
FROM dpa_tipo_f_ruolo m, dpa_corr_globali n
WHERE id_tipo_funz IN (
SELECT id_tipo_funzione
FROM dpa_funzioni
WHERE cod_funzione =
'PROTO_EME')
AND m.id_ruolo_in_uo = n.system_id)
)
ORDER BY b.var_cognome
;
