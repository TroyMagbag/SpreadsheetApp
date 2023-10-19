/// <summary>
/// Troy Magbag
/// 11536876
/// Code for Spreadsheet form.
/// </summary>
namespace SpreadsheetEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// Cell class 
    /// </summary>
    public abstract class Cell
    {
        protected string text;
        protected string Value;

        private readonly int row = 0;
        private readonly int columns = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Constructor for cell. 
        /// </summary>
        /// <param name="Row">Max rows</param>
        /// <param name="Col">Max columns</param>
        internal Cell(int Row, int Col)
        {
            row = Row;
            columns = Col;
        }

        /// <summary>
        /// returns number of rows.
        /// </summary>
        public int RowIndex
        {
            get { return this.row; }
        }

        /// <summary>
        /// returns number of columns
        /// </summary>
        public int ColumnIndex
        {
            get { return this.columns; }
        }

        /// <summary>
        ///Getter and setter for text field  
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text) //Do not change if value is same as text.
                {
                    this.text = value;
                    this.OnPropertyChanged("text");

                }
            }
        }

        /// <summary>
        /// Geter and setter for Value field
        /// </summary>
        public string Values
        {
            get { return this.Value; }
            internal set
            {
                if (value != this.Value) // Do not change if values are same.
                {
                    this.Value = value;
                    this.OnPropertyChanged("Values");
                }
            }
        }

        /// <summary>
        /// Raises event when property is changed.
        /// </summary>
        /// <param name="name">Name of propery </param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));

            }
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// SpreadsceetCell inherits from abstract class.
    /// To be used in Spreadsheet class to use a cell object.
    /// </summary>
    public class SpreadsheetCell : Cell
    {
        public SpreadsheetCell(int rowIndex, int colIndex) : base(rowIndex, colIndex)
        {
        }
    }


    /// <summary>
    /// Actual Spreadsheet oject.
    /// Uses a 2D array to repersent spreadsheet.
    /// </summary>
    public class Spreadsheets
    {
        private int numRows, numCol;
        public Cell[,] spreadsheetCells;
        public event PropertyChangedEventHandler PropertyChanged;

        public Spreadsheets(int rows, int col)
        {
            numRows = rows;
            numCol = col;
            spreadsheetCells = new Cell[numRows, numCol];
            for (int numRow = 0; numRow < rows; numRow++) // Initalize all the cells for 2D array
            {
                for (int numCol = 0; numCol < col; numCol++)
                {
                    spreadsheetCells[numRow, numCol] = new SpreadsheetCell(numRow, numCol); //initalize cell 
                    spreadsheetCells[numRow, numCol].PropertyChanged += new PropertyChangedEventHandler(CellPropertyChanged);
                }
            }

        }

        /// <summary>
        /// Lets UI know when property changes.
        /// </summary>
        /// <param name="sender">Object.</param>
        /// <param name="e">Property being changed.</param>
        public void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "text")
            {
                if (((Cell)sender).Text[0] != '=')
                {
                    ((Cell)sender).Values = ((Cell)sender).Text;
                }
            }

            if (e.PropertyName == "Values")
            {
                if (((Cell)sender).Text[0] == '=')
                {
                    string algorithm = ((Cell)sender).Text.TrimStart('=');
                    /*int column = Convert.ToInt16(algorithm[0]) - 'A';
                    int row = Convert.ToInt16(algorithm.Substring(1)) - 1;
                    ((Cell)sender).Values = (GetCell(row, column)).Values;*/
                   ExpressionTree Tree = new ExpressionTree(algorithm);

                    //get list of variables and get values from the corresponding cells
                    List<string> variables = Tree.getVars();
                    foreach (string cell in variables)
                    {
                        int column = Convert.ToInt16(cell[0]) - 'A';
                        int row = Convert.ToInt16(cell.Substring(1)) - 1;
                        double value;
                        if (Double.TryParse(GetCell(row, column).Values, out value))
                        {
                            Tree.SetVariable(cell, value);
                        }
                        else
                        {
                            Tree.SetVariable(cell, 0);
                        }

                    }

                    ((Cell)sender).Values = Tree.Evaluate().ToString();

                }

            }
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs("Values"));
        }
        /// <summary>
        /// Returns the cell of given row and column number.
        /// </summary>
        /// <param name="row">Row number.</param>
        /// <param name="col">Column number</param>
        /// <returns></returns>
        public Cell GetCell(int row, int col)
        {
            if (row < 0 || row > numRows && col < 0 || col > numCol) //if not a valid cell return null 
            {
                return null;
            }

            return spreadsheetCells[row, col];
        }

        /// <summary>
        /// Returns number of columns in spreadsheet.
        /// </summary>
        /// <returns>The number of columns in spreadsheet.</returns>
        public int ColumnCount()
        {
            return numCol;
        }

        /// <summary>
        /// Returns number of rows in spreadsheet.
        /// </summary>
        /// <returns>The number of rows in spreadsheet.</returns>
        public int RowCount()
        {
            return numRows;
        }

    }
    public class ExpressionTree
    {
        private List<string> variable = new List<string>();
        private Stack<Node> ExpressTree = new Stack<Node>();
        private Dictionary<string, double> variables = new Dictionary<string, double>();
        private List<string> operators = new List<string>();

        
        public List<string> getVars()
        {
            return this.variable;
        }

        /// <summary>
        /// Changes oder of expression to be evaluated
        /// </summary>
        /// <param name="expression">List of strings that contains the parts of the expression</param>
        /// <returns>New List to be evaluated</returns>
        private List<string> EvalList(List<string> expression)
        {
            List<string> curList = new List<string>();
            Stack<string> values = new Stack<string>();

            foreach (string variable in expression)
            {
                if (!operators.Contains(variable))
                {
                    curList.Add(variable); // Adds variable or contant to list
                }

                else if (variable == "(") //looks for parentheses
                {
                    values.Push(variable); 
                }
                else if (variable == ")")
                {
                    string popped = values.Pop();
                    while (popped != "(")
                    {
                        curList.Add(popped);
                        popped = values.Pop();
                    }
                }
                else
                {
                    if (values.Count == 0)
                    {
                        values.Push(variable); // Adds operator to stack
                    }
                    else
                    {
                        while (Precedence(values.Peek()) >= Precedence(variable)) // if precedence is greater than the operator on the stack
                        {
                            curList.Add(values.Pop()); // adds operator on top of stack to the list
                            if (values.Count == 0)
                            {
                                break;
                            }
                        }
                        values.Push(variable); // Add variable to stack.
                    }
                }
            }
            while (values.Count != 0)
            {
                curList.Add(values.Pop()); // Adds rest to list
            }

            return curList;
        }

        /// <summary>
        /// Returns a value depending the precedence.
        /// </summary>
        /// <param name="op"></param>
        /// <returns>Precedence value</returns>
        private int Precedence(string op)
        {
            if (op == "+" || op == "-") //if operator is add or subtract
            {
                return 1;
            }
            if (op == "*" || op == "/") //if operator is multiply or divide
            {
                return 2;
            }
            else
            {
                return  0; // not a supported operation
            }
        }


        /// <summary>
        /// Evaluates the Node type
        /// </summary>
        /// <param name="node">Starting node</param>
        /// <returns>The value depending on the type of node</returns>
        private double Evaluate(Node node)
        {
            ConstantNode constantNode = node as ConstantNode;
            if ( constantNode != null) //constant number
            {
                return constantNode.ConstNode;
            }

            VariableNode variableNode = node as VariableNode;
            if (variableNode != null) //if variable 
            {
                return variables[variableNode.Variablenode];
            }

            OperatorNode operatorNode = node as OperatorNode;
            if (operatorNode != null) //if operator 
            {
                // but which one?
                switch (operatorNode.opType)
                {
                    case "+":
                        return Evaluate(operatorNode.Left) + Evaluate(operatorNode.Right); //adds
                    case "-":
                        return Evaluate(operatorNode.Left) - Evaluate(operatorNode.Right); //subtracts
                    case "*":
                        return Evaluate(operatorNode.Left) * Evaluate(operatorNode.Right); //mulitply
                    case "/":
                        return Evaluate(operatorNode.Left) / Evaluate(operatorNode.Right); //divide
                }
            }
            throw new NotSupportedException(); //if none of these throw an exception;
        }

        /// <summary>
        /// Takes current expression and adds each part into a string list to be used to be evaluated.
        /// </summary>
        /// <param name="expression">Current expression</param>
        /// <returns>Expression list to be evaluated.</returns>
        private List<string> GetOperators(string expression)
        {
            List<string> express = new List<string>(); //expression
            string oper = null; //operator            
            foreach (char cur in expression)
            {
                if (cur == '+' || cur == '-' || cur == '*' || cur == '/' || cur == '(' || cur == ')')
                {
                    //express.Add(oper);
                    //express.Add(cur.ToString()); // Add operator to express list.
                    operators.Add(cur.ToString()); // Add to gloabal operators.
                    //oper = null;
                
                    if (oper != null)
                    {
                        express.Add(oper);
                        express.Add(cur.ToString());
                        oper = null;
                    }
                    else
                    {
                        express.Add(cur.ToString());
                    }
                }

                else
                {
                    oper += cur; // Adds to string. 
                }
            }
            if (oper != null)
            {
                express.Add(oper); // Adds tp expresslist. 
            }

            return express;
        }

        /// <summary>
        /// Builds the expression tree using the in expression input.
        /// </summary>
        /// <param name="expression">The expression input by the user</param>
        public ExpressionTree(string expression)
        {
            List<string> express = EvalList(GetOperators(expression));
            double number;
            foreach (string op in express)
            {
                if (!operators.Contains(op))
                {

                    if (double.TryParse(op, out number))
                    {
                        ConstantNode node = new ConstantNode();
                        node.ConstNode = number;
                        ExpressTree.Push(node); //add constant number
                    }
                    else
                    {
                        VariableNode node = new VariableNode();
                        variables.Add(op, 0);
                        variable.Add(op);
                        node.Variablenode = op;
                        ExpressTree.Push(node); //add variable 
                    }

                }
                else
                {
                    OperatorNode node = new OperatorNode(op);
                    if (ExpressTree.Count != 0)
                    {
                        node.Right = ExpressTree.Pop();
                        node.Left = ExpressTree.Pop();
                    }
                   
                    ExpressTree.Push(node); //add operator
                }
                
            }

        }

        /// <summary>
        /// Adds the to the dicitonary of variables.
        /// </summary>
        /// <param name="variableName">Name of Variable</param>
        /// <param name="variableValue">Value of variable</param>
        public void SetVariable(string variableName, double variableValue)
        {
            variables[variableName] = variableValue;
        }

        /// <summary>
        /// Public function of Evaluate.
        /// </summary>
        /// <returns>The answer to the expression</returns>
        public double Evaluate()
        {
            return Evaluate(ExpressTree.Peek());
        }   
    }

    /// <summary>
    /// Abstract Node class.
    /// </summary>
    public abstract class Node
    {
    }

    /// <summary>
    /// Node class for constant numbers
    /// </summary>
    public class ConstantNode : Node
    {
        double val = 0.0;
        public double ConstNode
        {
            get { return val; }
            set { this.val = value; }
        }

    }

    /// <summary>
    /// Node class for variables 
    /// </summary>
    class VariableNode : Node
    {
        private string name = "";
        public string Variablenode
        {
            get { return name; }
            set { name = value; }
        }
    }

    /// <summary>
    /// Node class for math operators.
    /// </summary>
    class OperatorNode: Node
    {
        private string type;
        public OperatorNode(string op)
        {
            this.type = op;
            this.Left = null;
            this.Right = null;
        }

        /// <summary>
        /// Gets type or sets type
        /// </summary>
        public string opType
        {
            get { return type; }
            set { this.type = value; }
        }

        public Node Left { get; set; } // holds what is left of operator
        public Node Right { get; set; } //holds what is right of operator 
    }
}