using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quizz_
{
    /// <summary>
    /// Interaction logic for ChooseAvatarControl.xaml
	/// </summary>
    public partial class ChooseAvatarControl : UserControl
	{
        private List<string> AvatarList = new List<string>();
        public int SelectedIndex;
        private int BuzzerID;
        private Buzzer buzzer;

        public delegate void ChooseAvatarControlHandler(object sender, ChooseAvatarEventArgs e);

        public event ChooseAvatarControlHandler Finished;

        public void Start(int _buzzerID)
        {
            BuzzerID = _buzzerID;
            if (BuzzerID != 0)
                this.InitBuzzer();
            InitAvatarList();
            Render();
        }

        public ChooseAvatarControl()
        {
            this.InitializeComponent();
        }

        private void InitBuzzer()
        {
            buzzer = Main.buzzer;
            buzzer.Active(BuzzerID);
            buzzer.LightOn(BuzzerID);
            buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
        }

        private void InitAvatarList()
        {
            AvatarList.Clear();
            foreach (string strFile in Directory.GetFiles(@"..\..\Avatars\Southpark", "*.png"))
            {
                AvatarList.Add(strFile);
            }
            SelectedIndex = 0;
        }

        private void AnswerClick(object sender, AnswerClickEventArgs ace)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                if (BuzzerID == ace.Id && buzzer.IsActive(BuzzerID))
                {
                    if (ace.Answer == answercolor.blue)
                        Up();
                    if (ace.Answer == answercolor.yellow)
                        Down();
                    if (ace.Answer == answercolor.red)
                    {
                        Select();                       
                    }
                }
            }));
        }

        private void Render()
        {
            txtName.Text = AvatarList[SelectedIndex].Replace(@"..\..\Avatars\Southpark\", "").Replace(".png", "").ToUpper();
            imgAvatar.Source = (ImageSource) new ImageSourceConverter().ConvertFromString(AvatarList[SelectedIndex]);
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            Up();
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Down();
        } 

        private void Up()
        {
            SelectedIndex--;
            if (SelectedIndex < 0)
            {
                SelectedIndex = AvatarList.Count - 1;
            }
            for (int i=0; i < 4; i++)
            {
                if ((string) new ImageSourceConverter().ConvertToString(Main.players[i].Avatar) == AvatarList[SelectedIndex])
                {
                    Up();
                    break;
                }
            }
            Render();  
        }

        private void Down()
        {
            SelectedIndex++;
            if (SelectedIndex > AvatarList.Count - 1)
            {
                SelectedIndex = 0;
            }
            for (int i=0; i < 4; i++)
            {
                if ((string)new ImageSourceConverter().ConvertToString(Main.players[i].Avatar) == AvatarList[SelectedIndex])
                {
                    Down();
                    break;
                }
            }
            Render();
        }

        private void Select()
        {
            for (int i = 0; i < 4; i++)
            {
                if ((string)new ImageSourceConverter().ConvertToString(Main.players[i].Avatar) == AvatarList[SelectedIndex])
                {
                    return;                    
                }
            }

            Main.PlaySound("-o27 fruitbank.sid", 1);
            buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            buzzer.Inactive(BuzzerID);
            buzzer.LightOff(BuzzerID);
            buttonUp.Visibility = Visibility.Hidden;
            buttonDown.Visibility = Visibility.Hidden;
            buttonSelect.Visibility = Visibility.Hidden;
            Finished(this, new ChooseAvatarEventArgs(BuzzerID));
        }

        private void buttonSelect_Click_1(object sender, RoutedEventArgs e)
        {
            Main.PlaySound("-o27 fruitbank.sid", 1);
            Finished(this, new ChooseAvatarEventArgs(BuzzerID));
        }
	}

    public class ChooseAvatarEventArgs : EventArgs
    {
        public readonly int Id;

        public ChooseAvatarEventArgs(int i)
        {
            this.Id = i;
        }
    }
}