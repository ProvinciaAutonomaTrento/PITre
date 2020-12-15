using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaDocumentale_HERMES.DMFileOperation;
using System.Data.OracleClient;
using System.Data;

namespace DocsPaDocumentale_HERMES_HB
{
    /// <summary>
    /// Class factory per la crezione di un oggetto Service di Hummingbird
    /// </summary>
    public class DocumentManagerHermesHB : IDocumentManager
    {
        public string Libreria { get; set; }
        public string Este { get; set; }
        public string DMUser { get; set; }
        public string DMPassword { get; set; }
        public Byte[] pDoc { get; set; }
        public string NomeForm { get; set; }
        public string Titolo_Doc { get; set; }
        public string Appl_ID { get; set; }
        public string Autore { get; set; }
        public string Note { get; set; }
        public string Trustee { get; set; }

        public DocumentManagerHermesHB() {

            Libreria = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_lib"];
            Este = "";
            DMUser = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_user"];
            DMPassword = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_password"];
            Libreria = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_lib"];
            NomeForm = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_form"];
            Appl_ID = "";
            Autore = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_autore"];
            Trustee = "";
            DMUser = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_user"];
            DMPassword = System.Configuration.ConfigurationManager.AppSettings["Hummingbird_password"];
     
        
        }

        #region Metodi da implementare

        public bool GetFile(ref DocsPaVO.documento.FileDocumento fileDocumento, ref DocsPaVO.documento.FileRequest fileRequest)
        {
            
            
            throw new NotImplementedException();
        }

        public byte[] GetFile(string docNumber, string version, string versionId, string versionLabel)
        {
            
            String Estensione = "";
            byte[] outDoc = null;
        //    try
        //    {
                String tip_doc = get_tip_doc(docNumber);//aggiunto per NSD per tipologia
                String id_HB = get_vers_hermes(docNumber,versionId);
                DMFileOperationSoapClient ws = new DMFileOperationSoapClient();

                outDoc = ws.VisualizzaDocumentoSec(Libreria, id_HB.Trim(), ref Estensione, DMUser, DMPassword);
                if (outDoc == null || outDoc.Length == 0)
                {
                    outDoc = new byte[] { };
                }
                else
                {
                    Array.Resize<byte>(ref outDoc, outDoc.Length - 1);
                }
        //    }
        //    catch (Exception e)
        //    {
                //if (e.Message.Contains("Empty path name is not legal"))
        //            outDoc = new byte[] { };
        //    }

            return outDoc;
        }

        public bool PutFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocumento, string estensione)
        {

            Titolo_Doc = fileRequest.fileName;
            Note = fileRequest.descrizione;
            pDoc = fileDocumento.content;
            Este = estensione;
            String num_HB = "";
            String w_ins = "";
            bool esito = false;

            Appl_ID = get_Appl_ID(estensione.ToLowerInvariant());

            if (Appl_ID != null) {
                String tip_doc = get_tip_doc(fileRequest.docNumber);//aggiunto per NSD per tipologia
                DMFileOperationSoapClient ws = new DMFileOperationSoapClient();
                num_HB = ws.AggiungiDocumentoSec(Libreria, NomeForm, Titolo_Doc, Appl_ID, Autore, Note, pDoc, Trustee, DMUser, DMPassword);

                w_ins = set_fk_hermes(fileRequest.docNumber, num_HB, fileRequest.fileName,fileRequest.versionId);
                esito = true;
            }
            else
                esito = false;
            return esito;
        }
        
