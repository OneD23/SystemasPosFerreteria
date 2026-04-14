namespace SistemaFerreteriaV8
{
    partial class VentanaRegistroCaja
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VentanaRegistroCaja));
            label1 = new Label();
            label2 = new Label();
            Balance = new TextBox();
            Aceptar = new Button();
            Cancelar = new Button();
            Codigo = new TextBox();
            label3 = new Label();
            label4 = new Label();
            turno = new ComboBox();
            Aviso = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(147, 32);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(224, 24);
            label1.TabIndex = 0;
            label1.Text = "Registrar apertura de caja";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(63, 201);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(126, 20);
            label2.TabIndex = 1;
            label2.Text = "Balance en caja:";
            // 
            // Balance
            // 
            Balance.HideSelection = false;
            Balance.Location = new Point(217, 203);
            Balance.Margin = new Padding(4, 3, 4, 3);
            Balance.Name = "Balance";
            Balance.Size = new Size(198, 23);
            Balance.TabIndex = 2;
            // 
            // Aceptar
            // 
            Aceptar.BackColor = Color.Red;
            Aceptar.FlatStyle = FlatStyle.Flat;
            Aceptar.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Aceptar.ForeColor = Color.White;
            Aceptar.Location = new Point(295, 335);
            Aceptar.Margin = new Padding(4, 3, 4, 3);
            Aceptar.Name = "Aceptar";
            Aceptar.Size = new Size(113, 47);
            Aceptar.TabIndex = 3;
            Aceptar.Text = "Aceptar";
            Aceptar.UseVisualStyleBackColor = false;
            Aceptar.Click += Aceptar_Click;
            // 
            // Cancelar
            // 
            Cancelar.BackColor = Color.Red;
            Cancelar.FlatStyle = FlatStyle.Flat;
            Cancelar.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Cancelar.ForeColor = Color.White;
            Cancelar.Location = new Point(163, 335);
            Cancelar.Margin = new Padding(4, 3, 4, 3);
            Cancelar.Name = "Cancelar";
            Cancelar.Size = new Size(113, 47);
            Cancelar.TabIndex = 4;
            Cancelar.Text = "Cancelar";
            Cancelar.UseVisualStyleBackColor = false;
            // 
            // Codigo
            // 
            Codigo.Location = new Point(217, 156);
            Codigo.Margin = new Padding(4, 3, 4, 3);
            Codigo.Name = "Codigo";
            Codigo.Size = new Size(198, 23);
            Codigo.TabIndex = 6;
            Codigo.KeyPress += Codigo_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(26, 153);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(164, 20);
            label3.TabIndex = 5;
            label3.Text = "Codigo del Empleado:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.White;
            label4.Location = new Point(147, 243);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(54, 20);
            label4.TabIndex = 7;
            label4.Text = "Turno:";
            // 
            // turno
            // 
            turno.FormattingEnabled = true;
            turno.Items.AddRange(new object[] { "Matutino.", "vespestito." });
            turno.Location = new Point(217, 246);
            turno.Margin = new Padding(4, 3, 4, 3);
            turno.Name = "turno";
            turno.Size = new Size(198, 23);
            turno.TabIndex = 9;
            // 
            // Aviso
            // 
            Aviso.AutoSize = true;
            Aviso.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Aviso.ForeColor = Color.Red;
            Aviso.Location = new Point(149, 114);
            Aviso.Margin = new Padding(4, 0, 4, 0);
            Aviso.Name = "Aviso";
            Aviso.Size = new Size(228, 24);
            Aviso.TabIndex = 10;
            Aviso.Text = "Ya existe una caja abierta!";
            // 
            // VentanaRegistroCaja
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(572, 471);
            Controls.Add(Aviso);
            Controls.Add(turno);
            Controls.Add(label4);
            Controls.Add(Codigo);
            Controls.Add(label3);
            Controls.Add(Cancelar);
            Controls.Add(Aceptar);
            Controls.Add(Balance);
            Controls.Add(label2);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "VentanaRegistroCaja";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Apertura de caja";
            Load += VentanaRegistroCaja_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Balance;
        private System.Windows.Forms.Button Aceptar;
        private System.Windows.Forms.Button Cancelar;
        private System.Windows.Forms.TextBox Codigo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox turno;
        private System.Windows.Forms.Label Aviso;
    }
}