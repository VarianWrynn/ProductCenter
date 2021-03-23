using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data.Entity.Core.Objects;

namespace PermaisuriCMS.BLL {
    public class UserInfoServices
    {
        /// <summary>
        /// 检验用户是否登录成功  Is loging user exist
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public User_Profile_Model checkUserLogin(User_Profile_Model userinfo)
        {
            //判断用户是否存在  Is this user exist
            using (var db = new PermaisuriCMSEntities())
            {
                //var UserInfo = contextDB.GetSingleUserProfiler(u => u == null ? null : new User_Profile_Model
                //{
                //    User_Guid = u.User_Guid,
                //    User_Account = u.User_Account,
                //    User_Pwd = u.User_Pwd,
                //    Display_Name = u.Display_Name,
                //    //Status = u.Status,
                //    Primary_Email = u.Primary_Email,
                //    Mobile_Phone = u.Mobile_Phone

                //}, userinfo.User_Account, userinfo.User_Pwd.GetPWDMD5());
                var md5Pwd = userinfo.User_Pwd.GetPwdmd5();
                //这里的变量千万别和传递进来的变量相同，否则会遇到意想不到的问题，比如userInfo.UserStatusID永远为0...2013年10月25日6:16:37
                //var UserInfo = db.CMS_User_Profile_V.Where(u => u.User_Account == userinfo.User_Account && u.UserPWD==md5PWD).FirstOrDefault();
                var ui =
                    db.CMS_User_Profile_V.FirstOrDefault(
                        u => u.User_Account == userinfo.User_Account && u.UserPWD == md5Pwd);
                if (ui == null)
                    return null;

                return new User_Profile_Model
                {
                    User_Guid = ui.User_Guid,
                    User_Account = ui.User_Account,
                    User_Pwd = ui.UserPWD,
                    Display_Name = ui.DisplayName,
                    Primary_Email = ui.Primary_Email,
                    // ui.UserStatusID.HasValue ? ui.UserStatusID.Value :0 无法转化！！！！！！！！！！2013-10-25 6:19:52
                    UserStatusID = ui.UserStatusID.HasValue ? ui.UserStatusID.Value : userinfo.UserStatusID,
                    UserStatusName = ui.UserStatusName,
                    //IsChannelControl = ui.IsChannelControl.HasValue ? ui.IsChannelControl.Value : false
                    IsChannelControl = ui.IsChannelControl.HasValue && ui.IsChannelControl.Value
                };
            }
        }

        /// <summary>
        /// Get Menu Resource by User Infomation
        /// Change:Add OrderBy Func ,2013年11月20日16:25:55
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public List<Menu_Resource_Model> GetMenuResourceByUserInfo(User_Profile_Model userinfo)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //var query = contextDB.User_Role_Menu_V.Where(v => v.User_Account == userinfo.User_Account && v.User_Pwd == userinfo.User_Pwd).OrderBy(v=>v.MenuID);
                /*2014年1月6日13:08:32 去掉密码校验，因为密码已经搬到WEBPO去了 简化查询*/
                var query = db.User_Role_Menu_V.Where(v => v.User_Account == userinfo.User_Account)
                    .OrderBy(v => v.SortNo);

