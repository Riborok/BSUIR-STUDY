using System;
using System.Numerics;
using System.Windows.Forms;
using RabinCryptosystem;

namespace Encryptor
{
    public class Validator
    {
        private const string pFieldName = "private key p";
        private const string qFieldName = "private key q";
        private const string bFieldName = "private key b";
        private const string nFieldName = "private key n";
        
        private readonly Control _tbErrors;

        public Validator(Control tbErrors)
        {
            _tbErrors = tbErrors;
        }
        
        public void TryGet_b(string text, out BigInteger b, ref bool isValid, BigInteger n)
        {
            isValid &= ValidateInput(text, bFieldName, out b);
            isValid &= ValidateNatural(bFieldName, b);
            if (isValid)
                isValid &= ValidateMaxValue(bFieldName, b, nFieldName, n);
        }
        
        private bool ValidateInput(string input, string fieldName, out BigInteger value) {
            if (!BigInteger.TryParse(input, out value)) {
                _tbErrors.Text += $@"Invalid {fieldName} format. Enter a valid BigInteger value.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        private bool ValidateNatural(string fieldName, BigInteger value)
        {
            if (value <= 0)
            {
                _tbErrors.Text += $@"{fieldName} must be a natural number (greater than zero).{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        private bool ValidateMaxValue(string fieldName, BigInteger value, string maxValueName, BigInteger maxValue)
        {
            if (value >= maxValue)
            {
                _tbErrors.Text += $@"{fieldName} must be less than {maxValueName}.{Environment.NewLine}";
                return false;
            }
            return true;
        }

        public void TryGet_n(string text, out BigInteger n, ref bool isValid)
        {
            isValid &= ValidateInput(text, nFieldName, out n);
            Validate_n(n, ref isValid);
        }
        
        public void Validate_n(BigInteger n, ref bool isValid)
        {
            const int Min_p = byte.MaxValue + 1;
            isValid &= ValidateMinValue(nFieldName, n, Min_p.ToString(), Min_p);
        }
        
        private bool ValidateMinValue(string fieldName, BigInteger value, string minValueName, BigInteger minValue)
        {
            if (value <= minValue)
            {
                _tbErrors.Text += $@"{fieldName} must be greater than {minValueName}.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        public void TryGet_p(string text, out BigInteger p, ref bool isValid)
        {
            TryGetFactor(text, pFieldName, out p, ref isValid);
        }
        
        public void TryGet_q(string text, out BigInteger q, ref bool isValid)
        {
            TryGetFactor(text, qFieldName, out q, ref isValid);
        }
        
        private void TryGetFactor(string text, string fieldName, out BigInteger field, ref bool isValid)
        {
            isValid &= ValidateInput(text, fieldName, out field);
            isValid &= ValidatePrime(fieldName, field);
            isValid &= ValidateMod4(fieldName, field);
        }
        
        private bool ValidatePrime(string fieldName, BigInteger value) {
            if (!MillerRabin.MillerRabinTest(value))
            {
                _tbErrors.Text += $@"{fieldName} must be a prime number.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        private bool ValidateMod4(string fieldName, BigInteger value) {
            if (value % 4 != 3) {
                _tbErrors.Text += $@"{fieldName} must satisfy the condition: {fieldName} = 3 (mod 4).{Environment.NewLine}";
                return false;
            }
            return true;
        }
    }
}