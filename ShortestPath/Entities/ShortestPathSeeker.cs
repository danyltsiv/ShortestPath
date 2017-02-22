using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortestPath.Entities
{
    public class ShortestPathSeeker<TObj, TKey>
    {
        private readonly Func<TObj, TKey> _fromHandler;
        private readonly Func<TObj, TKey> _toHandler;
        private readonly IEnumerable<TObj> _dataSource;

        private readonly List<List<TObj>> _paths;
        private readonly Stack<TKey> _wasHere;

        public ShortestPathSeeker(Func<TObj, TKey> fromHandler, Func<TObj, TKey> toHandler, IEnumerable<TObj> dataSource)
        {
            _fromHandler = fromHandler;
            _toHandler = toHandler;
            _paths = new List<List<TObj>>();
            _wasHere = new Stack<TKey>();
            _dataSource = dataSource;
        }

        public List<List<TObj>> FindAllPaths(TKey from, TKey to)
        {
            _paths.Clear();
            FindAllPaths(from, to, null);

            return _paths;
        }

        private void FindAllPaths(TKey from, TKey to, List<TObj> pref = null)
        {
            if (_wasHere.Contains(from)) return;
            
            List<TObj> current = pref ?? new List<TObj>();

            _wasHere.Push(from);
            List<TObj> relationsOfFrom = GetRelationsOf(from);

            TObj toRelation = relationsOfFrom.FirstOrDefault(g => _toHandler(g).Equals(to));

            if (relationsOfFrom != null)
            {
                if (toRelation != null)
                {
                    List<TObj> finalPath = new List<TObj>();
                    finalPath.AddRange(current);
                    finalPath.Add(toRelation);
                    _paths.Add(finalPath);
                }

                foreach (var rel in relationsOfFrom)
                {
                    TKey toRel = _toHandler(rel);
                    if (toRel.Equals(to) || _wasHere.Contains(toRel)) continue;
                    current.Add(rel);
                    FindAllPaths(toRel, to, current);
                    current.RemoveAt(current.Count - 1);
                }
            }
            _wasHere.Pop();
        }

        private List<TObj> GetRelationsOf(TKey obj)
        {
            return this._dataSource.Where(g => _fromHandler(g).Equals(obj)).ToList();
        }

        public IEnumerable<List<TObj>> GetShortestPaths(TKey from, TKey to)
        {
            IEnumerable<List<TObj>> paths = FindAllPaths(from, to);
            int minCount = paths.Select(g => g.Count).Min();

            return paths.Where(g => g.Count == minCount);
        }

        public Tuple<List<TObj>, double> GetShortestDistance<TProp>(TKey from, TKey to, Func<TObj, TProp> distancePropertyHanler) where TProp : struct, IConvertible
        {
            List<List<TObj>> paths = FindAllPaths(from, to);
            int shortestIndex = 0;
            double minDist = paths.Select(g => g.Select(
                (p, i) =>
                {
                    shortestIndex = i;
                    return Convert.ToDouble(distancePropertyHanler(p));
                }
                ).Sum()).Min();

            return new Tuple<List<TObj>, double>(paths.ElementAt(shortestIndex), minDist);
        }
    }
}
