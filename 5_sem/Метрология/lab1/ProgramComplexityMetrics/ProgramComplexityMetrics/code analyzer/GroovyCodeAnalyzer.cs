using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProgramComplexityMetrics.extension;

namespace ProgramComplexityMetrics.code_analyzer {
    internal class GroovyCodeAnalyzer : ICodeAnalyzer {
        private static readonly  HashSet<string> Operators = new HashSet<string> {
            "abstract", "assert", "break", "case", "catch", "class", "const", "continue", "def", "default", "do", "else", "enum", 
            "extends", "final", "finally", "for", "goto", "if", "implements", "import", "in", "instanceof", "interface", 
            "native", "new", "package", "private", "protected", "public", "return", "static", "strictfp", "super", 
            "switch", "synchronized", "throw", "throws", "transient", "try", "volatile", "while"
        };
        
        private static readonly HashSet<string> IgnoreList = new HashSet<string> {
            "boolean", "byte", "char", "double", "float", "int", "long", "short", "void", "String"
        };

        private const string OperandPattern = @"\b[a-zA-Z_][\w_]*\b(?!\s*\()|\b\d+(\.\d+)?\b";
        private const string OperatorPattern = @"[+\-*/%&|^=!<>\,\.~]+|\?\.|\?\:|;";
        private const string TernaryPattern = @"\?[^\:;]+\:";
        private const string MethodPattern = @"[\w_]+\s*\("; //(?![\s\S]*\{)
        private const string RoundParenthesesPattern = @"(?<!\b[a-zA-Z_][a-zA-Z0-9_]*\s*)\(";
        private const string ParenthesesPattern = @"\[|\{";
        private const string StringLiteralPattern = @"""[^""]*""|'[^']*'";
        
        private void AddToDictionary(Dictionary<string, int> dictionary, string key) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key]++;
            } else {
                dictionary[key] = 1;
            }
        }
        
        private void AddStringLiteralsToDictionary(Dictionary<string, int> operands, string code) {
            // Searching for string literals
            Regex stringLiteralRegex = new Regex(StringLiteralPattern);
            MatchCollection stringLiterals = stringLiteralRegex.Matches(code);

            // Creating a list of string literals
            foreach (Match match in stringLiterals) {
                AddToDictionary(operands, match.Value);
            }
        }
        
        private void AddTernaryOperatorToDictionary(Dictionary<string, int> operators, string ternaryOperator) {
            Regex ternaryOperatorRegex = new Regex(TernaryPattern);
            string ternaryOperatorResult = ternaryOperatorRegex.Replace(ternaryOperator, "? :");
            
            AddToDictionary(operators, ternaryOperatorResult);
        }

        private void AddMethodToDictionary(Dictionary<string, int> operators, string method) {
            const string methodParenthesis = @"\(";
            Regex methodParenthesisRegex = new Regex(methodParenthesis);
            string methodResult = methodParenthesisRegex.Replace(method, "()");
            
            AddToDictionary(operators, methodResult);
        }
        
        private void AddRoundParenthesesToDictionary(Dictionary<string, int> operators, string parenthesis) {
            AddToDictionary(operators, "()");
        }
        
        private void AddParenthesesToDictionary(Dictionary<string, int> operators, string parenthesis) {
            switch (parenthesis) {
                case "[":
                    AddToDictionary(operators, "[]");
                    break;
                case "{":
                    AddToDictionary(operators, "{}");
                    break;
            }
        }
        
        private string RemoveStringLiterals(string code) {
            // Searching for string literals
            Regex stringLiteralRegex = new Regex(StringLiteralPattern);

            // Removing string literals from the code
            return stringLiteralRegex.Replace(code, "");
        }
        
        private string RemoveHeaderAndImports(string code) {
            string pattern = @"^#!.*|^import\s+.*$";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            
            return regex.Replace(code, "").Trim();
        }
        
        private string RemoveComments(string code) {
            string pattern = @"(//.*?$)|(/\*.*?\*/)|(/\*[\s\S]*?\*/)";
            string cleanedCode = Regex.Replace(code, pattern, "", RegexOptions.Multiline);
            
            return cleanedCode;
        } 
        
        private string CleanupCode(string code) {
            string codeWithoutComments = RemoveComments(code);
            string cleanedCode = RemoveHeaderAndImports(codeWithoutComments);
            
            return cleanedCode;
        }
        
        public (Dictionary<string, int> operators, Dictionary<string, int> operands) Analyze(string code) {
            Dictionary<string, int> operands = new Dictionary<string, int>();
            Dictionary<string, int> operators = new Dictionary<string, int>();
            
            // Deleting header and imports
            string cleanedCode = CleanupCode(code);
            
            // Adding string literals to the operands dictionary
            AddStringLiteralsToDictionary(operands, cleanedCode);
            
            // Removing string literals and
            string readyCode = RemoveStringLiterals(cleanedCode);
            
            // Searching for operands and operators
            Regex operandRegex = new Regex(OperandPattern);
            Regex operatorRegex = new Regex(OperatorPattern);
            Regex ternaryRegex = new Regex(TernaryPattern);
            Regex methodRegex = new Regex(MethodPattern);
            Regex roundParenthesesRegex = new Regex(RoundParenthesesPattern);
            Regex parenthesesRegex = new Regex(ParenthesesPattern);
            MatchCollection operandMatches = operandRegex.Matches(readyCode);
            MatchCollection operatorMatches = operatorRegex.Matches(readyCode);
            MatchCollection ternaryMatches = ternaryRegex.Matches(readyCode);
            MatchCollection methodMatches = methodRegex.Matches(readyCode);
            MatchCollection roundParenthesesMatches = roundParenthesesRegex.Matches(readyCode);
            MatchCollection parenthesesMatches = parenthesesRegex.Matches(readyCode);
            
            // Adding operands
            foreach (Match match in operandMatches) {
                string operand = match.Value;
                
                if (!IgnoreList.Contains(operand)) {
                    AddToDictionary(!Operators.Contains(operand) ? operands : operators, operand);
                }
            }
            
            // Adding operators
            foreach (Match match in operatorMatches) {
                AddToDictionary(operators, match.Value);
            }
            
            // Adding ternary operators
            foreach (Match match in ternaryMatches) {
                AddTernaryOperatorToDictionary(operators, match.Value);
            }
            
            // Adding methods
            foreach (Match match in methodMatches) {
                AddMethodToDictionary(operators, match.Value);
            }
            
            // Adding round parentheses
            foreach (Match match in roundParenthesesMatches) {
                AddRoundParenthesesToDictionary(operators, match.Value);
            }
            
            // Adding parentheses
            foreach (Match match in parenthesesMatches) {
                AddParenthesesToDictionary(operators, match.Value);
            }
            
            if (operators.ContainsKey("if ()") && operators.ContainsKey("else")) {
                operators["if ()"] -= operators["else"];
            }
            if (operators.ContainsKey("else")) {
                operators.ChangeKey("else", "if () else");
            }
            if (operators.ContainsKey("try") && operators.ContainsKey("finally")) {
                operators["try"] -= operators["finally"];
            }
            if (operators.ContainsKey("finally")) {
                operators.ChangeKey("finally", "try catch finally");
            }
            if (operators.ContainsKey("try")) {
                operators.ChangeKey("try", "try catch");
            }
            if (operators.ContainsKey("catch ()")) {
                operators.Remove("catch ()");
            }

            if (operators.ContainsKey("while") && operators.ContainsKey("do")) {
                operators["while"] -= operators["do"];
            }
            if (operators.ContainsKey("do")) {
                operators.ChangeKey("do", "do while");
            }

            if (operators.ContainsKey("case")) {
                operators.Remove("case");
            }
            
            return (operators, operands);
        }
    }
}