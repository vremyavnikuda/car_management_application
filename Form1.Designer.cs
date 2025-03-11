﻿namespace CarManagementApp
{
    partial class Form1
    {
        /// <summary>
        /// Объект, содержащий компоненты формы.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Панель верхней части формы, содержащая элементы управления.
        /// </summary>
        private System.Windows.Forms.Panel panelTop;

        /// <summary>
        /// Метка для подписи поля поиска.
        /// </summary>
        private System.Windows.Forms.Label lblSearch;

        /// <summary>
        /// Текстовое поле для ввода поискового запроса.
        /// </summary>
        private System.Windows.Forms.TextBox txtSearch;

        /// <summary>
        /// Метка для подписи комбобокса сортировки.
        /// </summary>
        private System.Windows.Forms.Label lblSort;

        /// <summary>
        /// Комбобокс для выбора параметра сортировки.
        /// </summary>
        private System.Windows.Forms.ComboBox cmbSort;

        /// <summary>
        /// Кнопка для добавления нового автомобиля.
        /// </summary>
        private System.Windows.Forms.Button btnAdd;

        /// <summary>
        /// Кнопка для редактирования выбранного автомобиля.
        /// </summary>
        private System.Windows.Forms.Button btnEdit;

        /// <summary>
        /// Кнопка для удаления выбранного автомобиля.
        /// </summary>
        private System.Windows.Forms.Button btnDelete;

        /// <summary>
        /// Кнопка для импорта данных в формате JSON.
        /// </summary>
        private System.Windows.Forms.Button btnImportJson;

        /// <summary>
        /// Кнопка для экспорта данных в формате JSON.
        /// </summary>
        private System.Windows.Forms.Button btnExportJson;

        /// <summary>
        /// Кнопка для импорта данных в формате XML.
        /// </summary>
        private System.Windows.Forms.Button btnImportXml;

        /// <summary>
        /// Кнопка для экспорта данных в формате XML.
        /// </summary>
        private System.Windows.Forms.Button btnExportXml;

        /// <summary>
        /// Кнопка для генерации отчёта.
        /// </summary>
        private System.Windows.Forms.Button btnGenerateReport;

        /// <summary>
        /// DataGridView для отображения списка автомобилей.
        /// </summary>
        private System.Windows.Forms.DataGridView dgvCars;

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoMenuItem;


        /// <summary>
        /// Освобождает все используемые ресурсы, как управляемые, так и неуправляемые.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> для освобождения управляемых и неуправляемых ресурсов; 
        /// <c>false</c> для освобождения только неуправляемых ресурсов.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Не меняй его ЭТО СУПЕР ВАЖНО
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvCars = new System.Windows.Forms.DataGridView();

            this.contextMenu = new System.Windows.Forms.ContextMenuStrip();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoMenuItem = new System.Windows.Forms.ToolStripMenuItem();


            //
            // contextMenu
            //
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.editMenuItem,
                this.deleteMenuItem,
                this.infoMenuItem});
            //
            // editMenuItem
            //
            this.editMenuItem.Text = "Редактировать";
            this.editMenuItem.Click += new System.EventHandler(this.editMenuItem_Click);
            //
            // deleteMenuItem
            //
            this.deleteMenuItem.Text = "Удалить";
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            //
            // infoMenuItem
            //
            this.infoMenuItem.Text = "Информация";
            this.infoMenuItem.Click += new System.EventHandler(this.infoMenuItem_Click);

            //
            // dgvCars
            //
            this.dgvCars.ContextMenuStrip = this.contextMenu;


            this.panelTop = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSort = new System.Windows.Forms.Label();
            this.cmbSort = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnImportJson = new System.Windows.Forms.Button();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.btnImportXml = new System.Windows.Forms.Button();
            this.btnExportXml = new System.Windows.Forms.Button();
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCars)).BeginInit();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblSearch);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Controls.Add(this.lblSort);
            this.panelTop.Controls.Add(this.cmbSort);
            this.panelTop.Controls.Add(this.btnAdd);
            this.panelTop.Controls.Add(this.btnEdit);
            this.panelTop.Controls.Add(this.btnDelete);
            this.panelTop.Controls.Add(this.btnImportJson);
            this.panelTop.Controls.Add(this.btnExportJson);
            this.panelTop.Controls.Add(this.btnImportXml);
            this.panelTop.Controls.Add(this.btnExportXml);
            this.panelTop.Controls.Add(this.btnGenerateReport);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.TabIndex = 0;
            //
            // lblSearch
            //
            this.lblSearch.Location = new System.Drawing.Point(10, 10);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(50, 20);
            this.lblSearch.Text = "Поиск:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(65, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(150, 20);
            this.txtSearch.TabIndex = 1;
            //
            // lblSort
            //
            this.lblSort.Location = new System.Drawing.Point(230, 10);
            this.lblSort.Name = "lblSort";
            this.lblSort.Size = new System.Drawing.Size(50, 20);
            this.lblSort.Text = "Сортировка:";
            //
            // cmbSort
            //
            this.cmbSort.Location = new System.Drawing.Point(285, 10);
            this.cmbSort.Name = "cmbSort";
            this.cmbSort.Size = new System.Drawing.Size(120, 21);
            this.cmbSort.TabIndex = 2;
            //
            // btnAdd
            //
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdd.Location = new System.Drawing.Point(10, 35);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 23);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Создать";
            this.btnAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAdd.UseVisualStyleBackColor = true;
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(110, 35);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(90, 23);
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.TabIndex = 5;
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(210, 35);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 23);
            this.btnDelete.Text = "Удалить";
            this.btnDelete.TabIndex = 6;
            //
            // btnImportJson
            //
            this.btnImportJson.Location = new System.Drawing.Point(310, 35);
            this.btnImportJson.Name = "btnImportJson";
            this.btnImportJson.Size = new System.Drawing.Size(90, 23);
            this.btnImportJson.Text = "Импорт JSON";
            this.btnImportJson.TabIndex = 7;
            //
            // btnExportJson
            //
            this.btnExportJson.Location = new System.Drawing.Point(410, 35);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(90, 23);
            this.btnExportJson.Text = "Экспорт JSON";
            this.btnExportJson.TabIndex = 8;
            //
            // btnImportXml
            //
            this.btnImportXml.Location = new System.Drawing.Point(510, 35);
            this.btnImportXml.Name = "btnImportXml";
            this.btnImportXml.Size = new System.Drawing.Size(90, 23);
            this.btnImportXml.Text = "Импорт XML";
            this.btnImportXml.TabIndex = 9;
            //
            // btnExportXml
            //
            this.btnExportXml.Location = new System.Drawing.Point(610, 35);
            this.btnExportXml.Name = "btnExportXml";
            this.btnExportXml.Size = new System.Drawing.Size(90, 23);
            this.btnExportXml.Text = "Экспорт XML";
            this.btnExportXml.TabIndex = 10;
            //
            // btnGenerateReport
            //
            this.btnGenerateReport.Location = new System.Drawing.Point(710, 35);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(90, 23);
            this.btnGenerateReport.Text = "Сгенерировать отчет";
            this.btnGenerateReport.TabIndex = 11;
            //
            // dgvCars
            //
            this.dgvCars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCars.Location = new System.Drawing.Point(0, 60);
            this.dgvCars.Name = "dgvCars";
            this.dgvCars.Size = new System.Drawing.Size(800, 390);
            this.dgvCars.TabIndex = 12;
            this.dgvCars.Columns.Add("ClientName", "ФИО");
            this.dgvCars.Columns.Add("Brand", "Марка");
            this.dgvCars.Columns.Add("CreationDate", "Дата создания");
            this.dgvCars.Columns.Add("RepairStartDate", "Дата начала ремонта");
            this.dgvCars.Columns.Add("RepairEndDate", "Дата окончания ремонта");
            this.dgvCars.Columns.Add("Cost", "Стоимость");
            //
            // Form1
            //
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvCars);
            this.Controls.Add(this.panelTop);
            this.Name = "Form1";
            this.Text = "Car Management App";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCars)).EndInit();
            this.ResumeLayout(false);

            // Set all buttons to use the same font as btnEdit
            System.Drawing.Font defaultButtonFont = this.btnEdit.Font;
            this.btnAdd.Font = defaultButtonFont;
            this.btnDelete.Font = defaultButtonFont;
            this.btnImportJson.Font = defaultButtonFont;
            this.btnExportJson.Font = defaultButtonFont;
            this.btnImportXml.Font = defaultButtonFont;
            this.btnExportXml.Font = defaultButtonFont;
            this.btnGenerateReport.Font = defaultButtonFont;
        }

        #endregion
    }
}
