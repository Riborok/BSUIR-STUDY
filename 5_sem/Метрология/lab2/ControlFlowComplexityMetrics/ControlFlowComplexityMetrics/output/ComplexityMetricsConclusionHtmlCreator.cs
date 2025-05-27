using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace ControlFlowComplexityMetrics.output {
    internal static class ComplexityMetricsConclusionHtmlCreator {
        public static string Create(int CL, double cl, int CLI, int opersAmount) {
            var metrics = (CL, cl, CLI, opersAmount);

            var builder = new StringBuilder();

            AppendStyles(builder);
            AppendTitle(builder);
            AppendMetrics(builder, metrics);

            return builder.ToString();
        }

        private static (int UniqueOperatorsCount, int TotalOperatorsCount, int UniqueOperandsCount, int TotalOperandsCount, int ProgramDictionary, int ProgramLength, double ProgramVolume) CalculateMetrics(Dictionary<string, int> operators, Dictionary<string, int> operands) {
            var uniqueOperatorsCount = operators.Count;
            var totalOperatorsCount = operators.Values.Sum();

            var uniqueOperandsCount = operands.Count;
            var totalOperandsCount = operands.Values.Sum();

            var programDictionary = uniqueOperatorsCount + uniqueOperandsCount;
            var programLength = totalOperatorsCount + totalOperandsCount;
            var programVolume = programLength * Math.Log(programDictionary, 2);

            return (uniqueOperatorsCount, totalOperatorsCount, uniqueOperandsCount, totalOperandsCount, programDictionary, programLength, programVolume);
        }

        private static void AppendStyles(StringBuilder builder) {
            builder.AppendLine("<style>");
            builder.AppendLine("h2 { color: #333; }");
            builder.AppendLine("p { font-family: Arial, sans-serif; font-size: 16px; line-height: 1.6; }");
            builder.AppendLine("</style>");
        }

        private static void AppendTitle(StringBuilder builder) {
            var encoder = HtmlEncoder.Default;
            builder.AppendLine("<h2>" + encoder.Encode("Jilba Metric") + "</h2>");
        }

        private static void AppendMetrics(StringBuilder builder, (int CL, double cl, int CLI, int opersAmount) metrics) {
            var encoder = HtmlEncoder.Default;
            builder.AppendLine($"<p><strong>{encoder.Encode("Amount of all operators:")}</strong> {metrics.opersAmount}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Amount of conditional operators:")}</strong> {metrics.CL}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Ratio of number of conditional operators to number of all operators:")}</strong> {metrics.cl}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Maximum nesting:")}</strong> {metrics.CLI}</p>");
        }
    }
}