using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Data.Entity;
using EntityFramework.Extensions;

namespace PermaisuriCMS.BLL
{
    public class MediaServices
    {
        public List<MediaLibrary_Model> GetChannelMediaList(long SKUID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                List<MediaLibrary_Model> list = new List<MediaLibrary_Model>();
                var query = db.MediaLibrary.Where(c => c.SKU_Media_Relation.Any(r => r.SKUID == SKUID)).OrderByDescending(m=>m.MediaID);
                foreach (MediaLibrary c in query)
                {
                    list.Add(new MediaLibrary_Model
                    {
                        fileFormat = c.fileFormat,
                        fileHeight = c.fileHeight.HasValue ? c.fileHeight.Value : 0,
                        fileWidth = c.fileWidth.HasValue ? c.fileWidth.Value : 0,
                        fileSize = c.fileSize,
                        MediaID = c.MediaID,
                        Description = c.Description,
                        HMNUM = c.HMNUM,
                        ImgName = c.ImgName,
                        ProductID = c.ProductID,
                        MediaType = c.MediaType,
                        SerialNum = c.SerialNum
                    });
                }
                return list;
            }
        }

        /// <summary>
        /// 查询出所有图像，不需要分组，该方法提供给前端同步云端模块用 2014年4月14日17:45:13
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<MediaLibrary_Model> GetMediaList(MediaLibrary_QueryModel queryModel, out int count)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.MediaLibrary.Include(m => m.SKU_Media_Relation).Where(d => d.CloudStatusID != 4);

