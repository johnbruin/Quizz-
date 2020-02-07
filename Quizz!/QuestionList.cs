﻿using System;
using System.Data;

namespace Quizz_
{
    public class QuestionList
    {
        private DataSet QuestionsDS = new DataSet();
        public int Count
        {
            get
            {
                if (QuestionsDS.Tables["question"] == null)
                {
                    return 0;
                }
                else
                {
                    return QuestionsDS.Tables["question"].Rows.Count;
                }
            }
        }

        public void ReadQuestions(string filename)
        {
            QuestionsDS.ReadXml(filename);            
        }

        public Question GetRandomQuestion()
        {
            DataRowCollection dr = QuestionsDS.Tables["question"].Rows;
            if (dr.Count > 0)
            {
                Random RandomNumber = new Random();
                int r = RandomNumber.Next(dr.Count);
                Question question = new Question();
                question.text = dr[r]["text"].ToString();
                string strNumbers = "1234";
                strNumbers = Shuffle(strNumbers);
                question.answerBlue = dr[r]["answer" + strNumbers.Substring(0, 1)].ToString();
                if (strNumbers.Substring(0, 1).ToString() == "1") question.CorrectAnswer = answercolor.blue;
                question.answerOrange = dr[r]["answer" + strNumbers.Substring(1, 1)].ToString();
                if (strNumbers.Substring(1, 1).ToString() == "1") question.CorrectAnswer = answercolor.orange;
                question.answerGreen = dr[r]["answer" + strNumbers.Substring(2, 1)].ToString();
                if (strNumbers.Substring(2, 1).ToString() == "1") question.CorrectAnswer = answercolor.green;
                question.answerYellow = dr[r]["answer" + strNumbers.Substring(3, 1)].ToString();
                if (strNumbers.Substring(3, 1).ToString() == "1") question.CorrectAnswer = answercolor.yellow;
                question.media = dr[r]["media"].ToString();

                dr[r].Delete();
                QuestionsDS.Tables["question"].AcceptChanges();

                return question;
            }
            else
            {
                return null;
            }
        }

        public static string Shuffle(string input)
        {
            string output = "";
            Random rnd = new Random();
            while (input.Length > 0)
            {
                int copyfrom = rnd.Next(input.Length);
                output += input[copyfrom];
                input = input.Remove(copyfrom, 1);
            }
            return output;
        }
    }
}
