using UnityEngine;
using UnityEngine.Events;

public class OnPowerWrapper : MonoBehaviour
{
    [SerializeField] private PowerTile tile;

    public UnityEvent OnPower = new();
    public UnityEvent OffPower = new();



    private void Start()
    {
        tile.onPowered.AddListener(InvokePowerEvent);
    }


    private void InvokePowerEvent(bool powerBool)
    {
        if (powerBool)
        {
            OnPower.Invoke();
            return;
        }
        OffPower.Invoke();
    }
}
