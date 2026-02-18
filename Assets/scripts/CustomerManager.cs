using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Visual")]
    public SpriteRenderer customerRenderer;
    public List<Sprite> customerSprites = new();

    [Header("Animation (optional)")]
    public Animator animator;                 // if you have appear/leave triggers
    public float appearTime = 0.4f;           // time for appear animation
    public float leaveTime = 0.4f;            // time for leave animation
    public bool hideWhenStopped = true;

    Coroutine loop;
    bool servedFlag;
    int lastIdx = -1;

    public void StartLoop()
    {
        StopLoop();
        servedFlag = false;

        if (hideWhenStopped)
            gameObject.SetActive(true);

        loop = StartCoroutine(Loop());
    }

    public void StopLoop()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
        servedFlag = false;

        if (hideWhenStopped)
            gameObject.SetActive(false);
    }

    // Called by GameManager after burger served
    public void OnServed()
    {
        servedFlag = true;
    }

    IEnumerator Loop()
    {
        while (true)
        {
            SpawnRandomCustomer();

            if (animator) animator.SetTrigger("Appear");
            yield return new WaitForSeconds(appearTime);

            // Wait until served
            yield return new WaitUntil(() => servedFlag);
            servedFlag = false;

            if (animator) animator.SetTrigger("Leave");
            yield return new WaitForSeconds(leaveTime);
        }
    }

    void SpawnRandomCustomer()
    {
        if (!customerRenderer) return;
        if (customerSprites == null || customerSprites.Count == 0) return;

        int idx = Random.Range(0, customerSprites.Count);

        // avoid repeating twice in a row
        if (customerSprites.Count > 1 && idx == lastIdx)
            idx = (idx + 1) % customerSprites.Count;

        lastIdx = idx;
        customerRenderer.sprite = customerSprites[idx];
    }
}
