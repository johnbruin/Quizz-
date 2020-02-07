using System;
using System.ComponentModel;
using System.Windows;

namespace Quizz_
{
    public enum gametype
    {
        fastest
    }

    /// <summary>
    /// Interaction logic for Quizz!.xaml
    /// </summary>
    public partial class Main : Window
    {
        public static Buzzer buzzer;
        public static QuestionList questionlist = new QuestionList();
        public static Player[] players;

        //public static SID sidplayer;
        public static MusicPlayer musicPlayer;

        private int questioncounter = 0;
        private int questionsperround = 10;
        private int bombplayer = 0;       

        public Main()
        {
            //WindowState = WindowState.Normal;
            //WindowStyle = WindowStyle.None;
            //Topmost = true;
            //WindowState = WindowState.Maximized;

            InitializeComponent();

            Main.musicPlayer = new MusicPlayer();

            try
            {
                buzzer = new Buzzer();
            }
            catch
            {
                buzzer = null;
            }

            InitPlayers();

            ChoosePlayers();            
        }

        private void InitPlayers()
        {
            players = new Player[4];
            for (int i=0;i<4;i++)
            {
                players[i] = new Player();
                players[i].Active = false;
            }
        }

        private void ReadQuestions()
        {
            if (questionlist.Count == 0)
            {
                questionlist.ReadQuestions("../../questions/70-536.xml");
            }
        }

        #region Choose Players
        private void ChoosePlayers()
        {
            Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");
            
            var controlChoosePlayers = new ChoosePlayerControl();
            controlChoosePlayers.Finished += new ChoosePlayerControl.ChoosePlayerControlHandler(controlChoosePlayers_Finished);
            
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlChoosePlayers);
        }
        private void controlChoosePlayers_Finished(object sender, EventArgs e)
        {
            ChooseAvatars();
        }
        #endregion

        #region Enter Names
        private void EnterNames()
        {
            for (int i = 1; i <= 4; i++)
            {
                if (buzzer != null)
                {
                    buzzer.Inactive(i);
                    buzzer.LightOff(i);
                }
            }
            EnterNamesControl controlEnterNames = new EnterNamesControl();
            controlEnterNames.Finished += new EnterNamesControl.EnterNamesControlHandler(controlEnterNames_Finished);
            
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlEnterNames);
        }
        private void controlEnterNames_Finished(object sender, EventArgs e)
        {
            ChooseAvatars();
        }
        #endregion

        #region Choose Avatars
        private void ChooseAvatars()
        {
            ChooseAvatarsControl controlChooseAvatars = new ChooseAvatarsControl();
            controlChooseAvatars.Finished += new ChooseAvatarsControl.ChooseAvatarsControlHandler(controlChooseAvatars_Finished);

            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlChooseAvatars);
        }
        private void controlChooseAvatars_Finished(object sender, EventArgs e)
        {
            IntermezzoRound1();
        }
        #endregion

        #region Intermezzo Round 1
        private void IntermezzoRound1()
        {
            Intermezzo controlIntermezzo = new Intermezzo();
            controlIntermezzo.Finished += new Intermezzo.IntermezzoHandler(IntermezzoRound1_Finished);
            controlIntermezzo.Title.Content = "ANSWER AS FAST AS YOU CAN";
            controlIntermezzo.Line1.Text = "THE FASTEST YOU ARE THE MORE POINTS YOU WILL SCORE BUT BEWARE FOR WRONG ANSWERS!";
            controlIntermezzo.Line2.Text = "400 FOR THE 1ST; 300 FOR THE 2ND; 200 FOR THE 3RD; 100 FOR THE 4TH BUT -200 POINTS FOR A WRONG ANSWER!";
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlIntermezzo);
        }

        private void IntermezzoRound1_Finished(object sender, EventArgs e)
        {
            Main.musicPlayer.Stop();
            Main.musicPlayer.Play("Fruitbank_T003.sid_MOS6581R3.mp3");

            Round1();
        }

        #endregion

        #region Round 1
        private QuestionControl controlRound1Questions;
        private void Round1()
        {
            questioncounter = 0;
            questionsperround = 4;
            controlRound1Questions = new QuestionControl();
            controlRound1Questions.GameType = gametype.fastest;
            controlRound1Questions.Finished += new QuestionControl.QuestionControlHandler(controlRound1Questions_Finished);
            
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlRound1Questions);
            Round1NextQuestion();
        }
        private void Round1NextQuestion()
        {
            questioncounter++;
            ReadQuestions();
            controlRound1Questions.Start();
        }
        void controlRound1Questions_Finished(object sender, EventArgs e)
        {
            ShowScores();
        }
        private void ShowScores()
        {
            ShowScoreControl controlShowScores = new ShowScoreControl();
            controlShowScores.Finished += new ShowScoreControl.ShowScoreControlHandler(controlShowScores_Finished);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlShowScores);
        }
        private void controlShowScores_Finished(object sender, EventArgs e)
        {
            Main.musicPlayer.Stop();

            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlRound1Questions);
            if (questioncounter < questionsperround)
            {
                Round1NextQuestion();
            }
            else
            {
                ShowWinner();
            }
        }
        #endregion

        #region Show Winner
        private void ShowWinner()
        {
            Main.musicPlayer.Play("Fruitbank_T005.sid_MOS6581R3.mp3");

            WinnerControl controlWinner = new WinnerControl();
            controlWinner.Finished += new WinnerControl.WinnerControlHandler(controlWinner_Finished);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlWinner);
        }
        private void controlWinner_Finished(object sender, EventArgs e)
        {
            Main.musicPlayer.Stop();
            ShowHighScores();
        }
        #endregion

        #region Show High Scores
        private void ShowHighScores()
        {
            Main.musicPlayer.Play("Parallax_T002.sid_MOS6581R2.mp3");

            HighScoreControl controlHighScores = new HighScoreControl();
            for (int i=0; i<4; i++)
            {
                if (players[i].Active)
                {
                    controlHighScores.AddHighScore(players[i].Name, players[i].Score.ToString("00000"));
                }
            }
            controlHighScores.Finished += new HighScoreControl.HighScoreControlHandler(controlHighScores_Finished);
            mainGrid.Children.Clear();
            mainGrid.Children.Add(controlHighScores);
        }
        private void controlHighScores_Finished(object sender, EventArgs e)
        {
            Main.musicPlayer.Stop();
            InitPlayers();
            ChoosePlayers();
        }
        #endregion

        public static int PlayersActiveCount()
        {
            int ActiveCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (players[i].Active)
                    ActiveCount++;
            }
            return ActiveCount;
        }

        public static int GetPlayer(int buzzerID)
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                if (players[i].Buzzer == buzzerID)
                    break;
            }
            return i;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                Main.musicPlayer.Stop();
            }
            catch
            { }
        }
    }
}