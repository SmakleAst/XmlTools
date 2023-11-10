using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Lab3
{
    public class XmlTools
    {
        private XDocument _document;

        public XmlTools(string documentName) =>
            _document = XDocument.Load(documentName);
        
        public void GetPersonInfo(string personName)
        {
            var employee = _document.Descendants("Employee")
                              .Where(e => e.Element("FullName").Value == personName)
                              .FirstOrDefault();

            if (employee != null)
            {
                Console.WriteLine($"Общая информация:");
                Console.WriteLine($"\tФИО: {employee.Element("FullName").Value}");
                Console.WriteLine($"\tГод рождения: {employee.Element("BirthYear").Value}");
                Console.WriteLine($"\tАдрес: {employee.Element("Address").Value}");
                Console.WriteLine($"\tТелефон: {employee.Element("Phone").Value}");
                Console.WriteLine();


                var works = employee.Descendants("WorkList")
                    .Elements()
                    .Select(w => new
                    {
                        Name = w.Element("Name").Value,
                        StartDate = DateTime.Parse(w.Element("StartDate").Value),
                        EndDate = w.Element("EndDate").Value,
                        Department = w.Element("Department").Value
                    })
                    .OrderBy(w => w.StartDate);

                Console.WriteLine("История трудовой деятельности:");
                foreach (var work in works)
                {
                    Console.WriteLine($"\tНазвание компании: {work.Name}");
                    Console.WriteLine($"\tДата начала: {work.StartDate.ToShortDateString()}");
                    Console.WriteLine($"\tДата окончания: {work.EndDate}");
                    Console.WriteLine($"\tОтдел: {work.Department}");
                    Console.WriteLine();
                }


                var salaries = employee.Descendants("SalaryList")
                    .Elements()
                    .Select(s => new
                    {
                        Year = s.Element("Year")?.Value,
                        Month = s.Element("Mounth")?.Value,
                        Size = s.Element("Size")?.Value
                    });

                Console.WriteLine("Начисления заработной платы:");
                foreach (var salary in salaries)
                {
                    Console.WriteLine($"\tГод: {salary.Year}");
                    Console.WriteLine($"\tМесяц: {salary.Month}");
                    Console.WriteLine($"\tРазмер: {salary.Size} рублей");
                    Console.WriteLine();
                }

            }
            else
            {
                Console.WriteLine($"Сотрудник с фамилией '{personName}' не найден.");
            }
        }

        public void GetWorkingEmployees()
        {
            var employees = _document.Descendants("Employee");

            var departmentData = employees.SelectMany(employee => employee.Descendants("WorkList").Elements())
                                          .Where(work => work.Element("EndDate").Value.Equals(string.Empty))
                                          .GroupBy(work => work.Element("Department").Value)
                                          .Select(group => new
                                          {
                                              Department = group.Key,
                                              NumberOfEmployees = group.Count(),
                                              Positions = group.Select(work => work.Element("Name").Value).Distinct().ToList()
                                          });

            foreach (var data in departmentData)
            {
                Console.WriteLine($"Отдел: {data.Department}");
                Console.WriteLine($"Количество работающих сотрудников: {data.NumberOfEmployees}");
                Console.WriteLine("Список должностей:");
                foreach (var position in data.Positions)
                {
                    Console.WriteLine($"- {position}");
                }
                double totalEmployees = employees.Count();
                double percentage = (data.NumberOfEmployees / totalEmployees) * 100;
                Console.WriteLine($"Доля работающих сотрудников из общего числа: {percentage:F2}%");
                Console.WriteLine(new string('-', 40));
            }
        }

        public void GetMultiDepartmentEmployees()
        {
            var employees = _document.Descendants("Employee")
                .Where(e => e.Element("WorkList")
                .Elements()
                .Count() > 1 && e.Element("WorkList")
                .Elements()
                .Any(w => w.Element("EndDate").Value == string.Empty))
                .Select(e => new
                {
                    FullName = e.Element("FullName").Value,
                    Departments = e.Element("WorkList").Elements()
                        .Where(w => w.Element("EndDate").Value == string.Empty)
                        .Select(w => new
                        {
                            Department = w.Element("Department").Value,
                            Salary = Convert.ToInt32(w.Element("Salary").Value)
                        })
                        .OrderByDescending(w => w.Salary)
                        .First()
                });

            foreach (var employee in employees)
            {
                Console.WriteLine($"Сотрудник: {employee.FullName}");
                Console.WriteLine($"Отдел: {employee.Departments.Department}");
                Console.WriteLine($"Зарплата: {employee.Departments.Salary}");
                Console.WriteLine();
            }
        }

        public void GetDepartmentsWithFewEmployees()
        {
            var employees = _document.Descendants("Employee")
                .SelectMany(e => e.Descendants("WorkList").Elements()
                .Where(work => string.IsNullOrEmpty(work.Element("EndDate").Value))
                .Select(work => work.Element("Department").Value))
                .GroupBy(department => department)
                .ToDictionary(group => group.Key, group => group.Count())
                .Where(pair => pair.Value <= 3)
                .Select(pair => pair.Key);

            Console.WriteLine("Отделы, в которых работает не более 3 сотрудников:");
            foreach (var department in employees)
            {
                Console.WriteLine(department);
            }
        }

        public void GetMostHireAndFireYears()
        {
            var maxHiredYear = _document.Descendants("Employee")
                .SelectMany(e => e.Descendants("WorkList").Elements()
                .GroupBy(x => DateTime.Parse(x.Element("StartDate").Value).Year))
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .First()
                .Year;

            var minHiredYear = _document.Descendants("Employee")
                .SelectMany(e => e.Descendants("WorkList").Elements()
                .GroupBy(x => DateTime.Parse(x.Element("StartDate").Value).Year))
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count)
                .First()
                .Year;

            var maxFiredYear = _document.Descendants("Employee")
                .SelectMany(e => e.Descendants("WorkList").Elements()
                .Where(work => !string.IsNullOrEmpty(work.Element("EndDate").Value))
                .GroupBy(x => DateTime.Parse(x.Element("EndDate").Value).Year)).Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .First()
                .Year;

            var minFiredYear = _document.Descendants("Employee")
                .SelectMany(e => e.Descendants("WorkList").Elements()
                .Where(work => !string.IsNullOrEmpty(work.Element("EndDate").Value))
                .GroupBy(x => DateTime.Parse(x.Element("EndDate").Value).Year)).Select(g => new { Year = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count)
                .First()
                .Year;

            Console.WriteLine("Год с наибольшим числом нанятых сотрудников: " + maxHiredYear);
            Console.WriteLine("Год с наименьшим числом нанятых сотрудников: " + minHiredYear);
            Console.WriteLine("Год с наибольшим числом уволенных сотрудников: " + maxFiredYear);
            Console.WriteLine("Год с наименьшим числом уволенных сотрудников: " + minFiredYear);
        }

        public void GetDollarSchedule(string code, Tuple<string, string> dates)
        {

            var currencyName = _document.Descendants("Item")
                .Where(item => item.Attribute("ID").Value == code)
                .Select(item => item.Element("Name").Value)
                .FirstOrDefault();

            if (currencyName.Equals("Белорусский рубль"))
            {
                var document = XDocument.Load("C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\ValuesBelarus.xml");

                DateTime startTime = DateTime.Parse(dates.Item1);
                DateTime endTime = DateTime.Parse(dates.Item2);

                var query = document.Descendants("Record")
                    .Where(e =>
                        DateTime.Parse(e.Attribute("Date").Value) >= startTime &&
                        DateTime.Parse(e.Attribute("Date").Value) <= endTime)
                    .Select(e => e.Element("Value")?.Value)
                    .ToList();

                using (var sr = new StreamWriter("graphic.txt"))
                {
                    foreach (var elem in query)
                    {
                        sr.Write(elem.Replace(',', '.') + " ");
                    }
                }

                Console.WriteLine(string.Join(" ", query));
            }
            else
            {
                var document = XDocument.Load("C:\\Users\\Smakl\\source\\repos\\Mirea\\TSPO\\Lab3\\Lab3\\ValuesDollar.xml");

                DateTime startTime = DateTime.Parse(dates.Item1);
                DateTime endTime = DateTime.Parse(dates.Item2);

                var query = document.Descendants("Record")
                    .Where(e =>
                        DateTime.Parse(e.Attribute("Date").Value) >= startTime &&
                        DateTime.Parse(e.Attribute("Date").Value) <= endTime)
                    .Select(e => e.Element("Value")?.Value)
                    .ToList();

                using (var sr = new StreamWriter("graphic.txt"))
                {
                    foreach (var elem in query)
                    {
                        sr.Write(elem.Replace(',', '.') + " ");
                    }
                }

                Console.WriteLine(string.Join(" ", query));
            }
        }

        public void GetDragMetalsSchelude(Tuple<string, string> dates)
        {
            DateTime fromDate = DateTime.Parse(dates.Item1);
            DateTime toDate = DateTime.Parse(dates.Item2);

            var metalRecords = _document.Descendants("Record")
                .Where(r => {
                    DateTime recordDate = DateTime.ParseExact(r.Attribute("Date").Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    return recordDate >= fromDate && recordDate <= toDate;
                })
                .GroupBy(r => r.Attribute("Code").Value)
                .Select(g => new {
                    MetalCode = g.Key,
                    SellValues = g.Select(r => decimal.Parse(r.Element("Sell").Value.Replace(",", "."), CultureInfo.InvariantCulture)),
                    MaxSell = g.Max(r => decimal.Parse(r.Element("Sell").Value.Replace(",", "."), CultureInfo.InvariantCulture)),
                    MinSell = g.Min(r => decimal.Parse(r.Element("Sell").Value.Replace(",", "."), CultureInfo.InvariantCulture))
                });

            foreach (var elem in metalRecords)
            {
                using (var sr = new StreamWriter($"{elem.MetalCode}.txt"))
                {
                    foreach (var el in elem.SellValues)
                    {
                        sr.Write(el.ToString().Replace(',', '.') + " ");
                    }
                }
            }

            foreach (var metalRecord in metalRecords)
            {
                Console.WriteLine($"Максимальная: {metalRecord.MaxSell}");
                Console.WriteLine($"Минимальная: {metalRecord.MinSell}");
                Console.WriteLine();
            }
        }

        public void GetAstrakhanSanatories()
        {
            var sanatoriums = _document.Descendants("record")
            .Where(record => record.Element("unit").Value == "Астраханская область")
            .Select(record => new
            {
                Name = record.Element("name").Value,
                LegalAddress = record.Element("legaladdress").Value,
                ActualAddress = record.Element("actualaddress").Value
            });

            foreach (var sanatorium in sanatoriums)
            {
                Console.WriteLine("Название: " + sanatorium.Name);
                Console.WriteLine("Юридический адрес: " + sanatorium.LegalAddress);
                Console.WriteLine("Фактический адрес: " + sanatorium.ActualAddress);
                Console.WriteLine();
            }
        }
    }
}
