using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    public ChoiceGroup stage1Characters;
    public ChoiceGroup stage2Actions;
    public ChoiceGroup stage3RemainingCharacters;
    public ChoiceGroup stage4RemainingActions;
    public GameObject resultPanel;
    public TMPro.TextMeshProUGUI resultText;

    private Sprite personA, personB, personC;
    private string actionA, actionB, actionC;
    private GameManager manager;
    private List<Sprite> remainingPeople = new List<Sprite>();
    private List<string> remainingActions = new List<string>();

    [Header("Layout Settings")]
    public float resultPanelOffsetX = 6f; // Controls how far the result panel appears from the last stage horizontally


    public float moveSpeed = 4f;  // Controls the movement speed of the entire round
    private bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    }


    public void Init(Sprite[] pool, GameManager gm)
    {
        manager = gm;

        // Pick 3 random characters
        personA = pool[Random.Range(0, pool.Length)];
        personB = pool[Random.Range(0, pool.Length)];
        personC = pool[Random.Range(0, pool.Length)];

        // Stage 1 (Character selection)
        stage1Characters.Init(new Sprite[] { personA, personB, personC });
        stage1Characters.onChoiceSelected = OnStage1Choice;

        // Stage 2 (Action selection) must be initialized, otherwise parentGroup will be null
        stage2Actions.InitActionLabels(new string[] { "Fuck", "Marry", "Kill" });
        stage2Actions.onChoiceSelected = OnStage2Choice;

        // Register remaining callbacks
        stage3RemainingCharacters.onChoiceSelected = OnStage3Choice;
        stage4RemainingActions.onChoiceSelected = OnStage4Choice;

        Canvas resultCanvas = resultPanel.GetComponent<Canvas>();
        if (resultCanvas != null)
        {
            // Ensure it renders *below* selected sprites
            resultCanvas.sortingOrder = 1;
        }


        // Default visibility states
        stage1Characters.gameObject.SetActive(true);
        stage2Actions.gameObject.SetActive(false);
        stage3RemainingCharacters.gameObject.SetActive(false);
        stage4RemainingActions.gameObject.SetActive(false);
        resultPanel.SetActive(false);
    }


    // Stage 1: Choose one character
    void OnStage1Choice(int index)
    {
        Sprite[] imgs = stage1Characters.GetSprites();
        Sprite chosen = imgs[index];
        personA = chosen;

        remainingPeople.Clear();
        for (int i = 0; i < imgs.Length; i++)
            if (i != index) remainingPeople.Add(imgs[i]);

        // Instead of immediately hiding the group,
        // tell ChoiceGroup to handle the selected behavior itself.
        stage1Characters.HandleChoiceVisuals(index);

        // Then start coroutine to move to the next stage after a short delay
        StartCoroutine(WaitAndShowStage2());
    }

    IEnumerator WaitAndShowStage2()
    {
        yield return new WaitForSeconds(1.5f); // Wait for enlargement animation
        stage2Actions.gameObject.SetActive(true);
    }


    // Stage 2: Choose action for personA
    void OnStage2Choice(int index)
    {
        Debug.Log("[RoundManager] Stage 2 choice made: " + index);
        string[] actions = { "Fuck", "Marry", "Kill" };
        actionA = actions[index];

        remainingActions = new List<string>(actions);
        remainingActions.Remove(actionA);

        // Visually enlarge and slide out like stage1
        stage2Actions.HandleChoiceVisuals(index);

        // After delay, show remaining characters
        StartCoroutine(WaitAndShowStage3());
    }

    IEnumerator WaitAndShowStage3()
    {
        yield return new WaitForSeconds(1.5f);

        stage3RemainingCharacters.Init(remainingPeople.ToArray());
        stage3RemainingCharacters.gameObject.SetActive(true);
    }



    // Stage 3: Choose next character
    void OnStage3Choice(int index)
    {
        personB = remainingPeople[index];
        remainingPeople.RemoveAt(index);

        // Visually enlarge and slide out like stage1/stage2
        stage3RemainingCharacters.HandleChoiceVisuals(index);

        // After delay, show remaining actions
        StartCoroutine(WaitAndShowStage4());
    }

    IEnumerator WaitAndShowStage4()
    {
        yield return new WaitForSeconds(1.5f);

        stage4RemainingActions.InitActionLabels(remainingActions.ToArray());
        stage4RemainingActions.gameObject.SetActive(true);
    }

    // Stage 4: Choose action for personB, remaining gets last action
    void OnStage4Choice(int index)
    {
        actionB = remainingActions[index];
        remainingActions.RemoveAt(index);
        actionC = remainingActions[0];
        personC = remainingPeople[0];

        // Same behavior: enlarge + slide out
        stage4RemainingActions.HandleChoiceVisuals(index);

        // After short delay, show results
        StartCoroutine(WaitAndShowResult());
    }

    IEnumerator WaitAndShowResult()
    {
        yield return new WaitForSeconds(1.5f);
        ShowResult();
    }


    void ShowResult()
    {
        Debug.Log("[RoundManager] Showing result panel");
        // Calculate ResultPanel position (follow Stage4's position)
        if (stage4RemainingActions != null && resultPanel != null)
        {
            Vector3 stage4Pos = stage4RemainingActions.transform.localPosition;
            resultPanel.transform.localPosition = stage4Pos + new Vector3(resultPanelOffsetX, 0, 0);
        }

        // Activate the result panel
        resultPanel.SetActive(true);

        // Update the result text - show all selections
        resultText.text =
            "Result:\n" +
            "A: " + personA.name + " → " + actionA + "\n" +
            "B: " + personB.name + " → " + actionB + "\n" +
            "C: " + personC.name + " → " + actionC;

        // Delay before spawning the next round
        StartCoroutine(NextRoundDelay());
    }


    IEnumerator NextRoundDelay()
    {
        Debug.Log("[RoundManager] Starting 3 second delay before next round");
        yield return new WaitForSeconds(3f);
        Debug.Log("[RoundManager] Delay complete, spawning next round");
        manager.SpawnNewRound();
        Destroy(gameObject);
    }
}
