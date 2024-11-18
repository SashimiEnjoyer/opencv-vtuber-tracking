using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FUnit
{
    public class DirectoryUtil
    {
        public static string[] GetDirectories(string path, string searchPattern = "*")
        {
            string[] directories = null;
            try
            {
                directories = Directory.GetDirectories(path, searchPattern);
            }
            catch (DirectoryNotFoundException exception)
            {
                Debug.LogError(path + "\nDirectory not found\n\n" + exception.ToString());
            }
            catch (PathTooLongException exception)
            {
                Debug.LogError(path + "\nThe path is too long\n\n" + exception.ToString());
            }
            catch (IOException exception)
            {
                Debug.LogError(path + "\nCould not access directory\n\n" + exception.ToString());
            }
            catch (System.UnauthorizedAccessException exception)
            {
                Debug.LogError(path + "\nYou do not have permission to access the directory\n\n" + exception.ToString());
            }

            if (directories == null)
            {
                directories = new string[0];
            }
            return directories;
        }

        public static IEnumerable<string> GetFilesRecursively(string path, string searchPattern = "*.*")
        {
            var files = new List<string>();
            GetFilesRecursively(path, searchPattern, files);
            return files;
        }

        public static void GetFilesRecursively(string path, string searchPattern, List<string> files)
        {
            files.AddRange(GetFiles(path, searchPattern));
            var subdirectories = GetDirectories(path);
            foreach (var subdirectory in subdirectories)
            {
                GetFilesRecursively(subdirectory, searchPattern, files);
            }
        }

        public static string[] GetFiles(string path, string searchPattern = "*.*")
        {
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path, searchPattern);
            }
            catch (DirectoryNotFoundException exception)
            {
                Debug.LogError(path + "\nDirectory not found\n\n" + exception.ToString());
            }
            catch (PathTooLongException exception)
            {
                Debug.LogError(path + "\nThe path is too long\n\n" + exception.ToString());
            }
            catch (IOException exception)
            {
                Debug.LogError(path + "\nCould not access directory\n\n" + exception.ToString());
            }
            catch (System.UnauthorizedAccessException exception)
            {
                Debug.LogError(path + "\nYou do not have permission to access the directory\n\n" + exception.ToString());
            }

            if (files == null)
            {
                files = new string[0];
            }
            return files;
        }

        public static bool TryToCreateDirectory(string directoryName)
        {
            directoryName = PathUtil.NormalizePath(directoryName);
            if (Directory.Exists(directoryName))
            {
                return true;
            }

            var succeeded = false;
            var errorMessage = "";
            try
            {
                Directory.CreateDirectory(directoryName);
                succeeded = true;
            }
            catch (IOException)
            {
                errorMessage = "Invalid path";
            }
            catch (System.UnauthorizedAccessException)
            {
                errorMessage = "You do not have access";
            }
            catch (System.ArgumentException)
            {
                errorMessage = (directoryName.Length == 0) ?
                    "Path is empty" : "The path contains invalid characters";
            }
            catch (System.NotSupportedException)
            {
                errorMessage = "Path is not supported";
            }

            if (!succeeded)
            {
                Debug.LogError("Could not create directory： " + directoryName + "\n" + errorMessage);
            }
            return succeeded;
        }
    }
}