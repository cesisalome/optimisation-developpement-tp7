using System.Diagnostics;

namespace optimisation_developpement_tp7
{
    public class Program
    {
        // Liste statique pour stocker tous les logs en mémoire
        private static readonly List<Log> AllLogs = [];
        // Index pour requêtes ultra-rapides (hors filtrage par timestamp)
        private static readonly HashSet<string> AllUserIds = [];
        private static readonly Dictionary<string, int> AllActionCounts = [];
        private static readonly Dictionary<(string, string), int> AllUserActionCounts = [];
        
        public static void Main()
        {
            // Chargement des logs en mémoire au démarrage
            LoadAllLogs();

            // Exemple d'utilisation
            long t1 = 0, t2 = long.MaxValue;
            string userId = "qyvthinu.gxreh@fvqcku.example";
            string actionId = "9e6ded42-5b55-bfa5-f493-392b6bc53bb1";
            int k = 5;

            Console.WriteLine($"Q1: Utilisateurs distincts = {CountDistinctUserIds(t1, t2)}");
            Console.WriteLine($"Q2: Top {k} actions = {string.Join(", ", TopKActions(k, t1, t2))}");
            Console.WriteLine($"Q3: Nb d'occurrences (user, action) = {CountUserAction(userId, actionId, t1, t2)}");
            Console.WriteLine($"Mémoire utilisée : {GC.GetTotalMemory(false) / (1024 * 1024)} Mo");
        }

        // Chargement de tous les logs en mémoire et création des index
        private static void LoadAllLogs()
        {
            Stopwatch sw = Stopwatch.StartNew();
            AllLogs.Clear();
            AllUserIds.Clear();
            AllActionCounts.Clear();
            AllUserActionCounts.Clear();
            foreach (var file in Directory.EnumerateFiles("data", "*.csv"))
            using (var reader = new StreamReader(file))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var log = new Log {
                        Timestamp = long.Parse(parts[0]),
                        UserId = parts[1],
                        ActionId = Guid.Parse(parts[2])
                    };
                    AllLogs.Add(log);
                    // Indexation
                    AllUserIds.Add(log.UserId);
                    var actionKey = log.ActionId.ToString();
                    if (!AllActionCounts.TryAdd(actionKey, 1))
                        AllActionCounts[actionKey]++;
                    var userActionKey = (log.UserId, actionKey);
                    if (!AllUserActionCounts.TryAdd(userActionKey, 1))
                        AllUserActionCounts[userActionKey]++;
                }
            }
            sw.Stop();
            Console.WriteLine($"All logs loaded and indexed in {sw.ElapsedMilliseconds} ms");
        }

        // Q1 – Nombre d’utilisateurs distincts
        public static int CountDistinctUserIds(long? t1 = 0, long? t2 = long.MaxValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int result;
            if (t1 == 0 && t2 == long.MaxValue)
            {
                result = AllUserIds.Count;
            }
            else
            {
                var userIds = new HashSet<string>();
                foreach (var log in AllLogs)
                    if (log.Timestamp >= t1 && log.Timestamp <= t2)
                        userIds.Add(log.UserId);
                result = userIds.Count;
            }
            sw.Stop();
            Console.WriteLine($"Q1 executed in {sw.ElapsedMilliseconds} ms");
            return result;
        }

        // Q2 – Top-k des actions les plus fréquentes
        public static List<string> TopKActions(int k, long? t1 = 0, long? t2 = long.MaxValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<string> result;
            if (t1 == 0 && t2 == long.MaxValue)
            {
                result = [.. AllActionCounts
                    .OrderByDescending(pair => pair.Value)
                    .Take(k)
                    .Select(pair => pair.Key)];
            }
            else
            {
                var actionCounts = new Dictionary<string, int>();
                foreach (var log in AllLogs)
                {
                    if (log.Timestamp >= t1 && log.Timestamp <= t2)
                    {
                        var key = log.ActionId.ToString();
                        if (!actionCounts.TryAdd(key, 1))
                            actionCounts[key]++;
                    }
                }
                result = [.. actionCounts
                    .OrderByDescending(pair => pair.Value)
                    .Take(k)
                    .Select(pair => pair.Key)];
            }
            sw.Stop();
            Console.WriteLine($"Q2 executed in {sw.ElapsedMilliseconds} ms");
            return result;
        }

        // Q3 – Nombre d’occurrences d’un couple
        public static int CountUserAction(string userId, string actionId, long? t1 = 0, long? t2 = long.MaxValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int result;
            if (t1 == 0 && t2 == long.MaxValue)
            {
                result = AllUserActionCounts.TryGetValue((userId, actionId), out int count) ? count : 0;
            }
            else
            {
                int count = 0;
                foreach (var log in AllLogs)
                {
                    if (log.Timestamp >= t1 && log.Timestamp <= t2 &&
                        log.UserId == userId && log.ActionId.ToString() == actionId)
                        count++;
                }
                result = count;
            }
            sw.Stop();
            Console.WriteLine($"Q3 executed in {sw.ElapsedMilliseconds} ms");
            return result;
        }
    }
}