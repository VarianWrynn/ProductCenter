using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermaisuriCMS.Model
{

    public enum EnumJobStatus
    {
        Prepared = 10,
        Done = 20,
        TimeOut = 30,
        Canceled = 40
    }

    public static partial class EnumExtensions
    {
        public static string ToText(this EnumJobStatus value)
        {
            switch ((value))
            {
                case EnumJobStatus.Prepared:
                    return "任务待打印（任务产⽣生时默认状态）";
                case EnumJobStatus.Done:
                    return "任务成功完成";
                case EnumJobStatus.TimeOut:
                    return "任务超时失败";
                case EnumJobStatus.Canceled:
                    return "任务取消";
                default:
                    return "Unknow";
            }
        }
    }
}
