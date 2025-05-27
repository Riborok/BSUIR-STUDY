using System;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using DigitalSigner.Extensions;
using DigitalSigner.FileUtils;
using DigitalSigner.Managers;
using DigitalSigner.Params;
using RSADigitalSignature;

namespace DigitalSigner {
    public partial class MainForm : Form
    {
        private readonly Validator _validator;
        private readonly BufferManager<string[]> _textBufferManager;

        public MainForm() {
            InitializeComponent();
            AdditionalInitialization();
            StartPosition = FormStartPosition.CenterScreen;

            _validator = new Validator(tbErrors);
            _textBufferManager = CreateInitBufferManager();
        }

        private void AdditionalInitialization() {
            cbHashingAlgorithm.SelectedIndex = 0;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private BufferManager<string[]> CreateInitBufferManager()
        {
            const string filter = @"Text files (*.txt)|*.txt";
            
            return new BufferManager<string[]>(
                (buffer) => {
                    tbText.Text = buffer != null ? string.Join(Environment.NewLine, buffer) : string.Empty;
                },
                new FileManager<string[]>(tbTextFileName, new TextArrayFileService(), filter)
            );
        }
        
        private void butNewInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_textBufferManager.Create);
        }

        private void butOpenInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_textBufferManager.Open);
        }

        private void butSaveInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_textBufferManager.Save);
        }
        
        private void butSaveAsInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_textBufferManager.SaveAs);
        }

        private void HandleFileManagerAction(Action action) {
            tbErrors.Text = string.Empty;
            try {
                action();
            }
            catch (IOException exception) {
                tbErrors.Text += exception.Message + Environment.NewLine;
            }
        }
        
        private void butResetInitText_Click(object sender, EventArgs e) {
            ResetBuffer(_textBufferManager);
        }

        private void ResetBuffer<T>(BufferManager<T> bufferManager) where T : class {
            tbErrors.Text = string.Empty;
            bufferManager.Reset();
        }

        private void butSign_Click(object sender, EventArgs e) {
            tbErrors.Text = string.Empty;
            if (_textBufferManager.Buffer == null || !TryGetSignParams(out var signParams))
                return;

            SetSignParamsText(signParams);


            if (BigInteger.TryParse(_textBufferManager.Buffer.GetLast(string.Empty), out _)) {
                var signature = RSASignature.GetSignature(CalcHashWithoutLastString(signParams.r), signParams.d, signParams.r).ToString();
                _textBufferManager.Buffer.SetLast(signature);
            }
            else {
                var signature = RSASignature.GetSignature(CalcHash(signParams.r), signParams.d, signParams.r).ToString();
                _textBufferManager.Buffer = _textBufferManager.Buffer.Append(signature);
            }
        }
        
        private bool TryGetSignParams(out SignParams signParams) {
            var isValid = true;
            
            _validator.TryGet_p(tbP.Text, out var p, ref isValid);
            _validator.TryGet_q(tbQ.Text, out var q, ref isValid);
            _validator.TryGet_e(tbE.Text, out var e, ref isValid, (p - 1) * (q - 1));
            signParams = new SignParams(p, q, e);
            
            return isValid;
        }
        
        private void SetSignParamsText(SignParams signParams) {
            tbR.Text = signParams.r.ToString();
            tbPhi.Text = signParams.phi.ToString();
            tbD.Text = signParams.d.ToString();
        }

        private BigInteger CalcHash(BigInteger r) {
            var strings = _textBufferManager.Buffer!;
            var buffer = strings.ToByteArray();
            return CalcHash(buffer, r);
        }

        private BigInteger CalcHashWithoutLastString(BigInteger r) {
            var strings = _textBufferManager.Buffer!;
            var buffer = strings.ToByteArray(strings.Length - 1);
            return CalcHash(buffer, r);
        }

        private BigInteger CalcHash(byte[] buffer, BigInteger r) {
            return cbHashingAlgorithm.SelectedIndex switch
            {
                0 => Hasher.HashBasic(buffer, r),
                1 => Hasher.HashSHA256(buffer, r),
                2 => Hasher.HashSHA512(buffer, r),
                3 => Hasher.HashMD5(buffer, r),
                _ => throw new ArgumentOutOfRangeException(nameof(cbHashingAlgorithm),
                    @"Invalid hashing algorithm selected.")
            };
        }

        private void butCheckSign_Click(object sender, EventArgs e) {
            tbErrors.Text = string.Empty;
            if (_textBufferManager.Buffer == null || !TryGetCheckSignParams(out var checkSignParams))
                return;

            if (BigInteger.TryParse(_textBufferManager.Buffer.GetLast(string.Empty), out var signature)) {
                var hash = CalcHashWithoutLastString(checkSignParams.r);
                if (RSASignature.CheckSignature(signature, checkSignParams.e, checkSignParams.r, hash)) 
                    MessageBox.Show(@"Digital signature is valid.", @"Signature Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(@"Digital signature is not valid.", @"Signature Verification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                tbErrors.Text = @"Invalid format for digital signature.";
            }
        }
        
        private bool TryGetCheckSignParams(out CheckSignParams checkSignParams) {
            var isValid = true;
            _validator.TryGet_e(tbE.Text, out var e, ref isValid);
            _validator.TryGet_r(tbR.Text, out var r, ref isValid);
            checkSignParams = new CheckSignParams(e, r);
            return isValid;
        }
    }
}
