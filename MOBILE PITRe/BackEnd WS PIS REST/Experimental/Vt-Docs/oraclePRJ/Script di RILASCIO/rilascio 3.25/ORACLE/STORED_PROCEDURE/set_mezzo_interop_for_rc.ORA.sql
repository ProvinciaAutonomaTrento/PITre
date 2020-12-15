begin 
Utl_Backup_Plsql_code ('PROCEDURE','set_mezzo_interop_for_rc'); 
end;
/

create or replace
PROCEDURE set_mezzo_interop_for_rc
IS
  /******************************************************************************
  NAME:       set_mezzo_interop_for_RC
  PURPOSE:  importando il corrispondente da RC il mezzo di spedizione deve
  essre MAIL se sono settati solo i campo email.
  oppure INTEROPERABILITA SE SONO SETTATI I CAMPI VAR_COD_AOO, COM_AMM E EMAIL
  OPPURE LETTERE SE NESSUNO DEI TRA CAMPI E' SETTATO.
  C'è STATO UN BUG PER CUI SE IL CORRISPONDENTE ERA GIà STATO SCARICATO E SI MODIFICANO
  QUESTI CAMPI IN RC, IL MEZZO O NON SI AGGIORNA OPPURE SI AGGIUNGE NELLA DOCUMENTTYPES
  ANZICHè FARE UPDATE..
  QUESTA SP SISTEMA LE COSE..
  ******************************************************************************/
BEGIN
  DECLARE
    interop NUMBER;
    mail    NUMBER;
    lettera NUMBER;
    CURSOR c
    IS
      SELECT system_id,
        var_codice_amm var_cod_amm,
        var_codice_aoo var_cod_aoo,
        var_email email
      FROM dpa_corr_globali d
      WHERE cha_tipo_corr IN ('C', 'S')
      AND cha_tipo_ie      = 'E'
      ORDER BY system_id;
    rc c%ROWTYPE;
  BEGIN
    OPEN c;
    LOOP
      FETCH c INTO rc;
      EXIT
    WHEN c%NOTFOUND;
      SELECT system_id
      INTO interop
      FROM documenttypes d
      WHERE UPPER (d.description) IN ('INTEROPERABILITA');
      SELECT system_id
      INTO lettera
      FROM documenttypes d
      WHERE UPPER (d.description) IN ('LETTERA');
      SELECT system_id
      INTO mail
      FROM documenttypes d
      WHERE UPPER (d.description) IN ('MAIL');
      IF (rc.email                IS NOT NULL ) THEN
        BEGIN
          IF (rc.var_cod_aoo IS NOT NULL ) THEN
            BEGIN
              IF (rc.var_cod_amm IS NOT NULL ) THEN
                BEGIN
                  --caso interop
                  DELETE
                  FROM dpa_t_canale_corr d
                  WHERE d.id_corr_globale = rc.system_id;
                  INSERT
                  INTO dpa_t_canale_corr
                    (
                      system_id,
                      id_corr_globale,
                      id_documenttype,
                      cha_preferito
                    )
                    VALUES
                    (
                      seq.NEXTVAL,
                      rc.system_id,
                      interop,
                      '1'
                    );
                  COMMIT;
                END;
              ELSE
                --EMAIL NOT  NULL VAR_COD_AOO NOT  NULL MA VAR_COD_AMM NULL, QUINDI è MAIL
                BEGIN
                  DELETE FROM dpa_t_canale_corr d WHERE d.id_corr_globale = rc.system_id;
                  INSERT
                  INTO dpa_t_canale_corr
                    (
                      system_id,
                      id_corr_globale,
                      id_documenttype,
                      cha_preferito
                    )
                    VALUES
                    (
                      seq.NEXTVAL,
                      rc.system_id,
                      MAIL,
                      '1'
                    );
                  COMMIT;
                END;
              END IF;
            END;
          ELSE
            --EMAIL NOT NULL, MA VAR_COD_AOO NULL QUINDI è MAIL
            BEGIN
              DELETE FROM dpa_t_canale_corr d WHERE d.id_corr_globale = rc.system_id;
              INSERT
              INTO dpa_t_canale_corr
                (
                  system_id,
                  id_corr_globale,
                  id_documenttype,
                  cha_preferito
                )
                VALUES
                (
                  seq.NEXTVAL,
                  rc.system_id,
                  MAIL,
                  '1'
                );
              COMMIT;
            END;
          END IF;
        END;
      ELSE --EMAIL NULL OR EMPTY SICURAMENTE è LETTERA
        BEGIN
          DELETE FROM dpa_t_canale_corr d WHERE d.id_corr_globale = rc.system_id;
          INSERT
          INTO dpa_t_canale_corr
            (
              system_id,
              id_corr_globale,
              id_documenttype,
              cha_preferito
            )
            VALUES
            (
              seq.NEXTVAL,
              rc.system_id,
              lettera,
              '1'
            );
          COMMIT;
        END;
      END IF;
    END LOOP;
  EXCEPTION
  WHEN NO_DATA_FOUND THEN
    RAISE;
  WHEN OTHERS THEN
    CLOSE c;
    RAISE;
  END;
END;
/
