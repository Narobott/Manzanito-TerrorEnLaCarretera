using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpriteCarousel : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Content")]
    public GameObject itemPrefab;
    public Sprite[] sprites;

    [Header("Layout")]
    public float spacing = 4f;

    [Header("Motion")]
    public float damping = 10f;
    public float snapStrength = 12f;
    public float dragSensitivity = 0.02f;
    public float maxVelocity = 60f;

    [Header("Scaling")]
    public float centerScale = 1f;
    public float sideScale = 0.65f;
    public float outerScale = 0.25f;

    const int visibleCount = 5;

    List<Transform> activeItems = new List<Transform>();

    float position;
    float velocity;
    bool dragging;
    int lastSelectedIndex = 0;
    int leftmostIndex = 0;

    void Start() => Initialize();

    void Initialize()
    {

        if (PlayerPrefs.HasKey("Skin"))
        {
            leftmostIndex = WrapIndex(PlayerPrefs.GetInt("Skin") - 2);
            lastSelectedIndex = PlayerPrefs.GetInt("Skin");
        }
        else
        {
            PlayerPrefs.SetInt("Skin", 0);
            leftmostIndex = WrapIndex(-2);
            lastSelectedIndex = 0;
        }

        position = 0f;
        velocity = 0f;

        foreach (Transform t in activeItems)
            if (t != null) Destroy(t.gameObject);
        activeItems.Clear();

        for (int i = 0; i < visibleCount; i++)
        {
            CreateItem(WrapIndex(leftmostIndex + i));
        }

        UpdateVisuals();
    }

    void CreateItem(int spriteIndex)
    {
        GameObject obj = Instantiate(itemPrefab, transform);
        obj.GetComponent<Image>().sprite = sprites[spriteIndex];
        obj.GetComponent<Image>().preserveAspect = true;
        activeItems.Add(obj.transform);
    }

    void Update()
    {
        if (!dragging)
        {
            float dt = Time.deltaTime;
            float target = Mathf.Round(position / spacing) * spacing;
            float error = target - position;

            velocity += error * snapStrength * dt;
            velocity *= Mathf.Exp(-damping * dt);
            velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
            position += velocity * dt;

            if (Mathf.Abs(velocity) < 0.005f && Mathf.Abs(error) < 0.005f)
            {
                velocity = 0f;
                position = target;
            }
        }

        int SelectedIndex = WrapIndex(leftmostIndex + 2);

        int current = SelectedIndex;

        if (current != lastSelectedIndex)
        {
            lastSelectedIndex = current;
            PlayerPrefs.SetInt("Skin", lastSelectedIndex);
        }

        UpdateVisuals();
        RecycleIfNeeded();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < activeItems.Count; i++)
        {
            float slotOffset = i - 2;
            float worldX = slotOffset * spacing - position;

            activeItems[i].localPosition = new Vector3(worldX, 0f, 0f);

            float normalizedDist = Mathf.Abs(worldX) / spacing;

            float scale;
            if (normalizedDist <= 1f)
                scale = Mathf.Lerp(centerScale, sideScale, normalizedDist);
            else
                scale = Mathf.Lerp(sideScale, outerScale, normalizedDist - 1f);

            activeItems[i].localScale = Vector3.one * scale;

            float brightness = Mathf.Lerp(1f, 0.35f, Mathf.Clamp01(normalizedDist));
            activeItems[i].GetComponent<Image>().color = new Color(brightness, brightness, brightness, 1f);
        }
    }

    void RecycleIfNeeded()
    {
        if (position > spacing * 0.5f)
        {
            position -= spacing;
            ShiftRight();
        }
        else if (position < -spacing * 0.5f)
        {
            position += spacing;
            ShiftLeft();
        }
    }

    void ShiftLeft()
    {
        Transform last = activeItems[activeItems.Count - 1];
        activeItems.RemoveAt(activeItems.Count - 1);
        Destroy(last.gameObject);

        leftmostIndex = WrapIndex(leftmostIndex - 1);
        InsertLeft(leftmostIndex);
    }

    void ShiftRight()
    {
        Transform first = activeItems[0];
        activeItems.RemoveAt(0);
        Destroy(first.gameObject);

        int newRightIndex = WrapIndex(leftmostIndex + visibleCount);
        leftmostIndex = WrapIndex(leftmostIndex + 1);
        InsertRight(newRightIndex);
    }

    void InsertLeft(int spriteIndex)
    {
        GameObject obj = Instantiate(itemPrefab, transform);
        obj.GetComponent<Image>().sprite = sprites[spriteIndex];
        obj.GetComponent<Image>().preserveAspect = true;
        activeItems.Insert(0, obj.transform);
    }

    void InsertRight(int spriteIndex)
    {
        GameObject obj = Instantiate(itemPrefab, transform);
        obj.GetComponent<Image>().sprite = sprites[spriteIndex];
        obj.GetComponent<Image>().preserveAspect = true;
        activeItems.Add(obj.transform);
    }

    int WrapIndex(int index)
    {
        if (sprites.Length == 0) return 0;
        return (index % sprites.Length + sprites.Length) % sprites.Length;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        velocity = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        position -= eventData.delta.x * dragSensitivity;
        velocity = -eventData.delta.x * dragSensitivity / Mathf.Max(Time.deltaTime, 0.0001f);
        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }
}