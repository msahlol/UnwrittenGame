using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Ability
{
    public string name;
    public string description;
    public float damage;
    public float abilityDuration;
    public float cooldownTime;
    public float manaCost;
    public float statusEffectChance;
    public bool isOffCooldown = true;
    public bool isUnlocked = false;

    public Ability(string name, string description, float damage, float abilityDuration, float cooldownTime, float manaCost, float statusEffectChance = 0.0f, bool isUnlocked = false)
    {
        this.name = name;
        this.description = description;
        this.damage = damage;
        this.abilityDuration = abilityDuration;
        this.cooldownTime = cooldownTime;
        this.manaCost = manaCost;
        this.statusEffectChance = statusEffectChance;
        this.isUnlocked = isUnlocked;
    }
}

public class PlayerAbilities : MonoBehaviour
{
    public List<Ability> abilityList;
    public string element;

    public GameObject fireBurstPrefab;
    public GameObject fireBallPrefab;
    public GameObject flameThrowerPrefab;
    public GameObject explosionProjectilePrefab;
    public GameObject fireAuraPrefab;

    public GameObject iceBurstPrefab;
    public GameObject iceSwordPrefab;
    public GameObject iceChargePrefab;
    public GameObject iceLancePrefab;
    public GameObject icicleRainProjectilePrefab;
    public GameObject iceAuraPrefab;

