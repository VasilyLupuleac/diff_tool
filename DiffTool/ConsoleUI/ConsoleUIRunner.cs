using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;

namespace ConsoleUI
{
    class ConsoleUI
    {
        public static void Main(string[] args)
        {
            string path1, path2;
            if (args.Length == 2)
            {
                path1 = args[0];
                path2 = args[1];
            }
            else
            {
                Console.Write("Please enter the path to the first file:\n> ");
                path1 = Console.ReadLine();
                Console.Write("Please enter the path to the second file:\n> ");
                path2 = Console.ReadLine();
            }

            if (!File.Exists(path1) || !File.Exists(path2))
            {
                Console.WriteLine("File(s) do not exist");
                return;
            }

            var model = new Model.Model(path1, path2);
            var changes = model.ChangeBlocks;

            foreach (var changeBlock in changes)
                Print(changeBlock);
        }

        private static void Print(Model.ChangeBlock changeBlock)
        {
            const ConsoleColor insColor = ConsoleColor.Green;
            const ConsoleColor delColor = ConsoleColor.Red;

            var prevColor = Console.ForegroundColor;

            Console.WriteLine($"Line {changeBlock.StartPos + 1}:");

            Console.ForegroundColor = delColor;
            foreach (var del in changeBlock.Delete)
            {
                Console.WriteLine($"-   {del}");
            }

            Console.ForegroundColor = insColor;
            foreach (var ins in changeBlock.Insert)
            {
                Console.WriteLine($"+   {ins}");
            }

            Console.WriteLine();
            Console.ForegroundColor = prevColor;
        }
    }
}
