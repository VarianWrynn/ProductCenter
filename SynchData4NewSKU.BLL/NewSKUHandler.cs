using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchData4NewSKU.DAL;
using NLog;
using System.Data.Entity.Infrastructure;
using System.Configuration;

namespace SynchData4NewSKU.BLL
{
   public  class NewSKUHandler
    {
       private static Logger logger = LogManager.GetCurrentClassLogger(); 
       public int DoSynchDataBySP()
       {

           using (PermaisuriCMSNewSKUEntities db = new PermaisuriCMSNewSKUEntities())
           {
               try
               {
                   var timeOut = ConfigurationManager.AppSettings["HMSynchTimeOut"];
                   var adapter = (IObjectContextAdapter)db;
                   var objectContext = adapter.ObjectContext;
                   int duration =3;//默认三分钟
                   if(!Int32.TryParse(timeOut, out duration)) {
                       duration = 3;
                   };
                   objectContext.CommandTimeout = duration * 60; // value in seconds

                   var retVal = db.SynchData_NewSKUData_SP();

                   return retVal;
               }
               catch (Exception ex)
               {
                   logger.Error(ex.Message);
                   logger.Error("InnerException:"+ex.InnerException);
                   logger.Error(ex.StackTrace);
                   return -10;
               }
           }
       }
    }
}
