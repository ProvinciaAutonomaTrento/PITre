--------------------------------------------------------
--  DDL for Procedure SP_INSERT_DPA_SUPPORTO
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_INSERT_DPA_SUPPORTO" (
   p_copia                    NUMBER,
   p_collfisica               VARCHAR,
   p_dataultimaver            DATE,
   p_dataeliminazione         DATE,
   p_esitoultimaver           NUMBER,
   p_numerover                NUMBER,
   p_dataproxver              DATE,
   p_dataappomarca            DATE,
   p_datascadmarca            DATE,
   p_marca                    VARCHAR,
   p_idcons                   NUMBER,
   p_tiposupp                 NUMBER,
   p_stato                    CHAR,
   p_note                     VARCHAR,
   p_query                    CHAR,
   p_idsupp                   NUMBER,
   p_percverifica             NUMBER,
   p_progressivomarca         NUMBER,
   p_result             OUT   NUMBER,
   p_newid              OUT   NUMBER
)
IS
   numsuppprod     NUMBER := 0;
   numsupptotali   NUMBER := 0;
   numsupporto     NUMBER := 0;
BEGIN
   SELECT seq_supporto.NEXTVAL
     INTO numsupporto
     FROM DUAL;

   IF (p_query = 'I')
   THEN
      INSERT INTO dpa_supporto
                  (system_id, copia, data_produzione,
                   var_collocazione_fisica, data_ultima_verifica,
                   data_eliminazione, esito_ultima_verifica,
                   verifiche_effettuate, data_prox_verifica,
                   data_appo_marca, data_scadenza_marca,
                   var_marca_temporale, id_conservazione, id_tipo_supporto,
                   cha_stato, var_note, perc_verifica, num_marca
                  )
           VALUES (numsupporto, p_copia, SYSDATE,
                   p_collfisica, p_dataultimaver,
                   p_dataeliminazione, p_esitoultimaver,
                   p_numerover, p_dataproxver,
                   p_dataappomarca, p_datascadmarca,
                   p_marca, p_idcons, p_tiposupp,
                   p_stato, p_note, p_percverifica, p_progressivomarca
                  );

      p_newid := numsupporto;

      SELECT COUNT (*)
        INTO numsuppprod
        FROM dpa_supporto
       WHERE (cha_stato = 'P' OR cha_stato = 'E' OR cha_stato = 'V')
         AND id_conservazione = p_idcons;

      SELECT COUNT (*)
        INTO numsupptotali
        FROM dpa_supporto
       WHERE id_conservazione = p_idcons;

      IF (numsuppprod = numsupptotali)
      THEN
         UPDATE dpa_area_conservazione
            SET cha_stato = 'C'
          WHERE system_id = p_idcons;

         UPDATE dpa_items_conservazione
            SET cha_stato = 'C'
          WHERE id_conservazione = p_idcons;

         p_result := 1;
      ELSE
         p_result := 0;
      END IF;
   ELSE
      UPDATE dpa_supporto
         SET data_produzione = SYSDATE,
             var_collocazione_fisica = p_collfisica,
             data_prox_verifica = p_dataproxver,
             cha_stato = p_stato,
             var_note = p_note,
             data_ultima_verifica = SYSDATE,
             verifiche_effettuate = p_numerover,
             esito_ultima_verifica = p_esitoultimaver,
             perc_verifica = p_percverifica
       WHERE system_id = p_idsupp;

      p_newid := p_idsupp;

      SELECT COUNT (*)
        INTO numsuppprod
        FROM dpa_supporto
       WHERE (cha_stato = 'P' OR cha_stato = 'E' OR cha_stato = 'V')
         AND id_conservazione = (SELECT id_conservazione
                                   FROM dpa_supporto
                                  WHERE system_id = p_idsupp);

      SELECT COUNT (*)
        INTO numsupptotali
        FROM dpa_supporto
       WHERE id_conservazione = (SELECT id_conservazione
                                   FROM dpa_supporto
                                  WHERE system_id = p_idsupp);

      IF (numsuppprod = numsupptotali)
      THEN
         UPDATE dpa_area_conservazione
            SET cha_stato = 'C'
          WHERE system_id = (SELECT id_conservazione
                               FROM dpa_supporto
                              WHERE system_id = p_idsupp);

         UPDATE dpa_items_conservazione
            SET cha_stato = 'C'
          WHERE id_conservazione = (SELECT id_conservazione
                                      FROM dpa_supporto
                                     WHERE system_id = p_idsupp);

         p_result := 1;
      ELSE
         p_result := 0;
      END IF;
   END IF;
END; 

/
