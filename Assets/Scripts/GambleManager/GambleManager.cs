using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambleManager : MonoBehaviour
{
    public PlayerSlash player;
    public RectTransform gambleScreen;
    public RectTransform titleScreen;
    public CoinCount gameManager;
    public DialogueManager dialoguer;

    private Dialogue[] dialogues = new Dialogue[3];
    public Dialogue dialogue1;
    public Dialogue dialogue2;
    public Dialogue dialogue3;

    public InfiniteScroll slotMachine1;
    public InfiniteScroll slotMachine2;
    public InfiniteScroll slotMachine3;
    // Start is called before the first frame update
    private bool slotsArrive = false;
    private bool titleArrive = false;
    private bool ready = false;

    private bool freeSpin;
    private static int timeGambles;
    private static int[] slot0 = new int[3] {0, 0, 0};    //enemy speed
    private static int[] slot1 = new int[3] {5, 5, 5};    //enemy count
    private static int[] slot2 = new int[3] {12, 12, 12};    //enemy health
    private static int[] slot3 = new int[3] {4, 4, 4};     //shot size
    private static int[] slot4 = new int[3] {9, 9, 9};     //shot life
    private static int[] slot5 = new int[3] {10, 10, 10};    //shot damage
    private static int[] slot6 = new int[3] {7, 7, 7};     //fire rate
    
    private static int[][] key = new int[][] {slot0, slot1, slot2, slot3, slot4, slot5, slot6};
    [SerializeField] AudioClip[] sounds;
    AudioSource gambleAudioSource;

    private int maxGambles = 2;
    private int gambles = 0;

    void Start()
    {
        freeSpin = true;
        Vector3 slotsPos = gambleScreen.localPosition;
        gambleScreen.localPosition = new Vector3(slotsPos.x, 1080, slotsPos.z);

        gambleAudioSource = GetComponent<AudioSource>();
        bringInSlots();

        Vector3 titlePos = titleScreen.localPosition;
        titleScreen.localPosition = new Vector3(titlePos.x, -700, titlePos.z);

        dialoguer = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        dialogues[0] = dialogue1;
        dialogues[1] = dialogue2;
        dialogues[2] = dialogue3;
    }

    // Update is called once per frame
    void Update()
    {
        if (ready && Input.GetKeyUp(KeyCode.G)) {
            Debug.Log("Gamble!");
            ready = false;
            gambles++;
            if (gambles == maxGambles) {
                timeGambles++;
                dialoguer.StartDialogue(dialogues[timeGambles]);
                gameObject.SetActive(false);
            } else { 
                StartCoroutine(Gamble());
            }
        }
    }

    IEnumerator Gamble() {

        
        // Level pull
        // Update background
        slotMachine1.spinning = true;
        slotMachine2.spinning = true;
        slotMachine3.spinning = true;

         int outcome = Random.Range(0, 7);
        //int outcome = 1;
        Debug.Log(outcome);
        StartCoroutine(slotMachine1.SpinTo(key[outcome][0]));

        float timer = 0.5f;
        while (timer > 0.0f) {
            timer -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(slotMachine2.SpinTo(key[outcome][1]));

        timer = 0.5f;
        while (timer > 0.0f) {
            timer -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(slotMachine3.SpinTo(key[outcome][2]));

        while (slotMachine1.spinning || slotMachine2.spinning || slotMachine3.spinning) {
            yield return null;
        }
        ready = true;
        rewardStats(outcome);
    }

    public void rewardStats(int statNumber) {
        if (statNumber == 0) {
            //enemy speed
            EnemyBehavior.IncreaseSpeed();
        } else if (statNumber == 1) {
            //enemy count
            Room.IncreaseSpawns();
        } else if (statNumber == 2) {
            Enemy.IncreaseHealth();
            //enemy health
        } else if (statNumber == 3) {
            player.attackSizeUp();
        } else if (statNumber == 4) {
            player.attackTimeUp();
        } else if (statNumber == 5) {
            player.damageUp();
        } else if (statNumber == 6) {
            player.attackSpeedUp();
        } else {
            player.attackSpeedUp();
        }
    }

    public void bringInSlots() {
        StopAllCoroutines();
        gambleSounds();
        //Vector3 slotsPos = gambleScreen.localPosition;
        //gambleScreen.localPosition = new Vector3(slotsPos.x, 1080, slotsPos.z);

        //gambleAudioSource = GetComponent<AudioSource>();
        //bringInSlots();

        //Vector3 titlePos = titleScreen.localPosition;
        //titleScreen.localPosition = new Vector3(titlePos.x, -700, titlePos.z);

        //dialoguer = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        //dialogues[0] = dialogue1;
        //dialogues[1] = dialogue2;
        //dialogues[2] = dialogue3;
        StartCoroutine(GamblePanelEnters());
        
    }

    IEnumerator GamblePanelEnters() {

        while (Vector3.Distance(gambleScreen.localPosition, Vector3.zero) > 5) {
            Vector3 interpPos = Vector3.Lerp(gambleScreen.localPosition, Vector3.zero, 0.025f);
            gambleScreen.localPosition = interpPos;
            
            if (!titleArrive && Vector3.Distance(gambleScreen.localPosition, Vector3.zero) <= 10) {
                titleArrive = true;
                bringInTitle();
            }

            if (Vector3.Distance(gambleScreen.localPosition, Vector3.zero) <= 5) {
                gambleScreen.localPosition = Vector3.zero;
            }
            yield return null;
        }
        titleArrive = false;
    }

    public void bringInTitle() {
        StartCoroutine(TitlePanelEnters());
    }

    IEnumerator TitlePanelEnters() {
        Vector3 dest = new Vector3(0, -360, 0);
        while (Vector3.Distance(titleScreen.localPosition, dest) > 5) {
            Vector3 interpPos = Vector3.Lerp(titleScreen.localPosition, dest, 0.025f);
            titleScreen.localPosition = interpPos;

            if (Vector3.Distance(titleScreen.localPosition, dest) <= 5) {
                titleScreen.localPosition = dest;
            }
            yield return null;
        }
        ready = true;
    }
    void gambleSounds()
    {
        //AudioClip clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        //gambleAudioSource.PlayOneShot(clip);
    }
}
