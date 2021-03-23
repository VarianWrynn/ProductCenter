//*****************************************************************************************************************************************************************************************
//											Modification history
//*****************************************************************************************************************************************************************************************
// C/A/D Change No   Author     Date        Description 

//	C	WL-1		Lee		    25/12/2013	Created this page
//******************************************************************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;

namespace PermaisuriCMS.BLL
{
    public class HMCommonServices
    {
        /// <summary>
        /// 2013年12月25日10:55:28
        /// Change1:将原来自动删除该图像关联的SKU信息，改成查询判断如果存在关联信息，则不自动删除，返回报错信息给前端 。 2014年5月15日11:29:15
        /// </summary>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public bool DeleteCMSMedia(long MediaID, out string errMsg)
        {
            errMsg = string.Empty;
            using (var db = new PermaisuriCMSEntities())
            {
                ////这样做的好处在于能直接删除一个对象，而不需要先从数据库中提取数据，创建实体对象，再查找并删除之，从而能有效地提升效率
                //MediaLibrary media = new MediaLibrary { MediaID = MediaID };
                //db.Set<MediaLibrary>().Attach(media);
                //db.MediaLibrary.Remove(media);
                //return db.SaveChanges() > 0;

                //var query = db.MediaLibrary.Include(s=>s.SKU_Media_Relation).FirstOrDefault(s => s.MediaID == MediaID);
                //db.MediaLibrary.Remove(query);
                //return db.SaveChanges() > 0;

                var query = db.MediaLibrary.FirstOrDefault(s => s.MediaID == MediaID);
                if (query != null && query.SKU_Media_Relation.Count > 0)
                {
                    errMsg = "This item can no be delete, because some skus have been associated with it.";
                    return false;
                }
                db.MediaLibrary.Remove(query);
                /*以下方法是用于删除图像的时候，自动删除所关联的信息，如果不删除，将会引发打开数据异常！*/
                //db.Entry(query).Collection(m => m.SKU_Media_Relation).Load();
                //db.MediaLibrary.Remove(query);
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 获取、组装WePO eCom 图像信息（根据HM#来查询）
        /// CreateDate:2013年12月25日14:46:09
        /// Change1:新增对于没有图像的情况下的处理，客户端脚本报错。
        /// </summary>
        /// <param name="HMNNUM"></param>
        /// <returns></returns>
        public List<OtherSystemImages> GetImagesFromOtherSystem(String HMNNUM)
        {
            String eComUrl = ConfigurationManager.AppSettings["EcomProductImageUrl"];
            String webPoUrl = ConfigurationManager.AppSettings["WebPOProductImageUrl"];
            String webpoRelStr = "../../../";//替换掉webPO数据库提取出来的路径前缀
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //var wpImgs = db.WebPO_ImageUrls_V.Where(v => v.HMNUM == HMNNUM && !String.IsNullOrEmpty(v.SmallPic) && !String.IsNullOrEmpty(v.Pic))
                //.Select(x => new OtherSystemImages
                //{
                //    SmallPic = webPoUrl + x.SmallPic.Replace(webpoRelStr, ""),
                //    Pic = webPoUrl + x.Pic.Replace(webpoRelStr, ""),
                //    SystemName = "WebPO System"
                //}).ToList();

                /*   
                http://192.168.0.6/pictures/HONGJIANG/51482.00CHRALU.jpg 正面
                http://192.168.0.6/pictures/HONGJIANG/51482.00CHRALU-1.jpg//侧面
                http://192.168.0.6/pictures/HONGJIANG/51482.00CHRALU-2.jpg //背面
                 * 
                 * http://192.168.0.6/pictures/HONGJIANG/51482.00CHRALU-1-Small.jpg 略缩
                 */

                var wpImgList = new  List<OtherSystemImages>();
 
                var wpImg = db.WebPO_ImageUrls_V.Where(v => v.HMNUM == HMNNUM).FirstOrDefault();
                if (wpImg != null)
                {
                    var oldName = wpImg.Pic.Substring(0, wpImg.Pic.LastIndexOf("."));
                    var LName = oldName + "-1";//做面
                    var RName = oldName + "-2";//背面

                    wpImgList.Add(new OtherSystemImages //正面
                    {
                        SmallPic = webPoUrl + wpImg.SmallPic.Replace(webpoRelStr, ""),
                        Pic = webPoUrl + wpImg.Pic.Replace(webpoRelStr, ""),
                        SystemName = "WebPO System"
                    });

                    wpImgList.Add(new OtherSystemImages //左面
                    {
                        SmallPic = webPoUrl + wpImg.Pic.Replace(oldName, LName + "-Small").Replace(webpoRelStr, ""),
                        Pic = webPoUrl + wpImg.Pic.Replace(oldName, LName).Replace(webpoRelStr, ""),
                        SystemName = "WebPO System"
                    });

                    wpImgList.Add(new OtherSystemImages //右面
                    {
                        SmallPic = webPoUrl + wpImg.Pic.Replace(oldName, RName + "-Small").Replace(webpoRelStr, ""),
                        Pic = webPoUrl + wpImg.Pic.Replace(oldName, RName).Replace(webpoRelStr, ""),
                        SystemName = "WebPO System"
                    });
                }


                var eComImgs = db.Ecom_ImageUrls_V.Where(v => v.HMNUM == HMNNUM && !String.IsNullOrEmpty(v.ProductPicture))
                    .Select(v => new OtherSystemImages
                {
                    SmallPic = eComUrl + v.ProductPicture,
                    Pic = eComUrl + v.ProductPicture.Replace("_80.", "_320."),
                    SystemName = "eCom System"
                }).ToList();

                wpImgList.AddRange(eComImgs);//将 ICollection 的元素添加到 ArrayList 的末尾。


                if (wpImgList.Count == 0)
                {

                    wpImgList.Add(new OtherSystemImages //正面
                    {
                        SmallPic = "../Content/images/NoPic.jpg",
                        Pic = "../Content/images/NoPic.jpg",
                        SystemName = "CMS default picture"
                    });
                }
                return wpImgList;
            }
        }
    }
}
