using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserDataManager
{
    public UserData userData;

    private readonly string saveFile = Application.persistentDataPath + "/userdata.dat";

    public UserDataManager(){
        this.userData = LoadData();
    }

    private UserData LoadData(){
        UserData data = new();

        if (File.Exists(saveFile)){
            string content = File.ReadAllText(saveFile);
            userData = JsonUtility.FromJson<UserData>(content);
        }
        return data;
    }

    public void SaveData(){
        string content = JsonUtility.ToJson(userData);

        File.WriteAllText(saveFile,content);
    }
}
