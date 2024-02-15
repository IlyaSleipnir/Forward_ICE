using Forward_ICE.Engines;
using Forward_ICE.Stands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forward_ICE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var engine = new ICE(@"..\..\Inputs\input.txt");
            var stand = new TestingStand(engine);
            double timeLimit = 1000;
            double precision = 1000;
            //engine.Logging = true;

            while (true)
            {
                Console.WriteLine("Введите температуры окружающей среды:");
                double TEnv = double.Parse(Console.ReadLine().Replace(',', '.'), CultureInfo.InvariantCulture);

                var time = stand.StartOverheatTesting(TEnv, precision, timeLimit);

                if (engine.IsTimeLimitOver)
                {
                    Console.WriteLine($"Время работы = {time} сек");
                }
                else
                {
                    Console.WriteLine($"Превышен лимит времени в {timeLimit} сек");
                }
                Console.WriteLine();

                var maxValues = stand.StartMaxPowerTesting(TEnv, precision, timeLimit);

                Console.WriteLine($"MaxN = {maxValues["MaxN"]} кВт; MaxV = {maxValues["MaxV"]} радиан/сек");
                Console.WriteLine();

                Console.WriteLine("Вы хотите выйти? (y/n):");
                var input = Console.ReadLine()[0];
                if (input == 'y')
                    break;
            }
        }
    }
}
