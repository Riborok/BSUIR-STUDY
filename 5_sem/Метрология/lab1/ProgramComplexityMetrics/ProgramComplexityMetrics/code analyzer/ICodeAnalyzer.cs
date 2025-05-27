using System.Collections.Generic;

namespace ProgramComplexityMetrics.code_analyzer {
    internal interface ICodeAnalyzer {
        public (Dictionary<string, int> operators, Dictionary<string, int> operands) Analyze(string code);
    }
}