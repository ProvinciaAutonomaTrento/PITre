--------------------------------------------------------
--  DDL for Procedure CREATE_NEW_NODO_TITOLARIO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."CREATE_NEW_NODO_TITOLARIO" (
   p_idamm                           NUMBER,
   p_livellonodo                     NUMBER,
   p_description                     VARCHAR2,
   p_codicenodo                      VARCHAR2,
   p_idregistronodo                  NUMBER,
   p_idparent                        NUMBER,
   p_varcodliv1                      VARCHAR2,
   p_mesiconservazione               NUMBER,
   p_charw                           CHAR,
   p_idtipofascicolo                 NUMBER,
   p_bloccafascicolo                 VARCHAR2,
   p_sysidtitolario                  NUMBER,
   p_notenodo                        VARCHAR,
   p_bloccafigli                     VARCHAR2,
   p_contatoreattivo                 VARCHAR2,
   p_numprottit                      NUMBER,
   p_consenticlassificazione         VARCHAR,
   p_bloccaclass                     VARCHAR,
   p_idtitolario               OUT   NUMBER
)
IS
BEGIN
   DBMS_OUTPUT.put_line ('inizio stored');

   DECLARE
      CURSOR currreg
      IS
         SELECT system_id
           FROM dpa_el_registri
          WHERE id_amm = p_idamm AND cha_rf = '0';

      secproj         NUMBER;
      secfasc         NUMBER;
      secroot         NUMBER;
      varchiavetit    VARCHAR2 (64);
      varchiavefasc   VARCHAR2 (64);
      varchiaveroot   VARCHAR2 (64);
      consenticlass   VARCHAR2 (1 BYTE);
   BEGIN
      p_idtitolario := 0;

      SELECT seq.NEXTVAL
        INTO secproj
        FROM DUAL;

      p_idtitolario := secproj;
      DBMS_OUTPUT.put_line ('controllo id registro');

      IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
      THEN
         varchiavetit :=
                  p_idamm || '_' || p_codicenodo || '_' || p_idparent || '_0';
      ELSE
         varchiavetit :=
                 p_codicenodo || '_' || p_idparent || '_' || p_idregistronodo;
      END IF;

      DBMS_OUTPUT.put_line ('controllo blocca class');

      IF (p_bloccaclass = '1')
      THEN
         UPDATE project
            SET cha_consenti_class = '0',
                cha_rw = 'R'
          WHERE system_id = p_idparent;
      END IF;

      BEGIN
         DBMS_OUTPUT.put_line ('inizio primo inserimento');

         IF (p_consenticlassificazione IS NULL)
         THEN
            consenticlass := '0';
         ELSE
            consenticlass := p_consenticlassificazione;
         END IF;

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secproj, p_description, 'Y', 'T',
                      p_codicenodo, p_idamm, p_idregistronodo, p_livellonodo,
                      NULL, p_idparent, p_varcodliv1,
                      SYSDATE, NULL, NULL, p_charw,
                      p_mesiconservazione, varchiavetit, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine primo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN raise;
           -- p_idtitolario := 0;
           -- RETURN;
      END;

      BEGIN
         SELECT seq.NEXTVAL
           INTO secfasc
           FROM DUAL;

         IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
         THEN
            varchiavefasc := p_codicenodo || '_' || p_idtitolario || '_0';
         ELSE
            varchiavefasc :=
               p_codicenodo || '_' || p_idtitolario || '_'
               || p_idregistronodo;
         END IF;

         DBMS_OUTPUT.put_line ('inizio secondo inserimento');

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secfasc, p_description, 'Y', 'F',
                      p_codicenodo, p_idamm, p_idregistronodo, NULL,
                      'G', p_idtitolario, NULL,
                      SYSDATE, 'A', NULL, p_charw,
                      p_mesiconservazione, varchiavefasc, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine secondo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN
            p_idtitolario := 0;
            RETURN;
      END;

      BEGIN
         IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
         THEN
            varchiaveroot := p_codicenodo || '_' || secfasc || '_0';
         ELSE
            varchiaveroot :=
                    p_codicenodo || '_' || secfasc || '_' || p_idregistronodo;
         END IF;

         SELECT seq.NEXTVAL
           INTO secroot
           FROM DUAL;

         DBMS_OUTPUT.put_line ('inizio terzo inserimento');

         INSERT INTO project
                     (system_id, description, iconized, cha_tipo_proj,
                      var_codice, id_amm, id_registro, num_livello,
                      cha_tipo_fascicolo, id_parent, var_cod_liv1,
                      dta_apertura, cha_stato, id_fascicolo, cha_rw,
                      num_mesi_conservazione, var_chiave_fasc, id_tipo_fasc,
                      cha_blocca_fasc, id_titolario, dta_creazione,
                      var_note, cha_blocca_figli, cha_conta_prot_tit,
                      num_prot_tit, cha_consenti_class
                     )
              VALUES (secroot, 'Root Folder', 'Y', 'C',
                      NULL, p_idamm, NULL, NULL,
                      NULL, secfasc, NULL,
                      SYSDATE, NULL, secfasc, p_charw,
                      p_mesiconservazione, varchiaveroot, p_idtipofascicolo,
                      p_bloccafascicolo, p_sysidtitolario, SYSDATE,
                      p_notenodo, p_bloccafigli, p_contatoreattivo,
                      p_numprottit, consenticlass
                     );

         DBMS_OUTPUT.put_line ('fine terzo inserimento');
      EXCEPTION
         WHEN OTHERS
         THEN
            p_idtitolario := 0;
            RETURN;
      END;

