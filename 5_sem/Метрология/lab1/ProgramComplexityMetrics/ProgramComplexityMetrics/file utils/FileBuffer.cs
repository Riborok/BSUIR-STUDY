using System;

namespace ProgramComplexityMetrics.file_utils {
	internal class FileBuffer<T> where T : class {
		private readonly Action<T?> _updateText;
		private readonly FileInteraction<T> _fileInteraction;
		private T? _buffer;
		
		public FileBuffer(Action<T?> updateText, FileInteraction<T> fileInteraction) {
			_updateText = updateText;
			_fileInteraction = fileInteraction;
		}

		public void Reset() {
			_fileInteraction.Reset();
			_buffer = null;
			_updateText(_buffer);
		}
		
		public void Create() {
			_fileInteraction.Create();
		}
		
		public void Open() {
			var opened = _fileInteraction.Open();
			if (opened != null) {
				_buffer = opened;
				_updateText(_buffer);
			}
		}
		
		public void SaveAs() {
			_fileInteraction.SaveAs(_buffer);
		}
		
		public void Save() {
			_fileInteraction.Save(_buffer);
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
