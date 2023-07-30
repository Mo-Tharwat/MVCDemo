using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Demo.PL.Helpers
{
    public static class DocumentSettings
    {
        /// <summary>
        /// this method is used for : uploading a file in a server and change the file name to make it unique
        /// </summary>
        /// <param name="file">the file that you want to upload</param>
        /// <param name="folderName">Name Of the Folder in the Server that the File Well be located</param>
        /// <returns>returns the new file name in the server to use it after the changes made to the name</returns>
        public static string UploadFile(IFormFile file, string folderName)
        {
            // Locate Folder Path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

            // Get File Name and make it Unique
            string fileName = $"{Guid.NewGuid()}{file.FileName}";

            // Get File Path
            string filePath = Path.Combine(folderPath, fileName);

            // Save File as streams : [Data Per Time]
            using var fs = new FileStream(filePath,FileMode.Create);

            file.CopyTo(fs);

            return fileName;
        }

        public static void DeleteFile(string fileName, string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName, fileName);

            if (File.Exists(filePath))
                    File.Delete(filePath);
        }
    }
}
