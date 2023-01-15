using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenDone : MonoBehaviour
{
    public void Done()
    {
        Destroy(gameObject);
    }
}
