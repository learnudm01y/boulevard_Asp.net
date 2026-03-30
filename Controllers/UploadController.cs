using Boulevard.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Razor.Tokenizer;

namespace Boulevard.Controllers
{
    public class UploadController : BaseController
    {
        [HttpPost]
        public IHttpActionResult PostImages()
        {
            var httpRequest = HttpContext.Current.Request;
            var imagelist = new List<string>();

            if (httpRequest.Files.Count <= 0)
                return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotAcceptable, message = "No file is uploaded!", isSuccess = false });
            //       HttpPostedFileBase filebase =
            //new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);
            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                var postedFile = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[i]); ;
                if (postedFile == null)
                    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.Created, message = "File is invalid!", isSuccess = false });
                var path = "Content/Upload/Images";
                var fileName = ImageProcess.UploadOriginalFile(postedFile, path);

                imagelist.Add(fileName);
            }
            return SuccessMessage(imagelist);
        }








        [HttpPost]
        public IHttpActionResult PostFiles()
        {
            var httpRequest = HttpContext.Current.Request;
            var imagelist = new List<string>();

            if (httpRequest.Files.Count <= 0)
                return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotAcceptable, message = "No file is uploaded!", isSuccess = false });
            //       HttpPostedFileBase filebase =
            //new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);
            for (int i = 0; i < httpRequest.Files.Count; i++)
            {
                var postedFile = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[i]); ;
                if (postedFile == null)
                    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.Created, message = "File is invalid!", isSuccess = false });
                var path = "Content/Upload/Files";
                var fileName = ImageProcess.UploadFileImport(postedFile, path);

                imagelist.Add(fileName);
            }
            return SuccessMessage(imagelist);
        }


        public IHttpActionResult PostVideos()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request.Files[0];

                //if (httpRequest.Files.Count <= 0)
                //    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotAcceptable, message = "No file is uploaded!", isSuccess = false });
                //       HttpPostedFileBase filebase =
                //new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]);
                var postedFile = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[0]); ;
                if (postedFile == null)
                    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.Created, message = "File is invalid!", isSuccess = false });
                var path = "Content/Upload/Videos";
                var fileName = ImageProcess.UploadVideoFile(postedFile, path);

                return SuccessMessage(fileName);
            }
            catch (Exception exce)
            {

                throw;
            }

        }
    }
}
