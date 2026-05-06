using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int _index;
    private bool _lightsRevealed;

    [SerializeField]
    private InputActionReference interactAction;

    [SerializeField]
    private Renderer[] towerLightMeshes;

    [SerializeField]
    private Renderer onAirSignRenderer;

    [SerializeField]
    private Material onAirActiveMaterial;

    void OnEnable() => interactAction.action.Enable();

    void OnDisable() => interactAction.action.Disable();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeTowerLightsHidden();
        HideDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction.action.WasPressedThisFrame())
        {
            if (textComponent.text == lines[_index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[_index];
            }
        }
    }

    void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[_index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (_index < lines.Length - 1)
        {
            _index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            CompleteDialogue();
            HideDialogue();
        }
    }

    private void CompleteDialogue()
    {
        if (_lightsRevealed)
        {
            return;
        }

        if (towerLightMeshes == null || towerLightMeshes.Length == 0)
        {
            Debug.LogWarning(
                $"{nameof(Dialogue)} on '{name}' has no tower light mesh renderers assigned.",
                this
            );
        }
        else
        {
            for (int i = 0; i < towerLightMeshes.Length; i++)
            {
                Renderer meshRenderer = towerLightMeshes[i];
                if (meshRenderer == null)
                {
                    Debug.LogWarning(
                        $"{nameof(Dialogue)} on '{name}' has a missing tower light mesh reference at index {i}.",
                        this
                    );
                    continue;
                }

                meshRenderer.enabled = true;
            }
        }

        if (onAirSignRenderer == null)
        {
            Debug.LogWarning(
                $"{nameof(Dialogue)} on '{name}' has no on-air sign renderer assigned.",
                this
            );
        }
        else if (onAirActiveMaterial == null)
        {
            Debug.LogWarning(
                $"{nameof(Dialogue)} on '{name}' has no on-air active material assigned.",
                this
            );
        }
        else
        {
            onAirSignRenderer.material = onAirActiveMaterial;
        }

        _lightsRevealed = true;
    }

    private void InitializeTowerLightsHidden()
    {
        if (towerLightMeshes == null || towerLightMeshes.Length == 0)
        {
            Debug.LogWarning(
                $"{nameof(Dialogue)} on '{name}' has no tower light mesh renderers assigned.",
                this
            );
            return;
        }

        for (int i = 0; i < towerLightMeshes.Length; i++)
        {
            Renderer meshRenderer = towerLightMeshes[i];
            if (meshRenderer == null)
            {
                Debug.LogWarning(
                    $"{nameof(Dialogue)} on '{name}' has a missing tower light mesh reference at index {i}.",
                    this
                );
                continue;
            }

            if (!_lightsRevealed)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    public void ShowDialogue()
    {
        gameObject.SetActive(true);
        StartDialogue();
    }

    public void HideDialogue()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
        _index = 0;
    }
}
