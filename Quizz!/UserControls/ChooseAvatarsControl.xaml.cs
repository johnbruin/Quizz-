using System;
using System.Windows;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for ChooseAvatarsControl.xaml
    /// </summary>
    public partial class ChooseAvatarsControl
	{
        private int FinishedCount;

        public delegate void ChooseAvatarsControlHandler(object sender, EventArgs e);

        public event ChooseAvatarsControlHandler Finished;

        public ChooseAvatarsControl()
		{
			this.InitializeComponent();

            if (Main.players[0].Active)
            {
                avatarPlayer1.Finished += new ChooseAvatarControl.ChooseAvatarControlHandler(avatarPlayer1_Finished);
                avatarPlayer1.txtName.Text = Main.players[0].Name;
                avatarPlayer1.SelectedIndex = 1;
                avatarPlayer1.Start(Main.players[0].Buzzer);                
            }
            else
            {
                avatarPlayer1.Visibility = Visibility.Hidden;
            }

            if (Main.players[1].Active)
            {
                avatarPlayer2.Finished += new ChooseAvatarControl.ChooseAvatarControlHandler(avatarPlayer2_Finished);
                avatarPlayer2.txtName.Text = Main.players[1].Name;
                avatarPlayer2.SelectedIndex = 2;
                avatarPlayer2.Start(Main.players[1].Buzzer);                
            }
            else
            {
                avatarPlayer2.Visibility = Visibility.Hidden;
            }

            if (Main.players[2].Active)
            {
                avatarPlayer3.Finished += new ChooseAvatarControl.ChooseAvatarControlHandler(avatarPlayer3_Finished);
                avatarPlayer3.txtName.Text = Main.players[2].Name;
                avatarPlayer3.SelectedIndex = 3;
                avatarPlayer3.Start(Main.players[2].Buzzer);                
            }
            else
            {
                avatarPlayer3.Visibility = Visibility.Hidden;
            }

            if (Main.players[3].Active)
            {
                avatarPlayer4.Finished += new ChooseAvatarControl.ChooseAvatarControlHandler(avatarPlayer4_Finished);
                avatarPlayer4.txtName.Text = Main.players[3].Name;
                avatarPlayer4.SelectedIndex = 4;
                avatarPlayer4.Start(Main.players[3].Buzzer);                
            }
            else
            {
                avatarPlayer4.Visibility = Visibility.Hidden;
            }
    	}

        private void avatarPlayer1_Finished(object sender, ChooseAvatarEventArgs cae)
        {
            Main.players[0].Avatar = avatarPlayer1.imgAvatar.Source;
            Main.players[0].Name = avatarPlayer1.txtName.Text;
            FinishedCount++;
            if (FinishedCount == Main.PlayersActiveCount()) Finished(sender, cae);
        }

        private void avatarPlayer2_Finished(object sender, ChooseAvatarEventArgs cae)
        {
            Main.players[1].Avatar = avatarPlayer2.imgAvatar.Source;
            Main.players[1].Name = avatarPlayer2.txtName.Text;
            FinishedCount++;
            if (FinishedCount == Main.PlayersActiveCount()) Finished(sender, cae);
        }

        private void avatarPlayer3_Finished(object sender, ChooseAvatarEventArgs cae)
        {
            Main.players[2].Avatar = avatarPlayer3.imgAvatar.Source;
            Main.players[2].Name = avatarPlayer3.txtName.Text;
            FinishedCount++;
            if (FinishedCount == Main.PlayersActiveCount()) Finished(sender, cae);
        }

        private void avatarPlayer4_Finished(object sender, ChooseAvatarEventArgs cae)
        {
            Main.players[3].Avatar = avatarPlayer4.imgAvatar.Source;
            Main.players[3].Name = avatarPlayer4.txtName.Text;
            FinishedCount++;
            if (FinishedCount == Main.PlayersActiveCount()) Finished(sender, cae);
        }
    }
}