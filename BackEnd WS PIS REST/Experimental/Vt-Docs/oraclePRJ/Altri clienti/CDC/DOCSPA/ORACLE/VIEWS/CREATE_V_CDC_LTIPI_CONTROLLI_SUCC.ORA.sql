Begin
EXECUTE IMMEDIATE 'CREATE OR REPLACE FORCE VIEW v_cdc_ltipi_controlli_succ (valore)
                    AS
                       SELECT valore
                         FROM dpa_associazione_valori
                        WHERE id_oggetto_custom =
                                 (SELECT DISTINCT oc.system_id
                                             FROM dpa_oggetti_custom oc,
                                                  dpa_ogg_custom_comp occ,
                                                  dpa_tipo_atto ta,
                                                  dpa_associazione_templates asst,
                                                  dpa_tipo_oggetto togg
                                            WHERE oc.system_id = occ.id_ogg_custom
                                              AND ta.system_id = occ.id_template
                                              AND asst.doc_number IS NULL
                                              AND asst.id_oggetto = oc.system_id
                                              AND togg.system_id = oc.id_tipo_oggetto
                                              AND oc.descrizione = ''Tipologia''
                                              AND UPPER (ta.var_desc_atto) =
                                              ''CONTROLLO SUCCESSIVO SCCLA''
                                              AND ta.id_amm = @id_amm)';
END;
/