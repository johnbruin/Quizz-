using System;
using System.Windows;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for EnterNamesControl.xaml
    /// </summary>
    public partial class EnterNamesControl
	{
        public delegate void EnterNamesControlHandler(object sender, EventArgs e);

        public event EnterNamesControlHandler Finished;

        public EnterNamesControl()
		{
			this.InitializeComponent();

            namePlayer1.Visibility = Visibility.Hidden;
            namePlayer2.Visibility = Visibility.Hidden;
            namePlayer3.Visibility = Visibility.Hidden;
            namePlayer4.Visibility = Visibility.Hidden;

            if (Main.players[0].Active)
            {
                namePlayer1.Visibility = Visibility.Visible;
                namePlayer1.txtName.Text = "PLAYER 1";
                namePlayer1.Finished += new EnterNameControl.EnterNameControlHandler(namePlayer1_Finished);
                namePlayer1.Start(Main.players[0].Buzzer);
            }
            else
            {
                namePlayer1_Finished(this, null);
            }
 		}

        private void namePlayer1_Finished(object sender, EnterNameEventArgs ene)
        {
            Main.players[0].Name = namePlayer1.txtName.Text;
            if (Main.players[1].Active)
            {
                namePlayer2.Visibility = Visibility.Visible;
                namePlayer2.txtName.Text = "PLAYER 2";
                namePlayer2.Finished += new EnterNameControl.EnterNameControlHandler(namePlayer2_Finished);
                namePlayer2.Start(Main.players[1].Buzzer);
            }
            else
            {
                namePlayer2_Finished(this, null);
            }
        }

        private void namePlayer2_Finished(object sender, EnterNameEventArgs ene)
        {
            Main.players[1].Name = namePlayer2.txtName.Text;
            if (Main.players[2].Active)
            {
                namePlayer3.Visibility = Visibility.Visible;
                namePlayer3.txtName.Text = "PLAYER 3";
                namePlayer3.Finished += new EnterNameControl.EnterNameControlHandler(namePlayer3_Finished);
                namePlayer3.Start(Main.players[2].Buzzer);
            }
            else
            {
                namePlayer3_Finished(this, null);
            }
        }

        private void namePlayer3_Finished(object sender, EnterNameEventArgs ene)
        {
            Main.players[2].Name = namePlayer3.txtName.Text;
            if (Main.players[3].Active)
            {
                namePlayer4.Visibility = Visibility.Visible;
                namePlayer4.txtName.Text = "PLAYER 4";
                namePlayer4.Finished += new EnterNameControl.EnterNameControlHandler(namePlayer4_Finished);
                namePlayer4.Start(Main.players[3].Buzzer);
            }
            else
            {
                namePlayer4_Finished(this, null);
            }
        }

        private void namePlayer4_Finished(object sender, EnterNameEventArgs ene)
        {
            Main.players[3].Name = namePlayer4.txtName.Text;
            Finished(sender, ene);
        }
    }
}