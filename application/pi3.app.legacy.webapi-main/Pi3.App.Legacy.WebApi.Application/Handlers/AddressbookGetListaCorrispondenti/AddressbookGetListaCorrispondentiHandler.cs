// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AddressbookGetListaCorrispondentiRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetListaCorrispondenti;
using System.Diagnostics;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetListaCorrispondenti
{
    public class AddressbookGetListaCorrispondentiHandler : IRequestHandler<AddressbookGetListaCorrispondentiRequest, AddressbookGetListaCorrispondentiResult>
    {
        #region Public Members

        public AddressbookGetListaCorrispondentiHandler(
            ILogger<AddressbookGetListaCorrispondentiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<AddressbookGetListaCorrispondentiResult> Handle(AddressbookGetListaCorrispondentiRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.utente.Corrispondente> output = new();
            try
            {
                output = await this.GetListaCorr(request.queryCorrispondente);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output.ToArray());
        }


        private async Task<List<DocsPaVO.utente.Corrispondente>> GetListaCorr(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente)
        {
            List<DocsPaVO.utente.Corrispondente> listaCorr = new();

            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {
                listaCorr = await this.ListaCorrEstSciolti(queryCorrispondente);
            }
            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.INTERNO)
            {
                listaCorr = await this.ListaCorrispondentiInt(queryCorrispondente);
            }
            if (queryCorrispondente.tipoUtente == DocsPaVO.addressbook.TipoUtente.GLOBALE)
            {
                var listaCorrint = await this.ListaCorrispondentiInt(queryCorrispondente);
                if ((queryCorrispondente.codiceRubrica != null && queryCorrispondente.codiceRubrica != "") && listaCorrint.Count > 0)
                {
                    listaCorr = listaCorrint;
                }
                else
                {
                    listaCorr = await this.ListaCorrEstSciolti(queryCorrispondente);
                    for (int i = 0; i < listaCorrint.Count; i++)
                    {
                        listaCorr.Add(listaCorrint[i]);
                    }
                }
            }
            return listaCorr;
        }

        private async Task<List<DocsPaVO.utente.Corrispondente>> ListaCorrEstSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            var listaCorr = await this.ListaCorrispondentiEst(qco);
            if (listaCorr == null)
            {
                return listaCorr;
            }
            var listaSciolti = await this.ListaUtSciolti(qco);
            if (listaSciolti == null)
            {
                listaCorr = null;
                return listaCorr;
            }
            List<DocsPaVO.utente.Corrispondente> temp = new();

            for (int i = 0; i < listaSciolti.Count; i++)
            {
                DocsPaVO.utente.Utente ut = (DocsPaVO.utente.Utente)listaSciolti[i];
                bool isInEst = false;
                for (int k = 0; k < listaCorr.Count; k++)
                {
                    if (ut.systemId.Equals(((DocsPaVO.utente.Corrispondente)listaCorr[k]).systemId)) isInEst = true;
                }
                if (!isInEst) temp.Add(ut);
            }
            for (int j = 0; j < temp.Count; j++)
            {
                listaCorr.Add(temp[j]);
            }
            return listaCorr;

        }
        private async Task<List<DocsPaVO.utente.Corrispondente>> ListaUtSciolti(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            List<DocsPaVO.utente.Corrispondente> listaCorr = new();
            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);

            if (objQueryCorrispondente.isUODefined() || objQueryCorrispondente.isRuoloDefined())
            {
                return listaCorr;
            }
            var generalPredicate = this.GetRegPredicate(qco);
            generalPredicate = generalPredicate.And(a => (a.ID_AMM == null || a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()));

            if (objQueryCorrispondente.codiceRubrica != null)
            {

                if (objQueryCorrispondente.fineValidita)
                    generalPredicate = generalPredicate.And(a => !a.DTA_FINE.HasValue);

                generalPredicate = generalPredicate.And(a =>
                a.VAR_COD_RUBRICA != null && a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                a.CHA_TIPO_IE != null && a.CHA_TIPO_CORR != null && a.CHA_TIPO_IE.Equals("E") && a.CHA_TIPO_CORR.Equals("S")
                );
                var parent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(generalPredicate).OrderBy(a => a.VAR_DESC_CORR).FirstOrDefaultAsync();
                if (parent == null)
                {
                    return listaCorr;
                }

                if (objQueryCorrispondente.getChildren == false)
                {
                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("P"))
                    {
                        generalPredicate = PredicateBuilder.New<CorrGlobaliEntity>();
                        generalPredicate = generalPredicate.And(a =>
                        a.VAR_COD_RUBRICA != null && a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                        a.CHA_TIPO_IE != null && a.CHA_TIPO_CORR != null && a.CHA_TIPO_IE.Equals("E") && a.CHA_TIPO_CORR.Equals("S")
                        );
                    }
                }

            }
            else
            {
                generalPredicate = this.GetRegPredicate(qco);
                if (objQueryCorrispondente.nomeUtente != null)
                {
                    generalPredicate = generalPredicate.And(a =>
                        a.VAR_NOME != null && a.VAR_NOME.ToUpper().Contains(objQueryCorrispondente.nomeUtente.ToUpper()) &&
                        a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("P") &&
                        a.CHA_TIPO_IE != null && a.CHA_TIPO_CORR != null && a.CHA_TIPO_IE.Equals("E") && a.CHA_TIPO_CORR.Equals("S")
                        );

                }
                else
                {
                    generalPredicate = generalPredicate.And(a =>
                        a.VAR_COGNOME != null && a.VAR_COGNOME.ToUpper().Contains(objQueryCorrispondente.cognomeUtente.ToUpper()) &&
                        a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("P") &&
                        a.CHA_TIPO_IE != null && a.CHA_TIPO_CORR != null && a.CHA_TIPO_IE.Equals("E") && a.CHA_TIPO_CORR.Equals("S")
                        );
                }
            }

            if (objQueryCorrispondente.fineValidita)
                generalPredicate = generalPredicate.And(a => !a.DTA_FINE.HasValue);

            this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(generalPredicate).Distinct().ForEach(p =>
            {
                if (p.CHA_TIPO_URP.ToString().Equals("P"))
                {
                    DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                    corrispondenteUtente.systemId = p.SYSTEM_ID.ToString();
                    corrispondenteUtente.descrizione = p.VAR_COGNOME + " " + p.VAR_NOME;
                    corrispondenteUtente.codiceCorrispondente = p.VAR_CODICE;
                    corrispondenteUtente.codiceRubrica = p.VAR_COD_RUBRICA;
                    corrispondenteUtente.dettagli = p.CHA_DETTAGLI != null ? p.CHA_DETTAGLI.Equals("1") : false;
                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                    sp.serverSMTP = p.VAR_SMTP;
                    sp.portaSMTP = p.NUM_PORTA_SMTP != null ? p.NUM_PORTA_SMTP.ToString() : null;
                    corrispondenteUtente.serverPosta = sp;
                    corrispondenteUtente.tipoIE = "E";
                    if (p.ID_REGISTRO != null)
                    {
                        corrispondenteUtente.idRegistro = p.ID_REGISTRO.ToString();
                    }
                    corrispondenteUtente.email = p.VAR_EMAIL;
                    corrispondenteUtente.codiceAOO = p.VAR_CODICE_AOO;
                    corrispondenteUtente.codiceAmm = p.VAR_CODICE_AMM;

                    listaCorr.Add(corrispondenteUtente);
                }

            });
            return listaCorr;

        }
        private async Task<List<DocsPaVO.utente.Corrispondente>> ListaCorrispondentiEst(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            List<DocsPaVO.utente.Corrispondente> corrs = new();
            var generalPredicate = this.GetRegPredicate(qco);

            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);

            bool selectedAVarCog = false;
            bool selectedEVarCog = false;
            IQueryable<InfoCorr>? bQuery1 = null;
            IQueryable<CorrConTipoRuolo>? bQuery2 = null;
            IQueryable<CorrGlobaliEntity> bQuery3 = null;
            int querySelector = 0;
            List<string> listTipoCorr = new() { "C", "S" };
            generalPredicate = generalPredicate.And(a => (a.ID_AMM == null || a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()));

            if (objQueryCorrispondente.codiceRubrica != null)
            {

                if (objQueryCorrispondente.fineValidita)
                    generalPredicate = generalPredicate.And(a => !a.DTA_FINE.HasValue);

                generalPredicate = generalPredicate.And(a =>
                a.VAR_COD_RUBRICA != null && a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                a.CHA_TIPO_IE != null && a.CHA_TIPO_CORR != null && a.CHA_TIPO_IE.Equals("E") && listTipoCorr.Contains(a.CHA_TIPO_CORR)
                );

                var parent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(generalPredicate).OrderBy(a => a.VAR_DESC_CORR).FirstOrDefaultAsync();

                if (parent == null)
                {
                    return new();
                }
                if (objQueryCorrispondente.getChildren == false)
                {
                    var pepPred = PredicateBuilder.New<InfoCorr>();
                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("P"))
                    {
                        selectedEVarCog = true;
                        pepPred = pepPred.And(a =>
                        a.A.VAR_COD_RUBRICA != null && a.A.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) && a.A.CHA_TIPO_IE != null && a.A.CHA_TIPO_IE.Equals("E") &&
                        a.A.CHA_TIPO_CORR != null && listTipoCorr.Contains(a.A.CHA_TIPO_CORR) && (a.A.ID_AMM == null || a.A.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong())
                        );

                        if (objQueryCorrispondente.fineValidita)
                            pepPred = pepPred.And(a => !a.B.DTA_FINE.HasValue);

                        bQuery1 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   from b in this._dbContext.PeopleGroupEntities.AsNoTracking()
                                   from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   from d in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                   from e in this._dbContext.PeopleEntities.AsNoTracking()
                                   where (b.PEOPLE_SYSTEM_ID == a.ID_PEOPLE) &&
                                   (c.ID_GRUPPO == b.GROUPS_SYSTEM_ID) &&
                                   (d.SYSTEM_ID == c.ID_TIPO_RUOLO) &&
                                   (e.SYSTEM_ID == a.ID_PEOPLE)
                                   select new InfoCorr()
                                   {
                                       A = a,
                                       B = b,
                                       C = c,
                                       D = d,
                                       E = e
                                   });
                        querySelector = 1;
                    }
                    else
                    {
                        querySelector = 3;
                        bQuery3 = this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(generalPredicate).OrderBy(a => a.VAR_DESC_CORR);
                    }
                }
                else
                {
                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("P"))
                    {
                        return new();
                    }

                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("U"))
                    {
                        selectedAVarCog = true;
                        bQuery3 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   where (a.ID_UO == parent.SYSTEM_ID || a.ID_PARENT == parent.SYSTEM_ID) && a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("E")
                                   select a
                                   );


                        querySelector = 3;

                    }


                }
            }
            else
            {
                var pepPred = PredicateBuilder.New<CorrGlobaliEntity>();
                var codIfNoDescPred = PredicateBuilder.New<CorrGlobaliEntity>();
                var uoListPred = PredicateBuilder.New<CorrGlobaliEntity>();

                if (objQueryCorrispondente.fineValidita)
                    pepPred = pepPred.And(a => !a.DTA_FINE.HasValue);

                bQuery3 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking() select a);
                querySelector = 3;

                if (objQueryCorrispondente.codiceUO != null)
                {
                    codIfNoDescPred = pepPred.And(a => a.VAR_CODICE != null && a.VAR_CODICE.ToUpper().Equals(objQueryCorrispondente.codiceUO.ToUpper()) &&
                        a.CHA_TIPO_URP != null &&
                        a.CHA_TIPO_IE != null &&
                        a.CHA_TIPO_CORR != null &&
                        a.CHA_TIPO_URP.Equals("U") &&
                        a.CHA_TIPO_IE.Equals("E") &&
                        a.CHA_TIPO_CORR.Equals("S"));
                }

                if (objQueryCorrispondente.descrizioneUO != null)
                {
                    var uoList = objQueryCorrispondente.descrizioneUO.Split(';').ToList();

                    codIfNoDescPred = pepPred.And(a =>
                        a.CHA_TIPO_URP != null &&
                        a.CHA_TIPO_IE != null &&
                        a.CHA_TIPO_CORR != null &&
                        a.CHA_TIPO_URP.Equals("U") &&
                        a.CHA_TIPO_IE.Equals("E") &&
                        a.CHA_TIPO_CORR.Equals("S"));

                    uoList.ForEach(uo =>
                    {
                        uoListPred = uoListPred.Or(a => a.VAR_DESC_CORR != null && uo.ToUpper().Contains(a.VAR_DESC_CORR.ToUpper()));
                    });

                    if (uoList.Count > 0)
                    {
                        codIfNoDescPred = codIfNoDescPred.And(uoListPred);
                    }
                }

                pepPred = pepPred.And(codIfNoDescPred);
                bQuery3 = bQuery3.Where(pepPred);



                if (objQueryCorrispondente.isRuoloDefined())
                {
                    selectedAVarCog = true;
                    bQuery2 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                               where a.ID_TIPO_RUOLO == b.SYSTEM_ID && a.VAR_DESC_CORR != null &&
                               a.VAR_DESC_CORR.ToUpper().Contains(objQueryCorrispondente.descrizioneRuolo.ToUpper()) &&
                               a.CHA_TIPO_URP != null &&
                               a.CHA_TIPO_URP.Equals("R") &&
                               a.CHA_TIPO_IE != null &&
                               a.CHA_TIPO_IE.Equals("E") &&
                               a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()
                               select new CorrConTipoRuolo()
                               {
                                   A = a,
                                   B = b
                               });

                    if (objQueryCorrispondente.fineValidita)
                    {
                        bQuery2 = bQuery2.Where(a => !a.A.DTA_FINE.HasValue);
                    }

                    querySelector = 2;
                }
                if (objQueryCorrispondente.isUtenteDefined())
                {
                    var pred = PredicateBuilder.New<InfoCorr>();
                    var regs = objQueryCorrispondente.idRegistri.ToList();

                    pred = pred.And(a => (a.A.ID_AMM == null || (a.A.ID_REGISTRO == null && a.A.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong())) || ((a.A.ID_REGISTRO != null && regs.Contains(a.A.ID_REGISTRO.ToString()))));


                    selectedEVarCog = true;
                    bQuery1 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from b in this._dbContext.PeopleGroupEntities.AsNoTracking()
                               from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                               from d in this._dbContext.TipoRuoloEntities.AsNoTracking()
                               from e in this._dbContext.PeopleEntities.AsNoTracking()
                               where (b.PEOPLE_SYSTEM_ID == a.ID_PEOPLE) &&
                               (c.ID_GRUPPO == b.GROUPS_SYSTEM_ID) &&
                               (d.SYSTEM_ID == c.ID_TIPO_RUOLO) &&
                               (e.SYSTEM_ID == a.ID_PEOPLE) &&
                               a.CHA_TIPO_IE != null &&
                               a.CHA_TIPO_IE.Equals("E")
                               select new InfoCorr()
                               {
                                   A = a,
                                   B = b,
                                   C = c,
                                   D = d,
                                   E = e
                               });

                    querySelector = 1;

                    if (objQueryCorrispondente.fineValidita)
                    {
                        pred = pred.And(a => !a.B.DTA_FINE.HasValue);
                    }

                    if (objQueryCorrispondente.nomeUtente != null)
                    {
                        pred = pred.And(a => a.E.VAR_NOME != null && a.E.VAR_NOME.ToUpper().StartsWith(objQueryCorrispondente.nomeUtente.ToUpper()) &&
                        a.A.CHA_TIPO_URP != null &&
                        a.A.CHA_TIPO_URP.Equals("P") &&
                        a.A.CHA_TIPO_IE != null &&
                        a.A.CHA_TIPO_IE.Equals("E") &&
                        a.A.CHA_TIPO_CORR != null &&
                        a.A.CHA_TIPO_CORR.Equals("S")
                        );
                    }
                    else
                    {
                        pred = pred.And(a => a.E.VAR_COGNOME != null && a.E.VAR_NOME.ToUpper().StartsWith(objQueryCorrispondente.cognomeUtente.ToUpper().Replace("'", "''")) &&
                        a.A.CHA_TIPO_URP != null &&
                        a.A.CHA_TIPO_URP.Equals("P") &&
                        a.A.CHA_TIPO_IE != null &&
                        a.A.CHA_TIPO_IE.Equals("E") &&
                        a.A.CHA_TIPO_CORR != null &&
                        a.A.CHA_TIPO_CORR.Equals("S")
                        );
                    }

                    if (objQueryCorrispondente.isRuoloDefined())
                    {
                        pred = pred.And(a => a.D.VAR_DESC_RUOLO != null &&
                        a.D.VAR_DESC_RUOLO.ToUpper().Contains(objQueryCorrispondente.descrizioneRuolo.ToUpper().Replace("'", "''")));
                    }
                    bQuery1 = bQuery1.Where(pred);
                }
            }


            HashSet<string> sysIdFound = new();
            List<DocsPaVO.utente.Corrispondente> output = new();

            switch (querySelector)
            {
                case 1:
                    bQuery1 = bQuery1.OrderBy(a => a.E.VAR_COGNOME).Distinct();

                    //bQuery1 = bQuery1.DistinctBy(a => new
                    //{
                    //    a.A.SYSTEM_ID,
                    //    a.A.ID_PEOPLE,
                    //    a.A.ID_REGISTRO,
                    //    a.A.ID_AMM,
                    //    a.E.VAR_NOME,
                    //    a.E.VAR_COGNOME,
                    //    a.E.EMAIL_ADDRESS,
                    //    a.E.CHA_NOTIFICA,
                    //    a.E.VAR_TELEFONO,
                    //    a.A.VAR_DESC_CORR,
                    //    a.A.VAR_CODICE,
                    //    a.A.VAR_COD_RUBRICA,
                    //    a.A.CHA_DETTAGLI,
                    //    a.A.CHA_TIPO_URP,
                    //    a.A.VAR_SMTP,
                    //    a.A.NUM_PORTA_SMTP,
                    //    a.A.VAR_CODICE_AMM,
                    //    a.A.VAR_CODICE_AOO,
                    //    a.A.DTA_FINE,
                    //    RUOLO_SYSTEM_ID = a.C.SYSTEM_ID,
                    //    RUOLO_DESC = a.D.VAR_DESC_RUOLO,
                    //    RUOLO_CODICE = a.C.VAR_CODICE,
                    //    RUOLO_ID_UO = a.C.ID_UO,
                    //    RUOLO_COD_RUBRICA = a.C.VAR_COD_RUBRICA,
                    //    RUOLO_DETTAGLI = a.C.CHA_DETTAGLI,
                    //    a.E.CHA_NOTIFICA_CON_ALLEGATO,
                    //    a.E.DISABLED,
                    //    a.E.VAR_SEDE
                    //});
                    var resultbQuery1 = await bQuery1.ToListAsync();
                    foreach (var row in resultbQuery1)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.A.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.A.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.A.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.A.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.A.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;
                                    if (row.A.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.A.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.A.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.A.CHA_PA != null ? row.A.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.A.NUM_LIVELLO != null ? row.A.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.A.VAR_SMTP;
                                    sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.A.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.A.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.A.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.A.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "E";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.A.ID_PARENT != null
                                        && !row.A.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.A.ID_PARENT.ToString(), dataSet);
                                    }

                                    output.Add(corrispondenteUO);
                                    sysIdFound.Add(row.A.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.A.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.A.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.A.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.A.ID_AMM != null ? row.A.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.A.ID_GRUPPO != null ? row.A.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.A.VAR_CODICE_AOO != null ? row.A.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.A.VAR_CODICE_AMM != null ? row.A.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP;
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "E";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                if (objQueryCorrispondente.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO.ToString(), dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO != null ? row.A.ID_UO.ToString() : null, dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "I";
                            }
                            if (row.A.CHA_TIPO_URP.Equals("P"))
                            {
                                DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                                corrispondenteUtente.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteUtente.idPeople = row.A.ID_PEOPLE.ToString();
                                corrispondenteUtente.descrizione = row.A.VAR_DESC_CORR.ToString();
                                corrispondenteUtente.codiceCorrispondente = row.A.VAR_CODICE.ToString();
                                corrispondenteUtente.codiceRubrica = row.A.VAR_COD_RUBRICA.ToString();
                                corrispondenteUtente.dta_fine = row.A.DTA_FINE.ToString();
                                corrispondenteUtente.disabilitato = row.E.DISABLED;
                                corrispondenteUtente.idAmministrazione = row.A.ID_AMM.ToString();
                                corrispondenteUtente.codiceAOO = row.A.VAR_CODICE_AOO.ToString();
                                corrispondenteUtente.codiceAmm = row.A.VAR_CODICE_AMM.ToString();
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteUtente.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteUtente.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP.ToString();
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteUtente.serverPosta = sp;
                                corrispondenteUtente.email = row.E.EMAIL_ADDRESS;
                                corrispondenteUtente.notifica = row.E.CHA_NOTIFICA;
                                corrispondenteUtente.telefono = row.E.VAR_TELEFONO;
                                corrispondenteUtente.tipoIE = "E";
                                corrispondenteUtente.tipoCorrispondente = "P";
                                corrispondenteUtente.notificaConAllegato = row.E.CHA_NOTIFICA_CON_ALLEGATO != null ? row.E.CHA_NOTIFICA_CON_ALLEGATO.Equals("1") : false;

                                if (row.E.VAR_SEDE != null)
                                {
                                    corrispondenteUtente.sede = row.E.VAR_SEDE;
                                }

                                DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                                ruoloUtente.systemId = row.C.SYSTEM_ID.ToString();
                                ruoloUtente.descrizione = row.D.VAR_DESC_RUOLO;
                                ruoloUtente.codiceCorrispondente = row.C.VAR_CODICE;
                                ruoloUtente.codiceRubrica = row.C.VAR_COD_RUBRICA;
                                ruoloUtente.dettagli = row.C.CHA_DETTAGLI != null ? row.C.CHA_DETTAGLI.Equals("1") : false;

                                if (objQueryCorrispondente.isUODefined())
                                {
                                    if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, row.C.ID_UO?.ToString(), dataSet)) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, row.C.ID_UO?.ToString(), dataSet)))
                                    {
                                        ruoloUtente.uo = GetParents(row.C.ID_UO?.ToString(), dataSet);

                                        List<Ruolo> ruoli = new();
                                        ruoli.Add(ruoloUtente);
                                        corrispondenteUtente.ruoli = ruoli.ToArray();
                                        output.Add(corrispondenteUtente);
                                    }
                                }
                                else
                                {
                                    ruoloUtente.uo = GetParents(row.C.ID_UO?.ToString(), dataSet);
                                    List<Ruolo> ruoli = new();
                                    ruoli.Add(ruoloUtente);
                                    corrispondenteUtente.ruoli = ruoli.ToArray();
                                    output.Add(corrispondenteUtente);
                                }

                            }
                        }
                    }
                    break;
                case 2:
                    bQuery2 = bQuery2.OrderBy(a => a.A.VAR_COGNOME);

                    var resultbQuery2 = await bQuery2.ToListAsync();
                    foreach (var row in resultbQuery2)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.A.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.A.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.A.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.A.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.A.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;
                                    if (row.A.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.A.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.A.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.A.CHA_PA != null ? row.A.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.A.NUM_LIVELLO != null ? row.A.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.A.VAR_SMTP;
                                    sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.A.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.A.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.A.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.A.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "E";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.A.ID_PARENT != null
                                        && !row.A.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.A.ID_PARENT.ToString(), dataSet);
                                    }

                                    output.Add(corrispondenteUO);
                                    sysIdFound.Add(row.A.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.A.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.A.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.A.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.A.ID_AMM != null ? row.A.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.A.ID_GRUPPO != null ? row.A.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.A.VAR_CODICE_AOO != null ? row.A.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.A.VAR_CODICE_AMM != null ? row.A.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP;
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "E";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                if (objQueryCorrispondente.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO.ToString(), dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO != null ? row.A.ID_UO.ToString() : null, dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "E";
                            }
                        }
                    }

                    break;
                case 3:
                    var resultbQuery3 = await bQuery3.ToListAsync();
                    foreach (var row in resultbQuery3)
                    {
                        if (row.CHA_TIPO_URP.Equals("U"))
                        {
                            bool rowAlreadyFound = sysIdFound.Contains(row.SYSTEM_ID.ToString());
                            if (!rowAlreadyFound)
                            {
                                DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                corrispondenteUO.systemId = row.SYSTEM_ID.ToString();
                                corrispondenteUO.descrizione = row.VAR_DESC_CORR;
                                corrispondenteUO.codiceCorrispondente = row.VAR_CODICE;
                                corrispondenteUO.codiceRubrica = row.VAR_COD_RUBRICA;
                                corrispondenteUO.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;
                                if (row.ID_REGISTRO != null)
                                {
                                    corrispondenteUO.idRegistro = row.ID_REGISTRO.ToString();
                                }
                                corrispondenteUO.email = row.VAR_EMAIL;
                                corrispondenteUO.interoperante = row.CHA_PA != null ? row.CHA_PA.Equals("1") : false;
                                corrispondenteUO.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;
                                corrispondenteUO.livello = row.NUM_LIVELLO != null ? row.NUM_LIVELLO.ToString() : string.Empty;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.VAR_SMTP;
                                sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteUO.serverPosta = sp;
                                corrispondenteUO.idAmministrazione = row.ID_AMM.ToString();
                                corrispondenteUO.codiceAOO = row.VAR_CODICE_AOO;
                                corrispondenteUO.codiceAmm = row.VAR_CODICE_AMM;
                                corrispondenteUO.codiceIstat = row.VAR_CODICE_ISTAT;
                                corrispondenteUO.tipoIE = "I";
                                corrispondenteUO.tipoCorrispondente = "U";

                                output.Add(corrispondenteUO);
                                sysIdFound.Add(row.SYSTEM_ID.ToString());
                            }
                        }
                        if (row.CHA_TIPO_URP.Equals("R"))
                        {
                            DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                            corrispondenteRuolo.systemId = row.SYSTEM_ID.ToString();
                            corrispondenteRuolo.descrizione = row.VAR_DESC_CORR;
                            corrispondenteRuolo.codiceCorrispondente = row.VAR_CODICE;
                            corrispondenteRuolo.codiceRubrica = row.VAR_COD_RUBRICA;
                            corrispondenteRuolo.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;

                            corrispondenteRuolo.idAmministrazione = row.ID_AMM != null ? row.ID_AMM.ToString() : null;
                            corrispondenteRuolo.idGruppo = row.ID_GRUPPO != null ? row.ID_GRUPPO.ToString() : null;
                            corrispondenteRuolo.codiceAOO = row.VAR_CODICE_AOO != null ? row.VAR_CODICE_AOO.ToString() : null;
                            corrispondenteRuolo.codiceAmm = row.VAR_CODICE_AMM != null ? row.VAR_CODICE_AMM.ToString() : null;
                            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                            sp.serverSMTP = row.VAR_SMTP;
                            sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                            corrispondenteRuolo.serverPosta = sp;
                            corrispondenteRuolo.tipoIE = "I";
                            corrispondenteRuolo.tipoCorrispondente = "R";
                            if (row.ID_REGISTRO != null)
                            {
                                corrispondenteRuolo.idRegistro = row.ID_REGISTRO.ToString();
                            }
                            corrispondenteRuolo.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;

                            if (corrispondenteRuolo.uo != null)
                                corrispondenteRuolo.uo.tipoIE = "I";
                        }
                    }
                    break;
            }


            return output;
        }

        private async Task<List<DocsPaVO.utente.Corrispondente>> ListaCorrispondentiInt(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            List<DocsPaVO.utente.Corrispondente> listaCorr = new List<DocsPaVO.utente.Corrispondente>();
            var idTenant = qco.idAmministrazione.AsLong();
            var codiceRubrica = qco.codiceRubrica.ToUpper();
            var tipo = string.Empty;
            try
            {
                IQueryable<CorrispondenteResultEntity> queryable = null;

                if(!string.IsNullOrEmpty(qco.codiceRubrica))
                {
                    var queryableParent = _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.ID_AMM == idTenant &&
                                    c.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica) && 
                                    c.CHA_TIPO_IE == "I" && c.CHA_TIPO_CORR == "S")
                        .Select(c => new
                        {
                            c.SYSTEM_ID,
                            c.CHA_TIPO_URP,
                            c.VAR_DESC_CORR,
                            c.ID_GRUPPO,
                            c.DTA_FINE,
                            c.ID_AMM,
                            c.ID_REGISTRO,
                            c.VAR_CODICE,
                            c.VAR_COD_RUBRICA,
                            c.VAR_SMTP,
                            c.NUM_PORTA_SMTP,
                            c.ID_UO,
                            c.ID_PARENT,
                            c.CHA_DETTAGLI,
                            c.VAR_CODICE_AOO,
                            c.VAR_CODICE_AMM,
                            c.VAR_EMAIL,
                            c.CHA_PA,
                            c.VAR_CODICE_ISTAT,
                            c.NUM_LIVELLO
                        });
                    if (qco.fineValidita)
                        queryableParent = queryableParent.Where(c => !c.DTA_FINE.HasValue);

                    var parent = await queryableParent                               
                                .OrderBy(c => c.VAR_DESC_CORR)
                                .FirstOrDefaultAsync();

                    if(parent == null)
                        return listaCorr;

                    if(!qco.getChildren)
                    {
                        if(parent.CHA_TIPO_URP == "P")
                        {
                            tipo = "P";
                            queryable = this._dbContext.CorrGlobaliEntities
                               .Join(this._dbContext.PeopleGroupEntities,
                                   a => a.ID_PEOPLE,
                                   b => b.PEOPLE_SYSTEM_ID,
                                   (a, b) => new { a, b })
                               .Join(this._dbContext.CorrGlobaliEntities,
                                   j => j.b.GROUPS_SYSTEM_ID,
                                   c => c.ID_GRUPPO,
                                   (j, c) => new { j.a, j.b, c })
                               .Join(this._dbContext.TipoRuoloEntities,
                                   j => j.c.ID_TIPO_RUOLO,
                                   d => d.SYSTEM_ID,
                                   (j, d) => new { j.a, j.b, j.c, d })
                               .Join(this._dbContext.PeopleEntities,
                                   j => j.a.ID_PEOPLE,
                                   e => e.SYSTEM_ID,
                                   (j, e) => new { j.a, j.b, j.c, j.d, e })
                               .Where(j => j.a.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica) &&
                                        j.a.CHA_TIPO_CORR == "S" &&
                                       j.a.ID_AMM == idTenant)
                               .Select(j => new CorrispondenteResultEntity()
                                       {
                                           SYSTEM_ID = j.a.SYSTEM_ID,
                                           ID_PEOPLE = j.a.ID_PEOPLE,
                                           ID_REGISTRO = j.a.ID_REGISTRO,
                                           ID_AMM = j.a.ID_AMM,
                                           VAR_NOME = j.e.VAR_NOME,
                                           VAR_COGNOME = j.e.VAR_COGNOME,
                                           EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                           CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                           VAR_TELEFONO = j.e.VAR_TELEFONO,
                                           VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                           VAR_CODICE = j.a.VAR_CODICE,
                                           VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                           CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                           CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                           VAR_SMTP = j.a.VAR_SMTP,
                                           NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                           VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                           VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                           DTA_FINE = j.a.DTA_FINE,
                                           DTA_FINE_IN_RUOLO = j.b.DTA_FINE,
                                           RUOLO_SYSTEM_ID = j.c.SYSTEM_ID,
                                           RUOLO_DESC = j.d.VAR_DESC_RUOLO,
                                           RUOLO_CODICE = j.c.VAR_CODICE,
                                           RUOLO_ID_UO = j.c.ID_UO,
                                           RUOLO_COD_RUBRICA = j.c.VAR_COD_RUBRICA,
                                           RUOLO_DETTAGLI = j.c.CHA_DETTAGLI,
                                           CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                           DISABLED = j.e.DISABLED,
                                           VAR_SEDE = j.e.VAR_SEDE
                                       });

                            if (qco.fineValidita)
                                queryable = queryable.Where(j => !j.DTA_FINE_IN_RUOLO.HasValue);

                            queryable = queryable.Distinct().OrderBy(j => j.VAR_COGNOME);
                        }
                        else if (parent.CHA_TIPO_URP == "R")
                        {
                            tipo = "R";
                            queryable = _dbContext.CorrGlobaliEntities
                                .Join(_dbContext.TipoRuoloEntities,
                                    a => a.ID_TIPO_RUOLO,
                                    b => b.SYSTEM_ID,
                                    (a, b) => new { a, b })
                                .Where(j => j.a.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica) &&
                                       j.a.ID_AMM == idTenant &&
                                       j.a.CHA_TIPO_CORR.Equals("S"))
                                .Select(j => new CorrispondenteResultEntity()
                                {
                                    SYSTEM_ID = j.a.SYSTEM_ID,
                                    ID_REGISTRO = j.a.ID_REGISTRO,
                                    ID_AMM = j.a.ID_AMM,
                                    VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                    VAR_COGNOME = j.a.VAR_COGNOME,
                                    VAR_DESC_RUOLO = j.b.VAR_DESC_RUOLO,
                                    NUM_LIV_B = j.b.NUM_LIVELLO,
                                    ID_GRUPPO = j.a.ID_GRUPPO,
                                    VAR_CODICE = j.a.VAR_CODICE,
                                    VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                    CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                    VAR_SMTP = j.a.VAR_SMTP,
                                    NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                    ID_UO = j.a.ID_UO,
                                    ID_PARENT = j.a.ID_PARENT,
                                    CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                    VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                    VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                    DTA_FINE = j.a.DTA_FINE,
                                    VAR_EMAIL = j.a.VAR_EMAIL
                                });

                            if (qco.fineValidita)
                                queryable = queryable.Where(j => !j.DTA_FINE.HasValue);

                            queryable = queryable.OrderBy(j => j.VAR_DESC_CORR);
                        }
                        else
                        {
                            tipo = "U";
                            queryable = queryableParent.Select(j => new CorrispondenteResultEntity()
                            {
                                SYSTEM_ID = j.SYSTEM_ID,
                                ID_AMM = j.ID_AMM,
                                ID_REGISTRO = j.ID_REGISTRO,
                                VAR_DESC_CORR = j.VAR_DESC_CORR,
                                VAR_CODICE = j.VAR_CODICE,
                                VAR_COD_RUBRICA = j.VAR_COD_RUBRICA,
                                CHA_TIPO_URP = j.CHA_TIPO_URP,
                                VAR_SMTP = j.VAR_SMTP,
                                NUM_PORTA_SMTP = j.NUM_PORTA_SMTP,
                                ID_UO = j.ID_UO,
                                ID_PARENT = j.ID_PARENT,
                                CHA_DETTAGLI = j.CHA_DETTAGLI,
                                VAR_CODICE_AOO = j.VAR_CODICE_AOO,
                                VAR_CODICE_AMM = j.VAR_CODICE_AMM,
                                DTA_FINE = j.DTA_FINE,
                                VAR_EMAIL = j.VAR_EMAIL,
                                CHA_PA = j.CHA_PA,
                                VAR_CODICE_ISTAT = j.VAR_CODICE_ISTAT,
                                NUM_LIVELLO = j.NUM_LIVELLO
                            });

                            queryable = queryable.OrderBy(j => j.VAR_DESC_CORR);
                        }
                    }
                    else
                    {
                        if (parent.CHA_TIPO_URP == "P")
                            return listaCorr;

                        if (parent.CHA_TIPO_URP == "U")
                        {
                            tipo = "R";
                            queryable = _dbContext.CorrGlobaliEntities
                                .GroupJoin(this._dbContext.TipoRuoloEntities,
                                    a => a.ID_TIPO_RUOLO,
                                    b => b.SYSTEM_ID,
                                    (a, b) => new { a, b })
                                .SelectMany(j => j.b.DefaultIfEmpty(), (j, b) => new { j.a, b })
                                .Where(j => (j.a.ID_UO == parent.SYSTEM_ID || j.a.ID_PARENT == parent.SYSTEM_ID) &&
                                            j.a.CHA_TIPO_IE == "I" &&
                                            j.a.CHA_TIPO_CORR == "S")
                                .Select(j => new CorrispondenteResultEntity()
                                {
                                    SYSTEM_ID = j.a.SYSTEM_ID,
                                    ID_REGISTRO = j.a.ID_REGISTRO,
                                    ID_AMM = j.a.ID_AMM,
                                    VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                    VAR_COGNOME = j.a.VAR_COGNOME,
                                    VAR_EMAIL = j.a.VAR_EMAIL,
                                    VAR_DESC_RUOLO = j.b != null ? j.b.VAR_DESC_RUOLO : string.Empty,
                                    NUM_LIV_B = j.b != null ? j.b.NUM_LIVELLO : null,
                                    ID_GRUPPO = j.a.ID_GRUPPO,
                                    VAR_CODICE = j.a.VAR_CODICE,
                                    VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                    CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                    VAR_SMTP = j.a.VAR_SMTP,
                                    NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                    ID_UO = j.a.ID_UO,
                                    ID_PARENT = j.a.ID_PARENT,
                                    CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                    VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                    VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                    DTA_FINE = j.a.DTA_FINE,
                                    VAR_CODICE_ISTAT = j.a.VAR_CODICE_ISTAT
                                });

                            if (qco.fineValidita)
                                queryable = queryable.Where(j => !j.DTA_FINE.HasValue);

                            queryable = queryable.OrderBy(j => j.VAR_DESC_CORR);
                        }

                        if (parent.CHA_TIPO_URP == "R")
                        {
                            tipo = "P";
                            queryable = this._dbContext.CorrGlobaliEntities
                               .Join(this._dbContext.PeopleGroupEntities,
                                   a => a.ID_PEOPLE,
                                   b => b.PEOPLE_SYSTEM_ID,
                                   (a, b) => new { a, b })
                               .Join(this._dbContext.CorrGlobaliEntities,
                                   j => j.b.GROUPS_SYSTEM_ID,
                                   c => c.ID_GRUPPO,
                                   (j, c) => new { j.a, j.b, c })
                               .Join(this._dbContext.TipoRuoloEntities,
                                   j => j.c.ID_TIPO_RUOLO,
                                   d => d.SYSTEM_ID,
                                   (j, d) => new { j.a, j.b, j.c, d })
                               .Join(this._dbContext.PeopleEntities,
                                   j => j.a.ID_PEOPLE,
                                   e => e.SYSTEM_ID,
                                   (j, e) => new { j.a, j.b, j.c, j.d, e })
                               .Where(j => j.b.GROUPS_SYSTEM_ID == parent.ID_GRUPPO && 
                                           j.a.CHA_TIPO_IE == "I" &&
                                           j.a.CHA_TIPO_CORR == "S")
                               .Select(j => new CorrispondenteResultEntity()
                               {
                                   SYSTEM_ID = j.a.SYSTEM_ID,
                                   ID_PEOPLE = j.a.ID_PEOPLE,
                                   ID_REGISTRO = j.a.ID_REGISTRO,
                                   ID_AMM = j.a.ID_AMM,
                                   VAR_NOME = j.e.VAR_NOME,
                                   VAR_COGNOME = j.e.VAR_COGNOME,
                                   EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                   CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                   VAR_TELEFONO = j.e.VAR_TELEFONO,
                                   VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                   VAR_CODICE = j.a.VAR_CODICE,
                                   VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                   CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                   CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                   VAR_SMTP = j.a.VAR_SMTP,
                                   NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                   VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                   VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                   DTA_FINE = j.a.DTA_FINE,
                                   DTA_FINE_IN_RUOLO = j.b.DTA_FINE,
                                   RUOLO_SYSTEM_ID = j.c.SYSTEM_ID,
                                   RUOLO_DESC = j.d.VAR_DESC_RUOLO,
                                   RUOLO_CODICE = j.c.VAR_CODICE,
                                   RUOLO_ID_UO = j.c.ID_UO,
                                   RUOLO_COD_RUBRICA = j.c.VAR_COD_RUBRICA,
                                   RUOLO_DETTAGLI = j.c.CHA_DETTAGLI,
                                   RUOLO_DATA_FINE = j.c.DTA_FINE,
                                   CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                   DISABLED = j.e.DISABLED,
                                   VAR_SEDE = j.e.VAR_SEDE
                               }); 

                            if (qco.fineValidita)
                                queryable = queryable.Where(j => !j.DTA_FINE_IN_RUOLO.HasValue && !j.RUOLO_DATA_FINE.HasValue && !j.DTA_FINE.HasValue);

                            queryable = queryable.Distinct().OrderBy(j => j.VAR_COGNOME);
                        }
                    }
                }
                else
                {
                    var queryableParent = _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.ID_AMM == idTenant);

                    if (qco.fineValidita)
                        queryableParent = queryableParent.Where(c => !c.DTA_FINE.HasValue);

                    if (!string.IsNullOrEmpty(qco.codiceUO))
                    {
                        var codiceUO = qco.codiceUO.ToUpper();
                        queryableParent = queryableParent.Where(c => c.VAR_CODICE.ToUpper().Equals(codiceUO) &&
                                          c.CHA_TIPO_URP == "U" &&
                                          c.CHA_TIPO_IE == "I" &&
                                          c.CHA_TIPO_CORR == "S");
                     }

                    if(!string.IsNullOrEmpty(qco.descrizioneUO))
                    {
                        var uoListPred = PredicateBuilder.New<CorrGlobaliEntity>();
                        queryableParent = queryableParent.Where(c => c.CHA_TIPO_URP == "U" &&
                                          c.CHA_TIPO_IE == "I" &&
                                          c.CHA_TIPO_CORR == "S");

                        var uoList = qco.descrizioneUO.Split(';').ToList();
                        if (uoList.Count > 0)
                        {
                            uoList.ForEach(uo =>
                            {
                                uoListPred = uoListPred.Or(a => a.VAR_DESC_CORR != null && uo.ToUpper().Contains(a.VAR_DESC_CORR.ToUpper()));
                            });
                            queryableParent = queryableParent.Where(uoListPred);
                        }
                    }

                    queryable = queryableParent.Select(j => new CorrispondenteResultEntity()
                    {
                        SYSTEM_ID = j.SYSTEM_ID,
                        ID_AMM = j.ID_AMM,
                        ID_REGISTRO = j.ID_REGISTRO,
                        VAR_DESC_CORR = j.VAR_DESC_CORR,
                        VAR_CODICE = j.VAR_CODICE,
                        VAR_COD_RUBRICA = j.VAR_COD_RUBRICA,
                        CHA_TIPO_URP = j.CHA_TIPO_URP,
                        VAR_SMTP = j.VAR_SMTP,
                        NUM_PORTA_SMTP = j.NUM_PORTA_SMTP,
                        ID_UO = j.ID_UO,
                        ID_PARENT = j.ID_PARENT,
                        CHA_DETTAGLI = j.CHA_DETTAGLI,
                        VAR_CODICE_AOO = j.VAR_CODICE_AOO,
                        VAR_CODICE_AMM = j.VAR_CODICE_AMM,
                        DTA_FINE = j.DTA_FINE,
                        VAR_EMAIL = j.VAR_EMAIL,
                        CHA_PA = j.CHA_PA,
                        VAR_CODICE_ISTAT = j.VAR_CODICE_ISTAT,
                        NUM_LIVELLO = j.NUM_LIVELLO
                    })
                    .OrderBy(j => j.VAR_DESC_CORR);

                    if(qco.isRuoloDefined())
                    {
                        queryable = _dbContext.CorrGlobaliEntities
                                .Join(_dbContext.TipoRuoloEntities,
                                    a => a.ID_TIPO_RUOLO,
                                    b => b.SYSTEM_ID,
                                    (a, b) => new { a, b })
                                .Where(j => EF.Functions.Like(j.a.VAR_DESC_CORR.ToUpper(), $"{qco.descrizioneRuolo.ToUpper()}%") &&
                                        j.a.ID_AMM == idTenant &&
                                        j.a.CHA_TIPO_URP == "R" &&
                                        j.a.CHA_TIPO_IE == "I")
                                .Select(j => new CorrispondenteResultEntity()
                                {
                                    SYSTEM_ID = j.a.SYSTEM_ID,
                                    ID_REGISTRO = j.a.ID_REGISTRO,
                                    ID_AMM = j.a.ID_AMM,
                                    VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                    VAR_COGNOME = j.a.VAR_COGNOME,
                                    VAR_DESC_RUOLO = j.b.VAR_DESC_RUOLO,
                                    NUM_LIV_B = j.b.NUM_LIVELLO,
                                    ID_GRUPPO = j.a.ID_GRUPPO,
                                    VAR_CODICE = j.a.VAR_CODICE,
                                    VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                    CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                    VAR_SMTP = j.a.VAR_SMTP,
                                    NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                    ID_UO = j.a.ID_UO,
                                    ID_PARENT = j.a.ID_PARENT,
                                    CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                    VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                    VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                    DTA_FINE = j.a.DTA_FINE,
                                    VAR_EMAIL = j.a.VAR_EMAIL
                                });

                        if (qco.fineValidita)
                            queryable = queryable.Where(j => !j.DTA_FINE.HasValue);

                        queryable = queryable.OrderBy(j => j.VAR_DESC_CORR);
                    }

                    if (qco.isUtenteDefined())
                    {
                        queryable = this._dbContext.CorrGlobaliEntities
                               .Join(this._dbContext.PeopleGroupEntities,
                                   a => a.ID_PEOPLE,
                                   b => b.PEOPLE_SYSTEM_ID,
                                   (a, b) => new { a, b })
                               .Join(this._dbContext.CorrGlobaliEntities,
                                   j => j.b.GROUPS_SYSTEM_ID,
                                   c => c.ID_GRUPPO,
                                   (j, c) => new { j.a, j.b, c })
                               .Join(this._dbContext.TipoRuoloEntities,
                                   j => j.c.ID_TIPO_RUOLO,
                                   d => d.SYSTEM_ID,
                                   (j, d) => new { j.a, j.b, j.c, d })
                               .Join(this._dbContext.PeopleEntities,
                                   j => j.a.ID_PEOPLE,
                                   e => e.SYSTEM_ID,
                                   (j, e) => new { j.a, j.b, j.c, j.d, e })
                               .Where(j => j.a.ID_AMM == idTenant &&
                                           j.a.CHA_TIPO_URP == "P" &&
                                           j.a.CHA_TIPO_IE == "I" &&
                                           j.a.CHA_TIPO_CORR == "S")
                               .Select(j => new CorrispondenteResultEntity()
                               {
                                   SYSTEM_ID = j.a.SYSTEM_ID,
                                   ID_PEOPLE = j.a.ID_PEOPLE,
                                   ID_REGISTRO = j.a.ID_REGISTRO,
                                   ID_AMM = j.a.ID_AMM,
                                   VAR_NOME = j.e.VAR_NOME,
                                   VAR_COGNOME = j.e.VAR_COGNOME,
                                   EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                   CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                   VAR_TELEFONO = j.e.VAR_TELEFONO,
                                   VAR_DESC_CORR = j.a.VAR_DESC_CORR,
                                   VAR_CODICE = j.a.VAR_CODICE,
                                   VAR_COD_RUBRICA = j.a.VAR_COD_RUBRICA,
                                   CHA_DETTAGLI = j.a.CHA_DETTAGLI,
                                   CHA_TIPO_URP = j.a.CHA_TIPO_URP,
                                   VAR_SMTP = j.a.VAR_SMTP,
                                   NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                                   VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                                   VAR_CODICE_AOO = j.a.VAR_CODICE_AOO,
                                   DTA_FINE = j.a.DTA_FINE,
                                   DTA_FINE_IN_RUOLO = j.b.DTA_FINE,
                                   RUOLO_SYSTEM_ID = j.c.SYSTEM_ID,
                                   RUOLO_DESC = j.d.VAR_DESC_RUOLO,
                                   RUOLO_CODICE = j.c.VAR_CODICE,
                                   RUOLO_ID_UO = j.c.ID_UO,
                                   RUOLO_COD_RUBRICA = j.c.VAR_COD_RUBRICA,
                                   RUOLO_DETTAGLI = j.c.CHA_DETTAGLI,
                                   RUOLO_DATA_FINE = j.c.DTA_FINE,
                                   CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                   DISABLED = j.e.DISABLED,
                                   VAR_SEDE = j.e.VAR_SEDE
                               });

                        if (qco.fineValidita)
                            queryable = queryable.Where(j => !j.DTA_FINE_IN_RUOLO.HasValue);

                        if(!string.IsNullOrEmpty(qco.nomeUtente))
                        {
                            var nomeUtente = qco.nomeUtente.ToUpper();
                            queryable = queryable.Where(j => EF.Functions.Like(j.VAR_NOME.ToUpper(), $"{nomeUtente}%"));
                        }
                        else
                        {
                            var cognomeUtente = qco.cognomeUtente.ToUpper();
                            queryable = queryable.Where(j => EF.Functions.Like(j.VAR_COGNOME.ToUpper(), $"{cognomeUtente}%"));
                        }

                        if(qco.isRuoloDefined())
                        {
                            var descrizioneRuolo = qco.descrizioneRuolo.ToUpper();
                            queryable = queryable.Where(j => EF.Functions.Like(j.RUOLO_DESC.ToUpper(), $"{descrizioneRuolo}%"));
                        }

                        queryable = queryable.Distinct().OrderBy(j => j.VAR_COGNOME);
                    }

                    var parent = await queryableParent
                                .OrderBy(c => c.VAR_DESC_CORR)
                                .FirstOrDefaultAsync();
                }

                var corrispondente = await queryable.AsNoTracking()
                                        .ToListAsync();
                if(corrispondente.Any())
                {
                    HashSet<string> sysIdFound = new();
                    foreach (var row in corrispondente)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;
                                    if (row.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.CHA_PA != null ? row.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.NUM_LIVELLO != null ? row.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.VAR_SMTP;
                                    sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "I";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.ID_PARENT != null
                                        && !row.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.ID_PARENT.ToString(), dataSet);
                                    }

                                    listaCorr.Add(corrispondenteUO);
                                    sysIdFound.Add(row.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.ID_AMM != null ? row.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.ID_GRUPPO != null ? row.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.VAR_CODICE_AOO != null ? row.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.VAR_CODICE_AMM != null ? row.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.VAR_SMTP;
                                sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "I";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;
                                if (qco.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.ID_UO.ToString(), dataSet);
                                    listaCorr.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.ID_UO != null ? row.ID_UO.ToString() : null, dataSet);
                                    listaCorr.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "I";
                            }
                            if (row.CHA_TIPO_URP.Equals("P"))
                            {
                                DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                                corrispondenteUtente.systemId = row.SYSTEM_ID.ToString();
                                corrispondenteUtente.idPeople = row.ID_PEOPLE.ToString();
                                corrispondenteUtente.descrizione = row.VAR_DESC_CORR.ToString();
                                corrispondenteUtente.codiceCorrispondente = row.VAR_CODICE?.ToString();
                                corrispondenteUtente.codiceRubrica = row.VAR_COD_RUBRICA.ToString();
                                corrispondenteUtente.dta_fine = row.DTA_FINE.HasValue ? row.DTA_FINE.ToString() : string.Empty;
                                corrispondenteUtente.disabilitato = row.DISABLED;
                                corrispondenteUtente.idAmministrazione = row.ID_AMM.ToString();
                                corrispondenteUtente.codiceAOO = row.VAR_CODICE_AOO ?? string.Empty;
                                corrispondenteUtente.codiceAmm = row.VAR_CODICE_AMM ?? string.Empty;
                                if (row.ID_REGISTRO != null)
                                {
                                    corrispondenteUtente.idRegistro = row.ID_REGISTRO.ToString();
                                }
                                corrispondenteUtente.dettagli = row.CHA_DETTAGLI == "1";
                                corrispondenteUtente.serverPosta = new DocsPaVO.utente.ServerPosta()
                                {
                                    serverSMTP = row.VAR_SMTP ?? string.Empty,
                                    portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null
                                 };
                                corrispondenteUtente.email = row.EMAIL_ADDRESS;
                                corrispondenteUtente.notifica = row.CHA_NOTIFICA;
                                corrispondenteUtente.telefono = row.VAR_TELEFONO;
                                corrispondenteUtente.tipoIE = "I";
                                corrispondenteUtente.tipoCorrispondente = "P";
                                corrispondenteUtente.notificaConAllegato = row.CHA_NOTIFICA_CON_ALLEGATO == "1";

                                if (row.VAR_SEDE != null)
                                {
                                    corrispondenteUtente.sede = row.VAR_SEDE;
                                }

                                DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo()
                                {
                                    systemId = row.RUOLO_SYSTEM_ID.ToString(),
                                    descrizione = row.RUOLO_DESC,
                                    codiceCorrispondente = row.RUOLO_CODICE,
                                    codiceRubrica = row.RUOLO_COD_RUBRICA,
                                    dettagli = row.RUOLO_DETTAGLI == "1"
                                };

                                if (qco.isUODefined())
                                {
                                    if ((qco.descrizioneUO != null && HasDefinedUo(qco.descrizioneUO, 1, row.RUOLO_ID_UO?.ToString(), dataSet)) || 
                                        (qco.codiceUO != null && HasDefinedUo(qco.codiceUO, 2, row.RUOLO_ID_UO?.ToString(), dataSet)))
                                    {
                                        ruoloUtente.uo = GetParents(row.RUOLO_ID_UO?.ToString(), dataSet);

                                        List<Ruolo> ruoli = new();
                                        ruoli.Add(ruoloUtente);
                                        corrispondenteUtente.ruoli = ruoli.ToArray();
                                    }
                                }
                                else
                                {
                                    ruoloUtente.uo = GetParents(row.RUOLO_ID_UO?.ToString(), dataSet);
                                    List<Ruolo> ruoli = new();
                                    ruoli.Add(ruoloUtente);
                                    corrispondenteUtente.ruoli = ruoli.ToArray();
                                }
                                listaCorr.Add(corrispondenteUtente);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return listaCorr;
        }


        private async Task<List<DocsPaVO.utente.Corrispondente>> ListaCorrispondentiIntOld(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            List<DocsPaVO.utente.Corrispondente> corrs = new();
            var generalPredicate = this.GetRegPredicate(qco);

            bool selectedAVarCog = false;
            bool selectedEVarCog = false;

            DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente = CorrectApiciQuery(qco);

            IQueryable<InfoCorr>? bQuery1 = null;
            IQueryable<CorrConTipoRuolo>? bQuery2 = null;
            IQueryable<CorrGlobaliEntity> bQuery3 = null;
            int querySelector = 0;

            if (objQueryCorrispondente.codiceRubrica != null)
            {
                generalPredicate = generalPredicate.And(a => (a.ID_AMM == null || a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()));

                if (objQueryCorrispondente.fineValidita)
                    generalPredicate = generalPredicate.And(a => !a.DTA_FINE.HasValue);

                generalPredicate = generalPredicate.And(a => a.VAR_COD_RUBRICA != null && 
                     a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                     a.CHA_TIPO_IE == "I" 
                     && a.CHA_TIPO_CORR == "S");

                var parent = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(generalPredicate)
                    .OrderBy(a => a.VAR_DESC_CORR)
                    .FirstOrDefaultAsync();

                if (parent == null)
                {
                    return new();
                }
                if (objQueryCorrispondente.getChildren == false)
                {
                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("P"))
                    {
                        selectedEVarCog = true;

                        var queryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                           .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(),
                               a => a.ID_PEOPLE,
                               b => b.PEOPLE_SYSTEM_ID,
                               (a, b) => new { a, b })
                           .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                               j => j.b.GROUPS_SYSTEM_ID,
                               c => c.ID_GRUPPO,
                               (j, c) => new { j.a, j.b, c })
                           .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(),
                               j => j.c.ID_TIPO_RUOLO,
                               d => d.SYSTEM_ID,
                               (j, d) => new { j.a, j.b, j.c, d })
                           .Join(this._dbContext.PeopleEntities.AsNoTracking(),
                               j => j.a.ID_PEOPLE,
                               e => e.SYSTEM_ID,
                               (j, e) => new { j.a, j.b, j.c, j.d, e })
                           .Where(j => j.a.CHA_TIPO_IE == "I" &&
                                   j.a.CHA_TIPO_CORR == "S" &&
                                   j.a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                                   (j.a.ID_AMM == null || j.a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()));

                        if (objQueryCorrispondente.fineValidita)
                            queryable = queryable.Where(j => !j.b.DTA_FINE.HasValue);

                        bQuery1 = queryable
                             .Select(j => new InfoCorr()
                             {
                                 A = j.a,
                                 B = j.b,
                                 C = j.c,
                                 D = j.d,
                                 E = new PeopleEntity()
                                 {
                                     VAR_NOME = j.e.VAR_NOME,
                                     VAR_COGNOME = j.e.VAR_COGNOME,
                                     EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                     CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                     VAR_TELEFONO = j.e.VAR_TELEFONO,
                                     CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                     DISABLED = j.e.DISABLED,
                                     VAR_SEDE = j.e.VAR_SEDE
                                 }
                             });

                        querySelector = 1;
                    }
                    else if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("R"))
                    {
                        selectedAVarCog = true;

                        bQuery2 = _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Join(_dbContext.TipoRuoloEntities.AsNoTracking(),
                                a => a.ID_TIPO_RUOLO,
                                b => b.SYSTEM_ID,
                                (a, b) => new { a, b })
                            .Where(j => j.a.VAR_COD_RUBRICA != null &&
                                   j.a.CHA_TIPO_IE != null &&
                                   j.a.CHA_TIPO_IE.Equals("I") &&
                                   j.a.VAR_COD_RUBRICA.ToUpper().Equals(objQueryCorrispondente.codiceRubrica.ToUpper()) &&
                                   (j.a.ID_AMM == null || j.a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong()) &&
                                   j.a.CHA_TIPO_CORR != null &&
                                   j.a.CHA_TIPO_CORR.Equals("S"))
                            .Select(j => new CorrConTipoRuolo()
                            {
                                A = j.a,
                                B = j.b
                            });

                        querySelector = 2;

                        if (objQueryCorrispondente.fineValidita)
                            bQuery2 = bQuery2.Where(a => !a.A.DTA_FINE.HasValue);
                    }
                    else
                    {
                        querySelector = 3;
                        bQuery3 = this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(generalPredicate).OrderBy(a => a.VAR_DESC_CORR);
                    }
                }
                else
                {
                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("P"))
                    {
                        return new();
                    }

                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("U"))
                    {
                        selectedAVarCog = true;

                        bQuery2 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                   where ((a.ID_TIPO_RUOLO == b.SYSTEM_ID || a.ID_TIPO_RUOLO == null) &&
                                   (a.ID_UO == parent.SYSTEM_ID || a.ID_PARENT == parent.SYSTEM_ID) &&
                                   a.CHA_TIPO_CORR != null &&
                                   a.CHA_TIPO_CORR.Equals("S") &&
                                   a.CHA_TIPO_IE != null &&
                                   a.CHA_TIPO_IE.Equals("I")
                                   )
                                   select new CorrConTipoRuolo()
                                   {
                                       A = a,
                                       B = b
                                   });

                        if (objQueryCorrispondente.fineValidita)
                        {
                            bQuery2 = bQuery2.Where(a => !a.A.DTA_FINE.HasValue);
                        }
                        querySelector = 2;

                    }

                    if (parent.CHA_TIPO_URP != null && parent.CHA_TIPO_URP.Equals("R"))
                    {
                        var pepPred = PredicateBuilder.New<InfoCorr>();

                        selectedEVarCog = true;

                        var queryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(),
                                a => a.ID_PEOPLE,
                                b => b.PEOPLE_SYSTEM_ID,
                                (a, b) => new { a, b })
                            .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                                j => j.b.GROUPS_SYSTEM_ID,
                                c => c.ID_GRUPPO,
                                (j, c) => new { j.a, j.b, c })
                            .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(),
                                j => j.c.ID_TIPO_RUOLO,
                                d => d.SYSTEM_ID,
                                (j, d) => new { j.a, j.b, j.c, d })
                            .Join(this._dbContext.PeopleEntities.AsNoTracking(),
                                j => j.a.ID_PEOPLE,
                                e => e.SYSTEM_ID,
                                (j, e) => new { j.a, j.b, j.c, j.d, e })
                            .Where(j => j.b.GROUPS_SYSTEM_ID == parent.ID_GRUPPO &&
                                    j.a.CHA_TIPO_IE == "I" &&
                                    j.a.CHA_TIPO_CORR == "S");
                           
                        if (objQueryCorrispondente.fineValidita)
                        {
                            queryable = queryable.Where(j => !j.b.DTA_FINE.HasValue && !j.c.DTA_FINE.HasValue && !j.a.DTA_FINE.HasValue);
                        }

                        bQuery1 = queryable
                             .Select(j => new InfoCorr()
                             {
                                 A = j.a,
                                 B = j.b,
                                 C = j.c,
                                 D = j.d,
                                 E = new PeopleEntity()
                                 {
                                     VAR_NOME = j.e.VAR_NOME,
                                     VAR_COGNOME = j.e.VAR_COGNOME,
                                     EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                     CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                     VAR_TELEFONO = j.e.VAR_TELEFONO,
                                     CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                     DISABLED = j.e.DISABLED,
                                     VAR_SEDE = j.e.VAR_SEDE
                                 }
                             });

                        querySelector = 1;
                    }
                }
            }
            else
            {
                var pepPred = PredicateBuilder.New<CorrGlobaliEntity>();
                var codIfNoDescPred = PredicateBuilder.New<CorrGlobaliEntity>();
                var uoListPred = PredicateBuilder.New<CorrGlobaliEntity>();

                if (objQueryCorrispondente.fineValidita)
                    pepPred = pepPred.And(a => !a.DTA_FINE.HasValue);

                bQuery3 = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking() select a);
                querySelector = 3;

                if (objQueryCorrispondente.codiceUO != null)
                {
                    codIfNoDescPred = pepPred.And(a => a.VAR_CODICE != null && a.VAR_CODICE.ToUpper().Equals(objQueryCorrispondente.codiceUO.ToUpper()) &&
                        a.CHA_TIPO_URP != null &&
                        a.CHA_TIPO_IE != null &&
                        a.CHA_TIPO_CORR != null &&
                        a.CHA_TIPO_URP.Equals("U") &&
                        a.CHA_TIPO_IE.Equals("I") &&
                        a.CHA_TIPO_CORR.Equals("S"));
                }

                if (objQueryCorrispondente.descrizioneUO != null)
                {
                    var uoList = objQueryCorrispondente.descrizioneUO.Split(';').ToList();

                    codIfNoDescPred = pepPred.And(a =>
                        a.CHA_TIPO_URP != null &&
                        a.CHA_TIPO_IE != null &&
                        a.CHA_TIPO_CORR != null &&
                        a.CHA_TIPO_URP.Equals("U") &&
                        a.CHA_TIPO_IE.Equals("I") &&
                        a.CHA_TIPO_CORR.Equals("S"));

                    uoList.ForEach(uo =>
                    {
                        uoListPred = uoListPred.Or(a => a.VAR_DESC_CORR != null && uo.ToUpper().Contains(a.VAR_DESC_CORR.ToUpper()));
                    });

                    if (uoList.Count > 0)
                    {
                        codIfNoDescPred = codIfNoDescPred.And(uoListPred);
                    }
                }

                pepPred = pepPred.And(codIfNoDescPred);
                bQuery3 = bQuery3.Where(pepPred);



                if (objQueryCorrispondente.isRuoloDefined())
                {
                    selectedAVarCog = true;

                    bQuery2 = _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Join(_dbContext.TipoRuoloEntities.AsNoTracking(),
                                a => a.ID_TIPO_RUOLO,
                                b => b.SYSTEM_ID,
                                (a, b) => new { a, b })
                            .Where(j => j.a.VAR_DESC_CORR != null &&
                                   j.a.VAR_DESC_CORR.ToUpper().Contains(objQueryCorrispondente.descrizioneRuolo.ToUpper()) &&
                                   j.a.CHA_TIPO_URP != null &&
                                   j.a.CHA_TIPO_URP.Equals("R") &&
                                   j.a.CHA_TIPO_IE != null &&
                                   j.a.CHA_TIPO_IE.Equals("I") &&
                                   j.a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong())
                            .Select(j => new CorrConTipoRuolo()
                            {
                                A = j.a,
                                B = j.b
                            });

                    if (objQueryCorrispondente.fineValidita)
                    {
                        bQuery2 = bQuery2.Where(a => !a.A.DTA_FINE.HasValue);
                    }

                    querySelector = 2;
                }
                if (objQueryCorrispondente.isUtenteDefined())
                {
                    var pred = PredicateBuilder.New<InfoCorr>();
                    var regs = objQueryCorrispondente.idRegistri.ToList();

                    pred = pred.And(a => (a.A.ID_AMM == null || (a.A.ID_REGISTRO == null && a.A.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong())) || ((a.A.ID_REGISTRO != null && regs.Contains(a.A.ID_REGISTRO.ToString()))));


                    selectedEVarCog = true;

                    bQuery1 = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                           .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(),
                               a => a.ID_PEOPLE,
                               b => b.PEOPLE_SYSTEM_ID,
                               (a, b) => new { a, b })
                           .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                               j => j.b.GROUPS_SYSTEM_ID,
                               c => c.ID_GRUPPO,
                               (j, c) => new { j.a, j.b, c })
                           .Join(this._dbContext.TipoRuoloEntities.AsNoTracking(),
                               j => j.c.ID_TIPO_RUOLO,
                               d => d.SYSTEM_ID,
                               (j, d) => new { j.a, j.b, j.c, d })
                           .Join(this._dbContext.PeopleEntities.AsNoTracking(),
                               j => j.a.ID_PEOPLE,
                               e => e.SYSTEM_ID,
                               (j, e) => new { j.a, j.b, j.c, j.d, e })
                           .Select(j => new InfoCorr()
                           {
                               A = j.a,
                               B = j.b,
                               C = j.c,
                               D = j.d,
                               E = new PeopleEntity()
                               {
                                   VAR_NOME = j.e.VAR_NOME,
                                   VAR_COGNOME = j.e.VAR_COGNOME,
                                   EMAIL_ADDRESS = j.e.EMAIL_ADDRESS,
                                   CHA_NOTIFICA = j.e.CHA_NOTIFICA,
                                   VAR_TELEFONO = j.e.VAR_TELEFONO,
                                   CHA_NOTIFICA_CON_ALLEGATO = j.e.CHA_NOTIFICA_CON_ALLEGATO,
                                   DISABLED = j.e.DISABLED,
                                   VAR_SEDE = j.e.VAR_SEDE
                               }
                           });

                    querySelector = 1;

                    if (objQueryCorrispondente.fineValidita)
                    {
                        pred = pred.And(a => !a.B.DTA_FINE.HasValue);
                    }

                    if (objQueryCorrispondente.nomeUtente != null)
                    {
                        pred = pred.And(a => a.E.VAR_NOME != null && a.E.VAR_NOME.ToUpper().StartsWith(objQueryCorrispondente.nomeUtente.ToUpper()) &&
                        a.A.CHA_TIPO_URP != null &&
                        a.A.CHA_TIPO_URP.Equals("P") &&
                        a.A.CHA_TIPO_IE != null &&
                        a.A.CHA_TIPO_IE.Equals("I") &&
                        a.A.CHA_TIPO_CORR != null &&
                        a.A.CHA_TIPO_CORR.Equals("S")
                        );
                    }
                    else
                    {
                        pred = pred.And(a => a.E.VAR_COGNOME != null && a.E.VAR_NOME.ToUpper().StartsWith(objQueryCorrispondente.cognomeUtente.ToUpper().Replace("'", "''")) &&
                        a.A.CHA_TIPO_URP != null &&
                        a.A.CHA_TIPO_URP.Equals("P") &&
                        a.A.CHA_TIPO_IE != null &&
                        a.A.CHA_TIPO_IE.Equals("I") &&
                        a.A.CHA_TIPO_CORR != null &&
                        a.A.CHA_TIPO_CORR.Equals("S")
                        );
                    }

                    if (objQueryCorrispondente.isRuoloDefined())
                    {
                        pred = pred.And(a => a.D.VAR_DESC_RUOLO != null &&
                        a.D.VAR_DESC_RUOLO.ToUpper().Contains(objQueryCorrispondente.descrizioneRuolo.ToUpper().Replace("'", "''")));
                    }
                    bQuery1 = bQuery1.Where(pred);
                }
            }

            HashSet<string> sysIdFound = new();
            List<DocsPaVO.utente.Corrispondente> output = new();

            switch (querySelector)
            {
                case 1:
                    var resultbQuery1 = await bQuery1.Distinct()
                                        .OrderBy(a => a.A.VAR_DESC_CORR)
                                        .ToListAsync();

                    foreach (var row in resultbQuery1)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.A.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.A.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.A.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.A.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.A.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;
                                    if (row.A.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.A.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.A.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.A.CHA_PA != null ? row.A.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.A.NUM_LIVELLO != null ? row.A.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.A.VAR_SMTP;
                                    sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.A.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.A.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.A.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.A.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "I";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.A.ID_PARENT != null
                                        && !row.A.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.A.ID_PARENT.ToString(), dataSet);
                                    }

                                    output.Add(corrispondenteUO);
                                    sysIdFound.Add(row.A.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.A.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.A.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.A.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.A.ID_AMM != null ? row.A.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.A.ID_GRUPPO != null ? row.A.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.A.VAR_CODICE_AOO != null ? row.A.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.A.VAR_CODICE_AMM != null ? row.A.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP;
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "I";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                if (objQueryCorrispondente.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO.ToString(), dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO != null ? row.A.ID_UO.ToString() : null, dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "I";
                            }
                            if (row.A.CHA_TIPO_URP.Equals("P"))
                            {
                                DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente();
                                corrispondenteUtente.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteUtente.idPeople = row.A.ID_PEOPLE.ToString();
                                corrispondenteUtente.descrizione = row.A.VAR_DESC_CORR.ToString();
                                corrispondenteUtente.codiceCorrispondente = row.A.VAR_CODICE.ToString();
                                corrispondenteUtente.codiceRubrica = row.A.VAR_COD_RUBRICA.ToString();
                                corrispondenteUtente.dta_fine = row.A.DTA_FINE.ToString();
                                corrispondenteUtente.disabilitato = row.E.DISABLED;
                                corrispondenteUtente.idAmministrazione = row.A.ID_AMM.ToString();
                                corrispondenteUtente.codiceAOO = row.A.VAR_CODICE_AOO ?? string.Empty;
                                corrispondenteUtente.codiceAmm = row.A.VAR_CODICE_AMM ?? string.Empty;
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteUtente.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteUtente.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP ?? string.Empty;
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteUtente.serverPosta = sp;
                                corrispondenteUtente.email = row.E.EMAIL_ADDRESS;
                                corrispondenteUtente.notifica = row.E.CHA_NOTIFICA;
                                corrispondenteUtente.telefono = row.E.VAR_TELEFONO;
                                corrispondenteUtente.tipoIE = "I";
                                corrispondenteUtente.tipoCorrispondente = "P";
                                corrispondenteUtente.notificaConAllegato = row.E.CHA_NOTIFICA_CON_ALLEGATO != null ? row.E.CHA_NOTIFICA_CON_ALLEGATO.Equals("1") : false;

                                if (row.E.VAR_SEDE != null)
                                {
                                    corrispondenteUtente.sede = row.E.VAR_SEDE;
                                }

                                DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo();
                                ruoloUtente.systemId = row.C.SYSTEM_ID.ToString();
                                ruoloUtente.descrizione = row.D.VAR_DESC_RUOLO;
                                ruoloUtente.codiceCorrispondente = row.C.VAR_CODICE;
                                ruoloUtente.codiceRubrica = row.C.VAR_COD_RUBRICA;
                                ruoloUtente.dettagli = row.C.CHA_DETTAGLI != null ? row.C.CHA_DETTAGLI.Equals("1") : false;

                                if (objQueryCorrispondente.isUODefined())
                                {
                                    if ((objQueryCorrispondente.descrizioneUO != null && HasDefinedUo(objQueryCorrispondente.descrizioneUO, 1, row.C.ID_UO?.ToString(), dataSet)) || (objQueryCorrispondente.codiceUO != null && HasDefinedUo(objQueryCorrispondente.codiceUO, 2, row.C.ID_UO?.ToString(), dataSet)))
                                    {
                                        ruoloUtente.uo = GetParents(row.C.ID_UO?.ToString(), dataSet);

                                        List<Ruolo> ruoli = new();
                                        ruoli.Add(ruoloUtente);
                                        corrispondenteUtente.ruoli = ruoli.ToArray();
                                        output.Add(corrispondenteUtente);
                                    }
                                }
                                else
                                {
                                    ruoloUtente.uo = GetParents(row.C.ID_UO?.ToString(), dataSet);
                                    List<Ruolo> ruoli = new();
                                    ruoli.Add(ruoloUtente);
                                    corrispondenteUtente.ruoli = ruoli.ToArray();
                                    output.Add(corrispondenteUtente);
                                }

                            }

                        }
                    }

                    break;
                case 2:
                    bQuery2 = bQuery2.OrderBy(a => a.A.VAR_COGNOME);

                    var resultbQuery2 = await bQuery2.ToListAsync();
                    foreach (var row in resultbQuery2)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.A.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.A.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.A.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.A.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.A.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;
                                    if (row.A.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.A.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.A.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.A.CHA_PA != null ? row.A.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.A.NUM_LIVELLO != null ? row.A.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.A.VAR_SMTP;
                                    sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.A.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.A.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.A.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.A.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "I";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.A.ID_PARENT != null
                                        && !row.A.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.A.ID_PARENT.ToString(), dataSet);
                                    }

                                    output.Add(corrispondenteUO);
                                    sysIdFound.Add(row.A.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.A.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.A.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.A.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.A.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.A.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.A.DTA_FINE != null ? row.A.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.A.ID_AMM != null ? row.A.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.A.ID_GRUPPO != null ? row.A.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.A.VAR_CODICE_AOO != null ? row.A.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.A.VAR_CODICE_AMM != null ? row.A.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.A.VAR_SMTP;
                                sp.portaSMTP = row.A.NUM_PORTA_SMTP != null ? row.A.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "I";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.A.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.A.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.A.CHA_DETTAGLI != null ? row.A.CHA_DETTAGLI.Equals("1") : false;
                                if (objQueryCorrispondente.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO.ToString(), dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.A.ID_UO != null ? row.A.ID_UO.ToString() : null, dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "I";
                            }
                        }
                    }

                    break;
                case 3:
                    var resultbQuery3 = await bQuery3.ToListAsync();
                    foreach (var row in resultbQuery3)
                    {
                        var dataSet = await this.RicercaUOParentInt(row);

                        if (dataSet != null && dataSet.Count > 0)
                        {
                            if (row.CHA_TIPO_URP.Equals("U"))
                            {
                                bool rowAlreadyFound = sysIdFound.Contains(row.SYSTEM_ID.ToString());
                                if (!rowAlreadyFound)
                                {
                                    DocsPaVO.utente.UnitaOrganizzativa corrispondenteUO = new DocsPaVO.utente.UnitaOrganizzativa();
                                    corrispondenteUO.systemId = row.SYSTEM_ID.ToString();
                                    corrispondenteUO.descrizione = row.VAR_DESC_CORR;
                                    corrispondenteUO.codiceCorrispondente = row.VAR_CODICE;
                                    corrispondenteUO.codiceRubrica = row.VAR_COD_RUBRICA;
                                    corrispondenteUO.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;
                                    if (row.ID_REGISTRO != null)
                                    {
                                        corrispondenteUO.idRegistro = row.ID_REGISTRO.ToString();
                                    }
                                    corrispondenteUO.email = row.VAR_EMAIL;
                                    corrispondenteUO.interoperante = row.CHA_PA != null ? row.CHA_PA.Equals("1") : false;
                                    corrispondenteUO.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;
                                    corrispondenteUO.livello = row.NUM_LIVELLO != null ? row.NUM_LIVELLO.ToString() : string.Empty;
                                    DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                    sp.serverSMTP = row.VAR_SMTP;
                                    sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                                    corrispondenteUO.serverPosta = sp;
                                    corrispondenteUO.idAmministrazione = row.ID_AMM.ToString();
                                    corrispondenteUO.codiceAOO = row.VAR_CODICE_AOO;
                                    corrispondenteUO.codiceAmm = row.VAR_CODICE_AMM;
                                    corrispondenteUO.codiceIstat = row.VAR_CODICE_ISTAT;
                                    corrispondenteUO.tipoIE = "I";
                                    corrispondenteUO.tipoCorrispondente = "U";
                                    //qui si ritrova la parentela
                                    if (row.ID_PARENT != null
                                        && !row.ID_PARENT.ToString().Equals("0"))
                                    {
                                        corrispondenteUO.parent = GetParents(row.ID_PARENT.ToString(), dataSet);
                                    }

                                    output.Add(corrispondenteUO);
                                    sysIdFound.Add(row.SYSTEM_ID.ToString());
                                }
                            }
                            if (row.CHA_TIPO_URP.Equals("R"))
                            {
                                DocsPaVO.utente.Ruolo corrispondenteRuolo = new DocsPaVO.utente.Ruolo();
                                corrispondenteRuolo.systemId = row.SYSTEM_ID.ToString();
                                corrispondenteRuolo.descrizione = row.VAR_DESC_CORR;
                                corrispondenteRuolo.codiceCorrispondente = row.VAR_CODICE;
                                corrispondenteRuolo.codiceRubrica = row.VAR_COD_RUBRICA;
                                corrispondenteRuolo.dta_fine = row.DTA_FINE != null ? row.DTA_FINE.AsDateFormat() : null;

                                corrispondenteRuolo.idAmministrazione = row.ID_AMM != null ? row.ID_AMM.ToString() : null;
                                corrispondenteRuolo.idGruppo = row.ID_GRUPPO != null ? row.ID_GRUPPO.ToString() : null;
                                corrispondenteRuolo.codiceAOO = row.VAR_CODICE_AOO != null ? row.VAR_CODICE_AOO.ToString() : null;
                                corrispondenteRuolo.codiceAmm = row.VAR_CODICE_AMM != null ? row.VAR_CODICE_AMM.ToString() : null;
                                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                                sp.serverSMTP = row.VAR_SMTP;
                                sp.portaSMTP = row.NUM_PORTA_SMTP != null ? row.NUM_PORTA_SMTP.ToString() : null;
                                corrispondenteRuolo.serverPosta = sp;
                                corrispondenteRuolo.tipoIE = "I";
                                corrispondenteRuolo.tipoCorrispondente = "R";
                                if (row.ID_REGISTRO != null)
                                {
                                    corrispondenteRuolo.idRegistro = row.ID_REGISTRO.ToString();
                                }
                                corrispondenteRuolo.dettagli = row.CHA_DETTAGLI != null ? row.CHA_DETTAGLI.Equals("1") : false;
                                if (objQueryCorrispondente.isUODefined())
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.ID_UO.ToString(), dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                else
                                {
                                    corrispondenteRuolo.uo = this.GetParents(row.ID_UO != null ? row.ID_UO.ToString() : null, dataSet);
                                    output.Add(corrispondenteRuolo);
                                }
                                if (corrispondenteRuolo.uo != null)
                                    corrispondenteRuolo.uo.tipoIE = "I";
                            }


                        }
                    }
                    break;
            }



            return output;
        }


        private bool HasDefinedUo(string val, int type, string idParent, List<CorrGlobaliEntity> dt)
        {
            bool ret = false;
            var parentRow = dt.FirstOrDefault(p => p.SYSTEM_ID == idParent.AsLong());
            if ((type == 1 && val.Contains(parentRow.VAR_DESC_CORR)) || (type == 2 && parentRow.VAR_CODICE != null && parentRow.VAR_CODICE.ToUpper().Equals(val.ToUpper())))
            {
                return true;
            }
            else
            {
                if (parentRow.ID_PARENT != 0)
                {
                    ret = HasDefinedUo(val, type, parentRow.ID_PARENT?.ToString(), dt);
                }
            }
            return ret;
        }




        private UnitaOrganizzativa GetParents(string idParent, List<CorrGlobaliEntity> dt)
        {
            DocsPaVO.utente.UnitaOrganizzativa parent = new DocsPaVO.utente.UnitaOrganizzativa();
            var par = dt.FirstOrDefault(a => a.SYSTEM_ID == idParent.AsLong());

            if (par == null)
                return null;

            parent.systemId = par.SYSTEM_ID.ToString();
            parent.descrizione = par.VAR_DESC_CORR;
            parent.codiceCorrispondente = par.VAR_CODICE;
            parent.codiceRubrica = par.VAR_COD_RUBRICA;
            parent.livello = par.NUM_LIVELLO != null ? par.NUM_LIVELLO.ToString() : null;
            parent.codiceAOO = par.VAR_CODICE_AOO;
            parent.codiceAmm = par.VAR_CODICE_AMM;
            parent.codiceIstat = par.VAR_CODICE_ISTAT;
            parent.idAmministrazione = par.ID_AMM.ToString();
            parent.dettagli = !string.IsNullOrEmpty(par.CHA_DETTAGLI) ? par.CHA_DETTAGLI.Equals("1") : false;
            parent.serverPosta = new DocsPaVO.utente.ServerPosta()
            {
                serverSMTP = par.VAR_SMTP,
                portaSMTP = par.NUM_PORTA_SMTP != null ? par.NUM_PORTA_SMTP.ToString() : string.Empty
            };
            if (par.ID_REGISTRO != null)
            {
                parent.idRegistro = par.ID_REGISTRO.ToString();
            }
            parent.email = par.VAR_EMAIL;
            parent.interoperante = par.CHA_PA == "1";

            if (par.ID_PARENT > 0)
            {
                parent.parent = GetParents(par.ID_PARENT.ToString(), dt);
            }

            return parent;
        }

        private ExpressionStarter<CorrGlobaliEntity> GetRegPredicate(DocsPaVO.addressbook.QueryCorrispondente objQueryCorrispondente)
        {
            var predicate = PredicateBuilder.New<CorrGlobaliEntity>();
            if (objQueryCorrispondente.idRegistri != null)
            {
                var regs = objQueryCorrispondente.idRegistri.ToList();
                predicate = predicate.And(a => (a.ID_AMM == null || (a.ID_REGISTRO == null && a.ID_AMM == objQueryCorrispondente.idAmministrazione.AsLong())) || ((a.ID_REGISTRO != null && regs.Contains(a.ID_REGISTRO.ToString()))));
            }
            else
            {
                predicate = predicate.And(a => true);
            }
            return predicate;
        }



        private class InfoCorr
        {
            public CorrGlobaliEntity? A { get; set; }
            public PeopleGroupEntity? B { get; set; }
            public CorrGlobaliEntity? C { get; set; }
            public TipoRuoloEntity? D { get; set; }
            public PeopleEntity? E { get; set; }


        }


        private class CorrConTipoRuolo
        {

            public CorrGlobaliEntity? A { get; set; }
            public TipoRuoloEntity? B { get; set; }

        }

        private async Task<List<CorrGlobaliEntity>> RicercaUOParentInt(CorrispondenteResultEntity corrispondente)
        {
            var parentBq = this._dbContext.CorrGlobaliEntities.AsNoTracking().
                         Where(a => a.ID_AMM == corrispondente.ID_AMM &&
                                    a.CHA_TIPO_IE == "I" &&
                                    a.CHA_TIPO_URP == "U" &&
                                    !a.DTA_FINE.HasValue);

            (string isConnectByPrior, bool keyFound) = await this._configurationService.TryGetValue<string>("USA_CONNECTBYPRIOR_OR_WITH");
            List<CorrGlobaliEntity> output = new();
            if (keyFound && !string.IsNullOrEmpty(isConnectByPrior) && isConnectByPrior.Equals("1"))
            {

                if (corrispondente.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = await this.GetParent(corrispondente.SYSTEM_ID, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrispondente.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = await this.GetParent(corrispondente.ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrispondente.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = await this.GetParent(corrispondente.RUOLO_ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }
            }
            else
            {
                if (corrispondente.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  a.NUM_LIVELLO < corrispondente.NUM_LIVELLO && a.ID_AMM == corrispondente.ID_AMM
                                  select a);

                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrispondente.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking() 
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrispondente.ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrispondente.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                };

                if (corrispondente.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrispondente.RUOLO_ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrispondente.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                }
            }

            return output;
        }

        private async Task<List<CorrGlobaliEntity>> RicercaUOParentInt(InfoCorr corrGlobaliEntity)
        {
            var parentBq = this._dbContext.CorrGlobaliEntities.AsNoTracking().
                Where(a => a.ID_AMM == corrGlobaliEntity.A.ID_AMM && (a.CHA_TIPO_IE != null ? a.CHA_TIPO_IE.Equals("I") : false) &&
                (a.CHA_TIPO_URP != null ? a.CHA_TIPO_URP.Equals("U") : false) &&
                !a.DTA_FINE.HasValue);

            (string isConnectByPrior, bool keyFound) = await this._configurationService.TryGetValue<string>("USA_CONNECTBYPRIOR_OR_WITH");
            List<CorrGlobaliEntity> output = new();
            if (keyFound && !string.IsNullOrEmpty(isConnectByPrior) && isConnectByPrior.Equals("1"))
            {

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.A.SYSTEM_ID, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.A.ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.C.ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

            }
            else
            {
                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  a.NUM_LIVELLO < corrGlobaliEntity.A.NUM_LIVELLO && a.ID_AMM == corrGlobaliEntity.A.ID_AMM
                                  select a);

                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrGlobaliEntity.A.ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrGlobaliEntity.A.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                };

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrGlobaliEntity.C.ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrGlobaliEntity.A.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                }
            }

            return output;
        }

        private async Task<List<CorrGlobaliEntity>> RicercaUOParentInt(CorrConTipoRuolo corrGlobaliEntity)
        {
            var parentBq = this._dbContext.CorrGlobaliEntities.AsNoTracking().
               Where(a => a.ID_AMM == corrGlobaliEntity.A.ID_AMM && (a.CHA_TIPO_IE != null ? a.CHA_TIPO_IE.Equals("I") : false) &&
               (a.CHA_TIPO_URP != null ? a.CHA_TIPO_URP.Equals("U") : false) &&
               !a.DTA_FINE.HasValue);

            (string isConnectByPrior, bool keyFound) = await this._configurationService.TryGetValue<string>("USA_CONNECTBYPRIOR_OR_WITH");
            List<CorrGlobaliEntity> output = new();
            if (keyFound && !string.IsNullOrEmpty(isConnectByPrior) && isConnectByPrior.Equals("1"))
            {

                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.A.ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }
            }
            else
            {
                if (corrGlobaliEntity.A.CHA_TIPO_URP.Equals("R"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrGlobaliEntity.A.ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrGlobaliEntity.A.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                }
            }

            return output;
        }

        private async Task<List<CorrGlobaliEntity>> RicercaUOParentInt(CorrGlobaliEntity corrGlobaliEntity)
        {
            var parentBq = this._dbContext.CorrGlobaliEntities.AsNoTracking().
               Where(a => a.ID_AMM == corrGlobaliEntity.ID_AMM && (a.CHA_TIPO_IE != null ? a.CHA_TIPO_IE.Equals("I") : false) &&
               (a.CHA_TIPO_URP != null ? a.CHA_TIPO_URP.Equals("U") : false) &&
               !a.DTA_FINE.HasValue);

            (string isConnectByPrior, bool keyFound) = await this._configurationService.TryGetValue<string>("USA_CONNECTBYPRIOR_OR_WITH");
            List<CorrGlobaliEntity> output = new();
            if (keyFound && !string.IsNullOrEmpty(isConnectByPrior) && isConnectByPrior.Equals("1"))
            {
                if (corrGlobaliEntity.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.SYSTEM_ID, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrGlobaliEntity.CHA_TIPO_URP.Equals("R") || corrGlobaliEntity.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.ID_UO, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }


            }
            else
            {
                if (corrGlobaliEntity.CHA_TIPO_URP.Equals("U"))
                {
                    var parent = await this.GetParent(corrGlobaliEntity.SYSTEM_ID, new(), parentBq);
                    output = parent.OrderBy(a => a.VAR_DESC_CORR).ToList();
                }

                if (corrGlobaliEntity.CHA_TIPO_URP.Equals("R") || corrGlobaliEntity.CHA_TIPO_URP.Equals("P"))
                {
                    var parent = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  from b in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                  where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
                                  a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") &&
                                  b.SYSTEM_ID == corrGlobaliEntity.ID_UO &&
                                  a.NUM_LIVELLO <= b.NUM_LIVELLO && a.ID_AMM == corrGlobaliEntity.ID_AMM
                                  select a);

                    output = await parent.OrderBy(a => a.VAR_DESC_CORR).ToListAsync();
                }
            }

            return output;
        }

        private IEnumerable<T> SelectRecursive<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }


        /*
        private async Task<List<CorrGlobaliEntity>> GetParent(long? systemId, long? idAmm)
        {

            var uoEnt = await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(a => a.SYSTEM_ID == systemId);

            var baseQuery = await (this._dbContext.CorrGlobaliEntities.AsNoTracking().
                Where(a => a.ID_AMM == idAmm && (a.CHA_TIPO_IE != null ? a.CHA_TIPO_IE.Equals("I") : false) &&
                (a.CHA_TIPO_URP != null ? a.CHA_TIPO_URP.Equals("U") : false) &&
                !a.DTA_FINE.HasValue && a.SYSTEM_ID != systemId)).ToListAsync();

            var bQ = baseQuery.Prepend(uoEnt);
            var lookup = bQ.ToLookup(x => x.SYSTEM_ID);
            return SelectRecursive(lookup[systemId ?? -1], x => lookup[x.ID_PARENT ?? -1]).ToList();


        }
        */


        private async Task<List<CorrGlobaliEntity>> GetParent(long? idCorrGlob, List<CorrGlobaliEntity> parentsList, IQueryable<CorrGlobaliEntity> cors)
        {
            if (cors.Any())
            {
                var corr = cors.Where(x => x.SYSTEM_ID == idCorrGlob).FirstOrDefault();
                if (corr != null)
                {
                    parentsList.Add(corr);
                    return await GetParent(corr.ID_PARENT, parentsList, cors);
                }
            }
            return parentsList;
        }


        private DocsPaVO.addressbook.QueryCorrispondente CorrectApiciQuery(DocsPaVO.addressbook.QueryCorrispondente qco)
        {
            DocsPaVO.addressbook.QueryCorrispondente res = new DocsPaVO.addressbook.QueryCorrispondente();
            res.codiceGruppo = CorrectApici(qco.codiceGruppo);
            res.codiceRubrica = CorrectApici(qco.codiceRubrica);
            res.codiceRuolo = CorrectApici(qco.codiceRuolo);
            res.codiceUO = CorrectApici(qco.codiceUO);
            res.cognomeUtente = CorrectApici(qco.cognomeUtente);
            res.descrizioneGruppo = CorrectApici(qco.descrizioneGruppo);
            res.descrizioneRuolo = CorrectApici(qco.descrizioneRuolo);
            res.descrizioneUO = CorrectApici(qco.descrizioneUO);
            res.fineValidita = qco.fineValidita;
            res.getChildren = qco.getChildren;
            res.idAmministrazione = qco.idAmministrazione;
            res.idRegistri = qco.idRegistri;
            res.nomeUtente = CorrectApici(qco.nomeUtente);
            res.systemId = qco.systemId;
            res.tipoUtente = qco.tipoUtente;
            res.email = qco.email;
            return res;
        }
        private static string CorrectApici(string str)
        {
            if (str != null)
            {
                return str.Replace("'", "''");
            }
            else
            {
                return str;
            }
        }
        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetListaCorrispondentiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected class CorrispondenteResultEntity
        {
            public long SYSTEM_ID { get; set; }
            public long? ID_PEOPLE { get; set; }
            public long? ID_REGISTRO { get; set; }
            public long? ID_AMM { get; set; }
            public string? VAR_NOME { get; set; }
            public string? VAR_COGNOME { get; set; }
            public string? EMAIL_ADDRESS { get; set; }
            public string? CHA_NOTIFICA { get; set; }
            public string? VAR_TELEFONO { get; set; }
            public string? VAR_DESC_CORR { get; set; }
            public string? VAR_CODICE { get; set; }
            public string? VAR_COD_RUBRICA { get; set; }
            public string? CHA_DETTAGLI { get; set; }
            public string? CHA_TIPO_URP { get; set; }
            public string? VAR_SMTP { get; set; }
            public long? NUM_PORTA_SMTP { get; set; }
            public string? VAR_CODICE_AMM { get; set; }
            public string? VAR_CODICE_AOO { get; set; }
            public DateTime? DTA_FINE { get; set; }
            public DateTime? DTA_FINE_IN_RUOLO { get; set; }
            public long? RUOLO_SYSTEM_ID { get; set; }
            public string? RUOLO_DESC { get; set; }
            public string? RUOLO_CODICE { get; set; }
            public long? RUOLO_ID_UO { get; set; }
            public string? RUOLO_COD_RUBRICA { get; set; }
            public string? RUOLO_DETTAGLI { get; set; }
            public DateTime? RUOLO_DATA_FINE { get; set; }
            public string? CHA_NOTIFICA_CON_ALLEGATO { get; set; }
            public string? DISABLED { get; set; }
            public string? VAR_SEDE { get; set; }
            public string? VAR_DESC_RUOLO { get; set; }
            public long? NUM_LIV_B { get; set; }
            public long? NUM_LIVELLO { get; set; }
            public long? ID_GRUPPO { get; set; }
            public long? ID_UO { get; set; }
            public long? ID_PARENT { get; set; }
            public string? VAR_EMAIL { get; set; }
            public string? CHA_PA { get; set; }
            public string? VAR_CODICE_ISTAT { get; set; }
        }
        #endregion
    }
}