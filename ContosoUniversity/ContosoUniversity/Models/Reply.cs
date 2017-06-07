using System;

namespace ContosoUniversity.Models
{
    public class Reply
    {
        public int ID { get; set; }
        public string WriterID { get; set; }
        public ApplicationUser Writer { get; set; }
        public string Content { get; set; }        
        public DateTime cDate { get; set; }

        public int ArticleID { get; set; }
        public Article Article{ get; set; }
    }
}
