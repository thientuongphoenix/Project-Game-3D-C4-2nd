using UnityEngine;

public class CheckMoney : TextAbstract
{
    public void ShowNotEnoughMoney()
    {
        this.textPro.text = "You do not have enough money to buy!";
        this.gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), 2f);
    }

    public void ShowOutOfRangeMessage()
    {
        this.textPro.text = "Cannot place tower! Too far from player!";
        this.gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), 2f);
    }

    public void ShowInvalidPositionMessage()
    {
        this.textPro.text = "Cannot place tower! Invalid position!";
        this.gameObject.SetActive(true);
        CancelInvoke();
        Invoke(nameof(Hide), 2f);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
