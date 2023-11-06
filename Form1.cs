using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using BLL.Models;
using BLL.Services;

namespace Lab3
{
    public partial class Form1 : Form
    {
        SingersService sinService = new SingersService();
        SongsService sonService = new SongsService();
        List<LabelsDto> allLabels;
        List<SingersDto> allSingers;

        public Form1()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData()
        {
            allLabels = sinService.GetLabels();
            loadSingers();
            loadSongs();
            comboBox1.DataSource = allLabels;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Id";
        }

        private void loadSingers()
        {
            allSingers = sinService.GetAllSingers();
            bindingSourceSingers.DataSource = allSingers;
            FillLabelsCombobox();
        }

        private void loadSongs()
        {
            bindingSourceSongs.DataSource = sonService.GetAllSongs();
        }

        private void FillLabelsCombobox()
        {
            ((DataGridViewComboBoxColumn)dataGridViewSingers.Columns["label0"]).DataSource = allLabels;
            ((DataGridViewComboBoxColumn)dataGridViewSingers.Columns["label0"]).DisplayMember = "Name";
            ((DataGridViewComboBoxColumn)dataGridViewSingers.Columns["label0"]).ValueMember = "Id";
        }


        /// <summary>
        /// Сохранить изменения в таблице.
        /// </summary>
        private void buttonSaveSinger_Click(object sender, EventArgs e)
        {
            bool res = Validate();
            if (res)
            {
                //sinService.GetType();
                sinService.Save();
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.comboBox1.DataSource = allLabels;
            f.comboBox1.DisplayMember = "Name";
            f.comboBox1.ValueMember = "Id";

            DialogResult result = f.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            SingersDto singer = new SingersDto();
            singer.Label_ID = (int)f.comboBox1.SelectedValue;
            singer.Name_singer = f.textBox1.Text;
            singer.SubscriptionCost = f.numericUpDown1.Value;
            singer.Description = f.textBox2.Text;
            sinService.CreateSinger(singer);
            allSingers = sinService.GetAllSingers();
            bindingSourceSingers.DataSource = allSingers;
            MessageBox.Show("Новый исполнитель добавлен");
        }

        private int getSelectedRow(DataGridView dataGridView)
        {
            int index = -1;
            if (dataGridView.SelectedRows.Count > 0 || dataGridView.SelectedCells.Count == 1)
            {
                if (dataGridView.SelectedRows.Count > 0)
                    index = dataGridView.SelectedRows[0].Index;
                else index = dataGridView.SelectedCells[0].RowIndex;
            }
            return index;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            int index = getSelectedRow(dataGridViewSingers);
            if (index != -1)
            {
                int id = 0;
                bool converted = Int32.TryParse(dataGridViewSingers[0, index].Value.ToString(), out id);
                if (converted == false)
                    return;

                SingersDto sd = allSingers.Where(i => i.Id == id).FirstOrDefault();
                if (sd != null)
                {
                    Form2 f = new Form2();
                    f.comboBox1.DataSource = allLabels;
                    f.comboBox1.DisplayMember = "Name";
                    f.comboBox1.ValueMember = "Id";
                    f.numericUpDown1.Value = sd.SubscriptionCost;
                    f.comboBox1.SelectedValue = sd.Label_ID;
                    f.textBox1.Text = sd.Name_singer;
                    f.textBox2.Text = sd.Description;

                    DialogResult result = f.ShowDialog(this);

                    if (result == DialogResult.Cancel)
                        return;

                    sd.SubscriptionCost = f.numericUpDown1.Value;
                    sd.Name_singer = f.textBox1.Text;
                    sd.Description = f.textBox2.Text;
                    sd.Label_ID = (int)f.comboBox1.SelectedValue;
                    sinService.UpdateSinger(sd);

                    MessageBox.Show("Объект обновлен");
                    bindingSourceSingers.DataSource = sinService.GetAllSingers();
                }
            }
            else
                MessageBox.Show("Ни один объект не выбран!");
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = getSelectedRow(dataGridViewSingers);
            if (index != -1)
            {
                int id = 0;
                bool converted = Int32.TryParse(dataGridViewSingers[0, index].Value.ToString(), out id);
                if (converted == false)
                    return;
                sinService.DeleteSinger(id);
                bindingSourceSingers.DataSource = sinService.GetAllSingers();
            }
        }

        private void buttonMakeSong_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            if (f3.ShowDialog() == DialogResult.OK)
                bindingSourceSongs.DataSource = sonService.GetAllSongs();
        }

        private void buttonGetReport1_Click(object sender, EventArgs e)
        {
            dataGridViewReport1.DataSource = ReportsService.Report((int)comboBox1.SelectedValue);
        }

        private class SPResult
        {
            public string Song { get; set; }
            public string SingerName { get; set; }
            public DateTime Date { get; set; }
        }

        private void buttonReport2_Click(object sender, EventArgs e)
        {
            dataGridViewReport2.DataSource = ReportsService.GetReportSongs((int)numericUpDown1.Value, (int)numericUpDown2.Value);
        }
    }
}
