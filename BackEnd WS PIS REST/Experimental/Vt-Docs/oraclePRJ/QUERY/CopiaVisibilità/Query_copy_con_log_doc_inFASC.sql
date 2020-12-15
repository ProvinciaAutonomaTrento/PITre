--COPIA_DOC_IN_FASC_COPIA_VISIB

 INSERT      ALL
 /*+ parallel(security,4) append */INTO security
            (thing, accessrights, personorgroup, cha_tipo_diritto,
            var_note_sec,id_gruppo_trasm)
            /*+ append */ 
             into dpa_copy_log (thing, accessrights, id_ruolo_dest, cha_tipo_diritto, var_note_sec,id_ruolo_orig)
            (SELECT          /*+index (security) */
            DISTINCT (LINK),
            (SELECT accessrights
            FROM security
            WHERE thing = LINK
            AND personorgroup = 12319775
            AND ROWNUM = 1),
            12321842 , 'A', 'ACQUISITO PER COPIA VISIBILITA',1 as id_gruppo_orig
            FROM project_components pg
            WHERE pg.project_id IN (
            SELECT /*+ index (p1) */
            p1.system_id    --, accessrights, 26177060, 'A',
            --  'ACQUISITO PER COPIA VISIBILITA'
            FROM   (SELECT /*+index (s)  index (p) */
            thing, accessrights
            FROM security s, project p
            WHERE s.thing = p.system_id
            AND s.personorgroup = 12319775
             AND ( s.cha_tipo_diritto in('P','T','F') or (s.cha_tipo_diritto = 'A' and nvl(s.var_note_sec, 'NON ACQ') <> 'ACQUISITO PER COPIA VISIBILITA') ) 
            AND (   p.cha_tipo_fascicolo = 'P'
            OR p.cha_tipo_fascicolo IS NULL
            )
            AND s.accessrights > 20
            AND p.cha_tipo_proj = 'F'
             AND (p.id_registro in(86107) or p.id_registro is null) 
            AND NOT EXISTS (
            SELECT /*+index (p1) */
            'x'
            FROM project p1
            WHERE p1.system_id = p.id_fascicolo
            AND p1.cha_tipo_fascicolo = 'G')
            AND EXISTS (
            SELECT /*+index (t) */
            'x'
            FROM project t
            WHERE t.system_id = p.id_parent
            AND cha_tipo_proj = 'T'
            AND EXISTS (
            SELECT /*+index (s) */
            'x'
            FROM security s
            WHERE s.personorgroup IN
            (12321842)
            AND accessrights > 20
            AND s.thing =
            t.system_id))),
            project p1
            WHERE p1.id_fascicolo = thing
            --OR p1.system_id = thing
            
            )
            
            AND NOT EXISTS (
            SELECT /*+index (security) */
            'x'
            FROM security
            WHERE personorgroup = 12321842
            AND thing = pg.LINK))
        