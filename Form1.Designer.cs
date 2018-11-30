namespace S7SiemensManager
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.CbActions = new System.Windows.Forms.ComboBox();
            this.BtnGo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CbActions
            // 
            this.CbActions.FormattingEnabled = true;
            this.CbActions.Items.AddRange(new object[] {
            "Dbs comments cleaner",
            "Dbs motors cleaner",
            "GenlistMotors",
            "Symbolics table importation ",
            "Material checker with i/o symbolics                   80%",
            "Action1",
            "Action22",
            "LoloUseParams"});
            this.CbActions.Location = new System.Drawing.Point(12, 12);
            this.CbActions.Name = "CbActions";
            this.CbActions.Size = new System.Drawing.Size(634, 24);
            this.CbActions.TabIndex = 2;
            this.CbActions.Text = "Select me !!xx";
            // 
            // BtnGo
            // 
            this.BtnGo.Location = new System.Drawing.Point(652, 12);
            this.BtnGo.Name = "BtnGo";
            this.BtnGo.Size = new System.Drawing.Size(136, 44);
            this.BtnGo.TabIndex = 3;
            this.BtnGo.Text = "GO";
            this.BtnGo.UseVisualStyleBackColor = true;
            this.BtnGo.Click += new System.EventHandler(this.BtnGo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnGo);
            this.Controls.Add(this.CbActions);
            this.Name = "Form1";
            this.Text = "loloS7Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox CbActions;
        private System.Windows.Forms.Button BtnGo;
    }
}

