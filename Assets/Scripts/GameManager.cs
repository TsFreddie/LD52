using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryInfo
{
    public int count;
    public float averageCost;
}

public struct StoryResult
{
    public bool isKarma;
    public bool isReturning;
    public int prestige;
}

public class CustomerInstance
{
    public int appearanceSeed { get; private set; }
    public Customer customerBase { get; private set; }

    public int customerHealth { get; private set; }
    public int customerMaxHealth { get; private set; }
    public int customerAttack { get; private set; }
    public int customerShield { get; private set; }
    public int customerCharm { get; private set; }

    public Story baseStory { get; private set; }
    public int nextStoryDay { get; private set; }
    public int elementValue { get; private set; }
    public int priceValue { get; private set; }
    public KarmaSequence activeKarmanSequence { get; private set; }
    public MedicineElement customerWantedElement { get; private set; }

    public bool isVIP { get; private set; }

    public CustomerInstance(Customer customer, bool isVip)
    {
        customerBase = customer;
        appearanceSeed = Random.Range(int.MinValue, int.MaxValue);

        customerMaxHealth = Random.Range(10, 100);
        customerHealth = customerMaxHealth;
        customerAttack = Random.Range(10, 100);
        customerShield = Random.Range(10, 100);
        customerCharm = Random.Range(10, 100);

        baseStory = RandomStory(customer.stories);
        GenerateStoryStat();
    }

    private Story RandomStory(WeightedValue<Story>[] stories)
    {
        var totalWeight = stories.Sum(s => s.weight);
        var random = Random.Range(0, totalWeight);
        var currentWeight = 0.0f;
        foreach (var story in stories)
        {
            currentWeight += story.weight;
            if (random < currentWeight)
            {
                return story.value;
            }
        }

        return stories.Last().value;
    }

    private void GenerateStoryStat()
    {
        var storyPrice = baseStory.standardPrice;
        var minPrice = storyPrice * (customerBase.minPremium / 100f);
        var maxPrice = storyPrice * (customerBase.maxPremium / 100f);

        bool customerWantedWrongElement = Random.Range(0.0f, 100.0f) >= customerBase.elementKnowledge;

        if (customerWantedWrongElement)
        {
            var wrongElement = Random.Range(0, 3);
            while (wrongElement == (int)baseStory.storyElement)
            {
                wrongElement = Random.Range(0, 3);
            }

            customerWantedElement = (MedicineElement)wrongElement;
        }
        else
        {
            customerWantedElement = baseStory.storyElement;
        }

        priceValue = Mathf.RoundToInt(Random.Range(minPrice, maxPrice));

        if (baseStory.enableKarma)
        {
            var randomKarmaDay = Random.Range(baseStory.karmaMinDay, baseStory.karmaMaxDay) + 1;
            nextStoryDay = GameManager.Singleton.currentDay + randomKarmaDay;
        }
        else
        {
            nextStoryDay = -1;
        }

        elementValue = Random.Range(baseStory.storyValueMin, baseStory.storyValueMax) + 1;
        if (baseStory.storyElement == MedicineElement.Health && customerHealth + elementValue > customerMaxHealth)
        {
            customerMaxHealth = customerHealth + elementValue;
        }
    }

