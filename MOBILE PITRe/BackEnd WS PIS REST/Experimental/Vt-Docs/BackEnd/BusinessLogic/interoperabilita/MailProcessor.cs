using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Interoperabilità;
using System.Xml;
using DocsPaVO.filtri;
using log4net;

namespace BusinessLogic.Interoperabilità
{
    public class MailProcessor
    {
        private static ILog logger = LogManager.GetLogger(typeof(MailProcessor));

        private List<FiltroMailStrategy> _filtroStrategies;

        public MailProcessor(List<FiltroMail> filtri)
        {
            _filtroStrategies = FiltroMailStrategy.GetFiltriMail(filtri);
        }

        public bool IsValidMail(CMMsg message)
        {
            foreach(FiltroMailStrategy temp in _filtroStrategies){
                if (!temp.IsValidMail(message)) return false;
            }
            return true;
        }
    }

    internal abstract class FiltroMailStrategy
    {
        protected string _valore;

        public FiltroMailStrategy(string valore)
        {
            _valore = valore;
        }

        public static List<FiltroMailStrategy> GetFiltriMail(List<FiltroMail> filtri)
        {
            List<FiltroMailStrategy> res = new List<FiltroMailStrategy>();
            if(filtri==null) return res;
            filtri.FindAll(e => !string.IsNullOrEmpty(e.Valore)).ForEach(e => res.Add(GetFiltro(e)));
            return res;
        }

        private static FiltroMailStrategy GetFiltro(FiltroMail filtro)
        {
            if (filtro.Tipo == TipoFiltroMail.OGGETTO) return new FiltroOggettoMailStrategy(filtro.Valore);
            if (filtro.Tipo == TipoFiltroMail.MITTENTE) return new FiltroMittenteMailStrategy(filtro.Valore);
            return new FiltroDefaultMailStrategy(filtro.Valore);
        }

        public abstract bool IsValidMail(CMMsg message);
    }

    internal class FiltroOggettoMailStrategy : FiltroMailStrategy
    {
        private static ILog logger = LogManager.GetLogger(typeof(FiltroOggettoMailStrategy));

        public FiltroOggettoMailStrategy(string valore)
            : base(valore)
        {
        }

        public override bool IsValidMail(CMMsg message)
        {
            string oggetto = message.subject;
            logger.Debug("oggetto mail: " + oggetto + ", oggetto richiesto: " + _valore);
            return oggetto != null && oggetto.ToUpper().Equals(_valore.ToUpper());
        }
    }

    internal class FiltroMittenteMailStrategy : FiltroMailStrategy
    {
        private static ILog logger = LogManager.GetLogger(typeof(FiltroMittenteMailStrategy));

        public FiltroMittenteMailStrategy(string valore)
            : base(valore)
        {
        }

        public override bool IsValidMail(CMMsg message)
        {
            string mittente = message.from;
            logger.Debug("mittente mail: "+mittente+", mittente richiesto: "+_valore);
            return mittente.ToUpper().Equals(_valore.ToUpper());
        }
    }

    internal class FiltroDefaultMailStrategy : FiltroMailStrategy
    {
        public FiltroDefaultMailStrategy(string valore)
            : base(valore)
        {
        }

        public override bool IsValidMail(CMMsg message)
        {
            return true;
        }
    }
}
