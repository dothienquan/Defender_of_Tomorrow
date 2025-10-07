using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlowEffect : MonoBehaviour
{
    // lưu các nguồn slow đang tác dụng (vùng, kỹ năng…)
    private readonly Dictionary<object, float> sources = new Dictionary<object, float>();
    private float currentMultiplier = 1f;

    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>() ?? GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public void AddOrUpdateSource(object source, float multiplier)
    {
        multiplier = Mathf.Clamp(multiplier, 0.1f, 1f);
        sources[source] = multiplier;
        Recalculate();
    }

    public void RemoveSource(object source)
    {
        if (sources.Remove(source))
            Recalculate();
    }

    void Recalculate()
    {
        float minMul = 1f;
        foreach (var kv in sources)
            if (kv.Value < minMul) minMul = kv.Value;

        currentMultiplier = minMul;

        // áp cảm giác chậm qua animator
        if (anim) anim.speed = currentMultiplier;
    }

    void FixedUpdate()
    {
        if (rb && currentMultiplier < 1f)
        {
            // kìm vận tốc về hệ số slow (mềm để tránh giật)
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, rb.linearVelocity * currentMultiplier, 0.5f);
        }
    }

    void OnDisable()
    {
        // khôi phục animator khi effect bị tắt
        if (anim) anim.speed = 1f;
        sources.Clear();
        currentMultiplier = 1f;
    }
}
