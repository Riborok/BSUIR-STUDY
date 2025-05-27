using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ProgramComplexityMetrics.code_analyzer;
using ProgramComplexityMetrics.file_utils;
using ProgramComplexityMetrics.output;

namespace ProgramComplexityMetrics {
    public partial class MainForm : Form {
        private const string AllFilesFilter = "All Files (*.*)|*.*";
        private const string HtmlFilesFilter = "HTML Files (*.html)|*.html";
        
        private const string OperatorsTitle = "Operators";
        private const string OperandsTitle = "Operands";
        private const string CountTitle = "Count";

        private readonly FileBuffer<string> _programFileBuffer;
        private readonly FileBuffer<string> _resultFileBuffer;
        private readonly ICodeAnalyzer _codeAnalyzer;
        
        public MainForm() {
            InitializeComponent();
            AdditionalInitialization();
            _programFileBuffer = CreateFileBuffer((buffer) => {
                tbProgram.Text = buffer ?? string.Empty;
            }, tbProgramFileName, AllFilesFilter);
            _resultFileBuffer = CreateFileBuffer((buffer) => {
                tbResult.DocumentText = buffer ?? string.Empty;
            }, tbResultFileName, HtmlFilesFilter);
            _codeAnalyzer = new GroovyCodeAnalyzer();
        }
        
        private void AdditionalInitialization() {
            StartPosition = FormStartPosition.CenterScreen;
        }
        
        private static FileBuffer<string> CreateFileBuffer(Action<string?> updateText, Control srcFileName, string filter) {
            return new FileBuffer<string>(
                updateText,
                new FileInteraction<string>(srcFileName, new TextFileService(), filter)
            );
        }
        
        private void tbProgram_TextChanged(object sender, EventArgs e) {
            _programFileBuffer.Buffer = tbProgram.Text;
        }

        private void butNewProgram_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_programFileBuffer.Create);
        }

        private void butNewResult_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_resultFileBuffer.Create);
        }

        private void butOpenProgram_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_programFileBuffer.Open);
        }

        private void butOpenResult_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_resultFileBuffer.Open);
        }

        private void butSaveProgram_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_programFileBuffer.Save);
        }

        private void butSaveResult_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_resultFileBuffer.Save);
        }

        private void butSaveAsProgram_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_programFileBuffer.SaveAs);
        }

        private void butSaveAsResult_Click(object sender, EventArgs e) {
            HandleActionFileInteraction(_resultFileBuffer.SaveAs);
        }
        
        private void HandleActionFileInteraction(Action action) {
            tbErrors.Text = string.Empty;
            try {
                action();
            }
            catch (IOException exception) {
                tbErrors.Text += exception.Message + Environment.NewLine;
            }
        }

        private void butResetProgram_Click(object sender, EventArgs e) {
            ResetBuffer(_programFileBuffer);
        }

        private void butResetResult_Click(object sender, EventArgs e) {
            ResetBuffer(_resultFileBuffer);
        }
        
        private void ResetBuffer<T>(FileBuffer<T> fileBuffer) where T : class {
            tbErrors.Text = string.Empty;
            fileBuffer.Reset();
        }

        private void butRun_Click(object sender, EventArgs e) {
            if (_programFileBuffer.Buffer == null) {
                return;
            }
    
            AnalyzeAndSaveResults();
        }

        private void AnalyzeAndSaveResults() {
            var (operators, operands) = _codeAnalyzer.Analyze(_programFileBuffer.Buffer!);
            var operatorsTable = GenerateTable(OperatorsTitle, CountTitle, operators);
            var operandsTable = GenerateTable(OperandsTitle, CountTitle, operands);
            var complexityMetricsConclusion = GenerateConclusion(operators, operands);
            SaveResultToFile(operatorsTable, operandsTable, complexityMetricsConclusion);
        }

        private static string GenerateTable(string keyTitle, string valueTitle, IReadOnlyDictionary<string, int> data) {
            var keys = data.Keys.ToList();
            var values = data.Values.Select(value => value.ToString()).ToList();
            return TableHtmlCreator.Create(keyTitle, valueTitle, keys, values);
        }

        private static string GenerateConclusion(Dictionary<string, int> operators, Dictionary<string, int> operands) {
            return ComplexityMetricsConclusionHtmlCreator.Create(operators, operands);
        }

        private void SaveResultToFile(string operatorsTable, string operandsTable, string complexityMetricsConclusion) {
            _resultFileBuffer.Buffer = operatorsTable + operandsTable + complexityMetricsConclusion;
        }
    }
}
