namespace CarManagementApp
{
    partial class CarEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.TextBox txtBrand;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown numYear;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.NumericUpDown numPower;
        private System.Windows.Forms.Label lblCost;
        private System.Windows.Forms.NumericUpDown numCost;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        //изменения 11.03.2025
        private System.Windows.Forms.Label lblClientName;
        private System.Windows.Forms.TextBox txtClientName;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label lblIssue;
        private System.Windows.Forms.TextBox txtIssue;

        // Дополнительные поля для расширенной формы
        private System.Windows.Forms.Label lblFullCarInfo;
        private System.Windows.Forms.TextBox txtFullCarInfo;
        private System.Windows.Forms.Label lblDiagnosis;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.Label lblResponsiblePerson;
        private System.Windows.Forms.TextBox txtResponsiblePerson;
        private System.Windows.Forms.Label lblRepairPerson;
        private System.Windows.Forms.TextBox txtRepairPerson;
        private System.Windows.Forms.Label lblRepairStart;
        private System.Windows.Forms.DateTimePicker dtpRepairStart;
        private System.Windows.Forms.Label lblRepairEnd;
        private System.Windows.Forms.DateTimePicker dtpRepairEnd;
        //
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblBrand = new System.Windows.Forms.Label();
            this.txtBrand = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.numYear = new System.Windows.Forms.NumericUpDown();
            this.lblPower = new System.Windows.Forms.Label();
            this.numPower = new System.Windows.Forms.NumericUpDown();
            this.lblCost = new System.Windows.Forms.Label();
            this.numCost = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            // Инициализация новых компонентов
            this.lblClientName = new System.Windows.Forms.Label();
            this.txtClientName = new System.Windows.Forms.TextBox();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.lblIssue = new System.Windows.Forms.Label();
            this.txtIssue = new System.Windows.Forms.TextBox();
            this.lblFullCarInfo = new System.Windows.Forms.Label();
            this.txtFullCarInfo = new System.Windows.Forms.TextBox();
            this.lblDiagnosis = new System.Windows.Forms.Label();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.lblResponsiblePerson = new System.Windows.Forms.Label();
            this.txtResponsiblePerson = new System.Windows.Forms.TextBox();
            this.lblRepairPerson = new System.Windows.Forms.Label();
            this.txtRepairPerson = new System.Windows.Forms.TextBox();
            this.lblRepairStart = new System.Windows.Forms.Label();
            this.dtpRepairStart = new System.Windows.Forms.DateTimePicker();
            this.lblRepairEnd = new System.Windows.Forms.Label();
            this.dtpRepairEnd = new System.Windows.Forms.DateTimePicker();

