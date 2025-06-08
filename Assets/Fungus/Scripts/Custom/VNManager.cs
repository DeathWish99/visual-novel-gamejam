using Fungus;
using UnityEngine;

public class VNManager : MonoBehaviour
{
    [SerializeField] private Flowchart flowchart;

    public static VNManager Instance { get; private set; }

    private bool _isAutoMode;
    public bool IsAutoMode => _isAutoMode;

    private bool _isSkipMode;
    public bool IsSkipMode => _isSkipMode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ToggleAutoMode()
    {
        _isAutoMode = !IsAutoMode;
        Debug.Log("Auto Mode: " + _isAutoMode);
    }

    public void ToggleSkipMode()
    {
        _isSkipMode = !IsSkipMode;
        Debug.Log("Skip Mode: " + _isSkipMode);
    }

    public void TurnOffAll()
    {
        _isAutoMode = false;
        _isSkipMode = false;
        Debug.Log("All modes turned off.");
    }
}
