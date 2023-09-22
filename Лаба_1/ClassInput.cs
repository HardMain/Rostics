namespace Лаба_1
{
    internal class ClassInput
    {
        static public void Input(out double res, string msg, string errorMsg)
        {
            string? enter;
            do
            {
                Console.Write(msg);
                enter = Console.ReadLine();
                if (!double.TryParse(enter, out res))
                    Console.Write(errorMsg);

            } while (!double.TryParse(enter, out res));
        }
    }
}
