using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using EntityFramework;
using EntityFramework.Extensions;

namespace PermaisuriCMS.BLL
{
    public class ShipViaServices
    {
        public List<CMS_ShipVia_Model> GetShipViaList(CMS_ShipVia_Model qModel, out int count)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                var query = db.CMS_ShipVia.AsQueryable();

                if (!string.IsNullOrEmpty(qModel.SHIPVIA))
                {
                    query = query.Where(m => m.SHIPVIA.StartsWith(qModel.SHIPVIA));
                }
                if (qModel.IsDefaultShipViaInd > -1)
                {
                    switch (qModel.IsDefaultShipViaInd)
                    {
                        case 0:
                            query = query.Where(m => m.IsDefaultShipVia == true);
                            break;
                        case 1:
                            query = query.Where(m => m.IsDefaultShipVia == false);
                            break;
                        default:
                            break;
                    }

                }
                if (qModel.ShipViaTypeID > 0)
                {
                    query = query.Where(m => m.ShipViaTypeID == qModel.ShipViaTypeID);
                }

                count = query.Count();

                return query.OrderByDescending(m => m.SHIPVIAID)
                    .Skip((qModel.page - 1) * qModel.rows).Take(qModel.rows).Select(m => new CMS_ShipVia_Model
                    {
                        SHIPVIAID = m.SHIPVIAID,
                        SHIPVIA = m.SHIPVIA,
                        ShipViaTypeID = m.ShipViaTypeID,
                        IsDefaultShipVia = m.IsDefaultShipVia,
                        CarrierCode = m.CarrierCode,
                        CarrierRouting = m.CarrierRouting,
                        ExpressNumLength = m.ExpressNumLength.HasValue ? m.ExpressNumLength.Value : 0,
                        ExpressRule = m.ExpressRule,
                        UpdateBy = m.UpdateBy,
                        CMS_ShipViaType = new CMS_ShipViaType_Model
                        {
                            ShipViaTypeID = m.CMS_ShipViaType.ShipViaTypeID,
                            ShipViaTypeName = m.CMS_ShipViaType.ShipViaTypeName
                        }
                    }).ToList();
            }
        }

        /// <summary>
        /// 2014年5月12日10:05:27。CMS只需要知道SHIPVIAType（物流or快递）就足够了，但是同步到eCom需要确切知道type下面的子类型
        /// </summary>
        /// <param name="sModel"></param>
        /// <returns></returns>
        public bool UpdateDefaultShipVia(CMS_ShipVia_Model sModel, User_Profile_Model userModel)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                //先设置这个类型（快递Or物流）所有的ShipVia子类型都为false
                db.CMS_ShipVia.Where(s => s.ShipViaTypeID == sModel.ShipViaTypeID).Update(s => new CMS_ShipVia { 
                   IsDefaultShipVia = false
                });

                //再设置当前被CMS操作员点击的那个ShipVia为默认的ShipVia的类型
                db.CMS_ShipVia.Where(s => s.SHIPVIAID == sModel.SHIPVIAID && s.ShipViaTypeID == sModel.ShipViaTypeID).Update(s => new CMS_ShipVia
                {
                    IsDefaultShipVia = true,
                    UpdateBy = userModel.User_Account,
                    UpdateOn = DateTime.Now
                });
                return db.SaveChanges() > -1;
            }
        }

        public bool AddShipVia(CMS_ShipVia_Model shipViaModel, User_Profile_Model userModel,out string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                msg = string.Empty;
                var query = db.CMS_ShipVia.FirstOrDefault(s => s.SHIPVIA == shipViaModel.SHIPVIA);
                if (query != null)
                {
                    msg = string.Format("The name of {0} has been in existence already", shipViaModel.SHIPVIA);
                    return false;
                }

                //如果客户端请求讲当前SHIPVIA作为默认的项目传送给eCom，则需要单独处理
                if (shipViaModel.IsDefaultShipVia == true)
                {
                    db.CMS_ShipVia.Where(s => s.ShipViaTypeID == shipViaModel.ShipViaTypeID).Update(s => new CMS_ShipVia
                    {
                        IsDefaultShipVia = false
                    });
                }

                db.CMS_ShipVia.Add(new CMS_ShipVia {
                    SHIPVIA = shipViaModel.SHIPVIA,
                    ShipViaTypeID = shipViaModel.ShipViaTypeID,
                    ExpressMethod = shipViaModel.ExpressMethod,
                    ExpressNumLength = shipViaModel.ExpressNumLength,
                    IsDefaultShipVia = shipViaModel.IsDefaultShipVia,
                    CarrierCode = shipViaModel.CarrierCode,
                    CarrierRouting = shipViaModel.CarrierRouting,
                    ExpressRule = shipViaModel.ExpressRule,
                    CreateBy =userModel.User_Account,
                    CreateOn = DateTime.Now,
                    UpdateBy = userModel.User_Account,
                    UpdateOn = DateTime.Now
                });
                return db.SaveChanges()>0;
            }
        }

        public bool EditShipVia(CMS_ShipVia_Model shipViaModel, User_Profile_Model userModel, out string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                msg = string.Empty;
                var query = db.CMS_ShipVia.FirstOrDefault(s => s.SHIPVIAID == shipViaModel.SHIPVIAID);
                if (query == null)
                {
                    msg = string.Format("The name of {0} does not exist", shipViaModel.SHIPVIA);
                    return false;
                }

                //如果客户端请求讲当前SHIPVIA作为默认的项目传送给eCom，则需要单独处理
                if (shipViaModel.IsDefaultShipVia == true)
                {
                    db.CMS_ShipVia.Where(s => s.ShipViaTypeID == shipViaModel.ShipViaTypeID).Update(s => new CMS_ShipVia
                    {
                        IsDefaultShipVia = false
                    });
                }
                //query.SHIPVIA = shipViaModel.SHIPVIA;
                query.ShipViaTypeID = shipViaModel.ShipViaTypeID;
                query.ExpressMethod = shipViaModel.ExpressMethod;
                query.ExpressNumLength = shipViaModel.ExpressNumLength;
                query.IsDefaultShipVia = shipViaModel.IsDefaultShipVia;
                query.CarrierCode = shipViaModel.CarrierCode;
                query.CarrierRouting = shipViaModel.CarrierRouting;
                query.ExpressRule = shipViaModel.ExpressRule;
                query.UpdateBy = userModel.User_Account;
                query.UpdateOn = DateTime.Now;
                return db.SaveChanges() > 0;
            }
        }



        public bool DeleteShipVia(CMS_ShipVia_Model shipViaModel, User_Profile_Model userModel, out string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                msg = string.Empty;
                var isDefault=false;
                var query = db.CMS_ShipVia.FirstOrDefault(s => s.SHIPVIAID == shipViaModel.SHIPVIAID);
                if (query == null)
                {
                    msg = string.Format("The name of {0} does not exist", shipViaModel.SHIPVIA);
                    return false;
                }

                //如果客户端请求讲当前SHIPVIA作为默认的项目传送给eCom，则需要单独处理
                isDefault = query.IsDefaultShipVia ;
                db.CMS_ShipVia.Remove(query);
                if (isDefault)
                {
                    var randomSV = db.CMS_ShipVia.Where(s => s.SHIPVIAID != shipViaModel.SHIPVIAID && s.ShipViaTypeID == query.ShipViaTypeID)
                        .OrderByDescending(r=>r.SHIPVIAID).FirstOrDefault();
                    if (randomSV != null)
                    {
                        randomSV.IsDefaultShipVia = true;
                    }
                }
                return db.SaveChanges() > 0;
            }
        }

    }
}
