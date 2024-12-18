using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath ="";
    public string saveFileName ="";

    public bool CheckToSeeIfFileExists()
    {
       if(File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
       {
           return true;
       }
       else
       {
           return false;
       }
    }
    //delete the save file
    public void DeleteSaveFile()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    public void CreateNewCharacterSaveFile(CharacterSaveData characterData){
        // Make save file   
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try{
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Directory Created"+savePath);

            string dataToStore = JsonUtility.ToJson(characterData, true);

            using(FileStream stream = new FileStream(savePath, FileMode.Create) ){
                using (StreamWriter fileWriter = new StreamWriter(stream)){
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch(Exception ex){
            Debug.LogError("Error Saving File, Game Not Saved "+savePath+"\n"+ex);
        }
    }

    public CharacterSaveData LoadSaveFile(){
        CharacterSaveData characterData = null;

        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if(File.Exists(loadPath)){
            try{
                string dataToLoad = "";

                using(FileStream stream = new FileStream(loadPath, FileMode.Open)){

                using(StreamReader reader = new StreamReader(stream)){
                    dataToLoad = reader.ReadToEnd();
                }
            }   
            characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch(Exception ex){
            }
        }
        return characterData;
    }
}
