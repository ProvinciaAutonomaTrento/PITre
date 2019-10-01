using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Procedimento;

namespace BusinessLogic.Procedimenti
{
    public class AnalisiManager
    {
        /// <summary>
        /// Calcola la durata media di un procedimento da una lista di istanze
        /// </summary>
        /// <param name="procedimenti"></param>
        /// <returns></returns>
        public static Double DurataMediaProcedimento(List<DettaglioProcedimento> istanze)
        {
            Double result = 0;
            int countProcedimentiChiusi = 0;
            int sommaDurata = 0;

            foreach (DettaglioProcedimento p in istanze)
            {
                if (p.Stati.Last().StatoFinale)
                {
                    int durata = (p.Stati.Last().DataStato - p.Stati.First().DataStato).Days;
                    sommaDurata = sommaDurata + durata;
                    countProcedimentiChiusi++;
                }
            }

            if (countProcedimentiChiusi > 0)
            {
                result = Math.Round((double)sommaDurata / (double)countProcedimentiChiusi, 1, MidpointRounding.AwayFromZero);
            }
            else
            {
                result = -1;
            }

            return result;
        }

        public static Dictionary<string, double> DurataMediaFasiProcedimento(List<DettaglioProcedimento> istanze, string idTipoFasc, string idAmm)
        {
            Dictionary<string, double> conteggioPerFasi = new Dictionary<string, double>();

            Dictionary<string, double> conteggioPerStati = new Dictionary<string, double>();
            List<DettaglioProcedimento> istanzeChiuse = istanze.Where(i => i.Stati.Last().StatoFinale).ToList();

            if (istanzeChiuse.Count > 0)
            {
                // Recupero diagramma di stati
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoFasc(idTipoFasc, idAmm);

                // Costruisco 
                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                {
                    conteggioPerStati.Add(stato.SYSTEM_ID.ToString(), 0);
                }

                foreach (DettaglioProcedimento procedimento in istanzeChiuse)
                {
                    for (int i = 0; i < procedimento.Stati.Count(); i++)
                    {
                        double val;
                        if (conteggioPerStati.TryGetValue(procedimento.Stati[i].IdStato, out val))
                        {
                            if (i < procedimento.Stati.Count() - 1)
                            {
                                conteggioPerStati[procedimento.Stati[i].IdStato] += (procedimento.Stati[i + 1].DataStato - procedimento.Stati[i].DataStato).Days;
                            }
                        }
                    }
                }

                List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> assStatiDiagrammi = BusinessLogic.DiagrammiStato.DiagrammiStato.GetFasiStatiDiagramma(diagramma.SYSTEM_ID.ToString(), null);

                // Aggrego i conteggi per stato in conteggi per fase
                foreach (DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma statoDiagramma in assStatiDiagrammi)
                {
                    if (!conteggioPerFasi.ContainsKey(statoDiagramma.PHASE.DESCRIZIONE))
                    {
                        conteggioPerFasi.Add(statoDiagramma.PHASE.DESCRIZIONE, 0f);
                    }

                    conteggioPerFasi[statoDiagramma.PHASE.DESCRIZIONE] += conteggioPerStati[statoDiagramma.STATO.SYSTEM_ID.ToString()];
                }

                // Calcolo le medie
                List<KeyValuePair<string, double>> fasi = new List<KeyValuePair<string, double>>(conteggioPerFasi);
                foreach (KeyValuePair<string, double> kvp in fasi)
                {
                    conteggioPerFasi[kvp.Key] = kvp.Value / (double)istanzeChiuse.Count;
                }
            }



            return conteggioPerFasi;
        }

        public static int ProcedimentiChiusi(List<DettaglioProcedimento> istanze)
        {
            int result = 0;

            foreach (DettaglioProcedimento p in istanze)
            {
                if (p.Stati.Last().StatoFinale)
                {
                    result++;
                }
            }

            return result;
        }
    }
}
