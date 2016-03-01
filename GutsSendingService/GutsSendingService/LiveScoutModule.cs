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

namespace Guts.SendingService
{
    public class LiveScoutModule : IStartable
    {
        private static readonly ILog g_log = LogManager.GetLogger(typeof(LiveScoutModule));
        private readonly string m_feed_name;
        private readonly ILiveScout m_live_scout;
        private readonly Timer m_meta_timer;
        private readonly bool m_test;
        //private bool _doWork;

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
            m_meta_timer.Elapsed += (sender, args) => m_live_scout.GetMatchList(6, 2);
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
        }

        private void FeedErrorHandler(object sender, FeedErrorEventArgs e)
        {
            g_log.WarnFormat("{0}: Received FeedError with {1} severity", m_feed_name, e.Severity);
            if (e.Severity == ErrorSeverity.CRITICAL)
            {
                Stop();
            }
        }

        private void LineupsHandler(object sender, LineupsEventArgs e)
        {
            g_log.InfoFormat("{0}: Received Lineups for match {1} with {2} players", m_feed_name, e.Lineups.MatchId, e.Lineups.Players.Count);
            var obj = new LineUpsEntity
            {
                MatchId = e.Lineups.MatchId,
                PlayersCount = e.Lineups.Players.Count,
                ManagersCount = e.Lineups.Managers.Count,
                TeamOfficals = e.Lineups.TeamOfficals.Count
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            SendQueue(json);
        }

        private void MatchBookingReplyHandler(object sender, MatchBookingReplyEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchBookingReply for match {1} with {2} result", m_feed_name, e.MatchBooking.MatchId, e.MatchBooking.Result);
            var obj = new MatchBookingReply
            {
                MatchId = e.MatchBooking.MatchId,
                Message = e.MatchBooking.Message,
                Result = e.MatchBooking.Result.ToString()
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            SendQueue(json);
        }

        private void MatchDataHandler(object sender, MatchDataEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchData for match {1}", m_feed_name, e.MatchData.MatchId, e.MatchData.MatchTime);
        }

        private void MatchListHandler(object sender, MatchListEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchList with {1} matches", m_feed_name, e.MatchList.Length);
            Subscribe(e);
        }

        private void MatchListUpdateHandler(object sender, MatchListEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchListUpdate with {1} matches", m_feed_name, e.MatchList.Length);
            Subscribe(e);
        }

        private void MatchStopHandler(object sender, MatchStopEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchStop for match {1} for reason {2}", m_feed_name, e.MatchId, e.Reason);
            var obj = new MatchStopEntity
            {
                MatchId = e.MatchId,
                Reason = e.Reason
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            SendQueue(json);
        }

        private void MatchUpdateDeltaHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchUpdateDelta for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            g_log.InfoFormat("{0}: ALANA", e.ToString());
        }

        private void MatchUpdateDeltaUpdateHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchUpdateDeltaUpdate for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            g_log.InfoFormat("{0}: ALANA", e.ToString());
        }

        private void MatchUpdateFullHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchUpdateFull for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            //var obj = new MatchUpdateFullEntity
            //{
            //    MatchId = e.MatchUpdate.MatchHeader.MatchId,
            //    AttacksTeam1 = e.MatchUpdate.Attacks.Team1,
            //    AttackTeam2 = e.MatchUpdate.Attacks.Team2,
            //    RedCardsTeam1 = e.MatchUpdate.RedCards.Team1,
            //    RedCardsTeam2 = e.MatchUpdate.RedCards.Team2,
            //    PossessionTeam1 = e.MatchUpdate.Possession.Team1,
            //    PossessionTeam2 = e.MatchUpdate.Possession.Team2
            //};
            //var json = new JavaScriptSerializer().Serialize(obj);
            //SendQueue(json);
        }

        private void MatchUpdateHandler(object sender, MatchUpdateEventArgs e)
        {
            g_log.InfoFormat("{0}: Received MatchUpdate for match {1}", m_feed_name, e.MatchUpdate.MatchHeader.MatchId);
            var obj = new MatchUpdateEntity
            {
                MatchId = e.MatchUpdate.MatchHeader.MatchId,
                BetStatus = e.MatchUpdate.MatchHeader.BetStatus.ToString()
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            SendQueue(json);
        }

        private void OddsSuggestionHandler(object sender, OddsSuggestionEventArgs e)
        {
            g_log.InfoFormat("{0}: Received OddsSuggestion for match {1} with {2} odds", m_feed_name, e.MatchId, e.Odds.Length);
            var oddObj = new OddsSuggestionEntity
            {
                MatchId = e.MatchId,
                OddLength = e.Odds.Length
            };
            var json = new JavaScriptSerializer().Serialize(oddObj);
            SendQueue(json);
        }

        private void OpenedHandler(object sender, ConnectionChangeEventArgs connection_change_event_args)
        {
            g_log.Info("LiveScout feed connected");
        }
        private void ScoutInfoHandler(object sender, ScoutInfoEventArgs e)
        {
            g_log.InfoFormat("{0}: Received ScoutInfo for match {1} with {2} infos", m_feed_name, e.MatchId, e.ScoutInfos.Length);
            var obj = new ScoutInfoEntity
            {
                MatchId = e.MatchId,
                ScoutInfoLength = e.ScoutInfos.Length
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            SendQueue(json);
        }
        private void SendQueue(string json)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "sendEntity",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.SetPersistent(true);

                channel.BasicPublish(exchange: "",
                                     routingKey: "sendEntity",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", json);
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
                //Max 100 events in single request
                const int MAX_COUNT = 3;
                while (to_subscribe.Any())
                {
                    m_live_scout.Subscribe(to_subscribe.Take(MAX_COUNT));
                    to_subscribe = to_subscribe.Skip(MAX_COUNT).ToList();
                }
            }
        }
    }
}