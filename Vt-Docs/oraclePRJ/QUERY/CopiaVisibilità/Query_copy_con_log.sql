  --COPY_VISIBILITY
  
  
  insert all
   /*+ parallel(security,4) append */  into security (thing, accessrights, personorgroup, cha_tipo_diritto, var_note_sec,id_gruppo_trasm)
     /*+ append */  into dpa_copy_log (thing, accessrights, id_ruolo_dest, cha_tipo_diritto, var_note_sec,id_ruolo_orig)
        (
        SELECT
        DISTINCT(thing), accessrights,
        12321842 as PERSONORGROUP,
        'A' as CHA_TIPO_DIRITTO,
        'ACQUISITO PER COPIA VISIBILITA' as VAR_NOTE_SEC, @idGruppoRuoloOrigine@ as id_gruppo_orig
        FROM security, profile
        WHERE
        security.thing = profile.system_id
        AND
        personorgroup = 12319775
        AND
        accessrights > 20
         AND profile.cha_tipo_proto in ('A','P','I','G') 
         AND (profile.id_registro in (86107) or profile.id_registro is null) 
        
         AND ( security.cha_tipo_diritto in('P','T','F') or (security.cha_tipo_diritto = 'A' and nvl(security.var_note_sec, 'NON ACQ') <> 'ACQUISITO PER COPIA VISIBILITA') ) 
        --Condizione di non esistenza
        --AND NOT EXISTS
      --  (
    --    select 'x' from security where thing = profile.system_id and personorgroup = 12321842
  --      )
        )
    