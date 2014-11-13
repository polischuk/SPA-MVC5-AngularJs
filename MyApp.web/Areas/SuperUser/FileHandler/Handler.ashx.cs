using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MyApp.data;

namespace MyApp.web.Areas.SuperUser.FileHandler
{
    public class Handler : IHttpHandler
    {
        private readonly JavaScriptSerializer _js;

        private string CustomContentFolder
        {
            get { return Path.Combine(HttpContext.Current.Server.MapPath("~/Content/ObligContent/")); }
        }

        private string _obligWebPath = "~/Content/ObligContent/";

        private volatile object _syncRoot = new object();
        private readonly ApplicationDbContext _context;
        private object SyncRoot
        {
            get { return _syncRoot; }
        }

        public Handler()
        {
            _context = DependencyResolver.Current.GetService<ApplicationDbContext>();
            _js = new JavaScriptSerializer { MaxJsonLength = 4194304 };
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context)) DeliverFile(context);
                    else if (context.User.Identity.IsAuthenticated) ListCurrentFiles(context);
                    break;

                case "POST":
                case "PUT":
                    if (!context.User.Identity.IsAuthenticated) return;
                    UploadFile(context);
                    break;

                case "DELETE":
                    if (!context.User.Identity.IsAuthenticated) return;
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            //What to delete?
            var fileType = context.Request["what"];
            var oid = context.Request["objectId"];
            var name = context.Request["f"];

            string filePath;

            if (fileType == null)
            {
                filePath = CustomContentFolder + name;
            }
            else
            {
                switch (fileType)
                {
                    case "oblig-content":
                        filePath = CustomContentFolder + context.Request.QueryString["GalleryId"] + name;
                        break;
                    default:
                        return;
                }
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                //UploadPartialFile(headers["X-File-Name"], context, statuses, sessionId);
                context.Response.Write("Partial file upload is not supported!");
            }

            WriteJsonIframeSafe(context, statuses);
        }



        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            var imageId = context.Request.QueryString["ImageId"];
            var compressionType = context.Request.QueryString["Compression"];
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];

                if (file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName)) continue;
                if (imageId == null)
                {
                    //What to delete?
                    var fileType = context.Request["what"];
                    var oid = context.Request["objectId"];

                    string dirPath = CustomContentFolder;

                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                    var name = string.Empty;

                    if (fileType == "oblig-content") name = file.FileName;
                    else name = oid + ".png";


                    var fullPath = dirPath + Path.GetFileName(name);

                    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                    file.SaveAs(fullPath);

                    var fullName = Path.GetFileName(fullPath);

                    if (fileType == "oblig-content")
                    {
                        fullPath = _obligWebPath + fullName;

                        statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath, true));
                    }
                    else statuses.Add(new FilesStatus(fullName, file.ContentLength, fullPath, false));
                }

                

                else if (!string.IsNullOrWhiteSpace(imageId))
                {
                    var fullPath = CustomContentFolder + "AccountImage\\";
                    if (!Directory.Exists(fullPath))
                        Directory.CreateDirectory(fullPath);
  
                    file.SaveAs(fullPath + imageId + ".png");
                }

            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                context.Response.ContentType = context.Request["HTTP_ACCEPT"].Contains("application/json") ? "application/json" : "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = _js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var fileType = context.Request["what"];

            string filePath;

            switch (fileType)
            {
                default:
                    return;
            }

            if (File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }

        private void ListCurrentFiles(HttpContext context)
        {
            if (!Directory.Exists(CustomContentFolder))
            {
                Directory.CreateDirectory(CustomContentFolder);
            }

            var files =
                new DirectoryInfo(CustomContentFolder)
                    .GetFiles("*", SearchOption.TopDirectoryOnly)
                    .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FilesStatus(f, true))
                    .ToArray();

            string jsonObj = _js.Serialize(files);
            context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
            context.Response.Write(jsonObj);
            context.Response.ContentType = "application/json";
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            var codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public ActionResult UploadImage(string fileData)
        {
            string dataWithoutJpegMarker = fileData.Replace("data:image/jpeg;base64,", String.Empty);
            byte[] filebytes = Convert.FromBase64String(dataWithoutJpegMarker);
            string writePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/ObligContent/")) + new Guid() + ".jpg";
            using (FileStream fs = new FileStream(writePath,
                                            FileMode.OpenOrCreate,
                                            FileAccess.Write,
                                            FileShare.None))
            {
                fs.Write(filebytes, 0, filebytes.Length);
            }
            return new EmptyResult();
        }
    }
}