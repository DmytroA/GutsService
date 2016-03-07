using Guts.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reciever
{
    public class GamePlayDataContext : DbContext 
    {
        public GamePlayDataContext()
            : base ("GutsLiveScoutEntities")
        {
            
        }
        public DbSet<LiveScoutEntity> LiveScouts { get; set; }
        public DbSet<LiveScoutEventTypyEntity> LiveScoutEvents { get; set; }
        public DbSet<LiveScoutJsonEntity> LiveScoutJsons { get; set; }
    }
}
