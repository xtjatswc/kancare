namespace FastQues.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuestionRecord")]
    public partial class QuestionRecord
    {
        public int ID { get; set; }

        public int? QuestionnaireID { get; set; }

        public DateTime? RecordTime { get; set; }

        [StringLength(50)]
        public string StatusFlags { get; set; }

        [StringLength(50)]
        public string Province { get; set; }

        [StringLength(50)]
        public string City { get; set; }

    }
}
