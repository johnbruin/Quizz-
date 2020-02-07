namespace Quizz_
{
    public interface IQuestionList
    {
        Question GetNextQuestion();
        void Initialize(params object[] args);
    }
}
