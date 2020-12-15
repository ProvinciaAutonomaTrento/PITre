--------------------------------------------------------
--  DDL for View VS13_NUM_MEDIO_DOC_IN_FASC
--------------------------------------------------------

  CREATE OR REPLACE FORCE VIEW "ITCOLL_6GIU12"."VS13_NUM_MEDIO_DOC_IN_FASC" ("COD_STRUT", "MEDIA_NUM_DOC_PER_FASC") AS 
  select   cod_strut, avg(num_doc_per_fasc) media_num_doc_per_fasc

from(

SELECT  getcodtit(p1.id_parent), DECODE (GROUPING (codrfappartenza (s.personorgroup)),
                 1, 'TOTALE DOC',
                 codrfappartenza (s.personorgroup)
                ) cod_strut,
         COUNT (LINK) num_doc_per_fasc
    FROM project p1, project_components pg,security s
   WHERE EXISTS (
            SELECT 'x'
              FROM project p, security s
             WHERE p.cha_tipo_fascicolo = 'P'
               AND s.thing = p.system_id
               AND codrfappartenza (s.personorgroup) != ' '
               AND s.accessrights = 0
               AND p1.id_fascicolo = p.system_id)
     AND p1.cha_tipo_proj = 'C'
     AND pg.project_id = p1.system_id
     and s.thing=p1.system_id and s.accessrights=0
GROUP BY  (codrfappartenza (s.personorgroup),getcodtit(p1.id_parent))
)
group by cod_strut
order by cod_strut ;
