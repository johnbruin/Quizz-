using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Quizz_
{
    public class MusicPlayer
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();

        public void Play(string filename)
        {
            mediaPlayer.Stop();
            mediaPlayer.Open(new Uri(@"../../music/" + filename, UriKind.Relative));
            mediaPlayer.Play();
        }

        public void Stop()
        {
            mediaPlayer.Stop();
        }

        public void Pause()
        {
            mediaPlayer.Pause();
        }
    }
}
