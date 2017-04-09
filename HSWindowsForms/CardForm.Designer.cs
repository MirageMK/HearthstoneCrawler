﻿namespace HSWindowsForms
{
    partial class CardForm
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
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn1 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn2 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn1 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn3 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn5 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn2 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn3 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn1 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn4 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn6 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            this.enumBinder1 = new Telerik.WinControls.UI.Data.EnumBinder();
            this.enumBinder2 = new Telerik.WinControls.UI.Data.EnumBinder();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.deckBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.enumBinder3 = new Telerik.WinControls.UI.Data.EnumBinder();
            this.enumBinder4 = new Telerik.WinControls.UI.Data.EnumBinder();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deckBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // enumBinder1
            // 
            this.enumBinder1.Source = typeof(HSCore.DeckType);
            gridViewComboBoxColumn1.DataSource = this.enumBinder1;
            gridViewComboBoxColumn1.DataType = typeof(HSCore.DeckType);
            gridViewComboBoxColumn1.DisplayMember = "Description";
            gridViewComboBoxColumn1.FieldName = "DeckType";
            gridViewComboBoxColumn1.HeaderText = "DeckType";
            gridViewComboBoxColumn1.IsAutoGenerated = true;
            gridViewComboBoxColumn1.Name = "DeckType";
            gridViewComboBoxColumn1.ValueMember = "Value";
            this.enumBinder1.Target = gridViewComboBoxColumn1;
            // 
            // enumBinder2
            // 
            this.enumBinder2.Source = typeof(HSCore.SourceEnum);
            gridViewComboBoxColumn2.DataSource = this.enumBinder2;
            gridViewComboBoxColumn2.DataType = typeof(HSCore.SourceEnum);
            gridViewComboBoxColumn2.DisplayMember = "Description";
            gridViewComboBoxColumn2.FieldName = "Source";
            gridViewComboBoxColumn2.HeaderText = "Source";
            gridViewComboBoxColumn2.IsAutoGenerated = true;
            gridViewComboBoxColumn2.Name = "Source";
            gridViewComboBoxColumn2.ValueMember = "Value";
            this.enumBinder2.Target = gridViewComboBoxColumn2;
            // 
            // radGridView1
            // 
            this.radGridView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.radGridView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.radGridView1.ForeColor = System.Drawing.Color.Black;
            this.radGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridView1.Location = new System.Drawing.Point(0, 0);
            // 
            // 
            // 
            gridViewTextBoxColumn1.EnableExpressionEditor = false;
            gridViewTextBoxColumn1.FieldName = "Name";
            gridViewTextBoxColumn1.HeaderText = "Name";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.MinWidth = 150;
            gridViewTextBoxColumn1.Name = "Name";
            gridViewTextBoxColumn1.Width = 150;
            gridViewTextBoxColumn2.EnableExpressionEditor = false;
            gridViewTextBoxColumn2.FieldName = "Class";
            gridViewTextBoxColumn2.HeaderText = "Class";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.MinWidth = 75;
            gridViewTextBoxColumn2.Name = "Class";
            gridViewTextBoxColumn2.Width = 75;
            gridViewDecimalColumn1.DataType = typeof(int);
            gridViewDecimalColumn1.EnableExpressionEditor = false;
            gridViewDecimalColumn1.FieldName = "Tier";
            gridViewDecimalColumn1.HeaderText = "Tier";
            gridViewDecimalColumn1.IsAutoGenerated = true;
            gridViewDecimalColumn1.MinWidth = 50;
            gridViewDecimalColumn1.Name = "Tier";
            gridViewComboBoxColumn3.DataSource = this.enumBinder3;
            gridViewComboBoxColumn3.DataType = typeof(object);
            gridViewComboBoxColumn3.DisplayMember = "Description";
            gridViewComboBoxColumn3.EnableExpressionEditor = false;
            gridViewComboBoxColumn3.FieldName = "DeckType";
            gridViewComboBoxColumn3.HeaderText = "DeckType";
            gridViewComboBoxColumn3.IsAutoGenerated = true;
            gridViewComboBoxColumn3.IsVisible = false;
            gridViewComboBoxColumn3.Name = "DeckType";
            gridViewComboBoxColumn3.ValueMember = "Value";
            gridViewComboBoxColumn5.DataSource = this.enumBinder4;
            gridViewComboBoxColumn5.DataType = typeof(object);
            gridViewComboBoxColumn5.DisplayMember = "Description";
            gridViewComboBoxColumn5.EnableExpressionEditor = false;
            gridViewComboBoxColumn5.FieldName = "Source";
            gridViewComboBoxColumn5.HeaderText = "Source";
            gridViewComboBoxColumn5.IsAutoGenerated = true;
            gridViewComboBoxColumn5.MinWidth = 100;
            gridViewComboBoxColumn5.Name = "Source";
            gridViewComboBoxColumn5.ValueMember = "Value";
            gridViewComboBoxColumn5.Width = 100;
            gridViewTextBoxColumn3.EnableExpressionEditor = false;
            gridViewTextBoxColumn3.FieldName = "Id";
            gridViewTextBoxColumn3.HeaderText = "Id";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.IsVisible = false;
            gridViewTextBoxColumn3.Name = "Id";
            gridViewTextBoxColumn3.ReadOnly = true;
            gridViewDecimalColumn2.DataType = typeof(int);
            gridViewDecimalColumn2.EnableExpressionEditor = false;
            gridViewDecimalColumn2.FieldName = "Dust";
            gridViewDecimalColumn2.HeaderText = "Dust";
            gridViewDecimalColumn2.IsAutoGenerated = true;
            gridViewDecimalColumn2.MinWidth = 50;
            gridViewDecimalColumn2.Name = "Dust";
            gridViewDecimalColumn2.ReadOnly = true;
            gridViewDecimalColumn3.DataType = typeof(int);
            gridViewDecimalColumn3.EnableExpressionEditor = false;
            gridViewDecimalColumn3.FieldName = "MyDust";
            gridViewDecimalColumn3.HeaderText = "MyDust";
            gridViewDecimalColumn3.IsAutoGenerated = true;
            gridViewDecimalColumn3.MinWidth = 50;
            gridViewDecimalColumn3.Name = "MyDust";
            gridViewDecimalColumn3.ReadOnly = true;
            gridViewDateTimeColumn1.EnableExpressionEditor = false;
            gridViewDateTimeColumn1.FieldName = "UpdateDate";
            gridViewDateTimeColumn1.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            gridViewDateTimeColumn1.HeaderText = "UpdateDate";
            gridViewDateTimeColumn1.IsAutoGenerated = true;
            gridViewDateTimeColumn1.MinWidth = 75;
            gridViewDateTimeColumn1.Name = "UpdateDate";
            gridViewDateTimeColumn1.ReadOnly = true;
            gridViewDateTimeColumn1.Width = 75;
            gridViewTextBoxColumn4.EnableExpressionEditor = false;
            gridViewTextBoxColumn4.FieldName = "DuplicateIndicatior";
            gridViewTextBoxColumn4.HeaderText = "DuplicateIndicatior";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.IsVisible = false;
            gridViewTextBoxColumn4.Name = "DuplicateIndicatior";
            gridViewTextBoxColumn4.ReadOnly = true;
            gridViewTextBoxColumn5.DataType = typeof(System.Collections.Generic.Dictionary<HSCore.Model.Card, int>);
            gridViewTextBoxColumn5.EnableExpressionEditor = false;
            gridViewTextBoxColumn5.FieldName = "Cards";
            gridViewTextBoxColumn5.HeaderText = "Cards";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.IsVisible = false;
            gridViewTextBoxColumn5.Name = "Cards";
            this.radGridView1.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewDecimalColumn1,
            gridViewComboBoxColumn3,
            gridViewComboBoxColumn5,
            gridViewTextBoxColumn3,
            gridViewDecimalColumn2,
            gridViewDecimalColumn3,
            gridViewDateTimeColumn1,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5});
            this.radGridView1.MasterTemplate.DataSource = this.deckBindingSource;
            this.radGridView1.MasterTemplate.EnableGrouping = false;
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.ReadOnly = true;
            this.radGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView1.ShowGroupPanel = false;
            this.radGridView1.Size = new System.Drawing.Size(562, 270);
            this.radGridView1.TabIndex = 0;
            this.radGridView1.Text = "radGridView1";
            // 
            // deckBindingSource
            // 
            this.deckBindingSource.DataSource = typeof(HSCore.Model.Deck);
            // 
            // enumBinder3
            // 
            this.enumBinder3.Source = typeof(HSCore.DeckType);
            gridViewComboBoxColumn4.DataSource = this.enumBinder3;
            gridViewComboBoxColumn4.DataType = typeof(HSCore.DeckType);
            gridViewComboBoxColumn4.DisplayMember = "Description";
            gridViewComboBoxColumn4.FieldName = "DeckType";
            gridViewComboBoxColumn4.HeaderText = "DeckType";
            gridViewComboBoxColumn4.IsAutoGenerated = true;
            gridViewComboBoxColumn4.Name = "DeckType";
            gridViewComboBoxColumn4.ValueMember = "Value";
            this.enumBinder3.Target = gridViewComboBoxColumn4;
            // 
            // enumBinder4
            // 
            this.enumBinder4.Source = typeof(HSCore.SourceEnum);
            gridViewComboBoxColumn6.DataSource = this.enumBinder4;
            gridViewComboBoxColumn6.DataType = typeof(HSCore.SourceEnum);
            gridViewComboBoxColumn6.DisplayMember = "Description";
            gridViewComboBoxColumn6.FieldName = "Source";
            gridViewComboBoxColumn6.HeaderText = "Source";
            gridViewComboBoxColumn6.IsAutoGenerated = true;
            gridViewComboBoxColumn6.Name = "Source";
            gridViewComboBoxColumn6.ValueMember = "Value";
            this.enumBinder4.Target = gridViewComboBoxColumn6;
            // 
            // CardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 270);
            this.Controls.Add(this.radGridView1);
            this.Name = "CardForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "CardForm";
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deckBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.Data.EnumBinder enumBinder1;
        private Telerik.WinControls.UI.Data.EnumBinder enumBinder2;
        private Telerik.WinControls.UI.Data.EnumBinder enumBinder3;
        private Telerik.WinControls.UI.Data.EnumBinder enumBinder4;
        private System.Windows.Forms.BindingSource deckBindingSource;
    }
}