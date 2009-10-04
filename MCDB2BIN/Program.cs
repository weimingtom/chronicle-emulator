using System;
using System.IO;
using System.Text;

namespace MCDB2BIN
{
    internal static class Program
    {
        public static readonly string Database = "Server=localhost;Database=mcdb;Username=chronicle;Password=chr0nicle;Pooling=true;Min Pool Size=4;Max Pool Size=16";
        public static long AllDataCounter = 0;
        public static int TotalCount = 0;
        public static int CurrentCount = 0;
        public static int CurrentPercent = 0;

        private static void Main()
        {
            Console.WriteLine("  {0,-24}___{1,-16}___{2,-24}", new string('_', 24), new string('_', 16), new string('_', 24));
            Console.WriteLine(" /{0,-24}   {1,-16}   {2,-24}\\", "Data", "Count", "Time");
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", new string('-', 24), new string('-', 16), new string('-', 24));
            PerformanceTimer timer = new PerformanceTimer();
            timer.Unpause();

            using (BinaryWriter writer = new BinaryWriter(new FileStream("Data.bin", FileMode.Create, FileAccess.Write), Encoding.ASCII))
            {
                AbilityExport.Export(writer);
                SkillExport.Export(writer);
                NPCExport.Export(writer);
                ReactorExport.Export(writer);
                MobExport.Export(writer);
                QuestExport.Export(writer);
                ItemExport.Export(writer);
                MapExport.Export(writer);
            }

            timer.Pause();
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", new string('-', 24), new string('-', 16), new string('-', 24));
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "All", AllDataCounter, timer.Duration);
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", new string('-', 24), new string('-', 16), new string('-', 24));
            Console.WriteLine(" \\{0,-24}___{1,-16}___{2,-24}/", new string('_', 24), new string('_', 16), new string('_', 24));
            Console.WriteLine();
            Console.Write("Press Any Key To Exit");
            Console.ReadKey(true);
        }

        public static void ResetCounter(int pTotalCount)
        {
            CurrentPercent = 0;
            CurrentCount = 0;
            TotalCount = pTotalCount;
            Console.Title = "Progress: 0%";
        }

        public static void IncrementCounter()
        {
            ++CurrentCount;
            int percent = (CurrentCount * 100) / TotalCount;
            if (percent > CurrentPercent)
            {
                CurrentPercent = percent;
                Console.Title = "Progress: " + CurrentPercent + "%";
            }
        }
    }
}
