using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using OfficeOpenXml;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using File = Google.Apis.Drive.v3.Data.File;

namespace moonNest.editor
{
    internal class GoogleDriveHelper
    {
        #region Drive Service
        static DriveService _driveService;
        static DriveService DriveService
        {
            get
            {
                if(_driveService == null)
                {
                    string[] Scopes = { DriveService.Scope.Drive };
                    UserCredential credential;
                    using(var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                    {
                        // The file token.json stores the user's access and refresh tokens, and is created
                        // automatically when the authorization flow completes for the first time.
                        string tempFolder = "Temp";
                        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets,
                            Scopes,
                            "user",
                            CancellationToken.None,
                            new FileDataStore(tempFolder, true)).Result;
                        Console.WriteLine("Credential file saved to: " + tempFolder);
                    }

                    // Create Drive API service.
                    _driveService = new DriveService(new BaseClientService.Initializer() { HttpClientInitializer = credential });
                }
                return _driveService;
            }
        }
        #endregion

        internal static async Task<string> CreateFile(string contentType)
        {
            var memoryStream = new MemoryStream();
            var ep = new ExcelPackage(memoryStream);
            ep.Workbook.Worksheets.Add("Sheet 1");
            ep.Save();

            EditorUtility.DisplayProgressBar("Create File", "Creating file...", 1f);

            var file = new File() { Name = Application.productName };
            var createRequest = DriveService.Files.Create(file, memoryStream, contentType);
            var uploadProgress = await createRequest.UploadAsync();
            EditorUtility.ClearProgressBar();
            if(uploadProgress.Status == UploadStatus.Failed)
            {
                EditorUtility.ClearProgressBar();
                Draw.DisplayDialog("Create File Failed", uploadProgress.Exception.Message, "Close");
                return null;
            }
            else
            {
                EditorUtility.DisplayProgressBar("Create File", "Setting permission...", 1f);

                string fileId = createRequest.ResponseBody.Id;
                var permission = new Permission() { Type = "anyone", Role = "writer" };
                await DriveService.Permissions.Create(permission, fileId).ExecuteAsync();
                EditorUtility.ClearProgressBar();

                if(Draw.DisplayDialog("Complete", "Create new google drive file successful!", "Open File", "Close"))
                {
                    Application.OpenURL("https://docs.google.com/spreadsheets/d/" + fileId);
                }
                return fileId;
            }
        }

        internal static Stream Import(string fileId)
        {
            var task = DriveService.Files.Get(fileId).ExecuteAsStreamAsync();
            while(!task.IsCompleted)
            {
                EditorUtility.DisplayProgressBar("Import/Export", "Downloading ...", 1f);
                Thread.Sleep(50);
            }
            EditorUtility.ClearProgressBar();
            return task.Result;
        }

        internal static void Export(string fileId, Stream stream, string contentType)
        {
            var file = new File();
            var task = DriveService.Files.Update(file, fileId, stream, contentType).UploadAsync();
            while(!task.IsCompleted)
            {
                EditorUtility.DisplayProgressBar("Import/Export", "Exporting ...", 1f);
                Thread.Sleep(50);
            }
            EditorUtility.ClearProgressBar();
            if(Draw.DisplayDialog("Import/Export", "Export Complete!", "Open File", "Close"))
            {
                Application.OpenURL("https://docs.google.com/spreadsheets/d/" + fileId);
            }
        }
    }
}