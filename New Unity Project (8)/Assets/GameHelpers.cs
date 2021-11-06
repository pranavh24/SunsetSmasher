using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelpers
{
    public static IEnumerator timedFunction(float time, System.Action func)
    {
        yield return new WaitForSeconds(time);
        func();
    }
}
