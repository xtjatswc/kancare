namespace FastQues.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuestionRecordDetail")]
    public partial class QuestionRecordDetail
    {
        public int ID { get; set; }

        public int? QuestionRecordID { get; set; }

        public int? QuestionnaireID { get; set; }

        public int? QuestionID { get; set; }

        public int QuestionOptionID { get; set; }

        [StringLength(500)]
        public string OptionValue { get; set; }
    }
}
