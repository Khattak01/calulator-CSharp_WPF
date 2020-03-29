using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Assignment_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> history = new List<string>();//history record of all the calculation
        public MainWindow()
        {
            InitializeComponent();
            this.btn_xsquare.IsEnabled = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //event handler for entering numbers into textBox
            Button btn = (Button)sender;
            if(btn==this.btn_equal)
            {
                if (!containsAny(this.txt_main.Text, new string[] { "+", "-", "*", "/","%" }))
                    return;//check if the textBox contains any operator
                string[] arr = extractData(this.txt_main.Text);
                history.Add(arr[0] + arr[2] + arr[1]+"=");
                double result = 0;
                try
                {
                    result = performCalculation(arr[2], double.Parse(arr[0]), double.Parse(arr[1]));
                }
                catch(Exception err)
                {
                    Console.WriteLine(err.Message);
                    return;
                }
                this.lbl_result.Content = result.ToString();
                history.Add(result.ToString());
                this.lbl_history.Content = "";
                foreach(string line in history)
                    this.lbl_history.Content += line+"\n";
            }
            if (!validateInput(this.txt_main.Text.ToString()))
            {
                MessageBox.Show("Enter numbers, +, -, *, / and = only", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!(btn==this.btn_equal))
                this.txt_main.Text = this.txt_main.Text.ToString()+btn.Content.ToString();
        }
        public static double performCalculation(string oper, double number1, double number2)
        {
            if (oper == "+")
                return (number1 + number2);
            else if (oper == "-")
                return number1 - number2;
            else if (oper == "*")
                return number1 * number2;
            else if (oper == "/")
                return Math.Round((number1 / number2), 3);//round upto two three decimal places
            else if (oper == "%")
                return (number1 % number2);
            else
                return 0;
        }
        private void btnCancel(object sender, RoutedEventArgs e)
        {
            // event handler for handling cancel, clear and backspace
            Button btn = (Button)sender;
            if (btn == this.btn_c)
            {
                this.txt_main.Text = "";
                this.lbl_result.Content = "";
            }
            else if (btn == this.btn_ce)
                this.txt_main.Text = "";
            else//backspace
                try
                {
                    this.txt_main.Text = this.txt_main.Text.ToString().Substring(0, this.txt_main.Text.ToString().Length - 1);
                }
                catch (Exception)
                {
                    return;
                }
        }
        private void txt_main_TextChanged(object sender, TextChangedEventArgs e)
        {
            //will trigger when user change textBox value
            char[] arr = { '*', '/', '+', '-','%' };
            string str = this.txt_main.Text;
            if (str.Length> 0 && containsAny(str.Substring(0,str.Length-1),new string[]{ "+", "-", "*", "/","%" }))
                if (str[str.Length-1]==arr[0] || str[str.Length - 1] == arr[1] || str[str.Length - 1] == arr[2] || str[str.Length - 1] == arr[3] || str[str.Length - 1] == arr[4])
                    this.txt_main.Text = replacePosition(str,arr);
               // MessageBox.Show(str.Substring(0, str.Length - 1));                
            
            this.txt_main.Text = removeMultipleSameOperator(this.txt_main.Text);
            if (!validateInput(this.txt_main.Text.ToString()))
            {
                try
                {
                    this.txt_main.Text = removeExtraChar(this.txt_main.Text);
                    this.txt_main.SelectionStart = this.txt_main.Text.Length; //adjust cursor position
                }
                catch (Exception)
                {
                    return;
                }
                MessageBox.Show("Enter numbers, +, -, *, /, % and = only", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //this.txt_main.Text = replacePosition(this.txt_main.Text, new char[] { '+', '-', '*', '/' });
        }
        private string[] extractData(string str)
        {
            //extract values and operator from the textBox
            string[] arr = new string[3];
            bool i = true;
            foreach (char ch in str)
            {
                if (ch >= '0' && ch <= '9' && i)
                    arr[0] += ch;
                else if (ch >= '0' && ch <= '9' && !i)
                    arr[1] += ch;
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%')
                { 
                    arr[2] = ch.ToString();
                    i=false;
                }
            }
            return arr;
        }
        private string removeMultipleSameOperator(string str)
        {
            //remove multiple operator from textBox e.g if user enter 1+-*8 then we will perform * operation only and the result will be 8
            string returnValue = "";
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] == '+' || str[i] == '/' || str[i] == '-' || str[i] == '*' || str[i] == '%'))
                {
                    try
                    {
                        if (str[i - 1] == '+' || str[i - 1] == '/' || str[i - 1] == '-' || str[i - 1] == '*' || str[i - 1] == '%')
                        {
                            //MessageBox.Show(str[i].ToString() + "  " + str[i - 1].ToString());
                            this.txt_main.Text = this.txt_main.Text.Replace(str[i], str[i - 1]);
                            break;
                        }
                        else
                            returnValue += str[i];
                    }
                    catch(Exception)
                    { }
                } 
                else
                    returnValue += str[i];
                }
            return returnValue;
        }
        private string removeExtraChar(string str)
        {
            //this methode will remove extra characters from string e.g will ristrict the user to enter only digits and operations
            string retrunValue = "";
            foreach (char ch in str)
            {
                if (ch >= '0' && ch <= '9')
                    retrunValue += ch;
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%')
                    retrunValue += ch;
            }
            return retrunValue;
        }

        private bool validateInput(string num)
        {
            //validate textBox value against the following conditions
            foreach (char ch in num)
            {
                if (ch >= '0' && ch <= '9')
                { }
                else if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '%' || ch == '=')
                { }
                else
                    return false;
            }
            return true;
        }
        private void clearHistory(object sender, RoutedEventArgs e)
        {
            history.Clear();
            this.lbl_history.Content = "";
        }
        public static bool containsAny(string str, string[] arr)
        {
            //check if the string contains the array values
            foreach (string subStr in arr)
            {
                if (str.Contains(subStr))
                    return true;
            }
            return false;
        }
       /* public static int getPosition(string str, char[] arr)
        {//get the position of the operator
            int pos = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i].Equals(arr[0]) || str[i].Equals(arr[1]) || str[i].Equals(arr[2]) || str[i].Equals(arr[3]) || str[i].Equals(arr[4]))
                {
                    pos = i;
                    break;
                }
            }
            return pos;
        }*/
        public static string replacePosition(string str, char[] arr)
        {
            string val = "";
            string copy = str;
            if (str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    //nsole.WriteLine("Out : "+str[i]);
                    if (str[i].Equals(arr[0]) || str[i].Equals(arr[1]) || str[i].Equals(arr[2]) || str[i].Equals(arr[3]) || str[i].Equals(arr[4]))
                    {
                        copy = copy.Replace(copy[i], copy[copy.Length - 1]);
                        val = copy.Remove(str.Length - 1);
                    }
                }
                return val;
            }
            else
                return str;
        }

    }
}
