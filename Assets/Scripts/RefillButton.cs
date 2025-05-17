using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RefillButton : MonoBehaviour
{
    public ShapeGenerator generator;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            generator.RefillField();
        });
    }
}
