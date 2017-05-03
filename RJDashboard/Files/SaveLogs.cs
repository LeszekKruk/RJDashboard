using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJDashboard.Files
{
    public class SaveLogs
    {
        public static async void Save(string fileName, string dataToSave)
        {
            if (dataToSave.Length > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    await outputFile.WriteAsync(dataToSave);
                }
            }
        }

        public static async void Save(string fileName, List<string> dataToSave)
        {
            if (dataToSave.Count() > 0)
            {
                StringBuilder fileContents = new StringBuilder();
                foreach (var data in dataToSave)
                {
                    fileContents.AppendLine(data);
                }

                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    await outputFile.WriteAsync(fileContents.ToString());
                }
            }
        }
    }
}
