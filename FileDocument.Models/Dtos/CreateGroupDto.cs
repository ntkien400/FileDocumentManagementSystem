using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDocument.Models.Dtos
{
    public class CreateGroupDto
    {
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
