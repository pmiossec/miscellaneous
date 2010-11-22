#region
using System;
using System.Collections.Generic;
#endregion

namespace Helper
{
    /// <summary>
    /// Calendrier des jours ouvr�s et f�ri�s fran�ais
    /// </summary>
    public static class Calendar
    {
        /// <summary>
        /// Retourne La date de Paques
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime Paques(int year)
        {
            int a = year%19;
            int b = year/100;
            int c = (b - (b/4) - ((8*b + 13)/25) + (19*a) + 15)%30;
            int d = c - (c/28)*(1 - (c/28)*(29/(c + 1))*((21 - a)/11));
            int e = d - ((year + (year/4) + d + 2 - b + (b/4))%7);
            int mois = 3 + ((e + 40)/44);
            int jour = e + 28 - (31*(mois/4));
            return new DateTime(year, mois, jour);
        }

        /// <summary>
        /// Lundi de paques
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime PaquesLundi(int year)
        {
            return Paques(year).AddDays(1);
        }

        /// <summary>
        /// La date De l'Ascenssion
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime Ascension(int year)
        {
            return Paques(year).AddDays(39);
        }

        /// <summary>
        /// Pentecote
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime Pentecote(int year)
        {
            return Paques(year).AddDays(49);
        }

        /// <summary>
        /// Lundi de Pentecote
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static DateTime PentecoteLundi(int year)
        {
            return Paques(year).AddDays(50);
        }

        /// <summary>
        /// Liste des jours feri�s
        /// </summary>
        /// <param name="year">l'ann� dont on veut la liste des jours feri�s</param>
        /// <param name="isPentecoteHoliday">true si le lundi de pentecote est consid�r� comme un jour f�ri�
        /// false sinon</param>
        /// <returns>Liste des jours feri�s</returns>
        public static List<DateTime> GetHolidays(int year, bool isPentecoteHoliday)
        {
            var holidays = new List<DateTime>();

            // jour de l'an
            holidays.Add(new DateTime(year, 1, 1));
            //'Jour de l''an');
            // 1er mai (f�te du Travail)
            holidays.Add(new DateTime(year, 5, 1));

            // 8er mai (Armistice 1945)
            holidays.Add(new DateTime(year, 5, 8));

            // 15 ao�t (Assomption)
            holidays.Add(new DateTime(year, 7, 14));

            // 15 ao�t (Assomption)
            holidays.Add(new DateTime(year, 8, 15));

            // 1er novembre (Toussaint)
            holidays.Add(new DateTime(year, 11, 1));

            // 11 Novembre (Armistice 1918)
            holidays.Add(new DateTime(year, 11, 11));

            // 25 d�cembre (No�l)
            holidays.Add(new DateTime(year, 12, 25));
            //(****************************** F�tes mobiles *********************************
            // P�ques
            //listDate.Add(Paques(year));

            // Lundi de P�ques (lundi apr�s P�ques)
            holidays.Add(PaquesLundi(year));

            // Ascension (6e jeudi (39j) apr�s P�ques)
            holidays.Add(Ascension(year));

            // Pentec�te (7e dimanche (49j) apr�s P�ques)
            //holidays.Add(Pentecote(year));

            // Lundi de la Pentec�te (lundi apr�s la Pentec�te)
            holidays.Add(PentecoteLundi(year)); //Ce n'est plus une date f�ri�e

            return holidays;
        }

        /// <summary>
        /// Prochain jour ouvr� en tenant compte que pentecote n'est pas un jour f�ri�
        /// </summary>
        /// <param name="day"></param>
        /// <returns>la date du prochain jour ouvr�</returns>
        public static DateTime GetNextOpenDay(DateTime day)
        {
            return GetNextOpenDay(day, false);
        }

        /// <summary>
        /// Prochain jour ouvr�
        /// </summary>
        /// <param name="day">la date du jour pour lequel on veut connaitre le prochain jour ouvr�</param>
        /// <param name="isPentecoteHoliday">true si le lundi de pentecote est consid�r� comme un jour f�ri�
        /// false sinon</param>
        /// <returns>la date du prochain jour ouvr�</returns>
        public static DateTime GetNextOpenDay(DateTime day, bool isPentecoteHoliday)
        {
            DateTime nextDay = day.AddDays(1);
            if (nextDay.DayOfWeek == DayOfWeek.Saturday
                || nextDay.DayOfWeek == DayOfWeek.Sunday)
            {
                return GetNextOpenDay(nextDay);
            }
            List<DateTime> holidays = GetHolidays(nextDay.Year, isPentecoteHoliday);
            foreach (DateTime holiday in holidays)
            {
                if (nextDay.Equals(holiday))
                {
                    return GetNextOpenDay(nextDay);
                }
            }
            return nextDay;
        }
    }
}