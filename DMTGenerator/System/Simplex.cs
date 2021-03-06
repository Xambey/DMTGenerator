﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMTGenerator
{
    class Simplex
    {
                //source - симплекс таблица без базисных переменных
        List<List<double>> table; //симплекс таблица
 
        int m, n;
 
        List<int> basis; //список базисных переменных
 
        public Simplex(List<List<double>> source, Function func, Operation op, List<int> _signs = null)
        {

            m = source.Count; //высота
            n = source[0].Count; //ширина

            table = new List<List<double>>();
            basis = new List<int>();

            table.AddRange(Enumerable.Repeat(default(List<double>), m));

            Random r = new Random();
            List<int> signs;
            if (op == Operation.Generate)
            {
                signs = new List<int>(4);
                signs.Add(1);
                signs.Add(1);
                signs.Add(1);
                signs.Add(1);
            }
            else
            {
                signs = _signs;
            }


            for (int i = 0, t = 0; i < m; i++)
            {
                table[i] = new List<double>();
                table[i].AddRange(Enumerable.Repeat(default(double), n + m - 1));
                for (int j = 0; j < table[0].Count; j++)
                {
                    if (j < n)
                        table[i][j] = source[i][j];
                    else
                        table[i][j] = 0;
                }
                //выставляем коэффициент 1 перед базисной переменной в строке
                if ((n + i) < table[0].Count)
                {
                    table[i][n + i] = signs[t++];
                    basis.Add(n + i);
                }
            }
            n = table[0].Count;
        }
 
        //result - в этот массив будут записаны полученные значения X
        public List<List<double>> Calculate(ref List<double> result)
        {
            int count = 0;
            int mainCol, mainRow; //ведущие столбец и строка
            while (!IsItEnd())
            {
                if (count > 50)
                    return null;
                mainCol = findMainCol();
                mainRow = findMainRow(mainCol);
                basis[mainRow] = mainCol;

                List<List<double>> new_table = new List<List<double>>(m);
                new_table.AddRange(Enumerable.Repeat(default(List<double>), m));

                for(int i = 0; i < m; i++)
                {
                    new_table[i] = new List<double>();
                    for(int j = 0; j < n; j++)
                    {
                        new_table[i].Add(new double());
                    }
                }
                

                for (int j = 0; j < n; j++)
                {
                    new_table[mainRow][j] = table[mainRow][j] / table[mainRow][mainCol];
                }
                for (int i = 0; i < m; i++)
                {
                    if (i == mainRow)
                        continue;
 
                    for (int j = 0; j < n; j++)
                        new_table[i][j] = table[i][j] - table[i][mainCol] * new_table[mainRow][j];
                }
                table = new_table;
                count++;
            }
            //заносим в result найденные значения X
            for (int i = 0; i < result.Count; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                {
                    result[i] = (table[k][0]);
                }
                else
                    result[i] = 0;
            }
 
            return table;
        }
 
        private bool IsItEnd()
        {
            bool flag = true;
 
            for (int j = 1; j < n; j++)
            {
                if (table[m - 1][j] < 0)
                {
                    flag = false;
                    break;
                }
            }
 
            return flag;
        }
 
        private int findMainCol()
        {
            int mainCol = 1;
 
            for (int j = 2; j < n; j++)
                if (table[m - 1][j] < table[m - 1][mainCol])
                    mainCol = j;
 
            return mainCol;
        }
 
        private int findMainRow(int mainCol)
        {
            int mainRow = 0;
 
            for (int i = 0; i < m - 1; i++)
                if (table[i][mainCol] > 0)
                {
                    mainRow = i;
                    break;
                }
 
            for (int i = mainRow + 1; i < m - 1; i++)
                if ((table[i][mainCol] > 0) && ((table[i][0] / table[i][mainCol]) < (table[mainRow][0] / table[mainRow][mainCol])))
                    mainRow = i;
 
            return mainRow;
        }
    }
}
