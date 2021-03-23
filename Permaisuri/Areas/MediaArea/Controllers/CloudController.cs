using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Permaisuri.Cloud;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Areas.MediaArea.Controllers
{
    public class CloudController : Controller
    {
        //
        // GET: /MediaArea/Cloud/ http://localhost:3224/MediaArea/Cloud/Index

        public ActionResult Index()
        {
            ViewBag.CMSImgUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath + "/MediaLib/Files/";	
            return View();
        }


        public ActionResult GetMediaList(MediaLibrary_QueryModel queryModel)
        {
            MediaServices ms = new MediaServices();
            int count = 0;
            try
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        rows = ms.GetMediaList(queryModel, out count),
                        total = count
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }


        public ActionResult CloudUploadWithMediaList(List<long> MediaIDList)
        {

            CloudUpload cloud = new CloudUpload( Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"]));
            cloud.CloudUploadWithMediaList(MediaIDList);
            return Json(new NBCMSResultJson
            {
                Status = StatusType.OK,
                Data = "OK"
            });
        }
         
    }
}
