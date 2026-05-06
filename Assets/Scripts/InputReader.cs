using UnityEngine;

public class InputReader : MonoBehaviour
{
    // Static instance so any script can find it: InputReader.Instance
    public static InputReader Instance { get; private set; }

    private InputSystem_Actions _inputActions;

    void Awake()
    {
        // Singleton pattern: Ensure only one exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Keep input alive across scenes

        _inputActions = new InputSystem_Actions();
    }

    private void OnEnable() => _inputActions?.Enable();

    private void OnDisable() => _inputActions?.Disable();

    public InputSystem_Actions.PlayerActions PlayerActions => _inputActions.Player;
    public InputSystem_Actions.DebugActions DebugActions => _inputActions.Debug;

    public void EnablePlayerInput()
    {
        if (_inputActions == null)
            return;

        _inputActions.Debug.Disable(); // Disable debug input when enabling player input
        _inputActions.Player.Enable();
    }

    public void EnableDebugInput()
    {
        if (_inputActions == null)
            return;

        _inputActions.Player.Disable();
        _inputActions.Debug.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }
}
