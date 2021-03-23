using System;
using System.Configuration;
using System.ServiceProcess;
using NLog;

namespace SynchData4NewSKU
{
    public partial class Service1 : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var timerInterval = ConfigurationManager.AppSettings["HMDataTimerInterval"];
                Logger.Info("WEBPO--CMS new HM 数据同步服务启动");
                //var aTimmer = new System.Timers.Timer();
                //aTimmer.Interval = (1000 * 60) * System.Convert.ToInt32(TimerInterval); //间隔时间
                //aTimmer.Elapsed += new System.Timers.ElapsedEventHandler(RunningSrv);//到达时间的时候执行事件；   
                //aTimmer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；   
                //aTimmer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；   

                var aTimmer = new System.Timers.Timer {Interval = (1000*60)*Convert.ToInt32(timerInterval)};
                aTimmer.Elapsed += RunningSrv;//到达时间的时候执行事件；   
                aTimmer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；   
                aTimmer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；   
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.Message);
                Logger.Fatal("======================");
                Logger.Fatal(ex.Source);
            }
        }

        protected override void OnStop()
        {
            Logger.Info("WEBPO--CMS new HM 数据同步服务停止");
        }


        /// <summary>  
        /// 定时检查，并执行方法  
        /// </summary>  
        /// <param name="source"></param>  
        /// <param name="e"></param>  
        private void RunningSrv(object source, System.Timers.ElapsedEventArgs e)
        {
            Logger.Debug(">>>>>>>>>>>>>>>>>>>开始执行 Ecom--CMS new status 数据同步服务 定时服务<<<<<<<<<<<<<<<<<<<<<<<<,");
            NewSkuServices.RunServices();
        }  
    }
}
