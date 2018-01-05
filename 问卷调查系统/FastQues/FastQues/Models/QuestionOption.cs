namespace FastQues.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuestionOption")]
    public partial class QuestionOption
    {
        public int ID { get; set; }

        [StringLength(200)]
        public string OptionName { get; set; }

        public int? QuestionID { get; set; }

        public decimal? SortNo { get; set; }

        public int? OptionType { get; set; }

        public int? QuestionnaireID { get; set; }
    }
}