-- SE IL NODO HA REGISTRO NULL ALLORA DEVONO ESSERE CREATI TANTI RECORD NELLA
-- DPA_REG_FASC QUANTI SONO I REGISTRI INTERNI ALL'AMMINISTRAZIONE
      IF (p_idregistronodo IS NULL OR p_idregistronodo = '')
      THEN
         FOR currentreg IN currreg
         LOOP
            BEGIN
               DBMS_OUTPUT.put_line ('inizio primo inserimento dpa_reg_fasc');

               INSERT INTO dpa_reg_fasc
                           (system_id, id_titolario, num_rif,
                            id_registro
                           )
                    VALUES (seq.NEXTVAL, p_idtitolario, 1,
                            currentreg.system_id
                           );

               DBMS_OUTPUT.put_line ('fine primo inserimento dpa_reg_fasc');
            EXCEPTION
               WHEN OTHERS
               THEN
                  p_idtitolario := 0;
                  RETURN;
            END;
         END LOOP;

-- inoltre bisogna inserire un record nella dpa_reg_Fasc relativo al registro null
-- per tutte quelle amministrazioni che non hanno abilitata la funzione di fascicolazione
--multi registro
         DBMS_OUTPUT.put_line ('inizio secondo inserimento dpa_reg_fasc');

         INSERT INTO dpa_reg_fasc
                     (system_id, id_titolario, num_rif,
                      id_registro
                     )
              VALUES (seq.NEXTVAL, p_idtitolario, 1,
                      NULL  -- SE IL NODO ? COMUNE A TUTTI p_idRegistro = NULL
                     );

         DBMS_OUTPUT.put_line ('fine secondo inserimento dpa_reg_fasc');
      ELSE                 -- il nodo creato ?  associato  a uno solo registro
         BEGIN
            DBMS_OUTPUT.put_line ('inizio terzo inserimento dpa_reg_fasc');

            INSERT INTO dpa_reg_fasc
                        (system_id, id_titolario, num_rif,
                         id_registro
                        )
                 VALUES (seq.NEXTVAL, p_idtitolario, 1,
                         p_idregistronodo     -- REGISTRO SU CUI  CREO IL NODO
                        );

            DBMS_OUTPUT.put_line ('fine terzo inserimento dpa_reg_fasc');
         EXCEPTION
            WHEN OTHERS
            THEN
               p_idtitolario := 0;
               RETURN;
         END;
      END IF;
   END;
END create_new_nodo_titolario; 

/
