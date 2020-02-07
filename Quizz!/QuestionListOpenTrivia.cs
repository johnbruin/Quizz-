using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quizz_
{
    public class QuestionListOpenTrivia : IQuestionList
    {
        private JToken questions;
        private JToken current;
        private static readonly string session = Guid.NewGuid().ToString().Substring(0,8);

        public void Initialize(params object[] args)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(@"https://opentdb.com/");

                //var response = client.GetAsync($"api.php?amount=5&token={session}").Result;
                var response = client.GetAsync($"api.php?amount=7&category=11&difficulty=easy").Result;
                response.EnsureSuccessStatusCode();
                var rawMessage = response.Content.ReadAsStringAsync().Result;
                var msgJObject = (JObject)JsonConvert.DeserializeObject(rawMessage);
                if (msgJObject["response_code"].ToObject<int>() != 0)
                    throw new ApplicationException("Error retrieving questions");
                questions = msgJObject["results"];
            }
        }

        const string strNumbers = "1234";

        public Question GetNextQuestion()
        {
            current = current?.Next ?? questions.First;

            var q = new Question
            {
                text = System.Net.WebUtility.HtmlDecode(current["question"].Value<string>())
            };

            switch (current["type"].Value<string>())
            {
                case "boolean":
                    q.CorrectAnswer = current["correct_answer"].Value<string>().ToUpper() == "TRUE"
                        ? answercolor.blue
                        : answercolor.orange;
                    q.answerBlue = "True";
                    q.answerOrange = "False";
                    break;
                case "multiple":
                    var choices = current["incorrect_answers"].Values<string>()
                        .Concat(new[] { current["correct_answer"].Value<string>() }).ToArray();
                    var shuffle = Shuffle(strNumbers);
                    q.CorrectAnswer = (answercolor)shuffle.IndexOf('4');
                    q.answerBlue = System.Net.WebUtility.HtmlDecode(choices[int.Parse(shuffle.Substring(0, 1)) - 1]);
                    q.answerOrange = System.Net.WebUtility.HtmlDecode(choices[int.Parse(shuffle.Substring(1, 1)) - 1]);
                    q.answerGreen = System.Net.WebUtility.HtmlDecode(choices[int.Parse(shuffle.Substring(2, 1)) - 1]);
                    q.answerYellow = System.Net.WebUtility.HtmlDecode(choices[int.Parse(shuffle.Substring(3, 1)) - 1]);
                    break;
            }
            return q;
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
