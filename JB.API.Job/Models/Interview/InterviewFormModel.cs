using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Interview
{
    public class InterviewFormModel
    {
        public string Title { get; set; }
        public string Round { get; set; }
        public string Note { get; set; }

        [Column(TypeName = "jsonb")]
        public InterviewFormSectionModel[] Sections { get; set; }
    }
}
