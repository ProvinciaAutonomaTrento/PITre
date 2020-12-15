using System;

namespace HolidayChecker.Easter
{
    /// <summary>
    /// Enumerazione degli algoritmi disponibili per il calcolo del giorno di Pasqua
    /// </summary>
    public enum EasterAlgorithmEnum
    {
        OudinTondering
    }

    /// <summary>
    /// Questa classe si occupa di instanziare una classe per il calcolo del
    /// giorno di Pasqua e di richiederne il calcolo.
    /// </summary>
    public class EasterAlgorithmSelector
    {
        /// <summary>
        /// Funzione per il calcolo del giorno di Pasqua
        /// </summary>
        /// <param name="easterAlgorithm">Algoritmo da utilizzare per il calcolo</param>
        /// <param name="year">Anno di cui calcolare la Pasqua</param>
        /// <returns>Data di Pasqua</returns>
        public DateTime GetEasterDate(EasterAlgorithmEnum easterAlgorithm, int year)
        {
            // Istanza da utilizzare per il calcolo del giorno  di Pasqua
            IEasterCalculator calculator = null;

            // A seconda del tipo di algoritmo da utilizzare, viene instanziata
            // la classe adatta
            switch (easterAlgorithm)
            {
                case EasterAlgorithmEnum.OudinTondering:
                    calculator = new OudinTonderingEasterCalculator();
                    break;
                default:
                    // Eccezione
                    throw new Exception("Algoritmo non riconosciuto");
            }

            // Calcolo e restituzione del giorno di Pasqua
            return calculator.GetEaster(year);

        }
    }
}
