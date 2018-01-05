namespace FastQues.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Question")]
    public partial class Question
    {
        public int ID { get; set; }

        public int QuestionnaireID { get; set; }

        [StringLength(50)]
        public string QuestionNo { get; set; }

        [StringLength(300)]
        public string QuestionName { get; set; }

        public int? QuestionType { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? SortNo { get; set; }

        public int? IsNotNull { get; set; }

        public string QuestionDescription { get; set; }

        [StringLength(50)]
        public string RangeLength { get; set; }

        [StringLength(300)]
        public string RequiredMsg { get; set; }

        [StringLength(300)]
        public string RangeLengthMsg { get; set; }

        public int? IsEmailValid { get; set; }


    }
}
