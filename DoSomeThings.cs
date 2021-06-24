using System.Text;
using System;
using System.Text.RegularExpressions;
using K4os.Compression.LZ4.Streams;

namespace AppTest
{
    public class DoSomeThings
    {
        public static bool IsUsernameLongEnough(String input)
        {
            return input.Trim().Length >= 5;
        }

        public static bool IsPasswordStrongEnough(String input)
        {
            Regex regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            bool length = false;
            bool upperCase = false;
            bool symbol = false;
            foreach(char c in input)
            {
                if(char.IsUpper(c))
                {
                    upperCase = true;
                }
                if (regexItem.IsMatch(c.ToString()))
                {
                    symbol = true;
                }
            }
            if(input.Length >= 10)
            {
                length = true;
            }
            return upperCase && symbol && length;
        }

        public static bool IsSwCodeValid(String input)
        {
            return input.Replace("-", "").Trim().Length == 12;
        }

        public static bool IsEmailValid(String input)
        {
            int cntA = 0;
            bool a = false;
            bool b = false;
            foreach(char c in input)
            {
                if(c == '@')
                {
                    cntA += 1;
                    if (cntA == 1)
                    {
                        a = true;
                    }
                    else if (cntA > 1)
                    {
                        a = false;
                    }
                }
                else if (c == '.' && a)
                {
                    b = true;
                }
            }
            return a && b;
        }
    }
}