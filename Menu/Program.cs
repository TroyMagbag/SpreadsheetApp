using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetEngine;
namespace Menu
{
    static class Program
    {
        static void Main(string[] args)
        {
            int input = 1;           
            string expression = "2+2";
            ExpressionTree Tree = new ExpressionTree(expression);


            // main loop of the program 
            while (input != 4)
            {
                Console.Write("Current Expression: ");
                Console.WriteLine(expression);
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Enter a new expression.");
                Console.WriteLine("2. Set a variable value");
                Console.WriteLine("3. Evaluate Tree");
                Console.WriteLine("4. Quit");
                Console.Write("Enter command: ");
                input = Convert.ToInt32(Console.ReadLine());
                
                switch (input)
                {
                    case 1: 
                        Console.Write("Enter a new Expression: ");
                        expression = Console.ReadLine();
                        Tree = new ExpressionTree(expression);
                        break;
                   
                    case 2: 
                        Console.Write("Enter a variable name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter a value: ");
                        int value = Convert.ToInt32(Console.ReadLine());
                        Tree.SetVariable(name, value);
                        break;

                  
                    case 3: 
                        Console.WriteLine(Tree.Evaluate());
                        break;
                }
            }
        }       
    }
}
