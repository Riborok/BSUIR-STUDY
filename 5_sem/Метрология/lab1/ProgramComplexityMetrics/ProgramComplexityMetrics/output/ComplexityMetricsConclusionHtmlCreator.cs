using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace ProgramComplexityMetrics.output {
    internal static class ComplexityMetricsConclusionHtmlCreator {
        public static string Create(Dictionary<string, int> operators, Dictionary<string, int> operands) {
            var metrics = CalculateMetrics(operators, operands);

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
            builder.AppendLine("<h2>" + encoder.Encode("Halstead Metric") + "</h2>");
        }

        private static void AppendMetrics(StringBuilder builder, (int UniqueOperatorsCount, int TotalOperatorsCount, int UniqueOperandsCount, int TotalOperandsCount, int ProgramDictionary, int ProgramLength, double ProgramVolume) metrics) {
            var encoder = HtmlEncoder.Default;
            builder.AppendLine($"<p><strong>{encoder.Encode("Amount of unique operators:")}</strong> {metrics.UniqueOperatorsCount}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Total number of operators:")}</strong> {metrics.TotalOperatorsCount}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Amount of unique operands:")}</strong> {metrics.UniqueOperandsCount}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Total number of operands:")}</strong> {metrics.TotalOperandsCount}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Program dictionary (sum of unique operators and operands):")}</strong> {metrics.ProgramDictionary}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Program length (sum of total operators and operands):")}</strong> {metrics.ProgramLength}</p>");
            builder.AppendLine($"<p><strong>{encoder.Encode("Program volume (number of bits needed to record the program):")}</strong> {metrics.ProgramVolume:F2} bits</p>");
        }
    }
}