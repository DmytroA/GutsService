using Guts.Core.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    [JsonConverter(typeof(GamePlayJsonConverter))]
    [Table("LiveScout")]
    public class LiveScoutEntity
    {
        [Key]
        [JsonIgnore]
        public long Id { get; set; }

        [JsonProperty("Timestamp")]
        public DateTime LocalStamp { get; set; }

        [NotMapped]
        [JsonProperty("Type")]
        public GameType EventType
        {
            get
            {
                return (GameType)this.EventTypeId;
            }
            set
            {
                this.EventTypeId = (int)value;
            }
        }

        public long LiveScoutJsonId { get; set; }
        public int EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public LiveScoutEventTypyEntity Event { get; set; }

        [ForeignKey("LiveScoutJsonId")]
        public LiveScoutJsonEntity Json { get; set; }

        //public string JsonText { get; set; }
    }
}
