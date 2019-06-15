using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    public void loadSceneATC(){
        SceneManager.LoadScene("AimingTensionCreationTest");
    }

    public void loadScenePowerPickup(){
        SceneManager.LoadScene("PowerPickupTest");
    }

    public void loadSceneTPV(){
        SceneManager.LoadScene("TensionProjectileVelocityTest");
    }

    public void loadSceneMainApp(){
        SceneManager.LoadScene("MainApplication");
    }
}
