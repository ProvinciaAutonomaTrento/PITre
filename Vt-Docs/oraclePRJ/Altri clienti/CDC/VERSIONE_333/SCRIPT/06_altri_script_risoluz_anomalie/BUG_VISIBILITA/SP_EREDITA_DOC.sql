

CREATE OR REPLACE PROCEDURE sp_eredita_vis_doc(
    idcorrglobaleuo    IN NUMBER,
    idcorrglobaleruolo IN NUMBER,
    idgruppo           IN NUMBER,
    livelloruolo       IN NUMBER,
    idregistro         IN NUMBER,
    parilivello        IN NUMBER,
    returnvalue OUT NUMBER )
IS
BEGIN
  returnvalue   := 0;
  IF parilivello = 0 THEN
    BEGIN
      INSERT       -- hint disabled: append parallel(security,8) 
      INTO security
        (
          thing,
          personorgroup,
          accessrights,
          id_gruppo_trasm,
          cha_tipo_diritto
        )
      SELECT
        /*+ index(s) index(p) */
        DISTINCT s.thing,
        idgruppo,
        (
        CASE
          WHEN ( s.accessrights    = 255
          AND ( s.cha_tipo_diritto = 'P'
          OR s.cha_tipo_diritto    = 'A' ) )
          THEN 63
          ELSE s.accessrights
        END ) AS acr,
        NULL, --  SABRI s.ID_GRUPPO_TRASM,
        'A'
      FROM security s,
        PROFILE p
      WHERE p.system_id    = s.thing
      AND p.cha_privato    = '0'
      AND ( p.id_registro  = idregistro
      OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a,
          dpa_corr_globali b,
          GROUPS c,
          dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = 'R'
        AND b.cha_tipo_ie    = 'I'
        AND b.dta_fine      IS NULL
        AND a.num_livello    > livelloruolo
        AND b.id_uo          = idcorrglobaleuo
        AND d.id_registro    = idregistro
        )
      AND NOT EXISTS
        (SELECT
          /*+index (s1) */
          'x'
        FROM security s1
        WHERE s1.personorgroup = idgruppo
        AND s1.thing           = p.system_id
        );
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      raise;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
  ELSE
    BEGIN
      INSERT	--        append parallel(security,8) /* */
      INTO security
        (
          thing,
          personorgroup,
          accessrights,
          id_gruppo_trasm,
          cha_tipo_diritto
        )
      SELECT
        /*+ index(s) index(p) */
        DISTINCT s.thing,
        idgruppo,
        (
        CASE
          WHEN ( s.accessrights    = 255
          AND ( s.cha_tipo_diritto = 'P'
          OR s.cha_tipo_diritto    = 'A' ) )
          THEN 63
          ELSE s.accessrights
        END ) AS acr,
        NULL, --  SABRI s.ID_GRUPPO_TRASM,
        'A'
      FROM security s,
        PROFILE p
      WHERE p.system_id    = s.thing
      AND p.cha_privato    = '0'
      AND ( p.id_registro  = idregistro
      OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        ( SELECT DISTINCT c.system_id
        FROM dpa_tipo_ruolo a,
          dpa_corr_globali b,
          GROUPS c,
          dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = 'R'
        AND b.cha_tipo_ie    = 'I'
        AND b.dta_fine      IS NULL
        AND a.num_livello   >= livelloruolo
        AND b.id_uo          = idcorrglobaleuo
        AND d.id_registro    = idregistro
        )
      AND NOT EXISTS
        (SELECT
          /*+index (s1) */
          'x'
        FROM security s1
        WHERE s1.personorgroup = idgruppo
        AND s1.thing           = p.system_id
        );
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      raise;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
  END IF;
  /* UO INFERIORI */
  IF parilivello = 0 THEN
    BEGIN
      INSERT -- hint disabled: append parallel(security,8) 
      INTO security
        (
          thing,
          personorgroup,
          accessrights,
          id_gruppo_trasm,
          cha_tipo_diritto
        )
      SELECT
        /*+ index(s) index(p) */
        DISTINCT s.thing,
        idgruppo,
        (
        CASE
          WHEN ( s.accessrights    = 255
          AND ( s.cha_tipo_diritto = 'P'
          OR s.cha_tipo_diritto    = 'A' ) )
          THEN 63
          ELSE s.accessrights
        END ) AS acr,
        NULL, --  SABRI s.ID_GRUPPO_TRASM,
        'A'
      FROM security s,
        PROFILE p
      WHERE p.system_id    = s.thing
      AND p.cha_privato    = '0'
      AND ( p.id_registro  = idregistro
      OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a,
          dpa_corr_globali b,
          GROUPS c,
          dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = 'R'
        AND b.cha_tipo_ie    = 'I'
        AND b.dta_fine      IS NULL
        AND a.num_livello    > livelloruolo
        AND d.id_registro    = idregistro
        AND b.id_uo         IN
          (SELECT system_id
          FROM dpa_corr_globali
          WHERE cha_tipo_ie            = 'I'
          AND CHA_TIPO_URP             = 'U' 
          AND dta_fine                IS NULL
          -- AND id_old                   = 0
            START WITH id_parent       = idcorrglobaleuo
            CONNECT BY PRIOR system_id = id_parent
          )
        )
      AND NOT EXISTS
        (SELECT
          /*+index (s1) */
          'x'
        FROM security s1
        WHERE s1.personorgroup = idgruppo
        AND s1.thing           = p.system_id
        );
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      raise;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
  ELSE
    BEGIN
      Insert -- hint disabled: append parallel(security,8) 
      INTO security
        (
          thing,
          personorgroup,
          accessrights,
          id_gruppo_trasm,
          cha_tipo_diritto
        )
      SELECT
        /*+ index(s) index(p) */
        DISTINCT s.thing,
        idgruppo,
        (
        CASE
          WHEN ( s.accessrights    = 255
          AND ( s.cha_tipo_diritto = 'P'
          OR s.cha_tipo_diritto    = 'A' ) )
          THEN 63
          ELSE s.accessrights
        END ) AS acr,
        NULL, --  SABRI s.ID_GRUPPO_TRASM,
        'A'
      FROM security s,
        PROFILE p
      WHERE p.system_id    = s.thing
      AND p.cha_privato    = '0'
      AND ( p.id_registro  = idregistro
      OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a,
          dpa_corr_globali b,
          GROUPS c,
          dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = 'R'
        AND b.cha_tipo_ie    = 'I'
        AND b.dta_fine      IS NULL
        AND a.num_livello   >= livelloruolo
        AND d.id_registro    = idregistro
        AND b.id_uo         IN
          (SELECT system_id
          FROM dpa_corr_globali
          WHERE cha_tipo_ie            = 'I'
          AND CHA_TIPO_URP             = 'U' 
          AND dta_fine                IS NULL
          -- AND id_old                   = 0
            START WITH id_parent       = idcorrglobaleuo
            CONNECT BY PRIOR system_id = id_parent
          )
        )
      AND NOT EXISTS
        (Select 
          /*+index (s1) */
          'x'
        FROM security s1
        WHERE s1.personorgroup = idgruppo
        AND s1.thing           = p.system_id
        );
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      raise;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
  END IF;
END;
/


