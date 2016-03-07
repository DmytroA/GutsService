using Sportradar.SDK.FeedProviders.Common;
using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guts.Core.Entities
{
    public class MatchUpdateDeltaUpdateEntity
    {
        public MatchUpdateDeltaUpdateEntity() 
        {
            this.Type = GameType.MatchUpdateDeltaUpdate;
        }
        public GameType Type { get; private set; }
        public long MatchId { get; set; }
        public HomeAway<int> Attacks { get; set; }
        public HomeAway<int> BlackCards { get; set; }
        public IdNameTuple Category { get; set; }
        public HomeAway<int> Corners { get; set; }
        public IdNameTuple Court { get; set; }
        public HomeAway<int> DangerousAttacks { get; set; }
        public HomeAway<int> DirectFoulsPeriod { get; set; }

        public HomeAway<int> DirectFreeKicks { get; set; }
        public List<ScoutEvent> Events { get; set; }
        public HomeAway<int> FreeKicks { get; set; }

        public HomeAway<int> FreeThrows { get; set; }

        public HomeAway<int> GoalkeeperSaves { get; set; }

        public HomeAway<int> GoalKicks { get; set; }
        public IceConditions? IceConditions { get; set; }
        public HomeAway<int> Injuries { get; set; }
        public Innings Innings { get; set; }
        public bool? IsTieBreak { get; set; }
        public Team? KickoffTeam { get; set; }
        public Team? KickoffTeamFirstHalf { get; set; }
        public Team? KickoffTeamOt { get; set; }
        public Team? KickoffTeamSecondHalf { get; set; }
        public List<Format> MatchFormat { get; set; }
        public MatchHeader MatchHeader { get; set; }
        public ScoutMatchStatus MatchStatus { get; set; }
        public DateTime MatchStatusStart { get; set; }
        public HomeAway<int> Offsides { get; set; }
        public Team? OpeningFaceoff1StPeriod { get; set; }
        public Team? OpeningFaceoff2NdPeriod { get; set; }
        public Team? OpeningFaceoff3RdPeriod { get; set; }
        public Team? OpeningFaceoffOvertime { get; set; }
        public HomeAway<int> Penalties { get; set; }
        public PitchConditions PitchConditions { get; set; }
        public Team PossesionTeam { get; set; }
        public HomeAway<int?> Possession { get; set; }
        public HomeAway<int> RedCards { get; set; }
        public IDictionary<ScoreType, HomeAway<double>> Score { get; set; }
        public Team Serve { get; set; }
        public HomeAway<int> ShotsBlocked { get; set; }

        public HomeAway<int> ShotsOffTarget { get; set; }

        public HomeAway<int> ShotsOnTarget { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public HomeAway<int> Suspensions { get; set; }

        public HomeAway<int> Throwins { get; set; }
        public IdNameTuple Sport { get; set; }
        public IdNameTuple Tournament { get; set; }
        public WeatherConditions WeatherConditions { get; set; }
        public HomeAway<int> YellowCards { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
