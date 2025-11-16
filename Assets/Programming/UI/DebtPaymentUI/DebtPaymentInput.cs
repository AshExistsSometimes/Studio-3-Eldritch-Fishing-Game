using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebtPaymentInput : MonoBehaviour
{
    [Header("References")]
    public DebtManager debtManager;
    public TMP_InputField inputField;
    public TMP_Text inputText;
    public Button confirmButton;
    public TMP_Text PlayerMoneyText;
    public PlayerController playerController;

    [Header("Colours")]
    public Color DefaultTextColour = Color.white;
    public Color CantAffordTextColour = Color.red;

    [HideInInspector]
    public int InputtedPaymentAmount = 0;
    private int ConfirmedInputtedAmount = 0;

    private void Awake()
    {
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.onValueChanged.AddListener(OnInputChanged);

        confirmButton.interactable = false;
        HideDebtWindow();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        // Clamp to debt
        InputtedPaymentAmount = Mathf.Min(InputtedPaymentAmount, debtManager.DebtRemaining);

        // Update $ display
        inputText.text = $"${InputtedPaymentAmount}";

        // Update color & confirm button
        if (EconomyManager.instance.CanPlayerAffordItem(InputtedPaymentAmount) && InputtedPaymentAmount > 0)
        {
            inputText.color = DefaultTextColour;
            confirmButton.interactable = true;
        }
        else
        {
            inputText.color = CantAffordTextColour;
            confirmButton.interactable = false;
        }

        // Update player's money
        if (PlayerMoneyText != null)
            PlayerMoneyText.text = $"${EconomyManager.instance.Currency}";

        // Close menu hotkey
        if (Input.GetKeyDown(InputManager.GetKeyCode("CloseMenu")))
        {
            HideDebtWindow();
        }

        // Enter key confirms payment
        if (Input.GetKeyDown(KeyCode.Return) && confirmButton.interactable)
        {
            ConfirmButtonPressed();
        }
    }

    private void OnInputChanged(string value)
    {
        if (!int.TryParse(value, out InputtedPaymentAmount))
            InputtedPaymentAmount = 0;

        InputtedPaymentAmount = Mathf.Min(InputtedPaymentAmount, debtManager.DebtRemaining);
    }

    public void ConfirmButtonPressed()
    {
        if (EconomyManager.instance.CanPlayerAffordItem(InputtedPaymentAmount) && InputtedPaymentAmount > 0)
        {
            ConfirmedInputtedAmount = InputtedPaymentAmount;
            debtManager.TryPayDebt(ConfirmedInputtedAmount);

            // Reset input
            InputtedPaymentAmount = 0;
            inputField.text = "";

            // Update money text immediately
            if (PlayerMoneyText != null)
                PlayerMoneyText.text = $"${EconomyManager.instance.Currency}";
        }
    }

    public void ShowDebtWindow()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);

        // Unlock cursor and enable free movement
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (playerController != null)
            playerController.canMove = false;

        // Focus input field
        inputField.ActivateInputField();
    }

    public void HideDebtWindow()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.canMove = true;
    }
}

