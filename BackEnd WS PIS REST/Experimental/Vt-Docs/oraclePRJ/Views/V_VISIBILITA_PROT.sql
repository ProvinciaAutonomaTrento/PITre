--------------------------------------------------------
--  DDL for View V_VISIBILITA_PROT
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."V_VISIBILITA_PROT" ("SEGNATURA_ID", "THING", "PERSONORGROUP", "RUOLO", "COD__RUBR_RUOLO", "USER_ID", "PEOPLE", "DIRITTO", "TIPO_DIRITTO", "ID_GRUPPO_TRASM", "NUM_PROTO", "ID_REGISTRO", "NUM_ANNO_PROTO") AS 
  SELECT p.docname segnatura_id, s.thing thing, s.personorgroup personorgroup,
       getpeoplename (s.personorgroup) ruolo,getcodRubricaRuolo(s.personorgroup) cod__rubr_ruolo,GETPEOPLEuserid(s.personorgroup) USER_ID  ,
       getdescruolo (s.personorgroup) people, s.accessrights diritto,
       s.cha_tipo_diritto tipo_diritto, s.id_gruppo_trasm,
       p.num_proto num_proto, p.id_registro id_registro,
       p.num_anno_proto num_anno_proto
  FROM security s, PROFILE p
 WHERE s.thing = p.system_id AND p.num_proto > 0 ;
