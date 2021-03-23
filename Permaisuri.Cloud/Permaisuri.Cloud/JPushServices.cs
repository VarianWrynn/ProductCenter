using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ifunction.JPush;
using ifunction.JPush.V3;

namespace Permaisuri.Cloud
{
    /// <summary>
    /// 激光推送类
    /// 2014年12月11日
    /// </summary>
    public class JPushServices
    {
        //appKey是在极光推送服务器上，根据当前的app生成的而一个key。
        //private readonly string _appKey = CommonAppService.Current.GetSettingValue("CRM_AppSetting", "appKey");
        //private readonly string _masterSecret = CommonAppService.Current.GetSettingValue("CRM_AppSetting", "masterSecret");

// ReSharper disable once ConvertToConstant.Local
        private readonly string _appKey = "c34d3ca23c3d45ad037e64b1";
// ReSharper disable once ConvertToConstant.Local
        private readonly string _masterSecret = "b733cc078c8f523d82a57518";


        public void SendMessage(IEnumerable<SaleMessage> messages)
        {
            JPush(messages);
        }


        private void JPush(IEnumerable<SaleMessage> messages)
        {
            foreach (var message in messages)
            {
                //var customizedValues = new Dictionary<string, string>
                //{
                //    {"type", message.MessageType.ToString()},
                //    {"id", message.RelateId.ToString()}
                //};

                //{"JName":"0","JValue":"/Web/user/index?getCustomerId=28861,测试极光推送"}
                //{"JName":"1001","JValue":"28861"}
                //{"JName":"2","JValue":""}
                //{"JName":"10","JValue":""}

                var jp = new JavaScriptSerializer().Deserialize<JParams>(message.JParams);
                var customizedValues = new Dictionary<string, string>
                {
                    {"jName", jp.JName},
                    {"jValue", jp.JValue}
                };
                var client = new JPushClientV3(_appKey, _masterSecret);
                var audience = new Audience();

                //audience.AddItem(PushTypeV3.Broadcast, "all");
                audience.AddItem(PushTypeV3.ByAlias, message.EmployeeId.ToString().Replace("-", ""));
                var response = client.SendPushMessage(new PushMessageRequestV3
                {
                    Notification = new Notification
                    {
                        AndroidNotification = new AndroidNotificationParameters
                        {
                            Title = message.Title ?? "",
                            Alert = message.Content ?? "",
                            CustomizedValues = customizedValues,
                        }
                    },
                    AppMessage = new AppMessage { Title = message.Title ?? "", Content = message.Content ?? "" },
                    Audience = audience,
                    Platform = PushPlatform.Android,
                    LifeTime = 863000,
                });

                //更新信息状态
                message.IsSent = true;
                //Modify(message);
            }
        }
    }


    public partial class SaleMessage
    {
        public SaleMessage()
        {

        }

        public SaleMessage(Guid employeeId)
        {
            EmployeeId = employeeId;
            CreateTime = DateTime.Now;
            IsSent = false;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool? IsSent { get; set; }
        public int? MessageType { get; set; }
        public int? RelateId { get; set; }
        public Guid? EmployeeId { get; set; }

        public DateTime? SentTime { get; set; }
        public string JParams { get; set; }
    }


    public class JParams
    {
        /// <summary>
        /// 
        /// </summary>
        public string JName { get; set; }

        /// <summary>
        /// </summary>
        public string JValue { get; set; }


    }

    public enum JActionEnum
    {
        /// <summary>
        /// 网页
        /// </summary>
        WebPage = 0,

        /// <summary>
        /// 客户详情页 需要配合 
        /// </summary>
        CustomerDetails = 1001,

        /// <summary>
        /// 顾问-主页（今日）
        /// </summary>
        EmployeeHome = 1,

        /// <summary>
        /// 顾问-任务
        /// </summary>
        EmployeeTask = 2,


        /// <summary>
        /// 经理-主页
        /// </summary>
        ManagerHome = 10,


        /// <summary>
        /// 经理-目标设置
        /// </summary>
        ManagerTargetSetting = 11,

        /// <summary>
        /// 经理-团队
        /// </summary>
        ManagerTeam = 12
    }
}
