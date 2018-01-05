namespace FastQues.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Questionnaire")]
    public partial class Questionnaire
    {
        public int ID { get; set; }

        [StringLength(100)]
        public string QuestionnaireName { get; set; }

        [StringLength(100)]
        public string Source { get; set; }

        [Column(TypeName = "text")]
        public string Remark { get; set; }
    }
}