    public StoryResult TreatStory(Medicine med)
    {
        var storyResult = new StoryResult();

        var valueSuccess = false;
        var sideEffectSuccess = false;

        if (med == null)
        {
            storyResult.isReturning =
                Random.Range(0.0f, 1.0f) < (customerMaxHealth - customerHealth) / customerMaxHealth;
            storyResult.isKarma = false;
            storyResult.prestige--;
            return storyResult;
        }

        if (baseStory.storyElement == MedicineElement.Null || elementValue == 0 ||
            (med.element == baseStory.storyElement && med.elementValue >= elementValue))
        {
            valueSuccess = true;

            switch (med.element)
            {
                case MedicineElement.Health:
                    customerHealth += med.elementValue;
                    break;
                case MedicineElement.Attack:
                    customerAttack += med.elementValue;
                    break;
                case MedicineElement.Shield:
                    customerShield += med.elementValue;
                    break;
                case MedicineElement.Charm:
                    customerCharm += med.elementValue;
                    break;
            }
        }

        var storySideEffect = baseStory.successSideEffects;
        var medHasSideEffect = med.sideEffect != MedicineSideEffect.Null;
        var hasStorySideEffect = storySideEffect != null && storySideEffect.Length > 0;
        var medSideEffectHit = Random.Range(0, 100) < med.sideEffectProbability;
        if (medHasSideEffect && hasStorySideEffect && storySideEffect.Contains(med.sideEffect) && medSideEffectHit)
        {
            sideEffectSuccess = true;
        }

        if (valueSuccess) storyResult.prestige++;
        if (med.element != customerWantedElement)
        {
            storyResult.prestige--;
        }

        if (med.sideEffect != MedicineSideEffect.Null && medSideEffectHit)
        {
            storyResult.prestige--;
            storyResult.isReturning = true;
            switch (med.sideEffect)
            {
                case MedicineSideEffect.Dizzy:
                    customerHealth -= customerHealth / 3;
                    break;
                case MedicineSideEffect.Vomit:
                    customerHealth -= customerHealth / 2;
                    break;
                case MedicineSideEffect.Death:
                    customerHealth = 0;
                    break;
            }
        }
        else
        {
            storyResult.isReturning =
                Random.Range(0.0f, 1.0f) < (customerMaxHealth - customerHealth) / customerMaxHealth;
        }

        if (customerHealth <= 0)
        {
            baseStory.enableKarma = true;
            activeKarmanSequence = GameManager.Singleton.deathKarmaSequence;
        }

        if (customerHealth > 0 && baseStory.enableKarma)
        {
            storyResult.isKarma = true;
            if (valueSuccess && (!sideEffectSuccess || baseStory.valueFirst))
            {
                activeKarmanSequence = baseStory.valueKarma;
            }
            else if (sideEffectSuccess)
            {
                activeKarmanSequence = baseStory.sideEffectKarma;
            }
            else
            {
                activeKarmanSequence = baseStory.failKarma;
            }
        }

        return storyResult;
    }

    public void NextStory()
    {
        var storyPool = activeKarmanSequence == null ? customerBase.stories : activeKarmanSequence.subStories;

        baseStory = RandomStory(storyPool);
        GenerateStoryStat();
    }
}

public struct VIP
{
    public Customer customer;
    public int day;
}

public class GameManager : MonoBehaviour
{
    public enum Phase
    {
        /// <summary>
        /// 游戏未开始
        /// </summary>
        PreGame,

        /// <summary>
        /// 买入阶段
        /// </summary>
        Buy,

        /// <summary>
        /// 出售阶段
        /// </summary>
        Sell,

        /// <summary>
        /// 日结算阶段
        /// </summary>
        End,
    }

    /// <summary>
    /// Singleton instance of the GameManager
    /// </summary>
    public static GameManager Singleton { get; private set; }

    [Header("Icon setup")]
    public Sprite[] MedicineElementIcons;

    public Sprite[] MedicineSideEffectIcons;

    public int prestige { get; private set; }
    public int coins { get; private set; }

    // How many people you've killed
    public int death { get; private set; }

    public ProgressionRuleset currentRuleSet;

    private Dictionary<Medicine, InventoryInfo> _inventory;

    public Action OnStatUpdate;
    public Action OnInventoryUpdate;
    public Action OnBuyListUpdate;
    public Action OnPhaseChange;
    public Action OnCustomerUpdate;

    public int currentDay { get; private set; }

    public Phase currentPhase { get; private set; } = Phase.PreGame;

    public KarmaSequence deathKarmaSequence { get; private set; }

    /// <summary>
    /// 活跃顾客
    /// </summary>
    private List<CustomerInstance> _activeCustomers;

    /// <summary>
    /// 待激活顾客
    /// </summary>
    private List<CustomerInstance> _nextActiveCustomers;

