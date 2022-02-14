using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JB.Blog.DTOs.Blog
{
    public class BlogUserResponse
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public int OrganizationId { get; set; }
    }
}
