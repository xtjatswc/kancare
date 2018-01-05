namespace FastQues.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class FastQuesContext : DbContext
    {
        public FastQuesContext()
            : base("name=FastQuesConn")
        {
        }

        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Questionnaire> Questionnaire { get; set; }
        public virtual DbSet<QuestionOption> QuestionOption { get; set; }
        public virtual DbSet<QuestionRecord> QuestionRecord { get; set; }
        public virtual DbSet<QuestionRecordDetail> QuestionRecordDetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
                .Property(e => e.QuestionNo)
                .IsUnicode(false);

            modelBuilder.Entity<Question>()
                .Property(e => e.QuestionName)
                .IsUnicode(false);

            modelBuilder.Entity<Questionnaire>()
                .Property(e => e.QuestionnaireName)
                .IsUnicode(false);

            modelBuilder.Entity<Questionnaire>()
                .Property(e => e.Source)
                .IsUnicode(false);

            modelBuilder.Entity<Questionnaire>()
                .Property(e => e.Remark)
                .IsUnicode(false);

            modelBuilder.Entity<QuestionOption>()
                .Property(e => e.OptionName)
                .IsUnicode(false);

            modelBuilder.Entity<QuestionOption>()
                .Property(e => e.SortNo)
                .HasPrecision(10, 2);

            modelBuilder.Entity<QuestionRecordDetail>()
                .Property(e => e.OptionValue)
                .IsUnicode(false);
        }
    }
}
