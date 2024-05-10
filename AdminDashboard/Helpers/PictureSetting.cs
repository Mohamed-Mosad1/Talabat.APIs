namespace AdminDashboard.Helpers
{
	public class PictureSetting
	{
		public static string UploadFile(IFormFile file, string folderName)
		{
			// Get Folder Path
			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", folderName);
			// Set FileName Unique
			var fileName = Guid.NewGuid() + file.FileName;
			// Get File Path
			var filePath = Path.Combine(folderPath, fileName);
			// Save File as Streams
			var fileStream = new FileStream(filePath, FileMode.Create);
			// Copy File Into Stream
			file.CopyTo(fileStream);
			// Return FileName
			return Path.Combine("images\\products", fileName);
		}

		public static void DeleteFile(string folderName, string fileName)
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", folderName, fileName);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	
	}
}
