namespace Лаба_1
{
    internal class Exercise_3
    {
        static public void SolutionEx3()
        {
            Console.WriteLine("Exercise3\n");

            float a1 = 1000f;
            float b1 = 0.0001f;
            float res1 = Formula(a1, b1);

            double a2 = 1000;
            double b2 = 0.0001; 
            double res2 = Formula(a2, b2);

            Console.WriteLine($"float res = {res1}\ndouble res = {res2}");
        }
        static private float Formula(float a, float b)
        {
            float expr1 = (float)Math.Pow(a - b, 4);
            float expr2 = a * a * a * a - 4 * a * a * a * b;
            float expr3 = 6 * a * a * b * b - a * a * b * b * b + b * b * b * b;

            float res = (expr1 - expr2) / expr3;

            return res;
        }
        static private double Formula(double a, double b)
        {
            double expr1 = Math.Pow(a - b, 4);
            double expr2 = a * a * a * a - 4 * a * a * a * b;
            double expr3 = 6 * a * a * b * b - a * a * b * b * b + b * b * b * b;

            double res = (expr1 - expr2) / expr3;

            return res;
        }
    }
}
