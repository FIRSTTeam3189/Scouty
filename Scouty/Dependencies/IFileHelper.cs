using System;

namespace Scouty
{
	public interface IFileHelper
	{
		string GetLocalFilePath(string filename);
		bool FileExists(string path);
		void DeleteFile(string path);
		string ReadFile(string path);
		void WriteFile(string path, string text);

	}
}
