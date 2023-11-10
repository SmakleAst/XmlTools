using System;

namespace Lab3
{
    public class Program
    {
        static void Main(string[] args)
        {

            var xmlTools = new XmlTools("Persons.xml");

            Console.WriteLine("Первое задание:");
            xmlTools.GetPersonInfo("Иванов И.И.");

            Console.WriteLine("Второе задание:");
            xmlTools.GetWorkingEmployees();

            Console.WriteLine("Третье задание:");
            xmlTools.GetMultiDepartmentEmployees();

            Console.WriteLine("Четвертое задание:");
            xmlTools.GetDepartmentsWithFewEmployees();

            Console.WriteLine("Пятое задание:");
            xmlTools.GetMostHireAndFireYears();

            xmlTools = new XmlTools("Currency.xml");

            //R01235 - Доллар, R01090B - Белорусский рубль
            Console.WriteLine("Первое задание:");
            Console.WriteLine("Введите код валюты и диапазон отслеживания (dd.mm.yyyy): ");
            xmlTools.GetDollarSchedule(Console.ReadLine(),
                new Tuple<string, string>(Console.ReadLine(), Console.ReadLine()));

            Console.WriteLine("Второе задание:");
            Console.WriteLine("Введите диапазон отслеживания (dd.mm.yyyy): ");
            xmlTools = new XmlTools("DragMetals.xml");
            xmlTools.GetDragMetalsSchelude(new Tuple<string, string>(Console.ReadLine(), Console.ReadLine()));

            Console.WriteLine("Третье задание:");
            xmlTools = new XmlTools("Task3.xml");
            xmlTools.GetAstrakhanSanatories();

            Console.ReadLine();
        }
    }
}