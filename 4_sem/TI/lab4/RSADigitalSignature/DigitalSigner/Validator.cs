// ReSharper disable InconsistentNaming
using System;
using System.Numerics;
using System.Windows.Forms;
using RSADigitalSignature;

namespace DigitalSigner
{
    public class Validator
    {
        private const string pFieldName = "private key p";
        private const string qFieldName = "private key q";
        private const string eFieldName = "public key e";
        private const string rFieldName = "public key r";
        
        private readonly Control _tbErrors;

        public Validator(Control tbErrors) {
            _tbErrors = tbErrors;
        }

        public void TryGet_r(string text, out BigInteger r, ref bool isValid) {
            isValid &= ValidateInput(text, rFieldName, out r);
        }
        
        private bool ValidateInput(string input, string fieldName, out BigInteger value) {
            if (!BigInteger.TryParse(input, out value)) {
                _tbErrors.Text += $@"Invalid {fieldName} format. Enter a valid BigInteger value.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        public void TryGet_e(string text, out BigInteger e, ref bool isValid, BigInteger phi) {
            TryGet_e(text, out e, ref isValid);
            isValid &= ValidateMaxValue(eFieldName, e, nameof(phi), phi);
            isValid &= ValidateCoprime(eFieldName, e, nameof(phi), phi);
        }
        
        private bool ValidateMaxValue(string fieldName, BigInteger value, string maxValueName, BigInteger maxValue) {
            if (value >= maxValue) {
                _tbErrors.Text += $@"{fieldName} must be less than {maxValueName}.{Environment.NewLine}";
                return false;
            }
            return true;
        }

        private bool ValidateCoprime(string fieldNameA, BigInteger a, string fieldNameB, BigInteger b) {
            if (!MathUtils.AreCoprime(a, b)) {
                _tbErrors.Text += $@"{fieldNameA} and {fieldNameB} must be coprime.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        public void TryGet_e(string text, out BigInteger e, ref bool isValid) {
            const int one = 1;
            isValid &= ValidateInput(text, eFieldName, out e);
            isValid &= ValidateMinValue(eFieldName, e, nameof(one), one);
        }
        
        private bool ValidateMinValue(string fieldName, BigInteger value, string minValueName, BigInteger minValue) {
            if (value <= minValue) {
                _tbErrors.Text += $@"{fieldName} must be greater than {minValueName}.{Environment.NewLine}";
                return false;
            }
            return true;
        }
        
        public void TryGet_p(string text, out BigInteger p, ref bool isValid) {
            TryGetFactor(text, pFieldName, out p, ref isValid);
        }
        
        public void TryGet_q(string text, out BigInteger q, ref bool isValid) {
            TryGetFactor(text, qFieldName, out q, ref isValid);
        }
        
        private void TryGetFactor(string text, string fieldName, out BigInteger field, ref bool isValid) {
            isValid &= ValidateInput(text, fieldName, out field);
            isValid &= ValidatePrime(fieldName, field);
        }
        
        private bool ValidatePrime(string fieldName, BigInteger value) {
            if (!MillerRabin.MillerRabinTest(value)) {
                _tbErrors.Text += $@"{fieldName} must be a prime number.{Environment.NewLine}";
                return false;
            }
            return true;
        }
    }
}