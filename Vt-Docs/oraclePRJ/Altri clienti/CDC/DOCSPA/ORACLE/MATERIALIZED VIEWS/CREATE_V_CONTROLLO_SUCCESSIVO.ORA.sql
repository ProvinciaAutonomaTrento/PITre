BEGIN
EXECUTE IMMEDIATE 'CREATE MATERIALIZED VIEW V_CDC_CONTROLLO_SUCCESSIVO
                        AS 
                        (SELECT dpa_tipo_atto.id_amm, p.system_id, p.var_prof_oggetto,
                                p.dta_proto AS dta_protocollo, p.var_segnatura, p.dta_annulla,
                                v.dta_arrivo, s.personorgroup,
                                NVL (getvaloreoggettodb (dpa_tipo_atto.id_amm,
                                                         p.docnumber,
                                                         dpa_tipo_atto.var_desc_atto,
                                                         ''Tipologia''
                                                        ),
                                     ''Z-n.c.''
                                    ) AS tipologia,
                                getvaloreoggettodb (dpa_tipo_atto.id_amm,
                                                    p.docnumber,
                                                    dpa_tipo_atto.var_desc_atto,
                                                    ''Num. Decreto''
                                                   ) AS num_decr,
                                getvaloreoggettodb_tipodata
                                                           (dpa_tipo_atto.id_amm,
                                                            p.docnumber,
                                                            dpa_tipo_atto.var_desc_atto,
                                                            ''Data decreto''
                                                           ) AS data_decr,
                                getvaloreoggettodb_tipodata
                                          (dpa_tipo_atto.id_amm,
                                           p.docnumber,
                                           dpa_tipo_atto.var_desc_atto,
                                           ''Data ritorno primo rilievo''
                                          ) AS data_ritorno_primo_rilievo,
                                getvaloreoggettodb_tipodata
                                              (dpa_tipo_atto.id_amm,
                                               p.docnumber,
                                               dpa_tipo_atto.var_desc_atto,
                                               ''Data invio deferimento''
                                              ) AS data_invio_deferimento,
                                getvaloreoggettodb (dpa_tipo_atto.id_amm,
                                                    p.docnumber,
                                                    dpa_tipo_atto.var_desc_atto,
                                                    ''Primo Revisore''
                                                   ) AS primo_revisore,
                                getvaloreoggettodb (dpa_tipo_atto.id_amm,
                                                    p.docnumber,
                                                    dpa_tipo_atto.var_desc_atto,
                                                    ''Secondo Revisore''
                                                   ) AS secondo_revisore,
                                getvaloreoggettodb
                                               (dpa_tipo_atto.id_amm,
                                                p.docnumber,
                                                dpa_tipo_atto.var_desc_atto,
                                                ''Magistrato Istruttore''
                                               ) AS magistrato_istruttore,
                                getvaloreoggettodb (dpa_tipo_atto.id_amm,
                                                    p.docnumber,
                                                    dpa_tipo_atto.var_desc_atto,
                                                   ''Stato''
                                                   ) AS stato,
                                getvaloreoggettodb_tipodata
                                                  (dpa_tipo_atto.id_amm,
                                                   p.docnumber,
                                                   dpa_tipo_atto.var_desc_atto,
                                                   ''Data primo rilievo''
                                                  ) AS data_primo_rilievo,
                                getvaloreoggettodb_tipodata
                                                (dpa_tipo_atto.id_amm,
                                                 p.docnumber,
                                                 dpa_tipo_atto.var_desc_atto,
                                                 ''Data secondo rilievo''
                                                ) AS data_secondo_rilievo,
                                getvaloreoggettodb_tipodata
                                                   (dpa_tipo_atto.id_amm,
                                                    p.docnumber,
                                                    dpa_tipo_atto.var_desc_atto,
                                                    ''Data osservazione''
                                                   ) AS data_osservazione,
                                getvaloreoggettodb_tipodata
                                   (dpa_tipo_atto.id_amm,
                                    p.docnumber,
                                    dpa_tipo_atto.var_desc_atto,
                                    ''Data restituz. Amministrazione''
                                   ) AS data_restituz_amministrazione,
                                getvaloreoggettodb_tipodata
                                                  (dpa_tipo_atto.id_amm,
                                                   p.docnumber,
                                                   dpa_tipo_atto.var_desc_atto,
                                                   ''Data Registrazione''
                                                  ) AS data_registrazione,
                                getvaloreoggettodb_tipodata
                                                       (dpa_tipo_atto.id_amm,
                                                        p.docnumber,
                                                        dpa_tipo_atto.var_desc_atto,
                                                        ''Data delibera''
                                                       ) AS data_delibera,
                                getvaloreoggettodb_tipodata
                                             (dpa_tipo_atto.id_amm,
                                              p.docnumber,
                                              dpa_tipo_atto.var_desc_atto,
                                              ''Data scadenza controllo''
                                             ) AS data_scadenza_controllo
                           FROM PROFILE p, dpa_tipo_atto, VERSIONS v, security s
                          WHERE p.id_tipo_atto = dpa_tipo_atto.system_id
                            AND p.docnumber = v.docnumber
                            AND dta_annulla IS NULL
                            AND s.thing = p.system_id
                            AND s.accessrights = 255
                            AND s.cha_tipo_diritto = ''P''
                            AND v.VERSION = 1
                            AND dpa_tipo_atto.var_desc_atto = ''Controllo Successivo SCCLA'')';
                            
END;