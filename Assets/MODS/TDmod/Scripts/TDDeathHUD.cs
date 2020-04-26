using UnityEngine;
using UnityEngine.UI;
using SpaceGeon.UI;


class TDDeathHUD : BasicHUD, iDeathHUD
{
    [SerializeField]
    private GameObject deathPointsText;

    protected GameObject DeathPointsText { get => deathPointsText; set => deathPointsText = value; }

    public void SetDeathPointsText(int count)
    {
        DeathPointsText.GetComponent<Text>().text = count.ToString();
    }

    public void SetDeathPointsText(string text)
    {
        DeathPointsText.GetComponent<Text>().text = text;
    }

    public void RebornClicked()
    {
        master.Reborn();
    }

    public void RageQuitClicked()
    {
        master.LoadMenuFromGame();
    }
}
