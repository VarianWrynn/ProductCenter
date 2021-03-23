﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    /// <summary>
    /// HM#的材料信息
    /// CreateDate：2014年1月8日15:59:52
    /// </summary>
    public class CMS_HMNUM_Material_Model
    {
        public long MaterialID { get; set; }
        public string MaterialName { get; set; }
        public string MaterialDesc { get; set; }
        public string CreateBy { get; set; }
        public System.DateTime CreateOn { get; set; }
        public System.DateTime ModifyOn { get; set; }
        public string ModifyBy { get; set; }
    }
}
