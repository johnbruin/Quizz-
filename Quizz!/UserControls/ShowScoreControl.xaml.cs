using System;
using System.Windows;
using System.Windows.Threading;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for ShowScoreControl.xaml
    /// </summary>
    public partial class ShowScoreControl
	{
        private Buzzer buzzer;

        public delegate void ShowScoreControlHandler(object sender, EventArgs e);

        public event ShowScoreControlHandler Finished;

		public ShowScoreControl()
		{
			InitializeComponent();
            try
            {
                InitBuzzer();
            }
            catch { }
            ShowScores();
		}

        public void ShowScores()
        {
            PlayerScoreControl[] psc = new PlayerScoreControl[4];
            stackShowScores.Children.Clear();
            for (int i = 0; i < 4; i++)
            {
                psc[i] = new PlayerScoreControl();
                stackShowScores.Children.Add(psc[i]);
                psc[i].imageOverlay.Visibility = Visibility.Hidden;
                psc[i].imgAvatar.Source = Main.players[i].Avatar;
                psc[i].txtName.Text = Main.players[i].Name;
                psc[i].txtScore.Text = Main.players[i].Score.ToString("00000");
                if (!Main.players[i].Active)
                    psc[i].Visibility = Visibility.Hidden;
             }
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