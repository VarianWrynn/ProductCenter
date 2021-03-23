namespace InitHMAllItem
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnBrowse = new System.Windows.Forms.Button();
            this.basicHMPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.MCScreen = new System.Windows.Forms.TextBox();
            this.btnMaterial = new System.Windows.Forms.Button();
            this.btnColour = new System.Windows.Forms.Button();
            this.groupHMPath = new System.Windows.Forms.TextBox();
            this.btnBrowseGroup = new System.Windows.Forms.Button();
            this.OpenMC = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(170, 131);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // basicHMPath
            // 
            this.basicHMPath.Enabled = false;
            this.basicHMPath.Location = new System.Drawing.Point(103, 19);
            this.basicHMPath.Name = "basicHMPath";
            this.basicHMPath.Size = new System.Drawing.Size(508, 21);
            this.basicHMPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "基础HM文件";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(502, 131);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // MCScreen
            // 
            this.MCScreen.Location = new System.Drawing.Point(116, 179);
            this.MCScreen.Multiline = true;
            this.MCScreen.Name = "MCScreen";
            this.MCScreen.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.MCScreen.Size = new System.Drawing.Size(508, 362);
            this.MCScreen.TabIndex = 4;
            this.MCScreen.TextChanged += new System.EventHandler(this.MCScreen_TextChanged);
            // 
            // btnMaterial
            // 
            this.btnMaterial.Enabled = false;
            this.btnMaterial.Location = new System.Drawing.Point(12, 347);
            this.btnMaterial.Name = "btnMaterial";
            this.btnMaterial.Size = new System.Drawing.Size(75, 46);
            this.btnMaterial.TabIndex = 5;
            this.btnMaterial.Text = "提取Material";
            this.btnMaterial.UseMnemonic = false;
            this.btnMaterial.UseVisualStyleBackColor = true;
            this.btnMaterial.Click += new System.EventHandler(this.btnMaterial_Click);
            // 
            // btnColour
            // 
            this.btnColour.Enabled = false;
            this.btnColour.Location = new System.Drawing.Point(12, 456);
            this.btnColour.Name = "btnColour";
            this.btnColour.Size = new System.Drawing.Size(75, 46);
            this.btnColour.TabIndex = 6;
            this.btnColour.Text = "提取颜色";
            this.btnColour.UseMnemonic = false;
            this.btnColour.UseVisualStyleBackColor = true;
            this.btnColour.Click += new System.EventHandler(this.btnColour_Click);
            // 
            // groupHMPath
            // 
            this.groupHMPath.Enabled = false;
            this.groupHMPath.Location = new System.Drawing.Point(103, 71);
            this.groupHMPath.Name = "groupHMPath";
            this.groupHMPath.Size = new System.Drawing.Size(508, 21);
            this.groupHMPath.TabIndex = 7;
            // 
            // btnBrowseGroup
            // 
            this.btnBrowseGroup.Location = new System.Drawing.Point(294, 131);
            this.btnBrowseGroup.Name = "btnBrowseGroup";
            this.btnBrowseGroup.Size = new System.Drawing.Size(104, 23);
            this.btnBrowseGroup.TabIndex = 8;
            this.btnBrowseGroup.Text = "BrowseHMGroup";
            this.btnBrowseGroup.UseVisualStyleBackColor = true;
            this.btnBrowseGroup.Click += new System.EventHandler(this.btnBrowseGroup_Click);
            // 
            // OpenMC
            // 
            this.OpenMC.Location = new System.Drawing.Point(14, 204);
            this.OpenMC.Name = "OpenMC";
            this.OpenMC.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.OpenMC.Size = new System.Drawing.Size(75, 46);
            this.OpenMC.TabIndex = 9;
            this.OpenMC.Text = "初始化颜色材料列表";
            this.OpenMC.UseMnemonic = false;
            this.OpenMC.UseVisualStyleBackColor = true;
            this.OpenMC.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 587);
            this.Controls.Add(this.OpenMC);
            this.Controls.Add(this.btnBrowseGroup);
            this.Controls.Add(this.groupHMPath);
            this.Controls.Add(this.btnColour);
            this.Controls.Add(this.btnMaterial);
            this.Controls.Add(this.MCScreen);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.basicHMPath);
            this.Controls.Add(this.btnBrowse);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox basicHMPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox MCScreen;
        private System.Windows.Forms.Button btnMaterial;
        private System.Windows.Forms.Button btnColour;
        private System.Windows.Forms.TextBox groupHMPath;
        private System.Windows.Forms.Button btnBrowseGroup;
        private System.Windows.Forms.Button OpenMC;

        private delegate void AppendTextHandler(string content);//委托，更新文本框 
    }
}

