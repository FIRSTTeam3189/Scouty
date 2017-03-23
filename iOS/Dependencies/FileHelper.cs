using System;
using System.IO;
using Scouty;
using Scouty.iOS;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace Scouty.iOS
{
	public class FileHelper : IFileHelper
	{
		public FileHelper() { }
		public string GetLocalFilePath(string filename)
		{
			var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var libFolder = Path.Combine(docFolder, "..", "Library", "Local");

			if (!Directory.Exists(libFolder))
				Directory.CreateDirectory(libFolder);

			return Path.Combine(libFolder, filename);
		}

		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void DeleteFile(string path)
		{
			File.Delete(path);
		}

		public string ReadFile(string path)
		{
			return File.ReadAllText(path);
		}

		public void WriteFile(string path, string text)
		{
			File.WriteAllText(path, text);
		}
	}
}
