using System.IO;

namespace ProgramComplexityMetrics.file_utils {
	internal interface IFileService<T> {
		void CreateFile(string path);
		T ReadFile(string path);
		void SaveFile(string path, T content);
	}

	internal class BinaryFileService : IFileService<byte[]> {
		public void CreateFile(string path) {
			try {
				File.Create(path).Dispose();
			} catch {
				throw new IOException("The file could not be created!");
			}
		}

		public byte[] ReadFile(string path) {
			try {
				return File.ReadAllBytes(path);
			} catch {
				throw new IOException("The file could not be read!");
			}
		}

		public void SaveFile(string path, byte[] content) {
			try {
				File.WriteAllBytes(path, content);
			} catch {
				throw new IOException("The file could not be saved!");
			}
		}
	}
	
	internal class TextFileService : IFileService<string> {
		public void CreateFile(string path) {
			try {
				File.Create(path).Dispose();
			}
			catch {
				throw new IOException("The file could not be created!");
			}
		}

		public string ReadFile(string path) {
			try {
				return File.ReadAllText(path);
			}
			catch {
				throw new IOException("The file could not be read!");
			}
		}

		public void SaveFile(string path, string content) {
			try {
				File.WriteAllText(path, content);
			}
			catch {
				throw new IOException("The file could not be saved!");
			}
		}
	}
}
