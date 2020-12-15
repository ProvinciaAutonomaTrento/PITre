--------------------------------------------------------
--  DDL for Procedure INITPEC3
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INITPEC3" is
--DECLARE
  system_id              NUMBER(10,0);
  var_email_registro     VARCHAR2(128 BYTE);
  var_user_mail          VARCHAR2(128 BYTE);
  var_pwd_mail           VARCHAR2(32 BYTE);
  var_server_smtp        VARCHAR2(64 BYTE);
  num_porta_smtp         NUMBER(10,0);
  var_server_pop         VARCHAR2(64 BYTE);
  num_porta_pop          NUMBER(10,0);
  var_user_smtp          VARCHAR2(128 BYTE);
  var_pwd_smtp           VARCHAR2(128 BYTE);
  cha_smtp_ssl           VARCHAR2(1 BYTE);
  cha_pop_ssl            VARCHAR2(1 BYTE);
  cha_smtp_sta           VARCHAR2(1 BYTE);
  var_server_imap        VARCHAR2(128 BYTE);
  num_porta_imap         NUMBER(10,0);
  var_tipo_connessione   VARCHAR2(10 BYTE);
  var_inbox_imap         VARCHAR2(128 BYTE);
  var_box_mail_elaborate VARCHAR2(50 BYTE);
  var_mail_non_elaborate VARCHAR2(50 BYTE);
  cha_imap_ssl           VARCHAR2(1 BYTE);
  cha_ricevuta_pec       VARCHAR2(2 BYTE);
  var_solo_mail_pec      VARCHAR2(1 BYTE);
  cha_consulta           VARCHAR2(1 BYTE);
  cha_spedisci           VARCHAR2(1 BYTE);
  cha_notifica           VARCHAR2(1 BYTE);
  system_id_reg          NUMBER(10,0);
  id_ruolo               NUMBER(10,0);
  id_amm                 NUMBER(10,0);
  email                  VARCHAR2(128 BYTE);
  cha_rf                 VARCHAR2(1 BYTE);
  --per dpa_ass_doc_mail_interop
  id_profile NUMBER(10,0);
  var_email  VARCHAR2(128 BYTE);
  id_reg     NUMBER(10,0);
  --per dpa_mail_corr_esterni
  id_corr_esterno        NUMBER(10,0);
  var_email_corr_esterno VARCHAR2(128 BYTE);
  --cursore per popolare DPA_MAIL_REGISTRI
  CURSOR INFO_MAIL_REG
  IS
    SELECT SYSTEM_ID ,
      VAR_EMAIL_REGISTRO ,
      VAR_USER_MAIL ,
      VAR_PWD_MAIL ,
      VAR_SERVER_SMTP ,
      NUM_PORTA_SMTP ,
      VAR_SERVER_POP ,
      NUM_PORTA_POP ,
      VAR_USER_SMTP ,
      VAR_PWD_SMTP ,
      CHA_SMTP_SSL ,
      CHA_POP_SSL ,
      CHA_SMTP_STA ,
      VAR_SERVER_IMAP ,
      NUM_PORTA_IMAP ,
      VAR_TIPO_CONNESSIONE ,
      VAR_INBOX_IMAP ,
      VAR_BOX_MAIL_ELABORATE ,
      VAR_MAIL_NON_ELABORATE ,
      CHA_IMAP_SSL ,
      CHA_RICEVUTA_PEC ,
      VAR_SOLO_MAIL_PEC
    FROM DPA_EL_REGISTRI
    WHERE VAR_EMAIL_REGISTRO IS NOT NULL;
  -- cursore per popolare DPA_VIS_MAIL_REGISTRI
  CURSOR INFO_VIS_REG
  IS
    SELECT rr.id_registro ,
      rr.id_ruolo_in_uo ,
      el.id_amm ,
      el.var_email_registro ,
      el.cha_rf
    FROM DPA_L_RUOLO_REG rr,
      DPA_EL_REGISTRI el
    WHERE rr.id_registro       = el.system_id
    AND el.var_email_registro IS NOT NULL;
  --cursore per popolare DPA_ASS_DOC_MAIL_INTEROP
  CURSOR ASS_DOC_MAIL
  IS
    SELECT p.docnumber,
      r.var_email_registro,
      r.system_id
    FROM profile p,
      dpa_doc_arrivo_par d,
      dpa_corr_globali c,
      dpa_el_registri r
    WHERE p.cha_interop      = 'S'
    AND p.docnumber          = d.id_profile
    AND d.cha_tipo_mitt_dest = 'M'
    AND d.id_mitt_dest       = c.system_id
    AND c.id_registro        = r.system_id;
  --cursore per DPA_MAIL_CORR_ESTERNI
  CURSOR MAIL_CORR_ESTERNI
  IS
    SELECT system_id,
      var_email
    FROM DPA_CORR_GLOBALI
    WHERE CHA_TIPO_IE = 'E'
    AND var_email    IS NOT NULL
    AND var_email    <>' ';

