using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerOnePrompt;
    public TextMeshProUGUI playerTwoPrompt;
    public TextMeshProUGUI score1;  // Added score 1 (still useful for flashing)
    public GameObject gameOverPanel;  // Panel that holds the lose text and other UI
    public ParticleSystem explosionEffect; // The explosion effect to trigger when time runs out
    public AudioSource backgroundMusic;  // AudioSource for the background music
    public AudioSource explosionSound;  // AudioSource for the explosion sound

    public float bombTime = 30f;
    private float timeDecreaseRate = 0.1f;

    private string playerOneSequence;
    private string playerTwoSequence;

    private bool playerOneDone = false;
    private bool playerTwoDone = false;

    private bool roundInProgress = false;

    private PlayerInput playerOne;
    private PlayerInput playerTwo;

    void Start()
    {
        playerOne = new PlayerInput(new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D });  // WASD for Player 1
        playerTwo = new PlayerInput(new KeyCode[] { KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 }); // 7, 8, 9 for Player 2

        // Set the prompt references in PlayerInput so we can control it
        playerOne.SetPromptReference(playerOnePrompt);
        playerTwo.SetPromptReference(playerTwoPrompt);

        GenerateNewSequences(); // Initial sequence generation
        gameOverPanel.SetActive(false); // Make sure the game over panel is initially inactive
        score1.alpha = 0f; // Make sure score is hidden initially

        backgroundMusic.Play();  // Start background music when the game starts
    }

    void Update()
    {
        bombTime -= Time.deltaTime;
        timerText.text = "Time: " + bombTime.ToString("F2");

        if (bombTime <= 0)
        {
            StartCoroutine(GameOver());
            return;
        }

        if (playerOne.CheckInput() && !playerOneDone)
        {
            playerOneDone = true;
        }

        if (playerTwo.CheckInput() && !playerTwoDone)
        {
            playerTwoDone = true;
        }

        // Flash score and bounce combo when both players complete their inputs
        if (playerOneDone && playerTwoDone && !roundInProgress)
        {
            roundInProgress = true;

            bombTime += 5f; // Add time when both finish
            timeDecreaseRate += 0.01f; // Speed up over time

            playerOneDone = false;
            playerTwoDone = false;

            GenerateNewSequences(); // Generate new sequences
            StartCoroutine(WaitForNextRound());

            StartCoroutine(FlashScore()); // Start flashing the score
            StartCoroutine(BounceComboStrings()); // Start the bounce effect for combo strings
        }
    }

    // Coroutine to flash the score for 0.6 seconds
    IEnumerator FlashScore()
    {
        score1.alpha = 1f; // Make the score visible
        yield return new WaitForSeconds(0.6f); // Wait for 0.6 seconds

        score1.alpha = 0f; // Hide the score
    }

    // Coroutine to make the combo strings bounce a little
    IEnumerator BounceComboStrings()
    {
        Vector3 originalPosition1 = playerOnePrompt.transform.position;
        Vector3 originalPosition2 = playerTwoPrompt.transform.position;

        // Apply small upward movement and return to original position
        float bounceHeight = 10f;
        float bounceDuration = 0.2f;

        float elapsedTime = 0f;

        while (elapsedTime < bounceDuration)
        {
            float bounceAmount = Mathf.Sin((elapsedTime / bounceDuration) * Mathf.PI) * bounceHeight;
            playerOnePrompt.transform.position = originalPosition1 + new Vector3(0, bounceAmount, 0);
            playerTwoPrompt.transform.position = originalPosition2 + new Vector3(0, bounceAmount, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure they return to the exact original position
        playerOnePrompt.transform.position = originalPosition1;
        playerTwoPrompt.transform.position = originalPosition2;
    }

    void GenerateNewSequences()
    {
        playerOneSequence = GenerateSequence("WASD");
        playerTwoSequence = GenerateSequence("789");

        playerOne.SetSequence(playerOneSequence);
        playerTwo.SetSequence(playerTwoSequence);

        playerOnePrompt.text = playerOneSequence;
        playerTwoPrompt.text = playerTwoSequence;

        playerOne.ResetInputState();
        playerTwo.ResetInputState();
    }

    string GenerateSequence(string keySet)
    {
        string sequence = "";
        for (int i = 0; i < 5; i++)
        {
            sequence += keySet[Random.Range(0, keySet.Length)];
        }
        return sequence;
    }

    IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(1f);
        roundInProgress = false;
    }

    IEnumerator GameOver()
    {
        // Trigger explosion effect immediately when time runs out
        explosionEffect.Play();

        // Play explosion sound immediately after bomb timer hits 0
        explosionSound.Play();

        // Wait for 0.6 seconds before showing the game over panel
        yield return new WaitForSeconds(0.6f);

        // Stop the background music and show the game over panel
        backgroundMusic.Pause();
        gameOverPanel.SetActive(true);

        // Wait until space bar is pressed to restart the game
        bool restartPressed = false;
        while (!restartPressed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                restartPressed = true;
            }
            yield return null;
        }

        // Restart the game once space is pressed
        RestartGame();

        // Wait for the second spacebar press before starting the music again
        bool secondSpacePressed = false;
        while (!secondSpacePressed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                secondSpacePressed = true;
            }
            yield return null;
        }

        // Now, start the background music after the second space bar hit
        backgroundMusic.Play();
    }

    void RestartGame()
    {
        // Reset all necessary values and UI elements
        bombTime = 10f;
        timeDecreaseRate = 0.1f;
        playerOneDone = false;
        playerTwoDone = false;
        roundInProgress = false;

        gameOverPanel.SetActive(false); // Hide game over panel

        // Reset any other UI elements if needed
        timerText.text = "Time: " + bombTime.ToString("F2");

        // Restart the game logic
        GenerateNewSequences(); // Restart the game with new sequences
    }
}