        public string get_Appl_ID (string estensione) {
        string Appl_ID = null;
        
        switch (estensione.ToLowerInvariant())
        {
            case "lwp":
                Appl_ID = "LOTUS WORD PRO";
                break;
            case "sam":
                Appl_ID = "LOTUS WORD PRO";
                break;
            case "eml":
                Appl_ID = "MS OUTLOOK";
                break;
            case "doc":
                Appl_ID = "MS WORD";
                break;
            case "dot":
                Appl_ID = "MS WORD";
                break;
            case "docx":
                Appl_ID = "MS WORD";
                break;
            case "xls":
                Appl_ID = "MS EXCEL";
                break;
            case "xlsx":
                Appl_ID = "MS EXCEL";
                break;
            case "pdf":
                Appl_ID = "ACROBAT";
                break;
            case "123":
                Appl_ID = "L123-97";
                break;
            case "qpw":
                Appl_ID = "QPW";
                break;
            case "mpp":
                Appl_ID = "MS PROJECT";
                break;
            case "mpt":
                Appl_ID = "MS PROJECT";
                break;
            case "wpd":
                Appl_ID = "WORDPERFECT";
                break;
            case "pps":
                Appl_ID = "MS POWERPOINT";
                break;
            case "ppt":
                Appl_ID = "MS POWERPOINT";
                break;
            case "gif":
                Appl_ID = "PUB GIF";
                break;
            case "jpg":
                Appl_ID = "PUB JPG";
                break;
            case "jpeg":
                Appl_ID = "PUB JPG";
                break;
            case "png":
                Appl_ID = "PUB PNG";
                break;
            case "xml":
                Appl_ID = "PUB XML";
                break;
            case "vsd":
                Appl_ID = "VISIO";
                break;
            case "vss":
                Appl_ID = "VISIO";
                break;
            case "dwg":
                Appl_ID = "AUTOCAD";
                break;
            case "dxf":
                Appl_ID = "AUTOCAD";
                break;
            case "zip":
                Appl_ID = "ARCHIVIO ZIP";
                break;
            case "msg":
                Appl_ID = "EMAIL";
                break;
            case "p7m":
                Appl_ID = "FIRMA DIGITALE";
                break;
         
        }

        return Appl_ID;
        }

        public string set_fk_hermes(string id_hermes, string id_hb, string file_name, string versione) {

            string esito = "";
            OracleConnection objConn = new OracleConnection(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = objConn;
            objCmd.CommandText = "PKG_HERMES_HUMMINGBIRD.SET_FK_HERMES";
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("in_id_hermes", OracleType.VarChar).Value = id_hermes.ToString();
            objCmd.Parameters.Add("in_id_hummingbird", OracleType.VarChar).Value = id_hb.ToString();
            objCmd.Parameters.Add("in_file_name", OracleType.VarChar).Value = file_name.ToString();
            objCmd.Parameters.Add("in_versione", OracleType.VarChar).Value = versione.ToString();
            objCmd.Parameters.Add("out_esito", OracleType.VarChar, 2000).Direction = ParameterDirection.Output;

            try
            {
                objConn.Open();
                objCmd.ExecuteNonQuery();

                if (!(objCmd.Parameters["out_esito"].Value is DBNull))

                    esito = (string)objCmd.Parameters["out_esito"].Value;
                else
                    esito = "1";
   
            }
            catch (Exception ex)
            {
                objConn.Close();
            }
            finally
            {
                objConn.Close();
            }
           
            return esito;
        }

        public string get_last_hermes(string id_hermes)
        {

            string esito = "";
            OracleConnection objConn = new OracleConnection(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = objConn;
            objCmd.CommandText = "PKG_HERMES_HUMMINGBIRD.GET_LAST_HERMES";
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("in_id_hermes", OracleType.VarChar).Value = id_hermes.ToString();
            objCmd.Parameters.Add("out_id_hb", OracleType.VarChar, 2000).Direction = ParameterDirection.Output;

            try
            {
                objConn.Open();
                objCmd.ExecuteNonQuery();

                if (!(objCmd.Parameters["out_id_hb"].Value is DBNull))

                    esito = (string)objCmd.Parameters["out_id_hb"].Value;
                else
                    esito = "1";

            }
            catch (Exception ex)
            {
                objConn.Close();
            }
            finally
            {
                objConn.Close();
            }

            return esito;
        }

        public string get_vers_hermes(string id_hermes, string versione)
        {

            string esito = "";
            OracleConnection objConn = new OracleConnection(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = objConn;
            objCmd.CommandText = "PKG_HERMES_HUMMINGBIRD.GET_VERS_HERMES";
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("in_id_hermes", OracleType.VarChar).Value = id_hermes.ToString();
            objCmd.Parameters.Add("in_versione", OracleType.VarChar).Value = versione.ToString();
            objCmd.Parameters.Add("out_id_hb", OracleType.VarChar, 2000).Direction = ParameterDirection.Output;

            try
            {
                objConn.Open();
                objCmd.ExecuteNonQuery();

                if (!(objCmd.Parameters["out_id_hb"].Value is DBNull))

                    esito = (string)objCmd.Parameters["out_id_hb"].Value;
                else
                    esito = "1";

            }
            catch (Exception ex)
            {
                objConn.Close();
            }
            finally
            {
                objConn.Close();
            }

            return esito;
        }

        public string get_tip_doc(string id_doc)
        {

            string esito = "";
            OracleConnection objConn = new OracleConnection(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            OracleCommand objCmd = new OracleCommand();
            objCmd.Connection = objConn;
            objCmd.CommandText = "PKG_HERMES_HUMMINGBIRD.GET_TIP_DOC";
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.Parameters.Add("in_id_doc", OracleType.VarChar).Value = id_doc.ToString();
            objCmd.Parameters.Add("out_des_tipo", OracleType.VarChar, 2000).Direction = ParameterDirection.Output;

            try
            {
                objConn.Open();
                objCmd.ExecuteNonQuery();

                if (!(objCmd.Parameters["out_des_tipo"].Value is DBNull))

                    esito = (string)objCmd.Parameters["out_des_tipo"].Value;
                else
                    esito = "1";

            }
            catch (Exception ex)
            {
                objConn.Close();
            }
            finally
            {
                objConn.Close();
            }

            return esito;
        }

        #endregion

        #region Metodi da non implementare
        public bool AddAttachment(DocsPaVO.documento.Allegato allegato, string putfile)
        {
            throw new NotImplementedException();
        }

        public bool AddDocumentoInCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            throw new NotImplementedException();
        }

        public bool AddPermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            throw new NotImplementedException();
        }

        public bool AddPermissionToRole(DocsPaVO.documento.DirittoOggetto rights)
        {
            throw new NotImplementedException();
        }

        public bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare)
        {
            throw new NotImplementedException();
        }

        public bool AnnullaProtocollo(ref DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.documento.ProtocolloAnnullato protocolloAnnullato)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoGrigio(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.utente.Ruolo[] ruoliSuperiori)
        {
            throw new NotImplementedException();
        }

        public bool CreateDocumentoStampaRegistro(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo)
        {
            throw new NotImplementedException();
        }

        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            throw new NotImplementedException();
        }

