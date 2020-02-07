using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for ChoosePlayerControl.xaml
    /// </summary>
    public partial class ChoosePlayerControl
	{
        private Buzzer buzzer;
        private Storyboard myStoryboard = new Storyboard();
        private bool Waiting;

        public delegate void ChoosePlayerControlHandler(object sender, EventArgs e);

        public event ChoosePlayerControlHandler Finished;

		public ChoosePlayerControl()
		{
			this.InitializeComponent();
            InitBuzzer();
            progPlayerTimer.Visibility = Visibility.Hidden;
    	}

        private void InitBuzzer()
        {
            buzzer = Main.buzzer;
            if (buzzer != null)
            {
                buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
                for (int i = 1; i <= 4; i++)
                {
                    buzzer.Active(i);
                    buzzer.LightOff(i);
                }
            }
        }

        private void PlayerTimer(int seconds)
        {
            Waiting = true;
            progPlayerTimer.Visibility = Visibility.Visible;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 100;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(seconds));

            myStoryboard.Completed += new EventHandler(myStoryboard_Completed);
            myStoryboard.Children.Clear();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, "progPlayerTimer");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ProgressBar.ValueProperty));
            
            myStoryboard.Begin(this, true);
        }

        void myStoryboard_Completed(object sender, EventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T002.sid_MOS6581R3.mp3");

            Ending(this, e);
        }

        void Ending(object sender, EventArgs e)
        {
            if (buzzer != null)
                buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            myStoryboard.Stop(this);
            Finished(this, null);
            Waiting = false;
        }

        private void AnswerClick(object sender, AnswerClickEventArgs ace)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (ace.Answer != answercolor.red && buzzer.IsActive(ace.Id))
                {
                    if (ace.Answer == answercolor.blue && !Main.players[0].Active)
                    {
                        Main.players[0].Active = true;
                        Main.players[0].Buzzer = ace.Id;
                        buzzer.Inactive(ace.Id);
                        buzzer.LightOn(ace.Id);

                        buttonBlue.Visibility = Visibility.Hidden;
                        if (!Waiting) PlayerTimer(10);
                    }
                    else if (ace.Answer == answercolor.orange && !Main.players[1].Active)
                    {
                        Main.players[1].Active = true;
                        Main.players[1].Buzzer = ace.Id;
                        buzzer.Inactive(ace.Id);
                        buzzer.LightOn(ace.Id);

                        buttonOrange.Visibility = Visibility.Hidden;
                        if (!Waiting) PlayerTimer(10);
                    }
                    else if (ace.Answer == answercolor.green && !Main.players[2].Active)
                    {
                        Main.players[2].Active = true;
                        Main.players[2].Buzzer = ace.Id;
                        buzzer.Inactive(ace.Id);
                        buzzer.LightOn(ace.Id);

                        buttonGreen.Visibility = Visibility.Hidden;
                        if (!Waiting) PlayerTimer(10);
                    }
                    else if (ace.Answer == answercolor.yellow && !Main.players[3].Active)
                    {
                        Main.players[3].Active = true;
                        Main.players[3].Buzzer = ace.Id;
                        buzzer.Inactive(ace.Id);
                        buzzer.LightOn(ace.Id);

                        buttonYellow.Visibility = Visibility.Hidden;
                        if (!Waiting) PlayerTimer(10);
                    }

                    if ( Main.players[0].Active && Main.players[1].Active && Main.players[2].Active && Main.players[3].Active )
                        Ending(this, null);
                }
            }));
        }

        private void buttonBlue_Click(object sender, RoutedEventArgs e)
        {
            Main.players[0].Active = true;
            Main.players[1].Active = false;
            Main.players[2].Active = false;
            Main.players[3].Active = false;
            Ending(this, null);
        }

        private void buttonOrange_Click(object sender, RoutedEventArgs e)
        {
            Main.players[0].Active = false;
            Main.players[1].Active = true;
            Main.players[2].Active = false;
            Main.players[3].Active = false;
            Ending(this, null);
        }

        private void buttonGreen_Click(object sender, RoutedEventArgs e)
        {
            Main.players[0].Active = false;
            Main.players[1].Active = false;
            Main.players[2].Active = true;
            Main.players[3].Active = false;
            Ending(this, null);
        }

        private void buttonYellow_Click(object sender, RoutedEventArgs e)
        {
            Main.players[0].Active = false;
            Main.players[1].Active = false;
            Main.players[2].Active = false;
            Main.players[3].Active = true;
            Ending(this, null);
        }
    }
}