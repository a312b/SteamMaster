using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DummyClassSolution.Properties;
using SteamSharpCore.steamStore.models;
using SteamSharpCore.steamUser.models;
using DatabaseCore;
using DatabaseCore.lib.converter.models;
using gameDatabase = DatabaseCore.Database;
using Timer = System.Timers.Timer;
using static SteamSharpCore.steamStore.models.SteamStoreGame;

//requires SteamSharp

namespace SteamUI
{
    public partial class SteamTheme : Form
    {
        public delegate void RecommendDelegate(string steamID); //Add SteamID
        public const int WmNclbuttondown = 0xA1;
        public const int Htcaption = 0x2;
        public static string DevKey = Settings.Default.DevKey;
        public static int ElapsedTime;
        private readonly SteamSharpCore.SteamSharp _steamSharp = new SteamSharpCore.SteamSharp(DevKey);
        private readonly gameDatabase _database = new gameDatabase(); 

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

        //Where the game list is made by calling SteamSharp.GameListByIds on a set app ID array (that app ID array is now achieved by getting it from a player)
        //and iterates over all the games to then call functions LoadHeaderImages and LoadGameInfo displaying the game date.
        //A "roundCount" is kept to determine how far we've gotten.
        private void GenerateGameList()
        {
            const int minGameTime = 4; //in minutes
            const int maxRecommendations = 18;
            string steamId = steamIdTextBox.Text;
            List<int> idList = new List<int>();

            if (DevKey == "null")
            {
                MessageBox.Show(
                    "You need to enter a Steam Developer API key in the settings box by clicking the cog located in the top right corner.",
                    "Please enter Steam API Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<UserGameTime.Game> formGameListFromId = _steamSharp.SteamUserGameTimeListById(steamId);
            foreach (UserGameTime.Game game in formGameListFromId)
            {
                if (idList.Count < maxRecommendations && game.playtime_forever > minGameTime)
                {
                    idList.Add(game.appid);
                }
            }

            DisplayGamesInUI(idList);

            Dictionary<int, Game> userGameListFromIds = _database.FindGamesById(idList);
            List<Tag> totalTagList = new List<SteamStoreGame.Tag>();
            if (userGameListFromIds != null)
            {
                foreach (Game game in userGameListFromIds.Values)
                {
                    totalTagList.AddRange(game.Tags);
                }
            }

            IOrderedEnumerable<IGrouping<string, SteamStoreGame.Tag>> tagsOrderByDescending = totalTagList.GroupBy(tag => tag.description)
            .OrderByDescending(tags => tags.Count());
            foreach (var tag in tagsOrderByDescending)
            {
                //do something with tag.Key + tag.Count();
            }
        }

        //Takes the idList and creates a dictionary containing the appID and database Game. We then iterate over these games to call seperate functions to actually display the info.
        private void DisplayGamesInUI(List<int> idList)
        {
            int roundCount = 0;
            Dictionary<int, Game> userGameListFromIds = _database.FindGamesById(idList);

            ClearGameListBox();
            if (userGameListFromIds == null) return;
            foreach (Game game in userGameListFromIds.Values)
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
                label54, label59, label64, label69, label74, label79, label84, label89
            };
            Label[] devLabels =
            {
                label4, label8, label13, label18, label23, label28, label33, label38, label43,
                label48, label53, label58, label63, label68, label73, label78, label83, label88
            };
            Label[] releaseLabels =
            {
                label5, label7, label12, label17, label22, label27, label32, label37, label42,
                label47, label52, label57, label62, label67, label72, label77, label82, label87 //not added new releaselabels becuase they aren't used right now
            };
            Label[] gameLabels =
            {
                label2, label6, label11, label16, label21, label26, label31, label36, label41,
                label46, label51, label56, label61, label66, label71, label76, label81, label86
            };
            Label[] priceLabels =
            {
                label3, label10, label15, label20, label25, label30, label35, label40, label45,
                label50, label55, label60, label65, label70, label75, label80, label85, label90
            };
            RichTextBox[] descriptionBoxes =
            {
                richTextBox1, richTextBox2, richTextBox3, richTextBox4, richTextBox5,
                richTextBox6, richTextBox7, richTextBox8, richTextBox9, richTextBox10,
                richTextBox11, richTextBox12, richTextBox13, richTextBox14, richTextBox15,
                richTextBox16, richTextBox17, richTextBox18
            };

            if (roundCount < gameLabels.Length)
            {
                gameLabels[roundCount].Text = game.Title;
                gameLabels[roundCount].Visible = true;
                foreach (string developer in game.Developer)
                    SB.Append(developer + ", ");
                devLabels[roundCount].Text = SB.ToString().Remove(SB.Length - 2, 1);
                descriptionBoxes[roundCount].Text = game.Description.Substring(0, 275);
                releaseLabels[roundCount].Visible = true;
                releaseLabels[roundCount].Text = game.ReleaseDate; //right now testing out showing the releasedate together with the developer, but no decision has been made yet
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

        //Clears the viwed game list box, to make sure its clean for a new recommendation computation
        public void ClearGameListBox()
        {
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6,
                pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12,
                pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18
            };
            Label[] gameLabels =
            {
                label2, label6, label11, label16, label21, label26, label31, label36, label41,
                label46, label51, label56, label61, label66, label71, label76, label81, label86
            };

            foreach (PictureBox pb in pictureBoxes)
            {
                if (pb.Image != null)
                {
                    pb.Image = null;
                }
            }

            foreach (Label gameLabel in gameLabels)
            {
                gameLabel.Visible = false;
            }
            flowLayoutPanel1.Visible = false;
        }

        //Loads the appropriate image for the corresponding app ID.
        public void LoadHeaderImages(int appId, int pb)
        {
            PictureBox[] pictureBoxes =
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6,
                pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12,
                pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18
            };
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

        //Resizes the TableLayoutPanels and makes sure the labels that are hidden are in the correct visible state.
        private void ResizeTablePanels(TableLayoutPanel sender)
        {
            
            Size full = new Size(646, 164);
            Size regular = new Size(646, 104);
            sender.Size = sender.Size != full ? full : regular;
            Label[] hiddenLabels =
            {
                label4, label8, label13, label18, label23, label28,
                label33, label38, label43, label48, label53, label58,
                label63, label68, label73, label78, label83, label88
            };
            foreach (PictureBox pictureBox in sender.Controls.OfType<PictureBox>())
            {
                if (pictureBox.Image == null) return;
                TableLayoutPanel tpl = pictureBox.Parent as TableLayoutPanel;
                tpl.SetRowSpan(pictureBox, tpl.GetRowSpan(pictureBox) == 3 ? 4 : 3);
            }
            foreach (RichTextBox richTextBox in sender.Controls.OfType<RichTextBox>())
            {
                richTextBox.Visible = richTextBox.Visible == false;
            }
            foreach (Button storeBtn in sender.Controls.OfType<Button>())
            {
                storeBtn.Visible = storeBtn.Visible == false;
            }
            foreach (Label label in sender.Controls.OfType<Label>())
            {
                foreach (Label hiddenLabel in hiddenLabels)
                {
                    if (label.Name == hiddenLabel.Name)
                    {
                        label.Visible = label.Visible == false;
                    }
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

        public event RecommendDelegate RecommendButtomClick;

        private void btnRecommend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(steamIdTextBox.Text))
            {
                MessageBox.Show("Please enter a Steam64 ID");
            }
            else
            {
                ElapsedTime = 0;
                Timer elaspedTimer = new Timer();
                elaspedTimer.Elapsed += timer1_Tick;
                elaspedTimer.Interval = 1000;
                elaspedTimer.Start();

                loadingPictureBox.Visible = true;
                Cursor.Current = Cursors.WaitCursor;
                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.DoWork += (s, a) =>
                {
                    GenerateGameList(); //For testing purposes GenerateGameList can be called instead of the RecommendButtomClick delegate
                    //RecommendButtomClick(steamIdTextBox.Text); //If you crash use GenerateGameList()

                };
                bgWorker.RunWorkerCompleted += (s, a) =>
                {
                    flowLayoutPanel1.Visible = true;
                    Cursor.Current = Cursors.Default;
                    timeElapsedLabel.Text = "Time elapsed: " + ElapsedTime + " sec";
                    elaspedTimer.Stop();
                    elaspedTimer.Dispose();
                    loadingPictureBox.Visible = false;
                };
                bgWorker.RunWorkerAsync();
            }
        }


        //Event: Clicking on the specified object calls the TLP converter and 
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
                    RichTextBox textBoxClick = sender as RichTextBox;
                    TableLayoutPanel textBoxTpl = textBoxClick.Parent as TableLayoutPanel;
                    return textBoxTpl;
                }
                TableLayoutPanel labelTpl = labelClick.Parent as TableLayoutPanel;
                return labelTpl;
            }
            TableLayoutPanel pbtpl = pbClick.Parent as TableLayoutPanel;
            return pbtpl;
        }

        //Prompts the user for a steam API key obtained at http://steamcommunity.com/dev/apikey
        private void btnDevKey_Click(object sender, EventArgs e)
        {
            Form devPrompt = new DevPrompt();
            devPrompt.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ElapsedTime++;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamid.xyz");
        }
    }
}