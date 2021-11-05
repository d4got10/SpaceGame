// HaltonSequence.cs

using UnityEngine;

public class HaltonTest : MonoBehaviour
{

    HaltonSequence positionsequence = new HaltonSequence();

    void Start()
    {
        float size = 20.0f * 2;
        positionsequence.Reset();
        Vector3 position = Vector3.zero;
        int amount = 200;
        for (int i = 0; i < amount; i++)
        {
            positionsequence.Increment();
            //position.set(positionsequence.m_CurrentPos);
            //Debug.Log(positionsequence.m_CurrentPos);
            position = positionsequence.m_CurrentPos;
            //position.x -=0.5f;
            position.y = 0.0f;
            //position.z -=0.5f;
            position *= size;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
        }
    }
}