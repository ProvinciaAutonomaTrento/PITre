--------------------------------------------------------
--  DDL for View RDE_REGISTRO_2
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."RDE_REGISTRO_2" ("IDREGISTROREMOTO", "DESCRIZIONE", "CODICE") AS 
  SELECT system_id AS idregistroremoto, var_desc_registro AS descrizione,
var_codice AS codice
FROM dpa_el_registri
WHERE (cha_rf IS NULL OR cha_rf = '0')
AND (cha_disabilitato IS NULL OR cha_disabilitato = '0')
;