            ((System.ComponentModel.ISupportInitialize)(this.numYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCost)).BeginInit();
            this.SuspendLayout();
            // 
            // lblClientName
            // 
            this.lblClientName.Location = new System.Drawing.Point(12, 15);
            this.lblClientName.Name = "lblClientName";
            this.lblClientName.Size = new System.Drawing.Size(120, 23);
            this.lblClientName.Text = "ФИО владельца:";
            // 
            // txtClientName
            // 
            this.txtClientName.Location = new System.Drawing.Point(150, 12);
            this.txtClientName.Name = "txtClientName";
            this.txtClientName.Size = new System.Drawing.Size(250, 20);
            // 
            // lblBrand
            // 
            this.lblBrand.Location = new System.Drawing.Point(12, 45);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(120, 23);
            this.lblBrand.Text = "Марка автомобиля:";
            // 
            // txtBrand
            // 
            this.txtBrand.Location = new System.Drawing.Point(150, 42);
            this.txtBrand.Name = "txtBrand";
            this.txtBrand.Size = new System.Drawing.Size(250, 20);
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.Location = new System.Drawing.Point(12, 75);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(120, 23);
            this.lblPhoneNumber.Text = "Контактный телефон:";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Location = new System.Drawing.Point(150, 72);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(250, 20);
            // 
            // lblIssue
            // 
            this.lblIssue.Location = new System.Drawing.Point(12, 105);
            this.lblIssue.Name = "lblIssue";
            this.lblIssue.Size = new System.Drawing.Size(120, 23);
            this.lblIssue.Text = "Неисправность:";
            // 
            // txtIssue
            // 
            this.txtIssue.Location = new System.Drawing.Point(150, 102);
            this.txtIssue.Multiline = true;
            this.txtIssue.Name = "txtIssue";
            this.txtIssue.Size = new System.Drawing.Size(250, 60);
            // 
            // lblModel
            // 
            this.lblModel.Location = new System.Drawing.Point(12, 175);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(120, 23);
            this.lblModel.Text = "Модель:";
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(150, 172);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(250, 20);
            // 
            // lblYear
            // 
            this.lblYear.Location = new System.Drawing.Point(12, 205);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(120, 23);
            this.lblYear.Text = "Год выпуска:";
            // 
            // numYear
            // 
            this.numYear.Location = new System.Drawing.Point(150, 202);
            this.numYear.Minimum = new decimal(new int[] { 1900, 0, 0, 0 });
            this.numYear.Maximum = new decimal(new int[] { DateTime.Now.Year, 0, 0, 0 });
            this.numYear.Name = "numYear";
            this.numYear.Size = new System.Drawing.Size(120, 20);
            this.numYear.Value = new decimal(new int[] { DateTime.Now.Year, 0, 0, 0 });
            // 
            // lblPower
            // 
            this.lblPower.Location = new System.Drawing.Point(12, 235);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(120, 23);
            this.lblPower.Text = "Мощность (л.с.):";
            // 
            // numPower
            // 
            this.numPower.Location = new System.Drawing.Point(150, 232);
            this.numPower.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numPower.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numPower.Name = "numPower";
            this.numPower.Size = new System.Drawing.Size(120, 20);
            this.numPower.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // lblCost
            // 
            this.lblCost.Location = new System.Drawing.Point(12, 265);
            this.lblCost.Name = "lblCost";
            this.lblCost.Size = new System.Drawing.Size(120, 23);
            this.lblCost.Text = "Стоимость работ:";
            // 
            // numCost
            // 
            this.numCost.Location = new System.Drawing.Point(150, 262);
            this.numCost.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            this.numCost.Name = "numCost";
            this.numCost.Size = new System.Drawing.Size(120, 20);
            this.numCost.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblFullCarInfo
            // 
            this.lblFullCarInfo.Location = new System.Drawing.Point(12, 295);
            this.lblFullCarInfo.Name = "lblFullCarInfo";
            this.lblFullCarInfo.Size = new System.Drawing.Size(120, 23);
            this.lblFullCarInfo.Text = "Инфо об авто:";
            // 
            // txtFullCarInfo
            // 
            this.txtFullCarInfo.Location = new System.Drawing.Point(150, 292);
            this.txtFullCarInfo.Name = "txtFullCarInfo";
            this.txtFullCarInfo.Size = new System.Drawing.Size(250, 20);
            // 
            // lblDiagnosis
            // 
            this.lblDiagnosis.Location = new System.Drawing.Point(12, 325);
            this.lblDiagnosis.Name = "lblDiagnosis";
            this.lblDiagnosis.Size = new System.Drawing.Size(120, 23);
            this.lblDiagnosis.Text = "Диагноз:";
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(150, 322);
            this.txtDiagnosis.Multiline = true;
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(250, 60);
            // 
            // lblResponsiblePerson
            // 
            this.lblResponsiblePerson.Location = new System.Drawing.Point(12, 395);
            this.lblResponsiblePerson.Name = "lblResponsiblePerson";
            this.lblResponsiblePerson.Size = new System.Drawing.Size(120, 23);
            this.lblResponsiblePerson.Text = "Ответственное лицо:";
            // 
            // txtResponsiblePerson
            // 
            this.txtResponsiblePerson.Location = new System.Drawing.Point(150, 392);
            this.txtResponsiblePerson.Name = "txtResponsiblePerson";
            this.txtResponsiblePerson.Size = new System.Drawing.Size(250, 20);
            // 
            // lblRepairPerson
            // 
            this.lblRepairPerson.Location = new System.Drawing.Point(12, 425);
            this.lblRepairPerson.Name = "lblRepairPerson";
            this.lblRepairPerson.Size = new System.Drawing.Size(120, 23);
            this.lblRepairPerson.Text = "Исполнитель:";
            // 
            // txtRepairPerson
            // 
            this.txtRepairPerson.Location = new System.Drawing.Point(150, 422);
            this.txtRepairPerson.Name = "txtRepairPerson";
            this.txtRepairPerson.Size = new System.Drawing.Size(250, 20);
            // 
            // lblRepairStart
            // 
            this.lblRepairStart.Location = new System.Drawing.Point(12, 455);
            this.lblRepairStart.Name = "lblRepairStart";
            this.lblRepairStart.Size = new System.Drawing.Size(120, 23);
            this.lblRepairStart.Text = "Начало ремонта:";
            // 
            // dtpRepairStart
            // 
            this.dtpRepairStart.Location = new System.Drawing.Point(150, 452);
            this.dtpRepairStart.Name = "dtpRepairStart";
            this.dtpRepairStart.Size = new System.Drawing.Size(200, 20);
            // 
            // lblRepairEnd
            // 
            this.lblRepairEnd.Location = new System.Drawing.Point(12, 485);
            this.lblRepairEnd.Name = "lblRepairEnd";
            this.lblRepairEnd.Size = new System.Drawing.Size(120, 23);
            this.lblRepairEnd.Text = "Окончание ремонта:";
            // 
            // dtpRepairEnd
            // 
            this.dtpRepairEnd.Location = new System.Drawing.Point(150, 482);
            this.dtpRepairEnd.Name = "dtpRepairEnd";
            this.dtpRepairEnd.Size = new System.Drawing.Size(200, 20);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(150, 522);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Сохранить";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(300, 522);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 30);
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CarEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 570);
            this.Controls.Add(this.lblClientName);
            this.Controls.Add(this.txtClientName);
            this.Controls.Add(this.lblBrand);
            this.Controls.Add(this.txtBrand);
            this.Controls.Add(this.lblPhoneNumber);
            this.Controls.Add(this.txtPhoneNumber);
            this.Controls.Add(this.lblIssue);
            this.Controls.Add(this.txtIssue);
            this.Controls.Add(this.lblModel);
            this.Controls.Add(this.txtModel);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.numYear);
            this.Controls.Add(this.lblPower);
            this.Controls.Add(this.numPower);
            this.Controls.Add(this.lblCost);
            this.Controls.Add(this.numCost);
            this.Controls.Add(this.lblFullCarInfo);
            this.Controls.Add(this.txtFullCarInfo);
            this.Controls.Add(this.lblDiagnosis);
            this.Controls.Add(this.txtDiagnosis);
            this.Controls.Add(this.lblResponsiblePerson);
            this.Controls.Add(this.txtResponsiblePerson);
            this.Controls.Add(this.lblRepairPerson);
            this.Controls.Add(this.txtRepairPerson);
            this.Controls.Add(this.lblRepairStart);
            this.Controls.Add(this.dtpRepairStart);
            this.Controls.Add(this.lblRepairEnd);
            this.Controls.Add(this.dtpRepairEnd);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CarEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактирование автомобиля";
            ((System.ComponentModel.ISupportInitialize)(this.numYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
    }
}
