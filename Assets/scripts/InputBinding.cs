using System;
using UnityEngine;

public class InputBinding : MonoBehaviour
{
    public KeyCode StartKey;
    public KeyCode BunKey;
    public KeyCode PattyKey;
    public KeyCode CookPressureKey;
    public KeyCode CookEndKey;
    public KeyCode PlateAddedKey;
    public KeyCode NextCustomerKey;
    public KeyCode TrashKey;

    public static InputBinding Instance;


    public Action OnStartPressed;
    public Action OnBunAdded;
    public Action OnPattyAdded;
    public Action OnCookPressurrePressed;
    public Action OnCookEndPressed;
    public Action OnPlateAdded;
    public Action OnNextCustomerPressed;
    public Action OnTrashKeyPressed;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(StartKey))
        {
            OnStartPressed?.Invoke();
        }
        if (Input.GetKeyDown(BunKey))
        {
            OnBunAdded?.Invoke();
        }
        if (Input.GetKeyDown(PattyKey))
        {
            OnPattyAdded?.Invoke();
        }
        if (Input.GetKeyDown(CookPressureKey))
        {
            OnCookPressurrePressed?.Invoke();
        }
        if (Input.GetKeyDown(CookEndKey))
        {
            OnCookEndPressed?.Invoke();
        }
        if(Input.GetKeyDown(PlateAddedKey))
        {
            OnPlateAdded?.Invoke();
        }
        if (Input.GetKeyDown(NextCustomerKey))
        {
            OnNextCustomerPressed?.Invoke();
        }
        if (Input.GetKeyDown(TrashKey))
        {
            OnTrashKeyPressed?.Invoke();
        }

    }
}
