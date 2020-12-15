using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaConservazione.Metadata;
using BusinessLogic.Utenti;
using DocsPaVO.areaConservazione;

namespace DocsPaConservazione.Metadata
{
    public class UniSincroFileInfo
    {
        // addFile("Application/PDF", "7344372", "\\documenti\\20227473\\20227483.PDF.P7M", "F0C5A02F11BCB2B24F36DAF5E6B4B6F2DBE3F892B43687E5B21F8705AF7E9813");
        public Metadata.UniSincro.File[] FileList
        {
            get
            {
                return UniSincroFileList.ToArray();
            }
        }
        List<Metadata.UniSincro.File> UniSincroFileList = new List<Metadata.UniSincro.File>();
        public void addUniSincroFileInfo(string formato, string ID, string path, string impronta)
        {
            Metadata.UniSincro.File File = new Metadata.UniSincro.File
            {
                format = formato,
                ID = new Metadata.UniSincro.Identifier { Value = ID },
                Path = path,
                Hash = new Metadata.UniSincro.Hash { Value = impronta }
            };
            UniSincroFileList.Add(File);
        }
    }

    class XmlUniSincro
    {

        DocsPaConservazione.Metadata.UniSincro.IdC unisincro=null;
        public string XmlFile
        {
            get
            {

                return Utils.SerializeObject<DocsPaConservazione.Metadata.UniSincro.IdC>(unisincro,false);
            }
        }

        public DocsPaConservazione.Metadata.UniSincro.IdC Istanza
        {
            get
            {
                return unisincro;
            }
            set
            {
                value = unisincro;
            }

        }

        private Metadata.UniSincro.NameAndSurname getAgent(DocsPaVO.utente.InfoUtente infoUtenteConservazione)
        {
            DocsPaVO.utente.Utente ut = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtenteConservazione.idPeople);
            return  new Metadata.UniSincro.NameAndSurname { FirstName = ut.nome, LastName = ut.cognome };

        }


        public XmlUniSincro(Metadata.UniSincro.File[] file, DocsPaVO.utente.InfoUtente infoUtenteConservazione, InfoConservazione infoCons)
        {
            if (unisincro == null)
                unisincro = new Metadata.UniSincro.IdC();


            unisincro.SelfDescription = new Metadata.UniSincro.SelfDescription
            {
                CreatingApplication = new Metadata.UniSincro.CreatingApplication
                {
                    Name = "PITRE",
                    Producer = "NTTDATA",
                    Version = "1.0"
                }
                ,
                ID = new UniSincro.Identifier { Value = infoCons.SystemID }


            };
            unisincro.VdC = new Metadata.UniSincro.VdC
            {
                ID = new Metadata.UniSincro.Identifier
                {
                    Value = infoCons.SystemID
                }
            };

            List<Metadata.UniSincro.Agent> lstAgents = new List<Metadata.UniSincro.Agent>();
            lstAgents.Add(new Metadata.UniSincro.Agent
            {
                AgentName = new Metadata.UniSincro.AgentName { Item = getAgent (infoUtenteConservazione) },
                role = Metadata.UniSincro.AgentRole.Operator,
                type = Metadata.UniSincro.AgentType.person
            });

            Metadata.UniSincro.DetachedTimeStamp dts = new Metadata.UniSincro.DetachedTimeStamp
            {
                normal = DateTime.Now,
                format = ""
                
            };

            unisincro.Process = new Metadata.UniSincro.Process
            {
                Agent = lstAgents.ToArray(),
                TimeReference = new Metadata.UniSincro.TimeReference { Item = dts }
            };

            if (file != null)
                AddUniSincroFiles(file);
            

        }

        public void AddUniSincroFiles(Metadata.UniSincro.File[] file)
        {
            Metadata.UniSincro.FileGroup fg = new Metadata.UniSincro.FileGroup { File = file };
            List<Metadata.UniSincro.FileGroup> fgl = new List<Metadata.UniSincro.FileGroup>();
            fgl.Add(fg);
            unisincro.FileGroup = fgl.ToArray();
        }

    }
}

