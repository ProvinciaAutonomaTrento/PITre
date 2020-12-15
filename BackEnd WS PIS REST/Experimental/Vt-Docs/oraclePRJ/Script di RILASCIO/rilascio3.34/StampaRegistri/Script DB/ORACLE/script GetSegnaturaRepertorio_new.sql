create or replace 
FUNCTION          GETSEGNATURAREPERTORIO_NEW (
   docId    INT,
   idAmm    INT)
   RETURN VARCHAR
IS
   segnatura         VARCHAR (32767);

   formato_cont      VARCHAR2 (255 BYTE);
   valore_ogg_db     VARCHAR2 (100 BYTE);
   anno_rep          NUMBER;
   cod_db            VARCHAR2 (50 BYTE);
   dta_inserimento   DATE;
   idaoorf           NUMBER;
   cod_reg           VARCHAR2 (16 BYTE);
   cod_amm           VARCHAR2 (16 BYTE);
   dta_annullato     DATE;

   CURSOR cur
   IS
      SELECT oc.formato_contatore,
             t.valore_oggetto_db,
             t.anno,
             t.codice_db,
             t.dta_ins,
             t.id_aoo_rf,
             t.dta_annullamento
        FROM    dpa_associazione_templates t
             JOIN
                dpa_oggetti_custom oc
             ON     t.doc_number = docId
                AND t.id_oggetto = oc.system_id
                AND oc.repertorio = '1';
BEGIN
   segnatura := formato_cont;

   OPEN cur;

   LOOP
      FETCH cur
      INTO formato_cont,
           valore_ogg_db,
           anno_rep,
           cod_db,
           dta_inserimento,
           idaoorf,
           dta_annullato;

      IF valore_ogg_db IS NOT NULL
      THEN
         formato_cont :=
            REPLACE (UPPER (formato_cont), 'ANNO', anno_rep || '');
         formato_cont :=
            REPLACE (UPPER (formato_cont), 'CONTATORE', valore_ogg_db || '');
         formato_cont :=
            REPLACE (UPPER (formato_cont), 'COD_UO', cod_db || '');
         formato_cont :=
            REPLACE (UPPER (formato_cont),
                     'GG/MM/AAAA HH:MM',
                     TO_CHAR (dta_inserimento, 'DD/MM/YYYY HH24:MM') || '');
         formato_cont :=
            REPLACE (UPPER (formato_cont),
                     'GG/MM/AAAA',
                     TO_CHAR (dta_inserimento, 'DD/MM/YYYY') || '');

         IF idaoorf IS NOT NULL AND idaoorf != 0
         THEN
            SELECT var_codice
              INTO cod_reg
              FROM dpa_el_registri
             WHERE system_id = idaoorf;
         END IF;

         IF cod_reg IS NOT NULL
         THEN
            formato_cont :=
               REPLACE (UPPER (formato_cont), 'RF', cod_reg || '');
            formato_cont :=
               REPLACE (UPPER (formato_cont), 'AOO', cod_reg || '');
         END IF;


         IF idAmm IS NOT NULL
         THEN
            SELECT var_codice_amm
              INTO cod_amm
              FROM dpa_amministra
             WHERE system_id = idAmm;

            formato_cont :=
               REPLACE (UPPER (formato_cont), 'COD_AMM', cod_reg || '');
         END IF;
       
        
         segnatura := formato_cont;
      ELSE
         segnatura := '';
      END IF;

      EXIT;
   END LOOP;

   RETURN segnatura;
END GETSEGNATURAREPERTORIO_NEW;