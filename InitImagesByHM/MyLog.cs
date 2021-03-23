using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitImagesByHM
{
  public  class MyLog
    {
      private static string logPath = ConfigurationManager.AppSettings["DestinationDir"];
      public static void Log(string str)
      {
          StreamWriter sw = null;
          try
          {
              DirectoryInfo di = new DirectoryInfo(logPath);//获取D:\testing 目录的信息
              if (!di.Exists)//如果没有该目录，则直接创建一个
              {
                  di.Create();
              }
              FileInfo fileInfo = new FileInfo(logPath+"/ExceptionSKU.txt");//获取 D:\testing\test.txt 文本的信息
              if (!fileInfo.Exists)//同样的，如果没有该文件，则直接创建一个，注意使用CreateText创建完同时返回 StreamWriter 流
              {
                  sw = fileInfo.CreateText();
              }
              else
              {
                  sw = fileInfo.AppendText(); //存在该文件，则对该文件流进行写入叠加操作（即不覆盖掉之前写的东西）
              }
              sw.WriteLine(str);
              sw.Flush(); //注意关闭文件流
              sw.Close();
              sw.Dispose();
          }
          catch (Exception ex)
          {
              if (sw != null)
              {
                  sw.Close();
                  sw.Flush();
                  sw = null;
              }

          }
      }
    }
}
