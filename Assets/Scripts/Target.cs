using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isHit = false;   // prevents double counting

    public void Hit()
    {
        if (isHit) return;      // ignore if already hit
        isHit = true;

        // Notify the game manager
        GameManager.instance.TargetHit();

        // Optionally hide or disable the target after being hit
        gameObject.SetActive(false);
    }
}
