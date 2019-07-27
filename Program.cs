﻿using System;

namespace CalculatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();

            Calculator calculator = new Calculator();
            string answer = calculator.Compute(input);

            Console.WriteLine(answer);
        }
    }
}
