using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NPOI.SS.Formula.Functions;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class ChannelController : Controller
    {
        //
        // GET: /Channel/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult GetChannelList(Channel_Model queryModel) { 
			try {
				var cis = new ChannelInfoServices();
				int count = 0;
                List<Channel_Model> list = cis.GetChannelList(queryModel,out count);
				return Json(new NBCMSResultJson {
					Status = StatusType.OK,
					Data = new {
						total = count,
						rows = list
					}
				});
			}
			catch (Exception ex){
				NBCMSLoggerManager.Fatal(ex.Message);
				NBCMSLoggerManager.Fatal(ex.StackTrace);
				NBCMSLoggerManager.Error("");
				return Json(new NBCMSResultJson {
					Status = StatusType.Exception,
					Data = ex.Message
				});
			}				
		}

        /// <summary>
        /// Change1：新增重复插入判断（ChannleName).2014年2月20日
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		public ActionResult AddChannel(Channel_Model model) {
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
                var cis = new ChannelInfoServices();

                //if(curUserInfo==null)
                //{
                //    return Json(new NBCMSResultJson
                //    {
                //        Status = StatusType.Exception,
                //        Data = "aa"
                //    });
                //}

                var msg = string.Empty;
                if (cis.AddChannel(model, curUserInfo.User_Account, ref msg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully add Channel"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == string.Empty ? "faile to add new Channel" : msg
                    });
                }
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


        public ActionResult DeleteChannel(int ChannelID)
        {
            try
            {
                ChannelInfoServices cis = new ChannelInfoServices();
                string mgs = string.Empty;
                if (cis.DeleteChannel(ChannelID, ref mgs))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully delete channel"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = mgs == string.Empty ? "Faile to delete channel" : mgs
                    });
                }
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

        public ActionResult EditChannel(Channel_Model model)
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

                User_Profile_Model curUserInfo = new CommonController().GetCurrentUserbyCookie( Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
                ChannelInfoServices cis = new ChannelInfoServices();

                string msg = string.Empty;
                if (cis.EditChannel(model, curUserInfo.User_Account, ref msg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "Successfully edit channel"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == string.Empty ? "faile to add new Channel" : msg
                    });
                }
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
