

-- VELTRI - inserimento Nuova microfunzione per abilitare il tab archivio in ricerca fascicoli


/* Formatted on 2012/07/16 10:38 (Formatter Plus v4.8.8) */
BEGIN
   utl_insert_chiave_microfunz
      ('DO_TAB_ARCHIVIO',
       'Abilita il tab archivio nelle ricerche fascicoli',
       NULL,
       'Y',
       NULL,
       '3.29',
       NULL
      );
END;
/

/* Formatted on 2012/07/16 10:38 (Formatter Plus v4.8.8) */
BEGIN
   utl_insert_chiave_microfunz
      ('CERCA_DOC_ADV',
       'Ricerca documenti avanzata selezionata di default',
       NULL,
       'Y',
       NULL,
       '3.29',
       NULL
      );
END;
/

-- Andrea De Marco - MEV per Ospedale Maggiore
BEGIN
                               utl_insert_chiave_microfunz
                               ('FASC_RICLASS',
                               'Abilita il pulsante riclassifica nei dettagli del fascicolo',
                               NULL,
                               'Y',
                               NULL,
                               '3.29',
                               NULL
                               );
                END;


