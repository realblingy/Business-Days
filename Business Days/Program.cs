using System;
using System.Collections.Generic;

/**
 * 
 * NOTE: BusinessDaysExtended does not handle holidays like Easter,
 * which uses the Moon to determine its date.
 * 
 */

namespace Business_Days
{
    public class RecurringRules
    {
        public DayOfWeek dayOfWeek;
        public int week;
        public int month;

        public RecurringRules(DayOfWeek dayOfWeek, int week, int month)
        {
            this.dayOfWeek = dayOfWeek;
            this.week = week;
            this.month = month;
        }
    }

    public class PublicHoliday
    {
        public int day;
        public int month;
        public bool recurring;
        public RecurringRules recurringRules;

        public PublicHoliday(int month, int day)
        {
            this.month = month;
            this.day = day;
            this.recurring = false;
        }

        public PublicHoliday(RecurringRules recurringRules)
        {
            this.recurring = true;
            this.recurringRules = recurringRules;
        }

        public DateTime generateDate(int year)
        {
            DateTime date;

            if (this.recurring)
            {
                date = new DateTime(year, this.recurringRules.month, 1);
                while (date.DayOfWeek != this.recurringRules.dayOfWeek)
                {
                    date = date.AddDays(1);
                }
                int daysToBeAdded = 7 * (this.recurringRules.week - 1);
                date = date.AddDays(daysToBeAdded);
            }
            else
            {
                date = new DateTime(year, this.month, this.day);
            }

            return date;
        }
    }

    class Program
    {

        static int WeekdaysBetweenTwoDates(DateTime firstDate, DateTime secondDate)
        {
            int weekdays = 0;
            firstDate = firstDate.AddDays(1);

            while (firstDate < secondDate)
            {
                DayOfWeek firstDateDay = firstDate.DayOfWeek;
                if (firstDateDay != DayOfWeek.Saturday && firstDateDay != DayOfWeek.Sunday)
                {
                    weekdays += 1;
                }
                firstDate = firstDate.AddDays(1);
            }

            return weekdays;
        }

        static int BusinessDaysBetweenTwoDates(DateTime firstDate, DateTime secondDate, IList<DateTime> publicHolidays)
        {
            int weekdays = 0;
            HashSet<string> publicHolidaysSet = new HashSet<string>();

            firstDate = firstDate.AddDays(1);

            for (int i = 0; i < publicHolidays.Count; i++)
            {
                publicHolidaysSet.Add(publicHolidays[i].ToString());
            }

            while (firstDate < secondDate)
            {
                DayOfWeek firstDateDay = firstDate.DayOfWeek;
                if (firstDateDay != DayOfWeek.Saturday
                    && firstDateDay != DayOfWeek.Sunday
                    && !publicHolidaysSet.Contains(firstDate.ToString())
                    )
                {
                    weekdays += 1;
                }
                firstDate = firstDate.AddDays(1);
            }

            return weekdays;
        }

        static int BusinessDaysBetweenTwoDatesExtended(DateTime firstDate, DateTime secondDate, IList<PublicHoliday> publicHolidays)
        {
            int weekdays = 0;
            HashSet<string> publicHolidaysSet = new HashSet<string>();
            firstDate = firstDate.AddDays(1);
            int currYear = firstDate.Year;

            for (int i = 0; i < publicHolidays.Count; i++)
            {
                publicHolidaysSet.Add(publicHolidays[i].generateDate(currYear).ToString());
            }

            while (firstDate < secondDate)
            {
                // We must update the years of all public holidays since we now disregard the year attribute in the function's input.
                DayOfWeek firstDateDay = firstDate.DayOfWeek;
                if (currYear < firstDate.Year)
                {
                    currYear = firstDate.Year;
                    publicHolidaysSet = new HashSet<string>();
                    for (int i = 0; i < publicHolidays.Count; i++)
                    {
                        publicHolidaysSet.Add(publicHolidays[i].generateDate(currYear).ToString());
                    }
                }

                if (firstDateDay != DayOfWeek.Saturday
                    && firstDateDay != DayOfWeek.Sunday
                    && !publicHolidaysSet.Contains(firstDate.ToString())
                    )
                {
                    weekdays += 1;
                }

                firstDate = firstDate.AddDays(1);
            }

            return weekdays;
        }

