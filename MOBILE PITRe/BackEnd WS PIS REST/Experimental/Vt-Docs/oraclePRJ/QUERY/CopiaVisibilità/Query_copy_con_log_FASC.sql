  
--COPY_VISIBILITY_FASC

   INSERT ALL
            /*+ parallel(security,4) append */
            INTO security
            (
            THING,
            ACCESSRIGHTS,
            PERSONORGROUP,
            CHA_TIPO_DIRITTO,
            VAR_NOTE_SEC,
            ID_GRUPPO_TRASM
            )
            /*+ append */ 
             into dpa_copy_log (thing, accessrights, id_ruolo_dest, cha_tipo_diritto, var_note_sec,id_ruolo_orig)
                   (SELECT P1.SYSTEM_ID,
            ACCESSRIGHTS,
            12321842,
            'A',
            'ACQUISITO PER COPIA VISIBILITA', 1 as id_gruppo_orig
            FROM
            (SELECT /*+index (s) */ THING,
            ACCESSRIGHTS
            FROM security S,
            project P
            where S.THING             = P.SYSTEM_ID
            AND S.PERSONORGROUP       = 12319775
             AND ( s.cha_tipo_diritto in('P','T','F') or (s.cha_tipo_diritto = 'A' and nvl(s.var_note_sec, 'NON ACQ') <> 'ACQUISITO PER COPIA VISIBILITA') ) 
            AND (P.CHA_TIPO_FASCICOLO = 'P'
            OR p.CHA_TIPO_FASCICOLO  IS NULL)
            AND S.ACCESSRIGHTS        > 20
            and P.CHA_TIPO_PROJ       = 'F'
             AND (p.id_registro in(86107) or p.id_registro is null) 
            
            AND NOT EXISTS
            (SELECT /*+index (p1) */ 'x'
            FROM project p1
            WHERE P1.system_id        = P.ID_FASCICOLO
            AND P1.CHA_TIPO_FASCICOLO = 'G'
            )
            AND EXISTS
            (SELECT /*+index (t) */ 'x'
            FROM project T
            WHERE t.system_id =p.id_parent
            AND cha_tipo_proj ='T'
            AND EXISTS
            (SELECT /*+index (s) */ 'x'
            from security S
            WHERE s.PERSONORGROUP IN ( 12321842 )
            AND accessrights       > 20
            AND s.thing            = T.SYSTEM_ID
            )
            )
            AND NOT EXISTS
            (SELECT /*+index (s) */ 'x'
            FROM security
            where security.THING       = P.SYSTEM_ID
            AND security.PERSONORGROUP = 12321842
            )
                       ),
            project P1
            WHERE P1.ID_FASCICOLO = thing
            OR P1.SYSTEM_ID       = THING
            )
        