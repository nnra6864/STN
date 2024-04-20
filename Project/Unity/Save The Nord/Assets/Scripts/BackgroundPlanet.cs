using System.Collections;
using UnityEngine;

public class BackgroundPlanet : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Rotate());
    }

    IEnumerator Rotate()
    {
        var rotationAmount = new Vector3(Random.Range(1, 5), Random.Range(1, 5), Random.Range(1, 5));
        while (true)
        {
            transform.Rotate(rotationAmount * Time.deltaTime);
            yield return null;
        }
    }
}
