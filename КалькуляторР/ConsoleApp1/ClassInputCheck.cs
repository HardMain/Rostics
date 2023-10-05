using System.Text;

namespace Calculate
{
    internal class InputAndCheck
    {
        static public void Menu()
        {
            InputAndCheck check = new InputAndCheck();
            Func<int, int, int, bool> condition;

            string? errorMsg = "Неверное введеное значение. Введите еще раз.";
            bool isNonExit = true;
            string? tempMsg = "";
            int choiceMenu = 0;

            Console.WriteLine($"{new string(' ', 7)} Калькулятор множеств\n");

            condition = (countSets, prmtr2, prmtr3) => (countSets > 10 || countSets < 1);
            tempMsg = $"Введите количество множеств(до 10): ";
            int countSets = check.ClearAndEnterByCondition(tempMsg.Length, -1, tempMsg, errorMsg, condition);

            Set.SetRange();

            List<Set> sets = new List<Set>(countSets);

            for (int i = 0; i < countSets; i++)
                sets.Add(new Set());

            while (isNonExit)
            {
                condition = (choiceMenu, prmtr2, prmtr3) => (choiceMenu < 1 || choiceMenu > 6);
                tempMsg = "\t    Меню\n" +
                          "1. Выбрать множество для заполнения\n" +
                          "2. Операции над множествами\n" +
                          "3. Завершение работы программы\nВаш выбор: ";
                choiceMenu = check.ClearAndEnterByCondition(11, -1, tempMsg, errorMsg, condition);
                tempMsg = $"Выберите множество для заполнения(1 - {countSets}): ";

                switch (choiceMenu)
                {
                    case 1:
                        Set.ShowRange();
                        FillSets(tempMsg, errorMsg, sets);
                        break;
                    case 2:
                        Set.ShowRange();
                        Set.OperationsSets(sets);
                        break;
                    case 3:
                        isNonExit = false;
                        break;
                }
                if (isNonExit)
                {
                    Console.Write("Нажмите на любую клавишу для продолжения...");
                    Console.ReadKey(); Console.Clear();
                    Console.WriteLine($"{new string(' ', 7)} Добро пожаловать в калькулятор множеств!\n");
                }
            }
        }
        static private void FillSets(string msg, string errorMsg, List<Set> sets)
        {
            InputAndCheck check = new InputAndCheck();
            int choiceSet = 0; int choiceFill = 0;
            Func<int, int, int, bool> condition;

            string? msgFill = "1. Заполнить множество рандомными значениями\n" +
                              "2. Заполнить множество числами одного знака\n" +
                              "3. Заполнить множество вручную\n" +
                              "4. Заполнить множество четными/нечетными значениями\n" +
                              "5. Заполнить множество кратного числу\nВаш выбор: ";

            condition = (choiceSet, countSets, prmtr3) => (choiceSet < 1 || choiceSet > countSets);
            choiceSet = check.ClearAndEnterByCondition(msg.Length, -1, msg, errorMsg, condition, 0, sets.Capacity) - 1;

            condition = (choice, prmtr2, prmtr3) => !(0 < choice && choice < 6);
            choiceFill = check.ClearAndEnterByCondition(11, -1, msgFill, errorMsg, condition, 0);

            switch (choiceFill)
            {
                case 1:
                    sets[choiceSet].SetPower(); sets[choiceSet].FillRandom();
                    break;
                case 3:
                    sets[choiceSet].SetPower(); sets[choiceSet].FillManual();
                    break;
                case 5:
                    sets[choiceSet].FillMultiply();
                    break;
                case 2:
                    sets[choiceSet].FillSign();
                    break;
                case 4:
                    sets[choiceSet].FillParity();
                    break;
            }
            sets[choiceSet].ShowSet(choiceSet, true);
        }
        static public string CheckAndEnterFormuls(string namesSets, string namesOperations)
        {
            string? formula = "";

            formula = Console.ReadLine();
            formula = formula.ToUpper();
            formula = formula.Replace(" ", "");
            formula = formula.Replace("--", "");

            return formula;
        }

        public void ClearStrInConsole(int startSymbol, int numberRows, string? message = "", 
            ConsoleColor inColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.White, int backPositionCursorX = -1, int backPositionCursorY = -1)
        {

            if (numberRows == -1)
                numberRows = Console.CursorTop;

            Console.SetCursorPosition(startSymbol, numberRows);
            Console.ForegroundColor = inColor;
            Console.Write(new string(' ', Console.BufferWidth - startSymbol));

            if (message != "")
                Console.SetCursorPosition(startSymbol, numberRows);

            Console.Write(message);
            Console.ForegroundColor = backColor;

            if (backPositionCursorX == -1 || backPositionCursorY == -1)
                Console.SetCursorPosition(startSymbol, numberRows);
            else
            {
                Console.SetCursorPosition(backPositionCursorX, backPositionCursorY);
                ClearStrInConsole(backPositionCursorX, backPositionCursorY);
            }
        }
        public void ClearAndEnter(ref string? from, ref int to, int startSymbol, int numberRows, string? message = "", 
            ConsoleColor inColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.White, int backPositionCursorX = -1, int backPositionCursorY = -1)
        {
            ClearStrInConsole(backPositionCursorX, backPositionCursorY);
            do {
                Console.SetCursorPosition(backPositionCursorX, backPositionCursorY);
                from = Console.ReadLine();
                if (!(int.TryParse(from, out to)))
                    ClearStrInConsole(startSymbol, numberRows, message, inColor, backColor, backPositionCursorX, backPositionCursorY);
            } while (!(int.TryParse(from, out to)));
        }
        public int ClearAndEnterByCondition(int startSymbol, int backRows, string? message, string? errorMsg, 
            Func<int, int, int, bool> condition, int prmtr1 = -1, int prmtr2 = -1, int prmtr3 = -1)
        {
            InputAndCheck check = new InputAndCheck();
            string? strFrom = "";

            Console.Write(message);

            if (backRows == -1)
                backRows = Console.CursorTop;

            do {
                check.ClearAndEnter(ref strFrom, ref prmtr1, 0, backRows + 2, errorMsg, ConsoleColor.White, ConsoleColor.White, startSymbol, backRows);
                if (condition(prmtr1, prmtr2, prmtr3))
                    check.ClearStrInConsole(0, backRows + 2, errorMsg, ConsoleColor.White, ConsoleColor.White);
            } while (condition(prmtr1, prmtr2, prmtr3));
            check.ClearStrInConsole(0, backRows + 2);

            return prmtr1;
        }
    }
}