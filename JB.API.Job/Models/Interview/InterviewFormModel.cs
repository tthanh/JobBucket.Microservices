using System.ComponentModel.DataAnnotations.Schema;

namespace JB.Job.Models.Interview
{
    public class InterviewFormModel
    {
        public float OverallRating { get; set; }
        public int Result { get; set; }
        public string Note { get; set; }

        [Column(TypeName = "jsonb")]
        public InterviewFormSectionModel[] Sections { get; set; }
    }
}
