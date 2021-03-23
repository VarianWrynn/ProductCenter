using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    public class HMGroupCreateController : Controller
    {
        //
        // GET: /HMGroupCreate/

        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// 获取类别下拉单
        /// CrateDate:2013年12月12日20:33:42
        /// </summary>
        /// <param name="ParentCategoryID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWebPO_CategoryList(long ParentCategoryID)
        {

            HMGroupCreateServices gSvr = new HMGroupCreateServices();
            return Json(
                gSvr.GetWebPO_CategoryList(ParentCategoryID), JsonRequestBehavior.AllowGet
                );
        }
        /// <summary>
        /// 组合产品的基础信息添加，用于Create New Product Group 页面的第一阶段
        /// </summary>
        /// <param name="gpModel"></param>
        /// <returns>如果成功，返回新增成功的数据库ID</returns>
        public ActionResult HMGroupBaseInfoAdd(CMS_HMNUM_Model gpModel)
        {
            try
            {    
                User_Profile_Model curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                HMGroupCreateServices gSvr = new HMGroupCreateServices();
                string errMsg = string.Empty;
                long newID = gSvr.HMGroupBaseInfoAdd(gpModel, curUserInfo.User_Account, ref errMsg);
                if (newID > 0)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = newID
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = errMsg == string.Empty ? "Faile to add HM# Group" : errMsg
                });
            }
            catch (DbEntityValidationException e)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error("");
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        NBCMSLoggerManager.Error(String.Format(" Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = e.Message
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 为组合产品新增基础产品，用于Create HM Group 页面的使用
        /// CraeteTime:2013年11月19日11:37:55
        /// </summary>
        /// <param name="rModel"></param>
        /// <returns>如果新增成功，返回新的ID,和价格信息，价格信息用于第三阶段Costing的展示...</returns>
        public ActionResult AddNewHM4Group(CMS_HMGroup_Relation_Model rModel)
        {
            try
            {
                if (rModel.ProductID < 1 || rModel.ChildrenProductID < 1)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }
                
                User_Profile_Model curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                HMGroupCreateServices gpSvr = new HMGroupCreateServices();
                string errMsg = string.Empty;
                long newID = gpSvr.AddNewHM4Group(rModel, curUserInfo.User_Account);
                if (newID > 0)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = new
                        {
                            newID = newID,
                            //根据当前主产品的信息获取其子产品的价格，一个一个返回，客户端要判断到傻眼，删除新增编辑都要用JS判断...
                            //2013年11月19日17:41:54
                            ChildrenCostList = gpSvr.GetChildrenCost(rModel)
                        }
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = errMsg == string.Empty ? "Faile to add HM# Group" : errMsg
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }



        /// <summary>
        /// 根据关系表的ID删除当前组合产品的下的某【一个】子产品
        /// Change：删除也返回当前组合产品对于的各种子产品的价格，用于前端价格的动态展示。2013年11月20日15:08:42
        /// </summary>
        /// <param name="rModel"></param>
        /// <returns></returns>
        public ActionResult DeleteChildrenHM(CMS_HMGroup_Relation_Model rModel)
        {
            try
            {
                if (rModel.RID < 1)
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Request is illegal!"
                    });
                }
                HMGroupCreateServices gpSvr = new HMGroupCreateServices();
                string errMsg = string.Empty;
                if (gpSvr.DeleteChildrenHM(rModel, ref errMsg))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.OK,
                        Data = new
                        {
                            ChildrenCostList = gpSvr.GetChildrenCost(rModel)
                        }
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = errMsg == string.Empty ? "Faile to delete this item" : errMsg
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Exception,
                    Data = ex.Message
                });
            }
        }

        /// <summary>
        /// 用户添加组合产品的时候，选择HM# 自动实现AutoCompleted的功能
        /// 2013年11月19日10:37:42
        /// </summary>
        /// <param name="hmModel"></param>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public ActionResult GetProductInfo(CMS_HMNUM_Model hmModel)
        {
            try
            {
                HMGroupCreateServices gsvr = new HMGroupCreateServices();
                /*尝试在前端构建ExcludedProductIDs数组，但是非常复杂，既需要考虑新增，又需要考虑删除，同时还需要考虑更新，
                 将原先的id为1的改成ID为2的，非常复杂而且不利于后续维护，最好的办法只能在这里多做一次数据库查询，一劳永逸..
                 2014年3月18日*/
                hmModel.ExcludedProductIDs = gsvr.GetChildrenProductID(hmModel.ProductID);

                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = new ProductCommonServices().GetProductsByKeyWords(hmModel, 0)
                });

            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.Source);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.Error,
                    Data = ex.Message
                });
            }
        }


        /// <summary>
        /// 更新组合产品的价格，用于Create HM Group页面的价格更新。和非组合产品调用的是同一个方法。
        /// CreateDate:2013年11月19日17:27:11
        /// </summary>
        /// <param name="model"></param>
        /// <param name="costing"></param>
        /// <returns></returns>
        public ActionResult EditHMGroupCosting(CMS_HMNUM_Model model, CMS_HM_Costing_Model costing)
        {
            try
            {
                User_Profile_Model curUserInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);

                HmConfigServices hcSvr = new HmConfigServices();
                if (!hcSvr.EditHmnumCosting(model, ref costing, curUserInfo.User_Account))
                {
                    return Json(new NBCMSResultJson
                    {
                        Status = StatusType.Error,
                        Data = "Fail to udate current HM#'s costing"
                    });
                }
                return Json(new NBCMSResultJson
                {
                    Status = StatusType.OK,
                    Data = costing
                });
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
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
