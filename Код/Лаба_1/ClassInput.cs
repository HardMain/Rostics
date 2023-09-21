namespace Лаба_1
{
    internal class ClassInput
    {
        static private string? _enter;
        static private string? _errorMsg;

        static public void Input(out double res, string msg)
        {
            _errorMsg = "\nОшибка! Переменная должна быть вещественного типа!\n\n";
            do
            {
                Console.Write(msg);
                _enter = Console.ReadLine();
                if (!double.TryParse(_enter, out res))
                    Console.Write(_errorMsg);

            } while (!double.TryParse(_enter, out res));
        }
    }
}
