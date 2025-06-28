using System.Collections.Generic;
using UnityEngine;
using VisualNovel.GameJam.Manager;

public class UnitSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private List<EnemyPrefabPair> enemyPrefabs;

    [Header("Positions")]
    [SerializeField] private RectTransform playerPosition;
    [SerializeField] private RectTransform companionPosition;
    [SerializeField] private List<RectTransform> enemyPositions;

    private Dictionary<EnemyType, GameObject> prefabMap;

    private void Awake()
    {
        BuildPrefabMap();
    }

    public CombatUnit SpawnPlayer()
    {
        return Instantiate(playerPrefab, playerPosition).GetComponent<CombatUnit>();
    }

    public CombatUnit SpawnCompanion()
    {
        return Instantiate(companionPrefab, companionPosition).GetComponent<CombatUnit>();
    }

    public List<CombatUnit> SpawnEnemies(List<EnemyType> enemyTypes)
    {
        List<CombatUnit> enemies = new();
        int index = 0;

        foreach (var type in enemyTypes)
        {
            if (prefabMap.TryGetValue(type, out GameObject prefab) && index < enemyPositions.Count)
            {
                CombatUnit enemy = Instantiate(prefab, enemyPositions[index]).GetComponent<CombatUnit>();
                enemies.Add(enemy);
                index++;
            }
        }

        return enemies;
    }

    private void BuildPrefabMap()
    {
        prefabMap = new Dictionary<EnemyType, GameObject>();
        foreach (var pair in enemyPrefabs)
        {
            if (!prefabMap.ContainsKey(pair.type))
                prefabMap.Add(pair.type, pair.prefab);
        }
    }
}
