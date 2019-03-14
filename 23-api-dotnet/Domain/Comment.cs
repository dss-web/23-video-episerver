using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual.Domain
{
    public class Comment
    {
        public int? CommentId;

        public int? ObjectId;
        public CommentObjectType ObjectType;

        public int? UserId;
        public string Name;
        public string Email;
        public string TruncatedName;

        public string PrettyDate;
        public string PrettyTime;

        public string ShortDate;
        public string CreationDateANSI;

        public string ContentText;
        public string Content;
    }
}
