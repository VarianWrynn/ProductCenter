using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Permaisuri.Upload
{
    public class FilesStatus
    {
        public const string HandlerPath = "../Upload/";//根目录下...Lee 2013年10月15日15:12:33

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }

        public string only4DeleteUrl { get; set; }

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo) { SetValues(fileInfo.Name, (int)fileInfo.Length, fileInfo.FullName,0,""); }

        /// <summary>
        ///  注意删除路径，只想的是Media的Controller而不是默认的路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileLength"></param>
        /// <param name="fullPath"></param>
        /// <param name="id">newID即指 新增的MediaID</param>
        /// <param name="HMNUM"></param>
        public FilesStatus(string fileName, int fileLength, string fullPath, long id,string HMNUM) { SetValues(fileName, fileLength, fullPath, id, HMNUM); }

        private void SetValues(string fileName, int fileLength, string fullPath, long id,string HMNUNM)
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = HandlerPath + "UploadHandler.ashx?f=" + fileName + "&HMNUM=" + HMNUNM;
            delete_url = HandlerPath + "UploadHandler.ashx?f=" + fileName;
            delete_type = "DELETE";

            only4DeleteUrl = "../Media/Delete?f=" + fileName + "&id=" + id + "&HMNUM=" + HMNUNM;//根目录/Media/Delete...

            var ext = Path.GetExtension(fullPath);

            var fileSize = ConvertBytesToMegabytes(new FileInfo(fullPath).Length);
            if (fileSize > 3 || !IsImage(ext)) thumbnail_url = "/Content/images/generalFile.png";
            //else thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath);
            else thumbnail_url = HandlerPath + "UploadHandler.ashx?f=" + fileName + "&HMNUM=" + HMNUNM;
        }

        private bool IsImage(string ext)
        {
            return ext == ".gif" || ext == ".jpg" || ext == ".png";
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}