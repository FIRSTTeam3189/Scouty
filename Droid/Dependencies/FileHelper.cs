using System;
using System.IO;
using Scouty.Droid;

using Scouty;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace Scouty.Droid
{
	public class FileHelper : IFileHelper
	{
		public FileHelper() { }
		public string GetLocalFilePath(string filename)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, filename);
		}

		public bool FileExists(string path) {
			return File.Exists(path);
		}

		public void DeleteFile(string path) {
			File.Delete(path);
		}

		public string ReadFile(string path) {
			return File.ReadAllText(path);
		}

		public void WriteFile(string path, string text) {
			File.WriteAllText(path, text);
		}
	}
}
