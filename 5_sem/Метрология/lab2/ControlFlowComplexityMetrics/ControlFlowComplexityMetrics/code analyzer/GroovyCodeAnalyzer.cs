using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ControlFlowComplexityMetrics.extension;

namespace ControlFlowComplexityMetrics.code_analyzer {
    internal class GroovyCodeAnalyzer : ICodeAnalyzer {
        private static readonly  HashSet<string> Operators = new HashSet<string> {
            "case", "do", "else", 
            "for", "if", "instanceof",
            "new", "while"
        };
        
        private static readonly HashSet<string> IgnoreList = new HashSet<string> {
            "boolean", "byte", "char", "double", "float", "int", "long", "short", "void", "String"
        };
        
        private const string OperandPattern = @"\b[a-zA-Z_][\w_]*\b(?!\s*\()|\b\d+(\.\d+)?\b";
        private const string StringLiteralPattern = @"""[^""]*""|'[^']*'";
        private const string OperatorPattern = @"[+\-*/%&|^=!<>\,\.~]+|\?\.|\?\:|;";
        private const string MethodPattern = @"[\w_]+\s*\("; //(?![\s\S]*\{)
        
        private void AddToDictionary(Dictionary<string, int> dictionary, string key) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key]++;
            } else {
                dictionary[key] = 1;
            }
        }

        private void AddMethodToDictionary(Dictionary<string, int> operators, string method) {
            const string methodParenthesis = @"\(";
            Regex methodParenthesisRegex = new Regex(methodParenthesis);
            string methodResult = methodParenthesisRegex.Replace(method, "()");
            
            AddToDictionary(operators, methodResult);
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

        private int CalcAllOperators(Dictionary<string, int> operators) {
            int amount = 0;
            foreach (var oper in operators) {
                amount += oper.Value;
            }

            return amount;
        }
        
        private int CalcConditionalOperators(Dictionary<string, int> operators) {
            int amount = 0;
            
            if (operators.ContainsKey("if ()")) {
                amount += operators["if ()"];
            }
            
            if (operators.ContainsKey("if () else")) {
                amount += operators["if () else"];
            }
            
            if (operators.ContainsKey("for ()")) {
                amount += operators["for ()"];
            }
            
            if (operators.ContainsKey("while ()")) {
                amount += operators["while ()"];
            }
            
            if (operators.ContainsKey("do while")) {
                amount += operators["do while"];
            }
            
            if (operators.ContainsKey("case")) {
                amount += operators["case"];
            }

            return amount;
        }

        private int CalcMaxNesting(string code) {
            int maxNesting = 0;
            int nestedSwitch = 0;
            Stack<char> stack = new Stack<char>();

            for (int i = 0; i < code.Length; i++) {
                if (code[i].Equals('{')) {
                    if (stack.Count != 0) {
                        if (stack.Peek().Equals('<')) {
                            continue;
                        }
                    }

                    stack.Push('{');
                    if (stack.Count - nestedSwitch > maxNesting) {
                        maxNesting = stack.Count - nestedSwitch;
                    }
                    
                    continue;
                }
                
                if (code[i].Equals('s')) {
                    if (code.Substring(i, 6).Equals("switch")) {
                        stack.Push('<');
                        nestedSwitch++;
                    }

                    continue;
                }
                
                if (code[i].Equals('c')) {
                    if (code.Substring(i, 4).Equals("case")) {
                        stack.Push('c');
                        
                        if (stack.Count - nestedSwitch > maxNesting) {
                            maxNesting = stack.Count - nestedSwitch;
                        }
                    }
                    
                    continue;
                }
                
                if (code[i].Equals('}')) {
                    if (stack.Peek() == 'c') {
                        while (stack.Peek() == 'c') {
                            stack.Pop();
                        }

                        nestedSwitch--;
                    }
                    
                    stack.Pop();
                }
            }

            return maxNesting;
        }
        
        public (int CL, double cl, int CLI, int opersAmount) Analyze(string code) {
            Dictionary<string, int> operators = new Dictionary<string, int>();
            
            // Deleting header and imports
            string cleanedCode = CleanupCode(code);
            
            // Removing string literals and
            string readyCode = RemoveStringLiterals(cleanedCode);
            
            // Searching for operands and operators
            Regex operandRegex = new Regex(OperandPattern);
            Regex operatorRegex = new Regex(OperatorPattern);
            Regex methodRegex = new Regex(MethodPattern);
            MatchCollection operandMatches = operandRegex.Matches(readyCode);
            MatchCollection operatorMatches = operatorRegex.Matches(readyCode);
            MatchCollection methodMatches = methodRegex.Matches(readyCode);
            
            // Adding word operators
            foreach (Match match in operandMatches) {
                if (Operators.Contains(match.Value)) {
                    AddToDictionary(operators, match.Value);
                }
            }
            
            // Adding operators
            foreach (Match match in operatorMatches) {
                AddToDictionary(operators, match.Value);
            }
            
            // Adding methods
            foreach (Match match in methodMatches) {
                AddMethodToDictionary(operators, match.Value);
            }
            
            if (operators.ContainsKey("if ()") && operators.ContainsKey("else")) {
                operators["if ()"] -= operators["else"];
            }
            if (operators.ContainsKey("else")) {
                operators.ChangeKey("else", "if () else");
            }

            if (operators.ContainsKey("while") && operators.ContainsKey("do")) {
                operators["while"] -= operators["do"];
            }
            if (operators.ContainsKey("do")) {
                operators.ChangeKey("do", "do while");
            }

            int CL = CalcConditionalOperators(operators);
            int opersAmount = CalcAllOperators(operators);
            double cl = (double)CL / opersAmount;
            int CLI = CalcMaxNesting(readyCode) - 1;
            
            return (CL, cl, CLI, opersAmount);
        }
    }
}