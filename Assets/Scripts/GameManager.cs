using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public RectTransform[] barSlots;  // 7 элементов
    public GameObject winPanel;
    public GameObject losePanel;

    private readonly List<ShapeBehaviour> _bar = new();

    private void Awake() => Instance = this;

    public bool TryAddToActionBar(ShapeBehaviour shape)
    {
        if (_bar.Count >= 7) return false;

        _bar.Add(shape);
        shape.TransformTo(barSlots[_bar.Count - 1]);

        // ищем тройку
        var trio = _bar
            .GroupBy(s => s.Data)
            .FirstOrDefault(g => g.Count() >= 3);

        if (trio != null)
        {
            foreach (var s in trio.Take(3))
            {
                _bar.Remove(s);
                Destroy(s.gameObject);
            }
            RepackBar();
            ShapeGenerator.ActiveShapes -= 3;
        }
        else if (_bar.Count == 7)
            losePanel.SetActive(true);

        // победа?
        if (ShapeGenerator.ActiveShapes == 0 && _bar.Count == 0)
            winPanel.SetActive(true);

        return true;
    }

    private void RepackBar()
    {
        for (int i = 0; i < _bar.Count; i++)
            _bar[i].TransformTo(barSlots[i]);
    }
}
