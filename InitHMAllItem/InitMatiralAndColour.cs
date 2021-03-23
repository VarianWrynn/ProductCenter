using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InitHMAllItem
{
    public partial class InitMatiralAndColour : Form
    {
        public InitMatiralAndColour()
        {
            InitializeComponent();
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
                this.MCPath.Text = openFileDialog1.FileName;          //显示文件路径
            }

            openFileDialog1.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.MCPath.Text.Length > 1)
            {
                InitMaterialAndColorList initClass = new InitMaterialAndColorList(this.MCPath.Text);
                initClass.OperateNotify += MC_OperateNotify;//关联委托
                initClass.InitThoseList();
            }
            else
            {

                MessageBox.Show(" 请选择文件！");
            }
        }

        /// <summary>
        /// 用来在前台即时显示当前处理的文件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MC_OperateNotify(object sender, HMEventArgs e)
        {
            var ath = new AppendTextHandler(this.txtBoxResult.AppendText);
            txtBoxResult.BeginInvoke(ath, new object[] { e.Message + Environment.NewLine });
            //btnOpenMC.BeginInvoke(ath, new object[] { e.Message + Environment.NewLine });
        }

    }
}
