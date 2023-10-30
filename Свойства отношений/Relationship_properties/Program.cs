using System.IO;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics;

string[] strs;
string path = "C://Users//Владимир//Desktop//Политех//Дискретная математика//Лабы//Свойства отношений//Relationship_properties//file.txt";

strs = File.ReadAllLines(path);
for (int i = 0; i < strs.Length; i++)
    strs[i] = strs[i].Replace(" ", "");

int[,] matrix = new int[6, 6]; 
for (int i = 0; i < strs.Length; i++)
{
    for (int j = 0; j < strs[i].Length; j++)
		matrix[i,j] = int.Parse(strs[i][j].ToString());
}

bool[] properties = new bool[7] { true, true, true, true, true, true, true };

for (int i = 0; i < strs.Length; i++)
{
	for (int j = 0; j < strs[i].Length; j++)
	{
        if (!((i == j && matrix[i, j] == 1) || (i != j && (matrix[i, j] == 0 || matrix[i, j] == 1)))) 
			properties[0] = false;  //рефлексивное

		if (!((i == j && matrix[i, j] == 0) || (i != j && (matrix[i, j] == 1 || matrix[i, j] == 0))))
			properties[1] = false;	//антирефлексивное

		if (!(matrix[i,j] == matrix[j, i]))
			properties[2] = false;  //симметричное

		if (!((i == j && (matrix[i, j] == 1 || matrix[i, j] == 0)) || (i != j && (matrix[i, j] != matrix[j, i] || matrix[i, j] == 0 && matrix[j, i] == 0))))
			properties[3] = false;  //антисимметричное 

        if (!((i == j && matrix[i, j] == 0) || (i != j && (matrix[i, j] != matrix[j, i] || matrix[i, j] == 0 && matrix[j, i] == 0))))
            properties[4] = false;  //асимметричное 

        if (matrix[i, j] == 1)
        {
            for (int k = 0; k < 6; k++)
            {
                if (matrix[j, k] == 1 && matrix[i, k] != 1)
                    properties[5] = false;  //транзитивность                    
            }                                                        
        }                                               
                                                        
        if (!((i == j && (matrix[i, j] == 0 || matrix[i, j] == 1)) || (i != j && (matrix[i, j] == 1 || matrix[j, i] == 1))))
            properties[6] = false;	//полнота           
    }
}

string numberToWord(int i)
{
	switch (i)
	{
		case 0:
			return "Рефлексивность: ";
		case 1:
            return "Антирефлексивность: ";
		case 2:
            return "Симметричность: ";
        case 3:
            return "Антисимметричность: ";
        case 4:
            return "Ассиметричность: ";
        case 5:
            return "Транзитивность: ";
        case 6:
            return "Полнота: ";
		default:
			return "";
    }
}

for (int i = 0; i < 7; i++)
    Console.WriteLine($"{numberToWord(i)} {properties[i]}");