using System;
using System.Collections.Generic;

namespace demo2
{
    
    public class QuizQuestion
    {
        // The text of the question
        public string QuestionText { get; set; }

        // A list of possible answer options
        public List<string> Options { get; set; }

        // The correct answer from the options
        public string CorrectAnswer { get; set; }

        // Explanation shown after user answers
        public string Explanation { get; set; }
    }
}
