using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for QuestionControl.xaml
    /// </summary>
    public partial class QuestionControl
    {
        private Storyboard myStoryboard;
        private Buzzer buzzer;
        private Question question;
        private int ScoreAdder;
        private bool GameOver;
        private SolidColorBrush backcolor;

        public gametype GameType = gametype.fastest;
        public bool Running;
        public delegate void QuestionControlHandler(object sender, EventArgs e);

        public event QuestionControlHandler Finished;
        private Dictionary<answercolor, Button> AnswerButtons;

        public QuestionControl()
        {
            Main.musicPlayer.Stop();

            this.InitializeComponent();

            scorePlayer1.imgAvatar.Source = Main.players[0].Avatar;
            scorePlayer1.txtName.Text = Main.players[0].Name;

            scorePlayer2.imgAvatar.Source = Main.players[1].Avatar;
            scorePlayer2.txtName.Text = Main.players[1].Name;

            scorePlayer3.imgAvatar.Source = Main.players[2].Avatar;
            scorePlayer3.txtName.Text = Main.players[2].Name;

            scorePlayer4.imgAvatar.Source = Main.players[3].Avatar;
            scorePlayer4.txtName.Text = Main.players[3].Name;

            UpdateScores();

            AnswerButtons = new Dictionary<answercolor, Button>
            {
                { answercolor.blue, textBlue },
                { answercolor.green, textGreen },
                { answercolor.orange, textOrange },
                { answercolor.yellow, textYellow }
            };

            if (!Main.players[0].Active)
            {
                scorePlayer1.Visibility = Visibility.Hidden;
            }

            if (!Main.players[1].Active)
            {
                scorePlayer2.Visibility = Visibility.Hidden;
            }

            if (!Main.players[2].Active)
            {
                scorePlayer3.Visibility = Visibility.Hidden;
            }

            if (!Main.players[3].Active)
            {
                scorePlayer4.Visibility = Visibility.Hidden;
            }
        }

        private void InitControl()
        {
            switch (GameType)
            {
                case gametype.fastest:
                    ScoreAdder = 400;
                    break;
            }

            scorePlayer1.imageOverlay.Visibility = Visibility.Hidden;
            scorePlayer2.imageOverlay.Visibility = Visibility.Hidden;
            scorePlayer3.imageOverlay.Visibility = Visibility.Hidden;
            scorePlayer4.imageOverlay.Visibility = Visibility.Hidden;

            scorePlayer1.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/wrong.png");
            scorePlayer2.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/wrong.png");
            scorePlayer3.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/wrong.png");
            scorePlayer4.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/wrong.png");

            textBlue.Background = Brushes.Black;
            textOrange.Background = Brushes.Black;
            textGreen.Background = Brushes.Black;
            textYellow.Background = Brushes.Black;
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

        private void UpdateScores()
        {
            scorePlayer1.txtScore.Text = Main.players[0].Score.ToString("00000");
            scorePlayer2.txtScore.Text = Main.players[1].Score.ToString("00000");
            scorePlayer3.txtScore.Text = Main.players[2].Score.ToString("00000");
            scorePlayer4.txtScore.Text = Main.players[3].Score.ToString("00000");
        }

        private void AnswerClick(object sender, AnswerClickEventArgs ace)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (!GameOver && buzzer.IsActive(ace.Id) && ace.Answer != answercolor.red)
                {
                    if (AnswerButtons[ace.Answer].Content == null)
                        return; //ignore not used colors/buttons

                    buzzer.Inactive(ace.Id);
                    buzzer.LightOff(ace.Id);

                    Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");
                    
                    Main.players[Main.GetPlayer(ace.Id)].Answer = ace.Answer;

                    if (ace.Answer == question.CorrectAnswer)
                    {
                        switch (Main.GetPlayer(ace.Id))
                        {
                            case 0:
                                scorePlayer1.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png");
                                break;
                            case 1:
                                scorePlayer2.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                                break;
                            case 2:
                                scorePlayer3.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                                break;
                            case 3:
                                scorePlayer4.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                                break;
                        }

                        Main.players[Main.GetPlayer(ace.Id)].Score = Main.players[Main.GetPlayer(ace.Id)].Score + ScoreAdder;
                        if (GameType == gametype.fastest)
                            ScoreAdder = ScoreAdder - 100;
                    }
                    else
                    {
                        if (GameType == gametype.fastest)
                            Main.players[Main.GetPlayer(ace.Id)].Score = Main.players[Main.GetPlayer(ace.Id)].Score - 200;
                        if (Main.players[Main.GetPlayer(ace.Id)].Score < 0)
                            Main.players[Main.GetPlayer(ace.Id)].Score = 0;
                    }

                    bool ready = true;
                    for (int i = 1; i <= 4; i++)
                    {
                        if (buzzer.IsActive(i))
                            ready = false;
                    }
                    if (ready)
                        Ending();
                }
            }));
        }

        public void Start()
        {
            try
            {
                InitBuzzer();
            }
            catch { }

            InitControl();

            ShowQuestion();

            RoundTimer(15);
            Running = true;
            myStoryboard.Begin(progRoundTimer, true);
        }

        private void ShowQuestion()
        {
            for (int i = 0; i < 4; i++)
            {
                Main.players[i].Answer = answercolor.red;
            }

            scorePlayer1.txtName.Background = Brushes.Black;
            scorePlayer2.txtName.Background = Brushes.Black;
            scorePlayer3.txtName.Background = Brushes.Black;
            scorePlayer4.txtName.Background = Brushes.Black;

            textBlue.Background = Brushes.Black;
            textOrange.Background = Brushes.Black;
            textGreen.Background = Brushes.Black;
            textYellow.Background = Brushes.Black;

            Main.musicPlayer.Stop();

            question = Main.questionlist.GetNextQuestion();

            this.textQuestion.Text = question.text;
            this.textBlue.Content = question.answerBlue;
            this.textOrange.Content = question.answerOrange;
            if (question.answerGreen != null)
            {
                this.textGreen.Content = question.answerGreen;
                this.textGreen.Visibility = Visibility.Visible;
            }
            else
            {
                this.textGreen.Content = null;
                this.textGreen.Visibility = Visibility.Hidden;
            }

            if (question.answerYellow != null)
            {
                this.textYellow.Content = question.answerYellow;
                this.textYellow.Visibility = Visibility.Visible;
            }
            else
            {
                this.textYellow.Content = null;
                this.textYellow.Visibility = Visibility.Hidden;
            }

            Main.musicPlayer.Play("Super_Trucker_T001.sid_MOS6581R2.mp3");
        }

        private void CollectPoints()
        {
            if (Main.players[0].Active)
            {
                Main.players[0].Score = Main.players[0].Score + ScoreAdder;
                if (ScoreAdder > 1)
                    scorePlayer1.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png");
            }
            else if (Main.players[1].Active)
            {
                if (ScoreAdder > 1)
                    scorePlayer2.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                Main.players[1].Score = Main.players[1].Score + ScoreAdder;
            }
            else if (Main.players[2].Active)
            {
                if (ScoreAdder > 1)
                    scorePlayer3.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                Main.players[2].Score = Main.players[2].Score + ScoreAdder;
            }
            else if (Main.players[3].Active)
            {
                if (ScoreAdder > 1)
                    scorePlayer4.imageOverlay.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("../../Images/right.png"); ;
                Main.players[3].Score = Main.players[3].Score + ScoreAdder;
            }
        }

        private void textBlue_Click(object sender, RoutedEventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");

            if (question.CorrectAnswer != answercolor.blue)
                ScoreAdder = -200;
            CollectPoints();
            Ending();
        }

        private void textOrange_Click(object sender, RoutedEventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");

            if (question.CorrectAnswer != answercolor.orange)
                ScoreAdder = -200;
            CollectPoints();
            Ending();
        }

        private void textGreen_Click(object sender, RoutedEventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");

            if (question.CorrectAnswer != answercolor.green)
                ScoreAdder = -200;
            CollectPoints();
            Ending();
        }

        private void textYellow_Click(object sender, RoutedEventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T001.sid_MOS6581R3.mp3");

            if (question.CorrectAnswer != answercolor.yellow)
                ScoreAdder = -200;
            CollectPoints();
            Ending();
        }

        private void RoundTimer(int seconds)
        {
            progRoundTimer.Visibility = Visibility.Visible;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 100;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(seconds));

            myStoryboard = new Storyboard();
            myStoryboard.Completed += new EventHandler(myStoryboard_Completed);
            myStoryboard.Children.Clear();
            myStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, "progRoundTimer");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ProgressBar.ValueProperty));
        }

        private void EndingTimer(int seconds)
        {
            Main.musicPlayer.Play("Fruitbank_T005.sid_MOS6581R3.mp3");

            progRoundTimer.Visibility = Visibility.Hidden;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 100;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(seconds));

            Storyboard EndingStoryboard = new Storyboard();
            EndingStoryboard.Completed += new EventHandler(EndingStoryboard_Completed);
            EndingStoryboard.Children.Clear();
            EndingStoryboard.Children.Add(myDoubleAnimation);
            Storyboard.SetTargetName(myDoubleAnimation, "progRoundTimer");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(ProgressBar.ValueProperty));
            EndingStoryboard.Begin(progRoundTimer);
        }

        void myStoryboard_Completed(object sender, EventArgs e)
        {
            Main.musicPlayer.Play("Fruitbank_T002.sid_MOS6581R3.mp3");

            Ending();
        }

        void Ending()
        {
            UpdateScores();
            if (buzzer != null)
            {
                buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            }
            myStoryboard.Stop(progRoundTimer);

            Running = false;

            for (int i = 0; i < 4; i++)
            {
                backcolor = new SolidColorBrush();
                if (Main.players[i].Answer == answercolor.blue) backcolor = Brushes.Blue;
                else if (Main.players[i].Answer == answercolor.orange) backcolor = Brushes.Orange;
                else if (Main.players[i].Answer == answercolor.green) backcolor = Brushes.LimeGreen;
                else if (Main.players[i].Answer == answercolor.yellow) backcolor = Brushes.Yellow;
                else backcolor = Brushes.Red;

                if (i == 0) scorePlayer1.txtName.Background = backcolor;
                else if (i == 1) scorePlayer2.txtName.Background = backcolor;
                else if (i == 2) scorePlayer3.txtName.Background = backcolor;
                else if (i == 3) scorePlayer4.txtName.Background = backcolor;
            }

            if (question.CorrectAnswer == answercolor.blue) textBlue.Background = Brushes.Green;
            if (question.CorrectAnswer == answercolor.orange) textOrange.Background = Brushes.Green;
            if (question.CorrectAnswer == answercolor.green) textGreen.Background = Brushes.Green;
            if (question.CorrectAnswer == answercolor.yellow) textYellow.Background = Brushes.Green;

            Main.musicPlayer.Stop();

            scorePlayer1.imageOverlay.Visibility = Visibility.Visible;
            scorePlayer2.imageOverlay.Visibility = Visibility.Visible;
            scorePlayer3.imageOverlay.Visibility = Visibility.Visible;
            scorePlayer4.imageOverlay.Visibility = Visibility.Visible;

            EndingTimer(4);
        }

        private void EndingStoryboard_Completed(object sender, EventArgs e)
        {
            Finished(sender, e);
        }
    }
}
