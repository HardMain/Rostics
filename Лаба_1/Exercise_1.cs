namespace Лаба_1
{
    internal class Exercise_1
    {
        static public void SolutionEx1()
        {
            Console.WriteLine("Exercise1\n");

            double m, n;

            string errorMsg = "\nОшибка! Переменная должна быть вещественного типа!\n\n";

            ClassInput.Input(out m, "m?", errorMsg);
            ClassInput.Input(out n, "n?", errorMsg);

            Console.WriteLine();

            double answer1;

            Console.WriteLine("1.");

            if (m == 1)
                Console.WriteLine("Нельзя вычислить!");
            else
            {
                answer1 = n++ / --m;
                Console.Write($"m={m} n={n} n++/--m={answer1}\n\n");
            }

            bool answer2;

            Console.WriteLine("2.");

            if (m == 0)
                Console.WriteLine("Нельзя вычислить!");
            else
            {
                answer2 = n-- > n / m++;
                Console.Write($"m={m} n={n} n-->n/m++={answer2}\n\n");
            }

            bool answer3;

            Console.WriteLine("3.");

            answer3 = m<n++;

            Console.Write($"m={m} n={n} m<n++={answer3}\n\n");

            double x;
            double answer4;

            Console.WriteLine("4.");

            ClassInput.Input(out x, "x?", errorMsg);

            answer4 = 1 + x * Math.Pow(Math.Cos(x), 2) + Math.Pow(Math.Sin(x), 3);
            Console.Write($"x={x} 1+xcos^2(x)+sin^3(x)={answer4}\n");

            Console.Write("\nНажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
