using Microsoft.CloudInfrastructure.SecurityHelper;
using System;
using System.IO;
using System.Net;

namespace GetAPILogsfromServers
{
    internal class Program
    {
        // variables declaration

        public static DirectoryInfo directoryInfo = null;
        public static FileInfo[] files = null;
        public static string[] machineNames = System.Configuration.ConfigurationManager.AppSettings["MchineNames"].Split(',');

        private static void Main(string[] args)
        {
            ImpersonationHelper.InvokeActionAsImpersonatedUserForRemoteResource<object, object>(
                                                ConfigurationHelper.AxUserName,
                                                ConfigurationHelper.AxUserDomain,
                                                ConfigurationHelper.AxUserPassword,
                                                f => { return ReadLogFiles();
                                                }, null);
        }

        private static object ReadLogFiles()
        {
            string archiveFolder = null;
            string folder = null;
            string server = null;
            WebRequest FileRequest;
            WebResponse FileResponse;
            //StreamReader reader;

            try
            {
                if (!string.IsNullOrEmpty(machineNames.ToString()) && !string.IsNullOrEmpty(machineNames.ToString()))
                {
                    for (int k = 0; k < machineNames.Length; k++)
                    {
                        server = machineNames[k];

                        archiveFolder = System.Configuration.ConfigurationManager.AppSettings["ArchiveFolder"];
                        folder = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
                        folder = Path.Combine(@"\\" + machineNames[k] + folder);
                        archiveFolder = Path.Combine(@"\\" + machineNames[k] + archiveFolder, "Archive\\\\");

                        DateTime Lastmodifieddate = System.IO.File.GetLastWriteTime(folder);
                        directoryInfo = new DirectoryInfo(folder);

                        files = directoryInfo.GetFiles();

                        for (int i = 0; i < files.Length; i++)
                        {
                            string fileCreationtime = files[i].CreationTime.ToShortDateString();
                            string currentDateTime = DateTime.Now.ToShortDateString();
                            FileRequest = System.Net.WebRequest.Create(files[i].FullName);

                            if (IsFileBlocked(files[i].FullName))
                                continue;

                            FileResponse = FileRequest.GetResponse();
                            Stream dataStream = FileResponse.GetResponseStream();
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                GetFiles getFiles = new GetFiles();
                                getFiles.ParseLogData(reader, server);
                                reader.Dispose();
                                MoveFile(archiveFolder, files[i].FullName.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
            return null;
        }

        private static bool IsFileBlocked(String fileFullName)
        {
            var request = WebRequest.Create(fileFullName);
            try
            {
                request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Message.ToLower().Contains("the process cannot access the file"))
                {
                    return true;
                }
            }
            return false;
        }

        private static void MoveFile(string archiveFolder, string fileName)
        {
            string destFile = null;
            if (!Directory.Exists(archiveFolder))
            {
                Directory.CreateDirectory(archiveFolder);
            }
            GC.Collect();
            string fname = Path.GetFileName(fileName.ToString());
            destFile = Path.Combine(archiveFolder, fname);
            File.Copy(fileName, destFile, true);
            File.Delete(fileName);
        }
    }
}