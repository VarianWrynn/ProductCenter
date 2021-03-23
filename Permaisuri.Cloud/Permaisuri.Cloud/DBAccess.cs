using System.Linq;
using PermaisuriCMS.DAL;
using EntityFramework.Extensions;

namespace Permaisuri.Cloud
{
   public class DBAccess
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hmnum"></param>
        /// <param name="cloudStatus">1:Not yet upload ; 2:uploading; 3:fail to upload; 4:cloud supported</param>
        public void UploadingCloudStatus(string hmnum, int cloudStatus)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                switch (cloudStatus)
                {
                    case 1:
                        db.MediaLibrary.Where(m => m.HMNUM == hmnum).Update(u => new MediaLibrary { CloudStatusID = 1 });
                        //db.MediaLibrary.Where(m => m.HMNUM == HMNUM).ForEach(m => m.CloudStatusID = 1);
                        //db.SaveChanges();
                        break;
                    case 2:
                        db.MediaLibrary.Where(m => m.HMNUM == hmnum).Update(u => new MediaLibrary { CloudStatusID = 2 });
                        break;
                    case 3:
                        db.MediaLibrary.Where(m => m.HMNUM == hmnum).Update(u => new MediaLibrary { CloudStatusID = 3 });
                        //db.MediaLibrary.Where(m => m.HMNUM == HMNUM).ForEach(m => m.CloudStatusID = 3);
                        //db.SaveChanges();
                        break;
                    case 4:
                        db.MediaLibrary.Where(m => m.HMNUM == hmnum).Update(u => new MediaLibrary { CloudStatusID = 4 });
                        //db.MediaLibrary.Where(m => m.HMNUM == HMNUM).ForEach(m => m.CloudStatusID = 4);
                        //db.SaveChanges();
                        break;
                }
            }
        }

        /// <summary>
        /// 通过MediaID来一项项的更新当前图片的云端ID 2014年4月12日
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="cloudStatus">1:Not yet upload ; 2:uploading; 3:fail to upload; 4:cloud supported</param>
        public void UploadingCloudStatusByMediaID(long mediaId, int cloudStatus)
        {
            using (var db = new PermaisuriCMSEntities())
            {
                db.MediaLibrary.Where(m => m.MediaID == mediaId).Update(u => new MediaLibrary { CloudStatusID = cloudStatus });
            }
        }

    }
}
