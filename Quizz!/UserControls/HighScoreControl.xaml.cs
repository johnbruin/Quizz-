using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Threading;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for HighScoreControl.xaml
    /// </summary>
    public partial class HighScoreControl
	{
        private DataSet HighScoresDS;
        private string filename = "../../HighScores.xml";

        private Buzzer buzzer;

        public delegate void HighScoreControlHandler(object sender, EventArgs e);

        public event HighScoreControlHandler Finished;

		public HighScoreControl()
		{
			InitializeComponent();
            try
            {
                InitBuzzer();
            }
            catch { }
            ReadHighScores();
            ShowHighScores();
		}

        public void ReadHighScores()
        {
            HighScoresDS = new DataSet();
            HighScoresDS.ReadXml(filename);  
        }

        public void WriteHighScores()
        {
            HighScoresDS.WriteXml(filename);
        }

        public void AddHighScore(string name, string score)
        {
            DataRow dr= HighScoresDS.Tables["highscore"].NewRow();
            dr["name"] = name;
            dr["score"] = score;
            HighScoresDS.Tables["highscore"].Rows.Add(dr);
            ShowHighScores();
        }

        public void ShowHighScores()
        {
            stackHighScores.Children.Clear();

            StackPanel stackTitle = new StackPanel();
            stackTitle.Orientation = Orientation.Horizontal;
            stackTitle.HorizontalAlignment = HorizontalAlignment.Center;

            Label labelTitle = new Label();
            labelTitle.Foreground = System.Windows.Media.Brushes.White;
            labelTitle.FontSize = 36;
            labelTitle.FontWeight = FontWeights.Bold;
            labelTitle.Content = "High Score Table";

            stackTitle.Children.Add(labelTitle);
            stackHighScores.Children.Add(stackTitle);

            DataRow[] HighScores = HighScoresDS.Tables["highscore"].Select("", "score desc");
            int j = HighScores.Length < 10 ? HighScores.Length : 10;
            for (int i = 0; i < j; i++)
            {
                StackPanel stackHighScore = new StackPanel();
                stackHighScore.Orientation = Orientation.Horizontal;
                stackHighScore.HorizontalAlignment = HorizontalAlignment.Center;

                Label labelName = new Label();
                labelName.Foreground = System.Windows.Media.Brushes.White;
                labelName.FontSize = 30 - (i*2);
                labelName.Content = HighScores[i]["name"].ToString();

                Label labelScore = new Label();
                labelScore.Foreground = System.Windows.Media.Brushes.White;
                labelScore.FontSize = 30 - (i*2);
                labelScore.Content = HighScores[i]["score"].ToString();

                stackHighScore.Children.Add(labelName);
                stackHighScore.Children.Add(labelScore);

                stackHighScores.Children.Add(stackHighScore);
                //Debug.WriteLine(HighScores[i]["name"].ToString() + HighScores[i]["score"].ToString());
            }
            this.WriteHighScores();
        }

        private void InitBuzzer()
        {
            buzzer = Main.buzzer;
            if (buzzer != null)
            {
                buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
                for (int i = 1; i <= 4; i++)
                {
                    buzzer.Inactive(i);
                    buzzer.LightOff(i);
                }
                for (int i = 0; i < 4; i++)
                {
                    if (Main.players[i].Active)
                    {
                        buzzer.Active(Main.players[i].Buzzer);
                        buzzer.LightOn(Main.players[i].Buzzer);
                    }
                }
            }
        }

        private void AnswerClick(object sender, AnswerClickEventArgs ace)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (ace.Answer == answercolor.red && buzzer.IsActive(ace.Id))
                {
                    Main.PlaySound("-o27 fruitbank.sid", 1);
                    Ending(this, null);
                }
            }));
        }

        void Ending(object sender, EventArgs e)
        {

            buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            Finished(this, null);
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            if (buzzer != null)
                buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            Finished(this, null);
        }
    }
}