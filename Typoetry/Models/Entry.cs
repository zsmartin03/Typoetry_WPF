using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typoetry.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }

        public Entry(int id, string title, string author, string text) {
            this.Id = id;
            this.Title = title;
            this.Author = author;
            this.Text = text;
        }
    }
}
