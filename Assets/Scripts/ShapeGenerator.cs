using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    [Header("Spawning")]
    public Transform spawnLeft;
    public Transform spawnRight;
    public GameObject shapePrefab;
    public int totalShapes = 90;
    public int burstSize = 3;
    public float spawnDelay = 0.07f;
    public float overlapRadius = 0.55f;

    public static int ActiveShapes;          // счётчик живых фигур

    private List<ShapeData> _pool;
    private Transform _root;

    private void Start()
    {
        _pool = Resources.LoadAll<ShapeData>("Shapes").ToList();
        if (_pool.Count == 0) { Debug.LogError("Нет ассетов ShapeData!"); return; }

        _root = new GameObject("RuntimeShapes").transform;
        StartCoroutine(SpawnRoutine());
    }

    /* ---------- публичный Refill ---------- */
    public void RefillField()
    {
        int needToSpawn = _root.childCount;
        foreach (Transform child in _root) Destroy(child.gameObject);
        StopAllCoroutines();
        ActiveShapes = 0;
        StartCoroutine(ReSpawnRoutine(needToSpawn));
    }

    /* ---------- Coroutine ---------- */
    private IEnumerator SpawnRoutine()
    {
        var bag = BuildBag(totalShapes).ToList();

        while (bag.Count > 0)
        {
            for (int i = 0; i < burstSize && bag.Count > 0; i++)
            {
                SpawnOne(bag[0]);
                bag.RemoveAt(0);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private IEnumerator ReSpawnRoutine(int value)
    {
        var bag = BuildBag(value).ToList();

        while (bag.Count > 0)
        {
            for (int i = 0; i < burstSize && bag.Count > 0; i++)
            {
                SpawnOne(bag[0]);
                bag.RemoveAt(0);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnOne(ShapeData data)
    {
        const int TRIES = 10;
        for (int tryN = 0; tryN < TRIES; tryN++)
        {
            float x = Random.Range(spawnLeft.position.x, spawnRight.position.x);
            Vector2 pos = new(x, spawnLeft.position.y);

            if (!Physics2D.OverlapCircle(pos, overlapRadius, LayerMask.GetMask("Shapes")))
            {
                InstantiateShape(data, pos);
                return;
            }
        }
        InstantiateShape(data, (Vector2)spawnLeft.position + Vector2.up);
    }

    private void InstantiateShape(ShapeData data, Vector2 pos)
    {
        var go = Instantiate(shapePrefab, pos, Quaternion.identity, _root);
        go.GetComponent<ShapeBehaviour>().Init(data);
        ActiveShapes++;
    }

    private IEnumerable<ShapeData> BuildBag(int count)
    {
        var bag = new List<ShapeData>(count);
        var rnd = _pool.OrderBy(_ => Random.value).ToList();
        int idx = 0;
        while (bag.Count < count)
        {
            bag.AddRange(Enumerable.Repeat(rnd[idx], 3));
            idx = (idx + 1) % rnd.Count;
        }
        return bag.OrderBy(_ => Random.value);
    }
}
