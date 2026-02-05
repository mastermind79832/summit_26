using UnityEngine;

namespace OLD
{
    public class Game : MonoBehaviour
    {
        private bool isGameStart;


        private void Start()
        {
            isGameStart = false;
        }


        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                isGameStart = true;
                StartGame();
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



        private void CustomerCame()
        {
            Debug.Log("new coustomer came");
            Debug.Log("cuntomer odered buger");

        }
    }
}