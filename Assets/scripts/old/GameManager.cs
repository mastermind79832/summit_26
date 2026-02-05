using UnityEngine;
using System.Collections;

namespace OLD
{
    public class GameManager : MonoBehaviour
    {
        private bool isGameStart;
        private bool isTimerStart;
        private bool isCooking;
        private string item;
        private float currentTime;
        //private string[] fringPan;
        //private string[] order = { "bun", "patte" , "bun"};
        [SerializeField] BunController bunController;
        [SerializeField] float bunCookingTime;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            isGameStart = false;
            isTimerStart = false;
            isCooking = false;
            //fringPan = new string[1];
        }

        // Update is called once per frame
        void Update()
        {

            if (isGameStart)
            {
                if (Input.GetButtonDown("Jump") && isCooking)
                {
                    TrashBin();
                    isCooking = false;
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    //bun cooking time is seting here
                    bunCookingTime = 10f;
                    Cooking("bun", bunCookingTime);
                }
                if (Input.GetKeyDown(KeyCode.P))

                {
                    Debug.Log("patte is cooking");
                }

            }
            else if (Input.GetButtonDown("Jump"))
            {
                isGameStart = true;
                StartGame();
            }

            //timer
            if (isTimerStart)
            {
                if (currentTime > 0)
                {
                    currentTime -= Time.deltaTime;
                    //Debug.Log(currentTime);
                }
                else
                {
                    isTimerStart = false;
                    isCooking = false;
                    Debug.Log("timer ends");
                }
            }




        }

        //starting game if the game is not strted 
        private void StartGame()
        {
            Debug.Log("game is started");
            //InvokeRepeating("CustomerCame", 1f,5f);
            //CustomerCame();
            Invoke("CustomerCame", 2f);
        }



        //if the item burns trash items to dusbin
        private void TrashBin()
        {
            Debug.Log("items move to bin");
        }

        private void CustomerCame()
        {
            Debug.Log("new coustomer came");
            Debug.Log("cuntomer odered buger");

        }

        private void Cooking(string _item, float cookingTime)
        {
            isCooking = true;
            item = _item;
            // Debug.Log(item + " is cooking");
            currentTime = cookingTime;
            //Debug.Log(currentTime);
            isTimerStart = true;
        }

    }
}