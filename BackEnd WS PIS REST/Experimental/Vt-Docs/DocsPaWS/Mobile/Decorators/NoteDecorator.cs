using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.Mobile;
using DocsPaVO.Note;
using DocsPaVO.utente;
using log4net;

namespace DocsPaWS.Mobile.Decorators
{
    public class NoteDecorator : ListDecorator<ToDoListElement>
    {
        private InfoUtente _infoUtente;
        private ILog logger = LogManager.GetLogger(typeof(ListDecorator<ToDoListElement>));

        public NoteDecorator(InfoUtente infoUtente,List<ToDoListElement> list) : base(list){
            this._infoUtente=infoUtente;
        }

        public NoteDecorator(InfoUtente infoUtente, ListDecorator<ToDoListElement> decorator) : base(decorator){
            this._infoUtente = infoUtente;
        }

        protected override List<ToDoListElement> executeList(List<ToDoListElement> input)
        {
            logger.Info("begin");
            foreach (ToDoListElement element in input)
            {
                if (element.Tipo==ElementType.DOCUMENTO)
                {
                    logger.Debug("ricerca nota del documento con id "+element.Id);
                    AssociazioneNota an = new AssociazioneNota();
                    an.TipoOggetto = DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento;
                    an.Id = element.Id;
                    InfoNota nota = BusinessLogic.Note.NoteManager.GetUltimaNota(_infoUtente, an);
                    if (nota != null)
                    {
                        logger.Debug("nota non nulla");
                        element.Note = nota.Testo;
                    }
                }
            }
            logger.Info("end");
            return input;
        }
    }
}