        static void Main(string[] args)
        {
            DateTime firstDate1 = new DateTime(2013, 10, 7);
            DateTime secondDate1 = new DateTime(2013, 10, 9);

            DateTime firstDate2 = new DateTime(2013, 10, 5);
            DateTime secondDate2 = new DateTime(2013, 10, 14);

            DateTime firstDate3 = new DateTime(2013, 10, 7);
            DateTime secondDate3 = new DateTime(2014, 1, 1);

            DateTime firstDate4 = new DateTime(2013, 10, 7);
            DateTime secondDate4 = new DateTime(2013, 10, 5);

            Console.WriteLine("Task One: Running Weekdays Between Two Dates\n");
            Console.WriteLine("October 7 2013 - October 9 2013: " + WeekdaysBetweenTwoDates(firstDate1, secondDate1));
            Console.WriteLine("October 5 2013 - October 14 2013: " + WeekdaysBetweenTwoDates(firstDate2, secondDate2));
            Console.WriteLine("October 7 2013 - January 1 2014: " + WeekdaysBetweenTwoDates(firstDate3, secondDate3));
            Console.WriteLine("October 7 2013 - October 5 2013: " + WeekdaysBetweenTwoDates(firstDate4, secondDate4));

            DateTime firstDate5 = new DateTime(2013, 12, 24);
            DateTime secondDate5 = new DateTime(2013, 12, 27);

            DateTime christmasDay = new DateTime(2013, 12, 25);
            DateTime boxingDay = new DateTime(2013, 12, 26);
            DateTime newYearDay = new DateTime(2014, 1, 1);

            List<DateTime> publicHolidays = new List<DateTime>();
            publicHolidays.Add(christmasDay);
            publicHolidays.Add(boxingDay);
            publicHolidays.Add(newYearDay);

            Console.WriteLine("\nTask Two: Running Business Days Between Two Dates\n");
            Console.WriteLine("October 7 2013 - October 9 2013: " + BusinessDaysBetweenTwoDates(firstDate1, secondDate1, publicHolidays));
            Console.WriteLine("December 24 2013 - December 27 2013: " + BusinessDaysBetweenTwoDates(firstDate5, secondDate5, publicHolidays));
            Console.WriteLine("October 7 2013 - January 1 2014: " + BusinessDaysBetweenTwoDates(firstDate3, secondDate3, publicHolidays));

            Console.WriteLine("\nTask Three: Running Business Days Between Two Dates\n");
            List<PublicHoliday> extendedPublicHolidays = new List<PublicHoliday>();

            RecurringRules queensBirthdayRules = new RecurringRules(DayOfWeek.Monday, 2, 6);
            PublicHoliday queensBirthday = new PublicHoliday(queensBirthdayRules);
            PublicHoliday randomJuneHoliday = new PublicHoliday(6, 20);

            extendedPublicHolidays.Add(queensBirthday);
            extendedPublicHolidays.Add(randomJuneHoliday);


            // There are 7 days in between 12th June and 21st June, with 12th June being Sunday.
            // Queens birthday will be on 13th June in 2022 (Monday)
            // Another random holiday has been added on 20th June (Sunday)
            // This means there should be 4 business days total.
            DateTime firstDate6 = new DateTime(2022, 6, 12);
            DateTime secondDate6 = new DateTime(2022, 6, 21);
            Console.WriteLine("June 12 2022 - June 21 2021: " + BusinessDaysBetweenTwoDatesExtended(firstDate6, secondDate6, extendedPublicHolidays));


        }
    }
}
