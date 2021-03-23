using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;
using PermaisuriCMS.BLL;

namespace Permaisuri.Areas.SKUArea.Controllers
{
    /// <summary>
    /// Q:为什么要把ShipVia置入这里维护？
    /// A：其实放在ProductConfiguration页面结构看起来更合理，但是该页面的脚本已经达到了10多个，是在不宜再置入更多扩展，
    /// 作为SKU、HMNUM的属性之一，ShipVia放在这里维护也能接受。 2014年5月9日17:18:27
    /// </summary>
    public class ShipViaController : Controller
    {
        //
        // GET: /SKUArea/ShipVia/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetShipViaList(CMS_ShipVia_Model qModel)
        {
            int count = 0;
            try
            {
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new
                    {
                        rows = new ShipViaServices().GetShipViaList(qModel, out count),
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

        /// <summary>
        /// CMS只需要知道SHIPVIAType（物流or快递）就足够了，但是同步到eCom需要确切知道type下面的子类型.2014年5月12日10:07:32
        /// </summary>
        /// <param name="sModel"></param>
        /// <returns></returns>
        public ActionResult UpdateDefaultShipVia(CMS_ShipVia_Model sModel, User_Profile_Model userModel)
        {
            try
            {
                var isSucceesfue = new ShipViaServices().UpdateDefaultShipVia(sModel, userModel);
                return Json(new NBCMSResultJson
                {
                    Status = isSucceesfue == true ? StatusType.OK : StatusType.Error,
                    Data = isSucceesfue == true ? "OK" : "Fail to set default shipvia for current item."
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


        public ActionResult AddShipVia(CMS_ShipVia_Model shipViaModel,User_Profile_Model userModel)
        {
            try
            {
                string msg = string.Empty;
                var isSucceesfue = new ShipViaServices().AddShipVia(shipViaModel, userModel, out msg);
                if (isSucceesfue)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == string.Empty ? "Fail to add new ship via" : msg
                    });
                }

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



        public ActionResult EditShipVia(CMS_ShipVia_Model shipViaModel, User_Profile_Model userModel)
        {
            try
            {
                string msg = string.Empty;
                var isSucceesfue = new ShipViaServices().EditShipVia(shipViaModel, userModel, out msg);
                if (isSucceesfue)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == string.Empty ? "Fail to edit ship via" : msg
                    });
                }

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


        public ActionResult DeleteShipVia(CMS_ShipVia_Model shipViaModel, User_Profile_Model userModel)
        {
            try
            {
                string msg = string.Empty;
                var isSucceesfue = new ShipViaServices().DeleteShipVia(shipViaModel, userModel, out msg);
                if (isSucceesfue)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = "OK"
                    });
                }
                else
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = msg == string.Empty ? "Fail to edit ship via" : msg
                    });
                }

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

    }
}
