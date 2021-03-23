using System.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using PermaisuriCMS.BLL;
using PermaisuriCMS.Common;
using PermaisuriCMS.Model;

namespace Permaisuri.Controllers
{
    /// <summary>
    /// 这个控制器用来保存一些基本的，反复被调用的方法。2014年2月18日10:32:52
    /// </summary>
    public class CommonController : Controller
    {

        /// <summary>
        /// 检查HMNUM是否存在，用户添加HMNUM,SKU和Product Configuration页面Autocompleted做校验。
        /// 由于前端使用了deffer模式，可以不用try catch记录异常了
        /// CreateDate:2014年2月18日10:41:08
        /// </summary>
        /// <param name="hmnum"></param>
        /// <returns>JSON格式，有效返回true,无效返回false.</returns>
        public ActionResult CheckHmnum(string hmnum)
        {
            var pcs = new ProductCommonServices();
            var hmModel = pcs.CheckHmnum(hmnum);

            return Json(new NBCMSResultJson
            {
                Status = hmModel == null ? StatusType.Error : StatusType.OK,
                Data = hmModel
            });
        }


        //public void SendToEcomByModel(CMS_SKU_Model SKUModel)
        //{
        //    string Send2eComPath = "";
        //    var imageName = "";
        //    var ImageStoragePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"]);
        //    if (SKUModel.pMedia != null)
        //    {
        //        imageName = SKUModel.pMedia.HMNUM + "\\" + SKUModel.pMedia.ImgName + SKUModel.pMedia.fileFormat;
        //        Send2eComPath = Path.Combine(ImageStoragePath, imageName);
        //    }

        //    SKUModel.Send2eComPath = Send2eComPath;
        //    new ICMSECOM.BLL.Insert2EComServices().Processing(SKUModel);
        //}

         
        /// <summary>
        /// 根据传递进来的Cookie的字符串解析成强类型返回。
        /// 不移动到BLL层的原因之一是 JavaScriptSerializer 是在Syetm.web这个命名空间下...
        /// CreateDate:2014年3月19日.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public User_Profile_Model GetCurrentUserbyCookie(string cookie)
        {
            var serializer = new JavaScriptSerializer();
            var decCookies = CryptTools.Decrypt(cookie);
            var useInfo = serializer.Deserialize(decCookies, typeof(User_Profile_Model)) as User_Profile_Model ??
                          new User_Profile_Model { 
                User_Account = "unknow"
            };
            return useInfo;
        }

        /// <summary>
        /// 由于获取Channel这个方法十分常用但是又和当前用户IsChannelControl关联，无法缓存，故而让着方法和Brand方法分散在不同的Controller维护，需要注意
        /// 2014年3月24日
        /// </summary>
        /// <param name="isNeedAll"></param>
        /// <returns></returns>
        public ActionResult GetChannelList(bool isNeedAll)
        {
            var useInfo = new CommonController().GetCurrentUserbyCookie(Request[ConfigurationManager.AppSettings["userInfoCookiesKey"]]);
            //ChannelID ChannelName
            var list = new ProductCommonServices().GetAllChannels(useInfo.IsChannelControl, useInfo.User_Guid);
            if (isNeedAll)
            {
                list.Insert(0, new Channel_Model//插入第一个位置
                {
                    ChannelID = 0,
                    ChannelName = "All"
                });
            }
            return Json(list);
        }
    }
}
