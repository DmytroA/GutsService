using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    [Table("LiveScoutRawData")]
    public class LiveScoutJsonEntity
    {
        [Key]
        public long Id { get; set; }
        public string Data { get;set; }
    }
}
