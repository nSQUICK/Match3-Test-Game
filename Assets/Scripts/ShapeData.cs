using UnityEngine;

[CreateAssetMenu(fileName = "New Shape", menuName = "Match3/Shape")]
public class ShapeData : ScriptableObject
{
    public ShapeForm form;
    public BorderColor color;
    public AnimalType animal;
    public SpecialEffect specialEffect = SpecialEffect.None;

    [Tooltip("Спрайт мордочки животного (сама рамка берётся по форме).")] // in my case sprite of dota 2 hero picture
    public Sprite faceSprite;
}
