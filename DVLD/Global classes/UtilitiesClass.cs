using GlobalClasses;
using System;
using System.IO;

namespace DVLD.Global_classes
{
    internal static class UtilitiesClass
    {
        // Ensure the destination folder exists; create it if necessary.
        // Returns true when the folder exists or was created successfully, false on error.
        private static bool CreateFolder(string DestinationFolder)
        {
            if (!Directory.Exists(DestinationFolder))
            {
                try
                {
                    Directory.CreateDirectory(DestinationFolder);
                    return true;
                }
                catch (Exception Ex) { ClsLogger.Log(Ex); return false; }
            }

            return true;
        }

        // Return a new GUID as string (used to build unique filenames).
        private static string GetGUID()
        {
            Guid NewGUID = Guid.NewGuid();

            return NewGUID.ToString();
        }

        // Replace the original filename with a GUID while preserving its extension.
        // Example: "photo.jpg" -> "c81e728d-9d4c-3f63-af06-... .jpg"
        private static string ReplaceFileNameWithGUID(string FileName)
        {
            FileInfo ImageFile = new FileInfo(FileName);

            string FEX = ImageFile.Extension;

            return GetGUID() + FEX;
        }

        public static bool CopyToProjectImagesFolder(ref string SourceImageFile)
        {
            string DestinationFolder = @"C:\DVLD-People-Images\";

            if (!CreateFolder(DestinationFolder))
                return false;

            string DestinationFile = DestinationFolder + ReplaceFileNameWithGUID(SourceImageFile);

            try
            {
                File.Copy(SourceImageFile, DestinationFile, true);
            }
            catch (Exception Ex) { ClsLogger.Log(Ex); return false; }

            // Replace source path with the new destination path so callers can use it.
            SourceImageFile = DestinationFile;

            return true;
        }
    }
}