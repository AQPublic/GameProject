using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R))
    {
        Die();
    } 
  }


    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

                TakeDamage(damage);
        }
    }


  public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log("Player took damage! HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            //Die();
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DIED!");
        currentHealth = maxHealth;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // TODO: Add respawn, game over UI, restart scene, etc.
        // For now, freeze the player:
        //GetComponent<CharacterController>().enabled = false;
    }
}
