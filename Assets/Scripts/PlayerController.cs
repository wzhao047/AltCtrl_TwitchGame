using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveLimitY = 4f;
    public float moveMagnitude;

    // Whether the player can currently select an option
    public bool canSelect = true;

    // Reference to the current active round (set dynamically)
    private RoundManager currentRound;

    void Start()
    {
        // Try to find current RoundManager in scene
        currentRound = FindObjectOfType<RoundManager>();
    }

    void Update()
    {
        // If no round is active, try to refresh the reference
        if (currentRound == null)
            currentRound = FindObjectOfType<RoundManager>();

        float moveDir = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveDir = 1f;
            else if (Keyboard.current.sKey.isPressed) moveDir = -1f;
        }

        Vector3 newPos = transform.position + Vector3.up * moveDir * moveSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, -moveLimitY, moveLimitY);
        transform.position = newPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canSelect) return;

        SpriteChoice choice = other.GetComponent<SpriteChoice>();
        if (choice != null)
        {
            // Lock selection for current stage
            canSelect = false;

            canSelect = false; // lock once per stage

            // Notify ChoiceGroup directly (RoundManager will handle stage logic)
            ChoiceGroup parentGroup = choice.GetComponentInParent<ChoiceGroup>();
            if (parentGroup != null)
                parentGroup.NotifyChoice(GetChoiceIndex(choice));

            // Optionally reset after short delay (handled by RoundManager stage switch)
        }
    }

    private int GetChoiceIndex(SpriteChoice choice)
    {
        // Try to find this choice index based on its order in parent group
        ChoiceGroup group = choice.GetComponentInParent<ChoiceGroup>();
        if (group == null) return 0;

        for (int i = 0; i < group.choices.Length; i++)
            if (group.choices[i] == choice.transform)
                return i;
        return 0;
    }


    // Called by RoundManager when next stage or round starts
    public void ResetSelection()
    {
        canSelect = true;
    }

    public void GoUP()
    {
        Vector3 newPos = transform.position + Vector3.up * moveMagnitude * moveSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, -moveLimitY, moveLimitY);
        transform.position = newPos;
    }
    public void GoDown()
    {
        Vector3 newPos = transform.position + Vector3.up * -moveMagnitude * moveSpeed * Time.deltaTime;
        newPos.y = Mathf.Clamp(newPos.y, -moveLimitY, moveLimitY);
        transform.position = newPos;
    }

}
