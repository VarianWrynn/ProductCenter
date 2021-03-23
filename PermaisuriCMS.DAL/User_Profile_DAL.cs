using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.DAL
{
    public partial class PermaisuriCMSEntities : System.Data.Entity.DbContext
    {
        ///// <summary>
        ///// 根据用户名、密码获取单个用户实例
        ///// </summary>
        ///// <typeparam name="T">泛型</typeparam>
        ///// <param name="t"></param>
        ///// <param name="user_Account">用户名</param>
        ///// <param name="user_Pwd">密码</param>
        ///// <returns></returns>
        //public T GetSingleUserProfiler<T>(Func<User_Profile, T> t, String user_Account, String user_Pwd)
        //{
        //    var query = this.User_Profile.Where(o => o.User_Account == user_Account && o.User_Pwd == user_Pwd).FirstOrDefault();
        //    return t(query);
        //}


        public IEnumerable<T> LoadWithUserProfilerWithLinq<T>(Func<User_Profile, T> t, String user_Account, String user_Pwd)
        {
            //DataLoadOptions dlo = new DataLoadOptions();
            //dlo.LoadWith<Schedule_Managment>(s => s.Schedule_UserInfo);
            var query = from p in this.User_Profile
                        select t(p);//t(p): t是 Func<User_Profile, T> 这个委托（类）的实例，所以需要传递一个类型为User_Profile的实例p进去，
            //再返回一个类型为T的实例回来-->也就是外面调用该方法的时候实例化 new User_Profile_Model{...}的那个对象返回去
            return query.ToList();
        }


        public IEnumerable<T> LoadWithUserProfilerWithFunc<T>(Func<User_Profile, T> t, String userAccount, String userPwd)
        {
            var query = this.User_Profile.Where(r => r.User_Account != userAccount)
                .Select(r => t(r));
            return query;
        }

        /*这种方法是让EF直接调用SQL语句执行并且传递参数*/
        //public int UpdateSchedule_UserInfo(Schedule_Managment instance)
        //{
        //    object[] parameters = new object[13];
        //    parameters[0] = instance.ID;
        //    parameters[1] = instance.Title;
        //    parameters[2] = instance.Contents;
        //    parameters[3] = instance.StartDate;
        //    parameters[4] = instance.EndDate;
        //    parameters[5] = instance.IsRemind;
        //    parameters[6] = instance.RemindType;
        //    parameters[7] = instance.RemindTime;
        //    parameters[8] = instance.IsTask;
        //    parameters[9] = instance.TaskID;
        //    parameters[10] = instance.AllDay;
        //    parameters[11] = instance.Priority;
        //    parameters[12] = instance.ModifyTime;

        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("UPDATE [Schedule_Managment] SET [Title]= {1},");
        //    sb.Append("[Contents]= {2},");
        //    sb.Append("[StartDate]= {3},");
        //    sb.Append("[EndDate]= {4},");
        //    sb.Append("[IsRemind]= {5},");
        //    sb.Append("[RemindType]= {6},");
        //    sb.Append("[RemindTime]= {7},");
        //    sb.Append("[IsTask]= {8},");
        //    sb.Append("[TaskID]= {9},");
        //    sb.Append("[AllDay]= {10},");
        //    sb.Append("[Priority]= {11},");
        //    sb.Append("[ModifyTime]= {12} ");
        //    sb.Append("where ID = {0} ");
        //    return ExecuteCommand(sb.ToString(), parameters);
        //    return ExecuteCommand("delete from Schedule_Managment where ID = {0}", id);
        //}


        /// <summary>
        /// 通过集团编码、用户名获取某一个用户信息(不需要密码）
        /// </summary>
        /// <typeparam name="T">UserAuthResultUserInfo实体类</typeparam>
        /// <param name="t"></param>
        /// <param name="ecCode"></param>
        /// <param name="userName"></param>
        /// <returns>返回一个用户信息</returns>
        //public T LoadWithECUserInfo<T>(Func<UAECUserBasicInfo, T> t, string ecCode, string userName)
        //{

        //    return (from p in this.UAECUserBasicInfo
        //            join userExt in this.UAECUserExtInfo on p.ECUserID equals userExt.ECUserID into extInfo
        //            from ext in extInfo.DefaultIfEmpty()
        //            join alias in this.ECUserAlias on p.ECUserID equals alias.UserID into ua
        //            from userAlias in ua.DefaultIfEmpty()
        //            join r in this.UARoleInfo on p.RoleID equals r.RoleID into roleInfo
        //            from role in roleInfo.DefaultIfEmpty()
        //            where p.ECCode == ecCode && p.UserName == userName
        //            select t(p)
        //                   ).FirstOrDefault();
        //}




    }                                                                                       
}
