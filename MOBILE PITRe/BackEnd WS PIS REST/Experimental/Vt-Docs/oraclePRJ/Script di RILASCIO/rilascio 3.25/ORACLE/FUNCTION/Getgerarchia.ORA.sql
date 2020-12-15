begin 
Utl_Backup_Plsql_code ('FUNCTION','Getgerarchia'); 
end;
/

create or replace
FUNCTION Getgerarchia(
      p_id_amm   IN VARCHAR,
      p_cod      IN VARCHAR,
      p_tipo_ie  IN VARCHAR,
      P_Id_Ruolo In Varchar 
      --,P_Codes Out Varchar
    )
    RETURN VARCHAR2
  AS
    P_Codes     VARCHAR(4000);
    V_C_Type    VARCHAR(2);
    v_p_cod     VARCHAR(64);
    v_system_id INT;
    v_id_parent INT;
    v_id_uo     INT;
    V_Id_Utente INT;
    Mydebugline INT;
    CharU       CHAR(1) := 'U';
    Charr       CHAR(1) := 'R';
    CharP       CHAR(1) := 'P';
    charF       CHAR(1) := 'F';
  BEGIN
    v_p_cod     := p_cod;
    mydebugline := 24;
    SELECT Id_Parent,
      System_Id,
      Id_Uo,
      Id_People,
      Cha_Tipo_Urp
    INTO v_id_parent,
      v_system_id,
      v_id_uo,
      v_id_utente,
      v_c_type
    FROM dpa_corr_globali
    WHERE var_cod_rubrica = v_p_cod
    AND cha_tipo_ie       = p_tipo_ie
    AND id_amm            = p_id_amm
    AND dta_fine         IS NULL;
    WHILE (1              > 0)
    LOOP
      IF v_c_type        = CharU THEN
        IF (v_id_parent IS NULL OR v_id_parent = 0) THEN
          EXIT;
        END IF;
        mydebugline := 43;
        SELECT Var_Cod_Rubrica,
          System_Id
        INTO V_P_Cod,
          V_System_Id
        FROM dpa_corr_globali
        WHERE System_Id  = V_Id_Parent
        AND Id_Amm       = P_Id_Amm
        AND dta_fine    IS NULL
        AND cha_tipo_urp = CharU;
      END IF;
      IF v_c_type    = CharR THEN
        IF (v_id_uo IS NULL OR v_id_uo = 0) THEN
          EXIT;
        END IF;
        mydebugline := 51;
        SELECT Var_Cod_Rubrica,
          System_Id
        INTO V_P_Cod,
          V_System_Id
        FROM dpa_corr_globali
        WHERE System_Id  = V_Id_Uo
        AND Id_Amm       = P_Id_Amm
        AND dta_fine    IS NULL
        AND cha_tipo_urp = CharU;
      END IF;
      IF v_c_type = CharP THEN
        SELECT Var_Cod_Rubrica
        INTO V_P_Cod
        FROM Dpa_Corr_Globali
        WHERE id_gruppo = p_id_Ruolo
        AND id_amm      = p_id_amm
        AND dta_fine   IS NULL;
      END IF;
      IF v_p_cod IS NULL THEN
        EXIT;
      END IF;
      mydebugline := 62;
      SELECT Id_Parent,
        System_Id,
        Id_Uo,
        Cha_Tipo_Urp
      INTO V_Id_Parent,
        V_System_Id,
        V_Id_Uo,
        V_C_Type
      FROM Dpa_Corr_Globali
      WHERE Var_Cod_Rubrica = V_P_Cod
      AND Id_Amm            = P_Id_Amm
      AND dta_fine         IS NULL
      AND cha_tipo_urp NOT IN (CharF,CharP);
      p_codes              := v_p_cod || ':' || p_codes;
    END LOOP;
    p_codes := p_codes || p_cod;
    RETURN P_Codes ;
  EXCEPTION
  WHEN OTHERS THEN
    RETURN 'Errore - '||V_P_Cod ||';' || V_C_Type ||';' ||V_System_Id ||' ruolo:'||P_Id_Ruolo ||' myline: '||Mydebugline ;
  END;
/
