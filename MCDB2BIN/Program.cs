using System;
using System.IO;
using System.Text;

namespace MCDB2BIN
{
    internal static class Program
    {
        public static readonly string Database = "Server=localhost;Database=mcdb4;Username=chronicle;Password=chr0nicle;Pooling=true;Min Pool Size=16;Max Pool Size=32";
        public static long AllDataCounter = 0;

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
    }
}
