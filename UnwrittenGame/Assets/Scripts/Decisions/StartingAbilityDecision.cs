using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingAbilityDecision : MonoBehaviour
{
    public GameObject player;
    public GameObject decisionMenu;

    public ParticleSystem fireJump;
    public ParticleSystem fireTrail;

    public ParticleSystem iceJump;
    public ParticleSystem iceTrail;

    public void ChooseFire()
    {
        player.GetComponent<PlayerAbilities>().abilityList.Clear();
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Fire Ball",
            "Basic attack that shoots fire at enemies",
            3.0f,
            0.0f,
            0.5f,
            5.0f,
            0.0f,
            true
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Flamethrower",
            "Short-range attack the conjures a stream of fire at enemies",
            5.0f,
            10.0f,
            2.0f,
            25.0f,
            0.01f
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Explosion",
            "Large scale area of effect attack that summons a mighty explosion to damage enemies",
            30.0f,
            0.0f,
            10.0f,
            70.0f
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Heat Wave",
            "Summons an aura around you that burns surrounding enemies",
            0.0f,
            20.0f,
            30.0f,
            50.0f,
            0.5f
        ));

        player.GetComponent<PlayerAbilities>().element = "Fire";

        player.GetComponent<PlayerController>().SetMovementPrefabsFire();

        EndDecision();
    }

    public void ChooseIce()
    {
        player.GetComponent<PlayerAbilities>().abilityList.Clear();
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Ice Blade",
            "Basic attack that swings a sword of ice at enemies",
            5.0f,
            0.35f,
            0.5f,
            5.0f,
            0.0f,
            true
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Frost Lance",
            "Long-range sniper attack that fires a sharp lance of ice at enemies",
            15.0f,
            1.0f,
            3.0f,
            25.0f,
            0.25f
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Icicle Rain",
            "Large scale area of effect attack that summons icicles to rain on your enemies",
            2.0f,
            10.0f,
            10.0f,
            70.0f,
            0.1f
        ));
        player.GetComponent<PlayerAbilities>().abilityList.Add(new Ability(
            "Arctic Wind",
            "Summons an aura around you that freezes surrounding enemies",
            0.0f,
            20.0f,
            30.0f,
            50.0f,
            0.5f
        ));

        player.GetComponent<PlayerAbilities>().element = "Ice";

        player.GetComponent<PlayerController>().SetMovementPrefabsIce();

        EndDecision();
    }

    void EndDecision()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        player.gameObject.GetComponent<PlayerController>().isInDecision = false;
        decisionMenu.SetActive(false);
        //gameObject.SetActive(false);
    }
}
