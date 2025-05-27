using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using Encryptor.FileUtils;
using Encryptor.Managers;
using RabinCryptosystem;

namespace Encryptor {
    public partial class MainForm : Form
    {
        private readonly Validator _validator;
        private readonly BufferManager<byte[]> _initTextBufferManager;
        private readonly BufferManager<string> _ciphertextBufferManager;

        public MainForm() {
            InitializeComponent();
            AdditionalInitialization();
            StartPosition = FormStartPosition.CenterScreen;

            _validator = new Validator(tbErrors);
            _initTextBufferManager = CreateInitBufferManager();
            _ciphertextBufferManager = CreateCipherBufferManager();
        }

        private void AdditionalInitialization() {
            StartPosition = FormStartPosition.CenterScreen;
        }

        private BufferManager<byte[]> CreateInitBufferManager()
        {
            const string filter = @"All files (*.*)|*.*";
            
            return new BufferManager<byte[]>(
                (buffer) => {
                    tbInitText.Text = buffer == null 
                        ? string.Empty 
                        : string.Join(" ", buffer.Select(b => b.ToString()));
                },
                new FileManager<byte[]>(tbInitTextFileName, new BinaryFileService(), filter)
            );
        }
        
        private BufferManager<string> CreateCipherBufferManager()
        {
            const string filter = @"Text files (*.txt)|*.txt";
            
            return new BufferManager<string>(
                (buffer) => {
                    tbCiphertext.Text = buffer ?? string.Empty;
                },
                new FileManager<string>(tbCiphertextFileName, new TextFileService(), filter)
            );
        }
        
        private void butNewInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_initTextBufferManager.Create);
        }

        private void butNewCiphertext_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_ciphertextBufferManager.Create);
        }

        private void butOpenInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_initTextBufferManager.Open);
        }
        
        private void butOpenCiphertext_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_ciphertextBufferManager.Open);
        }

        private void butSaveInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_initTextBufferManager.Save);
        }

        private void butSaveCiphertext_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_ciphertextBufferManager.Save);
        }
        
        private void butSaveAsInitText_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_initTextBufferManager.SaveAs);
        }
        
        private void butSaveAsCiphertext_Click(object sender, EventArgs e) {
            HandleFileManagerAction(_ciphertextBufferManager.SaveAs);
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
            ResetBuffer(_initTextBufferManager);
        }

        private void butResetCiphertext_Click(object sender, EventArgs e) {
            ResetBuffer(_ciphertextBufferManager);
        }

        private void ResetBuffer<T>(BufferManager<T> bufferManager) where T : class {
            tbErrors.Text = string.Empty;
            bufferManager.Reset();
        }

        private void butEncrypt_Click(object sender, EventArgs e) {
            tbErrors.Text = string.Empty;
            if (_initTextBufferManager.Buffer == null || !TryGetEncryptParams(out var n, out var b))
                return;
            tbN.Text = n.ToString();

            _ciphertextBufferManager.Buffer = RabinEncryptor.Encrypt(n, b, _initTextBufferManager.Buffer);
        }
        
        private bool TryGetEncryptParams(out BigInteger n, out BigInteger b) {
            var isValid = true;
            
            if (tbP.Text != string.Empty && tbQ.Text != string.Empty) {
                _validator.TryGet_p(tbP.Text, out var p, ref isValid);
                _validator.TryGet_q(tbQ.Text, out var q, ref isValid);
                n = p * q;
                _validator.Validate_n(n, ref isValid);
            }
            else {
                _validator.TryGet_n(tbN.Text, out n, ref isValid);
            }

            _validator.TryGet_b(tbB.Text, out b, ref isValid, n);
            
            return isValid;
        }

        private void butDecrypt_Click(object sender, EventArgs e) {
            tbErrors.Text = string.Empty;
            if (_ciphertextBufferManager.Buffer == null || !TryGetDecryptParams(out var p, out var q, out var n, out var b))
                return;
            tbN.Text = n.ToString();

            try
            {
                _initTextBufferManager.Buffer = RabinEncryptor.Decrypt(p, q, n, b, _ciphertextBufferManager.Buffer);
            }
            catch (ArgumentException exception)
            {
                tbErrors.Text += exception.Message + Environment.NewLine;
            }
        }
        
        private bool TryGetDecryptParams(out BigInteger p, out BigInteger q, out BigInteger n, out BigInteger b) {
            var isValid = true;
            
            _validator.TryGet_p(tbP.Text, out p, ref isValid);
            _validator.TryGet_q(tbQ.Text, out q, ref isValid);
            
            n = 0;
            if (isValid) {
                n = p * q;
                _validator.Validate_n(n, ref isValid);
            }
            
            _validator.TryGet_b(tbB.Text, out b, ref isValid, n);
            
            return isValid;
        }
    }
}
