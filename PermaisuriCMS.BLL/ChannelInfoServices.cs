using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Model;
using PermaisuriCMS.DAL;

namespace PermaisuriCMS.BLL
{
    public class ChannelInfoServices
    {
        public List<Channel_Model> GetChannelList(Channel_Model queryModel, out int count)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.Channel.AsQueryable();
                if (!string.IsNullOrEmpty(queryModel.ChannelName))
                {
                    //这种查询方式无效，即使是AsQueryable()。。。。2013年10月30日14:53:25 Lee
                    //query.Where(q => q.ChannelName.Contains(queryModel.ChannelName));
                    query = query.Where(q => q.ChannelName.Contains(queryModel.ChannelName));
                }
                if (!string.IsNullOrEmpty(queryModel.ShortName))
                {
                    query = query.Where(q => q.ShortName.Contains(queryModel.ShortName));
                }
                if (queryModel.queryAPI != 2)
                {
                    query = query.Where(q => q.API == (queryModel.queryAPI == 0 ? false : true));
                }
                if (queryModel.queryExport2CSV != 2)
                {
                    query = query.Where(q => q.API == (queryModel.queryExport2CSV == 0 ? false : true));
                }

                count = query.Count();
                query = query.OrderByDescending(q=>q.ChannelID).Skip((queryModel.page - 1) * queryModel.rows).Take(queryModel.rows);
                return query.Select(q => new Channel_Model
                {
                    ChannelID = q.ChannelID, 
                    ChannelName = q.ChannelName, 
                    ShortName = q.ShortName, 
                    API = q.API, 
                    Export2CSV = q.Export2CSV, 
                    Modifier = q.Modifier, 
                    Modify_Date = q.Modify_Date, 
                    strModify_Date = q.Modify_Date.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();
            }
        }

        public bool AddChannel(Channel_Model model, string user, ref string msg)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var isExist = db.Channel.FirstOrDefault(c => c.ChannelName == model.ChannelName);
                msg = string.Empty;
                if (isExist != null)
                {
                    msg = "This item does exist";
                    return false;
                }

                var newChannel = new Channel
                {
                    ChannelName = model.ChannelName,
                    ShortName = model.ShortName,
                    Modifier = user,
                    Modify_Date = DateTime.Now,
                    API = model.API,
                    Export2CSV = model.Export2CSV
                };
                db.Channel.Add(newChannel);
                return db.SaveChanges() > 0;
            }
        }

        public bool DeleteChannel(int ChannelID, ref string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                msg = string.Empty;
                var channel = db.Channel.FirstOrDefault(c => c.ChannelID == ChannelID);
                if (channel == null)
                {
                    msg = "this item does exist";
                    return false;
                }

                if (channel.CMS_SKU.Count > 0)
                {
                    msg = "this item can not be deleted, because it is associated with SKUOrder!";
                    return false;
                }

                db.Channel.Remove(channel);

                return db.SaveChanges() > 0;
            }
        }

        public bool EditChannel(Channel_Model model, string user, ref string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                var isExist = db.Channel.FirstOrDefault(c => c.ChannelName == model.ChannelName);
                msg = string.Empty;
                if (isExist == null)
                {
                    msg = "This item does not exist";
                    return false;
                }

                Channel entity = db.Channel.Where(a => a.ChannelID == model.ChannelID).FirstOrDefault();
                entity.API = model.API;
                entity.ChannelName = model.ChannelName;
                entity.ShortName = model.ShortName;
                entity.Export2CSV = model.Export2CSV;
                entity.Modify_Date = DateTime.Now;
                entity.Modifier = user;
                return db.SaveChanges() > 0;
            }
        }
    }
}
