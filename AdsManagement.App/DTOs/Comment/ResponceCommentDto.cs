using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.DTOs.Comment
{
    public class ResponceCommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public int Estimation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
