using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MatchmakingService
{
    public class Player
    {
        public int Id { get; set; }
        public int Skill { get; set; }
        public int Lag { get; set; }
        public DateTime EnqueueTime { get; set; }

        public Player(int id, int skill, int lag)
        {
            Id = id;
            Skill = skill;
            Lag = lag;
            EnqueueTime = DateTime.Now;
        }

        public TimeSpan WaitingTime()
        {
            return DateTime.Now - EnqueueTime;
        }
    }

    public class Team
    {
        public List<Player> Players { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }

        public double SkillDifference()
        {
            return Players.Max(p => p.Skill) - Players.Min(p => p.Skill);
        }

        public double LagDifference()
        {
            return Players.Max(p => p.Lag) - Players.Min(p => p.Lag);
        }

        public TimeSpan MaxWaitingTime()
        {
            return Players.Max(p => p.WaitingTime());
        }
    }

    public class MatchmakingService
    {
        private readonly List<Player> queue;
        private readonly int teamSize;
        private readonly object queueLock = new object();

        public MatchmakingService(int teamSize)
        {
            queue = new List<Player>();
            this.teamSize = teamSize;
        }

        public void EnqueuePlayer(Player player)
        {
            lock (queueLock)
            {
                queue.Add(player);
            }
        }

        public List<Team> RunMatchmaking()
        {
            List<Team> teams = new List<Team>();
            List<Player> playersToMatch;

            lock (queueLock)
            {
                // Копируем текущую очередь игроков
                playersToMatch = queue.ToList();
            }

            // Сортируем игроков по времени ожидания, затем по скиллу, затем по лагу
            playersToMatch = playersToMatch.OrderByDescending(p => p.WaitingTime())
                                           .ThenBy(p => p.Skill)
                                           .ThenBy(p => p.Lag)
                                           .ToList();

            // Формируем команды
            while (playersToMatch.Count >= teamSize)
            {
                Team team = new Team();
                List<Player> selectedPlayers = SelectBestTeam(playersToMatch, teamSize);
                team.Players.AddRange(selectedPlayers);
                teams.Add(team);

                // Удаляем выбранных игроков из списка
                foreach (var player in selectedPlayers)
                {
                    playersToMatch.Remove(player);
                }

                lock (queueLock)
                {
                    // Удаляем игроков из основной очереди
                    foreach (var player in selectedPlayers)
                    {
                        queue.Remove(player);
                    }
                }
            }

            return teams;
        }

        private List<Player> SelectBestTeam(List<Player> players, int teamSize)
        {
            // Алгоритм выбора лучшей команды
            List<Player> bestTeam = null;
            double bestScore = double.MaxValue;

            // Ограничиваем количество комбинаций для избежания высокой сложности
            int maxCombinations = 1000;
            int combinationsTried = 0;

            var possibleTeams = GetPossibleTeams(players, teamSize);

            foreach (var teamPlayers in possibleTeams)
            {
                combinationsTried++;
                if (combinationsTried > maxCombinations)
                    break;

                double score = CalculateTeamScore(teamPlayers);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestTeam = teamPlayers;
                }
            }

            if (bestTeam == null)
            {
                // Если не удалось найти лучшую команду, выбираем первых N игроков
                bestTeam = players.Take(teamSize).ToList();
            }

            return bestTeam;
        }

        private IEnumerable<List<Player>> GetPossibleTeams(List<Player> players, int teamSize)
        {
            // Генерация возможных комбинаций игроков для команд
            return GetCombinations(players, teamSize);
        }

        private double CalculateTeamScore(List<Player> teamPlayers)
        {
            double skillDifference = teamPlayers.Max(p => p.Skill) - teamPlayers.Min(p => p.Skill);
            double lagDifference = teamPlayers.Max(p => p.Lag) - teamPlayers.Min(p => p.Lag);
            TimeSpan maxWaitingTime = teamPlayers.Max(p => p.WaitingTime());

            // Весовые коэффициенты для каждого параметра
            const double skillWeight = 0.5;
            const double lagWeight = 0.3;
            const double waitingTimeWeight = 0.2;

            // Нормализация значений
            double normalizedSkillDiff = skillDifference / 1000.0;
            double normalizedLagDiff = lagDifference / 1000.0;
            double normalizedWaitingTime = maxWaitingTime.TotalSeconds / 60.0;

            double score = skillWeight * normalizedSkillDiff
                         + lagWeight * normalizedLagDiff
                         - waitingTimeWeight * normalizedWaitingTime;

            return score;
        }

        private static IEnumerable<List<Player>> GetCombinations(List<Player> list, int length)
        {
            if (length == 1) return list.Select(t => new List<Player> { t });
            return GetCombinations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new List<Player> { t2 }).ToList());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            MatchmakingService matchmakingService = new MatchmakingService(teamSize: 2);

            // Добавляем игроков в очередь
            matchmakingService.EnqueuePlayer(new Player(1, 1050, 50));
            matchmakingService.EnqueuePlayer(new Player(2, 1180, 50));
            matchmakingService.EnqueuePlayer(new Player(3, 1220, 50));
            matchmakingService.EnqueuePlayer(new Player(4, 1350, 50));

            // Ждем, чтобы имитировать время ожидания
            Thread.Sleep(2000);

            // Запускаем матчмейкинг
            List<Team> teams = matchmakingService.RunMatchmaking();

            // Выводим команды
            foreach (var team in teams)
            {
                Console.WriteLine("Команда:");
                foreach (var player in team.Players)
                {
                    Console.WriteLine($"Игрок {player.Id}, Скилл: {player.Skill}, Лаг: {player.Lag}, Ожидание: {player.WaitingTime().TotalSeconds}с");
                }
                Console.WriteLine($"Разница в скиллах: {team.SkillDifference()}");
                Console.WriteLine($"Разница в лаге: {team.LagDifference()}");
                Console.WriteLine($"Максимальное время ожидания: {team.MaxWaitingTime().TotalSeconds}с");
                Console.WriteLine();
            }
        }
    }
}
