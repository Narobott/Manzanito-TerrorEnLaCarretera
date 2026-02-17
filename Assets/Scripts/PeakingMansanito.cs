using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PeakingMansanito : MonoBehaviour
{
    [SerializeField] private Vector3 idleRect;
    [SerializeField] private Vector3 peakRect;

    private bool isPeaking = false;

    private void Start()
    {
        StartCoroutine(Measomojiji());
    }

    private IEnumerator Measomojiji()
    {
        while (true)
        {
            float newTimeToWait = UnityEngine.Random.Range(5f, 10f);
            yield return new WaitForSeconds(newTimeToWait);

            if (!isPeaking)
            {
                StartCoroutine(Asomadita());
            }
        }
    }

    private IEnumerator Asomadita()
    {
        isPeaking = true;

        float alpha = 0.0f;
        bool reachingPeak = true;
        Vector3 pos;

        while (reachingPeak)
        {
            alpha += 0.5f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            pos = Vector3.Lerp(idleRect, peakRect, alpha);
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = pos;

            if (alpha >= 1.0f)
            {
                reachingPeak = false;
            }
            yield return null;
        }

        while (!reachingPeak)
        {
            alpha -= 0.5f * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            pos = Vector3.Lerp(idleRect, peakRect, alpha);
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = pos;

            if (alpha <= 0.0f)
            {
                break;
            }
            yield return null;
        }

        isPeaking = false;
    }

    public void manzanitoClick()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition3D = idleRect;

        // Generate mansanito rigidbody user gan drag arround

    }

}
