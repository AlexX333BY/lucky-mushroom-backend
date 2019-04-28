using LuckyMushroom.DataTransferObjects;
using LuckyMushroom.Models;
using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace LuckyMushroom.Helpers
{
    public class RequestPhotoSaver
    {
        static RequestPhotoSaver() {
            SaveDirectory = Directory.GetCurrentDirectory();
        }

        protected static DirectoryInfo saveDirectory;

        public static string SaveDirectory { 
            get => saveDirectory.FullName; 
            set {
                saveDirectory = Directory.CreateDirectory(value);
            }
        }

        public string SavePhoto(byte[] photo, string extension)
        {
            byte[] fileHash = new SHA1Managed().ComputeHash(photo);
            string filename = SaveDirectory + DateTime.UtcNow.ToString("s") + '_' 
                + string.Concat(fileHash.Select(b => b.ToString("x2"))) + extension;
            File.WriteAllBytes(filename, photo);
            return filename;
        }

        public byte[] ReadPhoto(string filename)
        {
            return File.ReadAllBytes(filename);
        }
    }
}