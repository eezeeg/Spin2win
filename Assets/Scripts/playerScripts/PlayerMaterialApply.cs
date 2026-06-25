using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialApply : MonoBehaviour
{
    [Header("marble assign")]
    [SerializeField] private GameObject material1Target;
    [SerializeField] private List<GameObject> insides;
    private void Awake()
    {
        ApplySelectedMaterials();
    }

    private void ApplySelectedMaterials()
    {
        if (material1Target != null && NewMonoBehaviourScript.selectedMat1 != null)
        {
            Renderer renderer = material1Target.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material = NewMonoBehaviourScript.selectedMat1;
            }
        }
        int shapeId = 0;

        foreach (var shape in insides)
        {
            if (shape != null && NewMonoBehaviourScript.selectedMat2 != null)
            {
                Renderer renderer = shape.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material = NewMonoBehaviourScript.selectedMat2;
                }
            }

            if (shape != null)
            {
                shape.SetActive(shapeId == NewMonoBehaviourScript.ShapeId);
            }

            shapeId++;
        }

    }
}