    public void UseAbility(int abilityIndex)
    {
        Debug.Log(abilityIndex);
        if (abilityList.Count > abilityIndex && abilityList[abilityIndex].isOffCooldown)
        {
            if (abilityIndex == 0)
            {
                if (element.Equals("Fire"))
                {
                    GameObject projectile = Instantiate(fireBallPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    Instantiate(fireBurstPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    projectile.GetComponent<ProjectileHandler>().direction = gameObject.GetComponent<PlayerController>().gameCamera.transform.forward;
                }
                if (element.Equals("Ice"))
                {
                    GameObject iceSword = Instantiate(iceSwordPrefab, transform.position + transform.up, transform.rotation);
                    iceSword.gameObject.GetComponent<FollowPlayer>().player = gameObject;
                    StartCoroutine(CastMeleeSpell(abilityIndex, iceSword));
                }
                abilityList[abilityIndex].isOffCooldown = false;
                StartCoroutine(CooldownRoutine(abilityIndex));
            }
            if (abilityIndex == 1)
            {
                if (element.Equals("Fire"))
                {
                    GameObject flameThrower = Instantiate(flameThrowerPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    flameThrower.transform.parent = gameObject.GetComponent<PlayerController>().focalPoint.transform;
                    StartCoroutine(CastHoldableSpell(abilityIndex, flameThrower));
                }
                if (element.Equals("Ice"))
                {
                    GameObject iceCharge = Instantiate(iceChargePrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    iceCharge.transform.parent = gameObject.GetComponent<PlayerController>().focalPoint.transform;
                    StartCoroutine(CastChargeableSpell(abilityIndex, iceCharge));
                }
            }
            if (abilityIndex == 2)
            {
                if (element.Equals("Fire"))
                {
                    GameObject projectile = Instantiate(explosionProjectilePrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    Instantiate(fireBurstPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    projectile.GetComponent<ProjectileHandler>().direction = gameObject.GetComponent<PlayerController>().gameCamera.transform.forward;
                }
                if (element.Equals("Ice"))
                {
                    GameObject projectile = Instantiate(icicleRainProjectilePrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    Instantiate(iceBurstPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
                    projectile.GetComponent<ProjectileHandler>().direction = gameObject.GetComponent<PlayerController>().gameCamera.transform.forward;
                }
                abilityList[abilityIndex].isOffCooldown = false;
                StartCoroutine(CooldownRoutine(abilityIndex));
            }
            if (abilityIndex == 3)
            {
                if (element.Equals("Fire"))
                {
                    GameObject fireAura = Instantiate(fireAuraPrefab, transform.position - transform.up, Quaternion.Euler(new Vector3(90, 0, 0)));
                    fireAura.gameObject.GetComponent<FollowPlayer>().player = gameObject;
                    fireAura.gameObject.GetComponent<FollowPlayer>().offset = new Vector3(0, -1, 0);
                    StartCoroutine(CastAuraSpell(abilityIndex, fireAura));
                }
                if (element.Equals("Ice"))
                {
                    GameObject iceAura = Instantiate(iceAuraPrefab, transform.position - transform.up, Quaternion.Euler(new Vector3(90, 0, 0)));
                    iceAura.gameObject.GetComponent<FollowPlayer>().player = gameObject;
                    iceAura.gameObject.GetComponent<FollowPlayer>().offset = new Vector3(0, -1, 0);
                    StartCoroutine(CastAuraSpell(abilityIndex, iceAura));
                }
            }
        }
    }

    IEnumerator CastMeleeSpell(int abilityIndex, GameObject prefab)
    {
        float timeElapsed = 0;
        while (timeElapsed < abilityList[abilityIndex].abilityDuration)
        {
            prefab.transform.Rotate(new Vector3(0, Time.deltaTime / abilityList[abilityIndex].abilityDuration * 360, 0));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(prefab);
    }

    IEnumerator CastHoldableSpell(int abilityIndex, GameObject prefab)
    {
        float timeRemaining = abilityList[abilityIndex].abilityDuration;
        while (Input.GetMouseButton(0) && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        var main = prefab.GetComponent<ParticleSystem>().main;
        main.loop = false;
        abilityList[abilityIndex].isOffCooldown = false;
        StartCoroutine(CooldownRoutine(abilityIndex));
    }

    IEnumerator CastChargeableSpell(int abilityIndex, GameObject prefab)
    {
        float timeElapsed = 0;
        while (!Input.GetMouseButtonUp(0))
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        var main = prefab.GetComponent<ParticleSystem>().main;
        main.loop = false;
        var emberMain = prefab.transform.Find("Embers").GetComponent<ParticleSystem>().main;
        emberMain.loop = false;
        var smokeMain = prefab.transform.Find("Smoke").GetComponent<ParticleSystem>().main;
        smokeMain.loop = false;

        if (timeElapsed > abilityList[abilityIndex].abilityDuration)
        {
            GameObject projectile = Instantiate(iceLancePrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation * Quaternion.Euler(new Vector3(0, -90, 0)));
            Instantiate(iceBurstPrefab, transform.position + transform.forward + transform.up - transform.right, transform.rotation);
            projectile.GetComponent<ProjectileHandler>().direction = gameObject.GetComponent<PlayerController>().gameCamera.transform.forward;
            abilityList[abilityIndex].isOffCooldown = false;
            StartCoroutine(CooldownRoutine(abilityIndex));
        }
    }

    IEnumerator CastAuraSpell(int abilityIndex, GameObject prefab)
    {
        float timeRemaining = abilityList[abilityIndex].abilityDuration;
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        var main = prefab.GetComponent<ParticleSystem>().main;
        main.loop = false;
        var emberMain = prefab.transform.Find("Embers").GetComponent<ParticleSystem>().main;
        emberMain.loop = false;
        abilityList[abilityIndex].isOffCooldown = false;
        StartCoroutine(CooldownRoutine(abilityIndex));
    }

    IEnumerator CooldownRoutine(int abilityIndex)
    {
        gameObject.GetComponent<MenuHandler>().hud.transform.Find("Ability Select").GetComponent<TextMeshProUGUI>().color = new Color32(171, 156, 156, 255);
        yield return new WaitForSeconds(abilityList[abilityIndex].cooldownTime);
        abilityList[abilityIndex].isOffCooldown = true;
        if (gameObject.GetComponent<PlayerController>().selectedAbility == abilityIndex)
        {
            gameObject.GetComponent<MenuHandler>().hud.transform.Find("Ability Select").GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
        }
    }
}
