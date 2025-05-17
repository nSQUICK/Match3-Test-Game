using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;          // GetPhysicsShape

[RequireComponent(typeof(Rigidbody2D))]
public class ShapeBehaviour : MonoBehaviour
{
    public SpriteRenderer frameR;
    public SpriteRenderer faceR;

    [HideInInspector] public ShapeData Data;

    /* --- палитра рамки --- */
    static readonly Color32 RED = new(0xF5, 0x5B, 0x5B, 255);
    static readonly Color32 GREEN = new(0x4E, 0xD1, 0x6D, 255);
    static readonly Color32 BLUE = new(0x4E, 0x9A, 0xFF, 255);

    public void Init(ShapeData data)
    {
        Data = data;

        // лицо + рамка + цвет
        faceR.sprite = data.faceSprite;
        frameR.sprite = Resources.Load<Sprite>($"ShapeFrames/{data.form}Frame");
        frameR.color = data.color switch
        {
            BorderColor.Red => RED,
            BorderColor.Green => GREEN,
            _ => BLUE
        };

        // спец-эффект (пока только Heavy)
        var rb = GetComponent<Rigidbody2D>();
        rb.mass = data.specialEffect == SpecialEffect.Heavy ? 2f : 1f;

        RebuildCollider();
    }

    /* ---------- клик ---------- */
    private bool _clicked;
    public void OnMouseDown()
    {
        Debug.Log($"CLICK on {name}", this);
        if (_clicked) return;
        if (GameManager.Instance.TryAddToActionBar(this))
        {
            _clicked = true;
            GetComponent<Rigidbody2D>().simulated = false;
        }
    }

    /* ---------- плавное перемещение в слот ---------- */
    public void TransformTo(RectTransform slot) => StartCoroutine(Fly(slot));
    private IEnumerator Fly(RectTransform slot)
    {
        Vector3 s = transform.position;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(s, slot.position, t);
            yield return null;
        }
        transform.SetParent(slot);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one*40;
        transform.localRotation = Quaternion.EulerAngles(0,0,0);
    }

    /* ---------- Collider 2D, учитывая масштаб ---------- */
    private void RebuildCollider()
    {
        DestroyImmediate(GetComponent<Collider2D>());

        if (Data.form == ShapeForm.Circle)
        {
            var circ = gameObject.AddComponent<CircleCollider2D>();
            float radiusWorld = frameR.bounds.extents.x;
            circ.radius = radiusWorld / transform.lossyScale.x;
            return;
        }

        PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>();
        var sprite = frameR.sprite;

        if (sprite.GetPhysicsShapeCount() == 0)
        {
            poly.pathCount = 1;
            poly.SetPath(0, new[]{ new Vector2(-.5f,-.5f), new(.5f,-.5f),
                                   new(.5f,.5f),   new(-.5f,.5f) });
            return;
        }

        poly.pathCount = sprite.GetPhysicsShapeCount();
        for (int i = 0; i < poly.pathCount; i++)
        {
            var pts = new List<Vector2>();
            sprite.GetPhysicsShape(i, pts);

            for (int p = 0; p < pts.Count; p++)
            {
                Vector3 world = frameR.transform.TransformPoint(pts[p]);
                pts[p] = transform.InverseTransformPoint(world);
            }
            poly.SetPath(i, pts);
        }
    }

}