                if (!String.IsNullOrEmpty(queryModel.HMNUM))
                {
                    query = query.Where(m => m.HMNUM.Contains(queryModel.HMNUM));
                }
                if (!string.IsNullOrEmpty(queryModel.SKUOrder))
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.SKU.Contains(queryModel.SKUOrder)));
                }
                if (queryModel.Format > 0)
                {

                    query = query.Where(m => m.MediaType == queryModel.Format);
                }
                if (queryModel.Status > 0)
                {
                    if (queryModel.Status == 1)//Attached to SKU
                    {
                        query = query.Where(m => m.SKU_Media_Relation.Any(r => r.MediaID > 0));
                    }

                    if (queryModel.Status == 2)//unattach
                    {
                        query = query.Except(query.Where(m => m.SKU_Media_Relation.Any(r => r.MediaID > 0)));
                    }
                }

                if (queryModel.Channel > 0)
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.ChannelID == queryModel.Channel));
                }
                if (queryModel.Brand > 0)
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.BrandID == queryModel.Brand));
                }
                if (queryModel.IsExcludeSKU)
                {
                    query = query.Except(query.Where(m => m.SKU_Media_Relation.Any(r => r.SKUID == queryModel.SKUID)));
                }
                if (queryModel.ProductID > 0)
                {
                    query = query.Where(m => m.CMS_StockKey.CMS_HMNUM.Any(r => r.ProductID == queryModel.ProductID));
                }
                if (queryModel.CloudStatusID > 0)
                {
                    query = query.Where(m => m.CloudStatusID == queryModel.CloudStatusID);
                }
                count = query.Count();

                return query.OrderByDescending(m => m.MediaID)
                    .Skip((queryModel.page - 1) * queryModel.rows)
                    .Take(queryModel.rows).ToList().Select(c => new MediaLibrary_Model
                {
                    fileFormat = c.fileFormat,
                    fileHeight = c.fileHeight.HasValue ? c.fileHeight.Value : 0,
                    fileWidth = c.fileWidth.HasValue ? c.fileWidth.Value : 0,
                    fileSize = c.fileSize,
                    MediaID = c.MediaID,
                    Description = c.Description,
                    HMNUM = c.HMNUM,
                    ImgName = c.ImgName,
                    ProductID = c.ProductID,
                    MediaType = c.MediaType,
                    SerialNum = c.SerialNum,
                    strCreateOn = c.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                    CloudStatusID = c.CloudStatusID,
                    MediaCloudStatus = new MediaCloudStatus_Model{
                        CloudStatusId = c.MediaCloudStatus.CloudStatusId,
                        CloudStatusName = c.MediaCloudStatus.CloudStatusName
                    },
                    CMS_SKU = c.SKU_Media_Relation.Where(r => r.MediaID == c.MediaID)
                    .Select(s => new CMS_SKU_Model
                    {
                        ChannelName = s.CMS_SKU.Channel.ChannelName,
                        ProductName = s.CMS_SKU.ProductName,
                        SKU = s.CMS_SKU.SKU
                    }).ToList()
                }).ToList();
            }

        }
    

        /// <summary>
        /// 带有分组信息的图像列表。2014年4月25日10:29:26
        /// 由于发现在MediaLibrary页面返回去的分组信息带到800组，转化成JSON造成网路负担和前后台解析缓慢，所以就吧需要分组和不需要分组的反复分离开来。
        /// 实际上MediaLibrary不需要分组
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="count"></param>
        /// <param name="groupByList"></param>
        /// <returns></returns>
        public List<MediaLibrary_Model> GetMediaLibraryListWithGroup(MediaLibrary_QueryModel queryModel, out int count, out List<MediaGroupBy_Model> groupByList)
        {

            var query = this.GetMediaList(queryModel, out count);

            //Load from memory.经过测试 ,如果不先toList()，还是会去数据库查询3次！之所以要AsEnumerable()是因为必须调用到Take()这个方法
            //但是这种预先全部preloading到memoery的方法，可能不适合MedialLibrary页面，for the performance考虑，可能要抽离出2个方法 2014年1月22日10:25:02
            var newQuery = query.AsEnumerable();
            count = newQuery.Count();//第一次触发数据库查询（没有toList()情况下)

            groupByList = newQuery.GroupBy(gp => new //Group By in Memory
            {
                fileWidth = gp.fileWidth,
                fileHeight = gp.fileHeight
            }).OrderBy(g => g.Key.fileWidth).Select(g => new MediaGroupBy_Model
            {
                fileWidth = g.Key.fileWidth,
                fileHeight = g.Key.fileHeight
            }).ToList();//第二次触发查询（没有toList()情况下)

            return newQuery.ToList();
        }

        /// <summary>
        /// Change:新增按照图像分辨率进行分组的信息，以适应新的需求 2014年1月6日10:55:16
        /// Change2:应David要求，去掉时间的筛选
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<MediaLibrary_Model> GetMediaLibraryList(MediaLibrary_QueryModel queryModel, out int count)
        {
            List<MediaLibrary_Model> list = new List<MediaLibrary_Model>();

            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.MediaLibrary.Include(m => m.SKU_Media_Relation).Where(d => d.MediaID > 0);

                if (!String.IsNullOrEmpty(queryModel.HMNUM))
                {
                    query = query.Where(m => m.HMNUM.Contains(queryModel.HMNUM));
                }
                if (!string.IsNullOrEmpty(queryModel.SKUOrder))
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.SKU.Contains(queryModel.SKUOrder)));
                }
                if (queryModel.Format > 0)
                {
                  
                    query = query.Where(m => m.MediaType == queryModel.Format);
                }
                if (queryModel.Status > 0)
                {
                    
                    if (queryModel.Status == 1)//Attached to SKU
                    {
                        query = query.Where(m => m.SKU_Media_Relation.Any(r => r.MediaID > 0));
                    }

                    if (queryModel.Status == 2)//unattach
                    {
                        query = query.Except(query.Where(m => m.SKU_Media_Relation.Any(r => r.MediaID > 0)));
                    }
                }

                if (queryModel.Channel > 0)
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.ChannelID == queryModel.Channel));
                }
                if (queryModel.Brand > 0)
                {
                    query = query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.BrandID == queryModel.Brand));
                }
                if (queryModel.IsExcludeSKU)
                {
                    //ChangeDate:2013年12月6日16:16:02 原来是根据SKUOrder来排除，但是这样子在批量复制（DupliatedProduct)情况下，一个
                    //渠道的SKUOrder选择了图片之后，会影响到其他渠道的SKUOrder的图片关联，故而应该用SKUID来关联排除。
                    //query = query.Except(query.Where(m => m.SKU_Media_Relation.Any(r => r.CMS_SKU.SKU == queryModel.SKUOrder)));
                    query = query.Except(query.Where(m => m.SKU_Media_Relation.Any(r => r.SKUID == queryModel.SKUID)));
                }
                if (queryModel.ProductID > 0)
                {
                    //query = query.Where(m => m.ProductID == queryModel.ProductID); changed 2014年4月11日10:40:02
                    query = query.Where(m => m.CMS_StockKey.CMS_HMNUM.Any(r => r.ProductID == queryModel.ProductID));
                }
                if (queryModel.CloudStatusID > 0)
                {
                    query = query.Where(m => m.CloudStatusID == queryModel.CloudStatusID);
                }

                count = query.Count();//第一次触发数据库查询（没有toList()情况下)

                query = query.OrderByDescending(m => m.MediaID).Skip((queryModel.page - 1) * queryModel.rows).Take(queryModel.rows);

                foreach (MediaLibrary c in query)////第三次触发查询（没有toList()情况下)
                {
                    list.Add(new MediaLibrary_Model
                    {
                        fileFormat = c.fileFormat,
                        fileHeight = c.fileHeight.HasValue ? c.fileHeight.Value : 0,
                        fileWidth = c.fileWidth.HasValue ? c.fileWidth.Value : 0,
                        fileSize = c.fileSize,
                        MediaID = c.MediaID,
                        Description = c.Description,
                        HMNUM = c.HMNUM,
                        ImgName = c.ImgName,
                        ProductID = c.ProductID,
                        MediaType = c.MediaType,
                        SerialNum = c.SerialNum,
                        strCreateOn = c.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                        CloudStatusID = c.CloudStatusID,
                        CMS_SKU = c.SKU_Media_Relation.Where(r => r.MediaID == c.MediaID)
                        .Select(s => new CMS_SKU_Model
                        {
                            ChannelName = s.CMS_SKU.Channel.ChannelName,
                            ProductName = s.CMS_SKU.ProductName,
                            SKU = s.CMS_SKU.SKU
                        }).ToList()
                    });
                }

                return list;
            }

        }


        /// <summary>
        /// 提供ProductConfiguration页面直接上传Media之后，需要根据SKUID重新刷新获取图像信息的操作。由于GetImageList里面的SKUID已经另有其用，
        /// 所以无法共享一个方法，只能新建这个方法。
        /// CreateDate:2014年1月24日17:30:14
        /// </summary>
        /// <param name="SKUID"></param>
        /// <returns></returns>
        public List<MediaLibrary_Model> GetImageListBySKUID(long SKUID)
        {
            List<MediaLibrary_Model> list = new List<MediaLibrary_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //var query = db.MediaLibrary.Where(m => m.SKU_Media_Relation.Any(r => r.SKUID == SKUID));
                //foreach (MediaLibrary c in query)
                //{
                //    list.Add(new MediaLibrary_Model
                //    {
                //        fileFormat = c.fileFormat,
                //        fileHeight = c.fileHeight.HasValue ? c.fileHeight.Value : 0,
                //        fileWidth = c.fileWidth.HasValue ? c.fileWidth.Value : 0,
                //        fileSize = c.fileSize,
                //        MediaID = c.MediaID,
                //        HMNUM = c.HMNUM,
                //        ImgName = c.ImgName,
                //        ProductID = c.ProductID,
                //        MediaType = c.MediaType,
                //    });
                //}
                //return list;

                var query2 = db.SKU_Media_Relation.Where(r => r.SKUID == SKUID).AsEnumerable().Select(
                        k => new MediaLibrary_Model {
                            IsPrimaryImages = k.PrimaryImage,
                            fileFormat = k.MediaLibrary.fileFormat,
                            fileHeight = k.MediaLibrary.fileHeight.HasValue ? k.MediaLibrary.fileHeight.Value : 0,
                            fileWidth = k.MediaLibrary.fileWidth.HasValue ? k.MediaLibrary.fileWidth.Value : 0,
                            fileSize = k.MediaLibrary.fileSize,
                            MediaID = k.MediaLibrary.MediaID,
                            HMNUM = k.MediaLibrary.HMNUM,
                            ImgName = k.MediaLibrary.ImgName,
                            ProductID = k.MediaLibrary.ProductID,
                            MediaType = k.MediaLibrary.MediaType,
                            CloudStatusID = k.MediaLibrary.CloudStatusID,
                            strCreateOn = k.MediaLibrary.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                            CMS_SKU = k.MediaLibrary.SKU_Media_Relation.Where(l => l.MediaID == k.MediaID).Select(s => new CMS_SKU_Model
                            {
                                ChannelName = s.CMS_SKU.Channel.ChannelName,
                                ProductName = s.CMS_SKU.ProductName,
                                SKU = s.CMS_SKU.SKU
                            }).ToList()
                        }
                    ).ToList();

                return query2;
            }
        }



        public List<MediaLibrary_Model> GetImageListByProductID(long StockKeyID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //用AsEnumerable()方法，可以直接使用 c.CreateOn.ToString("yyyy-MM-dd HH:mm:ss")而不会报linQ错误
                var query = db.MediaLibrary.Where(m => m.StockKeyID == StockKeyID).Include(r=>r.SKU_Media_Relation).AsEnumerable();

                return query.Select(c => new MediaLibrary_Model
                {
                    fileFormat = c.fileFormat,
                    fileHeight = c.fileHeight.HasValue ? c.fileHeight.Value : 0,
                    fileWidth = c.fileWidth.HasValue ? c.fileWidth.Value : 0,
                    fileSize = c.fileSize,
                    MediaID = c.MediaID,
                    HMNUM = c.HMNUM,
                    ImgName = c.ImgName,
                    ProductID = c.ProductID,
                    MediaType = c.MediaType,
                    IsPrimaryImages = c.PrimaryImage.HasValue?c.PrimaryImage.Value:false,
                    CloudStatusID = c.CloudStatusID,
                    strCreateOn = c.CreateOn.ToString("yyyy-MM-dd HH:mm:ss"),
                    CMS_SKU = c.SKU_Media_Relation.Where(k => k.MediaID == c.MediaID).Select(s => new CMS_SKU_Model
                    {
                        ChannelName = s.CMS_SKU.Channel.ChannelName,
                        ProductName = s.CMS_SKU.ProductName,
                        SKU = s.CMS_SKU.SKU
                    }).ToList()
                }).ToList();
            }
        }

        /// <summary>
        /// 设置SKU相关的图片为Primary Image
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public bool SetPrimaryImage(long SKUID, long MediaID)
        {
            //设置所有和SKUID相关的SKU_Media_Relation的PrimaryImage字段为false
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                /***
                 * var tranny = DbContext.Database.Connection.BeginTransaction();
                 * tranny.Commit();
                 ***/
                //var curRelation = db.SKU_Media_Relation.Where(r => r.SKUID == SKUID && r.MediaID == MediaID).FirstOrDefault();
                //if (curRelation == null)
                //{
                //    return false;
                //}
                //using (TransactionScope transaction = new TransactionScope())
                //{
                //    db.Database.ExecuteSqlCommand("update SKU_Media_Relation set PrimaryImage = 0 where SKUID = @SKUID", new SqlParameter("@SKUID", SKUID));
                //    curRelation.PrimaryImage = true;
                //    int retInt = db.SaveChanges();
                //    transaction.Complete();
                //    return retInt > 0;
                //} 注释不要删掉 2014年4月1日

                db.SKU_Media_Relation.Where(r => r.SKUID == SKUID).ForEach(r =>
                {
                    if (r.MediaID == MediaID)
                    {
                        r.PrimaryImage = true;
                    }
                    else
                    {
                        r.PrimaryImage = false;
                    }
                });
                var ret =  db.SaveChanges();
                return ret > -1;// don't write equals 0
            }
        }


        /// <summary>
        /// 由于CMS的子产品允许有多张图片，eCom只接收一张，所以必须在CMS页面指定一个“Primary Image”的规则，设置了
        /// Primaryiamge的图片会发送到eCom去。（无法用组合产品的Primary image,因为当前子产品又可以和其他产品组合
        /// 成一个新的组合产品）。  2014年4月21日
        /// </summary>
        /// <param name="ProductID"></param>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public bool SetPrimaryImageForHMNUM(long ProductID, long MediaID)
        {
            //设置所有和SKUID相关的MediaLibrary的PrimaryImage字段为false
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                db.MediaLibrary.Where(r => r.ProductID == ProductID).ForEach(r =>
                {
                    if (r.MediaID == MediaID)
                    {
                        r.PrimaryImage = true;
                    }
                    else
                    {
                        r.PrimaryImage = false;
                    }
                });
                return db.SaveChanges() > 0;
            }
        }

        public bool addMediaLibrary(MediaLibrary_Model c, string User_Account, out long newID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var hm = db.CMS_HMNUM.FirstOrDefault(h=>h.ProductID ==c.ProductID);
                var ExistPImg = hm.CMS_StockKey.MediaLibrary.FirstOrDefault(r => r.PrimaryImage == true);
                MediaLibrary newMedia = new MediaLibrary
                {
                    ProductID = c.ProductID,
                    StockKeyID = hm.StockKeyID,
                    HMNUM = c.HMNUM,
                    SerialNum = c.SerialNum,
                    ImgName = c.ImgName,
                    MediaType = c.MediaType,
                    fileFormat = c.fileFormat,
                    fileSize = c.fileSize,
                    fileWidth = c.fileWidth,
                    fileHeight = c.fileHeight,
                    Description = c.Description,
                    CreateOn = DateTime.Now,
                    CreateBy = User_Account,
                    CloudStatusID =1,
                    PrimaryImage = ExistPImg == null ? true : false
                };
                db.MediaLibrary.Add(newMedia);
                int eVal = db.SaveChanges();
                newID = newMedia.MediaID;
                return eVal > 0;
            }
        }

        /// <summary>
        /// 该方法是addMediaLibrary的扩展，用于在SKUOrder页面直接添加的图像后，新增了一步图像-SKU-关系的逻辑。
        /// CreateDate:2014年1月24日16:29:56。
        /// </summary>
        /// <param name="c"></param>
        /// <param name="User_Account"></param>
        /// <param name="SKUID"></param>
        /// <param name="newID"></param>
        /// <returns></returns>
        public bool addMediaLibraryWithSKURelation(MediaLibrary_Model c, string User_Account, long SKUID, out long newID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var hm = db.CMS_HMNUM.FirstOrDefault(h => h.ProductID == c.ProductID);//hm不可能为空
                var ExistPImg = hm.CMS_StockKey.MediaLibrary.FirstOrDefault(r => r.PrimaryImage == true);
                MediaLibrary newMedia = new MediaLibrary
                {
                    ProductID = c.ProductID,
                    HMNUM = c.HMNUM,
                    StockKeyID = hm.StockKeyID,
                    SerialNum = c.SerialNum,
                    ImgName = c.ImgName,
                    MediaType = c.MediaType,
                    fileFormat = c.fileFormat,
                    fileSize = c.fileSize,
                    fileWidth = c.fileWidth,
                    fileHeight = c.fileHeight,
                    Description = c.Description,
                    CreateOn = DateTime.Now,
                    CreateBy = User_Account,
                    CloudStatusID =1,
                    PrimaryImage = ExistPImg==null?true:false
                };
                db.MediaLibrary.Add(newMedia);

                //好吧，多查询一次应该不会死....2014年2月14日14:18:57
                var IsExistPM = db.SKU_Media_Relation.FirstOrDefault(s=>s.SKUID==SKUID&&s.PrimaryImage==true);

                SKU_Media_Relation newModel = new SKU_Media_Relation
                {
                    MediaID = newMedia.MediaID,
                    SKUID = SKUID,
                    PrimaryImage = IsExistPM==null?true:false 
                };
                db.SKU_Media_Relation.Add(newModel);
                int eVal = db.SaveChanges();
                newID = newMedia.MediaID;
                return eVal > 0;
            }
        }    

        /// <summary>
        /// 根据传递进来的文件名删除文件
        /// Change1:在删除文件之前，必须先删除文件的关系表
        /// </summary>
        /// <param name="MediaID"></param>
        /// <returns></returns>
        public bool removeMediaLibraryByID(long MediaID)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //必须先删除关系，否则会造成数据大概 整个页面Null reference Object!
                /*Store update, insert, or delete statement affected an unexpected number of rows (0). 
                 * Entities may have been modified or deleted since entities were loaded. Refresh ObjectStateManager
                 * SQL Profiler查询追踪到的是 exec sp_executesql N'delete [dbo].[SKU_Media_Relation] where ([RelationID] = @0)',N'@0 bigint',@0=0
                 明显的 ，是以RelationID做为Key去删除！所以这种方法不科学....2014年2月14日14:57:34*/
                //SKU_Media_Relation mr = new SKU_Media_Relation { MediaID = MediaID };
                //db.Set<SKU_Media_Relation>().Attach(mr);
                //db.SKU_Media_Relation.Remove(mr);


                //var query = db.SKU_Media_Relation.Where(r=>r.MediaID==MediaID);
                //foreach (var m in query)
                //{
                //    db.SKU_Media_Relation.Remove(m);
                //}

                //MediaLibrary ml = new MediaLibrary { MediaID = MediaID };
                //db.Set<MediaLibrary>().Attach(ml);
                //db.MediaLibrary.Remove(ml);
                //return db.SaveChanges() > 0;

                /*以下方法是用于删除图像的时候，自动删除所关联的信息，如果不删除，将会引发打开数据异常！*/
                var query = db.MediaLibrary.FirstOrDefault(s => s.MediaID == MediaID);
                db.Entry(query).Collection(m => m.SKU_Media_Relation).Load();
                db.MediaLibrary.Remove(query);
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 获取图片相关的网站产品的信息 （MediaLibrary页面用到）
        /// </summary>
        /// <param name="mediaID"></param>
        /// <returns></returns>
        public List<CMS_SKU_Model> GetCMS_SKUByMediaID(long mediaID)
        {
            List<CMS_SKU_Model> list = new List<CMS_SKU_Model>();
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_SKU.Where(w => w.SKU_Media_Relation.Any(r => r.MediaID == mediaID));
                foreach (CMS_SKU p in query)
                {
                    list.Add(new CMS_SKU_Model
                    {
                        SKU = p.SKU,
                        ProductName = p.ProductName,
                        ChannelName = p.Channel == null ? "None" : p.Channel.ChannelName
                    });
                }
                return list;
            }
        }

      

    }
}
