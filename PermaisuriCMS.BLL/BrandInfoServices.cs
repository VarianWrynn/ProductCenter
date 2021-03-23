using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.Model;
using PermaisuriCMS.DAL;

namespace PermaisuriCMS.BLL {
	public class BrandInfoServices {
		//public List<Brands_Info_Model> GetBrandList(int page, int rows, string sort, string order, string strwhere, out int count) {
        public List<Brands_Info_Model> GetBrandList(Brands_Info_Model queryModel, out int count)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                List<Brands_Info_Model> list = new List<Brands_Info_Model>();
                var query = db.Brand.AsQueryable();
                if (!string.IsNullOrEmpty(queryModel.Brand_Name))
                {
                    query = query.Where(q => q.BrandName.Contains(queryModel.Brand_Name));
                }

                if (!string.IsNullOrEmpty(queryModel.Short_Name))
                {
                    query = query.Where(q => q.ShortName.Contains(queryModel.Short_Name));
                }
                if (queryModel.bStatus > 0)
                {
                    switch (queryModel.bStatus)
                    {
                        case 1:
                            query = query.Where(q => q.Active == true);
                            break;
                        case 2:
                            query = query.Where(q => q.Active == false);
                            break;
                        default:
                            break;
                    }
                }
                count = query.Count();
                query = query.OrderByDescending(q => q.BrandID).Skip((queryModel.page - 1) * queryModel.rows).Take(queryModel.rows);
                foreach (var ui in query)
                {
                    list.Add(new Brands_Info_Model
                    {
                        Brand_Id = ui.BrandID,
                        Brand_Name = ui.BrandName,
                        Short_Name = ui.ShortName,
                        Active = ui.Active,
                        Modifier = ui.Modifier,
                        Modifier_Date = ui.Modifier_Date.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
                return list;
            }
        }

        public bool AddBrand(Brands_Info_Model model, string user, ref string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                var isExist = db.Brand.FirstOrDefault(c => c.BrandName == model.Brand_Name);
                msg = string.Empty;

                if (isExist != null)
                {
                    msg = "This item does exist";
                    return false;
                }

                var newBrand = new Brand
                {
                    BrandName = model.Brand_Name,
                    ShortName = model.Short_Name,
                    Modifier = user,
                    Modifier_Date = DateTime.Now,
                    Active = model.Active
                };
                db.Brand.Add(newBrand);
                return db.SaveChanges() > 0;
            }
        }

        public bool EditBrand(Brands_Info_Model model, string user, ref string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {

                var isExist = db.Brand.FirstOrDefault(c => c.BrandName == model.Brand_Name);
                msg = string.Empty;

                if (isExist == null)
                {
                    msg = "This item does NOT exist";
                    return false;
                }
                Brand entity = db.Brand.Where(a => a.BrandID == model.Brand_Id).FirstOrDefault();
                entity.BrandName = model.Brand_Name;
                entity.ShortName = model.Short_Name;
                entity.Active = model.Active;
                entity.Modifier = user;
                entity.Modifier_Date = DateTime.Now;
                return db.SaveChanges() > 0;
            }
        }

        public bool DeleteBrand(int Brand_Id,ref string msg)
        {
            using (PermaisuriCMSEntities db = new PermaisuriCMSEntities())
            {
                msg = string.Empty;
                var brand = db.Brand.FirstOrDefault(b => b.BrandID == Brand_Id);
                if (brand == null)
                {
                    msg = "this item does not exist";
                    return false;
                }

                if (brand.CMS_SKU.Count > 0)
                {
                    msg = "this item can not be deleted, because it is associated with SKUOrder!";
                    return false;
                }

                db.Brand.Remove(brand);

                //db.Brand.Remove(db.Brand.Where(a => a.BrandID == Brand_Id).FirstOrDefault());
                return db.SaveChanges() >= 0;
            }
        }
	}
}
