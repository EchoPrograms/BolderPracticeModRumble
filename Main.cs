
using MelonLoader;
using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace RumbleFirst { 

    public class Main : MelonMod
    {
        public static Main _instance;
        static InputActionMap actionMap = new InputActionMap("actionmap");
        static InputAction rightTrigger = InputActionSetupExtensions.AddAction(actionMap, "Right Grip", InputActionType.Button, "<XRController>{RightHand}/trigger"); float rightTriggerValue;
        static InputAction leftTrigger = InputActionSetupExtensions.AddAction(actionMap, "Left Grip", InputActionType.Button, "<XRController>{LeftHand}/trigger"); float leftTriggerValue;
        static InputAction aButton = InputActionSetupExtensions.AddAction(actionMap, "A Button", InputActionType.Button, "<XRController>{RightHand}/primaryButton"); bool aButtonValue;
        static InputAction bButton = InputActionSetupExtensions.AddAction(actionMap, "B Button", InputActionType.Button, "<XRController>{RightHand}/secondaryButton"); bool bButtonValue;
        string currentSceneName;
        bool firstBootFlag = true;
        
        public override void OnEarlyInitializeMelon()
        {
            _instance = this;
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentSceneName = sceneName;
            base.OnSceneWasLoaded(buildIndex, sceneName);

            if (!firstBootFlag) return;
            actionMap.Enable();
            LoggerInstance.Msg("Input Initiated");
            firstBootFlag = false;
        }
        Transform hand;
        Material red;
        bool leftFlag;
        bool waitingOnCooldown;
        bool rightFlag;
        GameObject spawnhint;
        float distance = 2f;
        public override void OnFixedUpdate()
        {
            if (currentSceneName != "Park") return;
            if(hand == null)
            {
                try {
                    hand = Camera.main.gameObject.transform.parent.parent.GetChild(2).GetChild(0);
                    red = GameObject.Find("________________SCENE_________________/Park/Main static group/Root/Root_base_mesh_005__2_").GetComponent<MeshRenderer>().material;
                } catch { }
            } else
            {
               
                if (bButtonValue)
                {
                    if (!rightFlag)
                    {
                        if (currentSceneName == "Park" && PhotonNetwork.IsMasterClient)
                        {
                            
                            spawnhint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            spawnhint.GetComponent<MeshRenderer>().material = red;
                            spawnhint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                        }
                        rightFlag = true;
                        
                    }
                    if(spawnhint != null)
                    {
                        spawnhint.transform.position = hand.TransformPoint(hand.localPosition + new Vector3(0, distance, 0));

                    }
                }
                else
                {
                    if (rightFlag == true && !waitingOnCooldown)
                    {
                        if (currentSceneName == "Park" && PhotonNetwork.IsMasterClient)
                        {
                            GameObject.Destroy(spawnhint);
                            PhotonNetwork.Instantiate("LargeRock", hand.TransformPoint(hand.localPosition + new Vector3(0, distance, 0)), Quaternion.identity);
                        }
                        MelonCoroutines.Start(resetSpawn());
                    }
                }
                
            }

            base.OnUpdate();

            if (firstBootFlag) return;
            rightTriggerValue = rightTrigger.ReadValue<float>();
            leftTriggerValue = leftTrigger.ReadValue<float>();
            aButtonValue = aButton.ReadValue<float>() == 1;

            bButtonValue = bButton.ReadValue<float>() == 1;

        }
        System.Collections.IEnumerator resetSpawn()
        {
            waitingOnCooldown = true;
            LoggerInstance.Msg("Cooldown started");
            yield return new WaitForSeconds(5.0f);
            LoggerInstance.Msg("Cooldown Ended");
            rightFlag = false;
            waitingOnCooldown = false;
        }
    }
}
