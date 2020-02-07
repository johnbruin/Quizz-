using System;
using System.Windows;
using System.Windows.Threading;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for Intermezzo.xaml
    /// </summary>
    public partial class Intermezzo
	{
        private Buzzer buzzer;

        public delegate void IntermezzoHandler(object sender, EventArgs e);

        public event IntermezzoHandler Finished;

		public Intermezzo()
		{
			this.InitializeComponent();
            try
            {
                InitBuzzer();
            }
            catch { }
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

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            if (buzzer != null)
                buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            Finished(this, null);
        }
    }
}