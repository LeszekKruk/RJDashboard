using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJDashboard.Files
{
    struct AppFolders
    {
        public string FolderName(FolderType folderType)
        {
            string folderName = "";

            switch (folderType)
            {
                case FolderType.Application:
                    folderName = "RJDashboard";
                    break;
                case FolderType.LabelSettings:
                    folderName = "Label settings";
                    break;
                case FolderType.LogEvents:
                    folderName = "Log events";
                    break;
                case FolderType.Database:
                    folderName = "Databases";
                    break;
                default:
                    break;
            }

            return folderName;
        }

        public string Create(string path, FolderType folderType)
        {
            string pathAndFolderName = "";

            pathAndFolderName = path + "\\" + FolderName(folderType);

            try
            {
                if (!Directory.Exists(pathAndFolderName))
                {
                    DirectoryInfo di = Directory.CreateDirectory(pathAndFolderName);
                }
            }
            catch (Exception)
            {

            }
            return pathAndFolderName;
        }
    }

    enum FolderType
    {
        Application = 0,
        LabelSettings = 1,
        LogEvents = 2,
        Database = 3
    };
}
