using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Challenge
{
    int given_ammo;
    string objective_text;
    public Vector3 spawnCenter;
    int spawnWidth;
    int spawnHeight;
    int[] targetList;
    challengeHandler handler;
    
    public Challenge(int given_ammo, string objective_text,
                         Vector3 spawnCenter, int spawnWidth, int spawnHeight,
                         int[] targetList, challengeHandler challengeHandler)
        {



            this.given_ammo = given_ammo;
            this.objective_text = objective_text;
            this.spawnCenter = spawnCenter;
            this.spawnWidth = spawnWidth;
            this.spawnHeight = spawnHeight;
            this.targetList = targetList;
            this.handler = challengeHandler;


        }

        public void initiate()
        {
            handler.inTransition = true;
            handler.clear_targets();
            handler.spawn_targets(spawnCenter, spawnWidth, spawnHeight, targetList);
            handler.playerInput.give_ammo(given_ammo);
            handler.StartCoroutine(handler.EndTransitionAfter(3.4f));
            //handler.change_wind();

        }

}





public class challengeHandler : MonoBehaviour
{


    public Challenge[] challenges = new Challenge[10];
    public int challengeIndex;
    public int total_score;
    public int high_score;
    public GameObject[] targetPrefabs;
    LinkedList<GameObject> currentTargets = new LinkedList<GameObject>();
    public PlayerInput playerInput;
    public scoreUI scoreUI;
    public GameObject scoreAndAmmoUIobj;
    public Text resultText;
    public Text ScoreText;
    public Text instructionText;
    private bool onEndScreen = false;
    public bool inTransition = false;


    void Awake()
    {
        challenges[0] = new Challenge(3, "one close stationary target, low ammo", new Vector3(783.809998f, 0, 408.600006f),
            2, 2, new int[] { 1, 0, 0, 0 }, this);
        challenges[1] = new Challenge(4, "one close moving target, low ammo", new Vector3(783.809998f, 0, 437.799988f),
            4, 4, new int[] { 0, 1, 0, 0 }, this);
        challenges[2] = new Challenge(5, "one far stationary target, low ammo", new Vector3(783.809998f, 0, 602.700012f),
            6, 6, new int[] { 1, 0, 0, 0 }, this);
        challenges[3] = new Challenge(14, "one far moving and stationary target, mid ammo", new Vector3(783.809998f, 0, 588.599976f),
            10, 10, new int[] { 1, 1, 0, 0 }, this);
        challenges[4] = new Challenge(16, "multiple moving and stationary targets from mid to far, mid to high ammo", new Vector3(783.809998f, 0, 621f),
            12, 12, new int[] { 2, 2, 0, 0 }, this);
        challenges[5] = new Challenge(21, "multiple moving target far away, high ammo", new Vector3(783.809998f, 0, 653f),
            12, 12, new int[] { 0, 4, 0, 0 }, this);
        challenges[6] = new Challenge(14, "a star target in mid range, high ammo", new Vector3(783.809998f, 0, 568.599976f),
            12, 12, new int[] { 0, 0, 0, 1 }, this);
        challenges[7] = new Challenge(7, "a very far stationary target, mid ammo", new Vector3(783.809998f, 0, 927.299988f),
            12, 12, new int[] { 1, 0, 0, 0 }, this);

    }

    IEnumerator Start()
    {
        challengeIndex = 0;
        total_score = 0;
        high_score = PlayerPrefs.GetInt("HighScore", 0);
        yield return new WaitForSeconds(0.5f);   // to let the scene finish loading
        challenges[challengeIndex].initiate();
    }



    void Update()
    {
        if (onEndScreen)
        {
            if (Input.GetKeyUp(KeyCode.R)) {
                SceneManager.LoadScene("SampleScene");
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }

        }
    }

    public void clear_targets()
    {
        foreach (GameObject target in currentTargets)
        {
            Destroy(target);
        }

        currentTargets.Clear();
    }

    public void spawn_targets(Vector3 spawnCenter, int spawnWidth, int spawnHeight,
                     int[] targetList)
    {
        int prefabIndex = 0;
        foreach (int number in targetList)
        {
            for (int i = 0; i < number; i++)
            {
                float x = Random.Range(spawnCenter.x - spawnWidth, spawnCenter.x + spawnWidth);
                float z = Random.Range(spawnCenter.z - spawnHeight, spawnCenter.z + spawnHeight);
                Vector3 spawnPosition = new Vector3(x, spawnCenter.y, z);

                GameObject spawned = Instantiate(targetPrefabs[prefabIndex], spawnPosition, Quaternion.identity);
                Debug.Log($"Requested {spawnPosition}, actual {spawned.transform.position}", spawned);
                spawned.SendMessage("connect_challenger_handler", this, SendMessageOptions.DontRequireReceiver);
                currentTargets.AddLast(spawned);
            }
            prefabIndex++;
        }
    }

    public void removeCurrentTarget(GameObject target)
    {
        if (currentTargets.Remove(target))
        {
            Destroy(target);
        }

        if (currentTargets.Count == 0)
        {
          
            OnChallengeComplete();
        }

    }

    void OnChallengeComplete()
    {
        challengeIndex++;
        
        total_score += (playerInput.clipRounds + playerInput.totalAmmo) * 5;
        scoreUI.setScoreText(total_score);
   
        if (total_score > high_score || high_score == null)
        {
            high_score = total_score;
            PlayerPrefs.SetInt("HighScore", high_score);
            PlayerPrefs.Save();
            Debug.Log("New high score: " + high_score);
        }
        

        if (challengeIndex < challenges.Length && challenges[challengeIndex] != null)
        {
            
            challenges[challengeIndex].initiate();
        }
        else
        {
            end_game(true);
        }
    }


    public IEnumerator check_lose()
    {
        int startedOn = challengeIndex;
        yield return new WaitForSeconds(3f);
        if (startedOn == challengeIndex && currentTargets.Count != 0 && !inTransition)
        {
            end_game(false);
        }
    }


    public void end_game(bool win)
    {
        scoreAndAmmoUIobj.SetActive(false);
        Debug.Log("END GAME CALLED. Win: " + win);
        playerInput.give_ammo(0);
        onEndScreen = true;

        if (win)
        {
            StartCoroutine(ShowResult("Congrats Private!\nNow you can go and kill some Krauts!", win));
        }
        else
        {
            StartCoroutine(ShowResult("This is real sniping Private!\n not finishing on your girl's tits!", win));
        }

    }

    private IEnumerator ShowResult(string text, bool win)
    {
        Debug.Log("Waiting to show result...");

        yield return new WaitForSeconds(1f);

        resultText.enabled = true;
        resultText.text = text;

        if (win)
        {
            ScoreText.enabled = true;
            ScoreText.text = $"High Score: {high_score} \nScore: {total_score}";
        }

        instructionText.enabled = true;
        playerInput.enabled = false;

    }




    public void show_challenge_objective()
    {
        return;
    }

    public void change_wind()
    {
        return;
    }


    public IEnumerator EndTransitionAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        inTransition = false;
    }





}
