using System.Collections.Generic;

namespace ControlFlowComplexityMetrics.code_analyzer {
    internal interface ICodeAnalyzer {
        public (int CL, double cl, int CLI, int opersAmount) Analyze(string code);
    }
}