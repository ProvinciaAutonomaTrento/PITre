using System;

namespace HolidayChecker.Easter
{
    /// <summary>
    /// Questa interfaccia definisce le funzioni che devono essere fornite da un calcolatore
    /// del giorno di Pasqua.
    /// </summary>
    interface IEasterCalculator
    {
        /// <summary>
        /// Funzione per la determinazione del giorno di Pasqua per un determinato
        /// anno
        /// </summary>
        /// <param name="year">L'anno di cui calcolare la data della Pasqua</param>
        /// <returns>Data del giorno di Pasqua</returns>
        DateTime GetEaster(int year);
    }
}
