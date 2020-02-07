using System.Windows.Media;

namespace Quizz_
{
    public class Player
    {
        public bool Active
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }

        public ImageSource Avatar
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }

        public int Buzzer
        {
            get;
            set;
        }

        public answercolor Answer
        {
            get;
            set;
        }
    }
}
