using System;
using System.Collections.Generic;
using Code.Camera;
using Code.Characters;
using Code.Enums;
using Code.Extensions;
using Code.Interactable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Tiles {
    public class Map : MonoBehaviour {
        private const float TILE_SIZE = 2;
        private const float TERRAIN_NOISE_SCALE = 0.075f;
        private const float TREE_NOISE_SCALE = 0.06f;
        private const float TERRAIN_NOISE_MULTIPLIER = 7.5f;

        private const float WATER_THRESHOLD = 0.3f;
        private const float SAND_THRESHOLD = 0.35f;
        private const float GRASS_THRESHOLD = 0.7f;
        private const float TREE_THRESHOLD = 0.05f;

        private const float BASE_LEVEL = SAND_THRESHOLD;

        private const int BASE_SIZE = 3;
        private const int BASE_TRANSITION_SIZE = 5;
        private const int MINE_MIN_DISTANCE = 3;

        [field: SerializeField] private List<Player> StartingPlayers;

        private Vector3Int BasePosition;

        private Transform Camera;
        private List<Tile> MineTiles;
        private float NoiseOffsetTerrain;
        private float NoiseOffsetTrees;
        private float NoiseOffsetTreesDetails;

        private Tile PlayerCastle;
        private Dictionary<Vector3Int, Tile> Tiles;

        private Transform TilesTransform, CharactersTransform;

        [field: SerializeField] private int Seed { get; set; }
        [field: SerializeField] private MapType MapType { get; set; }
        [field: SerializeField] private Tile Water { get; set; }
        [field: SerializeField] private Tile Sand { get; set; }
        [field: SerializeField] private Tile Grass { get; set; }
        [field: SerializeField] private Tile Rock { get; set; }
        [field: SerializeField] private Trees Trees { get; set; }
        [field: SerializeField] private List<GameObject> TreesDetails { get; set; }
        [field: SerializeField] private Castle Castle { get; set; }
        [field: SerializeField] private Mine Mine { get; set; }
        [field: SerializeField] private int Radius { get; set; }


        private void Start() {
            LeanTween.init(1200);
            this.TilesTransform = this.transform.Find("Tiles");
            this.CharactersTransform = this.transform.Find("Characters");
            this.Generate();
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            this.Camera.GetComponent<CameraController>()
                .LookAt(this.PlayerCastle.transform.position + new Vector3(0, this.PlayerCastle.Height, 0), 0.1f);
        }

        private void Generate() {
            long start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Debug.Log("Map generation started...");
            if (this.Seed == 0) this.Seed = Random.Range(0, 10_000_000);
            Random.InitState(this.Seed);
            Debug.Log($"Map seed: {this.Seed}");

            this.NoiseOffsetTerrain = Random.Range(0, 10_000_000);
            this.NoiseOffsetTrees = Random.Range(0, 10_000_000);
            this.NoiseOffsetTreesDetails = Random.Range(0, 10_000_000);

            this.BasePosition = new Vector3Int(
                Random.Range(BASE_SIZE, this.Radius / 2 - BASE_SIZE - 2) * (Utils.Utils.Rate(.5f) ? -1 : 1),
                Random.Range(BASE_SIZE, this.Radius / 2 - BASE_SIZE - 2) * (Utils.Utils.Rate(.5f) ? -1 : 1)
            );
            this.BasePosition.z = -this.BasePosition.x - this.BasePosition.y;

            Tile startingTile = this.GenerateTile(Vector3.zero, Vector3Int.zero);
            List<Tile> tiles = new() {
                startingTile
            };
            this.Tiles = new Dictionary<Vector3Int, Tile> {
                [Vector3Int.zero] = startingTile
            };
            this.MineTiles = new List<Tile>();
            this.PlayerCastle = null;

            for (int i = 0; i < this.Radius; i++) tiles = this.GenerateNeighbours(tiles);
            foreach (Tile tile in this.Tiles.Values) {
                for (int i = 0; i < 6; i++) {
                    Tile neighbour = this.Tiles.GetValueOrDefault(tile.GridPosition + AngleToCoordinates(60 * i), null);
                    if (neighbour == null) continue;
                    tile.Neighbours[AngleToDirection(60 * i)] = neighbour;
                }
            }

            if (this.MapType == MapType.Playable) {
                Tile tile = Utils.Utils.Sample(this.MineTiles);
                Mine mine = Instantiate(this.Mine, tile.Objects.transform);
                tile.name = $"Mine - {tile.name}";
                mine.transform.LookAt(this.PlayerCastle!.transform);
                tile.Walkable = false;
                tile.AllowMultipleEntities = true;
                this.PlayerCastle.Explore(10);
            }

            Debug.Log(
                $"Map generation done:\n\t{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms\n\t{this.Tiles.Keys.Count} tiles generated"
            );
        }

        private List<Tile> GenerateNeighbours(List<Tile> tiles) {
            List<Tile> neighbours = new();
            foreach (Tile tile in tiles) {
                for (int i = 0; i < 6; i++) {
                    Vector3 position = tile.transform.position
                                       + new Vector3(
                                           TILE_SIZE * Mathf.Cos(60 * i * Mathf.Deg2Rad),
                                           0,
                                           TILE_SIZE * Mathf.Sin(60 * i * Mathf.Deg2Rad)
                                       );
                    Vector3Int gridPosition = tile.GridPosition + AngleToCoordinates(60 * i);
                    if (this.Tiles.GetValueOrDefault(gridPosition, null) != null) continue;
                    Tile neighbour = this.GenerateTile(position, gridPosition);
                    this.Tiles[gridPosition] = neighbour;
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }

        private Tile GenerateTile(Vector3 position, Vector3Int gridPosition) {
            float terrainNoise;
            float treeNoise;

            float distanceFromBase = HexDistance(gridPosition, this.BasePosition);

            switch (this.MapType) {
                case MapType.Playable when distanceFromBase <= BASE_SIZE:
                    terrainNoise = BASE_LEVEL;
                    treeNoise = 0;
                    break;
                case MapType.Playable when distanceFromBase <= BASE_SIZE + BASE_TRANSITION_SIZE: {
                    float ratio = (distanceFromBase - BASE_SIZE) / BASE_TRANSITION_SIZE;
                    terrainNoise = Mathf.PerlinNoise(
                        (position.x + this.NoiseOffsetTerrain) * TERRAIN_NOISE_SCALE,
                        (position.z + this.NoiseOffsetTerrain) * TERRAIN_NOISE_SCALE
                    );
                    treeNoise = Mathf.PerlinNoise(
                        (position.x + this.NoiseOffsetTrees) * TREE_NOISE_SCALE,
                        (position.z + this.NoiseOffsetTrees) * TREE_NOISE_SCALE
                    );

                    terrainNoise = BASE_LEVEL + (terrainNoise - BASE_LEVEL) * ratio;
                    break;
                }
                case MapType.MainTitle:
                default:
                    terrainNoise = Mathf.PerlinNoise(
                        (position.x + this.NoiseOffsetTerrain) * TERRAIN_NOISE_SCALE,
                        (position.z + this.NoiseOffsetTerrain) * TERRAIN_NOISE_SCALE
                    );
                    treeNoise = Mathf.PerlinNoise(
                        (position.x + this.NoiseOffsetTrees) * TREE_NOISE_SCALE,
                        (position.z + this.NoiseOffsetTrees) * TREE_NOISE_SCALE
                    );
                    break;
            }

            Tile tile;
            switch (terrainNoise) {
                case < WATER_THRESHOLD:
                    // Water
                    tile = Instantiate(this.Water, this.TilesTransform);
                    tile.name = $"Water - {gridPosition.ToString()}";
                    break;
                case < SAND_THRESHOLD: {
                    // Sand
                    tile = Instantiate(this.Sand, this.TilesTransform);
                    tile.name = $"Sand - {gridPosition.ToString()}";
                    Vector3 scale = new(1, 1, 1);
                    scale.y *= 1 + (terrainNoise - WATER_THRESHOLD) * TERRAIN_NOISE_MULTIPLIER;
                    tile.transform.localScale = scale;
                    break;
                }
                case < GRASS_THRESHOLD: {
                    // Grass
                    tile = Instantiate(this.Grass, this.TilesTransform);
                    tile.name = $"Grass - {gridPosition.ToString()}";
                    Vector3 scale = new(1, 1, 1);
                    scale.y *= 1 + (terrainNoise - WATER_THRESHOLD) * TERRAIN_NOISE_MULTIPLIER;
                    tile.transform.localScale = scale;

                    if (treeNoise is > 0.5f - TREE_THRESHOLD and < 0.5f + TREE_THRESHOLD) {
                        float diff = Mathf.Abs(treeNoise - 0.5f);
                        float treesDetailsNoise = Mathf.PerlinNoise(
                            (position.x + this.NoiseOffsetTreesDetails) * TREE_NOISE_SCALE,
                            (position.z + this.NoiseOffsetTreesDetails) * TREE_NOISE_SCALE
                        );
                        GameObject trees;
                        if (diff > TREE_THRESHOLD * 0.8f && treesDetailsNoise > 0.5f) {
                            trees = Instantiate(Utils.Utils.Sample(this.TreesDetails), tile.Objects.transform);
                        } else {
                            trees = Instantiate(this.Trees, tile.Objects.transform).gameObject;
                        }

                        trees.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                        tile.Walkable = false;
                    }

                    break;
                }
                default: {
                    // Rock
                    tile = Instantiate(this.Rock, this.TilesTransform);
                    tile.name = $"Rock - {gridPosition.ToString()}";
                    Vector3 scale = new(1, 1, 1);
                    scale.y *= 1 + (terrainNoise - WATER_THRESHOLD) * TERRAIN_NOISE_MULTIPLIER;
                    tile.transform.localScale = scale;
                    break;
                }
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (this.MapType) {
                case MapType.Playable when gridPosition == this.BasePosition: {
                    Castle castle = Instantiate(this.Castle, tile.Objects.transform);
                    this.OnEndOfFrame(
                        () => {
                            castle.SetCompleted(false);
                            foreach (Player startingPlayer in this.StartingPlayers) {
                                castle.SpawnPlayer(startingPlayer);
                            }
                        }
                    );
                    tile.name = $"Castle - {tile.name}";
                    castle.transform.eulerAngles = new Vector3(
                        0,
                        -Vector2.SignedAngle(new Vector2(0, 1), new Vector2(position.x, position.z)) + 180,
                        0
                    );
                    this.PlayerCastle = tile;
                    tile.Walkable = false;
                    break;
                }
                case MapType.Playable when distanceFromBase is >= MINE_MIN_DISTANCE and <= BASE_SIZE:
                    this.MineTiles.Add(tile);
                    break;
            }

            Transform tileTransform = tile.transform;
            tile.Height = tileTransform.localScale.y;
            tileTransform.position = position;
            tile.GridPosition = gridPosition;
            tile.Position = tileTransform.position;
            tile.ArrangeObjects();
            if (this.MapType == MapType.Playable) tile.SetInvisible();

            return tile;
        }

        private static Vector3Int AngleToCoordinates(float angle) {
            return angle switch {
                0 => new Vector3Int(1, 0, -1),
                60 => new Vector3Int(1, -1, 0),
                120 => new Vector3Int(0, -1, 1),
                180 => new Vector3Int(-1, 0, 1),
                240 => new Vector3Int(-1, 1, 0),
                300 => new Vector3Int(0, 1, -1),
                _ => throw new Exception("[Map:AngleToCoordinates] Invalid angle.")
            };
        }

        private static string AngleToDirection(float angle) {
            return angle switch {
                0 => "East",
                60 => "South-east",
                120 => "South-west",
                180 => "West",
                240 => "North-west",
                300 => "North-east",
                _ => throw new Exception("[Map:AngleToDirection] Invalid angle.")
            };
        }

        private static int HexDistance(Vector3 pos1, Vector3 pos2) {
            return (int)((Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y) + Math.Abs(pos1.z - pos2.z)) / 2f);
        }
    }
}
