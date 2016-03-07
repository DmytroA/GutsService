using log4net;
using RabbitMQ.Client;
using Sportradar.SDK.Common.Interfaces;
using Sportradar.SDK.FeedProviders.Common;
using Sportradar.SDK.FeedProviders.LiveScout;
using System;
using System.Linq;
using System.Text;
using System.Timers;
using System.Web.Script.Serialization;
using Guts.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Guts.SendingService
{
    public class LiveScoutModule : IStartable
    {
        private static readonly ILog g_log = LogManager.GetLogger(typeof(LiveScoutModule));
        private readonly string m_feed_name;
        private readonly ILiveScout m_live_scout;
        private readonly Timer m_meta_timer;
        private readonly bool m_test;

        public LiveScoutModule(ILiveScout live_scout, string feed_name, bool test)
        {
            m_live_scout = live_scout;
            m_feed_name = feed_name;
            m_test = test;
            m_live_scout.OnOpened += OpenedHandler;
            m_live_scout.OnClosed += ClosedHandler;
            m_live_scout.OnLineups += LineupsHandler;
            m_live_scout.OnMatchBookingReply += MatchBookingReplyHandler;
            m_live_scout.OnMatchData += MatchDataHandler;
            m_live_scout.OnMatchList += MatchListHandler;
            m_live_scout.OnMatchListUpdate += MatchListUpdateHandler;
            m_live_scout.OnMatchStop += MatchStopHandler;
            m_live_scout.OnMatchUpdate += MatchUpdateHandler;
            m_live_scout.OnMatchUpdateDelta += MatchUpdateDeltaHandler;
            m_live_scout.OnMatchUpdateDeltaUpdate += MatchUpdateDeltaUpdateHandler;
            m_live_scout.OnMatchUpdateFull += MatchUpdateFullHandler;
            m_live_scout.OnOddsSuggestion += OddsSuggestionHandler;
            m_live_scout.OnScoutInfo += ScoutInfoHandler;
            m_live_scout.OnFeedError += FeedErrorHandler;
            m_meta_timer = new Timer(TimeSpan.FromHours(2).TotalMilliseconds);
            m_meta_timer.Elapsed += (sender, args) => m_live_scout.GetMatchList(6, 2, true);
        }

        public void Start()
        {
            g_log.InfoFormat("{0}: Starting", m_feed_name);
            m_live_scout.Start();
            m_meta_timer.Start();
            m_live_scout.GetMatchList(6, 2);
        }

        public void Stop()
        {
            g_log.InfoFormat("{0}: Stopping", m_feed_name);
            m_meta_timer.Stop();
            m_live_scout.Stop();
        }
        private void ClosedHandler(object sender, ConnectionChangeEventArgs e)
        {
            g_log.Info("LiveScout feed disconnected");
            var obj = new ClosedHandlerEntity
            {
               Timestamp = e.LocalTimestamp
            };
            var json = JsonConvert.SerializeObject(obj);
            SendQueue(json);
        }

        private void FeedErrorHandler(object sender, FeedErrorEventArgs e)
        {
            g_log.WarnFormat("{0}: Sending FeedError with {1} severity", m_feed_name, e.Severity.ToString(), e.Cause.ToString(), e.ErrorMessage, e.LocalTimestamp);
            var obj = new FeedErrorEntity
            {
                Cause = e.Cause.ToString(),
                ErrorMessage = e.ErrorMessage,
                LocalStamp = e.LocalTimestamp,
                Severity = e.Severity.ToString()
            };

            var json = JsonConvert.SerializeObject(obj);
            SendQueue(json);
            
            if (e.Severity == ErrorSeverity.CRITICAL)
            {
                Stop();
            }
        }

        private void LineupsHandler(object sender, LineupsEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending Lineups for match {1} with {2} players", m_feed_name, e.Lineups.MatchId, e.Lineups.Players.Count);
            var obj = new LineUpsEntity
            {
                MatchId = e.Lineups.MatchId,
                Managers = e.Lineups.Managers,
                Players = e.Lineups.Players,
                TeamOfficials = e.Lineups.TeamOfficals,
                AdditionalData = e.Lineups.AdditionalData,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchBookingReplyHandler(object sender, MatchBookingReplyEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchBookingReply for match {1} with {2} result", m_feed_name, e.MatchBooking.MatchId, e.MatchBooking.Result);
            var obj = new MatchBookingReply
            {
                MatchId = e.MatchBooking.MatchId,
                Message = e.MatchBooking.Message,
                Result = e.MatchBooking.Result,
                AdditionalData = e.MatchBooking.AdditionalData,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchDataHandler(object sender, MatchDataEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchData for match {1}", m_feed_name, e.MatchData.MatchId, e.MatchData.MatchTime);
            var obj = new MatchDataEntity
            {
                MatchId = e.MatchData.MatchId,
                MatchTime = e.MatchData.MatchTime,
                RemainingTimeInPeriod = e.MatchData.RemainingTimeInPeriod,
                AdditionalData = e.MatchData.AdditionalData,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchListHandler(object sender, MatchListEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchList with {1} matches", m_feed_name, e.MatchList.Length);
            Subscribe(e);
            var obj = new MatchListEntity
            {
                MatchList = e.MatchList,
                WasRequested = e.WasRequested,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchListUpdateHandler(object sender, MatchListEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchListUpdate with {1} matches", m_feed_name, e.MatchList.Length);
            Subscribe(e);
            var obj = new MatchListUpdateEntity
            {
                MatchList = e.MatchList,
                WasRequested = e.WasRequested,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchStopHandler(object sender, MatchStopEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchStop for match {1} for reason {2}", m_feed_name, e.MatchId);
            var obj = new MatchStopEntity
            {
                MatchId = e.MatchId,
                Reason = e.Reason,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchUpdateDeltaHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchUpdateDelta for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            var obj = new MatchUpdateDeltaEntity
            {
                MatchId = e.MatchUpdate.MatchHeader.MatchId,
                Attacks = e.MatchUpdate.Attacks,
                BlackCards = e.MatchUpdate.BlackCards,
                Category = e.MatchUpdate.Category,
                Corners = e.MatchUpdate.Corners,
                Court = e.MatchUpdate.Court,
                DangerousAttacks = e.MatchUpdate.DangerousAttacks,
                DirectFoulsPeriod = e.MatchUpdate.DirectFoulsPeriod,
                DirectFreeKicks = e.MatchUpdate.DirectFreeKicks,
                Events = e.MatchUpdate.Events,
                FreeKicks = e.MatchUpdate.FreeKicks,
                FreeThrows = e.MatchUpdate.FreeThrows,
                GoalkeeperSaves = e.MatchUpdate.GoalkeeperSaves,
                GoalKicks = e.MatchUpdate.GoalKicks,
                IceConditions = e.MatchUpdate.IceConditions,
                Injuries = e.MatchUpdate.Injuries,
                Innings = e.MatchUpdate.Innings,
                IsTieBreak = e.MatchUpdate.IsTieBreak,
                KickoffTeam = e.MatchUpdate.KickoffTeam,
                KickoffTeamFirstHalf = e.MatchUpdate.KickoffTeamFirstHalf,
                KickoffTeamOt = e.MatchUpdate.KickoffTeamOt,
                KickoffTeamSecondHalf = e.MatchUpdate.KickoffTeamSecondHalf,
                MatchFormat = e.MatchUpdate.MatchFormat,
                MatchHeader = e.MatchUpdate.MatchHeader,
                MatchStatus = e.MatchUpdate.MatchStatus,
                MatchStatusStart = e.MatchUpdate.MatchStatusStart,
                Offsides = e.MatchUpdate.Offsides,
                OpeningFaceoff1StPeriod = e.MatchUpdate.OpeningFaceoff1StPeriod,
                OpeningFaceoff2NdPeriod = e.MatchUpdate.OpeningFaceoff2NdPeriod,
                OpeningFaceoff3RdPeriod = e.MatchUpdate.OpeningFaceoff3RdPeriod,
                OpeningFaceoffOvertime = e.MatchUpdate.OpeningFaceoffOvertime,
                Penalties = e.MatchUpdate.Penalties,
                PitchConditions = e.MatchUpdate.PitchConditions,
                PossesionTeam = e.MatchUpdate.PossesionTeam,
                Possession = e.MatchUpdate.Possession,
                RedCards = e.MatchUpdate.RedCards,
                Score = e.MatchUpdate.Score,
                Serve = e.MatchUpdate.Serve,
                ShotsBlocked = e.MatchUpdate.ShotsBlocked,
                ShotsOffTarget = e.MatchUpdate.ShotsOffTarget,
                ShotsOnTarget = e.MatchUpdate.ShotsOnTarget,
                Sport = e.MatchUpdate.Sport,
                SurfaceType = e.MatchUpdate.SurfaceType,
                Suspensions = e.MatchUpdate.Suspensions,
                Throwins = e.MatchUpdate.Throwins,
                Tournament = e.MatchUpdate.Tournament,
                WeatherConditions = e.MatchUpdate.WeatherConditions,
                YellowCards = e.MatchUpdate.YellowCards,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void MatchUpdateDeltaUpdateHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchUpdateDeltaUpdate for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            var obj = new MatchUpdateDeltaUpdateEntity
            {
                MatchId = e.MatchUpdate.MatchHeader.MatchId,
                Attacks = e.MatchUpdate.Attacks,
                BlackCards = e.MatchUpdate.BlackCards,
                Category = e.MatchUpdate.Category,
                Corners = e.MatchUpdate.Corners,
                Court = e.MatchUpdate.Court,
                DangerousAttacks = e.MatchUpdate.DangerousAttacks,
                DirectFoulsPeriod = e.MatchUpdate.DirectFoulsPeriod,
                DirectFreeKicks = e.MatchUpdate.DirectFreeKicks,
                Events = e.MatchUpdate.Events,
                FreeKicks = e.MatchUpdate.FreeKicks,
                FreeThrows = e.MatchUpdate.FreeThrows,
                GoalkeeperSaves = e.MatchUpdate.GoalkeeperSaves,
                GoalKicks = e.MatchUpdate.GoalKicks,
                IceConditions = e.MatchUpdate.IceConditions,
                Injuries = e.MatchUpdate.Injuries,
                Innings = e.MatchUpdate.Innings,
                IsTieBreak = e.MatchUpdate.IsTieBreak,
                KickoffTeam = e.MatchUpdate.KickoffTeam,
                KickoffTeamFirstHalf = e.MatchUpdate.KickoffTeamFirstHalf,
                KickoffTeamOt = e.MatchUpdate.KickoffTeamOt,
                KickoffTeamSecondHalf = e.MatchUpdate.KickoffTeamSecondHalf,
                MatchFormat = e.MatchUpdate.MatchFormat,
                MatchHeader = e.MatchUpdate.MatchHeader,
                MatchStatus = e.MatchUpdate.MatchStatus,
                MatchStatusStart = e.MatchUpdate.MatchStatusStart,
                Offsides = e.MatchUpdate.Offsides,
                OpeningFaceoff1StPeriod = e.MatchUpdate.OpeningFaceoff1StPeriod,
                OpeningFaceoff2NdPeriod = e.MatchUpdate.OpeningFaceoff2NdPeriod,
                OpeningFaceoff3RdPeriod = e.MatchUpdate.OpeningFaceoff3RdPeriod,
                OpeningFaceoffOvertime = e.MatchUpdate.OpeningFaceoffOvertime,
                Penalties = e.MatchUpdate.Penalties,
                PitchConditions = e.MatchUpdate.PitchConditions,
                PossesionTeam = e.MatchUpdate.PossesionTeam,
                Possession = e.MatchUpdate.Possession,
                RedCards = e.MatchUpdate.RedCards,
                Score = e.MatchUpdate.Score,
                Serve = e.MatchUpdate.Serve,
                ShotsBlocked = e.MatchUpdate.ShotsBlocked,
                ShotsOffTarget = e.MatchUpdate.ShotsOffTarget,
                ShotsOnTarget = e.MatchUpdate.ShotsOnTarget,
                Sport = e.MatchUpdate.Sport,
                SurfaceType = e.MatchUpdate.SurfaceType,
                Suspensions = e.MatchUpdate.Suspensions,
                Throwins = e.MatchUpdate.Throwins,
                Tournament = e.MatchUpdate.Tournament,
                WeatherConditions = e.MatchUpdate.WeatherConditions,
                YellowCards = e.MatchUpdate.YellowCards,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj);
            SendQueue(json);
        }

        private void MatchUpdateFullHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchUpdateFull for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            var obj = new MatchUpdateFullEntity
            {
                MatchId = e.MatchUpdate.MatchHeader.MatchId,
                Attacks = e.MatchUpdate.Attacks,
                BlackCards = e.MatchUpdate.BlackCards,
                Category = e.MatchUpdate.Category,
                Corners = e.MatchUpdate.Corners,
                Court = e.MatchUpdate.Court,
                DangerousAttacks = e.MatchUpdate.DangerousAttacks,
                DirectFoulsPeriod = e.MatchUpdate.DirectFoulsPeriod,
                DirectFreeKicks = e.MatchUpdate.DirectFreeKicks,
                Events = e.MatchUpdate.Events,
                FreeKicks = e.MatchUpdate.FreeKicks,
                FreeThrows = e.MatchUpdate.FreeThrows,
                GoalkeeperSaves = e.MatchUpdate.GoalkeeperSaves,
                GoalKicks = e.MatchUpdate.GoalKicks,
                IceConditions = e.MatchUpdate.IceConditions,
                Injuries = e.MatchUpdate.Injuries,
                Innings = e.MatchUpdate.Innings,
                IsTieBreak = e.MatchUpdate.IsTieBreak,
                KickoffTeam = e.MatchUpdate.KickoffTeam,
                KickoffTeamFirstHalf = e.MatchUpdate.KickoffTeamFirstHalf,
                KickoffTeamOt = e.MatchUpdate.KickoffTeamOt,
                KickoffTeamSecondHalf = e.MatchUpdate.KickoffTeamSecondHalf,
                MatchFormat = e.MatchUpdate.MatchFormat,
                MatchHeader = e.MatchUpdate.MatchHeader,
                MatchStatus = e.MatchUpdate.MatchStatus,
                MatchStatusStart = e.MatchUpdate.MatchStatusStart,
                Offsides = e.MatchUpdate.Offsides,
                OpeningFaceoff1StPeriod = e.MatchUpdate.OpeningFaceoff1StPeriod,
                OpeningFaceoff2NdPeriod = e.MatchUpdate.OpeningFaceoff2NdPeriod,
                OpeningFaceoff3RdPeriod = e.MatchUpdate.OpeningFaceoff3RdPeriod,
                OpeningFaceoffOvertime = e.MatchUpdate.OpeningFaceoffOvertime,
                Penalties = e.MatchUpdate.Penalties,
                PitchConditions = e.MatchUpdate.PitchConditions,
                PossesionTeam = e.MatchUpdate.PossesionTeam,
                Possession = e.MatchUpdate.Possession,
                RedCards = e.MatchUpdate.RedCards,
                Score = e.MatchUpdate.Score,
                Serve = e.MatchUpdate.Serve,
                ShotsBlocked = e.MatchUpdate.ShotsBlocked,
                ShotsOffTarget = e.MatchUpdate.ShotsOffTarget,
                ShotsOnTarget = e.MatchUpdate.ShotsOnTarget,
                Sport = e.MatchUpdate.Sport,
                SurfaceType = e.MatchUpdate.SurfaceType,
                Suspensions = e.MatchUpdate.Suspensions,
                Throwins = e.MatchUpdate.Throwins,
                Tournament = e.MatchUpdate.Tournament,
                WeatherConditions = e.MatchUpdate.WeatherConditions,
                YellowCards = e.MatchUpdate.YellowCards,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj);
            SendQueue(json);
        }

        private void MatchUpdateHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending MatchUpdate for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId, e.MatchUpdate.MatchHeader);
            var obj = new MatchUpdateEntity
            {
                MatchId = e.MatchUpdate.MatchHeader.MatchId,
                Attacks = e.MatchUpdate.Attacks,
                BlackCards = e.MatchUpdate.BlackCards,
                Category = e.MatchUpdate.Category,
                Corners = e.MatchUpdate.Corners,
                Court = e.MatchUpdate.Court,
                DangerousAttacks = e.MatchUpdate.DangerousAttacks,
                DirectFoulsPeriod = e.MatchUpdate.DirectFoulsPeriod,
                DirectFreeKicks = e.MatchUpdate.DirectFreeKicks,
                Events = e.MatchUpdate.Events,
                FreeKicks = e.MatchUpdate.FreeKicks,
                FreeThrows = e.MatchUpdate.FreeThrows,
                GoalkeeperSaves = e.MatchUpdate.GoalkeeperSaves,
                GoalKicks = e.MatchUpdate.GoalKicks,
                IceConditions = e.MatchUpdate.IceConditions,
                Injuries = e.MatchUpdate.Injuries,
                Innings = e.MatchUpdate.Innings,
                IsTieBreak = e.MatchUpdate.IsTieBreak,
                KickoffTeam = e.MatchUpdate.KickoffTeam,
                KickoffTeamFirstHalf = e.MatchUpdate.KickoffTeamFirstHalf,
                KickoffTeamOt = e.MatchUpdate.KickoffTeamOt,
                KickoffTeamSecondHalf = e.MatchUpdate.KickoffTeamSecondHalf,
                MatchFormat = e.MatchUpdate.MatchFormat,
                MatchHeader = e.MatchUpdate.MatchHeader,
                MatchStatus = e.MatchUpdate.MatchStatus,
                MatchStatusStart = e.MatchUpdate.MatchStatusStart,
                Offsides = e.MatchUpdate.Offsides,
                OpeningFaceoff1StPeriod = e.MatchUpdate.OpeningFaceoff1StPeriod,
                OpeningFaceoff2NdPeriod = e.MatchUpdate.OpeningFaceoff2NdPeriod,
                OpeningFaceoff3RdPeriod = e.MatchUpdate.OpeningFaceoff3RdPeriod,
                OpeningFaceoffOvertime = e.MatchUpdate.OpeningFaceoffOvertime,
                Penalties = e.MatchUpdate.Penalties,
                PitchConditions = e.MatchUpdate.PitchConditions,
                PossesionTeam = e.MatchUpdate.PossesionTeam,
                Possession = e.MatchUpdate.Possession,
                RedCards = e.MatchUpdate.RedCards,
                Score = e.MatchUpdate.Score,
                Serve = e.MatchUpdate.Serve,
                ShotsBlocked = e.MatchUpdate.ShotsBlocked,
                ShotsOffTarget = e.MatchUpdate.ShotsOffTarget,
                ShotsOnTarget = e.MatchUpdate.ShotsOnTarget,
                Sport = e.MatchUpdate.Sport,
                SurfaceType = e.MatchUpdate.SurfaceType,
                Suspensions = e.MatchUpdate.Suspensions,
                Throwins = e.MatchUpdate.Throwins,
                Tournament = e.MatchUpdate.Tournament,
                WeatherConditions = e.MatchUpdate.WeatherConditions,
                YellowCards = e.MatchUpdate.YellowCards,
                Timestamp = DateTime.UtcNow
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void OddsSuggestionHandler(object sender, OddsSuggestionEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending OddsSuggestion for match {1} with {2} odds", m_feed_name, e.MatchId, e.Odds.Length);
            var obj = new OddsSuggestionEntity
            {
                MatchId = e.MatchId,
                OddLength = e.Odds.Length,
                Odds = e.Odds,
                Timestamp = DateTime.UtcNow,
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }

        private void OpenedHandler(object sender, ConnectionChangeEventArgs connection_change_event_args)
        {
            g_log.Info("LiveScout feed connected");
            var obj = new OpenedHandlerEntity
            {
                Timestamp = DateTime.UtcNow,
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }
        private void ScoutInfoHandler(object sender, ScoutInfoEventArgs e)
        {
            g_log.InfoFormat("{0}: Sending ScoutInfo for match {1} with {2} infos", m_feed_name, e.MatchId, e.ScoutInfos.Length);
            var obj = new ScoutInfoEntity
            {
                MatchId = e.MatchId,
                ScoutInfoLength = e.ScoutInfos.Length,
                Timestamp = DateTime.UtcNow,
                ScoutInfos = e.ScoutInfos
            };
            var json = JsonConvert.SerializeObject(obj, new IsoDateTimeConverter());
            SendQueue(json);
        }
        private void SendQueue(string json)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "LiveScout",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;                

                channel.BasicPublish(exchange: "",
                                     routingKey: "LiveScout",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(" [x] Sent ");
            }
        }
        private void Subscribe(MatchListEventArgs e)
        {
            var to_subscribe = e.MatchList
                .Where(x => x.MatchHeader.IsBooked == true || x.MatchHeader.IsBooked == null)
                .Select(x => x.MatchHeader.MatchId)
                .ToList();
            g_log.InfoFormat("{0}: Subscribing to {1} events", m_feed_name, to_subscribe.Count);
            if (m_test)
            {
                g_log.InfoFormat("Test subscribing to {0} events", to_subscribe.Count);
                foreach (long id in to_subscribe)
                {
                    m_live_scout.SubscribeTest(id);
                }
            }
            else
            {
                g_log.InfoFormat("Subscribing to {0} events", to_subscribe.Count);

                const int MAX_COUNT = 100;
                while (to_subscribe.Any())
                {
                    m_live_scout.Subscribe(to_subscribe.Take(MAX_COUNT));
                    to_subscribe = to_subscribe.Skip(MAX_COUNT).ToList();
                }
            }
        }
    }
}