using System;

namespace Enums
{
    public class Operator
    {
        public const string Grace = "GRACE";
        public const string Ionia = "IONIA";
        private string _operator;
        public Operator(string value)
        {
            if (value?.ToUpper() == Grace)
            {
                _operator = Grace;
                return;
            }
            if (value?.ToUpper() == Ionia)
            {
                _operator = Ionia;
                return;
            }
            throw new Exception("Invalid Operator");
        }
        public override string ToString()
        {
            return _operator;
        }
    }
}
