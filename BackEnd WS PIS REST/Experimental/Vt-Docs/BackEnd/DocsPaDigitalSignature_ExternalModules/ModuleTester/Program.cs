using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BusinessLogic.Interoperabilità;
using System.Collections;
using System.Data;
using DocsPaVO.documento;
using DocsPaVO.utente;
using BusinessLogic.Utenti;
using System.Configuration;
using DocsPaVO.areaConservazione;
using System.Security.Cryptography;
using DocsPaVO.Grid;
using DocsPaVO.ricerche;
using DocsPaVO.filtri;
using DocsPaVO.amministrazione;
using DocsPaVO.Grids;
using DocsPaVO.Mobile;
using DocsPaVO.Mobile.Responses;
using DocsPaVO.fascicolazione;
using DocsPaVO.Mobile.Requests;
using DocsPaVO.Smistamento;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Xml;
using iTextSharp.text.pdf;
using DocsPaWR = DocsPaVO.documento;


namespace ModuleTester
{
    class Program
    {
        static void prova_ref()
        {
            DocsPaVO.documento.FileRequest fr1 = new DocsPaVO.documento.FileRequest();
            DocsPaVO.utente.InfoUtente it1 = new DocsPaVO.utente.InfoUtente();
            DocsPaVO.documento.FileDocumento fd1 = BusinessLogic.Documenti.FileManager.getFile(fr1, it1);

          //  DocsPaWebService ws = new DocsPaWebService();
          //  DocsPaWR.FileRequest fr0 = new DocsPaWR.FileRequest();
          //  DocsPaWR.InfoUtente it0 = new DocsPaWR.InfoUtente();
          //  DocsPaWR.FileDocumento fd = ws.DocumentoGetFile(fr0, it0);

        }



        /// <summary>
        /// Ritorna true se in un file PDF sono presenti delle firme pades
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <returns></returns>
        private static bool IsPdfPades(DocsPaVO.documento.FileDocumento fileDoc)
        {
            try
            {
                int numSig = 0;
                iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(fileDoc.content);
                iTextSharp.text.pdf.AcroFields af = r.AcroFields;
                if (af != null)
                {
                    numSig = af.GetSignatureNames().Count;
                    if (numSig > 0)
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }





        static void addChiave()
        {
            /*
            string chiave = "BE_IDENTIFICATIVO_SUAP";
            localhost.ChiaveConfigurazione chiaveConfig = new localhost.ChiaveConfigurazione
            {
                Codice = chiave,
                IDAmministrazione = "361",
                Modificabile = "1",
                Valore = "286",
                Descrizione = "Valore identificativo SUAP"

            };
            localhost.DocsPaWebService ws = new localhost.DocsPaWebService();

            ws.addChiaveConfigurazione(chiaveConfig);*/

            /*
            string chiave = "FE_SCAMBIA_DESCRIZIONE_ALLEGATO";
            localhost.ChiaveConfigurazione chiaveConfig = new localhost.ChiaveConfigurazione
            {
                Codice = chiave,
                IDAmministrazione = "361",
                Modificabile = "1",
                Valore = "1",
                Descrizione = "Modifica descrizione allegato in fase di scambio con documento principale"

            };
            localhost.DocsPaWebService ws = new localhost.DocsPaWebService();

            ws.addChiaveConfigurazione(chiaveConfig);
            */
        }

        static void TestTSA()
        {

            DocsPaVO.areaConservazione.InputMarca richiesta = new InputMarca();

            SHA256Managed sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(System.IO.File.ReadAllBytes(@"c:\24792519.pdf.p7m"));

            string hexHash = BitConverter.ToString(hash);
            hexHash = hexHash.Replace("-", "");

            richiesta.file_p7m = hexHash;
            DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = new DocsPaVO.areaConservazione.OutputResponseMarca();
            DocsPa_TSAuthority.TSR_Request ts = new DocsPa_TSAuthority.TSR_Request();
            resultMarca = ts.getTimeStamp(richiesta);

            /*
            DocsPa_TSAuthority.TSR_Request ts = new DocsPa_TSAuthority.TSR_Request();
            InputMarca TimeStampQuery= new InputMarca ();
            TimeStampQuery.
            
            ts.getTimeStamp (
             * */

        }


        static void testPDF()
        {

           // iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(@"C:\tipi\KS-DK-07-001-EN.pdf");
            //iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(@"C:\tipi\JavaScriptClock.pdf");
            //iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(@"C:\tipi\Prova _marca.pdf");
            //string meta = System.Text.ASCIIEncoding.ASCII.GetString(r.Metadata);

        }


        public static bool isScanned(string name)
        {
            bool isGuid = false;
            if (!string.IsNullOrEmpty(name))
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}_ACQMASSIVA{0,1})$");
                isGuid = guidRegEx.IsMatch(name);
            }
            return isGuid;

        }

