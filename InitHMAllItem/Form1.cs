using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using System.Threading;
using System.Configuration;

namespace InitHMAllItem
{
    

    public partial class Form1 : Form
    {
        private static Logger HMLog = LogManager.GetCurrentClassLogger(); 

        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 用来在前台即时显示当前处理的文件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HM_OperateNotify(object sender, HMEventArgs e)
        {
            var ath = new AppendTextHandler(this.MCScreen.AppendText);

            MCScreen.BeginInvoke(ath, new object[] { e.Message + Environment.NewLine });
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();     //显示选择文件对话框
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.basicHMPath.Text = openFileDialog1.FileName;          //显示文件路径
            }

            openFileDialog1.Dispose();
        }


        private void btnBrowseGroup_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();     //显示选择文件对话框
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.groupHMPath.Text = openFileDialog1.FileName;          //显示文件路径
            }

            openFileDialog1.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {

                if (this.basicHMPath.Text.Length > 1||this.groupHMPath.Text.Length>0)
                {

                    this.MCScreen.Clear();

                    this.btnStart.Enabled = false;

                    this.btnBrowse.Enabled = false;

                    //GroupHM
                    String TaskType = ConfigurationManager.AppSettings["TaskType"];
                    if (TaskType == "GroupHM")//执行组合产品的初始化
                    {
                        GroupDataProcess process = new GroupDataProcess(this.groupHMPath.Text);
                        process.OperateNotify += HM_OperateNotify;//关联委托
                        Thread t = new Thread(new ThreadStart(process.StartProcess));
                        t.Start();
                    }
                    else//执行基础产品的初始化
                    {
                        DataProcess process = new DataProcess(this.basicHMPath.Text);
                        process.OperateNotify += HM_OperateNotify;//关联委托
                        Thread t = new Thread(new ThreadStart(process.StartProcess));
                        t.Start();
                    }
                    //this.txtBoxResult.AppendText(prints);

                }

                else

                    MessageBox.Show(" 请选择文件！");
            }
            catch (Exception ex)
            {
                this.MCScreen.AppendText(ex.Message);
                //注意 ，如果用多线程，子线程出来一场，主线程压根不会捕获到，所以必须在子线程里面再try catch。。。
                //Lee 2013年12月12日16:37:36
                HMLog.Error(ex.Message);
                HMLog.Error(ex.StackTrace);
                HMLog.Error(ex.Source);
                HMLog.Error("");
                
            }
        }

        /// <summary>
        /// 获取颜色列表信息，提交给Larfier筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMaterial_Click(object sender, EventArgs e)
        {
            if (this.basicHMPath.Text.Length > 1)
            {
                DataProcess process = new DataProcess(this.basicHMPath.Text, this.groupHMPath.Text);
                process.OperateNotify += HM_OperateNotify;//关联委托
                Thread t = new Thread(new ThreadStart(process.GetMaterialList));
                t.Start();
            }
            else
            {

                MessageBox.Show(" 请选择文件！");
            }
        }

        private void btnColour_Click(object sender, EventArgs e)
        {

            if (this.basicHMPath.Text.Length > 1)
            {
                DataProcess process = new DataProcess(this.basicHMPath.Text,this.groupHMPath.Text);
                process.OperateNotify += HM_OperateNotify;//关联委托
                Thread t = new Thread(new ThreadStart(process.GetColourList));
                t.Start();
            }
            else
            {

                MessageBox.Show(" 请选择文件！");
            }

           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitMatiralAndColour fm = new InitMatiralAndColour();
            fm.Show();
        }

        private void MCScreen_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
