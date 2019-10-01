using System;

namespace HolidayChecker.Easter
{
    /// <summary>
    /// Questa classe implementa il calcolo del giorno di Pasqua come definito dall'algoritmo
    /// di Oudin Tondering
    /// </summary>
    class OudinTonderingEasterCalculator : IEasterCalculator
    {

        public DateTime GetEaster(int year)
        {
            // Data da restituire
            DateTime easterDate;

            // Calcolo del numero d'oro - 1
            int goldNumber = year % 19;

            // Calcolo del fattore comune
            int commonNumber = year / 100;

            // Calcolo del numero di giorni dal 21 marzo al plenilunio pasquale, 
            // prima delle correzioni che tengano conto delle eccezioni che si verificano quando 
            // l'epatta è uguale a 24 o 25 (e H è uguale a 28 o 29)
            // E' pari a 23 - epatta
            int h = (commonNumber - commonNumber / 4 - (8 * commonNumber + 13) / 25 + 19 * goldNumber + 15) % 30;

            // Calcolo del numero di giorni dal 21 marzo al plenilunio pasquale, corretto però per 
            // tener conto delle eccezioni che si verificano quando l'epatta è uguale a 24 o 25 
            // (e H è uguale a 28 o 29)
            int i = h + (h / 28) * (1 - (29 / (h + 1)) * ((21 - goldNumber) / 11));

            // Calcolo del giorno della settimana del plenilunio pasquale (dove 0 = domenica, 1 = lunedì, ecc.)
            int j = (year + year / 4 + i + 2 - commonNumber + commonNumber / 4) % 7;

            // Calcolo del numero di giorni dal 21 marzo alla domenica del plenilunio pasquale 
            // o precedente il plenilunio.
            int l = i - j;

            // Calcolo del mese di Pasqua
            int month = 3 + (l + 40) / 44;

            // Calcolo del giorno di Pasqua
            int day = l + 28 - 31 * (month / 4);

            // Creazione della data di Pasqua
            easterDate = new DateTime(year, month, day);

            // Restituzione della data
            return easterDate;
        }
    }
}
