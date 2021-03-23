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
        //public IEnumerable<PO_Order_V> GetPOOrderList(string ids)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("SELECT * FROM PO_Order_V  ");
        //    strSql.Append(" where MerchantID in (@ids) ");
        //    SqlParameter[] paramters = {
        //                                  new SqlParameter("@ids", SqlDbType.NVarChar,2000)
        //                                };
        //    paramters[0].Value = ids;
        //    return this.Set<PO_Order_V>().SqlQuery(strSql.ToString(), paramters);
        //}
    }
}