    /// <summary>
    /// 等待报应的顾客
    /// </summary>
    private List<CustomerInstance> _karmaCustomers;

    /// <summary>
    /// VIP 客户出现日期
    /// </summary>
    public List<VIP> _vipCustomers;

    /// <summary>
    /// 活跃VIP
    /// </summary>
    private List<CustomerInstance> _activeVIPs;
    //
    // /// <summary>
    // /// 等待报应的VIP客户
    // /// </summary>
    // private List<CustomerInstance> _karmaVIP;

    /// <summary>
    /// 今天会通知的 Karma 客户
    /// </summary>
    private List<CustomerInstance> _todayKarmaCustomers;

    public CustomerInstance currentCustomer { get; private set; }

    public void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _inventory = new Dictionary<Medicine, InventoryInfo>();
        _priceTrend = new Dictionary<Medicine, Trend>();
        _previousBuyPrices = new Dictionary<Medicine, int>(); // probably useless but it's a jam so who cares
        _buyPrices = new Dictionary<Medicine, int>();
        _activeCustomers = new List<CustomerInstance>();
        _nextActiveCustomers = new List<CustomerInstance>();
        _karmaCustomers = new List<CustomerInstance>();
        _activeVIPs = new List<CustomerInstance>();
        // _karmaVIP = new List<CustomerInstance>();
        _todayKarmaCustomers = new List<CustomerInstance>();

