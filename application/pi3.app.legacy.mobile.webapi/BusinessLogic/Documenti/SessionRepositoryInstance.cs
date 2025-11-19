// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Documenti
{
    public class SessionRepositorySyncronizer {
        public static DocsPaVO.documento.FileRequest[] CopyToRepository(SessionRepositoryFileManager fileManager, DocsPaVO.documento.FileRequest[] versions)
        {
            List<DocsPaVO.documento.FileRequest> newVersions = new List<DocsPaVO.documento.FileRequest>();
            string tmpVersionLabel;
            foreach (DocsPaVO.documento.FileRequest version in versions)
            {
                tmpVersionLabel = version.versionLabel;

                version.repositoryContext = null;

                if (fileManager.ExistFile(version))
                {
                    // Reperimento del file dal repository di sessione
                    DocsPaVO.documento.FileDocumento document = fileManager.GetFile(version);

                    // Inserimento del file nel repository documentale
                    DocsPaVO.documento.FileRequest result = BusinessLogic.Documenti.FileManager.putFile(version, document, fileManager.Context.Owner);

                    if (!string.IsNullOrEmpty(tmpVersionLabel) && result != null)
                        result.versionLabel = tmpVersionLabel;

                    newVersions.Add(result);

                    // Rimozione del file dal repository di sessione
                    version.repositoryContext = null;
                }
                else
                    newVersions.Add(version);
            }

            return newVersions.ToArray();
        }

        public static DocsPaVO.documento.FileRequest CopyToRepository(SessionRepositoryFileManager fileManager, DocsPaVO.documento.FileRequest version)
        {
            DocsPaVO.documento.FileRequest result = null;

            version.repositoryContext = null;

            if (fileManager.ExistFile(version))
            {
                // Reperimento del file dal repository di sessione
                DocsPaVO.documento.FileDocumento document = fileManager.GetFile(version);

                // Inserimento del file nel repository documentale
                result = BusinessLogic.Documenti.FileManager.putFile(version, document, fileManager.Context.Owner);
            }
            else
                result = version;

            return result;
        }


    }

    [Serializable()]
    public class SessionRepositoryFileManager
    {
        private SessionRepositoryFileManager(DocsPaVO.documento.SessionRepositoryContext context)
        {
            this.Context = context;

            // Verifica che il repository di sessione non sia scaduto
            this.CheckForRepositoryExpired();

            // Reperimento dell'owner persistito  e confronto con l'owner corrente
            // per verificare utilizzi non autorizzati

            //2023-11-06: modifica per adeguarlo alla creazione al PiTre Cloud
            //DocsPaVO.utente.InfoUtente owner = (DocsPaVO.utente.InfoUtente)SessionRepositorySerializerHelper.Deserialize(Path.Combine(GetContextRepositoryPath(this.Context), "owner.dat"));
            //2023-11-06: FINE MODIFICA
            var fileContent = File.ReadAllText(Path.Combine(GetContextRepositoryPath(this.Context), "owner.dat"));
            DocsPaVO.utente.InfoUtente owner = JsonConvert.DeserializeObject<DocsPaVO.utente.InfoUtente>(fileContent);

            if (!owner.dst.Equals(this.Context.Owner.dst))
                throw new ApplicationException(string.Format("Utente {0} non autorizzato all'utilizzo del repository", this.Context.Owner.userId));
        }

        public DocsPaVO.documento.SessionRepositoryContext Context
        {
            get;
            protected set;
        }


        public static DocsPaVO.documento.SessionRepositoryContext NewRepository(DocsPaVO.utente.InfoUtente infoUtente, bool forceCreation)
        {
            if (!forceCreation && IsSessionRepositoryDisabled(infoUtente))
                // Gestione dei repository di sessione non abilitata
                return null;
            else
            {
                DocsPaVO.documento.SessionRepositoryContext context = new DocsPaVO.documento.SessionRepositoryContext(infoUtente);

                string path = GetContextRepositoryPath(context);

                // Creazione directory del repository
                Directory.CreateDirectory(path);

                // Serializzazione owner 

                //2023-11-06: modifica per adeguarlo alla creazione al PiTre Cloud
                //SessionRepositorySerializerHelper.SerializeObject(Path.Combine(path, "owner.dat"), infoUtente);
                //2023-11-06: FINE MODIFICA

                File.WriteAllText(Path.Combine(path, "owner.dat"), JsonConvert.SerializeObject(infoUtente));

                return context;
            }
        }

        private static string GetContextRepositoryPath(DocsPaVO.documento.SessionRepositoryContext context)
        {
            //if (SessionRepositoryDisabled)
            //    // Gestione dei repository di sessione non abilitata
            //    return null;
            //else

            if (context != null)
                return Path.Combine(SessionRepositoryFileManager.RepositoryRootPath, context.Token);
            else
                return null;
        }

        public static string RepositoryRootPath
        {
            get
            {
                //if (SessionRepositoryDisabled)
                //    // Gestione dei repository di sessione non abilitata
                //    return null;
                //else
                //{
                // Reperimento della directory base del repository documentale
                string docRoot = DocsPaVO.Settings.AppSettings.Instance.DOC_ROOT;

                if (string.IsNullOrEmpty(docRoot))
                    throw new ApplicationException("Base Directory del repository documentale non definita nella chiave di configurazione DOC_ROOT");

                return Path.Combine(docRoot, "SessionRepository");
                //}
            }
        }

        public static bool IsSessionRepositoryDisabled(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string valoreChiaveDB = string.Empty;
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(infoUtente.idAmministrazione);
            if (chiaviAmm != null && chiaviAmm.ContainsKey("BE_SESSION_REPOSITORY_DISABLED"))
                //valoreChiaveDB = ((DocsPaVO.amministrazione.ChiaveConfigurazione)chiaviAmm["BE_SESSION_REPOSITORY_DISABLED"]).Valore.ToString();
                valoreChiaveDB = chiaviAmm["BE_SESSION_REPOSITORY_DISABLED"].ToString();
            else
                valoreChiaveDB = "false";

            //const string KEY = "SESSION_REPOSITORY_DISABLED";

            bool retValue;
            //Boolean.TryParse(System.Configuration.ConfigurationSettings.AppSettings[KEY], out retValue);
            Boolean.TryParse(valoreChiaveDB, out retValue);
            return retValue;
        }

        public static SessionRepositoryFileManager GetFileManager(DocsPaVO.documento.SessionRepositoryContext context)
        {
            //if (SessionRepositoryDisabled)
            //    // Gestione dei repository di sessione non abilitata
            //    return null;
            //else

            if (context != null)
                return new SessionRepositoryFileManager(context);
            else
                return null;
        }

        protected void CheckForRepositoryExpired()
        {
            if (!Directory.Exists(SessionRepositoryFileManager.GetContextRepositoryPath(this.Context)))
                throw new ApplicationException(string.Format("Il repository '{0}' risulta scaduto e non può essere più utilizzato", this.Context.Token));
        }

        public bool ExistFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();

            return File.Exists(this.GetFilePath(fileRequest));
        }

        public DocsPaVO.documento.FileDocumento GetFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();

            this.CheckForFileExist(fileRequest);

            //2023-11-06: modifica per adeguarlo alla creazione al PiTre Cloud
            //return (DocsPaVO.documento.FileDocumento)SessionRepositorySerializerHelper.Deserialize(this.GetFilePath(fileRequest));
            //2023-11-06: FINE MODIFICA
            var fileDocumentContent = File.ReadAllText(this.GetFilePath(fileRequest));

            return JsonConvert.DeserializeObject<DocsPaVO.documento.FileDocumento>(fileDocumentContent);

        }

        protected virtual string GetFilePath(DocsPaVO.documento.FileRequest fileRequest)
        {
            if (fileRequest is DocsPaVO.documento.Allegato)
                return this.GetAttatchmentFilePath((DocsPaVO.documento.Allegato)fileRequest);
            else
                return Path.Combine(GetContextRepositoryPath(this.Context), fileRequest.version);
        }

        protected void CheckForFileExist(DocsPaVO.documento.FileRequest fileRequest)
        {
            if (!this.ExistFile(fileRequest))
                throw new ApplicationException(string.Format("File '{0}' inesistente nel repository", this.GetFilePath(fileRequest)));
        }

        protected string GetAttatchmentFilePath(DocsPaVO.documento.Allegato attatchment)
        {
            string path = this.GetOrCreateAttatchmentFolder(attatchment);
            return Path.Combine(path, attatchment.version);
        }

        protected string GetOrCreateAttatchmentFolder(DocsPaVO.documento.Allegato attatchment)
        {
            return Directory.CreateDirectory(Path.Combine(GetContextRepositoryPath(this.Context), attatchment.versionLabel)).FullName;
        }

        public void SetFile(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento file)
        {
            this.CheckForRepositoryExpired();

            //2023-11-06: modifica per adeguarlo alla creazione al PiTre Cloud
            //SessionRepositorySerializerHelper.SerializeObject(this.GetFilePath(fileRequest), file);
            //2023-11-06: FINE MODIFICA

            File.WriteAllText(this.GetFilePath(fileRequest), JsonConvert.SerializeObject(file));
        }

        public void RemoveFile(DocsPaVO.documento.FileRequest fileRequest)
        {
            this.CheckForRepositoryExpired();

            this.CheckForFileExist(fileRequest);

            File.Delete(this.GetFilePath(fileRequest));
        }

        public void Delete()
        {
            Directory.Delete(SessionRepositoryFileManager.GetContextRepositoryPath(this.Context), true);
        }

    }
}
