using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public enum SystemState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    Unit playerUnit;
    Unit enemyUnit;

    public HUDScript playerHUD;
    public HUDScript enemyHUD;

    public Text StateChangeText;

    public int clickcounterHigh;
    public int clickcounterMedium;

    public GameObject mediumButton;
    public GameObject highButton;

    public bool isDefending;
    public GameObject defendBubble;

    public ParticleSystem PlayerDamage;
    public ParticleSystem EnemyDamage;
    public ParticleSystem HealEffect;
    public ParticleSystem PlayerShoot;
    public ParticleSystem EnemyShoot;

    public SystemState state;
    // Start is called before the first frame update
    void Start()
    {
        state = SystemState.START;
        enemy.SetActive(true);
        player.SetActive(true);
        clickcounterHigh = 5;
        clickcounterMedium = 8;
        StartCoroutine(BattleSetup());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("QUIIIIT");
        }
    }

    IEnumerator BattleSetup()
    {
        GameObject playerGO = Instantiate(player, playerSpawnPoint.position, Quaternion.identity);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemy, enemySpawnPoint.position, Quaternion.identity);
        enemyUnit = enemyGO.GetComponent<Unit>();

         StateChangeText.text = " An " + enemyUnit.unitName + " has appeared! ";

        playerHUD.SetUP(playerUnit);
        enemyHUD.SetUP(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = SystemState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttackLow()
    {
        // Damage the enemy
        PlayerShoot.Play();
        yield return new WaitForSeconds(1f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.lowDamage);
        EnemyDamage.Play();
        enemyHUD.SetHP(enemyUnit.currentHP);
        StateChangeText.text = "Successful Low attack";

        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead

        if (isDead)
        {
            state = SystemState.WON;
            StartCoroutine(EndGame());
        }
        else
        {
            state = SystemState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Check state based on what happeded
    }

    IEnumerator PlayerAttackMedium()
    {
        // Damage the enemy
        PlayerShoot.Play();
        yield return new WaitForSeconds(1f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.mediumDamage);

        EnemyDamage.Play();

        enemyHUD.SetHP(enemyUnit.currentHP);
        StateChangeText.text = "Successful Medium attack";

        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead

        if (isDead)
        {
            state = SystemState.WON;
            StartCoroutine(EndGame());
        }
        else
        {
            state = SystemState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Check state based on what happeded
    }

    IEnumerator PlayerAttackHigh()
    {
        // Damage the enemy
        PlayerShoot.Play();
        yield return new WaitForSeconds(1f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.highDamage);

        EnemyDamage.Play();

        enemyHUD.SetHP(enemyUnit.currentHP);
        StateChangeText.text = "Successful High attack";

        yield return new WaitForSeconds(2f);

        // Check if the enemy is dead

        if (isDead)
        {
            state = SystemState.WON;
            StartCoroutine(EndGame());
        }
        else
        {
            state = SystemState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Check state based on what happeded
    }

    IEnumerator EnemyTurn()
    {
        StateChangeText.text = enemyUnit.unitName + " attacks! ";
        EnemyShoot.Play();

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.enemydamage);
        Debug.Log(enemyUnit.enemydamage);

        PlayerDamage.Play();
        //Instantiate(PlayerDamage, playerSpawnPoint.position, Quaternion.identity);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);


        if (isDead)
        {
            state = SystemState.LOST;
            StartCoroutine(EndGame());
        }
        else
        {
            state = SystemState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EndGame()
    {
        if (state == SystemState.WON)
        {
            Debug.Log("WONNNNNN");
            StateChangeText.text = "You won!";
            enemy.SetActive(false);
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
        else if (state == SystemState.LOST)
        {
            StateChangeText.text = "You Lost!";
            player.SetActive(false);
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void PlayerTurn()
    {
        isDefending = false;
        defendBubble.SetActive(false);
        StateChangeText.text = "Choose an Action:";  
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(playerUnit.playerHeal);
        Debug.Log(playerUnit.playerHeal);
        playerHUD.SetHP(playerUnit.currentHP);
        HealEffect.Play();
        StateChangeText.text = "Ahh more Health!";

        yield return new WaitForSeconds(2f);

        state = SystemState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerDefend()
    {
        isDefending = true;
        defendBubble.SetActive(true);
        StateChangeText.text = "Nice Defence!";
        yield return new WaitForSeconds(2f);

        state = SystemState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void EnemyDmg()
    {
        if (isDefending)
        {
            playerUnit.currentHP = Mathf.RoundToInt(enemyUnit.enemydamage / 2);
            Debug.Log(enemyUnit.enemydamage);
        }
        else
        {
            playerUnit.currentHP -= enemyUnit.enemydamage;
        }
    }

    public void AttackButtonHigh()
    {
        if (state != SystemState.PLAYERTURN)
            return;

        clickcounterHigh--;
        if (clickcounterHigh <= 0)
        {
            highButton.SetActive(false);
        }

        StartCoroutine(PlayerAttackHigh());
    }

    public void AttackButtonLow()
    {
        if (state != SystemState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttackLow());
    }

    public void AttackButtonMedium()
    {
        if (state != SystemState.PLAYERTURN)
            return;

        clickcounterMedium --;
        if (clickcounterMedium <= 0)
        {
            mediumButton.SetActive(false);
        }

        StartCoroutine(PlayerAttackMedium());
    }

    public void HealButton()
    {
        if (state != SystemState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    public void DefendButton()
    {
        if (state != SystemState.PLAYERTURN)
            return;

        StartCoroutine(PlayerDefend());

        Debug.Log("Defending!!");
    }




}
