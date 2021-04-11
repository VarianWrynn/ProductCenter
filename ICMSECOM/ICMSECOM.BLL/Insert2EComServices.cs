using System;
using System.Collections.Generic;
using System.Globalization;
using PermaisuriCMS.Model;
using ICMSECOM.DAL;
using System.Data.Entity.Validation;
using PermaisuriCMS.Common;

namespace ICMSECOM.BLL
{
    public class Insert2EComServices
    {
        /// <summary>
        /// Change1:既然文件类型的LOG已经记录了StackTrace,那么 数据库只要记录一个Message就足够了，不要再记录StackTrace了。2014年5月14日11:24:52
        /// </summary>
        /// <param name="model"></param>
        public void Processing(CMS_SKU_Model model)
        {
            HMNUMServices hmSvr = new HMNUMServices();
            SKUServices skuSvr = new SKUServices();

            IDictionary<string, string> ls = new Dictionary<string, string>();
            using (EcomEntities db = new EcomEntities())
            {
                try
                {
                    int UnitQTY = 0;
                    hmSvr.HMNUMGroup_Action(model, db, ref UnitQTY);
                    hmSvr.HMNUM_Action(model.SKU_HM_Relation.CMS_HMNUM, db, UnitQTY, model.Send2eComPath, 
                        decimal.Parse(model.SKU_Costing.EstimateFreight, NumberStyles.Currency, new CultureInfo("en-US")));
                    hmSvr.Carton_Action(model.SKU_HM_Relation.CMS_HMNUM, db);
                    skuSvr.SKU_Action(model, db);
                    skuSvr.SKUURL_Action(model, db);
                    db.SaveChanges();
                    UpdateStatus(model, "", 1, "Synchronized");
                    LogInsertEcom(model, "");
                }
                catch (DbEntityValidationException ex)
                {
                    var exMsg = String.Empty;

                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        NBCMSLoggerManager.Error("");
                        NBCMSLoggerManager.Error("");
                        //HMLog.Error("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            string temp = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            NBCMSLoggerManager.Error(temp);
                            exMsg += temp;
                        }
                        NBCMSLoggerManager.Error("");
                    }
                    exMsg += ex.Message + ":" + ex.StackTrace;
                    UpdateStatus(model, ex.Message, 2, "Failed");
                    LogInsertEcom(model, ex.Message);
                }
                catch (Exception ex)
                {
                    var exMsg = String.Empty;
                    bool isInnerEx = false;
                    if (ex.InnerException != null)
                    {

                        if (ex.InnerException.InnerException != null)
                        {
                            NBCMSLoggerManager.Error("InnerException.InnerException");
                            NBCMSLoggerManager.Error(ex.InnerException.InnerException.Message);
                            NBCMSLoggerManager.Error(ex.InnerException.InnerException.Source);
                            NBCMSLoggerManager.Error(ex.InnerException.InnerException.StackTrace);
                            NBCMSLoggerManager.Error("");
                            exMsg = ex.InnerException.InnerException.Message;// +":" + ex.InnerException.InnerException.StackTrace;
                            isInnerEx = true;
                        }
                        if (!isInnerEx)
                        {
                            NBCMSLoggerManager.Error(ex.InnerException.Message);
                            NBCMSLoggerManager.Error(ex.InnerException.Source);
                            NBCMSLoggerManager.Error(ex.InnerException.StackTrace);
                            NBCMSLoggerManager.Error("");
                            exMsg = ex.InnerException.Message;// +":" + ex.InnerException.StackTrace;
                        }
                    }
                    if (!isInnerEx)
                    {
                        NBCMSLoggerManager.Error("");
                        NBCMSLoggerManager.Error(ex.Message);
                        NBCMSLoggerManager.Error(ex.StackTrace);
                        NBCMSLoggerManager.Error("");
                        exMsg = ex.Message;//+ ":" + ex.StackTrace;
                    }
                    UpdateStatus(model, exMsg, 2, "Failed");
                    LogInsertEcom(model, exMsg);
                }
            }
        }

        public void LogInsertEcom(CMS_SKU_Model model, string exMsg)
        {
            IDictionary<string, string> ls = new Dictionary<string, string>();
            ls.Add("SKUID", model.SKUID.ToString());
            ls.Add("ProductID", model.SKU_HM_Relation.CMS_HMNUM.ProductID.ToString());
            ls.Add("HMNUM", model.SKU_HM_Relation.CMS_HMNUM.HMNUM);
            ls.Add("SKUOrder", model.SKU);
            ls.Add("Channel", model.ChannelName);
            ls.Add("Status", "0");
            ls.Add("CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ls.Add("CreatedBy", model.Modifier);
            ls.Add("ModifiedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ls.Add("ModifiedBy", model.Modifier);
            ls.Add("Comments", exMsg == "" ? "successfully" : exMsg);
            NBCMSLoggerManager.NBCMSLogger("CMS_Ecom_Sync_Result", "", ls);
        }

        public void UpdateStatus(CMS_SKU_Model model, string exMsg, int StatusID, string StatusDesc)
        {
            IDictionary<string, string> ls = new Dictionary<string, string>();
            ls.Add("SKUID", model.SKUID.ToString());
            ls.Add("StatusID", StatusID.ToString());
            ls.Add("StatusDesc", StatusDesc);
            ls.Add("UpdateOn", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ls.Add("UpdateBy", model.Modifier);
            ls.Add("Comments", exMsg == "" ? "successfully" : exMsg);
            NBCMSLoggerManager.NBCMSLogger("CMS_Ecom_Sync", "", ls);
        }

    }
}
