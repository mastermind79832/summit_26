using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool IsGameStart;
    public bool IsCustomerArrived;
    public int CustomerCount;
    public int BunCount;
    public int PattyCount;

    private void Start()
    {
        IsGameStart = false;
        IsCustomerArrived = false;
        CustomerCount = 0;
    }

    private void OnEnable()
    {
        InputBinding.Instance.OnStartPressed += StartGame;
        InputBinding.Instance.OnNextCustomerPressed += NextCustomerArrived;
    }

    private void OnDisable()
    {
        InputBinding.Instance.OnStartPressed -= StartGame;
        InputBinding.Instance.OnNextCustomerPressed -= NextCustomerArrived;
    }

    private void StartGame()
    {
        IsGameStart = true;
        InputBinding.Instance.OnStartPressed -= StartGame;
    }

    public void NextCustomerArrived()
    {
        if (IsCustomerArrived)
            return;

        IsCustomerArrived = true;
    }

    public void SendToPlate()
    {

    }

}
