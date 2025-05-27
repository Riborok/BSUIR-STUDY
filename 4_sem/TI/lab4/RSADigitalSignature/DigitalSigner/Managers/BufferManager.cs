using System;
using DigitalSigner.FileUtils;

namespace DigitalSigner.Managers {
	internal class BufferManager<T> where T : class {
		private readonly Action<T?> _updateText;
		private readonly FileManager<T> _fileManager;
		private T? _buffer;
		
		public BufferManager(Action<T?> updateText, FileManager<T> fileManager) {
			_updateText = updateText;
			_fileManager = fileManager;
		}

		public void Reset() {
			_fileManager.Reset();
			_buffer = null;
			_updateText(_buffer);
		}
		
		public void Create() {
			_fileManager.Create();
		}
		
		public void Open() {
			var opened = _fileManager.Open();
			if (opened != null) {
				_buffer = opened;
				_updateText(_buffer);
			}
		}
		
		public void SaveAs() {
			_fileManager.SaveAs(_buffer);
		}
		
		public void Save() {
			_fileManager.Save(_buffer);
		}

		public T? Buffer {
			get => _buffer;
			set {
				_buffer = value;
				_updateText(_buffer);
			}
		} 
	}
}
