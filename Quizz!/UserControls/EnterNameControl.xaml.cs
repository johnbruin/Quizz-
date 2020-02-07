using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Quizz_
{
    public class EnterNameEventArgs : EventArgs
    {
        public readonly int Id;

        public EnterNameEventArgs(int i)
        {
            this.Id = i;
        }
    }

	/// <summary>
	/// Interaction logic for EnterNameControl.xaml
	/// </summary>
	public partial class EnterNameControl : UserControl
	{
        private string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?/-@*&$'";
        private List<string> CharacterList;
        private int SelectedIndex = 3;
        private int BuzzerID;
        private Buzzer buzzer;

        public delegate void EnterNameControlHandler(object sender, EnterNameEventArgs e);

        public event EnterNameControlHandler Finished;

        public void Start(int _buzzerID)
        {
            BuzzerID = _buzzerID;
            if (BuzzerID != 0)
                this.InitBuzzer();
        }

		public EnterNameControl()
		{
			this.InitializeComponent();
            this.InitCharacterList(); 
		}

        private void InitBuzzer()
        {
            buzzer = Main.buzzer;
            buzzer.Active(BuzzerID);
            buzzer.LightOn(BuzzerID);
            buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
        }

        private void InitCharacterList()
        {
            CharacterList = new List<string>();
            CharacterList.Add("SPACE");
            CharacterList.Add("CLEAR");
            CharacterList.Add("DEL");
            CharacterList.Add("DONE");
            for (int i = 0; i < Characters.Length; i++)
            {
                CharacterList.Add(Characters.Substring(i, 1));
            }
            Render();
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
                        Select();
                }
            }));
        }

        private void Render()
        {
            if (SelectedIndex == 0)
            {
                this.lbl1.Content = CharacterList[CharacterList.Count - 2];
                this.lbl2.Content = CharacterList[CharacterList.Count - 1];
            }
            else if (SelectedIndex == 1)
            {
                this.lbl1.Content = CharacterList[CharacterList.Count - 1];
                this.lbl2.Content = CharacterList[SelectedIndex - 1];
            }
            else
            {
                this.lbl1.Content = CharacterList[SelectedIndex - 2];
                this.lbl2.Content = CharacterList[SelectedIndex - 1];
            }
            this.lbl3.Content = CharacterList[SelectedIndex];
            
            if (SelectedIndex == CharacterList.Count - 1)
            {
                this.lbl4.Content = CharacterList[0];
                this.lbl5.Content = CharacterList[1];
            }
            else if (SelectedIndex == CharacterList.Count - 2)
            {
                this.lbl4.Content = CharacterList[SelectedIndex + 1];
                this.lbl5.Content = CharacterList[0];
            }
            else
            {
                this.lbl4.Content = CharacterList[SelectedIndex + 1];
                this.lbl5.Content = CharacterList[SelectedIndex + 2];
            }
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
            if (buzzer != null) buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            SelectedIndex--;
            if (SelectedIndex < 0)
            {
                SelectedIndex = CharacterList.Count - 1;
            }
            Render();
            if (buzzer != null) buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
        }

        private void Down()
        {
            if (buzzer != null) buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
            SelectedIndex++;
            if (SelectedIndex > CharacterList.Count - 1)
            {
                SelectedIndex = 0;
            }
            Render();
            if (buzzer != null) buzzer.OnAnswerClick += new Buzzer.AnswerHandler(AnswerClick);
        }

        private void Select()
        {
            if (CharacterList[SelectedIndex] == "DONE")
            {
                if (txtName.Text.Trim() != "")
                {
                    Main.PlaySound("-o27 fruitbank.sid", 1);
                    txtName.Text = txtName.Text.Trim();
                    if (BuzzerID > 0)
                    {
                        buzzer.LightOff(BuzzerID);
                        buzzer.Inactive(BuzzerID);
                    }
                    stackChooser.Visibility = Visibility.Hidden;
                    if (BuzzerID > 0)
                        buzzer.OnAnswerClick -= new Buzzer.AnswerHandler(AnswerClick);
                    Finished(this, new EnterNameEventArgs(BuzzerID));                    
                }
            }
            else if (CharacterList[SelectedIndex] == "CLEAR")
            {
                txtName.Text = "";
            }
            else if (CharacterList[SelectedIndex] == "SPACE")
            {
                txtName.Text = txtName.Text + " ";
            }
            else if (CharacterList[SelectedIndex] == "DEL")
            {
                if (txtName.Text.Length > 0)
                {
                    txtName.Text = txtName.Text.Substring(0, txtName.Text.Length - 1);
                }
            }
            else
            {
                if (txtName.Text.Length < 10)
                    txtName.Text = txtName.Text + CharacterList[SelectedIndex];
            }
        }
       
	}
}