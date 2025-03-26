using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int SurveyId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // "single", "multiple", "text"
        public bool IsRequired { get; set; }
        public int QuestionOrder { get; set; }
        public List<AnswerOption> Options { get; set; }
    }
}
