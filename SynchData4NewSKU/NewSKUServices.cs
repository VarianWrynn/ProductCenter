using SynchData4NewSKU.BLL;
using NLog;

namespace SynchData4NewSKU
{
    public class NewSkuServices
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); 

        public static void RunServices()
        {
            var bllHandler = new NewSKUHandler();

            var retVal = bllHandler.DoSynchDataBySP();
            if (retVal>0)
            {
                Logger.Error("Synchronizing HM's data between Ecom and CMS system successfully!");
            }
            else if (retVal == 0)
            {
                Logger.Error("Synchronizing HM's data between Ecom and CMS system successfully,There is no new data in Ecom!");
            }
            else
            {
                Logger.Error("Synchronizing HM's data between Ecom and CMS system failed!");
            }
        }
    }
}
