using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DotweenAnimatorHelper : MonoBehaviour
{
    [SerializeField] private List<Vector3> initialPositions;
    
    [Button]
    private void SetDuration(float duration)
    {
        var doTweens = GetComponentsInChildren<DOTweenAnimation>();

        foreach (var component in doTweens)
        {
            component.duration = duration;
        }
    }

    [Button]
    private void SetDelay(float delay)
    {
        var doTweens = GetComponentsInChildren<DOTweenAnimation>();

        foreach (var component in doTweens)
        {
            component.delay = delay;
        }
    }

    [Button]
    private void SetRandomScale(float minScale, float maxScale)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).localScale = Random.Range(minScale, maxScale) * Vector3.one;
        }
    }

    [Button]
    private void SetRandomAngle()
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            transform.GetChild(i).eulerAngles = new Vector3(Random.Range(-180f, 180),
                Random.Range(-180f, 180),
                Random.Range(-180f, 180));
        }
    }

    [Button]
    private void SetRandomDuration(float minDuration, float maxDuration)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            doTweens.duration = Random.Range(minDuration, maxDuration);
        }
    }

    [Button]
    private void SetRandomDelay(float minDelay, float maxDelay)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            doTweens.duration = Random.Range(minDelay, maxDelay);
        }
    }

    [Button]
    private void SetPartialDelay(float startValue, float incrementalValue)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            doTweens.delay = startValue + i * incrementalValue;
        }
    }
    
    [Button]
    private void SetEase(Ease ease)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            doTweens.easeType = ease;
        }
    }
    
    [Button]
    private void MultiplyEndValue(float coefficient)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            doTweens.endValueV3 = 2 * doTweens.endValueV3;
        }
    }
    
    [Button]
    private void SetRandomY(float minY,float maxY)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var go = transform.GetChild(i);
            go.transform.position = new Vector3(go.transform.position.x,Random.Range(minY, maxY),go.transform.position.z);
        }
    }

    [Button]
    private void SaveInitialPositions()
    {
        initialPositions.Clear();
        
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var go = transform.GetChild(i);
            initialPositions.Add(go.transform.localPosition);
        }
    }
    
    [Button]
    private void GoToInitialPositions()
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var go = transform.GetChild(i);
            go.transform.localPosition = initialPositions[i];
        }
    }
    
    [Button]
    private void CalculateAndSetEndValue()
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var go = transform.GetChild(i);
            var delta = go.transform.localPosition - initialPositions[i];
            var doTweens = go.GetComponent<DOTweenAnimation>();
            doTweens.endValueV3 = delta;
        }
    }
    
    [Button]
    private void SetDelayByDistance(float minDelay,float maxDelay)
    {
        var childCount = transform.childCount;

        var minDistance = initialPositions.Min(vec => vec.z);
        Debug.Log(minDistance);
        var maxDistance = initialPositions.Max(vec => vec.z);
        Debug.Log(maxDistance);

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponent<DOTweenAnimation>();
            var t =  (doTweens.transform.localPosition.z - minDistance)/(maxDistance-minDistance);
            doTweens.delay = Mathf.Lerp(minDelay, maxDelay, t);
        }
    }
    
    [Button]
    private void RemoveComponentByIndex(int index)
    {
        var childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var doTweens = transform.GetChild(i).GetComponents<DOTweenAnimation>();
            DestroyImmediate(doTweens[index]);
        }
    }
}