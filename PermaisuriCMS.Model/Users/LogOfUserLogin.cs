using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{
    [Serializable]
    public class LogOfUserLogin_Model
    {
        public long ID { get; set; }
        public string User_Account { get; set; }
        public string Display_Name { get; set; }
        public string Logging_IP { get; set; }
        public string Machine_Name { get; set; }
        public string Logging_Location { get; set; }
        public bool LoggingStatue { get; set; }
        public System.DateTime Logging_Date { get; set; }
        public String LoggingDate { get; set; }
        public string Remark { get; set; }

        public int page { get; set; }

        public int rows { get; set; }
    }
}
