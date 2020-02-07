using HidLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

/*
Buzzer 1
knop   3: 0000 0001
blauw  3: 0001 0000
oranje 3: 0000 1000
groen  3: 0000 0100
geel   3: 0000 0010

Buzzer 2
Knop   3: 0010 0000
blauw  4: 0000 0010
oranje 4: 0000 0001
groen  3: 1000 0000
geel   3: 0100 0000

Buzzer 3
Knop   4: 0000 0100
blauw  4: 0100 0000
oranje 4: 0010 0000
groen  4: 0001 0000
geel   4: 0000 1000

Buzzer 4
knop   4: 1000 0000
blauw  5: 1111 1000
oranje 5: 1111 0100
groen  5: 1111 0010
geel   5: 1111 0001
*/

namespace Quizz_
{
    public enum answercolor {
        blue,
        orange,
        green,
        yellow,
        red
    }

    public class AnswerClickEventArgs : EventArgs
    {
        public readonly int Id;
        public readonly answercolor Answer;

        public AnswerClickEventArgs(int i, answercolor a)
        {
            this.Id = i;
            this.Answer = a;
        }         
    }

    public class Buzzer 
    {
        private bool[] ActiveBuzzers = new bool[4];

        public void Active(int BuzzerID)
        {
            ActiveBuzzers[BuzzerID - 1] = true;
        }

        public void Inactive(int BuzzerID)
        {
            ActiveBuzzers[BuzzerID - 1] = false;
        }

        public bool IsActive(int BuzzerID)
        {
            return ActiveBuzzers[BuzzerID - 1];
        }

        private HidDevice _device;
        private const int VendorId = 0x054C;
        private const int ProductId = 0x1000;

        private byte[] Lights;

        public delegate void AnswerHandler(object sender, AnswerClickEventArgs ace);
        public event AnswerHandler OnAnswerClick;
        
        public Buzzer()
        {
            ActiveBuzzers[0] = true;
            ActiveBuzzers[1] = true;
            ActiveBuzzers[2] = true;
            ActiveBuzzers[3] = true;

            try
            {
                _device = HidDevices.Enumerate(VendorId, ProductId).FirstOrDefault();
                if (_device != null)
                {
                    _device.MonitorDeviceEvents = true;
                    _device.Removed += _device_Removed;
                    _device.Inserted += _device_Inserted;
                    _device.OpenDevice();
                    _device.MonitorDeviceEvents = true;
                    _device.ReadReport(OnReport);
                }
                else
                {
                    MessageBox.Show("PLEASE CONNECT BUZZERS");
                }
            }
            catch
            {

            }

            Lights = new byte[9];

            for (int i = 0; i < Lights.Length - 1; i++)
            {
                Lights[i] = 0;
            }
            this.SendData(Lights);
        }

        private void OnReport(HidReport report)
        {
            try
            {
                var output = report.Data;

                Debug.WriteLine(ShowBits(output[0]));
                Debug.WriteLine(ShowBits(output[1]));
                Debug.WriteLine(ShowBits(output[2]));
                Debug.WriteLine(ShowBits(output[3]));
                Debug.WriteLine(ShowBits(output[4]));
                Debug.WriteLine(" ");
                //return;

                //Buzzer 1
                if (IsBitSet(output[2], 5)) OnAnswerClick(this, new AnswerClickEventArgs(1, answercolor.blue));
                if (IsBitSet(output[2], 4)) OnAnswerClick(this, new AnswerClickEventArgs(1, answercolor.orange));
                if (IsBitSet(output[2], 3)) OnAnswerClick(this, new AnswerClickEventArgs(1, answercolor.green));
                if (IsBitSet(output[2], 2)) OnAnswerClick(this, new AnswerClickEventArgs(1, answercolor.yellow));
                if (IsBitSet(output[2], 1)) OnAnswerClick(this, new AnswerClickEventArgs(1, answercolor.red));

                //Buzzer 2
                if (IsBitSet(output[3], 2)) OnAnswerClick(this, new AnswerClickEventArgs(2, answercolor.blue));
                if (IsBitSet(output[3], 1)) OnAnswerClick(this, new AnswerClickEventArgs(2, answercolor.orange));
                if (IsBitSet(output[2], 8)) OnAnswerClick(this, new AnswerClickEventArgs(2, answercolor.green));
                if (IsBitSet(output[2], 7)) OnAnswerClick(this, new AnswerClickEventArgs(2, answercolor.yellow));
                if (IsBitSet(output[2], 6)) OnAnswerClick(this, new AnswerClickEventArgs(2, answercolor.red));

                //Buzzer 3
                if (IsBitSet(output[3], 7)) OnAnswerClick(this, new AnswerClickEventArgs(3, answercolor.blue));
                if (IsBitSet(output[3], 6)) OnAnswerClick(this, new AnswerClickEventArgs(3, answercolor.orange));
                if (IsBitSet(output[3], 5)) OnAnswerClick(this, new AnswerClickEventArgs(3, answercolor.green));
                if (IsBitSet(output[3], 4)) OnAnswerClick(this, new AnswerClickEventArgs(3, answercolor.yellow));
                if (IsBitSet(output[3], 3)) OnAnswerClick(this, new AnswerClickEventArgs(3, answercolor.red));

                //Buzzer 4
                if (IsBitSet(output[4], 4)) OnAnswerClick(this, new AnswerClickEventArgs(4, answercolor.blue));
                if (IsBitSet(output[4], 3)) OnAnswerClick(this, new AnswerClickEventArgs(4, answercolor.orange));
                if (IsBitSet(output[4], 2)) OnAnswerClick(this, new AnswerClickEventArgs(4, answercolor.green));
                if (IsBitSet(output[4], 1)) OnAnswerClick(this, new AnswerClickEventArgs(4, answercolor.yellow));
                if (IsBitSet(output[3], 8)) OnAnswerClick(this, new AnswerClickEventArgs(4, answercolor.red));
            }
            catch
            {

            }
            finally
            {
                _device.ReadReport(OnReport);
            }
        }

        private void SendData(byte[] data)
        {
            HidReport report = new HidReport(data.Length);
            report.Data = data;
            _device.WriteReport(report);
        }

        public void LightOff(int BuzzerID)
        {
            Lights[BuzzerID] = 0;
            this.SendData(Lights);
        }

        public bool IsLightOn(int BuzzerID)
        {
            return (Lights[BuzzerID] == 255);
        }

        public void LightOn(int BuzzerID)
        {
            Lights[BuzzerID] = 255;
            this.SendData(Lights);
        }

        private void _device_Inserted()
        {
            //MessageBox.Show("BUZZERS ARE READY");
        }

        private void _device_Removed()
        {
            MessageBox.Show("PLEASE CONNECT BUZZERS");
        }

        private string ShowBits (byte val)
        { 
            string strResult = "";
            for(int t=128; t > 0; t = t/2)
            { 
              if((val & t) != 0) strResult = strResult + "1";  
              if((val & t) == 0) strResult = strResult + "0";
            }
            return strResult;
        }

        private bool IsBitSet(byte val, int position)
        {
            string strResult = "";
            for (int t = 128; t > 0; t = t / 2)
            {
                if ((val & t) != 0) strResult = strResult + "1";
                if ((val & t) == 0) strResult = strResult + "0";
            }
            return (strResult.Substring(strResult.Length - position, 1) == "1");
        }
    }
}
