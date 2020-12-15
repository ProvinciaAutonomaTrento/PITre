using System;
using System.Linq;
using HolidayChecker.Easter;

namespace HolidayChecker
{
    /// <summary>
    /// Questa classe fornisce il punto di accesso alle funzionalità esposte dalla libreria
    /// </summary>
    public class HolidaysChecker
    {
        /// <summary>
        /// Questa funzione verifica se un dato giorno corrisponde ad una festività
        /// </summary>
        /// <param name="day">Giorno da verificare</param>
        /// <returns>True se la data è una data festiva</returns>
        public bool IsHoliday(DateTime day, EasterAlgorithmEnum easterAlgorithm)
        {
            // Risultato
            bool isHoliday = false;

            // Calcolatore per il giorno di Pasqua
            EasterAlgorithmSelector selector = new EasterAlgorithmSelector();

            // Rimozione ore, minuti, secondi per normalizzare la data
            day = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);

            // Array dei giorni festivi
            DateTime[] festivityDayes = new DateTime[] 
            {
                new DateTime(day.Year, 1, 1),                                   //Capodanno
                new DateTime(day.Year, 1, 6),                                   //Epifania
                selector.GetEasterDate(easterAlgorithm, day.Year),              //Pasqua
                selector.GetEasterDate(easterAlgorithm, day.Year).AddDays(1),  //Lunedì dell'Angelo
                new DateTime(day.Year, 4, 25),                                  //Festa della Liberazione
                new DateTime(day.Year, 5, 1),                                   //Festa dei lavoratori
                new DateTime(day.Year, 6, 2),                                   //Festa della Repubblica
                new DateTime(day.Year, 8, 15),                                  //Ferragosto
                new DateTime(day.Year, 11, 1),                                  //Ognissanti
                new DateTime(day.Year, 12, 8 ),                                 //Immacolata Concezione
                new DateTime(day.Year, 12, 25),                                 //Natale
                new DateTime(day.Year, 12, 26)                                  //Santo Stefano
            };

            // Per prima cosa si verifica se l'array dei giorni di festa
            // contiene la data passata per parametro
            isHoliday = festivityDayes.Contains(day);

            // A questo punto si verifica se per caso il giorno della data
            // passata è Domenica o se è Sabato
            isHoliday |= day.DayOfWeek == DayOfWeek.Sunday;
            isHoliday |= day.DayOfWeek == DayOfWeek.Saturday;

            // Restituzione del risultato
            return isHoliday;

        }

        /// <summary>
        /// Questa funzione restituisce il primo giorno feriale successivo alla data 
        /// passata per parametro
        /// </summary>
        /// <param name="date">La data per cui calcolare il primo giorno feriale successivo</param>
        /// <param name="easterAlgorithm">Algoritmo da utilizzare per il calcolo del giorno di Pasqua</param>
        /// <returns>La data del primo giorno feriale successivo alla data passata per parametro</returns>
        public DateTime GetFirstWeekDayAfterDate(DateTime date, EasterAlgorithmEnum easterAlgorithm)
        {
            // Data da restituire
            DateTime toReturn = date.AddDays(1);

            // Finchè non viene trovato un giorno feriale successivo a date...
            while (this.IsHoliday(toReturn, easterAlgorithm))
                toReturn = toReturn.AddDays(1);

            // Restituzione del risultato dell'elaborazione
            return toReturn;
        }

        /// <summary>
        /// Questa funzione restituisce il primo giorno feriale a partire dalla data
        /// passata per parametro a cui viene sommato un numero n di giorni
        /// </summary>
        /// <param name="date">La data di partenza</param>
        /// <param name="n">Il numero di giorni da sommare alla data</param>
        /// <param name="easterAgorithm">Algoritmo da utilizzare per il calcolo del giorno di Pasqua</param>
        /// <returns>Il primo giorni feriale a partire da date + n</returns>
        public DateTime GetFirstWeekDayAfterDate(DateTime date, int n, EasterAlgorithmEnum easterAgorithm)
        {
            // Data da restituire
            DateTime toReturn = date.AddDays(n - 1);

            // Calcolo del primo giorno feriale successivo alla data toReturn
            return this.GetFirstWeekDayAfterDate(toReturn, easterAgorithm);

        }

    }
}
