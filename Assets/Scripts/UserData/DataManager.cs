
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Action<int> OnIngameStarsUpdated; 
    [Header("User Data")]
    private UserData userData;
    private string saveFile;
    [Header("Ingame Star")]
    private int ingameStars = 0;
    private int IngameStars{
        get{
            return ingameStars;
        }
        set{
            ingameStars = value;
            OnIngameStarsUpdated?.Invoke(ingameStars);
        }
    }

    [Header("Data Manager UI")]
    [SerializeField] private DataManagerUI dataManagerUI;
    [Header("Game Manager UI")]
    [SerializeField] private GameManagerUI gameManagerUI;

    private void Awake() {
        saveFile = Application.persistentDataPath + "/userdata.dat";
    }

    private async void Start(){
        this.userData = await LoadData();

        userData.OnUserDataChanged += (o) => {SaveData();};
        userData.OnUserDataChanged += dataManagerUI.UpdateStatUI;
        userData.OnUserDataChanged += gameManagerUI.ValidatePlayOnButton;

        userData.OnUserDataChanged?.Invoke(userData);

        GameManager.Instance.OnGameStarted += ResetIngameStarsCount;
        OnIngameStarsUpdated += dataManagerUI.UpdateIngameStar;
    }

    private void OnDestroy(){
        userData.OnUserDataChanged -= dataManagerUI.UpdateStatUI;
        userData.OnUserDataChanged -= gameManagerUI.ValidatePlayOnButton;
        GameManager.Instance.OnGameStarted -= ResetIngameStarsCount;
        OnIngameStarsUpdated -= dataManagerUI.UpdateIngameStar;
    }

    private async Task<UserData> LoadData(){
        UserData data = new();
        TaskCompletionSource<UserData> tcs = new TaskCompletionSource<UserData>();
        if (File.Exists(saveFile)){
            string content = File.ReadAllText(saveFile);
            userData = JsonUtility.FromJson<UserData>(content);
        }
        tcs.SetResult(data);
        return await tcs.Task;
    }

    public void SaveData(){
        string content = JsonUtility.ToJson(userData);
        File.WriteAllText(saveFile,content);
    }

    public void IncrementLevelProgress(){
        userData.AddLevel(1);
    }

    public int GetLevel(){
        return userData.level;
    }

    public int GetGold(){
        return userData.gold;
    }

    // use negative value to do minus calculation
    public void TransactGold(int gold){
        userData.AddGold(gold);
    }

    public void AddIngameStars(int value){
        IngameStars += value;
    }

    private void ResetIngameStarsCount(){
        IngameStars = 0;
    }

    public void AddIngameStarsToUserData(){
        userData.AddStar(IngameStars);
    }

    #region prototype
    public void Add1000Gold(){
        userData.AddGold(1000);
    }
    #endregion
}
