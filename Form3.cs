using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL.Models;
using BLL.Services;

namespace Lab3
{
    public partial class Form3 : Form
    {
        SingersService sb = new SingersService();
        public Form3()
        {
            InitializeComponent();
            ListBoxSingersOnSong.DataSource = sb.GetAllSingers();
            ListBoxSingersOnSong.DisplayMember = "Name_singer";
            ListBoxSingersOnSong.ValueMember = "Id";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ListBoxSingersOnSong.CheckedItems.Count == 0)
            {
                MessageBox.Show("Не выбран ни один исполнитель");
                return;
            }
            List<int> items = new List<int>();
            foreach(var i in ListBoxSingersOnSong.CheckedItems)
                items.Add((i as SingersDto).Id);

            SongsDto song = new SongsDto
            {
                Song = tbSong.Text,
                Singer = tbSinger.Text,
                Date = DateTime.Parse(tbDate.Text),
                SongPerformersIds = items,
            };

            SongsService service = new SongsService();
            bool result = service.MakeSongs(song);
            if (result)
            {
                MessageBox.Show("Успешно!");
            }
            else
            {
                MessageBox.Show("Failed!");
            }

        }
    }
}
