using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace SteamUI
{
    public partial class TagFilterHolder : Form
    {
        public TagFilterHolder()
        {
            InitializeComponent();

        }

        private void steamIdTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            ClearTextBox();
        }

        private void steamIdTextBox_Leave(object sender, EventArgs e)
        {
            ClearTextBox();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            ClearTextBox();
        }

        private void ClearTextBox()
        {
            switch (steamIdTextBox.Text)
            {
                case "Enter SteamID...":
                    steamIdTextBox.Text = "";
                    break;
                case "":
                    steamIdTextBox.Text = "Enter SteamID...";
                    break;
            }
        }

        private int _combinedRank = 0;
        private void SearchButton_Click(object sender, EventArgs e)
        {
            GenerateFilteredGameList();
            //textBox1.Hide();
        }

        private void GenerateFilteredGameList()
        {
            int roundCount = 0;
            string steamId = steamIdTextBox.Text.ToLower();
            DummyClass dummy1 = new DummyClass();
            List<Game> formGameList = dummy1.GetGameListByName(steamId);

            if (formGameList != null)
            {
                this.Size = new Size(Size.Width, 678);
                ClearGameListBox();
                foreach (Game game in formGameList)
                {
                    _combinedRank = 0;
                    if ((CheckGenre(game) | CheckGameMode(game) | CheckSpecifier(game)) & (_combinedRank >= minimumRank.Value)) //rank >= minimumRank.Value
                    {
                        LoadHeaderImages(game.AppId, roundCount);
                        LoadGameInfo(game, roundCount);
                        roundCount++;
                    }
                }
            }
        }

        private bool CheckSpecifier(Game game)
        {
            bool match = false;
            foreach (string tag in game.Genre)
            {
                foreach (string specifier in specifierCheckedListBox.CheckedItems)
                {
                    if (tag == specifier)
                    {
                        _combinedRank++;
                        match = true;
                    }
                }
            }
            return match;
        }

        private bool CheckGameMode(Game game)
        {
            bool match = false;
            foreach (string tag in game.Genre)
            {
                foreach (string gameMode in gameModeListBox.CheckedItems)
                {
                    if (tag == gameMode)
                    {
                        _combinedRank++;
                        match = true;
                    }
                }
            }
            return match;
        }

        private bool CheckGenre(Game game)
        {
            bool match = false;
            foreach (string tag in game.Genre)
            {
                foreach (string genre in genreCheckListBox.CheckedItems)
                {
                    if (tag == genre)
                    {
                        _combinedRank++;
                        match = true;
                    }
                }
            }
            return match;
        }

        private void LoadGameInfo(Game game, int roundCount)
        {
            Label[] devLabels = { devLabel1, label14, label17, label20, label23, label26, label31, label34, label37, label40, label43, label46, label49, label52, label55};
            Label[] releaseLabels = {releaseLabel1, label15, label18, label21, label23, label27, label32, label35, label36, label41, label42, label47, label50, label53, label56 };
            Label[] gameLabels = { gameNameLabel1, label13, label16, label19, label22, label25, label30, label33, label36, label39, label42, label45, label48, label51, label54, label57 };
            TextBox[] descriptionBoxes = {descriptionBox1, textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15};

            gameLabels[roundCount].Text = game.Name;
            gameLabels[roundCount].Visible = true;
            devLabels[roundCount].Text = "Developer: " + game.Developer;
            descriptionBoxes[roundCount].Text = game.Description;
            releaseLabels[roundCount].Text = "Release Year: " + game.ReleaseYear.ToString();
            
            flowLayoutPanel2.Visible = true;
        }

        private void ClearGameListBox()
        {
            PictureBox[] pictureBoxes = { gameThumbPictureBox1, gameThumbPictureBox11, gameThumbPictureBox12, gameThumbPictureBox13, gameThumbPictureBox14, gameThumbPictureBox15, gameThumbPictureBox16, gameThumbPictureBox17, gameThumbPictureBox18,
                gameThumbPictureBox19, gameThumbPictureBox20, gameThumbPictureBox21, gameThumbPictureBox22, gameThumbPictureBox23, gameThumbPictureBox24, gameThumbPictureBox25 };
            Label[] gameLabels = { gameNameLabel1, label13, label16, label19, label22, label25, label30, label33, label36, label39, label42, label45, label48, label51, label54, label57 };

            foreach (PictureBox pb in pictureBoxes)
            {
                if (pb.Image != null)
                {
                    pb.Image = null;
                }
            }

            foreach (Label label in gameLabels)
            {
                label.Visible = false;
            }
            flowLayoutPanel2.Visible = false;
        }

        private void LoadHeaderImages(int appId, int pb)
        {
            PictureBox[] pictureBoxes = { gameThumbPictureBox1, gameThumbPictureBox11, gameThumbPictureBox12, gameThumbPictureBox13, gameThumbPictureBox14, gameThumbPictureBox15, gameThumbPictureBox16, gameThumbPictureBox17, gameThumbPictureBox18,
                gameThumbPictureBox19, gameThumbPictureBox20, gameThumbPictureBox21, gameThumbPictureBox22, gameThumbPictureBox23, gameThumbPictureBox24, gameThumbPictureBox25 };
            try
            {
                pictureBoxes[pb].Enabled = true;
                pictureBoxes[pb].Visible = true;
                pictureBoxes[pb].Load("http://cdn.akamai.steamstatic.com/steam/apps/" + appId + "/header.jpg");
            }
            catch (Exception e)
            {
                Debug.WriteLine("{0} exception caught", e);
            }
        }

        private void genreCheckListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                minimumRank.Value = genreCheckListBox.CheckedItems.Count + gameModeListBox.CheckedItems.Count + specifierCheckedListBox.CheckedItems.Count;
            }
        }

        private void gameModeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                minimumRank.Value = genreCheckListBox.CheckedItems.Count + gameModeListBox.CheckedItems.Count + specifierCheckedListBox.CheckedItems.Count;
            }
        }

        private void specifierListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                minimumRank.Value = genreCheckListBox.CheckedItems.Count + gameModeListBox.CheckedItems.Count + specifierCheckedListBox.CheckedItems.Count;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            minimumRank.Visible = !minimumRank.Visible;
            checkBox1.Visible = !checkBox1.Visible;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                minimumRank.Value = genreCheckListBox.CheckedItems.Count + gameModeListBox.CheckedItems.Count + specifierCheckedListBox.CheckedItems.Count;
            }
            else
            {
                minimumRank.Value = 0;
            }
        }

        //!!
        //to do: Maybe change the functionality of click event in the control instead
        private void GamePictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;

            if (pb != null) ResizeTablePanels(pb);
        }

        private void ResizeTablePanels(PictureBox pb)
        {
            Size full = new Size(597, 142);
            Size regular = new Size(196, 142);
            TableLayoutPanel tpl = pb.Parent as TableLayoutPanel;
            tpl.Size = tpl.Size != full ? full : regular;

            if (tpl.ColumnStyles[0].Width != 33)
            {
                tpl.ColumnStyles[0].SizeType = SizeType.Percent;
                tpl.ColumnStyles[0].Width = 33;
                tpl.ColumnStyles[1].SizeType = SizeType.Percent;
                tpl.ColumnStyles[1].Width = 47;
                tpl.ColumnStyles[2].SizeType = SizeType.Percent;
                tpl.ColumnStyles[2].Width = 20;

                tpl.RowStyles[0].SizeType = SizeType.Percent;
                tpl.RowStyles[0].Height = 9;
                tpl.RowStyles[1].SizeType = SizeType.Percent;
                tpl.RowStyles[1].Height = 12;
                tpl.RowStyles[2].SizeType = SizeType.Percent;
                tpl.RowStyles[2].Height = 71;
                tpl.RowStyles[3].SizeType = SizeType.Percent;
                tpl.RowStyles[3].Height = 8;
                //tpl.Location = new Point(3, 3); //to be worked on
            }
            else
            {
                tpl.ColumnStyles[0].SizeType = SizeType.Percent;
                tpl.ColumnStyles[0].Width = 100;
                tpl.ColumnStyles[1].SizeType = SizeType.Percent;
                tpl.ColumnStyles[1].Width = 0;
                tpl.ColumnStyles[2].SizeType = SizeType.Percent;
                tpl.ColumnStyles[2].Width = 0;

                tpl.RowStyles[0].SizeType = SizeType.Percent;
                tpl.RowStyles[0].Height = 92;
                tpl.RowStyles[1].SizeType = SizeType.Percent;
                tpl.RowStyles[1].Height = 0;
                tpl.RowStyles[2].SizeType = SizeType.Percent;
                tpl.RowStyles[2].Height = 0;
                tpl.RowStyles[3].SizeType = SizeType.Percent;
                tpl.RowStyles[3].Height = 8;
            }
        }

        private void StartSearchPage_Load(object sender, EventArgs e)
        {
            foreach (Control tlpControl in flowLayoutPanel2.Controls)
            {
                foreach (PictureBox pictureBox in tlpControl.Controls.OfType<PictureBox>())
                {
                    ResizeTablePanels(pictureBox);
                }
            }
        }

        private void btnSteamStore1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            string currentAppId = "";
            foreach (PictureBox pb in btn.Parent.Controls.OfType<PictureBox>())
            {
                foreach (char c in pb.ImageLocation)
                {
                    if (char.IsDigit(c))
                    {
                        currentAppId += c;
                    }
                }
            }
            
            System.Diagnostics.Process.Start("http://store.steampowered.com/app/" + currentAppId);
        }
    }
}
