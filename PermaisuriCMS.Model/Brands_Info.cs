using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model {

    [Serializable]
    public class Brands_Info_Model
    {
        public Brands_Info_Model() { }

        public int Brand_Id { get; set; }

        public string Brand_Name { get; set; }

        public string Short_Name { get; set; }

        public bool Active { get; set; }

        public string Modifier { get; set; }

        public String Modifier_Date { get; set; }



        /// <summary>
        /// 用户查询
        /// CreateDate:2013年12月9日18:45:03
        /// </summary>
        public int page { get; set; }
        public int rows { get; set; }

        /// <summary>
        ///MVC后台bool类型无法自动初始化value为0,1的参数，即使前端使用了parseInt
        /// 0 -All
        /// 1 - Active
        /// 2 - deactive
        /// </summary>
        public int bStatus { set; get; }
    }
}
