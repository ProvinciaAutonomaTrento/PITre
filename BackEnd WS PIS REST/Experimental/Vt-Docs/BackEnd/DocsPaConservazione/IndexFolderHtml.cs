using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DocsPaVO.areaConservazione;
using log4net;

namespace DocsPaConservazione
{
    public class IndexFolderHtml
    {
        private ILog logger = LogManager.GetLogger(typeof(IndexFolderHtml));
        /// <summary>
        /// Questa classe si occupa di generare l'indice html e le relative pagine di ricerca
        /// </summary>
        /// <param name="items"></param>
        /// <param name="HtmlPath"></param>
        /// 
        private string html_Path = string.Empty;
        private static string namePageHtml = "RicContatori_";
        public IndexFolderHtml(ItemsConservazione[] items, string HtmlPath, string NumeroIstanza, string TipoIstanza, string Ente, string NoteIstanza)
        {
            html_Path = HtmlPath;  
            bool res = false;
            string[] htmlPath = new string[10];
            string[] titolo = new string[10];
            TextWriter twriter = null;
            titolo[0] = string.Empty;
            htmlPath[0] = Path.Combine(HtmlPath, "index.html");
      
            //nuova struttura directory!!!!!!!!!!!
            HtmlPath = Path.Combine(HtmlPath, "html");
            titolo[1] = "Ricerca per numero documento";
            htmlPath[1] = Path.Combine(HtmlPath, "RicDocNumber.html");
            res = createHTML(items, htmlPath[1], "docNumber");
            titolo[2] = "Ricerca per segnatura o numero di documento";
            htmlPath[2] = Path.Combine(HtmlPath, "RicSegnatura.html");
            res = createHTML(items, htmlPath[2], "segnatura");
            titolo[3] = "Ricerca per descrizione oggetto";
            htmlPath[3] = Path.Combine(HtmlPath, "RicOggetto.html");
            res = createHTML(items, htmlPath[3], "oggetto");
            titolo[4] = "Ricerca per codice fascicolo";
            htmlPath[4] = Path.Combine(HtmlPath, "RicFascicolo.html");
            res = createHTML(items, htmlPath[4], "fascicolo");
            titolo[5] = "Ricerca per data di creazione o protocollazione";
            htmlPath[5] = Path.Combine(HtmlPath, "RicData.html");
            res = createHTML(items, htmlPath[5], "data");
            titolo[6] = "Ricerca per nome file";
            htmlPath[6] = Path.Combine(HtmlPath, "RicFileName.html");
            res = createHTML(items, htmlPath[6], "fileName");
            titolo[7] = "Ricerca per mittente";
            htmlPath[7] = Path.Combine(HtmlPath, "RicMittente.html");
            res = createHTML(items, htmlPath[7], "mittente");
            titolo[8] = "Ricerca per creatore documento";
            htmlPath[8] = Path.Combine(HtmlPath, "RicCreatoreDocumento.html");
            res = createHTML(items, htmlPath[8], "creatoreDocumento");
            titolo[9] = "Ricerca per tipologia documento";
            htmlPath[9] = Path.Combine(HtmlPath, "RicTipologiaDocumento.html");
            res = createHTML(items, htmlPath[9], "tipologiaDocumento");
            try
            {
                if (!File.Exists(htmlPath[0]))
                {
                    twriter = new StreamWriter(htmlPath[0], false, Encoding.UTF8);
                    twriter.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                    twriter.WriteLine("<HTML>");
                    twriter.WriteLine("<HEAD>");
                    twriter.WriteLine("<TITLE> INDICE RICERCHE </TITLE>");
                    twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"static/main.css\" />");
                    twriter.WriteLine("</HEAD>");
                    twriter.WriteLine("<BODY>");
                    twriter.WriteLine("<div class=\"site-container\">");
	                twriter.WriteLine("<div class=\"header\">");
		            twriter.WriteLine("<img src=\"static/logo.jpg\" />");
	                twriter.WriteLine("</div>");
	                twriter.WriteLine("<div class=\"site-title\">");
		            twriter.WriteLine("<h3>Tutti i file contenuti all’interno di questo supporto sono UFFICIALI</h3>");
	                twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"body-content\">");
                    twriter.WriteLine("<h4 class=\"title-indicizza\"><a target=\"content\" href='./chiusura/file_chiusura.XML.p7m'>File di chiusura</a><br /><br /><a target=\"content\" href='./rapporto_versamento.xml'>Rapporto di versamento</a></h4>");
                    twriter.WriteLine("</div>");
	                twriter.WriteLine("<div class=\"body-content\">");
		            twriter.WriteLine("<h4 class=\"title-indicizza\">Indicizza per: </h4>");
		            twriter.WriteLine("<div class=\"col-sx\">");
			        twriter.WriteLine("<ul class=\"index\">");

                    for (int i = 1; i < htmlPath.Length; i++)
                    {
                        string path_relativo = "." + '\u002F'.ToString() + "html" + '\u002F'.ToString() + Path.GetFileName(htmlPath[i]);
                        twriter.WriteLine("<li><a target=\"content\" href='" + path_relativo + "'>" + titolo[i] + "</a></li>");
                    }

                    twriter.WriteLine("</ul>");
		            twriter.WriteLine("</div>");
		            twriter.WriteLine("<div class=\"col-dx\">");
			        twriter.WriteLine("<iframe name=\"content\" class=\"content-frame\"></iframe>");
		            twriter.WriteLine("</div>");
		            twriter.WriteLine("<div class=\"cl\"></div>");
                    twriter.WriteLine("<div class=\"sep\"></div>");
                    twriter.WriteLine("<table class=\"footer-table\">");
			        twriter.WriteLine(String.Format ("<tr><th>Ente:</th><td>{0}</td></tr>",Ente));
			        twriter.WriteLine(String.Format ("<tr><th>Istanza numero:</th><td>{0}</td></tr>",NumeroIstanza));
			        twriter.WriteLine(String.Format ("<tr><th>Tipo di istanza:</th><td>{0}</td></tr>",TipoIstanza));
			        twriter.WriteLine(String.Format ("<tr><th>Note istanza:</th><td>{0}</td></tr>",NoteIstanza));
		            twriter.WriteLine("</table>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<script>");
                    twriter.WriteLine("var elems = document.getElementsByTagName(\"a\");");
                    twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
	                twriter.WriteLine("elems[i].onclick = function () {");
		            twriter.WriteLine("setActive(this);");
	                twriter.WriteLine("};");
                    twriter.WriteLine("}");
                    twriter.WriteLine("function setActive(elem){");
	                twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
		            twriter.WriteLine("elems[i].className = \"\";");
	                twriter.WriteLine("}");
	                twriter.WriteLine("elem.className = \"active\";");
                    twriter.WriteLine("}");
                    twriter.WriteLine("</script>");
                    twriter.WriteLine("</BODY>");
                    twriter.WriteLine("</HTML>");

                }
            }
            catch (Exception exHtml)
            {
                string err = "Errore nella creazione del main index Html : " + exHtml.Message;
                logger.Debug(err);
            }
            finally
            {
                if (twriter != null)
                {
                    twriter.Flush();
                    twriter.Close();
                }
            }
        }
        
        /// <summary>
        /// Questo metodo crea gli oggetti RicercheHtml e li ordina secondo il tipo di ricerca passato come parametro
        /// </summary>
        /// <param name="items"></param>
        /// <param name="tipoRicerca"></param>
        /// <returns></returns>
        private RicercheHtml[] orderIndex(ItemsConservazione[] items, string tipoRicerca)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < items.Length; i++)
            {
                RicercheHtml ric = new RicercheHtml(items[i], tipoRicerca);
                list.Add(ric);
            }
            list.Sort();
            return (RicercheHtml[])list.ToArray(typeof(RicercheHtml));
        }

        private RicercheHtml[] orderIndex(ArrayList listaContatori, string tipoRicerca)
        {
            ArrayList list = new ArrayList();
            Contatore cont= null;            
            for (int i = 0; i < listaContatori.Count; i++)
            {
                try
                {
                    ArrayList appo = (ArrayList)listaContatori[i];
                    cont = ((Contatore)appo[0]);
                }
                catch
                {
                    cont = (Contatore)listaContatori[i];
                }
                finally
                {
                    RicercheHtml ric = new RicercheHtml(cont, tipoRicerca);
                    list.Add(ric);
                }
            }
            list.Sort();
            return (RicercheHtml[])list.ToArray(typeof(RicercheHtml));
        }

        /// <summary>
        /// Questo metodo crea i files Html con i dati contenuti negli items ordinandoli per tipo di ricerca
        /// </summary>
        /// <param name="items"></param>
        /// <param name="HtmlPath"></param>
        /// <param name="tipoRic"></param>
        /// <returns></returns>
        protected bool createHTML(ItemsConservazione[] items, string HtmlPath, string tipoRic)
        {
            RicercheHtml[] Ricerche = orderIndex(items, tipoRic);
            bool result = false;
            TextWriter twriter = null;
            //HtmlPath = Path.Combine(HtmlPath, "index.html");
            
            string paragrafo = string.Empty;
            try
            {
                if (!File.Exists(HtmlPath))
                {
                    twriter = new StreamWriter(HtmlPath, false, Encoding.UTF8);
                    twriter.WriteLine("<HTML>");
                    twriter.WriteLine("<HEAD>");
                    twriter.WriteLine("<TITLE> Ricerca per " + Ricerche[0].titoloRicerca + "  </TITLE>");
                    twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"../static/main.css\" />");
                    twriter.WriteLine("</HEAD>");
                    twriter.WriteLine("<BODY class=\"content\">");
                    twriter.WriteLine("<h3 align=center> Ricerca per " + Ricerche[0].titoloRicerca + "  </h3>");
                    twriter.WriteLine("<br><br>");
                    twriter.WriteLine("<ol type='1'>");
              


                    //twriter.WriteLine("<HTML>");
                    //twriter.WriteLine("<font face='Verdana' size=2>");
                    //twriter.WriteLine("<HEAD>");
                    //twriter.WriteLine("<TITLE> Ricerca per " + Ricerche[0].titoloRicerca + " </TITLE>");
                    //twriter.WriteLine("</HEAD>");
                    //twriter.WriteLine("<BODY>");
                    //twriter.WriteLine("<h3 align=center> Ricerca per " + Ricerche[0].titoloRicerca + " </h3>");
                    //twriter.WriteLine("<br><br>");
                    //twriter.WriteLine("<ol type='1'>");
                    ArrayList arrayTipologia = new ArrayList();

                    for (int i = 0; i < Ricerche.Length; i++)
                    {
                        //i tag <ul> e <li> lo chiudo e lo riapro in questo punto dopo aver controllato se il paragrafo
                        //è diverso da quello appena scritto!!!
                        //SE è IL PRIMO ELEMENTO NON DEVO METTERE I TAG DI CHIUSURA ALL'INIZIO!!!!!
                        switch (tipoRic)
                        {
                            case "tipologiaDocumento":
                                if (paragrafo.ToUpper() != Ricerche[i].valoreRicerca.ToUpper())
                                {
                                    string path_relativo = "." + '\u002F'.ToString() + namePageHtml + Ricerche[i].valoreRicerca + ".html";
                                    twriter.WriteLine("</ul>");
                                    twriter.WriteLine("</li>");
                                    //twriter.WriteLine("<li><b><a href='" + html_Path + "\\html\\" + namePageHtml + Ricerche[i].valoreRicerca + ".html'>" + Ricerche[i].valoreRicerca + "</a></b>");
                                    twriter.WriteLine("<li><b><a target=\"_blank\" href='" + path_relativo + "'>" + Ricerche[i].valoreRicerca + "</a></b>");
                                    twriter.WriteLine("<ul type='disc'>");
                                    this.creaContatori((RicercheHtml)Ricerche[i], ref arrayTipologia, false);
                                }
                                else
                                {
                                    this.creaContatori((RicercheHtml)Ricerche[i], ref arrayTipologia, true);                                    
                                }                                        
                                paragrafo = Ricerche[i].valoreRicerca;
                                break;                                                                                             
                            default:
                                if (i > 0 && paragrafo != Ricerche[i].valoreRicerca)
                                {
                                    twriter.WriteLine("</ul>");
                                    twriter.WriteLine("</li>");
                                    twriter.WriteLine("<li><b>" + Ricerche[i].valoreRicerca + "</b>");
                                    twriter.WriteLine("<ul type='disc'>");
                                    paragrafo = Ricerche[i].valoreRicerca;
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        twriter.WriteLine("<li><b>" + Ricerche[i].valoreRicerca + "</b>");
                                        if ((Ricerche[i].itemsCons.ID_Project != string.Empty ) && (tipoRic =="fascicolo"))
                                        {
                                            twriter.WriteLine("<br/><b>File metadati Fascicolo: </b><a target=\"_blank\" href='../Fascicoli/" + Ricerche[i].valoreRicerca + "/" + Ricerche[i].itemsCons.ID_Project.Replace('\u005C', '\u002F') + ".xml'> \\Fascicoli\\" + Ricerche[i].valoreRicerca + "\\" + Ricerche[i].itemsCons.ID_Project + ".xml</a>");
                                        }
                                        twriter.WriteLine("<ul type='disc'>");
                                        paragrafo = Ricerche[i].valoreRicerca;
                                      
                                    }
                                }
                                if (Ricerche[i].itemsCons.pathCD != string.Empty)
                                {
                                    twriter.WriteLine("<li><a target=\"_blank\" href='.." + Ricerche[i].itemsCons.pathCD.Replace('\u005C', '\u002F') + "'>" + Ricerche[i].itemsCons.pathCD + "</a></li>");
                                    twriter.WriteLine("<li type='circle'><b>File metadati: </b><a target=\"_blank\" href='.." + Ricerche[i].itemsCons.pathCD.Replace('\u005C', '\u002F') + ".xml'>" + Ricerche[i].itemsCons.pathCD + ".xml</a></li>");
                                }
                                if (Ricerche[i].itemsCons.timestampDoc != null && 
                                    Ricerche[i].itemsCons.timestampDoc.Count > 0)
                                {
                                    for (int t = 0; t < Ricerche[i].itemsCons.timestampDoc.Count; t++)
                                    {
                                        DocsPaVO.documento.TimestampDoc times = (DocsPaVO.documento.TimestampDoc)Ricerche[i].itemsCons.timestampDoc[t];
                                        string percorso = Ricerche[i].itemsCons.pathTimeStamp + '\u005C'.ToString() +"TS" + times.DOC_NUMBER + "-" + times.NUM_SERIE ;
                                        twriter.WriteLine("<li type='circle'><b>Timestamp: </b><a target=\"_blank\" href='.." + percorso.Replace('\u005C', '\u002F') + ".tsr'>" + percorso + ".tsr</a></li>");
                                    }
                                }


                                if (Ricerche[i].itemsCons.path_allegati != null)
                                {
                                    for (int j = 0; j < Ricerche[i].itemsCons.path_allegati.Count; j++)
                                    {
                                        string[] app = Ricerche[i].itemsCons.path_allegati[j].ToString().Split('@');
                                        twriter.WriteLine("<ul type='square'>");
                                        twriter.WriteLine("<li><b>File allegato: </b><a target=\"_blank\" href='.." + app[0].Replace('\u005C', '\u002F') + "'>" + app[0].ToString() + "</a></li>");
                                        
                                        for (int a = 1; a < app.Length; a++)
                                        {
                                            string allegato = app[0].Substring(0, app[0].Length-4) + '\u005C'.ToString() + app[a];
                                            twriter.WriteLine("<li type='circle'><b>Timestamp: </b><a target=\"_blank\" href='.." + allegato.Replace('\u005C', '\u002F') + "'>" + allegato + "</a></li>");
                                        }
                                        twriter.WriteLine("</ul>");
                                    }
                                }
                                break;
                        }
                    }
                    if (tipoRic == "tipologiaDocumento")
                    {
                        for (int j = 0; j < arrayTipologia.Count; j++)
                        {
                            ArrayList contatori = ((ArrayList)arrayTipologia[j]);
                            Contatore cont = (Contatore)contatori[0];
                            createHTML((ArrayList)arrayTipologia[j], html_Path + "\\html\\" + namePageHtml + cont.tipoDoc + ".html");
                        }
                    }
                    twriter.WriteLine("</ol>");
                    twriter.WriteLine("</BODY>");
                    twriter.WriteLine("</HTML>");
                }
                result = true;
            }
            catch (Exception exHtml)
            {
                string err = "Errore nella creazione della pagina " + HtmlPath + " : " + exHtml.Message;
                logger.Debug(err);
                result = false;
            }
            finally
            {
                if (twriter != null)
                {
                    twriter.Flush();
                    twriter.Close();
                }
            }
            return result;
        }

        protected void creaContatori(RicercheHtml ric, ref ArrayList tipologieDoc, bool concatena)
        {
            ArrayList arrayContatori = new ArrayList();
            ArrayList lista = new ArrayList();
            int index = tipologieDoc.Count;
            if (ric.itemsCons.template != null)
            {
                lista = ric.itemsCons.template.ELENCO_OGGETTI;
                if (lista.Count == 0)
                {
                    Contatore contatore = new Contatore();
                    contatore.items = (ItemsConservazione)ric.itemsCons.Clone();
                    contatore.tipoDoc = ric.valoreRicerca;
                    contatore.descContatore = "non valorizzati";
                    contatore.valoreContatore = "";
                    contatore.segnatura = "non valorizzati";
                    contatore.items.segnaturaContatore = "non valorizzati";
                    arrayContatori.Add(contatore);
                }
                else
                {
                    foreach(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in lista)
                    {
                        if(oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Contatore")
                        {
                            if (oggettoCustom.FORMATO_CONTATORE != "")
                            {
                                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                                {
                                    Contatore contatore = new Contatore();
                                    contatore.segnatura = oggettoCustom.FORMATO_CONTATORE;
                                    contatore.items = (ItemsConservazione)ric.itemsCons.Clone();
                                    contatore.items.descContatore = oggettoCustom.DESCRIZIONE;
                                    contatore.tipoDoc = ric.valoreRicerca;
                                    contatore.descContatore = oggettoCustom.DESCRIZIONE;
                                    contatore.valoreContatore = oggettoCustom.VALORE_DATABASE;
                                    contatore.segnatura = contatore.segnatura.Replace("ANNO", oggettoCustom.ANNO);
                                    contatore.segnatura = contatore.segnatura.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                                    if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                                    {
                                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                        if (reg != null)
                                        {
                                            contatore.segnatura = contatore.segnatura.Replace("RF", reg.codRegistro);
                                            contatore.segnatura = contatore.segnatura.Replace("AOO", reg.codRegistro);
                                        }
                                    }
                                    contatore.items.segnaturaContatore = contatore.segnatura;
                                    arrayContatori.Add(contatore);
                                }
                            }
                        }
                    }
                    if (arrayContatori.Count == 0)
                    {
                        Contatore contatore = new Contatore();
                        contatore.items = (ItemsConservazione)ric.itemsCons.Clone();
                        contatore.tipoDoc = ric.valoreRicerca;
                        contatore.descContatore = "non valorizzati";
                        contatore.valoreContatore = "";
                        contatore.segnatura = "non valorizzati";
                        contatore.items.segnaturaContatore = "non valorizzati";
                        arrayContatori.Add(contatore);
                    }
                }
            }
            else
            {
                Contatore contatore = new Contatore();
                contatore.items = (ItemsConservazione)ric.itemsCons.Clone();
                contatore.tipoDoc = ric.valoreRicerca;
                contatore.descContatore = "non valorizzati";
                contatore.valoreContatore = "";
                contatore.segnatura = "non valorizzati";
                contatore.items.segnaturaContatore = "non valorizzati";
                arrayContatori.Add(contatore);
            }
            if (!concatena)
            {
                tipologieDoc.Add(arrayContatori);
            }
            else
            {
                ((ArrayList)tipologieDoc[index - 1]).Add(arrayContatori);
            }
        }

        protected bool createHTML(ArrayList contatoriPerTipologia, string HtmlPath)
        {
            RicercheHtml[] Ricerche = orderIndex(contatoriPerTipologia, "tipoContatore");                
            bool result = false;
            TextWriter twriter = null;
            string paragrafo = string.Empty;
            ArrayList tipologieContatori = new ArrayList();            
            SortedList sl = new SortedList();
            int k=0;
            try
            {
                if (!File.Exists(HtmlPath))
                {
                    twriter = new StreamWriter(HtmlPath, false, Encoding.UTF8);
                    twriter.WriteLine("<HTML>");
                    twriter.WriteLine("<font face='Verdana' size=2>");
                    twriter.WriteLine("<HEAD>");
                    twriter.WriteLine("<TITLE> Ricerca per " + Ricerche[0].titoloRicerca + " </TITLE>");
                    twriter.WriteLine("</HEAD>");
                    twriter.WriteLine("<BODY>");
                    twriter.WriteLine("<h3 align=center> Ricerca per " + Ricerche[0].titoloRicerca + " </h3>");
                    twriter.WriteLine("<br><br>");
                    twriter.WriteLine("<ol type='1'>");

                    for (int i = 0; i < Ricerche.Length; i++)
                    {
                        ArrayList itemsPerContatore = new ArrayList();
                        k = tipologieContatori.Count;
                        if (paragrafo != Ricerche[i].descContatore)
                        {
                            string path_relativo = "." + '\u002F'.ToString() + "Documenti_" + Ricerche[i].tipologiaDoc.Trim() + Ricerche[i].descContatore.Trim() + ".html";
                            twriter.WriteLine("</ul>");
                            twriter.WriteLine("</li>");
                            //twriter.WriteLine("<li><b><a href='" + html_Path + "\\html\\" + "Documenti_" + Ricerche[i].tipologiaDoc.Trim() + Ricerche[i].descContatore.Trim() + ".html'>" + Ricerche[i].descContatore + "</a></b>");
                            twriter.WriteLine("<li><b><a href='" + path_relativo + "'>" + Ricerche[i].descContatore + "</a></b>");
                            twriter.WriteLine("<ul type='disc'>");
                            paragrafo = Ricerche[i].descContatore;
                            itemsPerContatore.Add(Ricerche[i].itemsCons);
                            tipologieContatori.Add(itemsPerContatore);
                            sl.Add(Ricerche[i].tipologiaDoc.Trim() + Ricerche[i].descContatore.Trim(), itemsPerContatore);                            
                        }
                        else
                        {
                            Ricerche[i].itemsCons.descContatore = Ricerche[i].descContatore;
                            ((ArrayList)tipologieContatori[k - 1]).Add(Ricerche[i].itemsCons);
                            sl.Remove(Ricerche[i].tipologiaDoc.Trim() + Ricerche[i].descContatore.Trim());
                            sl.Add(Ricerche[i].tipologiaDoc.Trim() + Ricerche[i].descContatore.Trim(), ((ArrayList)tipologieContatori[k - 1]));
                        }                
                    }

                    foreach (string key in sl.GetKeyList())
                    {
                        ArrayList items = (ArrayList)sl[key];
                        ItemsConservazione[] itemsCons = new ItemsConservazione[items.Count];
                        items.CopyTo(itemsCons);
                        createHTML(itemsCons, html_Path + "\\html\\" + "Documenti_" + key + ".html", "docContatore");
                    }

                    twriter.WriteLine("</ol>");
                    twriter.WriteLine("</BODY>");
                    twriter.WriteLine("</font>");
                    twriter.WriteLine("</HTML>");
                }
                result = true;
            }
            catch(Exception e)
            {
                string err = "Errore nella creazione della pagina " + HtmlPath + " : " + e.Message;
                logger.Debug(err);
                result = false;
            }
            finally
            {
                if (twriter != null)
                {
                    twriter.Flush();
                    twriter.Close();
                }
            }
            return result;
        }
    }
}
