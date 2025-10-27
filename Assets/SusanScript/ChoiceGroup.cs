using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class ChoiceGroup : MonoBehaviour
{
    public Transform[] choices;                 // 3 square parent transforms
    public SpriteRenderer[] images;             // sprite renderers (character images)
    public float enlargeFactor = 1.5f;          // adjustable scale factor
    public Action<int> onChoiceSelected;
    

    private int chosenIndex = -1;
    private bool locked = false;                // prevent multiple enlargements

    private float initialX; // store start position

    void Start()
    {
        initialX = transform.position.x;
    }



    public void Init(Sprite[] sprites)
    {
        for (int i = 0; i < choices.Length && i < sprites.Length; i++)
        {
            SpriteChoice sc = choices[i].GetComponent<SpriteChoice>();
            sc.Setup(this, i);

            // Assign sprite to each image
            if (i < images.Length && images[i] != null)
            {
                images[i].sprite = sprites[i];
            }

            // Find the name label under this choice (child TMP)
            TMPro.TextMeshPro nameLabel = choices[i].GetComponentInChildren<TMPro.TextMeshPro>();

            if (nameLabel != null)
            {
                // Assign sprite name as text
                nameLabel.text = sprites[i].name;
                nameLabel.color = Color.white;

                // Optional: ensure it's rendered above the sprite
                nameLabel.GetComponent<Renderer>().sortingOrder = 12;
            }
        }
    }


    public void InitActionLabels(string[] actions)
    {
        // Clear old name labels if they exist
        foreach (var c in choices)
        {
            TMPro.TextMeshPro nameText = c.GetComponentInChildren<TMPro.TextMeshPro>();
            if (nameText != null)
            {
                nameText.text = "";
            }
        }

        for (int i = 0; i < choices.Length && i < actions.Length; i++)
        {
            SpriteChoice sc = choices[i].GetComponent<SpriteChoice>();
            sc.SetupAction(this, i, actions[i]);

            // Find TMP text component for the action label
            TMPro.TextMeshPro text = choices[i].GetComponentInChildren<TMPro.TextMeshPro>();
            if (text != null)
            {
                text.text = actions[i];
                text.color = Color.white;
            }
        }
    }



    public Sprite[] GetSprites()
    {
        Sprite[] arr = new Sprite[images.Length];
        for (int i = 0; i < images.Length; i++)
            arr[i] = images[i].sprite;
        return arr;
    }


    public void NotifyChoice(int index)
    {
        // Prevent multiple triggers in the same group
        if (locked) return;
        locked = true;

        onChoiceSelected?.Invoke(index);
        HandleChoiceVisuals(index);
    }

    // Called after player selects one
    public void HandleChoiceVisuals(int index)
    {
        chosenIndex = index;
        bool isTextGroup = images == null || images.Length == 0;

        for (int i = 0; i < choices.Length; i++)
        {
            SpriteRenderer sr = choices[i].GetComponent<SpriteRenderer>();
            TMP_Text text = choices[i].GetComponentInChildren<TMP_Text>();

            if (i == chosenIndex)
            {
                // Selected option
                sr.sortingOrder = 10;

                if (!isTextGroup)
                {
                    // For image groups: raise image sorting order
                    if (i < images.Length && images[i] != null)
                    {
                        images[i].sortingOrder = 11;

                        
                    }
                }
                else
                {
                    // For text groups: keep text on top and highlight visually
                    if (text != null)
                    {
                        text.color = Color.yellow;
                        text.fontSize *= 1.2f;
                        text.GetComponent<Renderer>().sortingOrder = sr.sortingOrder + 1;
                    }
                    
                }

                // Both image and text groups execute the same enlarge animation
                StartCoroutine(SmoothScale(choices[i], enlargeFactor, 0.3f));
            }
            else
            {
                // Unselected options
                sr.sortingOrder = 1;
                if (!isTextGroup && i < images.Length && images[i] != null)
                    images[i].sortingOrder = 2;
            }
        }

        StartCoroutine(RemoveWhenOffScreen());
    }


    private IEnumerator SmoothScale(Transform target, float scaleFactor, float duration)
    {
        Vector3 start = target.localScale;
        Vector3 end = start * scaleFactor;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        target.localScale = end;
    }

    private IEnumerator RemoveWhenOffScreen()
    {
        float targetX = initialX - 100f;

        yield return new WaitUntil(() => transform.position.x < targetX);
        gameObject.SetActive(false);
    }
}
