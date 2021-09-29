using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace trialofAD.Models
{
    public class Group
    {
        public int Id { get; set; }
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }
    }
}