        public static bool IsGuid(string expression)
        {
            bool isGuid = false;
            if (!string.IsNullOrEmpty(expression))
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");
                isGuid = guidRegEx.IsMatch(expression);
            }
            return isGuid;
        }


        static FiltroRicerca[] creaFiltriADLFasc(InfoUtente infoUt, Ruolo ruolo,List<OrgTitolario> titolari)
        {
            List<FiltroRicerca> frList = new List<FiltroRicerca>();

            string listaTitolari = string.Empty;
            foreach (OrgTitolario tit  in titolari)
                listaTitolari += "," + tit.ID.ToString();
                
            listaTitolari = listaTitolari.Substring(1);

            frList.Add(new FiltroRicerca
            {
                argomento = "INCLUDI_FASCICOLI_FIGLI",
                valore = "N",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });


            frList.Add(new FiltroRicerca
            {
                argomento = "ID_TITOLARIO",
                valore =  listaTitolari ,// "7067503,96163",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "DOC_IN_FASC_ADL",
                valore = String.Format("{0}@{1}", infoUt.idPeople, ruolo.systemId),//"90249@347718",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "SOTTOFASCICOLO",
                valore = "",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_OWNER",
                valore = "False",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "EXTEND_TO_HISTORICIZED_AUTHOR",
                valore = "False",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_OWNER",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "CORR_TYPE_AUTHOR",
                valore = "R",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "ORACLE_FIELD_FOR_ORDER",
                valore = "A.DTA_CREAZIONE",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo ="P20"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "SQL_FIELD_FOR_ORDER",
                valore = "A.DTA_CREAZIONE",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "P20"
            });

            frList.Add(new FiltroRicerca
            {
                argomento = "ORDER_DIRECTION",
                valore = "DESC",
                searchTextOptions = SearchTextOptionsEnum.WholeWord,
                listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                nomeCampo = "P20"
            });


            return frList.ToArray();
        }

        static FiltroRicerca[] creaFiltriADLDoc(InfoUtente infoUt,  Ruolo ruolo)
        {

            string listaRegistri = string.Empty;
            foreach (DocsPaVO.utente.Registro reg in ruolo.registri)
            {
                if (!reg.flag_pregresso) 
                {
                    listaRegistri += ","+reg.systemId.ToString() ;
                }

            }

            listaRegistri = listaRegistri.Substring(1);

                 List<FiltroRicerca> frList = new List<FiltroRicerca>();
                 frList.Add(new FiltroRicerca
                 {
                     argomento = "PROT_ARRIVO",
                     valore = "true",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });


                 frList.Add(new FiltroRicerca
                 {
                     argomento = "PROT_PARTENZA",
                     valore = "true",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });

                 frList.Add(new FiltroRicerca
                 {
                     argomento = "PROT_INTERNO",
                     valore = "true",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });

                 frList.Add(new FiltroRicerca
                 {
                     argomento = "STAMPA_REG",
                     valore = "false",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });

                 frList.Add(new FiltroRicerca
                 {
                     argomento = "GRIGIO",
                     valore = "true",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });
              
                frList.Add(new FiltroRicerca
                 {
                     argomento = "PREDISPOSTO",
                     valore = "false",
                     searchTextOptions = SearchTextOptionsEnum.WholeWord,
                     listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                     listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                     listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                     listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                     listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                     listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                 });


                frList.Add(new FiltroRicerca
                {
                    argomento = "TIPO",
                    valore = "tipo",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });


            //QUI!!!!
                frList.Add(new FiltroRicerca
                {
                    argomento = DocsPaVO.filtri.ricerca.listaArgomenti.REGISTRO.ToString (),
                    valore =  listaRegistri, // "86107,8548943",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });


                //"90249@347718",
                frList.Add(new FiltroRicerca
                {
                    argomento =  DocsPaVO.filtri.ricerca.listaArgomenti.DOC_IN_ADL.ToString() ,
                    valore =  String.Format("{0}@{1}",infoUt.idPeople , ruolo.systemId),
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });


                frList.Add(new FiltroRicerca
                {
                    argomento = "EXTEND_TO_HISTORICIZED_OWNER",
                    valore = "false",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "EXTEND_TO_HISTORICIZED_AUTHOR",
                    valore = "false",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "CORR_TYPE_OWNER",
                    valore = "R",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "CORR_TYPE_AUTHOR",
                    valore = "R",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });


                frList.Add(new FiltroRicerca
                {
                    argomento = "ANNO_PROTOCOLLO",
                    valore = "",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO
                });


                frList.Add(new FiltroRicerca
                {
                    argomento = "ORACLE_FIELD_FOR_ORDER",
                    valore = "NVL (a.dta_proto, a.creation_time)",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                    nomeCampo ="D9"
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "SQL_FIELD_FOR_ORDER",
                    valore = "ISNULL (a.dta_proto, a.creation_time)",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                    nomeCampo ="D9"
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "ORDER_DIRECTION",
                    valore = "DESC",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                    nomeCampo ="D9"
                });

                frList.Add(new FiltroRicerca
                {
                    argomento = "DA_PROTOCOLLARE",
                    valore = "0",
                    searchTextOptions = SearchTextOptionsEnum.WholeWord,
                    listaFiltriTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomenti.NOTE_GENERALI,
                    listaFiltriModelliTrasmissione = DocsPaVO.filtri.trasmissione.listaArgomentiModelliTrasmissione.CODICE_MODELLO,
                    listaFiltriTrasmissioneNascosti = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.TIPO_OGGETTO,
                    listaFiltriDocumento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO,
                    listaFiltriFascicolo = DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL,
                    listaFiltriStampaRegistro = DocsPaVO.filtri.stampaRegistro.listaArgomenti.REGISTRO,
                });

                return frList.ToArray();
        }



        static void testMob ()
        {

            string userID = "PR34019";
            string idAmm = BusinessLogic.Utenti.UserManager.getIdAmmUtente(userID);
            Utente ut = BusinessLogic.Utenti.UserManager.getUtente(userID, idAmm);

            List<Ruolo> ru = BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople).Cast<Ruolo>().ToList();
            InfoUtente it = new InfoUtente(ut, ru.FirstOrDefault());
            FiltroRicerca[][] qv;

            qv = new FiltroRicerca[1][];
            qv[0] = creaFiltriADLDoc(it, ru.FirstOrDefault());
            List<SearchResultInfo> outList;
            int numTotPage, nRec;
            List<SearchObject> documenti = DocumentoGetQueryDocumentoPagingCustom(it, qv, 1, false, 100, out numTotPage, out nRec, false, false, false, null, null, out outList).Cast<SearchObject>().ToList();


            //ArrayList robbaF = getListaFascicoliPagingCustom(it,null,r.FirstOrDefault().registri [0],

            List<SearchResultInfo> idProjects = new List<SearchResultInfo>();

            DocsPaVO.fascicolazione.Classificazione classificazione = new DocsPaVO.fascicolazione.Classificazione();

            List<DocsPaVO.utente.Registro> regList = ru.FirstOrDefault().registri.Cast<DocsPaVO.utente.Registro>().ToList();

            // argomento = "ID_TITOLARIO",
            //valore = "7067503,96163",

            List<OrgTitolario> titolari = BusinessLogic.Amministrazione.TitolarioManager.getTitolariUtilizzabili(it.idAmministrazione).Cast<OrgTitolario>().ToList();

            List<SearchObject> fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliPagingCustom(
                it,
                null,
                regList.FirstOrDefault(),
                creaFiltriADLFasc(it, ru.FirstOrDefault(), titolari),
                false, true, false, out numTotPage, out  nRec, 1, 20, false, out idProjects, null, string.Empty, false, false, null, null, true).Cast<SearchObject>().ToList();






            List<RicercaElement> element = new List<RicercaElement>();

            foreach (SearchObject d in documenti)
            {
                string tipodoc = (from n in d.SearchObjectField where n.SearchObjectFieldID == "D3" select n.SearchObjectFieldValue).FirstOrDefault().ToString();
                string oggettodoc = (from n in d.SearchObjectField where n.SearchObjectFieldID == "D4" select n.SearchObjectFieldValue).FirstOrDefault().ToString();

                RicercaElement r = RicercaElement.buildInstance(new InfoDocumento { idProfile = d.SearchObjectID, oggetto = oggettodoc, tipoProto = tipodoc });
                element.Add(r);
            }



            foreach (SearchObject f in fascicoli)
            {
                string descrizionefasc = (from n in f.SearchObjectField where n.SearchObjectFieldID == "P4" select n.SearchObjectFieldValue).FirstOrDefault().ToString();
                string notefasc = (from n in f.SearchObjectField where n.SearchObjectFieldID == "P8" select n.SearchObjectFieldValue).FirstOrDefault().ToString();

                List<DocsPaVO.Note.InfoNota> noteFascLst = new List<DocsPaVO.Note.InfoNota>();
                noteFascLst.Add(new DocsPaVO.Note.InfoNota { Testo = notefasc });

                RicercaElement r = RicercaElement.buildInstance(new Fascicolo { systemID = f.SearchObjectID, descrizione = descrizionefasc, noteFascicolo = noteFascLst.ToArray() });
                element.Add(r);
            }


            RicercaResponse retval = new RicercaResponse();
            retval.Risultati = element;
            retval.TotalRecordCount = element.Count();
            retval.Code = RicercaResponseCode.OK;

            return;
        }


        static void bcplay()
        {

            DocsPaVO.DiagrammaStato.Stato workflowState = null;
            if (workflowState != null && workflowState.STATO_FINALE)
            {
               
            }



            byte[] data = System.IO.File.ReadAllBytes(@"C:\tipi\To_sign.txt");
            //Org.BouncyCastle.Cms.CmsSignedData sigDataInput = new Org.BouncyCastle.Cms.CmsSignedData(data);
        }

        public static string Truncate( string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }


        static string getExts(string infile)
        {
            string fname = System.IO.Path.GetFileNameWithoutExtension(infile);
            
            if (fname .Contains('.'))
                fname = fname.Remove (fname.IndexOf ('.'));

            string extname = infile.Replace(fname, string.Empty);
            return extname;
        }


        public static string getEstensioneIntoP7M( string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") || 
                    items[i].ToUpper().EndsWith("TSD") || 
                    items[i].ToUpper().EndsWith("M7M")))
                {
                    retValue = items[i];
                    break;
                }
            }

            return retValue;

            //string estensione = fullname.Substring(fullname.IndexOf(".") + 1);
            //if (estensione.ToUpper().EndsWith("P7M"))
            //    estensione = estensione.Substring(0, estensione.IndexOf("."));
            //return estensione;							

        }

        static void provaScarico()
        {
         //   string docNum = "12753140";// "12702440";//"12673880";
            //string docNum = "12753241"; //p7m revocato + timestamp
            string docNum = "12784106";// "12755350"; // "12753301"; //tsd
            string userID = "PR34019";
            Utente ut = BusinessLogic.Utenti.UserManager.getUtente(userID, BusinessLogic.Utenti.UserManager.getIdAmmUtente(userID));
            InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(ut, (Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople)[0]);
            SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNum, docNum);
            string versione = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNum, infoUtente);
            //DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest { docNumber = docNum, versionId = versione };
            DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sd.documenti[0];


            var pippo = BusinessLogic.Documenti.FileManager.getFileFileInformation(fr, infoUtente);
            FileDocumento fileDOC = BusinessLogic.Documenti.FileManager.getFile (fr, infoUtente);
            //BusinessLogic.Documenti.FileManager.processFileInformation (fr, infoUtente);
            //BusinessLogic.Documenti.FileManager.processFileInformationCRL(fr, infoUtente);

            //FileDocumento fileDOC = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, false);
        }
        static void provaxsd()
        {
            string xsdFile = @"c:\Sviluppo\Vt-Docs\DocsPaWS\" + "XML/SUAP-ente-1.0.1.xsd"; // "XML/pratica_suap-1.0.1.xsd";
            string xml = File.ReadAllText(@"C:\temp\ENTESUAP.XML");
            if (File.Exists(xsdFile))
            {
                BusinessLogic.report.ProtoASL.ReportXML.XmlValidator xmlV = new BusinessLogic.report.ProtoASL.ReportXML.XmlValidator();
                bool esito = xmlV.ValidateXmlString(xml, xsdFile, null);

                   
            }

        }

  

        static void provaXML()
        {
            //BusinessLogic.XmlParsing.suap.SuapManager s = new BusinessLogic.XmlParsing.suap.SuapManager("SUAPENTE");
            //s.ImportSuapEnteXMLIntoTemplate (null,null,null,File.ReadAllBytes (@"C:\Users\user\Desktop\suap\FioroniEmail\PDRFNC88D19C794E-27092013-0926.SUAP.XML"));


            string unf = File.ReadAllText(@"C:\Users\user\Desktop\suap\ENTESUAP_v1.XML");
            string pippolo = "";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(unf);
            // Save the document to a file and auto-indent the output.
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);

                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        ms.Position = 0;
                        pippolo = sr.ReadToEnd();
                    }
                }
            }
            

            
            
        }


        static void pdfTesting()
        {

            byte[] src = File.ReadAllBytes(@"C:\Tipi\TESTBASIC1.pdf");

            string pathToTimestampedPdf = @"C:\out.pdf";
            PdfReader reader = new PdfReader(src);
            PdfStamper pdfStamper = PdfStamper.CreateSignature (reader, new FileStream(pathToTimestampedPdf, FileMode.Create, FileAccess.Write, FileShare.None),'\0',null,true);

            System.Drawing.Image bitmap = new System.Drawing.Bitmap(@"C:\tipi\skizzo.bmp",true);

            MemoryStream imageStream = new MemoryStream();
            bitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bitmapBytes = imageStream.ToArray();

            iTextSharp.text.Image image =  iTextSharp.text.Image.GetInstance(bitmapBytes);

            PdfContentByte underContent;
            image.SetAbsolutePosition(0, 0);
            underContent = pdfStamper.GetUnderContent(1);
            underContent.AddImage(image);
            pdfStamper.Close();

        }
        /*
        static void testhsm()
        {
            string alias="CZZMHL73B49H612K";
            string dominio="INFOCERT";
            string pin="12345678";
            hsb.HSMService s = new hsb.HSMService();
            string ret = s.GetCertificatoHSM (alias,dominio);


            bool OTPResult = s.RichiediOTP(alias, dominio);
            byte[] file = System.IO.File.ReadAllBytes (@"C:\tipi\lilllo.pdf");
            string OTP = Console.ReadLine();

            byte[] outfile = s.FirmaFileCADES(file, alias, dominio, pin, OTP, true, false);
            //byte[] outfile = s.FirmaFilePADES(file, alias, dominio, pin, OTP, true);
            File.WriteAllBytes (@"C:\tipi\hsm\lilllo.pdf",outfile);
        }

        static void testhsmMulti()
        {
            byte[] file = null;
            string alias = "CZZMHL73B49H612K";
            string dominio = "INFOCERT";
            string pin = "12345678";
            hsb.HSMService s = new hsb.HSMService();
            string sessionToken = s.Session_OpenMultiSign(false, false, hsb.SignType.PADES);

            file = System.IO.File.ReadAllBytes(@"C:\tipi\lilllo.pdf");
            string f1 = s.Session_PutFileToSign(sessionToken, file, "lilllo.pdf");

            file = System.IO.File.ReadAllBytes(@"C:\tipi\Pippo_scannerizzato.pdf");
            string f2 = s.Session_PutFileToSign(sessionToken, file, "Pippo_scannerizzato.pdf");

            file = System.IO.File.ReadAllBytes(@"C:\tipi\ali_tangent.txt");
            string f3 = s.Session_PutFileToSign(sessionToken, file, "ali_tangent.txt");

            bool OTPResult = s.RichiediOTP(alias, dominio);
            string OTP = Console.ReadLine();

            bool resSig = s.Session_RemoteSign(sessionToken, alias, dominio, pin, OTP );
            if (resSig)
            {
                byte[] outfile1 = s.Session_GetSignedFile(sessionToken, f1);
                byte[] outfile2 = s.Session_GetSignedFile(sessionToken, f2);
                byte[] outfile3 = s.Session_GetSignedFile(sessionToken, f3);

                File.WriteAllBytes(@"C:\tipi\hsm\multi_PADES\MULTISIGN_1.pdf.p7m", outfile1);
                File.WriteAllBytes(@"C:\tipi\hsm\multi_PADES\MULTISIGN_2.pdf.p7m", outfile2);
                File.WriteAllBytes(@"C:\tipi\hsm\multi_PADES\MULTISIGN_2.txt.p7m", outfile3);
            }
            s.Session_CloseMultiSign(sessionToken);
        }


        */


        static void GenKey(int len=128)
        {
            
            
            byte[] buff = new byte[len / 2];
            RNGCryptoServiceProvider rng = new
                                    RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            StringBuilder sb = new StringBuilder(len);
            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));
            Console.WriteLine(sb);
        }

        static void provaActa()
        {
            //prova_acta.ClrVerification cl = new prova_acta.ClrVerification();
            //byte[] xml = File.ReadAllBytes(@"C:\tipi\ENAC_invio cartaceo 112013ter.pdf");

            //cl.VerificaCertificato(xml, DateTime.Now, false);
        }
        static void provaStamp()
        {
            string docNum = "14244412";// "12702440";//"12673880";
            string userID = "PR34019";

            Utente ut = BusinessLogic.Utenti.UserManager.getUtente(userID, BusinessLogic.Utenti.UserManager.getIdAmmUtente(userID));
            InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(ut, (Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople)[0]);
            SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNum, docNum);
            DocsPaVO.documento.FileRequest fr =(DocsPaVO.documento.FileRequest ) sd.documenti[0];
            bool retValue = BusinessLogic.Documenti.FileManager.RemotePdfStamp(infoUtente, fr, "ENAC-GIG-13/12/2013-0014832-P");

        }

        static void Main(string[] args)
        {

            provaActa();
            return;
            provaStamp();
            return;

            FileInformation f = new FileInformation();
            

            //testhsmMulti();
            //return;
            //pdfTesting();
            

            //provaxsd();
          
           /*
            WebClient webClient = new WebClient();
            byte []cr =System.IO.File.ReadAllBytes(@"C:\temp\cert");
            webClient.UploadData("http://localhost/codapdf/pippo.txt", "PUT", cr);
            */
            addChiave();

            //creaSuap();
            provatipo();       


            return;

            DocsPaDB.Query_DocsPAWS.Documenti docsss = new DocsPaDB.Query_DocsPAWS.Documenti();
            List<DocsPaVO.documento.FileRequest> frqs =  docsss.GetFileInfosToProcess(100);

            foreach (DocsPaVO.documento.FileRequest fr in frqs)
            {
                if (fr.docNumber == "12764622")
                {
                    InfoUtente iut = new InfoUtente { idAmministrazione = fr.versionLabel , idPeople = fr.idPeople, idGruppo = fr.idPeopleDelegato};
                    
                    BusinessLogic.Documenti.FileManager.processFileInformationCRL(fr, iut);
                }
            }


            provaScarico();
           
            return;
            //string xml = File.ReadAllText(@"C:\Adobe_PDF_per_la_Firma_digitale-signed.pdf.p7m.tsd.xml");
       
            

            string ext = getEstensioneIntoP7M("pippo.pdf.p7m.tsd.m7m");
            
            BusinessLogic.Documenti.DigitalSignature.Helpers.sbustaFileFirmato(File.ReadAllBytes(@"C:\Tipi\marcati e firmati\modulo-signed.pdf")); ;

            string docNum = "12707268";// "12702440";//"12673880";
            string userID= "PR34019";

            Utente ut =BusinessLogic.Utenti.UserManager.getUtente (userID,BusinessLogic.Utenti.UserManager.getIdAmmUtente (userID));
            InfoUtente infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente (ut,(Ruolo) BusinessLogic.Utenti.UserManager.getRuoliUtente (ut.idPeople)[0]);
            SchedaDocumento sd=  BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNum, docNum);

            FileDocumento fild = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest) sd.documenti[0], infoUtente);

            string attachmentName = "c:\\tipi\\1maggio_detached.pdf";
            FileDocumento fd = new FileDocumento { content = File.ReadAllBytes(attachmentName) };
            IsPdfPades(fd);

            return;
            bcplay();
         //   PrendiILDoc();
            return;

        

            //BusinessLogic.Utenti.UserManager.getUtente("1 or CHA_AMMINISTRATORE='1'");
            byte[] p7mFile = System.IO.File.ReadAllBytes(@"C:\tipi\11745338.pdf.P7M");
            SignatureVerify.Verifica v = new SignatureVerify.Verifica();
            Object[] arg = { DateTime.Parse("2011-07-01T00:00:00+00:00"), "C:\\tipi\\cache\\", "false" };
            string fileVerifica = v.VerificaByte(p7mFile, null, arg);
            //return;


            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd tsdConv = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd();
            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.m7m m7mConv = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.m7m();


            //List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile> tsrFiles = new List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile>();
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\Tipi\tc\compose\8604857-7207164323744861.tsr"),Name =@"C:\Tipi\tc\compose\8604857-7207164323744861.tsr" });
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\Tipi\tc\compose\8604857-7207164424669786.tsr"), Name = @"C:\Tipi\tc\compose\8604857-7207164424669786.tsr" });
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\Tipi\tc\compose\8604857-7207243203726783.tsr"), Name = @"C:\Tipi\tc\compose\8604857-7207243203726783.tsr" });
            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile pFile = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\Tipi\tc\compose\8604857.pdf.P7M") };
            //byte[] co = tsdConv.create(pFile, tsrFiles.ToArray()).Content;




            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile m7mFile = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\pro3.pdf.tsd") };
            //tsdConv.explode(m7mFile.Content);
            ////m7mConv.P7M.Content
            //byte[] p7m = tsdConv.Data.Content;
            //byte[] tsr = tsdConv.TSR[0].Content;

            //BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp checkMarca = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
            //DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = checkMarca.Verify(p7m, tsr);



            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile p7mFile = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\tipi\TC\8516909.P7M") };
            
            //tsrFiles.Add (new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content =System.IO.File.ReadAllBytes(@"C:\tipi\TC\TS8516909-123313035.tsr")});
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\tipi\TC\TS8516909-124084535.tsr") });
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\tipi\TC\TS8516909-124084578.tsr") });
            //tsrFiles.Add(new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = System.IO.File.ReadAllBytes(@"C:\tipi\TC\TS8516909-124084612.tsr") });

            
          
            //BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile  cf= tsdConv.create(p7mFile, tsrFiles.ToArray());
            
            
            //cf = m7mConv.create(p7mFile, tsrFiles.ToArray());

            //return;
            
        }


        private static bool popolaValoreTipologiaCampoTestuale(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            if (ogg != null)
            {
                ogg.VALORE_DATABASE = valore;
                return true;
            }
            return false;
        }

        private static bool popolaValoreTipologiaDropDown(DocsPaVO.ProfilazioneDinamica.Templates t, string nome, string valore)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg = trovaOggettoPerNome(t, nome);
            DocsPaVO.ProfilazioneDinamica.ValoreOggetto valoDef = null;
            foreach (DocsPaVO.ProfilazioneDinamica.ValoreOggetto valo in ogg.ELENCO_VALORI)
            {
                if (valo.VALORE.ToLower ().Equals (valore.ToLower()) &&
                    valo.ABILITATO ==1 
                    )
                {
                    if (valo.VALORE_DI_DEFAULT == "SI")
                        valoDef = valo;
                    ogg.VALORE_DATABASE = valo.VALORE;
                    return true;
                }
            }
            //popolo il default
            ogg.VALORE_DATABASE = valoDef.VALORE;
            
            return false;
        }

        
        
        private static DocsPaVO.ProfilazioneDinamica.OggettoCustom trovaOggettoPerNome(DocsPaVO.ProfilazioneDinamica.Templates t, string nome)
        {
            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in t.ELENCO_OGGETTI)
                if (ogg.DESCRIZIONE.ToLower().Equals(nome.ToLower()))
                    return ogg;

            return null;

        }




        private static DocsPaVO.ProfilazioneDinamica.Templates creaSuap()
        {




            String [] campiTipologiaSUAP = new String[] { 
                                                        "Numero Protocollo SUAP",
                                                        "Numero Pratica SUAP",
                                                        "Impresa",
                                                        "Legale rappresentante",
                                                        "Tipo intervento",
                                                        "Tipo procedimento",
                                                        "Dichiarante",
                                                        "Impianto produttivo"};


            /*
            Numero Protocollo SUAP
            Numero Pratica SUAP
            Impresa
            Legale rappresentante
            Tipo intervento (valori menu a tendina)
            Tipo procedimento (valori menu a tendina)
            Dichiarante
            Impianto produttivo
             */
            DocsPaVO.ProfilazioneDinamica.Templates templatesuap = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione("SUAPENTE");
            string numeroProtSuap = "1234";
            string numeroPratSuap = "90293029032";
            string impresa = "pippo snc";
            string legaleRapp = "paolino paperino";
            string tipoIntervento = "subentro";
            string tipoProcedimento = "SCIA";
            string Dichiarante = "PAPERON de Paperoni";
            string ImpiantoProduttivo = "via tor de schiavi roma rm 00100";


            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[0], numeroProtSuap);
            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[1], numeroPratSuap);
            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[2], impresa);
            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[3], legaleRapp);
            popolaValoreTipologiaDropDown(templatesuap, campiTipologiaSUAP[4], tipoIntervento);
            popolaValoreTipologiaDropDown(templatesuap, campiTipologiaSUAP[5], tipoProcedimento);
            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[6], Dichiarante);
            popolaValoreTipologiaCampoTestuale(templatesuap, campiTipologiaSUAP[7], ImpiantoProduttivo);
            return templatesuap;

            
        }

        private static void provatipo()
        {



         
            string docNum;
            string userID;
            Utente ut;
            InfoUtente infoUtente;
            SchedaDocumento sd;

            //docNum = "14087847";// "14084281";// "12702440";//"12673880";
            docNum = "14150250";
            userID = "PR34019";

            ut = BusinessLogic.Utenti.UserManager.getUtente(userID, BusinessLogic.Utenti.UserManager.getIdAmmUtente(userID));
            infoUtente = BusinessLogic.Utenti.UserManager.GetInfoUtente(ut, (Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople)[0]);
            sd = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, docNum, docNum);

            sd.template = creaSuap();
            sd.tipologiaAtto =  new DocsPaVO.documento.TipologiaAtto  { descrizione =  sd.template.DESCRIZIONE, systemId = sd.template.ID_TIPO_ATTO};
            sd.daAggiornareTipoAtto = true;

            bool bho;
            Ruolo ruolo= (Ruolo)BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtente.idPeople)[0];
            BusinessLogic.Documenti.DocSave.save(infoUtente, sd, false, out bho, ruolo);
        }
        /*
        private static void PrendiILDoc()
        {

            backend.DocsPaWebService ws = new backend.DocsPaWebService();


            backend.UserLogin ul = ws.VerificaUtente("PR34019");
            backend.Utente utente;
            ul.Password = "pass";
            string ipaddr;
            backend.LoginResult result;
            result = ws.Login(ul, true, "", out utente, out ipaddr);
            backend.Ruolo primoRuolo = utente.ruoli.FirstOrDefault();
            backend.InfoUtente it = new backend.InfoUtente
            {

                idPeople = utente.idPeople,
                userId = utente.userId,
                dst = utente.dst,
                idAmministrazione = utente.idAmministrazione,
                sede = utente.sede,
                urlWA = utente.urlWA,
                matricola = utente.matricola,
                extApplications = utente.extApplications,
                codWorkingApplication = utente.codWorkingApplication,
            };

            //BUG - A Volte utente arriva con idAmm nullo
            if (primoRuolo != null)
            {
                it.idCorrGlobali = primoRuolo.systemId;
                it.idGruppo = primoRuolo.idGruppo;
                if (utente != null && string.IsNullOrEmpty(utente.idAmministrazione))
                {
                    it.idAmministrazione = primoRuolo.idAmministrazione;
                }
            }

            //inps 46187
            //pitre 8605609

            backend.InfoDocumento id = ws.GetInfoDocumento(it, "12612994", "12612994");

            backend.SchedaDocumento sd = ws.DocumentoGetDettaglioDocumento(it, "12612994", "12612994");

            backend.FileRequest fr = sd.documenti[0];
            backend.FileDocumento fd = ws.DocumentoGetFile(fr, it);
            byte[] contenuto = fd.content ;


            return;
        }
        */
        static void Main1(string[] args)
        {

            //TestTSA();;
            //return;
            //testPDF();


           // mob.VTDocsWSMobileClient c = new mob.VTDocsWSMobileClient();
            

            string userID = "PR34019";
            string idAmm = BusinessLogic.Utenti.UserManager.getIdAmmUtente(userID);
            Utente ut = BusinessLogic.Utenti.UserManager.getUtente(userID, idAmm);

            List<Ruolo> ru = BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople).Cast<Ruolo>().ToList();
            InfoUtente it = new InfoUtente(ut, ru.FirstOrDefault());

            ut.ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(ut.idPeople);

            RuoloInfo ruoloInfo = RuoloInfo.buildInstance(ru.FirstOrDefault());
            UserInfo userInfo = UserInfo.buildInstance(ut);

            List<EseguiSmistamentoElement> smistaEle = new List<EseguiSmistamentoElement> ();
            smistaEle.Add (new EseguiSmistamentoElement { Competenza= true,Conoscenza =false, IdRuolo ="347718",IdUO ="86123",IdUtente ="8571599",NoteIndividuali =""});
            EseguiSmistamentoRequest request = new EseguiSmistamentoRequest();
            request.Ruolo = ruoloInfo;
            request.UserInfo = userInfo;
            request.IdDocumento = "8602769";
            request.IdTrasmissione = "8602782";
            request.IdTrasmissioneSingola = "8602783";
            request.IdTrasmissioneUtente = "8602784";
            request.NoteGenerali = "";
            request.Elements = smistaEle;




            //questo andrebbe nel backend
            
            MittenteSmistamento mittente=new MittenteSmistamento();
            mittente.IDAmministrazione = request.UserInfo.IdAmministrazione;
            mittente.IDPeople = request.UserInfo.IdPeople;

            foreach (RegistroInfo temp in request.Ruolo.Registri)
            {
                mittente.RegistriAppartenenza.Add(temp.SystemId);
            }

            mittente.EMail = BusinessLogic.Utenti.UserManager.getUtenteById(request.UserInfo.IdPeople).email;
            mittente.IDCorrGlobaleRuolo=request.Ruolo.Id;
            mittente.IDGroup=request.Ruolo.IdGruppo;
            mittente.LivelloRuolo = request.Ruolo.Livello;
            DocsPaVO.Smistamento.UOSmistamento uoSmista = null;
            DocsPaVO.Smistamento.UOSmistamento[] uoInferiori = null;

            uoSmista= BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOAppartenenza(request.Ruolo.Id, mittente, false);
            uoSmista.UoInferiori = BusinessLogic.SmistamentoDocumenti.SmistamentoManager.GetUOInferiori(uoSmista.ID, mittente) ;
            uoInferiori = uoSmista.UoInferiori.Cast<DocsPaVO.Smistamento.UOSmistamento>().ToArray();

            request.Elements[0].fillUOSmistamento(uoSmista);

            //mob.VTDocsWSMobileClient mc = new mob.VTDocsWSMobileClient ();


           // mc.eseguiSmistamento(request);


            //DocsPaVO.Mobile.Requests.GetAdlRequest r = new DocsPaVO.Mobile.Requests.GetAdlRequest ();
            /*
            r.UserInfo = UserInfo.buildInstance (ut);
            r.RuoloInfo = RuoloInfo.buildInstance (ru.FirstOrDefault ());
            DocsPaVO.utente.Registro regi = (DocsPaVO.utente.Registro )ru[0].registri[0];
            r.RegistroInfo = RegistroInfo.buildInstance (regi);
            
            var risu=   c.getAdl(r);
             */
            return;


            //DocsPaUtils.Functions.Functions.ToDate("31/08/2012 16:01:03");
            log4net.Config.XmlConfigurator.Configure();
            Console.WriteLine("START");
            InviaMostro();
            //getDocumento();
            return;
            /*
            BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp vts = new BusinessLogic.Documenti.DigitalSignature.VerifyTimeStamp();
            byte[] tsr = System.IO.File.ReadAllBytes(@"C:\Tipi\prv\fileDaFirmare_h.txt.p7m.tsr");
            byte [] p7m =  System.IO.File.ReadAllBytes(@"C:\Tipi\prv\fileDaFirmare.txt.p7m");
            OutputResponseMarca rm = vts.Verify (p7m,tsr);
          */

            //proveEccezzioni();

            //BusinessLogic.interoperabilita.InteroperabilitaEccezioni.processaXmlEccezioni(@"C:\work\", "ecce2.xml", null, null, null, null);
            // BusinessLogic.Interoperabilità.InteroperabilitaNotificaAnnullamento.processaXmlAnnullamento(@"C:\work\", "Annullamento.xml", null, null, null);


            //BusinessLogic.Interoperabilità.InteroperabilitaUtils.findIdProfile("PAT", "143", 2012);
            //provevarie();
            //proveEccezzioni();

            provaIAZZI();
        }

        static void provaIAZZI()
        {
            bool gestioneRicevutePec = true;
            string docNumMailDSN = string.Empty;
            BusinessLogic.Interoperabilità.SvrPosta svr = new BusinessLogic.Interoperabilità.SvrPosta();
            DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed = null;
            mailProcessed = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed();
            BusinessLogic.Interoperabilità.CMMsg mc = new BusinessLogic.Interoperabilità.CMMsg();
            string EML = System.IO.File.ReadAllText(@"C:\work\messageKeane.eml");
            //mc = svr.getMessage(EML);
            if (mc != null)
            {
                mailProcessed.PecXRicevuta = BusinessLogic.interoperabilita.InteroperabilitaManager.getTipoRicevutaPec(mc);
                //Gestione DSN controllo se DSN e in caso recupero il docNumer dal subject
                if (mailProcessed.PecXRicevuta == DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification)
                {
                    if (mc.subject.Contains("#") || gestioneRicevutePec)
                    {
                        docNumMailDSN = extractDocNumberFromSubject(mc.subject);
                    }
                }
            }

        }


        static void InviaMostro()
        {
            Console.WriteLine("1");
            try
            {

                DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance("361");
                Console.WriteLine("Chiavi {0}", chiaviAmm.ListaChiavi.Count);
                DocsPaVO.utente.Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByCodice("D320SEG");


                if (role == null)
                    Console.WriteLine("roleNULL");

                Console.WriteLine("2");
                DocsPaVO.utente.Utente user = BusinessLogic.Utenti.UserManager.getUtente("PR34019", "361");
                user.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
                if (user == null)
                    Console.WriteLine("UserNULL");


                Console.WriteLine("3");
                DocsPaVO.utente.InfoUtente userInfo = new DocsPaVO.utente.InfoUtente(user, role);

                Console.WriteLine("DST _> {0}", userInfo.dst);


                string fileName = "vdirs.txt";
                string docNumber = ConfigurationManager.AppSettings["DOCNUMBER"];
                string pathFileName = @"D:\DOCSPA\UNITN_TEST\vdirs.txt";
                FileDocumento fileDoc = new FileDocumento();
                DocsPaVO.documento.FileRequest fileReq = new DocsPaVO.documento.Documento();
                Console.WriteLine("Creo Response");
                Console.WriteLine(String.Format("Docnumber {0}", docNumber));


                string response = BusinessLogic.Documenti.FileManager.PutFileBatch
                    (fileName, docNumber, "0", "0", "0", userInfo, pathFileName, ref fileReq, ref fileDoc);
                fileReq.versionId = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumber, userInfo);

                if (response.ToLower().Equals("y"))
                {
                    Console.WriteLine("Putfile del mostro");
                    fileReq = BusinessLogic.Documenti.FileManager.putFile(fileReq, fileDoc, userInfo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("EX {0}--- {1} ", e.Message, e.StackTrace));
            }

        }

        static void getDocumento()
        {

            DocsPaVO.utente.Utente user = BusinessLogic.Utenti.UserManager.getUtente("PR34019", "361");
            DocsPaVO.utente.Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByCodice("D320SEG");
            DocsPaVO.utente.InfoUtente userInfo = new DocsPaVO.utente.InfoUtente(user, role);

            DocsPaVO.documento.SchedaDocumento document = BusinessLogic.Documenti.DocManager.getDettaglio(userInfo, "8583381", "8583381");

            DocsPaVO.documento.FileDocumento file = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)document.documenti[0], userInfo);


        }

        public static string extractDocNumberFromSubject(string subject)
        {
            string retval = string.Empty;
            if (subject.Contains("#"))
            {
                string[] split = subject.Split('#');
                if (split.Length > 1)
                    retval = split[1];
            }
            retval = retval.Replace("#", string.Empty);
            try
            {
                BusinessLogic.Documenti.DocManager.GetTipoDocumento(retval);
            }
            catch (Exception e)
            {
                retval = null;
            }
            return retval;
        }

        static void provevarie()
        {
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.email = "pippo@lkfls.com";

            DocsPaVO.utente.Utente u = BusinessLogic.Utenti.UserManager.getUtente("PR27557", "361");

            DocsPaVO.utente.Ruolo r = null;
            ArrayList ruoloList = BusinessLogic.Utenti.UserManager.getRuoliUtente(u.idPeople);
            foreach (DocsPaVO.utente.Ruolo ri in ruoloList)
                r = ri;



            DocsPaVO.utente.InfoUtente iu = BusinessLogic.Utenti.UserManager.GetInfoUtente(u, r);
            ArrayList alRegi = BusinessLogic.Utenti.RegistriManager.getRegistri(r.systemId);


            BusinessLogic.RubricaComune.RubricaServices.isCorrispondenteFederatoByEmail(null, "pippo@poppo.com");

            return;
            DocsPaVO.utente.Registro regi = BusinessLogic.Utenti.RegistriManager.getRegistro("1031582");


            // DocsPaVO.utente.Corrispondente corr=  BusinessLogic.Utenti.UserManager.getCorrispondenteByEmail("p3pats007@pec.infotn.it", iu, "1031582" out pippo);

            DataSet ds = BusinessLogic.Utenti.UserManager.GetCorrByEmail("p3pats007@pec.infotn.it", regi.systemId);



            string docNumber = "8560523";
            string idCorrGlobale = regi.systemId;

            string motivo = "mail non inviata";
            string email = "p3pats007@pec.infotn.it";



            List<string> corrList = InteroperabilitaUtils.getCorrIdListByEmail(email, regi.systemId);
            //usiamo poi il metodo tradizionale 
            string idCorr = BusinessLogic.Utenti.UserManager.getCorrispondenteByEmail(email, iu, regi).systemId;
            if (!string.IsNullOrEmpty(idCorr))
                corrList.Add(idCorr);


            List<DocsPaVO.documento.ProtocolloDestinatario> pdList = new List<ProtocolloDestinatario>();

            foreach (string corrId in corrList)
            {
                DocsPaVO.utente.Corrispondente corri = new DocsPaVO.utente.Corrispondente { systemId = corrId };
                ArrayList statoInvioAL = BusinessLogic.Documenti.ProtocolloUscitaManager.aggiornamentoConferma(docNumber, corri);
                foreach (DocsPaVO.documento.ProtocolloDestinatario p in statoInvioAL)
                    pdList.Add(p);

            }

            if (pdList.Count == 1)
            {

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                bool result = obj.updStatoInvioEccezione(pdList[0].systemId, motivo);


            }

        }
        void proveTSR()
        {

            string url = "https://ws-s.marca.intra.infotn.it:15602/Resources/WebServices/MarcaturaTemporale.serviceagent/Soap11";
            DocsPa_TSAuthority_InfoTN.MarcaWCF m = new DocsPa_TSAuthority_InfoTN.MarcaWCF();
            string dispo = m.getMarcheDisponibili(url);

            DocsPaVO.areaConservazione.InputMarca im = new DocsPaVO.areaConservazione.InputMarca();
            Console.WriteLine(dispo);

            Console.ReadLine();

            im.file_p7m = @"D:\vers\marca\fileDaFirmare.txt.p7m";
            /*
            //DocsPaVO.areaConservazione.OutputResponseMarca outVal = m.getMarcaByFile(im, url);
            DocsPaVO.areaConservazione.OutputResponseMarca outVal = m.getMarcaByHash(im, url);
             */
            string marca64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@"D:\vers\marca\fileDaFirmare.txt.p7m.tsr"));
            string file64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(@"D:\vers\marca\fileDaFirmare.txt.p7m"));
            Console.WriteLine(marca64);
            Console.WriteLine(file64);
            DocsPaVO.areaConservazione.OutputResponseMarca outVal = m.verificaTimeStamp(marca64, file64, url);
            string esi = String.Format("Esito {0} Data {1} serNum {2}   NomeTsa {3}", outVal.esito, outVal.docm_date, outVal.sernum, outVal.TSA.TSARFC2253Name);
            string tsr = outVal.marca;
            Console.WriteLine(esi);


            //System.IO.File.WriteAllBytes(@"D:\vers\marca\fileDaFirmare_h.txt.p7m.tsr", Convert.FromBase64String(tsr));
            //System.IO.File.WriteAllText(@"D:\vers\marca\fileDaFirmare_h.txt.p7m.tsr.b64", tsr);

            /*
            //DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("S105SEG", DocsPaVO.addressbook.TipoUtente.INTERNO, administrationSyd);
            //DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("PR34019", DocsPaVO.addressbook.TipoUtente.INTERNO, "361");
            
     */

        }
        void provafirma1()
        {

            string filename = null;
            filename = "c:\\tipi\\18279424_ottav.pdf.P7M.P7M";
            //filename = "c:\\tipi\\fir\\DoppiaFirma_TestFirmaUSBKEY.txt.p7m.p7m.p7m";
            BusinessLogic.Documenti.DigitalSignature.VerifySignature vs = new BusinessLogic.Documenti.DigitalSignature.VerifySignature();
            DocsPaVO.documento.VerifySignatureResult result = vs.Verify(filename);

        }
        /*
        static void proveEccezzioni()
        {
            BusinessLogic.Interoperabilità.SvrPosta s = new BusinessLogic.Interoperabilità.SvrPosta();
            string eml = System.IO.File.ReadAllText(@"C:\work\message.eml");
            CMMsg mail = s.getMessage(eml);

            string xmlFile = System.IO.File.ReadAllText(@"C:\work\segnatura.xml");



            BusinessLogic.interoperabilita.InteroperabilitaEccezioni ce = new BusinessLogic.interoperabilita.InteroperabilitaEccezioni(mail, xmlFile);


        }
        */


        public static ArrayList DocumentoGetQueryDocumentoPagingCustom(InfoUtente infoUtente, DocsPaVO.filtri.FiltroRicerca[][] queryList, int numPage, bool security, int pageSize, out int numTotPage, out int nRec, bool getIdProfilesList, bool gridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, out List<SearchResultInfo> idProfileList)
        {
            List<SearchResultInfo> toSet = new List<SearchResultInfo>();
            ArrayList objListaInfoDocumenti = null;
            numTotPage = 0;
            nRec = 0;

            try
            {
                objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, queryList, numPage, pageSize, security, export, gridPersonalization, visibleFieldsTemplate, documentsSystemId, out numTotPage, out nRec, getIdProfilesList, out toSet);
            }
            catch (Exception e)
            {
               // logger.Debug("Errore in DocsPaWS.asmx  - metodo: DocumentoGetQueryDocumentoPagingCustom", e);

                objListaInfoDocumenti = null;
            }
            idProfileList = toSet;
            return objListaInfoDocumenti;
        }


        public static System.Collections.ArrayList getListaFascicoli(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, byte[] datiExcel, string serverPath)
        {
            #region Codice Commentato
            /*logger.Debug("getListaFascicoli");
			System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

			DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
			try 
			{
				db.openConnection();
				string queryString = getQueryFascicoli(infoUtente,objClassificazione.registro);
				if (objClassificazione != null)
				{
					if (childs)
						queryString += " AND A.VAR_CODICE LIKE '" + objClassificazione.codice +"%'";
					else
						queryString += " AND A.ID_PARENT = " + objClassificazione.systemID;
				}
					queryString += getSqlQuery(objListaFiltri);
				queryString += " ORDER BY A.SYSTEM_ID";
				logger.Debug(queryString);
				
				DataSet dataSet = new DataSet();
				db.fillTable(queryString, dataSet, "PROJECT");	

				//creazione della lista oggetti
				foreach(DataRow dataRow in dataSet.Tables["PROJECT"].Rows) 
				{
					listaFascicoli.Add(getFascicolo(dataSet, dataRow));
				}  
				dataSet.Dispose();
				db.closeConnection();
				
			} 
			catch (Exception e) 
			{
				logger.Debug (e.Message);				
				db.closeConnection();
				throw new Exception("F_System");
			}		*/
            #endregion

            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            listaFascicoli = fascicoli.GetListaFascicoli(infoUtente, objClassificazione, objListaFiltri, enableUfficioRef, enableProfilazione, childs, datiExcel, serverPath);

            if (listaFascicoli == null)
            {
                
                throw new Exception("F_System");

            }

            fascicoli.Dispose();

            return listaFascicoli;
        }


    }
}
