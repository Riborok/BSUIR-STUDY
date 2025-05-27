using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace ProgramComplexityMetrics.output {
    internal static class TableHtmlCreator {
        public static string Create(string column1Name, string column2Name, List<string> column1Data, List<string> column2Data) {
            ValidateInput(column1Data, column2Data);

            return GenerateTableContent(column1Name, column2Name, column1Data, column2Data);
        }

        private static void ValidateInput(List<string> column1Data, List<string> column2Data) {
            if (column1Data.Count != column2Data.Count)
                throw new ArgumentException("The lengths of the columns must be the same.");
        }

        private static string GenerateTableContent(string column1Name, string column2Name, List<string> column1Data, List<string> column2Data) {
            var builder = new StringBuilder();

            AppendStyles(builder);
            AppendTableStart(builder);
            AppendTableHeader(builder, column1Name, column2Name);
            AppendTableBody(builder, column1Data, column2Data);
            AppendTableEnd(builder);

            return builder.ToString();
        }

        private static void AppendStyles(StringBuilder builder) {
            builder.AppendLine("<style>");
            builder.AppendLine("table { border-collapse: collapse; margin: 20px 0; }");
            builder.AppendLine("th, td { border: 1px solid black; padding: 8px; }");
            builder.AppendLine("</style>");
        }

        private static void AppendTableStart(StringBuilder builder) {
            builder.AppendLine("<table>");
        }

        private static void AppendTableHeader(StringBuilder builder, string column1Name, string column2Name) {
            var encoder = HtmlEncoder.Default;
            builder.AppendLine("<thead>");
            builder.AppendLine($"<tr><th>{encoder.Encode(column1Name)}</th><th>{encoder.Encode(column2Name)}</th></tr>");
            builder.AppendLine("</thead>");
        }

        private static void AppendTableBody(StringBuilder builder, List<string> column1Data, List<string> column2Data) {
            var encoder = HtmlEncoder.Default;
            builder.AppendLine("<tbody>");
            for (int i = 0; i < column1Data.Count; i++) {
                builder.AppendLine($"<tr><td>{encoder.Encode(column1Data[i])}</td><td>{encoder.Encode(column2Data[i])}</td></tr>");
            }
            builder.AppendLine("</tbody>");
        }

        private static void AppendTableEnd(StringBuilder builder) {
            builder.AppendLine("</table>");
        }
    }
}