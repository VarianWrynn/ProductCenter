using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    [Serializable]
    public class LogOfUserOperating_Model
    {
        public long ID { get; set; }
        public string User_Account { get; set; }
        public string Display_Name { get; set; }
        public string Model_Name { get; set; }
        public string Action_Name { get; set; }
        public int Operating_Type { get; set; }
        public string OldData { get; set; }
        public string NewDate { get; set; }
        public System.DateTime Operating_Date { get; set; }
        public String OperatingDate { get; set; }
        public string IP_Address { get; set; }
        public string Remark { get; set; }

        public int page { get; set; }

        public int rows { get; set; }
    }
}
