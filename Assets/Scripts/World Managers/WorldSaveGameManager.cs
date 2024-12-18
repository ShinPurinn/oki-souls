using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [SerializeField] private PlayerManager player;

    [Header("Save/Load")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Saved Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string fileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlots01;
    // public CharacterSaveData characterSlots02;
    // public CharacterSaveData characterSlots03;
    // public CharacterSaveData characterSlots04;
    // public CharacterSaveData characterSlots05;
    // public CharacterSaveData characterSlots06;
    // public CharacterSaveData characterSlots07;
    // public CharacterSaveData characterSlots08;
    // public CharacterSaveData characterSlots09;
    // public CharacterSaveData characterSlots10;

    private void Awake()
    {
        // only one instance of this script can exist at a time, if another exists, destroy it -cliff
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    private void DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(){
        switch(currentCharacterSlotBeingUsed){
            case CharacterSlot.CharacterSlot_01:
                fileName = "characterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                fileName = "characterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                fileName = "characterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                fileName = "characterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                fileName = "characterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                fileName = "characterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                fileName = "characterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                fileName = "characterSlot_08";
                break;
            case CharacterSlot.CharacterSlot_09:
                fileName = "characterSlot_09";
                break;
            case CharacterSlot.CharacterSlot_10:
                fileName = "characterSlot_10";
                break;
            default:
                break;
        }
    }

    public void CreateNewGame(){
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();
        currentCharacterData = new CharacterSaveData();
    }

    public void LoadGame(){
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame(){
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;

        player.saveGameDataToCurrentCharacterData(ref currentCharacterData);

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }
    public IEnumerator LoadWorldScene()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
        yield return null;
    }

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }
}