BEGIN
  -- Popola la DPA_MAIL_REGISTRI
  OPEN INFO_MAIL_REG;
  LOOP
    FETCH INFO_MAIL_REG
    INTO system_id ,
      var_email_registro ,
      var_user_mail ,
      var_pwd_mail ,
      var_server_smtp ,
      num_porta_smtp ,
      var_server_pop ,
      num_porta_pop ,
      var_user_smtp ,
      var_pwd_smtp ,
      cha_smtp_ssl ,
      cha_pop_ssl ,
      cha_smtp_sta ,
      var_server_imap ,
      num_porta_imap ,
      var_tipo_connessione ,
      var_inbox_imap ,
      var_box_mail_elaborate ,
      var_mail_non_elaborate ,
      cha_imap_ssl ,
      cha_ricevuta_pec ,
      var_solo_mail_pec;
    INSERT
    INTO DPA_MAIL_REGISTRI VALUES
      (
        SEQ_DPA_MAIL_REGISTRI.nextval ,
        system_id ,
        '1' ,
        var_email_registro ,
        var_user_mail ,
        var_pwd_mail ,
        var_server_smtp ,
        cha_smtp_ssl ,
        cha_pop_ssl ,
        num_porta_smtp ,
        cha_smtp_sta ,
        var_server_pop ,
        num_porta_pop ,
        var_user_smtp ,
        var_pwd_smtp ,
        var_inbox_imap ,
        var_server_imap ,
        num_porta_imap ,
        var_tipo_connessione ,
        var_box_mail_elaborate ,
        var_mail_non_elaborate ,
        cha_imap_ssl ,
        var_solo_mail_pec ,
        cha_ricevuta_pec,
        ''
      );
    IF INFO_MAIL_REG%NOTFOUND THEN
      EXIT;
    END IF;
  END LOOP;
  CLOSE INFO_MAIL_REG;
  --Popolo la DPA_VIS_MAIL_REGISTRI
  OPEN INFO_VIS_REG;
  LOOP
    FETCH INFO_VIS_REG INTO system_id_reg ,id_ruolo ,id_amm ,email ,cha_rf;
    cha_consulta := '0';
    cha_spedisci := '0';
    cha_notifica := '0';
    --aggiorna flag notifica per fegistro/rf
    IF cha_rf = '1' THEN
      SELECT COUNT(*)
      INTO cha_notifica
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf
      WHERE fr.id_ruolo_in_uo    = id_ruolo
      AND fr.id_tipo_funz        = tf.system_id
      AND UPPER(tf.var_cod_tipo) = 'PRAU_RF';
    ELSE
      SELECT COUNT(*)
      INTO cha_notifica
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf
      WHERE fr.id_ruolo_in_uo    = id_ruolo
      AND fr.id_tipo_funz        = tf.system_id
      AND UPPER(tf.var_cod_tipo) = 'PRAU';
    END IF;
    IF cha_notifica > 1 THEN
      cha_notifica := 1;
    END IF;
    
    --aggiorna flag spedisci per fegistro/rf
    SELECT COUNT(*)
    INTO cha_spedisci
    FROM dpa_tipo_f_ruolo fr,
      dpa_tipo_funzione tf,
      dpa_funzioni f
    WHERE fr.id_ruolo_in_uo    = id_ruolo
    AND fr.id_tipo_funz        = tf.system_id
    AND  tf.system_id = f.id_tipo_funzione
    AND f.cod_funzione      = 'DO_OUT_SPEDISCI';
    IF cha_spedisci > 1 THEN
      cha_spedisci := 1;
    END IF;
    
    --aggiorna flag consulta per fegistro/rf
    IF cha_rf = '1' THEN
      SELECT COUNT(*)
      INTO cha_consulta
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf,
        dpa_funzioni f
      WHERE fr.id_ruolo_in_uo = id_ruolo
      AND fr.id_tipo_funz     = tf.system_id
      AND tf.system_id        = f.id_tipo_funzione
      AND f.cod_funzione      = 'GEST_CASELLA_IST_RF';
    ELSE
      SELECT COUNT(*)
      INTO cha_consulta
      FROM dpa_tipo_f_ruolo fr,
        dpa_tipo_funzione tf,
        dpa_funzioni f
      WHERE fr.id_ruolo_in_uo = id_ruolo
      AND fr.id_tipo_funz     = tf.system_id
      AND tf.system_id        = f.id_tipo_funzione
      AND f.cod_funzione      = 'GEST_CASELLA_IST';
    END IF;
    IF cha_consulta > 1 THEN
      cha_consulta := 1;
    END IF;
    INSERT
    INTO DPA_VIS_MAIL_REGISTRI VALUES
      (
        SEQ_DPA_VIS_MAIL_REGISTRI.nextval ,
        system_id_reg ,
        id_ruolo ,
        email ,
        cha_consulta ,
        cha_spedisci ,
        cha_notifica
      );
    IF INFO_VIS_REG%NOTFOUND THEN
      EXIT;
    END IF;
  END LOOP;
  CLOSE INFO_VIS_REG;
  OPEN ASS_DOC_MAIL;
  LOOP
    FETCH ASS_DOC_MAIL INTO id_profile, var_email, id_reg;
    INSERT
    INTO DPA_ASS_DOC_MAIL_INTEROP VALUES
      (
        SEQ_DPA_ASS_DOC_MAIL_INTEROP.nextval ,
        id_profile ,
        id_reg ,
        var_email
      );
    IF ASS_DOC_MAIL%NOTFOUND THEN
      EXIT;
    END IF;
  END LOOP;
  CLOSE ASS_DOC_MAIL;
  OPEN MAIL_CORR_ESTERNI;
  LOOP
    FETCH MAIL_CORR_ESTERNI INTO id_corr_esterno, var_email_corr_esterno;
    INSERT
    INTO DPA_MAIL_CORR_ESTERNI VALUES
      (
        SEQ_DPA_MAIL_CORR_ESTERNI.nextval ,
        id_corr_esterno ,
        var_email_corr_esterno,
        '1',
        ''
      );
    IF MAIL_CORR_ESTERNI%NOTFOUND THEN
      EXIT;
    END IF;
  END LOOP;
  CLOSE MAIL_CORR_ESTERNI;
  Commit;
  
    EXCEPTION when OTHERS then RAISE; 
END;

/
