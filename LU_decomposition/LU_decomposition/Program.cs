using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LU_decomposition
{
    public interface IMatrixPrinter
    {
        void PrintMatrix<T>(T[,] array);
    }
    abstract class Matrix
    {
        protected double[,] matrix;
        public int sizeMatrix;
        public double[,] GetMatrix()
        {
            return matrix;
        }

        public int GetSizeMatrix()
        {
            return sizeMatrix;
        }

        public virtual void matrixInputFromFile()
        {
            try
            {
                // Путь к файлу для чтения
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Matrix.txt");
                string[] Lines = File.ReadAllLines(filePath);
                int rowCount = Lines.Length;
                int columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;
                if (rowCount != columnCount)
                {
                    Console.WriteLine("Матрица не является квадратной. ");
                    Console.WriteLine("Программа завершена. ");
                    Environment.Exit(0);

                }

                matrix = new double[rowCount, rowCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }

        }

        public virtual void matrixInput()
        {
            Console.WriteLine("Введите размер квадратной матрицы: ");
            sizeMatrix = Convert.ToInt32(Console.ReadLine());
            matrix = new double[sizeMatrix, sizeMatrix];

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.WriteLine("Введите " + (i + 1) + " " + (j + 1) + " " + " элемент матрицы");
                    matrix[i, j] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }

        public virtual void printMatrix()
        {
            for (int i = 0; i < sizeMatrix; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < sizeMatrix; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();

        }


    }

    class MainMatrix : Matrix
    {
        public void convergenceOfMethods() // проверка на сходимость
        {
            double[,] predominanceOfDiagonalElements = new double[sizeMatrix, 2];
            double sumOfRow = 0;
            int countDiag = 0;
            for (int i = 0; i < sizeMatrix; i++)
            {
                sumOfRow = 0;
                for (int j = 0; j < sizeMatrix; j++)
                {
                    sumOfRow += Math.Abs(matrix[i, j]);

                    if (i == j)
                    {
                        sumOfRow -= Math.Abs(matrix[i, j]);

                        predominanceOfDiagonalElements[i, 0] = Math.Abs(matrix[i, j]);
                    }
                    predominanceOfDiagonalElements[i, 1] = sumOfRow;
                }
            }

            for (int i = 0; i < sizeMatrix; i++)
            {
                if (predominanceOfDiagonalElements[i, 0] >= predominanceOfDiagonalElements[i, 1])
                {
                    countDiag++;
                }
            }
            if (countDiag == sizeMatrix)
            {
                Console.WriteLine("Итерационные методы сходятся. ");
            }
            else
            {
                Console.WriteLine("Нет сходимости итерационных методов, возможно, некоторые методы сходится не будут.");
                Console.WriteLine("Рекомендуется преобразовать матрицу и ввести ее снова.");
            }
        }

        public override void matrixInputFromFile()
        {
            Console.WriteLine("Введенная матрица A: ");
            base.matrixInputFromFile();
        }

    }

    class FreeMatrix : Matrix
    {
        private int rowCount;
        private int columnCount;
        public override void matrixInputFromFile()
        {
            try
            {
               
                // Путь к файлу для чтения
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FreeMembers.txt");
                string[] Lines = File.ReadAllLines(filePath);
                rowCount = Lines.Length;
                columnCount = Lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                sizeMatrix = rowCount;

                matrix = new double[rowCount, columnCount];

                for (int i = 0; i < rowCount; i++)
                {
                    string[] values = Lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(values[j]);
                    }
                }


            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении файла: " + e.Message);

            }
        }

        public override void printMatrix()
        {
            Console.WriteLine("Введенный вектор свободных членов B: ");
            for (int i = 0; i < rowCount; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < columnCount; j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
            }
            Console.WriteLine();
        }
        public void matrixVector(int sizeMatrix)
        {

            Console.WriteLine("Введите вектор свободных значений");

            matrix = new double[sizeMatrix, 1];

            for (int i = 0; i < sizeMatrix; i++)
            {
                {
                    Console.WriteLine("Введите " + (i + 1) + " элемент вектора");
                    matrix[i, 0] = Convert.ToDouble(Console.ReadLine());
                }
            }
        }
    }

    class SLAE: IMatrixPrinter
    {

        MainMatrix mainMatrix = new MainMatrix();
        FreeMatrix freeMembers = new FreeMatrix();

        private double[,] Amatrix;
        private double[,] Free;
        private double[,] Lmatrix;
        private double[,] Umatrix;

        private double determinant;
        int size;
        
        public SLAE(MainMatrix matrix, FreeMatrix freeMembers)
        {
            Amatrix = matrix.GetMatrix();
            Free = freeMembers.GetMatrix();
            this.mainMatrix = matrix;
            this.freeMembers = freeMembers;
            Lmatrix = new double[mainMatrix.sizeMatrix, mainMatrix.sizeMatrix];
            Umatrix = new double[mainMatrix.sizeMatrix, mainMatrix.sizeMatrix];
            size = mainMatrix.GetSizeMatrix();

        }
        public void PrintMatrix<T>(T[,] array)
        {
            for (int i = 0; i < size; i++)
            {
                if (i != 0)
                {
                    Console.WriteLine();
                }

                for (int j = 0; j < size; j++)
                {
                    Console.Write(array[i, j] + " ");
                }
            }
        }
        public void LUdecomposition()
        {

            for (int i = 0; i < size; i++)
            {
                
                for (int j = 0; j < size; j++)
                {
                    Umatrix[i, j] = 0;
                    Lmatrix[i, j] = 0;
                }
                Lmatrix[i, i] = 1;
                
            }

            double sigma = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i <= j)
                    {
                        sigma = 0;
                        for (int k = 0; k <= i; k++)
                        {
                            sigma = sigma + Lmatrix[i, k] * Umatrix[k, j];
                            
                        }
                        
                        Umatrix[i, j] = Amatrix[i, j] - sigma;
                        //Console.WriteLine(Umatrix[i, j]);
                        //Console.WriteLine("Umatrix = " + Umatrix[i, j] + " i = " + i + " j = " + j);
                        //Console.WriteLine("sigma = " + sigma);
                        

                    }

                    if (i > j)
                    {
                        sigma = 0;
                        for (int k = 0; k <= j; k++)
                        {
                            sigma = sigma + Lmatrix[i, k] * Umatrix[k, j];
                           
                        }
                        Lmatrix[i, j] = (Amatrix[i, j] - sigma) / Umatrix[j, j];
                       
                    }
                }

            }


            Console.WriteLine("Lmatrix: ");
            PrintMatrix(Lmatrix);
            Console.WriteLine();
            Console.WriteLine("Umatrix: ");
            PrintMatrix(Umatrix);
            Console.WriteLine();
        }
        public void SLAESolution()
        {
            double[] Yvalues = new double[size];  // находим решение системы Ly = b
            double[] Xvalues = new double[size]; // находим решение системы Ux = y
            for (int i = 0; i < size; i++)
            {
                Yvalues[i] = 1;
                Xvalues[i] = 1;
            }
            double sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum = 0;
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        sum = sum + Lmatrix[i, j] * Yvalues[j];
                    }
                    
                }
                Yvalues[i] = (Free[i, 0] - sum) / Lmatrix[i, i];
                //Console.WriteLine("y =  " + Yvalues[i]);
            }

            for (int i = 0; i < size; i++)
            {
                sum = 0;
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        sum = sum + Umatrix[size - i - 1, size - j - 1] * Xvalues[size - j - 1];
                    }

                }
                Xvalues[size - i - 1] = (Yvalues[size - i - 1] - sum) / Umatrix[size - i - 1, size - i - 1];
               
            }
            Console.WriteLine();
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine("X" + (i + 1) + " = " + Xvalues[i]);
            }

        }

        public void determinantCalculation()
        {
            double detL = 1;
            double detU = 1;
            for (int i = 0; i < size; i++)
            {
                detL = detL * Lmatrix[i, i];
                detU = detU * Umatrix[i, i];
            }
            determinant = detU * detL;
            Console.WriteLine("Опеределитель матрицы A = " + determinant);
        }
    }
    internal class Program
    {


        static void Main(string[] args)
        {
            int choice1;
            int choice2;

            

            MainMatrix matrix1 = new MainMatrix();
            Console.WriteLine();
            Console.WriteLine("Матрицы хранятся в папке проекта, для редактирования файлов используйте путь: ");
            Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice1 = Convert.ToInt32(Console.ReadLine());
            if (choice1 == 1)
            {
                matrix1.matrixInput();
            }
            if (choice1 == 2)
            {
                matrix1.matrixInputFromFile();
            }

            matrix1.printMatrix();

            FreeMatrix matrix2 = new FreeMatrix();
            Console.WriteLine("Выберите предложенный вариант - введите соответствующее варианту число ");
            Console.WriteLine("1. Ввести матрицу вручуню");
            Console.WriteLine("2. Ввести матрицу из файла Matrix.txt");
            choice2 = Convert.ToInt32(Console.ReadLine());
            if (choice2 == 1)
            {
                matrix2.matrixVector(matrix1.sizeMatrix);
            }
            if (choice2 == 2)
            {
                matrix2.matrixInputFromFile();
            }

            matrix2.printMatrix();

           
            SLAE slae = new SLAE(matrix1, matrix2);
            slae.LUdecomposition();
            slae.SLAESolution();
            slae.determinantCalculation();

        }
    }
}
