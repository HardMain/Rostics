﻿namespace Лаба_1
{
    internal class Exercise_2
    {
        static public void SolutionEx2()
        {
            Console.WriteLine("Exercise2\n");

            double x, y;

            Console.WriteLine("Введите координаты точки: ");

            ClassInput.Input(out x, "x?");
            ClassInput.Input(out y, "y?");

            bool res = (x * x + y * y <= 1 && !(x>0 && y>0));

            Console.Write($"\nПопадание точки в область: {res}\n");

            Console.Write("\nНажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
