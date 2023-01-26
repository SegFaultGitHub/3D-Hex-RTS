using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Code.Interactable;
using UnityEngine;

namespace Code.Tiles {
    public class Tile : MonoBehaviour {
        private LTDescr FogTween, Tween;
        public Dictionary<string, Tile> Neighbours { get; private set; }
        [field: SerializeField] public Vector3Int GridPosition { get; set; }
        [field: SerializeField] public Vector3 Position { private get; set; }
        [field: SerializeField] public bool Walkable { get; set; }
        public float Height { get; set; }
        private bool Visited { get; set; }
        public bool Seen { get; private set; }
        [field: SerializeField] public bool Occupied { get; set; }
        private GameObject Model { get; set; }
        public GameObject Objects { get; private set; }
        private GameObject Fog { get; set; }

        [field: SerializeField] public bool AllowMultipleEntities { get; set; }

        protected virtual void Awake() {
            this.Model = this.transform.Find("Model").gameObject;
            this.Objects = this.transform.Find("Model/Objects").gameObject;
            this.Fog = this.transform.Find("Fog").gameObject;
            this.Neighbours = new Dictionary<string, Tile>();
        }

        public void ArrangeObjects() {
            Vector3 scale = new(1, 1 / this.Height, 1);
            this.Objects.transform.localScale = scale;
        }

        private IEnumerable<Tile> NonWalkableNeighbours(float? stepOffset) {
            return (
                from tile in this.Neighbours.Values
                let heightDifference = stepOffset == null || Mathf.Abs(tile.Height - this.Height) < stepOffset
                where !tile.Walkable && heightDifference && !tile.Occupied
                select tile).ToList();
        }

        private List<Tile> WalkableNeighbours(float? stepOffset) {
            return (
                from tile in this.Neighbours.Values
                let heightDifference = stepOffset == null || Mathf.Abs(tile.Height - this.Height) < stepOffset
                where tile.Walkable && heightDifference && !tile.Occupied
                select tile).ToList();
        }

        public Tile GetRandomNeighbour(float stepOffset, int minDistance, int maxDistance) {
            List<Tile> choices = new();
            Dictionary<Tile, bool> visited = new() {
                [this] = true
            };
            List<Tile> toVisit = this.WalkableNeighbours(stepOffset);

            while (toVisit.Count > 0) {
                for (int i = toVisit.Count - 1; i >= 0; i--) {
                    Tile tile = toVisit[i];
                    if (tile.DistanceFrom(this) >= minDistance)
                        choices.Add(tile);
                    visited[tile] = true;
                    toVisit.Remove(tile);
                    toVisit.AddRange(
                        tile.WalkableNeighbours(stepOffset)
                            .Where(neighbour => !visited.GetValueOrDefault(neighbour, false) && this.DistanceFrom(neighbour) <= maxDistance)
                    );
                }
            }

            return Utils.Utils.Sample(choices);
        }