                return query.Select(view => new Menu_Resource_Model
                {
                    //icon = View.Icon,
                    MenuID = view.MenuID,
                    MenuUrl = view.MenuUrl,
                    MR_ID = view.MR_ID,
                    name = view.MenuName,
                    SortNo = view.SortNo,
                    ParentMenuID = view.ParentMenuID,
                    iconSkin = view.Icon == null ? "" : view.Icon.Trim()
                }).ToList();
            }
        }

        /// <summary>
        /// Get User List
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sort">SortColumn,eg:Display_Name</param>
        /// <param name="order">asc or desc</param>
        /// <returns></returns>
        public List<CMS_User_Profile_V_Model> GetUserList(User_Profile_QueryModel queryModel, out int count)
        {
            var list = new List<CMS_User_Profile_V_Model>();
            using (var db = new PermaisuriCMSEntities())
            {

                var query = db.CMS_User_Profile_V.Include(u => u.User_Channel_Relation).AsQueryable();
                if (!String.IsNullOrEmpty(queryModel.User_Account))
                {
                    query = query.Where(u => u.User_Account.Contains(queryModel.User_Account));
                }
                if (!String.IsNullOrEmpty(queryModel.Display_Name))
                {
                    query = query.Where(u => u.DisplayName.Contains(queryModel.Display_Name));
                }
                if (queryModel.UserStatusID > 0)
                {
                    switch (queryModel.UserStatusID)
                    {
                        case 1: //CMS Enabled
                            query = query.Where(u => u.UserStatusID == 1);
                            break;
                        case 2: //CMS Disabled
                            query = query.Where(u => u.UserStatusID == 2);
                            break;
                        case 3: //WebPO Disabled
                            query = query.Where(u => u.IsDisabled == 1);
                            break;
                        case 4: //WebPO Enaled
                            query = query.Where(u => u.IsDisabled == 0 || u.IsDisabled == null);
                            break;
                        default:
                            break;
                    }
                }
                if (!String.IsNullOrEmpty(queryModel.Primary_Email))
                {
                    query = query.Where(u => u.Primary_Email.Contains(queryModel.Primary_Email));
                }

                count = query.Count();
                query =
                    query.OrderBy(u => u.User_Account).Skip((queryModel.page - 1)*queryModel.rows).Take(queryModel.rows);
                list.AddRange(query.Select(ui => new CMS_User_Profile_V_Model
                {
                    User_Guid = ui.User_Guid,
                    User_Account = ui.User_Account,
                    Display_Name = ui.DisplayName,
                    Last_Logon = ui.Last_Logon.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss"),
                    UserStatusID = ui.UserStatusID.GetValueOrDefault(),
                    UserStatusName = ui.UserStatusName,
                    UserPWD = ui.UserPWD,
                    Primary_Email = ui.Primary_Email,
                    IsChannelControl = ui.IsChannelControl.HasValue ? ui.IsChannelControl.Value : false
                        
                    /*2013年11月1日15:23:41 经测试，在relation表为null的情况下，会一次性关联relation返回top 10条记录，如果有数据还未做测试*/
                    //Channels = ui.User_Channel_Relation.Select(r => new Channel_Model
                    //{
                    //    ChannelID = r.Channel.ChannelID,
                    //    ChannelName = r.Channel.ChannelName
                    //}).ToList()
                    /*在查询用户整个列表的时候还是不要关联去查询渠道，查询当个用户的信息的时候才关联查询该用户的渠道信息，符合业务特征也更简洁！
                         * 2013年11月1日15:32:22
                         */
                }));
                return list;
            }
        }

        /// <summary>
        /// 通过GUID来获取当前用户对于的渠道信息，用于User Management 模块的 Channels Settings使用
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public List<Channel_Model> GetChannelByGuid(Guid userGuid)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query =
                    db.User_Channel_Relation.Include(r => r.Channel)
                        .Where(r => r.User_Guid == userGuid)
                        .Select(r => new Channel_Model
                        {
                            ChannelID = r.ChannelID,
                            ChannelName = r.Channel.ChannelName
                        });
                return query.ToList();
            }
        }


        /// <summary>
        /// Check User Account is existing or not
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns>if existing,return true, otherwise return false</returns>
        public bool IsExistUser(String userAccount)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.User_Profile.FirstOrDefault(o => o.User_Account == userAccount);
                //return query == null ? false : true;
                return query != null;
            }
        }

        /// <summary>
        /// Add New User
        /// </summary>
        /// <param name="model"></param>
        /// <param name="curUserAccount"></param>
        /// <returns></returns>
        public bool AddUser(User_Profile_Model model, String curUserAccount)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var newUser = new User_Profile
                {
                    User_Guid = Guid.NewGuid(),
                    Display_Name = model.Display_Name,
                    Created_By = curUserAccount,
                    Created_On = DateTime.Now,
                    //Status = model.Status,
                    Primary_Email = model.Primary_Email,
                    Mobile_Phone = model.Mobile_Phone,
                    User_Account = model.User_Account,
                    User_Pwd = model.User_Pwd
                };
                db.User_Profile.Add(newUser);
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Edit A User 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="curUserAccount"></param>
        /// <returns></returns>
        public bool EditUser(User_Profile_Model model, String curUserAccount)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var entity = db.User_Profile.FirstOrDefault(a => a.User_Guid == model.User_Guid);

                if (entity == null) return  false;
                entity.Display_Name = model.Display_Name;
                entity.Primary_Email = model.Primary_Email;
                //entity.Status = model.Status;
                entity.User_Pwd = model.User_Pwd;
                entity.Modified_On = DateTime.Now;
                entity.Modified_By = curUserAccount;

                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// Delete A User By User_Guid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteUser(User_Profile_Model model)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                //这样做的好处在于能直接删除一个对象，而不需要先从数据库中提取数据，创建实体对象，再查找并删除之，从而能有效地提升效率
                var user = new User_Profile {User_Guid = model.User_Guid};
                db.Set<User_Profile>().Attach(user);
                db.User_Profile.Remove(user);
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// Seems very nubility
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <returns></returns>
        public List<Security_Role_Model> GetAllRolesWithUser(Guid User_Guid)
        {
            var list = new List<Security_Role_Model>();
            using (var db = new PermaisuriCMSEntities())
            {
                var roles = db.Security_Role;
                list.AddRange(from role in roles
                    let urr = role.User_Role_Relation.FirstOrDefault(r => r.User_Guid == User_Guid)
                    select new Security_Role_Model
                    {
                        Role_GUID = role.Role_GUID,
                        Role_Name = role.Role_Name,
                        User_Checked = urr != null
                    });
            }
            return list;
        }

        /// <summary>
        ///  更新当前用户是否启用Channel关联，用于UserManagement-OpenChannel的操作
        /// Author:Lee,Date:2013年11月2日12:12:27
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <param name="IsChannelControl"></param>
        /// <returns></returns>
        public bool UpdateUserChannelControl(Guid User_Guid, bool IsChannelControl)
        {
            const string strSql =
                "update User_Profile set IsChannelControl =@IsChannelControl where User_Guid = @User_Guid";
            SqlParameter[] paramters =
            {
                new SqlParameter("@User_Guid", SqlDbType.UniqueIdentifier),
                new SqlParameter("@IsChannelControl", SqlDbType.Bit, 10)
            };
            paramters[0].Value = User_Guid;
            paramters[1].Value = IsChannelControl;
            using (var db = new PermaisuriCMSEntities())
            {
                return db.Database.ExecuteSqlCommand(strSql, paramters) > 0;

            }
        }

        /// <summary>
        /// 这种做法少一步查询罢了...但是将来改动库表结构什么要一个个去查，很痛苦
        /// 2013年10月25日16:13:05
        /// </summary>
        /// <param name="status"></param>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public bool UpdateUserStats(Int16 status, Guid userGuid)
        {
            const string strSql = "update User_Profile set UserStatusID =@Status where User_Guid = @User_Guid";
            SqlParameter[] paramters =
            {
                new SqlParameter("@Status", SqlDbType.SmallInt, 10),
                new SqlParameter("@User_Guid", SqlDbType.UniqueIdentifier)
            };
            paramters[0].Value = status;
            paramters[1].Value = userGuid;
            using (var db = new PermaisuriCMSEntities())
            {
                return db.Database.ExecuteSqlCommand(strSql, paramters) > 0;

            }
        }

        /// <summary>
        /// 更新用户的登录时间，CheckUserLogin成功过后执行
        /// </summary>
        /// <param name="User_Guid"></param>
        /// <returns></returns>
        public bool UpdateUserLast_Logon(Guid User_Guid)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var query = db.User_Profile.FirstOrDefault(u => u.User_Guid == User_Guid);
                if (query == null)
                {
                    NBCMSLoggerManager.Error(
                        String.Format("try to update user_profile last_logon faile,because {0} does not exist!",
                            User_Guid));
                    return false;
                }
                query.Last_Logon = DateTime.Now;
                return db.SaveChanges() > 0;
            }
        }


        /// <summary>
        /// 触发CMS和WebPO的账号进行数据同步。由于一开始CMS设置了自己的用户信息，并且使用GUID进行关联。后来要求用户统一在WEBPO进行设置
        /// 而WebPO采用自增长而非GUID的方式记录数据。因此需要采取一种同步机制而不是简单的View视图关联....
        /// </summary>
        /// <param name="userAccount">当前执行该同步的用户</param>
        /// <param name="affectedRows"></param>
        /// <returns>大于0：成功； 等于0：没有数据需要同步；小于0：同步失败</returns>
        public int UserSynchWithWebPO(string userAccount, out int affectedRows)
        {
            using (var db = new PermaisuriCMSEntities())
            {

                var affectedCount = new ObjectParameter("affectedRows", DbType.Int32);
                int retVal = db.SynchData_UsersFromWEBPO_SP(userAccount, affectedCount);
                affectedRows = Convert.ToInt32(affectedCount.Value);
                return retVal;
            }
        }


        /// <summary>
        /// 获取用户状态
        /// </summary>
        /// <returns></returns>
        public List<Object> GetAllUserStatus()
        {
            var list = new List<Object>();
            using (var db = new PermaisuriCMSEntities())
            {
                foreach (var us in db.User_Status)
                {
                    list.Add(new
                    {
                        UserStaustID = us.UserStatusID,
                        UserStatusName = us.UserStatusName
                    });
                }
                return list;
            }
        }


        /// <summary>
        /// 更新当前用户对于的Channel信息，用户User Management模块的Channel Setting
        /// Author:Lee Date:2013年11月1日18:06:15
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="ArrChannels"></param>
        /// <returns></returns>
        public bool UpdateUserChannel(Guid userGuid, IEnumerable<int> ArrChannels)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                using (var transaction = new TransactionScope())
                {
                    db.Database.ExecuteSqlCommand("delete from User_Channel_Relation where User_Guid = @User_Guid",
                        new SqlParameter("@User_Guid", userGuid));

                    //User_Channel_Relation relation = new User_Channel_Relation { User_Guid = User_Guid };
                    //db.Set<User_Channel_Relation>().Attach(relation);
                    //db.User_Channel_Relation.Remove(relation);这种方法没有数据的时候会报错
                    if (ArrChannels != null)
                    {
                        foreach (var mediaId in ArrChannels)
                        {
                            db.User_Channel_Relation.Add(new User_Channel_Relation
                            {
                                User_Guid = userGuid,
                                ChannelID = mediaId
                            });
                        }
                    }
                    var retInt = db.SaveChanges();
                    transaction.Complete(); // don't be miss,or the SQL statement will never be executed
                    return retInt > 0;
                }
            }
        }


        public IEnumerable<User_Profile_Model> GetUserListWithLinq(User_Profile_Model userinfo)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var userInfo = db.LoadWithUserProfilerWithLinq(u => u == null
                    ? null
                    : new User_Profile_Model
                    {
                        User_Guid = u.User_Guid,
                        User_Account = u.User_Account,
                        User_Pwd = u.User_Pwd,
                        Display_Name = u.Display_Name,
                        Primary_Email = u.Primary_Email,
                        Mobile_Phone = u.Mobile_Phone

                    }, userinfo.User_Account, userinfo.User_Pwd.GetPwdmd5());
                return userInfo;
            }
        }

        public IEnumerable<User_Profile_Model> GetUserListWithFunc(User_Profile_Model userinfo)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                var userInfo = db.LoadWithUserProfilerWithFunc(u => u == null
                    ? null
                    : new User_Profile_Model
                    {
                        User_Guid = u.User_Guid,
                        User_Account = u.User_Account,
                        User_Pwd = u.User_Pwd,
                        Display_Name = u.Display_Name,
                        Primary_Email = u.Primary_Email,
                        Mobile_Phone = u.Mobile_Phone

                    }, userinfo.User_Account, userinfo.User_Pwd.GetPwdmd5());
                return userInfo;
            }
        }
    }
}
