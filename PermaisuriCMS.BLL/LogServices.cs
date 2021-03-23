//*****************************************************************************************************************************************************************************************
//											Modification history
//*****************************************************************************************************************************************************************************************
// C/A/D Change No   Author     Date        Description 

//	A	WL-1		Lee		    25/10/2013	修改文件的查询方式，改用标准的EntityFramwork查询。新增一个Remark字段用来记录登录失败的原因。
//******************************************************************************************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
   public class LogServices
    {
       public List<LogOfUserLogin_Model> GetLoginInfoList(LogOfUserLogin_Model qModel, out int count)
       {
           var list = new List<LogOfUserLogin_Model>();
           using (var db = new PermaisuriCMSEntities())
           {
               var query = db.LogOfUserLogin.AsEnumerable();
               count = query.Count();
               query = query.OrderByDescending(l=>l.ID).Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows);
               list.AddRange(query.Select(log => new LogOfUserLogin_Model
               {
                   ID = log.ID, 
                   User_Account = log.User_Account, 
                   Display_Name = log.Display_Name, 
                   Logging_Date = log.Logging_Date, 
                   Logging_IP = log.Logging_IP, 
                   Logging_Location = log.Logging_Location, 
                   LoggingStatue = log.LoggingStatue, 
                   Machine_Name = log.Machine_Name, 
                   LoggingDate = log.Logging_Date.ToString("yyyy-MM-dd HH:mm:ss"), 
                   Remark = log.Remark
               }));
               return list;
           }
       }


       public List<LogOfUserOperating_Model> GetLogOfUserOperatingList(LogOfUserOperating_Model qModel, out int count)
       {
           var list = new List<LogOfUserOperating_Model>();
           using (var db = new PermaisuriCMSEntities())
           {
               var query = db.LogOfUserOperating.AsEnumerable();
               count = query.Count();
               query = query.OrderByDescending(l=>l.ID).Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows);
               list.AddRange(query.Select(log => new LogOfUserOperating_Model
               {
                   ID = log.ID, Action_Name = log.Action_Name, Display_Name = log.Display_Name, User_Account = log.User_Account, Model_Name = log.Model_Name, Operating_Date = log.Operating_Date, IP_Address = log.IP_Address, OperatingDate = log.Operating_Date.ToString("yyyy-MM-dd HH:mm:ss")
               }));
           }
           return list;
       }

       public List<LogOfUserOperatingDetails_Model> GetLogOfUserOperatingDetails(LogOfUserOperatingDetails_Model qModel, out int count)
       {
           var list = new List<LogOfUserOperatingDetails_Model>();
           using (var db = new PermaisuriCMSEntities())
           {
               qModel.To = qModel.To.AddDays(1);//这么解决 算爆了，客户端再也不用set来 set去了 2014年3月13日，不用担心性能，这里还没牵涉到数据库
               var query = db.LogOfUserOperatingDetails.Where(l=>l.CreateOn>qModel.From&&l.CreateOn<qModel.To);
               if (!String.IsNullOrEmpty(qModel.HMNUM))
               {
                   query = query.Where(l => l.HMNUM.StartsWith(qModel.HMNUM));
               }
               if (!String.IsNullOrEmpty(qModel.SKU))
               {
                   query = query.Where(l => l.HMNUM.StartsWith(qModel.SKU));
               }
               if (!String.IsNullOrEmpty(qModel.ModelName))
               {
                   query = query.Where(l => l.HMNUM.StartsWith(qModel.ModelName));
               }
               if (!String.IsNullOrEmpty(qModel.CreateBy))
               {
                   query = query.Where(l => l.HMNUM.StartsWith(qModel.CreateBy));
               }
               if (!String.IsNullOrEmpty(qModel.Descriptions))
               {
                   query = query.Where(l => l.HMNUM.StartsWith(qModel.Descriptions));
               }

               count = query.Count();
               query = query.OrderByDescending(l => l.ID).Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows);
               foreach (var log in query)
               {
                   var strSku = "";
                   if (!string.IsNullOrEmpty(log.SKU))
                   {
                       strSku = log.SKU + "<br>(" + log.ChannelName + ")";
                   }
                   list.Add(new LogOfUserOperatingDetails_Model
                   {
                       ID = log.ID,
                       ModelName = log.ModelName + "<br>(" + log.ActionName + ")",
                       SKU = strSku,
                       HMNUM = log.HMNUM ?? string.Empty,
                       Descriptions = log.Descriptions,
                       CreateBy = log.CreateBy + "<br>" + log.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                   });
               }
           }
           return list;
       }

     
    }
}
