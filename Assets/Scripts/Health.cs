using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public RectTransform healthBar;

    public const int maxHealth = 100;

    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    [ClientRpc]
    void RpcRespawn() {
        if (isLocalPlayer)
            transform.position = Vector3.zero;
    }

	public void TakeDamage(int amount) {
        if(!isServer) {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0) {
            if (destroyOnDeath) {
                Destroy(gameObject);
            } else {
                currentHealth = maxHealth;
                
                RpcRespawn();
            }
        }

    }

    void OnChangeHealth(int currentHealth) {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }
}
