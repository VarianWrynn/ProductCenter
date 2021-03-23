using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InitImagesByHM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fld = new FolderBrowserDialog();//c#实现的类，代码关键
            fld.ShowDialog();
           // fld.RootFolder =Environment.SpecialFolder. ;
         

            //fld.SelectedPath = "D:/美图图库";

            string temPath = fld.SelectedPath;

            if (temPath == "")
            {
                MessageBox.Show("你没有选择目录啊");//没有选择目录，点击了取消
            }
            else
            {
                this.txtPath.Text = temPath;
            }
            fld.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.txtPath.Text.Length > 1)
            {

                this.txtBoxResult.Clear();

                this.btnStart.Enabled = false;

                this.btnBrowse.Enabled = false;
                ImageHandler imgHandler = new ImageHandler(this.txtPath.Text);
                imgHandler.OperateNotify += imgHandler_OperateNotify;//关联委托
                imgHandler.InitImages();

            }

            else

                MessageBox.Show(" 请选择文件！");
        }


        /// <summary>
        /// 用来在前台即时显示当前处理的文件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgHandler_OperateNotify(object sender, InitImagesEventArgs e)
        {
            var ath = new AppendTextHandler(this.txtBoxResult.AppendText);

            txtBoxResult.BeginInvoke(ath, new object[] { e.Message + Environment.NewLine });
        }


        /// <summary>
        /// 用来通知线程的执行结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vssConverter_ThreadCompleted(object sender, EventArgs e)
        {
            if (convertOK == 0)

                convertOK++;

            else
            {
                this.btnBrowse.Enabled = true;

                this.btnStart.Enabled = true;

                this.btnQuit.Enabled = true;

                this.txtBoxResult.AppendText("############# 所有的图像都已经初始化完毕 ############");
            }
        }


        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.ExitThread(); //关闭程序.
            Close();//关闭当前窗口.
            Dispose();//释放内存
        }
    }
}
