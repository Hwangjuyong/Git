using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models
{
    public class Article
    {
        public int ID { get; set; }
        public string WriterID { get; set; }
        public ApplicationUser Writer { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime cDate { get; set; }

        public ICollection<Reply> Replys { get; set; }
    }
}
