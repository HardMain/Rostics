using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Calculate
{
    internal class Set
    {
        static private string _namesSets = "ABCDEFGHIJ";
        static private string _namesOperations = "-()UN\\^";
        static private string _errorMessage = "";
        static private Func<int, int, int, bool> _condition = (a, b, c) => false;
        static private InputAndCheck _check = new InputAndCheck();
        static private int _startPosition = 0;
        static private int _endPosition = 0;

        private int _power;
        private List<int> _set;

        static public int StartPosition { get; set; }
        static public int EndPosition { get; set; }
        static public void SetRange()
        {
            string? strStartPosition = "";

            Console.WriteLine($"\tЗадайте универсум\nНачальное значение:\nКонечное значение:");

            int tempBackX = 20, tempBackY = Console.CursorTop - 2;
            int tempMsgY = tempBackY + 3;

            _errorMessage = "Неверное введеное значение. Введите еще раз.";
            _check.ClearAndEnter(ref strStartPosition, ref _startPosition, 0, tempMsgY, _errorMessage, ConsoleColor.Red, ConsoleColor.White, tempBackX, tempBackY);
            _check.ClearStrInConsole(0, tempMsgY);

            string? errorMsg = "Неверное введеное значение. Введите еще раз.";
            _condition = (_endPosition, _startPosition, prmtr3) => _endPosition < _startPosition;
            _endPosition = new InputAndCheck().ClearAndEnterByCondition(19, Console.CursorTop - 2, "", errorMsg, _condition, int.MinValue, _startPosition);

            StartPosition = _startPosition;
            EndPosition = _endPosition;
        }
        static public bool ShowSets(List<Set> sets, bool NonEmpty = false)
        {
            Console.WriteLine("   Заполненные множества");

            int i = 0;
            bool isEmptySets = true;

            foreach (var item in sets)
            {
                if (!item.IsEmpty(item))
                    isEmptySets = false;
                item.ShowSet(i++, false);
            }
            if (isEmptySets && !NonEmpty)
                _check.ClearStrInConsole(0, Console.CursorTop, "Нет заполненных множеств!", ConsoleColor.White, ConsoleColor.White, 0, Console.CursorTop + 2);
            if (!isEmptySets)
                Console.WriteLine();

            return isEmptySets;
        }
        static public void ShowRange()
        {
            Console.WriteLine($"\tУниверсум\nНачальное значение: {_startPosition}\nКонечное значение: {_endPosition}\n");
        }
        static public void OperationsSets(List<Set> sets)
        {
            bool isEmpty = ShowSets(sets);
            if (isEmpty)
                return;

            Console.Write("    Операции\n'u' - объединение\n'n' - пересечение\n'\\' - разность\n'^' - симметрическая разность\n'-' - дополнение\n" +
                          "'(' - открывающаяся скобка\n')' - закрывающаяся скобка\n\nВведите формулу, используя доступные множества и операции: ");

            string? formula = InputAndCheck.CheckAndEnterFormuls(_namesSets, _namesOperations);

            List<Set> calculate = new List<Set>();

            CallFirstsOperations(ref formula, sets, ref calculate);

            Console.Write("\nОтвет: ");
            if (calculate.Count == 0)
                Console.WriteLine("<< пустое множество >>\n");
            else
            {
                if (calculate[0].ShowSet(-1, true))
                    Console.WriteLine("<< пустое множество >>\n");
            }
        }
        static private void CallFirstsOperations(ref string expression, List<Set> sets, ref List<Set> calculate)
        {
            int positionOpenBracket = int.MaxValue;                   //открывающаяся скобка
            int positionAddition = int.MaxValue;                      //дополнение
            int positionUnion = int.MaxValue;                         //объединение
            int positionIntersection = int.MaxValue;                  //пересечение
            int positionDifference = int.MaxValue;                    //разность
            int positionSymmetricDifference = int.MaxValue;           //симметрическая разность
            int positionFirstOperation = -1;

            try
            {
                while (expression.Length != 0)
                {
                    positionAddition = expression.IndexOf('-') >= 0 ? expression.IndexOf('-') : int.MaxValue;                  //дополнение
                    positionUnion = expression.IndexOf('U') >= 0 ? expression.IndexOf('U') : int.MaxValue;                       //объединение
                    positionIntersection = expression.IndexOf('N') >= 0 ? expression.IndexOf('N') : int.MaxValue;               //пересечение
                    positionDifference = expression.IndexOf('\\') >= 0 ? expression.IndexOf('\\') : int.MaxValue;                //разность
                    positionSymmetricDifference = expression.IndexOf('^') >= 0 ? expression.IndexOf('^') : int.MaxValue;         //симметрическая разность
                    positionOpenBracket = expression.IndexOf('(') >= 0 ? expression.IndexOf('(') : int.MaxValue;

                    positionFirstOperation = Math.Min(Math.Min(Math.Min(positionSymmetricDifference, positionDifference), Math.Min(positionUnion, positionIntersection)),
                                             Math.Min(positionAddition, positionOpenBracket));

                    if (positionOpenBracket == positionFirstOperation)
                        CallsOperationsForBrackets(ref expression, positionOpenBracket, sets, ref calculate);
                    if (positionAddition == positionFirstOperation)
                        OperationAddition(ref expression, positionAddition, sets, ref calculate);
                    else if (positionUnion == positionFirstOperation)
                        OperationUnion(ref expression, positionUnion, sets, ref calculate);
                    else if (positionIntersection == positionFirstOperation)
                        OperationIntersection(ref expression, positionIntersection, sets, ref calculate);
                    else if (positionDifference == positionFirstOperation)
                        OperationDifference(ref expression, positionDifference, sets, ref calculate);
                    else if (positionSymmetricDifference == positionFirstOperation)
                        OperationSymmetricDifference(ref expression, positionSymmetricDifference, sets, ref calculate);
                }
            }
            catch (Exception)
            {
                _check.ClearStrInConsole(0, Console.CursorTop + 1, "Некорректный ввод формулы. Введите еще раз.", ConsoleColor.Red, ConsoleColor.White, 0, Console.CursorTop + 2);
            }
        }
        static private void OperationAddition(ref string formula, int positionAddition, List<Set> sets, ref List<Set> calculate)
        {
            char afterOperation = ' ';

            if (formula.Length > positionAddition + 1)
                afterOperation = formula[positionAddition + 1];

            if (_namesSets.Contains(afterOperation))
            {
                calculate.Add(new Set());
                for (int i = _startPosition; i <= _endPosition; i++)
                {
                    if (!sets[_namesSets.IndexOf(afterOperation)]._set.Contains(i))
                        calculate[calculate.Count - 1]._set.Add(i);
                }
                formula = formula.Substring(0, positionAddition) + formula.Substring(positionAddition + 2);
            }
            else if (afterOperation == '(')
            {
                CallsOperationsForBrackets(ref formula, positionAddition + 1, sets, ref calculate);

                Set buff = new Set(calculate[calculate.Count - 1]);
                calculate.RemoveAt(calculate.Count - 1);
                calculate.Add(new Set());

                for (int i = _startPosition; i <= _endPosition; i++)
                {
                    if (!buff._set.Contains(i))
                        calculate[calculate.Count - 1]._set.Add(i);
                }
                formula = formula.Substring(0, positionAddition) + formula.Substring(positionAddition + 1);
            }
        }
        static private void OperationUnion(ref string formula, int positionUnion, List<Set> sets, ref List<Set> calculate)
        {
            char beforeOperation = ' ';
            char afterOperation1 = ' ';
            char afterOperation2 = ' ';

            if (positionUnion > 0)
                beforeOperation = formula[positionUnion - 1];
            if (formula.Length > positionUnion + 1)
                afterOperation1 = formula[positionUnion + 1];
            if (formula.Length > positionUnion + 2)
                afterOperation2 = formula[positionUnion + 2];

            if (afterOperation1 == '-' && (afterOperation2 == '(' || _namesSets.Contains(afterOperation2)))
                OperationAddition(ref formula, positionUnion + 1, sets, ref calculate);     // "NamesSets" u - |  "NamesSets" \ "("  |
            else if (afterOperation1 == '(')
                CallsOperationsForBrackets(ref formula, positionUnion + 1, sets, ref calculate);    // u (
            else if (_namesSets.Contains(beforeOperation) && _namesSets.Contains(afterOperation1)) 
            {
                calculate.Add(new Set(sets[_namesSets.IndexOf(beforeOperation)]));
                for (int i = 0; i < sets[_namesSets.IndexOf(afterOperation1)]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(afterOperation1)]._set[i]))   //"NamesSets" u "NamesSets"
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                }
                formula = formula.Substring(0, positionUnion - 1) + formula.Substring(positionUnion + 2);
            }
            else if (beforeOperation == ' ' && _namesSets.Contains(afterOperation1))
            {
                for (int i = 0; i < sets[_namesSets.IndexOf(afterOperation1)]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(afterOperation1)]._set[i]))  // u NamesSets
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                }
                formula = formula.Substring(0, positionUnion) + formula.Substring(positionUnion + 2);
            }
            else if (_namesSets.Contains(beforeOperation) && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < sets[_namesSets.IndexOf(beforeOperation)]._set.Count; i++)
                {   
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(beforeOperation)]._set[i]))    // NamesSets u
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(beforeOperation)]._set[i]);
                }
                formula = formula.Substring(0, positionUnion - 1) + formula.Substring(positionUnion + 1);
            }
            else if (beforeOperation == ' ' && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 2]._set.Contains(calculate[calculate.Count - 1]._set[i]))    // ... u ...
                        calculate[calculate.Count - 2]._set.Add(calculate[calculate.Count - 1]._set[i]);
                }
                calculate.RemoveAt(calculate.Count - 1);
                formula = formula.Substring(0, positionUnion) + formula.Substring(positionUnion + 1);
            }
        }
        static private void OperationDifference(ref string formula, int positionDifference, List<Set> sets, ref List<Set> calculate)
        {
            char beforeOperation = ' ';
            char afterOperation1 = ' ';
            char afterOperation2 = ' ';

            if (positionDifference > 0)
                beforeOperation = formula[positionDifference - 1];
            if (formula.Length > positionDifference + 1)
                afterOperation1 = formula[positionDifference + 1];
            if (formula.Length > positionDifference + 2)
                afterOperation2 = formula[positionDifference + 2];

            if (afterOperation1 == '-' && (afterOperation2 == '(' || _namesSets.Contains(afterOperation2)))
                OperationAddition(ref formula, positionDifference + 1, sets, ref calculate);     // "NamesSets" \ - |  "NamesSets" \ "("  |
            else if (afterOperation1 == '(')
                CallsOperationsForBrackets(ref formula, positionDifference + 1, sets, ref calculate);    // \ (
            else if (_namesSets.Contains(beforeOperation) && _namesSets.Contains(afterOperation1))
            {
                calculate.Add(new Set(sets[_namesSets.IndexOf(beforeOperation)]));
                for (int i = 0; i < sets[_namesSets.IndexOf(afterOperation1)]._set.Count; i++)
                {
                    if (calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(afterOperation1)]._set[i]))   //"NamesSets" \ "NamesSets"
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                }
                formula = formula.Substring(0, positionDifference - 1) + formula.Substring(positionDifference + 2);
            }
            else if (beforeOperation == ' ' && _namesSets.Contains(afterOperation1))
            {
                char nameSet2 = formula[positionDifference + 1];
                for (int i = 0; i < sets[_namesSets.IndexOf(nameSet2)]._set.Count; i++)
                {
                    if (calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(nameSet2)]._set[i]))  // \ NamesSets
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(nameSet2)]._set[i]);
                }
                formula = formula.Substring(0, positionDifference) + formula.Substring(positionDifference + 2);
            }
            else if (_namesSets.Contains(beforeOperation) && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                char nameSet1 = formula[positionDifference - 1];
                for (int i = 0; i < sets[_namesSets.IndexOf(nameSet1)]._set.Count; i++)
                {
                    if (calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(nameSet1)]._set[i]))    // NamesSets \
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(nameSet1)]._set[i]);
                }
                formula = formula.Substring(0, positionDifference - 1) + formula.Substring(positionDifference + 1);
            }
            else if (beforeOperation == ' ' && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (calculate[calculate.Count - 2]._set.Contains(calculate[calculate.Count - 1]._set[i]))    // ... \ ...
                        calculate[calculate.Count - 2]._set.Remove(calculate[calculate.Count - 1]._set[i]);
                }
                calculate.RemoveAt(calculate.Count - 1);
                formula = formula.Substring(0, positionDifference) + formula.Substring(positionDifference + 1);
            }
        }
        static private void OperationIntersection(ref string formula, int positionIntersection, List<Set> sets, ref List<Set> calculate)
        {
            char beforeOperation = ' ';
            char afterOperation1 = ' ';
            char afterOperation2 = ' ';

            if (positionIntersection > 0)
                beforeOperation = formula[positionIntersection - 1];
            if (formula.Length > positionIntersection + 1)
                afterOperation1 = formula[positionIntersection + 1];
            if (formula.Length > positionIntersection + 2)
                afterOperation2 = formula[positionIntersection + 2];

            if (afterOperation1 == '-' && (afterOperation2 == '(' || _namesSets.Contains(afterOperation2)))
                OperationAddition(ref formula, positionIntersection + 1, sets, ref calculate);     // "NamesSets" n - |  "NamesSets" \ "("  | 
            else if (afterOperation1 == '(')
                CallsOperationsForBrackets(ref formula, positionIntersection + 1, sets, ref calculate);    // n (
            else if (_namesSets.Contains(beforeOperation) && _namesSets.Contains(afterOperation1))
            {
                calculate.Add(new Set(sets[_namesSets.IndexOf(beforeOperation)]));
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (!sets[_namesSets.IndexOf(afterOperation1)]._set.Contains(calculate[calculate.Count - 1]._set[i]))   // "NamesSets" n "NamesSets"
                    {
                        calculate[calculate.Count - 1]._set.Remove(calculate[calculate.Count - 1]._set[i]);
                        --i;
                    }
                }
                formula = formula.Substring(0, positionIntersection - 1) + formula.Substring(positionIntersection + 2);
            }
            else if (beforeOperation == ' ' && _namesSets.Contains(afterOperation1))
            {
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (!sets[_namesSets.IndexOf(afterOperation1)]._set.Contains(calculate[calculate.Count - 1]._set[i]))   // n "NamesSets" 
                    {
                        calculate[calculate.Count - 1]._set.Remove(calculate[calculate.Count - 1]._set[i]);
                        --i;
                    }
                }
                formula = formula.Substring(0, positionIntersection) + formula.Substring(positionIntersection + 2);
            }
            else if (_namesSets.Contains(beforeOperation) && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (!sets[_namesSets.IndexOf(beforeOperation)]._set.Contains(calculate[calculate.Count - 1]._set[i]))   // "NamesSets" n  
                    {
                        calculate[calculate.Count - 1]._set.Remove(calculate[calculate.Count - 1]._set[i]);
                        --i;
                    }
                }
                formula = formula.Substring(0, positionIntersection - 1) + formula.Substring(positionIntersection + 1);
            }
            else if (beforeOperation == ' ' && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < calculate[calculate.Count - 2]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(calculate[calculate.Count - 2]._set[i]))    // ... n ...  
                    {
                        calculate[calculate.Count - 2]._set.Remove(calculate[calculate.Count - 2]._set[i]);
                        --i;
                    }
                }
                calculate.RemoveAt(calculate.Count - 1);
                formula = formula.Substring(0, positionIntersection) + formula.Substring(positionIntersection + 1);
            }
        }
        static private string SearchingExpressionInBrackets(ref string formula, int positionOpenBracket)
        {
            int positionCloseBracket = -1;                  //закрывающаяся скобка
            string? middleExpression = "";                  //промежуточное выражение

            bool isExit = false;
            int countOpenBracket = 0;
            int countCloseBracket = 0;
            for (int i = positionOpenBracket; i < formula.Length && !isExit; i++)
            {
                for (int j = i; j < formula.Length && !isExit; j++)
                {
                    if (formula[j] == '(')
                        ++countOpenBracket;
                    if (formula[j] == ')')
                        ++countCloseBracket;
                    if (countOpenBracket == countCloseBracket)
                    {
                        positionCloseBracket = j;
                        isExit = true;
                    }
                }
            }
            middleExpression = formula.Substring(positionOpenBracket + 1, positionCloseBracket - (positionOpenBracket + 1));

            return middleExpression;
        }
        static private void OperationSymmetricDifference(ref string formula, int positionSymmetricDifference, List<Set> sets, ref List<Set> calculate) // !
        {
            char beforeOperation = ' ';
            char afterOperation1 = ' ';
            char afterOperation2 = ' ';

            if (positionSymmetricDifference > 0)
                beforeOperation = formula[positionSymmetricDifference - 1];
            if (formula.Length > positionSymmetricDifference + 1)
                afterOperation1 = formula[positionSymmetricDifference + 1];
            if (formula.Length > positionSymmetricDifference + 2)
                afterOperation2 = formula[positionSymmetricDifference + 2];

            if (afterOperation1 == '-' && (afterOperation2 == '(' || _namesSets.Contains(afterOperation2)))
                OperationAddition(ref formula, positionSymmetricDifference + 1, sets, ref calculate);     // "NamesSets" ^ - |  "NamesSets" \ "("  |
            else if (afterOperation1 == '(')
                CallsOperationsForBrackets(ref formula, positionSymmetricDifference + 1, sets, ref calculate);    // ^ (
            else if (_namesSets.Contains(beforeOperation) && _namesSets.Contains(afterOperation1))
            {
                calculate.Add(new Set(sets[_namesSets.IndexOf(beforeOperation)]));
                for (int i = 0; i < sets[_namesSets.IndexOf(afterOperation1)]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(afterOperation1)]._set[i]))   //"NamesSets" ^ "NamesSets"
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                    else
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                }
                formula = formula.Substring(0, positionSymmetricDifference - 1) + formula.Substring(positionSymmetricDifference + 2);
            }
            else if (beforeOperation == ' ' && _namesSets.Contains(afterOperation1))
            {
                for (int i = 0; i < sets[_namesSets.IndexOf(afterOperation1)]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(afterOperation1)]._set[i]))  // ^ NamesSets   !
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                    else
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(afterOperation1)]._set[i]);
                }
                formula = formula.Substring(0, positionSymmetricDifference) + formula.Substring(positionSymmetricDifference + 2);
            }
            else if (_namesSets.Contains(beforeOperation) && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < sets[_namesSets.IndexOf(beforeOperation)]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 1]._set.Contains(sets[_namesSets.IndexOf(beforeOperation)]._set[i]))    // NamesSets ^ !
                        calculate[calculate.Count - 1]._set.Add(sets[_namesSets.IndexOf(beforeOperation)]._set[i]);
                    else
                        calculate[calculate.Count - 1]._set.Remove(sets[_namesSets.IndexOf(beforeOperation)]._set[i]);
                }
                formula = formula.Substring(0, positionSymmetricDifference - 1) + formula.Substring(positionSymmetricDifference + 1);
            }
            else if (beforeOperation == ' ' && (afterOperation1 == ' ' || _namesOperations.Contains(afterOperation1)))
            {
                for (int i = 0; i < calculate[calculate.Count - 1]._set.Count; i++)
                {
                    if (!calculate[calculate.Count - 2]._set.Contains(calculate[calculate.Count - 1]._set[i]))    // ... ^ ...  !
                        calculate[calculate.Count - 2]._set.Add(calculate[calculate.Count - 1]._set[i]);
                    else
                        calculate[calculate.Count - 2]._set.Remove(calculate[calculate.Count - 1]._set[i]);
                }
                calculate.RemoveAt(calculate.Count - 1);
                formula = formula.Substring(0, positionSymmetricDifference) + formula.Substring(positionSymmetricDifference + 1);
            }
        }
        static private void CallsOperationsForBrackets(ref string formula, int positionOpenBracket, List<Set> sets, ref List<Set> calculate)
        {
            string? middleExpression = SearchingExpressionInBrackets(ref formula, positionOpenBracket);

            formula = formula.Remove(formula.IndexOf(middleExpression) - 1, middleExpression.Length + 2);

            CallFirstsOperations(ref middleExpression, sets, ref calculate);
        }

        public Set(int power = 0)
        {
            _power = power;
            _set = new List<int>(_power);
        }
        public Set(Set copy) : this()
        {
            _power = copy._power;
            _set.AddRange(copy._set);
        }
        public void FillRandom()
        {
            _set.Clear();

            int el;
            while (_power-- > 0)
            {
                el = new Random().Next(_startPosition, _endPosition + 1);
                if (!_set.Contains(el))
                    _set.Add(el);
                else
                    _power++;
            }

            _power = _set.Count;
        }
        public void FillManual()
        {
            _set.Clear();

            InputAndCheck check = new InputAndCheck();

            string? strEl = "";
            int el = 0; int count = 1; int length = 0;

            int numberRows = Console.GetCursorPosition().Top;

            _errorMessage = "Неверно введен элемент. Введите еще раз.";
            while (_power-- > 0)
            {
                Console.Write($"Введите {count}-й елемент: ");
                length = Convert.ToString(count).Length - 1;

                check.ClearAndEnter(ref strEl, ref el, 0, numberRows + 2, _errorMessage, ConsoleColor.White, ConsoleColor.White, 21 + length, numberRows);
                check.ClearStrInConsole(0, numberRows + 2, "", ConsoleColor.White, ConsoleColor.White, 0, numberRows + 1);

                if (_set.Contains(el) || el < _startPosition || el > _endPosition)
                {
                    check.ClearStrInConsole(0, numberRows + 2, _errorMessage, ConsoleColor.White, ConsoleColor.White, 21 + length, numberRows);
                    _power++;
                }
                else
                {
                    ++count; ++numberRows;
                    _set.Add(el);
                }
            }
            if (_power != -1)
            check.ClearStrInConsole(0, numberRows + 1);
        }
        public void FillMultiply(int mult = -1) //кратность
        {
            _set.Clear();

            if (mult == -1)
                mult = SetMultiply();

            int el = _startPosition;

            if (el % mult != 0 && el < 0)
                el += Math.Abs(el) % mult;
            else if (el % mult != 0 && el > 0)
                el += mult - el % mult;

            while (el <= _endPosition)
            {
                _set.Add(el);
                el += mult;
            }

            _power = _set.Count;
            
            if (_power == 0)
                _check.ClearStrInConsole(0, Console.CursorTop, $"Не нашлось элементов из универсума кратных '{mult}'!", ConsoleColor.White, ConsoleColor.White, 0, Console.CursorTop + 2);
        }
        public void FillParity() //четность
        {
            _set.Clear();

            int parity = SetParity();

            if (parity == 1)
                FillMultiply(2);
            else if (parity == 2)
            {
                int el = _startPosition;

                if (el % 2 == 0)
                    ++el;

                while (el <= _endPosition)
                {
                    _set.Add(el);
                    el += 2;
                }
            }

            _power = _set.Count;
        }
        public void FillSign() //знак  !!
        {
            _set.Clear();

            int sign = SetSigh();
            _errorMessage = "Не нашлось отрицательных чисел для заполнения множества!";

            int el = _startPosition;
            if (el < 0 && sign == 2)
            {
                while (el <= _endPosition && el < 0)    // 1 - неотрицательные | 2 - отрицательные
                    _set.Add(el++);
            }
            else if (el >= 0 && sign == 1)
            {
                while (el <= _endPosition)
                    _set.Add(el++);
            }
            else if (el < 0 && sign == 1 && _endPosition >= 0)
            {
                el = 0;
                while (el <= _endPosition)
                    _set.Add(el++);
            }
            else if (_startPosition < 0 && sign == 1 && _endPosition < 0)
                _check.ClearStrInConsole(0, Console.CursorTop, _errorMessage, ConsoleColor.White, ConsoleColor.White, 0, Console.CursorTop + 2);
            else if (_startPosition >= 0 && sign == 2)
                _check.ClearStrInConsole(0, Console.CursorTop, _errorMessage, ConsoleColor.White, ConsoleColor.White, 0, Console.CursorTop + 2);

            _power = _set.Count;
        }
        public bool ShowSet(int choiceSet = -1, bool esc = false)
        {
            if (_set.Count == 0)
                return true;

            if (choiceSet != -1)
                Console.Write($"Множество {_namesSets[choiceSet]}: ");

            foreach (var item in _set)
                Console.Write(item + " ");

            Console.WriteLine();
            if (esc)
                Console.WriteLine();

            return false;
        }
        public bool IsEmpty(Set set)
        {
            return set._power == 0;
        }
        public void SetPower()
        {
            string? msg = "Введите количество элементов в множестве: ";
            string? errorMsg = "Неверно введена мощность множества. Введите еще раз.";
            _condition = (_power, _startPosition, _endPosition) => _endPosition - _startPosition + 1 < _power || _power < 0;
            _power = new InputAndCheck().ClearAndEnterByCondition(42, -1, msg, errorMsg, _condition, int.MinValue, _startPosition, _endPosition);
        }
        public string NamesSets(int countSets)
        {
            string tempStr = "";
            for (int i = 0; i < countSets; i++)
                tempStr += _namesSets[i] + $" -> {i + 1}|";
            return tempStr;
        }

        private int SetMultiply()
        {
            string? msg = "Введите какому числу будет кратно множество: ";
            string? errorMsg = "Неверно введена кратность числа. Введите еще раз.";

            if (_startPosition == 0 && _endPosition == 0)
                _condition = (multiply, prmtr2, prmtr3) => multiply < 1;
            else
                _condition = (multiply, prmtr2, prmtr3) => multiply < 1 || multiply > Math.Max(Math.Abs(_startPosition), Math.Abs(_endPosition));

            return new InputAndCheck().ClearAndEnterByCondition(45, -1, msg, errorMsg, _condition, int.MinValue, _endPosition);
        }
        private int SetParity()
        {
            string? msg = "\tКакие числа входят в множество\n1)Четные\n2)Нечетные\n-> ";
            string? errorMsg = "Неверно введено значение. Введите еще раз.";
            _condition = (parity, prmtr2, prmtr3) => !(parity == 1 || parity == 2);
            int parity = new InputAndCheck().ClearAndEnterByCondition(3, -1, msg, errorMsg, _condition, 0);

            return parity; 
        }
        private int SetSigh()
        {
            string? msg = "\tКакие числа входят в множество\n1)Неотрицательные\n2)Отрицательные\n-> ";
            string? errorMsg = "Неверно введено значение. Введите еще раз.";
            _condition = (prmtr1, prmtr2, prmtr3) => !(prmtr1 == 1 || prmtr1 == 2);
            int sign = new InputAndCheck().ClearAndEnterByCondition(3, -1, msg, errorMsg, _condition, 0);

            return sign;
        }
    }
}