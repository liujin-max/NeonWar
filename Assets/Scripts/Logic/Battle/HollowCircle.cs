using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class HollowCircle : MonoBehaviour
{
    public float OuterRadius = 0.55f;
    public float InnerRadius = 0.5f;
    public int OuterSegments = 64;
    public int InnerSegments = 64;


    void Start()
    {
        GenerateHollowCircle();
    }

    void GenerateHollowCircle()
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

        Vector2[] outerVertices = new Vector2[OuterSegments];
        Vector2[] innerVertices = new Vector2[InnerSegments];

        {
            float angleStep = 2 * Mathf.PI / OuterSegments;

            for (int i = 0; i < OuterSegments; i++)
            {
                float angle = i * angleStep;
                outerVertices[i] = new Vector2(Mathf.Cos(angle) * OuterRadius, Mathf.Sin(angle) * OuterRadius);
            }
        }

        {
            float angleStep = 2 * Mathf.PI / InnerSegments;

            for (int i = 0; i < InnerSegments; i++)
            {
                float angle = i * angleStep;
                innerVertices[i] = new Vector2(Mathf.Cos(angle) * InnerRadius, Mathf.Sin(angle) * InnerRadius);
            }
        }


        polygonCollider.pathCount = 2;
        polygonCollider.SetPath(0, outerVertices);
        polygonCollider.SetPath(1, innerVertices);
    }
}
