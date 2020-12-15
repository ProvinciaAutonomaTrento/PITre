begin 
Utl_Backup_Plsql_code ('PROCEDURE','sp_eredita_vis_fasc'); 
end;
/

create or replace
PROCEDURE          sp_eredita_vis_fasc (
   idcorrglobaleuo      IN       NUMBER,
   idcorrglobaleruolo   IN       NUMBER,
   idgruppo             IN       NUMBER,
   livelloruolo         IN       NUMBER,
   idregistro           IN       NUMBER,
   parilivello          IN       NUMBER,
   returnvalue          OUT      NUMBER
)
IS
BEGIN
   returnvalue := 0;

   BEGIN
      INSERT INTO security  -- hint disabled: append parallel(security,8) 
                  (thing, personorgroup, accessrights, id_gruppo_trasm,
                   cha_tipo_diritto)
         SELECT thing, personorgroup, accessrights, id_gruppo_trasm,
                cha_tipo_diritto
           FROM (SELECT          /*+ index (a) */
                         a.system_id AS thing,
                                 idgruppo AS personorgroup,
                                 255 AS accessrights, NULL AS id_gruppo_trasm,
                                 'P' AS cha_tipo_diritto
                            FROM project a
                           WHERE NOT EXISTS (
                                    SELECT /*+index (s1) */ 'x'
                                      FROM security s1
                                     WHERE s1.personorgroup = idgruppo
                                       AND s1.thing = a.system_id)
                             AND (   (    a.cha_tipo_proj = 'T'
                                      AND (   a.id_registro = idregistro
                                           OR a.id_registro IS NULL
                                          )
                                     )
                                  OR (    a.cha_tipo_proj = 'F'
                                      AND a.cha_tipo_fascicolo = 'G'
                                      AND (   a.id_registro = idregistro
                                           OR a.id_registro IS NULL
                                          )
                                     )
                                 ))
         UNION
         (SELECT          /*+ index (b) */
                  b.system_id AS thing, idgruppo AS personorgroup,
                          255 AS accessrights, NULL AS id_gruppo_trasm,
                          'P' AS cha_tipo_diritto
                     FROM project b
                    WHERE NOT EXISTS (
                             SELECT  /*+index (s1) */  'x'
                               FROM security s1
                              WHERE s1.personorgroup = idgruppo
                                AND s1.thing = b.system_id)
                      AND b.cha_tipo_proj = 'C'
                      AND id_parent IN (
                             SELECT /*+ index (project) */
                                    system_id
                               FROM project
                              WHERE cha_tipo_proj = 'F'
                                AND cha_tipo_fascicolo = 'G'
                                AND (   id_registro = idregistro
                                     OR id_registro IS NULL
                                    )));
   EXCEPTION
      WHEN DUP_VAL_ON_INDEX
      THEN
         raise;
      WHEN OTHERS
      THEN
         returnvalue := 1;
         RETURN;
   END;

   IF parilivello = 0
   THEN
      BEGIN
         INSERT  INTO security --append parallel(security,8) /* */  
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                    s.thing, idgruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                           OR s.cha_tipo_diritto = 'F'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL, 'A'
                       FROM security s, project p
                      WHERE NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id)
                        AND p.system_id = s.thing
                        AND p.system_id IN (
                               SELECT *
                                 FROM (SELECT /*+ index (a ) */
                                              a.system_id
                                         FROM project a
                                        WHERE (    (    a.cha_tipo_proj = 'F'
                                                    AND a.cha_tipo_fascicolo =
                                                                           'P'
                                                   )
                                               AND (   a.id_registro =
                                                                    idregistro
                                                    OR a.id_registro IS NULL
                                                   )
                                              ))
                               UNION
                               (SELECT /*+ index (b )  */
                                       b.system_id
                                  FROM project b
                                 WHERE b.cha_tipo_proj = 'C'
                                   AND b.id_parent IN (
                                          SELECT /*+ index (project) */
                                                 system_id
                                            FROM project
                                           WHERE cha_tipo_proj = 'F'
                                             AND cha_tipo_fascicolo = 'P'
                                             AND (   id_registro = idregistro
                                                  OR id_registro IS NULL
                                                 ))))
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello > livelloruolo
                                  AND b.id_uo = idcorrglobaleuo
                                  AND d.id_registro = idregistro);
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            raise;
         WHEN OTHERS
         THEN
            returnvalue := 2;
            RETURN;
      END;
   ELSE
      BEGIN
         INSERT -- hint disabled: append parallel(security,8) 
		 INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                    s.thing, idgruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                           OR s.cha_tipo_diritto = 'F'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL, 'A'
                       FROM security s, project p
                      WHERE NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id)
                        AND p.system_id = s.thing
                        AND p.system_id IN (
                               SELECT *
                                 FROM (SELECT /*+ index (a ) */
                                              a.system_id
                                         FROM project a
                                        WHERE (    (    a.cha_tipo_proj = 'F'
                                                    AND a.cha_tipo_fascicolo =
                                                                           'P'
                                                   )
                                               AND (   a.id_registro =
                                                                    idregistro
                                                    OR a.id_registro IS NULL
                                                   )
                                              ))
                               UNION
                               (SELECT /*+ index (b )  */
                                       b.system_id
                                  FROM project b
                                 WHERE b.cha_tipo_proj = 'C'
                                   AND b.id_parent IN (
                                          SELECT /*+ index (project) */
                                                 system_id
                                            FROM project
                                           WHERE cha_tipo_proj = 'F'
                                             AND cha_tipo_fascicolo = 'P'
                                             AND (   id_registro = idregistro
                                                  OR id_registro IS NULL
                                                 ))))
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello >= livelloruolo
                                  AND b.id_uo = idcorrglobaleuo
                                  AND d.id_registro = idregistro);
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            raise;
         WHEN OTHERS
         THEN
            returnvalue := 3;
            RETURN;
      END;
   END IF;

   IF parilivello = 0
   THEN
      BEGIN
         INSERT -- hint disabled: append parallel(security,8) 
		 INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                    s.thing, idgruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                           OR s.cha_tipo_diritto = 'F'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL, 'A'
                       FROM security s, project p
                      WHERE NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id)
                        AND p.system_id = s.thing
                        AND p.system_id IN (
                               SELECT *
                                 FROM (SELECT /*+ index (a ) */
                                              a.system_id
                                         FROM project a
                                        WHERE (    (    a.cha_tipo_proj = 'F'
                                                    AND a.cha_tipo_fascicolo =
                                                                           'P'
                                                   )
                                               AND (   a.id_registro =
                                                                    idregistro
                                                    OR a.id_registro IS NULL
                                                   )
                                              ))
                               UNION
                               (SELECT /*+ index (b )  */
                                       b.system_id
                                  FROM project b
                                 WHERE b.cha_tipo_proj = 'C'
                                   AND b.id_parent IN (
                                          SELECT /*+ index (project) */
                                                 system_id
                                            FROM project
                                           WHERE cha_tipo_proj = 'F'
                                             AND cha_tipo_fascicolo = 'P'
                                             AND (   id_registro = idregistro
                                                  OR id_registro IS NULL
                                                 ))))
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello > livelloruolo
                                  AND d.id_registro = idregistro
                                  AND b.id_uo IN (
                                         SELECT     system_id
                                               FROM dpa_corr_globali
                                              WHERE cha_tipo_ie = 'I'
                                                AND dta_fine IS NULL
                                                AND id_old = 0
                                         START WITH id_parent =
                                                               idcorrglobaleuo
                                         CONNECT BY PRIOR system_id =
                                                                     id_parent));
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            raise;
         WHEN OTHERS
         THEN
            returnvalue := 4;
            RETURN;
      END;
   ELSE
      BEGIN
         INSERT -- hint disabled: append parallel(security,8) 
		 INTO security
                     (thing, personorgroup, accessrights, id_gruppo_trasm,
                      cha_tipo_diritto)
            SELECT          /*+ index(s) index(p) */
                    s.thing, idgruppo,
                            (CASE
                                WHEN (    s.accessrights = 255
                                      AND (   s.cha_tipo_diritto = 'P'
                                           OR s.cha_tipo_diritto = 'A'
                                           OR s.cha_tipo_diritto = 'F'
                                          )
                                     )
                                   THEN 63
                                ELSE s.accessrights
                             END
                            ) AS acr,
                            NULL, 'A'
                       FROM security s, project p
                      WHERE NOT EXISTS (
                               SELECT 'x'
                                 FROM security s1
                                WHERE s1.personorgroup = idgruppo
                                  AND s1.thing = p.system_id)
                        AND p.system_id = s.thing
                        AND p.system_id IN (
                               SELECT *
                                 FROM (SELECT /*+ index (a ) */
                                              a.system_id
                                         FROM project a
                                        WHERE (    (    a.cha_tipo_proj = 'F'
                                                    AND a.cha_tipo_fascicolo =
                                                                           'P'
                                                   )
                                               AND (   a.id_registro =
                                                                    idregistro
                                                    OR a.id_registro IS NULL
                                                   )
                                              ))
                               UNION
                               (SELECT /*+ index (b )  */
                                       b.system_id
                                  FROM project b
                                 WHERE b.id_parent IN (
                                          SELECT /*+ index (project) */
                                                 system_id
                                            FROM project
                                           WHERE cha_tipo_proj = 'F'
                                             AND cha_tipo_fascicolo = 'P'
                                             AND (   id_registro = idregistro
                                                  OR id_registro IS NULL
                                                 ))))
                        AND s.personorgroup IN (
                               SELECT c.system_id
                                 FROM dpa_tipo_ruolo a,
                                      dpa_corr_globali b,
                                      GROUPS c,
                                      dpa_l_ruolo_reg d
                                WHERE a.system_id = b.id_tipo_ruolo
                                  AND b.id_gruppo = c.system_id
                                  AND d.id_ruolo_in_uo = b.system_id
                                  AND b.cha_tipo_urp = 'R'
                                  AND b.cha_tipo_ie = 'I'
                                  AND b.dta_fine IS NULL
                                  AND a.num_livello >= livelloruolo
                                  AND d.id_registro = idregistro
                                  AND b.id_uo IN (
                                         SELECT     system_id
                                               FROM dpa_corr_globali
                                              WHERE cha_tipo_ie = 'I'
                                                AND dta_fine IS NULL
                                                AND id_old = 0
                                         START WITH id_parent =
                                                               idcorrglobaleuo
                                         CONNECT BY PRIOR system_id =
                                                                     id_parent));
      EXCEPTION
         WHEN DUP_VAL_ON_INDEX
         THEN
            raise;
         WHEN OTHERS
         THEN
            returnvalue := 5;
            RETURN;
      END;
   END IF;
END; 
/
