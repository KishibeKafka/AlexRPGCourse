using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major stats")]
    public Stat strength; // 1点 damage， 1% crit.power爆伤 
    public Stat agility;  // 1% evasion闪避 ， 1% crit.chance爆率
    public Stat intelligence; // 1点法伤， x点法抗
    public Stat vitality; // 加血

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    [SerializeField] private float ailmentsDuration = 4;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;

    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    public int currentHealth;

    public System.Action onHealthChanged;
    protected bool isDead;

    protected virtual void Start()
    {
        currentHealth = GetMaxHealthValue();
        critPower.setDefaultValue(150);

        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
            isIgnited = false;
        if (chilledTimer < 0)
            isChilled = false;
        if (shockedTimer < 0)
            isShocked = false;
        if (isIgnited)
            ApplyIgniteDamage();
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCritDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.takeDamage(totalDamage);
    }

    #region Magical damage and ailments

    public virtual void DoMagicDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightningDamage = lightningDamage.GetValue();

        int totalMagicDamage = _fireDamage + _iceDamage + _lightningDamage + intelligence.GetValue();
        totalMagicDamage = CheckTargetResistance(_targetStats, totalMagicDamage);

        _targetStats.takeDamage(totalMagicDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightningDamage) <= 0)
            return;

        AttemptToApplyAilments(_targetStats, _fireDamage, _iceDamage, _lightningDamage);
    }

    private void AttemptToApplyAilments(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightningDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightningDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightningDamage;
        bool canApplyShock = _lightningDamage > _fireDamage && _lightningDamage > _iceDamage;

        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
            }
            else if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
            }
            else
                canApplyShock = true;
        }

        if (canApplyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicDamage)
    {
        totalMagicDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicDamage = Mathf.Clamp(totalMagicDamage, 0, int.MaxValue);
        return totalMagicDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
            return;

        if (_ignite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }
        if (_chill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;
            fx.ChillFxFor(ailmentsDuration);
        }
        if (_shock)
        {
            isShocked = _shock;
            shockedTimer = ailmentsDuration;
            fx.ShockFxFor(ailmentsDuration);
        }
    }

    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            igniteDamageTimer = igniteDamageCooldown;

            DecreaseHealthBy(igniteDamage);

            if (currentHealth <= 0 && !isDead)
                Die();
        }
    }

    public void SetupIgniteDamage(int _damage)
    {
        igniteDamage = _damage;
    }

    #endregion

    public virtual void takeDamage(int _damage)
    {
        DecreaseHealthBy(_damage);
        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    protected virtual void DecreaseHealthBy(int _damage)
    {
        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged();
    }

    protected virtual void Die()
    {
        isDead = true;
    }
    #region Stat calculations
    private int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    private bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (_targetStats.isShocked)
            totalEvasion -= 20;
        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Missed!");
            return true;
        }
        return false;
    }

    private bool CanCrit()
    {
        int totalCritChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < totalCritChance)
            return true;
        return false;
    }

    private int CalculateCritDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
    #endregion
}
