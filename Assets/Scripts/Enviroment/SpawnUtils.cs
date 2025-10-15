using UnityEngine;

public static class SpawnUtils
{
    private const float TAU = Mathf.PI * 2f;

    public static Vector3 RandomPointInCircleXZ(Vector3 center, float maxRadius)
    {
        float angle = Random.value * TAU;
        float radius = Mathf.Sqrt(Random.value) * maxRadius;
       
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);
    }

    public static Vector3 RandomPointInAnnulusXZ(Vector3 center, float minRadius, float maxRadius)
    {
        float angle = Random.value * TAU;
        float radius0 = minRadius * minRadius;
        float radius1 = maxRadius * maxRadius;
        float radius = Mathf.Sqrt(Random.Range(radius0, radius1));

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);
    }
}
