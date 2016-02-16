using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostStaffing = CreateServiceHost(typeof(Service.Staffing.StaffingService), "STAFFING");
            var hostHistory = CreateServiceHost(typeof(Service.History.HistoryService), "HISTORY", enableSqlLog: false);
            var hostStorage = CreateServiceHost(typeof(Service.File.FileService), "STORAGE", enableSqlLog: false);
            var hostVacation = CreateServiceHost(typeof(Service.Vacation.VacationService), "VACATION");

            hostStaffing.Open();
            hostHistory.Open();
            hostStorage.Open();
            hostVacation.Open();

            char ch;
            do
            {
                Console.WriteLine("Press 'Q' key for exit.");
                ch = Console.ReadKey().KeyChar;
            } while (ch != 'q' && ch != 'Q');
        }

        private static ServiceHost CreateServiceHost(Type serviceType, string name, bool enabledLog = true, bool enableSqlLog = true)
        {
            var sh = new ServiceHost(serviceType);

            sh.Faulted += (_, e) => Console.WriteLine($"[-{name} FAULTED]");
            sh.Closed += (_, e) => Console.WriteLine($"[+{name} CLOSED]");
            sh.Closing += (_, e) => Console.WriteLine($"[+{name} CLOSING]");
            sh.Opened += (_, e) => Console.WriteLine($"[+{name} OPENED]");
            sh.Opening += (_, e) => Console.WriteLine($"[+{name} OPENING]");
            sh.UnknownMessageReceived += (_, e) => Console.WriteLine($"[-{name} UNKNOWN MESSAGE] {e.Message}");
            if (enabledLog)
                Service.Base.BaseService.StaticLog += (s, e) =>
                {
                    if (s.GetType() == serviceType)
                        Console.WriteLine($"[{name} LOG] {e}");
                };
            if (enableSqlLog)
                Service.Base.BaseService.StaticSqlLog += (s, e) =>
                {
                    if (s.GetType() == serviceType)
                        Console.WriteLine($"[{name} SQL LOG] {e}");
                };

            return sh;
        }
    }
}
