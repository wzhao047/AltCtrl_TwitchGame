using UnityEngine;
using TMPro;

public class SpriteChoice : MonoBehaviour
{
    private ChoiceGroup parentGroup;
    public int index;
    private bool selected = false;
    private string actionLabel = "";
    private Vector3 originalScale;
    public TextMeshPro label; // optional text for action labels (Fuck / Marry / Kill)

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void Setup(ChoiceGroup group, int idx)
    {
        parentGroup = group;
        index = idx;
        selected = false;
    }

    public void SetupAction(ChoiceGroup group, int idx, string labelText)
    {
        parentGroup = group;
        index = idx;
        selected = false;
        actionLabel = labelText;
        if (label != null)
            label.text = actionLabel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (selected) return;
        if (other.CompareTag("Player"))
        {
            selected = true;
            parentGroup.NotifyChoice(index);
        }
    }
}
