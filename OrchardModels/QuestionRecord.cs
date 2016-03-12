namespace JabbR.Models
{
    public class QuestionRecord {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Question { get; set; }
        public virtual string Answer { get; set; }
        public virtual int Delay { get; set; }
        public virtual bool Active { get; set; }

    }
}