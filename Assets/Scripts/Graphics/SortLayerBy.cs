using UnityEngine;
using UnityEngine.Rendering;

public class SortLayerBy : MonoBehaviour
{
    private SortingGroup sortingGroup;

    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    void Update()
    {
        sortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }
}