        deathKarmaSequence = new KarmaSequence();
        deathKarmaSequence.prestige = -1;
        deathKarmaSequence.subStories = Array.Empty<WeightedValue<Story>>();
        deathKarmaSequence.ending = StoryEnding.Dead;
        deathKarmaSequence.text = "has passed away.";
    }

    public void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        currentDay = 1;
        prestige = 0;
        coins = 100;
        currentPhase = Phase.PreGame;

        OnStatUpdate?.Invoke();
        OnBuyListUpdate?.Invoke();
        OnInventoryUpdate?.Invoke();
        OnPhaseChange?.Invoke();
        OnCustomerUpdate?.Invoke();
    }

    #region ICONS

    public Sprite GetMedicineElementIcon(MedicineElement element)
    {
        return MedicineElementIcons[(int)element];
    }

    public Sprite GetMedicineSideEffectIcon(MedicineSideEffect sideEffect)
    {
        return MedicineSideEffectIcons[(int)sideEffect];
    }

    #endregion

    #region INFO

    public void GetInventoryList(List<Medicine> medicineList)
    {
        medicineList.Clear();
        medicineList.AddRange(_inventory.Keys);
    }

    public InventoryInfo GetInventoryInfo(Medicine medicine)
    {
        if (_inventory.TryGetValue(medicine, out var info))
        {
            return info;
        }

        return null;
    }

    #endregion

    #region BUY

    private enum PriceTrendMode
    {
        // Moving towards the base price
        // Price will be at around base price in three days
        Stabling,

        // Increase in increasing rate by day
        Rising,

        // Decrease in increasing rate by day
        Falling,
    }

    private struct Trend
    {
        public int startingDay;
        public int endingDay;
        public PriceTrendMode mode;

        public Trend(int startingDay, int endingDay, PriceTrendMode mode)
        {
            this.startingDay = startingDay;
            this.endingDay = endingDay;
            this.mode = mode;
        }
    }

    private Dictionary<Medicine, int> _previousBuyPrices;
    private Dictionary<Medicine, int> _buyPrices;
    private Dictionary<Medicine, Trend> _priceTrend;

    public void RestockBuyList()
    {
        void SetTrendForMedicine(Medicine med, PriceTrendMode mode)
        {
            var trendMinDays = mode switch
            {
                PriceTrendMode.Stabling => 3,
                PriceTrendMode.Rising => 3,
                PriceTrendMode.Falling => 3,
                _ => 5,
            };

            var trendMaxDays = mode switch
            {
                PriceTrendMode.Stabling => 7,
                PriceTrendMode.Rising => 7,
                PriceTrendMode.Falling => 7,
                _ => 12,
            } + 1;

            var endingDay = currentDay + Random.Range(trendMinDays, trendMaxDays);
            _priceTrend[med] = new Trend(currentDay, endingDay, mode);
        }

        int ProcessPriceTrend(Medicine med, int currentPrice, Trend trend)
        {
            var basePrice = med.cost.baseValue;
            var runningDays = currentDay - trend.startingDay;
            var totalDays = trend.endingDay - trend.startingDay;
            var maxPrice = med.cost.baseValue * (1 + med.cost.variancePercentage / 100f);
            var minPrice = med.cost.baseValue * (1 - med.cost.variancePercentage / 100f);
            var trendMode = trend.mode;
            var priceDiff = currentPrice - basePrice;

            var targetPrice = trendMode switch
            {
                PriceTrendMode.Stabling => basePrice + priceDiff * Mathf.Max(3 - runningDays, 0) / 3.0f,
                PriceTrendMode.Rising => currentPrice + (maxPrice - currentPrice) * runningDays / totalDays,
                PriceTrendMode.Falling => currentPrice + (minPrice - currentPrice) * runningDays / totalDays,
                _ => basePrice,
            };

            // Added a little bit of randomness to the price
            var randomMin = trendMode == PriceTrendMode.Rising ? 0.0f : -0.1f;
            var randomMax = trendMode == PriceTrendMode.Falling ? 0.0f : 0.1f;
            targetPrice += targetPrice * Random.Range(randomMin, randomMax);
            return Mathf.Max(1, Mathf.RoundToInt(targetPrice));
        }

        _previousBuyPrices = _buyPrices;
        _previousBuyPrices ??= new Dictionary<Medicine, int>();
        _buyPrices = new Dictionary<Medicine, int>();
        foreach (var med in currentRuleSet.availableMedicines)
        {
            var basePrice = med.cost.baseValue;
            var isNew = !_previousBuyPrices.ContainsKey(med);
            var currentPrice = isNew ? basePrice : _previousBuyPrices[med];
            var trendEnded = _priceTrend.TryGetValue(med, out var trend) && trend.endingDay < currentDay;

            if (isNew)
            {
                SetTrendForMedicine(med, PriceTrendMode.Stabling);
            }
            else if (trendEnded)
            {
                SetTrendForMedicine(med, (PriceTrendMode)Random.Range(0, 3));
            }

            var medTrend = _priceTrend[med];
            _buyPrices.Add(med, ProcessPriceTrend(med, currentPrice, medTrend));
        }

        OnBuyListUpdate?.Invoke();
    }

    public void GetBuyList(List<Medicine> medicineList)
    {
        medicineList.Clear();
        medicineList.AddRange(_buyPrices.Keys);
    }

    public int GetPrice(Medicine medicine)
    {
        if (_buyPrices.TryGetValue(medicine, out var price))
        {
            return price;
        }

        return -1;
    }

    public int GetPrevPrice(Medicine medicine)
    {
        if (_previousBuyPrices.TryGetValue(medicine, out var price))
        {
            return price;
        }

        return -1;
    }

    public void BuyMedicine(Medicine med, int price)
    {
        if (coins >= price)
        {
            coins -= price;
            if (_inventory.ContainsKey(med))
            {
                var info = _inventory[med];
                info.count++;
                info.averageCost = (info.averageCost * (info.count - 1) + price) / info.count;
            }
            else
            {
                _inventory.Add(med, new InventoryInfo()
                {
                    count = 1,
                    averageCost = price,
                });
            }

            OnStatUpdate?.Invoke();
            OnInventoryUpdate?.Invoke();
        }
    }

    public void SellBackMedicine(Medicine med, int price)
    {
        if (_inventory.ContainsKey(med) && _inventory[med].count > 0)
        {
            _inventory[med].count--;
            coins += price;

            if (_inventory[med].count == 0)
            {
                _inventory.Remove(med);
            }

            OnStatUpdate?.Invoke();
            OnInventoryUpdate?.Invoke();
        }
    }

    #endregion

    #region SELL

    /// <summary>
    /// How many customer left today
    /// </summary>
    public int customerLeft;

    public Medicine selectedMedicine;

    private CustomerInstance GenerateRandomCustomer()
    {
        var randomCustomer = Random.Range(0, currentRuleSet.availableCustomers.Length);
        var customer = currentRuleSet.availableCustomers[randomCustomer];
        var customerInstance = new CustomerInstance(customer, false);
        return customerInstance;
    }

    private void CalculateCustomerCount()
    {
        customerLeft = Mathf.Clamp(2 + prestige / 2, 0, 20);
    }

    private void NextCustomer()
    {
        if (customerLeft <= 0)
        {
            if (_activeVIPs.Count > 0)
            {
                var vip = _activeVIPs[0];
                _activeVIPs.RemoveAt(0);
                currentCustomer = vip;
                currentCustomer.NextStory();
            }
            else
            {
                currentCustomer = null;
            }

            return;
        }

        customerLeft--;

        // 每多一个活跃顾客，回头客的概率增加10%，若有10个以上活跃顾客必出回头客。
        var isCustomerNew = _activeCustomers.Count == 0 || Random.Range(0.0f, 1.0f) < _activeCustomers.Count * 0.1f;

        if (isCustomerNew)
        {
            currentCustomer = GenerateRandomCustomer();
        }
        else
        {
            var randomIndex = Random.Range(0, _activeCustomers.Count);
            currentCustomer = _activeCustomers[randomIndex];
            _activeCustomers.RemoveAt(randomIndex);
            currentCustomer.NextStory();
        }

        OnCustomerUpdate?.Invoke();
    }

    private void ActivateTodayVIP()
    {
        if (_vipCustomers.Count == 0) return;

        foreach (var vip in _vipCustomers)
        {
            if (vip.day != currentDay) continue;

            var vipInstance = new CustomerInstance(vip.customer, true);
            _activeVIPs.Add(vipInstance);
            break;
        }
    }

    private void ReAddNextDayCustomers()
    {
        _activeCustomers.AddRange(_nextActiveCustomers);
        _nextActiveCustomers.Clear();
    }

    private void ClaimTodayKarma()
    {
        foreach (var customer in _karmaCustomers)
        {
            if (currentDay >= customer.nextStoryDay)
            {
                _todayKarmaCustomers.Add(customer);
            }
        }

        _karmaCustomers.RemoveAll(c => _todayKarmaCustomers.Contains(c));
    }

    #endregion

    public void Progress()
    {
        switch (currentPhase)
        {
            case Phase.Buy:
                currentPhase = Phase.Sell;
                CalculateCustomerCount();
                NextCustomer();
                OnPhaseChange?.Invoke();
                break;
            case Phase.Sell:
                var result = currentCustomer.TreatStory(selectedMedicine);

                if (result.isKarma)
                {
                    _karmaCustomers.Add(currentCustomer);
                }
                else if (result.isReturning)
                {
                    _nextActiveCustomers.Add(currentCustomer);
                }

                prestige += result.prestige;
                if (selectedMedicine != null)
                {
                    _inventory[selectedMedicine].count--;
                    if (_inventory[selectedMedicine].count == 0)
                    {
                        _inventory.Remove(selectedMedicine);
                    }

                    coins += currentCustomer.priceValue;
                }

                OnStatUpdate?.Invoke();

                if (customerLeft > 0)
                {
                    NextCustomer();
                    OnCustomerUpdate?.Invoke();
                }
                else
                {
                    currentPhase = Phase.End;
                    OnPhaseChange?.Invoke();
                }

                break;
            case Phase.PreGame:
            case Phase.End:
                currentDay++;
                ActivateTodayVIP();
                ReAddNextDayCustomers();
                ClaimTodayKarma();
                OnStatUpdate?.Invoke();
                currentPhase = Phase.Buy;
                OnPhaseChange?.Invoke();
                RestockBuyList();
                break;
        }
    }

    public void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }
}