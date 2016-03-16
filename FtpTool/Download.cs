using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Configuration;

namespace FtpTool
{
    public class Download
    {
        private readonly string _ftpServer;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _serverPath;
        private readonly string _localPath;
        private const int gMaxRetry = 5;
        private static string gLocalPath = @"";


        public Download(string server, string username, string password, string remotePath, string localPath)
        {
            _ftpServer = !string.IsNullOrWhiteSpace(server) ? server : ConfigurationManager.AppSettings["FtpServer"];
            _userName = !string.IsNullOrWhiteSpace(username) ? username : ConfigurationManager.AppSettings["FtpUserName"];
            _password = !string.IsNullOrWhiteSpace(password) ? password : ConfigurationManager.AppSettings["Password"];
            _serverPath = !string.IsNullOrWhiteSpace(remotePath) ? remotePath : ConfigurationManager.AppSettings["ServerPath"];
            _localPath = !string.IsNullOrWhiteSpace(localPath) ? localPath : ConfigurationManager.AppSettings["LocalPath"];
        }

        public int Execute()
        {

            Console.WriteLine("Downloading files to {0}", _localPath);
            var fileList = new List<string>();
            var downloadUri = _ftpServer + "/" + _serverPath + "/";
            fileList = BuildFileList(downloadUri);

            return DownloadImages(fileList);
        }
        private List<string> BuildFileList(string uri)
        {
            var fileList = new List<string>();
            var serverUri = new UriBuilder("ftp", uri);
            var request = (FtpWebRequest)WebRequest.Create(serverUri.ToString());
            request.Credentials = new NetworkCredential(_userName, _password);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            var response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            using (StreamReader reader = new StreamReader(responseStream))
            {
                var aLine = "";
                while (true)
                {
                    aLine = reader.ReadLine();
                    if (aLine != null)
                    {
                        if (aLine.Contains(".jpg"))
                        {
                            fileList.Add(uri + "/" + aLine);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return fileList;
        }

        private int DownloadImages(List<string> fileList)
        {
            var count = 0;
           
            using (var request = new WebClient())
            {
                request.Credentials = new NetworkCredential(_userName, _password);

                foreach (string key in fileList)
                {
                    var keyUri = "ftp://" + key;
                    var destinationPath = key.Replace(_ftpServer, _localPath).Replace("/", @"\");

                    var retryAttempt = 0;
                    while (retryAttempt <= gMaxRetry)
                    {
                        try
                        {
                            var dr = request.ToString();
                            request.DownloadFile(keyUri, destinationPath);
                            retryAttempt = gMaxRetry + 1;
                            count++;
                        }
                        catch (Exception ex)
                        {
                            retryAttempt++;
                            Console.WriteLine("Failed to download file: {0} \n {1}", key, ex.Message);
                            if (retryAttempt <= gMaxRetry)
                            {
                                Console.WriteLine("Retry attempt {0}/{1}", retryAttempt, gMaxRetry);
                            }
                            else
                            {
                                Console.WriteLine(
                                    "Unable to download file after {0} attempts. Please engage eVox support.", gMaxRetry);
                            }

                        }
                    }
                }
                return count;
            }
        }
    }
}
