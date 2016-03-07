using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    [Table("LiveScoutEventType")]
    public class LiveScoutEventTypeEntity
    {
        [Key]
        public int Id { get; set; }
        public string EventType { get; set; }
    }
}
