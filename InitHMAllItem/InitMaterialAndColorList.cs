using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using NLog;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using System.Data.Entity.Validation;

namespace InitHMAllItem
{
    /// <summary>
    /// 操作信息事件代理
    /// </summary>
    public delegate void MCNotifyHandler(object sender, HMEventArgs e);

    /// <summary>
    /// Created:2014年1月25日10:10:30
    /// </summary>
    public class InitMaterialAndColorList
    {
        private string _path =String.Empty;//操作图像的根目录
        public static string connStr = string.Empty;
        public static string UpdateBy = "HM-ALL ITEM";
        private static Logger MCLog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 操作信息事件
        /// </summary>
        public event MCNotifyHandler OperateNotify;


        protected virtual void OnOperateNotify(object sender, HMEventArgs e)
        {
            if (OperateNotify != null)

                OperateNotify(sender, e);
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="operatePath"></param>
        public InitMaterialAndColorList(string path)
        {
            _path = path;
            connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + _path + ";Extended Properties=Excel 12.0;";
        }

        public void InitThoseList()
        {
          
            var delegateThread = new Thread(StartToDo);
            delegateThread.Start();
        }

        /// <summary>
        /// Parallel方法实现，仅供参考，注意数据库实例需要放在Parallel里面实现
        /// </summary>
        private void StartToDo2()
        {
            //构建连接字符串
            OleDbConnection Conn = new OleDbConnection(connStr);
            Conn.Open();
            //填充数据
            string sql = string.Format("select * from [{0}$]", "Final data");
            OleDbDataAdapter da = new OleDbDataAdapter(sql, connStr);
            DataSet ds = new DataSet();
            da.Fill(ds);
            Conn.Close();
            var Test = ds.Tables[0].AsEnumerable();
            //using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            //{
            int j = 0;
            Parallel.ForEach(Test, (dr, loopState) =>
            {
                using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
                {
                    j++;
                    OnOperateNotify(this, new HMEventArgs(string.Format("开始第{0}的处理", j)));
                    string strColor = dr["Final Color"].ToString();
                    string strMaterial = dr["Final Material"].ToString();
                    if (!String.IsNullOrEmpty(strColor))
                    {
                        var newC = new CMS_SKU_Colour
                        {
                            ColourName = strColor,
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            ModifyBy = UpdateBy,
                            ModifyOn = DateTime.Now
                        };
                        db.CMS_SKU_Colour.Add(newC);
                    }

                    if (!String.IsNullOrEmpty(strMaterial))
                    {
                        db.CMS_SKU_Material.Add(new CMS_SKU_Material
                        {
                            MaterialName = strMaterial,
                            CreateBy = UpdateBy,
                            CreateOn = DateTime.Now,
                            ModifyBy = UpdateBy,
                            ModifyOn = DateTime.Now
                        });
                    }
                    db.SaveChanges();
                }

            });//end of Parallel.ForEach
        }
        private void StartToDo()
        {
            try
            {
                //构建连接字符串
                OleDbConnection Conn = new OleDbConnection(connStr);
                Conn.Open();
                //填充数据
                string sql = string.Format("select * from [{0}$]", "Final data");
                OleDbDataAdapter da = new OleDbDataAdapter(sql, connStr);
                DataSet ds = new DataSet();
                da.Fill(ds);
                Conn.Close();
                var Test = ds.Tables[0].AsEnumerable();
                using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
                {
                    //int j = 0;
                    //Parallel.ForEach(Test, (dr, loopState) =>
                    //{
                    //    using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
                    //    {
                    //        j++;
                    //        OnOperateNotify(this, new HMEventArgs(string.Format("开始第{0}的处理", j)));
                    //        string strColor = dr["Final Color"].ToString();
                    //        string strMaterial = dr["Final Material"].ToString();
                    //        if (!String.IsNullOrEmpty(strColor))
                    //        {
                    //            var newC = new CMS_SKU_Colour
                    //            {
                    //                ColourName = strColor,
                    //                CreateBy = UpdateBy,
                    //                CreateOn = DateTime.Now,
                    //                ModifyBy = UpdateBy,
                    //                ModifyOn = DateTime.Now
                    //            };
                    //            db.CMS_SKU_Colour.Add(newC);
                    //        }

                    //        if (!String.IsNullOrEmpty(strMaterial))
                    //        {
                    //            db.CMS_SKU_Material.Add(new CMS_SKU_Material
                    //            {
                    //                MaterialName = strMaterial,
                    //                CreateBy = UpdateBy,
                    //                CreateOn = DateTime.Now,
                    //                ModifyBy = UpdateBy,
                    //                ModifyOn = DateTime.Now
                    //            });
                    //        }
                    //        db.SaveChanges();
                    //    }
                       
                    //});//end of Parallel.ForEach



                    int j = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        j++;
                        OnOperateNotify(this, new HMEventArgs(string.Format("开始第{0}的处理",j)));
                        string strColor = dr["Final Color"].ToString();
                        string strMaterial = dr["Final Material"].ToString();

                        if (!String.IsNullOrEmpty(strColor))
                        {
                            db.CMS_SKU_Colour.Add(new CMS_SKU_Colour
                            {
                                ColourName = strColor,
                                CreateBy = UpdateBy,
                                CreateOn = DateTime.Now,
                                ModifyBy = UpdateBy,
                                ModifyOn = DateTime.Now
                            });
                            db.SaveChanges();
                        }

                        if (!String.IsNullOrEmpty(strMaterial))
                        {
                            db.CMS_SKU_Material.Add(new CMS_SKU_Material
                            {
                                MaterialName = strMaterial,
                                CreateBy = UpdateBy,
                                CreateOn = DateTime.Now,
                                ModifyBy = UpdateBy,
                                ModifyOn = DateTime.Now
                            });
                            db.SaveChanges();
                        }
                    }

                    OnOperateNotify(this, new HMEventArgs("全部处理完毕，可以退出了"));
                }
            }
            catch (DbEntityValidationException e)
            {
                OnOperateNotify(this, new HMEventArgs("Error!"));
                OnOperateNotify(this, new HMEventArgs("出错啦！!"));
                MCLog.Error("");
                MCLog.Error("");
                foreach (var eve in e.EntityValidationErrors)
                {
                    MCLog.Error("");
                    MCLog.Error("");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        MCLog.Error("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        OnOperateNotify(this, new HMEventArgs(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage)));
                    }
                    MCLog.Error("");
                }
                MCLog.Error("");
                MCLog.Error("");
                OnOperateNotify(this, new HMEventArgs(""));
                OnOperateNotify(this, new HMEventArgs(""));

            }
            catch (Exception ex)
            {
                OnOperateNotify(this, new HMEventArgs("Error~~~~~~~~~~~~~~~"));
                OnOperateNotify(this, new HMEventArgs("出错啦~~~~~~~~~~~~~~"));
                OnOperateNotify(this, new HMEventArgs(ex.Message));
                MCLog.Error("");
                MCLog.Error("Exception Started");
                MCLog.Error(ex.Message);
                MCLog.Error(ex.Source);
                MCLog.Error(ex.StackTrace);
                MCLog.Error("Exception End");
                MCLog.Error("");
                OnOperateNotify(this, new HMEventArgs(""));
                OnOperateNotify(this, new HMEventArgs(""));
            }
        }
    }
}
