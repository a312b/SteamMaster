using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DatabaseCore.lib.converter.models;
using DummyClassSolution;
using SteamSharpCore.steamStore.models;
using Timer = System.Timers.Timer;

//requires SteamSharp

namespace SteamUI
{
    public partial class SteamTheme : Form
    {
        public delegate List<Game> RecommendDelegate(string steamID); //Add SteamID
        public const int WmNclbuttondown = 0xA1;
        public const int Htcaption = 0x2;
        public static int ElapsedTime;

        public SteamTheme()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        //Makes the form dragable - the panel used makes the form act like a border.
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WmNclbuttondown, Htcaption, 0);
            }
        }

        private void steamIdTextBox_Click(object sender, EventArgs e)
        {
            steamIdTextBox.Text = "";
        }

        //Takes a list of DatabaseCore games and then iterate over these games to call two seperate functions to actually display the info.
        private void DisplayGamesInUI(List<Game> gameList)
        {
            int roundCount = 0;

            if (gameList == null) return;
            foreach (Game game in gameList.Take(30).ToList())
            {
                LoadHeaderImages(game.SteamAppId, roundCount);
                LoadGameInfo(game, roundCount);
                roundCount++;
            }
        }

        //Takes the current game and the roundCount (iteration#) ands sets the appropriate data 
        //in the specified array of labels and richtextboxes
        public void LoadGameInfo(Game game, int roundCount)
        {
            StringBuilder SB = new StringBuilder();
            Label[] tagLabels =
            {
                label1, label9, label14, label19, label24, label29, label34, label39, label44, label49,
                label54, label59, label64, label69, label74, label79, label84, label89, label94, label99,
                label104, label109, label114, label119, label124, label129, label134, label139, label144,
                label149
            };
            Label[] devLabels =
            {
                label4, label8, label13, label18, label23, label28, label33, label38, label43, label48,
                label53, label58, label63, label68, label73, label78, label83, label88, label92, label98,
                label103, label108, label113, label118, label123, label128, label133, label137, label142,
                label147 
            };
            Label[] releaseLabels =
            {
                label5, label7, label12, label17, label22, label27, label32, label37, label42, label47,
                label52, label57, label62, label67, label72, label77, label82, label87, label95, label100,
                label102, label107, label112, label117, label122, label127, label132, label140, label145,
                label150
            };
            Label[] gameLabels =
            {
                label2, label6, label11, label16, label21, label26, label31, label36, label41, label46,
                label51, label56, label61, label66, label71, label76, label81, label86, label91, label96,
                label101, label106, label111, label116, label121, label126, label131, label136, label141,
                label146
            };
            Label[] priceLabels =
            {
                label3, label10, label15, label20, label25, label30, label35, label40, label45, label50,
                label55, label60, label65, label70, label75, label80, label85, label90, label93, label98,
                label105, label110, label115, label120, label125, label130, label135, label138, label143,
                label148
            };
            DescriptionBox[] descriptionBoxes =
            {
                descriptionBox1, descriptionBox2, descriptionBox3, descriptionBox4, descriptionBox5,
                descriptionBox6, descriptionBox7, descriptionBox8, descriptionBox9, descriptionBox10,
                descriptionBox11, descriptionBox12, descriptionBox13, descriptionBox14, descriptionBox15,
                descriptionBox16, descriptionBox17, descriptionBox18, descriptionBox19, descriptionBox20,
                descriptionBox21, descriptionBox22, descriptionBox23, descriptionBox24, descriptionBox25,
                descriptionBox26, descriptionBox27, descriptionBox28, descriptionBox29, descriptionBox30
            };

            if (roundCount < gameLabels.Length)
            {
                gameLabels[roundCount].Text = game.Title;
                gameLabels[roundCount].Visible = true;

                if (game.Developer == null || game.Developer.Count <= 0)
                    game.Developer = new List<string>() {"No Developer"};
                foreach (string developer in game.Developer)
                    SB.Append(developer + ", ");

                devLabels[roundCount].Text = SB.ToString().Remove(SB.Length - 2, 1);
                descriptionBoxes[roundCount].Text = game.Description;
                releaseLabels[roundCount].Text = game.ReleaseDate;
                releaseLabels[roundCount].Visible = true;
                if (game.Price == 0)
                {
                    priceLabels[roundCount].Text = "Free";
                }
                else
                {
                    priceLabels[roundCount].Text = game.Price + " €";
                }
                priceLabels[roundCount].Visible = true;
                SB.Clear();
                foreach (SteamStoreGame.Tag tag in game.Tags)
                    SB.Append(tag.description + ", ");
                tagLabels[roundCount].Text = SB.ToString().Remove(SB.Length - 2, 1);
                tagLabels[roundCount].Visible = true;
            }
        }

        //Loads the appropriate image for the corresponding app ID.
        public void LoadHeaderImages(int appId, int pb)
        {
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6,
                pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12,
                pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18,
                pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24,
                pictureBox25, pictureBox26, pictureBox27, pictureBox28, pictureBox29, pictureBox30
            };
            try
            {
                pictureBoxes[pb].Enabled = true;
                pictureBoxes[pb].Visible = true;

                pictureBoxes[pb].Load("http://cdn.akamai.steamstatic.com/steam/apps/" + appId + "/header.jpg");
            }
            catch (Exception e)
            {
                Debug.WriteLine("{0} header image exception caught", e);
            }
        }

        //Resizes the TableLayoutPanels and makes sure the labels that are hidden are in the correct visible state.
        private void ResizeTablePanels(TableLayoutPanel sender)
        {
            Size full = new Size(646, 204);
            Size regular = new Size(646, 104);
            sender.Size = sender.Size != full ? full : regular;
            Label[] hiddenLabels =
            {
                label4, label8, label13, label18, label23, label28,
                label33, label38, label43, label48, label53, label58,
                label63, label68, label73, label78, label83, label88, label92, label98,
                label103, label108, label113, label118, label123, label128, label133, label137, label142,
                label147
            };
            foreach (PictureBox pictureBox in sender.Controls.OfType<PictureBox>())
            {
                if (pictureBox.Image == null) return;
                TableLayoutPanel tpl = pictureBox.Parent as TableLayoutPanel;
                tpl.SetRowSpan(pictureBox, tpl.GetRowSpan(pictureBox) == 3 ? 4 : 3);
            }
            foreach (DescriptionBox descriptionBox in sender.Controls.OfType<DescriptionBox>())
            {
                descriptionBox.Visible = descriptionBox.Visible == false;
            }
            foreach (Button storeBtn in sender.Controls.OfType<Button>())
            {
                storeBtn.Visible = storeBtn.Visible == false;
            }
            foreach (Label label in sender.Controls.OfType<Label>())
            {
                foreach (Label hiddenLabel in hiddenLabels.Where(hiddenLabel => label.Name == hiddenLabel.Name))
                {
                    label.Visible = label.Visible == false;
                }
            }
        }

        //Button that takes you to steam store by opening link with app ID. 
        //The app ID is retrived by finding the button's parents control's picturebox 
        //and retrieving it from the image location as any digit in the link.
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
            Process.Start("http://store.steampowered.com/app/" + currentAppId);
        }

        public event RecommendDelegate RecommendButtonClick;

        private void btnRecommend_Click(object sender, EventArgs e)
        {
            double steamId;
            if (string.IsNullOrWhiteSpace(steamIdTextBox.Text) || !double.TryParse(steamIdTextBox.Text, out steamId))
            {
                MessageBox.Show("Please enter a Steam64 ID");
            }
            else
            {
                ElapsedTime = 0;
                Timer elaspedTimer = new Timer {Interval = 1000};
                elaspedTimer.Elapsed += timer1_Tick;
                elaspedTimer.Start();

                loadingPictureBox.Visible = true;
                Cursor.Current = Cursors.WaitCursor;
                flowLayoutPanel1.Visible = false;
                BackgroundWorker bgWorker = new BackgroundWorker();
                List<Game> gameList = new List<Game>();

                bgWorker.DoWork += (s, a) =>
                {
                    gameList = RecommendButtonClick(steamIdTextBox.Text);
                };
                bgWorker.RunWorkerCompleted += (s, a) =>
                {
                    DisplayGamesInUI(gameList);
                    timeElapsedLabel.Text = "Time elapsed: " + ElapsedTime + " sec";
                    elaspedTimer.Stop();
                    loadingPictureBox.Visible = false;
                    Cursor.Current = Cursors.Default;
                    flowLayoutPanel1.Visible = true;
                };
                bgWorker.RunWorkerAsync();
            }
        }

        //Event: Clicking on the specified object calls the TLP converter and ResizeTablePanels 
        private void object_Click(object sender, EventArgs e)
        {
            TableLayoutPanel tableObject = tplConverter(sender) as TableLayoutPanel;
            ResizeTablePanels(tableObject);
        }

        //Makes sure the returned type is a TableLayoutPanel(TLP) by 
        //converting the object clicked (PictureBox, TextBox or TableLayoutpanel) to a TLP
        private object tplConverter(object sender)
        {
            TableLayoutPanel tableClickTpl = sender as TableLayoutPanel;
            if (tableClickTpl != null) return tableClickTpl;
            PictureBox pbClick = sender as PictureBox;
            if (pbClick == null)
            {
                Label labelClick = sender as Label;
                if (labelClick == null)
                {
                    DescriptionBox textBoxClick = sender as DescriptionBox;
                    TableLayoutPanel textBoxTpl = textBoxClick.Parent as TableLayoutPanel;
                    return textBoxTpl;
                }
                TableLayoutPanel labelTpl = labelClick.Parent as TableLayoutPanel;
                return labelTpl;
            }
            TableLayoutPanel pbtpl = pbClick.Parent as TableLayoutPanel;
            return pbtpl;
        }

        //At every tick (1000 ms) of the timer1 the variable ElapsedTime is counted up
        private void timer1_Tick(object sender, EventArgs e)
        {
            ElapsedTime++;
        }

        //Opens link for user to obtain a Steam64 ID
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamid.xyz");
        }

        //For testing - if steamIdTextBox is selected and the "alt" key is pressed a replacement ID is used.
        private void steamIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                steamIdTextBox.Text = "76561197987505654";
            }
        }
    }
}