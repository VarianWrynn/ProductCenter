using System;
using System.Web.Mvc;
using PermaisuriCMS.Model;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using System.Configuration;
using System.Web.Script.Serialization;

namespace Permaisuri.Controllers
{
    public class BrandsController : Controller
    {
        //
        // GET: /Brands/

        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Change:Change to support EF style, ChangeDate:2013年12月9日18:51:11
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public ActionResult GetBrandList(Brands_Info_Model queryModel)
        {
            try
            {
                var bis = new BrandInfoServices();
                int count;
                var list = bis.GetBrandList(queryModel, out count);
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        total = count,
                        rows = list
                    }
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        public ActionResult AddBrand(Brands_Info_Model model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                var curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                var bis = new BrandInfoServices();
                string msg = string.Empty;
                if (bis.AddBrand(model, curUserInfo.User_Account, ref msg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully add brand"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = msg == string.Empty ? "faile to add new brand" : msg
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        public ActionResult EditBrand(Brands_Info_Model model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }

                var cookis = Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]];
                var serializer = new JavaScriptSerializer();
                var decCookies = CryptTools.Decrypt(cookis);
                var curUserInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model;
                var bis = new BrandInfoServices();

                var msg = string.Empty;
                if (curUserInfo != null && bis.EditBrand(model, curUserInfo.User_Account, ref msg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully edit brand"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = msg == string.Empty ? "faile to add new brand" : msg
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }

        }

        public ActionResult DeleteBrand(int brandId)
        {
            try
            {
                var bis = new BrandInfoServices();
                var mgs = string.Empty;

                if (bis.DeleteBrand(brandId, ref mgs))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully delete brand"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = mgs == string.Empty ? "Faile to delete brand" : mgs
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Fatal(ex.Message);
                NBCMSLoggerManager.Fatal(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }
    }
}