        public Tile FindNearestTrees(float stepOffset, int maxDistance) {
            Dictionary<Tile, bool> visited = new() {
                [this] = true
            };
            List<Tile> toVisit = this.WalkableNeighbours(stepOffset);

            while (toVisit.Count > 0) {
                for (int i = toVisit.Count - 1; i >= 0; i--) {
                    Tile tile = toVisit[i];

                    Trees trees = tile.GetComponentInChildren<Trees>();
                    if (trees is not null) {
                        Debug.Log("Tree found");
                        return tile;
                    }

                    visited[tile] = true;
                    toVisit.Remove(tile);

                    if (!tile.Walkable) continue;

                    toVisit.AddRange(
                        tile.WalkableNeighbours(stepOffset)
                            .Where(neighbour => !visited.GetValueOrDefault(neighbour, false) && this.DistanceFrom(neighbour) <= maxDistance)
                    );
                    toVisit.AddRange(
                        tile.NonWalkableNeighbours(stepOffset)
                            .Where(neighbour => !visited.GetValueOrDefault(neighbour, false) && this.DistanceFrom(neighbour) <= maxDistance)
                    );
                }
            }

            Debug.Log("No tree found.");
            return null;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public Path PathFind(Tile to, float stepOffset) {
            long start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            float _H(Tile tile) => (tile.Position - to.Position).sqrMagnitude;

            List<Tile> openSet = new() {
                this
            };
            Dictionary<Tile, Tile> cameFrom = new();
            Dictionary<Tile, float> gScore = new() {
                [this] = 0
            };
            Dictionary<Tile, float> fScore = new() {
                [this] = _H(this)
            };

            Tile _CheapestNode(IEnumerable<Tile> tiles) {
                Tile result = null;
                float score = float.MaxValue;
                foreach (Tile tile in tiles) {
                    if (fScore.GetValueOrDefault(tile, float.MaxValue) >= score) continue;
                    score = fScore[tile];
                    result = tile;
                }
                return result;
            }
            Path _Path(Tile _to, bool complete = true) {
                // Debug.Log($"Path found in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms");
                if (_to == this)
                    return new Path {
                        Destination = to,
                        Tiles = new List<Tile>(),
                        Complete = true
                    };

                List<Tile> path = new() {
                    _to
                };
                Tile current = _to;
                while (cameFrom.ContainsKey(current)) {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }
                return new Path {
                    Destination = to,
                    Tiles = path,
                    Complete = complete,
                    RenewAt = (int)(path.Count * 0.75f)
                };
            }

            while (openSet.Count > 0) {
                if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start > 100)
                    return _Path(_CheapestNode(fScore.Keys), false);
                Tile current = _CheapestNode(openSet);
                if (current == to)
                    return _Path(current);

                openSet.Remove(current);
                foreach (Tile neighbour in current.WalkableNeighbours(stepOffset)) {
                    float tentativeGScore = gScore[current] + Mathf.Abs(current.Height - neighbour.Height);
                    if (tentativeGScore >= gScore.GetValueOrDefault(neighbour, float.MaxValue)) continue;
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + _H(neighbour);
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

            return _Path(_CheapestNode(fScore.Keys));
        }

        public int DistanceFrom(Tile other) {
            return (int)((Math.Abs(this.GridPosition.x - other.GridPosition.x)
                          + Math.Abs(this.GridPosition.y - other.GridPosition.y)
                          + Math.Abs(this.GridPosition.z - other.GridPosition.z))
                         / 2f);
        }

        public void SetInvisible() {
            this.Model.SetActive(false);
            this.Model.transform.localScale *= 0;
        }

        private void SetSeen() {
            if (this.Seen || this.Visited)
                return;
            this.Seen = true;
            this.Model.SetActive(true);
            if (this.FogTween != null) {
                LeanTween.cancel(this.FogTween.id);
                this.FogTween = null;
            }
            LeanTween.scale(this.Fog, new Vector3(0, 0, 0), 0.5f).setEaseInBack().setDestroyOnComplete(true);
            this.Tween = LeanTween.scale(this.Model, new Vector3(1, 1, 1), 1f)
                .setDelay(0.25f)
                .setEaseOutBack()
                .setOnComplete(() => this.Tween = null);
        }

        private void SetVisited() {
            if (this.Visited)
                return;
            this.SetSeen();
            this.Visited = true;
        }

        public void Explore(int radius) {
            this.SetVisited();
            Dictionary<Vector3, bool> seenTiles = new() {
                [this.GridPosition] = true
            };
            List<Tile> neighbours = new() {
                this
            };
            for (int i = 0; i < radius; i++)
                neighbours = SeeNeighbours(seenTiles, neighbours);
        }

        public void Feedback() {
            if (!this.Seen) {
                if (this.FogTween != null)
                    return;
                this.FogTween = LeanTween.scale(this.Fog, new Vector3(1.1f, 1.1f, 1.1f), 0.2f)
                    .setLoopPingPong(1)
                    .setEaseOutQuad()
                    .setOnComplete(() => this.FogTween = null);
            } else {
                if (this.Tween != null)
                    return;
                this.Tween = LeanTween.scale(this.Model, new Vector3(1.1f, 1.1f, 1.1f), 0.2f)
                    .setLoopPingPong(1)
                    .setEaseOutQuad()
                    .setOnComplete(() => this.Tween = null);
            }
        }

        private static List<Tile> SeeNeighbours(Dictionary<Vector3, bool> seenTiles, IEnumerable<Tile> tiles) {
            List<Tile> neighbours = new();
            foreach (Tile neighbour in
                     from tile in tiles
                     from neighbour in tile.Neighbours.Values
                     where !seenTiles.GetValueOrDefault(neighbour.GridPosition, false)
                     select neighbour) {
                seenTiles[neighbour.GridPosition] = true;
                neighbours.Add(neighbour);
                neighbour.SetSeen();
            }
            return neighbours;
        }
    }
}
