namespace _7DtD_LootViewer
{
    partial class LootViewer
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
            this.T_XMLFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.B_Load = new System.Windows.Forms.Button();
            this.M_Output = new System.Windows.Forms.TextBox();
            this.V_LContainer = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gb_Search = new System.Windows.Forms.GroupBox();
            this.r_LC = new System.Windows.Forms.RadioButton();
            this.r_Item = new System.Windows.Forms.RadioButton();
            this.gb_Search.SuspendLayout();
            this.SuspendLayout();
            // 
            // T_XMLFile
            // 
            this.T_XMLFile.Location = new System.Drawing.Point(12, 36);
            this.T_XMLFile.Name = "T_XMLFile";
            this.T_XMLFile.Size = new System.Drawing.Size(377, 20);
            this.T_XMLFile.TabIndex = 0;
            this.T_XMLFile.Text = "E:\\Steam\\steamapps\\common\\7 Days To Die\\Data\\Config\\loot.xml";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loot File:";
            // 
            // B_Load
            // 
            this.B_Load.Location = new System.Drawing.Point(395, 34);
            this.B_Load.Name = "B_Load";
            this.B_Load.Size = new System.Drawing.Size(75, 23);
            this.B_Load.TabIndex = 2;
            this.B_Load.Text = "Load";
            this.B_Load.UseVisualStyleBackColor = true;
            this.B_Load.Click += new System.EventHandler(this.B_Load_Click);
            // 
            // M_Output
            // 
            this.M_Output.Location = new System.Drawing.Point(15, 233);
            this.M_Output.Multiline = true;
            this.M_Output.Name = "M_Output";
            this.M_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.M_Output.Size = new System.Drawing.Size(663, 239);
            this.M_Output.TabIndex = 3;
            // 
            // V_LContainer
            // 
            this.V_LContainer.Enabled = false;
            this.V_LContainer.FormattingEnabled = true;
            this.V_LContainer.Location = new System.Drawing.Point(99, 206);
            this.V_LContainer.Name = "V_LContainer";
            this.V_LContainer.Size = new System.Drawing.Size(121, 21);
            this.V_LContainer.TabIndex = 4;
            this.V_LContainer.SelectionChangeCommitted += new System.EventHandler(this.V_LContainer_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "LootContainer:";
            // 
            // gb_Search
            // 
            this.gb_Search.Controls.Add(this.r_Item);
            this.gb_Search.Controls.Add(this.r_LC);
            this.gb_Search.Location = new System.Drawing.Point(20, 78);
            this.gb_Search.Name = "gb_Search";
            this.gb_Search.Size = new System.Drawing.Size(327, 36);
            this.gb_Search.TabIndex = 6;
            this.gb_Search.TabStop = false;
            this.gb_Search.Text = "Search Type:";
            // 
            // r_LC
            // 
            this.r_LC.AutoSize = true;
            this.r_LC.Checked = true;
            this.r_LC.Location = new System.Drawing.Point(25, 13);
            this.r_LC.Name = "r_LC";
            this.r_LC.Size = new System.Drawing.Size(108, 17);
            this.r_LC.TabIndex = 0;
            this.r_LC.TabStop = true;
            this.r_LC.Text = "Loot Container ID";
            this.r_LC.UseVisualStyleBackColor = true;
            this.r_LC.CheckedChanged += new System.EventHandler(this.r_LC_CheckedChanged);
            // 
            // r_Item
            // 
            this.r_Item.AutoSize = true;
            this.r_Item.Location = new System.Drawing.Point(139, 13);
            this.r_Item.Name = "r_Item";
            this.r_Item.Size = new System.Drawing.Size(76, 17);
            this.r_Item.TabIndex = 1;
            this.r_Item.TabStop = true;
            this.r_Item.Text = "Item Name";
            this.r_Item.UseVisualStyleBackColor = true;
            this.r_Item.CheckedChanged += new System.EventHandler(this.r_Item_CheckedChanged);
            // 
            // LootViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 484);
            this.Controls.Add(this.gb_Search);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.V_LContainer);
            this.Controls.Add(this.M_Output);
            this.Controls.Add(this.B_Load);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.T_XMLFile);
            this.Name = "LootViewer";
            this.Text = "Loot Viewer";
            this.gb_Search.ResumeLayout(false);
            this.gb_Search.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox T_XMLFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button B_Load;
        private System.Windows.Forms.TextBox M_Output;
        private System.Windows.Forms.ComboBox V_LContainer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gb_Search;
        private System.Windows.Forms.RadioButton r_Item;
        private System.Windows.Forms.RadioButton r_LC;
    }
}

