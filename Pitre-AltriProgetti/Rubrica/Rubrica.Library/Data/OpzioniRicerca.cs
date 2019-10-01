using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Rubrica.Library.Data
{
    /// <summary>
    /// Classe per la gestione delle opzioni di ricerca
    /// </summary>
    [Serializable()]
    public class OpzioniRicerca
    {
        /// <summary>
        /// Opzioni di ricerca dati
        /// </summary>
        public CriteriRicerca CriteriRicerca;

        /// <summary>
        /// Opzioni di paginazione dati
        /// </summary>
        /// <remarks>
        /// Se null, i dati non saranno paginati
        /// </remarks>
        public CriteriPaginazione CriteriPaginazione;

        /// <summary>
        /// Opzioni di ordinamento dati
        /// </summary>
        public CriteriOrdinamento CriteriOrdinamento;
    }

    /// <summary>
    /// Classe astratta per la gestione dei criteri di ricerca nella rubrica
    /// </summary>
    [Serializable()]
    public class CriteriRicerca
    {
        /// <summary>
        /// Criteri di ricerca
        /// </summary>
        public CriterioRicerca[] Criteri;

        /// <summary>
        /// Reperimento del filtro in formato SQL
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Reperimento dei filtri in formato SQL
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            string sql = string.Empty;

            if (this.Criteri != null)
            {
                foreach (CriterioRicerca criterio in this.Criteri)
                {
                    if (sql != string.Empty)
                        sql = string.Concat(sql, " AND ");
                    sql = string.Concat(sql, criterio.ToSql());
                }
            }

            return sql;
        }

    }

    /// <summary>
    /// Classe astratta per la gestione di un singolo criterio di ricerca nella rubrica
    /// </summary>
    [Serializable()]
    public class CriterioRicerca
    {
        /// <summary>
        /// Tipologia di ricerca
        /// </summary>
        public TipiRicercaParolaEnum TipoRicerca = TipiRicercaParolaEnum.ParolaIniziaCon;

        /// <summary>
        /// Nome del criterio di ricerca
        /// </summary>
        public string Nome;

        /// <summary>
        /// Valore da ricercare
        /// </summary>
        public object Valore;

        /// <summary>
        /// Reperimento del filtro in formato SQL
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            const string PATTERN_INIZIA_CON = "UPPER({0}) LIKE UPPER('{1}%')";
            const string PATTERN_PARTE_PAROLA = "UPPER({0}) LIKE UPPER('%{1}%')";
            const string PATTERN_PAROLA_INTERA = "UPPER({0}) = UPPER('{1}')";

            string pattern = string.Empty;
            if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIniziaCon)
                pattern = PATTERN_INIZIA_CON;
            else if (this.TipoRicerca == TipiRicercaParolaEnum.ParteDellaParola)
                pattern = PATTERN_PARTE_PAROLA;
            else if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIntera)
                pattern = PATTERN_PAROLA_INTERA;
            
            return string.Format(pattern, this.Nome.ToString(), this.Valore.ToString());
        }
    }

    /// <summary>
    /// Tipi di ricerca delle parole
    /// </summary>
    public enum TipiRicercaParolaEnum
    {
        ParolaIntera,
        ParteDellaParola,
        ParolaIniziaCon
    }

    /// <summary>
    /// Classe per il raggruppamento dei criteri di ordinamento dati
    /// </summary>
    [Serializable()]
    public class CriteriOrdinamento
    {
        /// <summary>
        /// 
        /// </summary>
        public CriterioOrdinamento[] Criteri;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            if (this.Criteri != null)
            {
                string retValue = string.Empty;

                foreach (CriterioOrdinamento item in this.Criteri)
                {
                    if (retValue != string.Empty)
                        retValue = string.Concat(retValue, ", ");
                     retValue = string.Concat(retValue, item.ToSql());
                }

                return retValue;
            }
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// Classe astratta per la gestione dei criteri di ordinamento nella rubrica
    /// </summary>
    [Serializable()]
    public class CriterioOrdinamento
    {
        /// <summary>
        /// Nome del criterio di ordinamento
        /// </summary>
        public string Nome;

        /// <summary>
        /// Tipologie di ordinamento dati
        /// </summary>
        public enum TipiOrdinamentoEnum { Asc, Desc }

        /// <summary>
        /// 
        /// </summary>
        public TipiOrdinamentoEnum Tipo;

        /// <summary>
        /// Reperimento dell'ordinamento in formato SQL
        /// </summary>
        /// <returns></returns>
        public virtual string ToSql()
        {
            return string.Format("{0} {1}", this.Nome.ToString(), this.Tipo.ToString());
        }
    }

    #region Gestione ricerca e ordinamento elementi rubrica

    ///// <summary>
    ///// Classe astratta per la gestione dei criteri di ordinamento degli elementi nella rubrica
    ///// </summary>
    //[Serializable()]
    //public class CriteroOrdinamentoElementiRubrica : CriterioOrdinamento
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public CampiRicercaElementiRubricaEnum Campo;
       
    //    /// <summary>
    //    /// Reperimento dell'ordinamento in formato SQL
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        return string.Format("{0} {1}", this.Campo.ToString(), this.Tipo.ToString());
    //    }
    //}

    ///// <summary>
    ///// Classe per la gestione di un criterio di ricerca elementi nella rubrica
    ///// </summary>
    //[Serializable()]
    //public class CriteroRicercaElementiRubrica : CriterioRicerca
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public CampiRicercaElementiRubricaEnum Campo;

    //    /// <summary>
    //    /// Reperimento del filtro in formato SQL
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        const string PATTERN_INIZIA_CON = "UPPER({0}) LIKE UPPER('{1}%')";
    //        const string PATTERN_PARTE_PAROLA = "UPPER({0}) LIKE UPPER('%{1}%')";
    //        const string PATTERN_PAROLA_INTERA = "UPPER({0}) = UPPER('{1}')";

    //        string pattern = string.Empty;
    //        if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIniziaCon)
    //            pattern = PATTERN_INIZIA_CON;
    //        else if (this.TipoRicerca == TipiRicercaParolaEnum.ParteDellaParola)
    //            pattern = PATTERN_PARTE_PAROLA;
    //        else if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIntera)
    //            pattern = PATTERN_PAROLA_INTERA;

    //        return string.Format(pattern, this.Campo.ToString(), this.Valore.ToString());
    //    }
    //}

    /// <summary>
    /// Campi utilizzati per la ricerca degli elementi in rubrica
    /// </summary>
    public enum CampiRicercaElementiRubricaEnum
    {
        Codice,  Descrizione, Citta
    }

    #endregion

    #region Gestione ricerca e ordinamento utenti in rubrica

    /// <summary>
    ///// Classe astratta per la gestione dei criteri di ordinamento degli utenti nella rubrica
    ///// </summary>
    //[Serializable()]
    //public class CriteroOrdinamentoUtenti : CriterioOrdinamento
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public CampiRicercaUtentiEnum Campo;

    //    /// <summary>
    //    /// Reperimento dell'ordinamento in formato SQL
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        return string.Format("{0} {1}", this.Campo.ToString(), this.Tipo.ToString());
    //    }
    //}

    ///// <summary>
    ///// Classe per la gestione dei criteri di ricerca utenti nella rubrica
    ///// </summary>
    //[Serializable()]
    //public class CriterioRicercaUtenti : CriterioRicerca
    //{
    //    /// <summary>
    //    /// Campo per la ricerca
    //    /// </summary>
    //    public CampiRicercaUtentiEnum Campo;

    //    /// <summary>
    //    /// Reperimento del filtro in formato SQL
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        const string PATTERN_INIZIA_CON = "UPPER({0}) LIKE UPPER('{1}%')";
    //        const string PATTERN_PARTE_PAROLA = "UPPER({0}) LIKE UPPER('%{1}%')";
    //        const string PATTERN_PAROLA_INTERA = "UPPER({0}) = UPPER('{1}')";

    //        string pattern = string.Empty;
    //        if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIniziaCon)
    //            pattern = PATTERN_INIZIA_CON;
    //        else if (this.TipoRicerca == TipiRicercaParolaEnum.ParteDellaParola)
    //            pattern = PATTERN_PARTE_PAROLA;
    //        else if (this.TipoRicerca == TipiRicercaParolaEnum.ParolaIntera)
    //            pattern = PATTERN_PAROLA_INTERA;
            
    //        return string.Format(pattern, this.Campo.ToString(), this.Valore.ToString());
    //    }
    //}

    /// <summary>
    /// Tipologie di ricerca ruolo utenti
    /// </summary>
    public enum CampiRicercaUtentiEnum
    {
        Nome
    }

    #endregion

    #region Gestione criteri paginazione

    /// <summary>
    /// Criteri necessari per la paginazione dei dati nelle ricerche degli oggetti in rubrica
    /// </summary>
    [Serializable()]
    public class CriteriPaginazione
    {
        /// <summary>
        /// 
        /// </summary>
        public CriteriPaginazione()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="oggettiPerPagina"></param>
        public CriteriPaginazione(int pagina, int oggettiPerPagina)
        {
            this.Pagina = pagina;
            this.OggettiPerPagina = oggettiPerPagina;
        }

        /// <summary>
        /// Indica il numero di pagina richiesto
        /// </summary>
        public int Pagina;

        /// <summary>
        /// Indica il numero di oggetti da includere nella ricerca per ciascuna pagina
        /// </summary>
        public int OggettiPerPagina;

        /// <summary>
        /// Indica il numero totale delle pagine restituite dalla ricerca
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata
        /// </remarks>
        /// </summary>
        public int TotalePagine;

        /// <summary>
        /// Indica il numero totale di oggetti (non paginati) restituiti dalla ricerca
        /// <remarks>
        /// Il dato è disponibile solamente come risultato della ricerca effettuata
        /// </remarks>
        /// </summary>
        public int TotaleOggetti;

        /// <summary>
        /// Impostazione numero totale record estratti
        /// </summary>
        public void SetTotaleOggetti(int totaleOggetti)
        {
            this.TotaleOggetti = totaleOggetti;

            // Determina il num di pagine totali
            this.TotalePagine = (totaleOggetti / this.OggettiPerPagina);
            
            if ((totaleOggetti % this.OggettiPerPagina) > 0)
                this.TotalePagine++;
        }
    }

    #endregion

    ///// <summary>
    ///// Classe per la gestione dei criteri di ricerca nella rubrica
    ///// </summary>
    //[Serializable()]
    //public class CriteriRicerca
    //{
    //    /// <summary>
    //    /// Lista dei filtri di ricerca
    //    /// </summary>
    //    public List<FiltroRicerca> Filtri = new List<FiltroRicerca>();

    //    /// <summary>
    //    /// Operatore logico con cui sono connessi i singoli filtri di ricerca
    //    /// </summary>
    //    public OperatoreLogicoRicercaEnum OperatoreLogico = OperatoreLogicoRicercaEnum.And;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public virtual string ToSql()
    //    {
    //        string sql = string.Empty;

    //        foreach (FiltroRicerca item in this.Filtri)
    //        {
    //            if (sql != string.Empty)
    //                sql = string.Format("{0} {1}", sql, this.OperatoreLogico.ToString());

    //            sql = string.Format("{0} {1}", sql, item.ToSql());
    //        }

    //        return sql;
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //[Serializable()]
    //[XmlInclude(typeof(FiltroRicercaStringa))]
    //[XmlInclude(typeof(FiltroRicercaNumerico))]
    //[XmlInclude(typeof(FiltroRicercaData))]
    //public abstract class FiltroRicerca
    //{
    //    /// <summary>
    //    /// Nome del filtro di ricerca
    //    /// </summary>
    //    public string Nome { get; set; }

    //    /// <summary>
    //    /// Operatore di filtro
    //    /// </summary>
    //    public OperatoreRicercaEnum Operatore { get; set; }

    //    /// <summary>
    //    /// Rappresentazione stringa SQL del filtro
    //    /// </summary>
    //    /// <returns></returns>
    //    public abstract string ToSql();

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    protected virtual string GetStringValue(Enum value)
    //    {
    //        FieldInfo fi = value.GetType().GetField(value.ToString());

    //        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

    //        if (attributes.Length > 0)
    //            return attributes[0].Description;
    //        else
    //            return value.ToString();
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //[Serializable()]
    //public class FiltroRicercaStringa : FiltroRicerca
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public TipiRicercaStringaEnum TipoRicerca = TipiRicercaStringaEnum.ParolaIntera;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public bool CaseSensitive;

    //    /// <summary>
    //    /// Valore da ricercare
    //    /// </summary>
    //    public string Valore { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        string nome = this.Nome;
    //        string operatore = this.GetStringValue(this.Operatore);
    //        string valore = this.Valore.Replace("'", "''");

    //        if (this.TipoRicerca == TipiRicercaStringaEnum.IniziaCon)
    //        {
    //            valore = string.Format("{0}%", valore);
    //            operatore = "LIKE";
    //        }
    //        else if (this.TipoRicerca == TipiRicercaStringaEnum.ParteDellaParola)
    //        {
    //            valore = string.Format("%{0}%", valore);
    //            operatore = "LIKE";
    //        }

    //        if (!this.CaseSensitive)
    //        {
    //            nome = string.Format("UPPER({0})", nome);
    //            valore = string.Format("UPPER('{0}')", valore);
    //        }
    //        else
    //            valore = string.Format("'{0}'", valore);


    //        return string.Format("{0} {1} {2}", nome, operatore, valore);
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //[Serializable()]
    //public class FiltroRicercaNumerico : FiltroRicerca
    //{
    //    /// <summary>
    //    /// Valore da ricercare
    //    /// </summary>
    //    public Int32 Valore { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public Int32 ValoreFinale { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        if (this.ValoreFinale != Int32.MinValue)
    //            return string.Format("{0} BETWEEN {1} AND {2}", this.Nome, this.Valore.ToString(), this.ValoreFinale.ToString());
    //        else
    //            return string.Format("{0} {1} {2}", this.Nome, this.GetStringValue(this.Operatore), this.Valore.ToString());
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //[Serializable()]
    //public class FiltroRicercaData : FiltroRicerca
    //{
    //    /// <summary>
    //    /// Valore da ricercare
    //    /// </summary>
    //    public DateTime Valore { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public DateTime ValoreFinale { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToSql()
    //    {
    //        if (this.ValoreFinale != DateTime.MinValue)
    //        {
    //            return string.Format("{0} BETWEEN R.FormatDate({1}) AND R.FormatDate({2})",
    //                this.Valore.ToString(), 
    //                this.ValoreFinale.ToString());
    //        }
    //        else
    //            return string.Format("{0} {1} FormatDate('{2}')", this.Nome,
    //                this.GetStringValue(this.Operatore), this.Valore.ToString());
    //    }
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public enum OperatoreLogicoRicercaEnum
    //{
    //    [Description("AND")]
    //    And,
    //    [Description("OR")]
    //    Or
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public enum OperatoreRicercaEnum
    //{
    //    [Description("=")]
    //    UgualeA,
    //    [Description(">")]
    //    MaggioreDi,
    //    [Description("<")]
    //    MinoreDi,
    //    [Description("!=")]
    //    DiversoDa
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //public enum TipiRicercaStringaEnum
    //{
    //    ParolaIntera,
    //    IniziaCon,
    //    ParteDellaParola
    //}
}