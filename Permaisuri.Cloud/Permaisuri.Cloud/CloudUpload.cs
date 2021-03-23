using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using PermaisuriCMS.Common;
using PermaisuriCMS.DAL;


namespace Permaisuri.Cloud
{
    public class CloudUpload
    {
        private readonly IAmazonS3 _s3Client = AWSClientFactory.CreateAmazonS3Client();
        private readonly string _defaultBucketName = string.Empty;
        private readonly string _imageStoragePath = string.Empty;


        public CloudUpload(string imageStoragePath)
        {
           
            _defaultBucketName = ConfigurationManager.AppSettings["AWSBucketName"];
            _imageStoragePath = imageStoragePath;
        }

        /// <summary>
        /// 2014年4月11日17:59:38
        /// </summary>
        public void DoUpload()
        {
            //CloudUpload upload = new CloudUpload();
            //upload.UploadDirectory("D:/ProjectFiles/Permaisuri/Permaisuri/Permaisuri/MediaLib/Files/9043.01", "9043.01");

            //var dirs = new DirectoryInfo(Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"]))
            //    .GetDirectories();


            //Parallel.ForEach(dirs,(r,loopstats)=>{
            //    CloudUpload upload = new CloudUpload();
            //    upload.UploadDirectory(r.FullName, r.Name);
            //});

            //S1：从HMNUM表里面查询哪些HMNUM还未在云端上建立文件夹的，则提取出来，直接整个文件上传
            //S2:从MediaLibrary表中查询哪些CloudStatusID不为4的，全部重新上传
            using (var db = new PermaisuriCMSEntities())
            {
                var needFolderUploadList = db.CMS_HMNUM.Where(h => h.IsCloud != true && h.CMS_StockKey.MediaLibrary.Count > 0).Take(3);
                //Parallel.ForEach(needFolderUploadList, (r, loopstatus) =>
                //{
                //    UploadDirectory(ImageStoragePath, r.HMNUM);
                //});

                Parallel.ForEach(needFolderUploadList, (r, loopstatus) => UploadDirectory(_imageStoragePath, r.HMNUM));

                //由于两个Parallel都是异步执行，所以这里单个上传需要过滤，只上传已经失败和未上传的那些图像，同时云端已经包含了这些文件夹（标识曾经上传过）
                var singelUploadList = db.MediaLibrary.Where(m => m.CMS_StockKey.CMS_HMNUM.Any(r => r.IsCloud == true))
                    .Where(m => m.CloudStatusID != 4).Where(m => m.CloudStatusID != 2).Take(10);
                Parallel.ForEach(singelUploadList, 
                    (r, loopstatus) => UploadFile(_imageStoragePath + "/" + r.HMNUM + "/" + r.ImgName + r.fileFormat, r.HMNUM, r.ImgName + r.fileFormat, r.MediaID));
            }
        }


        /// <summary>
        /// 用于给前端手工上传使用
        /// </summary>
        /// <param name="mediaIdList"></param>
        public void CloudUploadWithMediaList(IEnumerable<long> mediaIdList)
        {
            Parallel.ForEach(mediaIdList, (id, loopstatus) =>
                {
                    using (var db = new PermaisuriCMSEntities())
                    {
                        var r = db.MediaLibrary.FirstOrDefault(m => m.MediaID == id);
                        if (r != null)
                        {
                            UploadFile(_imageStoragePath + "/" + r.HMNUM + "/" + r.ImgName + r.fileFormat, r.HMNUM, r.ImgName + r.fileFormat, r.MediaID);
                        }
                    }

                });
        }

        /// <summary>
        /// 那么，bucketName就是HMNUM, dir就是图片跟目录+HMNUM.
        /// Change1:A bucket is owned by the AWS account that created it. Each AWS account can own up to 100 buckets at a time.
        /// Bucket ownership is not transferable; however, if a bucket is empty, you can delete it. After a bucket is deleted, 
        /// the name becomes available to reuse, but the name might not be available for you to reuse for various reasons. For 
        /// example, some other account could create a bucket with that name. Note, too, that it might take some time before 
        /// the name can be reused. So if you want to use the same bucket name, don't delete the bucket.
        /// http://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
        /// 所以,Bucket不应该用HMNUM来创建，而应该由我们指定配置一个！然后再Buket里面创建以HMNUM命名的文件夹。2014年4月9日
        /// </summary>
        /// <param name="dir">本地（绝对路径）文件夹</param>
        /// <param name="folderName">对应HMNUM,每一个HMNUM在云上当做一个新文件夹存储</param>
        private void UploadDirectory(string dir,string folderName)
        {
            try
            {
                //The specified key does not exist.

                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatus(folderName, 2);

                using (var directoryTransferUtility = new TransferUtility(_s3Client))
                {
                    var tuudRequest = new TransferUtilityUploadDirectoryRequest
                    {
                        CannedACL = S3CannedACL.PublicRead,
                        Directory = dir,
                        BucketName = _defaultBucketName + "/" + folderName,
                        SearchOption = System.IO.SearchOption.AllDirectories
                        //SearchPattern = "*.JPG"
                    };
                    directoryTransferUtility.UploadDirectory(tuudRequest);
                    dbAccess.UploadingCloudStatus(folderName, 4);
                }
             
                // Upload a directory.
                //directoryTransferUtility.UploadDirectory(dir, "media.cms.noblehouse/" + folderName); 这句话会导致图像变成private link,因为没有设置权限..2014年4月10日
            }
            catch (AmazonS3Exception ex)
            {
                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatus(folderName, 3);
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
            }
            catch (Exception ex)
            {
                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatus(folderName, 3);
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
            }

        }

        /// <summary>
        /// 上传单个图片到云端.2014年4月11日
        /// 上传单个图像之前应该图像路径下的HMNUM已经存在于云端服务器上，如果不存在，则需要调用该方法之后手动update CMS_HMNUM表上的IsCloud为ture，
        /// 一般来说，程序会先判断，如果不存在云端，则先调用UploadDirectory()方法，该方法会更新CMS_HMNUM表的云端标识，之后单图片上传再调用这个方法。 2014年4月14日
        /// </summary>
        /// <param name="dir">图像的全路径，绝对物理地址,包括文件名和文件后缀名</param>
        /// <param name="folderName"> 就是HMNUM....</param>
        /// <param name="fileName">图像的名称，包含格式，比如 123.jpg</param>
        /// <param name="mediaId">图像在库表里的唯一标识，用于单张图片云端交互之后的状态更新</param>
        private void UploadFile(string dir,string folderName,string fileName, long mediaId)
        {
            try
            {
                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatusByMediaID(mediaId, 2);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _defaultBucketName + "/" + folderName,
                    FilePath = dir,
                    //StorageClass = S3StorageClass.ReducedRedundancy,
                    //PartSize = 6291456, // 6 MB.
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(_s3Client);
                fileTransferUtility.Upload(fileTransferUtilityRequest);
                dbAccess.UploadingCloudStatusByMediaID(mediaId, 4);
            }
            catch (AmazonS3Exception ex)
            {
                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatusByMediaID(mediaId, 3);
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
            }
            catch (Exception ex)
            {
                var dbAccess = new DBAccess();
                dbAccess.UploadingCloudStatusByMediaID(mediaId, 3);
                NBCMSLoggerManager.Error("");
                NBCMSLoggerManager.Error(ex.Message);
                NBCMSLoggerManager.Error(ex.StackTrace);
                NBCMSLoggerManager.Error("");
            }
        }
    }
}
