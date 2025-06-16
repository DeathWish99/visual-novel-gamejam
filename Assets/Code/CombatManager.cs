using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private GameObject companionPrefab;
    [SerializeField] private List<EnemyPrefabPair> enemyPrefabs;

    private Dictionary<EnemyType, GameObject> PrefabMap { get; set; }
    private List<CombatUnit> Units { get; set; }

    private void Awake()
    {
        PrefabMap = new Dictionary<EnemyType, GameObject>();

        foreach (var pair in enemyPrefabs)
        {
            if (!PrefabMap.ContainsKey(pair.type)) PrefabMap.Add(pair.type, pair.prefab);
        }

        Units = new List<CombatUnit>();

        SetUp();
    }

    private void SetUp()
    {
        CombatSceneParameters parameters = GameManager.Instance.CurrentParameters;

        if (parameters.HasCompanion)
        {
            Units.Add(Instantiate(companionPrefab).GetComponent<CombatUnit>());
        }

        foreach (var enemyType in parameters.Enemies)
        {
            if (PrefabMap.TryGetValue(enemyType, out GameObject prefab))
            {
                Units.Add(Instantiate(prefab).GetComponent<CombatUnit>());
            }
        }
    }

    private void SetUpTurnOrder()
    {
        Units.Sort((a, b) =>
        {
            if (a.Stats.Speed == b.Stats.Speed)
            {
                if (a.Stats.IsPlayer) return -1;
                if (b.Stats.IsPlayer) return 1;
                return 0;
            }
            return b.Stats.Speed.CompareTo(a.Stats.Speed);
        });
    }
}