        public bool CreateProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione, out DocsPaVO.utente.Ruolo[] ruoliSuperiori, string conCopia = null)
        {
            throw new NotImplementedException();
        }

        public string GetFileExtension(string docnumber, string versionid)
        {
            throw new NotImplementedException();
        }

        public string GetLatestVersionId(string docNumber)
        {
            throw new NotImplementedException();
        }

        public string GetOriginalFileName(string docnumber, string versionid)
        {
            throw new NotImplementedException();
        }

        public bool IsVersionWithSegnature(string versionId)
        {
            throw new NotImplementedException();
        }

        public void ModifyAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            throw new NotImplementedException();
        }

        public bool ModifyExtension(ref DocsPaVO.documento.FileRequest fileRequest, string docNumber, string version_id, string version, string subVersion, string versionLabel)
        {
            throw new NotImplementedException();
        }

        public void ModifyVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            throw new NotImplementedException();
        }

        public bool ModifyVersionSegnatura(string versionId)
        {
            throw new NotImplementedException();
        }

        public bool PredisponiProtocollazione(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public bool ProtocollaDocumentoPredisposto(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione)
        {
            throw new NotImplementedException();
        }

        public void RefreshAclDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public bool Remove(params DocsPaVO.documento.InfoDocumento[] items)
        {
            throw new NotImplementedException();
        }

        public bool RemoveAttatchment(DocsPaVO.documento.Allegato allegato)
        {
            throw new NotImplementedException();
        }

        public bool RemovePermission(DocsPaVO.documento.DirittoOggetto infoDiritto)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVersion(DocsPaVO.documento.FileRequest fileRequest)
        {
            throw new NotImplementedException();
        }

        public bool RestoreDocumentoDaCestino(DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            throw new NotImplementedException();
        }

        public bool SalvaDocumento(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool ufficioReferenteEnabled, out bool ufficioReferenteSaved)
        {
            throw new NotImplementedException();
        }

        public bool ScambiaAllegatoDocumento(DocsPaVO.documento.Allegato allegato, DocsPaVO.documento.Documento documento)
        {
            throw new NotImplementedException();
        }
        #endregion

        
    }
}
