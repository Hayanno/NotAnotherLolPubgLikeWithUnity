using UnityEngine;

public class Boots : Loot {

    public Tier.Tiers tier;
    public float speed;

    public override void GetLooted() {
        switch (tier) {
            case Tier.Tiers.Common:
                playerController.SetSpeed(10.0f);
                playerController.IsBootEquiped(true);
                break;
            default:
                Debug.Log("Boots Tier Incorrect");
                break;
        }
    }
}
